using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Production;
using Apzon.Entities.Models.UserFields;
using Apzon.Services;
using SAPbobsCOM;

namespace Apzon.DataServices.Implements;

public class SolutionRepository : Repository<DataSet>, ISolution, IRepository<DataSet>, IDisposable
{
	protected readonly IUnitOfWork _unitOfWork;

	public SolutionRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
		_unitOfWork = unitOfWork;
	}

	public void Dispose()
	{
		Dispose();
	}

	public string Save(DataSet doc, string sessionId)
	{
		try
		{
			if (doc.IsNotNull())
			{
				int num = 0;
				List<AttachmentFile_Impl> list = new List<AttachmentFile_Impl>();
				if (doc.Tables[1].IsNotNull())
				{
					for (int i = 0; i < doc.Tables[1].Rows.Count; i++)
					{
						list.Add(new AttachmentFile_Impl
						{
							AbsEntry = Function.ParseInt(doc.Tables[1].Rows[i]["AbsEntry"]),
							Line = Function.ParseInt(doc.Tables[1].Rows[i]["Line"]),
							srcPath = Function.ToString(doc.Tables[1].Rows[i]["srcPath"]),
							trgtPath = Function.ToString(doc.Tables[1].Rows[i]["trgtPath"]),
							FileName = Function.ToString(doc.Tables[1].Rows[i]["FileName"]),
							FileExt = Function.ToString(doc.Tables[1].Rows[i]["FileExt"]),
							Date = Function.ParseDateTimes(doc.Tables[1].Rows[i]["Date"]),
							UsrID = Function.ParseInt(doc.Tables[1].Rows[i]["UsrID"]),
							Copied = Function.ToString(doc.Tables[1].Rows[i]["Copied"]),
							Override = Function.ToString(doc.Tables[1].Rows[i]["Override"]),
							subPath = Function.ToString(doc.Tables[1].Rows[i]["subPath"]),
							FreeText = Function.ToString(doc.Tables[1].Rows[i]["FreeText"])
						});
					}
				}
				HttpResult httpResult = ServiceUnitOfWork.Files.SaveAttachmentFiles(list, new Dictionary<string, object> { 
				{
					"SltCode,int",
					doc.Tables[0].Rows[0]["SltCode"]
				} }, "OSLT");
				if (httpResult.MessageCode != Enums.MessageCode.Success)
				{
					return httpResult.Message;
				}
				doc.Tables[0].Rows[0]["AtcEntry"] = (num = Function.ParseInt(httpResult.Content));
				if (!Exist(doc, sessionId, ApzObjectTypes.oKnowledgeBaseSolutions))
				{
					return AddNew(doc, sessionId, ApzObjectTypes.oKnowledgeBaseSolutions, num).Message;
				}
				return UpdateData(doc, sessionId, ApzObjectTypes.oKnowledgeBaseSolutions, num);
			}
			return "Error Data!";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private bool Exist(DataSet doc, string sessionId, ApzObjectTypes objType)
	{
		try
		{
			return Function.ParseInt(ExecuteScarlar("select top 1 1 from OSLT where \"SltCode\" = @sltCode;", CommandType.Text, new SqlParameter("@sltCode", Function.ParseInt(doc.Tables[0].Rows[0]["SltCode"])))) == 1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return !Function.ParseInt(doc.Tables[0].Rows[0]["SltCode"]).Equals(0);
		}
	}

	protected virtual HttpResult AddNew(DataSet doc, string sessionId, ApzObjectTypes objType, int absEntry)
	{
		try
		{
			DataService.GetBusinessObjectTemplate(sessionId, "oKnowledgeBaseSolutions");
			string text = Mapping(doc, EnumCommandType.Add);
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\"><dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + text + "</dis:AddObject></env:Body></env:Envelopes>";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					if (absEntry > 0)
					{
						DeleteFile(absEntry);
					}
					return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					int num = Function.ParseInt(xElement3.Value);
					Logging.Write("WATCH", $"Add new Solution - Id - {num}");
					return new HttpResult(Enums.MessageCode.Success, "", num);
				}
			}
		}
		catch (Exception ex)
		{
			if (absEntry > 0)
			{
				DeleteFile(absEntry);
			}
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult();
	}

	public virtual string Mapping(DataSet doc, EnumCommandType commandType)
	{
		DataTable dataTable = doc.Tables[0];
		_ = doc.Tables[1];
		string text = "<BOM>\r\n                          <BO>\r\n                          <AdmInfo>\r\n                            <Object>oKnowledgeBaseSolutions</Object>\r\n                          </AdmInfo>";
		try
		{
			if (commandType == EnumCommandType.Update)
			{
				text = text + "<QueryParams><SolutionCode>" + dataTable.Rows[0]["SltCode"]?.ToString() + "</SolutionCode></QueryParams>";
			}
			text += "<KnowledgeBaseSolutions><row>";
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["ItemCode"])) || commandType == EnumCommandType.Update)
			{
				text = text + "<ItemCode>" + Function.RawText(dataTable.Rows[0]["ItemCode"]) + "</ItemCode>";
			}
			if (Function.ParseInt(dataTable.Rows[0]["StatusNum"]) != 0 || commandType == EnumCommandType.Update)
			{
				text = text + "<Status>" + Function.ParseInt(dataTable.Rows[0]["StatusNum"]) + "</Status>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["Subject"])) || commandType == EnumCommandType.Update)
			{
				text = text + "<Solution>" + Function.RawText(dataTable.Rows[0]["Subject"]) + "</Solution>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["Symptom"])) || commandType == EnumCommandType.Update)
			{
				text = text + "<Symptom>" + Function.RawText(dataTable.Rows[0]["Symptom"]) + "</Symptom>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["Cause"])) || commandType == EnumCommandType.Update)
			{
				text = text + "<Cause>" + Function.RawText(dataTable.Rows[0]["Cause"]) + "</Cause>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["Descriptio"])) || commandType == EnumCommandType.Update)
			{
				text = text + "<Description>" + Function.RawText(dataTable.Rows[0]["Descriptio"]) + "</Description>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["Attachment"])) || commandType == EnumCommandType.Update)
			{
				text = text + "<Attachments>" + Function.RawText(dataTable.Rows[0]["Attachment"]) + "</Attachments>";
			}
			IEnumerable<string> enumerable = from t in (from DataColumn c in dataTable.Columns
					select c.ColumnName).ToList()
				where t.StartsWith("U_")
				select t;
			if (enumerable != null && enumerable.ToList() != null && enumerable.ToList().Count > 0)
			{
				List<UserFieldsImpl> list = new List<UserFieldsImpl>();
				for (int num = 0; num < enumerable.ToList().Count; num++)
				{
					UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
					if (dataTable.Columns[enumerable.ToList()[num]].DataType == typeof(DateTime))
					{
						userFieldsImpl.Value = Function.ToString(Function.ParseDateTime(dataTable.Rows[0][enumerable.ToList()[num]]));
						userFieldsImpl.Type = BoFieldTypes.db_Date;
					}
					else if (dataTable.Columns[enumerable.ToList()[num]].DataType == typeof(double))
					{
						userFieldsImpl.Value = Function.ToString(Function.ParseDouble(dataTable.Rows[0][enumerable.ToList()[num]]));
						userFieldsImpl.Type = BoFieldTypes.db_Numeric;
					}
					else
					{
						userFieldsImpl.Value = Function.ToString(Function.ToString(dataTable.Rows[0][enumerable.ToList()[num]]));
						userFieldsImpl.Type = BoFieldTypes.db_Alpha;
					}
					userFieldsImpl.Name = enumerable.ToList()[num];
					list.Add(userFieldsImpl);
				}
				text += DataService.MappingUdf(list);
			}
			text += "</row></KnowledgeBaseSolutions>";
			text += "</BO></BOM>";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return text;
	}

	protected virtual string UpdateData(DataSet doc, string sessionId, ApzObjectTypes objType, int absEntry)
	{
		try
		{
			string text = Mapping(doc, EnumCommandType.Update);
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\"><dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + text + "</dis:UpdateObject></env:Body></env:Envelopes>";
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
					Logging.Write("WATCH", $"update document - {objType} - DocEntry - {num}");
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

	public virtual string DeleteDoc(int sltCode, string sessionId)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\"><dis:RemoveObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                             <BOM>\r\n                            <BO>\r\n                             <AdmInfo>\r\n                               <Object>oKnowledgeBaseSolutions</Object>\r\n                             </AdmInfo>\r\n                             <QueryParams>\r\n                               <SolutionCode>" + sltCode + "</SolutionCode>\r\n                             </QueryParams>\r\n                            </BO>\r\n                           </BOM>\r\n                        </dis:RemoveObject></env:Body></env:Envelopes>";
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
					string arg = Function.ToString(xElement3.Value);
					Logging.Write("WATCH", $"Remove a Solution - {arg}");
					return string.Empty;
				}
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	protected string DeleteFile(int absEntry)
	{
		try
		{
			string query = "delete from ATC1 WHERE \"AbsEntry\" = " + absEntry + ";";
			if (ExecuteNonQuery(query, CommandType.Text) >= 0)
			{
				return null;
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}
}
