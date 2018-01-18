using System;
using System.Collections.Generic;
using recore.db;
using recore.db.FieldTypes;
using recore.db.Commands;

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
            RecordType newType = new RecordType("Log", "log");
            newType.Fields.Add(new TextField(){Name = "name", Length = 50, Nullable = true});
            newType.Fields.Add(new NumberField(){Name = "number", Nullable = true});
            service.Execute(new CreateRecordTypeCommand() {Target = newType});
            Record newLog = new Record()
            {
                Type = "log",
                Data = new Dictionary<string, object>()
                {
                    ["name"] = "First ever log!",
                    ["number"] = 123
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
            foreach (var type in data.GetAllTypes())
            {
                data.DeleteRecordType(type.RecordTypeId);
            }
            
            
            
            Console.WriteLine("All done");
        }
    }
}