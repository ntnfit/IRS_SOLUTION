using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class ApprovalTemplatesRepository : Repository<DataTable>, IApprovalTemplates, IDisposable
{
	public ApprovalTemplatesRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	public virtual string Mapping(DataSet ds, EnumCommandType commandType, string sessionId)
	{
		try
		{
			string empty = string.Empty;
			DataRow dataRow = ds.Tables[0].Rows[0];
			DataTable dataTable = ds.Tables[1];
			DataTable dataTable2 = ds.Tables[2];
			DataTable dataTable3 = ds.Tables[3];
			_ = ds.Tables[4];
			_ = ds.Tables[5];
			empty = ((commandType != EnumCommandType.Update) ? (empty + "<dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">") : (empty + "<dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">"));
			empty += "<BOM>\r\n                          <BO>\r\n                          <AdmInfo>\r\n                            <Object>oConfirmationLevels</Object>\r\n                          </AdmInfo>";
			empty += "<ApprovalTemplate>";
			empty += "<row>";
			empty = empty + "<Code>" + Function.RawText(dataRow["Code"]) + "</Code>";
			empty = empty + "<Name>" + Function.RawText(dataRow["Name"]) + "</Name>";
			empty = empty + "<Remarks>" + Function.RawText(dataRow["Name"]) + "</Remarks>";
			empty = ((!Function.ToString(dataRow["Conds"]).Equals("Y")) ? (empty + "<UseTerms>tNO</UseTerms>") : (empty + "<UseTerms>tYES</UseTerms>"));
			empty = ((!Function.ToString(dataRow["Active"]).Equals("Y")) ? (empty + "<IsActive>tNO</IsActive>") : (empty + "<IsActive>tYES</IsActive>"));
			empty = ((!Function.ToString(dataRow["AppOnUpd"]).Equals("Y")) ? (empty + "<IsActiveWhenUpdatingDocuments>tNO</IsActiveWhenUpdatingDocuments>") : (empty + "<IsActiveWhenUpdatingDocuments>tYES</IsActiveWhenUpdatingDocuments>"));
			empty += "<ApprovalTemplateUsers>";
			for (int i = 0; i < dataTable.Rows.Count - 1; i++)
			{
				empty += "<row>";
				empty = empty + "<UserID>" + Function.ParseInt(dataTable.Rows[i]["UserID"]) + "</UserID>";
				empty += "</row>";
			}
			empty += "</ApprovalTemplateUsers>";
			empty += "<ApprovalTemplateStages>";
			for (int j = 0; j < dataTable3.Rows.Count - 1; j++)
			{
				empty += "<row>";
				empty = empty + "<SortID>" + Function.ParseInt(dataTable3.Rows[j]["SortId"]) + "</SortID>";
				empty = empty + "<ApprovalStageCode>" + Function.ParseInt(dataTable3.Rows[j]["WstCode"]) + "</ApprovalStageCode>";
				empty = empty + "<Remarks>" + Function.ToString(dataTable3.Rows[j]["WstRemarks"]) + "<Remarks>";
				empty += "</row>";
			}
			empty += "</ApprovalTemplateStages>";
			empty += "<ApprovalTemplateDocuments>";
			for (int k = 0; k < dataTable2.Rows.Count - 1; k++)
			{
				empty += "<row>";
				switch (Function.ParseInt(dataTable2.Rows[k]["TransType"]))
				{
				case 23:
					empty += "<DocumentType>atdtQuotation</DocumentType>";
					break;
				case 17:
					empty += "<DocumentType>atdtOrder</DocumentType>";
					break;
				case 15:
					empty += "<DocumentType>atdtDelivery</DocumentType>";
					break;
				case 16:
					empty += "<DocumentType>atdtReturns</DocumentType>";
					break;
				case 234000031:
					empty += "<DocumentType>atdtReturnsRequest</DocumentType>";
					break;
				case 203:
					empty += "<DocumentType>atdtArDownPayment</DocumentType>";
					break;
				case 13:
					empty += "<DocumentType>atdtArInvoice</DocumentType>";
					break;
				case 14:
					empty += "<DocumentType>atdtArCreditMemo</DocumentType>";
					break;
				case 132:
					empty += "<DocumentType>atdtCorrectionInvoice</DocumentType>";
					break;
				case 22:
					empty += "<DocumentType>atdtPurchaseOrder</DocumentType>";
					break;
				case 20:
					empty += "<DocumentType>atdtGoodsReceiptPO</DocumentType>";
					break;
				case 21:
					empty += "<DocumentType>atdtGoodsReturns</DocumentType>";
					break;
				case 204:
					empty += "<DocumentType>atdtApDownPayment</DocumentType>";
					break;
				case 18:
					empty += "<DocumentType>atdtApInvoice</DocumentType>";
					break;
				case 19:
					empty += "<DocumentType>atdtApCreditMemo</DocumentType>";
					break;
				case 59:
					empty += "<DocumentType>atdtGoodsReceipt</DocumentType>";
					break;
				case 60:
					empty += "<DocumentType>atdtGoodsIssue</DocumentType>";
					break;
				case 67:
					empty += "<DocumentType>atdtInventoryTransfer</DocumentType>";
					break;
				case 540000006:
					empty += "<DocumentType>atdtPurchaseQuotation</DocumentType>";
					break;
				case 1250000001:
					empty += "<DocumentType>atdtInventoryTransferRequest</DocumentType>";
					break;
				case 46:
					empty += "<DocumentType>atdtOutgoingPayment</DocumentType>";
					break;
				case 1470000065:
					empty += "<DocumentType>atdtInventoryCounting</DocumentType>";
					break;
				case 10000071:
					empty += "<DocumentType>atdtInventoryPosting</DocumentType>";
					break;
				case 310000001:
					empty += "<DocumentType>atdtInventoryOpeningBalance</DocumentType>";
					break;
				}
				empty += "</row>";
			}
			empty += "</ApprovalTemplateDocuments>";
			empty += "</row>";
			empty += "</ApprovalTemplate>";
			empty += "</BO></BOM>";
			return (commandType != EnumCommandType.Update) ? (empty + "</dis:AddObject>") : (empty + "</dis:UpdateObject>");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return string.Empty;
		}
	}

	public virtual HttpResult Save(Tuple<DataSet, string> tuple, string sessionId)
	{
		try
		{
			EnumCommandType commandType = (tuple.Item2.Equals("A") ? EnumCommandType.Add : EnumCommandType.Update);
			string text = Mapping(tuple.Item1, commandType, sessionId);
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">" + text + "</env:Body></env:Envelopes>";
			DataService.GetBusinessObjectTemplate(sessionId, "102");
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					Logging.Write("ERROR", "Save Approval stages error: " + xElement2.Value);
					return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				List<XElement> list = (from t in xElement.Descendants()
					where t.Name.LocalName.Equals("RetKey")
					select t).ToList();
				if (list.Count > 0)
				{
					List<string> list2 = new List<string>();
					list2.AddRange(list.Select((XElement t) => Function.ToString(t.Value)));
					if (list2.Count > 0)
					{
						Logging.Write("WATCH", "Save Approval stages Success: " + string.Join(",", list2.ToArray()));
						return new HttpResult(Enums.MessageCode.Success, "", string.Join(",", list2.ToArray()));
					}
				}
			}
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public virtual HttpResult Delete(int templateCode, string sessionId)
	{
		try
		{
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void Dispose()
	{
		throw new NotImplementedException();
	}
}
