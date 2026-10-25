using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.HDbConnection;

namespace Apzon.Services;

public class BaseService<TEntity> : BaseServiceClient where TEntity : class
{
	public IDatabaseService DatabaseService { get; set; }

	public BaseService(IDatabaseService databaseService)
		: base(databaseService)
	{
		if (databaseService == null)
		{
			throw new ArgumentNullException("databaseService");
		}
		DatabaseService = databaseService;
	}

	public virtual HttpResult Process(ApzObjectTypes objtype, int bplId, Enums.TransactionType transtype, TEntity document, string objId = "", string objCode = "", bool lockNumbering = true)
	{
		try
		{
			HttpResult result = new HttpResult();
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				DatabaseService.LockNumbering(lockNumbering, objtype);
				string objNum = "";
				HttpResult httpResult = ((objtype != ApzObjectTypes.oUserDefinedObject) ? ProcessData(objtype, bplId, ref transtype, document, ref objId, ref objNum) : ProcessData(objtype, bplId, ref transtype, document, ref objId, ref objNum, objCode));
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					return new HttpResult(Enums.MessageCode.Error, httpResult.Message);
				}
				string objtype2;
				if (objtype == ApzObjectTypes.oUserDefinedObject)
				{
					objtype2 = objCode;
				}
				else
				{
					try
					{
						int num = (int)objtype;
						objtype2 = num.ToString();
					}
					catch (Exception)
					{
						objtype2 = objtype.ToString();
					}
				}
				HttpResult httpResult2 = NotifyProcedure(objtype2, transtype, -1, "", objId);
				if (!string.IsNullOrEmpty(httpResult2.Message))
				{
					return httpResult2;
				}
				DataTable dataTable = (DataTable)httpResult2.Content;
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					int num2 = Function.ParseInt(dataTable.Rows[0][0]);
					string text = Function.ToString(dataTable.Rows[0][1]);
					if (!string.IsNullOrEmpty(text))
					{
						return new HttpResult(Enums.MessageCode.Error, num2 + "-" + text);
					}
				}
				try
				{
					AddNotificationListener(objtype2, transtype, -1, "", objId);
				}
				catch (Exception ex2)
				{
					Logging.Write("ERROR", ex2);
				}
				dbTransaction.Commit();
				result = new HttpResult(Enums.MessageCode.Success, "", objNum);
			}
			Task.Run(() =>
			{
				using (DatabaseService.OpenConnection())
				{
					bool flag = true;
					using DbTransaction dbTransaction2 = DatabaseService.BeginTransaction();
					if (CommonList.ListLogInstance.TryGetValue(objtype, out var value) && value != null)
					{
						try
						{
							string query = "select top 1 \"MaxHistory\" from OADM;";
							int num3 = Function.ParseInt(ExecuteScalar(query, CommandType.Text));
							string[] array = objId.Split(';');
							if (num3 >= 0)
							{
								LogInstance logInstance = value.AsEnumerable().FirstOrDefault((LogInstance t) => t.HistoryTableName == t.TableLogInstanceKey);
								if (logInstance != null)
								{
									if (logInstance.Key == "")
									{
										foreach (LogInstance item in value)
										{
											int headerMaxInsKey = DatabaseService.MaxInstanceKey("", logInstance);
											DatabaseService.ProcessChangeLog(transtype, objtype, logInstance, item, headerMaxInsKey, "", num3);
										}
									}
									else
									{
										string[] array2 = array;
										foreach (string text2 in array2)
										{
											if (!string.IsNullOrEmpty(text2))
											{
												int headerMaxInsKey2 = DatabaseService.MaxInstanceKey(text2, logInstance);
												foreach (LogInstance item2 in value)
												{
													DatabaseService.ProcessChangeLog(transtype, objtype, logInstance, item2, headerMaxInsKey2, text2, num3);
												}
											}
										}
									}
								}
								else
								{
									Logging.Write("ERROR", $"create log failed, throw exception. object {objtype} id {objId}. No Header Table Log Found!");
									flag = false;
								}
							}
						}
						catch (Exception ex5)
						{
							Logging.Write("ERROR", $"create log failed, throw exception. object {objtype} id {objId}. {ex5.Message}");
							flag = false;
						}
						if (flag)
						{
							dbTransaction2.Commit();
						}
					}
				}
			});
			return result;
		}
		catch (SqlException ex3)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex3.Message);
			return new HttpResult(Enums.MessageCode.Error, "Could not commit transaction");
		}
		catch (Exception ex4)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex4.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex4.Message);
		}
	}

	public virtual HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, TEntity document, ref string objId, ref string objNum, string objcode = "")
	{
		throw new NotImplementedException();
	}

	public virtual HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, TEntity document, ref string objId, ref string objNum)
	{
		throw new NotImplementedException();
	}

	public HttpResult ProcessProcedure(string objtype, Enums.TransactionType transtype, int numOfkey, string keyColumns, string objId)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@objtype", string.IsNullOrEmpty(objtype) ? "" : objtype));
			list.Add(new SqlParameter("@transtype", string.IsNullOrEmpty(transtype.ToString()) ? "" : transtype.ToString()));
			list.Add(new SqlParameter("@num_of_key", (numOfkey > 0) ? numOfkey : 0));
			list.Add(new SqlParameter("@key_columns", string.IsNullOrEmpty(keyColumns) ? "" : keyColumns));
			list.Add(new SqlParameter("@obj_id", string.IsNullOrEmpty(objId) ? "" : objId));
			return new HttpResult(Enums.MessageCode.Success, "", ExecuteDataTable("apz_process", CommandType.StoredProcedure, list.ToArray()));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult NotifyProcedure(string objtype, Enums.TransactionType transtype, int numOfKey, string keyColumns, string objId)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@objtype", string.IsNullOrEmpty(objtype) ? "" : objtype));
			list.Add(new SqlParameter("@transtype", string.IsNullOrEmpty(transtype.ToString()) ? "" : transtype.ToString()));
			list.Add(new SqlParameter("@num_of_key", (numOfKey >= 0) ? numOfKey : 0));
			list.Add(new SqlParameter("@key_columns", string.IsNullOrEmpty(keyColumns) ? "" : keyColumns));
			list.Add(new SqlParameter("@obj_id", string.IsNullOrEmpty(objtype) ? "" : objId));
			return new HttpResult(Enums.MessageCode.Success, "", ExecuteDataTable("apz_notify", CommandType.StoredProcedure, list.ToArray()));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult AddNotificationListener(string objtype, Enums.TransactionType transtype, int numOfKey, string keyColumns, string objId)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@objtype", string.IsNullOrEmpty(objtype) ? "" : objtype));
			list.Add(new SqlParameter("@transtype", string.IsNullOrEmpty(transtype.ToString()) ? "" : transtype.ToString()));
			list.Add(new SqlParameter("@num_of_key", (numOfKey >= 0) ? numOfKey : 0));
			list.Add(new SqlParameter("@key_columns", string.IsNullOrEmpty(keyColumns) ? "" : keyColumns));
			list.Add(new SqlParameter("@obj_id", string.IsNullOrEmpty(objtype) ? "" : objId));
			return new HttpResult(Enums.MessageCode.Success, "", ExecuteDataTable("APZ_D_InsertEventListener", CommandType.StoredProcedure, list.ToArray()));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public string GetObjectId(ApzObjectTypes objtype)
	{
		return DatabaseService.GetObjectId(objtype);
	}

	public string GetObjectId(string udocode)
	{
		return DatabaseService.GetObjectId(udocode);
	}

	public void SetNextAutoKey(ApzObjectTypes objtype)
	{
		DatabaseService.SetNextAutoKey(objtype);
	}

	public string GetObjectNum(ApzObjectTypes objtype, int bplId, int series, int usersign)
	{
		return DatabaseService.GetObjectNum(objtype, bplId, series, usersign);
	}

	public string GetObjectNumBySeries(ApzObjectTypes objtype, int series, int usersign = -1)
	{
		return DatabaseService.GetObjectNumBySeries(objtype, series, usersign);
	}

	public string GetDocLineTableByObjectType(ApzObjectTypes objtype)
	{
		try
		{
			switch (objtype)
			{
			case ApzObjectTypes.oChartOfAccounts:
				return "ACT1";
			case ApzObjectTypes.oBusinessPartners:
				return "CRD1";
			case ApzObjectTypes.oItems:
				return "ITM1";
			case ApzObjectTypes.oVatGroups:
				return "VTG1";
			case ApzObjectTypes.oInvoices:
				return "INV1";
			case ApzObjectTypes.oCreditNotes:
				return "RIN1";
			case ApzObjectTypes.oDeliveryNotes:
				return "DLN1";
			case ApzObjectTypes.oReturnRequest:
				return "RRR1";
			case ApzObjectTypes.oReturns:
				return "RDN1";
			case ApzObjectTypes.oOrders:
				return "RDR1";
			case ApzObjectTypes.oQuotations:
				return "QUT1";
			case ApzObjectTypes.oPurchaseInvoices:
				return "PCH1";
			case ApzObjectTypes.oPurchaseCreditNotes:
				return "RPC1";
			case ApzObjectTypes.oAPReserveInvoice:
				return "PCH1";
			case ApzObjectTypes.oPurchaseDeliveryNotes:
				return "PDN1";
			case ApzObjectTypes.oPurchaseReturns:
				return "RPD1";
			case ApzObjectTypes.oPurchaseOrders:
				return "POR1";
			case ApzObjectTypes.oIncomingPayments:
				return "RCT1";
			case ApzObjectTypes.oJournalVouchers:
				return "BTD1";
			case ApzObjectTypes.oJournalEntries:
				return "JDT1";
			case ApzObjectTypes.oContacts:
				return "CLG1";
			case ApzObjectTypes.oVendorPayments:
				return "VPM1";
			case ApzObjectTypes.oSalesPersons:
				return "SLP1";
			case ApzObjectTypes.oInventoryGenEntry:
				return "IGN1";
			case ApzObjectTypes.oInventoryGenExit:
				return "IGE1";
			case ApzObjectTypes.oWarehouses:
				return "WHS1";
			case ApzObjectTypes.oProductTrees:
				return "ITT1";
			case ApzObjectTypes.oStockTransfer:
				return "WTR1";
			case ApzObjectTypes.oAlternateCatNum:
				return "SCN1";
			case ApzObjectTypes.oSalesOpportunities:
				return "OPR1";
			case ApzObjectTypes.oDrafts:
				return "DRF1";
			case ApzObjectTypes.oPaymentsDrafts:
				return "PDF1";
			case ApzObjectTypes.oKnowledgeBaseSolutions:
				return "SLT1";
			case ApzObjectTypes.oServiceContracts:
				return "CTR1";
			case ApzObjectTypes.oServiceCalls:
				return "SCL1";
			case ApzObjectTypes.oUserKeys:
				return "UKD1";
			case ApzObjectTypes.oQueue:
				return "QUE1";
			case ApzObjectTypes.oSalesForecast:
				return "FCT1";
			case ApzObjectTypes.oProductionOrders:
				return "WOR1";
			case ApzObjectTypes.oDownPayments:
				return "DPI1";
			case ApzObjectTypes.oPurchaseDownPayments:
				return "DPO1";
			case ApzObjectTypes.oStockTransferDraft:
				return "DRF1";
			case ApzObjectTypes.oPurchaseQuotations:
				return "PQT1";
			case ApzObjectTypes.oInventoryTransferRequest:
				return "WTQ1";
			case ApzObjectTypes.oPurchaseRequest:
				return "PRQ1";
			case ApzObjectTypes.oBlanketAgreement:
				return "OAT1";
			case ApzObjectTypes.oResources:
				return "RSC1";
			case ApzObjectTypes.oGoodsReturnRequest:
				return "PRR1";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}

	public string GetDocTableByObjectType(ApzObjectTypes objtype)
	{
		try
		{
			switch (objtype)
			{
			case ApzObjectTypes.oChartOfAccounts:
				return "OACT";
			case ApzObjectTypes.oBusinessPartners:
				return "OCRD";
			case ApzObjectTypes.oItems:
				return "OITM";
			case ApzObjectTypes.oVatGroups:
				return "OVTG";
			case ApzObjectTypes.oUsers:
				return "OUSR";
			case ApzObjectTypes.oInvoices:
				return "OINV";
			case ApzObjectTypes.oCreditNotes:
				return "ORIN";
			case ApzObjectTypes.oDeliveryNotes:
				return "ODLN";
			case ApzObjectTypes.oReturnRequest:
				return "ORRR";
			case ApzObjectTypes.oReturns:
				return "ORDN";
			case ApzObjectTypes.oOrders:
				return "ORDR";
			case ApzObjectTypes.oQuotations:
				return "OQUT";
			case ApzObjectTypes.oPurchaseInvoices:
				return "OPCH";
			case ApzObjectTypes.oPurchaseCreditNotes:
				return "ORPC";
			case ApzObjectTypes.oAPReserveInvoice:
				return "OPCH";
			case ApzObjectTypes.oPurchaseDeliveryNotes:
				return "OPDN";
			case ApzObjectTypes.oPurchaseReturns:
				return "ORPD";
			case ApzObjectTypes.oPurchaseOrders:
				return "OPOR";
			case ApzObjectTypes.oIncomingPayments:
				return "ORCT";
			case ApzObjectTypes.oJournalVouchers:
				return "OBTD";
			case ApzObjectTypes.oJournalEntries:
				return "OJDT";
			case ApzObjectTypes.oContacts:
				return "OCLG";
			case ApzObjectTypes.oVendorPayments:
				return "OVPM";
			case ApzObjectTypes.oSalesPersons:
				return "OSLP";
			case ApzObjectTypes.oInventoryGenEntry:
				return "OIGN";
			case ApzObjectTypes.oInventoryGenExit:
				return "OIGE";
			case ApzObjectTypes.oWarehouses:
				return "OWHS";
			case ApzObjectTypes.oProductTrees:
				return "OITT";
			case ApzObjectTypes.oStockTransfer:
				return "OWTR";
			case ApzObjectTypes.oAlternateCatNum:
				return "OSCN";
			case ApzObjectTypes.oSalesOpportunities:
				return "OOPR";
			case ApzObjectTypes.oDrafts:
				return "ODRF";
			case ApzObjectTypes.oPaymentsDrafts:
				return "OPDF";
			case ApzObjectTypes.oKnowledgeBaseSolutions:
				return "OSLT";
			case ApzObjectTypes.oServiceContracts:
				return "OCTR";
			case ApzObjectTypes.oServiceCalls:
				return "OSCL";
			case ApzObjectTypes.oUserKeys:
				return "OUKD";
			case ApzObjectTypes.oQueue:
				return "OQUE";
			case ApzObjectTypes.oSalesForecast:
				return "OFCT";
			case ApzObjectTypes.oProductionOrders:
				return "OWOR";
			case ApzObjectTypes.oDownPayments:
				return "ODPI";
			case ApzObjectTypes.oPurchaseDownPayments:
				return "ODPO";
			case ApzObjectTypes.oStockTransferDraft:
				return "ODRF";
			case ApzObjectTypes.oPurchaseQuotations:
				return "OPQT";
			case ApzObjectTypes.oInventoryTransferRequest:
				return "OWTQ";
			case ApzObjectTypes.oPurchaseRequest:
				return "OPRQ";
			case ApzObjectTypes.oBlanketAgreement:
				return "OOAT";
			case ApzObjectTypes.oResources:
				return "ORSC";
			case ApzObjectTypes.oMrp:
				return "APZ_OMRP";
			case ApzObjectTypes.oSchedule:
				return "AZP_OPRS";
			case ApzObjectTypes.oAdvanceForecast:
				return "APZ_OFCT";
			case ApzObjectTypes.oGoodsReturnRequest:
				return "OPRR";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}

	public virtual IUnitOfWork GetUnitOfWork()
	{
		if (DatabaseService is DatabaseServiceSql)
		{
			return new UnitOfWork(DatabaseService.ConnectionString);
		}
		return new HanaUnitOfWork(DatabaseService.ConnectionString);
	}

	public DataTable StandardizedTable(DataTable dtClient, string tableName)
	{
		DataTable tableStruct = GetTableStruct(tableName);
		tableStruct.Rows.Clear();
		if (tableStruct.Columns.Count == 0)
		{
			throw new Exception("Không thể lấy cấu trúc bảng");
		}
		for (int i = 0; i < dtClient.Rows.Count; i++)
		{
			tableStruct.ImportRow(dtClient.Rows[i]);
		}
		return tableStruct;
	}
}
