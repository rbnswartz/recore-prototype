using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using recore.db.FieldTypes;

namespace recore.db
{
    public class RecordType
    {
        public RecordType(string name, string table, Guid typeId)
        {
            this.Init(name, table, typeId);
        }

        public RecordType(string name, string table)
        {
            this.Init(name, table, Guid.NewGuid());
        }

        private void Init(string name, string table, Guid typeId)
        {
            this.Name = name;
            this.TableName = table;
            this.RecordTypeId = typeId;
            this.Fields = new List<IFieldType>();
        }
        
        public string Name;
        public Guid RecordTypeId;
        public string TableName;
        public List<IFieldType> Fields;

        public List<string> FieldNames => Fields.Select(f => f.Name).ToList();
    }
}