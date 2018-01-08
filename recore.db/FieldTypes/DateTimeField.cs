using System;

namespace recore.db.FieldTypes
{
    public class DateTimeField : IFieldType
    {
        public string Name;
        public bool Nullable;
        public bool IncludeTimeZone;
        public string ToCreate()
        {
            return $"{Name} timestamp {(IncludeTimeZone ? "with time zone " : " ")}{(!Nullable ? "NOT NULL" : "")}" ;
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