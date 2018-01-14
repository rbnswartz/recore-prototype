using System;

namespace recore.db.Commands
{
    public class CreateRecordResult : ResultBase
    {
        public Guid RecordId
        {
            get
            {
                if (this.Data.ContainsKey("RecordId"))
                {
                    return (Guid) this.Data["RecordId"];
                }

                return Guid.Empty;
            }
            set
            {
                if (this.Data.ContainsKey("RecordId"))
                {
                    this.Data["RecordId"] = value;
                }
                else
                {
                    this.Data.Add("RecordId", value);
                }
            }
        }
    }
}