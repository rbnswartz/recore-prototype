using recore.db.DataTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace recore.db.FieldTypes
{
    public class LookupField : IFieldType
    {
        public string Name { get; set; }
        public bool Nullable { get; set; }
        public string Target;
        public string Label { get; set; }

        public Type FieldType => typeof(Guid);

        public string ToCreate()
        {
            return $"{Name} uuid references {Target}({Target}id) {(!Nullable ? "not null" : "")}";
        }
    }
}
