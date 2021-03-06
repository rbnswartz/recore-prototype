﻿using System;
using System.Collections.Generic;
using System.Text;

namespace recore.db.Commands
{
    public class RetrieveAllCommand : CommandBase
    {
        public string RecordType
        {
            get
            {
                if (this.Data.ContainsKey("Type"))
                {
                    return (string) this.Data["Type"];
                }

                return null;
            }
            set
            {
                if (this.Data.ContainsKey("Type"))
                {
                    this.Data["Type"] = value;
                }
                else
                {
                    this.Data.Add("Type", value);
                }
            }
        }

        public List<string> Columns
        {
            get
            {
                if (this.Data.ContainsKey("Columns"))
                {
                    return (List<string>) this.Data["Columns"];
                }

                return null;
            }
            set
            {
                if (this.Data.ContainsKey("Columns"))
                {
                    this.Data["Columns"] = value;
                }
                else
                {
                    this.Data.Add("Columns", value);
                }
            }
        }
    }
}
