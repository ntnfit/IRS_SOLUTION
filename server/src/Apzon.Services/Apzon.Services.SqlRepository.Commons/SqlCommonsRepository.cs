using Apzon.Services.Repository.Commons;

namespace Apzon.Services.SqlRepository.Commons;

public class SqlCommonsRepository : CommonsRepository
{
	public SqlCommonsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public override string GetDatabaseListQuery(bool loginSap)
	{
		string result = "select a.CmpDB as dbName, a.CmpName as cmpName from APZ_SLSP a INNER JOIN \r\n\t\t\t\t\t\t\t(SELECT t1.[Database] from APZ_OLDD t1 INNER JOIN \r\n\t\t\t\t\t\t\t(Select * from APZ_OCDB WHERE Domain = @domain) t2 ON t1.Domain = t2.Domain) b ON b.[Database] = a.CmpDB";
		if (loginSap)
		{
			result = "select a.CmpDB as dbName, T3.CmpName as cmpName from APZ_SLSP a INNER JOIN \r\n                        (SELECT t1.[Database] from APZ_OLDD t1 INNER JOIN\r\n                        (Select * from APZ_OCDB WHERE Domain = @domain) t2 ON t1.Domain = t2.Domain) b ON b.[Database] = a.CmpDB\r\n\r\n                        INNER JOIN [SBO-COMMON].dbo.SRGC T3 ON A.CmpDB COLLATE SQL_Latin1_General_CP850_CI_AS = T3.dbName COLLATE SQL_Latin1_General_CP850_CI_AS";
		}
		return result;
	}

	public override string GetInfoCompanyDbServerQuery()
	{
		return "select CpnDBServer, CpnDBUser, CpnDBPass, Status from APZ_SLSP where CmpDB = @cmpDb";
	}

	protected override string GetCheckExitsDatabaseQuery()
	{
		return "SELECT [Version] FROM APZ_CINF";
	}

	protected override string ExistsTableQuery()
	{
		return "DECLARE @TableName VARCHAR(100) = @TablePara;\r\n                            DECLARE @Exists bigint = ( SELECT COUNT(1) FROM sys.tables WHERE name = @TableName AND type = 'U' );\r\n                            select @Exists";
	}
}
