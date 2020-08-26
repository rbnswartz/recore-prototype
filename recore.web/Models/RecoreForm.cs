using recore.db;
using recore.web.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace recore.web.Models
{
    public class RecoreForm
    {
        public string FormName;
        public Guid FormId;
        public string RecordType;
        public List<RecoreFormField> Fields;

        public RecoreForm()
        {
            this.Fields = new List<RecoreFormField>();
        }

        public RecoreForm(Record input)
        {
            FormId = input.Id;
            RecordType = (string)input.SafeGet("recordtype");
            FormName = (string)input.SafeGet("name");
            Fields = JsonConvert.DeserializeObject<List<RecoreFormField>>((string)input.SafeGet("fields"));
        }
    }
}
