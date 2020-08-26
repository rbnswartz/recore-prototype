using recore.db;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using recore.web.Extension;

namespace recore.web.Models
{
    public class RecoreView
    {
        public string recordType;
        public List<RecoreFormField> Columns;
        public Guid DefaultForm;

        public RecoreView(string type)
        {
            this.recordType = type;
            this.Columns = new List<RecoreFormField>();
        }

        public RecoreView(Record record)
        {
            this.recordType = (string)record.SafeGet("recordtype");
            string columns = (string)record.SafeGet("contents");
            if (columns == null)
            {
                this.Columns = null;
            }
            else
            {
                this.Columns = JsonConvert.DeserializeObject<List<RecoreFormField>>(columns);
            }
        }
    }
}
