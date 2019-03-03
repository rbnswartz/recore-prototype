using recore.db.FieldTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace recore.db.Commands
{
    public class AddFieldToRecordTypeCommand: CommandBase
    {
        public string RecordType 
        {
            get
            {
                if (this.Data.ContainsKey("RecordType"))
                {
                    return (string) this.Data["RecordType"];
                }

                return null;
            }
            set
            {
                if (this.Data.ContainsKey("RecordType"))
                {
                    this.Data["RecordType"] = value;
                }
                else
                {
                    this.Data.Add("RecordType", value);
                }
            }
        }
        public IFieldType Field 
        {
            get
            {
                if (this.Data.ContainsKey("Field"))
                {
                    return (IFieldType) this.Data["Field"];
                }

                return null;
            }
            set
            {
                if (this.Data.ContainsKey("Field"))
                {
                    this.Data["Field"] = value;
                }
                else
                {
                    this.Data.Add("Field", value);
                }
            }
        }
    }
}
