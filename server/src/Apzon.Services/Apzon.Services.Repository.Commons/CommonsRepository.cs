using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Services.Interfaces.Commons;
using Apzon.Services.Repository.Inventory.InventoryTransaction;

namespace Apzon.Services.Repository.Commons;

public class CommonsRepository : BaseService<DataTable>, ICommons
{
	protected class ExistValidColumnQueryEntity
	{
		public string TypeID { get; set; }

		public string Column { get; set; }

		public string Table { get; set; }

		public string Condition { get; set; }

		public bool IsString { get; set; }

		public ExistValidColumnQueryEntity(string typeID, bool isString, string column, string table, string condition)
		{
			TypeID = typeID;
			IsString = isString;
			Column = column;
			Table = table;
			Condition = condition;
		}
	}

	public CommonsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable ItemMaterInventory(string userCode, int BPLId)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "APZ_ItemMaster_ItemMaterInventory";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@UserCode", userCode));
			list.Add(new SqlParameter("@BPLId", BPLId));
			result = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "ItemMaterInventory   FAILED ", ex.Message);
		}
		return result;
	}

	public DataTable ItemMaster(string itemCode, Enums.ItemObjectType itObjType, string userCode, int BPLId, string cardcode = "", DateTime? docdate = null, double quantity = 1.0, string subCatNum = "")
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			if (string.IsNullOrEmpty(itemCode))
			{
				itemCode = "";
			}
			list.Add(new SqlParameter("@itemCode", itemCode));
			int num = (int)itObjType;
			list.Add(new SqlParameter("@itemType", num.ToString()));
			list.Add(new SqlParameter("@userName", userCode));
			list.Add(new SqlParameter("@branch", BPLId));
			list.Add(new SqlParameter("@CardCode", cardcode));
			list.Add(new SqlParameter("@DocDate", docdate));
			list.Add(new SqlParameter("@Quantity", quantity));
			return ExecuteDataTable("Apz_Cfl_GetListItem", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "ItemMaster   FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable ItemMasters(List<Common.ListItems> ItemCode)
	{
		try
		{
			new DataTable();
			string text = "SELECT A.\"ItemCode\", A.\"ItemName\", A.\"DfltWH\" AS \"DefaultWarehouse\", A.\"OnHand\" AS \"QuantityOnStock\", \r\n                                A.\"VatGourpSa\" AS \"SalesVATGroup\", A.\"VatGroupPu\" AS \"PurchaseVATGroup\", \r\n                                C.\"Rate\" AS \"RateSa\", C.\"Rate\" AS \"RatePu\", \"InvntryUom\", A.\"validFor\" \r\n                            FROM OITM A \r\n                                LEFT JOIN OVTG C ON A.\"VatGroupPu\" = C.\"Code\" \r\n                            WHERE 1 = 1";
			if (ItemCode != null && ItemCode.Any())
			{
				text += " and A.\"ItemCode\" in (''";
				foreach (Common.ListItems item in ItemCode)
				{
					text = text + ",'" + item.Value + "'";
				}
				text += ")";
			}
			text += " AND A.\"validFor\" ='Y'";
			return ExecuteDataTable(text, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "ItemMasters   FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable ListChooseItemMaster(string itemCode, Enums.ItemObjectType itObjType, string userCode, int BPLId, string userName = "", string cardcode = "", DateTime? docdate = null, double quantity = 1.0)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			int num;
			if (!string.IsNullOrEmpty(itemCode))
			{
				list.Add(new SqlParameter("@itemCode", itemCode));
				num = (int)itObjType;
				list.Add(new SqlParameter("@itemType", num.ToString()));
				list.Add(new SqlParameter("@userName", userCode));
				list.Add(new SqlParameter("@branch", BPLId));
				list.Add(new SqlParameter("@CardCode", cardcode));
				list.Add(new SqlParameter("@DocDate", docdate));
				list.Add(new SqlParameter("@Quantity", quantity));
				return ExecuteData("Apz_Cfl_GetListItem", CommandType.StoredProcedure, list.ToArray()).Tables[1];
			}
			num = (int)itObjType;
			list.Add(new SqlParameter("@ItemObjType", num.ToString()));
			list.Add(new SqlParameter("@UserCode", userCode));
			list.Add(new SqlParameter("@BPLId", BPLId));
			return ExecuteDataTable("SP_APZ_CFLChooseListITEM_ALL", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "ItemMaster   FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable SaleEmployee()
	{
		try
		{
			return ExecuteDataTable("SELECT \"SlpCode\"\r\n\t                              , \"SlpName\"\r\n                            FROM OSLP; ", CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SaleEmployee   FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetMaxID(string table)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "SELECT MAX(\"empID\") AS \"empID\" FROM " + table + " ;";
			result = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return result;
	}

	public string GetWhsDefaultByUser(string user)
	{
		try
		{
			string query = "SELECT A.\"Warehouse\" FROM OUDG A LEFT OUTER JOIN OUSR B ON A.\"UserSign\" = B.\"userSign\" WHERE B.USER_CODE = ' " + user + "';";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Convert.ToString(dataTable.Rows[0]["Warehouse"]);
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return string.Empty;
		}
	}

	public DataTable GetBinByWhsCode(string whsCode)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "SELECT \"AbsEntry\", \"BinCode\" FROM OBIN WHERE \"WhsCode\" ='" + whsCode + "';";
			result = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return result;
	}

	public DataTable ContactPerson(string cardCode)
	{
		try
		{
			DataTable result = new DataTable();
			if (!string.IsNullOrEmpty(cardCode))
			{
				SqlParameter sqlParameter = new SqlParameter("@cardCode", cardCode);
				result = ExecuteDataTable(ContactPersonQuery(), CommandType.Text, sqlParameter);
			}
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "ContactPerson   FAILED ", ex.Message);
			return new DataTable();
		}
	}

	protected virtual string ContactPersonQuery()
	{
		return "SELECT  isnull( CntctCode,0) as CntctCode,\r\n\t\t                             isnull(Name,'') as Name , CardCode\r\n                             FROM OCPR \r\n                             WHERE CardCode = @cardCode ";
	}

	public virtual DataTable GetAllContactPerson()
	{
		try
		{
			new DataTable();
			string query = "select isnull(A.CntctCode,0) as CntctCode,isnull(A.Cellolar,Phone1) as Telephone,B.CardCode,isnull(a.Name,'') as Name from OCPR A left join ocrd B on A.CardCode=B.CardCode";
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "ContactPerson   FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetPosition()
	{
		try
		{
			string query = "select * from OHPS;";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetPosition FAILED ", ex.ToString());
			return new DataTable();
		}
	}

	public DataTable VatGroup(string category)
	{
		try
		{
			string query = "apz_common_VatGroup";
			return ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@category", category));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "VatGroup FAILED ", ex.ToString());
			return new DataTable();
		}
	}

	public DataTable LoadDataCashFlow()
	{
		try
		{
			string query = "SELECT \"CFWId\" AS \"Code\", \"CFWName\" AS \"Name\" FROM OCFW;";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDataCashFlow FAILED ", ex.ToString());
			return new DataTable();
		}
	}

	public DataTable ListDocByCardCode(ApzObjectTypes type, string cardCode)
	{
		try
		{
			List<SqlParameter> prms = new List<SqlParameter>();
			string query = ListDocByCardCodeQuery(type, ApzObjectTypes.oNone, cardCode, "", ref prms);
			return ExecuteDataTable(query, CommandType.Text, prms.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "ListDocByCardCode   FAILED ", ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetListDocumentCopy(ApzObjectTypes objType, ApzObjectTypes toType, string cardCode, string postingDate = "")
	{
		try
		{
			List<SqlParameter> prms = new List<SqlParameter>();
			string query = ListDocByCardCodeQuery(objType, toType, cardCode, postingDate, ref prms);
			return ExecuteDataTable(query, CommandType.Text, prms.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "ListDocByCardCode   FAILED ", ex.Message);
		}
		return new DataTable();
	}

	public virtual string ListDocByCardCodeQuery(ApzObjectTypes type, ApzObjectTypes toType, string cardCode, string postingDate, ref List<SqlParameter> prms)
	{
		List<string> tableName = Function.GetTableName(type);
		string arg = tableName[0];
		string arg2 = tableName[1];
		prms = new List<SqlParameter>();
		string text;
		switch (type)
		{
		case ApzObjectTypes.oBlanketAgreement:
			text = $"SELECT \r\n                                                AbsId AS  DocEntry,\r\n                                                StartDate AS DocDate,\r\n                                                EndDate AS DocDueDate,\r\n                                                BPCode AS CardCode,\r\n                                                BPName AS CardName,\r\n                                                CntctCode,\r\n                                                '' AS Comments  ,\r\n                                                '0' AS DocTotal  ,\r\n                                                '0' AS VatSum  ,\r\n                                                '0' AS DiscPrcnt  ,\r\n                                                '0' AS DocRate,\r\n                                                '' NumAtCard\r\n                                                                        \r\n                                        FROM {arg}\r\n                                        WHERE Status='A' AND BpType = 'C' ";
			if (!string.IsNullOrEmpty(cardCode))
			{
				text += " AND BPCode =@CardCode";
				prms.Add(new SqlParameter("@CardCode", cardCode));
			}
			if (!string.IsNullOrEmpty(postingDate))
			{
				text += " AND StartDate <= @EndDate AND EndDate >= @EndDate";
				prms.Add(new SqlParameter("@EndDate", Function.ParseDateTime(postingDate, "yyyyMMdd")));
			}
			break;
		case ApzObjectTypes.oPurchaseBlanketAgreement:
			text = $"SELECT \r\n                                                AbsId AS  DocEntry,\r\n                                                StartDate AS DocDate,\r\n                                                EndDate AS DocDueDate,\r\n                                                BPCode AS CardCode,\r\n                                                BPName AS CardName,\r\n                                                CntctCode,\r\n                                                '' AS Comments  ,\r\n                                                '0' AS DocTotal  ,\r\n                                                '0' AS VatSum  ,\r\n                                                '0' AS DiscPrcnt  ,\r\n                                                '0' AS DocRate,\r\n                                                '' NumAtCard\r\n                                                                        \r\n                                        FROM {arg}\r\n                                        WHERE Status='A' AND BpType = 'S' ";
			if (!string.IsNullOrEmpty(cardCode))
			{
				text += " AND BPCode =@CardCode";
				prms.Add(new SqlParameter("@CardCode", cardCode));
			}
			if (!string.IsNullOrEmpty(postingDate))
			{
				text += " AND StartDate <= @EndDate AND EndDate >= @EndDate";
				prms.Add(new SqlParameter("@EndDate", Function.ParseDateTime(postingDate, "yyyyMMdd")));
			}
			break;
		case ApzObjectTypes.oPurchaseRequest:
			text = $"select *,Case When ReqType = 12 then 'User' When  ReqType = 171 then 'Employee' END AS ReqTypeName       \r\n                                            FROM {arg}\r\n                                            WHERE DocStatus='O' ";
			break;
		case ApzObjectTypes.oQuotations:
			text = $"DECLARE @isCopyClosedQuo NVARCHAR(1) = ISNULL((SELECT ISNULL(ClosedQuot,'') FROM oadm),'')\r\n\r\n                                        SELECT \r\n                                            *\r\n                                        FROM {arg}\r\n                                        WHERE (DocStatus='O' AND @isCopyClosedQuo<>'Y') \r\n\t                                        OR \r\n\t\t                                        (@isCopyClosedQuo = 'Y' and CANCELED = 'N' AND (DocStatus='C' OR DocStatus='O'))\r\n\t                                        ";
			if (!string.IsNullOrEmpty(cardCode))
			{
				text += " AND CardCode =@CardCode";
				prms.Add(new SqlParameter("@CardCode", cardCode));
			}
			break;
		default:
			text = $"SELECT \r\n                                                *\r\n                                            FROM {arg} a\r\n                                            WHERE a.DocStatus='O' and exists(select 1 from {arg2} where DocEntry = a.DocEntry and OpenQty <> 0) ";
			if (!string.IsNullOrEmpty(cardCode) && !new ApzObjectTypes[3]
			{
				ApzObjectTypes.oInventoryGenEntry,
				ApzObjectTypes.oInventoryGenExit,
				ApzObjectTypes.oMaterialRevaluation
			}.Contains(type))
			{
				text = ((type != ApzObjectTypes.oStockTransfer && type != ApzObjectTypes.oInventoryTransferRequest) ? (text + " AND CardCode = @CardCode") : (text + " AND isnull(CardCode, @CardCode) = @CardCode"));
				prms.Add(new SqlParameter("@CardCode", cardCode));
			}
			switch (type)
			{
			case ApzObjectTypes.oAPReserveInvoice:
			case ApzObjectTypes.oReserveInvoices:
				text += " and isins ='Y'";
				break;
			case ApzObjectTypes.oPurchaseInvoices:
				text += " and isins ='N'";
				break;
			}
			break;
		}
		if (type == ApzObjectTypes.oOrders || type == ApzObjectTypes.oPurchaseOrders)
		{
			text += " and \"Confirmed\" = 'Y'";
		}
		return text + " order by DocDate";
	}

	public List<DocumentsImpl> ListDocDraft(ApzObjectTypes type)
	{
		List<DocumentsImpl> list = new List<DocumentsImpl>();
		try
		{
			string empty = string.Empty;
			_ = string.Empty;
			empty = Function.ToString((int)type);
			List<SqlParameter> list2 = new List<SqlParameter>();
			string query = $"SELECT \"DocEntry\", \"DocNum\", \"DocDate\", \"CardCode\", \"CardName\", \"DocDueDate\", \"Comments\" FROM ODRF WHERE \"ObjType\" = '{empty}';";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, list2.ToArray());
			if (dataTable.IsNotNull())
			{
				foreach (DataRow row in dataTable.Rows)
				{
					list.Add(new DocumentsImpl
					{
						DocEntry = Function.ParseInt(row["DocEntry"]),
						DocNum = Function.ParseInt(row["DocNum"]),
						DocDate = Function.ParseDateTimes(row["DocDate"]),
						DocDueDate = Function.ParseDateTimes(row["DocDueDate"]),
						CardCode = Function.ToString(row["CardCode"]),
						CardName = Function.ToString(row["CardName"]),
						Comments = Function.ToString(row["Comments"])
					});
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "ListDocDraft   FAILED ", ex.Message);
		}
		return list;
	}

	public List<BusinessPartnersImpl> GetBusinessPartner(string cardCode, ApzObjectTypes type)
	{
		try
		{
			string text = "C,L,S";
			switch (type)
			{
			case ApzObjectTypes.oOrders:
			case ApzObjectTypes.oQuotations:
				text = "C,L";
				break;
			case ApzObjectTypes.oInvoices:
			case ApzObjectTypes.oCreditNotes:
			case ApzObjectTypes.oDeliveryNotes:
			case ApzObjectTypes.oReturns:
			case ApzObjectTypes.oReserveInvoices:
				text = "C";
				break;
			case ApzObjectTypes.oPurchaseInvoices:
			case ApzObjectTypes.oPurchaseCreditNotes:
			case ApzObjectTypes.oPurchaseDeliveryNotes:
			case ApzObjectTypes.oPurchaseReturns:
			case ApzObjectTypes.oAPReserveInvoice:
			case ApzObjectTypes.oProductionOrders:
				text = "S";
				break;
			}
			List<BusinessPartnersImpl> list = new List<BusinessPartnersImpl>();
			DataTable dataTable = ExecuteDataTable("SELECT A.CardCode,A.CardName,A.Balance, A.CntctPrsn,B.CntctCode  From OCRD A\r\n                                    LEFT JOIN OCPR B ON A.CardCode = B.CardCode AND A.CntctPrsn = B.Name WHERE A.CardType  in (select data from dbo.split(@CardType,',')) AND ((A.CardCode =@CardCode AND Isnull(@CardCode,'')<>'') OR (ISNULL(@CardCode,'')=''))\r\n                            ORDER BY A.DocEntry  DESC", CommandType.Text, new SqlParameter("@CardType", Function.ToString(text)), new SqlParameter("@CardCode", Function.ToString(cardCode)));
			if (dataTable.IsNotNull())
			{
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					BusinessPartnersImpl businessPartnersImpl = new BusinessPartnersImpl();
					businessPartnersImpl.CardCode = Function.ToString(dataTable.Rows[i]["CardCode"]);
					businessPartnersImpl.CardName = Function.ToString(dataTable.Rows[i]["CardName"]);
					businessPartnersImpl.CurrentAccountBalance = Function.ParseDouble(dataTable.Rows[i]["Balance"]);
					businessPartnersImpl.ContactPerson = Function.ToString(dataTable.Rows[i]["CntctPrsn"]);
					businessPartnersImpl.CntctCode = Function.ParseInt(dataTable.Rows[i]["CntctCode"]);
					list.Add(businessPartnersImpl);
				}
			}
			return list;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetBusinessPartner   FAILED ", ex.Message);
			return new List<BusinessPartnersImpl>();
		}
	}

	public DataTable GetShipType(int id)
	{
		try
		{
			new DataTable();
			string text = "SELECT   \"TrnspCode\" AS \"Code\",\r\n\t\t                                \"TrnspName\" AS \"Name\"\r\n                             FROM OSHP \r\n                             WHERE 1=1 ";
			if (id > 0)
			{
				text = text + " AND \"TrnspCode\" ='" + id + "'";
			}
			text += ";";
			return ExecuteDataTable(text, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetShipType   FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetShipTo(string cardCode)
	{
		try
		{
			return ExecuteDataTable(GetShipToQuery(), CommandType.Text, new SqlParameter("@CardCode", cardCode ?? ""));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetShipTo   FAILED ", ex.Message);
			return new DataTable();
		}
	}

	protected virtual string GetShipToQuery()
	{
		return "SELECT Address,ISNULL(Street,'') +CHAR(13)+ISNULL(ZipCode,'')+' '+ISNULL(City,'')+CHAR(13)+ISNULL(County,'') AS Street,CardCode FROM CRD1 \r\n                                WHERE AdresType='S' and 1=1  AND (isnull(@CardCode,'')='' OR CardCode =@CardCode)";
	}

	public DataTable GetBillTo(string cardCode)
	{
		try
		{
			return ExecuteDataTable(GetBillToQuerry(), CommandType.Text, new SqlParameter("@CardCode", Function.ToString(cardCode)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetBillTo   FAILED ", ex.Message);
			return new DataTable();
		}
	}

	protected virtual string GetBillToQuerry()
	{
		return "SELECT Address,ISNULL(Street,'') +CHAR(13)+ISNULL(ZipCode,'')+' '+ISNULL(City,'')+CHAR(13)+ISNULL(County,'') AS Street,CardCode FROM CRD1 \r\n                                WHERE AdresType='B' and 1=1  AND (isnull(@CardCode,'')='' OR CardCode =@CardCode)";
	}

	public DataSet GetDataCombobox(string cardCode)
	{
		try
		{
			DataSet dataSet = new DataSet();
			DataTable dataTable = new DataTable("BillTo");
			dataTable = GetBillTo(cardCode);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["CardCode"] = cardCode;
				dataTable.Rows.InsertAt(dataRow, 0);
			}
			dataSet.Tables.Add(dataTable);
			DataTable dataTable2 = new DataTable("ShipTo");
			dataTable2 = GetShipTo(cardCode);
			if (dataTable2 != null && dataTable2.Rows.Count > 0)
			{
				DataRow dataRow2 = dataTable2.NewRow();
				dataRow2["CardCode"] = cardCode;
				dataTable2.Rows.InsertAt(dataRow2, 0);
			}
			dataSet.Tables.Add(dataTable2);
			DataTable dataTable3 = new DataTable("ShipType");
			dataTable3 = GetShipType(0);
			if (dataTable3 != null && dataTable3.Rows.Count > 0)
			{
				DataRow dataRow3 = dataTable3.NewRow();
				dataRow3[dataTable3.Columns[0].ColumnName] = -1;
				dataTable3.Rows.Add(dataRow3);
			}
			dataSet.Tables.Add(dataTable3);
			DataTable dataTable4 = new DataTable("ContactPerson");
			dataTable4 = ContactPerson(cardCode);
			if (dataTable4 != null && dataTable4.Rows.Count > 0)
			{
				DataRow dataRow4 = dataTable4.NewRow();
				dataRow4["CardCode"] = cardCode;
				dataTable4.Rows.Add(dataRow4);
			}
			dataSet.Tables.Add(dataTable4);
			DataTable dataTable5 = new DataTable("TaxCode");
			dataTable5 = VatGroup(string.Empty);
			dataSet.Tables.Add(dataTable5);
			return dataSet;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDataCombobox  FAILED ", ex.Message);
			return new DataSet();
		}
	}

	public DataTable ForeCast()
	{
		try
		{
			new DataTable();
			return ExecuteDataTable("APZ_Common_ForeCast", CommandType.StoredProcedure, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "ForeCast FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetMRP()
	{
		try
		{
			new DataTable();
			return ExecuteDataTable("APZ_Common_GetMRP", CommandType.StoredProcedure, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetMRP  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetSchedule()
	{
		try
		{
			new DataTable();
			return ExecuteDataTable("APZ_Common_GetSchedule", CommandType.StoredProcedure, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetSchedule   FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetProductionOrder()
	{
		try
		{
			new DataTable();
			return ExecuteDataTable("APZ_Common_GetProductionOrder", CommandType.StoredProcedure, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetProductionOrder   FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetRequester(string requester)
	{
		try
		{
			new DataTable();
			string text = "SELECT \"Requester\", \"ReqName\" FROM OPRQ WHERE 1 = 1 ";
			if (!string.IsNullOrEmpty(requester))
			{
				text = text + " AND \"Requester\" ='" + requester + "'";
			}
			text += ";";
			return ExecuteDataTable(text, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetRequester   FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetAcctCode(string acctCode)
	{
		try
		{
			new DataTable();
			string text = "SELECT \"AcctCode\", \"AcctName\" FROM OACT WHERE 1 = 1 ";
			if (!string.IsNullOrEmpty(acctCode))
			{
				text = text + " AND \"AcctCode\" ='" + acctCode + "'";
			}
			text += ";";
			return ExecuteDataTable(text, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetAcctCode   FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public List<DocumentsImpl> ListTranferRequest(ApzObjectTypes type)
	{
		List<DocumentsImpl> list = new List<DocumentsImpl>();
		try
		{
			string query = "SELECT * From OWTQ WHERE \"DocStatus\"='O';";
			foreach (DataRow row in ExecuteDataTable(query, CommandType.Text).Rows)
			{
				list.Add(new DocumentsImpl
				{
					DocEntry = Function.ParseInt(row["DocEntry"]),
					DocNum = Function.ParseInt(row["DocNum"]),
					DocType = Function.ToString(row["DocType"]),
					Canceled = Function.ToString(row["CANCELED"]),
					HandWritten = Function.ToString(row["Handwrtten"]),
					DocStatus = Function.ToString(row["DocStatus"]),
					ObjType = Function.ParseInt(row["ObjType"]),
					DocDate = Function.ParseDateTimes(row["DocDate"]),
					DocDueDate = Function.ParseDateTimes(row["DocDueDate"]),
					CardCode = Function.ToString(row["CardCode"]),
					CardName = Function.ToString(row["CardName"]),
					Address = Function.ToString(row["Address"]),
					NumAtCard = Function.ToString(row["NumAtCard"]),
					VatSum = Function.ParseDouble(row["VatSum"]),
					VatSumFc = Function.ParseDouble(row["VatSumFC"]),
					DiscPrcnt = Function.ParseDouble(row["DiscPrcnt"]),
					TotalDiscount = Function.ParseDouble(row["DiscSum"]),
					TotalDiscountFC = Function.ParseDouble(row["DiscSumFC"]),
					DocCur = Function.ToString(row["DocCur"]),
					DocRate = Function.ParseDouble(row["DocRate"]),
					DocTotal = Function.ParseDouble(row["DocTotal"]),
					DocTotalFC = Function.ParseDecimal(row["DocTotalFC"]),
					PaidToDate = Function.ParseDouble(row["PaidToDate"]),
					PaidFC = Function.ParseDouble(row["PaidFC"]),
					GrosProfit = Function.ParseDouble(row["GrosProfit"]),
					GrosProfFC = Function.ParseDouble(row["GrosProfFC"]),
					Ref1 = Function.ToString(row["Ref1"]),
					Ref2 = Function.ToString(row["Ref2"]),
					Comments = Function.ToString(row["Comments"]),
					JrnlMemo = Function.ToString(row["JrnlMemo"]),
					TransNum = Function.ParseInt(row["TransId"]),
					ReceiptNum = Function.ParseInt(row["TransId"]),
					GroupNum = Function.ParseInt(row["GroupNum"]),
					SlpCode = Function.ParseInt(row["SlpCode"]),
					TrnspCode = Function.ParseInt(row["TrnspCode"]),
					CntctCode = Function.ParseInt(row["CntctCode"]),
					VatSumSys = Function.ParseDouble(row["VatSumSy"]),
					TotalDiscountSC = Function.ParseDouble(row["DiscSumSy"]),
					DocTotalSys = Function.ParseDouble(row["DocTotalSy"]),
					PaidSys = Function.ParseDouble(row["PaidSys"]),
					UpdateDate = Function.ParseDateTimes(row["UpdateDate"]),
					IsICT = Function.ToString(row["IsICT"]),
					CreateDate = Function.ParseDateTimes(row["CreateDate"]),
					Series = Function.ParseInt(row["Series"]),
					TaxDate = Function.ParseDateTimes(row["TaxDate"]),
					FinncPriod = Function.ParseInt(row["FinncPriod"]),
					UserSign = Function.ParseInt(row["UserSign"]),
					UserSign2 = Function.ParseInt(row["UserSign2"]),
					VatDate = Function.ParseDateTimes(row["VatDate"]),
					PaidSum = Function.ParseDouble(row["PaidSum"]),
					PaidSumFc = Function.ParseDouble(row["PaidSumFc"]),
					PaidSumSc = Function.ParseDouble(row["PaidSumSc"]),
					Filler = Function.ToString(row["Filler"]),
					CurSource = Function.ToString(row["CurSource"]),
					isIns = Function.ToString(row["isIns"]),
					ToWhsCode = Function.ToString(row["ToWhsCode"])
				});
			}
			for (int i = 0; i < list.Count; i++)
			{
				new Document_LinesImpl().DocEntry = list[i].DocEntry;
				DataSet dataLine = new TranferRequestRespository(DatabaseService).GetDataLine(list[i].DocEntry);
				List<Document_LinesImpl> list2 = ConvertHelper.ConvertTo<Document_LinesImpl>((dataLine.Tables.Count > 0) ? dataLine.Tables[0] : new DataTable());
				if (list2 != null)
				{
					list[i].Lines = list2.Where((Document_LinesImpl p) => p.LineStatus == "O").ToList();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "ListTranferRequest   FAILED ", ex.Message);
		}
		return list;
	}

	public DataTable GetCustomer()
	{
		try
		{
			new DataTable();
			return ExecuteDataTable("SELECT \"CardCode\",\"CardName\" from OCRD; ", CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetCustomer   FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable ItemMasterTypes(string itemTypes, string userCode, int BPLId)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "APZ_ItemMaster_ItemMasterTypes";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ItemType", itemTypes));
			list.Add(new SqlParameter("@UserCode", userCode));
			list.Add(new SqlParameter("@BPLId", BPLId));
			result = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "ItemMasterTypesT   FAILED ", ex.Message);
		}
		return result;
	}

	public DataTable ItemMasterByGroup(string itemGroup, string userCode, int BPLId)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "APZ_ItemMaster_ItemMasterByGroup";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ItmsGrpCod", Function.ParseInt(itemGroup)));
			list.Add(new SqlParameter("@UserCode", userCode));
			list.Add(new SqlParameter("@BPLId", BPLId));
			result = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "ItemMasterByGroup   FAILED ", ex.Message);
		}
		return result;
	}

	protected virtual string GetProcIsExistQuery()
	{
		return "SELECT * FROM sys.procedures WHERE Name = @procName";
	}

	public virtual DataTable GetFirstRecord(string tableName, string fieldKey)
	{
		try
		{
			if (!string.IsNullOrEmpty(tableName))
			{
				string text = null;
				text = "select top 1 RN = ROW_NUMBER() OVER (ORDER BY [" + fieldKey + "]), * FROM [" + tableName + "]";
				return ExecuteDataTable(text, CommandType.Text, (SqlParameter[])null);
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetFirstRecord   FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public virtual DataTable GetLastRecord(string tableName, string columnId)
	{
		try
		{
			if (!string.IsNullOrEmpty(tableName) && !string.IsNullOrEmpty(columnId))
			{
				string text = null;
				text = "select * from " + tableName;
				text = text + " where " + columnId + "=(select max(" + columnId + ") from " + tableName + ")";
				return ExecuteDataTable(text, CommandType.Text, (SqlParameter[])null);
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetLastRecord   FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public virtual DataTable GetNextRecord(string tableName, string columnId, string value)
	{
		try
		{
			if (!string.IsNullOrEmpty(tableName) && !string.IsNullOrEmpty(columnId))
			{
				string text = null;
				text += " With CTE as (";
				text = text + " select RN = ROW_NUMBER() OVER (ORDER BY [" + columnId + "]), * FROM [" + tableName + "])\n";
				text += " SELECT [Next Row].* FROM CTE [Current Row]";
				text += " LEFT JOIN CTE [Next Row] ON ";
				text += " [Next Row].RN = [Current Row].RN + 1";
				text = text + " WHERE [Current Row].[" + columnId + "] = '" + value + "'";
				return ExecuteDataTable(text, CommandType.Text, (SqlParameter[])null);
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetPreviousRecord   FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public virtual DataTable GetPreviousRecord(string tableName, string columnId, string value)
	{
		try
		{
			if (!string.IsNullOrEmpty(tableName) && !string.IsNullOrEmpty(columnId))
			{
				string text = null;
				text += " With CTE as (";
				text = text + " select RN = ROW_NUMBER() OVER (ORDER BY [" + columnId + "]), * FROM [" + tableName + "])\n";
				text += " SELECT [Previous Row].* FROM CTE [Current Row]";
				text += " LEFT JOIN CTE [Previous Row] ON ";
				text += " [Previous Row].RN = [Current Row].RN - 1";
				text = text + " WHERE [Current Row].[" + columnId + "] = '" + value + "'";
				return ExecuteDataTable(text, CommandType.Text, (SqlParameter[])null);
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetPreviousRecord   FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable LoadSeriesByPostingDate(string usercode, ApzObjectTypes apzObjectTypes, string docSubType, string postingDate)
	{
		try
		{
			return ExecuteDataTable("APZ_Series_LoadByPostingDate", CommandType.StoredProcedure, new SqlParameter("@ObjectCode", Function.ToString((int)apzObjectTypes)), new SqlParameter("@docSubType", docSubType), new SqlParameter("@UserCode", Function.ToString(usercode)), new SqlParameter("@refDate", postingDate));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	protected virtual string LoadSeriesByPostingDateQuery()
	{
		return "SELECT Series,SeriesName,ISNULL(BPLId,-1) BPLId FROM NNM1 WHERE ObjectCode=@ObjectCode AND (BPLId IN (SELECT BPLId FROM USR6 WHERE UserCode=@UserCode) OR ISNULL(BPLId,-1)=-1) and Indicator in (select Indicator from OFPR where F_RefDate <= @refDate and T_RefDate >= @refDate)";
	}

	public DataTable ShowSeries()
	{
		try
		{
			new DataTable();
			string query = ShowSeriesQuery();
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "ShowSeries   FAILED ", ex.Message);
			return new DataTable();
		}
	}

	protected virtual string ShowSeriesQuery()
	{
		return "SELECT Series,SeriesName from NNM1 ";
	}

	public DataTable GetSeries(int ObjectCode)
	{
		try
		{
			string seriesQuery = GetSeriesQuery();
			SqlParameter sqlParameter = new SqlParameter("@ObjectCode", Function.ToString(ObjectCode));
			return ExecuteDataTable(seriesQuery, CommandType.Text, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	protected virtual string GetSeriesQuery()
	{
		return "select Series as Value, SeriesName as Name\r\n                                from nnm1 where ObjectCode = @ObjectCode";
	}

	public DataTable GetNextNumberSeries(int series)
	{
		try
		{
			string nextNumberSeriesQuery = GetNextNumberSeriesQuery();
			SqlParameter sqlParameter = new SqlParameter("@series", series);
			return ExecuteDataTable(nextNumberSeriesQuery, CommandType.Text, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	protected virtual string GetNextNumberSeriesQuery()
	{
		return "select  nextNumber from nnm1 where series = @series";
	}

	public DataTable GetListSeries(string o, int b, string docSubType)
	{
		try
		{
			new DataTable();
			string query = "APZ_DocumentNumberSeries_GetListSeriesByObject";
			return ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@ObjectCode", o), new SqlParameter("@BPLId", b), new SqlParameter("@DocSubType", docSubType));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetBranchList   FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetCodeBySeries(string s, string o, int b)
	{
		try
		{
			new DataTable();
			string query = "APZ_DocumentNumberSeries_GetCodeByObject";
			return ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@ObjectCode", o), new SqlParameter("@SeriesName", s), new SqlParameter("@BPLId", b));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetBranchList   FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public int GetMaxDocEntrySaveAsDraf()
	{
		return GetUnitOfWork().Documents.GetMaxDocEntrySaveAsDraf();
	}

	public DataTable GetFormulars(string objType)
	{
		try
		{
			string query = "apz_formular_get";
			return ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@ObjType", objType));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable ListDocByCardCode_SalesOpportunity(ApzObjectTypes type, string cardCode)
	{
		DataTable result = new DataTable();
		try
		{
			List<string> tableName = Function.GetTableName(type);
			string queryListDocByCardCode_SalesOpportunity = GetQueryListDocByCardCode_SalesOpportunity(tableName[0]);
			if (!string.IsNullOrEmpty(cardCode))
			{
				queryListDocByCardCode_SalesOpportunity += " AND (\"CardCode\" =@cardCode OR \"CardCode\" is null);";
				SqlParameter sqlParameter = new SqlParameter("@cardCode", cardCode);
				result = ExecuteDataTable(queryListDocByCardCode_SalesOpportunity, CommandType.Text, sqlParameter);
			}
			else
			{
				queryListDocByCardCode_SalesOpportunity += ";";
				result = ExecuteDataTable(queryListDocByCardCode_SalesOpportunity, CommandType.Text, (SqlParameter[])null);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "ListDocByCardCode   FAILED ", ex.Message);
		}
		return result;
	}

	protected virtual string GetQueryListDocByCardCode_SalesOpportunity(string tableName)
	{
		return $"SELECT \r\n                                            isnull(DocEntry,0) DocEntry\r\n\t\t\t\t\t\t\t\t\t\t\t,isnull(DocNum,0) DocNum\r\n\t\t\t\t\t\t\t\t\t\t\t,isnull(DocDate,convert(date,'19000101')) DocDate\r\n                                            ,isnull(DocDueDate,convert(date,'19000101')) DocDueDate\r\n\t\t\t\t\t\t\t\t\t\t\t,isnull(CardCode,'') CardCode\r\n                                            ,isnull(CardName,'') CardName\r\n\t\t\t\t\t\t\t\t\t\t\t,isnull(Comments,'') Comments\r\n\t\t\t\t\t\t\t\t\t\t\t,isnull(CntctCode,-1) CntctCode\r\n                                            ,isnull(VatSum,0) VatSum\r\n\t\t\t\t\t\t\t\t\t\t\t, isnull(DocTotal,0) DocTotal\r\n\t\t\t\t\t\t\t\t\t\t\t,isnull(DiscPrcnt,0) DiscPrcnt\r\n\t\t\t\t\t\t\t\t\t\t\t,isnull(DocRate,0) DocRate\r\n                                            FROM {tableName} WHERE 1=1 ";
	}

	public DataSet Query(string query)
	{
		DataSet dataSet = new DataSet();
		DataTable dataTable = new DataTable("Message");
		dataTable.Columns.Add("Message", typeof(string));
		dataTable.Columns.Add("MsgType", typeof(string));
		try
		{
			DataTable dataTable2 = ExecuteDataTable(query, CommandType.Text);
			DataRow dataRow = dataTable.NewRow();
			dataRow["Message"] = "";
			dataRow["MsgType"] = "200";
			dataTable.Rows.Add(dataRow);
			dataSet.Tables.Add(dataTable.Copy());
			dataSet.Tables.Add(dataTable2.Copy());
			return dataSet;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString(), query);
			DataRow dataRow2 = dataTable.NewRow();
			dataRow2["Message"] = ex.Message;
			dataRow2["MsgType"] = "5xx";
			dataTable.Rows.Add(dataRow2);
			dataSet.Tables.Add(dataTable.Copy());
			dataSet.Tables.Add(new DataTable());
			return dataSet;
		}
	}

	public DataTable GetFormularSearch(string fieldName, string tblName)
	{
		try
		{
			string query = "apz_commons_formulasearch";
			return ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@FieldName", fieldName), new SqlParameter("@TableName", tblName));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return new DataTable();
	}

	public DataTable GetCflFormulars(string objType)
	{
		try
		{
			string query = "select * from \"@FORMULAR\" WHERE \"U_ObjType\" = @objectType and \"U_FormularType\" IN ('CBX', 'CFL');";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@objectType", objType));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return new DataTable();
	}

	public string CreateStoreUdf(string tblName, string line)
	{
		try
		{
			string query = "apz_udf_createstore";
			ExecuteNonQuery(query, CommandType.StoredProcedure, new SqlParameter("@tblName", tblName), new SqlParameter("@line", line));
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
	}

	public DataTable GetListOrcCode(string p, DateTime? date)
	{
		try
		{
			string query = "apz_document_GetListOrcCode";
			return ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@DimCode", p), new SqlParameter("@DocDate", date));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return new DataTable();
	}

	public DataTable GetDimensionByCode(string p)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(p))
			{
				p = "1";
			}
			string query = "SELECT * FROM ODIM WHERE \"DimCode\" =" + p + "  AND \"DimActive\" = 'Y';";
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return new DataTable();
	}

	public DataTable GetDimensions()
	{
		try
		{
			string query = "select * from odim where \"DimActive\"='Y';";
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return new DataTable();
	}

	public DataTable GetOpportunity(int id)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "apz_document_getOpportunity";
			result = ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@OpprId", id));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return result;
	}

	public DataTable GetOpportunityDetail(int id)
	{
		try
		{
			string query = "apz_document_getOpportunity_detail";
			return ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@OpprId", id));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return new DataTable();
	}

	public DataTable GetListCardCodeByTableName(string table, string bpType)
	{
		try
		{
			string query = "apz_Commons_GetListCardCodeByTableName";
			string text = "";
			text = ((!(table == "All")) ? ("select a.CardCode from " + table + " a left join OCRD b on a.CardCode=b.CardCode where cardtype='" + bpType + "'") : ("select CardCode from OCRD where cardtype='" + bpType + "'"));
			return ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@sql", text));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return new DataTable();
	}

	public DataTable GetListContacrPersonByAllCardCode()
	{
		try
		{
			string query = "SELECT \"CntctCode\", \"Name\", \"CardCode\" FROM OCPR;";
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return new DataTable();
	}

	public DataTable WareHouse()
	{
		try
		{
			new DataTable();
			string query = "SELECT   \"WhsCode\",\r\n\t\t                                 \"WhsName\"\r\n                             FROM OWHS\r\n                             WHERE 1=1 ";
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "ListDocByCardCode   FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetWareHouseByCode(string whsCode)
	{
		try
		{
			new DataTable();
			string query = "SELECT   \"WhsCode\",\r\n\t\t                                 \"WhsName\", \"Locked\", \"BPLid\"\r\n                             FROM OWHS\r\n                             WHERE \"WhsCode\"=@whsCode;";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@whsCode", whsCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetWareHouseByCode   FAILED ", ex.Message);
			return new DataTable();
		}
	}

	private bool checkItemIsInOITW(string itemCode, string warehouse)
	{
		bool result = false;
		try
		{
			string text = "select itemcode from oitw ";
			text = text + " Where itemcode='" + itemCode + "' and whscode='" + warehouse + "'";
			if (ExecuteDataTable(text, CommandType.Text, (SqlParameter[])null).Rows.Count > 0)
			{
				result = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "UpdateWarehouseItemOnHand   FAILED ", ex.Message);
			result = false;
		}
		return result;
	}

	public List<string> getUDFFieldSAP(string table)
	{
		List<string> list = new List<string>();
		try
		{
			string query = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + table + "' and  COLUMN_NAME like 'U\\_%' escape '\\'";
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

	public DataTable GetColumnNames(string tableName)
	{
		DataTable result = new DataTable();
		try
		{
			string query = $"SELECT c.name 'ColumnName', t.Name 'DataType', c.max_length 'MaxLength',  c.precision ,c.scale ,c.is_nullable\r\n                                            FROM sys.columns c\r\n                                            INNER JOIN sys.types t ON c.user_type_id = t.user_type_id\r\n                                            WHERE c.object_id = OBJECT_ID('{tableName}')";
			result = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "DatarowToDatacolumn ", ex.Message);
		}
		return result;
	}

	public DataTable GetListUserReports(string userName, string branch)
	{
		DataTable result = new DataTable();
		try
		{
			ExecuteNonQueryNoParam(GetListUserReportsQueryInsertLayout());
			string query = "Apz_Report_GetListUserReports";
			List<SqlParameter> list = new List<SqlParameter>();
			if (!string.IsNullOrEmpty(userName))
			{
				list.Add(new SqlParameter("@userName", userName));
			}
			if (!string.IsNullOrEmpty(branch))
			{
				list.Add(new SqlParameter("@branch", branch));
			}
			result = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetListUserReport ", ex.Message);
		}
		return result;
	}

	protected virtual string GetListUserReportsQueryInsertLayout()
	{
		return "INSERT INTO APZ_PRINTLAYOUT (TypeCode, TypeName, DocCode, DocName,FileName) \r\n\t\t\t\t\t\t\t    SELECT TypeCode, N'User Reports', DocCode, DocName, N'' FROM RDOC WHERE Author != 'System'\r\n                                AND NOT EXISTS \r\n                                                (SELECT 1 FROM APZ_PRINTLAYOUT \r\n                                                 WHERE RDOC.DocCode = APZ_PRINTLAYOUT.DocCode);";
	}

	public string GetFileNameReport(string code)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("apz_report_GetFileNameReport", CommandType.StoredProcedure, new SqlParameter("@Code", code));
			if (dataTable.IsNotNull())
			{
				return Function.ToString(dataTable.Rows[0][0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetFileNameReport ", ex.Message);
		}
		return "";
	}

	public DataTable GetStoreName(string code)
	{
		try
		{
			return ExecuteDataTable("Apz_Report_GetStoreName", CommandType.StoredProcedure, new SqlParameter("@Code", code));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetStoreName ", ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetParamStore(string store, string userName, string branch)
	{
		new DataTable();
		try
		{
			string query = "Apz_Report_GetParamStore";
			List<SqlParameter> list = new List<SqlParameter>();
			if (!string.IsNullOrEmpty(userName))
			{
				list.Add(new SqlParameter("@StoreName", store));
			}
			if (!string.IsNullOrEmpty(userName))
			{
				list.Add(new SqlParameter("@userName", userName));
			}
			if (!string.IsNullOrEmpty(branch))
			{
				list.Add(new SqlParameter("@branch", branch));
			}
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetParamStore ", ex.Message);
		}
		return new DataTable();
	}

	public DataSet ExecuteStore(string sql, List<Common.ParameterValue> lstParamater)
	{
		DataSet dataSet = new DataSet();
		DataTable dataTable = new DataTable("Message");
		dataTable.Columns.Add("Message", typeof(string));
		dataTable.Columns.Add("MsgType", typeof(string));
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			if (lstParamater != null && lstParamater.Count > 0)
			{
				foreach (Common.ParameterValue item in lstParamater)
				{
					list.Add(new SqlParameter(item.Name, item.Value));
				}
			}
			dataSet = ExecuteData(sql, CommandType.StoredProcedure, list.ToArray());
			if (dataSet == null)
			{
				return new DataSet();
			}
			DataRow dataRow = dataTable.NewRow();
			dataRow["Message"] = "";
			dataRow["MsgType"] = "200";
			dataTable.Rows.Add(dataRow);
			dataSet.Tables.Add(dataTable.Copy());
			return dataSet;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message, sql);
			DataRow dataRow2 = dataTable.NewRow();
			dataRow2["Message"] = ex.Message;
			dataRow2["MsgType"] = "5xx";
			dataTable.Rows.Add(dataRow2);
			dataSet.Tables.Add(dataTable.Copy());
			dataSet.Tables.Add(new DataTable());
			return dataSet;
		}
	}

	public DataTable GetBranchList(string userCode)
	{
		try
		{
			return ExecuteDataTable("APZ_Document_GetBranchList", CommandType.StoredProcedure, new SqlParameter("@UserCode", userCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetBranchList   FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetIRSConfig()
	{
		try
		{
			new DataTable();
			string query = "select * from IRS_Config;";
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetIRSConfig   FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetIsMultiBranch()
	{
		try
		{
			new DataTable();
			string query = "select \"MltpBrnchs\" from OADM;";
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetIsMultiBranch   FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDefaultData(string userCode, int branch)
	{
		try
		{
			return ExecuteDataTable("APZ_Commons_GetDefaultData", CommandType.StoredProcedure, new SqlParameter("@userName", userCode), new SqlParameter("@branch", branch));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDefaultData   FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable CheckExistsItem(string itemCode)
	{
		try
		{
			new DataTable();
			string query = "SELECT \"ItemCode\" AS \"itemcode\" FROM OITM WHERE \"ItemCode\" ='" + itemCode + "';";
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "CheckExistsItem   FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetAllFieldTable(string tablename)
	{
		try
		{
			new DataTable();
			SqlParameter sqlParameter = new SqlParameter("@apz", tablename);
			return ExecuteDataTable(GetAllFieldTableQuery(), CommandType.Text, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "CheckExistsItem   FAILED ", ex.Message);
			return new DataTable();
		}
	}

	protected virtual string GetAllFieldTableQuery()
	{
		return "SELECT COLUMN_NAME,ORDINAL_POSITION,DATA_TYPE,CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME=@apz";
	}

	public DateTime GetDate()
	{
		try
		{
			DateTime result = DateTime.Now;
			string query = "SELECT GETDATE() AS CurrentDateTime";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				result = dataTable.Rows[0].Field<DateTime>("CurrentDateTime");
			}
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "CheckExistsItem   FAILED ", ex.Message);
			return DateTime.Now;
		}
	}

	public string GetFieldNameFromTable(bool header, string field)
	{
		try
		{
			string query = "apz_commons_getfield";
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@Header", header), new SqlParameter("@Field", field));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ToString(dataTable.Rows[0][0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return field;
	}

	public DataTable GetRecord(int ObjType, string tbl, int DocNum, string TypeRecord, string userCode)
	{
		try
		{
			string query = "APZ_Document_GetRecord";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ObjType", ObjType));
			list.Add(new SqlParameter("@tbl", tbl));
			list.Add(new SqlParameter("@DocNum", DocNum));
			list.Add(new SqlParameter("@TypeRecord", TypeRecord));
			list.Add(new SqlParameter("@userCode", userCode));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetRecord   FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public string GetServerDate()
	{
		try
		{
			DataTable dataTable = ExecuteDataTable(GetServerDateQuery(), CommandType.Text);
			if (dataTable.Rows.Count > 0)
			{
				return Function.ToString(dataTable.Rows[0][0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}

	protected virtual string GetServerDateQuery()
	{
		return "SELECT CONVERT(NVARCHAR, GETDATE(),112)";
	}

	public DataTable GetAllEmployees()
	{
		try
		{
			string query = "apz_Employee_All";
			return ExecuteDataTable(query, CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetExecute(string sql, string filter = "")
	{
		try
		{
			DataTable dataTable = ExecuteDataTable(sql, CommandType.Text);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				string filterExpression = string.Empty;
				if (!string.IsNullOrEmpty(filter))
				{
					filterExpression = GetUnitOfWork().FilterSetting.GetFilterText(filter);
				}
				return dataTable.Select(filterExpression).CopyToDataTable();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetCheckQuantityInventory(string item)
	{
		try
		{
			string query = "select distinct ItemCode,SUM( isnull(b.OnHand,0) +isnull(b.OnOrder,0)-isnull(b.IsCommited,0)) as Avaiable from OITW b WHERE ItemCode = N'" + item + "' GROUP BY ItemCode";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetCheckQuantityInventory   FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetListSQL(string filter)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("APZ_QUERY_GetDataQueries", CommandType.StoredProcedure);
			string filterText = GetUnitOfWork().FilterSetting.GetFilterText(filter);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable.Select(filterText).CopyToDataTable();
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetListSQL   FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable Wizard_SelectItem_SalesOrder(string doc)
	{
		try
		{
			string empty = string.Empty;
			empty = "apz_document_copy";
			DataTable dataTable = ExecuteDataTable(empty, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetListSQL   FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable Wizard_SelectItem_SalesQuatation(string doc)
	{
		try
		{
			string empty = string.Empty;
			empty = "SELECT * FROM QUT1 WHERE DocEntry IN (" + doc + ")";
			DataTable dataTable = ExecuteDataTable(empty, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetListSQL   FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetAllProject()
	{
		try
		{
			string query = "SELECT \"PrjCode\", \"PrjName\", * FROM OPRJ WHERE \"Active\" = 'Y';";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetQuantityInWarehouse(string item, string warehouse)
	{
		try
		{
			string query = "SELECT \"OnHand\" AS \"onhand\" FROM OITW WHERE \"ItemCode\" ='" + item + "' AND \"WhsCode\" = '" + warehouse + "';";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetCheckQuantityInventory   FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetRevenueAccount()
	{
		try
		{
			string query = "APZ_Commons_GetRevenueAccount";
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
		}
		return new DataTable();
	}

	public DataTable GetUserCode()
	{
		try
		{
			DataTable dataTable = ExecuteDataTable(GetUserCodeQuery(), CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	protected virtual string GetUserCodeQuery()
	{
		return "select USERID,USER_CODE from ousr";
	}

	public DataTable GetRole()
	{
		try
		{
			string roleQuery = GetRoleQuery();
			DataTable dataTable = ExecuteDataTable(roleQuery, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	protected virtual string GetRoleQuery()
	{
		return "SELECT \"typeID\", \"name\" FROM OHTY;";
	}

	public DataTable GetQualifications()
	{
		try
		{
			string query = "select \"Id\",\"Name\" from APZ_OHQG;";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetUDFFieldValueBOMItem(string Father, int ChildNum)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Father", Father));
			list.Add(new SqlParameter("@ChildNum", ChildNum));
			string query = "APZ_Commons_GetUDFFieldValueItem";
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
		}
		return new DataTable();
	}

	public DataTable GetFreight(string tableName, string docEntry)
	{
		try
		{
			if (!string.IsNullOrEmpty(tableName) && !string.IsNullOrEmpty(docEntry) && docEntry.Split(',').ToList().Any((string t) => Function.ParseInt(t) > 0))
			{
				List<SqlParameter> list = new List<SqlParameter>
				{
					new SqlParameter("@Tbl", tableName),
					new SqlParameter("@DocEntry", docEntry)
				};
				return ExecuteDataTable("Apz_Document_GetFreight_Sale", CommandType.StoredProcedure, list.ToArray());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetCountry(int user)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@UserSign", user));
			string query = "APZ_Country_GetData";
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
		}
		return new DataTable();
	}

	public DataTable GetState(string Code)
	{
		new DataTable();
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Code", Code));
			string query = "APZ_GetState";
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public List<Common.Table_ID> GetDatabaseList(bool loginSap, string domain)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable(GetDatabaseListQuery(loginSap), CommandType.Text, new SqlParameter("@domain", (!string.IsNullOrEmpty(domain)) ? domain : ""));
			if (dataTable.IsNotNull())
			{
				List<Common.Table_ID> list = new List<Common.Table_ID>();
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					for (int i = 0; i < dataTable.Rows.Count; i++)
					{
						list.Add(new Common.Table_ID
						{
							ID = Function.ToString(dataTable.Rows[i]["dbName"]),
							Name = Function.ToString(dataTable.Rows[i]["cmpName"])
						});
					}
				}
				return list;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new List<Common.Table_ID>();
	}

	public virtual string GetDatabaseListQuery(bool loginSap)
	{
		return "";
	}

	protected virtual string GetDatabaseFollowDomain()
	{
		return "select a.name, b.HardwareKey from sys.databases a left join \r\n                (select Domain,HardwareKey,[Database] from APZ_OLDD where Domain = @domain) b On b.[Database] COLLATE SQL_Latin1_General_CP1_CI_AS = a.name COLLATE SQL_Latin1_General_CP1_CI_AS\r\n                where name != 'master' and name !='tempdb' and name != 'model' and name != 'msdb' and (@cloudSystem = 100 or  b.Domain = @domain)";
	}

	protected virtual string GetCompanyInfoQuery(string name)
	{
		return string.Format("IF EXISTS(SELECT *\r\n                                            FROM [{0}].INFORMATION_SCHEMA.tables \r\n                                                WHERE  TABLE_NAME = 'APZ_CINF') select A.Code,B.CompnyName CmpName,B.CompnyAddr CmpAdress,1 CmpType,B.Phone1 CmpTel,A.Version,A.TextCode  \r\n                                                    from [{0}].[dbo].APZ_CINF A, [{0}].[dbo].OADM B", name);
	}

	public List<Common.Table_ID> GetNewDbList(bool cloudSystem, bool loginSap, string domain, string databaseServer, string databaseUser, string databasePass)
	{
		try
		{
			if (cloudSystem)
			{
				return GetDatabaseList(loginSap, domain);
			}
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				ExecuteNonQuery("APZ_Commons_ResreshDB", CommandType.StoredProcedure, new SqlParameter("@domain", domain), new SqlParameter("@databaseServer", databaseServer), new SqlParameter("@databaseUser", databaseUser), new SqlParameter("@databasePass", databasePass));
				dbTransaction.Commit();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return GetDatabaseList(loginSap, domain);
	}

	public DataTable LoadDoc(string tableName)
	{
		try
		{
			if (!tableName.Contains(" "))
			{
				DataTable structureTable = GetStructureTable(tableName);
				string sql = GetSql(structureTable, tableName);
				DataTable dataTable = ExecuteDataTable(sql, CommandType.Text);
				if (dataTable.IsNotNull())
				{
					return dataTable;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetStructureTable(string tableName)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			string structureTableQuery = GetStructureTableQuery();
			list.Add(new SqlParameter("@TblName", tableName));
			DataTable dataTable = ExecuteDataTable(structureTableQuery, CommandType.Text, list.ToArray());
			if (dataTable.IsNotNull())
			{
				return dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	protected virtual string GetStructureTableQuery()
	{
		return "SELECT COLUMN_NAME,DATA_TYPE,COLUMN_DEFAULT,IS_NULLABLE\r\n                FROM INFORMATION_SCHEMA.COLUMNS\r\n                WHERE TABLE_NAME = @TblName";
	}

	protected virtual string GetSql(DataTable dt, string tableName)
	{
		try
		{
			string text = "SELECT ";
			for (int i = 0; i < Function.ParseInt(dt.Rows.Count); i++)
			{
				switch (Function.ToString(dt.Rows[i]["DATA_TYPE"]).ToUpper())
				{
				case "NVARCHAR":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",'') AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text6 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 2);
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text6 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "NTEXT":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",'') AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text11 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 1);
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text11 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "DATETIME":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",GETDATE()) AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text10 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 1);
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text10 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "CHAR":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",'') AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text3 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 2);
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text3 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "BIT":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",0) AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text8 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 1);
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text8 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "INT":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",0) AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text7 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 2);
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text7 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "SMALLINT":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",0) AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text12 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 2);
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text12 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "NUMERIC":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",0) AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text9 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 1);
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text9 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "DOUBLE":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",0) AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text4 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 1);
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text4 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "MONEY":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",0) AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text5 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 1);
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text5 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				default:
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",0) AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text2 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(2, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 4);
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text2 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				}
			}
			text = text.Substring(0, text.Length - 1);
			return text + Environment.NewLine + "FROM " + tableName;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "Function get SQL ERROR ";
		}
	}

	public DataTable GetAddNewRow(string tableName, int codemax)
	{
		try
		{
			if (!tableName.Contains(" "))
			{
				DataTable structureTable = GetStructureTable(tableName);
				string sqlAddNew = GetSqlAddNew(tableName);
				DataTable dataTable = ExecuteDataTable(sqlAddNew, CommandType.Text);
				DataRow dataRow = dataTable.NewRow();
				string empty = string.Empty;
				for (int i = 0; i < structureTable.Rows.Count; i++)
				{
					if (Function.ToString(structureTable.Rows[i]["IS_NULLABLE"]).ToUpper() == "NO" && (Function.ToString(structureTable.Rows[i]["DATA_TYPE"]).ToUpper() == "INT" || Function.ToString(structureTable.Rows[i]["DATA_TYPE"]).ToUpper() == "SMALLINT"))
					{
						empty = Function.ToString(structureTable.Rows[i]["COLUMN_NAME"]);
						dataRow[empty] = codemax + 1;
					}
				}
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				if (dataTable.IsNotNull())
				{
					return dataTable;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public string GetSqlAddNew(string tableName)
	{
		try
		{
			string text = "SELECT TOP 0 * ";
			text = text.Substring(0, text.Length - 1);
			return text + Environment.NewLine + " FROM " + tableName + ";";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "Function get SQL ERROR ";
		}
	}

	public string GetSqlAddNew(DataTable dt, string tableName)
	{
		try
		{
			string text = "SELECT TOP 1";
			for (int i = 0; i < Function.ParseInt(dt.Rows.Count); i++)
			{
				switch (Function.ToString(dt.Rows[i]["DATA_TYPE"]).ToUpper())
				{
				case "NVARCHAR":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "'' AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text10 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 2);
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text10 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "DATETIME":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "GETDATE() AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text7 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 1);
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text7 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "CHAR":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "'' AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text11 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 2);
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text11 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "BIT":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "0 AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text6 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 1);
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text6 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "INT":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",0) AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text5 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 1);
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text5 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "SMALLINT":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",0) AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text4 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 1);
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text4 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "NUMERIC":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",0) AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text8 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 1);
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text8 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "DOUBLE":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",0) AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text3 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 1);
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text3 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "MONEY":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",0) AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text9 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 1);
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text9 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				default:
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",0) AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text2 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(2, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 4);
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text2 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				}
			}
			text = text.Substring(0, text.Length - 1);
			return text + Environment.NewLine + "FROM " + tableName;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "Function get SQL ERROR ";
		}
	}

	public string Save(DataTable dtDoc, string tableName)
	{
		try
		{
			if (!tableName.Contains(" "))
			{
				using (DatabaseService.OpenConnection())
				{
					if (!dtDoc.IsNotNull())
					{
						using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
						string query = "DELETE " + tableName;
						if (ExecuteNonQuery(query, CommandType.Text) <= 0)
						{
							return "No record effected";
						}
						dbTransaction.Commit();
						return string.Empty;
					}
					DataTable structureTable = GetStructureTable(tableName);
					string key = "";
					for (int i = 0; i < structureTable.Rows.Count; i++)
					{
						if (Function.ToString(structureTable.Rows[i]["IS_NULLABLE"]).ToUpper() == "NO" && (Function.ToString(structureTable.Rows[i]["DATA_TYPE"]).ToUpper() == "INT" || Function.ToString(structureTable.Rows[i]["DATA_TYPE"]).ToUpper() == "SMALLINT") && string.IsNullOrEmpty(key))
						{
							key = Function.ToString(structureTable.Rows[i]["COLUMN_NAME"]);
						}
					}
					using DbTransaction dbTransaction2 = DatabaseService.BeginTransaction();
					for (int j = 0; j < dtDoc.Rows.Count; j++)
					{
						string text = Function.ToString(dtDoc.Rows[j][key]);
						if (text.Contains(" "))
						{
							return "Values Code not allow!";
						}
						string code = "select " + key + " from " + tableName + " where " + key + "='" + text + "'";
						if (Exists(code))
						{
							string text2 = UpdateData(dtDoc.Rows[j], structureTable, tableName, key);
							if (!string.IsNullOrEmpty(text2))
							{
								return text2;
							}
						}
						else
						{
							string text3 = InsertData(dtDoc.Rows[j], structureTable, tableName);
							if (!string.IsNullOrEmpty(text3))
							{
								return text3;
							}
						}
					}
					string text4 = "";
					text4 = text4 + "SELECT CONVERT(NVARCHAR," + key + ")  +',' FROM " + tableName + " FOR XML PATH('')";
					DataTable dataTable = ExecuteDataTable(text4, CommandType.Text);
					if (dataTable.IsNotNull())
					{
						string text5 = Function.ToString(dataTable.Rows[0][0]);
						if (!string.IsNullOrEmpty(text5))
						{
							List<string> list = (from t in dtDoc.AsEnumerable()
								select Function.ToString(t[key])).ToList();
							List<string> list2 = text5.Split(',').ToList();
							for (int num = 0; num < list2.Count; num++)
							{
								if (!list.Contains(list2[num]) && !string.IsNullOrEmpty(list2[num]))
								{
									string text6 = DeleteData(list2[num], tableName, key);
									if (!string.IsNullOrEmpty(text6))
									{
										return text6;
									}
								}
							}
						}
					}
					dbTransaction2.Commit();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return string.Empty;
	}

	private string DeleteData(string key, string tableName, string keyname)
	{
		try
		{
			string query = "DELETE " + tableName + " WHERE " + keyname + "='" + key + "'";
			if (ExecuteNonQuery(query, CommandType.Text) <= 0)
			{
				return "No record effected";
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string InsertData(DataRow dataRow, DataTable dtStr, string tableName)
	{
		try
		{
			string text = "INSERT INTO " + tableName + " ( ";
			for (int i = 0; i < dtStr.Rows.Count; i++)
			{
				string text2 = Function.ToString(dtStr.Rows[i]["COLUMN_NAME"]);
				if (!string.IsNullOrEmpty(Function.ToString(dataRow[text2])))
				{
					text = text + text2 + ",";
				}
			}
			text = text.Substring(0, text.Length - 1);
			text += " )   VALUES (";
			for (int j = 0; j < dtStr.Rows.Count; j++)
			{
				string text3 = Function.ToString(dtStr.Rows[j]["COLUMN_NAME"]);
				if (!string.IsNullOrEmpty(Function.ToString(dataRow[text3])))
				{
					text = text + "@" + text3 + ",";
				}
			}
			text = text.Substring(0, text.Length - 1);
			text += " )";
			List<SqlParameter> list = MappingSqlParameter(dataRow, dtStr);
			if (ExecuteNonQuery(text, CommandType.Text, list.ToArray()) <= 0)
			{
				return "No record effected";
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private List<SqlParameter> MappingSqlParameter(DataRow dataRow, DataTable dtStr)
	{
		List<SqlParameter> list = new List<SqlParameter>();
		for (int i = 0; i < dtStr.Rows.Count; i++)
		{
			string text = Function.ToString(dtStr.Rows[i]["COLUMN_NAME"]);
			string text2 = Function.ToString(dtStr.Rows[i]["DATA_TYPE"]);
			if (dataRow.IsExistsColumn(text) && !string.IsNullOrEmpty(Function.ToString(dataRow[text])))
			{
				switch (text2.ToUpper())
				{
				case "NVARCHAR":
					list.Add(new SqlParameter("@" + text, Function.ToString(dataRow[text])));
					break;
				case "DATETIME":
					list.Add(new SqlParameter("@" + text, Function.ParseDateTime(dataRow[text])));
					break;
				case "CHAR":
					list.Add(new SqlParameter("@" + text, Function.ToString(dataRow[text])));
					break;
				case "NTEXT":
					list.Add(new SqlParameter("@" + text, Function.ToString(dataRow[text])));
					break;
				case "SMALLINT":
					list.Add(new SqlParameter("@" + text, dataRow[text]));
					break;
				case "BIT":
					list.Add(new SqlParameter("@" + text, Function.ParseBool(dataRow[text])));
					break;
				case "DOUBLE":
					list.Add(new SqlParameter("@" + text, Function.ParseDouble(dataRow[text])));
					break;
				case "DECIMAL":
					list.Add(new SqlParameter("@" + text, Function.ParseDecimal(dataRow[text])));
					break;
				case "FLOAT":
					list.Add(new SqlParameter("@" + text, Function.ParseFloat(dataRow[text])));
					break;
				case "NUMERIC":
					list.Add(new SqlParameter("@" + text, Function.ParseDecimal(dataRow[text])));
					break;
				case "INT":
					list.Add(new SqlParameter("@" + text, Function.ParseInt(dataRow[text])));
					break;
				default:
					list.Add(new SqlParameter("@" + text, Function.ParseInt(dataRow[text])));
					break;
				}
			}
		}
		return list;
	}

	private string UpdateData(DataRow dataRow, DataTable dtStr, string tableName, string key)
	{
		try
		{
			string text = "UPDATE " + tableName + " SET ";
			for (int i = 0; i < dtStr.Rows.Count; i++)
			{
				string text2 = Function.ToString(dtStr.Rows[i]["COLUMN_NAME"]);
				if (text2 != key && !string.IsNullOrEmpty(Function.ToString(dataRow[text2])))
				{
					text = text + Environment.NewLine + text2 + "=@" + text2 + ",";
				}
			}
			text = text.Substring(0, text.Length - 1);
			text = text + " where " + key + "= @" + key;
			List<SqlParameter> list = MappingSqlParameter(dataRow, dtStr);
			if (ExecuteNonQuery(text, CommandType.Text, list.ToArray()) <= 0)
			{
				return "No record effected";
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public bool Exists(string code)
	{
		try
		{
			if (ExecuteDataTable(code, CommandType.Text).IsNotNull())
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

	public DataTable GetCurrencyByListNum(int listNum)
	{
		new DataTable();
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ListNum", listNum));
			string currencyByListNum = GetCurrencyByListNum();
			return ExecuteDataTable(currencyByListNum, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	protected virtual string GetCurrencyByListNum()
	{
		return "SELECT isnull(PrimCurr, '') FROM OPLN WHERE listNum = @ListNum";
	}

	public DataTable GetTableColumns(string table)
	{
		DataTable dataTable = new DataTable();
		try
		{
			if (ExistsTable(table))
			{
				string query = "SELECT TOP 0 * FROM " + table;
				dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
				DataRow row = dataTable.NewRow();
				dataTable.Rows.Add(row);
				dataTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return dataTable;
	}

	public string GetSelectAllFieldQuery(string table)
	{
		try
		{
			DataTable fieldsNameAndType = GetFieldsNameAndType(table);
			if (fieldsNameAndType == null || fieldsNameAndType.Rows.Count == 0)
			{
				return "";
			}
			string text = "SELECT ";
			for (int i = 0; i < fieldsNameAndType.Rows.Count; i++)
			{
				string text2 = Function.ToString(fieldsNameAndType.Rows[i][0]);
				string text3 = Function.ToString(fieldsNameAndType.Rows[i][1]);
				string text4 = "";
				text4 = text3.ToLower() switch
				{
					"bigint" => "0", 
					"binary" => "''", 
					"bit" => "0", 
					"char" => "''", 
					"date" => "CONVERT(datetime,'19000101')", 
					"datetime" => "CONVERT(datetime,'19000101')", 
					"datetime2" => "CONVERT(datetime,'19000101')", 
					"datetimeoffset" => "CONVERT(datetimeoffset, '19000101 01:00:00 +07:00')", 
					"decimal" => "0.0", 
					"float" => "0.0", 
					"image" => "''", 
					"int" => "0", 
					"money" => "0.0", 
					"nchar" => "''", 
					"ntext" => "''", 
					"numeric" => "0.0", 
					"nvarchar" => "''", 
					"real" => "0.0", 
					"rowversion" => "''", 
					"smalldatetime" => "CONVERT(datetime,'19000101')", 
					"smallint" => "0", 
					"smallmoney" => "0.0", 
					"sql_variant" => "''", 
					"text" => "''", 
					"time" => " convert(time,'00:00:00')", 
					"timestamp" => "''", 
					"tinyint" => "0", 
					"uniqueidentifier" => "''", 
					"varbinary" => "''", 
					"varchar" => "''", 
					"xml" => "''", 
					_ => "''", 
				};
				text = text + "\r\n                            ISNULL(" + text2 + "," + text4 + ") " + text2 + ",";
			}
			text = text.Remove(text.Length - 1);
			return text + " FROM " + table;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "";
		}
	}

	public DataTable GetFieldsNameAndType(string table)
	{
		try
		{
			if (!ExistsTable(table))
			{
				return new DataTable();
			}
			string query = "SELECT COLUMN_NAME,DATA_TYPE\r\n                                        FROM INFORMATION_SCHEMA.COLUMNS\r\n                                        WHERE TABLE_NAME = '" + table + "'";
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	private bool ExistsTable(string table)
	{
		bool result = false;
		try
		{
			string query = ExistsTableQuery();
			if (string.IsNullOrEmpty(table))
			{
				table = "";
			}
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@TablePara", table));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				result = Function.ParseInt(dataTable.Rows[0][0]) != 0;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	protected virtual string ExistsTableQuery()
	{
		return "DECLARE @TableName VARCHAR(100) = @TablePara;\r\n                            DECLARE @Exists BIT = ( SELECT CAST( COUNT(1) AS BIT ) FROM sys.tables WHERE name = @TableName AND type = 'U' );\r\n                            select @Exists";
	}

	private bool ExistsColumn(string table, string column)
	{
		bool result = false;
		try
		{
			string query = "DECLARE @ColumnName VARCHAR(100) = @Column;\r\n                            DECLARE @TableName VARCHAR(100) = @Table;\r\n                            DECLARE @Exists BIT = ( SELECT CAST( COUNT(1) AS BIT ) FROM sys.columns  WHERE name = @ColumnName  AND Object_ID = Object_ID(@TableName) );\r\n                            select @Exists";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Table", table));
			list.Add(new SqlParameter("@Column", column));
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, list.ToArray());
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

	public DataTable GetLicenseCar(string userName, int branch)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			if (!string.IsNullOrEmpty(userName))
			{
				list.Add(new SqlParameter("@userName", userName));
			}
			if (branch != -1)
			{
				list.Add(new SqlParameter("@branch", branch));
			}
			return ExecuteDataTable("Apz_DeliveryOrder_GetLicenseCar", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataSet DocumentGetComBoTabAccounting(string condition)
	{
		DataSet result = new DataSet();
		try
		{
			string query = "APZ_Commons_DocumentGetComBoTabAccounting";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Cond", condition));
			result = ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable DocumentLoadTabAccounting(int entry, ApzObjectTypes objTypes)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "APZ_Commons_DocumentLoadTabAccounting";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@entry", entry));
			list.Add(new SqlParameter("@objType", objTypes switch
			{
				ApzObjectTypes.oReserveInvoices => 13, 
				ApzObjectTypes.oAPReserveInvoice => 18, 
				_ => (int)objTypes, 
			}));
			result = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable DocumentLoadTabAccountingColumns()
	{
		DataTable dataTable = new DataTable();
		try
		{
			string query = "SELECT top 0\r\n\t\t\t                 \"JrnlMemo\"\r\n\t\t\t                 ,\"Project\"\r\n\t\t\t                 ,\"GroupNum\"\r\n\t\t\t                 ,\"PeyMethod\"\r\n\t\t\t                 ,\"Indicator\"\r\n\t\t\t                 ,\"LicTradNum\"\r\n\t\t\t                 ,\"ImportEnt\"\r\n\t\t\t                 ,\"PayDuMonth\"\r\n\t\t\t                 ,\"ExtraMonth\"\r\n\t\t\t                 ,\"ExtraDays\"\r\n\t\t\t                 ,\"CdcOffset\"\r\n                             ,\"UseShpdGd\"\r\n                             , 0 REFDOC\r\n\t\t\t                FROM ODLN;";
			dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			DataRow row = dataTable.NewRow();
			dataTable.Rows.Add(row);
			dataTable.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return dataTable;
	}

	public DataTable CheckExistUgpEntry(int ugpEntry)
	{
		try
		{
			string query = "SELECT \"UgpEntry\" FROM OITM WHERE \"UgpEntry\" = @UgpEntry ;";
			SqlParameter sqlParameter = new SqlParameter("@UgpEntry", ugpEntry);
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, sqlParameter);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "CheckExistItemGroupCode   FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDataParamaterSql(string sql)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				if (!string.IsNullOrEmpty(sql))
				{
					sql = sql.Replace("[", "").Replace("]", "");
				}
				DataTable result = ExecuteDataTable(sql, CommandType.Text, (SqlParameter[])null);
				dbTransaction.Rollback();
				return result;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataSet ExecuteSql(string sql, List<QueryParamater> paramater, string filter = "")
	{
		DataSet dataSet = new DataSet();
		DataTable dataTable = new DataTable("Message");
		dataTable.Columns.Add("Message", typeof(string));
		dataTable.Columns.Add("MsgType", typeof(string));
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string text = sql;
				if (paramater == null)
				{
					paramater = new List<QueryParamater>();
				}
				List<SqlParameter> lstPara = new List<SqlParameter>();
				text = ExecuteSqlStandarQuery(sql, paramater, ref lstPara);
				DataTable dataTable2 = ExecuteDataTable(text, CommandType.Text, lstPara.ToArray());
				string filterExpression = string.Empty;
				if (!string.IsNullOrEmpty(filter))
				{
					filterExpression = GetUnitOfWork().FilterSetting.GetFilterText(filter);
				}
				DataRow[] array = dataTable2.Select(filterExpression);
				if (array.Length != 0)
				{
					dataTable2 = array.CopyToDataTable();
				}
				dbTransaction.Rollback();
				DataRow dataRow = dataTable.NewRow();
				dataRow["Message"] = (dataTable2.IsNotNull() ? dataTable2.Rows.Count : 0) + " row(s) affected";
				dataRow["MsgType"] = "200";
				dataTable.Rows.Add(dataRow);
				dataSet.Tables.Add(dataTable.Copy());
				dataSet.Tables.Add(dataTable2.Copy());
				return dataSet;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message, sql);
			try
			{
				DataRow dataRow2 = dataTable.NewRow();
				dataRow2["Message"] = ex.Message;
				dataRow2["MsgType"] = "5xx";
				dataTable.Rows.Add(dataRow2);
				dataSet.Tables.Add(dataTable.Copy());
				dataSet.Tables.Add(new DataTable());
				return dataSet;
			}
			catch (Exception ex2)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			}
		}
		return new DataSet();
	}

	public HttpResult ExecuteSql(string query)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				DataSet obj = ExecuteData(query, CommandType.Text);
				dbTransaction.Rollback();
				return new HttpResult(Enums.MessageCode.Success, "", obj);
			}
		}
		catch (Exception ex)
		{
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected virtual string ExecuteSqlStandarQuery(string queryText, List<QueryParamater> paramater, ref List<SqlParameter> lstPara)
	{
		for (int i = 0; i < paramater.Count; i++)
		{
			queryText = queryText.Replace("N'" + paramater[i].ParaName + "'", "@apz" + i);
			queryText = queryText.Replace("'" + paramater[i].ParaName + "'", "@apz" + i);
			queryText = queryText.Replace(paramater[i].ParaName, "@apz" + i);
		}
		for (int j = 0; j < paramater.Count; j++)
		{
			lstPara.Add(new SqlParameter("@apz" + j, paramater[j].Value ?? ""));
		}
		return queryText;
	}

	public DataSet ExecuteSqlDataSet(string sql, List<QueryParamater> paramater, string filter = "")
	{
		DataSet dataSet = new DataSet();
		DataTable dataTable = new DataTable("Message");
		dataTable.Columns.Add("Message", typeof(string));
		dataTable.Columns.Add("MsgType", typeof(string));
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string text = sql;
				if (paramater == null)
				{
					paramater = new List<QueryParamater>();
				}
				List<SqlParameter> list = new List<SqlParameter>();
				for (int i = 0; i < paramater.Count; i++)
				{
					text = text.Replace("N'" + paramater[i].ParaName + "'", "@apz" + i);
					text = text.Replace("'" + paramater[i].ParaName + "'", "@apz" + i);
					text = text.Replace(paramater[i].ParaName, "@apz" + i);
				}
				for (int j = 0; j < paramater.Count; j++)
				{
					list.Add(new SqlParameter("@apz" + j, paramater[j].Value ?? ""));
				}
				DataSet dataSet2 = ExecuteData(text, CommandType.Text, list.ToArray());
				dbTransaction.Rollback();
				DataRow dataRow = dataTable.NewRow();
				dataRow["Message"] = (dataSet2.IsNotNull() ? dataSet2.Tables.Count : 0) + " Table(s) affected";
				dataRow["MsgType"] = "200";
				dataTable.Rows.Add(dataRow);
				dataSet.Tables.Add(dataTable.Copy());
				for (int k = 0; k < dataSet2.Tables.Count; k++)
				{
					DataTable dataTable2 = dataSet2.Tables[k];
					dataSet.Tables.Add(dataTable2.Copy());
				}
				return dataSet;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message, sql);
			try
			{
				DataRow dataRow2 = dataTable.NewRow();
				dataRow2["Message"] = ex.Message;
				dataRow2["MsgType"] = "5xx";
				dataTable.Rows.Add(dataRow2);
				dataSet.Tables.Add(dataTable.Copy());
				dataSet.Tables.Add(new DataTable());
				return dataSet;
			}
			catch (Exception ex2)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			}
		}
		return new DataSet();
	}

	public DataSet ExecuteSqlStoredProcedure(string sql, List<QueryParamater> paramater)
	{
		DataSet dataSet = new DataSet();
		DataTable dataTable = new DataTable("Message");
		dataTable.Columns.Add("Message", typeof(string));
		dataTable.Columns.Add("MsgType", typeof(string));
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				List<SqlParameter> list = new List<SqlParameter>();
				foreach (QueryParamater item in paramater)
				{
					list.Add(new SqlParameter(item.ParaName, item.Value));
				}
				DataTable dataTable2 = ExecuteDataTable(sql, CommandType.StoredProcedure, list.ToArray());
				dbTransaction.Rollback();
				DataRow dataRow = dataTable.NewRow();
				dataRow["Message"] = (dataTable2.IsNotNull() ? dataTable2.Rows.Count : 0) + " row(s) affected";
				dataRow["MsgType"] = "200";
				dataTable.Rows.Add(dataRow);
				dataSet.Tables.Add(dataTable.Copy());
				dataSet.Tables.Add(dataTable2.Copy());
				return dataSet;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message, sql);
			try
			{
				DataRow dataRow2 = dataTable.NewRow();
				dataRow2["Message"] = ex.Message;
				dataRow2["MsgType"] = "5xx";
				dataTable.Rows.Add(dataRow2);
				dataSet.Tables.Add(dataTable.Copy());
				dataSet.Tables.Add(new DataTable());
				return dataSet;
			}
			catch (Exception ex2)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			}
		}
		return new DataSet();
	}

	public DataSet ExecuteTemporyTable(string querytext, DataTable temporaryTable)
	{
		DataSet dataSet = new DataSet();
		DataTable dataTable = new DataTable("Message");
		dataTable.Columns.Add("Message", typeof(string));
		dataTable.Columns.Add("MsgType", typeof(string));
		string text = temporaryTable.TableName;
		if (string.IsNullOrEmpty(text))
		{
			text = "tbltemp";
		}
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				CreateTempTable(temporaryTable, "#" + text);
				BulkCopy(temporaryTable, "#" + text);
				DataTable dataTable2 = ExecuteDataTable(querytext, CommandType.Text);
				dbTransaction.Rollback();
				DataRow dataRow = dataTable.NewRow();
				dataRow["Message"] = (dataTable2.IsNotNull() ? dataTable2.Rows.Count : 0) + " row(s) affected";
				dataRow["MsgType"] = "200";
				dataTable.Rows.Add(dataRow);
				dataSet.Tables.Add(dataTable.Copy());
				dataSet.Tables.Add(dataTable2.Copy());
				return dataSet;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			try
			{
				DataRow dataRow2 = dataTable.NewRow();
				dataRow2["Message"] = ex.Message;
				dataRow2["MsgType"] = "5xx";
				dataTable.Rows.Add(dataRow2);
				dataSet.Tables.Add(dataTable.Copy());
				dataSet.Tables.Add(new DataTable());
				return dataSet;
			}
			catch (Exception ex2)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			}
		}
		return new DataSet();
	}

	public DataSet GetDataCategories(string userCode, string filter = "")
	{
		try
		{
			DataSet dataSet = new DataSet();
			string query = "APZ_QueryManager_GetCategory";
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@userCode", userCode));
			dataTable.TableName = "OQCN";
			dataSet.Tables.Add(dataTable);
			query = "SELECT \"IntrnalKey\" ID, \"QCategory\" AS \"Qcategory\", \"QName\" AS \"Qname\", \"QString\" AS \"Qstring\" FROM OUQR;";
			DataTable dataTable2 = ExecuteDataTable(query, CommandType.Text);
			if (!string.IsNullOrEmpty(filter))
			{
				string filterText = GetUnitOfWork().FilterSetting.GetFilterText(filter);
				DataRow[] array = dataTable2.Select(filterText);
				if (array.Length != 0)
				{
					DataTable dataTable3 = array.CopyToDataTable();
					dataTable3.TableName = "OUQR";
					dataSet.Tables.Add(dataTable3);
				}
				else
				{
					DataTable dataTable4 = dataTable2.Copy();
					dataTable4.Clear();
					dataSet.Tables.Add(dataTable4);
				}
			}
			else
			{
				dataSet.Tables.Add(dataTable2.Copy());
			}
			return dataSet;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public string SaveQuery(string sql, string name, string category)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@sql", sql),
				new SqlParameter("@queryName", name),
				new SqlParameter("@category", category)
			};
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				if (ExecuteNonQuery("apz_saveSql", CommandType.StoredProcedure, list.ToArray()) > 0)
				{
					dbTransaction.Commit();
					return "";
				}
				dbTransaction.Rollback();
				return "Could not save!";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "Could not save!";
		}
	}

	public string SaveQueryCategory(DataTable dt)
	{
		using (DatabaseService.OpenConnection())
		{
			using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
			try
			{
				if (!dt.IsNotNull())
				{
					return "No data to save!";
				}
				string query = "apz_saveCategory";
				List<SqlParameter> list = new List<SqlParameter>
				{
					new SqlParameter("@name", dt.Rows[0]["CatName"]),
					new SqlParameter("@catID", dt.Rows[0]["CategoryId"]),
					new SqlParameter("@userSign", dt.Rows[0]["UserSign"])
				};
				if (ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray()) > 0)
				{
					dbTransaction.Commit();
					return "";
				}
				dbTransaction.Rollback();
				return "Could not save!";
			}
			catch (Exception ex)
			{
				dbTransaction.Dispose();
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				return "Could not save!";
			}
		}
	}

	public DataTable GetDataStructure(string tblName)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@TblName", tblName);
			return ExecuteDataTable(GetDataStructureQuery(), CommandType.Text, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	protected virtual string GetDataStructureQuery()
	{
		return "SELECT COLUMN_NAME,COLUMN_DEFAULT,IS_NULLABLE,DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,NUMERIC_PRECISION,NUMERIC_SCALE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME=@TblName";
	}

	public DataTable GetItemOnwh(string item, string wh)
	{
		try
		{
			if (string.IsNullOrEmpty(item) || string.IsNullOrEmpty(wh))
			{
				return new DataTable();
			}
			return ExecuteDataTable(parameters: new SqlParameter[2]
			{
				new SqlParameter("@itemcode", item),
				new SqlParameter("@whscode", wh)
			}, query: GetItemOnwhQuery());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetItemOnwh   FAILED ", ex.Message);
			return new DataTable();
		}
	}

	protected virtual string GetItemOnwhQuery()
	{
		return "select isnull(\"OnHand\",0) \"InWhs\",isnull(\"IsCommited\",0) \"IsCommitedInWhs\",isnull(\"OnOrder\",0) \"OnOrderInWhs\", \"AvgPrice\" from oitw where \"ItemCode\" = @itemcode and \"WhsCode\" = @whscode;";
	}

	public DataTable GetPriceItem(int priceList, string itemcode)
	{
		try
		{
			string priceItemQuery = GetPriceItemQuery();
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@itemcode", itemcode));
			list.Add(new SqlParameter("@pricelist", priceList));
			return ExecuteDataTable(priceItemQuery, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetPriceItem", ex.Message);
			return new DataTable();
		}
	}

	protected virtual string GetPriceItemQuery()
	{
		return "select isnull(Price,0) as Price, isnull(Currency,'') as Currency from itm1 where ItemCode = @itemcode and PriceList = @pricelist";
	}

	public DataTable GetCurrency()
	{
		try
		{
			string query = "SELECT \"CurrCode\" AS \"Value\", \"CurrName\" AS \"Name\" FROM OCRN;";
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetCurrency ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetSearchTable(string table)
	{
		DataTable result = new DataTable();
		try
		{
			if (ExistsTable(table))
			{
				string searchTableQuery = GetSearchTableQuery();
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@tableName", table));
				result = ExecuteDataTable(searchTableQuery, CommandType.Text, list.ToArray());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetSearchTable  ", ex.Message);
		}
		return result;
	}

	protected virtual string GetSearchTableQuery()
	{
		return "SELECT  COLUMN_NAME+' '+DATA_TYPE+','\r\n\t                        FROM INFORMATION_SCHEMA.COLUMNS\r\n\t                        WHERE TABLE_NAME = @tableName\r\n\t                        FOR XML PATH('')";
	}

	public DataTable ExistFileInDoc(int docEntry, ApzObjectTypes FormObjectType, string fileName, string fileExt)
	{
		DataTable result = new DataTable();
		try
		{
			string tableByApzObjectType = Function.GetTableByApzObjectType(FormObjectType);
			string query = "select FileName from atc1 where AbsEntry=(select AtcEntry from " + tableByApzObjectType + " where DocEntry=@docEntry) \r\n\t                and FileExt=@fileExt and FileName=@fileName and AbsEntry is not null";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@docEntry", docEntry));
			list.Add(new SqlParameter("@fileExt", fileExt));
			list.Add(new SqlParameter("@fileName", fileName));
			result = ExecuteDataTable(query, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetSearchTable  ", ex.Message);
		}
		return result;
	}

	public DataTable ExistFileInDocOpportunity(int opprId, string fileName)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "Select U_AttFile From [@OPR62] Where docentry=(select U_Itemdetail from  [OOPR] where OpprId=@opprId) \r\n                            And U_AttFile=@fileName";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@opprId", opprId));
			list.Add(new SqlParameter("@fileName", fileName));
			result = ExecuteDataTable(query, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetSearchTable  ", ex.Message);
		}
		return result;
	}

	public DataTable GetOldBOMSalesQty(int docEntry, int lineNum, ApzObjectTypes FormObjectType)
	{
		DataTable result = new DataTable();
		try
		{
			string text = GetDocAndLinesTableName(FormObjectType).Split(',')[1];
			string query = "Select quantity From " + text + " Where docEntry=@docEntry And lineNum=@lineNum";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@docEntry", docEntry));
			list.Add(new SqlParameter("@lineNum", lineNum));
			result = ExecuteDataTable(query, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetOldBOMSalesQty  ", ex.Message);
		}
		return result;
	}

	public string GetDocAndLinesTableName(ApzObjectTypes FormObjectType)
	{
		try
		{
			if (FormObjectType <= ApzObjectTypes.BoBridge)
			{
				if (FormObjectType <= ApzObjectTypes.oClosingDateProcedure)
				{
					if (FormObjectType <= ApzObjectTypes.oBusinessPlaces)
					{
						switch (FormObjectType)
						{
						default:
							if (FormObjectType != ApzObjectTypes.oBusinessPlaces)
							{
								break;
							}
							goto end_IL_0000;
						case ApzObjectTypes.oInvoices:
							return "OINV,INV1";
						case ApzObjectTypes.oCreditNotes:
							return "ORIN,RIN1";
						case ApzObjectTypes.oDeliveryNotes:
							return "ODLN,DLN1";
						case ApzObjectTypes.oReturns:
							return "ORDN,RDN1";
						case ApzObjectTypes.oOrders:
							return "ORDR,RDR1";
						case ApzObjectTypes.oQuotations:
							return "OQUT,QUT1";
						case (ApzObjectTypes)9:
						case (ApzObjectTypes)11:
						case (ApzObjectTypes)26:
						case (ApzObjectTypes)27:
						case (ApzObjectTypes)29:
						case (ApzObjectTypes)32:
						case (ApzObjectTypes)34:
						case ApzObjectTypes.oDocumentNumbering:
						case (ApzObjectTypes)38:
						case (ApzObjectTypes)39:
						case (ApzObjectTypes)41:
						case (ApzObjectTypes)44:
						case (ApzObjectTypes)45:
						case (ApzObjectTypes)47:
						case (ApzObjectTypes)54:
						case (ApzObjectTypes)55:
						case (ApzObjectTypes)58:
						case (ApzObjectTypes)61:
						case ApzObjectTypes.oDistributionRule:
						case ApzObjectTypes.oProjectFinancials:
						case (ApzObjectTypes)72:
						case (ApzObjectTypes)74:
						case (ApzObjectTypes)75:
						case (ApzObjectTypes)76:
						case (ApzObjectTypes)79:
						case (ApzObjectTypes)80:
						case (ApzObjectTypes)82:
						case (ApzObjectTypes)83:
						case (ApzObjectTypes)84:
						case (ApzObjectTypes)85:
						case (ApzObjectTypes)86:
						case (ApzObjectTypes)87:
						case (ApzObjectTypes)88:
						case (ApzObjectTypes)89:
						case (ApzObjectTypes)90:
						case (ApzObjectTypes)92:
						case (ApzObjectTypes)94:
						case (ApzObjectTypes)95:
						case (ApzObjectTypes)96:
						case ApzObjectTypes.oInterest:
						case ApzObjectTypes.oInterestLevel:
						case (ApzObjectTypes)100:
						case ApzObjectTypes.oOpportunityReasons:
						case (ApzObjectTypes)105:
						case (ApzObjectTypes)106:
						case (ApzObjectTypes)107:
						case ApzObjectTypes.oOpportunityPartners:
						case ApzObjectTypes.oOpportunityCompetitor:
						case (ApzObjectTypes)110:
						case (ApzObjectTypes)111:
						case (ApzObjectTypes)113:
						case (ApzObjectTypes)114:
						case (ApzObjectTypes)115:
						case (ApzObjectTypes)118:
						case (ApzObjectTypes)119:
						case (ApzObjectTypes)120:
						case ApzObjectTypes.oApprovalRequests:
						case (ApzObjectTypes)122:
						case (ApzObjectTypes)123:
						case (ApzObjectTypes)124:
						case (ApzObjectTypes)129:
						case (ApzObjectTypes)130:
						case (ApzObjectTypes)131:
						case (ApzObjectTypes)132:
						case (ApzObjectTypes)133:
						case (ApzObjectTypes)135:
						case (ApzObjectTypes)136:
						case (ApzObjectTypes)137:
						case (ApzObjectTypes)139:
						case (ApzObjectTypes)141:
						case (ApzObjectTypes)148:
						case (ApzObjectTypes)149:
						case (ApzObjectTypes)154:
						case (ApzObjectTypes)155:
						case (ApzObjectTypes)157:
						case (ApzObjectTypes)159:
						case (ApzObjectTypes)161:
						case (ApzObjectTypes)167:
						case (ApzObjectTypes)168:
						case (ApzObjectTypes)169:
						case (ApzObjectTypes)172:
						case (ApzObjectTypes)173:
						case (ApzObjectTypes)174:
						case (ApzObjectTypes)175:
						case (ApzObjectTypes)177:
						case (ApzObjectTypes)179:
						case (ApzObjectTypes)180:
						case (ApzObjectTypes)181:
						case (ApzObjectTypes)183:
						case (ApzObjectTypes)184:
						case (ApzObjectTypes)185:
						case (ApzObjectTypes)186:
						case (ApzObjectTypes)187:
						case (ApzObjectTypes)188:
						case (ApzObjectTypes)192:
						case (ApzObjectTypes)195:
						case (ApzObjectTypes)196:
						case (ApzObjectTypes)197:
						case (ApzObjectTypes)199:
						case (ApzObjectTypes)207:
						case (ApzObjectTypes)208:
						case (ApzObjectTypes)209:
						case (ApzObjectTypes)210:
						case (ApzObjectTypes)213:
						case ApzObjectTypes.oUserDefinedTexts:
						case (ApzObjectTypes)216:
						case (ApzObjectTypes)220:
						case (ApzObjectTypes)222:
						case (ApzObjectTypes)225:
						case (ApzObjectTypes)226:
						case (ApzObjectTypes)227:
						case (ApzObjectTypes)228:
						case (ApzObjectTypes)230:
							break;
						case ApzObjectTypes.oNone:
						case ApzObjectTypes.oChartOfAccounts:
						case ApzObjectTypes.oBusinessPartners:
						case ApzObjectTypes.oBanks:
						case ApzObjectTypes.oItems:
						case ApzObjectTypes.oVatGroups:
						case ApzObjectTypes.oPriceLists:
						case ApzObjectTypes.oSpecialPrices:
						case ApzObjectTypes.oItemProperties:
						case ApzObjectTypes.oBusinessPartnerGroups:
						case ApzObjectTypes.oUsers:
						case ApzObjectTypes.oPurchaseInvoices:
						case ApzObjectTypes.oPurchaseCreditNotes:
						case ApzObjectTypes.oPurchaseDeliveryNotes:
						case ApzObjectTypes.oPurchaseReturns:
						case ApzObjectTypes.oPurchaseOrders:
						case ApzObjectTypes.oIncomingPayments:
						case ApzObjectTypes.oAPReserveInvoice:
						case ApzObjectTypes.oJournalVouchers:
						case ApzObjectTypes.oJournalEntries:
						case ApzObjectTypes.oStockTakings:
						case ApzObjectTypes.oContacts:
						case ApzObjectTypes.oCreditCards:
						case ApzObjectTypes.oCurrencyCodes:
						case ApzObjectTypes.oPaymentTermsTypes:
						case ApzObjectTypes.oBankPages:
						case ApzObjectTypes.oManufacturers:
						case ApzObjectTypes.oVendorPayments:
						case ApzObjectTypes.oLandedCostsCodes:
						case ApzObjectTypes.oShippingTypes:
						case ApzObjectTypes.oLengthMeasures:
						case ApzObjectTypes.oWeightMeasures:
						case ApzObjectTypes.oItemGroups:
						case ApzObjectTypes.oSalesPersons:
						case ApzObjectTypes.oCustomsGroups:
						case ApzObjectTypes.oChecksforPayment:
						case ApzObjectTypes.oInventoryGenEntry:
						case ApzObjectTypes.oInventoryGenExit:
						case ApzObjectTypes.oWarehouses:
						case ApzObjectTypes.oCommissionGroups:
						case ApzObjectTypes.oProductTrees:
						case ApzObjectTypes.oStockTransfer:
						case ApzObjectTypes.oWorkOrders:
						case ApzObjectTypes.oPurchaseLandedCost:
						case ApzObjectTypes.oCreditPaymentMethods:
						case ApzObjectTypes.oCreditCardPayments:
						case ApzObjectTypes.oAlternateCatNum:
						case ApzObjectTypes.oBudget:
						case ApzObjectTypes.oBudgetDistribution:
						case ApzObjectTypes.oMessages:
						case ApzObjectTypes.oBudgetScenarios:
						case ApzObjectTypes.oUserDefaultGroups:
						case ApzObjectTypes.oSalesOpportunities:
						case ApzObjectTypes.oSalesStages:
						case ApzObjectTypes.oActivityTypes:
						case ApzObjectTypes.oActivityLocations:
						case ApzObjectTypes.oDrafts:
						case ApzObjectTypes.oDeductionTaxHierarchies:
						case ApzObjectTypes.oDeductionTaxGroups:
						case ApzObjectTypes.oAdditionalExpenses:
						case ApzObjectTypes.oSalesTaxAuthorities:
						case ApzObjectTypes.oSalesTaxAuthoritiesTypes:
						case ApzObjectTypes.oSalesTaxCodes:
						case ApzObjectTypes.oQueryCategories:
						case ApzObjectTypes.oFactoringIndicators:
						case ApzObjectTypes.oPaymentsDrafts:
						case ApzObjectTypes.oAccountSegmentations:
						case ApzObjectTypes.oAccountSegmentationCategories:
						case ApzObjectTypes.oWarehouseLocations:
						case ApzObjectTypes.oForms1099:
						case ApzObjectTypes.oInventoryCycles:
						case ApzObjectTypes.oWizardPaymentMethods:
						case ApzObjectTypes.oBPPriorities:
						case ApzObjectTypes.oDunningLetters:
						case ApzObjectTypes.oUserFields:
						case ApzObjectTypes.oUserTables:
						case ApzObjectTypes.oPickLists:
						case ApzObjectTypes.oPaymentRunExport:
						case ApzObjectTypes.oUserQueries:
						case ApzObjectTypes.oMaterialRevaluation:
						case ApzObjectTypes.oCorrectionPurchaseInvoice:
						case ApzObjectTypes.oCorrectionPurchaseInvoiceReversal:
						case ApzObjectTypes.oCorrectionInvoice:
						case ApzObjectTypes.oCorrectionInvoiceReversal:
						case ApzObjectTypes.oContractTemplates:
						case ApzObjectTypes.oEmployeesInfo:
						case ApzObjectTypes.oCustomerEquipmentCards:
						case ApzObjectTypes.oWithholdingTaxCodes:
						case ApzObjectTypes.oBillOfExchangeTransactions:
						case ApzObjectTypes.oKnowledgeBaseSolutions:
						case ApzObjectTypes.oServiceContracts:
						case ApzObjectTypes.oServiceCalls:
						case ApzObjectTypes.oUserKeys:
						case ApzObjectTypes.oQueue:
						case ApzObjectTypes.oSalesForecast:
						case ApzObjectTypes.oTerritories:
						case ApzObjectTypes.oIndustries:
						case ApzObjectTypes.oProductionOrders:
						case ApzObjectTypes.oDownPayments:
						case ApzObjectTypes.oPurchaseDownPayments:
						case ApzObjectTypes.oPackagesTypes:
						case ApzObjectTypes.oUserObjectsMD:
						case ApzObjectTypes.oTeams:
						case ApzObjectTypes.oRelationships:
						case ApzObjectTypes.oUserPermissionTree:
						case ApzObjectTypes.oActivityStatus:
						case ApzObjectTypes.oChooseFromList:
						case ApzObjectTypes.oFormattedSearches:
						case ApzObjectTypes.oAttachments2:
						case ApzObjectTypes.oUserLanguages:
						case ApzObjectTypes.oMultiLanguageTranslations:
						case ApzObjectTypes.oDynamicSystemStrings:
						case ApzObjectTypes.oHouseBankAccounts:
							goto end_IL_0000;
						}
					}
					else
					{
						switch (FormObjectType)
						{
						case ApzObjectTypes.oLocalEra:
						case ApzObjectTypes.oNotaFiscalCFOP:
						case ApzObjectTypes.oNotaFiscalCST:
						case ApzObjectTypes.oNotaFiscalUsage:
						case ApzObjectTypes.oClosingDateProcedure:
							goto end_IL_0000;
						}
					}
				}
				else
				{
					switch (FormObjectType)
					{
					case ApzObjectTypes.oBPFiscalRegistryID:
					case ApzObjectTypes.oSalesTaxInvoice:
					case ApzObjectTypes.oPurchaseTaxInvoice:
					case ApzObjectTypes.oResources:
					case ApzObjectTypes.BoRecordset:
					case ApzObjectTypes.BoBridge:
						goto end_IL_0000;
					}
				}
			}
			else
			{
				switch (FormObjectType)
				{
				case ApzObjectTypes.oReserveInvoices:
					return "OINV,INV1";
				case ApzObjectTypes.oStockTransferDraft:
				case ApzObjectTypes.oInventoryPosting:
				case ApzObjectTypes.oInventoryOpenBalance:
				case ApzObjectTypes.oPurchaseQuotations:
				case ApzObjectTypes.oInventoryTransferRequest:
				case ApzObjectTypes.oBlanketAgreement:
				case ApzObjectTypes.oPurchaseRequest:
				case ApzObjectTypes.oInventoryCounting:
					goto end_IL_0000;
				}
			}
			return "ORDR,RDR1";
			end_IL_0000:;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "ORDR,RDR1";
	}

	public DataTable DocumentPrice(DataTable dt)
	{
		try
		{
			return ExecuteDataTable(parameters: new SqlParameter[6]
			{
				new SqlParameter("@CardCode", Function.ToString(dt.Rows[0]["CardCode"])),
				new SqlParameter("@DocDate", Function.ParseDateTime(dt.Rows[0]["DocDate"]) ?? DateTime.Today),
				new SqlParameter("@ItemCode", Function.ToString(dt.Rows[0]["ItemCode"])),
				new SqlParameter("@Quantity", Function.ParseDouble(dt.Rows[0]["Quantity"])),
				new SqlParameter("@uomCode", Function.ToString(dt.Rows[0]["UomCode"])),
				new SqlParameter("@infoPriceList", Function.ParseInt(dt.Rows[0]["InfoPriceList"]))
			}, query: DocumentPriceQuery());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	protected virtual string DocumentPriceQuery()
	{
		return "SELECT * FROM dbo.apz_document_price(@CardCode,@DocDate,@ItemCode,@Quantity, @uomCode, @infoPriceList)";
	}

	public DataTable GetDocLineUDFNameAndType()
	{
		try
		{
			return ExecuteDataTable("apz_document_GetDocLineUDFNameAndType", CommandType.StoredProcedure, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetDocUDFNameAndType()
	{
		try
		{
			return ExecuteDataTable("apz_document_GetDocUDFNameAndType", CommandType.StoredProcedure, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetSettingSap()
	{
		try
		{
			return ExecuteDataTable("APZ_Common_GetSeetingSap", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDataBrach(string userCode)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@userCode", userCode);
			return ExecuteDataTable("APZ_Commons_GetDataBrach", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable CheckExistValidValue(string queryID, string value)
	{
		try
		{
			List<ExistValidColumnQueryEntity> list = new ExistValidColumnQueryEntity[5]
			{
				new ExistValidColumnQueryEntity("CheckExistValidProject", isString: true, "PrjCode", "OPRJ", " And \"Active\"='Y'"),
				new ExistValidColumnQueryEntity("CheckExistValidItemCode", isString: true, "ItemCode", "OITM", ""),
				new ExistValidColumnQueryEntity("CheckExistValidUserCode", isString: true, "USER_CODE", "OUSR", " And \"Locked\"='N'"),
				new ExistValidColumnQueryEntity("CheckExistValidMRPLineCode", isString: true, "LineCode", "APZ_OLIN", ""),
				new ExistValidColumnQueryEntity("CheckExistValidVendor", isString: true, "CardCode", "OCRD", " And \"CardType\"='S'")
			}.ToList();
			if (list.Exists((ExistValidColumnQueryEntity t) => t.TypeID == queryID))
			{
				ExistValidColumnQueryEntity existValidColumnQueryEntity = list.FirstOrDefault((ExistValidColumnQueryEntity t) => t.TypeID == queryID);
				if (existValidColumnQueryEntity != null)
				{
					string text = "SELECT \"" + existValidColumnQueryEntity.Column + "\" FROM " + existValidColumnQueryEntity.Table + " WHERE \"" + existValidColumnQueryEntity.Column + "\" =@value";
					text = text + existValidColumnQueryEntity.Condition + ";";
					SqlParameter sqlParameter = new SqlParameter("@value", Function.ToString(value));
					return ExecuteDataTable(text, CommandType.Text, sqlParameter);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetDataPricelist()
	{
		try
		{
			return ExecuteDataTable("APZ_Commons_GetDataPricelist", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public string GetItemNameByCode(string code)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@Code", code);
			DataTable dataTable = ExecuteDataTable("APZ_Commons_GetItemNameByCode", CommandType.StoredProcedure, sqlParameter);
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				return "";
			}
			return Function.ToString(dataTable.Rows[0]["Name"]);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "";
		}
	}

	public string GetBPCurrency(string cardCode)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@cardCode", cardCode);
			DataTable dataTable = ExecuteDataTable("SELECT \"Currency\" FROM OCRD WHERE \"CardCode\"=@cardCode;", CommandType.Text, sqlParameter);
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				return "";
			}
			return Function.ToString(dataTable.Rows[0][0]);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "";
		}
	}

	public string GetNewDocNum(int objType, int series, int bplId, int usersign)
	{
		try
		{
			return GetObjectNum((ApzObjectTypes)objType, bplId, series, usersign);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "0";
	}

	public DataTable GetDataTaxCode()
	{
		try
		{
			return ExecuteDataTable("SELECT \"Code\", \"Name\" FROM OVTG WHERE \"Rate\" = 0;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDataItemByCode(string itemCode, int priceList, string userName, int branch)
	{
		try
		{
			return ExecuteDataTable("APZ_Commons_GetDataItemByCode", CommandType.StoredProcedure, new SqlParameter("@itemCode", itemCode), new SqlParameter("@priceList", priceList), new SqlParameter("@userName", userName), new SqlParameter("@branch", branch));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataSet ReceiptIssueBinData(string batchNo, string bin, ApzObjectTypes objtype)
	{
		try
		{
			return ExecuteData("apz_ReceiptIssueBin", CommandType.StoredProcedure, new SqlParameter("@BatchNo", Function.ToString(batchNo)), new SqlParameter("@Bin", Function.ToString(bin)), new SqlParameter("@Objtype", Function.ToString((int)objtype)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataTable GetColumnSchema(string tableName)
	{
		try
		{
			string query = "Apz_Document_GetColumnSchema";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@tableName", tableName));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetDataUomGroup()
	{
		try
		{
			return ExecuteDataTable("Select \"UgpEntry\" as \"Code\", \"UgpName\" as \"Name\" from OUGP;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public string GetUoMCodeByName(int uomGroup, string uomName)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("APZ_Commons_GetUoMCodeByName", CommandType.StoredProcedure, new SqlParameter("@uomGroup", uomGroup), new SqlParameter("@uomName", uomName));
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				return "";
			}
			return Function.ToString(dataTable.Rows[0]["UomCode"]);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "";
		}
	}

	public DataTable GetDataUomByCode(int uomGroup, string uomCode)
	{
		try
		{
			return ExecuteDataTable("APZ_Commons_GetUoMCodeByCode", CommandType.StoredProcedure, new SqlParameter("@uomGroup", uomGroup), new SqlParameter("@uomCode", uomCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetInventoryUoMByItemCode(string itemCode, Enums.ItemUomType type)
	{
		try
		{
			return ExecuteDataTable("APZ_Commons_GetInventoryUoMByItemCode", CommandType.StoredProcedure, new SqlParameter("@itemCode", itemCode), new SqlParameter("@type", (int)type));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetBaseUomByUomGroup(int uomGroup)
	{
		try
		{
			return ExecuteDataTable("APZ_Commons_GetBaseUomByUomGroup", CommandType.StoredProcedure, new SqlParameter("@uomGroup", uomGroup));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDataItemOnhand(string itemCode, string whsCode)
	{
		try
		{
			return ExecuteDataTable(parameters: new SqlParameter[2]
			{
				new SqlParameter("@itemCode", itemCode),
				new SqlParameter("@whsCode", whsCode)
			}, query: GetDataItemOnhandQuery());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	protected virtual string GetDataItemOnhandQuery()
	{
		return "select isnull(b.\"OnHand\",0) as \"OnHandItem\", isnull(b.\"IsCommited\",0) as \"IsCommittedItem\", isnull(b.\"OnOrder\",0) as \"OnOrderedItem\", \r\n                isnull(c.\"OnHand\",0) as \"OnHandWhs\", isnull(c.\"IsCommited\",0) as \"IsCommittedWhs\", isnull(c.\"OnOrder\",0) as \"OnOrderedWhs\", c.\"AvgPrice\" \r\n                from OITM b left join OITW c on b.\"ItemCode\" = c.\"ItemCode\" and c.\"WhsCode\" = @whsCode where b.\"ItemCode\" = @itemCode";
	}

	public DataTable GetDataBPByCode(string cardCode)
	{
		try
		{
			return ExecuteDataTable("APZ_Commons_GetDataBPByCode", CommandType.StoredProcedure, new SqlParameter("@cardCode", cardCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable VirDocumentGetBPPaymentTerm(string cardCode)
	{
		try
		{
			return ExecuteDataTable("SELECT \"Discount\", \"GroupNum\", \"ListNum\" FROM OCRD WHERE \"CardCode\" =  @cardCode;", CommandType.Text, new SqlParameter("@cardCode", cardCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable VirDocumentGetItemPriceByListNum(int listNum, string itemCode)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ListNum", listNum));
			list.Add(new SqlParameter("@ItemCode", itemCode));
			return ExecuteDataTable("APZ_Documents_GetItemPriceByListNum", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetPaymentTermByGroupNum(int groupNum)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@GroupNum", groupNum));
			return ExecuteDataTable("SELECT * FROM OCTG WHERE \"GroupNum\"=@GroupNum;", CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public string GetDatabaseVersion()
	{
		try
		{
			DataTable dataTable = ExecuteDataTable(GetDatabaseVersionQuery(), CommandType.Text);
			if (dataTable == null || dataTable.Rows.Count != 1)
			{
				return "";
			}
			return Function.Decrypt(RSA.PrivateKey, Function.ToString(dataTable.Rows[0][0]));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "";
		}
	}

	protected virtual string GetDatabaseVersionQuery()
	{
		return "SELECT [VERSION] FROM APZ_CINF";
	}

	public DataTable ValidAttachmentEntry(int atcEntry)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@AtcEntry", atcEntry));
			return ExecuteDataTable("SELECT 1 FROM OATC a INNER JOIN ATC1 b ON a.\"AbsEntry\" = b.\"AbsEntry\" WHERE a.\"AbsEntry\"=@AtcEntry;", CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable CreateFreightGridDatasource(string vatType)
	{
		try
		{
			string query = "SELECT T0.*,T1.\"Rate\" \"RateI\",T2.\"Rate\" \"RateO\" FROM OEXD T0 left join OVTG T1 on T0.\"VatGroupI\"=T1.\"Code\" left join OVTG T2 on T0.\"VatGroupO\"=T2.\"Code\";";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				query = "Select top 0 *,0.0 \"TaxAmount\",N'' \"ExpnsName\" From RDR3;";
				DataTable dataTable2 = ExecuteDataTable(query, CommandType.Text);
				foreach (DataRow row in dataTable.Rows)
				{
					DataRow dataRow2 = dataTable2.NewRow();
					dataRow2["ExpnsCode"] = Function.ToString(row["ExpnsCode"]);
					dataRow2["ExpnsName"] = Function.ToString(row["ExpnsName"]);
					dataRow2["VatGroup"] = ((vatType == "I") ? Function.ToString(row["VatGroupI"]) : Function.ToString(row["VatGroupO"]));
					dataRow2["DistrbMthd"] = Function.ToString(row["DistrbMthd"]);
					dataRow2["WTLiable"] = Function.ToString(row["WTLiable"]);
					dataRow2["LineTotal"] = Convert.ToDouble(0);
					dataRow2["Comments"] = "";
					dataRow2["VatPrcnt"] = ((vatType == "I") ? Function.ParseDouble(row["RateI"]) : Function.ParseDouble(row["RateO"]));
					dataRow2["VatSum"] = Convert.ToDouble(0);
					dataRow2["GrsAmount"] = Convert.ToDouble(0);
					dataRow2["OcrCode"] = "";
					dataRow2["OcrCode2"] = "";
					dataRow2["OcrCode3"] = "";
					dataRow2["OcrCode4"] = "";
					dataRow2["OcrCode5"] = "";
					dataRow2["Project"] = "";
					dataRow2["Status"] = "O";
					dataRow2["TaxAmount"] = Convert.ToDouble(0);
					dataRow2["Stock"] = Function.ToString(row["Stock"]);
					dataRow2["LstPchPrce"] = Function.ToString(row["LstPchPrce"]);
					dataTable2.Rows.Add(dataRow2);
					dataTable2.AcceptChanges();
				}
				return dataTable2;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public string GetHardwarekey(bool isCloudSystem, string database)
	{
		try
		{
			if (!isCloudSystem)
			{
				return Function.GetHardwareKey();
			}
			DataTable dataTable = ExecuteDataTable(GetHardwarekeyQuery(), CommandType.Text, new SqlParameter("@database", database));
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				return "";
			}
			return Function.ToString(dataTable.Rows[0][0]);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "";
		}
	}

	protected virtual string GetHardwarekeyQuery()
	{
		return "SELECT [HardwareKey] FROM APZ_OLDD WHERE [Database] = @database";
	}

	public string CheckExitsDatabase(string databaseName)
	{
		try
		{
			return Function.Decrypt(RSA.PrivateKey, Function.ToString(ExecuteScalar(GetCheckExitsDatabaseQuery(), CommandType.Text)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}

	protected virtual string GetCheckExitsDatabaseQuery()
	{
		return "";
	}

	public DataTable GetInfoCompanyDbServer(string databaseName)
	{
		try
		{
			return ExecuteDataTable(GetInfoCompanyDbServerQuery(), CommandType.Text, new SqlParameter("@cmpDb", databaseName));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return null;
	}

	public virtual string GetInfoCompanyDbServerQuery()
	{
		return "";
	}

	public DataTable GetDTGeneralSettings()
	{
		DataTable result = new DataTable();
		try
		{
			string query = "select * from OADM;";
			result = ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable GetDataBatch(string batNo, string itemCode, string whsCode)
	{
		try
		{
			return ExecuteDataTable("APZ_Commons_GetDataBatch", CommandType.StoredProcedure, new SqlParameter("@batNo", batNo), new SqlParameter("@itemCode", itemCode), new SqlParameter("@whsCode", whsCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDataBatchAndSerial(int absEntry, string distNumber, string itemCode, string whsCode)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			if (string.IsNullOrEmpty(distNumber))
			{
				distNumber = "";
			}
			if (string.IsNullOrEmpty(itemCode))
			{
				itemCode = "";
			}
			if (string.IsNullOrEmpty(whsCode))
			{
				whsCode = "";
			}
			list.Add(new SqlParameter("@absEntry", absEntry));
			list.Add(new SqlParameter("@distNumber", distNumber));
			list.Add(new SqlParameter("@itemCode", itemCode));
			list.Add(new SqlParameter("@whsCode", whsCode));
			return ExecuteDataTable("APZ_Commons_GetDataBatchAndSerial", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public virtual DataTable ExistsTransaction()
	{
		try
		{
			return ExecuteDataTable("APZ_Commons_Exists_Transactions", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetDataTypeObject(ApzObjectTypes objType)
	{
		try
		{
			return ExecuteDataTable("SELECT \"TypeCode\" AS \"Code\", \"TypeName\" AS \"Name\" FROM APZ_OTCF WHERE \"ObjectCode\" = @objType;", CommandType.Text, new SqlParameter("@objType", Function.ToString((int)objType)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public string FileAttachmentProcess(DataTable dtFile, int absEntry)
	{
		try
		{
			CreateTempTable(dtFile, "#APZ_ATTACHFILE");
			BulkCopy(dtFile, "#APZ_ATTACHFILE");
			DataTable dataTable = ExecuteDataTable("APZ_Process_FileAttachment", CommandType.StoredProcedure, new SqlParameter("@AbsEntry", Function.ParseInt(absEntry)));
			if (dataTable.IsNotNull())
			{
				if (Function.ParseInt(dataTable.Rows[0][0]) == 0)
				{
					return "ERR:1001 - Save file Error (AbsEntry return must greater than zero)";
				}
				return Function.ToString(dataTable.Rows[0][0]);
			}
			return "ERR:1002 - Save file Error (no AbsEntry return)";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "ERR:1003 - Exception occured:" + ex.Message;
		}
	}

	public HttpResult CheckExistsByCodeObject(Enums.TypeCFL type, string code)
	{
		try
		{
			object obj = ExecuteScalar("Apz_Commons_CheckExistsObjectByCode", CommandType.StoredProcedure, new SqlParameter("@type", (int)type), new SqlParameter("@code", code));
			return new HttpResult(Enums.MessageCode.Success, "", Function.ParseInt(obj) == 1);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new HttpResult(Enums.MessageCode.Success, "", false);
	}

	public string GetDocTypeFromDocEntryForDraft(int docEntry)
	{
		try
		{
			string query = "SELECT \"ObjType\" FROM ODRF WHERE \"DocEntry\" = @DocEntry;";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@DocEntry", Function.ParseInt(docEntry)));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Convert.ToString(dataTable.Rows[0]["ObjType"]);
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return string.Empty;
		}
	}

	public DataTable ValidateDocNumDraft(string docNum, int docType)
	{
		try
		{
			docNum = docNum.Replace("'", "");
			string query = "SELECT \"DocNum\", \"DocEntry\", \"CardCode\" FROM ODRF WHERE \"DocNum\" LIKE @DocNum AND \"ObjType\" = @ObjType;";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@DocNum", "%" + docNum + "%"), new SqlParameter("@ObjType", docType));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return new DataTable();
	}

	public DataTable ValidateDocNumByDocType(string docNum, ApzObjectTypes docType)
	{
		try
		{
			docNum = docNum.Replace("'", "");
			string tableName = Function.GetTableName(docType)[0];
			return ExecuteDataTable(docType switch
			{
				ApzObjectTypes.oItems => $"SELECT \"ItemCode\" as \"DocNum\", \"ItemCode\" as \"DocEntry\",  \"CardCode\" FROM OITM WHERE \"ItemCode\" LIKE @DocNum;", 
				ApzObjectTypes.oTrackingIssue => $"SELECT \"Id\" as \"DocNum\", \"Id\" as \"DocEntry\",  \"BpCode\" as \"CardCode\" FROM APZ_TGIE WHERE \"Id\" LIKE @DocNum;", 
				ApzObjectTypes.oMarketingCampaign => $"SELECT \"DocEntry\" as \"DocNum\", \"DocEntry\",  '' as \"CardCode\" FROM APZ_OSMC WHERE \"DocEntry\" LIKE @DocNum;", 
				ApzObjectTypes.oLandedCostsCodes => "SELECT \"AlcCode\" \"DocNum\", \"AlcCode\" \"DocEntry\", '' \"CardCode\" FROM OALC WHERE \"AlcCode\" LIKE @DocNum;", 
				ApzObjectTypes.oBlanketAgreement => "SELECT \"Number\" \"DocNum\", \"AbsID\" \"DocEntry\", \"BpCode\" \"CardCode\" FROM OOAT WHERE \"Number\" LIKE @DocNum;", 
				_ => ValidateDocNumByDocTypeQuery(tableName), 
			}, CommandType.Text, new SqlParameter("@DocNum", "%" + docNum + "%"));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return new DataTable();
	}

	protected virtual string ValidateDocNumByDocTypeQuery(string tableName)
	{
		return string.Format("\r\n                            IF COL_LENGTH('{0}', 'CardCode') IS NOT NULL\r\n                            BEGIN\r\n                              SELECT DocNum, DocEntry, CardCode FROM {0} WHERE DocNum LIKE @DocNum \r\n                            END\r\n                            ELSE\r\n                            BEGIN\r\n                            SELECT DocNum, DocEntry, '' as CardCode FROM {0} WHERE DocNum LIKE @DocNum \r\n                            END", tableName);
	}

	public DataTable GetTableColumnSchema(string tableName)
	{
		try
		{
			return ExecuteDataTable(GetTableColumnQuery(), CommandType.Text, new SqlParameter("@tableName", tableName));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetDefaultSeries(string obj, string docSubType, int userId)
	{
		try
		{
			return ExecuteDataTable(GetDefaultSeriesQuery(), CommandType.Text, new SqlParameter("@objCode", obj), new SqlParameter("@docSubType", docSubType), new SqlParameter("@userId", userId));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	protected virtual string GetDefaultSeriesQuery()
	{
		return "if exists(select 1 from NNM2 where ObjectCode = @objCode and DocSubType = @docSubType and UserSign = @userId)\r\n\t\tselect top 1 Series from NNM2  where ObjectCode = @objCode and DocSubType = @docSubType and UserSign = @userId\r\n\telse\r\n\t\tselect top 1 DfltSeries from ONNM where ObjectCode = @objCode and DocSubType = @docSubType";
	}

	public string GetNewApiAddress()
	{
		try
		{
			return Function.ToString(ExecuteScalar("Select \"ApiAddress\" from APZ_OUCF where \"Status\" = 'N';", CommandType.Text));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}

	public DataTable GetDataTableNewRow(string tableName)
	{
		try
		{
			if (ExistsTable(tableName))
			{
				string dataTableNewRowQuery = GetDataTableNewRowQuery();
				return ExecuteDataTable(dataTableNewRowQuery, CommandType.Text, new SqlParameter("@TblName", tableName));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	protected virtual string GetDataTableNewRowQuery()
	{
		return "SELECT COLUMN_NAME,COLUMN_DEFAULT,IS_NULLABLE,DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME=@TblName and (IS_NULLABLE = 'NO' or COLUMN_DEFAULT is not null);";
	}

	public DataTable GetFormulaSearchByTable(string tblName)
	{
		try
		{
			string query = "apz_commons_formulasearch_bytable";
			return ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@TableName", tblName));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new DataTable();
	}

	public DataTable ProcessDefaultColumnValue(DataTable dtSource, string tableName)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("select COLUMN_NAME,DATA_TYPE,COLUMN_DEFAULT from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME=@tableName", CommandType.Text, new SqlParameter("@tableName", tableName));
			if (dataTable != null && dataTable.Rows.Count > 0 && dtSource != null && dtSource.Rows.Count > 0)
			{
				EnumerableRowCollection<DataRow> enumerableRowCollection = from r in dataTable.AsEnumerable()
					where dtSource.Columns.Contains(Function.ToString(r["COLUMN_NAME"]))
					select r;
				if (enumerableRowCollection != null && enumerableRowCollection.Count() > 0)
				{
					List<DataRow> list = enumerableRowCollection.ToList();
					for (int num = 0; num < dtSource.Rows.Count; num++)
					{
						for (int num2 = 0; num2 < list.Count; num2++)
						{
							string columnName = Function.ToString(list[num2]["COLUMN_NAME"]);
							switch (Function.ToString(list[num2]["DATA_TYPE"]))
							{
							case "nvarchar":
							case "char":
								if (string.IsNullOrEmpty(Function.ToString(dtSource.Rows[num][columnName])))
								{
									if (list[num2]["COLUMN_DEFAULT"] == null || list[num2]["COLUMN_DEFAULT"] == DBNull.Value)
									{
										dtSource.Rows[num][columnName] = DBNull.Value;
										break;
									}
									string value = Function.ToString(list[num2]["COLUMN_DEFAULT"]).Replace("('", "").Replace("')", "");
									dtSource.Rows[num][columnName] = value;
								}
								break;
							case "int":
								if (Function.ParseInt(dtSource.Rows[num][columnName]) == 0)
								{
									if (list[num2]["COLUMN_DEFAULT"] == null || list[num2]["COLUMN_DEFAULT"] == DBNull.Value)
									{
										dtSource.Rows[num][columnName] = DBNull.Value;
										break;
									}
									string obj2 = Function.ToString(list[num2]["COLUMN_DEFAULT"]).Replace("(", "").Replace(")", "");
									dtSource.Rows[num][columnName] = Function.ParseInt(obj2);
								}
								break;
							case "smallint":
								if (Function.ParseShort(dtSource.Rows[num][columnName]) == 0)
								{
									if (list[num2]["COLUMN_DEFAULT"] == null || list[num2]["COLUMN_DEFAULT"] == DBNull.Value)
									{
										dtSource.Rows[num][columnName] = DBNull.Value;
										break;
									}
									string obj = Function.ToString(list[num2]["COLUMN_DEFAULT"]).Replace("(", "").Replace(")", "");
									dtSource.Rows[num][columnName] = Function.ParseShort(obj);
								}
								break;
							case "datetime":
							{
								DateTime? dateTime = Function.ParseDateTime(dtSource.Rows[num][columnName]);
								if (dateTime.HasValue)
								{
									_ = dateTime.Value;
									if (dateTime.Value.Year >= 1950)
									{
										break;
									}
								}
								if (list[num2]["COLUMN_DEFAULT"] == null || list[num2]["COLUMN_DEFAULT"] == DBNull.Value)
								{
									dtSource.Rows[num][columnName] = DBNull.Value;
									break;
								}
								string text = Function.ToString(list[num2]["COLUMN_DEFAULT"]).Replace("('", "").Replace("')", "");
								new DateTime(Function.ParseInt(text.Substring(0, 4)), Function.ParseInt(text.Substring(4, 2)), Function.ParseInt(text.Substring(6, 2)));
								dtSource.Rows[num][columnName] = Function.ParseShort(text);
								break;
							}
							}
						}
					}
					dtSource.AcceptChanges();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return dtSource;
	}

	public string GetOverSizeColumns(DataTable dtSource, string tableName)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("select COLUMN_NAME,CHARACTER_OCTET_LENGTH from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME=@tableName AND (DATA_TYPE='char' or DATA_TYPE='nvarchar')", CommandType.Text, new SqlParameter("@tableName", tableName));
			if (dataTable != null && dataTable.Rows.Count > 0 && dtSource != null && dtSource.Rows.Count > 0)
			{
				EnumerableRowCollection<DataRow> enumerableRowCollection = from r in dataTable.AsEnumerable()
					where dtSource.Columns.Contains(Function.ToString(r["COLUMN_NAME"]))
					select r;
				if (enumerableRowCollection != null && enumerableRowCollection.Count() > 0)
				{
					List<DataRow> list = enumerableRowCollection.ToList();
					for (int num = 0; num < dtSource.Rows.Count; num++)
					{
						for (int num2 = 0; num2 < list.Count; num2++)
						{
							string text = Function.ToString(list[num2]["COLUMN_NAME"]);
							int num3 = Function.ParseInt(list[num2]["CHARACTER_OCTET_LENGTH"]);
							string text2 = Function.ToString(dtSource.Rows[num][text]);
							if (num3 < text2.Length)
							{
								return "[column]:'" + text + "' [Value]:'" + text2 + "'";
							}
						}
					}
				}
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public DataTable GetDocumentHeader(int docNum, int type)
	{
		try
		{
			string docAndLinesTableName = GetDocAndLinesTableName((ApzObjectTypes)type);
			if (!string.IsNullOrEmpty(docAndLinesTableName))
			{
				return ExecuteDataTable("Select * from " + docAndLinesTableName.Split(',')[0] + " Where \"DocNum\"=@docNum;", CommandType.Text, new SqlParameter("@docNum", docNum));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new DataTable();
	}

	public bool CheckExistProject(string code)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("SELECT 1 FROM OPRJ WHERE \"PrjCode\" = @code;", CommandType.Text, new SqlParameter("@code", code));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return false;
	}

	public DataTable GetDataOperationByCode(string opeCode)
	{
		try
		{
			return ExecuteDataTable("SELECT * FROM APZ_OOMD WHERE \"OpeCode\" = @code;", CommandType.Text, new SqlParameter("@code", Function.ToString(opeCode)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetPrintPreferences(ApzObjectTypes objType, string userName, int branch)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "Apz_Common_GetPrintPreferences";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@objType", Function.ToString((int)objType)));
			list.Add(new SqlParameter("@userName", userName));
			list.Add(new SqlParameter("@branch", branch));
			result = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetPrintPreferences   FAILED ", ex.Message);
		}
		return result;
	}

	public virtual string AddFile(int absEntry)
	{
		try
		{
			return Function.ToString(ExecuteScalar("APZ_Attachment_Add", CommandType.StoredProcedure, new SqlParameter("@absEntry", absEntry)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC file oatc table FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public DataTable GetCompanyInfo()
	{
		try
		{
			return ExecuteDataTable("Select * from CINF;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public Tuple<string, DataTable> GetDataByObject(ApzObjectTypes objType, string userName, string lngCode)
	{
		try
		{
			string keyField = "";
			string queryFromObjectType = GetQueryFromObjectType(objType, userName, lngCode, out keyField);
			DataTable item = ExecuteDataTable(queryFromObjectType, CommandType.Text);
			return new Tuple<string, DataTable>(keyField, item);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new Tuple<string, DataTable>("", new DataTable());
		}
	}

	private string DMSGetQueryFromObjectType(ApzObjectTypes objectType, string userName, string lngCode, out string keyField)
	{
		keyField = "";
		try
		{
			string text = "";
			string text2 = " 1 = 1";
			switch (objectType)
			{
			case ApzObjectTypes.oItems:
				text = "OITM";
				keyField = "ItemCode";
				break;
			case ApzObjectTypes.oWarehouses:
				text = "OWHS";
				keyField = "WhsCode";
				break;
			case ApzObjectTypes.oBusinessPartners:
				text = "OCRD";
				keyField = "CardCode";
				break;
			case ApzObjectTypes.oInvoices:
				text = "OINV";
				keyField = "DocEntry";
				break;
			case ApzObjectTypes.oCreditNotes:
				text = "ORIN";
				keyField = "DocEntry";
				break;
			case ApzObjectTypes.oDeliveryNotes:
				text = "ODLN";
				keyField = "DocEntry";
				break;
			case ApzObjectTypes.oReturns:
				text = "ORDN";
				keyField = "DocEntry";
				break;
			case ApzObjectTypes.oOrders:
				text = "ORDR";
				keyField = "DocEntry";
				break;
			case ApzObjectTypes.oPurchaseInvoices:
				text = "OPCH";
				keyField = "DocEntry";
				break;
			case ApzObjectTypes.oPurchaseCreditNotes:
				text = "ORPC";
				keyField = "DocEntry";
				break;
			case ApzObjectTypes.oPurchaseDeliveryNotes:
				text = "OPDN";
				keyField = "DocEntry";
				break;
			case ApzObjectTypes.oPurchaseReturns:
				text = "ORPD";
				keyField = "DocEntry";
				break;
			case ApzObjectTypes.oPurchaseOrders:
				text = "OPOR";
				keyField = "DocEntry";
				break;
			case ApzObjectTypes.oQuotations:
				text = "OQUT";
				keyField = "DocEntry";
				break;
			case ApzObjectTypes.oIncomingPayments:
				text = "ORCT";
				keyField = "DocEntry";
				break;
			case ApzObjectTypes.oJournalEntries:
				text = "OJDT";
				keyField = "TransId";
				break;
			case ApzObjectTypes.oOutgoingPayment:
				text = "OVPM";
				keyField = "DocEntry";
				break;
			case ApzObjectTypes.oInventoryGenEntry:
				text = "OIGN";
				keyField = "DocEntry";
				break;
			case ApzObjectTypes.oInventoryGenExit:
				text = "OIGE";
				keyField = "DocEntry";
				break;
			case ApzObjectTypes.oEmployeesInfo:
				text = "OHEM";
				keyField = "empID";
				break;
			case ApzObjectTypes.oTaskManagement:
				text = "APZ_OCLG";
				keyField = "ClgCode";
				text2 = " \"Action\" = 'T'";
				break;
			case ApzObjectTypes.oNoteManagement:
				text = "APZ_OCLG";
				keyField = "ClgCode";
				text2 = " \"Action\" = 'E'";
				break;
			case ApzObjectTypes.oEventManagement:
				text = "APZ_OCLG";
				keyField = "ClgCode";
				text2 = " \"Action\" = 'V'";
				break;
			case ApzObjectTypes.oTrackingIssue:
				text = "APZ_TGIE";
				keyField = "Id";
				text2 = " \"IssueDeleted\" is null or \"IssueDeleted\" = 'N'";
				break;
			case ApzObjectTypes.oProjectManager:
				text = "APZ_OPRG";
				keyField = "AbsEntry";
				break;
			case ApzObjectTypes.oBlanketAgreement:
				text = "OOAT";
				keyField = "AbsID";
				break;
			case ApzObjectTypes.oProductionOrders:
				text = "OWOR";
				keyField = "DocEntry";
				break;
			}
			string text3 = Function.ToString(ExecuteScalar("APZ_Commons_GetSetingCflObject", CommandType.StoredProcedure, new SqlParameter("@userName", userName), new SqlParameter("@objType", objectType.ToString()), new SqlParameter("@lngCode", lngCode)));
			if (string.IsNullOrEmpty(text3))
			{
				string text4 = Function.ToString(ExecuteScalar("APZ_D_Commons_GetListColumnQueryFromObjectType", CommandType.StoredProcedure, new SqlParameter
				{
					ParameterName = "@tableData",
					Value = text
				}));
				if (!string.IsNullOrEmpty(text4))
				{
					return string.IsNullOrEmpty(text) ? "" : string.Format("select {2} from {0} where {1};", text, text2, text4);
				}
				return string.IsNullOrEmpty(text) ? "" : $"select * from {text} where {text2};";
			}
			return string.IsNullOrEmpty(text) ? "" : string.Format("select {1} from {0} where {2};", text, text3, text2);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return "";
		}
	}

	public HttpResult DMSGetDataByObject(ApzObjectTypes objType, string userName, string lngCode)
	{
		try
		{
			string keyField = "";
			string query = DMSGetQueryFromObjectType(objType, userName, lngCode, out keyField);
			DataTable item = ExecuteDataTable(query, CommandType.Text);
			return new HttpResult
			{
				MessageCode = Enums.MessageCode.Success,
				Message = "",
				Content = new Tuple<string, DataTable>(keyField, item)
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new HttpResult
			{
				MessageCode = Enums.MessageCode.ExceptionMessage,
				Message = ex.Message
			};
		}
	}

	private string GetQueryFromObjectType(ApzObjectTypes objectType, string userName, string lngCode, out string keyField)
	{
		keyField = "";
		try
		{
			string text = "";
			string text2 = " 1 = 1";
			switch (objectType)
			{
			case ApzObjectTypes.oItems:
				text = "OITM";
				keyField = "ItemCode";
				break;
			case ApzObjectTypes.oWarehouses:
				text = "OWHS";
				keyField = "WhsCode";
				break;
			case ApzObjectTypes.oBusinessPartners:
				text = "OCRD";
				keyField = "CardCode";
				break;
			case ApzObjectTypes.oInvoices:
				text = "OINV";
				keyField = "DocEntry";
				break;
			case ApzObjectTypes.oCreditNotes:
				text = "ORIN";
				keyField = "DocEntry";
				break;
			case ApzObjectTypes.oDeliveryNotes:
				text = "ODLN";
				keyField = "DocEntry";
				break;
			case ApzObjectTypes.oReturns:
				text = "ORDN";
				keyField = "DocEntry";
				break;
			case ApzObjectTypes.oOrders:
				text = "ORDR";
				keyField = "DocEntry";
				break;
			case ApzObjectTypes.oPurchaseInvoices:
				text = "OPCH";
				keyField = "DocEntry";
				break;
			case ApzObjectTypes.oPurchaseCreditNotes:
				text = "ORPC";
				keyField = "DocEntry";
				break;
			case ApzObjectTypes.oPurchaseDeliveryNotes:
				text = "OPDN";
				keyField = "DocEntry";
				break;
			case ApzObjectTypes.oPurchaseReturns:
				text = "ORPD";
				keyField = "DocEntry";
				break;
			case ApzObjectTypes.oPurchaseOrders:
				text = "OPOR";
				keyField = "DocEntry";
				break;
			case ApzObjectTypes.oQuotations:
				text = "OQUT";
				keyField = "DocEntry";
				break;
			case ApzObjectTypes.oIncomingPayments:
				text = "ORCT";
				keyField = "DocEntry";
				break;
			case ApzObjectTypes.oJournalEntries:
				text = "OJDT";
				keyField = "TransId";
				break;
			case ApzObjectTypes.oOutgoingPayment:
				text = "OVPM";
				keyField = "DocEntry";
				break;
			case ApzObjectTypes.oInventoryGenEntry:
				text = "OIGN";
				keyField = "DocEntry";
				break;
			case ApzObjectTypes.oInventoryGenExit:
				text = "OIGE";
				keyField = "DocEntry";
				break;
			case ApzObjectTypes.oEmployeesInfo:
				text = "OHEM";
				keyField = "empID";
				break;
			case ApzObjectTypes.oTaskManagement:
				text = "APZ_OCLG";
				keyField = "ClgCode";
				text2 = " \"Action\" = 'T'";
				break;
			case ApzObjectTypes.oNoteManagement:
				text = "APZ_OCLG";
				keyField = "ClgCode";
				text2 = " \"Action\" = 'E'";
				break;
			case ApzObjectTypes.oEventManagement:
				text = "APZ_OCLG";
				keyField = "ClgCode";
				text2 = " \"Action\" = 'V'";
				break;
			case ApzObjectTypes.oTrackingIssue:
				text = "APZ_TGIE";
				keyField = "Id";
				text2 = " \"IssueDeleted\" is null or \"IssueDeleted\" = 'N'";
				break;
			case ApzObjectTypes.oProjectManager:
				text = "APZ_OPRG";
				keyField = "AbsEntry";
				break;
			case ApzObjectTypes.oBlanketAgreement:
				text = "OOAT";
				keyField = "AbsID";
				break;
			case ApzObjectTypes.oProductionOrders:
				text = "OWOR";
				keyField = "DocEntry";
				break;
			case ApzObjectTypes.oProductTrees:
				text = "OITT";
				keyField = "Code";
				break;
			}
			string text3 = Function.ToString(ExecuteScalar("APZ_Commons_GetSetingCflObject", CommandType.StoredProcedure, new SqlParameter("@userName", userName), new SqlParameter("@objType", objectType.ToString()), new SqlParameter("@lngCode", lngCode)));
			string text4 = "";
			try
			{
				SqlParameter[] array = new SqlParameter[2]
				{
					new SqlParameter("@userName", userName),
					null
				};
				int num = (int)objectType;
				array[1] = new SqlParameter("@objType", num.ToString());
				text4 = Function.ToString(ExecuteScalar("APZ_GetDataByObj_ExtraCondition", CommandType.StoredProcedure, array));
			}
			catch (Exception)
			{
			}
			if (!string.IsNullOrEmpty(text4))
			{
				text2 = text2 + " and " + text4;
			}
			if (string.IsNullOrEmpty(text3))
			{
				return string.IsNullOrEmpty(text) ? "" : $"select * from {text} x where {text2};";
			}
			return string.IsNullOrEmpty(text) ? "" : string.Format("select {1} from {0} x where {2};", text, text3, text2);
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.ToString());
			return "";
		}
	}

	protected virtual string GetTableColumnQuery()
	{
		return "select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = @tableName";
	}

	public string CheckIsIns(int type, int docNum)
	{
		try
		{
			string text = "";
			if (type == 13)
			{
				text = "OINV";
			}
			if (type == 18)
			{
				text = "OPCH";
			}
			string query = "SELECT \"isIns\" FROM " + text + " WHERE \"DocEntry\"=@DocNum;";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@DocNum", docNum));
			if (dataTable.IsNotNull())
			{
				return Function.ToString(dataTable.Rows[0][0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return "";
	}

	public string CheckPosted(int type, int docEntry)
	{
		try
		{
			string text = "";
			if (type == 203)
			{
				text = "ODPI";
			}
			if (type == 204)
			{
				text = "ODPO";
			}
			string query = "SELECT \"Posted\" FROM " + text + " WHERE \"DocEntry\"=@docEntry;";
			return Function.ToString(ExecuteScalar(query, CommandType.Text, new SqlParameter("@docEntry", docEntry)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.ToString());
		}
		return "";
	}

	public HttpResult CheckDocumentDuplicateBpRefNo(ApzObjectTypes objType, string bpCode, string refNo)
	{
		try
		{
			if (string.IsNullOrEmpty(refNo))
			{
				return new HttpResult(Enums.MessageCode.Success, "", false);
			}
			string docTableByObjectType = GetDocTableByObjectType(objType);
			if (string.IsNullOrEmpty(docTableByObjectType))
			{
				return new HttpResult(Enums.MessageCode.Error, "Cannot check duplicate BP Reference No.");
			}
			string query = $"select top 1 1 from {docTableByObjectType} where \"CardCode\" = @cardCode and \"NumAtCard\" = @refNo;";
			int num = Function.ParseInt(ExecuteScalar(query, CommandType.Text, new SqlParameter("@cardCode", bpCode), new SqlParameter("@refNo", refNo)));
			return new HttpResult(Enums.MessageCode.Success, "", num == 1);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public string GetWhsDefault(string userName, int branch)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@userName", userName));
			list.Add(new SqlParameter("@branch", branch));
			DataTable dataTable = ExecuteDataTable("APZ_BOM_GetDefaultWhs", CommandType.StoredProcedure, list.ToArray());
			return dataTable.IsNotNull() ? Function.ToString(dataTable.Rows[0][0]) : string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return string.Empty;
		}
	}

	public DataTable GetListDomain()
	{
		try
		{
			string query = "select * from APZ_OCDB";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	public DataTable GetDataState(string countryCode)
	{
		try
		{
			return ExecuteDataTable("select \"Code\", \"Name\" from OCST where \"Country\" = @country;", CommandType.Text, new SqlParameter("@country", countryCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	public DataTable GetInfomationQuantityMenuDms(string userName)
	{
		try
		{
			string query = "APZ_D_GetListStore";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@cardType", Function.ToString("")));
			list.Add(new SqlParameter("@cardName", Function.ToString("")));
			list.Add(new SqlParameter("@subType", Function.ToString("S")));
			list.Add(new SqlParameter("@saleUser", Function.ToString(userName)));
			int count = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray()).Rows.Count;
			List<SqlParameter> list2 = new List<SqlParameter>();
			list2.Add(new SqlParameter("@UserName", userName));
			list2.Add(new SqlParameter("@ObjTypeTask", Function.ToString(1470000128)));
			list2.Add(new SqlParameter("@ObjTypeEvent", Function.ToString(1470000129)));
			list2.Add(new SqlParameter("@ObjTypeNote", Function.ToString(19000102)));
			list2.Add(new SqlParameter("@ActionType", ""));
			list2.Add(new SqlParameter("@ClgCode", SqlDbType.BigInt));
			list2.Add(new SqlParameter("@SeriesNum", SqlDbType.BigInt));
			list2.Add(new SqlParameter("@StartDate", DBNull.Value));
			list2.Add(new SqlParameter("@EndDate", DBNull.Value));
			int count2 = new DataView(ExecuteDataTable("APZ_SubActivities_GetByCode", CommandType.StoredProcedure, list2.ToArray()))
			{
				RowFilter = "Closed='N'"
			}.ToTable().Copy().Rows.Count;
			string query2 = "APZ_D_GetQuantityMenu";
			List<SqlParameter> list3 = new List<SqlParameter>
			{
				new SqlParameter
				{
					ParameterName = "@userName",
					Value = userName
				}
			};
			DataTable dataTable = new DataTable();
			dataTable = ExecuteDataTable(query2, CommandType.StoredProcedure, list3.ToArray());
			if (!dataTable.IsExistsColumn("QuantityStore"))
			{
				dataTable.Columns.Add("QuantityStore", typeof(int));
			}
			if (!dataTable.IsExistsColumn("QuantityTask"))
			{
				dataTable.Columns.Add("QuantityTask", typeof(int));
			}
			if (dataTable.Rows.Count > 0)
			{
				dataTable.Rows[0]["QuantityStore"] = count;
				dataTable.Rows[0]["QuantityTask"] = count2;
			}
			dataTable.AcceptChanges();
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	public DataTable GetDataKPIDms(int userCode, string KPIType)
	{
		try
		{
			new DataTable();
			string query = "APZ_D_GetKPI";
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@type", KPIType),
				new SqlParameter("@userSign", userCode)
			};
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	public DataTable GetDataConfigDMS(string appName)
	{
		try
		{
			new DataTable();
			string query = "APZ_D_GetDataConfigDMS";
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter
				{
					ParameterName = "@appName",
					Value = appName
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

	public DataTable GetDataAuthorizationToken(string tokenId)
	{
		try
		{
			return ExecuteDataTable("select * from APZ_OTPT where \"Token\" = @tokenId;", CommandType.Text, new SqlParameter("@tokenId", tokenId));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString(), tokenId);
			return new DataTable();
		}
	}

	public HttpResult DoQuery(string queryText, string userName)
	{
		try
		{
			if (string.IsNullOrEmpty(queryText))
			{
				return new HttpResult
				{
					MessageCode = Enums.MessageCode.QueryIsNull,
					Message = "Query is null"
				};
			}
			string query = $"{queryText}\r\ninsert into APZ_ODQL(\"QueryText\", \"ExecuteTime\", \"UserName\") values (@queryText, @executeTime, @userName);";
			DataTable obj = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@queryText", queryText), new SqlParameter("@executeTime", DateTime.Now), new SqlParameter("@userName", userName));
			return new HttpResult(Enums.MessageCode.Success, "", obj);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			string text = ex.Message;
			if (ex is DbException && ex.Message.ToLower().StartsWith("user-defined error"))
			{
				int startIndex = text.LastIndexOf("(at pos ");
				startIndex = text.IndexOf(':', startIndex);
				text = text.Substring(startIndex + 2, text.Length - startIndex - 2);
			}
			return new HttpResult
			{
				MessageCode = Enums.MessageCode.ExceptionMessage,
				Message = text
			};
		}
	}

	public HttpResult DoQueryWithTempTable(string queryText, string tempTableName, DataTable tempTableData, string userName)
	{
		try
		{
			if (string.IsNullOrEmpty(queryText))
			{
				return new HttpResult
				{
					MessageCode = Enums.MessageCode.QueryIsNull,
					Message = "Query is null"
				};
			}
			if (string.IsNullOrEmpty(tempTableName))
			{
				return new HttpResult
				{
					MessageCode = Enums.MessageCode.QueryIsNull,
					Message = "Temporary table name is null"
				};
			}
			if (!tempTableData.IsNotNull())
			{
				return new HttpResult
				{
					MessageCode = Enums.MessageCode.QueryIsNull,
					Message = "Temporary table is null"
				};
			}
			string text = string.Empty;
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				CreateTempTable(tempTableData, tempTableName);
				BulkCopy(tempTableData, tempTableName);
				string query = $"{queryText}\r\nInsert into APZ_ODQL(\"QueryText\", \"ExecuteTime\", \"UserName\") values (@queryText, @executeTime, @userName);";
				DataTable dataTable = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@queryText", queryText), new SqlParameter("@executeTime", DateTime.Now), new SqlParameter("@userName", userName));
				text = Function.ToString(dataTable.Rows[0][1]);
				if (Function.ParseInt(dataTable.Rows[0][0]) != 200)
				{
					dbTransaction.Rollback();
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), queryText);
					return new HttpResult
					{
						MessageCode = Enums.MessageCode.Error,
						Message = text
					};
				}
				dbTransaction.Commit();
			}
			return new HttpResult
			{
				MessageCode = Enums.MessageCode.Save,
				Content = text
			};
		}
		catch (Exception ex)
		{
			string message = ex.Message;
			if (ex is DbException)
			{
				message = "An unexpected error occurred.";
				string text2 = DatabaseService.ConvertSqlErrorMessage(ex.Message);
				if (!string.IsNullOrEmpty(text2))
				{
					message = text2;
				}
			}
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new HttpResult
			{
				MessageCode = Enums.MessageCode.ExceptionMessage,
				Message = message
			};
		}
	}

	public DataTable GetTableStructureWithDefaultValue(string tableName)
	{
		return DatabaseService.GetTableStruct(tableName);
	}

	public DataTable GetEInvoiceToken(string usercode)
	{
		return ExecuteDataTable("RA_CONNECTION_GET", CommandType.StoredProcedure, new SqlParameter("@UserName", usercode));
	}

	public HttpResult UpdateDatabase(double appVersion, int buldVersion, string foldeUpdate)
	{
		foldeUpdate += "\\Database";
		if (!Directory.Exists(foldeUpdate))
		{
			return new HttpResult(Enums.MessageCode.Error, "Folder update Irs did not exists", "");
		}
		string text = Function.ConvertDoubleToStringEnUs(appVersion, 2, 1) + "." + Function.ConvertDoubleToStringEnUs(buldVersion, 0, 3);
		string text2 = Function.ConvertDoubleToStringEnUs(Versions.ApplicationVersion, 2, 1) + "." + Function.ConvertDoubleToStringEnUs(Versions.BuildVersion, 0, 3);
		while (!text.Equals(text2))
		{
			buldVersion++;
			if (buldVersion > 999)
			{
				buldVersion = 0;
				appVersion += 0.01;
			}
			text = Function.ConvertDoubleToStringEnUs(appVersion, 2, 1) + "." + Function.ConvertDoubleToStringEnUs(buldVersion, 0, 3);
			string text3 = foldeUpdate + "\\" + text;
			if (Directory.Exists(text3))
			{
				ExecuteUpdateFolder(text3 + "\\TABLE");
				ExecuteUpdateFolder(text3 + "\\VIEW");
				ExecuteUpdateFolder(text3 + "\\TABLE_TYPE");
				ExecuteUpdateFolder(text3 + "\\TABLETYPE");
				ExecuteUpdateFolder(text3 + "\\REFERENCE_FUNCTION");
				ExecuteUpdateFolder(text3 + "\\FUNCTION");
				ExecuteUpdateFolder(text3 + "\\REFERENCE_STORE");
				ExecuteUpdateFolder(text3 + "\\REFERENCE STORE");
				ExecuteUpdateFolder(text3 + "\\STORE");
				ExecuteUpdateFolder(text3 + "\\SCRIPT");
			}
		}
		ExecuteNonQuery("update APZ_CINF set \"Version\" = @version;", CommandType.Text, new SqlParameter("@version", Function.Encrypt(RSA.PublicKey, text2)));
		return new HttpResult(Enums.MessageCode.Success);
	}

	private void ExecuteUpdateFolder(string folderUpdate)
	{
		try
		{
			if (!Directory.Exists(folderUpdate))
			{
				return;
			}
			string[] files = Directory.GetFiles(folderUpdate);
			List<string> stringFile = new List<string>();
			for (int i = 0; i < files.Length; i++)
			{
				if (!string.IsNullOrEmpty(Function.DecryptFile(files[i], out stringFile)))
				{
					continue;
				}
				for (int j = 0; j < stringFile.Count; j++)
				{
					try
					{
						ExecuteNonQueryNotParse(stringFile[j]);
					}
					catch (Exception ex)
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					}
				}
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.ToString());
		}
	}

	public DataTable GetAccessLog(string databaseName, string user, string fromDate, string toDate)
	{
		try
		{
			string accessLogQuery = GetAccessLogQuery();
			return ExecuteDataTable(accessLogQuery, CommandType.Text, new SqlParameter("@database", databaseName), new SqlParameter("@user", user), new SqlParameter("@fromDate", fromDate), new SqlParameter("@toDate", toDate));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new DataTable();
	}

	protected virtual string GetAccessLogQuery()
	{
		return "select ID, \"User\", \"LoginTime\",convert(varchar(8),\"LoginTime\",108) \"LoginHour\",\"LogoffTime\", convert(varchar(8),\"LogoffTime\",108) \"LogoffHour\", \"ClientLocalIpAddress\",\"ClientMachineName\",\"ClientWindowsLogonName\" \r\n                from apz_loglogin\r\n                where \"DataBase\" = @database\r\n                and(cast(\"LoginTime\" as date) between cast(@fromDate as date) and cast(@toDate as date)\r\n                  or cast(\"LogoffTime\" as date)  between cast(@fromDate as date) and cast(@toDate as date))\r\n                and \"User\" = @user  order by ID desc;";
	}

	public DataTable GetAccessLogUser()
	{
		try
		{
			return ExecuteDataTable("select \"USER_CODE\",\"U_NAME\",t1.\"Name\" \"Branch\",t2.\"Name\" \"Department\" from ousr t0 \r\nleft join oubr t1 on t0.\"Branch\"=t1.\"Code\"\r\nleft join oudp t2 on t0.\"Department\"=t2.\"Code\";", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new DataTable();
	}

	public DataTable GetAccessLogLastLogin(string databaseName)
	{
		try
		{
			string accessLogLastLoginQuery = GetAccessLogLastLoginQuery();
			return ExecuteDataTable(accessLogLastLoginQuery, CommandType.Text, new SqlParameter("@database", databaseName));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new DataTable();
	}

	protected virtual string GetAccessLogLastLoginQuery()
	{
		return " select y.\"User\",'' \"Name\",'' \"Branch\",'' \"Department\",y.\"LoginTime\",convert(varchar(8),y.\"LoginTime\",108) \"LoginHour\",y.\"LogoffTime\", convert(varchar(8),y.\"LogoffTime\",108) \"LogoffHour\",y.\"status\"\r\n ,y.\"ClientLocalIpAddress\",y.\"ClientMachineName\",y.\"ClientWindowsLogonName\",case when y.\"status\"='Y' then 1 else 0 end is_online \r\n from (select \"User\",Max(ID) ID from apz_loglogin  where \"DataBase\"=@database group by \"User\") x inner join apz_loglogin y on x.ID=y.ID ;";
	}

	public DataTable GetLogFormById(string formId, string userCode)
	{
		try
		{
			return ExecuteDataTable("select * from APZ_LOGFORM where \"FormId\"=@formId and \"UserCode\"=@userCode;", CommandType.Text, new SqlParameter("@formId", formId), new SqlParameter("@userCode", userCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataTable();
		}
	}

	public virtual string CheckExistColumn(string table, string column)
	{
		try
		{
			string query = "SELECT TOP 1 'Y' FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @table AND COLUMN_NAME = @column";
			if (Function.ToString(ExecuteScalar(query, CommandType.Text, new SqlParameter("@table", table), new SqlParameter("@column", column))) == "Y")
			{
				return "Y";
			}
			return "N";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return "";
		}
	}

	DataTable ICommons.ExecuteDataTable(string query, CommandType commandType, SqlParameter[] parameters)
	{
		return ExecuteDataTable(query, commandType, parameters);
	}

	int ICommons.ExecuteNonQuery(string query, CommandType commandType, SqlParameter[] parameters)
	{
		return ExecuteNonQuery(query, commandType, parameters);
	}

	DataSet ICommons.ExecuteData(string query, CommandType commandType, SqlParameter[] parameters)
	{
		return ExecuteData(query, commandType, parameters);
	}

	object ICommons.ExecuteScalar(string query, CommandType commandType, SqlParameter[] parameters)
	{
		return ExecuteScalar(query, commandType, parameters);
	}
}
