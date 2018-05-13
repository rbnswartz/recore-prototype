using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using recore.db;
using recore.db.Commands;

namespace recore.web.Controllers
{
    public class DataController : Controller
    {
        private string connectionString;
        public DataController(IConfiguration config){
            connectionString = config.GetValue<string>("recore:connectionstring");
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("data/{recordType}/")]
        public List<Record> GetAll(string recordType, List<string> columns)
        {
            IDataSource data = new Postgres(connectionString);
            DataService service = new DataService();
            service.data = data;
            if (columns.Count == 0)
            {
                RetrieveRecordTypeCommand retrieveCommand = new RetrieveRecordTypeCommand()
                {
                    RecordType = recordType,
                };

                RetrieveRecordTypeResult typeResult = (RetrieveRecordTypeResult)service.Execute(retrieveCommand);
                columns = typeResult.Type.FieldNames;
            }
            RetrieveAllCommand command = new RetrieveAllCommand()
            {
                RecordType = recordType,
                Columns = columns
            };
            RetrieveAllResult result = (RetrieveAllResult)service.Execute(command);
            return result.Result;
        }

        [HttpGet]
        [Route("data/{recordType}/{id}")]
        public Record Get(string recordType, Guid id, List<string> columns)
        {
            // because apparently comma seperation isn't enough to make a list
            if (columns.Count == 1)
            {
                columns = columns[0].Split(",").ToList();
            }
            IDataSource data = new Postgres(connectionString);
            DataService service = new DataService();
            service.data = data;
            if (columns.Count == 0)
            {
                RetrieveRecordTypeCommand retrieveCommand = new RetrieveRecordTypeCommand()
                {
                    RecordType = recordType,
                };

                RetrieveRecordTypeResult typeResult = (RetrieveRecordTypeResult)service.Execute(retrieveCommand);
                columns = typeResult.Type.FieldNames;
            }
            RetrieveRecordCommand command = new RetrieveRecordCommand()
            {
                Type = recordType,
                Fields = columns,
                Id = id,
                AllFields = false,
            };
            RetrieveRecordResult result = (RetrieveRecordResult)service.Execute(command);
            return result.Result;
        }

        [HttpPost]
        [Route("data/{recordType}/")]
        public Guid Post(string recordType, [FromBody] Record record)
        {
            IDataSource data = new Postgres(connectionString);
            DataService service = new DataService();
            service.data = data;
            record.Type = recordType;
            CreateRecordCommand command = new CreateRecordCommand()
            {
                Target = record,
            };
            CreateRecordResult result = (CreateRecordResult)service.Execute(command);
            return result.RecordId;
        }

        [HttpDelete]
        [Route("data/{recordType}/{id}")]
        public void Delete(string recordType, Guid id)
        {
            IDataSource data = new Postgres(connectionString);
            DataService service = new DataService();
            service.data = data;
            DeleteRecordCommand command = new DeleteRecordCommand()
            {
                Type = recordType,
                Id = id,
            };
            DeleteRecordResult result = (DeleteRecordResult)service.Execute(command);
        }

        [HttpPatch]
        [Route("data/{recordType}/{id}")]
        public void Update(string recordType, Guid id, [FromBody] Record record)
        {
            IDataSource data = new Postgres(connectionString);
            DataService service = new DataService();
            service.data = data;
            record.Id = id;
            record.Type = recordType;
            UpdateRecordCommand command = new UpdateRecordCommand()
            {
                Target = record
            };
            DeleteRecordResult result = (DeleteRecordResult)service.Execute(command);
        }
    }
}