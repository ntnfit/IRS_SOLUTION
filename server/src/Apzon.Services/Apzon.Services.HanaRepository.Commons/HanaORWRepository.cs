using Apzon.Services.Repository.Commons;

namespace Apzon.Services.HanaRepository.Commons;

public class HanaORWRepository : ORWRepository
{
	public HanaORWRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetTableColumnChildQuery()
	{
		return "select DISTINCT A.COLUMN_NAME, CASE WHEN IFNULL(T1.\"Descr\", '')='' THEN A.COLUMN_NAME ELSE T1.\"Descr\" END \"Dscription\", \r\n                     CASE WHEN IFNULL(b.\"ColEdit\", 'N') <> 'Y' THEN 'N' ELSE 'Y' END as \"ColEdit\", CASE WHEN IFNULL(b.\"ColIsUsed\", 'N') <> 'Y' THEN 'N' ELSE 'Y' END \"ColIsUsed\", '' as \"LogName\"\r\n                     from SYS.TABLE_COLUMNS A \r\n                     LEFT JOIN CUFD T1 ON A.TABLE_NAME = T1.\"TableID\" AND A.COLUMN_NAME = 'U_' || T1.\"AliasID\" \r\n                     INNER JOIN OUTB D ON SUBSTRING(A.TABLE_NAME, 2, 19) = D.\"TableName\"\r\n                     LEFT JOIN UDO4 B ON A.COLUMN_NAME = B.\"ColAlias\" and B.\"Code\" = D.\"UsedInObj\" \r\n                     WHERE TABLE_NAME = '@'||@column;";
	}

	protected override string GetColEditTableUDO2Query()
	{
		return "select A.COLUMN_NAME, case when B.\"Code\" is null then 'N' else 'Y' end \"IsCheck\" from SYS.TABLE_COLUMNS A \r\n\t\t\t\t\t  INNER JOIN OUDO C on A.TABLE_NAME ='@'||C.\"TableName\"\r\n\t\t\t\t\t  LEFT JOIN UDO2 B ON A.COLUMN_NAME = B.\"ColAlias\" and C.\"Code\" = B.\"Code\"  WHERE C.\"Code\" = @column;";
	}

	protected override string GetDropTableLogQuery(string logTable)
	{
		return string.Format("declare result int;\r\n\t                 select IFNULL((select 1 from SYS.TABLES where SCHEMA_NAME = CURRENT_SCHEMA and TABLE_NAME = '{0}'), 0) into result from dummy;\r\n                    if result = 0\r\n                    then\r\n                        select -1 into result from dummy;\r\n                    else\r\n                       drop table \"{0}\";\r\n                       select 1 into result from dummy;\r\n                    end if;\r\n                    select result from dummy;", "@" + logTable);
	}
}
