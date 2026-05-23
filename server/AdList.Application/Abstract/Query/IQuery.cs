using MediatR;

namespace AdList.Application.Abstract.Query;

public interface IQuery<out TResponse> : IRequest<TResponse>
    where TResponse : IQueryResponse
{
    //
}