namespace recore.db.Commands
{
    public class CreateRecordTypeCommand : CommandBase
    {
        public RecordType Target
        {
            get
            {
                if (this.Data.ContainsKey("Target"))
                {
                    return (RecordType) this.Data["Target"];
                }

                return null;
            }
            set
            {
                if (this.Data.ContainsKey("Target"))
                {
                    this.Data["Target"] = value;
                }
                else
                {
                    this.Data.Add("Target", value);
                }
            }
        }
    }
}