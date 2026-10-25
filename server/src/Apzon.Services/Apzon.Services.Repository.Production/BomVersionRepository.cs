using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Production;

namespace Apzon.Services.Repository.Production;

public class BomVersionRepository : BaseService<DataSet>, IBomVersion
{
	private const string TableDoc = "#tempdoc";

	private const string TableLine = "#templine";

	public BomVersionRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetDataDocBomVersion(string code, string version)
	{
		try
		{
			return ExecuteDataTable("APZ_BomVersion_GetDataDoc", CommandType.StoredProcedure, new SqlParameter("@Code", code), new SqlParameter("@Version", version));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDataLineBomVersion(int id)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@Father", id);
			return ExecuteDataTable("APZ_BomVersion_GetDataLine", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetColumnsDocBomVersion()
	{
		try
		{
			return ExecuteDataTable("APZ_BomVersion_GetColumnsDoc", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetColumnsLineBomVersion()
	{
		try
		{
			return ExecuteDataTable("APZ_BomVersion_GetColumnsLine", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataSet GetDataBomSap(string productionNo)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@Code", productionNo);
			return ExecuteData("APZ_BomVersion_GetDataBomSap", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	private bool CheckExistBomStandar(string code)
	{
		if (ExecuteDataTable("Select 1 from OITT where Code = @code", CommandType.Text, new SqlParameter("@code", code)).IsNotNull())
		{
			return true;
		}
		return false;
	}

	public HttpResult SaveBomStandar(DataSet dsBomver)
	{
		try
		{
			if (dsBomver == null || dsBomver.Tables.Count < 2 || dsBomver.Tables[0].Rows.Count == 0 || dsBomver.Tables[1].Rows.Count == 0)
			{
				return new HttpResult(Enums.MessageCode.Error, "Data could not be null!");
			}
			if (CheckExistBomStandar(Function.ToString(dsBomver.Tables[0].Rows[0]["Code"])))
			{
				return new HttpResult();
			}
			string docTableByObjectType = GetDocTableByObjectType(ApzObjectTypes.oProductTrees);
			string docLineTableByObjectType = GetDocLineTableByObjectType(ApzObjectTypes.oProductTrees);
			DataTable tableStruct = GetTableStruct(docTableByObjectType);
			DataTable tableStruct2 = GetTableStruct(docLineTableByObjectType);
			if (!tableStruct.IsNotNull() || !tableStruct2.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			string[] columnNames = (from DataColumn t in tableStruct.Columns
				select t.ColumnName).ToArray();
			for (int num = 0; num < tableStruct.Columns.Count; num++)
			{
				if (!dsBomver.Tables[0].Columns.Contains(tableStruct.Columns[num].ColumnName))
				{
					dsBomver.Tables[0].Columns.Add(new DataColumn(tableStruct.Columns[num].ColumnName, tableStruct.Columns[num].DataType));
				}
			}
			dsBomver.Tables[1].Columns.Remove("Father");
			string[] columnNames2 = (from DataColumn t in tableStruct2.Columns
				select t.ColumnName).ToArray();
			for (int num2 = 0; num2 < tableStruct2.Columns.Count; num2++)
			{
				if (!dsBomver.Tables[1].Columns.Contains(tableStruct2.Columns[num2].ColumnName))
				{
					dsBomver.Tables[1].Columns.Add(new DataColumn(tableStruct2.Columns[num2].ColumnName, tableStruct2.Columns[num2].DataType));
				}
			}
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(dsBomver.Tables[0].DefaultView.ToTable(distinct: false, columnNames).Copy());
			dataSet.Tables.Add(dsBomver.Tables[1].DefaultView.ToTable(distinct: false, columnNames2).Copy());
			return new HttpResult(Enums.MessageCode.Success, "", GetUnitOfWork().BillOfMasterials.SaveDocNoSAP(dataSet, Enums.TransactionType.A, ""));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult Save(DataSet dtset, Enums.TransactionType tranType, string objId, bool loginSap)
	{
		try
		{
			if (tranType == Enums.TransactionType.A && !loginSap)
			{
				objId = "1";
			}
			return Process(ApzObjectTypes.oBomVersion, -1, tranType, dtset, objId);
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
			return SaveData(dsSave, ref transtype, ref objId, ref objNum);
		case Enums.TransactionType.D:
			return Delete(ref transtype, ref objId, ref objNum);
		default:
			return new HttpResult(Enums.MessageCode.Error, "Could not execute function");
		}
	}

	private HttpResult Delete(ref Enums.TransactionType transtype, ref string objId, ref string objNum)
	{
		try
		{
			transtype = Enums.TransactionType.D;
			objNum = objId;
			SqlParameter sqlParameter = new SqlParameter("@id", Function.ParseInt(objId));
			if (ExecuteNonQuery("APZ_BomVersion_Delete", CommandType.StoredProcedure, sqlParameter) <= 0)
			{
				return new HttpResult(Enums.MessageCode.Error, "Delete error");
			}
			return new HttpResult(Enums.MessageCode.Success, "", objId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult SaveData(DataSet dtset, ref Enums.TransactionType trantype, ref string objid, ref string objNum)
	{
		try
		{
			if (dtset == null || dtset.Tables.Count < 2 || dtset.Tables[0].Rows.Count == 0 || dtset.Tables[1].Rows.Count == 0)
			{
				return new HttpResult(Enums.MessageCode.Error, "Data could not be null!");
			}
			HttpResult httpResult = new HttpResult();
			if (!string.IsNullOrEmpty(objid))
			{
				httpResult = SaveBomStandar(dtset.Copy());
			}
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				return httpResult;
			}
			CreateTempTable(dtset.Tables[0], "#tempdoc");
			CreateTempTable(dtset.Tables[1], "#templine");
			BulkCopy(dtset.Tables[0], "#tempdoc");
			BulkCopy(dtset.Tables[1], "#templine");
			DataTable dataTable = ExecuteDataTable("APZ_BomVersion_Save", CommandType.StoredProcedure, new SqlParameter("@tranType", trantype.ToString()));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				if (Function.ParseInt(dataTable.Rows[0][0]) == 0)
				{
					return new HttpResult();
				}
				return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable.Rows[0][0])}-{Function.ToString(dataTable.Rows[0][1])}");
			}
			return new HttpResult(Enums.MessageCode.Error, "Save error!");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public string GetNewVersionBom(string itemcode)
	{
		try
		{
			if (string.IsNullOrEmpty(itemcode))
			{
				return "";
			}
			SqlParameter sqlParameter = new SqlParameter("@code", (!string.IsNullOrEmpty(itemcode)) ? itemcode : "");
			DataTable dataTable = ExecuteDataTable("Select top 1 \"Version\" from APZ_OITT where \"Code\" = @code order by \"Version\" DESC;", CommandType.Text, sqlParameter);
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				return itemcode + "-0001";
			}
			string text = Function.ToString(dataTable.Rows[0]["Version"]);
			int num = Function.ParseInt(Function.ToString(text.Substring(itemcode.Length + 1))) + 1;
			string text2 = "00000" + (Function.ParseInt(Function.ToString(text.Substring(itemcode.Length + 1))) + 1);
			string text3 = ((num > 1000) ? Function.ToString(num) : text2.Substring(text2.Length - 4));
			return itemcode + "-" + text3;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "";
		}
	}

	public DataTable LoadDataNextLast(Enums.NavigateType type, string code)
	{
		try
		{
			return ExecuteDataTable("APZ_BomVersion_LoadDataNextLast", CommandType.StoredProcedure, new SqlParameter("@type", (int)type), new SqlParameter("@code", code));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public bool CheckBomSap(string code)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@code", code);
			DataTable dataTable = ExecuteDataTable("select \"Code\" from OITT where \"Code\" = @code;", CommandType.Text, sqlParameter);
			if (dataTable == null || dataTable.Rows.Count == 0)
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
