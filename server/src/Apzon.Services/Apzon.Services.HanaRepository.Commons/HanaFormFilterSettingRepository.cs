using Apzon.Services.Repository.Commons;

namespace Apzon.Services.HanaRepository.Commons;

public class HanaFormFilterSettingRepository : FormFilterSettingRepository
{
	public HanaFormFilterSettingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetListColumnQuery()
	{
		return "SELECT COLUMN_NAME FROM SYS.TABLE_COLUMNS WHERE SCHEMA_NAME = CURRENT_SCHEMA AND TABLE_NAME = @tableName ORDER BY POSITION;";
	}
}
