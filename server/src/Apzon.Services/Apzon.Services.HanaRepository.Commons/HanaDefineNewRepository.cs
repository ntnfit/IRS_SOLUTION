using Apzon.Services.Repository.Commons;

namespace Apzon.Services.HanaRepository.Commons;

public class HanaDefineNewRepository : DefineNewRepository
{
	public HanaDefineNewRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string ExistsTableGetQuery()
	{
		return "SELECT 1 FROM TABLE_COLUMNS WHERE SCHEMA_NAME = current_Schema and table_name = @TableName;";
	}

	protected override string ExistsColumnGetQuery()
	{
		return "SELECT 1 FROM TABLE_COLUMNS WHERE SCHEMA_NAME = current_Schema and table_name = @Table and column_name = @Column;";
	}

	protected override string ProcessDataGetQuery(string tableName, string tempTableTab, string fieldName)
	{
		return string.Format("\r\n                                    declare i int; \r\n                                    create local temporary table #TempData as (select top 0 * from {0});\r\n                                    exec 'drop table #TempData;';\r\n                                    exec 'create local temporary table #TempData as (select * from \"{1}\");';\r\n\t                                UPDATE #TempData SET \"{2}\"  = IFNULL((SELECT MAX(cast(\"{2}\" as int)) FROM {0}),0) + 1\r\n\t                                    where \"{2}\" = 0 OR \"{2}\" is null;\r\n                                     DELETE from {0};\r\n                                     INSERT INTO {0}\r\n                                     SELECT * FROM #TempData;", tableName, tempTableTab, fieldName);
	}

	protected override string ExistsColumnsQuery()
	{
		return "SELECT LENGTH FROM table_columns WHERE SCHEMA_NAME = current_Schema and table_name = @table and column_name = @column;";
	}
}
