using recore.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using recore.web.Extension;

namespace recore.web.Models
{
    public class SitemapItem
    {
        public string Label;
        public string Type;
        public string Url;
        public string RecordType;

        public SitemapItem(Record input)
        {
            Label = (string)input.SafeGet("label");
            Type = (string)input.SafeGet("type");
            Url = (string)input.SafeGet("url");
            RecordType = (string)input.SafeGet("recordtype");
        }
        public SitemapItem()
        {

        }
    }
}
