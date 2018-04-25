using System;
using System.Collections.Generic;
using System.Text;

namespace recore.db.Commands
{
    public class RetrieveRecordTypeResult : ResultBase
    {
        public RecordType Type
        {
            get
            {
                if (this.Data.ContainsKey("Type"))
                {
                    return (RecordType) this.Data["Type"];
                }

                return null;
            }
            set
            {
                if (this.Data.ContainsKey("Type"))
                {
                    this.Data["Type"] = value;
                }
                else
                {
                    this.Data.Add("Type", value);
                }
            }
        }
    }
}
