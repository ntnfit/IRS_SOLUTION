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

public class ApprovalStagesRepository : Repository<DataTable>, IApprovalStages, IDisposable
{
	public ApprovalStagesRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	public virtual string Mapping(DataRow document, DataTable documentLine, EnumCommandType commandType, string sessionId)
	{
		try
		{
			string empty = string.Empty;
			empty = ((commandType != EnumCommandType.Update) ? (empty + "<dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">") : (empty + "<dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">"));
			empty += "<BOM>\r\n                          <BO>\r\n                          <AdmInfo>\r\n                            <Object>oConfirmationLevels</Object>\r\n                          </AdmInfo>";
			empty += "<ApprovalStages>";
			empty += "<row>";
			empty = empty + "<Code>" + Function.RawText(document["WstCode"]) + "</Code>";
			empty = empty + "<Name>" + Function.RawText(document["Name"]) + "</Name>";
			empty = empty + "<NoOfApproversRequired>" + Function.RawText(document["MaxRejReqr"]) + "</NoOfApproversRequired>";
			empty = empty + "<Remarks>" + Function.RawText(document["Remarks"]) + "</Remarks>";
			empty += "<ApprovalStageApprovers>";
			for (int i = 0; i < documentLine.Rows.Count - 1; i++)
			{
				empty += "<row>";
				empty = empty + "<UserID>" + Function.RawText(documentLine.Rows[i]["UserID"]) + "</UserID>";
				empty += "</row>";
			}
			empty += "</ApprovalStageApprovers>";
			empty += "</row>";
			empty += "</ApprovalStages>";
			empty += "</BO></BOM>";
			return (commandType != EnumCommandType.Update) ? (empty + "</dis:AddObject>") : (empty + "</dis:UpdateObject>");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return string.Empty;
		}
	}

	public virtual HttpResult Save(Tuple<DataTable, DataTable, string> tuple, string sessionId)
	{
		try
		{
			EnumCommandType commandType = (tuple.Item3.Equals("A") ? EnumCommandType.Add : EnumCommandType.Update);
			string text = Mapping(tuple.Item1.Rows[0], tuple.Item2, commandType, sessionId);
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
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult();
	}

	public virtual HttpResult Delete(int stageCode, string sessionId)
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
