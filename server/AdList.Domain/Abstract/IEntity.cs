namespace AdList.Domain.Abstract;

public interface IEntity
{
    object? Id { get; }
}

public interface IEntity<TKey> : IEntity
{
    new TKey Id { get; set; }
}