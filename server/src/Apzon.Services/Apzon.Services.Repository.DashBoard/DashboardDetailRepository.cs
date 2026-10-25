using System;
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

public class DashboardDetailRepository : BaseService<DataSet>, IDashboardDetail
{
	private const string detailtemp = "#APZ_DAB1";

	private const string querytemp = "#APZ_DAB3";

	private const string cattemp = "#APZ_ODAB";

	public DashboardDetailRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public HttpResult SaveDetailInForm(DataSet ds, string action)
	{
		try
		{
			Enums.TransactionType transtype = Enums.TransactionType.A;
			switch (action)
			{
			case "A":
				transtype = Enums.TransactionType.A;
				break;
			case "U":
				transtype = Enums.TransactionType.U;
				break;
			case "D":
				transtype = Enums.TransactionType.D;
				break;
			}
			return Process(ApzObjectTypes.oDashboardCategories, -1, transtype, ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet dsSave, ref string objId, ref string objNum)
	{
		switch (transtype)
		{
		case Enums.TransactionType.A:
		case Enums.TransactionType.U:
			return SaveData(dsSave, transtype, ref objId, ref objNum);
		case Enums.TransactionType.D:
		{
			DataTable dataTable = ExecuteDataTable("SELECT T0.\"AbsEntry\",T1.\"Caption\" FROM APZ_WID1 T0 INNER JOIN APZ_OWID T1 ON T0.\"AbsEntry\"=T1.\"AbsEntry\"\r\n                        WHERE T0.\"ObjType\" = '7000030' AND T0.\"ObjCode\" = @ObjCode;", CommandType.Text, new SqlParameter("@ObjCode", Function.ParseInt(objId)));
			if (dataTable.IsNotNull() && Function.ParseInt(dataTable.Rows[0]["AbsEntry"]) != 0)
			{
				string text = Function.ToString(dataTable.Rows[0]["Caption"]);
				return new HttpResult(Enums.MessageCode.Error, "Could not remove this item. Linked widget is exists - " + text);
			}
			if (ExecuteNonQuery("UPDATE APZ_DAB1 SET \"MarkAsRemoved\"='Y' WHERE \"Id\"=@Id;", CommandType.Text, new SqlParameter("@Id", Function.ParseInt(objId))) <= 0)
			{
				return new HttpResult(Enums.MessageCode.Error, "No record affected");
			}
			return new HttpResult();
		}
		default:
			return new HttpResult(Enums.MessageCode.Error, "Could not execute function");
		}
	}

	public HttpResult SaveData(DataSet ds, Enums.TransactionType tranType, ref string objId, ref string objNum)
	{
		try
		{
			CreateTempTable(ds.Tables[0], "#APZ_DAB1");
			BulkCopy(ds.Tables[0], "#APZ_DAB1");
			CreateTempTable(ds.Tables[1], "#APZ_DAB3");
			BulkCopy(ds.Tables[1], "#APZ_DAB3");
			string value = Function.ToString(tranType);
			SqlParameter sqlParameter = new SqlParameter("@type", value);
			DataTable dataTable = ExecuteDataTable("APZ_Dashboard_SaveDashboardInForm", CommandType.StoredProcedure, sqlParameter);
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

	public virtual DataTable LoadAll()
	{
		try
		{
			return ExecuteDataTable("SELECT T0.*,T1.Name AS Category FROM  APZ_DAB1 T0 INNER JOIN dbo.APZ_ODAB T1 ON T0.CatId=T1.Id and ISNULL(MarkAsRemoved,'N')<>'Y'", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public HttpResult Save(DataTable dt)
	{
		try
		{
			int num = Function.ParseInt(dt.Rows[0]["Id"]);
			if (Exists(num))
			{
				ExecuteNonQuery("UPDATE APZ_DAB1 SET \"Layout\" = @Layout,\"Name\" = @Name WHERE \"Id\" = @Id;", CommandType.Text, new SqlParameter("@Id", num), new SqlParameter("@Layout", Function.ToString(dt.Rows[0]["Layout"])), new SqlParameter("@Name", Function.ToString(dt.Rows[0]["Name"])));
				return new HttpResult(Enums.MessageCode.Success, "", num);
			}
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string query = "SELECT MAX(\"Id\") FROM APZ_DAB1;";
				int num2 = Function.ParseInt(ExecuteScalar(query, CommandType.Text)) + 1;
				if (num2 <= 0)
				{
					return new HttpResult(Enums.MessageCode.Error, "Could not get next id");
				}
				ExecuteNonQuery("INSERT INTO APZ_DAB1\r\n                                            ( \"Id\" ,\r\n                                              \"Name\" ,\r\n                                              \"CatId\" ,\r\n                                              \"UserSign\" ,\r\n                                              \"Layout\" ,\r\n                                              \"ValidFor\" ,  \r\n                                              \"FormId\" ,\r\n                                              \"ItemId\"\r\n                                            )\r\n\t\t                                    VALUES(@Id,@Name,@CatId,@UserSign,@Layout,1,@FormId,@ItemId);\r\n\r\ndelete from APZ_DAB1 where \"CatId\" in  (select  \"CatId\" from APZ_DAB1 where \"Layout\" is not null group by \"CatId\") and \"Layout\" is null;\r\n", CommandType.Text, new SqlParameter("@Id", Function.ParseInt(num2)), new SqlParameter("@Name", Function.ToString(dt.Rows[0]["Name"])), new SqlParameter("@CatId", Function.ParseInt(dt.Rows[0]["CatId"])), new SqlParameter("@UserSign", Function.ParseInt(dt.Rows[0]["UserSign"])), new SqlParameter("@Layout", Function.ToString(dt.Rows[0]["Layout"])), new SqlParameter("@FormId", Function.ToString(dt.Rows[0]["FormId"])), new SqlParameter("@ItemId", Function.ToString(dt.Rows[0]["ItemId"])));
				dbTransaction.Commit();
				return new HttpResult(Enums.MessageCode.Success, "", num2);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult UpdateLayout(int id, string layout)
	{
		try
		{
			ExecuteNonQuery("UPDATE APZ_DAB1 SET \"Layout\"=@Layout WHERE \"Id\"= @Id;", CommandType.Text, new SqlParameter("@Id", id), new SqlParameter("@Layout", layout));
			return new HttpResult(Enums.MessageCode.Success, "", id);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public virtual DataTable GetById(int id)
	{
		try
		{
			return ExecuteDataTable("SELECT T0.*,T1.Name AS Category FROM  APZ_DAB1 T0 LEFT JOIN dbo.APZ_ODAB T1 ON T0.CatId=T1.Id WHERE T0.Id=@Id AND ISNULL(MarkAsRemoved,'N')<>'Y'", CommandType.Text, new SqlParameter("@Id", id));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetParams(int id)
	{
		try
		{
			return ExecuteDataTable("SELECT * FROM APZ_DAB4 WHERE \"Id\"= @Id;", CommandType.Text, new SqlParameter("@Id", id));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataTable();
		}
	}

	protected virtual string GetNewIdQuery()
	{
		return "SELECT ISNULL((SELECT MAX(Id) FROM APZ_DAB1),0) + 1";
	}

	public HttpResult SaveDataSet(DataSet ds)
	{
		try
		{
			DataTable dataTable = ds.Tables["APZ_DAB1"];
			DataTable dataTable2 = ds.Tables["APZ_DAB4"];
			int num = Function.ParseInt(dataTable.Rows[0]["Id"]);
			if (Exists(num))
			{
				using (DatabaseService.OpenConnection())
				{
					using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
					if (ExecuteNonQuery("UPDATE APZ_DAB1 SET \"Layout\" = @Layout, \"Name\" = @Name, \"SourceType\" = @SourceType, \"UdrCode\" = @UdrCode, \"CatId\" = @CatId, \r\n                                                                    \"FormId\" = @FormId, \"ItemId\" = @ItemId, \"Query\" = @query, \"SqlType\" = @sqlType WHERE \"Id\" = @Id;", CommandType.Text, new SqlParameter("@Id", num), new SqlParameter("@Layout", Function.ToString(dataTable.Rows[0]["Layout"])), new SqlParameter("@Name", Function.ToString(dataTable.Rows[0]["Name"])), new SqlParameter("@SourceType", Function.ToString(dataTable.Rows[0]["SourceType"])), new SqlParameter("@UdrCode", Function.ToString(dataTable.Rows[0]["UdrCode"])), new SqlParameter("@CatId", Function.ParseInt(dataTable.Rows[0]["CatId"])), new SqlParameter("@FormId", Function.ToString(dataTable.Rows[0]["FormId"])), new SqlParameter("@ItemId", Function.ToString(dataTable.Rows[0]["ItemId"])), new SqlParameter("@query", Function.ToString(dataTable.Rows[0]["Query"])), new SqlParameter("@sqlType", Function.ToString(dataTable.Rows[0]["SqlType"]))) <= 0)
					{
						return new HttpResult(Enums.MessageCode.Error, "No row affected");
					}
					int num2 = 0;
					foreach (DataRow row in dataTable2.Rows)
					{
						row["Id"] = num;
						row["LineNum"] = num2;
						num2++;
					}
					BulkToTempTable(dataTable2, "APZ_DAB4", "#APZ_DAB4");
					string query = "delete from APZ_DAB4 where \"Id\" = @Id;\r\n                                          exec 'insert into APZ_DAB4 select * from #APZ_DAB4;';";
					ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@Id", num));
					dbTransaction.Commit();
				}
				return new HttpResult(Enums.MessageCode.Success, "", num);
			}
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction2 = DatabaseService.BeginTransaction();
				string newIdQuery = GetNewIdQuery();
				int num3 = Function.ParseInt(ExecuteScalar(newIdQuery, CommandType.Text));
				if (num3 <= 0)
				{
					return new HttpResult(Enums.MessageCode.Error, "Could not get next id");
				}
				if (ExecuteNonQuery("INSERT INTO APZ_DAB1(\"Id\", \"Name\", \"CatId\", \"UserSign\", \"Layout\", \"ValidFor\", \"FormId\", \"ItemId\", \"SourceType\", \"UdrCode\", \"Query\", \"SqlType\")\r\n\t\t                                     VALUES(@Id, @Name, @CatId, @UserSign, @Layout, 1, @FormId, @ItemId, @SourceType, @UdrCode, @query, @sqlType);\r\n                                             delete from APZ_DAB1 where \"CatId\" in  (select  \"CatId\" from APZ_DAB1 where \"Layout\" is not null group by \"CatId\") and \"Layout\" is null;", CommandType.Text, new SqlParameter("@Id", Function.ParseInt(num3)), new SqlParameter("@Name", Function.ToString(dataTable.Rows[0]["Name"])), new SqlParameter("@CatId", Function.ParseInt(dataTable.Rows[0]["CatId"])), new SqlParameter("@UserSign", Function.ParseInt(dataTable.Rows[0]["UserSign"])), new SqlParameter("@Layout", Function.ToString(dataTable.Rows[0]["Layout"])), new SqlParameter("@FormId", Function.ToString(dataTable.Rows[0]["FormId"])), new SqlParameter("@ItemId", Function.ToString(dataTable.Rows[0]["ItemId"])), new SqlParameter("@SourceType", Function.ToString(dataTable.Rows[0]["SourceType"])), new SqlParameter("@query", Function.ToString(dataTable.Rows[0]["Query"])), new SqlParameter("@sqlType", Function.ToString(dataTable.Rows[0]["SqlType"])), new SqlParameter("@UdrCode", Function.ToString(dataTable.Rows[0]["UdrCode"]))) <= 0)
				{
					return new HttpResult(Enums.MessageCode.Error, "No row affected");
				}
				int num4 = 0;
				foreach (DataRow row2 in dataTable2.Rows)
				{
					row2["Id"] = num3;
					row2["LineNum"] = num4;
					num4++;
				}
				BulkToTempTable(dataTable2, "APZ_DAB4", "#APZ_DAB4");
				string query2 = "delete from APZ_DAB4 where \"Id\" = @Id;\r\n                                      exec 'insert into APZ_DAB4 select * from #APZ_DAB4;';";
				ExecuteNonQuery(query2, CommandType.Text, new SqlParameter("@Id", num3));
				dbTransaction2.Commit();
				return new HttpResult(Enums.MessageCode.Success, "", num3);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult Remove(int dashBoardId)
	{
		try
		{
			return Process(ApzObjectTypes.oDashboardDetail, -1, Enums.TransactionType.D, new DataSet(), dashBoardId.ToString(), "", lockNumbering: false);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private bool Exists(int id)
	{
		return ExecuteDataTable("SELECT 1 FROM APZ_DAB1 WHERE \"Id\"=@Id;", CommandType.Text, new SqlParameter("@Id", id)).IsNotNull();
	}
}
