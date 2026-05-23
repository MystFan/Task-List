using AdList.Application.Abstract.Command;


namespace AdList.Application.Commands.GetTasksCommand
{
    public record GetTasksCommandResponse : ICommandResponse
    {
        public GetTaskCommandResponse[] Tasks { get; init; } = [];

        public long TotalCount { get; init; }
    }

    public record GetTaskCommandResponse
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
