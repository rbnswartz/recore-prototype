using System;
using System.Collections.Generic;
using System.Text;

namespace recore.db.Query
{
    public class BasicQuery
    {
        public BasicQuery()
        {
            Columns = new List<string>();
            Filters = new List<QueryFilter>();
            Orderings = new List<QueryOrdering>();
        }
        public string RecordType;
        public List<string> Columns;
        public List<QueryFilter> Filters;
        public List<QueryOrdering> Orderings;
    }
}
