using AdList.Application.Abstract.Command;
using AdList.Application.Abstract.Implementation;

namespace AdList.Application.Commands.CreateUserCommand
{
    public record CreateUserCommand(string? Name, string? Email) : ICommand<EmptyResponse>;
}
