namespace Apzon.Entities.Models.Common;

public interface IRow<T> where T : class
{
	T ToObject();
}
