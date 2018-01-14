namespace recore.db.QueryFilters
{
    public interface IQueryFilter
    {
        string Field { get; set; }
        string ToSQL();
    }
}