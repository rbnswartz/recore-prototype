using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private IDataService service;
        public HomeController(IConfiguration config, IDataService dataService){
            service = dataService;
        }
        public IActionResult Index()
        {
            ViewData["sitemap"] = service.GetSiteMap();
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
