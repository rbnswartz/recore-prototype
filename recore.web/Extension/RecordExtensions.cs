using recore.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace recore.web.Extension
{
    public static class RecordExtensions
    {
        public static object SafeGet(this Record input, string field)
        {
            if (input.Data.ContainsKey(field))
            {
                return input.Data[field];
            }
            return null;
        }
    }
}
