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

internal class PromotionRegisterRepository : BaseService<DataSet>, IPromotionRegister
{
	private readonly string _connection;

	public PromotionRegisterRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetAttachment(DataTable dt, string usercode)
	{
		try
		{
			string query = "APZ_D_PromotionRegister_GetAttachment";
			if (dt.IsNotNull())
			{
				DataRow dataRow = dt.Rows[0];
				List<SqlParameter> list = new List<SqlParameter>
				{
					new SqlParameter
					{
						ParameterName = "@type",
						Value = Function.ToString(dataRow["type"])
					},
					new SqlParameter
					{
						ParameterName = "@promotionCode",
						Value = Function.ToString(dataRow["code"])
					},
					new SqlParameter
					{
						ParameterName = "@groupCode",
						Value = Function.ToString(dataRow["groupCode"])
					},
					new SqlParameter
					{
						ParameterName = "@cardCode",
						Value = Function.ToString(dataRow["cardCode"])
					},
					new SqlParameter
					{
						ParameterName = "@usercode",
						Value = Function.ToString(usercode)
					}
				};
				return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			}
			return null;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw ex;
		}
	}

	public HttpResult Save(DataSet ds, string usercode)
	{
		try
		{
			if (ds.IsNotNull())
			{
				DataTable dataTable = new DataTable();
				dataTable.Columns.Add("USERID", typeof(string));
				string query = "SELECT USERID FROM OUSR WHERE USER_CODE=@CODE;";
				DataTable dataTable2 = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@CODE", usercode));
				if (dataTable2.IsNotNull())
				{
					int num = Function.ParseInt(dataTable2.Rows[0][0]);
					DataRow dataRow = dataTable.NewRow();
					dataRow[0] = num;
					dataTable.Rows.Add(dataRow);
					ds.Tables.Add(dataTable.Copy());
					return Process(ApzObjectTypes.oPromotionRegister, -1, Enums.TransactionType.A, ds);
				}
				return new HttpResult(Enums.MessageCode.Error, "Người dùng không tồn tại");
			}
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Data empty");
			return new HttpResult(Enums.MessageCode.Error, "Dữ liệu không hợp lệ");
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
			switch (transtype)
			{
			case Enums.TransactionType.A:
			case Enums.TransactionType.U:
			{
				DataTable dataTable = ds.Tables[0];
				DataTable dataTable2 = ds.Tables[1];
				int num = Function.ParseInt(ds.Tables[2].Rows[0][0]);
				string query = "APZ_D_Promotion_Register";
				CreateTempTable(dataTable.Copy(), "#temp");
				BulkCopy(dataTable.Copy(), "#temp");
				ExecuteNonQuery(query, CommandType.StoredProcedure, new SqlParameter("@UserSign", num));
				if (dataTable2.IsNotNull())
				{
					CreateTempTable(dataTable2.Copy(), "#APZ_D_PMO5");
					BulkCopy(dataTable2.Copy(), "#APZ_D_PMO5");
					string query2 = "APZ_D_Promotion_SaveAttachment";
					ExecuteData(query2, CommandType.StoredProcedure);
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
}
