using System.Data;
using Apzon.Services.Repository.Administrator;

namespace Apzon.Services.SqlRepository.Administrator;

public class SqlGeneralSettingRepository(IDatabaseService databaseService) : GeneralSettingRepository(databaseService)
{
	private DataTable dtATC2 = new DataTable();

	private const string TempTable = "#dtAPZ_ATC2";
}
