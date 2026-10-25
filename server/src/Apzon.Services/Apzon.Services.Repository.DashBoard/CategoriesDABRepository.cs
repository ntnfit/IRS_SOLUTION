using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Dashboard;

namespace Apzon.Services.Repository.DashBoard;

public class CategoriesDABRepository : BaseService<DataTable>, ICategoriesDAB
{
	private const string detailtemp = "#APZ_DAB1";

	private const string querytemp = "#APZ_DAB3";

	private const string cattemp = "#APZ_ODAB";

	public CategoriesDABRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetTreeListCategory(string type, int userId)
	{
		try
		{
			return ExecuteDataTable("APZ_Dashboard_GetTreeListCategory", CommandType.StoredProcedure, new SqlParameter("@type", type), new SqlParameter("@userId", userId));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataSet GetDetailByDasId(int dasId, int userId)
	{
		try
		{
			return ExecuteData("APZ_Dashboard_GetDetailByDasId", CommandType.StoredProcedure, new SqlParameter("@dasId", dasId), new SqlParameter("@userId", userId));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataTable GetLayoutByCatId(int userId)
	{
		try
		{
			return ExecuteDataTable("APZ_Dashboard_GetLayoutByCatId", CommandType.StoredProcedure, new SqlParameter("@userId", userId));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	protected virtual string GetXMLByDasIdQuery()
	{
		return "select Layout,LayoutDefault from APZ_DAB1 where Id=@dasId and (UserSign=@userId OR ISNULL(UserSign,0)=0)";
	}

	public DataTable GetXMLByDasId(int DashId, int userId)
	{
		try
		{
			string xMLByDasIdQuery = GetXMLByDasIdQuery();
			return ExecuteDataTable(xMLByDasIdQuery, CommandType.Text, new SqlParameter("@dasId", DashId), new SqlParameter("@userId", userId));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public string SaveDashboard(string document, string dashboardName, int userId, int catId)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				DataTable dataTable = ExecuteDataTable("APZ_Dashboard_SaveDashboard", CommandType.StoredProcedure, new SqlParameter("@document", document), new SqlParameter("@dashboardName", dashboardName), new SqlParameter("@userId", userId), new SqlParameter("@catId", catId));
				if (dataTable != null)
				{
					dbTransaction.Commit();
					return Function.ToString(dataTable.Rows[0]["Id"]);
				}
				dbTransaction.Rollback();
				return "0";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public HttpResult UpdateDashboard(string document, int userId, int dashId)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				if (ExecuteDataTable("APZ_Dashboard_UpdateDashboard", CommandType.StoredProcedure, new SqlParameter("@document", document), new SqlParameter("@userId", userId), new SqlParameter("@dasId", dashId)) != null)
				{
					dbTransaction.Commit();
					return new HttpResult(Enums.MessageCode.Success, "", "Save success!");
				}
				dbTransaction.Rollback();
				return new HttpResult(Enums.MessageCode.Error, "Save error!");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataTable LoadData(int DasId, int userId)
	{
		try
		{
			DataTable result = new DataTable();
			DataTable dataTable = ExecuteDataTable("APZ_Dashboard_LoadDataByDasId", CommandType.StoredProcedure, new SqlParameter("@dasId", DasId), new SqlParameter("@userId", userId));
			DataTable dataTable2 = ExecuteDataTable("APZ_Dashboard_LoadParameterDasId", CommandType.StoredProcedure, new SqlParameter("@dasId", DasId));
			List<SqlParameter> list = new List<SqlParameter>();
			for (int i = 0; i < dataTable2.Rows.Count; i++)
			{
				dynamic val = Function.ToString(dataTable2.Rows[i]["Type"]) switch
				{
					"System.String" => (dynamic)Function.ToString(dataTable2.Rows[i]["Value"]), 
					"System.Int32" => Function.ParseInt(dataTable2.Rows[i]["Value"]), 
					"System.DateTime" => Function.ParseDateTime(dataTable2.Rows[i]["Value"]), 
					"System.Double" => Function.ParseDouble(dataTable2.Rows[i]["Value"]), 
					"System.Decimal" => Function.ParseDouble(dataTable2.Rows[i]["Value"]), 
					_ => Function.ToString(dataTable2.Rows[i]["Value"]), 
				};
				list.Add(new SqlParameter(Function.ToString(dataTable2.Rows[i]["Name"]), val));
			}
			string a = Function.ToString(dataTable.Rows[0]["SqlType"]);
			string query = Function.ToString(dataTable.Rows[0]["Query"]);
			if (string.Equals(a, "S"))
			{
				result = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			}
			if (string.Equals(a, "T"))
			{
				result = ExecuteDataTable(query, CommandType.Text, list.ToArray());
			}
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetParameterCatId(int catId, int userId)
	{
		try
		{
			return ExecuteDataTable("APZ_Dashboard_LoadParameterCatId ", CommandType.StoredProcedure, new SqlParameter("@catId", catId), new SqlParameter("@userId", userId));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetLookupCategories()
	{
		try
		{
			string query = "Select Id, Name from APZ_ODAB";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public string Remove(int dashId, int userId)
	{
		string empty = string.Empty;
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				if (dashId > 0 && userId > 0)
				{
					List<SqlParameter> list = new List<SqlParameter>();
					list.Add(new SqlParameter("@dashId", dashId));
					list.Add(new SqlParameter("@userId", userId));
					if (ExecuteNonQuery("APZ_Dashboard_Remove", CommandType.StoredProcedure, list.ToArray()) > 0)
					{
						dbTransaction.Commit();
						return "";
					}
					dbTransaction.Rollback();
					return "Cannot remove";
				}
				dbTransaction.Rollback();
				return "You cannot remove record default in system";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Remove failed ", ex.Message);
			return ex.Message;
		}
	}

	public DataTable GetDataCategories()
	{
		try
		{
			return ExecuteDataTable("APZ_Dashboard_GetCategory", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public string DeleteCategory(string ids)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string query = "APZ_Dashboard_DeleteCategories";
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@ids", ids));
				DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
				if (dataTable.IsNotNull())
				{
					if (Function.ParseInt(dataTable.Rows[0][0]) == 0)
					{
						dbTransaction.Commit();
						return "";
					}
					dbTransaction.Rollback();
					return $"{Function.ParseInt(dataTable.Rows[0][0])}-{Function.ToString(dataTable.Rows[0][1])}";
				}
				dbTransaction.Rollback();
				return "Delete error!";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "DeleteDocs  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public HttpResult UpdateCaption(DataTable dt)
	{
		try
		{
			if (!dt.IsNotNull())
			{
				return new HttpResult(Enums.MessageCode.Error, "Data is empty");
			}
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				int num = Function.ParseInt(dt.Rows[0]["Id"]);
				int num2;
				if (!Exists(num))
				{
					string query = "SELECT MAX(\"Id\") FROM APZ_ODAB;";
					num = Function.ParseInt(ExecuteScalar(query, CommandType.Text)) + 1;
					string updateCaptionQuery = GetUpdateCaptionQuery();
					num2 = ExecuteNonQuery(updateCaptionQuery, CommandType.Text, new SqlParameter("@Id", Function.ToString(dt.Rows[0]["Id"])), new SqlParameter("@Name", Function.ToString(dt.Rows[0]["Caption"])));
				}
				else
				{
					string updateCaptionQuery = "update APZ_ODAB SET \"Name\"=@Name Where \"Id\"=@Id;";
					num2 = ExecuteNonQuery(updateCaptionQuery, CommandType.Text, new SqlParameter("@Name", Function.ToString(dt.Rows[0]["Caption"])), new SqlParameter("@Id", num));
				}
				if (num2 != 0)
				{
					dbTransaction.Commit();
					return new HttpResult(Enums.MessageCode.Success, "", num);
				}
				return new HttpResult(Enums.MessageCode.Error, "No row affected");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected virtual string GetUpdateCaptionQuery()
	{
		return "Declare @DashboardId int = (SELECT ISNULL((SELECT MAX(Id) FROM APZ_DAB1),0)+1);\r\n                                INSERT INTO  APZ_ODAB  (Id, Name, Description, Valid) VALUES(@Id,  @Name, @Name, 1);\r\n                                INSERT INTO dbo.APZ_DAB1 ( Id , CatId , ValidFor ) VALUES  ( @DashboardId,   @Id ,  1 );";
	}

	private bool Exists(int id)
	{
		try
		{
			return ExecuteDataTable("SELECT 1 FROM APZ_ODAB WHERE \"Id\" = @Id;", CommandType.Text, new SqlParameter("@Id", id)).IsNotNull();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return false;
		}
	}

	public HttpResult SaveCategories(DataTable dt)
	{
		try
		{
			Enums.TransactionType transtype = Enums.TransactionType.A;
			return Process(ApzObjectTypes.oDashboardCategories, -1, transtype, dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable dsSave, ref string objId, ref string objNum)
	{
		new HttpResult();
		Enums.TransactionType transactionType = transtype;
		if ((uint)transactionType <= 1u)
		{
			return SaveData(dsSave, transtype, ref objId, ref objNum);
		}
		return new HttpResult(Enums.MessageCode.Error, "Could not execute function");
	}

	public HttpResult SaveData(DataTable dt, Enums.TransactionType tranType, ref string objId, ref string objNum)
	{
		try
		{
			if (dt == null || dt.Rows.Count == 0)
			{
				return new HttpResult(Enums.MessageCode.Error, "Data must not null!");
			}
			CreateTempTable(dt, "#APZ_ODAB");
			BulkCopy(dt, "#APZ_ODAB");
			DataTable dataTable = ExecuteDataTable("APZ_Dashboard_SaveCategories", CommandType.StoredProcedure);
			if (dataTable.IsNotNull())
			{
				if (Function.ParseInt(dataTable.Rows[0][0]) == 0)
				{
					return new HttpResult(Enums.MessageCode.Success);
				}
				return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable.Rows[0][0])}-{Function.ToString(dataTable.Rows[0][1])}");
			}
			return new HttpResult(Enums.MessageCode.Error, (tranType == Enums.TransactionType.A) ? "Save error!" : "Update error");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult RemoveCategory(int id)
	{
		try
		{
			if (id > 0)
			{
				using (DatabaseService.OpenConnection())
				{
					using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
					string query = "DELETE APZ_ODAB WHERE Id=@Id DELETE APZ_DAB1 WHERE CatId=@Id AND ISNULL(Name,'')=''  UPDATE APZ_DAB1 SET CatId=NULL WHERE CatId=@Id ";
					if (ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@Id", id)) == 0)
					{
						return new HttpResult(Enums.MessageCode.Error, "No row affected");
					}
					dbTransaction.Commit();
					return new HttpResult(Enums.MessageCode.Success, "", id);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult(Enums.MessageCode.Error, "Unknown error");
	}
}
