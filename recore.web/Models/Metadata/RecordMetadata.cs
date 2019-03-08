using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace recore.web.Models.Metadata
{
    public class RecordMetadata
    {
        public string Name;
        public Dictionary<string, RecordFieldMetadata> Fields;

        public RecordMetadata()
        {
            Fields = new Dictionary<string, RecordFieldMetadata>();
        }
    }
}
