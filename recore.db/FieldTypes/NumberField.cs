using System;

namespace recore.db.FieldTypes
{
    public class NumberField : IFieldType
    {
        public string Name;
        public int Max;
        public int Min;
        public bool Nullable;
        public string ToCreate()
        {
            return $"{Name} integer" + (!Nullable ? "NOT NULL" : "") ;
        }

        public string GetFieldName()
        {
            return Name;
        }

        public Type GetFieldType()
        {
            return typeof(int);
        }
    }
}