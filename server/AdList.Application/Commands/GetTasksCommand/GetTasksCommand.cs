using AdList.Application.Abstract.Command;
using AdList.Application.Infrastructure;
using System.Security.Claims;

namespace AdList.Application.Commands.GetTasksCommand
{
    public record GetTasksCommand : ICommand<GetTasksCommandResponse>, ICurrentPrincipalRequest
    {
        public int? StartIndex { get; set; }

        public int? EndIndex { get; set; }

        public SortOption[] Sorts { get; set; } = [];

        public ClaimsPrincipal? CurrentPrincipal { get; set; }
    }

    public enum SortDirection
    {
        Asc,
        Desc
    }

    public class SortOption
    {
        public string Name { get; set; } = null!;

        public SortDirection Direction { get; set; }
    }
}
