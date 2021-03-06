﻿using System;
using System.Collections.Generic;

namespace recore.db
{
    public class Record
    {
        public Record()
        {
            Data = new Dictionary<string, object>();
        }
        public Record(string recordType)
        {
            Data = new Dictionary<string, object>();
            Type = recordType;
        }
        public Guid Id { get; set; }
        public string Type { get; set; }
        public Dictionary<string, object> Data { get; set; }

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
            }
        }
    }
}