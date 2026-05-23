using AdList.Application.Abstract.Query;

namespace AdList.Application.Queries.GetSmartTaskQuery
{
    public record GetSmartTaskQueryResponse : IQueryResponse
    {
        public long Id { get; init; }

        public string Title { get; init; } = null!;

        public string? Description { get; init; }

        public DateTime? DueDate { get; init; }

        public string CompletionStatus { get; init; } = null!;

        public string AuthorName { get; init; } = null!;

        public DateTime CreatedAt { get; init; }
    }
}
