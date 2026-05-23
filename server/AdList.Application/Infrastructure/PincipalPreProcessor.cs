using MediatR.Pipeline;

namespace AdList.Application.Infrastructure.Processors;

public class PrincipalPreProcessor<TRequest>(IPrincipalProvider principalProvider) : IRequestPreProcessor<TRequest>
    where TRequest : ICurrentPrincipalRequest
{
    public Task Process(TRequest request, CancellationToken cancellationToken)
    {
        request.CurrentPrincipal = principalProvider.Current;
        return Task.CompletedTask;
    }
}