using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using recore.db;
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
            return View();
        }
    }
}