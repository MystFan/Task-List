using MediatR;

namespace AdList.Application.Abstract.Command;

public interface ICommand<out TResponse> : IRequest<TResponse>
    where TResponse : ICommandResponse
{
    //
}