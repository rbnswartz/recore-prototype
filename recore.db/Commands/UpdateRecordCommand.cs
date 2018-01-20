using System;

namespace recore.db.Commands
{
    public class UpdateRecordCommand : CommandBase
    {
        public Record Target
        {
            get => (Record)this["Target"];
            set => this["Target"] = value;
        }
    }
}