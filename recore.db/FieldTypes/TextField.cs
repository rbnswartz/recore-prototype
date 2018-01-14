using System;

namespace recore.db.FieldTypes
{
    public class TextField : IFieldType
    {
        public int Length;
        public bool Nullable;
        public string ToCreate()
        {
            return $"{Name} varchar({Length})" + (!Nullable ? "NOT NULL" : "") ;
        }
        
        public Type FieldType => typeof(string);
        public string Name { get; set; }
    }
}