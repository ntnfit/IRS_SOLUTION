namespace Apzon.DataServices;

public interface IRepositoryAsync<TEntity> : IRepository<TEntity> where TEntity : class
{
}
