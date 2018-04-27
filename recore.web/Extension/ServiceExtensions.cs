using recore.db;
using recore.db.Commands;
using recore.web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace recore.web.Extension
{
    public static class ServiceExtensions
    {
        public static List<SitemapItem> GetSiteMap(this DataService service)
        {
            List<SitemapItem> output = new List<SitemapItem>();
            RetrieveAllCommand getAll = new RetrieveAllCommand()
            {
                RecordType = "sitemap",
                Columns = new List<string>() { "url", "label", "recordtype", "type"},
            };
            RetrieveAllResult result = (RetrieveAllResult)service.Execute(getAll);
            foreach(var i in result.Result)
            {
                output.Add(new SitemapItem(i));
            }
            return output;
        }
    }
}
