using System;

namespace recore.db.FieldTypes
{
    public interface IFieldType
    {
        string ToCreate();
        string GetFieldName();
        Type GetFieldType();
    }
}