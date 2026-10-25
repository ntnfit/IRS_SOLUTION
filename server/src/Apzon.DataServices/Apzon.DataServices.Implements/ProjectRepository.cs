using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class ProjectRepository : Repository<Common.Table_Code>, IProject, IRepository<Common.Table_Code>, IDisposable
{
	public ProjectRepository(Apzon.Services.IUnitOfWork unitOfWork, IDataService dataService)
		: base(unitOfWork, dataService)
	{
	}

	public void Dispose()
	{
		Dispose();
	}

	public virtual string Save(DataTable dt, string sessionId)
	{
		try
		{
			string text = Mapping(dt, sessionId);
			if (!text.StartsWith("ERR:"))
			{
				StringBuilder stringBuilder = new StringBuilder("<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>ProjectsService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>");
				stringBuilder.Append(text);
				stringBuilder.Append("</soap:Body>  \r\n                            </soap:Envelope>");
				if (DataService.Process(stringBuilder.ToString()).Content is XmlElement xmlElement)
				{
					XElement xElement = XElement.Parse(xmlElement.OuterXml);
					if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
					{
						XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
						if (xElement2 != null)
						{
							throw new Exception(xElement2.Value);
						}
					}
					else
					{
						string text2 = UpdateUDF(dt);
						if (!string.IsNullOrEmpty(text2))
						{
							Logging.Write("ERROR", "[Projects setup] Update UDF failed: \"" + text2 + "\"");
						}
					}
				}
				return "";
			}
			return text.Remove(0, 4);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string UpdateUDF(DataTable dt)
	{
		try
		{
			List<DataColumn> list = new List<DataColumn>();
			for (int i = 0; i < dt.Columns.Count; i++)
			{
				if (dt.Columns[i].ColumnName.StartsWith("U_"))
				{
					list.Add(dt.Columns[i]);
				}
			}
			if (list.Count == 0)
			{
				return "";
			}
			string text = "";
			for (int j = 0; j < dt.Rows.Count; j++)
			{
				DataRow dataRow = dt.Rows[j];
				text += "Update OPRJ SET ";
				for (int k = 0; k < list.Count; k++)
				{
					text = text + Function.ReplaceSQLInjectCharInColumnName(list[k].ColumnName) + " = " + GetValueFromColumn(list[k], dt.Rows[j]) + ", ";
				}
				text = text.Remove(text.Length - 2);
				text = text + " Where PrjCode = '" + Function.ReplaceSQLInjectCharInCode(Function.ToString(dataRow["PrjCode"])) + "'" + Environment.NewLine;
			}
			if (!string.IsNullOrEmpty(text))
			{
				ExecuteNonQuery(text, CommandType.Text);
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
	}

	private string GetValueFromColumn(DataColumn col, DataRow row)
	{
		switch (col.DataType.ToString().ToLower())
		{
		case "system.string":
			return "N'" + Function.ToString(row[col]) + "'";
		case "system.int16":
			return Function.ParseShort(row[col]).ToString();
		case "system.int32":
			return Function.ParseInt(row[col]).ToString();
		case "system.int64":
			return Function.ParseLong(row[col]).ToString();
		case "system.double":
			return Function.ParseDecimal(row[col]).ToString();
		case "system.decimal":
			return Function.ParseDecimal(row[col]).ToString();
		case "system.datetime":
		{
			DateTime? dateTime = Function.ParseDateTime(row[col]);
			if (dateTime.HasValue)
			{
				_ = dateTime.Value;
				return "N'" + dateTime.Value.ToString("yyyyMMdd") + "'";
			}
			return "null";
		}
		case "system.single":
			return Function.ParseDecimal(row[col]).ToString();
		default:
			return "N'" + Function.ToString(row[col]) + "'";
		}
	}

	public virtual string Mapping(DataTable dt, string sessionId)
	{
		try
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				string text = Function.RawText(dt.Rows[i]["PrjCode"]);
				string text2 = "";
				text2 = (GetByPrjCode(sessionId, text) ? "UpdateProject" : "AddProject");
				stringBuilder.Append("<" + text2 + " xmlns=\"ProjectsService\">\r\n                            <Project> ");
				stringBuilder.Append("<Code>" + text + "</Code>");
				stringBuilder.Append("<Name>" + Function.RawText(dt.Rows[i]["PrjName"]) + "</Name>");
				DateTime? dateTime = Function.ParseDateTime(dt.Rows[i]["ValidFrom"]);
				if (dateTime.HasValue)
				{
					_ = dateTime.Value;
					stringBuilder.Append("<ValidFrom>" + dateTime.Value.ToString("yyyyMMdd") + "</ValidFrom>");
				}
				else
				{
					stringBuilder.Append("<ValidFrom></ValidFrom>");
				}
				DateTime? dateTime2 = Function.ParseDateTime(dt.Rows[i]["ValidTo"]);
				if (dateTime2.HasValue)
				{
					_ = dateTime2.Value;
					stringBuilder.Append("<ValidTo>" + dateTime2.Value.ToString("yyyyMMdd") + "</ValidTo>");
				}
				else
				{
					stringBuilder.Append("<ValidTo></ValidTo>");
				}
				if (Function.ToString(dt.Rows[i]["Active"]) == "Y")
				{
					stringBuilder.Append("<Active>tYES</Active>");
				}
				else
				{
					stringBuilder.Append("<Active>tNO</Active>");
				}
				stringBuilder.Append("</Project>\r\n                            </" + text2 + ">");
			}
			return stringBuilder.ToString();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "ERR:" + ex.Message;
		}
	}

	public string Delete(string prjCode, string sessionId)
	{
		throw new NotImplementedException();
	}

	public bool Exists(string prjCode, string sessionId)
	{
		throw new NotImplementedException();
	}

	public bool GetByPrjCode(string sessionId, string prjCode)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>ProjectsService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <GetProject xmlns=\"ProjectsService\">\r\n                                     <Project><Code>" + prjCode + "</Code></Project>\r\n                                    </GetProject>  \r\n                                </soap:Body>  \r\n                            </soap:Envelope>  ";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					throw new Exception(xElement2.Value);
				}
			}
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return false;
	}

	public virtual string DeleteProject(string code, string sessionId)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>ProjectsService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <DeleteProject xmlns=\"ProjectsService\">\r\n                                     <Project><Code>" + code + "</Code></Project>\r\n                                    </DeleteProject>  \r\n                                </soap:Body>  \r\n                            </soap:Envelope>  ";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					throw new Exception(xElement2.Value);
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
}
