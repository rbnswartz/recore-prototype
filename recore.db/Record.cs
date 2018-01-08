using System;
using System.Collections.Generic;

namespace recore.db
{
    public class Record
    {
        public Record()
        {
            Data = new Dictionary<string, object>();
        }
        public Guid Id;
        public string Type;
        public Dictionary<string, object> Data;
    }
}