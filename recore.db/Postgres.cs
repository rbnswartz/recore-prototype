using System;
using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using recore.db.FieldTypes;
using Newtonsoft.Json;

namespace recore.db
{
    public class Postgres : IDataSource
    {
        private NpgsqlConnection connection;

        public Postgres(string connectionString)
        {
            this.connection = new NpgsqlConnection(connectionString);
            this.connection.Open();
        }
        public bool CheckInitialized()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from information_schema.tables", this.connection);
            var reader = command.ExecuteReader();
            List<string> tables = new List<string>();
            while (reader.Read())
            {
                tables.Add((string)reader["table_name"]);
            }

            reader.Close();
            return tables.Contains("config") && tables.Contains("recordtype");
        }

        public void Initialize()
        {
            string createConfigText = @"create table Config (
                version integer not null);";
            string createRecordTypeText = @" create table RecordType
            (
            RecordTypeId uuid not null primary key,
            Name varchar(100) not null,
            TableName varchar(100) not null,
            Columns text not null
            )";
            NpgsqlCommand command = new NpgsqlCommand(createConfigText, this.connection);
            command.ExecuteNonQuery();
            command = new NpgsqlCommand(createRecordTypeText, this.connection);
            command.ExecuteNonQuery();
        }

        public List<RecordType> GetAllTypes()
        {
            List<RecordType> output = new List<RecordType>();
            NpgsqlCommand getTypes = new NpgsqlCommand("select RecordTypeId, Name, TableName, Columns from recordtype", this.connection);
            var reader = getTypes.ExecuteReader();
            while (reader.Read())
            {
                output.Add(this.ConvertDataRowToRecordTypes(reader));
            }
            reader.Close();
            return output;
        }

        public RecordType GetRecordType(Guid typeId)
        {
            NpgsqlCommand command = new NpgsqlCommand("select RecordTypeId, Name, TableName, Columns from recordtype where RecordTypeId = @id", this.connection);
            command.Parameters.Add(new NpgsqlParameter("id", DbType.Guid) {Value = typeId});
            var reader = command.ExecuteReader();
            reader.Read();
            RecordType type = this.ConvertDataRowToRecordTypes(reader);
            reader.Close();
            return type;
        }
        
        public RecordType GetRecordType(string typeName)
        {
            NpgsqlCommand command = new NpgsqlCommand("select RecordTypeId, Name, TableName, Columns from recordtype where TableName = @name", this.connection);
            command.Parameters.Add(new NpgsqlParameter("name", DbType.String) {Value = typeName});
            var reader = command.ExecuteReader();
            if (!reader.HasRows)
            {
                return null;
            }
            reader.Read();
            RecordType type = this.ConvertDataRowToRecordTypes(reader);
            reader.Close();
            return type;
        }

        private RecordType ConvertDataRowToRecordTypes(NpgsqlDataReader reader)
        {
            return new RecordType((string) reader["Name"], (string) reader["TableName"], (Guid) reader["RecordTypeId"])
            {
                Fields = JsonConvert.DeserializeObject<List<IFieldType>>((string)reader["Columns"], new JsonSerializerSettings(){TypeNameHandling = TypeNameHandling.Objects})
            };
        }

        public void CreateRecordType(RecordType type)
        {
            // Unsafe but left that way from prototype means
            NpgsqlParameter RecordTypeId = new NpgsqlParameter("RecordTypeId", DbType.Guid) {Value = type.RecordTypeId};
            NpgsqlParameter Name = new NpgsqlParameter("Name", DbType.String) {Value = type.Name};
            NpgsqlParameter TableName = new NpgsqlParameter("TableName", DbType.String) {Value = type.TableName};
            NpgsqlParameter Columns = new NpgsqlParameter("Columns", DbType.String) {Value = JsonConvert.SerializeObject(type.Fields, new JsonSerializerSettings() {TypeNameHandling = TypeNameHandling.Objects})};
            NpgsqlCommand insertTypeCommand = new NpgsqlCommand($"insert into recordtype (RecordTypeId, Name, TableName, Columns) values (@RecordTypeId,@Name,@TableName,@Columns)", this.connection);
            insertTypeCommand.Parameters.Add(RecordTypeId);
            insertTypeCommand.Parameters.Add(Name);
            insertTypeCommand.Parameters.Add(TableName);
            insertTypeCommand.Parameters.Add(Columns);
            insertTypeCommand.ExecuteNonQuery();
            NpgsqlCommand createTableCommand = new NpgsqlCommand($"Create table {type.TableName} ({JoinFieldDefinitions(type.Fields)});", this.connection);
            createTableCommand.ExecuteNonQuery();
        }

        public void DeleteRecordType(Guid typeId)
        {
            RecordType type = GetRecordType(typeId);
            NpgsqlCommand dropTable = new NpgsqlCommand($"drop table {type.TableName}", this.connection);
            NpgsqlCommand deleteCommand = new NpgsqlCommand("delete from recordtype where RecordTypeId = @id", this.connection);
            deleteCommand.Parameters.Add(new NpgsqlParameter("id", DbType.Guid) {Value = typeId});
            dropTable.ExecuteNonQuery();
            deleteCommand.ExecuteNonQuery();
        }

        public Guid CreateRecord(Record record)
        {
            Guid createdGuid = Guid.NewGuid();
            record.Data.Add($"{record.Type}Id", createdGuid);
            string insertSql =
                $"insert into {record.Type} ({string.Join(",", record.Data.Keys)}) values ({string.Join(",", record.Data.Keys.Select(k => $"@{k}"))})";
            NpgsqlCommand insertCommand =
                new NpgsqlCommand(insertSql, this.connection);
            foreach (var item in record.Data)
            {
                insertCommand.Parameters.Add(this.CreateParameter(item.Key, item.Value));
            }
            insertCommand.ExecuteNonQuery();
            return createdGuid;
        }

        public Record RetrieveRecord(string typeName, Guid id, List<string> columns)
        {
            RecordType type = this.GetRecordType(typeName);
            columns = EnsureIdColumn(columns, typeName);
            // Insecure. Needs to sanatize column names
            string selectSQL =
                $"select {(string.Join(",", columns))} from {typeName} where {typeName}Id = @id";
            NpgsqlCommand selectCommand = new NpgsqlCommand(selectSQL, this.connection);
            selectCommand.Parameters.Add(CreateParameter("id", id));
            using (var reader = selectCommand.ExecuteReader())
            {
                if (!reader.HasRows)
                {
                    return null;
                }
                reader.Read();
                return ReadToRecord(reader, columns, typeName);
            }
        }

        private List<string> EnsureIdColumn(List<string> input, string typeName)
        {
            if (!input.Contains(typeName))
            {
                input.Add(typeName + "id");
            }
            return input;
        }

        private Record ReadToRecord(NpgsqlDataReader reader, List<string> columns, string typeName)
        {
            Record output = new Record();
            foreach (string field in columns)
            {
                if (field == typeName + "id")
                {
                    output.Id = (Guid)reader[field];
                }
                output.Data.Add(field, reader[field]);
            }
            return output;
        }

        // To be replaced later with actual filters
        public List<Record> RetrieveAllRecords(string typeName, List<string> columns)
        {
            RecordType type = this.GetRecordType(typeName);
            columns = EnsureIdColumn(columns, typeName);
            List<Record> output = new List<Record>();
            // Insecure. Needs to sanatize column names
            string selectSQL =
                $"select {(string.Join(",", columns))} from {typeName}";
            NpgsqlCommand selectCommand = new NpgsqlCommand(selectSQL, this.connection);
            var reader = selectCommand.ExecuteReader();
            if (!reader.HasRows)
            {
                return null;
            }
            while (reader.Read())
            {
                output.Add(ReadToRecord(reader, columns, typeName));
            }
            reader.Close();
            return output;
        }

        private string JoinFieldDefinitions(List<IFieldType> fields)
        {
            List<string> fieldStrings = new List<string>();
            foreach (var field in fields)
            {
                fieldStrings.Add(field.ToCreate());
            }

            return string.Join(",", fieldStrings);
        }

        private NpgsqlParameter CreateParameter(string field, object input)
        {
            DbType type = new DbType();
            switch (input)
            {
                case int _:
                    type = DbType.Int16;
                    break;
                case long _:
                    type = DbType.Int64;
                    break;
                case string _:
                    type = DbType.String;
                    break;
                case Guid _:
                    type = DbType.Guid;
                    break;
                case DateTime _:
                    type = DbType.DateTime;
                    break;
            }
            NpgsqlParameter parameter = new NpgsqlParameter(field, type);
            parameter.Value = input;
            return parameter;
        }

        public void DeleteRecord(string recordType, Guid id)
        {
            string deleteSQL =
                $"delete from {recordType} where {recordType}id=@id";
            NpgsqlCommand selectCommand = new NpgsqlCommand(deleteSQL, this.connection);
            selectCommand.Parameters.Add(CreateParameter("id", id));
            selectCommand.ExecuteNonQuery();
        }

        public void UpdateRecord(Record record)
        {
            List<string> updates = record.Data.Keys.Select(k => $"{k}=@{k}").ToList();
            string insertSql =
                $"update {record.Type} {string.Join(",", updates)}";
            NpgsqlCommand updateCommand =
                new NpgsqlCommand(insertSql, this.connection);
            foreach (var item in record.Data)
            {
                updateCommand.Parameters.Add(this.CreateParameter(item.Key, item.Value));
            }
            updateCommand.ExecuteNonQuery();
        }
    }
}