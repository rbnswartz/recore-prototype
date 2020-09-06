using recore.db.Commands;

namespace recore.db
{
    public interface IDataService
    {
        ResultBase Execute(CommandBase command);
    }
}