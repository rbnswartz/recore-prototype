using System;
using System.Collections.Generic;
using System.Linq;
using recore.db.Commands;
using recore.db.FieldTypes;

namespace recore.db
{
    public class DataService
    {
        public IDataSource data;
        public ResultBase Execute(CommandBase command)
        {
            data.Open();
            ResultBase output = null;
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
                    if (createCommand.Target.Id == Guid.Empty)
                    {
                        createCommand.Target.Id = Guid.NewGuid();
                    }
                    createCommand.Target["createdon"] = DateTime.Now;
                    createCommand.Target["modifiedon"] = DateTime.Now;
                    Guid result = this.data.CreateRecord(createCommand.Target);
                    output =  new CreateRecordResult() { RecordId = result};
                    break;
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
                    
                    output =  new RetrieveRecordResult() { Result = this.data.RetrieveRecord(retrieveCommand.Type, retrieveCommand.Id, fieldsToGet)};
                    break;
                }
                case CreateRecordTypeCommand _:
                {
                    CreateRecordTypeCommand createCommand = (CreateRecordTypeCommand) command;
                            
                    createCommand.Target.Fields.Add(new PrimaryField() {Name = createCommand.Target.TableName + "Id"});
                    createCommand.Target.Fields.Add(new DateTimeField() {Name = "createdon", Nullable = false});
                    createCommand.Target.Fields.Add(new DateTimeField() {Name = "modifiedon", Nullable = false});
                    this.data.CreateRecordType(createCommand.Target);
                    output =  new CreateRecordTypeResult();
                    break;
                }
                case RetrieveAllCommand _:
                {
                    RetrieveAllCommand retreiveCommand = (RetrieveAllCommand) command;
                        if (retreiveCommand.Columns == null)
                        {
                            retreiveCommand.Columns = new List<string>();
                        }
                    var result = this.data.RetrieveAllRecords(retreiveCommand.RecordType, retreiveCommand.Columns);
                    if (result == null)
                        {
                            result = new List<Record>();
                        }
                    output =  new RetrieveAllResult() { Result = result };
                    break;
                }
                case RetrieveRecordTypeCommand _:
                {
                    RetrieveRecordTypeCommand retrievecommand = (RetrieveRecordTypeCommand)command;
                    var result = this.data.GetRecordType(retrievecommand.RecordType);
                    output = new RetrieveRecordTypeResult() { Type = result };
                    break;
                }
                case DeleteRecordCommand _:
                {
                    DeleteRecordCommand deleteCommand = (DeleteRecordCommand)command;
                    this.data.DeleteRecord(deleteCommand.Type, deleteCommand.Id);
                    output =  new DeleteRecordResult();
                    break;
                }
                case UpdateRecordCommand _:
                {
                    UpdateRecordCommand updateCommand = (UpdateRecordCommand)command;
                    this.data.UpdateRecord(updateCommand.Target);
                    output = new UpdateRecordResult();
                    break;
                }
                case RetrieveAllRecordTypesCommand _:
                {
                    var allTypes = this.data.RetrieveAllRecordTypes();
                    output =  new RetrieveAllRecordTypesResult() { RecordTypes = allTypes };
                    break;
                }
                default:
                {
                    throw  new Exception("Unknown command");
                }
            }
            data.Close();
            return output;
        }
    }
}