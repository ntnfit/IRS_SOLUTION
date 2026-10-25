using Apzon.Services;

namespace Apzon.DataServices;

public class RepositoryAsync<TEntity> : Repository<TEntity>, IRepositoryAsync<TEntity>, IRepository<TEntity> where TEntity : class
{
	public RepositoryAsync(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}
}
