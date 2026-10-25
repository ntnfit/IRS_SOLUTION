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
using Apzon.Entities.Models.Production;
using Apzon.Services.Interfaces.BusinessPartner;
using Apzon.Services.Interfaces.Opportunity;

namespace Apzon.Services.Repository.Opportunity;

public class OpportunityRepository(IDatabaseService databaseService) : BaseService<DataSet>(databaseService), IOpportunity
{
	private readonly string tmpTBNameDoc = "#APZ_OPPR_DOC";

	private readonly string tmpTBNameStage = "#APZ_OPPR_STA";

	private readonly string tmpTBNameCom = "#APZ_OPPR_COM";

	private readonly string tmpTBNameBus = "#APZ_OPPR_BUS";

	private readonly string tmpTBNameRea = "#APZ_OPPR_REA";

	private readonly string tmpTBNameAct = "#APZ_OPPR_ACT";

	private readonly string tmpTBNameAtt = "#APZ_OPPR_ATTC";

	private readonly string tmpTBNameIntr = "#APZ_OPPR_INTR";

	public int GetNewOpprId()
	{
		try
		{
			string query = "APZ_SalesOpportunity_GetNewOpprId";
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0][0]);
			}
			return -1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetNewOpprId  FAILED ", ex.Message);
			return -1;
		}
	}

	public DataTable GetDoc(int OpprId, string userCode)
	{
		try
		{
			string query = "APZ_SalesOpportunity_GetDoc";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@OpprId", OpprId));
			list.Add(new SqlParameter("@UserName", userCode));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetStage(int OpprId)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@OpprId", OpprId));
			return ExecuteDataTable("SELECT * FROM OPR1 WHERE \"OpprId\"=@OpprId;", CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetStage  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetStageDMS(int OpprId)
	{
		try
		{
			string query = "APZ_D_SalesOpportunity_GetStage";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@OpprId", OpprId));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetStage  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetCompetitor(int OpprId)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@OpprId", OpprId));
			return ExecuteDataTable("SELECT * from OPR3 where \"OpportId\"=@OpprId;", CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetCompetitor  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetPartner(int OpprId)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@OpprId", OpprId));
			return ExecuteDataTable("SELECT * FROM OPR2 WHERE \"OpportId\"=@OpprId;", CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetPartner  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetPartnerDMS(int OpprId)
	{
		try
		{
			string query = "APZ_D_SalesOpportunity_GetPartner";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@OpprId", OpprId));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetPartner  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetReason(int OpprId)
	{
		try
		{
			string query = "SELECT * from OPR5 where \"OpportId\"=@OpprId;";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@OpprId", OpprId));
			return ExecuteDataTable(query, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetReason  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetItemDetail(int OpprId)
	{
		try
		{
			string query = "APZ_SalesOpportunity_GetItemDetail";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@OpprId", OpprId));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetItemDetail  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetListStage()
	{
		DataTable result = new DataTable();
		try
		{
			string query = "APZ_SalesOpportunity_GetListStage";
			result = ExecuteDataTable(query, CommandType.StoredProcedure, (SqlParameter[])null);
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get List Stage  FAILED ", ex.Message);
			return result;
		}
	}

	public DataTable GetStage()
	{
		DataTable result = new DataTable();
		try
		{
			string query = "Select Num, Descript from OOST";
			result = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get List Stage  FAILED ", ex.Message);
			return result;
		}
	}

	public DataTable GetListCompetitor()
	{
		DataTable result = new DataTable();
		try
		{
			string query = "APZ_SalesOpportunity_GetListCompetitor";
			result = ExecuteDataTable(query, CommandType.StoredProcedure, (SqlParameter[])null);
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get List Competitor  FAILED ", ex.Message);
			return result;
		}
	}

	public DataTable GetListPartner()
	{
		DataTable result = new DataTable();
		try
		{
			string query = "APZ_SalesOpportunity_GetListPartner";
			result = ExecuteDataTable(query, CommandType.StoredProcedure, (SqlParameter[])null);
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get List Partner  FAILED ", ex.Message);
			return result;
		}
	}

	public DataTable GetListPartnerRelation()
	{
		DataTable result = new DataTable();
		try
		{
			string query = "APZ_SalesOpportunity_GetListPartnerRelation";
			result = ExecuteDataTable(query, CommandType.StoredProcedure, (SqlParameter[])null);
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get List Relation  FAILED ", ex.Message);
			return result;
		}
	}

	public DataTable GetListReason()
	{
		DataTable result = new DataTable();
		try
		{
			string query = "APZ_SalesOpportunity_GetListReason";
			result = ExecuteDataTable(query, CommandType.StoredProcedure, (SqlParameter[])null);
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get List Reason  FAILED ", ex.Message);
			return result;
		}
	}

	public DataTable GetLinkedActivities(int oprId)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "APZ_SalesOpportunity_GetLinkedActivities";
			SqlParameter[] parameters = new SqlParameter[1]
			{
				new SqlParameter("@oprId", oprId)
			};
			result = ExecuteDataTable(query, CommandType.StoredProcedure, parameters);
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get Linked Activities FAILED ", ex.Message);
			return result;
		}
	}

	public DataTable GetUserForListActivity()
	{
		DataTable result = new DataTable();
		try
		{
			string query = "Select Convert(nvarchar,UserID) Code,U_NAME Name from ousr";
			result = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get Linked Activities FAILED ", ex.Message);
			return result;
		}
	}

	public DataTable GetOwnerForStage()
	{
		DataTable result = new DataTable();
		try
		{
			result = ExecuteDataTable(GetOwnerForStageQuery(), CommandType.Text, (SqlParameter[])null);
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return result;
		}
	}

	protected virtual string GetOwnerForStageQuery()
	{
		return "select empID Code,lastName+', '+firstName Name from ohem";
	}

	public DataTable GetBPData(string cardCode)
	{
		DataTable result = new DataTable();
		try
		{
			result = ExecuteDataTable("Select \"CardName\" From OCRD where \"CardCode\"=@cardCode;", CommandType.Text, new SqlParameter("@cardCode", Function.ToString(cardCode)));
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return result;
		}
	}

	public DataTable SearchBPData(string cardCode)
	{
		DataTable result = new DataTable();
		try
		{
			result = ExecuteDataTable("Select \"CardName\",\"CardCode\",\"SlpCode\" From OCRD where \"CardCode\" = @cardCode;", CommandType.Text, new SqlParameter("@cardCode", Function.ToString(cardCode)));
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return result;
		}
	}

	public virtual string GetItemDetailUdfColumn()
	{
		return "SELECT COLUMN_NAME name,DATA_TYPE datatype FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME=@TableName AND COLUMN_NAME LIKE 'U\\_%' ESCAPE '\\'";
	}

	public string SaveDoc(string action, DataSet ds)
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
				return "Opportunity's Repository: Wrong action. Should be: 'A', 'U' OR 'D'";
			}
			return Process(ApzObjectTypes.oSalesOpportunities, -1, transactionType, ds).Message;
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
			ProcessOpportunity(ref transtype, ds, ref objId, ref objNum, ref ret);
			break;
		case Enums.TransactionType.U:
			ProcessOpportunity(ref transtype, ds, ref objId, ref objNum, ref ret);
			break;
		case Enums.TransactionType.D:
			ProcessDelete(ref ret, ref objId, ref objNum);
			break;
		}
		return ret;
	}

	private void ProcessDelete(ref HttpResult ret, ref string objId, ref string objNum)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@opprid", objId);
			DataTable dataTable = ExecuteDataTable("apz_process_Opportunity_Remove", CommandType.StoredProcedure, sqlParameter);
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

	private void ProcessOpportunity(ref Enums.TransactionType transtype, DataSet ds, ref string objId, ref string objNum, ref HttpResult ret)
	{
		try
		{
			if (ds == null || ds.Tables.Count < 5 || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
			{
				ret.Message = "No data to save";
			}
			else
			{
				IBusinessMasterDatas businessMasterData = GetUnitOfWork().BusinessMasterData;
				DataTable dataTable = new DataTable();
				DataTable dataTable2 = new DataTable();
				DataTable dataTable3 = new DataTable();
				DataTable dataTable4 = new DataTable();
				DataTable dataTable5 = new DataTable();
				DataTable dataTable6 = new DataTable();
				dataTable = ((ds.Tables[2] != null && ds.Tables[2].Columns.Count != 0) ? ds.Tables[2].Copy() : businessMasterData.GetBPTableStructure("OPR3"));
				dataTable2 = ((ds.Tables[3] != null && ds.Tables[3].Columns.Count != 0) ? ds.Tables[3].Copy() : businessMasterData.GetBPTableStructure("OPR2"));
				dataTable3 = ((ds.Tables[4] != null && ds.Tables[4].Columns.Count != 0) ? ds.Tables[4].Copy() : businessMasterData.GetBPTableStructure("OPR5"));
				if (ds.Tables[5] == null || ds.Tables[5].Columns.Count == 0)
				{
					dataTable4.Columns.Add("ClgCode", typeof(string));
					dataTable4.Columns.Add("OprId", typeof(string));
					dataTable4.Columns.Add("OprLine", typeof(int));
					dataTable4.AcceptChanges();
				}
				else
				{
					dataTable4 = ds.Tables[5].Copy();
				}
				dataTable5 = ((ds.Tables[6] != null && ds.Tables[6].Columns.Count != 0) ? ds.Tables[6].Copy() : businessMasterData.GetBPTableStructure("ATC1"));
				dataTable6 = ((ds.Tables[7] != null && ds.Tables[7].Columns.Count != 0) ? ds.Tables[7].Copy() : businessMasterData.GetBPTableStructure("OPR4"));
				for (int num = 7; num >= 2; num--)
				{
					ds.Tables.Remove(ds.Tables[num]);
				}
				ds.AcceptChanges();
				ds.Tables.AddRange(new DataTable[6] { dataTable, dataTable2, dataTable3, dataTable4, dataTable5, dataTable6 });
				ds.AcceptChanges();
				string value = ((transtype == Enums.TransactionType.A) ? "A" : "U");
				CreateTempTable(ds.Tables[0], tmpTBNameDoc);
				BulkCopy(ds.Tables[0], tmpTBNameDoc);
				CreateTempTable(ds.Tables[1], tmpTBNameStage);
				BulkCopy(ds.Tables[1], tmpTBNameStage);
				CreateTempTable(ds.Tables[2], tmpTBNameCom);
				BulkCopy(ds.Tables[2], tmpTBNameCom);
				CreateTempTable(ds.Tables[3], tmpTBNameBus);
				BulkCopy(ds.Tables[3], tmpTBNameBus);
				CreateTempTable(ds.Tables[4], tmpTBNameRea);
				BulkCopy(ds.Tables[4], tmpTBNameRea);
				CreateTempTable(ds.Tables[5], tmpTBNameAct);
				BulkCopy(ds.Tables[5], tmpTBNameAct);
				CreateTempTable(ds.Tables[6], tmpTBNameAtt);
				BulkCopy(ds.Tables[6], tmpTBNameAtt);
				CreateTempTable(ds.Tables[7], tmpTBNameIntr);
				BulkCopy(ds.Tables[7], tmpTBNameIntr);
				SqlParameter sqlParameter = new SqlParameter("@Type", value);
				DataTable dataTable7 = ExecuteDataTable("apz_process_Opportunity", CommandType.StoredProcedure, sqlParameter);
				if (dataTable7 != null && dataTable7.Rows.Count > 0)
				{
					ret.Message = Function.ToString(dataTable7.Rows[0]["ErrorMessage"]);
					string text = Function.ToString(dataTable7.Rows[0]["ErrorNumber"]);
					ret.Message = text + " - " + ret.Message;
				}
				else
				{
					ret.Message = "";
				}
			}
			if (string.IsNullOrEmpty(ret.Message))
			{
				objId = Function.ToString(ds.Tables[0].Rows[0]["OpprId"]);
				objNum = Function.ToString(ds.Tables[0].Rows[0]["OpprId"]);
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

	public virtual string ProcessDataReasons(DataTable dt, int retKey)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				ExecuteNonQuery("DELETE FROM OPR5 WHERE \"OpportId\"=@OpportId;", CommandType.Text, new SqlParameter("@OpportId", retKey));
				if (dt != null && dt.Rows.Count > 0)
				{
					List<SqlParameter> list = new List<SqlParameter>();
					string text = "INSERT INTO OPR5 (\"OpportId\", \"Line\", \"ReasondId\") VALUES ";
					for (int i = 0; i < dt.Rows.Count; i++)
					{
						text = text + "(@OpportId" + i + ",@Line" + i + ",@ReasondId" + i + "),";
						list.AddRange(new SqlParameter[3]
						{
							new SqlParameter("@OpportId" + i, retKey),
							new SqlParameter("@Line" + i, i),
							new SqlParameter("@ReasondId" + i, Function.ParseInt(dt.Rows[i]["ReasondId"]))
						});
					}
					if (text.EndsWith(","))
					{
						text = text.Remove(text.Length - 1);
					}
					ExecuteNonQuery(text, CommandType.Text, list.ToArray());
				}
				dbTransaction.Commit();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return "";
	}

	private bool ExistsOpportunity(int opprId)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("select 1 from OOPR where OpprId=@opprId", CommandType.Text, new SqlParameter("@opprId", opprId));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "check Stage  FAILED ", ex.Message);
			return false;
		}
	}

	public DataTable SearchDoc(DataTable dtSearch)
	{
		try
		{
			DataRow dataRow = dtSearch.Rows[0];
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@FromDate", Function.ParseDateTime(dataRow["FromDate"], "yyyyMMdd")),
				new SqlParameter("@ToDate", Function.ParseDateTime(dataRow["ToDate"], "yyyyMMdd")),
				new SqlParameter("@NoTrans", Function.ParseInt(dataRow["NoTrans"])),
				new SqlParameter("@BusPartner", Function.ToString(dataRow["BusPartner"])),
				new SqlParameter("@Status", Function.ToString(dataRow["Status"])),
				new SqlParameter("@Stage", Function.ParseInt(dataRow["Stage"])),
				new SqlParameter("@SaleEmp", Function.ParseInt(dataRow["SaleEmp"])),
				new SqlParameter("@UserName", Function.ToString(dataRow["UserName"]))
			};
			if (dtSearch.ExistsColumn("SearchKey"))
			{
				list.Add(new SqlParameter("@searchKey", Function.ToString(dataRow["SearchKey"])));
			}
			DataTable dataTable = ExecuteDataTable("APZ_SalesOpportunity_SearchDoc", CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SearchDoc  FAILED ", ex.Message);
		}
		return new DataTable();
	}

	public string DeleteDoc(string opprid)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string query = "apz_process_Opportunity_Remove";
				DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@opprid", opprid));
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					return Function.ToString(dataTable.Rows[0][0]);
				}
				dbTransaction.Commit();
				return "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string DeleteStage(int oId, int ln)
	{
		try
		{
			string query = "APZ_SalesOpportunity_DeleteStage";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@oId", oId));
			list.Add(new SqlParameter("@ln", ln));
			if (ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray()) > 0)
			{
				return "";
			}
			return "Row deleted";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "DeleteStage  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public DataTable GetLinkedDocuments(DataTable dTNumAndType)
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("DocDate", typeof(DateTime));
			dataTable.Columns.Add("DocType", typeof(string));
			dataTable.Columns.Add("Comments", typeof(string));
			dataTable.Columns.Add("DocTotal", typeof(double));
			dataTable.AcceptChanges();
			if (dTNumAndType != null && dTNumAndType.Rows.Count > 0)
			{
				for (int i = 0; i < dTNumAndType.Rows.Count; i++)
				{
					ApzObjectTypes objType = (ApzObjectTypes)Function.ParseInt(dTNumAndType.Rows[i]["ObjType"]);
					string tableByApzObjectType = Function.GetTableByApzObjectType(objType);
					int docNum = Function.ParseInt(dTNumAndType.Rows[i]["DocNum"]);
					string docTypeString = GetDocTypeString(objType);
					DataTable linkedDocument = GetLinkedDocument(tableByApzObjectType, docNum, docTypeString);
					if (linkedDocument != null && linkedDocument.Rows.Count > 0)
					{
						for (int j = 0; j < linkedDocument.Rows.Count; j++)
						{
							dataTable.ImportRow(linkedDocument.Rows[j]);
						}
					}
				}
			}
			dataTable.AcceptChanges();
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	private DataTable GetLinkedDocument(string tableName, int docNum, string docType)
	{
		try
		{
			return ExecuteDataTable(parameters: new SqlParameter[1]
			{
				new SqlParameter("@docNum", docNum)
			}, query: "Select \r\n    DocDate\r\n    ,'" + docType + " " + docNum + "' DocType\r\n    ,Comments\r\n    ,DocTotal\r\n    ,DocEntry\r\nfrom " + tableName + " where docNum=@docNum");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	private string GetDocTypeString(ApzObjectTypes objType)
	{
		try
		{
			return objType switch
			{
				ApzObjectTypes.oQuotations => "QU", 
				ApzObjectTypes.oOrders => "OR", 
				ApzObjectTypes.oDeliveryNotes => "DN", 
				ApzObjectTypes.oInvoices => "IN", 
				ApzObjectTypes.oPurchaseQuotations => "PQ", 
				ApzObjectTypes.oPurchaseOrders => "PO", 
				ApzObjectTypes.oPurchaseDeliveryNotes => "PD", 
				ApzObjectTypes.oPurchaseInvoices => "PU", 
				_ => "", 
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public double GetDocumentAmountAndProfit(int docType, int docNum)
	{
		try
		{
			string tableByApzObjectType = Function.GetTableByApzObjectType((ApzObjectTypes)docType);
			DataTable dataTable = ExecuteDataTable(" select \"DocTotal\"-\"VatSum\" as SumTotal from " + tableByApzObjectType + " where \"DocNum\"=@docNum;", CommandType.Text, new SqlParameter("@docNum", docNum));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseDouble(dataTable.Rows[0][0]);
			}
			return 0.0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return 0.0;
		}
	}

	public DataTable GetBPChanelContact(string cardCode)
	{
		try
		{
			return ExecuteDataTable(GetBpChanelContactQuery(), CommandType.Text, new SqlParameter("@cardCode", Function.ToString(cardCode)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	protected virtual string GetBpChanelContactQuery()
	{
		return "declare @defaultName nvarchar(100) = isnull((select CntctPrsn from OCRD where cardCode=@cardCode), '')\r\nselect \r\n\tCntctCode\r\n\t,Name\r\n\t, case when Name=@defaultName then 'Y'\r\n\telse 'N'\r\n\tend DfltCode\r\n\r\nfrom ocpr where Active = 'Y' and cardCode = @cardCode";
	}

	public DataTable GetComboInterest()
	{
		try
		{
			return ExecuteDataTable("select * from OOIN;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetComboInterestLevel()
	{
		try
		{
			return ExecuteDataTable("select * from OOIR;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetInterestRange(int opprId)
	{
		try
		{
			return ExecuteDataTable("Select * from OPR4 where \"OprId\"=@opprId;", CommandType.Text, new SqlParameter("@opprId", opprId));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetRecord(string position, int opprId, string userCode, int branch)
	{
		try
		{
			return ExecuteDataTable("apz_SalesOpportunity_GetRecord", CommandType.StoredProcedure, new SqlParameter("@position", position), new SqlParameter("@opprId", opprId), new SqlParameter("@UserCode", userCode), new SqlParameter("@BPLId", branch));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public string SaveAttachmentFiles(List<AttachmentFile_Impl> lstFiles, int docEntry)
	{
		string text = "";
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				int num = GetAbsEntryByDocEntry(docEntry);
				if (lstFiles != null && lstFiles.Count > 0)
				{
					if (num == 0)
					{
						num = Function.ParseInt(AddFile());
						if (num > 0)
						{
							text = DeleteFile(num);
							foreach (AttachmentFile_Impl lstFile in lstFiles)
							{
								lstFile.AbsEntry = num;
								text = SaveLineFile(lstFile);
								if (!string.IsNullOrEmpty(text))
								{
									break;
								}
							}
						}
					}
					else
					{
						text = DeleteFile(num);
						foreach (AttachmentFile_Impl lstFile2 in lstFiles)
						{
							lstFile2.AbsEntry = num;
							text = SaveLineFile(lstFile2);
							if (!string.IsNullOrEmpty(text))
							{
								break;
							}
						}
					}
				}
				else
				{
					text = DeleteFile(num);
				}
				if (string.IsNullOrWhiteSpace(text))
				{
					text = Function.ToString(num);
					dbTransaction.Commit();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			text = ex.Message;
		}
		return text;
	}

	private int GetAbsEntryByDocEntry(int docEntry)
	{
		try
		{
			string query = "SELECT \"AtcEntry\" FROM OOPR  WHERE  \"OpprId\"= " + docEntry + ";";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0][0]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return 0;
		}
	}

	private string DeleteFile(int absEntry)
	{
		try
		{
			string query = "delete from ATC1 WHERE \"AbsEntry\" = " + Function.ToString(absEntry) + ";";
			ExecuteNonQuery(query, CommandType.Text);
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public virtual string AddFile()
	{
		try
		{
			return Function.ToString(ExecuteScalar("insert into OATC (\"AbsEntry\")  select isnull(max(\"AbsEntry\"),0)+1 From OATC;\r\n                                   Update ONNM set \"AutoKey\"=(select isnull((select max(\"AbsEntry\")+1 from OATC),0)) where \"ObjectCode\"='221';\r\n                                   select isnull((select max(\"AbsEntry\") from OATC),0);", CommandType.Text));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC file oatc table FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public string SaveLineFile(AttachmentFile_Impl line)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			string text;
			if (!CheckLineFile(line.AbsEntry, line.Line))
			{
				text = "INSERT INTO \"ATC1\"\r\n                               (\"AbsEntry\"\r\n                               ,\"Line\"\r\n                               ,\"srcPath\"\r\n                               ,\"trgtPath\"\r\n                               ,\"FileName\"\r\n                               ,\"FileExt\"\r\n                               ,\"Date\"\r\n                               ,\"UsrID\"\r\n                               ,\"Copied\"\r\n                               ,\"Override\"\r\n                               ,\"subPath\")\r\n                         VALUES (";
				text += "@AbsEntry ";
				list.Add(new SqlParameter("@AbsEntry", Function.ParseInt(line.AbsEntry)));
				text += ",@Line ";
				list.Add(new SqlParameter("@Line", Function.ParseInt(line.Line)));
				text += ", @srcPath ";
				list.Add(new SqlParameter("@srcPath", Function.ToString(line.srcPath)));
				text += ",@trgtPath ";
				list.Add(new SqlParameter("@trgtPath", Function.ToString(line.trgtPath)));
				text += ",@FileName ";
				list.Add(new SqlParameter("@FileName", Function.ToString(line.FileName)));
				text += ", @FileExt ";
				list.Add(new SqlParameter("@FileExt", Function.ToString(line.FileExt)));
				text += ",@Date ";
				list.Add(new SqlParameter("@Date", Function.ParseDateTime(line.Date)));
				text += ",@UsrID ";
				list.Add(new SqlParameter("@UsrID", Function.ParseInt(line.UsrID)));
				text += ", @Copied ";
				list.Add(new SqlParameter("@Copied", Function.ToString(line.Copied)));
				text += ",@Override ";
				list.Add(new SqlParameter("@Override", Function.ToString(line.Override)));
				text += ", @subPath ";
				list.Add(new SqlParameter("@subPath", Function.ToString(line.subPath)));
				text += " );";
			}
			else
			{
				text = "\r\n                            update ATC1 set ";
				text += "\"srcPath\"=@srcPath ";
				list.Add(new SqlParameter("@srcPath", Function.ToString(line.srcPath)));
				text += ",\"trgtPath\"=@trgtPath ";
				list.Add(new SqlParameter("@trgtPath", Function.ToString(line.trgtPath)));
				text += ",\"FileName\"=@FileName ";
				list.Add(new SqlParameter("@FileName", Function.ToString(line.FileName)));
				text += ", \"FileExt\"=@FileExt ";
				list.Add(new SqlParameter("@FileExt", Function.ToString(line.FileExt)));
				text += ",\"Date\"=@Date ";
				list.Add(new SqlParameter("@Date", Function.ParseDateTime(line.Date)));
				text += ",\"UsrID\"=@UsrID ";
				list.Add(new SqlParameter("@UsrID", Function.ParseInt(line.UsrID)));
				text += ",\"Copied\"=@Copied ";
				list.Add(new SqlParameter("@Copied", Function.ToString(line.Copied)));
				text += ",\"Override\"=@Override ";
				list.Add(new SqlParameter("@Override", Function.ToString(line.Override)));
				text += ", \"subPath\"=@subPath ";
				list.Add(new SqlParameter("@subPath", Function.ToString(line.subPath)));
				text += " where \"AbsEntry\"=@AbsEntry ";
				list.Add(new SqlParameter("@AbsEntry", Function.ParseInt(line.AbsEntry)));
				text += " and \"Line\"=@Line;";
				list.Add(new SqlParameter("@Line", Function.ParseInt(line.Line)));
			}
			if (ExecuteNonQuery(text, CommandType.Text, list.ToArray()) > 0)
			{
				return null;
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC LINE FAILED ", ex.Message);
			return ex.Message;
		}
	}

	private bool CheckLineFile(int absEntry, int line)
	{
		try
		{
			string query = "SELECT \"AbsEntry\" FROM atc1  WHERE\r\n                        \"AbsEntry\" = " + absEntry + " and  \"Line\" = " + line + ";";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text);
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
}
