using System;
using System.Collections.Generic;
using recore.db;
using recore.db.FieldTypes;
using recore.db.Commands;
using System.Linq;

namespace recore
{
    class Program
    {
        static void Main(string[] args)
        {
            IDataSource data = new Postgres("Host=localhost; Port=5432; User Id=reuben; Password=password; Database=recore;");
            DataService service = new DataService();
            service.data = data;
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
            logType.Fields.Add(new NumberField(){Name = "number", Nullable = true});
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
            Record newLog = new Record()
            {
                Type = "log",
                Data = new Dictionary<string, object>()
                {
                    ["name"] = "First ever log!",
                    ["number"] = 123,
                    ["customer"] = createResult.RecordId,
                }
            };
            
            CreateRecordCommand com = new CreateRecordCommand()
            {
                Target = newLog,
            };

            CreateRecordResult result = (CreateRecordResult)service.Execute(com);
            
            Console.WriteLine(result.RecordId);
            Record createdRecord = ((RetrieveRecordResult)service.Execute(new RetrieveRecordCommand(){AllFields = true, Id = result.RecordId, Type = "log"})).Result;
            foreach (var item in createdRecord.Data)
            {
                Console.WriteLine($"{item.Key}:{item.Value}");
            }
            var allTypes = data.GetAllTypes().ToList();
            allTypes.Reverse();
            foreach (var type in allTypes)
            {
                data.DeleteRecordType(type.RecordTypeId);
            }
            
            
            
            Console.WriteLine("All done");
            Console.ReadLine();
        }
    }
}