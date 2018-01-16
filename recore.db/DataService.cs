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
                default:
                {
                    throw  new Exception("Unknown command");
                }
            }
        }
    }
}