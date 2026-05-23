using AdList.Application.Abstract.Query;
using AdList.Application.Infrastructure;
using System.Security.Claims;

namespace AdList.Application.Queries.GetSmartTaskQuery
{
    public record GetSmartTaskQuery : IQuery<GetSmartTaskQueryResponse>, ICurrentPrincipalRequest
    {
        public long Id { get; set; }

        public ClaimsPrincipal? CurrentPrincipal { get; set; }
    }
}
