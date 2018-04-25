using System;

namespace recore.db.FieldTypes
{
    public interface IFieldType
    {
        string ToCreate();
        string Name { get; set; }
        //string Label { get; set; }
        bool Nullable { get; set; }
        Type FieldType { get; }
    }
}