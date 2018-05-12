using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using recore.db;
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
            return View();
        }
    }
}