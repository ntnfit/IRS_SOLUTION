using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;

namespace Apzon.Services.Repository.ImportData.BusinessPartners;

public class ImportDataBPMasterRepository : BaseService<DataTable>
{
	public ImportDataBPMasterRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public bool CheckExistBP(string cardCode)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("Select 1 from ocrd where cardcode=@cardCode", CommandType.Text, new SqlParameter("@cardCode", cardCode));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return false;
	}

	public int GetBPManualSeries(string objectCode, string subType)
	{
		try
		{
			string query = "SELECT Series FROM nnm1 WHERE objectcode=@objectCode AND DocSubType=@subType AND IsManual='Y'";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@objectCode", objectCode), new SqlParameter("@subType", subType));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0][0]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return 0;
		}
	}
}
