using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.DMS;

namespace Apzon.Services.Repository.DMS;

internal class DMSSettingUDFRepository : BaseService<DataTable>, IDMSSettingUDF
{
	public DMSSettingUDFRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataSet GetListUdf(string tableName, int menuId)
	{
		try
		{
			string value = "";
			if (tableName.ToUpper().Contains("APZ_"))
			{
				value = "APZ";
			}
			DataSet dataSet = new DataSet();
			string query = "APZ_D_SettingUDF_GetData";
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter
				{
					ParameterName = "@menuID",
					Value = menuId
				},
				new SqlParameter
				{
					ParameterName = "@tableName",
					Value = tableName
				},
				new SqlParameter
				{
					ParameterName = "@type",
					Value = value
				}
			};
			dataSet = ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
			if (dataSet.Tables.Count == 2)
			{
				dataSet.Tables[0].TableName = "ListUDF";
				dataSet.Tables[1].TableName = "ListCFL";
			}
			return dataSet;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public HttpResult GetData(string tableName, int menuId)
	{
		try
		{
			DataSet listUdf = GetListUdf(tableName, menuId);
			return new HttpResult
			{
				Message = "",
				MessageCode = Enums.MessageCode.Success,
				Content = listUdf
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult DMSGetDataUDF(int menuId, string tableName, string codeField, string code)
	{
		try
		{
			DataTable dataTable = GetListUdf(tableName, menuId).Tables["ListUDF"].Copy();
			string text = "";
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				text = text + "\"U_" + Function.ToString(dataTable.Rows[i]["AliasID"]) + "\",";
			}
			text = text.Remove(text.Length - 1);
			DataTable dataTable2 = new DataTable();
			if (codeField.Contains(";") && code.Contains(";"))
			{
				string[] array = codeField.Split(';');
				string[] array2 = code.Split(';');
				int num = ((array.Length <= array2.Length) ? array.Length : array2.Length);
				string text2 = " where ";
				List<SqlParameter> list = new List<SqlParameter>();
				for (int j = 0; j < num; j++)
				{
					text2 = ((j != 0) ? (text2 + "and \"" + array[j] + "\" = @code" + j + " ") : (text2 + "\"" + array[j] + "\" = @code" + j + " "));
					list.Add(new SqlParameter
					{
						ParameterName = "@code" + j,
						Value = array2[j]
					});
				}
				string query = "select " + text + " from " + tableName + text2 + ";";
				dataTable2 = ExecuteDataTable(query, CommandType.Text, list.ToArray());
			}
			else
			{
				string query2 = "select " + text + " from " + tableName + " where \"" + codeField + "\"=@code;";
				dataTable2 = ExecuteDataTable(query2, CommandType.Text, new SqlParameter
				{
					ParameterName = "@code",
					Value = code
				});
			}
			return new HttpResult
			{
				Message = "",
				MessageCode = Enums.MessageCode.Success,
				Content = dataTable2
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult Save(DataTable dt, string action)
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
			return Process(ApzObjectTypes.oSettingReport, -1, transtype, dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable dt, ref string objId, ref string objNum)
	{
		Enums.TransactionType transactionType = transtype;
		if ((uint)transactionType <= 1u)
		{
			return SaveData(dt, transtype, ref objId, ref objNum);
		}
		return new HttpResult(Enums.MessageCode.Error, "Could not execute function");
	}

	public HttpResult SaveData(DataTable dt, Enums.TransactionType tranType, ref string objId, ref string objNum)
	{
		try
		{
			CreateTempTable(dt.Copy(), "#temp");
			BulkCopy(dt.Copy(), "#temp");
			Function.ToString(tranType);
			DataTable dataTable = ExecuteDataTable("APZ_D_SettingUDF_SaveData", CommandType.StoredProcedure);
			if (dataTable.IsNotNull())
			{
				string text = Function.ToString(dataTable.Rows[0]["MessageCode"]);
				return new HttpResult(message: Function.ToString(dataTable.Rows[0]["Message"]), messageCode: (text == "200") ? Enums.MessageCode.Success : Enums.MessageCode.Error);
			}
			return new HttpResult(Enums.MessageCode.Error, "Data is not valid");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
