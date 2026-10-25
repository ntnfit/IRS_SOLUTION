using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Services;
using SAPbobsCOM;

namespace Apzon.DataServices.HanaImplements;

public class HanaDocumentsRepository : DocumentsRepository
{
	public HanaDocumentsRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, unitOfWork, dataService)
	{
	}

	protected override HttpResult SaveAttachment(DocumentsImpl doc, string sessionId, ApzObjectTypes objType, bool addNew)
	{
		if (addNew || doc.AtcEntry <= 0)
		{
			return _unitOfWork.Attachment2.Add(doc.AttachmentFiles, sessionId);
		}
		return _unitOfWork.Attachment2.Update(doc.AtcEntry, doc.AttachmentFiles, sessionId);
	}

	protected override bool Exist(DocumentsImpl doc, string sessionId, ApzObjectTypes objType)
	{
		try
		{
			List<string> tableName = Function.GetTableName(objType);
			if (doc.DocEntry <= 0)
			{
				return false;
			}
			if (tableName.Count == 0)
			{
				throw new Exception("Can not check exitsts data");
			}
			return !string.IsNullOrEmpty(ExecuteScarlar($"select 1 from {tableName[0]} where \"DocEntry\" = docEntry;", CommandType.Text, new SqlParameter("@docEntry", doc.DocEntry)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return !doc.DocEntry.Equals(0);
		}
	}

	protected override async Task<HttpResult> UpdateData(DocumentsImpl doc, string sessionId, ApzObjectTypes objType, ApzObjectTypes docObjectCode = ApzObjectTypes.oDrafts, bool ignoreUpdateEmptyValue = false)
	{
		try
		{
			List<BodyRequestObject> lstObj = new List<BodyRequestObject>();
			if (doc != null && doc.Udos != null && doc.Udos.Count > 0)
			{
				foreach (DocumentUdo udo2 in doc.Udos)
				{
					StringBuilder stringBuilder = new StringBuilder();
					if (udo2.dsChild.Tables.Count <= 0 || !udo2.UdoTable.IsNotNull())
					{
						continue;
					}
					int udoEntry = GetUdoEntry(doc, udo2, "OPMT", "@OPMT", "U_paymentEntry");
					DataTable udo = GetUdo("OPMT");
					if (udo != null && udo.Rows.Count > 0)
					{
						if (udoEntry == 0)
						{
							stringBuilder.Append(_unitOfWork.UDOs.MappingXml(udo, udo2.dtFather, udo2.dsChild, sessionId, "Add"));
							lstObj.Add(new BodyRequestObject
							{
								ObjectType = ApzObjectTypes.oNone,
								UdoCode = Function.ToString(udo.Rows[0]["Code"]),
								Action = Enums.HttpAction.AddNew,
								BodyObject = stringBuilder.ToString()
							});
						}
						else
						{
							stringBuilder.Append(_unitOfWork.UDOs.MappingXml(udo, udo2.dtFather, udo2.dsChild, sessionId, "Update"));
							lstObj.Add(new BodyRequestObject
							{
								ObjectType = ApzObjectTypes.oNone,
								UdoCode = Function.ToString(udo.Rows[0]["Code"]),
								Key = Function.ParseInt(udo2.dtFather.Rows[0]["DocEntry"]),
								Action = Enums.HttpAction.Update,
								BodyObject = stringBuilder.ToString()
							});
						}
					}
				}
			}
			_ = string.Empty;
			new List<int>();
			string bodyObject = await MappingAsync(doc, EnumCommandType.Update, objType, docObjectCode, ignoreUpdateEmptyValue);
			lstObj.Add(new BodyRequestObject
			{
				ObjectType = objType,
				Key = doc.DocEntry,
				OutObjectKey = true,
				Action = Enums.HttpAction.Update,
				BodyObject = bodyObject
			});
			HttpResult httpResult = DataService.ProcessMutiTransaction(sessionId, lstObj);
			if (httpResult.MessageCode != Enums.MessageCode.Success)
			{
				Logging.Write("ERROR", "Update Error", objType.ToString(), doc.DocEntry.ToString(), sessionId, httpResult.Message);
				return httpResult;
			}
			Function.ParseInt(httpResult.Content);
			return httpResult;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override string GetUdoEntryQuery(string udoTableName)
	{
		return "SELECT IFNULL((SELECT MAX(\"DocEntry\") FROM \"" + udoTableName + "\"),0)+1 ID from DUMMY;";
	}

	private async Task<string> MappingAsync(DocumentsImpl doc, EnumCommandType commandType, ApzObjectTypes objType, ApzObjectTypes docObjectCode = ApzObjectTypes.oDrafts, bool ignoreUpdateEmptyValue = false)
	{
		if (!DocumentsRepository.MainCurrency.ContainsKey(_connectionString))
		{
			lock (DocumentsRepository.MainCurrency)
			{
				if (!DocumentsRepository.MainCurrency.ContainsKey(_connectionString))
				{
					DocumentsRepository.MainCurrency.Add(_connectionString, ServiceUnitOfWork.GeneralSetting.GetMainCurrency());
				}
			}
		}
		StringBuilder xml = new StringBuilder("{\r\n");
		try
		{
			string text = Function.ToString(doc.DocStatus).ToUpper();
			bool isMapping = ((objType == ApzObjectTypes.oDrafts || objType == ApzObjectTypes.oOrders || objType == ApzObjectTypes.oQuotations || objType == ApzObjectTypes.oPurchaseOrders || objType == ApzObjectTypes.oPurchaseQuotations || objType == ApzObjectTypes.oReturnRequest || objType == ApzObjectTypes.oGoodsReturnRequest || objType == ApzObjectTypes.oPurchaseRequest || ((objType == ApzObjectTypes.oDownPayments || objType == ApzObjectTypes.oPurchaseDownPayments) && doc.Posted == "0")) && (text.Equals("O") || text.Equals("OPEN"))) || commandType == EnumCommandType.Add;
			Task<string> task = Task.Run(() => MappingHeader(doc, objType, docObjectCode, commandType, isMapping));
			Task<string> t1 = Task.Run(() => MappingLine(doc, objType, docObjectCode, isMapping, commandType, ignoreUpdateEmptyValue));
			Task<string> t2 = Task.Run(() =>
			{
				List<string> lstFieldNotValidated = null;
				lock (DocumentsRepository.ListUdfHeaderNotValidated)
				{
					if (!DocumentsRepository.ListUdfHeaderNotValidated.ContainsKey(_connectionString) || DocumentsRepository.ListUdfHeaderNotValidated[_connectionString] == null)
					{
						DocumentsRepository.ListUdfHeaderNotValidated.Add(_connectionString, ServiceUnitOfWork.UDFs.GetListFieldNotValidated("ADOC"));
					}
					lstFieldNotValidated = DocumentsRepository.ListUdfHeaderNotValidated[_connectionString];
				}
				return DataService.MappingUdf(doc.UserFields, commandType, ignoreUpdateEmptyValue, lstFieldNotValidated);
			});
			StringBuilder stringBuilder = xml;
			stringBuilder.Append(await task);
			stringBuilder = xml;
			stringBuilder.Append(await t2);
			stringBuilder = xml;
			stringBuilder.Append(await t1);
			xml.Append("}");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return xml.ToString();
	}

	public override string Mapping(DocumentsImpl doc, EnumCommandType commandType, ApzObjectTypes objType, ApzObjectTypes docObjectCode = ApzObjectTypes.oDrafts, bool ignoreUpdateEmptyValue = false)
	{
		if (!DocumentsRepository.MainCurrency.ContainsKey(_connectionString))
		{
			lock (DocumentsRepository.MainCurrency)
			{
				if (!DocumentsRepository.MainCurrency.ContainsKey(_connectionString))
				{
					DocumentsRepository.MainCurrency.Add(_connectionString, ServiceUnitOfWork.GeneralSetting.GetMainCurrency());
				}
			}
		}
		string text = DocumentsRepository.MainCurrency[_connectionString];
		StringBuilder stringBuilder = new StringBuilder("{\r\n");
		try
		{
			if (objType == ApzObjectTypes.oDrafts)
			{
				stringBuilder.Append(Function.GetJsonString("DocObjectCode", Function.ToString((int)docObjectCode)));
			}
			if ((objType == ApzObjectTypes.oDrafts || doc.Series < 0) && doc.DocNum > 0)
			{
				stringBuilder.Append(Function.GetJsonString("DocNum", doc.DocNum));
			}
			if (new ApzObjectTypes[2]
			{
				ApzObjectTypes.oDownPayments,
				ApzObjectTypes.oPurchaseDownPayments
			}.Contains(objType) || (objType == ApzObjectTypes.oDrafts && new ApzObjectTypes[2]
			{
				ApzObjectTypes.oDownPayments,
				ApzObjectTypes.oPurchaseDownPayments
			}.Contains(docObjectCode)))
			{
				stringBuilder.Append(Function.GetJsonString("DownPaymentType", ((Enums.DownPaymentType)Function.ParseInt(doc.Posted)/*cast due to constrained. prefix*/).ToString()));
			}
			if (!doc.SlpCode.Equals(0))
			{
				stringBuilder.Append(Function.GetJsonString("SalesPersonCode", Function.ParseInt(doc.SlpCode)));
			}
			if (doc.OwnerCode > 0)
			{
				stringBuilder.Append(Function.GetJsonString("DocumentsOwner", Function.ParseInt(doc.OwnerCode)));
			}
			stringBuilder.Append(Function.GetJsonString("Comments", Function.ToString(doc.Comments)));
			stringBuilder.Append(Function.GetJsonString("OpeningRemarks", Function.ToString(doc.Header)));
			stringBuilder.Append(Function.GetJsonString("ClosingRemarks", Function.ToString(doc.Footer)));
			if (objType != ApzObjectTypes.oInventoryGenExit && objType != ApzObjectTypes.oInventoryGenEntry)
			{
				stringBuilder.Append(Function.GetJsonString("ContactPersonCode", doc.CntctCode));
			}
			ApzObjectTypes[] source = new ApzObjectTypes[6]
			{
				ApzObjectTypes.oPurchaseRequest,
				ApzObjectTypes.oInventoryGenEntry,
				ApzObjectTypes.oInventoryGenExit,
				ApzObjectTypes.oStockTransfer,
				ApzObjectTypes.oStockTransferDraft,
				ApzObjectTypes.oInventoryTransferRequest
			};
			if (!source.Contains(objType) || (objType == ApzObjectTypes.oDrafts && !source.Contains(doc.DocObjectCode)))
			{
				stringBuilder.Append(Function.GetJsonString("NumAtCard", Function.ToString(doc.NumAtCard)));
			}
			if (doc.AtcEntry != 0 && doc.AttachmentFiles != null && doc.AttachmentFiles.Count > 0)
			{
				stringBuilder.Append(Function.GetJsonString("AttachmentEntry", Function.ParseInt(doc.AtcEntry)));
			}
			else
			{
				stringBuilder.Append(Function.GetJsonString("AttachmentEntry", null));
			}
			if (!string.IsNullOrEmpty(doc.Ref1))
			{
				stringBuilder.Append(Function.GetJsonString("Reference1", Function.ToString(doc.Ref1)));
			}
			if (!string.IsNullOrEmpty(doc.Ref2))
			{
				stringBuilder.Append(Function.GetJsonString("Reference2", Function.ToString(doc.Ref2)));
			}
			if (doc.PartSupply == "N")
			{
				stringBuilder.Append(Function.GetJsonString("PartialSupply", "tNO"));
			}
			else
			{
				stringBuilder.Append(Function.GetJsonString("PartialSupply", "tYES"));
			}
			if (doc.UseShpdGd == "Y")
			{
				stringBuilder.Append(Function.GetJsonString("UseShpdGoodsAct", "tYES"));
			}
			else
			{
				stringBuilder.Append(Function.GetJsonString("UseShpdGoodsAct", "tNO"));
			}
			if (!string.IsNullOrEmpty(doc.PriceMode))
			{
				stringBuilder.Append(Function.GetJsonString("PriceMode", (doc.PriceMode == "N") ? "pmdNet" : "pmdGross"));
			}
			string text2 = Function.ToString(doc.DocStatus).ToUpper();
			bool flag = ((objType == ApzObjectTypes.oDrafts || objType == ApzObjectTypes.oOrders || objType == ApzObjectTypes.oQuotations || objType == ApzObjectTypes.oPurchaseOrders || objType == ApzObjectTypes.oPurchaseQuotations || objType == ApzObjectTypes.oReturnRequest || objType == ApzObjectTypes.oGoodsReturnRequest || objType == ApzObjectTypes.oPurchaseRequest || ((objType == ApzObjectTypes.oDownPayments || objType == ApzObjectTypes.oPurchaseDownPayments) && doc.Posted == "0")) && (text2.Equals("O") || text2.Equals("OPEN"))) || commandType == EnumCommandType.Add;
			if (flag)
			{
				if (objType != ApzObjectTypes.oPurchaseRequest || (objType == ApzObjectTypes.oDrafts && docObjectCode != ApzObjectTypes.oPurchaseRequest))
				{
					stringBuilder.Append(Function.GetJsonString("Address", Function.ToString(doc.Address)));
				}
				if ((!string.IsNullOrEmpty(doc.isIns) && new ApzObjectTypes[4]
				{
					ApzObjectTypes.oInvoices,
					ApzObjectTypes.oPurchaseInvoices,
					ApzObjectTypes.oReserveInvoices,
					ApzObjectTypes.oAPReserveInvoice
				}.Contains(objType)) || (objType == ApzObjectTypes.oDrafts && new ApzObjectTypes[4]
				{
					ApzObjectTypes.oInvoices,
					ApzObjectTypes.oPurchaseInvoices,
					ApzObjectTypes.oReserveInvoices,
					ApzObjectTypes.oAPReserveInvoice
				}.Contains(docObjectCode)))
				{
					stringBuilder.Append(Function.GetJsonString("ReserveInvoice", Function.ToString(doc.isIns)));
				}
				if (objType != ApzObjectTypes.oPurchaseRequest || (objType == ApzObjectTypes.oDrafts && docObjectCode != ApzObjectTypes.oPurchaseRequest))
				{
					stringBuilder.Append(Function.GetJsonString("Address2", Function.ToString(doc.Address2)));
				}
				if (!string.IsNullOrEmpty(doc.AgentCode))
				{
					stringBuilder.Append(Function.GetJsonString("AgentCode", Function.ToString(doc.AgentCode)));
				}
				if (doc.AnnualInvoiceDeclarationReference != 0)
				{
					stringBuilder.Append(Function.GetJsonString("AnnualInvoiceDeclarationReference", Function.ParseInt(doc.AnnualInvoiceDeclarationReference)));
				}
				if (!string.IsNullOrEmpty(doc.BPChCode))
				{
					stringBuilder.Append(Function.GetJsonString("BPChannelCode", Function.ToString(doc.BPChCode)));
				}
				if (doc.BPChCntc != 0)
				{
					stringBuilder.Append(Function.GetJsonString("BPChannelContact", Function.ParseInt(doc.BPChCntc)));
				}
				if (doc.BPLId != 0 && doc.BPLId != -1)
				{
					stringBuilder.Append(Function.GetJsonString("BPL_IDAssignedToInvoice", Function.ParseInt(doc.BPLId)));
				}
				if (doc.BlanketAgreementNumber != 0)
				{
					stringBuilder.Append(Function.GetJsonString("BlanketAgreementNumber", Function.ParseInt(doc.BlanketAgreementNumber)));
				}
				if (!string.IsNullOrEmpty(doc.Box1099))
				{
					stringBuilder.Append(Function.GetJsonString("Box1099", Function.ToString(doc.Box1099)));
				}
				if (doc.Series != 0)
				{
					stringBuilder.Append(Function.GetJsonString("Series", doc.Series));
				}
				if (!string.IsNullOrEmpty(doc.Handwrtten))
				{
					stringBuilder.Append(Function.GetJsonString("HandWritten", Function.ToString(doc.Handwrtten)));
				}
				if (!string.IsNullOrEmpty(doc.CardCode))
				{
					stringBuilder.Append(Function.GetJsonString("CardCode", Function.ToString(doc.CardCode)));
				}
				if (!string.IsNullOrEmpty(doc.CardName))
				{
					stringBuilder.Append(Function.GetJsonString("CardName", Function.ToString(doc.CardName)));
				}
				if (doc.CdcOffset != 0)
				{
					stringBuilder.Append(Function.GetJsonString("CashDiscountDateOffset", Function.ParseInt(doc.CdcOffset)));
				}
				if (!string.IsNullOrEmpty(doc.CntrlBnk))
				{
					stringBuilder.Append(Function.GetJsonString("CentralBankIndicator", Function.ToString(doc.CntrlBnk)));
				}
				if (!string.IsNullOrEmpty(doc.Footer))
				{
					stringBuilder.Append(Function.GetJsonString("ClosingRemarks", Function.ToString(doc.Footer)));
				}
				if (!string.IsNullOrEmpty(doc.CtlAccount))
				{
					stringBuilder.Append(Function.GetJsonString("ControlAccount", Function.ToString(doc.CtlAccount)));
				}
				if (!doc.DiscPrcnt.Equals(0.0) || commandType == EnumCommandType.Update)
				{
					stringBuilder.Append(Function.GetJsonString("DiscountPercent", Function.ParseDecimal(doc.DiscPrcnt)));
				}
				if (!string.IsNullOrEmpty(doc.DocCur))
				{
					stringBuilder.Append(Function.GetJsonString("DocCurrency", Function.ToString(doc.DocCur)));
				}
				if (doc.DocDate > new DateTime(1900, 1, 1))
				{
					stringBuilder.Append(Function.GetJsonString("DocDate", Convert.ToDateTime(doc.DocDate).ToString("yyyyMMdd")));
				}
				if (doc.DocDueDate > new DateTime(1900, 1, 1))
				{
					stringBuilder.Append(Function.GetJsonString("DocDueDate", Convert.ToDateTime(doc.DocDueDate).ToString("yyyyMMdd")));
				}
				if (!doc.DocRate.Equals(0.0))
				{
					stringBuilder.Append(Function.GetJsonString("DocRate", Function.ParseDecimal(doc.DocRate)));
				}
				if (!string.IsNullOrEmpty(doc.DocType))
				{
					if (doc.DocType == "S")
					{
						stringBuilder.Append(Function.GetJsonString("DocType", "dDocument_Service"));
					}
					else
					{
						stringBuilder.Append(Function.GetJsonString("DocType", "dDocument_Items"));
					}
				}
				if (new ApzObjectTypes[6]
				{
					ApzObjectTypes.oDownPayments,
					ApzObjectTypes.oPurchaseDownPayments,
					ApzObjectTypes.oInvoices,
					ApzObjectTypes.oPurchaseInvoices,
					ApzObjectTypes.oReserveInvoices,
					ApzObjectTypes.oAPReserveInvoice
				}.Contains(objType) || (objType == ApzObjectTypes.oDrafts && new ApzObjectTypes[6]
				{
					ApzObjectTypes.oDownPayments,
					ApzObjectTypes.oPurchaseDownPayments,
					ApzObjectTypes.oInvoices,
					ApzObjectTypes.oPurchaseInvoices,
					ApzObjectTypes.oReserveInvoices,
					ApzObjectTypes.oAPReserveInvoice
				}.Contains(docObjectCode)))
				{
					if (!doc.DownPayment.Equals(0.0))
					{
						stringBuilder.Append(Function.GetJsonString("DownPayment", Function.ParseDecimal(doc.DownPayment)));
					}
					if (doc.DocCur == text)
					{
						stringBuilder.Append(Function.GetJsonString("DownPaymentAmount", doc.DPMTotal));
					}
					else
					{
						stringBuilder.Append(Function.GetJsonString("DownPaymentAmountFC", doc.DPMTotal));
					}
					if (!doc.DpmPrcnt.Equals(0.0))
					{
						stringBuilder.Append(Function.GetJsonString("DownPaymentPercentage", Function.ParseDecimal(doc.DpmPrcnt)));
					}
				}
				if (doc.TaxDate > new DateTime(1900, 1, 1))
				{
					stringBuilder.Append(Function.GetJsonString("TaxDate", Convert.ToDateTime(doc.TaxDate).ToString("yyyyMMdd")));
				}
				if (!doc.TrnspCode.Equals(0))
				{
					stringBuilder.Append(Function.GetJsonString("TransportationCode", Function.ParseInt(doc.TrnspCode)));
				}
				if (doc.VatDate > new DateTime(1900, 1, 1))
				{
					stringBuilder.Append(Function.GetJsonString("VatDate", Convert.ToDateTime(doc.VatDate).ToString("yyyyMMdd")));
				}
				if (!doc.VatPercent.Equals(0.0))
				{
					stringBuilder.Append(Function.GetJsonString("VatPercent", Function.ParseDecimal(doc.VatPercent)));
				}
				if (objType == ApzObjectTypes.oPurchaseRequest || docObjectCode == ApzObjectTypes.oPurchaseRequest)
				{
					if (!string.IsNullOrEmpty(doc.Requester))
					{
						stringBuilder.Append(Function.GetJsonString("Requester", Function.ToString(doc.Requester)));
					}
					if (!string.IsNullOrEmpty(doc.Email))
					{
						stringBuilder.Append(Function.GetJsonString("RequesterEmail", Function.ToString(doc.Email)));
					}
					if (!string.IsNullOrEmpty(doc.ReqName))
					{
						stringBuilder.Append(Function.GetJsonString("RequesterName", Function.ToString(doc.ReqName)));
					}
					if (!Function.ParseInt(doc.Branch).Equals(0))
					{
						stringBuilder.Append(Function.GetJsonString("RequesterBranch", Function.ParseInt(doc.Branch)));
					}
					if (!Function.ParseInt(doc.Department).Equals(0))
					{
						stringBuilder.Append(Function.GetJsonString("RequesterDepartment", Function.ParseInt(doc.Department)));
					}
					if (!Function.ParseInt(doc.ReqType).Equals(0))
					{
						stringBuilder.Append(Function.GetJsonString("ReqType", Function.ParseInt(doc.ReqType)));
					}
				}
				if (doc.ReqDate > new DateTime(1900, 1, 1))
				{
					stringBuilder.Append(Function.GetJsonString("RequriedDate", Convert.ToDateTime(doc.ReqDate).ToString("yyyyMMdd")));
				}
				if (((objType != ApzObjectTypes.oPurchaseOrders && objType != ApzObjectTypes.oPurchaseRequest && objType != ApzObjectTypes.oPurchaseQuotations && (objType != ApzObjectTypes.oPurchaseDownPayments || !(doc.Posted == "0")) && objType != ApzObjectTypes.oDrafts) || (docObjectCode != ApzObjectTypes.oPurchaseOrders && docObjectCode != ApzObjectTypes.oPurchaseRequest && docObjectCode != ApzObjectTypes.oPurchaseQuotations && (docObjectCode != ApzObjectTypes.oPurchaseDownPayments || !(doc.Posted == "0")) && objType == ApzObjectTypes.oDrafts)) && !string.IsNullOrEmpty(doc.ShipToCode))
				{
					stringBuilder.Append(Function.GetJsonString("ShipToCode", Function.ToString(doc.ShipToCode)));
				}
				if (!string.IsNullOrEmpty(doc.PayToCode))
				{
					stringBuilder.Append(Function.GetJsonString("PayToCode", Function.ToString(doc.PayToCode)));
				}
				stringBuilder.Append(Function.GetJsonString("PickRemark", Function.ToString(doc.PickRmrk)));
			}
			if (!string.IsNullOrEmpty(doc.JrnlMemo))
			{
				stringBuilder.Append(Function.GetJsonString("JournalMemo", Function.ToString(doc.JrnlMemo)));
			}
			if (objType != ApzObjectTypes.oInventoryGenEntry && objType != ApzObjectTypes.oInventoryGenExit)
			{
				if (!string.IsNullOrEmpty(doc.Project) || commandType == EnumCommandType.Add)
				{
					stringBuilder.Append(Function.GetJsonString("Project", Function.ToString(doc.Project)));
				}
				if (!string.IsNullOrEmpty(doc.Indicator))
				{
					stringBuilder.Append(Function.GetJsonString("Indicator", Function.ToString(doc.Indicator)));
				}
				if (!string.IsNullOrEmpty(doc.LicTradNum))
				{
					stringBuilder.Append(Function.GetJsonString("FederalTaxID", Function.ToString(doc.LicTradNum)));
				}
				if (text2.Equals("O") || text2.Equals("OPEN") || commandType == EnumCommandType.Add)
				{
					if (Function.ParseInt(doc.GroupNum) > 0)
					{
						stringBuilder.Append(Function.GetJsonString("PaymentGroupCode", Function.ToString(doc.GroupNum)));
					}
					if (!string.IsNullOrEmpty(doc.PeyMethod))
					{
						stringBuilder.Append(Function.GetJsonString("PaymentMethod", Function.ToString(doc.PeyMethod)));
					}
					if (commandType == EnumCommandType.Add || objType == ApzObjectTypes.oDrafts)
					{
						if (Function.ParseInt(doc.ExtraMonth) > 0)
						{
							stringBuilder.Append(Function.GetJsonString("ExtraMonth", Function.ToString(doc.ExtraMonth)));
							BoPayTermDueTypes boPayTermDueTypes;
							try
							{
								boPayTermDueTypes = doc.StartFrom;
							}
							catch
							{
								boPayTermDueTypes = BoPayTermDueTypes.pdt_None;
							}
							stringBuilder.Append(Function.GetJsonString("StartFrom", Function.ToString(boPayTermDueTypes)));
						}
						if (Function.ParseInt(doc.ExtraDays) > 0)
						{
							stringBuilder.Append(Function.GetJsonString("ExtraDays", Function.ToString(doc.ExtraDays)));
						}
					}
				}
			}
			List<string> listFieldNotValidated = ServiceUnitOfWork.UDFs.GetListFieldNotValidated("ADOC");
			stringBuilder.Append(DataService.MappingUdf(doc.UserFields, commandType, ignoreUpdateEmptyValue, listFieldNotValidated));
			if (doc.Lines != null && doc.Lines.Count > 0)
			{
				listFieldNotValidated = ServiceUnitOfWork.UDFs.GetListFieldNotValidated("ADO1");
				stringBuilder.Append("\"DocumentLines\":[");
				for (int i = 0; i < doc.Lines.Count; i++)
				{
					Document_LinesImpl document_LinesImpl = doc.Lines[i];
					string text3 = Function.ToString(document_LinesImpl.LineStatus).ToUpper();
					stringBuilder.Append("{\r\n");
					if (document_LinesImpl.LineNum >= 0)
					{
						stringBuilder.Append(Function.GetJsonString("LineNum", Function.ParseInt(document_LinesImpl.LineNum)));
					}
					if (flag)
					{
						if (!string.IsNullOrEmpty(document_LinesImpl.AcctCode))
						{
							stringBuilder.Append(Function.GetJsonString("AccountCode", Function.ToString(document_LinesImpl.AcctCode)));
						}
						if (!document_LinesImpl.ActBaseEnt.Equals(0))
						{
							stringBuilder.Append(Function.GetJsonString("ActualBaseEntry", Function.ParseInt(document_LinesImpl.ActBaseEnt)));
						}
						if (document_LinesImpl.ActDelDate > new DateTime(1900, 1, 1))
						{
							stringBuilder.Append(Function.GetJsonString("ActualDeliveryDate", Convert.ToDateTime(document_LinesImpl.ActDelDate).ToString("yyyyMMdd")));
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.Address))
						{
							stringBuilder.Append(Function.GetJsonString("Address", Function.ToString(document_LinesImpl.Address)));
						}
						if (!document_LinesImpl.AgrNo.Equals(0))
						{
							stringBuilder.Append(Function.GetJsonString("AgreementNo", Function.ParseInt(document_LinesImpl.AgrNo)));
						}
						if (!document_LinesImpl.AgrLnNum.Equals(0))
						{
							stringBuilder.Append(Function.GetJsonString("AgreementRowNumber", Function.ParseInt(document_LinesImpl.AgrLnNum)));
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.CodeBars))
						{
							stringBuilder.Append(Function.GetJsonString("BarCode", Function.ToString(document_LinesImpl.CodeBars)));
						}
						if (!document_LinesImpl.BaseType.Equals(0) && !document_LinesImpl.BaseType.Equals(-1))
						{
							stringBuilder.Append(Function.GetJsonString("BaseType", Function.ParseInt(document_LinesImpl.BaseType)));
							if (!document_LinesImpl.BaseEntry.Equals(0))
							{
								stringBuilder.Append(Function.GetJsonString("BaseEntry", Function.ParseInt(document_LinesImpl.BaseEntry)));
							}
							if (!document_LinesImpl.BaseLine.Equals(-1))
							{
								stringBuilder.Append(Function.GetJsonString("BaseLine", Function.ParseInt(document_LinesImpl.BaseLine)));
							}
						}
						if (!document_LinesImpl.Commission.Equals(0.0) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							stringBuilder.Append(Function.GetJsonString("CommisionPercent", Function.ParseDecimal(document_LinesImpl.Commission)));
						}
						if (!string.IsNullOrWhiteSpace(document_LinesImpl.OcrCode))
						{
							stringBuilder.Append(Function.GetJsonString("CostingCode", document_LinesImpl.OcrCode));
						}
						else
						{
							stringBuilder.Append("\"CostingCode\": null,");
						}
						if (!string.IsNullOrWhiteSpace(document_LinesImpl.OcrCode2))
						{
							stringBuilder.Append(Function.GetJsonString("CostingCode2", document_LinesImpl.OcrCode2));
						}
						else
						{
							stringBuilder.Append("\"CostingCode2\": null,");
						}
						if (!string.IsNullOrWhiteSpace(document_LinesImpl.OcrCode3))
						{
							stringBuilder.Append(Function.GetJsonString("CostingCode3", document_LinesImpl.OcrCode3));
						}
						else
						{
							stringBuilder.Append("\"CostingCode3\": null,");
						}
						if (!string.IsNullOrWhiteSpace(document_LinesImpl.OcrCode4))
						{
							stringBuilder.Append(Function.GetJsonString("CostingCode4", document_LinesImpl.OcrCode4));
						}
						else
						{
							stringBuilder.Append("\"CostingCode4\": null,");
						}
						if (!string.IsNullOrWhiteSpace(document_LinesImpl.OcrCode5))
						{
							stringBuilder.Append(Function.GetJsonString("CostingCode5", document_LinesImpl.OcrCode5));
						}
						else
						{
							stringBuilder.Append("\"CostingCode5\": null,");
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.Currency))
						{
							stringBuilder.Append(Function.GetJsonString("Currency", Function.ToString(document_LinesImpl.Currency)));
						}
						if (!document_LinesImpl.ExciseAmt.Equals(0.0))
						{
							stringBuilder.Append(Function.GetJsonString("ExciseAmount", Function.ParseDecimal(document_LinesImpl.ExciseAmt)));
						}
						if (document_LinesImpl.BaseType != 202 && (!string.IsNullOrEmpty(document_LinesImpl.ItemCode) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update)))
						{
							stringBuilder.Append(Function.GetJsonString("ItemCode", Function.ToString(document_LinesImpl.ItemCode)));
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.Dscription) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							stringBuilder.Append(Function.GetJsonString("ItemDescription", Function.ToString(document_LinesImpl.Dscription)));
						}
						if (!document_LinesImpl.InvQty.Equals(0.0))
						{
							stringBuilder.Append(Function.GetJsonString("InventoryQuantity", Function.ParseDecimal(document_LinesImpl.InvQty)));
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.Text) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							stringBuilder.Append(Function.GetJsonString("ItemDetails", Function.ToString(document_LinesImpl.Text)));
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.VendorNum) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							stringBuilder.Append(Function.GetJsonString("VendorNum", Function.ToString(document_LinesImpl.VendorNum)));
						}
						if (!document_LinesImpl.LocCode.Equals(0))
						{
							stringBuilder.Append(Function.GetJsonString("LocationCode", Function.ParseInt(document_LinesImpl.LocCode)));
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.unitMsr) && document_LinesImpl.BaseType <= 0)
						{
							stringBuilder.Append(Function.GetJsonString("MeasureUnit", Function.ToString(document_LinesImpl.unitMsr)));
						}
						if (!document_LinesImpl.SpecifiedPriceBefDi)
						{
							if (document_LinesImpl.BaseType != 202)
							{
								if ((Math.Abs(document_LinesImpl.PriceBefDi) > 1E-07 || commandType == EnumCommandType.Update) && !document_LinesImpl.SpecifiedPriceAfterVat)
								{
									if (doc.PriceMode != "G")
									{
										stringBuilder.Append(Function.GetJsonString("UnitPrice", document_LinesImpl.PriceBefDi));
									}
									else
									{
										stringBuilder.Append(Function.GetJsonString("GrossPrice", document_LinesImpl.PriceAfVAT));
									}
								}
								else if (Math.Abs(document_LinesImpl.PriceAfVAT) > 1E-07)
								{
									stringBuilder.Append(Function.GetJsonString("PriceAfterVAT", document_LinesImpl.PriceAfVAT));
								}
							}
							if (commandType == EnumCommandType.Add || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update) || (ignoreUpdateEmptyValue && Math.Abs(document_LinesImpl.DiscPrcnt) > 1E-07))
							{
								stringBuilder.Append(Function.GetJsonString("DiscountPercent", document_LinesImpl.DiscPrcnt));
							}
						}
						else
						{
							string value = "";
							double num = 0.0;
							if (document_LinesImpl.BaseType != 202)
							{
								if ((!string.IsNullOrEmpty(doc.DocCur) && doc.DocCur == text) || string.IsNullOrEmpty(doc.DocCur))
								{
									value = ((!(doc.PriceMode != "G")) ? Function.GetJsonString("GrossTotal", document_LinesImpl.GTotal) : Function.GetJsonString("LineTotal", document_LinesImpl.LineTotal));
									num = document_LinesImpl.LineTotal;
								}
								else
								{
									value = ((!(doc.PriceMode != "G")) ? Function.GetJsonString("GrossTotalFC", document_LinesImpl.GTotalFC) : Function.GetJsonString("RowTotalFC", document_LinesImpl.TotalFrgn));
									num = Function.ParseDouble(document_LinesImpl.TotalFrgn);
								}
							}
							if (Math.Abs(num) < 1E-07 && Math.Abs(document_LinesImpl.PriceBefDi) < 1E-07 && Math.Abs(document_LinesImpl.PriceAfVAT) < 1E-07)
							{
								if (document_LinesImpl.BaseType != 202)
								{
									stringBuilder.Append(Function.GetJsonString("UnitPrice", 0));
									stringBuilder.Append(value);
								}
							}
							else
							{
								if (document_LinesImpl.BaseType != 202)
								{
									stringBuilder.Append(value);
								}
								if (document_LinesImpl.DiscPrcnt > 0.0)
								{
									if (document_LinesImpl.BaseType != 202)
									{
										if ((Math.Abs(document_LinesImpl.PriceBefDi) > 1E-07 || commandType == EnumCommandType.Update) && !document_LinesImpl.SpecifiedPriceAfterVat)
										{
											if (doc.PriceMode != "G")
											{
												stringBuilder.Append(Function.GetJsonString("UnitPrice", document_LinesImpl.PriceBefDi));
											}
											else
											{
												stringBuilder.Append(Function.GetJsonString("GrossPrice", document_LinesImpl.PriceAfVAT));
											}
										}
										else if (Math.Abs(document_LinesImpl.PriceAfVAT) > 1E-07)
										{
											stringBuilder.Append(Function.GetJsonString("PriceAfterVAT", document_LinesImpl.PriceAfVAT));
										}
										if (commandType == EnumCommandType.Update)
										{
											stringBuilder.Append(Function.GetJsonString("DiscountPercent", document_LinesImpl.DiscPrcnt));
										}
									}
								}
								else
								{
									stringBuilder.Append(Function.GetJsonString("DiscountPercent", 0));
									if (document_LinesImpl.BaseType != 202)
									{
										if (commandType != EnumCommandType.Update)
										{
											if (Math.Abs(document_LinesImpl.Quantity * document_LinesImpl.PriceBefDi * (1.0 - document_LinesImpl.DiscPrcnt / 100.0) - num) < 1E-07)
											{
												if (doc.PriceMode != "G")
												{
													stringBuilder.Append(Function.GetJsonString("UnitPrice", document_LinesImpl.PriceBefDi));
												}
												else
												{
													stringBuilder.Append(Function.GetJsonString("GrossPrice", document_LinesImpl.PriceAfVAT));
												}
											}
											else if ((Math.Abs(document_LinesImpl.PriceBefDi) < 1E-07 && Math.Abs(document_LinesImpl.PriceAfVAT) > 1E-07) || document_LinesImpl.SpecifiedPriceAfterVat)
											{
												stringBuilder.Append(Function.GetJsonString("PriceAfterVAT", document_LinesImpl.PriceAfVAT));
											}
											else
											{
												stringBuilder.Append(Function.GetJsonString("UnitPrice", 0));
											}
										}
										else if ((Math.Abs(document_LinesImpl.PriceAfVAT) > 1E-07 && Math.Abs(document_LinesImpl.PriceBefDi) < 1E-07) || document_LinesImpl.SpecifiedPriceAfterVat)
										{
											stringBuilder.Append(Function.GetJsonString("PriceAfterVAT", document_LinesImpl.PriceAfVAT));
										}
										else if (doc.PriceMode != "G")
										{
											stringBuilder.Append(Function.GetJsonString("UnitPrice", document_LinesImpl.PriceBefDi));
										}
										else
										{
											stringBuilder.Append(Function.GetJsonString("GrossPrice", document_LinesImpl.PriceAfVAT));
										}
									}
								}
							}
						}
						if (!new int[8] { 15, 13, 14, 16, 18, 19, 20, 21 }.Contains(document_LinesImpl.BaseType))
						{
							if (!string.IsNullOrEmpty(document_LinesImpl.CogsAcct))
							{
								stringBuilder.Append(Function.GetJsonString("COGSAccountCode", Function.RawText(document_LinesImpl.CogsAcct)));
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.CogsOcrCod))
							{
								stringBuilder.Append(Function.GetJsonString("COGSCostingCode", Function.RawText(document_LinesImpl.CogsOcrCod)));
							}
							else
							{
								stringBuilder.Append("\"COGSCostingCode\": null,");
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.CogsOcrCo2))
							{
								stringBuilder.Append(Function.GetJsonString("COGSCostingCode2", Function.RawText(document_LinesImpl.CogsOcrCo2)));
							}
							else
							{
								stringBuilder.Append("\"COGSCostingCode2\": null,");
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.CogsOcrCo3))
							{
								stringBuilder.Append(Function.GetJsonString("COGSCostingCode3", Function.RawText(document_LinesImpl.CogsOcrCo3)));
							}
							else
							{
								stringBuilder.Append("\"COGSCostingCode3\": null,");
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.CogsOcrCo4))
							{
								stringBuilder.Append(Function.GetJsonString("COGSCostingCode4", Function.RawText(document_LinesImpl.CogsOcrCo4)));
							}
							else
							{
								stringBuilder.Append("\"COGSCostingCode4\": null,");
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.CogsOcrCo5))
							{
								stringBuilder.Append(Function.GetJsonString("COGSCostingCode5", Function.RawText(document_LinesImpl.CogsOcrCo5)));
							}
							else
							{
								stringBuilder.Append("\"COGSCostingCode5\": null,");
							}
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.Project) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update) || commandType == EnumCommandType.Add)
						{
							stringBuilder.Append(Function.GetJsonString("ProjectCode", Function.ToString(document_LinesImpl.Project)));
						}
						if (!document_LinesImpl.Quantity.Equals(0.0) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							stringBuilder.Append(Function.GetJsonString("Quantity", Function.ParseDecimal(document_LinesImpl.Quantity)));
						}
						if (document_LinesImpl.LineStatus != "C" && document_LinesImpl.TargetType <= 0 && !document_LinesImpl.Rate.Equals(0.0) && (commandType == EnumCommandType.Add || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update && (document_LinesImpl.TreeType == "N" || (document_LinesImpl.TreeType == "I" && document_LinesImpl.BOMTreeType != "0" && Function.ParseInt(document_LinesImpl.BOMTreeType) == 0)))))
						{
							if (!string.IsNullOrEmpty(document_LinesImpl.Currency) && document_LinesImpl.Currency == text)
							{
								stringBuilder.Append(Function.GetJsonString("Rate", 0));
							}
							else
							{
								stringBuilder.Append(Function.GetJsonString("Rate", Function.ParseDecimal(document_LinesImpl.Rate)));
							}
						}
						if (document_LinesImpl.PQTReqDate > new DateTime(1900, 1, 1))
						{
							stringBuilder.Append(Function.GetJsonString("RequiredDate", Convert.ToDateTime(document_LinesImpl.PQTReqDate).ToString("yyyyMMdd")));
						}
						if (!document_LinesImpl.PQTReqQty.Equals(0.0))
						{
							stringBuilder.Append(Function.GetJsonString("RequiredQuantity", Function.ParseDecimal(document_LinesImpl.PQTReqQty)));
						}
						if (!document_LinesImpl.PackQty.Equals(0.0) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							stringBuilder.Append(Function.GetJsonString("PackageQuantity", Function.ParseDecimal(document_LinesImpl.PackQty)));
						}
						if (!document_LinesImpl.SlpCode.Equals(0))
						{
							stringBuilder.Append(Function.GetJsonString("SalesPersonCode", Function.ParseInt(document_LinesImpl.SlpCode)));
						}
						_ = document_LinesImpl.ShipDate;
						if (document_LinesImpl.ShipDate > new DateTime(1900, 1, 1))
						{
							stringBuilder.Append(Function.GetJsonString("ShipDate", Convert.ToDateTime(document_LinesImpl.ShipDate).ToString("yyyyMMdd")));
						}
						if (!document_LinesImpl.TrnsCode.Equals(0))
						{
							stringBuilder.Append(Function.GetJsonString("ShippingMethod", Function.ParseInt(document_LinesImpl.TrnsCode)));
						}
						if (objType != ApzObjectTypes.oPurchaseOrders && objType != ApzObjectTypes.oDrafts && docObjectCode != ApzObjectTypes.oPurchaseOrders && objType == ApzObjectTypes.oDrafts)
						{
							if (!string.IsNullOrEmpty(document_LinesImpl.ShipToCode) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								stringBuilder.Append(Function.GetJsonString("ShipToCode", Function.ToString(document_LinesImpl.ShipToCode)));
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.ShipToDesc) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								stringBuilder.Append(Function.GetJsonString("ShipToDescription", Function.ToString(document_LinesImpl.ShipToDesc)));
							}
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.TaxCode))
						{
							stringBuilder.Append(Function.GetJsonString("TaxCode", Function.ToString(document_LinesImpl.TaxCode)));
						}
						if (document_LinesImpl.NumPerMsr > 0.0)
						{
							stringBuilder.Append(Function.GetJsonString("UnitsOfMeasurment", document_LinesImpl.NumPerMsr));
						}
						else
						{
							stringBuilder.Append(Function.GetJsonString("UnitsOfMeasurment", 1));
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.UseBaseUn) && new string[4] { "N", "Y", "tYES", "tNO" }.Contains(document_LinesImpl.UseBaseUn))
						{
							stringBuilder.Append(Function.GetJsonString("UseBaseUnits", Function.ToString(document_LinesImpl.UseBaseUn)));
						}
						if (!document_LinesImpl.UomEntry.Equals(0))
						{
							stringBuilder.Append(Function.GetJsonString("UoMEntry", Function.ParseInt(document_LinesImpl.UomEntry)));
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.VatGroup))
						{
							stringBuilder.Append(Function.GetJsonString("VatGroup", Function.ToString(document_LinesImpl.VatGroup)));
						}
						if (doc.DocCur == text)
						{
							stringBuilder.Append(Function.GetJsonString("TaxTotal", document_LinesImpl.LineVat));
						}
						else
						{
							stringBuilder.Append(Function.GetJsonString("TaxTotal", document_LinesImpl.VatSumFrgn));
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.WhsCode))
						{
							stringBuilder.Append(Function.GetJsonString("WarehouseCode", Function.ToString(document_LinesImpl.WhsCode)));
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.SubCatNum) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							stringBuilder.Append(Function.GetJsonString("SupplierCatNum", Function.ToString(document_LinesImpl.SubCatNum)));
						}
						if (objType == ApzObjectTypes.oPurchaseRequest || (objType == ApzObjectTypes.oDrafts && docObjectCode == ApzObjectTypes.oPurchaseRequest))
						{
							stringBuilder.Append(Function.GetJsonString("LineVendor", Function.ToString(document_LinesImpl.LineVendor)));
						}
						if (new ApzObjectTypes[2]
						{
							ApzObjectTypes.oCreditNotes,
							ApzObjectTypes.oPurchaseCreditNotes
						}.Contains(objType) || (objType == ApzObjectTypes.oDrafts && new ApzObjectTypes[2]
						{
							ApzObjectTypes.oCreditNotes,
							ApzObjectTypes.oPurchaseCreditNotes
						}.Contains(docObjectCode)))
						{
							stringBuilder.Append((document_LinesImpl.NoInvtryMv == "Y") ? Function.GetJsonString("WithoutInventoryMovement", "tYES") : Function.GetJsonString("WithoutInventoryMovement", "tNO"));
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.TreeType) && !"0".Equals(document_LinesImpl.TreeType))
						{
							stringBuilder.Append(Function.GetJsonString("TreeType", Function.ToString(document_LinesImpl.TreeType)));
						}
						if (objType == ApzObjectTypes.oReturns)
						{
							if (!string.IsNullOrEmpty(document_LinesImpl.EnSetCost) && document_LinesImpl.EnSetCost.Equals("Y"))
							{
								stringBuilder.Append(Function.GetJsonString("EnableReturnCost", "tYES"));
								stringBuilder.Append(Function.GetJsonString("ReturnCost", Function.ParseDecimal(document_LinesImpl.RetCost)));
							}
							else
							{
								stringBuilder.Append(Function.GetJsonString("EnableReturnCost", "tNO"));
							}
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.TaxOnly))
						{
							stringBuilder.Append(Function.GetJsonString("TaxOnly", Function.ToString(document_LinesImpl.TaxOnly)));
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.CountryOrg))
						{
							stringBuilder.Append(Function.GetJsonString("CountryOrg", document_LinesImpl.CountryOrg));
						}
					}
					if (text2.Equals("O") || text2.Equals("OPEN") || text3.Equals("OPEN") || text3.Equals("O"))
					{
						stringBuilder.Append(Function.GetJsonString("FreeText", Function.ToString(document_LinesImpl.FreeTxt)));
					}
					if (!string.IsNullOrEmpty(document_LinesImpl.LineStatus) && document_LinesImpl.LineStatus == "C" && (new ApzObjectTypes[5]
					{
						ApzObjectTypes.oQuotations,
						ApzObjectTypes.oOrders,
						ApzObjectTypes.oPurchaseRequest,
						ApzObjectTypes.oPurchaseOrders,
						ApzObjectTypes.oPurchaseQuotations
					}.Contains(objType) || (new ApzObjectTypes[5]
					{
						ApzObjectTypes.oQuotations,
						ApzObjectTypes.oOrders,
						ApzObjectTypes.oPurchaseRequest,
						ApzObjectTypes.oPurchaseOrders,
						ApzObjectTypes.oPurchaseQuotations
					}.Contains(docObjectCode) && objType == ApzObjectTypes.oDrafts)))
					{
						stringBuilder.Append(Function.GetJsonString("LineStatus", "bost_Close"));
					}
					if (objType == ApzObjectTypes.oInventoryGenEntry && doc.Lines.FirstOrDefault((Document_LinesImpl t) => t.BaseType == 202) != null && !doc.Lines.Any((Document_LinesImpl t) => t.BaseLine >= 0))
					{
						stringBuilder.Append(Function.GetJsonString("TransactionType", (document_LinesImpl.TranType == "R") ? "botrntReject" : "botrntComplete"));
					}
					stringBuilder.Append(DataService.MappingUdf(document_LinesImpl.UserFields, commandType, ignoreUpdateEmptyValue, listFieldNotValidated));
					stringBuilder.Append("\"BatchNumbers\":[");
					if (doc.Lines[i].BatchNumbers != null && doc.Lines[i].BatchNumbers.Count > 0)
					{
						for (int num2 = 0; num2 < doc.Lines[i].BatchNumbers.Count; num2++)
						{
							stringBuilder.Append("{\r\n");
							BatchNumbersImpl batchNumbersImpl = doc.Lines[i].BatchNumbers[num2];
							if (!string.IsNullOrEmpty(batchNumbersImpl.BatchNumber))
							{
								stringBuilder.Append(Function.GetJsonString("BatchNumber", Function.ToString(batchNumbersImpl.BatchNumber)));
							}
							if (!string.IsNullOrEmpty(batchNumbersImpl.ManufacturerSerialNumber))
							{
								stringBuilder.Append(Function.GetJsonString("ManufacturerSerialNumber", Function.ToString(batchNumbersImpl.ManufacturerSerialNumber)));
							}
							if (!string.IsNullOrEmpty(batchNumbersImpl.InternalSerialNumber))
							{
								stringBuilder.Append(Function.GetJsonString("InternalSerialNumber", Function.ToString(batchNumbersImpl.InternalSerialNumber)));
							}
							if (batchNumbersImpl.ExpiryDate.HasValue && batchNumbersImpl.ExpiryDate > new DateTime(1900, 1, 1))
							{
								stringBuilder.Append(Function.GetJsonString("ExpiryDate", Convert.ToDateTime(batchNumbersImpl.ExpiryDate).ToString("yyyyMMdd")));
							}
							else
							{
								stringBuilder.Append(Function.GetJsonString("ExpiryDate", null));
							}
							if (batchNumbersImpl.ManufacturingDate.HasValue && batchNumbersImpl.ManufacturingDate > new DateTime(1900, 1, 1))
							{
								stringBuilder.Append(Function.GetJsonString("ManufacturingDate", Convert.ToDateTime(batchNumbersImpl.ManufacturingDate).ToString("yyyyMMdd")));
							}
							else
							{
								stringBuilder.Append(Function.GetJsonString("ManufacturingDate", null));
							}
							if (batchNumbersImpl.AddmisionDate.HasValue && batchNumbersImpl.AddmisionDate > new DateTime(1900, 1, 1))
							{
								stringBuilder.Append(Function.GetJsonString("AddmisionDate", Convert.ToDateTime(batchNumbersImpl.AddmisionDate).ToString("yyyyMMdd")));
							}
							else
							{
								stringBuilder.Append(Function.GetJsonString("AddmisionDate", null));
							}
							if (!string.IsNullOrEmpty(batchNumbersImpl.Location))
							{
								stringBuilder.Append(Function.GetJsonString("Location", Function.ToString(batchNumbersImpl.Location)));
							}
							if (!string.IsNullOrEmpty(batchNumbersImpl.Notes))
							{
								stringBuilder.Append(Function.GetJsonString("Notes", Function.ToString(batchNumbersImpl.Notes)));
							}
							if (commandType == EnumCommandType.Update || batchNumbersImpl.Quantity > 0.0)
							{
								stringBuilder.Append(Function.GetJsonString("Quantity", Function.ParseDecimal(batchNumbersImpl.Quantity)));
							}
							if (0 < batchNumbersImpl.TrackingNote)
							{
								stringBuilder.Append(Function.GetJsonString("TrackingNote", Function.ToString(batchNumbersImpl.TrackingNote)));
							}
							if (0 < batchNumbersImpl.TrackingNoteLine)
							{
								stringBuilder.Append(Function.GetJsonString("TrackingNoteLine", Function.ToString(batchNumbersImpl.TrackingNoteLine)));
							}
							stringBuilder.Append(DataService.MappingUdf(batchNumbersImpl.UserFields, EnumCommandType.Add, ignoreUpdateEmptyValue));
							stringBuilder.Append("},");
						}
					}
					stringBuilder.Append("],");
					stringBuilder.Append("\"SerialNumbers\":[");
					if (doc.Lines[i].SerialNumbers != null && doc.Lines[i].SerialNumbers.Count > 0)
					{
						for (int num3 = 0; num3 < doc.Lines[i].SerialNumbers.Count; num3++)
						{
							stringBuilder.Append("{\r\n");
							SerialNumbersImpl serialNumbersImpl = doc.Lines[i].SerialNumbers[num3];
							if (!string.IsNullOrEmpty(serialNumbersImpl.BatchID))
							{
								stringBuilder.Append(Function.GetJsonString("BatchID", Function.ToString(serialNumbersImpl.BatchID)));
							}
							if (serialNumbersImpl.SystemSerialNumber > 0)
							{
								stringBuilder.Append(Function.GetJsonString("SystemSerialNumber", serialNumbersImpl.SystemSerialNumber));
							}
							if (!string.IsNullOrEmpty(serialNumbersImpl.ManufacturerSerialNumber))
							{
								stringBuilder.Append(Function.GetJsonString("ManufacturerSerialNumber", Function.ToString(serialNumbersImpl.ManufacturerSerialNumber)));
							}
							if (!string.IsNullOrEmpty(serialNumbersImpl.InternalSerialNumber))
							{
								stringBuilder.Append(Function.GetJsonString("InternalSerialNumber", Function.ToString(serialNumbersImpl.InternalSerialNumber)));
							}
							if (!new ApzObjectTypes[6]
							{
								ApzObjectTypes.oDeliveryNotes,
								ApzObjectTypes.oInvoices,
								ApzObjectTypes.oReserveInvoices,
								ApzObjectTypes.oInventoryGenExit,
								ApzObjectTypes.oStockTransfer,
								ApzObjectTypes.oInventoryTransferRequest
							}.Contains(objType))
							{
								if (serialNumbersImpl.ExpiryDate.HasValue && serialNumbersImpl.ExpiryDate > new DateTime(1900, 1, 1))
								{
									stringBuilder.Append(Function.GetJsonString("ExpiryDate", Convert.ToDateTime(serialNumbersImpl.ExpiryDate).ToString("yyyyMMdd")));
								}
								else
								{
									stringBuilder.Append(Function.GetJsonString("ExpiryDate", null));
								}
								if (serialNumbersImpl.ManufactureDate.HasValue && serialNumbersImpl.ManufactureDate > new DateTime(1900, 1, 1))
								{
									stringBuilder.Append(Function.GetJsonString("ManufactureDate", Convert.ToDateTime(serialNumbersImpl.ManufactureDate).ToString("yyyyMMdd")));
								}
								else
								{
									stringBuilder.Append(Function.GetJsonString("ManufactureDate", null));
								}
								if (serialNumbersImpl.ReceptionDate.HasValue && serialNumbersImpl.ReceptionDate > new DateTime(1900, 1, 1))
								{
									stringBuilder.Append(Function.GetJsonString("ReceptionDate", Convert.ToDateTime(serialNumbersImpl.ReceptionDate).ToString("yyyyMMdd")));
								}
								else
								{
									stringBuilder.Append(Function.GetJsonString("ReceptionDate", null));
								}
							}
							if (!string.IsNullOrEmpty(serialNumbersImpl.Location))
							{
								stringBuilder.Append(Function.GetJsonString("Location", Function.ToString(serialNumbersImpl.Location)));
							}
							if (!string.IsNullOrEmpty(serialNumbersImpl.Notes))
							{
								stringBuilder.Append(Function.GetJsonString("Notes", Function.ToString(serialNumbersImpl.Notes)));
							}
							if (commandType == EnumCommandType.Update || serialNumbersImpl.Quantity > 0.0)
							{
								stringBuilder.Append(Function.GetJsonString("Quantity", Function.ParseDecimal(serialNumbersImpl.Quantity)));
							}
							if (serialNumbersImpl.BaseLineNumber >= 0)
							{
								stringBuilder.Append(Function.GetJsonString("BaseLineNumber", Function.ParseInt(serialNumbersImpl.BaseLineNumber)));
							}
							if (0 < serialNumbersImpl.TrackingNote)
							{
								stringBuilder.Append(Function.GetJsonString("TrackingNote", Function.ToString(serialNumbersImpl.TrackingNote)));
							}
							if (0 < serialNumbersImpl.TrackingNoteLine)
							{
								stringBuilder.Append(Function.GetJsonString("TrackingNoteLine", Function.ToString(serialNumbersImpl.TrackingNoteLine)));
							}
							stringBuilder.Append(DataService.MappingUdf(serialNumbersImpl.UserFields, EnumCommandType.Add, ignoreUpdateEmptyValue));
							stringBuilder.Append("},");
						}
					}
					stringBuilder.Append("],");
					stringBuilder.Append("\"DocumentLinesBinAllocations\":[");
					if (doc.Lines[i].BinAllocations != null && doc.Lines[i].BinAllocations.Count > 0)
					{
						for (int num4 = 0; num4 < doc.Lines[i].BinAllocations.Count; num4++)
						{
							stringBuilder.Append("{\r\n");
							DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl = doc.Lines[i].BinAllocations[num4];
							if (!documentLinesBinAllocationsImpl.BinAbsEntry.Equals(0))
							{
								stringBuilder.Append(Function.GetJsonString("BinAbsEntry", Function.ParseInt(documentLinesBinAllocationsImpl.BinAbsEntry)));
							}
							if (!documentLinesBinAllocationsImpl.Quantity.Equals(0.0))
							{
								stringBuilder.Append(Function.GetJsonString("Quantity", Function.ParseDecimal(documentLinesBinAllocationsImpl.Quantity)));
							}
							if (documentLinesBinAllocationsImpl.SerialAndBatchNumbersBaseLine.HasValue && documentLinesBinAllocationsImpl.SerialAndBatchNumbersBaseLine >= 0)
							{
								stringBuilder.Append(Function.GetJsonString("SerialAndBatchNumbersBaseLine", Function.ParseInt(documentLinesBinAllocationsImpl.SerialAndBatchNumbersBaseLine)));
							}
							else
							{
								stringBuilder.Append(Function.GetJsonString("SerialAndBatchNumbersBaseLine", -1));
							}
							if (documentLinesBinAllocationsImpl.BaseLineNumber >= 0)
							{
								stringBuilder.Append(Function.GetJsonString("BaseLineNumber", Function.ParseInt(documentLinesBinAllocationsImpl.BaseLineNumber)));
							}
							stringBuilder.Append("},");
						}
					}
					stringBuilder.Append("],");
					if (document_LinesImpl != null)
					{
						stringBuilder.Append("\"DocumentLineAdditionalExpenses\":[");
						if (document_LinesImpl.ExpnsCode_F1 > 0)
						{
							stringBuilder.Append("{\r\n");
							if (commandType != EnumCommandType.Add)
							{
								stringBuilder.Append(Function.GetJsonString("LineNumber", document_LinesImpl.LineNum));
							}
							else
							{
								stringBuilder.Append(Function.GetJsonString("LineNumber", i));
							}
							stringBuilder.Append(Function.GetJsonString("GroupCode", 0));
							stringBuilder.Append(Function.GetJsonString("ExpenseCode", document_LinesImpl.ExpnsCode_F1));
							if (!Function.ParseDecimal(document_LinesImpl.LineTotal_F1).Equals(0m) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								stringBuilder.Append(Function.GetJsonString("LineTotal", Function.ParseDecimal(document_LinesImpl.LineTotal_F1)));
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.VatGroup_F1) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								stringBuilder.Append(Function.GetJsonString("VatGroup", Function.ToString(document_LinesImpl.VatGroup_F1)));
							}
							if (!Function.ParseDecimal(document_LinesImpl.VatPrcnt_F1).Equals(0m) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								stringBuilder.Append(Function.GetJsonString("TaxPercent", Function.ParseDecimal(document_LinesImpl.VatPrcnt_F1)));
							}
							if (!Function.ParseDecimal(document_LinesImpl.VatSum_F1).Equals(0m) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								stringBuilder.Append(Function.GetJsonString("TaxSum", Function.ParseDecimal(document_LinesImpl.VatSum_F1)));
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode_F1) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								stringBuilder.Append(Function.GetJsonString("DistributionRule", Function.ToString(document_LinesImpl.OcrCode_F1)));
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode2_F1) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								stringBuilder.Append(Function.GetJsonString("DistributionRule2", Function.ToString(document_LinesImpl.OcrCode2_F1)));
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode3_F1) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								stringBuilder.Append(Function.GetJsonString("DistributionRule3", Function.ToString(document_LinesImpl.OcrCode3_F1)));
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode4_F1) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								stringBuilder.Append(Function.GetJsonString("DistributionRule4", Function.ToString(document_LinesImpl.OcrCode4_F1)));
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode5_F1) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								stringBuilder.Append(Function.GetJsonString("DistributionRule5", Function.ToString(document_LinesImpl.OcrCode5_F1)));
							}
							stringBuilder.Append("},");
						}
						if (document_LinesImpl.ExpnsCode_F2 > 0)
						{
							stringBuilder.Append("{");
							if (commandType != EnumCommandType.Add)
							{
								stringBuilder.Append(Function.GetJsonString("LineNumber", document_LinesImpl.LineNum));
							}
							else
							{
								stringBuilder.Append(Function.GetJsonString("LineNumber", i));
							}
							stringBuilder.Append(Function.GetJsonString("GroupCode", 1));
							stringBuilder.Append(Function.GetJsonString("ExpenseCode", document_LinesImpl.ExpnsCode_F2));
							if (!Function.ParseDecimal(document_LinesImpl.LineTotal_F2).Equals(0m) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								stringBuilder.Append(Function.GetJsonString("LineTotal", Function.ParseDecimal(document_LinesImpl.LineTotal_F2)));
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.VatGroup_F2) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								stringBuilder.Append(Function.GetJsonString("VatGroup", Function.ToString(document_LinesImpl.VatGroup_F2)));
							}
							if (!Function.ParseDecimal(document_LinesImpl.VatPrcnt_F2).Equals(0m) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								stringBuilder.Append(Function.GetJsonString("TaxPercent", Function.ParseDecimal(document_LinesImpl.VatPrcnt_F2)));
							}
							if (!Function.ParseDecimal(document_LinesImpl.VatSum_F2).Equals(0m) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								stringBuilder.Append(Function.GetJsonString("TaxSum", Function.ParseDecimal(document_LinesImpl.VatSum_F2)));
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode_F2) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								stringBuilder.Append(Function.GetJsonString("DistributionRule", Function.ToString(document_LinesImpl.OcrCode_F2)));
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode2_F2) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								stringBuilder.Append(Function.GetJsonString("DistributionRule2", Function.ToString(document_LinesImpl.OcrCode2_F2)));
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode3_F2) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								stringBuilder.Append(Function.GetJsonString("DistributionRule3", Function.ToString(document_LinesImpl.OcrCode3_F2)));
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode4_F2) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								stringBuilder.Append(Function.GetJsonString("DistributionRule4", Function.ToString(document_LinesImpl.OcrCode4_F2)));
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode5_F2) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								stringBuilder.Append(Function.GetJsonString("DistributionRule5", Function.ToString(document_LinesImpl.OcrCode5_F2)));
							}
							stringBuilder.Append("},");
						}
						if (document_LinesImpl.ExpnsCode_F3 > 0)
						{
							stringBuilder.Append("{");
							if (commandType != EnumCommandType.Add)
							{
								stringBuilder.Append(Function.GetJsonString("LineNumber", document_LinesImpl.LineNum));
							}
							else
							{
								stringBuilder.Append(Function.GetJsonString("LineNumber", i));
							}
							stringBuilder.Append(Function.GetJsonString("GroupCode", 2));
							stringBuilder.Append(Function.GetJsonString("ExpenseCode", document_LinesImpl.ExpnsCode_F3));
							if (!Function.ParseDecimal(document_LinesImpl.LineTotal_F3).Equals(0m) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								stringBuilder.Append(Function.GetJsonString("LineTotal", Function.ParseDecimal(document_LinesImpl.LineTotal_F3)));
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.VatGroup_F3) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								stringBuilder.Append(Function.GetJsonString("VatGroup", Function.ToString(document_LinesImpl.VatGroup_F3)));
							}
							if (!Function.ParseDecimal(document_LinesImpl.VatPrcnt_F3).Equals(0m) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								stringBuilder.Append(Function.GetJsonString("TaxPercent", Function.ParseDecimal(document_LinesImpl.VatPrcnt_F3)));
							}
							if (!Function.ParseDecimal(document_LinesImpl.VatSum_F3).Equals(0m) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								stringBuilder.Append(Function.GetJsonString("TaxSum", Function.ParseDecimal(document_LinesImpl.VatSum_F3)));
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode_F3) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								stringBuilder.Append(Function.GetJsonString("DistributionRule", Function.ToString(document_LinesImpl.OcrCode_F3)));
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode2_F3) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								stringBuilder.Append(Function.GetJsonString("DistributionRule2", Function.ToString(document_LinesImpl.OcrCode2_F3)));
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode3_F3) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								stringBuilder.Append(Function.GetJsonString("DistributionRule3", Function.ToString(document_LinesImpl.OcrCode3_F3)));
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode4_F3) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								stringBuilder.Append(Function.GetJsonString("DistributionRule4", Function.ToString(document_LinesImpl.OcrCode4_F3)));
							}
							if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode5_F3) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
							{
								stringBuilder.Append(Function.GetJsonString("DistributionRule5", Function.ToString(document_LinesImpl.OcrCode5_F3)));
							}
							stringBuilder.Append("},");
						}
						stringBuilder.Append("],");
					}
					stringBuilder.Append("},");
				}
				stringBuilder.Append("],");
			}
			stringBuilder.Append("\"DownPaymentsToDraw\": [");
			if ((new ApzObjectTypes[4]
			{
				ApzObjectTypes.oInvoices,
				ApzObjectTypes.oReserveInvoices,
				ApzObjectTypes.oAPReserveInvoice,
				ApzObjectTypes.oPurchaseInvoices
			}.Contains(objType) || (objType == ApzObjectTypes.oDrafts && new ApzObjectTypes[4]
			{
				ApzObjectTypes.oInvoices,
				ApzObjectTypes.oReserveInvoices,
				ApzObjectTypes.oAPReserveInvoice,
				ApzObjectTypes.oPurchaseInvoices
			}.Contains(docObjectCode))) && (commandType == EnumCommandType.Add || objType == ApzObjectTypes.oDrafts) && doc.DownPaymentsToDraw != null && doc.DownPaymentsToDraw.Any())
			{
				foreach (DownPaymentsToDrawImpl item in doc.DownPaymentsToDraw)
				{
					stringBuilder.Append("{");
					stringBuilder.Append(Function.GetJsonString("DocEntry", item.DocEntry));
					stringBuilder.Append("\"DownPaymentsToDrawDetails\": [");
					foreach (DownPaymentToDrawDetailImpl downPaymentsToDrawDetail in item.DownPaymentsToDrawDetails)
					{
						stringBuilder.Append("{");
						stringBuilder.Append(Function.GetJsonString("VatGroupCode", downPaymentsToDrawDetail.VatGroupCode));
						if (downPaymentsToDrawDetail.AmountToDraw != 0.0)
						{
							stringBuilder.Append(Function.GetJsonString("AmountToDraw", downPaymentsToDrawDetail.AmountToDraw));
						}
						if (downPaymentsToDrawDetail.AmountToDrawFC != 0.0)
						{
							stringBuilder.Append(Function.GetJsonString("AmountToDrawFC", downPaymentsToDrawDetail.AmountToDrawFC));
						}
						if (downPaymentsToDrawDetail.GrossAmountToDraw != 0.0)
						{
							stringBuilder.Append(Function.GetJsonString("GrossAmountToDraw", downPaymentsToDrawDetail.GrossAmountToDraw));
						}
						if (downPaymentsToDrawDetail.GrossAmountToDrawFC != 0.0)
						{
							stringBuilder.Append(Function.GetJsonString("GrossAmountToDrawFC", downPaymentsToDrawDetail.GrossAmountToDrawFC));
						}
						stringBuilder.Append("},");
					}
					stringBuilder.Append("],");
					stringBuilder.Append("},");
				}
			}
			stringBuilder.Append("],");
			stringBuilder.Append("\"DocumentSpecialLines\":[");
			if (doc.SpecialLines != null && doc.SpecialLines.Count > 0)
			{
				for (int num5 = 0; num5 < doc.SpecialLines.Count; num5++)
				{
					stringBuilder.Append("{");
					if (doc.SpecialLines[num5].LineType == BoDocSpecialLineType.dslt_Subtotal)
					{
						stringBuilder.Append(Function.GetJsonString("LineType", "dslt_Subtotal"));
					}
					else
					{
						stringBuilder.Append(Function.GetJsonString("LineType", "dslt_Text"));
					}
					stringBuilder.Append(Function.GetJsonString("AfterLineNumber", doc.SpecialLines[num5].AfterLineNumber));
					stringBuilder.Append(Function.GetJsonString("OrderNumber", doc.SpecialLines[num5].OrderNumber));
					if (!string.IsNullOrEmpty(doc.SpecialLines[num5].LineText))
					{
						stringBuilder.Append(Function.GetJsonString("LineText", Function.ToString(doc.SpecialLines[num5].LineText)));
					}
					stringBuilder.Append("},");
				}
			}
			stringBuilder.Append("],");
			stringBuilder.Append("\"DocumentAdditionalExpenses\":[");
			if (doc.Expenses != null && doc.Expenses.Count > 0)
			{
				for (int num6 = 0; num6 < doc.Expenses.Count; num6++)
				{
					DocumentsAdditionalExpensesImpl documentsAdditionalExpensesImpl = doc.Expenses[num6];
					if (documentsAdditionalExpensesImpl == null || string.IsNullOrEmpty(documentsAdditionalExpensesImpl.Status) || !documentsAdditionalExpensesImpl.Status.ToUpper().Equals("O") || !(documentsAdditionalExpensesImpl.LineTotal > 0.0))
					{
						continue;
					}
					stringBuilder.Append("{");
					if (!Function.ParseInt(documentsAdditionalExpensesImpl.ExpnsCode).Equals(0))
					{
						stringBuilder.Append(Function.GetJsonString("ExpenseCode", Function.ParseInt(documentsAdditionalExpensesImpl.ExpnsCode)));
					}
					if (!Function.ParseDecimal(documentsAdditionalExpensesImpl.TotalFrgn).Equals(0m))
					{
						stringBuilder.Append(Function.GetJsonString("LineTotal", Function.ParseDecimal(documentsAdditionalExpensesImpl.TotalFrgn)));
					}
					else if (!Function.ParseDecimal(documentsAdditionalExpensesImpl.LineTotal).Equals(0m))
					{
						stringBuilder.Append(Function.GetJsonString("LineTotal", Function.ParseDecimal(documentsAdditionalExpensesImpl.LineTotal)));
					}
					stringBuilder.Append(Function.GetJsonString("Remarks", Function.ToString(documentsAdditionalExpensesImpl.Comments)));
					string text4 = "";
					text4 = Function.ToString(documentsAdditionalExpensesImpl.DistrbMthd) switch
					{
						"Q" => "aedm_Quantity", 
						"V" => "aedm_Volume", 
						"W" => "aedm_Weight", 
						"E" => "aedm_Equally", 
						"T" => "aedm_RowTotal", 
						_ => "aedm_None", 
					};
					if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.DistrbMthd))
					{
						stringBuilder.Append(Function.GetJsonString("DistributionMethod", text4));
					}
					if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.VatGroup))
					{
						stringBuilder.Append(Function.GetJsonString("VatGroup", Function.ToString(documentsAdditionalExpensesImpl.VatGroup)));
					}
					if (!Function.ParseDecimal(documentsAdditionalExpensesImpl.VatPrcnt).Equals(0m))
					{
						stringBuilder.Append(Function.GetJsonString("TaxPercent", Function.ParseDecimal(documentsAdditionalExpensesImpl.VatPrcnt)));
					}
					if (!Function.ParseDecimal(documentsAdditionalExpensesImpl.VatSumFrgn).Equals(0m))
					{
						stringBuilder.Append(Function.GetJsonString("TaxSum", Function.ParseDecimal(documentsAdditionalExpensesImpl.VatSumFrgn)));
					}
					else if (!Function.ParseDecimal(documentsAdditionalExpensesImpl.VatSum).Equals(0m))
					{
						stringBuilder.Append(Function.GetJsonString("TaxSum", Function.ParseDecimal(documentsAdditionalExpensesImpl.VatSum)));
					}
					if (!Function.ParseDecimal(documentsAdditionalExpensesImpl.DedVatSum).Equals(0m))
					{
						stringBuilder.Append(Function.GetJsonString("DeductibleTaxSum", Function.ParseDecimal(documentsAdditionalExpensesImpl.DedVatSum)));
					}
					if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.TaxCode))
					{
						stringBuilder.Append(Function.GetJsonString("TaxCode", Function.ToString(documentsAdditionalExpensesImpl.TaxCode)));
					}
					if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.TaxType))
					{
						stringBuilder.Append(Function.GetJsonString("TaxType", Function.ToString(documentsAdditionalExpensesImpl.TaxType)));
					}
					if (!Function.ParseDecimal(documentsAdditionalExpensesImpl.EquVatPer).Equals(0m))
					{
						stringBuilder.Append(Function.GetJsonString("EqualizationTaxPercent", Function.ParseDecimal(documentsAdditionalExpensesImpl.EquVatPer)));
					}
					if (!Function.ParseDecimal(documentsAdditionalExpensesImpl.EquVatSum).Equals(0m))
					{
						stringBuilder.Append(Function.GetJsonString("EqualizationTaxSum", Function.ParseDecimal(documentsAdditionalExpensesImpl.EquVatSum)));
					}
					if (!Function.ParseInt(documentsAdditionalExpensesImpl.BaseType).Equals(0) && !Function.ParseInt(documentsAdditionalExpensesImpl.BaseType).Equals(-1))
					{
						stringBuilder.Append(Function.GetJsonString("BaseDocType", Function.ParseInt(documentsAdditionalExpensesImpl.BaseType)));
						if (!Function.ParseInt(documentsAdditionalExpensesImpl.BaseAbsEnt).Equals(0))
						{
							stringBuilder.Append(Function.GetJsonString("BaseDocEntry", Function.ParseInt(documentsAdditionalExpensesImpl.BaseAbsEnt)));
						}
						stringBuilder.Append(Function.GetJsonString("BaseDocLine", Function.ParseInt(documentsAdditionalExpensesImpl.BaseLnNum)));
					}
					if (documentsAdditionalExpensesImpl.LstPchPrce == "Y")
					{
						stringBuilder.Append(Function.GetJsonString("LastPurchasePrice", "tYES"));
					}
					else if (documentsAdditionalExpensesImpl.LstPchPrce == "N")
					{
						stringBuilder.Append(Function.GetJsonString("LastPurchasePrice", "tNO"));
					}
					if (documentsAdditionalExpensesImpl.Stock == "Y")
					{
						stringBuilder.Append(Function.GetJsonString("Stock", "tYES"));
					}
					else if (documentsAdditionalExpensesImpl.Stock == "N")
					{
						stringBuilder.Append(Function.GetJsonString("Stock", "tNO"));
					}
					if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.OcrCode))
					{
						stringBuilder.Append(Function.GetJsonString("DistributionRule", Function.ToString(documentsAdditionalExpensesImpl.OcrCode)));
					}
					if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.OcrCode2))
					{
						stringBuilder.Append(Function.GetJsonString("DistributionRule2", Function.ToString(documentsAdditionalExpensesImpl.OcrCode2)));
					}
					if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.OcrCode3))
					{
						stringBuilder.Append(Function.GetJsonString("DistributionRule3", Function.ToString(documentsAdditionalExpensesImpl.OcrCode3)));
					}
					if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.OcrCode4))
					{
						stringBuilder.Append(Function.GetJsonString("DistributionRule4", Function.ToString(documentsAdditionalExpensesImpl.OcrCode4)));
					}
					if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.OcrCode5))
					{
						stringBuilder.Append(Function.GetJsonString("DistributionRule5", Function.ToString(documentsAdditionalExpensesImpl.OcrCode5)));
					}
					if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.Project))
					{
						stringBuilder.Append(Function.GetJsonString("Project", Function.ToString(documentsAdditionalExpensesImpl.Project)));
					}
					stringBuilder.Append("},");
				}
			}
			stringBuilder.Append("],");
			stringBuilder.Append("\"DocumentReferences\":[");
			if (doc.DocumentRef != null && doc.DocumentRef.Count > 0)
			{
				for (int num7 = 0; num7 < doc.DocumentRef.Count; num7++)
				{
					stringBuilder.Append("{");
					stringBuilder.Append(Function.GetJsonString("RefDocEntr", doc.DocumentRef[num7].RefDocEntr));
					stringBuilder.Append(Function.GetJsonString("RefDocNum", doc.DocumentRef[num7].RefDocNum));
					stringBuilder.Append(Function.GetJsonString("RefObjType", (Enums.ReferencedObjectTypeEnum)doc.DocumentRef[num7].RefObjType));
					stringBuilder.Append("},");
				}
			}
			stringBuilder.Append("],");
			stringBuilder.Append("}");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return stringBuilder.ToString();
	}

	private string MappingHeader(DocumentsImpl doc, ApzObjectTypes objType, ApzObjectTypes docObjectCode, EnumCommandType commandType, bool isMapping)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (objType == ApzObjectTypes.oDrafts)
		{
			stringBuilder.Append(Function.GetJsonString("DocObjectCode", Function.ToString((int)docObjectCode)));
		}
		if ((objType == ApzObjectTypes.oDrafts || doc.Series < 0) && doc.DocNum > 0)
		{
			stringBuilder.Append(Function.GetJsonString("DocNum", doc.DocNum));
		}
		if (new ApzObjectTypes[2]
		{
			ApzObjectTypes.oDownPayments,
			ApzObjectTypes.oPurchaseDownPayments
		}.Contains(objType) || (objType == ApzObjectTypes.oDrafts && new ApzObjectTypes[2]
		{
			ApzObjectTypes.oDownPayments,
			ApzObjectTypes.oPurchaseDownPayments
		}.Contains(docObjectCode)))
		{
			stringBuilder.Append(Function.GetJsonString("DownPaymentType", ((Enums.DownPaymentType)Function.ParseInt(doc.Posted)/*cast due to constrained. prefix*/).ToString()));
		}
		if (!doc.SlpCode.Equals(0))
		{
			stringBuilder.Append(Function.GetJsonString("SalesPersonCode", Function.ParseInt(doc.SlpCode)));
		}
		if (doc.OwnerCode > 0)
		{
			stringBuilder.Append(Function.GetJsonString("DocumentsOwner", Function.ParseInt(doc.OwnerCode)));
		}
		stringBuilder.Append(Function.GetJsonString("Comments", Function.ToString(doc.Comments)));
		stringBuilder.Append(Function.GetJsonString("OpeningRemarks", Function.ToString(doc.Header)));
		stringBuilder.Append(Function.GetJsonString("ClosingRemarks", Function.ToString(doc.Footer)));
		if (objType != ApzObjectTypes.oInventoryGenExit && objType != ApzObjectTypes.oInventoryGenEntry)
		{
			stringBuilder.Append(Function.GetJsonString("ContactPersonCode", doc.CntctCode));
		}
		ApzObjectTypes[] source = new ApzObjectTypes[6]
		{
			ApzObjectTypes.oPurchaseRequest,
			ApzObjectTypes.oInventoryGenEntry,
			ApzObjectTypes.oInventoryGenExit,
			ApzObjectTypes.oStockTransfer,
			ApzObjectTypes.oStockTransferDraft,
			ApzObjectTypes.oInventoryTransferRequest
		};
		if (!source.Contains(objType) || (objType == ApzObjectTypes.oDrafts && !source.Contains(doc.DocObjectCode)))
		{
			stringBuilder.Append(Function.GetJsonString("NumAtCard", Function.ToString(doc.NumAtCard)));
		}
		if (doc.AtcEntry != 0 && doc.AttachmentFiles != null && doc.AttachmentFiles.Count > 0)
		{
			stringBuilder.Append(Function.GetJsonString("AttachmentEntry", Function.ParseInt(doc.AtcEntry)));
		}
		else
		{
			stringBuilder.Append(Function.GetJsonString("AttachmentEntry", null));
		}
		if (!string.IsNullOrEmpty(doc.Ref1))
		{
			stringBuilder.Append(Function.GetJsonString("Reference1", Function.ToString(doc.Ref1)));
		}
		if (!string.IsNullOrEmpty(doc.Ref2))
		{
			stringBuilder.Append(Function.GetJsonString("Reference2", Function.ToString(doc.Ref2)));
		}
		if (doc.PartSupply == "N")
		{
			stringBuilder.Append(Function.GetJsonString("PartialSupply", "tNO"));
		}
		else
		{
			stringBuilder.Append(Function.GetJsonString("PartialSupply", "tYES"));
		}
		if (doc.UseShpdGd == "Y")
		{
			stringBuilder.Append(Function.GetJsonString("UseShpdGoodsAct", "tYES"));
		}
		else
		{
			stringBuilder.Append(Function.GetJsonString("UseShpdGoodsAct", "tNO"));
		}
		if (!string.IsNullOrEmpty(doc.PriceMode))
		{
			stringBuilder.Append(Function.GetJsonString("PriceMode", (doc.PriceMode == "N") ? "pmdNet" : "pmdGross"));
		}
		if (!string.IsNullOrWhiteSpace(Function.ToString(doc.OpenForLaC)) && (objType == ApzObjectTypes.oPurchaseInvoices || objType == ApzObjectTypes.oPurchaseDeliveryNotes))
		{
			stringBuilder.Append(Function.GetJsonString("OpenForLandedCosts", (doc.OpenForLaC == "N") ? Function.ToString(BoYesNoEnum.tNO) : Function.ToString(BoYesNoEnum.tYES)));
		}
		string text = Function.ToString(doc.DocStatus).ToUpper();
		if (isMapping)
		{
			if (objType != ApzObjectTypes.oPurchaseRequest || (objType == ApzObjectTypes.oDrafts && docObjectCode != ApzObjectTypes.oPurchaseRequest))
			{
				stringBuilder.Append(Function.GetJsonString("Address", Function.ToString(doc.Address)));
			}
			if ((!string.IsNullOrEmpty(doc.isIns) && new ApzObjectTypes[4]
			{
				ApzObjectTypes.oInvoices,
				ApzObjectTypes.oPurchaseInvoices,
				ApzObjectTypes.oReserveInvoices,
				ApzObjectTypes.oAPReserveInvoice
			}.Contains(objType)) || (objType == ApzObjectTypes.oDrafts && new ApzObjectTypes[4]
			{
				ApzObjectTypes.oInvoices,
				ApzObjectTypes.oPurchaseInvoices,
				ApzObjectTypes.oReserveInvoices,
				ApzObjectTypes.oAPReserveInvoice
			}.Contains(docObjectCode)))
			{
				stringBuilder.Append(Function.GetJsonString("ReserveInvoice", Function.ToString(doc.isIns)));
			}
			if (objType != ApzObjectTypes.oPurchaseRequest || (objType == ApzObjectTypes.oDrafts && docObjectCode != ApzObjectTypes.oPurchaseRequest))
			{
				stringBuilder.Append(Function.GetJsonString("Address2", Function.ToString(doc.Address2)));
			}
			if (!string.IsNullOrEmpty(doc.AgentCode))
			{
				stringBuilder.Append(Function.GetJsonString("AgentCode", Function.ToString(doc.AgentCode)));
			}
			if (doc.AnnualInvoiceDeclarationReference != 0)
			{
				stringBuilder.Append(Function.GetJsonString("AnnualInvoiceDeclarationReference", Function.ParseInt(doc.AnnualInvoiceDeclarationReference)));
			}
			if (!string.IsNullOrEmpty(doc.BPChCode))
			{
				stringBuilder.Append(Function.GetJsonString("BPChannelCode", Function.ToString(doc.BPChCode)));
			}
			if (doc.BPChCntc != 0)
			{
				stringBuilder.Append(Function.GetJsonString("BPChannelContact", Function.ParseInt(doc.BPChCntc)));
			}
			if (doc.BPLId != 0 && doc.BPLId != -1)
			{
				stringBuilder.Append(Function.GetJsonString("BPL_IDAssignedToInvoice", Function.ParseInt(doc.BPLId)));
			}
			if (doc.BlanketAgreementNumber != 0)
			{
				stringBuilder.Append(Function.GetJsonString("BlanketAgreementNumber", Function.ParseInt(doc.BlanketAgreementNumber)));
			}
			if (!string.IsNullOrEmpty(doc.Box1099))
			{
				stringBuilder.Append(Function.GetJsonString("Box1099", Function.ToString(doc.Box1099)));
			}
			if (doc.Series > 0)
			{
				stringBuilder.Append(Function.GetJsonString("Series", doc.Series));
			}
			if (doc.Series < 0)
			{
				stringBuilder.Append(Function.GetJsonString("HandWritten", "Y"));
			}
			if (!string.IsNullOrEmpty(doc.CardCode))
			{
				stringBuilder.Append(Function.GetJsonString("CardCode", Function.ToString(doc.CardCode)));
			}
			if (!string.IsNullOrEmpty(doc.CardName))
			{
				stringBuilder.Append(Function.GetJsonString("CardName", Function.ToString(doc.CardName)));
			}
			if (doc.CdcOffset != 0)
			{
				stringBuilder.Append(Function.GetJsonString("CashDiscountDateOffset", Function.ParseInt(doc.CdcOffset)));
			}
			if (!string.IsNullOrEmpty(doc.CntrlBnk))
			{
				stringBuilder.Append(Function.GetJsonString("CentralBankIndicator", Function.ToString(doc.CntrlBnk)));
			}
			if (!string.IsNullOrEmpty(doc.Footer))
			{
				stringBuilder.Append(Function.GetJsonString("ClosingRemarks", Function.ToString(doc.Footer)));
			}
			if (!string.IsNullOrEmpty(doc.CtlAccount))
			{
				stringBuilder.Append(Function.GetJsonString("ControlAccount", Function.ToString(doc.CtlAccount)));
			}
			if (!doc.DiscPrcnt.Equals(0.0) || commandType == EnumCommandType.Update)
			{
				stringBuilder.Append(Function.GetJsonString("DiscountPercent", Function.ParseDecimal(doc.DiscPrcnt)));
			}
			if (!string.IsNullOrEmpty(doc.DocCur))
			{
				stringBuilder.Append(Function.GetJsonString("DocCurrency", Function.ToString(doc.DocCur)));
			}
			if (doc.DocDate > new DateTime(1900, 1, 1))
			{
				stringBuilder.Append(Function.GetJsonString("DocDate", Convert.ToDateTime(doc.DocDate).ToString("yyyyMMdd")));
			}
			if (doc.DocDueDate > new DateTime(1900, 1, 1))
			{
				stringBuilder.Append(Function.GetJsonString("DocDueDate", Convert.ToDateTime(doc.DocDueDate).ToString("yyyyMMdd")));
			}
			if (!doc.DocRate.Equals(0.0))
			{
				stringBuilder.Append(Function.GetJsonString("DocRate", Function.ParseDecimal(doc.DocRate)));
			}
			if (!string.IsNullOrEmpty(doc.DocType))
			{
				if (doc.DocType == "S")
				{
					stringBuilder.Append(Function.GetJsonString("DocType", "dDocument_Service"));
				}
				else
				{
					stringBuilder.Append(Function.GetJsonString("DocType", "dDocument_Items"));
				}
			}
			if (new ApzObjectTypes[6]
			{
				ApzObjectTypes.oDownPayments,
				ApzObjectTypes.oPurchaseDownPayments,
				ApzObjectTypes.oInvoices,
				ApzObjectTypes.oPurchaseInvoices,
				ApzObjectTypes.oReserveInvoices,
				ApzObjectTypes.oAPReserveInvoice
			}.Contains(objType) || (objType == ApzObjectTypes.oDrafts && new ApzObjectTypes[6]
			{
				ApzObjectTypes.oDownPayments,
				ApzObjectTypes.oPurchaseDownPayments,
				ApzObjectTypes.oInvoices,
				ApzObjectTypes.oPurchaseInvoices,
				ApzObjectTypes.oReserveInvoices,
				ApzObjectTypes.oAPReserveInvoice
			}.Contains(docObjectCode)))
			{
				if (!doc.DownPayment.Equals(0.0))
				{
					stringBuilder.Append(Function.GetJsonString("DownPayment", Function.ParseDecimal(doc.DownPayment)));
				}
				if (doc.DocCur == DocumentsRepository.MainCurrency[_connectionString])
				{
					stringBuilder.Append(Function.GetJsonString("DownPaymentAmount", doc.DPMTotal));
				}
				else
				{
					stringBuilder.Append(Function.GetJsonString("DownPaymentAmountFC", doc.DPMTotal));
				}
				if (!doc.DpmPrcnt.Equals(0.0))
				{
					stringBuilder.Append(Function.GetJsonString("DownPaymentPercentage", Function.ParseDecimal(doc.DpmPrcnt)));
				}
			}
			if (doc.TaxDate > new DateTime(1900, 1, 1))
			{
				stringBuilder.Append(Function.GetJsonString("TaxDate", Convert.ToDateTime(doc.TaxDate).ToString("yyyyMMdd")));
			}
			if (!doc.TrnspCode.Equals(0))
			{
				stringBuilder.Append(Function.GetJsonString("TransportationCode", Function.ParseInt(doc.TrnspCode)));
			}
			if (doc.VatDate > new DateTime(1900, 1, 1))
			{
				stringBuilder.Append(Function.GetJsonString("VatDate", Convert.ToDateTime(doc.VatDate).ToString("yyyyMMdd")));
			}
			if (!doc.VatPercent.Equals(0.0))
			{
				stringBuilder.Append(Function.GetJsonString("VatPercent", Function.ParseDecimal(doc.VatPercent)));
			}
			if (objType == ApzObjectTypes.oPurchaseRequest || docObjectCode == ApzObjectTypes.oPurchaseRequest)
			{
				if (!string.IsNullOrEmpty(doc.Requester))
				{
					stringBuilder.Append(Function.GetJsonString("Requester", Function.ToString(doc.Requester)));
				}
				if (!string.IsNullOrEmpty(doc.Email))
				{
					stringBuilder.Append(Function.GetJsonString("RequesterEmail", Function.ToString(doc.Email)));
				}
				if (!string.IsNullOrEmpty(doc.ReqName))
				{
					stringBuilder.Append(Function.GetJsonString("RequesterName", Function.ToString(doc.ReqName)));
				}
				if (!Function.ParseInt(doc.Branch).Equals(0))
				{
					stringBuilder.Append(Function.GetJsonString("RequesterBranch", Function.ParseInt(doc.Branch)));
				}
				if (!Function.ParseInt(doc.Department).Equals(0))
				{
					stringBuilder.Append(Function.GetJsonString("RequesterDepartment", Function.ParseInt(doc.Department)));
				}
				if (!Function.ParseInt(doc.ReqType).Equals(0))
				{
					stringBuilder.Append(Function.GetJsonString("ReqType", Function.ParseInt(doc.ReqType)));
				}
			}
			if (doc.ReqDate > new DateTime(1900, 1, 1))
			{
				stringBuilder.Append(Function.GetJsonString("RequriedDate", Convert.ToDateTime(doc.ReqDate).ToString("yyyyMMdd")));
			}
			if (((objType != ApzObjectTypes.oPurchaseOrders && objType != ApzObjectTypes.oPurchaseRequest && objType != ApzObjectTypes.oPurchaseQuotations && (objType != ApzObjectTypes.oPurchaseDownPayments || !(doc.Posted == "0")) && objType != ApzObjectTypes.oDrafts) || (docObjectCode != ApzObjectTypes.oPurchaseOrders && docObjectCode != ApzObjectTypes.oPurchaseRequest && docObjectCode != ApzObjectTypes.oPurchaseQuotations && (docObjectCode != ApzObjectTypes.oPurchaseDownPayments || !(doc.Posted == "0")) && objType == ApzObjectTypes.oDrafts)) && !string.IsNullOrEmpty(doc.ShipToCode))
			{
				stringBuilder.Append(Function.GetJsonString("ShipToCode", Function.ToString(doc.ShipToCode)));
			}
			if (!string.IsNullOrEmpty(doc.PayToCode))
			{
				stringBuilder.Append(Function.GetJsonString("PayToCode", Function.ToString(doc.PayToCode)));
			}
			stringBuilder.Append(Function.GetJsonString("PickRemark", Function.ToString(doc.PickRmrk)));
		}
		if (!string.IsNullOrEmpty(doc.JrnlMemo))
		{
			stringBuilder.Append(Function.GetJsonString("JournalMemo", Function.ToString(doc.JrnlMemo)));
		}
		if (objType != ApzObjectTypes.oInventoryGenEntry && objType != ApzObjectTypes.oInventoryGenExit)
		{
			if (!string.IsNullOrEmpty(doc.Project) || commandType == EnumCommandType.Add)
			{
				stringBuilder.Append(Function.GetJsonString("Project", Function.ToString(doc.Project)));
			}
			if (!string.IsNullOrEmpty(doc.Indicator))
			{
				stringBuilder.Append(Function.GetJsonString("Indicator", Function.ToString(doc.Indicator)));
			}
			if (!string.IsNullOrEmpty(doc.LicTradNum))
			{
				stringBuilder.Append(Function.GetJsonString("FederalTaxID", Function.ToString(doc.LicTradNum)));
			}
			if (text.Equals("O") || text.Equals("OPEN") || commandType == EnumCommandType.Add)
			{
				if (Function.ParseInt(doc.GroupNum) > 0)
				{
					stringBuilder.Append(Function.GetJsonString("PaymentGroupCode", Function.ToString(doc.GroupNum)));
				}
				if (!string.IsNullOrEmpty(doc.PeyMethod))
				{
					stringBuilder.Append(Function.GetJsonString("PaymentMethod", Function.ToString(doc.PeyMethod)));
				}
				if (commandType == EnumCommandType.Add || objType == ApzObjectTypes.oDrafts)
				{
					if (Function.ParseInt(doc.ExtraMonth) > 0)
					{
						stringBuilder.Append(Function.GetJsonString("ExtraMonth", Function.ToString(doc.ExtraMonth)));
						BoPayTermDueTypes boPayTermDueTypes;
						try
						{
							boPayTermDueTypes = doc.StartFrom;
						}
						catch
						{
							boPayTermDueTypes = BoPayTermDueTypes.pdt_None;
						}
						stringBuilder.Append(Function.GetJsonString("StartFrom", Function.ToString(boPayTermDueTypes)));
					}
					if (Function.ParseInt(doc.ExtraDays) > 0)
					{
						stringBuilder.Append(Function.GetJsonString("ExtraDays", Function.ToString(doc.ExtraDays)));
					}
				}
			}
		}
		stringBuilder.Append("\"DownPaymentsToDraw\": [");
		if ((new ApzObjectTypes[4]
		{
			ApzObjectTypes.oInvoices,
			ApzObjectTypes.oReserveInvoices,
			ApzObjectTypes.oAPReserveInvoice,
			ApzObjectTypes.oPurchaseInvoices
		}.Contains(objType) || (objType == ApzObjectTypes.oDrafts && new ApzObjectTypes[4]
		{
			ApzObjectTypes.oInvoices,
			ApzObjectTypes.oReserveInvoices,
			ApzObjectTypes.oAPReserveInvoice,
			ApzObjectTypes.oPurchaseInvoices
		}.Contains(docObjectCode))) && (commandType == EnumCommandType.Add || objType == ApzObjectTypes.oDrafts) && doc.DownPaymentsToDraw != null && doc.DownPaymentsToDraw.Any())
		{
			foreach (DownPaymentsToDrawImpl item in doc.DownPaymentsToDraw)
			{
				stringBuilder.Append("{");
				stringBuilder.Append(Function.GetJsonString("DocEntry", item.DocEntry));
				stringBuilder.Append("\"DownPaymentsToDrawDetails\": [");
				foreach (DownPaymentToDrawDetailImpl downPaymentsToDrawDetail in item.DownPaymentsToDrawDetails)
				{
					stringBuilder.Append("{");
					stringBuilder.Append(Function.GetJsonString("VatGroupCode", downPaymentsToDrawDetail.VatGroupCode));
					if (downPaymentsToDrawDetail.AmountToDraw != 0.0)
					{
						stringBuilder.Append(Function.GetJsonString("AmountToDraw", downPaymentsToDrawDetail.AmountToDraw));
					}
					if (downPaymentsToDrawDetail.AmountToDrawFC != 0.0)
					{
						stringBuilder.Append(Function.GetJsonString("AmountToDrawFC", downPaymentsToDrawDetail.AmountToDrawFC));
					}
					if (downPaymentsToDrawDetail.GrossAmountToDraw != 0.0)
					{
						stringBuilder.Append(Function.GetJsonString("GrossAmountToDraw", downPaymentsToDrawDetail.GrossAmountToDraw));
					}
					if (downPaymentsToDrawDetail.GrossAmountToDrawFC != 0.0)
					{
						stringBuilder.Append(Function.GetJsonString("GrossAmountToDrawFC", downPaymentsToDrawDetail.GrossAmountToDrawFC));
					}
					stringBuilder.Append("},");
				}
				stringBuilder.Append("],");
				stringBuilder.Append("},");
			}
		}
		stringBuilder.Append("],");
		stringBuilder.Append("\"DocumentSpecialLines\":[");
		if (doc.SpecialLines != null && doc.SpecialLines.Count > 0)
		{
			for (int i = 0; i < doc.SpecialLines.Count; i++)
			{
				stringBuilder.Append("{");
				if (doc.SpecialLines[i].LineType == BoDocSpecialLineType.dslt_Subtotal)
				{
					stringBuilder.Append(Function.GetJsonString("LineType", "dslt_Subtotal"));
				}
				else
				{
					stringBuilder.Append(Function.GetJsonString("LineType", "dslt_Text"));
				}
				stringBuilder.Append(Function.GetJsonString("AfterLineNumber", doc.SpecialLines[i].AfterLineNumber));
				stringBuilder.Append(Function.GetJsonString("LineNum", doc.SpecialLines[i].LineNum));
				if (!string.IsNullOrEmpty(doc.SpecialLines[i].LineText))
				{
					stringBuilder.Append(Function.GetJsonString("LineText", Function.ToString(doc.SpecialLines[i].LineText)));
				}
				stringBuilder.Append("},");
			}
		}
		stringBuilder.Append("],");
		stringBuilder.Append("\"DocumentAdditionalExpenses\":[");
		if (doc.Expenses != null && doc.Expenses.Count > 0)
		{
			for (int j = 0; j < doc.Expenses.Count; j++)
			{
				DocumentsAdditionalExpensesImpl documentsAdditionalExpensesImpl = doc.Expenses[j];
				if (documentsAdditionalExpensesImpl == null || string.IsNullOrEmpty(documentsAdditionalExpensesImpl.Status) || !documentsAdditionalExpensesImpl.Status.ToUpper().Equals("O") || !(documentsAdditionalExpensesImpl.LineTotal > 0.0))
				{
					continue;
				}
				stringBuilder.Append("{");
				if (!Function.ParseInt(documentsAdditionalExpensesImpl.ExpnsCode).Equals(0))
				{
					stringBuilder.Append(Function.GetJsonString("ExpenseCode", Function.ParseInt(documentsAdditionalExpensesImpl.ExpnsCode)));
				}
				if (!Function.ParseDecimal(documentsAdditionalExpensesImpl.TotalFrgn).Equals(0m))
				{
					stringBuilder.Append(Function.GetJsonString("LineTotal", Function.ParseDecimal(documentsAdditionalExpensesImpl.TotalFrgn)));
				}
				else if (!Function.ParseDecimal(documentsAdditionalExpensesImpl.LineTotal).Equals(0m))
				{
					stringBuilder.Append(Function.GetJsonString("LineTotal", Function.ParseDecimal(documentsAdditionalExpensesImpl.LineTotal)));
				}
				stringBuilder.Append(Function.GetJsonString("Remarks", Function.ToString(documentsAdditionalExpensesImpl.Comments)));
				string text2 = "";
				text2 = Function.ToString(documentsAdditionalExpensesImpl.DistrbMthd) switch
				{
					"Q" => "aedm_Quantity", 
					"V" => "aedm_Volume", 
					"W" => "aedm_Weight", 
					"E" => "aedm_Equally", 
					"T" => "aedm_RowTotal", 
					_ => "aedm_None", 
				};
				if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.DistrbMthd))
				{
					stringBuilder.Append(Function.GetJsonString("DistributionMethod", text2));
				}
				if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.VatGroup))
				{
					stringBuilder.Append(Function.GetJsonString("VatGroup", Function.ToString(documentsAdditionalExpensesImpl.VatGroup)));
				}
				if (!Function.ParseDecimal(documentsAdditionalExpensesImpl.VatPrcnt).Equals(0m))
				{
					stringBuilder.Append(Function.GetJsonString("TaxPercent", Function.ParseDecimal(documentsAdditionalExpensesImpl.VatPrcnt)));
				}
				if (!Function.ParseDecimal(documentsAdditionalExpensesImpl.VatSumFrgn).Equals(0m))
				{
					stringBuilder.Append(Function.GetJsonString("TaxSum", Function.ParseDecimal(documentsAdditionalExpensesImpl.VatSumFrgn)));
				}
				else if (!Function.ParseDecimal(documentsAdditionalExpensesImpl.VatSum).Equals(0m))
				{
					stringBuilder.Append(Function.GetJsonString("TaxSum", Function.ParseDecimal(documentsAdditionalExpensesImpl.VatSum)));
				}
				if (!Function.ParseDecimal(documentsAdditionalExpensesImpl.DedVatSum).Equals(0m))
				{
					stringBuilder.Append(Function.GetJsonString("DeductibleTaxSum", Function.ParseDecimal(documentsAdditionalExpensesImpl.DedVatSum)));
				}
				if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.TaxCode))
				{
					stringBuilder.Append(Function.GetJsonString("TaxCode", Function.ToString(documentsAdditionalExpensesImpl.TaxCode)));
				}
				if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.TaxType))
				{
					stringBuilder.Append(Function.GetJsonString("TaxType", Function.ToString(documentsAdditionalExpensesImpl.TaxType)));
				}
				if (!Function.ParseDecimal(documentsAdditionalExpensesImpl.EquVatPer).Equals(0m))
				{
					stringBuilder.Append(Function.GetJsonString("EqualizationTaxPercent", Function.ParseDecimal(documentsAdditionalExpensesImpl.EquVatPer)));
				}
				if (!Function.ParseDecimal(documentsAdditionalExpensesImpl.EquVatSum).Equals(0m))
				{
					stringBuilder.Append(Function.GetJsonString("EqualizationTaxSum", Function.ParseDecimal(documentsAdditionalExpensesImpl.EquVatSum)));
				}
				if (!Function.ParseInt(documentsAdditionalExpensesImpl.BaseType).Equals(0) && !Function.ParseInt(documentsAdditionalExpensesImpl.BaseType).Equals(-1))
				{
					stringBuilder.Append(Function.GetJsonString("BaseDocType", Function.ParseInt(documentsAdditionalExpensesImpl.BaseType)));
					if (!Function.ParseInt(documentsAdditionalExpensesImpl.BaseAbsEnt).Equals(0))
					{
						stringBuilder.Append(Function.GetJsonString("BaseDocEntry", Function.ParseInt(documentsAdditionalExpensesImpl.BaseAbsEnt)));
					}
					stringBuilder.Append(Function.GetJsonString("BaseDocLine", Function.ParseInt(documentsAdditionalExpensesImpl.BaseLnNum)));
				}
				if (documentsAdditionalExpensesImpl.LstPchPrce == "Y")
				{
					stringBuilder.Append(Function.GetJsonString("LastPurchasePrice", "tYES"));
				}
				else if (documentsAdditionalExpensesImpl.LstPchPrce == "N")
				{
					stringBuilder.Append(Function.GetJsonString("LastPurchasePrice", "tNO"));
				}
				if (documentsAdditionalExpensesImpl.Stock == "Y")
				{
					stringBuilder.Append(Function.GetJsonString("Stock", "tYES"));
				}
				else if (documentsAdditionalExpensesImpl.Stock == "N")
				{
					stringBuilder.Append(Function.GetJsonString("Stock", "tNO"));
				}
				if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.OcrCode))
				{
					stringBuilder.Append(Function.GetJsonString("DistributionRule", Function.ToString(documentsAdditionalExpensesImpl.OcrCode)));
				}
				if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.OcrCode2))
				{
					stringBuilder.Append(Function.GetJsonString("DistributionRule2", Function.ToString(documentsAdditionalExpensesImpl.OcrCode2)));
				}
				if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.OcrCode3))
				{
					stringBuilder.Append(Function.GetJsonString("DistributionRule3", Function.ToString(documentsAdditionalExpensesImpl.OcrCode3)));
				}
				if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.OcrCode4))
				{
					stringBuilder.Append(Function.GetJsonString("DistributionRule4", Function.ToString(documentsAdditionalExpensesImpl.OcrCode4)));
				}
				if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.OcrCode5))
				{
					stringBuilder.Append(Function.GetJsonString("DistributionRule5", Function.ToString(documentsAdditionalExpensesImpl.OcrCode5)));
				}
				if (!string.IsNullOrEmpty(documentsAdditionalExpensesImpl.Project))
				{
					stringBuilder.Append(Function.GetJsonString("Project", Function.ToString(documentsAdditionalExpensesImpl.Project)));
				}
				stringBuilder.Append("},");
			}
		}
		stringBuilder.Append("],");
		stringBuilder.Append("\"DocumentReferences\":[");
		if (doc.DocumentRef != null && doc.DocumentRef.Count > 0)
		{
			for (int k = 0; k < doc.DocumentRef.Count; k++)
			{
				stringBuilder.Append("{");
				stringBuilder.Append(Function.GetJsonString("RefDocEntr", doc.DocumentRef[k].RefDocEntr));
				stringBuilder.Append(Function.GetJsonString("RefDocNum", doc.DocumentRef[k].RefDocNum));
				stringBuilder.Append(Function.GetJsonString("RefObjType", (Enums.ReferencedObjectTypeEnum)doc.DocumentRef[k].RefObjType));
				stringBuilder.Append("},");
			}
		}
		stringBuilder.Append("],");
		return stringBuilder.ToString();
	}

	private string MappingLine(DocumentsImpl doc, ApzObjectTypes objType, ApzObjectTypes docObjectCode, bool isMapping, EnumCommandType commandType, bool ignoreUpdateEmptyValue)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (doc.Lines != null && doc.Lines.Count > 0)
		{
			List<string> lstFieldNotValidated = null;
			lock (DocumentsRepository.ListUdfLineNotValidated)
			{
				if (!DocumentsRepository.ListUdfLineNotValidated.ContainsKey(_connectionString) || DocumentsRepository.ListUdfLineNotValidated[_connectionString] == null)
				{
					DocumentsRepository.ListUdfLineNotValidated.Add(_connectionString, ServiceUnitOfWork.UDFs.GetListFieldNotValidated("ADO1"));
				}
				lstFieldNotValidated = DocumentsRepository.ListUdfLineNotValidated[_connectionString];
			}
			stringBuilder.Append("\"DocumentLines\":[");
			for (int i = 0; i < doc.Lines.Count; i++)
			{
				Document_LinesImpl document_LinesImpl = doc.Lines[i];
				string text = Function.ToString(document_LinesImpl.LineStatus).ToUpper();
				stringBuilder.Append("{\r\n");
				if (document_LinesImpl.LineNum >= 0)
				{
					stringBuilder.Append(Function.GetJsonString("LineNum", Function.ParseInt(document_LinesImpl.LineNum)));
				}
				string text2 = Function.ToString(doc.DocStatus).ToUpper();
				if (isMapping)
				{
					if (!string.IsNullOrEmpty(document_LinesImpl.AcctCode))
					{
						stringBuilder.Append(Function.GetJsonString("AccountCode", Function.ToString(document_LinesImpl.AcctCode)));
					}
					if (!document_LinesImpl.ActBaseEnt.Equals(0))
					{
						stringBuilder.Append(Function.GetJsonString("ActualBaseEntry", Function.ParseInt(document_LinesImpl.ActBaseEnt)));
					}
					if (document_LinesImpl.ActDelDate > new DateTime(1900, 1, 1))
					{
						stringBuilder.Append(Function.GetJsonString("ActualDeliveryDate", Convert.ToDateTime(document_LinesImpl.ActDelDate).ToString("yyyyMMdd")));
					}
					if (!string.IsNullOrEmpty(document_LinesImpl.Address))
					{
						stringBuilder.Append(Function.GetJsonString("Address", Function.ToString(document_LinesImpl.Address)));
					}
					if (!document_LinesImpl.AgrNo.Equals(0))
					{
						stringBuilder.Append(Function.GetJsonString("AgreementNo", Function.ParseInt(document_LinesImpl.AgrNo)));
					}
					if (!document_LinesImpl.AgrLnNum.Equals(0))
					{
						stringBuilder.Append(Function.GetJsonString("AgreementRowNumber", Function.ParseInt(document_LinesImpl.AgrLnNum)));
					}
					if (!string.IsNullOrEmpty(document_LinesImpl.CodeBars))
					{
						stringBuilder.Append(Function.GetJsonString("BarCode", Function.ToString(document_LinesImpl.CodeBars)));
					}
					if (!document_LinesImpl.BaseType.Equals(0) && !document_LinesImpl.BaseType.Equals(-1))
					{
						stringBuilder.Append(Function.GetJsonString("BaseType", Function.ParseInt(document_LinesImpl.BaseType)));
						if (!document_LinesImpl.BaseEntry.Equals(0))
						{
							stringBuilder.Append(Function.GetJsonString("BaseEntry", Function.ParseInt(document_LinesImpl.BaseEntry)));
						}
						if (!document_LinesImpl.BaseLine.Equals(-1))
						{
							stringBuilder.Append(Function.GetJsonString("BaseLine", Function.ParseInt(document_LinesImpl.BaseLine)));
						}
					}
					if (!document_LinesImpl.Commission.Equals(0.0) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
					{
						stringBuilder.Append(Function.GetJsonString("CommisionPercent", Function.ParseDecimal(document_LinesImpl.Commission)));
					}
					if (!string.IsNullOrWhiteSpace(document_LinesImpl.OcrCode))
					{
						stringBuilder.Append(Function.GetJsonString("CostingCode", document_LinesImpl.OcrCode));
					}
					else
					{
						stringBuilder.Append("\"CostingCode\": null,");
					}
					if (!string.IsNullOrWhiteSpace(document_LinesImpl.OcrCode2))
					{
						stringBuilder.Append(Function.GetJsonString("CostingCode2", document_LinesImpl.OcrCode2));
					}
					else
					{
						stringBuilder.Append("\"CostingCode2\": null,");
					}
					if (!string.IsNullOrWhiteSpace(document_LinesImpl.OcrCode3))
					{
						stringBuilder.Append(Function.GetJsonString("CostingCode3", document_LinesImpl.OcrCode3));
					}
					else
					{
						stringBuilder.Append("\"CostingCode3\": null,");
					}
					if (!string.IsNullOrWhiteSpace(document_LinesImpl.OcrCode4))
					{
						stringBuilder.Append(Function.GetJsonString("CostingCode4", document_LinesImpl.OcrCode4));
					}
					else
					{
						stringBuilder.Append("\"CostingCode4\": null,");
					}
					if (!string.IsNullOrWhiteSpace(document_LinesImpl.OcrCode5))
					{
						stringBuilder.Append(Function.GetJsonString("CostingCode5", document_LinesImpl.OcrCode5));
					}
					else
					{
						stringBuilder.Append("\"CostingCode5\": null,");
					}
					if (!string.IsNullOrEmpty(document_LinesImpl.Currency))
					{
						stringBuilder.Append(Function.GetJsonString("Currency", Function.ToString(document_LinesImpl.Currency)));
					}
					if (!document_LinesImpl.ExciseAmt.Equals(0.0))
					{
						stringBuilder.Append(Function.GetJsonString("ExciseAmount", Function.ParseDecimal(document_LinesImpl.ExciseAmt)));
					}
					if (document_LinesImpl.BaseType != 202 && (!string.IsNullOrEmpty(document_LinesImpl.ItemCode) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update)))
					{
						stringBuilder.Append(Function.GetJsonString("ItemCode", Function.ToString(document_LinesImpl.ItemCode)));
					}
					if (!string.IsNullOrEmpty(document_LinesImpl.Dscription) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
					{
						stringBuilder.Append(Function.GetJsonString("ItemDescription", Function.ToString(document_LinesImpl.Dscription)));
					}
					if (!document_LinesImpl.InvQty.Equals(0.0))
					{
						stringBuilder.Append(Function.GetJsonString("InventoryQuantity", Function.ParseDecimal(document_LinesImpl.InvQty)));
					}
					if (!string.IsNullOrEmpty(document_LinesImpl.Text) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
					{
						stringBuilder.Append(Function.GetJsonString("ItemDetails", Function.ToString(document_LinesImpl.Text)));
					}
					if (!string.IsNullOrEmpty(document_LinesImpl.VendorNum) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
					{
						stringBuilder.Append(Function.GetJsonString("VendorNum", Function.ToString(document_LinesImpl.VendorNum)));
					}
					if (!document_LinesImpl.LocCode.Equals(0))
					{
						stringBuilder.Append(Function.GetJsonString("LocationCode", Function.ParseInt(document_LinesImpl.LocCode)));
					}
					if (!string.IsNullOrEmpty(document_LinesImpl.unitMsr) && document_LinesImpl.UomEntry <= 0)
					{
						stringBuilder.Append(Function.GetJsonString("MeasureUnit", Function.ToString(document_LinesImpl.unitMsr)));
					}
					if (!document_LinesImpl.SpecifiedPriceBefDi || (document_LinesImpl.BaseType == 202 && objType == ApzObjectTypes.oInventoryGenEntry))
					{
						if (document_LinesImpl.BaseType != 202 || (document_LinesImpl.BaseLine != -1 && document_LinesImpl.IsByPrdct == "Y"))
						{
							if ((Math.Abs(document_LinesImpl.PriceBefDi) > 1E-07 || commandType == EnumCommandType.Update) && !document_LinesImpl.SpecifiedPriceAfterVat)
							{
								if (doc.PriceMode != "G")
								{
									stringBuilder.Append(Function.GetJsonString("UnitPrice", document_LinesImpl.PriceBefDi));
								}
								else
								{
									stringBuilder.Append(Function.GetJsonString("GrossPrice", document_LinesImpl.PriceAfVAT));
								}
							}
							else if (Math.Abs(document_LinesImpl.PriceAfVAT) > 1E-07)
							{
								stringBuilder.Append(Function.GetJsonString("PriceAfterVAT", document_LinesImpl.PriceAfVAT));
							}
						}
						if (commandType == EnumCommandType.Add || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update) || (ignoreUpdateEmptyValue && Math.Abs(document_LinesImpl.DiscPrcnt) > 1E-07))
						{
							stringBuilder.Append(Function.GetJsonString("DiscountPercent", document_LinesImpl.DiscPrcnt));
						}
					}
					else
					{
						string value = "";
						double num = 0.0;
						if (document_LinesImpl.BaseType != 202)
						{
							if ((!string.IsNullOrEmpty(doc.DocCur) && doc.DocCur == DocumentsRepository.MainCurrency[_connectionString]) || string.IsNullOrEmpty(doc.DocCur))
							{
								value = ((!(doc.PriceMode != "G")) ? Function.GetJsonString("GrossTotal", document_LinesImpl.GTotal) : Function.GetJsonString("LineTotal", document_LinesImpl.LineTotal));
								num = document_LinesImpl.LineTotal;
							}
							else
							{
								value = ((!(doc.PriceMode != "G")) ? Function.GetJsonString("GrossTotalFC", document_LinesImpl.GTotalFC) : Function.GetJsonString("RowTotalFC", document_LinesImpl.TotalFrgn));
								num = Function.ParseDouble(document_LinesImpl.TotalFrgn);
							}
						}
						if (Math.Abs(num) < 1E-07 && Math.Abs(document_LinesImpl.PriceBefDi) < 1E-07 && Math.Abs(document_LinesImpl.PriceAfVAT) < 1E-07)
						{
							if (document_LinesImpl.BaseType != 202)
							{
								stringBuilder.Append(Function.GetJsonString("UnitPrice", 0));
								stringBuilder.Append(value);
							}
						}
						else
						{
							if (document_LinesImpl.BaseType != 202)
							{
								stringBuilder.Append(value);
							}
							if (document_LinesImpl.DiscPrcnt > 0.0)
							{
								if (document_LinesImpl.BaseType != 202)
								{
									if ((Math.Abs(document_LinesImpl.PriceBefDi) > 1E-07 || commandType == EnumCommandType.Update) && !document_LinesImpl.SpecifiedPriceAfterVat)
									{
										if (doc.PriceMode != "G")
										{
											stringBuilder.Append(Function.GetJsonString("UnitPrice", document_LinesImpl.PriceBefDi));
										}
										else
										{
											stringBuilder.Append(Function.GetJsonString("GrossPrice", document_LinesImpl.PriceAfVAT));
										}
									}
									else if (Math.Abs(document_LinesImpl.PriceAfVAT) > 1E-07)
									{
										stringBuilder.Append(Function.GetJsonString("PriceAfterVAT", document_LinesImpl.PriceAfVAT));
									}
									if (commandType == EnumCommandType.Update)
									{
										stringBuilder.Append(Function.GetJsonString("DiscountPercent", document_LinesImpl.DiscPrcnt));
									}
								}
							}
							else
							{
								stringBuilder.Append(Function.GetJsonString("DiscountPercent", 0));
								if (document_LinesImpl.BaseType != 202)
								{
									if (commandType != EnumCommandType.Update)
									{
										if (doc.SaveFromForm || Math.Abs(document_LinesImpl.Quantity * document_LinesImpl.PriceBefDi * (1.0 - document_LinesImpl.DiscPrcnt / 100.0) - num) < 1E-07)
										{
											if (doc.PriceMode != "G")
											{
												stringBuilder.Append(Function.GetJsonString("UnitPrice", document_LinesImpl.PriceBefDi));
											}
											else
											{
												stringBuilder.Append(Function.GetJsonString("GrossPrice", document_LinesImpl.PriceAfVAT));
											}
										}
										else if ((Math.Abs(document_LinesImpl.PriceBefDi) < 1E-07 && Math.Abs(document_LinesImpl.PriceAfVAT) > 1E-07) || document_LinesImpl.SpecifiedPriceAfterVat)
										{
											stringBuilder.Append(Function.GetJsonString("PriceAfterVAT", document_LinesImpl.PriceAfVAT));
										}
										else
										{
											stringBuilder.Append(Function.GetJsonString("UnitPrice", 0));
										}
									}
									else if ((Math.Abs(document_LinesImpl.PriceAfVAT) > 1E-07 && Math.Abs(document_LinesImpl.PriceBefDi) < 1E-07) || document_LinesImpl.SpecifiedPriceAfterVat)
									{
										stringBuilder.Append(Function.GetJsonString("PriceAfterVAT", document_LinesImpl.PriceAfVAT));
									}
									else if (doc.PriceMode != "G")
									{
										stringBuilder.Append(Function.GetJsonString("UnitPrice", document_LinesImpl.PriceBefDi));
									}
									else
									{
										stringBuilder.Append(Function.GetJsonString("GrossPrice", document_LinesImpl.PriceAfVAT));
									}
								}
							}
						}
					}
					if (!new int[8] { 15, 13, 14, 16, 18, 19, 20, 21 }.Contains(document_LinesImpl.BaseType))
					{
						if (!string.IsNullOrEmpty(document_LinesImpl.CogsAcct))
						{
							stringBuilder.Append(Function.GetJsonString("COGSAccountCode", Function.RawText(document_LinesImpl.CogsAcct)));
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.CogsOcrCod))
						{
							stringBuilder.Append(Function.GetJsonString("COGSCostingCode", Function.RawText(document_LinesImpl.CogsOcrCod)));
						}
						else
						{
							stringBuilder.Append("\"COGSCostingCode\": null,");
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.CogsOcrCo2))
						{
							stringBuilder.Append(Function.GetJsonString("COGSCostingCode2", Function.RawText(document_LinesImpl.CogsOcrCo2)));
						}
						else
						{
							stringBuilder.Append("\"COGSCostingCode2\": null,");
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.CogsOcrCo3))
						{
							stringBuilder.Append(Function.GetJsonString("COGSCostingCode3", Function.RawText(document_LinesImpl.CogsOcrCo3)));
						}
						else
						{
							stringBuilder.Append("\"COGSCostingCode3\": null,");
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.CogsOcrCo4))
						{
							stringBuilder.Append(Function.GetJsonString("COGSCostingCode4", Function.RawText(document_LinesImpl.CogsOcrCo4)));
						}
						else
						{
							stringBuilder.Append("\"COGSCostingCode4\": null,");
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.CogsOcrCo5))
						{
							stringBuilder.Append(Function.GetJsonString("COGSCostingCode5", Function.RawText(document_LinesImpl.CogsOcrCo5)));
						}
						else
						{
							stringBuilder.Append("\"COGSCostingCode5\": null,");
						}
					}
					if (!string.IsNullOrEmpty(document_LinesImpl.Project) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update) || commandType == EnumCommandType.Add)
					{
						stringBuilder.Append(Function.GetJsonString("ProjectCode", Function.ToString(document_LinesImpl.Project)));
					}
					if (!document_LinesImpl.Quantity.Equals(0.0) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
					{
						stringBuilder.Append(Function.GetJsonString("Quantity", Function.ParseDecimal(document_LinesImpl.Quantity)));
					}
					if (document_LinesImpl.LineStatus != "C" && document_LinesImpl.TargetType <= 0 && !document_LinesImpl.Rate.Equals(0.0) && (commandType == EnumCommandType.Add || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update && (document_LinesImpl.TreeType == "N" || (document_LinesImpl.TreeType == "I" && document_LinesImpl.BOMTreeType != "0" && Function.ParseInt(document_LinesImpl.BOMTreeType) == 0)))))
					{
						if (!string.IsNullOrEmpty(document_LinesImpl.Currency) && document_LinesImpl.Currency == DocumentsRepository.MainCurrency[_connectionString])
						{
							stringBuilder.Append(Function.GetJsonString("Rate", 0));
						}
						else
						{
							stringBuilder.Append(Function.GetJsonString("Rate", Function.ParseDecimal(document_LinesImpl.Rate)));
						}
					}
					if (document_LinesImpl.PQTReqDate > new DateTime(1900, 1, 1))
					{
						stringBuilder.Append(Function.GetJsonString("RequiredDate", Convert.ToDateTime(document_LinesImpl.PQTReqDate).ToString("yyyyMMdd")));
					}
					if (!document_LinesImpl.PQTReqQty.Equals(0.0))
					{
						stringBuilder.Append(Function.GetJsonString("RequiredQuantity", Function.ParseDecimal(document_LinesImpl.PQTReqQty)));
					}
					if (!document_LinesImpl.PackQty.Equals(0.0) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
					{
						stringBuilder.Append(Function.GetJsonString("PackageQuantity", Function.ParseDecimal(document_LinesImpl.PackQty)));
					}
					if (!document_LinesImpl.SlpCode.Equals(0))
					{
						stringBuilder.Append(Function.GetJsonString("SalesPersonCode", Function.ParseInt(document_LinesImpl.SlpCode)));
					}
					_ = document_LinesImpl.ShipDate;
					if (document_LinesImpl.ShipDate > new DateTime(1900, 1, 1))
					{
						stringBuilder.Append(Function.GetJsonString("ShipDate", Convert.ToDateTime(document_LinesImpl.ShipDate).ToString("yyyyMMdd")));
					}
					if (!document_LinesImpl.TrnsCode.Equals(0))
					{
						stringBuilder.Append(Function.GetJsonString("ShippingMethod", Function.ParseInt(document_LinesImpl.TrnsCode)));
					}
					if (objType != ApzObjectTypes.oPurchaseOrders && objType != ApzObjectTypes.oDrafts && docObjectCode != ApzObjectTypes.oPurchaseOrders && objType == ApzObjectTypes.oDrafts)
					{
						if (!string.IsNullOrEmpty(document_LinesImpl.ShipToCode) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							stringBuilder.Append(Function.GetJsonString("ShipToCode", Function.ToString(document_LinesImpl.ShipToCode)));
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.ShipToDesc) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							stringBuilder.Append(Function.GetJsonString("ShipToDescription", Function.ToString(document_LinesImpl.ShipToDesc)));
						}
					}
					if (!string.IsNullOrEmpty(document_LinesImpl.TaxCode))
					{
						stringBuilder.Append(Function.GetJsonString("TaxCode", Function.ToString(document_LinesImpl.TaxCode)));
					}
					if (document_LinesImpl.NumPerMsr > 0.0)
					{
						stringBuilder.Append(Function.GetJsonString("UnitsOfMeasurment", document_LinesImpl.NumPerMsr));
					}
					else
					{
						stringBuilder.Append(Function.GetJsonString("UnitsOfMeasurment", 1));
					}
					if (!string.IsNullOrEmpty(document_LinesImpl.UseBaseUn) && new string[4] { "N", "Y", "tYES", "tNO" }.Contains(document_LinesImpl.UseBaseUn))
					{
						stringBuilder.Append(Function.GetJsonString("UseBaseUnits", Function.ToString(document_LinesImpl.UseBaseUn)));
					}
					if (!document_LinesImpl.UomEntry.Equals(0))
					{
						stringBuilder.Append(Function.GetJsonString("UoMEntry", Function.ParseInt(document_LinesImpl.UomEntry)));
					}
					if (!string.IsNullOrEmpty(document_LinesImpl.VatGroup))
					{
						stringBuilder.Append(Function.GetJsonString("VatGroup", Function.ToString(document_LinesImpl.VatGroup)));
					}
					if (doc.DocCur == DocumentsRepository.MainCurrency[_connectionString])
					{
						stringBuilder.Append(Function.GetJsonString("TaxTotal", document_LinesImpl.LineVat));
					}
					else
					{
						stringBuilder.Append(Function.GetJsonString("TaxTotal", document_LinesImpl.VatSumFrgn));
					}
					if (!string.IsNullOrEmpty(document_LinesImpl.WhsCode))
					{
						stringBuilder.Append(Function.GetJsonString("WarehouseCode", Function.ToString(document_LinesImpl.WhsCode)));
					}
					if (!string.IsNullOrEmpty(document_LinesImpl.SubCatNum) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
					{
						stringBuilder.Append(Function.GetJsonString("SupplierCatNum", Function.ToString(document_LinesImpl.SubCatNum)));
					}
					if (objType == ApzObjectTypes.oPurchaseRequest || (objType == ApzObjectTypes.oDrafts && docObjectCode == ApzObjectTypes.oPurchaseRequest))
					{
						stringBuilder.Append(Function.GetJsonString("LineVendor", Function.ToString(document_LinesImpl.LineVendor)));
					}
					if (new ApzObjectTypes[2]
					{
						ApzObjectTypes.oCreditNotes,
						ApzObjectTypes.oPurchaseCreditNotes
					}.Contains(objType) || (objType == ApzObjectTypes.oDrafts && new ApzObjectTypes[2]
					{
						ApzObjectTypes.oCreditNotes,
						ApzObjectTypes.oPurchaseCreditNotes
					}.Contains(docObjectCode)))
					{
						stringBuilder.Append((document_LinesImpl.NoInvtryMv == "Y") ? Function.GetJsonString("WithoutInventoryMovement", "tYES") : Function.GetJsonString("WithoutInventoryMovement", "tNO"));
					}
					if (!string.IsNullOrEmpty(document_LinesImpl.TreeType) && !"0".Equals(document_LinesImpl.TreeType))
					{
						stringBuilder.Append(Function.GetJsonString("TreeType", Function.ToString(document_LinesImpl.TreeType)));
					}
					if (objType == ApzObjectTypes.oReturns)
					{
						if (!string.IsNullOrEmpty(document_LinesImpl.EnSetCost) && document_LinesImpl.EnSetCost.Equals("Y"))
						{
							stringBuilder.Append(Function.GetJsonString("EnableReturnCost", "tYES"));
							stringBuilder.Append(Function.GetJsonString("ReturnCost", Function.ParseDecimal(document_LinesImpl.RetCost)));
						}
						else
						{
							stringBuilder.Append(Function.GetJsonString("EnableReturnCost", "tNO"));
						}
					}
					if (!string.IsNullOrEmpty(document_LinesImpl.TaxOnly))
					{
						stringBuilder.Append(Function.GetJsonString("TaxOnly", Function.ToString(document_LinesImpl.TaxOnly)));
					}
					if (!string.IsNullOrEmpty(document_LinesImpl.CountryOrg))
					{
						stringBuilder.Append(Function.GetJsonString("CountryOrg", document_LinesImpl.CountryOrg));
					}
				}
				if (text2.Equals("O") || text2.Equals("OPEN") || text.Equals("OPEN") || text.Equals("O"))
				{
					stringBuilder.Append(Function.GetJsonString("FreeText", Function.ToString(document_LinesImpl.FreeTxt)));
				}
				if (!string.IsNullOrEmpty(document_LinesImpl.LineStatus) && document_LinesImpl.LineStatus == "C" && (new ApzObjectTypes[5]
				{
					ApzObjectTypes.oQuotations,
					ApzObjectTypes.oOrders,
					ApzObjectTypes.oPurchaseRequest,
					ApzObjectTypes.oPurchaseOrders,
					ApzObjectTypes.oPurchaseQuotations
				}.Contains(objType) || (new ApzObjectTypes[5]
				{
					ApzObjectTypes.oQuotations,
					ApzObjectTypes.oOrders,
					ApzObjectTypes.oPurchaseRequest,
					ApzObjectTypes.oPurchaseOrders,
					ApzObjectTypes.oPurchaseQuotations
				}.Contains(docObjectCode) && objType == ApzObjectTypes.oDrafts)))
				{
					stringBuilder.Append(Function.GetJsonString("LineStatus", "bost_Close"));
				}
				if (objType == ApzObjectTypes.oInventoryGenEntry && doc.Lines.FirstOrDefault((Document_LinesImpl t) => t.BaseType == 202) != null && !doc.Lines.Any((Document_LinesImpl t) => t.BaseLine >= 0))
				{
					stringBuilder.Append(Function.GetJsonString("TransactionType", (document_LinesImpl.TranType == "R") ? "botrntReject" : "botrntComplete"));
				}
				stringBuilder.Append(DataService.MappingUdf(document_LinesImpl.UserFields, commandType, ignoreUpdateEmptyValue, lstFieldNotValidated));
				stringBuilder.Append("\"BatchNumbers\":[");
				if (doc.Lines[i].BatchNumbers != null && doc.Lines[i].BatchNumbers.Count > 0)
				{
					for (int num2 = 0; num2 < doc.Lines[i].BatchNumbers.Count; num2++)
					{
						stringBuilder.Append("{\r\n");
						BatchNumbersImpl batchNumbersImpl = doc.Lines[i].BatchNumbers[num2];
						if (!string.IsNullOrEmpty(batchNumbersImpl.BatchNumber))
						{
							stringBuilder.Append(Function.GetJsonString("BatchNumber", Function.ToString(batchNumbersImpl.BatchNumber)));
						}
						if (!string.IsNullOrEmpty(batchNumbersImpl.ManufacturerSerialNumber))
						{
							stringBuilder.Append(Function.GetJsonString("ManufacturerSerialNumber", Function.ToString(batchNumbersImpl.ManufacturerSerialNumber)));
						}
						if (!string.IsNullOrEmpty(batchNumbersImpl.InternalSerialNumber))
						{
							stringBuilder.Append(Function.GetJsonString("InternalSerialNumber", Function.ToString(batchNumbersImpl.InternalSerialNumber)));
						}
						if (batchNumbersImpl.ExpiryDate.HasValue && batchNumbersImpl.ExpiryDate > new DateTime(1900, 1, 1))
						{
							stringBuilder.Append(Function.GetJsonString("ExpiryDate", Convert.ToDateTime(batchNumbersImpl.ExpiryDate).ToString("yyyyMMdd")));
						}
						else
						{
							stringBuilder.Append(Function.GetJsonString("ExpiryDate", null));
						}
						if (batchNumbersImpl.ManufacturingDate.HasValue && batchNumbersImpl.ManufacturingDate > new DateTime(1900, 1, 1))
						{
							stringBuilder.Append(Function.GetJsonString("ManufacturingDate", Convert.ToDateTime(batchNumbersImpl.ManufacturingDate).ToString("yyyyMMdd")));
						}
						else
						{
							stringBuilder.Append(Function.GetJsonString("ManufacturingDate", null));
						}
						if (batchNumbersImpl.AddmisionDate.HasValue && batchNumbersImpl.AddmisionDate > new DateTime(1900, 1, 1))
						{
							stringBuilder.Append(Function.GetJsonString("AddmisionDate", Convert.ToDateTime(batchNumbersImpl.AddmisionDate).ToString("yyyyMMdd")));
						}
						else
						{
							stringBuilder.Append(Function.GetJsonString("AddmisionDate", null));
						}
						if (!string.IsNullOrEmpty(batchNumbersImpl.Location))
						{
							stringBuilder.Append(Function.GetJsonString("Location", Function.ToString(batchNumbersImpl.Location)));
						}
						if (!string.IsNullOrEmpty(batchNumbersImpl.Notes))
						{
							stringBuilder.Append(Function.GetJsonString("Notes", Function.ToString(batchNumbersImpl.Notes)));
						}
						if (commandType == EnumCommandType.Update || batchNumbersImpl.Quantity > 0.0)
						{
							stringBuilder.Append(Function.GetJsonString("Quantity", Function.ParseDecimal(batchNumbersImpl.Quantity)));
						}
						if (0 < batchNumbersImpl.TrackingNote)
						{
							stringBuilder.Append(Function.GetJsonString("TrackingNote", Function.ToString(batchNumbersImpl.TrackingNote)));
						}
						if (0 < batchNumbersImpl.TrackingNoteLine)
						{
							stringBuilder.Append(Function.GetJsonString("TrackingNoteLine", Function.ToString(batchNumbersImpl.TrackingNoteLine)));
						}
						stringBuilder.Append(DataService.MappingUdf(batchNumbersImpl.UserFields, EnumCommandType.Add, ignoreUpdateEmptyValue));
						stringBuilder.Append("},");
					}
				}
				stringBuilder.Append("],");
				stringBuilder.Append("\"SerialNumbers\":[");
				if (doc.Lines[i].SerialNumbers != null && doc.Lines[i].SerialNumbers.Count > 0)
				{
					for (int num3 = 0; num3 < doc.Lines[i].SerialNumbers.Count; num3++)
					{
						stringBuilder.Append("{\r\n");
						SerialNumbersImpl serialNumbersImpl = doc.Lines[i].SerialNumbers[num3];
						if (!string.IsNullOrEmpty(serialNumbersImpl.BatchID))
						{
							stringBuilder.Append(Function.GetJsonString("BatchID", Function.ToString(serialNumbersImpl.BatchID)));
						}
						if (serialNumbersImpl.SystemSerialNumber > 0)
						{
							stringBuilder.Append(Function.GetJsonString("SystemSerialNumber", serialNumbersImpl.SystemSerialNumber));
						}
						if (!string.IsNullOrEmpty(serialNumbersImpl.ManufacturerSerialNumber))
						{
							stringBuilder.Append(Function.GetJsonString("ManufacturerSerialNumber", Function.ToString(serialNumbersImpl.ManufacturerSerialNumber)));
						}
						if (!string.IsNullOrEmpty(serialNumbersImpl.InternalSerialNumber))
						{
							stringBuilder.Append(Function.GetJsonString("InternalSerialNumber", Function.ToString(serialNumbersImpl.InternalSerialNumber)));
						}
						if (!new ApzObjectTypes[4]
						{
							ApzObjectTypes.oDeliveryNotes,
							ApzObjectTypes.oInvoices,
							ApzObjectTypes.oReserveInvoices,
							ApzObjectTypes.oInventoryGenExit
						}.Contains(objType))
						{
							if (serialNumbersImpl.ExpiryDate.HasValue && serialNumbersImpl.ExpiryDate > new DateTime(1900, 1, 1))
							{
								stringBuilder.Append(Function.GetJsonString("ExpiryDate", Convert.ToDateTime(serialNumbersImpl.ExpiryDate).ToString("yyyyMMdd")));
							}
							else
							{
								stringBuilder.Append(Function.GetJsonString("ExpiryDate", null));
							}
							if (serialNumbersImpl.ManufactureDate.HasValue && serialNumbersImpl.ManufactureDate > new DateTime(1900, 1, 1))
							{
								stringBuilder.Append(Function.GetJsonString("ManufactureDate", Convert.ToDateTime(serialNumbersImpl.ManufactureDate).ToString("yyyyMMdd")));
							}
							else
							{
								stringBuilder.Append(Function.GetJsonString("ManufactureDate", null));
							}
							if (serialNumbersImpl.ReceptionDate.HasValue && serialNumbersImpl.ReceptionDate > new DateTime(1900, 1, 1))
							{
								stringBuilder.Append(Function.GetJsonString("ReceptionDate", Convert.ToDateTime(serialNumbersImpl.ReceptionDate).ToString("yyyyMMdd")));
							}
							else
							{
								stringBuilder.Append(Function.GetJsonString("ReceptionDate", null));
							}
						}
						if (!string.IsNullOrEmpty(serialNumbersImpl.Location))
						{
							stringBuilder.Append(Function.GetJsonString("Location", Function.ToString(serialNumbersImpl.Location)));
						}
						if (!string.IsNullOrEmpty(serialNumbersImpl.Notes))
						{
							stringBuilder.Append(Function.GetJsonString("Notes", Function.ToString(serialNumbersImpl.Notes)));
						}
						if (commandType == EnumCommandType.Update || serialNumbersImpl.Quantity > 0.0)
						{
							stringBuilder.Append(Function.GetJsonString("Quantity", Function.ParseDecimal(serialNumbersImpl.Quantity)));
						}
						if (serialNumbersImpl.BaseLineNumber >= 0)
						{
							stringBuilder.Append(Function.GetJsonString("BaseLineNumber", Function.ParseInt(serialNumbersImpl.BaseLineNumber)));
						}
						if (0 < serialNumbersImpl.TrackingNote)
						{
							stringBuilder.Append(Function.GetJsonString("TrackingNote", Function.ToString(serialNumbersImpl.TrackingNote)));
						}
						if (0 < serialNumbersImpl.TrackingNoteLine)
						{
							stringBuilder.Append(Function.GetJsonString("TrackingNoteLine", Function.ToString(serialNumbersImpl.TrackingNoteLine)));
						}
						stringBuilder.Append(DataService.MappingUdf(serialNumbersImpl.UserFields, EnumCommandType.Add, ignoreUpdateEmptyValue));
						stringBuilder.Append("},");
					}
				}
				stringBuilder.Append("],");
				stringBuilder.Append("\"DocumentLinesBinAllocations\":[");
				if (doc.Lines[i].BinAllocations != null && doc.Lines[i].BinAllocations.Count > 0)
				{
					for (int num4 = 0; num4 < doc.Lines[i].BinAllocations.Count; num4++)
					{
						stringBuilder.Append("{\r\n");
						DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl = doc.Lines[i].BinAllocations[num4];
						if (!documentLinesBinAllocationsImpl.BinAbsEntry.Equals(0))
						{
							stringBuilder.Append(Function.GetJsonString("BinAbsEntry", Function.ParseInt(documentLinesBinAllocationsImpl.BinAbsEntry)));
						}
						if (!documentLinesBinAllocationsImpl.Quantity.Equals(0.0))
						{
							stringBuilder.Append(Function.GetJsonString("Quantity", Function.ParseDecimal(documentLinesBinAllocationsImpl.Quantity)));
						}
						if (documentLinesBinAllocationsImpl.SerialAndBatchNumbersBaseLine.HasValue && documentLinesBinAllocationsImpl.SerialAndBatchNumbersBaseLine >= 0)
						{
							stringBuilder.Append(Function.GetJsonString("SerialAndBatchNumbersBaseLine", Function.ParseInt(documentLinesBinAllocationsImpl.SerialAndBatchNumbersBaseLine)));
						}
						else
						{
							stringBuilder.Append(Function.GetJsonString("SerialAndBatchNumbersBaseLine", -1));
						}
						if (documentLinesBinAllocationsImpl.BaseLineNumber >= 0)
						{
							stringBuilder.Append(Function.GetJsonString("BaseLineNumber", Function.ParseInt(documentLinesBinAllocationsImpl.BaseLineNumber)));
						}
						stringBuilder.Append("},");
					}
				}
				stringBuilder.Append("],");
				if (document_LinesImpl != null)
				{
					stringBuilder.Append("\"DocumentLineAdditionalExpenses\":[");
					if (document_LinesImpl.ExpnsCode_F1 > 0)
					{
						stringBuilder.Append("{\r\n");
						if (commandType != EnumCommandType.Add)
						{
							stringBuilder.Append(Function.GetJsonString("LineNumber", document_LinesImpl.LineNum));
						}
						else
						{
							stringBuilder.Append(Function.GetJsonString("LineNumber", i));
						}
						stringBuilder.Append(Function.GetJsonString("GroupCode", 0));
						stringBuilder.Append(Function.GetJsonString("ExpenseCode", document_LinesImpl.ExpnsCode_F1));
						if (!Function.ParseDecimal(document_LinesImpl.LineTotal_F1).Equals(0m) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							stringBuilder.Append(Function.GetJsonString("LineTotal", Function.ParseDecimal(document_LinesImpl.LineTotal_F1)));
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.VatGroup_F1) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							stringBuilder.Append(Function.GetJsonString("VatGroup", Function.ToString(document_LinesImpl.VatGroup_F1)));
						}
						if (!Function.ParseDecimal(document_LinesImpl.VatPrcnt_F1).Equals(0m) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							stringBuilder.Append(Function.GetJsonString("TaxPercent", Function.ParseDecimal(document_LinesImpl.VatPrcnt_F1)));
						}
						if (!Function.ParseDecimal(document_LinesImpl.VatSum_F1).Equals(0m) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							stringBuilder.Append(Function.GetJsonString("TaxSum", Function.ParseDecimal(document_LinesImpl.VatSum_F1)));
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode_F1) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							stringBuilder.Append(Function.GetJsonString("DistributionRule", Function.ToString(document_LinesImpl.OcrCode_F1)));
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode2_F1) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							stringBuilder.Append(Function.GetJsonString("DistributionRule2", Function.ToString(document_LinesImpl.OcrCode2_F1)));
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode3_F1) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							stringBuilder.Append(Function.GetJsonString("DistributionRule3", Function.ToString(document_LinesImpl.OcrCode3_F1)));
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode4_F1) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							stringBuilder.Append(Function.GetJsonString("DistributionRule4", Function.ToString(document_LinesImpl.OcrCode4_F1)));
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode5_F1) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							stringBuilder.Append(Function.GetJsonString("DistributionRule5", Function.ToString(document_LinesImpl.OcrCode5_F1)));
						}
						stringBuilder.Append("},");
					}
					if (document_LinesImpl.ExpnsCode_F2 > 0)
					{
						stringBuilder.Append("{");
						if (commandType != EnumCommandType.Add)
						{
							stringBuilder.Append(Function.GetJsonString("LineNumber", document_LinesImpl.LineNum));
						}
						else
						{
							stringBuilder.Append(Function.GetJsonString("LineNumber", i));
						}
						stringBuilder.Append(Function.GetJsonString("GroupCode", 1));
						stringBuilder.Append(Function.GetJsonString("ExpenseCode", document_LinesImpl.ExpnsCode_F2));
						if (!Function.ParseDecimal(document_LinesImpl.LineTotal_F2).Equals(0m) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							stringBuilder.Append(Function.GetJsonString("LineTotal", Function.ParseDecimal(document_LinesImpl.LineTotal_F2)));
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.VatGroup_F2) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							stringBuilder.Append(Function.GetJsonString("VatGroup", Function.ToString(document_LinesImpl.VatGroup_F2)));
						}
						if (!Function.ParseDecimal(document_LinesImpl.VatPrcnt_F2).Equals(0m) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							stringBuilder.Append(Function.GetJsonString("TaxPercent", Function.ParseDecimal(document_LinesImpl.VatPrcnt_F2)));
						}
						if (!Function.ParseDecimal(document_LinesImpl.VatSum_F2).Equals(0m) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							stringBuilder.Append(Function.GetJsonString("TaxSum", Function.ParseDecimal(document_LinesImpl.VatSum_F2)));
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode_F2) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							stringBuilder.Append(Function.GetJsonString("DistributionRule", Function.ToString(document_LinesImpl.OcrCode_F2)));
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode2_F2) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							stringBuilder.Append(Function.GetJsonString("DistributionRule2", Function.ToString(document_LinesImpl.OcrCode2_F2)));
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode3_F2) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							stringBuilder.Append(Function.GetJsonString("DistributionRule3", Function.ToString(document_LinesImpl.OcrCode3_F2)));
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode4_F2) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							stringBuilder.Append(Function.GetJsonString("DistributionRule4", Function.ToString(document_LinesImpl.OcrCode4_F2)));
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode5_F2) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							stringBuilder.Append(Function.GetJsonString("DistributionRule5", Function.ToString(document_LinesImpl.OcrCode5_F2)));
						}
						stringBuilder.Append("},");
					}
					if (document_LinesImpl.ExpnsCode_F3 > 0)
					{
						stringBuilder.Append("{");
						if (commandType != EnumCommandType.Add)
						{
							stringBuilder.Append(Function.GetJsonString("LineNumber", document_LinesImpl.LineNum));
						}
						else
						{
							stringBuilder.Append(Function.GetJsonString("LineNumber", i));
						}
						stringBuilder.Append(Function.GetJsonString("GroupCode", 2));
						stringBuilder.Append(Function.GetJsonString("ExpenseCode", document_LinesImpl.ExpnsCode_F3));
						if (!Function.ParseDecimal(document_LinesImpl.LineTotal_F3).Equals(0m) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							stringBuilder.Append(Function.GetJsonString("LineTotal", Function.ParseDecimal(document_LinesImpl.LineTotal_F3)));
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.VatGroup_F3) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							stringBuilder.Append(Function.GetJsonString("VatGroup", Function.ToString(document_LinesImpl.VatGroup_F3)));
						}
						if (!Function.ParseDecimal(document_LinesImpl.VatPrcnt_F3).Equals(0m) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							stringBuilder.Append(Function.GetJsonString("TaxPercent", Function.ParseDecimal(document_LinesImpl.VatPrcnt_F3)));
						}
						if (!Function.ParseDecimal(document_LinesImpl.VatSum_F3).Equals(0m) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							stringBuilder.Append(Function.GetJsonString("TaxSum", Function.ParseDecimal(document_LinesImpl.VatSum_F3)));
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode_F3) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							stringBuilder.Append(Function.GetJsonString("DistributionRule", Function.ToString(document_LinesImpl.OcrCode_F3)));
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode2_F3) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							stringBuilder.Append(Function.GetJsonString("DistributionRule2", Function.ToString(document_LinesImpl.OcrCode2_F3)));
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode3_F3) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							stringBuilder.Append(Function.GetJsonString("DistributionRule3", Function.ToString(document_LinesImpl.OcrCode3_F3)));
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode4_F3) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							stringBuilder.Append(Function.GetJsonString("DistributionRule4", Function.ToString(document_LinesImpl.OcrCode4_F3)));
						}
						if (!string.IsNullOrEmpty(document_LinesImpl.OcrCode5_F3) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update))
						{
							stringBuilder.Append(Function.GetJsonString("DistributionRule5", Function.ToString(document_LinesImpl.OcrCode5_F3)));
						}
						stringBuilder.Append("},");
					}
					stringBuilder.Append("],");
				}
				stringBuilder.Append("},");
			}
			stringBuilder.Append("],");
		}
		return stringBuilder.ToString();
	}

	public override async Task<HttpResult> SaveDraftToDocumentApproval(DocumentsImpl doc, string sessionId, string userName = "")
	{
		try
		{
			string text = "{ \"Document\": { \"DocEntry\": " + doc.draftKey + ",";
			if (doc.Comments != null)
			{
				text += Function.GetJsonString("Comments", doc.Comments);
			}
			if (doc.JrnlMemo != null)
			{
				text += Function.GetJsonString("JournalMemo", doc.JrnlMemo);
			}
			if (doc.Header != null)
			{
				text += Function.GetJsonString("OpeningRemarks", doc.Header);
			}
			if (doc.Footer != null)
			{
				text += Function.GetJsonString("ClosingRemarks", doc.Footer);
			}
			text += "} }";
			HttpResult httpResult = DataService.ProcessServiceJson(ApzObjectTypes.oDrafts, sessionId, text, Enums.HttpAction.SaveDraftToDocument);
			if (httpResult.Message.Contains("This entry already exists in the following tables"))
			{
				httpResult = DataService.ProcessServiceJson(ApzObjectTypes.oDrafts, sessionId, text, Enums.HttpAction.SaveDraftToDocument);
			}
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				Logging.Write("ERROR", "SaveDraftToDocumentApproval", sessionId, httpResult.Message);
				return httpResult;
			}
			Logging.Write("WATCH", "SaveDraftToDocumentApproval", sessionId, doc.draftKey.ToString());
			return new HttpResult(Enums.MessageCode.Success, "", Function.ParseInt(httpResult.Content));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult SaveDraftToDocument(DocumentsImpl doc, string sessionId)
	{
		try
		{
			if (new ApzObjectTypes[2]
			{
				ApzObjectTypes.oInventoryTransferRequest,
				ApzObjectTypes.oStockTransfer
			}.Contains(doc.DocObjectCode))
			{
				return _unitOfWork.StockTransfer.SaveDraftToDocument(doc, sessionId);
			}
			string xmlObject = ((doc.DocEntry > 0) ? Mapping(doc, EnumCommandType.Update, doc.DocObjectCode) : Mapping(doc, EnumCommandType.Add, doc.DocObjectCode));
			HttpResult httpResult = DataService.AddObject(doc.DocObjectCode, sessionId, xmlObject);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				return httpResult;
			}
			int num = Function.ParseInt(httpResult.Content);
			if (doc.DocEntry > 0)
			{
				string tableByApzObjectType = Function.GetTableByApzObjectType(doc.DocObjectCode);
				Logging.Write("WATCH", $"Remove document from approval process- docEntry {doc.DocEntry} - object type {doc.DocObjectCode}");
				ExecuteNonQuery("UPDATE " + tableByApzObjectType + " SET \"WddStatus\"='-' WHERE \"DocEntry\" =" + doc.DocEntry, CommandType.Text);
			}
			Logging.Write("WATCH", $"Add new document - {doc.DocObjectCode} - DocEntry - {num}");
			try
			{
				if (!ServiceUnitOfWork.Documents.SaveDraftToDocument(doc.DocObjectCode, num, doc.draftKey))
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "", "");
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			}
			return new HttpResult(Enums.MessageCode.Success, "", num);
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex2.Message);
		}
	}

	public override async Task<HttpResult> AddNew(DocumentsImpl doc, string sessionId, ApzObjectTypes objType, ApzObjectTypes docObjectCode = ApzObjectTypes.oDrafts)
	{
		try
		{
			List<BodyRequestObject> lstObj = new List<BodyRequestObject>();
			if (doc.Udos != null && doc.Udos.Count > 0)
			{
				foreach (DocumentUdo udo2 in doc.Udos)
				{
					if (udo2.dsChild.Tables.Count <= 0 || !udo2.UdoTable.IsNotNull())
					{
						continue;
					}
					int udoEntry = GetUdoEntry(doc, udo2, "OPMT", "@OPMT", "U_paymentEntry");
					DataTable udo = GetUdo("OPMT");
					if (udo != null && udo.Rows.Count > 0)
					{
						StringBuilder stringBuilder = new StringBuilder();
						if (udoEntry == 0)
						{
							stringBuilder.Append(_unitOfWork.UDOs.MappingXml(udo, udo2.dtFather, udo2.dsChild, sessionId, "Add"));
							lstObj.Add(new BodyRequestObject
							{
								ObjectType = ApzObjectTypes.oNone,
								UdoCode = Function.ToString(udo.Rows[0]["Code"]),
								Action = Enums.HttpAction.AddNew,
								BodyObject = stringBuilder.ToString()
							});
						}
						else
						{
							stringBuilder.Append(_unitOfWork.UDOs.MappingXml(udo, udo2.dtFather, udo2.dsChild, sessionId, "Update"));
							lstObj.Add(new BodyRequestObject
							{
								ObjectType = ApzObjectTypes.oNone,
								UdoCode = Function.ToString(udo.Rows[0]["Code"]),
								Key = Function.ParseInt(udo2.dtFather.Rows[0]["DocEntry"]),
								Action = Enums.HttpAction.Update,
								BodyObject = stringBuilder.ToString()
							});
						}
					}
				}
			}
			string bodyObject = await MappingAsync(doc, EnumCommandType.Add, objType, docObjectCode);
			lstObj.Add(new BodyRequestObject
			{
				ObjectType = objType,
				OutObjectKey = true,
				Action = Enums.HttpAction.AddNew,
				BodyObject = bodyObject
			});
			HttpResult httpResult = DataService.ProcessMutiTransaction(sessionId, lstObj);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				Logging.Write("ERROR", "Add Error", objType.ToString(), sessionId, httpResult.Message);
				return new HttpResult(Enums.MessageCode.Error, httpResult.Message);
			}
			int num = Function.ParseInt(httpResult.Content);
			return new HttpResult(Enums.MessageCode.Success, "", num);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override void ProcessSendMessage(string sessionId, string messageString)
	{
		DataService.ProcessJson(ApzObjectTypes.oMessages, sessionId, messageString);
	}

	public override HttpResult CancelDoc(int docEntry, string sessionId, ApzObjectTypes objType)
	{
		try
		{
			return DataService.ProcessJson(objType, sessionId, "", Enums.HttpAction.Cancel, docEntry);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult CloseDoc(int docEntry, string sessionId, ApzObjectTypes objType)
	{
		try
		{
			return DataService.ProcessJson(objType, sessionId, "", Enums.HttpAction.Close, docEntry);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override string RemoveAndCloseDocumentDraft(DataTable dt, string sessionId)
	{
		try
		{
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				BodyRequestObject bodyRequestObject = new BodyRequestObject();
				bodyRequestObject.ObjectType = ApzObjectTypes.oDrafts;
				bodyRequestObject.Key = Function.ParseInt(dt.Rows[i]["Id"]);
				bodyRequestObject.OutObjectKey = true;
				if (dt.Rows[i]["UpdateType"].ToString() == "C")
				{
					bodyRequestObject.Action = Enums.HttpAction.Close;
				}
				else
				{
					bodyRequestObject.Action = Enums.HttpAction.Delete;
				}
				list.Add(bodyRequestObject);
			}
			HttpResult httpResult = DataService.ProcessMutiTransaction(sessionId, list);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				Logging.Write("ERROR", $"Close document - {ApzObjectTypes.oDrafts} - {httpResult.Message}");
				return httpResult.Message;
			}
			int num = Function.ParseInt(httpResult.Content);
			Logging.Write("WATCH", $"Close document - {ApzObjectTypes.oDrafts} - DocEntry - {num}");
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override string RemoveDoc(int idDocEntry, string sessionId, ApzObjectTypes apzObjectTypes)
	{
		try
		{
			HttpResult httpResult = DataService.ProcessJson(apzObjectTypes, sessionId, "", Enums.HttpAction.Delete, idDocEntry);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				Logging.Write("ERROR", $"Remove document - {apzObjectTypes} - {httpResult.Message}");
				return httpResult.Message;
			}
			int num = Function.ParseInt(httpResult.Content);
			Logging.Write("WATCH", $"Remove document - {apzObjectTypes} - DocEntry - {num}");
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override HttpResult AddListDocument(List<DocumentsImpl> docs, string sessionId, ApzObjectTypes objType)
	{
		try
		{
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			if (docs != null && docs.Count > 0)
			{
				for (int i = 0; i < docs.Count; i++)
				{
					list.Add(new BodyRequestObject
					{
						ObjectType = objType,
						OutObjectKey = true,
						Action = Enums.HttpAction.AddNew,
						BodyObject = Mapping(docs[i], EnumCommandType.Add, objType)
					});
				}
				return DataService.ProcessMutiTransaction(sessionId, list);
			}
			return new HttpResult(Enums.MessageCode.Error, "Not enough data");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult UpdateListDocument(List<DocumentsImpl> docs, string sessionId)
	{
		try
		{
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			if (docs != null && docs.Count > 0)
			{
				for (int i = 0; i < docs.Count; i++)
				{
					ApzObjectTypes objType = (ApzObjectTypes)docs[i].ObjType;
					BodyRequestObject bodyRequestObject = new BodyRequestObject();
					bodyRequestObject.ObjectType = objType;
					bodyRequestObject.OutObjectKey = true;
					if (docs[i].DocEntry <= 0)
					{
						bodyRequestObject.BodyObject = Mapping(docs[i], EnumCommandType.Add, objType);
						bodyRequestObject.Action = Enums.HttpAction.AddNew;
					}
					else
					{
						bodyRequestObject.BodyObject = Mapping(docs[i], EnumCommandType.Update, objType);
						bodyRequestObject.Action = Enums.HttpAction.Update;
						bodyRequestObject.Key = docs[i].DocEntry;
					}
					list.Add(bodyRequestObject);
				}
				return DataService.ProcessMutiTransaction(sessionId, list);
			}
			return new HttpResult(Enums.MessageCode.Error, "Not enough data");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult AddListDocument(List<DocumentsImpl> docs, string sessionId)
	{
		try
		{
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			if (docs != null && docs.Count > 0)
			{
				for (int i = 0; i < docs.Count; i++)
				{
					ApzObjectTypes objType = (ApzObjectTypes)docs[i].ObjType;
					list.Add(new BodyRequestObject
					{
						ObjectType = objType,
						OutObjectKey = true,
						Action = Enums.HttpAction.AddNew,
						BodyObject = Mapping(docs[i], EnumCommandType.Add, objType)
					});
				}
				return DataService.ProcessMutiTransaction(sessionId, list);
			}
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override string DeleteLines(DocumentsImpl doc, string sessionId, ApzObjectTypes objType, List<int> lstRemoved)
	{
		return string.Empty;
	}

	protected override string GetUpdateExtnQuery()
	{
		return "insert into APZ_EXRL(\"DocEntry\",\"Token\",\"Status\") SELECT IFNULL((SELECT Max(\"DocEntry\") FROM APZ_EXRL),0)+1,:Guid,'N' FROM DUMMY;";
	}
}
