using AdList.Domain.Entities;

namespace AdList.Application.Abstract;

public interface ISmartTaskRepository : IRepository<SmartTask>
{
    IQueryable<SmartTask> GetOverdue();
}
