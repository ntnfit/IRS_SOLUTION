using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Promotion;

namespace Apzon.Services.Repository.Promotion;

internal class PromotionRepository : BaseService<DataSet>, IPromotion
{
	private readonly string _connection;

	private const string TempTableDoc = "#APZ_D_OPMO";

	private const string TempTableCondition = "#APZ_D_PMO1";

	private const string TempTableBenefit = "#APZ_D_PMO2";

	private const string TableDoc = "APZ_D_OPMO";

	private const string TableCondition = "APZ_D_PMO1";

	private const string TableBenefit = "APZ_D_PMO2";

	public PromotionRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataSet GetSyncData()
	{
		try
		{
			return ExecuteData("APZ_D_Promotion_SyncData", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataTable GetListPromotions(int promotionType, string cardCode, string promotionCode, string groupCode, string userCode)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@promotionType", promotionType),
				new SqlParameter("@cardCode", cardCode),
				new SqlParameter("@promotionCode", promotionCode),
				new SqlParameter("@groupCode", groupCode),
				new SqlParameter("@userCode", userCode)
			};
			return ExecuteDataTable("APZ_D_Promotion_GetList", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetHeader(string usercode, string code)
	{
		try
		{
			string query = "SELECT * FROM APZ_D_OPMO WHERE \"Code\"=@Code;";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@Code", code ?? ""));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataTable();
		}
	}

	public DataSet GetDetailByCode(string code, string groupCode, string userCode)
	{
		try
		{
			string query = "APZ_D_Promotion_GetDetail";
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@code", code ?? ""),
				new SqlParameter("@groupCode", groupCode ?? ""),
				new SqlParameter("@userCode", userCode ?? "")
			};
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataSet();
		}
	}

	public DataTable LoadCondition(string code)
	{
		try
		{
			string query = "APZ_D_Promotion_LoadCondition";
			return ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@code", code ?? ""));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataTable();
		}
	}

	public DataTable LoadBenefit(string code)
	{
		try
		{
			string query = "APZ_D_Promotion_LoadBenefit";
			return ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@code", code ?? ""));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataTable();
		}
	}

	public HttpResult Save(DataSet ds, string usercode)
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("UserCode", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow[0] = usercode;
			dataTable.Rows.Add(dataRow);
			ds.Tables.Add(dataTable.Copy());
			string text = Function.ToString(ds.Tables[0].Rows[0]["OldCode"]);
			if (string.IsNullOrEmpty(text))
			{
				return Process(ApzObjectTypes.oPromotion, -1, Enums.TransactionType.A, ds);
			}
			return Process(ApzObjectTypes.oPromotion, -1, Enums.TransactionType.U, ds, text, text, lockNumbering: false);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet ds, ref string objId, ref string objNum)
	{
		try
		{
			string value = Function.ToString(ds.Tables[3].Rows[0][0]);
			string text = Function.ToString(ds.Tables[0].Rows[0]["OldCode"]);
			switch (transtype)
			{
			case Enums.TransactionType.A:
			{
				BulkToTempTable(ds.Tables[0], "APZ_D_OPMO", "#APZ_D_OPMO");
				BulkToTempTable(ds.Tables[1], "APZ_D_PMO1", "#APZ_D_PMO1");
				BulkToTempTable(ds.Tables[2], "APZ_D_PMO2", "#APZ_D_PMO2");
				DataTable dataTable2 = ExecuteDataTable("apz_d_promotion_process", CommandType.StoredProcedure, new SqlParameter("@UserCode", value), new SqlParameter("@OldCode", ""), new SqlParameter("@transtype", "A"));
				if (dataTable2.IsNotNull())
				{
					int num2 = Function.ParseInt(dataTable2.Rows[0][0]);
					if (num2 > 0)
					{
						string text3 = Function.ToString(dataTable2.Rows[0][1]);
						throw new Exception(num2 + "-" + text3);
					}
				}
				return new HttpResult();
			}
			case Enums.TransactionType.U:
			{
				BulkToTempTable(ds.Tables[0], "APZ_D_OPMO", "#APZ_D_OPMO");
				BulkToTempTable(ds.Tables[1], "APZ_D_PMO1", "#APZ_D_PMO1");
				BulkToTempTable(ds.Tables[2], "APZ_D_PMO2", "#APZ_D_PMO2");
				DataTable dataTable = ExecuteDataTable("apz_d_promotion_process", CommandType.StoredProcedure, new SqlParameter("@UserCode", value), new SqlParameter("@OldCode", text ?? ""), new SqlParameter("@transtype", "U"));
				if (dataTable.IsNotNull())
				{
					int num = Function.ParseInt(dataTable.Rows[0][0]);
					if (num > 0)
					{
						string text2 = Function.ToString(dataTable.Rows[0][1]);
						throw new Exception(num + "-" + text2);
					}
				}
				return new HttpResult();
			}
			default:
				return new HttpResult(Enums.MessageCode.Error, "Unknown error");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataTable CheckPromotion(DataTable input)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				CreateTempTable(input, "#INPUT");
				BulkCopy(input, "#INPUT");
				int num = ((input.Rows.Count > 0) ? Function.ParseInt(input.Rows[0]["DocEntry"]) : (-1));
				return ExecuteDataTable("USP_PROMOTION_ENGINE_ALL", CommandType.StoredProcedure, new SqlParameter("@ProgramType", 1), new SqlParameter("@DocID", (num == 0) ? (-1) : num), new SqlParameter("@CardCode", (input.Rows.Count > 0) ? Function.ToString(input.Rows[0]["CardCode"]) : ""));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataTable();
		}
	}
}
