using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace recore.web.Models.Metadata
{
    public class RecordMetadata
    {
        public string Name { get; set; }
        public Dictionary<string, RecordFieldMetadata> Fields { get; set; }

        public RecordMetadata()
        {
            Fields = new Dictionary<string, RecordFieldMetadata>();
        }
    }
}
