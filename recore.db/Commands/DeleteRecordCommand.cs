using System;
using System.Collections.Generic;

namespace recore.db.Commands
{
    public class DeleteRecordCommand : CommandBase
    {
        public string Type
        {
            get => (string)this["Type"];
            set => this["Type"] = value;
        }
        public Guid Id
        {
            get => (Guid)this["Id"];
            set => this["Id"] = value;
        }
    }
}