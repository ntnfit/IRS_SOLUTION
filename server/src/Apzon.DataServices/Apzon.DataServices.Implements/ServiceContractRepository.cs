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

public class ServiceContractRepository : Repository<DataTable>, IServiceContract, IRepository<DataTable>, IDisposable
{
	protected readonly IUnitOfWork _unitOfWork;

	public ServiceContractRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
		_unitOfWork = unitOfWork;
	}

	public void Dispose()
	{
		Dispose();
	}

	public string Save(DataSet data, string sessionId)
	{
		try
		{
			if (data != null && data.Tables.Count > 0)
			{
				int num;
				if (data.Tables.Count > 2 && data.Tables[2].Rows.Count > 0)
				{
					if (Function.ParseInt(data.Tables[0].Rows[0]["AtcEntry"]) == 0)
					{
						List<AttachmentFile_Impl> list = new List<AttachmentFile_Impl>();
						if (data.Tables[1].IsNotNull())
						{
							for (int i = 0; i < data.Tables[2].Rows.Count; i++)
							{
								list.Add(new AttachmentFile_Impl
								{
									AbsEntry = Function.ParseInt(data.Tables[2].Rows[i]["AbsEntry"]),
									Line = Function.ParseInt(data.Tables[2].Rows[i]["Line"]),
									srcPath = Function.ToString(data.Tables[2].Rows[i]["srcPath"]),
									trgtPath = Function.ToString(data.Tables[2].Rows[i]["trgtPath"]),
									FileName = Function.ToString(data.Tables[2].Rows[i]["FileName"]),
									FileExt = Function.ToString(data.Tables[2].Rows[i]["FileExt"]),
									Date = Function.ParseDateTimes(data.Tables[2].Rows[i]["Date"]),
									UsrID = Function.ParseInt(data.Tables[2].Rows[i]["UsrID"]),
									Copied = Function.ToString(data.Tables[2].Rows[i]["Copied"]),
									Override = Function.ToString(data.Tables[2].Rows[i]["Override"]),
									subPath = Function.ToString(data.Tables[2].Rows[i]["subPath"]),
									FreeText = Function.ToString(data.Tables[2].Rows[i]["FreeText"])
								});
							}
						}
						HttpResult httpResult = ServiceUnitOfWork.Files.SaveAttachmentFiles(list, new Dictionary<string, object> { 
						{
							"ContractID,int",
							data.Tables[0].Rows[0]["ContractID"]
						} }, "OCTR");
						if (httpResult.MessageCode != Enums.MessageCode.Success)
						{
							return httpResult.Message;
						}
						num = Function.ParseInt(httpResult.Content);
					}
					else
					{
						num = Function.ParseInt(data.Tables[0].Rows[0]["AtcEntry"]);
					}
				}
				else
				{
					num = Function.ParseInt(data.Tables[0].Rows[0]["AtcEntry"]);
				}
				if (!Exist(Function.ParseInt(data.Tables[0].Rows[0]["ContractID"]), sessionId, ApzObjectTypes.oServiceContracts))
				{
					return AddNew(data, sessionId, ApzObjectTypes.oServiceContracts, num).Message;
				}
				return UpdateData(data, sessionId, ApzObjectTypes.oServiceContracts, num);
			}
			return "Loi du lieu!";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	protected virtual string UpdateData(DataSet data, string sessionId, ApzObjectTypes objType, int absEnty)
	{
		try
		{
			DataService.GetBusinessObjectTemplate(sessionId, objType.ToString());
			string text = Mapping(data, sessionId, EnumCommandType.Update, objType, absEnty);
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

	protected virtual HttpResult AddNew(DataSet data, string sessionId, ApzObjectTypes objType, int absEntry)
	{
		try
		{
			DataService.GetBusinessObjectTemplate(sessionId, objType.ToString());
			string text = Mapping(data, sessionId, EnumCommandType.Add, objType, absEntry);
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
					Logging.Write("WATCH", $"Add new document - {objType} - DocEntry - {num}");
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

	public virtual string Mapping(DataSet data, string sessionId, EnumCommandType commandType, ApzObjectTypes objType, int absEntry)
	{
		DataTable dataTable = data.Tables[0];
		DataTable dataTable2 = data.Tables[1];
		string text = "<BOM>\r\n                          <BO xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                          <AdmInfo>\r\n                            <Object>" + objType.ToString() + "</Object>\r\n                          </AdmInfo>";
		try
		{
			if (commandType == EnumCommandType.Update)
			{
				text = text + "<QueryParams><ContractID>" + Function.ParseInt(dataTable.Rows[0]["ContractID"]) + "</ContractID></QueryParams>";
			}
			text += " <ServiceContracts>\r\n                            <row>";
			if (Function.ParseInt(dataTable.Rows[0]["ContractID"]) > 0)
			{
				text = text + "<ContractID>" + Function.RawText(dataTable.Rows[0]["ContractID"]) + "</ContractID>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["CstmrCode"])))
			{
				text = text + "<CustomerCode>" + Function.RawText(dataTable.Rows[0]["CstmrCode"]) + "</CustomerCode>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["CstmrName"])))
			{
				text = text + "<CustomerName>" + Function.RawText(dataTable.Rows[0]["CstmrName"]) + "</CustomerName>";
			}
			if (Function.ParseInt(dataTable.Rows[0]["CntctCode"]) != 0)
			{
				text = text + "<ContactCode>" + Function.ParseInt(dataTable.Rows[0]["CntctCode"]) + "</ContactCode>";
			}
			int num = Function.ParseInt(dataTable.Rows[0]["Owner"]);
			if (num != 0)
			{
				text = text + "<Owner>" + num + "</Owner>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["Status"])))
			{
				text = ((string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["TermDate"])) || !(Function.ParseDateTimes(dataTable.Rows[0]["TermDate"]) > new DateTime(1900, 1, 1))) ? (text + "<Status>" + Function.RawText(dataTable.Rows[0]["Status"]) + "</Status>") : (text + "<Status>T</Status>"));
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["CntrcTmplt"])))
			{
				text = text + "<ContractTemplate>" + Function.RawText(dataTable.Rows[0]["CntrcTmplt"]) + "</ContractTemplate>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["CntrcType"])))
			{
				text = text + "<ContractType>" + Function.RawText(dataTable.Rows[0]["CntrcType"]) + "</ContractType>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["Renewal"])))
			{
				text = text + "<Renewal>" + Function.RawText(dataTable.Rows[0]["Renewal"]) + "</Renewal>";
			}
			int num2 = Function.ParseInt(dataTable.Rows[0]["RemindVal"]);
			if (num2 > 0)
			{
				text = text + "<ReminderTime>" + num2 + "</ReminderTime>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["RemindUnit"])))
			{
				text = text + "<RemindUnit>" + Function.RawText(dataTable.Rows[0]["RemindUnit"]) + "</RemindUnit>";
			}
			int num3 = Function.ParseInt(dataTable.Rows[0]["Duration"]);
			if (num3 > 0)
			{
				text = text + "<DurationOfCoverage>" + num3 + "</DurationOfCoverage>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["StartDate"])) && Function.ParseDateTimes(dataTable.Rows[0]["StartDate"]) > new DateTime(1900, 1, 1))
			{
				text = text + "<StartDate>" + Convert.ToDateTime(dataTable.Rows[0]["StartDate"]).ToString("yyyyMMdd") + "</StartDate>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["EndDate"])) && Function.ParseDateTimes(dataTable.Rows[0]["EndDate"]) > new DateTime(1900, 1, 1))
			{
				text = text + "<EndDate>" + Convert.ToDateTime(dataTable.Rows[0]["EndDate"]).ToString("yyyyMMdd") + "</EndDate>";
			}
			int num4 = Function.ParseInt(dataTable.Rows[0]["ResponsVal"]);
			if (num4 > 0)
			{
				text = text + "<ResolutionTime>" + num4 + "</ResolutionTime>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["ResponsUnt"])))
			{
				text = text + "<ResolutionUnit>" + Function.RawText(dataTable.Rows[0]["ResponsUnt"]) + "</ResolutionUnit>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["Descriptio"])))
			{
				text = text + "<Description>" + Function.RawText(dataTable.Rows[0]["Descriptio"]) + "</Description>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["MonEnabled"])))
			{
				text = text + "<MondayEnabled>" + Function.RawText(dataTable.Rows[0]["MonEnabled"]) + "</MondayEnabled>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["TueEnabled"])))
			{
				text = text + "<TuesdayEnabled>" + Function.RawText(dataTable.Rows[0]["TueEnabled"]) + "</TuesdayEnabled>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["WedEnabled"])))
			{
				text = text + "<WednesdayEnabled>" + Function.RawText(dataTable.Rows[0]["WedEnabled"]) + "</WednesdayEnabled>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["ThuEnabled"])))
			{
				text = text + "<ThursdayEnabled>" + Function.RawText(dataTable.Rows[0]["ThuEnabled"]) + "</ThursdayEnabled>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["FriEnabled"])))
			{
				text = text + "<FridayEnabled>" + Function.RawText(dataTable.Rows[0]["FriEnabled"]) + "</FridayEnabled>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["SatEnabled"])))
			{
				text = text + "<SaturdayEnabled>" + Function.RawText(dataTable.Rows[0]["SatEnabled"]) + "</SaturdayEnabled>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["SunEnabled"])))
			{
				text = text + "<SundayEnabled>" + Function.RawText(dataTable.Rows[0]["SunEnabled"]) + "</SundayEnabled>";
			}
			text = text + "<MondayStart>" + Function.ParseIntToTimeSpanString(Function.ParseInt(dataTable.Rows[0]["MonStart"])) + "</MondayStart>";
			text = text + "<MondayEnd>" + Function.ParseIntToTimeSpanString(Function.ParseInt(dataTable.Rows[0]["MonEnd"])) + "</MondayEnd>";
			text = text + "<TuesdayStart>" + Function.ParseIntToTimeSpanString(Function.ParseInt(dataTable.Rows[0]["TueStart"])) + "</TuesdayStart>";
			text = text + "<TuesdayEnd>" + Function.ParseIntToTimeSpanString(Function.ParseInt(dataTable.Rows[0]["TueEnd"])) + "</TuesdayEnd>";
			text = text + "<WednesdayStart>" + Function.ParseIntToTimeSpanString(Function.ParseInt(dataTable.Rows[0]["WedStart"])) + "</WednesdayStart>";
			text = text + "<WednesdayEnd>" + Function.ParseIntToTimeSpanString(Function.ParseInt(dataTable.Rows[0]["WedEnd"])) + "</WednesdayEnd>";
			text = text + "<ThursdayStart>" + Function.ParseIntToTimeSpanString(Function.ParseInt(dataTable.Rows[0]["ThuStart"])) + "</ThursdayStart>";
			text = text + "<ThursdayEnd>" + Function.ParseIntToTimeSpanString(Function.ParseInt(dataTable.Rows[0]["ThuEnd"])) + "</ThursdayEnd>";
			text = text + "<FridayStart>" + Function.ParseIntToTimeSpanString(Function.ParseInt(dataTable.Rows[0]["FriStart"])) + "</FridayStart>";
			text = text + "<FridayEnd>" + Function.ParseIntToTimeSpanString(Function.ParseInt(dataTable.Rows[0]["FriEnd"])) + "</FridayEnd>";
			text = text + "<SaturdayStart>" + Function.ParseIntToTimeSpanString(Function.ParseInt(dataTable.Rows[0]["SatStart"])) + "</SaturdayStart>";
			text = text + "<SaturdayEnd>" + Function.ParseIntToTimeSpanString(Function.ParseInt(dataTable.Rows[0]["SatEnd"])) + "</SaturdayEnd>";
			text = text + "<SundayStart>" + Function.ParseIntToTimeSpanString(Function.ParseInt(dataTable.Rows[0]["SunStrart"])) + "</SundayStart>";
			text = text + "<SundayEnd>" + Function.ParseIntToTimeSpanString(Function.ParseInt(dataTable.Rows[0]["SunEnd"])) + "</SundayEnd>";
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["InclParts"])))
			{
				text = text + "<IncludeParts>" + Function.RawText(dataTable.Rows[0]["InclParts"]) + "</IncludeParts>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["InclWork"])))
			{
				text = text + "<IncludeLabor>" + Function.RawText(dataTable.Rows[0]["InclWork"]) + "</IncludeLabor>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["InclTravel"])))
			{
				text = text + "<IncludeTravel>" + Function.RawText(dataTable.Rows[0]["InclTravel"]) + "</IncludeTravel>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["Remarks1"])))
			{
				text = text + "<TemplateRemarks>" + Function.RawText(dataTable.Rows[0]["Remarks1"]) + "</TemplateRemarks>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["Remarks2"])))
			{
				text = text + "<Remarks>" + Function.RawText(dataTable.Rows[0]["Remarks2"]) + "</Remarks>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["InclHldays"])))
			{
				text = text + "<IncludeHolidays>" + Function.RawText(dataTable.Rows[0]["InclHldays"]) + "</IncludeHolidays>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["SrvcType"])))
			{
				text = text + "<ServiceType>" + Function.RawText(dataTable.Rows[0]["SrvcType"]) + "</ServiceType>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["ResponseU"])))
			{
				text = text + "<ResponseUnit>" + Function.RawText(dataTable.Rows[0]["ResponseU"]) + "</ResponseUnit>";
			}
			int num5 = Function.ParseInt(dataTable.Rows[0]["ResponseV"]);
			if (num5 > 0)
			{
				text = text + "<ResponseTime>" + num5 + "</ResponseTime>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["TermDate"])) && Function.ParseDateTimes(dataTable.Rows[0]["TermDate"]) > new DateTime(1900, 1, 1))
			{
				text = text + "<TerminationDate>" + Convert.ToDateTime(dataTable.Rows[0]["TermDate"]).ToString("yyyyMMdd") + "</TerminationDate>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["BPType"])))
			{
				text = text + "<ServiceBPType>" + Function.RawText(dataTable.Rows[0]["BPType"]) + "</ServiceBPType>";
			}
			IEnumerable<string> enumerable = from t in (from DataColumn c in dataTable.Columns
					select c.ColumnName).ToList()
				where t.StartsWith("U_")
				select t;
			if (enumerable != null && enumerable.ToList() != null && enumerable.ToList().Count > 0)
			{
				List<UserFieldsImpl> list = new List<UserFieldsImpl>();
				for (int num6 = 0; num6 < enumerable.ToList().Count; num6++)
				{
					UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
					if (dataTable.Columns[enumerable.ToList()[num6]].DataType == typeof(DateTime))
					{
						userFieldsImpl.Value = Function.ToString(Function.ParseDateTime(dataTable.Rows[0][enumerable.ToList()[num6]]));
						userFieldsImpl.Type = BoFieldTypes.db_Date;
					}
					else if (dataTable.Columns[enumerable.ToList()[num6]].DataType == typeof(double))
					{
						userFieldsImpl.Value = Function.ToString(Function.ParseDouble(dataTable.Rows[0][enumerable.ToList()[num6]]));
						userFieldsImpl.Type = BoFieldTypes.db_Numeric;
					}
					else
					{
						userFieldsImpl.Value = Function.ToString(Function.ToString(dataTable.Rows[0][enumerable.ToList()[num6]]));
						userFieldsImpl.Type = BoFieldTypes.db_Alpha;
					}
					userFieldsImpl.Name = enumerable.ToList()[num6];
					list.Add(userFieldsImpl);
				}
				text += DataService.MappingUdf(list);
			}
			text += "</row></ServiceContracts>";
			if (dataTable2 != null && dataTable2.Rows.Count > 0)
			{
				text += "<ServiceContract_Lines>";
				for (int num7 = 0; num7 < dataTable2.Rows.Count; num7++)
				{
					text += "<row>";
					text = text + "<LineNum>" + Function.ParseInt(dataTable2.Rows[num7]["Line"]) + "</LineNum>";
					if (Function.ParseInt(dataTable2.Rows[num7]["ManufSN"]) != 0)
					{
						text = text + "<ManufacturerSerialNum>" + Function.ParseInt(dataTable2.Rows[num7]["ManufSN"]) + "</ManufacturerSerialNum>";
					}
					if (Function.ParseInt(dataTable2.Rows[num7]["InternalSN"]) != 0)
					{
						text = text + "<InternalSerialNum>" + Function.ParseInt(dataTable2.Rows[num7]["InternalSN"]) + "</InternalSerialNum>";
					}
					if (!string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[num7]["ItemCode"])))
					{
						text = text + "<ItemCode>" + Function.RawText(dataTable2.Rows[num7]["ItemCode"]) + "</ItemCode>";
					}
					if (!string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[num7]["ItemName"])))
					{
						text = text + "<ItemName>" + Function.RawText(dataTable2.Rows[num7]["ItemName"]) + "</ItemName>";
					}
					int num8 = Function.ParseInt(dataTable2.Rows[num7]["ItemGroup"]);
					if (num8 != 0)
					{
						text = text + "<ItemGroup>" + num8 + "</ItemGroup>";
					}
					if (!string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[num7]["StartDate"])) && Function.ParseDateTimes(dataTable2.Rows[num7]["StartDate"]) > new DateTime(1900, 1, 1))
					{
						text = text + "<StartDate>" + Convert.ToDateTime(dataTable2.Rows[num7]["StartDate"]).ToString("yyyyMMdd") + "</StartDate>";
					}
					else if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["StartDate"])) && Function.ParseDateTimes(dataTable.Rows[0]["StartDate"]) > new DateTime(1900, 1, 1))
					{
						text = text + "<StartDate>" + Convert.ToDateTime(dataTable.Rows[0]["StartDate"]).ToString("yyyyMMdd") + "</StartDate>";
					}
					if (!string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[num7]["EndDate"])) && Function.ParseDateTimes(dataTable2.Rows[num7]["EndDate"]) > new DateTime(1900, 1, 1))
					{
						text = text + "<EndDate>" + Convert.ToDateTime(dataTable2.Rows[num7]["EndDate"]).ToString("yyyyMMdd") + "</EndDate>";
					}
					else if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["EndDate"])) && Function.ParseDateTimes(dataTable.Rows[0]["EndDate"]) > new DateTime(1900, 1, 1))
					{
						text = text + "<EndDate>" + Convert.ToDateTime(dataTable.Rows[0]["EndDate"]).ToString("yyyyMMdd") + "</EndDate>";
					}
					if (!string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[num7]["TermDate"])) && Function.ParseDateTimes(dataTable2.Rows[num7]["TermDate"]) > new DateTime(1900, 1, 1))
					{
						text = text + "<TerminationDate>" + Convert.ToDateTime(dataTable2.Rows[num7]["TermDate"]).ToString("yyyyMMdd") + "</TerminationDate>";
					}
					else if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["TermDate"])) && Function.ParseDateTimes(dataTable.Rows[0]["TermDate"]) > new DateTime(1900, 1, 1))
					{
						text = text + "<TerminationDate>" + Convert.ToDateTime(dataTable.Rows[0]["TermDate"]).ToString("yyyyMMdd") + "</TerminationDate>";
					}
					IEnumerable<string> enumerable2 = from t in (from DataColumn c in dataTable2.Columns
							select c.ColumnName).ToList()
						where t.StartsWith("U_")
						select t;
					if (enumerable2 != null && enumerable2.ToList() != null && enumerable2.ToList().Count > 0)
					{
						List<UserFieldsImpl> list2 = new List<UserFieldsImpl>();
						for (int num9 = 0; num9 < enumerable2.ToList().Count; num9++)
						{
							UserFieldsImpl userFieldsImpl2 = new UserFieldsImpl();
							if (dataTable2.Columns[enumerable2.ToList()[num9]].DataType == typeof(DateTime))
							{
								userFieldsImpl2.Value = Function.ToString(Function.ParseDateTime(dataTable2.Rows[num7][enumerable2.ToList()[num9]]));
								userFieldsImpl2.Type = BoFieldTypes.db_Date;
							}
							else if (dataTable2.Columns[enumerable2.ToList()[num9]].DataType == typeof(double))
							{
								userFieldsImpl2.Value = Function.ToString(Function.ParseDouble(dataTable2.Rows[num7][enumerable2.ToList()[num9]]));
								userFieldsImpl2.Type = BoFieldTypes.db_Numeric;
							}
							else
							{
								userFieldsImpl2.Value = Function.ToString(Function.ToString(dataTable2.Rows[num7][enumerable2.ToList()[num9]]));
								userFieldsImpl2.Type = BoFieldTypes.db_Alpha;
							}
							userFieldsImpl2.Name = enumerable2.ToList()[num9];
							list2.Add(userFieldsImpl2);
						}
						text += DataService.MappingUdf(list2);
					}
					text += "</row>";
				}
				text += "</ServiceContract_Lines>";
			}
			text += "</BO></BOM>";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return text;
	}

	protected bool Exist(int contractID, string sessionId, ApzObjectTypes objType)
	{
		try
		{
			return Function.ParseInt(ExecuteScarlar("select top 1 1 from OCTR where \"ContractID\" = @contractId;", CommandType.Text, new SqlParameter("@contractId", contractID))) == 1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return !contractID.Equals(0);
		}
	}

	protected string DeleteFile(int absEntry)
	{
		try
		{
			string query = "delete from ATC1 WHERE \"AbsEntry\" = @absEntry;";
			if (ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@absEntry", absEntry)) >= 0)
			{
				query = "delete from OATC WHERE \"AbsEntry\" = @absEntry;";
				if (ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@absEntry", absEntry)) >= 0)
				{
					return "";
				}
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}
}
