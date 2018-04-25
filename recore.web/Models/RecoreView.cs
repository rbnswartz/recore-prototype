using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace recore.web.Models
{
    public class RecoreView
    {
        public string recordType;
        public Dictionary<string, string> Columns;

        public RecoreView(string type)
        {
            this.recordType = type;
            this.Columns = new Dictionary<string, string>();
        }
    }
}
