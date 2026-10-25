using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Interfaces;
using Apzon.Services;
using SAPbobsCOM;

namespace Apzon.DataServices.Implements;

internal class PaymentTermSetupRepository : MasterRepository, IPaymentTermSetUp
{
	public PaymentTermSetupRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	public virtual string Remove(int groupNum, string sessionId)
	{
		try
		{
			return base.Remove(groupNum, sessionId, ApzObjectTypes.oPaymentTermsTypes).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return ex.Message;
		}
	}

	public virtual string Save(DataTable dtDoc, string sessionId)
	{
		try
		{
			Logging.Write("WATCH", "Start save payment term");
			List<string> list = new List<string>();
			foreach (DataRow row in dtDoc.Rows)
			{
				string text = "";
				text = ((Function.ParseInt(row["GroupNum"]) != 0) ? ("<dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + Mapping(row, Enums.ServiceCommandType.Update) + "</dis:UpdateObject>") : ("<dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + Mapping(row, Enums.ServiceCommandType.Add) + "</dis:AddObject>"));
				list.Add(text);
			}
			string text2 = string.Join(" ", list.ToArray());
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">" + text2 + "</env:Body></env:Envelopes>";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					Logging.Write("ERROR", "End - Save payment term" + xElement2.Value);
					return xElement2.Value;
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					int num = Function.ParseInt(xElement3.Value);
					Logging.Write("WATCH", "[SUCCESS]End - Save payment term " + num);
					return "";
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return ex.Message;
		}
		return "";
	}

	private string Mapping(DataRow item, Enums.ServiceCommandType commandType)
	{
		try
		{
			string text = "<BOM><BO\r\n                    xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                    <AdmInfo>\r\n                        <Object>oPaymentTermsTypes</Object>\r\n                    </AdmInfo> ";
			if (commandType == Enums.ServiceCommandType.Update)
			{
				text = text + " <QueryParams>\r\n                                <GroupNumber>" + Function.ParseInt(item["GroupNum"]) + "</GroupNumber>\r\n                            </QueryParams>";
			}
			text += "<PaymentTermsTypes>";
			text += "<row>";
			text = text + "<PaymentTermsGroupName>" + Function.RawText(item["PymntGroup"]) + "</PaymentTermsGroupName>";
			text = Function.ToString(item["PayDuMonth"]) switch
			{
				"E" => text + "<StartFrom>" + BoPayTermDueTypes.pdt_MonthEnd.ToString() + "</StartFrom>", 
				"H" => text + "<StartFrom>" + BoPayTermDueTypes.pdt_HalfMonth.ToString() + "</StartFrom>", 
				"Y" => text + "<StartFrom>" + BoPayTermDueTypes.pdt_MonthStart.ToString() + "</StartFrom>", 
				_ => text + "<StartFrom>" + BoPayTermDueTypes.pdt_None.ToString() + "</StartFrom>", 
			} + "<NumberOfAdditionalMonths>" + Function.ParseInt(item["ExtraMonth"]) + "</NumberOfAdditionalMonths>";
			text = text + "<NumberOfAdditionalDays>" + Function.ParseInt(item["ExtraDays"]) + "</NumberOfAdditionalDays>";
			text = text + "<CreditLimit>" + Function.ParseDecimal(item["CredLimit"]) + "</CreditLimit>";
			text = text + "<GeneralDiscount>" + Function.ParseDecimal(item["VolumDscnt"]) + "</GeneralDiscount>";
			text = text + "<InterestOnArrears>" + Function.ParseDecimal(item["LatePyChrg"]) + "</InterestOnArrears>";
			if (Function.ParseLong(item["ListNum"]) != 0L)
			{
				text = text + "<PriceListNo>" + Function.ParseLong(item["ListNum"]) + "</PriceListNo>";
			}
			text = text + "<LoadLimit>" + Function.ParseDecimal(item["ObligLimit"]) + "</LoadLimit>";
			text = Function.ToString(item["OpenRcpt"]) switch
			{
				"3" => text + "<OpenReceipt>" + BoOpenIncPayment.oip_Cash.ToString() + "</OpenReceipt>", 
				"1" => text + "<OpenReceipt>" + BoOpenIncPayment.oip_Checks.ToString() + "</OpenReceipt>", 
				"4" => text + "<OpenReceipt>" + BoOpenIncPayment.oip_Credit.ToString() + "</OpenReceipt>", 
				"2" => text + "<OpenReceipt>" + BoOpenIncPayment.oip_BankTransfer.ToString() + "</OpenReceipt>", 
				_ => text + "<OpenReceipt>" + BoOpenIncPayment.oip_No.ToString() + "</OpenReceipt>", 
			};
			if (!string.IsNullOrEmpty(Function.ToString(item["DiscCode"])))
			{
				text = text + "<DiscountCode>" + Function.RawText(item["DiscCode"]) + "</DiscountCode>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(item["DunningCod"])))
			{
				text = text + "<DunningCode>" + Function.RawText(item["DunningCod"]) + "</DunningCode>";
			}
			text = Function.ToString(item["BslineDate"]) switch
			{
				"P" => text + "<BaselineDate>" + BoBaselineDate.bld_PostingDate.ToString() + "</BaselineDate>", 
				"S" => text + "<BaselineDate>" + BoBaselineDate.bld_SystemDate.ToString() + "</BaselineDate>", 
				"C" => text + "<BaselineDate>" + BoBaselineDate.bld_ClosingDate.ToString() + "</BaselineDate>", 
				_ => text + "<BaselineDate>" + BoBaselineDate.bld_TaxDate.ToString() + "</BaselineDate>", 
			} + "<NumberOfToleranceDays>" + Function.ParseLong(item["TolDays"]) + "</NumberOfToleranceDays>";
			text += "</row>";
			return text + "</PaymentTermsTypes></BO></BOM>";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return "";
	}
}
