using recore.db.FieldTypes;
using System;
using System.Collections.Generic;

namespace recore.db
{
    public interface IDataSource
    {
        void Open();
        void Close();

        bool CheckInitialized();
        void Initialize();
        List<RecordType> GetAllTypes();
        RecordType GetRecordType(Guid typeId);
        RecordType GetRecordType(string typeName);
        void CreateRecordType(RecordType type);
        void DeleteRecordType(Guid typeId);
        Guid CreateRecord(Record record);
        void DeleteRecord(string recordType, Guid id);
        void UpdateRecord(Record record);
        Record RetrieveRecord(string typeName, Guid id, List<string> columns);
        List<Record> RetrieveAllRecords(string typeName, List<string> columns);
        List<RecordType> RetrieveAllRecordTypes();
        RecordType RetrieveRecordType(string typeName);
        void AddFieldToRecordType(string typeName, IFieldType field);
        void RemoveFieldFromRecordType(string typeName, string fieldName);
    }
}