using System;
using System.Collections.Generic;
using recore.db;
using recore.db.FieldTypes;

namespace recore
{
    class Program
    {
        static void Main(string[] args)
        {
            IDataSource data = new Postgres("Host=localhost; Port=5432; User Id=reuben; Password=password; Database=recore;");
            Console.WriteLine(data.CheckInitialized());
            if (!data.CheckInitialized())
            {
                data.Initialize();
            }
            RecordType newType = new RecordType("Log", "log");
            newType.Fields.Add(new TextField(){Name = "name", Length = 50, Nullable = true});
            newType.Fields.Add(new NumberField(){Name = "number", Nullable = true});
            data.CreateRecordType(newType);
            Record newLog = new Record()
            {
                Type = "log",
                Data = new Dictionary<string, object>()
                {
                    ["name"] = "First ever log!",
                    ["number"] = 123
                }
            };
            Guid created = data.CreateRecord(newLog);
            Console.WriteLine(created);
            Record createdRecord = data.RetrieveRecord("log", created, new List<string>() {"createdon", "logid"});
            foreach (var item in createdRecord.Data)
            {
                Console.WriteLine($"{item.Key}:{item.Value}");
            }
            foreach (var type in data.GetAllTypes())
            {
                data.DeleteRecordType(type.RecordTypeId);
            }
            Console.WriteLine("All done");
            Console.ReadLine();
        }
    }
}