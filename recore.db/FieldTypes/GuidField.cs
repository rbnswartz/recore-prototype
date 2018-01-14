﻿using System;

namespace recore.db.FieldTypes
{
    public class GuidField: IFieldType
    {
        public Boolean Nullable;
        
        public string ToCreate()
        {
            return $"{Name} uuid {(!Nullable ? "not null" : "")}";
        }
        
        public Type FieldType => typeof(Guid);
        public string Name { get; set; }
    }
}