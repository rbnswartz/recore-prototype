using System;
using System.Collections.Generic;
using System.Text;

namespace recore.db.Commands
{
    public class DeleteRecordTypeCommand : CommandBase
    {
        public string RecordType
        {
            get => (string)SafeGetValue("RecordType");
            set => SafeSetValue("RecordType", value);
        }
    }
}
