using System;
using System.Collections.Generic;
using recore.db;
using recore.db.FieldTypes;
using recore.db.Commands;
using System.Linq;
using recore.db.Query;
using WorkflowCore.Services;
using recore.workflow;
using Microsoft.Extensions.Hosting;

namespace recore
{
    class Program
    {
        static void Main(string[] args)
        {
            IDataSource data = new Postgres("Host=localhost; Port=5432; User Id=reuben; Password=password; Database=recore;");
            DataService service = new DataService(data);
            Console.WriteLine(data.CheckInitialized());
            if (!data.CheckInitialized())
            {
                data.Initialize();
            }
            RecordType CustomerType = new RecordType("Customer", "customer");
            CustomerType.Fields.Add(new TextField() { Name = "name", Length = 50});
            service.Execute(new CreateRecordTypeCommand() { Target = CustomerType });
            RecordType logType = new RecordType("Log", "log");
            logType.Fields.Add(new TextField(){Name = "name", Length = 50, Nullable = true});
            logType.Fields.Add(new NumberField("number",true));
            service.Execute(new CreateRecordTypeCommand() {Target = logType});
            Record firstCustomer = new Record()
            {
                Type = "customer",
                Data = new Dictionary<string, object>()
                {
                    ["name"] = "First customer",
                }
            };
            CreateRecordResult createResult = (CreateRecordResult)service.Execute(new CreateRecordCommand() { Target = firstCustomer });
            
            Record createdRecord = ((RetrieveRecordResult)service.Execute(new RetrieveRecordCommand(){AllFields = true, Id = createResult.RecordId, Type = "customer"})).Result;
            foreach (var item in createdRecord.Data)
            {
                Console.WriteLine($"{item.Key}:{item.Value}");
            }

            BasicQuery query = new BasicQuery()
            {
                RecordType = "customer",
                Columns = new List<string>() { "name" },
                Filters = new List<QueryFilter>()
                {
                    new QueryFilter()
                    {
                        Column = "name",
                        Operation = FilterOperation.Null
                    },
                    new QueryFilter()
                    {
                        Column = "name",
                        Operation = FilterOperation.NotNull
                    }
                },
                Orderings = new List<QueryOrdering>()
                {
                    new QueryOrdering()
                    {
                        Column = "name",
                        Descending = false,
                    }
                }
            };

            QueryRecordsCommand queryCommand = new QueryRecordsCommand() { Query = query };

            var queryResult = (QueryRecordsResult)service.Execute(queryCommand);
            Console.WriteLine(queryResult.Result.Count);

            var allTypes = data.GetAllTypes().ToList();
            allTypes.Reverse();
            foreach (var type in allTypes)
            {
                data.DeleteRecordType(type.RecordTypeId);
            }

            var persistance = new RecoreWorkflowPersistance(service);
            
            Console.WriteLine("All done");
            Console.ReadLine();
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args);
    }
}