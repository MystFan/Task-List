using AdList.Application.Abstract;

namespace AdList.DataAccess;

public class UnitOfWork : IUnitOfWork
{
    private readonly EFContext _context;

    public UnitOfWork(EFContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
