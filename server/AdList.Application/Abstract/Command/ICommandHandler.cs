using MediatR;

namespace AdList.Application.Abstract.Command;

public interface ICommandHandler<in TCommand, TCommandResponse> : IRequestHandler<TCommand, TCommandResponse>
    where TCommand : ICommand<TCommandResponse>
    where TCommandResponse : ICommandResponse
{
    //
}