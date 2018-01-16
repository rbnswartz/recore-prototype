using System;
using System.Collections.Generic;

namespace recore.db.Commands
{
    public class RetrieveRecordCommand: CommandBase
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
        public List<string> Fields
        {
            get => (List<string>)this["Fields"];
            set => this["Fields"] = value;
        }
        public bool AllFields
        {
            get => (bool)this["AllFields"];
            set => this["AllFields"] = value;
        }
    }
}