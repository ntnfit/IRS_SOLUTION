namespace Apzon.Services;

public interface IService<TEntity> where TEntity : class
{
	string Insert(TEntity entity);

	string Update(TEntity entity);

	string Remove(TEntity entity);

	TEntity GetById(int id);
}
