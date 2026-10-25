using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.DataServices.Interfaces;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class FreightSetupReopsitory : Repository<DataTable>, IFreightSetup
{
	public FreightSetupReopsitory(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	public virtual string Save(DataTable doc, string sessionId)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>AdditionalExpenses</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>" + Mapping(doc) + "</soap:Body>  \r\n                            </soap:Envelope>";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					return xElement2.Value;
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					int num = Function.ParseInt(xElement3.Value);
					Logging.Write("WATCH", $"Freight Setup - {num}");
					return Function.ToString(num);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return "";
	}

	public string Mapping(DataTable doc)
	{
		try
		{
			string text = "";
			foreach (DataRow row in doc.Rows)
			{
				string text2 = "";
				int num = Function.ParseInt(row["ExpnsCode"]);
				if (num >= 0)
				{
					string text3 = ((num > 0) ? "Update" : "Add");
					text2 = text2 + "<" + text3 + " xmlsn = \"AdditionalExpenses\">\r\n                                 <AdditionalExpenses>";
					if (text3 == "Update")
					{
						text2 = text2 + "<ExpensCode>" + num + "</ExpensCode>";
					}
					text2 = text2 + "<Name>" + Function.RawText(row["ExpnsName"]) + "</Name>";
					text2 = text2 + "<RevenuesAccount>" + Function.RawText(row["RevAcct"]) + "</RevenuesAccount>";
					text2 = text2 + "<ExpenseAccount>" + Function.RawText(row["ExpnsAcct"]) + "</ExpenseAccount>";
					text2 = text2 + "<OutputVATGroup>" + Function.RawText(row["VatGroupO"]) + "</OutputVATGroup>";
					text2 = text2 + "<InputVATGroup>" + Function.RawText(row["VatGroupI"]) + "</InputVATGroup>";
					text2 = text2 + "<FixedAmountRevenues>" + Function.RawText(row["RevFixSum"]) + "</FixedAmountRevenues>";
					text2 = text2 + "<FixedAmountExpenses>" + Function.RawText(row["ExpFixSum"]) + "</FixedAmountExpenses>";
					text2 = text2 + "<DistributionMethod>" + Function.RawText(row["DistrbMthd"]) + "</DistributionMethod>";
					text2 = text2 + "<WTLiable>" + Function.RawText(row["WTLiable"]) + "</WTLiable>";
					text2 = text2 + "<DistributionRule>" + Function.RawText(row["OcrCode"]) + "</DistributionRule>";
					text2 = text2 + "<Project>" + Function.RawText(row["Project"]) + "</Project>";
					text2 = text2 + "<LastPurchasePrice>" + Function.RawText(row["LstPchPrce"]) + "</LastPurchasePrice>";
					text2 = text2 + "<Stock>" + Function.RawText(row["Stock"]) + "</Stock>";
					text2 = text2 + "<DrawingMethod>" + Function.RawText(row["BaseMethod"]) + "</DrawingMethod>";
				}
				else
				{
					text2 = text2 + "<Remove xmlsn = \"AdditionalExpenses\"><AdditionalExpenses><ExpensCode>" + Function.RawText(Math.Abs(num)) + "</ExpensCode></AdditionalExpenses></Remove>";
				}
				text += text2;
			}
			return text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "";
		}
	}
}
