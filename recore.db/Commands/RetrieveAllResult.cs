using System;
using System.Collections.Generic;
using System.Text;

namespace recore.db.Commands
{
    public class RetrieveAllResult : ResultBase
    {
        public List<Record> Result
        {
            get
            {
                if (this.Data.ContainsKey("Result"))
                {
                    return (List<Record>) this.Data["Result"];
                }

                return null;
            }
            set
            {
                if (this.Data.ContainsKey("Result"))
                {
                    this.Data["Result"] = value;
                }
                else
                {
                    this.Data.Add("Result", value);
                }
            }
        }
    }
}
