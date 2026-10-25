using System.Collections.Generic;

namespace Apzon.Entities.Models.Common;

public interface ITable<T> where T : class
{
	void Add(Row<T> row);

	void Remove(Row<T> row);

	void Insert(int idx, Row<T> row);

	Row<T> GetNewRow();

	Row<T> FirstOrDefault();

	ITable<T> GetClone();

	List<Row<T>> GetEnumerable();

	List<T> ToListObject();
}
