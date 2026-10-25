using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.DataServices;
using Apzon.Entities.Models;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.UserFields;
using Apzon.Services.Interfaces.Sales;
using Newtonsoft.Json;
using SAPbobsCOM;

namespace Apzon.Api.Controllers.Purchase;

[ValidateLoginSapExceptionHandling]
public class GoodReceiptPoController : BaseApiController
{
	public ApzObjectTypes FormObjectTypes = ApzObjectTypes.oPurchaseDeliveryNotes;

	public DataTable GetLine(int id, string FilterLine = "")
	{
		return UnitOfWork.Documents.TableLine(id, FormObjectTypes, FilterLine);
	}

	public DataTable Search([FromBody] DocumentsImpl value)
	{
		try
		{
			return UnitOfWork.Documents.Search(value, FormObjectTypes);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpPost]
	[QueueModifiedAction]
	public HttpResult SaveDataSet([FromBody] DataSet ds)
	{
		if (!Global.LoginSap)
		{
			return UnitOfWork.Documents.SaveDataSet(ds, FormObjectTypes);
		}
		return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
	}

	[HttpPost]
	[QueueModifiedAction]
	public async Task<HttpResult> SaveDoc([FromBody] DocumentsImpl doc)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
			{
				return await unitOfWork.Documents.Save(doc, SessionUserInfo.SessionDiServer, FormObjectTypes);
			}
			return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
		}
		return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
	}

	[HttpPost]
	[QueueModifiedAction]
	public async Task<HttpResult> AddNew(DocumentsImpl doc)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			return await BrokerService.GetUnitOfWork(SessionId).Documents.AddNew(doc, SessionUserInfo.SessionDiServer, FormObjectTypes);
		}
		return new HttpResult(Enums.MessageCode.Error, "Unknown error");
	}

	[HttpPost]
	[QueueModifiedAction]
	public async Task<HttpResult> SaveAsDraft([FromBody] DocumentsImpl doc)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
			{
				return await unitOfWork.Documents.Save(doc, SessionUserInfo.SessionDiServer, ApzObjectTypes.oDrafts, FormObjectTypes);
			}
			return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
		}
		return new HttpResult(Enums.MessageCode.Error, "Chức năng này đang cập nhật");
	}

	public int GetMaxDocEntry()
	{
		return UnitOfWork.Documents.GetMaxDocEntry(FormObjectTypes);
	}

	public int GetMaxDocNum()
	{
		return UnitOfWork.Documents.GetMaxDocNum(FormObjectTypes);
	}

	public int GetMinDocEntry()
	{
		return UnitOfWork.Documents.GetMinDocEntry(FormObjectTypes);
	}

	[HttpPost]
	[QueueModifiedAction]
	public HttpResult Cancel([FromBody] int DocEntry)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			return BrokerService.GetUnitOfWork(SessionId).Documents.CancelDoc(DocEntry, SessionUserInfo.SessionDiServer, FormObjectTypes);
		}
		return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
	}

	[HttpPost]
	[QueueModifiedAction]
	public HttpResult Close([FromBody] int DocEntry)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			return BrokerService.GetUnitOfWork(SessionId).Documents.CloseDoc(DocEntry, SessionUserInfo.SessionDiServer, FormObjectTypes);
		}
		return UnitOfWork.Documents.CloseDoc(DocEntry, FormObjectTypes);
	}

	[HttpPost]
	public DataTable GetDocLinesByListDocEntry([FromBody] List<int> DocEntry)
	{
		return UnitOfWork.Documents.GetDocLinesByListDocEntry(DocEntry, FormObjectTypes);
	}

	[HttpPost]
	[QueueModifiedAction]
	public DataTable GetApprovalTemplates([FromBody] Tuple<DocumentsImpl, string, string> item)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new DataTable();
			}
			return BrokerService.GetUnitOfWork(SessionId).Documents.GetApprovalTemplate(item.Item1, SessionUserInfo.SessionDiServer, item.Item2, item.Item3, FormObjectTypes);
		}
		return new DataTable();
	}

	[HttpGet]
	public DataTable Duplicate(int docEntry)
	{
		return UnitOfWork.Documents.Duplicate(docEntry, FormObjectTypes);
	}

	[HttpGet]
	public DataTable GetPayment(int docEntry, int objType)
	{
		return UnitOfWork.Documents.GetPayment(docEntry, FormObjectTypes);
	}

	[HttpGet]
	public DataTable CopyFrom(string type, string entry, int bplId, string username)
	{
		IDocument documents = UnitOfWork.Documents;
		int formObjectTypes = (int)FormObjectTypes;
		return documents.CopyFrom(type, entry, formObjectTypes.ToString(), bplId, username);
	}

	[HttpGet]
	public DataTable CopyFromLine(string type, string entry)
	{
		return UnitOfWork.Documents.CopyFromLine(type, entry, 1470000113.ToString());
	}

	[HttpGet]
	public DataTable GetListOrcCode(string p, DateTime? date = null)
	{
		return UnitOfWork.Commons.GetListOrcCode(p, date);
	}

	[HttpGet]
	public DataTable GetDimensionByCode(string code)
	{
		return UnitOfWork.Commons.GetDimensionByCode(code);
	}

	[HttpGet]
	public DataTable GetDimensions()
	{
		return UnitOfWork.Commons.GetDimensions();
	}

	[HttpGet]
	public DataTable GetListBPCatalog()
	{
		return UnitOfWork.Documents.GetListBpCatalog(FormObjectTypes);
	}

	[HttpGet]
	public DataTable LoadBatchNumberStruct()
	{
		return UnitOfWork.Documents.LoadBatchNumberStruct(FormObjectTypes);
	}

	[HttpGet]
	public DataTable GetBatchSerialByCode(string itemcode, string whscode, int lineNum, string barcode, string objType)
	{
		return UnitOfWork.Documents.GetBatchSerialByCode(itemcode, whscode, lineNum, barcode, objType);
	}

	[HttpGet]
	public DataTable GetBatchSerialByDocEntry(int docEntry)
	{
		return UnitOfWork.Documents.GetBatchSerialByDocEntry(docEntry, FormObjectTypes);
	}

	[HttpGet]
	public DataTable CopyBatchSerial(string fromentry, string fromtype)
	{
		return UnitOfWork.Documents.CopyBatchSerial(fromentry, fromtype);
	}

	[HttpGet]
	public DataTable LoadSeries(string usercode)
	{
		return UnitOfWork.Documents.LoadSeries(usercode, FormObjectTypes);
	}

	[HttpGet]
	public int GetDocumentNumber(int series)
	{
		return UnitOfWork.Documents.GetDocumentNumber(series, FormObjectTypes);
	}

	[HttpGet]
	public DataTable GetFreight(int docEntry)
	{
		return UnitOfWork.Documents.GetFreight(docEntry, FormObjectTypes);
	}

	[HttpGet]
	public DataTable GetFreightDefault()
	{
		return UnitOfWork.Documents.GetFreightDefault(FormObjectTypes);
	}

	[HttpGet]
	public DataTable GetTaxGroup()
	{
		return UnitOfWork.Documents.GetTaxGroup(FormObjectTypes);
	}

	[HttpGet]
	public DataTable GetFreightCode()
	{
		return UnitOfWork.Documents.GetFreightCode(FormObjectTypes);
	}

	[HttpGet]
	public DataTable GetRateByVatGroup()
	{
		return UnitOfWork.Documents.GetRateByVatGroup(FormObjectTypes);
	}

	[HttpGet]
	public DataTable GetExpnsByCode(string expnsCode)
	{
		return UnitOfWork.Documents.GetExpnsByCode(FormObjectTypes, expnsCode);
	}

	[HttpGet]
	public DataSetResult GetDataByBarcode(string barcode, string userName, int branch)
	{
		try
		{
			return UnitOfWork.GoodsReceipt.GetDataByBarcode(barcode, userName, branch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSetResult();
	}

	[HttpPost]
	public HttpResult GetCurrentRate([FromBody] DataTable dtSearch)
	{
		return UnitOfWork.GoodsReceipt.GetCurrentRate(dtSearch);
	}

	[HttpPost]
	public async Task<HttpResult> AddReceiptPoWithBatch([FromBody] DataTable model)
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
				}
				IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
				if (!model.IsNotNull())
				{
					Logging.Write("WATCH", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), JsonConvert.SerializeObject(model));
					return new HttpResult(Enums.MessageCode.Error, "Dữ liệu gửi lên bị thiếu");
				}
				List<string> list = new List<string>();
				DocumentsImpl documentsImpl = new DocumentsImpl
				{
					Comments = "GRPO by Mobile App",
					DocDate = DateTime.Today,
					ObjType = 20,
					Lines = new List<Document_LinesImpl>()
				};
				var enumerable = from t in model.AsEnumerable()
					group t by new
					{
						POEntry = Function.ToString(t["POEntry"]),
						POLine = Function.ToString(t["POLine"])
					} into g
					select new
					{
						Key = g.Key,
						Items = g
					};
				int num = 0;
				foreach (var item in enumerable)
				{
					Document_LinesImpl document_LinesImpl = new Document_LinesImpl
					{
						BatchNumbers = new List<BatchNumbersImpl>(),
						BinAllocations = new List<DocumentLinesBinAllocationsImpl>()
					};
					if (documentsImpl.DocRate <= 0.0)
					{
						DataTable dataTable = new DataTable();
						dataTable.Columns.Add(new DataColumn("poEntry", typeof(int)));
						dataTable.Columns.Add(new DataColumn("docDate", typeof(DateTime)));
						DataRow dataRow = dataTable.NewRow();
						dataRow["poEntry"] = Function.ParseInt(model.Rows[0]["POEntry"]);
						dataRow["docDate"] = DateTime.Today;
						dataTable.Rows.Add(dataRow);
						HttpResult currentRate = UnitOfWork.GoodsReceipt.GetCurrentRate(dataTable);
						if (string.IsNullOrEmpty(currentRate.Message))
						{
							documentsImpl.DocRate = Function.ParseDouble(currentRate.Content);
						}
						else
						{
							Logging.Write("WATCH", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get rate error: ", currentRate.Message);
						}
					}
					document_LinesImpl.LineNum = num;
					double num2 = 0.0;
					List<DataColumn> list2 = (from DataColumn t in model.Columns
						where t.ColumnName.StartsWith("U_")
						select (t)).ToList();
					foreach (DataRow item2 in item.Items)
					{
						if (string.IsNullOrEmpty(documentsImpl.CardCode))
						{
							documentsImpl.CardCode = Function.ToString(item2["CardCode"]);
						}
						if (string.IsNullOrEmpty(document_LinesImpl.ItemCode))
						{
							document_LinesImpl.ItemCode = Function.ToString(item2["ItemCode"]);
						}
						if (item2.IsExistsColumn("WhsCode") && string.IsNullOrEmpty(document_LinesImpl.WhsCode))
						{
							document_LinesImpl.WhsCode = Function.ToString(item2["WhsCode"]);
						}
						if (item2.IsExistsColumn("Price"))
						{
							document_LinesImpl.Price = Function.ParseDouble(item2["Price"]);
						}
						if (item2.IsExistsColumn("Currency"))
						{
							document_LinesImpl.Currency = Function.ToString(item2["Currency"]);
						}
						if (item2.IsExistsColumn("AcctCode"))
						{
							document_LinesImpl.AcctCode = Function.ToString(item2["AcctCode"]);
						}
						if (item2.IsExistsColumn("POEntry") && Function.ParseInt(item2["POEntry"]) > 0)
						{
							document_LinesImpl.BaseEntry = Function.ParseInt(item2["POEntry"]);
							document_LinesImpl.BaseRef = Function.ParseInt(item2["POEntry"]);
							document_LinesImpl.BaseType = 22;
							list.Add(document_LinesImpl.BaseEntry.ToString());
						}
						if (item2.IsExistsColumn("POLine") && Function.ParseInt(item2["POLine"]) > 0)
						{
							document_LinesImpl.BaseLine = Function.ParseInt(item2["POLine"]);
						}
						BatchNumbersImpl batchNumbersImpl = new BatchNumbersImpl
						{
							Quantity = Function.ParseDouble(item2["Quantity"]),
							BatchNumber = Function.ToString(item2["DistNumber"]),
							AddmisionDate = DateTime.Today,
							BaseLineNumber = num
						};
						if (list2.Count > 0)
						{
							batchNumbersImpl.UserFields = new List<UserFieldsImpl>();
							for (int num3 = 0; num3 < list2.Count; num3++)
							{
								UserFieldsImpl userFieldsImpl = new UserFieldsImpl
								{
									Name = list2[num3].ColumnName
								};
								if (list2[num3].DataType == typeof(DateTime))
								{
									userFieldsImpl.Type = BoFieldTypes.db_Date;
								}
								else if (list2[num3].DataType == typeof(double) || list2[num3].DataType == typeof(decimal))
								{
									userFieldsImpl.Type = BoFieldTypes.db_Numeric;
									userFieldsImpl.SubType = BoFldSubTypes.st_Quantity;
								}
								else if (list2[num3].DataType == typeof(int) || list2[num3].DataType == typeof(short) || list2[num3].DataType == typeof(long))
								{
									userFieldsImpl.Type = BoFieldTypes.db_Numeric;
								}
								else
								{
									userFieldsImpl.Type = BoFieldTypes.db_Alpha;
								}
								userFieldsImpl.Value = item2[list2[num3]];
								batchNumbersImpl.UserFields.Add(userFieldsImpl);
							}
						}
						num2 += batchNumbersImpl.Quantity;
						document_LinesImpl.BatchNumbers.Add(batchNumbersImpl);
					}
					document_LinesImpl.Quantity = num2;
					documentsImpl.Lines.Add(document_LinesImpl);
					num++;
				}
				DataTable dataTable2 = UnitOfWork.Commons.CreateFreightGridDatasource("O");
				foreach (DataRow row in UnitOfWork.Commons.GetFreight("POR3", string.Join(",", list)).Rows)
				{
					if (!Function.ToString(row["Status"]).Equals("C"))
					{
						DataRow dataRow2 = dataTable2.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["ExpnsCode"]) == Function.ParseInt(row["ExpnsCode"]));
						if (dataRow2 != null)
						{
							dataRow2["LineTotal"] = Function.ParseDouble(row["LineTotal"]);
							dataRow2["TotalFrgn"] = Function.ParseDouble(row["TotalFrgn"]);
							dataRow2["Comments"] = Function.ToString(row["Comments"]);
							dataRow2["VatGroup"] = Function.ToString(row["VatGroup"]);
							dataRow2["VatPrcnt"] = Function.ParseDouble(row["VatPrcnt"]);
							dataRow2["VatSum"] = Function.ParseDouble(row["VatSum"]);
							dataRow2["DistrbMthd"] = Function.ToString(row["DistrbMthd"]);
							dataRow2["GrsAmount"] = Function.ParseDouble(row["GrsAmount"]);
							dataRow2["OcrCode"] = Function.ToString(row["OcrCode"]);
							dataRow2["OcrCode2"] = Function.ToString(row["OcrCode2"]);
							dataRow2["OcrCode3"] = Function.ToString(row["OcrCode3"]);
							dataRow2["OcrCode4"] = Function.ToString(row["OcrCode4"]);
							dataRow2["OcrCode5"] = Function.ToString(row["OcrCode5"]);
							dataRow2["Project"] = Function.ToString(row["Project"]);
							dataRow2["BaseType"] = Function.ParseInt(row["BaseType"]);
							dataRow2["BaseAbsEnt"] = Function.ParseInt(row["BaseAbsEnt"]);
							dataRow2["BaseLnNum"] = Function.ParseInt(row["BaseLnNum"]);
							dataRow2["BaseRef"] = Function.ParseInt(row["BaseRef"]);
						}
					}
				}
				dataTable2.AcceptChanges();
				DataRow[] array = (from t in dataTable2.Copy().AsEnumerable()
					where !string.IsNullOrEmpty(Function.ToString(t["ExpnsCode"])) && (Function.ParseDouble(t["LineTotal"]) != 0.0 || Function.ParseDouble(t["TotalFrgn"]) != 0.0)
					select t).ToArray();
				if (array.Length != 0)
				{
					documentsImpl.Expenses = array.CopyToDataTable().ToDocumentsAdditional();
				}
				return await unitOfWork.Documents.AddNew(documentsImpl, SessionUserInfo.SessionDiServer, FormObjectTypes);
			}
			return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	[HttpGet]
	public DataSetResult GetDataByDraftKey(string barcode)
	{
		try
		{
			return UnitOfWork.GoodsReceipt.GetDataByDraftKey(barcode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSetResult(ex.Message);
		}
	}

	[HttpPost]
	public async Task<HttpResult> AddReceiptPOFromDraftWithBatch([FromBody] DataSet model)
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
				}
				IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
				if (model.Tables.Count < 2 || !model.Tables[1].IsNotNull())
				{
					Logging.Write("WATCH", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), JsonConvert.SerializeObject(model));
					return new HttpResult(Enums.MessageCode.Error, "Dữ liệu gửi lên bị thiếu");
				}
				string entry = Function.ToString(model.Tables[0].Rows[0]["DraftKey"]);
				DataTable dataTable = UnitOfWork.Documents.CopyFrom(112.ToString(), entry, 20.ToString(), -1, "");
				DataTable dataTable2 = UnitOfWork.Documents.CopyFromLine(112.ToString(), entry, 20.ToString());
				new DocumentsImpl();
				DocumentsImpl documentsImpl;
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					documentsImpl = dataTable.Rows[0].ToDocument();
					documentsImpl.DocEntry = 0;
				}
				else
				{
					documentsImpl = null;
				}
				List<Document_LinesImpl> list = new List<Document_LinesImpl>();
				List<Document_SpecialLinesImpl> list2 = new List<Document_SpecialLinesImpl>();
				if (dataTable2 != null && dataTable2.Rows.Count > 0)
				{
					list = (from row in dataTable2.AsEnumerable()
						where "R".Equals(Function.ToString(row["LineType"]))
						select row).CopyToDataTable().ToDocumentLine();
					list = list.Where((Document_LinesImpl x) => !string.IsNullOrEmpty(x.ItemCode)).ToList();
					DataTable dataTable3 = (from row in dataTable2.AsEnumerable()
						where !"R".Equals(Function.ToString(row["LineType"]))
						select row).CopyToDataTable();
					if (list.Count > 0)
					{
						DataTable source = model.Tables[1];
						foreach (Document_LinesImpl line in list)
						{
							DataRow[] array = (from t in source.AsEnumerable()
								where !string.IsNullOrEmpty(Function.ToString(t["DistNumber"])) && Function.ToString(t["ItemCode"]).Equals(line.ItemCode)
								select t).ToArray();
							if (array.Length == 0)
							{
								continue;
							}
							line.Quantity = 0.0;
							DataTable dataTable4 = array.CopyToDataTable();
							line.BatchNumbers = new List<BatchNumbersImpl>();
							line.SerialNumbers = new List<SerialNumbersImpl>();
							line.BinAllocations = new List<DocumentLinesBinAllocationsImpl>();
							for (int num = 0; num < dataTable4.Rows.Count; num++)
							{
								DataRow dataRow = dataTable4.Rows[num];
								string text = Function.ToString(dataRow["ObjType"]);
								if (text == "10000044")
								{
									BatchNumbersImpl batchNumbersImpl = new BatchNumbersImpl();
									batchNumbersImpl.BaseLineNumber = line.LineNum;
									batchNumbersImpl.BatchNumber = Function.ToString(dataRow["DistNumber"]);
									batchNumbersImpl.Quantity = Function.ParseDouble(dataRow["Quantity"]);
									line.BatchNumbers.Add(batchNumbersImpl);
									line.Quantity += batchNumbersImpl.Quantity;
								}
								else if (text == "10000045")
								{
									SerialNumbersImpl serialNumbersImpl = new SerialNumbersImpl();
									string internalSerialNumber = (serialNumbersImpl.BatchID = Function.ToString(dataRow["DistNumber"]));
									serialNumbersImpl.InternalSerialNumber = internalSerialNumber;
									serialNumbersImpl.BaseLineNumber = line.LineNum;
									serialNumbersImpl.Quantity = Function.ParseDouble(dataRow["Quantity"]);
									line.SerialNumbers.Add(serialNumbersImpl);
								}
								int num2 = Function.ParseInt(dataRow["BinId"]);
								if (num2 > 0)
								{
									DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl = new DocumentLinesBinAllocationsImpl();
									documentLinesBinAllocationsImpl.BinAbsEntry = num2;
									documentLinesBinAllocationsImpl.BaseLineNumber = line.LineNum;
									documentLinesBinAllocationsImpl.BinActionType = BinActionTypeEnum.batToWarehouse;
									documentLinesBinAllocationsImpl.Quantity = Function.ParseDouble(dataRow["Quantity"]);
									documentLinesBinAllocationsImpl.SerialAndBatchNumbersBaseLine = 0;
									if (documentLinesBinAllocationsImpl.BinAbsEntry > 0)
									{
										line.BinAllocations.Add(documentLinesBinAllocationsImpl);
									}
								}
							}
						}
					}
					if (dataTable3.IsNotNull())
					{
						foreach (DataRow row in dataTable3.Rows)
						{
							Document_SpecialLinesImpl document_SpecialLinesImpl = new Document_SpecialLinesImpl();
							document_SpecialLinesImpl.AfterLineNumber = (list.Any() ? list[list.Count - 1].LineNum : (-1));
							string text3 = Function.ToString(row["LineType"]);
							if (text3 == "T")
							{
								document_SpecialLinesImpl.LineType = BoDocSpecialLineType.dslt_Text;
								document_SpecialLinesImpl.LineText = Function.ToString(row["ItemCode"]);
							}
							else if (text3 == "S")
							{
								document_SpecialLinesImpl.LineType = BoDocSpecialLineType.dslt_Subtotal;
								document_SpecialLinesImpl.Subtotal = Function.ParseDouble(row["LineTotal"]);
								document_SpecialLinesImpl.SubtotalFC = Function.ParseDouble(row["TotalFrgn"]);
							}
							list2.Add(document_SpecialLinesImpl);
						}
					}
				}
				if (documentsImpl != null)
				{
					documentsImpl.Lines = list;
					documentsImpl.SpecialLines = list2;
					documentsImpl.DocObjectCode = ApzObjectTypes.oPurchaseDeliveryNotes;
					documentsImpl.DocDate = DateTime.Today;
					return await unitOfWork.Documents.Save(documentsImpl, SessionUserInfo.SessionDiServer, ApzObjectTypes.oPurchaseDeliveryNotes);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult();
	}

	[HttpGet]
	public DataTable GetReferencedDocument(int docEntry)
	{
		return UnitOfWork.Documents.GetReferencedDocument(FormObjectTypes, docEntry);
	}

	[HttpGet]
	public DataTable ListChooseDocuments(string from, string to)
	{
		return UnitOfWork.Documents.ListChooseDocuments(FormObjectTypes, from, to);
	}

	public DataTable GetReferencedDocumentBy(int docEntry)
	{
		return UnitOfWork.Documents.GetReferencedDocumentBy(FormObjectTypes, docEntry);
	}
}
