using System;

namespace recore.db.FieldTypes
{
    public class BooleanField : IFieldType
    {
        public Boolean Nullable { get; set; }

        public string ToCreate()
        {
            return $"{Name} boolean {(!Nullable ? "not null" : "")}";
        }

        public Type FieldType => typeof(Boolean);
        public string Name { get; set; }
        public string Label { get; set; }

        public BooleanField(string name, bool nullable)
        {
            this.Nullable = nullable;
            this.Name = name;
        }
        public BooleanField() { }
    }
}