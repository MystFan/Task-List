namespace AdList.DataAccess
{
    public class DatabaseOptions
    {
        public const string SectionName = "database";

        public int? SqlCommandTimeout { get; init; }

        public IDictionary<string, string> ConnectionStrings { get; init; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }
}
