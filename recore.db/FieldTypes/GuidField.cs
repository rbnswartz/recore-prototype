using System;

namespace recore.db.FieldTypes
{
    public class GuidField: IFieldType
    {
        public string Name;
        public Boolean Nullable;
        public string ToCreate()
        {
            return $"{Name} uuid {(!Nullable ? "not null" : "")}";
        }

        public string GetFieldName()
        {
            return Name;
        }

        public Type GetFieldType()
        {
            return typeof(Guid);
        }
    }
}