using recore.db.Query;
using System;
using System.Collections.Generic;
using System.Text;

namespace recore.db.Commands
{
    public class QueryRecordsCommand: CommandBase
    {
        public BasicQuery Query
        {
            get => (BasicQuery)this["Query"];
            set => this["Query"] = value;
        }

    }
}
