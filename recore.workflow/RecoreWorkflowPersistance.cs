using Newtonsoft.Json;
using recore.db;
using recore.db.Commands;
using recore.db.FieldTypes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace recore.workflow
{
    public class RecoreWorkflowPersistance : IPersistenceProvider
    {
        private readonly IDataService service;
        private readonly string workflowRecordName = "workflow";
        public RecoreWorkflowPersistance(IDataService service)
        {
            this.service = service;
        }
        public Task ClearSubscriptionToken(string eventSubscriptionId, string token)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreateEvent(Event newEvent)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreateEventSubscription(EventSubscription subscription)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreateNewWorkflow(WorkflowInstance workflow)
        {
            Record workflowRecord = new Record(workflowRecordName)
            {
                ["id"] = workflow.Id,
                ["workflowdefinitionid"] = workflow.WorkflowDefinitionId,
                ["version"] = workflow.Version,
                ["description"] = workflow.Description,
                ["reference"] = workflow.Reference,
                ["status"] = workflow.Status,
                ["executionpointers"] = JsonConvert.SerializeObject(workflow.ExecutionPointers),
                ["nextexecution"] = workflow.NextExecution,
                ["data"] = JsonConvert.SerializeObject(workflow.Data),
                ["createdon"] = workflow.CreateTime,
                ["completetime"] = workflow.CompleteTime
            };
            var result = (CreateRecordResult)service.Execute(new CreateRecordCommand() { Target = workflowRecord });
            return new Task<string>(() => result.RecordId.ToString());
        }

        public void EnsureStoreExists()
        {
            RecordType workflowType = new RecordType("workflow", "workflow");
            workflowType.Fields = new List<IFieldType>()
            {
                new TextField("id",500,false),
                new TextField("workflowdefinitionid",500,false),
                new NumberField("version",false),
                new NumberField("status",false),
                new TextField("executionpointers",10000,false),
                new TextField("data", 10000, false),
                new DateField("completetime", true)
            };

            RecordType eventType = new RecordType("workflowevent", "workflowevent");
            eventType.Fields = new List<IFieldType>()
            {
                new TextField("id",500,false),
                new TextField("eventname",500,false),
                new TextField("eventkey", 500, false),
                new TextField("eventdata", 100000, false),
                new DateField("eventtime",false),
                new BooleanField("isprocessed",false),
            };

            RecordType eventSubscriptionType = new RecordType("eventsubscription", "eventsubscription")
            {
                Fields = new List<IFieldType>()
                {
                    new TextField("id",500,false),
                    new TextField("workflowid", 500, false),
                    new NumberField("stepid",false),
                    new TextField("executionpointerid", 500, false),
                    new TextField("eventname",500,false),
                    new TextField("eventkey", 500, false),
                    new DateField("subscribeasof", false),
                    new TextField("subscriptiondata", 100000, false),
                    new TextField("externaltoken", 500, false),
                    new TextField("externalworkferid", 500, false),
                    new DateField("externaltokenexpiry", true),
                }
            };

            service.Execute(new CreateRecordTypeCommand { Target = workflowType });
            service.Execute(new CreateRecordTypeCommand { Target = eventType });
            service.Execute(new CreateRecordTypeCommand { Target = eventSubscriptionType });
        }

        public Task<Event> GetEvent(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetEvents(string eventName, string eventKey, DateTime asOf)
        {
            throw new NotImplementedException();
        }

        public Task<EventSubscription> GetFirstOpenSubscription(string eventName, string eventKey, DateTime asOf)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetRunnableEvents(DateTime asAt)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetRunnableInstances(DateTime asAt)
        {
            throw new NotImplementedException();
        }

        public Task<EventSubscription> GetSubscription(string eventSubscriptionId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<EventSubscription>> GetSubscriptions(string eventName, string eventKey, DateTime asOf)
        {
            throw new NotImplementedException();
        }

        public Task<WorkflowInstance> GetWorkflowInstance(string Id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<WorkflowInstance>> GetWorkflowInstances(WorkflowStatus? status, string type, DateTime? createdFrom, DateTime? createdTo, int skip, int take)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<WorkflowInstance>> GetWorkflowInstances(IEnumerable<string> ids)
        {
            throw new NotImplementedException();
        }

        public Task MarkEventProcessed(string id)
        {
            throw new NotImplementedException();
        }

        public Task MarkEventUnprocessed(string id)
        {
            throw new NotImplementedException();
        }

        public Task PersistErrors(IEnumerable<ExecutionError> errors)
        {
            throw new NotImplementedException();
        }

        public Task PersistWorkflow(WorkflowInstance workflow)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetSubscriptionToken(string eventSubscriptionId, string token, string workerId, DateTime expiry)
        {
            throw new NotImplementedException();
        }

        public Task TerminateSubscription(string eventSubscriptionId)
        {
            throw new NotImplementedException();
        }
    }
}
