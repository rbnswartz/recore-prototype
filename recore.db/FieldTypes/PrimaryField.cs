using System;

namespace recore.db.FieldTypes
{
    public class PrimaryField : IFieldType
    {
        public string Name;
        public string ToCreate()
        {
            return $"{Name} uuid Not Null Primary Key";
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