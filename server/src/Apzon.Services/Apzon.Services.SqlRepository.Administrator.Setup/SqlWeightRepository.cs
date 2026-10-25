using System.Data;
using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.SqlRepository.Administrator.Setup;

public class SqlWeightRepository(IDatabaseService databaseService) : WeightRepository(databaseService)
{
	private DataTable dt = new DataTable();
}
