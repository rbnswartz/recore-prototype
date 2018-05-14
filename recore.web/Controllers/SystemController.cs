using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using recore.db;
using recore.db.Commands;
using recore.db.FieldTypes;
using recore.web.Models;

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
                    new TextField("url", 100, false),
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
            RecordType Form = new RecordType("Form", "form")
            {
                Fields = new List<IFieldType>()
                {
                    new TextField("name", 100, false),
                    new TextField("recordtype", 100, false),
                    new TextField("fields", 10000, false),
                    new BooleanField("defaultform", false),
                }
            };
            RecordType Log = new RecordType("Log", "log")
            {
                Fields = new List<IFieldType>()
                {
                    new TextField("text", 100, false),
                }
            };
            RecordType FormComponent = new RecordType("Form Component", "formcomponent")
            {
                Fields = new List<IFieldType>()
                {
                    new TextField("name", 100, false),
                    new TextField("definition", 10000, false),
                }
            };
            List<Record> forms = new List<Record>(){
                GenerateFormForRecordType(Form),
                GenerateFormForRecordType(View),
                GenerateFormForRecordType(Sitemap),
                GenerateFormForRecordType(Log),
                GenerateFormForRecordType(FormComponent),
            };

            service.Execute(new CreateRecordTypeCommand() { Target = Sitemap });
            service.Execute(new CreateRecordTypeCommand() { Target = View });
            service.Execute(new CreateRecordTypeCommand() { Target = Log });
            service.Execute(new CreateRecordTypeCommand() { Target = Form });
            service.Execute(new CreateRecordTypeCommand() { Target = FormComponent });

            foreach(var form in forms){
                service.Execute(new CreateRecordCommand{ Target = form});
            }

            // Create some of the base UI
            Record formView = new Record("view")
            {
                ["label"] = "All Forms",
                ["recordtype"] = "form",
                ["contents"] = JsonConvert.SerializeObject(new Dictionary<string, string>() { { "formid", "formid" }, { "name", "name" }, { "recordtype", "recordtype" }}),
            };

            // A view of views. Yeah the name is a little wonky
            Record viewView = new Record("view")
            {
                ["label"] = "All Views",
                ["recordtype"] = "view",
                ["contents"] = JsonConvert.SerializeObject(new Dictionary<string, string>() { { "viewid", "viewid" }, { "label", "label" }, { "recordtype", "recordtype" } }),
            };
            Guid formViewId = ((CreateRecordResult)service.Execute(new CreateRecordCommand { Target = formView })).RecordId;
            Guid viewViewId = ((CreateRecordResult)service.Execute(new CreateRecordCommand { Target = viewView })).RecordId;

            Record formViewSitemap = new Record("sitemap")
            {
                ["label"] = "All Forms",
                ["type"] = "view",
                ["url"] = formViewId.ToString(),
                ["recordtype"] = "form",
            };

            Record viewViewSitemap = new Record("sitemap")
            {
                ["label"] = "All Views",
                ["type"] = "view",
                ["url"] = viewViewId.ToString(),
                ["recordtype"] = "view",
            };

            service.Execute(new CreateRecordCommand { Target = formViewSitemap });
            service.Execute(new CreateRecordCommand { Target = viewViewSitemap });

            CreateFormComponents(service);

            return true;
        }
        void CreateFormComponents(DataService service)
        {
            Record textField = new Record("formcomponent")
            {
                ["name"] = "text-field",
                ["definition"] = @"
                Vue.component('text-field', {
                    props: ['label', 'name', 'fielddata'],
                    template: '<div class=""form-group""><label v-bind:for=""name"">{{label}}</label><input class=""form-control"" type =""text"" v-bind:id=""name"" v-bind:value=""fielddata"" v-on:input=""$emit(\'recorechange\', $event.target.value)""/></div>'
                });
                "
            };
            Record numberField = new Record("formcomponent")
            {
                ["name"] = "number-field",
                ["definition"] = @"
                Vue.component('number-field', {
                    props: ['label', 'name', 'fielddata'],
                    template: '<div class=""form-group""><label v-bind:for=""name"">{{label}}</label><input class=""form-control"" type =""number"" v-bind:id=""name"" v-bind:value=""fielddata"" v-on:input=""$emit(\'recorechange\', $event.target.value)""/></div>'
                });
                "
            };
            Record booleanField = new Record("formcomponent")
            {
                ["name"] = "boolean-field",
                ["definition"] = @"
                Vue.component('boolean-field', {
                    props: ['label', 'name', 'fielddata'],
                    template: '<div class=""form-group""><label v-bind:for=""name"">{{label}}</label><input class=""form-control"" type =""checkbox"" v-bind:id=""name"" v-bind:checked=""fielddata"" v-on:input=""$emit(\'recorechange\', $event.target.checked)""/></div>'
                });
                "
            };
            service.Execute(new CreateRecordCommand() { Target = textField });
            service.Execute(new CreateRecordCommand() { Target = numberField });
            service.Execute(new CreateRecordCommand() { Target = booleanField });
        }
        Record GenerateFormForRecordType(RecordType type)
        {
            Record output = new Record(){Type = "form"};
            output["name"] = "Main";
            output["recordtype"] = type.TableName;
            List<RecoreFormField> fields = new List<RecoreFormField>();
            foreach(var field in type.Fields)
            {
                fields.Add(new RecoreFormField {
                    Name = field.Name,
                    Field = field.Name,
                    Label = field.Name,
                    FieldType = "text-field"
                });
            }
            output["fields"] = JsonConvert.SerializeObject(fields);
            output["defaultform"] = true;
            return output;
        }
        [HttpGet]
        [Route("system/recordtype/{entityName}/fields")]
        public List<IFieldType> GetFields(string entityName)
        {
            DataService service = new DataService()
            {
                data = new Postgres(connectionString),
            };
            RetrieveRecordTypeCommand command = new RetrieveRecordTypeCommand()
            {
                RecordType = entityName,
            };
            var result = (RetrieveRecordTypeResult)service.Execute(command);
            return result.Type.Fields;
        }
        [HttpGet]
        [Route("system/recordtype/")]
        public List<RecordType> GetRecordTypes(string entityName)
        {
            DataService service = new DataService()
            {
                data = new Postgres(connectionString),
            };
            var command = new RetrieveAllRecordTypesCommand();
            var result = (RetrieveAllRecordTypesResult)service.Execute(command);
            return result.RecordTypes;
        }
    }
}