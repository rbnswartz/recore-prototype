using System.Collections.Generic;

namespace recore.db.Commands
{
    public interface ICommand
    {
        Dictionary<string, object> Data { get; set; }
    }
}