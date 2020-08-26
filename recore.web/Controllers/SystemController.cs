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
using recore.web.Extension;
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

        public IActionResult Metadata()
        {
            DataService service = new DataService()
            {
                data = new Postgres(connectionString),
            };
            ViewData["sitemap"] = service.GetSiteMap();
            return View();
        }

        [HttpGet]
        [Route("system/init")]
        public bool Init()
        {
            //TODO: Refactor this as it is getting really messy
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
                    new TextField("recordtype", 100, true),
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
                    new TextField("definition", 10000, true),
                    new TextField("url", 200, true),
                }
            };

            RecordType ViewComponent = new RecordType("View Component", "viewcomponent")
            {
                Fields = new List<IFieldType>()
                {
                    new TextField("name", 100, false),
                    new TextField("definition", 10000, false),
                }
            };

            List<Record> forms = new List<Record>(){
                GenerateFormForRecordType(Sitemap),
                GenerateFormForRecordType(Log),
                GenerateFormForRecordType(FormComponent),
                GenerateFormForRecordType(ViewComponent),
            };

            service.Execute(new CreateRecordTypeCommand() { Target = Sitemap });
            service.Execute(new CreateRecordTypeCommand() { Target = View });
            service.Execute(new CreateRecordTypeCommand() { Target = Log });
            service.Execute(new CreateRecordTypeCommand() { Target = Form });
            service.Execute(new CreateRecordTypeCommand() { Target = FormComponent });
            service.Execute(new CreateRecordTypeCommand() { Target = ViewComponent });

            foreach(var form in forms){
                service.Execute(new CreateRecordCommand{ Target = form});
            }

            var formResult = (CreateRecordResult)service.Execute(new CreateRecordCommand{ Target = GenerateFormForRecordType(Form, new Dictionary<string, RecoreFormField>() {
                ["recordtype"] = new RecoreFormField { Name = "recordtype", Field = "recordtype", FieldType = "recordtype-field", Label="Record Type"}
            })});
            var viewResult = (CreateRecordResult)service.Execute(new CreateRecordCommand{ Target = GenerateFormForRecordType(View)});
            var sitemapResult = (CreateRecordResult)service.Execute(new CreateRecordCommand { Target = GenerateFormForRecordType(Sitemap) });


            // Create some of the base UI
            Record formView = new Record("view")
            {
                ["label"] = "All Forms",
                ["recordtype"] = "form",
                ["contents"] = JsonConvert.SerializeObject(new List<RecoreFormField>() {
                    new RecoreFormField { Field = "formid", Label = "Edit", FieldType = "form-link", Config= new Dictionary<string, string> { { "formid", formResult.RecordId.ToString() } } },
                    new RecoreFormField { Field = "formid", Label = "Form Id", FieldType = "text-field" },
                    new RecoreFormField { Field = "name", Label = "Name", FieldType = "text-field" },
                    new RecoreFormField { Field = "recordtype", Label = "Record Type", FieldType = "text-field"}}),
            };

            // A view of views. Yeah the name is a little wonky
            Record viewView = new Record("view")
            {
                ["label"] = "All Views",
                ["recordtype"] = "view",
                ["contents"] = JsonConvert.SerializeObject(new List<RecoreFormField>() {
                    new RecoreFormField { Field = "viewid", Label = "Edit", FieldType = "form-link", Config= new Dictionary<string, string> { { "formid", viewResult.RecordId.ToString() } } },
                    new RecoreFormField { Field = "viewid", Label = "View Id", FieldType = "text-field" },
                    new RecoreFormField { Field = "label", Label = "Label", FieldType = "text-field" },
                    new RecoreFormField { Field = "recordtype", Label = "Record Type", FieldType = "text-field" }}),
            };

            // Sitemap view
            Record siteMapView = new Record("view")
            {
                ["label"] = "Sitemap",
                ["recordtype"] = "sitemap",
                ["contents"] = JsonConvert.SerializeObject(new List<RecoreFormField>() {
                    new RecoreFormField { Field = "sitemapid", Label = "Edit", FieldType = "form-link", Config= new Dictionary<string, string> { { "formid", sitemapResult.RecordId.ToString() } } },
                    new RecoreFormField { Field = "sitemapid", Label = "Sitemap Id", FieldType = "text-field" },
                    new RecoreFormField { Field = "label", Label = "Label", FieldType = "text-field" },
                    new RecoreFormField { Field = "type", Label = "Type", FieldType = "text-field" },
                    new RecoreFormField { Field = "url", Label = "Url", FieldType = "text-field" },
                    new RecoreFormField { Field = "recordtype", Label = "Record Type", FieldType = "text-field" }}),
            };

            Guid formViewId = ((CreateRecordResult)service.Execute(new CreateRecordCommand { Target = formView })).RecordId;
            Guid viewViewId = ((CreateRecordResult)service.Execute(new CreateRecordCommand { Target = viewView })).RecordId;
            Guid sitemapViewId = ((CreateRecordResult)service.Execute(new CreateRecordCommand { Target = siteMapView })).RecordId;

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

            Record sitemapViewSitemap = new Record("sitemap")
            {
                ["label"] = "Sitemap",
                ["type"] = "view",
                ["url"] = sitemapViewId.ToString(),
                ["recordtype"] = "sitemap",
            };

            Record metadataSitemap = new Record("sitemap")
            {
                ["label"] = "Metadata Editor",
                ["type"] = "url",
                ["url"] = "/System/Metadata",
            };

            service.Execute(new CreateRecordCommand { Target = formViewSitemap });
            service.Execute(new CreateRecordCommand { Target = viewViewSitemap });
            service.Execute(new CreateRecordCommand { Target = sitemapViewSitemap });
            service.Execute(new CreateRecordCommand { Target = metadataSitemap });

            CreateFormComponents(service);
            CreateViewComponents(service);

            return true;
        }
        void CreateCompleteRecordType(DataService service, RecordType type, List<RecoreFormField> viewFields, bool addToSitemap, bool addEditButton)
        {
            //Incomplete don't use. I'll be coming back to this later
            var form = GenerateFormForRecordType(type);
            var createResult = (CreateRecordTypeResult)service.Execute(new CreateRecordTypeCommand() { Target = type });
            var formResult = (CreateRecordResult)service.Execute(new CreateRecordCommand { Target = form });
            viewFields.Add(new RecoreFormField { Field = type.TableName + "id", FieldType = "form-link", Label = "Edit", Config = new Dictionary<string, string>() { { "formid", formResult.RecordId.ToString() } } });

            Record view = new Record("view")
            {
                ["label"] = "Form for " + type.TableName,
                ["recordtype"] = type.TableName,
                ["contents"] = JsonConvert.SerializeObject(viewFields),
            };
        }
        void CreateViewComponents(DataService service)
        {
            Record textField = new Record("viewcomponent")
            {
                ["name"] = "text-field",
                ["definition"] = @"
                Vue.component('text-field', {
                    props: ['label', 'name', 'value', 'config'],
                    template: '<div>{{value}}</div>'
                });
                "
            };
            Record formLink = new Record("viewcomponent")
            {
                ["name"] = "form-link",
                ["definition"] = @"
                Vue.component('form-link', {
                    props: ['label', 'name', 'value', 'config'],
                    data: function () {
                        return {
                            url: '/Form/' + this.config.formid + '/' + this.value,
                        };
                    },
                    template: '<a v-bind:href=""url"">Open</a>'
                });
                "
            };
            service.Execute(new CreateRecordCommand() { Target = textField });
            service.Execute(new CreateRecordCommand() { Target = formLink });
        }
        void CreateFormComponents(DataService service)
        {
            Record textField = new Record("formcomponent")
            {
                ["name"] = "text-field",
                ["definition"] = @"
                Vue.component('text-field', {
                    props: ['label', 'name', 'value', 'config'],
                    template: '<div class=""form-group""><label v-bind:for=""name"">{{label}}</label><input class=""form-control"" type =""text"" v-bind:id=""name"" v-bind:value=""value"" v-on:input=""$emit(\'recorechange\', $event.target.value)""/></div>'
                });
                "
            };
            Record numberField = new Record("formcomponent")
            {
                ["name"] = "number-field",
                ["definition"] = @"
                Vue.component('number-field', {
                    props: ['label', 'name', 'value', 'config'],
                    template: '<div class=""form-group""><label v-bind:for=""name"">{{label}}</label><input class=""form-control"" type =""number"" v-bind:id=""name"" v-bind:value=""value"" v-on:input=""$emit(\'recorechange\', $event.target.value)""/></div>'
                });
                "
            };
            Record booleanField = new Record("formcomponent")
            {
                ["name"] = "boolean-field",
                ["definition"] = @"
                Vue.component('boolean-field', {
                    props: ['label', 'name', 'value', 'config'],
                    template: '<div class=""form-group""><label v-bind:for=""name"">{{label}}</label><input class=""form-control"" type =""checkbox"" v-bind:id=""name"" v-bind:checked=""value"" v-on:input=""$emit(\'recorechange\', $event.target.checked)""/></div>'
                });
                "
            };
            Record textareaField = new Record("formcomponent"){
                ["name"] = "textarea-field",
                ["definition"] = @"
                Vue.component('textarea-field', {
                    props: ['label', 'name', 'value', 'config'],
                    template: '<div class=""form-group""><label v-bind:for=""name"">{{label}}</label><textarea class=""form-control"" v-bind:id=""name"" v-bind:value=""value"" v-on:input=""$emit(\'recorechange\', $event.target.value)""></textarea></div>'
                });
                ",
            };

            Record recordTypeLookupField = new Record("formcomponent"){
                ["name"] = "recordtype-field",
                ["definition"] = @"
    Vue.component('recordtype-field', {
        props: ['label', 'name', 'value', 'config'],
        data: function(){
            return {
                recordTypes:[]
            };
        },
        mounted: function(){
            var self = this;
            fetch('/metadata/recordtype')
            .then(function(stream) { 
                return stream.json()
            })
            .then(function(response) {
                console.log(response);
                response.forEach(i => {
                    self.recordTypes.push(i.name);
                });
            });
        },
        template: `<div class=""form-group"">
            <label v-bind:for=""name"">{{label}}</label>
            <select class=""form-control"" type =""checkbox"" v-bind:id=""name"" v-bind:checked=""value"" v-on:input=""$emit(\'recorechange\', $event.target.value)"">
                <option v-for=""type in recordTypes"" v-bind:value=""type"">{{type}}</option>
            </select>
            </div>`
    });",
            };
            Record testField = new Record("formcomponent") {
                ["name"] = "test-field",
                ["url"] = "/js/base-form-components.js",
            };

            service.Execute(new CreateRecordCommand() { Target = textField });
            service.Execute(new CreateRecordCommand() { Target = numberField });
            service.Execute(new CreateRecordCommand() { Target = booleanField });
            service.Execute(new CreateRecordCommand() { Target = textareaField });
            service.Execute(new CreateRecordCommand() { Target = recordTypeLookupField });
            service.Execute(new CreateRecordCommand() { Target = testField });
        }
        Record GenerateFormForRecordType(RecordType type, Dictionary<string, RecoreFormField> overrides = null)
        {
            string[] ignoredFields = {type.TableName + "Id", "modifiedon", "createdon"};
            Record output = new Record(){Type = "form"};
            output["name"] = "Main";
            output["recordtype"] = type.TableName;
            List<RecoreFormField> fields = new List<RecoreFormField>();
            foreach(var field in type.Fields)
            {
                if (ignoredFields.Contains(field.Name))
                {
                    continue;
                }
                if (overrides != null && overrides.ContainsKey(field.Name))
                {
                    fields.Add(overrides[field.Name]);
                    continue;
                }
                fields.Add(new RecoreFormField {
                    Name = field.Name,
                    Field = field.Name,
                    Label = field.Name,
                    FieldType = FigureOutFormFieldType(field),
                });
            }
            output["fields"] = JsonConvert.SerializeObject(fields);
            output["defaultform"] = true;
            return output;
        }

        string FigureOutFormFieldType(IFieldType field)
        {
            switch (field)
            {
                case NumberField _:
                    return "number-field";
                case BooleanField _:
                    return "boolean-field";
                default:
                    return "text-field";
            }
        }
    }
}