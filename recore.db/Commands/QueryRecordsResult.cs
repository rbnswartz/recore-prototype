using System;
using System.Collections.Generic;
using System.Text;

namespace recore.db.Commands
{
    public class QueryRecordsResult : ResultBase
    {
        public List<Record> Result
        {
            get => (List<Record>)this["Result"];
            set => this["Result"] = value;
        }
    }
}
