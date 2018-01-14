namespace recore.db.Commands
{
    public class CreateRecordCommand : CommandBase
    {
        public Record Target
        {
            get
            {
                if (this.Data.ContainsKey("Target"))
                {
                    return (Record) this.Data["Target"];
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