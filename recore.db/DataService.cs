using System;
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
                    Guid result = this.data.CreateRecord(((CreateRecordCommand) command).Target);
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