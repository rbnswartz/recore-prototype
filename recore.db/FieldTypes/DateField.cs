using System;

namespace recore.db.FieldTypes
{
    public class DateField : IFieldType
    {
        public bool Nullable;
        
        public string ToCreate()
        {
            return $"{Name} date" + (!Nullable ? "NOT NULL" : "") ;
        }
        
        public Type FieldType => typeof(DateTime);
        public string Name { get; set; }
    }
}