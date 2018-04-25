using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using recore.db;
using recore.db.Commands;
using recore.db.FieldTypes;

namespace recore.web.Controllers
{
    public class SystemController : Controller
    {
        private string connectionString;
        public SystemController(IConfiguration config){
            connectionString = config.GetValue<string>("recore:connectionstring");
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("system/init")]
        public bool Init()
        {
            DataService service = new DataService()
            {
                data = new Postgres(connectionString),
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
            RecordType Log = new RecordType("Log", "log")
            {
                Fields = new List<IFieldType>()
                {
                    new TextField("text", 100, false),
                }
            };
            service.Execute(new CreateRecordTypeCommand() { Target = Sitemap });
            service.Execute(new CreateRecordTypeCommand() { Target = View });
            service.Execute(new CreateRecordTypeCommand() { Target = Log });
            return true;
        }
    }
}