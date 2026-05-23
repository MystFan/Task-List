namespace AdList.Infrastructure
{
    public interface IDateTimeProvider
    {
        DateTime UtcNow { get; }

        DateTime Now { get; }
    }
}
