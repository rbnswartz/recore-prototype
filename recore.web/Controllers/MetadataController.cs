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
using recore.web.Models.Metadata;

namespace recore.web.Controllers
{
    public class MetadataController : Controller
    {
        private string connectionString;
        public MetadataController(IConfiguration config){
            connectionString = config.GetValue<string>("recore:connectionstring");
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("metadata/recordtype/{entityName}/fields")]
        public Dictionary<string,RecordFieldMetadata> GetFields(string entityName)
        {
            DataService service = new DataService(new Postgres(connectionString));
            RetrieveRecordTypeCommand command = new RetrieveRecordTypeCommand()
            {
                RecordType = entityName,
            };
            var result = (RetrieveRecordTypeResult)service.Execute(command);
            Dictionary<string, RecordFieldMetadata> output = new Dictionary<string, RecordFieldMetadata>();
            foreach(var i in result.Type.Fields)
            {
                output.Add(i.Name, SerializeFieldMetadata(i));
            }
            return output;
        }

        [HttpPost]
        [Route("metadata/recordtype/{entityName}/fields/{fieldName}")]
        public bool AddFieldToRecordType(string entityName,string fieldName, [FromBody]RecordFieldMetadata field)
        {
            DataService service = new DataService(new Postgres(connectionString));

            IFieldType createdField = DeserializeFieldMetadata(field);
            createdField.Name = fieldName;

            AddFieldToRecordTypeCommand command = new AddFieldToRecordTypeCommand()
            {
                RecordType = entityName,
                Field = createdField,
            };

            service.Execute(command);
            return true;
        }

        [HttpDelete]
        [Route("metadata/recordtype/{entityName}/fields/{fieldName}")]
        public bool RemoveFieldFromRecordType(string entityName,string fieldName)
        {
            DataService service = new DataService(new Postgres(connectionString));

            RemoveFieldFromRecordTypeCommand command = new RemoveFieldFromRecordTypeCommand()
            {
                RecordType = entityName,
                FieldName = fieldName,
            };

            service.Execute(command);
            return true;
        }

        [HttpDelete]
        [Route("metadata/recordtype/{entityName}")]
        public bool DeleteRecordType(string entityName)
        {
            DataService service = new DataService(new Postgres(connectionString));


            var command = new DeleteRecordTypeCommand()
            {
                RecordType = entityName,
            };

            service.Execute(command);
            return true;
        }

        [HttpPost]
        [Route("metadata/recordtype/{entityName}")]
        public bool CreateRecordType(string entityName, [FromBody] RecordMetadata recordType)
        {
            DataService service = new DataService(new Postgres(connectionString));

            RecordType createdType = new RecordType(recordType.Name, recordType.Name);
            foreach(var field in recordType.Fields)
            {
                IFieldType tmp = DeserializeFieldMetadata(field.Value);
                tmp.Name = field.Key;
                createdType.Fields.Add(tmp);
            }

            var command = new CreateRecordTypeCommand()
            {
                Target = createdType
            };

            service.Execute(command);
            return true;
        }

        [HttpGet]
        [Route("metadata/recordtype/")]
        public List<RecordMetadata> GetRecordTypes(string entityName)
        {
            DataService service = new DataService(new Postgres(connectionString));
            var command = new RetrieveAllRecordTypesCommand();
            var result = (RetrieveAllRecordTypesResult)service.Execute(command);
            var output = new List<RecordMetadata>(result.RecordTypes.Count);
            foreach(var i in result.RecordTypes)
            {
                var tmp = new RecordMetadata()
                {
                    Name = i.TableName
                };
                foreach(var field in i.Fields)
                {
                    tmp.Fields.Add(field.Name, SerializeFieldMetadata(field));
                }
                output.Add(tmp);
            }
            return output;
        }
        [HttpGet]
        [Route("metadata/metadatadump")]
        public List<RecordMetadata> MetadataDump()
        {
            DataService service = new DataService(new Postgres(connectionString));
            var command = new RetrieveAllRecordTypesCommand();
            var result = (RetrieveAllRecordTypesResult)service.Execute(command);
            var output = new List<RecordMetadata>(result.RecordTypes.Count);
            foreach(var i in result.RecordTypes)
            {
                var tmp = new RecordMetadata()
                {
                    Name = i.TableName
                };
                foreach(var field in i.Fields)
                {
                    tmp.Fields.Add(field.Name, SerializeFieldMetadata(field));
                }
                output.Add(tmp);
            }
            return output;
        }
        [HttpPost]
        [Route("metadata/metadatadump")]
        public List<RecordMetadata> TestMetadataUpload([FromBody] List<RecordMetadata> input)
        {
            List<RecordMetadata> output = new List<RecordMetadata>();
            foreach(var i in input)
            {
                var tmp = new RecordMetadata()
                {
                    Name = i.Name,
                };
                foreach(var field in i.Fields)
                {
                    tmp.Fields.Add(field.Key, SerializeFieldMetadata(DeserializeFieldMetadata(field.Value)));
                }
                output.Add(tmp);
            }
            return output;
        }
        private RecordFieldMetadata SerializeFieldMetadata(IFieldType field)
        {
            List<string> ignoredFields = new List<string>() { "Name", "FieldType" };
            var type = field.GetType();
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            foreach (var f in type.GetFields())
            {
                if (!ignoredFields.Contains(f.Name))
                {
                    metadata.Add(f.Name, f.GetValue(field).ToString());
                }
            }
            return new RecordFieldMetadata()
            {
                Type = type.Name,
                Nullable = field.Nullable,
                Metadata = metadata,
            };
        }
        private IFieldType DeserializeFieldMetadata(RecordFieldMetadata field)
        {
            Dictionary<string, Type> fieldList = new Dictionary<string, Type>()
            {
                ["BinaryField"] = typeof(BinaryField),
                ["BooleanField"] = typeof(BooleanField),
                ["DateField"] = typeof(DateField),
                ["DateTimeField"] = typeof(DateTimeField),
                ["LookupField"] = typeof(LookupField),
                ["GuidField"] = typeof(GuidField),
                ["NumberField"] = typeof(NumberField),
                ["PrimaryField"] = typeof(PrimaryField),
                ["TextField"] = typeof(TextField),
            };
            var fieldType = fieldList[field.Type];
            var createdField = (IFieldType)Activator.CreateInstance(fieldType);
            foreach(var f in field.Metadata)
            {
                fieldType.GetField(f.Key).SetValue(createdField, ConvertFromString(fieldType.GetField(f.Key).FieldType, f.Value));
            }
            createdField.Nullable = field.Nullable;
            return createdField;
        }

        private object ConvertFromString(Type type, string value)
        {
            switch (type.Name)
            {
                case "String":
                    return value;
                case "Int32":
                    return int.Parse(value);
                case "Boolean":
                    return bool.Parse(value);
            }
            return null;
        }
        
    }
}