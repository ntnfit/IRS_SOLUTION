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
using Apzon.Entities.Models.Documents;
using Apzon.Services.Interfaces.Sales;

namespace Apzon.Services.Repository.Sales;

public class SalesBlanketAgreementRepository : BaseService<DataSet>, ISalesBlanketAgreement
{
	private const string TableTempDoc = "#tempDoc";

	private const string TableTempLine = "#tempLine";

	public SalesBlanketAgreementRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetDoc(DocumentSBAImpl condiDoc, string username)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@FromDate", (condiDoc.StartDate <= new DateTime(1753, 1, 1)) ? ((DateTime?)null) : condiDoc.StartDate));
			list.Add(new SqlParameter("@ToDate", (condiDoc.EndDate <= new DateTime(1753, 1, 1)) ? ((DateTime?)null) : condiDoc.EndDate));
			list.Add(new SqlParameter("@BpCode", condiDoc.BpCode));
			list.Add(new SqlParameter("@Status", condiDoc.Status));
			list.Add(new SqlParameter("@BpType", condiDoc.BpType));
			list.Add(new SqlParameter("@UserCode", username));
			list.Add(new SqlParameter("@Number", condiDoc.Number));
			list.Add(new SqlParameter("@AbsID", condiDoc.AbsID));
			return ExecuteDataTable("APZ_SalesBlanketAgreement_GetDoc", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetOwner()
	{
		string query = "APZ_SalesBlanketAgreement_GetOwner";
		try
		{
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetNumerMax(int series)
	{
		string query = "APZ_SalesBlanketAgreement_GetNumerMax";
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Series", series));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetNameCntctCode()
	{
		string query = "APZ_SalesBlanketAgreement_GetNameCntctCode";
		try
		{
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetLine(int AbsId)
	{
		string query = "APZ_SalesBlanketAgreement_GetLine";
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@AgrNo", AbsId);
			return ExecuteDataTable(query, CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetContactPerson(string cardCode)
	{
		string query = "APZ_SalesBlanketAgreement_GetContactPerson";
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@CardCode", cardCode);
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, sqlParameter);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetItemGroup()
	{
		string query = "APZ_SalesBlanketAgreement_GetListItemGroup";
		try
		{
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetPriceList()
	{
		string query = "APZ_SalesBlanketAgreement_GetPriceList";
		try
		{
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	private HttpResult AddNew(DataSet doc)
	{
		if (doc == null || !doc.Tables[0].IsNotNull() || !doc.Tables[1].IsNotNull())
		{
			return new HttpResult(Enums.MessageCode.Error, "Data could not be null");
		}
		CreateTempTable(doc.Tables[0], "#tempDoc");
		BulkCopy(doc.Tables[0], "#tempDoc");
		CreateTempTable(doc.Tables[1], "#tempLine");
		BulkCopy(doc.Tables[1], "#tempLine");
		DataTable dataTable = ExecuteDataTable("APZ_SalesBlanketAgreement_Save", CommandType.StoredProcedure);
		if (dataTable != null && dataTable.Rows.Count > 0)
		{
			if (string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0][0])))
			{
				return new HttpResult(Enums.MessageCode.Success);
			}
			return new HttpResult(Enums.MessageCode.Error, Function.ToString(dataTable.Rows[0][0]));
		}
		return new HttpResult(Enums.MessageCode.Error, "Save error");
	}

	public string DeleteRow(int AgrNo)
	{
		using (DatabaseService.OpenConnection())
		{
			using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
			string query = "APZ_SalesBlanketAgreement_DeleteRow";
			try
			{
				SqlParameter sqlParameter = new SqlParameter("@AgrNo", AgrNo);
				if (ExecuteNonQuery(query, CommandType.StoredProcedure, sqlParameter) > 0)
				{
					dbTransaction.Commit();
					return null;
				}
				dbTransaction.Rollback();
				return "Can't Update!";
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				dbTransaction.Dispose();
				return ex.Message;
			}
		}
	}

	public int GetOwnerByBPCode(string cardCode)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@cardcode", cardCode);
			DataTable dataTable = ExecuteDataTable("APZ_SalesBlanketAgreement_GetOwnerByBPCode", CommandType.StoredProcedure, sqlParameter);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0]["OwnerCode"]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetOwnerByBPCode  FAILED ", ex.Message);
			return 0;
		}
	}

	public string Save(DataSet dt)
	{
		return Process(ApzObjectTypes.oBlanketAgreement, -1, Enums.TransactionType.A, dt).Message;
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet dsSave, ref string objId, ref string objNum)
	{
		return AddNew(dsSave);
	}

	public DataTable GetBlanketAgreementDetails(int AgrNo, int AgrLnNum)
	{
		string query = "APZ_SalesBlanketAgreement_GetBlanketAgreementDetails";
		try
		{
			return ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@AgrNo", AgrNo), new SqlParameter("@AgrLnNum", AgrLnNum));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetBlanketAgreementDetails  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable CheckExistActivity(int AgrNo, int AgrLnNum, int AgrEfctNum)
	{
		string query = "APZ_SalesBlanketAgreement_CheckExistActivity";
		try
		{
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@AgrNo", AgrNo), new SqlParameter("@AgrLnNum", AgrLnNum), new SqlParameter("@AgrEfctNum", AgrEfctNum));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "CheckExistActivity  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetActivitiesByActivityID(string lstClgCodes)
	{
		string query = "APZ_SalesBlanketAgreement_GetActivitiesByActivityID";
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@ClgCodes", lstClgCodes);
			return ExecuteDataTable(query, CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetActivitiesByActivityID  FAILED ", ex.Message);
			return new DataTable();
		}
	}
}
