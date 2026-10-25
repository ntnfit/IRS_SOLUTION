using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Repository.Commons;

namespace Apzon.Services.HanaRepository.Commons;

public class HanaContextActionRepository : ContextActionRepository
{
	public HanaContextActionRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public override string GetProcessGroupQuery()
	{
		return "   declare i integer := 0;\r\n                        declare n integer; \r\n                        CREATE local temporary   TABLE #ret AS (SELECT top 0 * FROM APZ_OCTA);\r\n                        insert into #ret select * from APZ_OCTA WHERE \"FormId\" = FormId AND \"ItemId\" = ItemId AND \"AbsEntry\" = AbsEntry;\r\n\r\n                        CREATE local temporary TABLE #tbl AS (SELECT top 0 * FROM APZ_OCTA);\r\n                        insert into #tbl select * from APZ_OCTA  WHERE \"FormId\" = FormId AND \"ItemId\" = ItemId AND \"AbsEntry\" <> AbsEntry;\r\n\r\n                        select IFNULL((SELECT COUNT(1) FROM #tbl), 0) into n from DUMMY;\r\n                        WHILE(:i < :n) DO\r\n                            INSERT INTO #ret (SELECT * FROM #tbl WHERE \"FatherId\" IN (SELECT \"AbsEntry\" FROM #ret));\r\n                            DELETE FROM #tbl WHERE \"AbsEntry\" IN (SELECT \"AbsEntry\" FROM #ret);\r\n                            select IFNULL((SELECT COUNT(1) FROM #tbl), 0) into n from DUMMY;\r\n                        i:= :i + 1;\r\n                        END WHILE;\r\n                        SELECT* FROM #ret;";
	}

	public override string GetTargetFieldQuery(string sonName)
	{
		try
		{
			string empty = string.Empty;
			return string.IsNullOrEmpty(sonName) ? "SELECT T0.\"ColAlias\"  \"Code\", T0.\"ColDesc\" \"Name\", T0.\"ColumnNum\" FROM UDO3 T0 WHERE T0.\"Code\" = @UdoCode AND T0.\"ColEdit\" = 'Y' \r\n                            UNION\r\n                            SELECT 'DocEntry', 'DocEntry', -1 from dummy\r\n                            ORDER BY T0.\"ColumnNum\";" : "SELECT   T0.\"ColAlias\" \"Code\",T0.\"ColDesc\" \"Name\"  \r\n                        FROM UDO4 T0\r\n                        INNER JOIN UDO1 T2 ON T0.\"Code\" = T2.\"Code\" AND T0.\"SonNum\" = T2.\"SonNum\" AND T2.\"SonName\" = @SonName\r\n                    AND T0.\"Code\" = @UdoCode\r\n                    AND T0.\"ColIsUsed\" = 'Y' AND T0.\"ColEdit\" = 'Y' Order by T0.\"ColumnNum\";";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return string.Empty;
		}
	}

	public override string QueryGetColumnsFromPhysicalTableName()
	{
		return "SELECT COLUMN_NAME,DATA_TYPE_NAME,LENGTH FROM Table_Columns WHERE SCHEMA_NAME = current_Schema and TABLE_NAME = @TableName;";
	}

	public override string QuerySavePhysicalData(string tableName)
	{
		return "exec 'insert into " + tableName + " select * from #TEMP_" + tableName + ";';";
	}

	public override DataTable GetHeaderByAlias(string alias, string formId, string itemId)
	{
		try
		{
			if (!string.IsNullOrEmpty(alias))
			{
				return ExecuteDataTable("SELECT * FROM APZ_OCTA WHERE \"UniqueAlias\" =@Alias and \"FormId\" = @formId \r\nAND (@formId <> 'oUserDefinedReports' or (@formId = 'oUserDefinedReports' and (\"ItemId\" = @itemId or \"ItemId\" = 'B$' || @itemId)));", CommandType.Text, new SqlParameter("@Alias", alias), new SqlParameter("@formId", formId), new SqlParameter("@itemId", itemId));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new DataTable();
	}
}
