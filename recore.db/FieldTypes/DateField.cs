using System;

namespace recore.db.FieldTypes
{
    public class DateField : IFieldType
    {
        public string Name;
        public bool Nullable;
        public string ToCreate()
        {
            return $"{Name} date" + (!Nullable ? "NOT NULL" : "") ;
        }

        public string GetFieldName()
        {
            return Name;
        }

        public Type GetFieldType()
        {
            return typeof(DateTime);
        }
    }
}