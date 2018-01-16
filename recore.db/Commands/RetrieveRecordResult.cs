namespace recore.db.Commands
{
    public class RetrieveRecordResult: ResultBase
    {
        public Record Result
        {
            get => (Record)this["Result"];
            set => this["Result"] = value;
        }
    }
}