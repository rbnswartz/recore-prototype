using System;
using System.Collections.Generic;
using System.Text;

namespace recore.db.Commands
{
    public class AddFieldToRecordTypeResult: ResultBase
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
        public string FieldName
        {
            get
            {
                if (this.Data.ContainsKey("FieldName"))
                {
                    return (string) this.Data["FieldName"];
                }

                return null;
            }
            set
            {
                if (this.Data.ContainsKey("FieldName"))
                {
                    this.Data["FieldName"] = value;
                }
                else
                {
                    this.Data.Add("Field", value);
                }
            }
        }
    }
}
