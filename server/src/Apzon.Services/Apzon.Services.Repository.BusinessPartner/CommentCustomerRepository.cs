using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.BusinessPartner;

namespace Apzon.Services.Repository.BusinessPartner;

public class CommentCustomerRepository : BaseService<DataTable>, ICommentCustomer
{
	private const string temp = "#APZ_D_OCMC";

	public CommentCustomerRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetDataCommentCustomer(int userSign, string relatedEnt, string relatedTyp)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "SELECT *, (select top 1 \"LinkThumbnails\" from APZ_D_UUPIM where \"UserSign\" = @userSign and \"ObjectType\" = @objectType) as \"LinkThumbnails\"\r\n                              FROM APZ_D_OCMC where \"UserSign\" = @userSign and \"RelatedEnt\" = @relatedEnt and \"RelatedTyp\" = @relatedTyp order by \"CreatedTime\" desc;";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@userSign", userSign));
			list.Add(new SqlParameter("@relatedEnt", relatedEnt ?? ""));
			list.Add(new SqlParameter("@relatedTyp", relatedTyp ?? ""));
			list.Add(new SqlParameter("@objectType", Function.ToString(1470000171)));
			return result = ExecuteDataTable(query, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "ShowAddress  FAILED ", ex.Message);
			return result;
		}
	}

	public HttpResult Save(DataTable data)
	{
		try
		{
			Enums.TransactionType transtype = Enums.TransactionType.A;
			return Process(ApzObjectTypes.oCommentCustomer, -1, transtype, data);
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
			return NewSaveData(dsSave, transtype, ref objId, ref objNum);
		}
		return new HttpResult(Enums.MessageCode.Error, "Could not execute function");
	}

	public HttpResult NewSaveData(DataTable data, Enums.TransactionType tranType, ref string objId, ref string objNum)
	{
		try
		{
			CreateTempTable(data, "#APZ_D_OCMC");
			BulkCopy(data, "#APZ_D_OCMC");
			DataTable dataTable = ExecuteDataTable("APZ_D_SaveComment", CommandType.StoredProcedure);
			if (dataTable.Rows.Count > 0)
			{
				if (Function.ToString(dataTable.Rows[0]["Code"]) == "200")
				{
					return new HttpResult
					{
						MessageCode = Enums.MessageCode.Success,
						Message = ""
					};
				}
				return new HttpResult
				{
					MessageCode = Enums.MessageCode.Error,
					Message = Function.ToString(dataTable.Rows[0]["Message"])
				};
			}
			return new HttpResult
			{
				MessageCode = Enums.MessageCode.Error,
				Message = Function.ToString("Save data comment fail")
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult SaveData(CommentCustomerImpl dt, Enums.TransactionType tranType, ref string objId, ref string objNum)
	{
		try
		{
			string text = Function.ToString(tranType);
			DataTable tableStruct = GetTableStruct("APZ_D_OCMC");
			if (!tableStruct.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			int num = 0;
			_ = from DataColumn t in tableStruct.Columns
				select t.ColumnName;
			DataTable dataTable = tableStruct.Clone();
			DataRow dataRow = dataTable.Rows.Add();
			dataRow["Code"] = dt.Code;
			dataRow["Content"] = dt.Content;
			dataRow["RelatedTyp"] = dt.RelatedTyp;
			dataRow["RelatedEnt"] = dt.RelatedEnt;
			dataRow["UserSign"] = dt.UserSign;
			dataRow["UserName"] = dt.UserName;
			dataRow["CreatedTime"] = dt.CreatedTime;
			dataRow["UpdatedTime"] = dt.UpdatedTime;
			if (MaxLogInst() > 0)
			{
				num = MaxLogInst();
			}
			if (text == "A")
			{
				dataRow["LogInstance"] = num + 1;
			}
			else
			{
				dataRow["LogInstance"] = dt.LogInstance;
			}
			CreateTempTable(dataTable, "#APZ_D_OCMC");
			BulkCopy(dataTable, "#APZ_D_OCMC");
			SqlParameter sqlParameter = new SqlParameter("@type", text);
			DataTable dataTable2 = ExecuteDataTable("APZ_D_CommentCustomer_Save", CommandType.StoredProcedure, sqlParameter);
			if (dataTable2 != null && dataTable2.Rows.Count > 0)
			{
				if (string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[0][1])))
				{
					objId = Function.ToString(dataTable2.Rows[0][0]);
					objNum = objId;
					return new HttpResult(Enums.MessageCode.Success, "", objId);
				}
				return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable2.Rows[0][0])}-{Function.ToString(dataTable2.Rows[0][1])}");
			}
			return new HttpResult(Enums.MessageCode.Error, "Data is not valid");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private int MaxLogInst()
	{
		try
		{
			string query = "SELECT Max(LogInstance)\r\n                              FROM APZ_D_OCMC";
			return Function.ParseInt(ExecuteScalar(query, CommandType.Text));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "ShowAddress  FAILED ", ex.Message);
			return 0;
		}
	}

	public HttpResult Delete(DataTable data)
	{
		try
		{
			return Process(ApzObjectTypes.oCommentCustomer, -1, Enums.TransactionType.D, data);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult DeleteData(CommentCustomerImpl entities, ref string objId, ref string objNum)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@code", entities.Code));
			list.Add(new SqlParameter("@userId", entities.UserSign));
			DataTable dataTable = ExecuteDataTable("APZ_D_CommentCustomer_Remove", CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				if (string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0][1])))
				{
					objId = Function.ToString(dataTable.Rows[0][0]);
					objNum = objId;
					return new HttpResult(Enums.MessageCode.Success, "", objId);
				}
				return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable.Rows[0][0])}-{Function.ToString(dataTable.Rows[0][1])}");
			}
			return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable.Rows[0][0])}-{Function.ToString(dataTable.Rows[0][1])}");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
