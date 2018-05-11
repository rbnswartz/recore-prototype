using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace recore.web.Models
{
    public class RecoreFormField
    {
        public string Label;
        public string Field;
        public string FieldType;
        public string Name;
        public Dictionary<string, string> Config;
    }
}
