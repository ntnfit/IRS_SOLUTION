using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.DataServices.Interfaces;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class DistributionRuleRepository : Repository<DataSet>, IDistributionRule, IRepository<DataSet>, IDisposable
{
	public DistributionRuleRepository(Apzon.Services.IUnitOfWork unitOfWork, IDataService dataService)
		: base(unitOfWork, dataService)
	{
	}

	public async Task<string> Save(string sessionId, DataSet ds)
	{
		_ = 1;
		try
		{
			if (ds == null || ds.Tables.Count != 2 || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0 || ds.Tables[1] == null || ds.Tables[1].Rows.Count == 0)
			{
				return "No data to save - DataSet or DataTable is null";
			}
			if (!Exist(Function.ToString(ds.Tables[0].Rows[0]["OcrCode"])))
			{
				return await AddNew(sessionId, ds);
			}
			return await UpdateData(sessionId, ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	protected bool Exist(string orcCode)
	{
		try
		{
			if (Function.ParseInt(ExecuteScarlar("select top 1 1 from OOCR where \"OcrCode\" = @ocrCode;", CommandType.Text, new SqlParameter("@ocrCode", orcCode))) < 1)
			{
				return false;
			}
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return false;
		}
	}

	protected virtual async Task<string> UpdateData(string sessionId, DataSet ds)
	{
		try
		{
			string text = Mapping(ds);
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>DistributionRulesService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <UpdateDistributionRule xmlns=\"DistributionRulesService\">" + Function.ToString(text) + "</UpdateDistributionRule>\r\n                                </soap:Body>  \r\n                            </soap:Envelope> ";
			XmlElement xmlElement = (await DataService.ProcessAsync(xmlObject)).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					return xElement2.Value;
				}
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	protected virtual async Task<string> AddNew(string sessionId, DataSet ds)
	{
		try
		{
			string text = Mapping(ds);
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>DistributionRulesService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <AddDistributionRule xmlns=\"DistributionRulesService\">" + Function.ToString(text) + "</AddDistributionRule>\r\n                                </soap:Body>  \r\n                            </soap:Envelope> ";
			XmlElement xmlElement = (await DataService.ProcessAsync(xmlObject)).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					return xElement2.Value;
				}
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string Mapping(DataSet ds)
	{
		string text = "";
		try
		{
			DataRow dataRow = ds.Tables[0].Rows[0];
			DataTable dataTable = ds.Tables[1];
			text += "<DistributionRule>";
			text = text + "<FactorCode>" + Function.RawText(dataRow["OcrCode"]) + "</FactorCode>";
			text = (string.IsNullOrEmpty(Function.ToString(dataRow["OcrName"])) ? (text + "<FactorDescription/>") : (text + "<FactorDescription>" + Function.RawText(dataRow["OcrName"]) + "</FactorDescription>"));
			text = text + "<TotalFactor>" + Function.ParseDouble(dataRow["OcrTotal"]) + "</TotalFactor>";
			text = ((!string.IsNullOrEmpty(Function.ToString(dataRow["Direct"])) && !(Function.ToString(dataRow["Direct"]) == "N")) ? (text + "<Direct>Y</Direct>") : (text + "<Direct>N</Direct>"));
			text = text + "<InWhichDimension>" + Function.ParseShort(dataRow["DimCode"]) + "</InWhichDimension>";
			text = ((!string.IsNullOrEmpty(Function.ToString(dataRow["Active"])) && !(Function.ToString(dataRow["Active"]) == "N")) ? (text + "<Active>tYES</Active>") : (text + "<Active>tNO</Active>"));
			text = ((!string.IsNullOrEmpty(Function.ToString(dataRow["IsFixedAmt"])) && !(Function.ToString(dataRow["IsFixedAmt"]) == "N")) ? (text + "<IsFixedAmount>tYES</IsFixedAmount>") : (text + "<IsFixedAmount>tNO</IsFixedAmount>"));
			text += "<DistributionRuleLines>";
			for (int i = 0; i < dataTable.Rows.Count; text += "</DistributionRuleLine>", i++)
			{
				text += "<DistributionRuleLine>";
				DataRow dataRow2 = dataTable.Rows[i];
				text = (string.IsNullOrEmpty(Function.ToString(dataRow2["PrcCode"])) ? (text + "<CenterCode/>") : (text + "<CenterCode>" + Function.RawText(dataRow2["PrcCode"]) + "</CenterCode>"));
				text = text + "<TotalInCenter>" + Function.ParseDouble(dataRow2["PrcAmount"]) + "</TotalInCenter>";
				DateTime? dateTime = Function.ParseDateTime(dataRow2["ValidFrom"]);
				if (dateTime.HasValue)
				{
					_ = dateTime.Value;
					if (dateTime.Value.Year >= 1990)
					{
						text = text + "<EffectiveFrom>" + dateTime.Value.ToString("yyyy-MM-dd") + "</EffectiveFrom>";
						goto IL_02f3;
					}
				}
				text += "<EffectiveFrom/>";
				goto IL_02f3;
				IL_02f3:
				DateTime? dateTime2 = Function.ParseDateTime(dataRow2["ValidTo"]);
				if (dateTime2.HasValue)
				{
					_ = dateTime2.Value;
					if (dateTime2.Value.Year >= 1990)
					{
						text = text + "<EffectiveTo>" + dateTime2.Value.ToString("yyyy-MM-dd") + "</EffectiveTo>";
						continue;
					}
				}
				text += "<EffectiveTo/>";
			}
			text += "</DistributionRuleLines>";
			text += "</DistributionRule>";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return text;
	}

	public virtual async Task<string> DeleteDistributeRule(string sessionId, string ocrCode)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>DistributionRulesService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <DeleteDistributionRule xmlns=\"DistributionRulesService\">\r\n                                     <DistributionRule><FactorCode>" + Function.RawText(ocrCode) + "</FactorCode></DistributionRule>\r\n                                    </DeleteDistributionRule>  \r\n                                </soap:Body>  \r\n                            </soap:Envelope>  ";
			XmlElement xmlElement = (await DataService.ProcessAsync(xmlObject)).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					return xElement2.Value;
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

	public void Dispose()
	{
		Dispose();
	}
}
