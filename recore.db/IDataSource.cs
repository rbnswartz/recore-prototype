using System;
using System.Collections.Generic;

namespace recore.db
{
    public interface IDataSource
    {
        bool CheckInitialized();
        void Initialize();
        List<RecordType> GetAllTypes();
        RecordType GetRecordType(Guid typeId);
        RecordType GetRecordType(string typeName);
        void CreateRecordType(RecordType type);
        void DeleteRecordType(Guid typeId);
        Guid CreateRecord(Record record);
        Record RetrieveRecord(string typeName, Guid id, List<string> columns);
        List<Record> RetrieveAllRecords(string typeName, List<string> columns);
    }
}