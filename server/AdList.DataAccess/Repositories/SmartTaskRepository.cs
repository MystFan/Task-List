using AdList.Application.Abstract;
using AdList.Domain.Entities;
using AdList.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AdList.DataAccess.Repositories;

public class SmartTaskRepository : Repository<SmartTask>, ISmartTaskRepository
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public SmartTaskRepository(EFContext context, IDateTimeProvider dateTimeProvider) : base(context)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public IQueryable<SmartTask> GetOverdue()
    {
        var now = _dateTimeProvider.UtcNow;
        return GetAll()
            .AsNoTracking()
            .Where(t => t.DueDate.HasValue && t.DueDate.Value < now && t.CompletionStatus != CompletionStatus.Completed);
    }
}
