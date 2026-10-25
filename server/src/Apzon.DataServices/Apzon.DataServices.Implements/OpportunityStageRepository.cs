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

public class OpportunityStageRepository(Apzon.Services.IUnitOfWork unitOfWork, IDataService dataService) : Repository<DataTable>(unitOfWork, dataService), IOpportunityStage, IDisposable
{
	private string _objectType = "oSalesStages";

	public string Delete(int seqNo, string sessionId)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\"><dis:RemoveObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                        <BOM>\r\n                          <BO>\r\n                            <AdmInfo>\r\n                                <Object>" + _objectType + "</Object>\r\n                            </AdmInfo>\r\n                            <QueryParams><SequenceNo>" + seqNo + "</SequenceNo></QueryParams>\r\n                            </BO>\r\n                        </BOM>\r\n                        </dis:RemoveObject>\r\n                    </env:Body></env:Envelopes>";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					Logging.Write("ERROR", "Delete Sales Stage error: " + xElement2.Value);
					return xElement2.Value;
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
						Logging.Write("WATCH", "Delete Sales Stage Success: " + string.Join(",", list2.ToArray()));
						return "";
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

	public virtual HttpResult Save(DataTable dt, string sessionId)
	{
		try
		{
			string text = string.Empty;
			if (dt != null && dt.Rows.Count > 0)
			{
				for (int i = 0; i < dt.Rows.Count; i++)
				{
					text += Mapping(dt.Rows[i], (!Exists(Function.ToString(dt.Rows[i]["Num"]), sessionId)) ? EnumCommandType.Add : EnumCommandType.Update);
				}
				string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">" + text + "</env:Body></env:Envelopes>";
				XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
				XElement xElement = XElement.Parse(xmlElement.OuterXml);
				if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
				{
					XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
					if (xElement2 != null)
					{
						Logging.Write("ERROR", "Save Sales Stages error: " + xElement2.Value);
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
							Logging.Write("WATCH", "Save Sales Stages Success: " + string.Join(",", list2.ToArray()));
							return new HttpResult(Enums.MessageCode.Success, "", string.Join(",", list2.ToArray()));
						}
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

	public virtual string Mapping(DataRow doc, EnumCommandType commandType)
	{
		string text = "";
		text = ((commandType != EnumCommandType.Update) ? (text + "<dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">") : (text + "<dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">"));
		text = text + "<BOM>\r\n                          <BO>\r\n                          <AdmInfo>\r\n                            <Object>" + _objectType + "</Object>\r\n                          </AdmInfo>";
		try
		{
			if (commandType == EnumCommandType.Update)
			{
				text = text + "<QueryParams><SequenceNo>" + Function.ParseInt(doc["Num"]) + "</SequenceNo></QueryParams>";
			}
			text += "<SalesStages>";
			text += "<row>";
			text = text + "<Name>" + Function.RawText(doc["Descript"]) + "</Name>";
			text = text + "<Stageno>" + Function.ParseInt(doc["StepId"]) + "</Stageno>";
			text = text + "<ClosingPercentage>" + Function.ParseDecimal(doc["CloPrcnt"]) + "</ClosingPercentage>";
			text = text + "<Cancelled>" + Function.ToString(doc["Canceled"]) + "</Cancelled>";
			text += "</row>";
			text += "</SalesStages>";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		text += "</BO></BOM>";
		if (commandType == EnumCommandType.Update)
		{
			return text + "</dis:UpdateObject>";
		}
		return text + "</dis:AddObject>";
	}

	public virtual bool Exists(string seqNo, string sessionId)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?>\r\n                            <env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                             <env:Header>\r\n                              <SessionID>" + sessionId + "</SessionID> \r\n                             </env:Header>\r\n                             <env:Body>\r\n                              <dis:GetByKey xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                               <Object>" + _objectType + "</Object> \r\n                               <SequenceNo>" + Function.RawText(seqNo) + "</SequenceNo> \r\n                              </dis:GetByKey>\r\n                             </env:Body>\r\n                            </env:Envelope>";
			XmlElement xmlElement = DataService.GetByKey(sessionId, xmlObject).Content as XmlElement;
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				return false;
			}
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return false;
	}

	public void Dispose()
	{
		throw new NotImplementedException();
	}
}
