using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Repository.Opportunity;

namespace Apzon.Services.HanaRepository.Opportunity;

public class HanaOpportunityRepository : OpportunityRepository
{
	public HanaOpportunityRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetOwnerForStageQuery()
	{
		return "select \"empID\" \"Code\",\"lastName\" || ', '|| \"firstName\" \"Name\" from ohem;";
	}

	protected override string GetBpChanelContactQuery()
	{
		return "declare defaultName nvarchar(100);\r\n                    select iFnull((select \"CntctPrsn\" from OCRD where \"CardCode\"=@cardCode), '') into defaultName from DUMMY;\r\n                SELECT \"CntctCode\", \"Name\", CASE WHEN \"Name\" = defaultName THEN 'Y' ELSE 'N' END AS \"DfltCode\" FROM ocpr WHERE \"Active\" = 'Y' \r\n                        AND \"CardCode\" = @cardCode;";
	}

	public override string ProcessDataReasons(DataTable dt, int retKey)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				ExecuteNonQuery("DELETE FROM OPR5 WHERE \"OpportId\"=@OpportId;", CommandType.Text, new SqlParameter("@OpportId", retKey));
				if (dt != null && dt.Rows.Count > 0)
				{
					List<SqlParameter> list = new List<SqlParameter>();
					string text = "INSERT INTO OPR5 (\"OpportId\", \"Line\", \"ReasondId\") \r\n";
					for (int i = 0; i < dt.Rows.Count; i++)
					{
						if (i > 0)
						{
							text += "UNION ALL\r\n";
						}
						text = text + "select @OpportId" + i + ",@Line" + i + ",@ReasondId" + i + " from DUMMY";
						list.AddRange(new SqlParameter[3]
						{
							new SqlParameter("@OpportId" + i, retKey),
							new SqlParameter("@Line" + i, i),
							new SqlParameter("@ReasondId" + i, Function.ParseInt(dt.Rows[i]["ReasondId"]))
						});
					}
					text += ";";
					ExecuteNonQuery(text, CommandType.Text, list.ToArray());
				}
				dbTransaction.Commit();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return "";
	}

	public override string AddFile()
	{
		try
		{
			return Function.ToString(ExecuteScalar("insert into OATC (\"AbsEntry\")  select ifnull(max(\"AbsEntry\"),0)+1 From OATC;\r\n                                   Update ONNM set \"AutoKey\"= ifnull((select max(\"AbsEntry\")+1 from OATC),0) where \"ObjectCode\"='221';\r\n                                   select ifnull((select max(\"AbsEntry\") from OATC),0) from DUMMY;", CommandType.Text));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC file oatc table FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public override string GetItemDetailUdfColumn()
	{
		return "SELECT COLUMN_NAME \"name\",DATA_TYPE_NAME \"datatype\" FROM TABLE_COLUMNS WHERE SCHEMA_NAME = CURRENT_SCHEMA and TABLE_NAME=@TableName AND COLUMN_NAME LIKE 'U\\_%' ESCAPE '\\';";
	}
}
