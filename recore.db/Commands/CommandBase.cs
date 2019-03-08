using System;
using System.Collections.Generic;

namespace recore.db.Commands
{
    public abstract class CommandBase
    {
        protected CommandBase()
        {
            this.Data = new Dictionary<string, object>();
        }
        public Dictionary<string, object> Data { get; set; }
        public object this[string key]
        {
            get
            {
                if (Data.ContainsKey(key))
                {
                    return Data[key];
                }

                throw new KeyNotFoundException($"Data with key {key} doesn't exist");
            }
            set
            {
                if (Data.ContainsKey(key))
                {
                    Data[key] = value;
                }
                else
                {
                    Data.Add(key,value);
                }
            }
        }
        public object SafeGetValue(string key)
        {
            if (this.Data.ContainsKey(key))
            {
                return this.Data[key];
            }
            return null;
        }
        public void SafeSetValue(string key, object value)
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