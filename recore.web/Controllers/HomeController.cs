using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using recore.db;
using recore.web.Extension;
using recore.web.Models;

namespace recore.web.Controllers
{
    public class HomeController : Controller
    {
        private string connectionString;
        public HomeController(IConfiguration config){
            connectionString = config.GetValue<string>("recore:connectionstring");
        }
        public IActionResult Index()
        {
            DataService service = new DataService()
            {
                data = new Postgres(connectionString),
            };
            ViewData["sitemap"] = service.GetSiteMap();
            ViewData["view"] = new RecoreView("sitemap"){
                Columns = new Dictionary<string,string>() {
                    { "label", "Label"},
                    { "url", "URL"},
                }
            };
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
