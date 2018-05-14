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
            DataService service = new DataService()
            {
                data = new Postgres(connectionString),
            };
            ViewData["id"] = recordId == Guid.Empty ? "" : recordId.ToString();
            ViewData["sitemap"] = service.GetSiteMap();
            ViewData["form"] = service.GetForm(formId);
            RetrieveAllCommand command = new RetrieveAllCommand() { Columns = new List<string> { "definition" } };
            ViewData["components"] = GetComponents(service); 
            return View();
        }

        private List<string> GetComponents(DataService service)
        {
            List<string> output = new List<string>();
            RetrieveAllCommand command = new RetrieveAllCommand() { RecordType="formcomponent", Columns = new List<string> { "definition" } };
            RetrieveAllResult result = (RetrieveAllResult)service.Execute(command);
            foreach(Record component in result.Result)
            {
                output.Add((string)component["definition"]);
            }
            return output;
        }
    }
}