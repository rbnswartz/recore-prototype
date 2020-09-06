using System;
using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using recore.db.FieldTypes;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using recore.db.Query;

namespace recore.db
{
    public class Postgres : IDataSource
    {
        private NpgsqlConnection connection;

        public Postgres(string connectionString)
        {
            this.connection = new NpgsqlConnection(connectionString);
        }
        public void Open(){
            this.connection.Open();
        }
        public void Close(){
            this.connection.Close();
        }
        public bool CheckInitialized()
        {
            this.Open();
            NpgsqlCommand command = new NpgsqlCommand("select * from information_schema.tables", this.connection);
            var reader = command.ExecuteReader();
            List<string> tables = new List<string>();
            while (reader.Read())
            {
                tables.Add((string)reader["table_name"]);
            }

            reader.Close();
            this.Close();
            return tables.Contains("config") && tables.Contains("recordtype");
        }

        public void Initialize()
        {
            this.Open();
            string createConfigText = @"create table Config (
                version integer not null);";
            string createRecordTypeText = @" create table RecordType
            (
            TableName varchar(100) not null primary key,
            Name varchar(100) not null,
            RecordTypeId uuid not null,
            Columns text not null
            )";
            NpgsqlCommand command = new NpgsqlCommand(createConfigText, this.connection);
            command.ExecuteNonQuery();
            command = new NpgsqlCommand(createRecordTypeText, this.connection);
            command.ExecuteNonQuery();
            this.Close();
        }

        public List<RecordType> GetAllTypes()
        {
            List<RecordType> output = new List<RecordType>();
            NpgsqlCommand getTypes = new NpgsqlCommand("select RecordTypeId, Name, TableName, Columns from recordtype", this.connection);
            this.Open();
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
            if(!IsSafe(typeName))
            {
                throw new Exception($"Record Type: {typeName} is invalid");
            }
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
            CheckRecordTypeIsSafe(type);
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
            CheckRecordIsSafe(record);
            record.Data.Add($"{record.Type}Id", record.Id);
            string insertSql =
                $"insert into {record.Type} ({string.Join(",", record.Data.Keys)}) values ({string.Join(",", record.Data.Keys.Select(k => $"@{k}"))})";
            NpgsqlCommand insertCommand =
                new NpgsqlCommand(insertSql, this.connection);
            foreach (var item in record.Data)
            {
                insertCommand.Parameters.Add(this.CreateParameter(item.Key, item.Value));
            }
            insertCommand.ExecuteNonQuery();
            return record.Id;
        }

        public Record RetrieveRecord(string typeName, Guid id, List<string> columns)
        {
            if (!IsSafe(typeName))
            {
                throw new Exception($"Record type: {typeName} is invalid");
            }

            foreach(var column in columns)
            {
                if(!IsSafe(column))
                {
                    throw new Exception($"Column: {column} is invalid");
                }
            }

            RecordType type = this.GetRecordType(typeName);


            columns = EnsureIdColumn(columns, typeName);
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
            if (!input.Contains(typeName + "id"))
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
                if (reader[field] is DBNull)
                {
                    continue;
                }
                output.Data.Add(field, reader[field]);
            }
            return output;
        }

        // To be replaced later with actual filters
        public List<Record> RetrieveAllRecords(string typeName, List<string> columns)
        {
            if (!IsSafe(typeName))
            {
                throw new Exception($"Record type {typeName} is invalid");
            }
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
                fieldStrings.Add(GenerateFieldCreateSql(field));
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
                case bool _:
                    type = DbType.Boolean;
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
            if (!IsSafe(recordType))
            {
                throw new Exception($"Record type {recordType} is invalid");
            }
            string deleteSQL =
                $"delete from {recordType} where {recordType}id=@id";
            NpgsqlCommand selectCommand = new NpgsqlCommand(deleteSQL, this.connection);
            selectCommand.Parameters.Add(CreateParameter("id", id));
            selectCommand.ExecuteNonQuery();
        }

        public void UpdateRecord(Record record)
        {
            CheckRecordIsSafe(record);
            List<string> updates = record.Data.Keys.Select(k => $"{k}=@{k}").ToList();
            string insertSql =
                $"update {record.Type} set {string.Join(",", updates)} where {record.Type}id = @id";
            NpgsqlCommand updateCommand =
                new NpgsqlCommand(insertSql, this.connection);
            updateCommand.Parameters.Add(CreateParameter("id", record.Id));
            foreach (var item in record.Data)
            {
                updateCommand.Parameters.Add(this.CreateParameter(item.Key, item.Value));
            }
            updateCommand.ExecuteNonQuery();
        }

        public List<RecordType> RetrieveAllRecordTypes()
        {
            List<RecordType> output = new List<RecordType>();
            string select = "select * from recordtype";
            NpgsqlCommand command = new NpgsqlCommand(select, this.connection);
            NpgsqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                output.Add(this.ConvertDataRowToRecordTypes(reader));
            }
            return output;
        }

        public RecordType RetrieveRecordType(string typeName)
        {
            string select = "select * from recordtype where tablename = @typeName";
            NpgsqlCommand command = new NpgsqlCommand(select, this.connection);
            command.Parameters.Add(CreateParameter("typeName", typeName));
            NpgsqlDataReader reader = command.ExecuteReader();
            reader.Read();
            var recordType = this.ConvertDataRowToRecordTypes(reader);
            reader.Close();
            return recordType;
        }

        public void AddFieldToRecordType(string typeName, IFieldType field)
        {
            if (!IsSafe(field.Name))
            {
                throw new InvalidOperationException($"Field name {field.Name} is invalid");
            }
            // Possible the manipulation of the record type value should be moved up to the data service
            var recordType = this.RetrieveRecordType(typeName);
            if (recordType.Fields.Exists(f => f.Name == field.Name))
            {
                throw new DuplicateNameException($"Record type {recordType} already has column {field.Name}");
            }
            recordType.Fields.Add(field);
            string alterSql = $"alter table {typeName} add column {field.ToCreate()}";
            NpgsqlCommand alterCommand = new NpgsqlCommand(alterSql, this.connection);
            alterCommand.ExecuteNonQuery();
            NpgsqlCommand updateRecordTypeCommand = new NpgsqlCommand($"update recordtype set Columns = @columns where recordtypeid = @recordtypeid", this.connection);
            updateRecordTypeCommand.Parameters.Add(CreateParameter("columns", JsonConvert.SerializeObject(recordType.Fields, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Objects })));
            updateRecordTypeCommand.Parameters.Add(CreateParameter("recordtypeid", recordType.RecordTypeId));
            updateRecordTypeCommand.ExecuteNonQuery();
        }

        public void RemoveFieldFromRecordType(string typeName, string fieldName)
        {
            if (!IsSafe(fieldName))
            {
                throw new InvalidOperationException($"Field name {fieldName} is invalid");
            }
            var recordType = this.RetrieveRecordType(typeName);
            if (!recordType.Fields.Exists(f => f.Name == fieldName))
            {
                throw new MissingFieldException($"Record type {recordType.TableName} doesn't have the column {fieldName}");
            }
            recordType.Fields.Remove(recordType.Fields.First(f => f.Name == fieldName));

            string alterSql = $"alter table {typeName} drop column {fieldName}";
            NpgsqlCommand alterCommand = new NpgsqlCommand(alterSql, this.connection);
            alterCommand.ExecuteNonQuery();
            NpgsqlCommand updateRecordTypeCommand = new NpgsqlCommand($"update recordtype set Columns = @columns where recordtypeid = @recordtypeid", this.connection);
            updateRecordTypeCommand.Parameters.Add(CreateParameter("columns", JsonConvert.SerializeObject(recordType.Fields, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Objects })));
            updateRecordTypeCommand.Parameters.Add(CreateParameter("recordtypeid", recordType.RecordTypeId));
            updateRecordTypeCommand.ExecuteNonQuery();
        }

        private bool IsSafe(string input){
            Regex safeRegex = new Regex("^[a-zA-Z0-9_]*$");
            return safeRegex.IsMatch(input);
        }

        private void CheckRecordIsSafe(Record input)
        {
            if(!IsSafe(input.Type))
            {
                throw new Exception($"Record type: {input.Type} is invalid");
            }
            foreach(var field in input.Data)
            {
                if(!IsSafe(field.Key))
                {
                    throw new Exception($"Field Name: {field.Key} is invalid");
                }
            }
        }
        private void CheckRecordTypeIsSafe(RecordType input)
        {
            if(!IsSafe(input.TableName))
            {
                throw new Exception($"Record type: {input.TableName} is invalid");
            }
            foreach(var field in input.Fields)
            {
                if(!IsSafe(field.Name))
                {
                    throw new Exception($"Field Name: {field.Name} is invalid");
                }
            }
        }
        private string GenerateFieldCreateSql(IFieldType field)
        {
            // There is probably a better way to do this but this is how things are currently
            switch (field)
            {
                case BooleanField _:
                    return $"{field.Name} boolean{(!field.Nullable ? " not null" : "")}";
                case NumberField _:
                    return $"{field.Name} integer" + (!field.Nullable ? " NOT NULL" : "") ;
                case PrimaryField _:
                    return $"{field.Name} uuid Not Null Primary Key";
                case TextField textField:
                    return $"{textField.Name} varchar({textField.Length})" + (!textField.Nullable ? " NOT NULL" : "") ;
                case LookupField lookupField:
                    return $"{field.Name} uuid references {lookupField.Target}({lookupField.Target}id) {(!field.Nullable ? " not null" : "")}";
                case DateField dateField:
                    return $"{dateField.Name} date" + (!dateField.Nullable ? " NOT NULL" : "") ;
                case DateTimeField dateTimeField:
                    return $"{dateTimeField.Name} timestamp {(dateTimeField.IncludeTimeZone ? "with time zone " : " ")}{(!dateTimeField.Nullable ? " NOT NULL" : "")}" ;
                case GuidField _:
                    return $"{field.Name} uuid{(!field.Nullable ? " not null" : "")}";
                default:
                    throw new NotSupportedException($"Field type {field.GetType().Name} is unsupported by this backend");
            }
        }

        private NpgsqlCommand CreateCommandForQuery(BasicQuery query)
        {
            NpgsqlCommand command = new NpgsqlCommand()
            {
                Connection = this.connection
            };
            StringBuilder queryText = new StringBuilder();
            queryText.Append("select ");
            queryText.Append(string.Join(",", query.Columns));
            queryText.Append($" from {query.RecordType}");
            if (query.Filters.Count != 0)
            {
                queryText.Append(" where ");
                for (int i = 0; i < query.Filters.Count; i++)
                {
                    var filter = query.Filters[i];
                    switch (filter.Operation)
                    {
                        case FilterOperation.Equal:
                            queryText.Append($"{filter.Column} = @{i}");
                            break;
                        case FilterOperation.GreaterThan:
                            queryText.Append($"{filter.Column} > @{i}");
                            break;
                        case FilterOperation.LessThan:
                            queryText.Append($"{filter.Column} < @{i}");
                            break;
                        case FilterOperation.NotEqual:
                            queryText.Append($"{filter.Column} <> @{i}");
                            break;
                        case FilterOperation.Null:
                            queryText.Append($"{filter.Column} is NULL");
                            break;
                        case FilterOperation.NotNull:
                            queryText.Append($"{filter.Column} is not NULL");
                            break;
                        default:
                            throw new NotSupportedException($"Operator {filter.Operation} is not supported");
                    }

                    if (!(filter.Operation == FilterOperation.NotNull || filter.Operation == FilterOperation.Null))
                    {
                        command.Parameters.Add(CreateParameter(i.ToString(), filter.Value));
                    }

                    // TODO: This is hardcoded to and. Should be changed in the future
                    if (i != query.Filters.Count - 1)
                    {
                        queryText.Append(" and ");
                    }
                }
            }
            if(query.Orderings.Count != 0)
            {
                var convertedOrders = query.Orderings.Select(i => $"{i.Column} {(i.Descending ? "desc" : "asc")}");
                queryText.Append($" order by {(string.Join(",", convertedOrders))}");
            }
            Console.WriteLine("Executing query " + queryText.ToString());
            command.CommandText = queryText.ToString();
            return command;
        }

        public List<Record> Query(BasicQuery query)
        {
            if (!IsSafe(query.RecordType))
            {
                throw new NotSupportedException($"Query target name {query.RecordType} is unsafe");
            }

            foreach(var column in query.Columns)
            {
                if (!IsSafe(column))
                {
                    throw new NotSupportedException($"Query column name {column} is unsafe");
                }
            }

            foreach(var filter in query.Filters)
            {
                if (!IsSafe(filter.Column))
                {
                    throw new NotSupportedException($"Query filter column name {filter} is unsafe");
                }
            }

            
            foreach(var order in query.Orderings)
            {
                if (!IsSafe(order.Column))
                {
                    throw new NotSupportedException($"Ordering column name {order.Column} is unsafe");
                }
            }

            List<Record> output = new List<Record>();
            NpgsqlCommand command = CreateCommandForQuery(query);
            NpgsqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                output.Add(this.ReadToRecord(reader, EnsureIdColumn(query.Columns, query.RecordType), query.RecordType));
            }
            return output;
        }
    }
}