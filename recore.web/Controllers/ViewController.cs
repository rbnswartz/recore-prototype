using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using recore.db;
using recore.db.Commands;
using recore.web.Extension;
using recore.web.Models;

namespace recore.web.Controllers
{
    public class ViewController : Controller
    {
        private string connectionString;
        public ViewController(IConfiguration config){
            connectionString = config.GetValue<string>("recore:connectionstring");
        }

        [HttpGet]
        [Route("View/{viewId}")]
        public IActionResult ShowView(Guid viewId)
        {
            DataService service = new DataService()
            {
                data = new Postgres(connectionString),
            };
            ViewData["sitemap"] = service.GetSiteMap();
            RecoreView view = service.GetView(viewId);
            ViewData["defaultForm"] = service.GetDefaultForm(view.recordType).FormId;
            ViewData["view"] = view;
            ViewData["components"] = GetComponents(service); 
            return View();
        }

        private List<string> GetComponents(DataService service)
        {
            List<string> output = new List<string>();
            RetrieveAllCommand command = new RetrieveAllCommand() { RecordType="viewcomponent", Columns = new List<string> { "definition" } };
            RetrieveAllResult result = (RetrieveAllResult)service.Execute(command);
            foreach(Record component in result.Result)
            {
                output.Add((string)component["definition"]);
            }
            return output;
        }
    }
}