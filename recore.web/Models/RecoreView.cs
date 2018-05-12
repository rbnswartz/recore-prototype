using recore.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using recore.web.Extension;

namespace recore.web.Models
{
    public class RecoreView
    {
        public string recordType;
        public Dictionary<string, string> Columns;
        public Guid DefaultForm;

        public RecoreView(string type)
        {
            this.recordType = type;
            this.Columns = new Dictionary<string, string>();
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
                this.Columns = JsonConvert.DeserializeObject<Dictionary<string, string>>(columns);
            }
        }
    }
}
