using System.Collections.Generic;

namespace Apzon.Services;

public class DataSourceModel<TEntity> where TEntity : class
{
	public PageModel Page { get; set; }

	public IEnumerable<TEntity> DataSource { get; set; }

	public IEnumerable<object> DataObjects { get; set; }
}
