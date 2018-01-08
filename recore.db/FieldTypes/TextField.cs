using System;

namespace recore.db.FieldTypes
{
    public class TextField : IFieldType
    {
        public string Name;
        public int Length;
        public bool Nullable;
        public string ToCreate()
        {
            return $"{Name} varchar({Length})" + (!Nullable ? "NOT NULL" : "") ;
        }

        public string GetFieldName()
        {
            return Name;
        }

        public Type GetFieldType()
        {
            return typeof(string);
        }
    }
}