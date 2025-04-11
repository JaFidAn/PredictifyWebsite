using Domain.Entities.Common;

namespace Application.Repositories;

public interface IRepository<T, TKey> where T : BaseEntity<TKey>
{
}

