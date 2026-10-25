using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.DMS;
using Apzon.Entities.Models.MasterData;
using Apzon.Entities.Models.MobileApp;
using Apzon.Services.Interfaces.BusinessPartner;

namespace Apzon.Services.Repository.BusinessPartner;

public class BusinessMasterDataRepository(IDatabaseService databaseService) : BaseService<DataSet>(databaseService), IBusinessMasterDatas
{
	private string TempBPTableName = "#APZ_BP_DOC";

	private string TempAttFileTableName = "#APZ_BP_ATC";

	private string TempAddressTableName = "#APZ_BP_LINE";

	private string TempCntPrsnTableName = "#APZ_BP_CNTC";

	private string TempBankAcctTableName = "#APZ_BP_BNKACT";

	private string TempPmntDateTableName = "#APZ_BP_PMNTDate";

	private string TempOtherDateTableName = "#APZ_BP_OTHER";

	private string TempCRD8TableName = "#APZ_BP_CRD8";

	public DataTable Table()
	{
		DataTable result = new DataTable();
		try
		{
			SqlParameter[] parameters = new SqlParameter[1]
			{
				new SqlParameter("@ListNum", -2)
			};
			result = ExecuteData("SP_APZ_CFL_ITMPRICE_REQUEST", CommandType.StoredProcedure, parameters).Tables[0];
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Read data OCRD  FAILED ", ex.Message);
		}
		return result;
	}

	public DataSet GetDataQuickBPPager(int pageNumber, int pageSize, DataTable searchData)
	{
		try
		{
			string text = "";
			List<SqlParameter> list = new List<SqlParameter>();
			new DataSet();
			string value = Function.ToString(searchData.Rows[0]["BPType"]);
			string value2 = Function.ToString(searchData.Rows[0]["BPName"]);
			string value3 = Function.ToString(searchData.Rows[0]["BPCode"]);
			string value4 = Function.ToString(searchData.Rows[0]["BPGroup"]);
			string value5 = Function.ToString(searchData.Rows[0]["Currency"]);
			text = "APZ_QuickBPMasterData_GetData";
			list.Add(new SqlParameter("@lowLimit", pageSize * pageNumber - pageSize));
			list.Add(new SqlParameter("@upLimit", pageSize * pageNumber));
			list.Add(new SqlParameter("@bpCode", value3));
			list.Add(new SqlParameter("@bpType", value));
			list.Add(new SqlParameter("@bpGroup", value4));
			list.Add(new SqlParameter("@currency", value5));
			list.Add(new SqlParameter("@bpName", value2));
			return ExecuteData(text, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataSet();
		}
	}

	public object GetNumberOfRecordQuickBP()
	{
		try
		{
			string query = "select count(*) from (select * from OCRD) t";
			return ExecuteScalar(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return null;
		}
	}

	public DataTable GetDataQuickBPMasterData(string cardCode)
	{
		try
		{
			DataTable dataTable = new DataTable();
			string text = "";
			if (string.IsNullOrEmpty(cardCode))
			{
				text = "SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY CardCode) row_num, * FROM OCRD) t";
				dataTable = ExecuteDataTable(text, CommandType.Text);
			}
			else
			{
				text = "SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY CardCode) row_num, * FROM OCRD where CardCode=@cardCode) t";
				dataTable = ExecuteDataTable(text, CommandType.Text, new SqlParameter("@cardCode", cardCode));
				dataTable.Clone();
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetBPByCardCode(string cardCode, string userCode, int bplId)
	{
		try
		{
			string query = "APZ_BusinessMasterData_GetDoc";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@CardCode", Function.ToString(cardCode)));
			list.Add(new SqlParameter("@UserCode", userCode));
			list.Add(new SqlParameter("@BPLId", bplId));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetBPByCardCode  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetBPByCardCodeWithAddress(string cardCode, string userCode, int bplId)
	{
		try
		{
			string query = "APZ_BusinessMasterData_GetDocWithAddress";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@CardCode", Function.ToString(cardCode)));
			list.Add(new SqlParameter("@UserCode", userCode));
			list.Add(new SqlParameter("@BPLId", bplId));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetBPByCardCode  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable CheckExistCardCode(string cardCode)
	{
		try
		{
			string query = "Select \"CardCode\" from OCRD where \"CardCode\"=@CardCode;";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@CardCode", Function.ToString(cardCode)));
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

	public ItemsImpl GetById(int id)
	{
		throw new NotImplementedException();
	}

	public List<ItemsImpl> Cfl()
	{
		List<ItemsImpl> list = new List<ItemsImpl>();
		try
		{
			DataTable dataTable = ExecuteDataTable("SELECT \"ItemCode\",\"ItemName\",\"DfltWH\",\"OnHand\" FROM OCRD;", CommandType.Text);
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				list.Add(new ItemsImpl
				{
					ItemCode = Function.ToString(dataTable.Rows[i]["ItemCode"]),
					ItemName = Function.ToString(dataTable.Rows[i]["ItemName"]),
					DfltWH = Function.ToString(dataTable.Rows[i]["DfltWH"]),
					OnHand = Function.ParseDouble(dataTable.Rows[i]["OnHand"])
				});
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return list;
	}

	public DataTable ShowCardGroup(string groupType)
	{
		DataTable dataTable = new DataTable();
		try
		{
			string query = "SELECT  \"GroupCode\",\"GroupName\",\"GroupType\"\r\n\t\t\t\t\t\t\t   FROM  OCRG where \"Locked\"='N'  and \"GroupType\"='" + groupType + "' order by \"GroupName\"; ";
			if (groupType == "A")
			{
				query = "SELECT  \"GroupCode\",\"GroupName\",\"GroupType\"\r\n\t\t\t\t\t\t\t   FROM  OCRG where \"Locked\"='N' order by \"GroupName\";";
				dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			}
			else
			{
				dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
				if (dataTable != null && dataTable.Columns.Count > 0)
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow["GroupCode"] = Function.ParseShort(-1);
					dataRow["GroupName"] = "Define New";
					dataRow["GroupType"] = "";
					dataTable.Rows.Add(dataRow);
					dataTable.AcceptChanges();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return dataTable;
	}

	public DataTable ShowCreditCardType()
	{
		DataTable result = new DataTable();
		try
		{
			string query = "SELECT * FROM  OCRC where \"Locked\"='N';";
			return result = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return result;
		}
	}

	public DataTable LoadUDFData(string cardCode)
	{
		try
		{
			List<string> udfField = GetUdfField();
			if (udfField.Count <= 0)
			{
				return new DataTable();
			}
			string text = "";
			foreach (string item in udfField)
			{
				text = text + "\"" + item + "\",";
			}
			text = text.Remove(text.Length - 1);
			(new SqlParameter[1])[0] = new SqlParameter("@cardCode", cardCode);
			string query = "select " + text + " from ocrd where \"CardCode\"=@cardCode;";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	private List<string> GetUdfField()
	{
		List<string> list = new List<string>();
		try
		{
			string query = "select \"FieldName\" from \"tblUFieldTable\" where \"TableName\"='OCRD';";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text);
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				list.Add(dataTable.Rows[i][0].ToString());
			}
			return list;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return list;
		}
	}

	public DataTable Search(DataTable source, string userCode)
	{
		DataTable result = new DataTable();
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			string text = "";
			DataRow dataRow = source.Rows[0];
			for (int i = 0; i < source.Columns.Count; i++)
			{
				string columnName = source.Columns[i].ColumnName;
				object obj = dataRow[columnName];
				if (obj == null || string.IsNullOrEmpty(Function.ToString(obj)))
				{
					continue;
				}
				Type dataType = source.Columns[i].DataType;
				if (dataType == typeof(DateTime))
				{
					if (Function.ParseDateTime(obj).HasValue)
					{
						text += string.Format(" and a.\"{0}\" = @{0}", columnName);
						list.Add(new SqlParameter($"@{columnName}", obj));
					}
				}
				else if (dataType == typeof(bool))
				{
					if (Function.ParseBool(obj))
					{
						text += string.Format(" and a.\"{0}\" = @{0}", columnName);
						list.Add(new SqlParameter($"@{columnName}", obj));
					}
				}
				else if (dataType == typeof(decimal) || dataType == typeof(double) || dataType == typeof(float))
				{
					text += string.Format(" and a.\"{0}\" = @{0}", columnName);
					list.Add(new SqlParameter($"@{columnName}", obj));
				}
				else if (dataType == typeof(short) || dataType == typeof(int) || dataType == typeof(long) || dataType == typeof(long) || dataType == typeof(short))
				{
					text += string.Format(" and a.\"{0}\" = @{0}", columnName);
					list.Add(new SqlParameter($"@{columnName}", obj));
				}
				else
				{
					text += GetSearchStringCompare(columnName);
					list.Add(new SqlParameter($"@{columnName}", obj));
				}
			}
			list.Add(new SqlParameter("@UserCode", userCode));
			result = ExecuteDataTable(GetSearchString(text), CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get data Return Search - Doc ", ex.Message);
		}
		return result;
	}

	protected virtual string GetSearchStringCompare(string colName)
	{
		return string.Format(" and a.\"{0}\" LIKE N'%'+@{0}+'%' ", colName);
	}

	protected virtual string GetSearchString(string condition)
	{
		return $"declare @dofilter char(1)\r\nset @dofilter =(select isnull((select DoFilter from cinf),'N'))\r\n\r\nDECLARE @DoMngMth CHAR(1) =(SELECT ISNULL((SELECT  DoMngMth FROM oadm ),''))\r\nselect \r\n            *\r\n            ,(select TrnspName from OSHP where TrnspCode = a.ShipType) as \"Shipping Type Name\"\r\n            ,(select PymntGroup from OCTG where GroupNum = a.GroupNum) as \"Payment Terms\"\r\n\t\t\tfrom ocrd a\r\n\t\t\twhere 1 = 1 \r\n\t\t\t{condition}\r\n\t\t\tAND (@dofilter='N' OR (@dofilter='Y' AND @DoMngMth NOT IN ('A','B')) OR (@dofilter='Y' AND @DoMngMth IN ('A','B') \r\n                AND\t(ISNULL(A.OwnerCode,-1)=-1 OR (A.OwnerCode IN (select distinct x.OwnerId from  apz_func_OwnerOverShip(@UserCode,2) x)))))";
	}

	public DataTable GetADoc(string cardCode, string userName)
	{
		DataTable result = new DataTable();
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@CardCode", cardCode));
			list.Add(new SqlParameter("@UserName", userName ?? ""));
			string query = "apz_BusinessMasterData_GetADoc";
			result = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable GetBPTableStructure(string table)
	{
		DataTable result = new DataTable();
		try
		{
			if (table.Any(char.IsWhiteSpace))
			{
				throw new Exception("Invalid table name");
			}
			string query = "Select top 0 * From " + table + ";";
			result = ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	private bool ExistsTable(string table)
	{
		bool result = false;
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Table", table));
			DataTable dataTable = ExecuteDataTable(GetExistsTableQuery(), CommandType.Text, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				result = Function.ToString(dataTable.Rows[0][0]).Trim().ToUpper() == "TRUE";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	protected virtual string GetExistsTableQuery()
	{
		return "DECLARE @TableName VARCHAR(100) = @Table;\r\n                            DECLARE @Exists BIT = ( SELECT CAST( COUNT(1) AS BIT ) FROM sys.tables WHERE name = @TableName AND type = 'U' );\r\n                            select @Exists";
	}

	public string SaveDocDataSet(DataSet ds, string action)
	{
		try
		{
			Enums.TransactionType transactionType = Enums.TransactionType.A;
			switch (action)
			{
			case "A":
				transactionType = Enums.TransactionType.A;
				break;
			case "U":
				transactionType = Enums.TransactionType.U;
				break;
			case "D":
				transactionType = Enums.TransactionType.D;
				break;
			default:
				return "BP Master data's Repository: Wrong action. Should be: 'A', 'U' OR 'D'";
			}
			return Process(ApzObjectTypes.oBusinessPartners, -1, transactionType, ds).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string DeleteDoc(string cardCode)
	{
		try
		{
			return Process(ApzObjectTypes.oBusinessPartners, -1, Enums.TransactionType.D, new DataSet(), cardCode).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet ds, ref string objId, ref string objNum)
	{
		HttpResult ret = new HttpResult();
		switch (transtype)
		{
		case Enums.TransactionType.A:
			ProcessBp(ref transtype, ds, ref objId, ref objNum, ref ret);
			break;
		case Enums.TransactionType.U:
			ProcessBp(ref transtype, ds, ref objId, ref objNum, ref ret);
			break;
		case Enums.TransactionType.D:
			ProcessDeleteBp(ref ret, ref objId, ref objNum);
			break;
		}
		return ret;
	}

	private void ProcessBp(ref Enums.TransactionType transtype, DataSet ds, ref string objId, ref string objNum, ref HttpResult ret)
	{
		try
		{
			if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
			{
				ret.Message = "No data to save";
			}
			else
			{
				string text = ((transtype == Enums.TransactionType.A) ? "A" : "U");
				if (text == "A" && ds.Tables.Contains("OtherTable") && ds.Tables["OtherTable"].Rows.Count > 0 && !string.IsNullOrWhiteSpace(Function.ToString(ds.Tables["OtherTable"].Rows[0]["OldCode"])))
				{
					text = "U";
				}
				IUnitOfWork unitOfWork = GetUnitOfWork();
				string overSizeColumns = unitOfWork.Commons.GetOverSizeColumns(ds.Tables["BP"], "OCRD");
				if (overSizeColumns != "")
				{
					ret.Message = overSizeColumns;
				}
				else
				{
					DataTable dataTable = unitOfWork.Commons.ProcessDefaultColumnValue(ds.Tables["BP"], "OCRD");
					if (string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["Currency"])))
					{
						dataTable.Rows[0]["Currency"] = "##";
					}
					CreateTempTable(dataTable, TempBPTableName);
					BulkCopy(dataTable, TempBPTableName);
					DataTable dt = unitOfWork.Commons.ProcessDefaultColumnValue(ds.Tables["AttFile"], "ATC1");
					CreateTempTable(dt, TempAttFileTableName);
					BulkCopy(dt, TempAttFileTableName);
					DataTable dt2 = unitOfWork.Commons.ProcessDefaultColumnValue(ds.Tables["Address"], "CRD1");
					CreateTempTable(dt2, TempAddressTableName);
					BulkCopy(dt2, TempAddressTableName);
					DataTable dt3 = unitOfWork.Commons.ProcessDefaultColumnValue(ds.Tables["CntPrsn"], "OCPR");
					CreateTempTable(dt3, TempCntPrsnTableName);
					BulkCopy(dt3, TempCntPrsnTableName);
					DataTable dt4 = unitOfWork.Commons.ProcessDefaultColumnValue(ds.Tables["BankAcct"], "OCRB");
					CreateTempTable(dt4, TempBankAcctTableName);
					BulkCopy(dt4, TempBankAcctTableName);
					DataTable dt5 = unitOfWork.Commons.ProcessDefaultColumnValue(ds.Tables["PmntDate"], "CRD5");
					CreateTempTable(dt5, TempPmntDateTableName);
					BulkCopy(dt5, TempPmntDateTableName);
					CreateTempTable(ds.Tables["OtherTable"], TempOtherDateTableName);
					BulkCopy(ds.Tables["OtherTable"], TempOtherDateTableName);
					DataTable dataTable2 = new DataTable();
					dataTable2 = (ds.Tables.Contains("CRD8") ? ds.Tables["CRD8"] : unitOfWork.Commons.GetTableColumns("CRD8"));
					CreateTempTable(dataTable2, TempCRD8TableName);
					BulkCopy(dataTable2, TempCRD8TableName);
					SqlParameter sqlParameter = new SqlParameter("@Type", text);
					DataTable dataTable3 = ExecuteDataTable("apz_process_BP", CommandType.StoredProcedure, sqlParameter);
					if (dataTable3 != null && dataTable3.Rows.Count > 0)
					{
						ret.Message = Function.ToString(dataTable3.Rows[0]["ErrorMessage"]);
						string text2 = Function.ToString(dataTable3.Rows[0]["ErrorNumber"]);
						ret.Message = text2 + " - " + ret.Message;
					}
					else
					{
						ret.Message = "";
					}
				}
			}
			if (string.IsNullOrEmpty(ret.Message))
			{
				objId = Function.ToString(ds.Tables[0].Rows[0]["CardCode"]);
				objNum = Function.ToString(ds.Tables[0].Rows[0]["CardCode"]);
			}
			else
			{
				objId = "";
				objNum = "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			ret.Message = ex.Message;
		}
	}

	private void ProcessDeleteBp(ref HttpResult ret, ref string objId, ref string objNum)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@cardCode", objId);
			DataTable dataTable = ExecuteDataTable("apz_process_BP_Remove", CommandType.StoredProcedure, sqlParameter);
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				ret.Message = "";
			}
			else
			{
				ret.Message = Function.ToString(dataTable.Rows[0][0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			ret.Message = ex.Message;
			ret.Message = "Update error: " + ex.Message;
		}
	}

	public bool CheckDoc(string itemCode)
	{
		try
		{
			string query = "SELECT \"CardCode\"\r\n                            FROM OCRD \r\n                            WHERE \"CardCode\" = @itemCode;";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@itemCode", itemCode));
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return false;
		}
	}

	public DataTable GetRecord(string position, string cardCode, string userCode, int BPLId)
	{
		DataTable result = new DataTable();
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@pos", position));
			list.Add(new SqlParameter("@CardCode", cardCode));
			list.Add(new SqlParameter("@UserCode", userCode));
			list.Add(new SqlParameter("@BPLId", BPLId));
			string query = "apz_BusinessMasterData_GetRecord";
			result = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable LoadUDFDataSAP(string cardCode)
	{
		try
		{
			List<string> uDFFieldSAP = GetUnitOfWork().Commons.getUDFFieldSAP("OCRD");
			string text = "";
			foreach (string item in uDFFieldSAP)
			{
				text = text + "\"" + item + "\",";
			}
			text = text.Remove(text.Length - 1);
			string query = "select " + text + " from OCRD where \"CardCode\"=@cardCode;";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@cardCode", cardCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetEmployeeCodeBySlpCode(int slpCode)
	{
		try
		{
			string query = GetEmployeeCodeBySlpCodeQuery() + slpCode + ";";
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	protected virtual string GetEmployeeCodeBySlpCodeQuery()
	{
		return "Select \"empID\" as \"OwnerCode\",(\"firstName\"+', '+\"lastName\") as \"OwnerName\" \r\n            from OHEM where \"salesPrson\"=";
	}

	public DataTable GetVatByCardCode(string cardCode)
	{
		try
		{
			return ExecuteDataTable(GetVatByCardCodeQuery(), CommandType.Text, new SqlParameter("@cardCode", Function.ToString(cardCode)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	protected virtual string GetVatByCardCodeQuery()
	{
		return "Select isnull(a.ECVatGroup,'') as VatGroup,  isnull(b.Rate,0) as Rate from OCRD a\r\n                            left join ovtg b on a.ECVatGroup=b.code where cardcode=@cardCode";
	}

	public DataTable GetAddressByCardCode(string cardCode)
	{
		try
		{
			return ExecuteDataTable("select * from crd1 Where ((\"CardCode\"=@cardCode and @cardCode != '') OR @cardCode = '');", CommandType.Text, new SqlParameter("@cardCode", Function.ToString(cardCode)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetListAddressByCardCode(string cardCode)
	{
		try
		{
			string query = "APZ_D_Store_GetListAddress";
			return ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@cardCode", Function.ToString(cardCode)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetSearchTableAddress()
	{
		return GetUnitOfWork().Commons.GetSearchTable("CRD1");
	}

	public DataTable GetContactPersons(string cardType)
	{
		try
		{
			return ExecuteDataTable("SELECT T0.\"CntctCode\",T0.\"Name\", T0.\"CardCode\" \r\n                                    FROM OCPR T0 INNER JOIN ocrd T1 ON T0.\"CardCode\"=T1.\"CardCode\" AND T1.\"CardType\" in (select Data from split(@CardType, ';'));", CommandType.Text, new SqlParameter("@CardType", Function.ToString(cardType)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataSet GetBPProperties(string cardCode)
	{
		try
		{
			string query = "SELECT \"QryGroup1\", \"QryGroup2\", \"QryGroup3\", \"QryGroup4\", \"QryGroup5\", \"QryGroup6\", \"QryGroup7\", \"QryGroup8\", \"QryGroup9\", \"QryGroup10\", \"QryGroup11\", \"QryGroup12\", \"QryGroup13\", \"QryGroup14\", \"QryGroup15\", \"QryGroup16\", \"QryGroup17\", \"QryGroup18\", \"QryGroup19\", \"QryGroup20\", \"QryGroup21\", \"QryGroup22\", \"QryGroup23\", \"QryGroup24\", \"QryGroup25\", \"QryGroup26\", \"QryGroup27\", \"QryGroup28\", \"QryGroup29\", \"QryGroup30\", \"QryGroup31\", \"QryGroup32\", \"QryGroup33\", \"QryGroup34\", \"QryGroup35\", \"QryGroup36\", \"QryGroup37\", \"QryGroup38\", \"QryGroup39\", \"QryGroup40\", \"QryGroup41\", \"QryGroup42\", \"QryGroup43\", \"QryGroup44\", \"QryGroup45\", \"QryGroup46\", \"QryGroup47\", \"QryGroup48\", \"QryGroup49\", \"QryGroup50\", \"QryGroup51\", \"QryGroup52\", \"QryGroup53\", \"QryGroup54\", \"QryGroup55\", \"QryGroup56\", \"QryGroup57\", \"QryGroup58\", \"QryGroup59\", \"QryGroup60\", \"QryGroup61\", \"QryGroup62\", \"QryGroup63\", \"QryGroup64\" FROM OCRD WHERE \"CardCode\" = @CardCode;\r\n                SELECT \"GroupName\" FROM OCQG ORDER BY \"GroupCode\" ASC; ";
			if (cardCode == "-1")
			{
				query = GetBPPropertiesQuery();
			}
			DataSet dataSet = ExecuteData(query, CommandType.Text, new SqlParameter("@CardCode", Function.ToString(cardCode)));
			if (dataSet != null && dataSet.Tables.Count == 2)
			{
				return dataSet;
			}
			return new DataSet();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	protected virtual string GetBPPropertiesQuery()
	{
		return "Select 'N' QryGroup1,'N' QryGroup2,'N' QryGroup3,'N' QryGroup4,'N' QryGroup5,'N' QryGroup6,'N' QryGroup7,\r\n            'N' QryGroup8,'N' QryGroup9,'N' QryGroup10,'N' QryGroup11,'N' QryGroup12,'N' QryGroup13,'N' QryGroup14,\r\n            'N' QryGroup15,'N' QryGroup16,'N' QryGroup17,'N' QryGroup18,'N' QryGroup19,'N' QryGroup20,'N' QryGroup21,\r\n            'N' QryGroup22,'N' QryGroup23,'N' QryGroup24,'N' QryGroup25,'N' QryGroup26,'N' QryGroup27,'N' QryGroup28,\r\n            'N' QryGroup29,'N' QryGroup30,'N' QryGroup31,'N' QryGroup32,'N' QryGroup33,'N' QryGroup34,'N' QryGroup35,\r\n            'N' QryGroup36,'N' QryGroup37,'N' QryGroup38,'N' QryGroup39,'N' QryGroup40,'N' QryGroup41,'N' QryGroup42,\r\n            'N' QryGroup43,'N' QryGroup44,'N' QryGroup45,'N' QryGroup46,'N' QryGroup47,'N' QryGroup48,'N' QryGroup49,\r\n            'N' QryGroup50,'N' QryGroup51,'N' QryGroup52,'N' QryGroup53,'N' QryGroup54,'N' QryGroup55,'N' QryGroup56,\r\n            'N' QryGroup57,'N' QryGroup58,'N' QryGroup59,'N' QryGroup60,'N' QryGroup61,'N' QryGroup62,'N' QryGroup63,'N' QryGroup64;\r\n            Select GroupName From OCQG Order By GroupCode ASC";
	}

	public DataTable ExistFileInDoc(string cardCode, string fileName, string fileExt)
	{
		DataTable result = new DataTable();
		try
		{
			string query = ExistFileInDocQuery();
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@cardCode", cardCode ?? ""));
			list.Add(new SqlParameter("@fileExt", fileExt ?? ""));
			list.Add(new SqlParameter("@fileName", fileName ?? ""));
			result = ExecuteDataTable(query, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	protected virtual string ExistFileInDocQuery()
	{
		return "select FileName from atc1 where AbsEntry=(select AtcEntry from OCRD where cardCode=@cardCode) \r\n\t                and FileExt=@fileExt and FileName=@fileName and AbsEntry is not null";
	}

	public DataTable LoadComboCountry()
	{
		try
		{
			string query = "select \"Code\",\"Name\" from OCRY;";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable ShowBankAccount()
	{
		try
		{
			string query = "select * from ODSC;";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable LoadBPBankAccount(string cardCode)
	{
		DataTable result = new DataTable();
		try
		{
			result = ExecuteDataTable("Select * from OCRB Where \"CardCode\"=@cardCode;", CommandType.Text, new SqlParameter("@cardCode", cardCode ?? ""));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable IsValidBankCode(string bankCode)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "select \"BankCode\" from ODSC Where \"BankCode\"=@BankCode;";
			SqlParameter sqlParameter = new SqlParameter("@BankCode", bankCode);
			result = ExecuteDataTable(query, CommandType.Text, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable LoadComboIndustry()
	{
		DataTable result = new DataTable();
		try
		{
			string query = "select \"IndCode\",\"IndName\",\"IndDesc\" from OOND;";
			result = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable LoadComboTerritory()
	{
		DataTable result = new DataTable();
		try
		{
			string query = "select \"territryID\",\"descript\" from OTER;";
			result = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable LoadComboLanguage()
	{
		DataTable result = new DataTable();
		try
		{
			string query = "select \"Code\",\"ShortName\",\"Name\" from OLNG Order by \"Name\";";
			result = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable LoadComboTechnician()
	{
		DataTable result = new DataTable();
		try
		{
			string query = LoadComboTechnicianQueryString();
			result = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	protected virtual string LoadComboTechnicianQueryString()
	{
		return "select a.empID,lastName+', ' + firstName Name from OHEM a inner join hem6 b on a.empID = b.empID inner join OHTY c on b.roleID= c.typeID where c.typeID=-2";
	}

	public DataTable GetFactoryIndicatorName(string code)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "select \"Name\" from OIDC Where \"Code\"=@Code;";
			result = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@Code", code));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable LoadComboPaymentTermPriority()
	{
		DataTable result = new DataTable();
		try
		{
			string query = "select \"PrioCode\",\"PrioDesc\" from OBPP;";
			result = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable LoadComboPaymentTermHolidays()
	{
		DataTable result = new DataTable();
		try
		{
			string query = "select \"HldCode\" from OHLD;";
			result = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable LoadPaymentDates(string cardCode)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "select * from CRD5 Where \"CardCode\"=@CardCode;";
			result = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@CardCode", cardCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable GetBPGroup()
	{
		DataTable result = new DataTable();
		try
		{
			string query = "select * from ocrg where \"Locked\"='N';";
			result = ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable ExistsStockTransaction(string cardCode)
	{
		try
		{
			return ExecuteDataTable("APZ_BusinessMasterData_exists_stocktransaction", CommandType.StoredProcedure, new SqlParameter("@CardCode", cardCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable ExistsLinked(string cardCode)
	{
		try
		{
			return ExecuteDataTable("APZ_BusinessMasterData_exists_linked", CommandType.StoredProcedure, new SqlParameter("@CardCode", cardCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public string ValidateChangedValue(DataSet ds)
	{
		string result = "Get no data from query";
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using (DatabaseService.BeginTransaction())
				{
					if (ds != null && ds.Tables.Count == 2 && ds.Tables[0] != null && ds.Tables[1] != null)
					{
						CreateTempTable(ds.Tables[0], "#APZ_BP_VALID_OLD");
						BulkCopy(ds.Tables[0], "#APZ_BP_VALID_OLD");
						CreateTempTable(ds.Tables[1], "#APZ_BP_VALID_NEW");
						BulkCopy(ds.Tables[1], "#APZ_BP_VALID_NEW");
						DataTable dataTable = ExecuteDataTable("APZ_BusinessMasterData_ValidateChangedValue", CommandType.Text);
						if (dataTable != null && dataTable.Rows.Count > 0)
						{
							result = Function.ToString(dataTable.Rows[0][0]);
						}
					}
					else
					{
						result = "Validate tables is empty";
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			result = ex.Message;
		}
		return result;
	}

	public DataTable ExistsLinkedAll(string cardCode)
	{
		try
		{
			return ExecuteDataTable("APZ_BusinessMasterData_all_linkedtype", CommandType.StoredProcedure, new SqlParameter("@CardCode", cardCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetPaymentTerm(int groupNum)
	{
		try
		{
			return ExecuteDataTable("Select \"LatePyChrg\",\"ListNum\",\"VolumDscnt\",\"CredLimit\",\"ObligLimit\" From OCTG Where \"GroupNum\" = @groupNum;", CommandType.Text, new SqlParameter("@groupNum", groupNum));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable ShowPaymentTerm()
	{
		try
		{
			return ExecuteDataTable("Select \"PymntGroup\",\"GroupNum\" From OCTG;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public string ExistLinkedContact(int cntctCode)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("APZ_BP_ExistLinkedContact", CommandType.StoredProcedure, new SqlParameter("@cntctCode", cntctCode));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ToString(dataTable.Rows[0][0]);
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public DataTable GetControlAccounts(string cardCode)
	{
		try
		{
			string query = "SELECT a.*, b.\"AcctName\", CASE \"AcctType\" WHEN 'D' THEN 'Down Payment' WHEN 'O' THEN 'Open Debts' END AS \"AcctTypeName\" FROM crd3 a LEFT JOIN OACT b ON a.\"AcctCode\" = b.\"AcctCode\" WHERE a.\"CardCode\" = @cardCode;";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@cardCode", cardCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetAccountTabName(string fatherCard, string debPayAcct, string dpmClear, string dpmIntAct, string connBp)
	{
		try
		{
			SqlParameter[] parameters = new SqlParameter[5]
			{
				new SqlParameter("@fatherCard", string.IsNullOrWhiteSpace(fatherCard) ? "" : fatherCard),
				new SqlParameter("@debPayAcct", string.IsNullOrWhiteSpace(debPayAcct) ? "" : debPayAcct),
				new SqlParameter("@dpmClear", string.IsNullOrWhiteSpace(dpmClear) ? "" : dpmClear),
				new SqlParameter("@dpmIntAct", string.IsNullOrWhiteSpace(dpmIntAct) ? "" : dpmIntAct),
				new SqlParameter("@connBP", string.IsNullOrWhiteSpace(connBp) ? "" : connBp)
			};
			string accountTabNameQuery = GetAccountTabNameQuery();
			return ExecuteDataTable(accountTabNameQuery, CommandType.Text, parameters);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	protected virtual string GetAccountTabNameQuery()
	{
		return "select\r\n\t(select CardName from OCRD where CardCode = @fatherCard) FatherCard\r\n\t,(select AcctName from OACT where AcctCode = @debPayAcct)  DebPayAcct\r\n\t,(select AcctName from OACT where AcctCode = @dpmClear)  DpmClear\r\n\t,(select AcctName from OACT where AcctCode = @dpmIntAct)  DpmIntAct\r\n\t,(select CardName from OCRD where CardCode = @connBP)  ConnBP\r\n";
	}

	public DataTable GetDefaultLanguage()
	{
		try
		{
			string defaultLanguageQuery = GetDefaultLanguageQuery();
			return ExecuteDataTable(defaultLanguageQuery, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	protected virtual string GetDefaultLanguageQuery()
	{
		return "select [SBO-COMMON].[DBO].[SLSD].deflang from [SBO-COMMON].[DBO].[SLSD] where name = (SELECT TOP 1 [SBO-COMMON].[DBO].[SRGC].LOC FROM [SBO-COMMON].[DBO].[SRGC] where dbName=DB_NAME())";
	}

	public DataTable LoadCBPaymentRunBank(string countryCode)
	{
		try
		{
			string query = LoadCBPaymentRunBankQuery();
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@countryCode", Function.ToString(countryCode)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	protected virtual string LoadCBPaymentRunBankQuery()
	{
		return "select '' BankCode,'' BankName\r\nunion all\r\nselect BankCode,BankName from ODSC where CountryCod=@countryCode";
	}

	public DataTable LoadCBPaymentRunAccount(string bankCode, string cryCode)
	{
		try
		{
			string query = LoadCBPaymentRunAccountQuery();
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@bankCode", Function.ToString(bankCode)), new SqlParameter("@cryCode", Function.ToString(cryCode)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	protected virtual string LoadCBPaymentRunAccountQuery()
	{
		return "select '' Account,'' IBAN,'' Branch,'' SwiftNum,'' ControlKey\r\nunion all\r\nselect Account,IBAN,Branch,SwiftNum,ControlKey from dsc1 where BankCode=@bankCode and country=@cryCode";
	}

	public DataTable LoadCBPaymentRunBCACode()
	{
		try
		{
			string query = LoadCBPaymentRunBCACodeQueryString();
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	protected virtual string LoadCBPaymentRunBCACodeQueryString()
	{
		return "select '' Code ,'' Name\r\n                    union all\r\n                    select * from OBCA";
	}

	public DataTable GetBpBranchAssignment(string cardCode)
	{
		try
		{
			string text = "";
			DataTable dataTable = new DataTable();
			if (string.IsNullOrWhiteSpace(cardCode))
			{
				text = "SELECT \"BPLId\", \"BPLName\", \"AliasName\", \"BPLFrName\", \"County\", \"State\", \"Country\", \"Disabled\", 'N' AS \"Checked\" FROM obpl order by \"BPLId\";";
				dataTable = ExecuteDataTable(text, CommandType.Text);
			}
			else
			{
				text = "SELECT a.\"BPLId\", \"BPLName\", \"AliasName\", \"BPLFrName\", \"County\", \"State\", \"Country\", \"Disabled\", CASE WHEN b.\"BPLId\" IS NULL THEN 'N' ELSE 'Y' END AS \"Checked\" FROM obpl a LEFT JOIN CRD8 b ON a.\"BPLId\" = b.\"BPLId\" AND \"CardCode\" = @cardCode order by a.\"BPLId\";";
				dataTable = ExecuteDataTable(text, CommandType.Text, new SqlParameter("@cardCode", cardCode));
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public ResponseResult GetListStore(GetListStoreParamsImpl data)
	{
		try
		{
			string query = "APZ_D_GetListStore_V2";
			DataTable dataTable = new DataTable();
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@cardType", Function.ToString(data.cardType)));
			list.Add(new SqlParameter("@cardName", Function.ToString(data.cardName)));
			list.Add(new SqlParameter("@subType", Function.ToString(data.subType)));
			list.Add(new SqlParameter("@saleUser", Function.ToString(data.saleUser)));
			list.Add(new SqlParameter("@type", Function.ToString(data.type)));
			list.Add(new SqlParameter("@filterType", Function.ToString(data.filterType)));
			list.Add(new SqlParameter("@searchText", Function.ToString(data.searchText)));
			list.Add(new SqlParameter("@filterProperties", Function.ToString(data.filterProperties)));
			list.Add(new SqlParameter("@pageIndex", Function.ToString(data.pageIndex)));
			list.Add(new SqlParameter("@fromDate", Function.ToString(data.fromDate)));
			list.Add(new SqlParameter("@toDate", Function.ToString(data.toDate)));
			list.Add(new SqlParameter("@latitude", data.latitude));
			list.Add(new SqlParameter("@longitude", data.longitude));
			list.Add(new SqlParameter("@u_LevelDL", Function.ToString(data.u_LevelDL)));
			dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			return new ResponseResult
			{
				Content = dataTable,
				Message = "",
				MessageCode = "200"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = "402"
			};
		}
	}

	public DataTable GetBpList(string cardType, string cardName, string subType, string saleUser, string type = "")
	{
		try
		{
			string query = "APZ_D_GetListStore";
			new DataTable();
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@cardType", Function.ToString(cardType)));
			list.Add(new SqlParameter("@cardName", Function.ToString(cardName)));
			list.Add(new SqlParameter("@subType", Function.ToString(subType)));
			list.Add(new SqlParameter("@saleUser", Function.ToString(saleUser)));
			list.Add(new SqlParameter("@type", Function.ToString(type)));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDetailBP(string cardCode, string userCode)
	{
		try
		{
			string query = "APZ_D_GetStoreDetail";
			new DataTable();
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@cardCode", cardCode));
			list.Add(new SqlParameter("@userCode", userCode));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDetailBPRevenue(string cardCode, string userCode)
	{
		try
		{
			string query = "APZ_D_Store_GetRevenue";
			new DataTable();
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@cardCode", cardCode));
			list.Add(new SqlParameter("@userCode", userCode));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetActivityByCardCode(string cardCode)
	{
		try
		{
			string query = "select * from apz_oclg where \"CardCode\" = @cardCode;";
			new DataTable();
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@cardCode", cardCode));
			return ExecuteDataTable(query, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable DMSGetDataCardType()
	{
		try
		{
			string query = "APZ_UserFields_GetListField";
			DataTable result = new DataTable();
			DataSet dataSet = new DataSet();
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter
				{
					ParameterName = "@TableId",
					Value = "OCRD"
				}
			};
			dataSet = ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
			DataTable dataTable = dataSet.Tables[0].Select("AliasID='CardType'").CopyToDataTable();
			int num = -1;
			if (dataTable.IsNotNull())
			{
				num = Function.ParseInt(dataTable.Rows[0]["FieldID"]);
			}
			if (dataSet.Tables[1].IsNotNull())
			{
				result = dataSet.Tables[1].Select("FieldID=" + num).CopyToDataTable();
			}
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable DMSGetDataPropertiesBusinessType()
	{
		try
		{
			string query = "APZ_D_GetListPropertiesBusinessType";
			new DataTable();
			return ExecuteDataTable(query, CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataSet DMSGetDetailAllStore(string userName)
	{
		try
		{
			string query = "APZ_D_GetAllDetailStore";
			new DataSet();
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter
				{
					ParameterName = "@saleUser",
					Value = userName
				}
			};
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	public DataTable CheckPotential(string cardCode)
	{
		try
		{
			string query = "APZ_D_Customers_CheckPotential";
			new DataTable();
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter
				{
					ParameterName = "@cardCode",
					Value = cardCode
				}
			};
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}
}
