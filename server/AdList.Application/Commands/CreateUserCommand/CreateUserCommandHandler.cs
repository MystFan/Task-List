using AdList.Application.Abstract;
using AdList.Application.Abstract.Command;
using AdList.Application.Abstract.Implementation;
using AdList.Domain.Entities;
using AdList.Domain.Exceptions;
using Microsoft.Extensions.Caching.Memory;

namespace AdList.Application.Commands.CreateUserCommand
{
    internal sealed class CreateUserCommandHandler(IMemoryCache memoryCache, IRepository<ApplicationUser> repository) : ICommandHandler<CreateUserCommand, EmptyResponse>
    {
        public async Task<EmptyResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            if (request.Email is null)
            {
                throw new DomainException(ExceptionReasonCode.UserEmailIsRequired, "User email is required.");
            }

            return await memoryCache.GetOrCreate(request.Email, async cacheEntry =>
            {
                bool userExists = await repository.ExistsAsync(user => user.Email == request.Email, cancellationToken);
                if (!userExists)
                {
                    var user = new ApplicationUser
                    {
                        Email = request.Email,
                        Name = request.Name
                    };

                    await repository.CreateAsync(user, cancellationToken);
                }

                return EmptyResponse.Instance;
            })!;
        }
    }
}
