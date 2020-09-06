using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using recore.db;
using recore.db.Commands;
using recore.web.Extension;

namespace recore.web.Controllers
{
    public class FormController : Controller
    {
        private string connectionString;
        public FormController(IConfiguration config){
            connectionString = config.GetValue<string>("recore:connectionstring");
        }

        [HttpGet]
        [Route("Form/{formId}/{recordId?}")]
        public IActionResult ShowForm(Guid formId, Guid recordId)
        {
            DataService service = new DataService(new Postgres(connectionString));
            ViewData["id"] = recordId == Guid.Empty ? "" : recordId.ToString();
            ViewData["sitemap"] = service.GetSiteMap();
            ViewData["form"] = service.GetForm(formId);
            ViewData["components"] = GetComponents(service);
            ViewData["scriptstoload"] = GetScriptsToLoad(service);
            return View();
        }

        private List<string> GetScriptsToLoad(DataService service)
        {
            List<string> output = new List<string>();
            RetrieveAllCommand command = new RetrieveAllCommand() { RecordType="formcomponent", Columns = new List<string> { "url" } };
            RetrieveAllResult result = (RetrieveAllResult)service.Execute(command);
            foreach(Record component in result.Result)
            {
                if (component.Data.ContainsKey("url"))
                {
                    output.Add((string)component["url"]);
                }
            }
            return output;
        }

        private List<string> GetComponents(DataService service)
        {
            List<string> output = new List<string>();
            RetrieveAllCommand command = new RetrieveAllCommand() { RecordType="formcomponent", Columns = new List<string> { "definition" } };
            RetrieveAllResult result = (RetrieveAllResult)service.Execute(command);
            foreach(Record component in result.Result)
            {
                if (component.Data.ContainsKey("definition"))
                {
                    output.Add((string)component["definition"]);
                }
            }
            return output;
        }
    }
}