using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Administrator;

namespace Apzon.Services.Repository.Administrator;

public class UnitOfMeasureGroupsRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), IUnitOfMeasureGroups
{
	public string TempTable = "#OUGP";

	public DataTable LoadDoc()
	{
		try
		{
			return ExecuteDataTable("APZ_UoMGroup_GetDoc", CommandType.StoredProcedure, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get Doc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public HttpResult Save(DataTable dt)
	{
		return Process(ApzObjectTypes.oUoMGroups, -1, Enums.TransactionType.U, dt);
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable document, ref string objId, ref string objNum)
	{
		objId = "";
		objNum = "";
		try
		{
			CreateTempTable(document, TempTable);
			BulkCopy(document, TempTable);
			int num = ExecuteNonQuery("SP_APZ_OUGP_SAVE", CommandType.StoredProcedure);
			objId = Function.ToString(num);
			objNum = objId;
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataTable LoadDefinition(int ugpEntry)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@UgpEntry", ugpEntry)
			};
			DataTable dataTable = ExecuteDataTable("APZ_UoMGroup_GetDefinitionByEntry", CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get Doc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public string CleanUoMGroup()
	{
		try
		{
			ExecuteNonQuery("SP_APZ_OUGP_CleanDefinition", CommandType.StoredProcedure, (SqlParameter[])null);
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Clean Definition  FAILED ", ex.Message);
			return "";
		}
	}

	public DataTable CheckGroupInItems(int ugpEntry)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@UgpEntry", ugpEntry)
			};
			return ExecuteDataTable("APZ_UoMGroup_CheckInItems", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get Doc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable CheckUoMInGroups(int uomEntry)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@UomEntry", uomEntry)
			};
			return ExecuteDataTable("APZ_UoM_CheckInGroup", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get Doc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDataUomInGroup(int ugpEntry, int uomEntry)
	{
		try
		{
			return ExecuteDataTable("Select a.*, b.\"BaseQty\" / b.\"AltQty\" \"ItemPerUnit\" from OUOM a INNER JOIN UGP1 b ON a.\"UomEntry\" = b.\"UomEntry\" where b.\"UgpEntry\" = @ugpEntry and a.\"UomEntry\" = @uomEntry;", CommandType.Text, new SqlParameter("@ugpEntry", ugpEntry), new SqlParameter("@uomEntry", uomEntry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDataUomInGroup FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public bool CheckUoMGroupExists(int ugpEntry)
	{
		try
		{
			string query = "select 1 from OUGP where \"UgpEntry\" = @ugpEntry;";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@ugpEntry", ugpEntry)).IsNotNull();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return false;
		}
	}

	public DataTable GetUomItem(string itemCode)
	{
		try
		{
			return ExecuteDataTable("select a.\"UomEntry\",a.\"UomCode\", a.\"UomName\", case when a.\"UomEntry\" = c.\"IUoMEntry\" then 'Y' else 'N' end \"BaseUom\" from OUOM a inner join UGP1 b on a.\"UomEntry\" = b.\"UomEntry\" \r\n                                                                inner join OITM c ON b.\"UgpEntry\" = c.\"UgpEntry\" where c.\"ItemCode\" = @itemCode and c.\"UgpEntry\" <> -1;", CommandType.Text, new SqlParameter("@itemCode", itemCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}
}
