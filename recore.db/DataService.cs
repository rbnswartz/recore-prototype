using System;
using System.Collections.Generic;
using System.Linq;
using recore.db.Commands;

namespace recore.db
{
    public class DataService
    {
        public IDataSource data;
        public ResultBase Execute(CommandBase command)
        {
            switch (command)
            {
                case CreateRecordCommand _:
                {
                    CreateRecordCommand createCommand = (CreateRecordCommand) command;
                    RecordType type = data.GetRecordType(createCommand.Target.Type);
                    List<string> sourceFieldNames = type.Fields.Select(t => t.Name).ToList();
                    foreach (string field in createCommand.Target.Data.Keys)
                    {
                        if (!sourceFieldNames.Contains(field))
                        {
                            throw new MissingFieldException($"Record with type {createCommand.Target.Type} doesn't have the column {field}");
                        }
                    }

                    createCommand.Target["createdon"] = DateTime.Now;
                    createCommand.Target["modifiedon"] = DateTime.Now;
                    Guid result = this.data.CreateRecord(createCommand.Target);
                    return new CreateRecordResult() { RecordId = result};
                }
                case RetrieveRecordCommand _:
                {
                    RetrieveRecordCommand retrieveCommand = (RetrieveRecordCommand) command;
                    RecordType type = data.GetRecordType(retrieveCommand.Type);
                    List<string> fieldsToGet;
                    if (retrieveCommand.AllFields)
                    {
                        fieldsToGet = type.FieldNames;
                    }
                    else
                    {
                        string badFieldName = retrieveCommand.Fields.FirstOrDefault(f => !type.FieldNames.Contains(f));
                        if (badFieldName != null)
                        {
                            throw new Exception($"Record Type {retrieveCommand.Type} doesn't contain the field {badFieldName}");
                        }

                        fieldsToGet = retrieveCommand.Fields;
                    }
                    
                    return new RetrieveRecordResult() { Result = this.data.RetrieveRecord(retrieveCommand.Type, retrieveCommand.Id, fieldsToGet)};
                }
                default:
                {
                    throw  new Exception("Unknown command");
                }
            }
        }
    }
}