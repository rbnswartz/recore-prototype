using System;
using System.Collections.Generic;
using System.Text;

namespace recore.db.Commands
{
    public class RetrieveAllRecordTypesResult : ResultBase
    {
        public List<RecordType> RecordTypes
        {
            get
            {
                if (this.Data.ContainsKey("RecordTypes"))
                {
                    return (List<RecordType>) this.Data["RecordTypes"];
                }

                return null;
            }
            set
            {
                if (this.Data.ContainsKey("RecordTypes"))
                {
                    this.Data["RecordTypes"] = value;
                }
                else
                {
                    this.Data.Add("RecordTypes", value);
                }
            }
        }
    }
}
