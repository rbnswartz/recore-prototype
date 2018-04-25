using System;

namespace recore.db.FieldTypes
{
    public class TextField : IFieldType
    {
        public TextField(string name, int length, bool nullable)
        {
            this.Name = name;
            this.Length = length;
            this.Nullable = nullable;
        }
        public TextField()
        {

        }
        public int Length;
        public Boolean Nullable { get; set; }
        public string ToCreate()
        {
            return $"{Name} varchar({Length})" + (!Nullable ? "NOT NULL" : "") ;
        }
        
        public Type FieldType => typeof(string);
        public string Name { get; set; }
    }
}