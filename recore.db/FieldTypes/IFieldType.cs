using System;

namespace recore.db.FieldTypes
{
    public interface IFieldType
    {
        string ToCreate();
        string Name { get; set; }
        Type FieldType { get; }
    }
}