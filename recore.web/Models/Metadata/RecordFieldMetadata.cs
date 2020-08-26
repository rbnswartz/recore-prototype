using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace recore.web.Models.Metadata
{
    public class RecordFieldMetadata
    {
        public string Type { get; set; }
        public bool Nullable { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
        public RecordFieldMetadata()
        {
            Metadata = new Dictionary<string, string>();
        }
    }
}
