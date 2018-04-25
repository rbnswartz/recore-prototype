using System;

namespace recore.db.FieldTypes
{
    public class GuidField: IFieldType
    {
        public Boolean Nullable { get; set; }
        
        public string ToCreate()
        {
            return $"{Name} uuid {(!Nullable ? "not null" : "")}";
        }
        
        public Type FieldType => typeof(Guid);
        public string Name { get; set; }
        public string Label { get; set; }
    }
}