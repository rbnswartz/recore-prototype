using System;
using System.Collections.Generic;
using System.Text;

namespace recore.db.Query
{
    public class QueryFilter
    {
        public string Column;
        public FilterOperation Operation;
        public object Value;
    }

    public enum FilterOperation
    {
        Equal,
        NotEqual,
        GreaterThan,
        LessThan,
        Null,
        NotNull
    }
}
