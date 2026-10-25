using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;

namespace Apzon.Services.Repository.ImportData.BillOfMaterials;

public class ImportDataBillOfMaterialsRepository : BaseService<DataTable>
{
	public ImportDataBillOfMaterialsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataSet GetBillOfMaterials(string code)
	{
		try
		{
			string query = "SELECT * FROM OITT WHERE code = @code;\r\n                            SELECT * FROM ITT1 WHERE father = @code";
			return ExecuteData(query, CommandType.Text, new SqlParameter("@code", code));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public bool ExistsBOM(string father)
	{
		try
		{
			string query = "SELECT 1 FROM OITT WHERE code=@father";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@father", father));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return false;
		}
	}

	public bool ExistsBOMChild(string father, int childNum, string code)
	{
		try
		{
			string query = "SELECT 1 FROM ITT1 WHERE father=@father AND childNum=@childNum AND code=@code";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@father", father), new SqlParameter("@childNum", childNum), new SqlParameter("@code", code));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return false;
		}
	}
}
