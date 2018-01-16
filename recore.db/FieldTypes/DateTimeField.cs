using System;

namespace recore.db.FieldTypes
{
    public class DateTimeField : IFieldType
    {
        public Boolean Nullable { get; set; }
        public bool IncludeTimeZone;
        public string ToCreate()
        {
            return $"{Name} timestamp {(IncludeTimeZone ? "with time zone " : " ")}{(!Nullable ? "NOT NULL" : "")}" ;
        }

        public string GetFieldName()
        {
            return Name;
        }
        
        public Type FieldType => typeof(DateTime);
        public string Name { get; set; }
    }
}