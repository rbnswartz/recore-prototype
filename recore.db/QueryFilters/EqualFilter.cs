namespace recore.db.QueryFilters
{
    public class EqualFilter : IQueryFilter
    {
        public string Field { get; set; }

        public string ToSQL()
        {
            return $"{Field} = ";
        }
    }
}