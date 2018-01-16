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

        public object this[string key]
        {
            get
            {
                if (this.Data.ContainsKey(key))
                {
                    return this.Data[key];
                }

                throw new KeyNotFoundException($"Field {key} doesn't exist in this record");
            }
            set
            {
                if (this.Data.ContainsKey(key))
                {
                    this.Data[key] = value;
                }
                else
                {
                    this.Data.Add(key, value);
                }

                throw new KeyNotFoundException($"Field {key} doesn't exist in this record");
            }
        }
    }
}