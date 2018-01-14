using System;

namespace recore.db.FieldTypes
{
    public class NumberField : IFieldType
    {
        public int Max;
        public int Min;
        public bool Nullable;
        public string ToCreate()
        {
            return $"{Name} integer" + (!Nullable ? "NOT NULL" : "") ;
        }
        
        public Type FieldType => typeof(int);
        public string Name { get; set; }
    }
}