using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using recore.db;
using recore.db.FieldTypes;

namespace recore.web.Controllers
{
    public class SystemController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public bool Init()
        {
            DataService service = new DataService()
            {
                data = new Postgres("Host=localhost; Port=5432; User Id=reuben; Password=password; Database=recore;"),
            };
            if (!service.data.CheckInitialized())
            {
                service.data.Initialize();
            }

            RecordType Sitemap = new RecordType("Sitemap", "sitemap")
            {
                Fields = new List<IFieldType>()
                {
                    new TextField("label", 100, false),
                    new TextField("type", 20, false),
                    new TextField("url", 20, false),
                    new TextField("recordtype", 100, false),
                }
            };
            RecordType View = new RecordType("View", "view")
            {
                Fields = new List<IFieldType>()
                {
                    new TextField("label", 100, false),
                    new TextField("recordtype", 100, false),
                    new TextField("contents", 10000, false),
                }
            };
            return true;
        }
    }
}