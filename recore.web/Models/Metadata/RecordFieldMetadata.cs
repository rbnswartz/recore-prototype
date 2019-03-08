using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace recore.web.Models.Metadata
{
    public class RecordFieldMetadata
    {
        public string Type;
        public bool Nullable;
        public Dictionary<string, string> Metadata;
        public RecordFieldMetadata()
        {
            Metadata = new Dictionary<string, string>();
        }
    }
}
