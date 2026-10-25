using System.Data.Entity;

namespace Apzon.Entities.Models;

public class SboCommonContext : DbContext
{
	static SboCommonContext()
	{
		Database.SetInitializer<ApzDbContext>(null);
	}

	public SboCommonContext(string connectionString)
	{
		Database.Connection.ConnectionString = connectionString;
	}
}
