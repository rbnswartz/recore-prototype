using System;

namespace recore.db.FieldTypes
{
    public class PrimaryField : IFieldType
    {
        public string ToCreate()
        {
            return $"{Name} uuid Not Null Primary Key";
        }
        
        public Type FieldType => typeof(Guid);
        public string Name { get; set; }
    }
}