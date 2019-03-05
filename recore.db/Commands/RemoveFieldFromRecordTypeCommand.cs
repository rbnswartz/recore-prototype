using System;
using System.Collections.Generic;
using System.Text;

namespace recore.db.Commands
{
    public class RemoveFieldFromRecordTypeCommand : CommandBase
    {
        public string RecordType
        {
            get => (string)SafeGetValue("TypeName");
            set => SafeSetValue("TypeName", value);
        }
        public string FieldName
        {
            get => (string)SafeGetValue("FieldName");
            set => SafeSetValue("FieldName", value);
        }
    }
}
