using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Services.Interfaces.Dashboard;

namespace Apzon.Services.Repository.DashBoard;

public class DashboardCategoriesRepository : BaseService<DataTable>, IDashboardCategories
{
	private const string TableTemp = "##dtCategory";

	public DashboardCategoriesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetData()
	{
		try
		{
			return ExecuteDataTable("SELECT * FROM apz_ODCT", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), " error Dashboard ", ex.Message);
			return new DataTable();
		}
	}

	public string SaveData(DataTable dtCategories)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				if (!dtCategories.IsNotNull())
				{
					ExecuteNonQuery("DELETE FROM apz_ODCT", CommandType.Text);
					dbTransaction.Commit();
					return "";
				}
				CreateTempTable(dtCategories, "##dtCategory");
				BulkCopy(dtCategories, "##dtCategory");
				ExecuteNonQuery("DELETE FROM apz_ODCT", CommandType.Text);
				if (ExecuteNonQuery("INSERT INTO apz_ODCT SELECT CatId, Name, Valid FROM ##dtCategory", CommandType.Text) > 0)
				{
					dbTransaction.Commit();
					return "";
				}
				return "Save Category not success!";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), " error Dashboard ", ex.Message);
			return ex.Message;
		}
	}

	public string CheckExistDashboardTab(int CatId)
	{
		try
		{
			if (ExecuteDataTable("SELECT * FROM APZ_ODTB WHERE CatId=@CatId", CommandType.Text, new SqlParameter("@CatId", CatId)).IsNotNull())
			{
				return "";
			}
			return "No row exist!";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), " error Dashboard ", ex.Message);
			return ex.Message;
		}
	}
}
