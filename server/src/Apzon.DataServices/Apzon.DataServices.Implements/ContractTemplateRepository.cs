using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Production;
using Apzon.Entities.Models.UserFields;
using Apzon.Services;
using SAPbobsCOM;

namespace Apzon.DataServices.Implements;

public class ContractTemplateRepository : Repository<DataTable>, IContractTemplate, IDisposable
{
	protected readonly IUnitOfWork _unitOfWork;

	public ContractTemplateRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
		_unitOfWork = unitOfWork;
	}

	public void Dispose()
	{
		Dispose();
	}

	protected virtual HttpResult SaveAttachment(List<AttachmentFile_Impl> lstFile, int absEntry, string tmplName, string sessionId, bool addNew)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("TmpltName,string", tmplName);
		return ServiceUnitOfWork.Files.SaveAttachmentFiles(lstFile, dictionary, "OCTT");
	}

	public virtual string SaveDoc(DataSet ds, string sessionId)
	{
		try
		{
			if (ds == null || ds.Tables.Count != 2 || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
			{
				return "No row to save";
			}
			string text = Function.ToString(ds.Tables[0].Rows[0]["SaveAction"]);
			List<AttachmentFile_Impl> list = ds.Tables[1].ToAttachmentFile();
			if (list != null && list.Count != 0)
			{
				HttpResult httpResult = SaveAttachment(list, Function.ParseInt(ds.Tables[0].Rows[0]["AtcEntry"]), Function.ToString(ds.Tables[0].Rows[0]["TmpltName"]), sessionId, text == "A");
				if (httpResult.MessageCode != Enums.MessageCode.Success)
				{
					return httpResult.Message;
				}
				ds.Tables[0].Rows[0]["AtcEntry"] = Function.ParseInt(httpResult.Content);
			}
			string value = ((text == "A") ? Mapping(ds.Tables[0].Rows[0], EnumCommandType.Add, sessionId) : Mapping(ds.Tables[0].Rows[0], EnumCommandType.Update, sessionId));
			StringBuilder stringBuilder = new StringBuilder("<?xml version=\"1.0\" encoding=\"UTF-16\"?>");
			stringBuilder.Append("<env:Envelope xmlns:env = \"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                <env:Header>\r\n                                 <SessionID>" + sessionId + "</SessionID></env:Header><env:Body>");
			stringBuilder.Append(value);
			stringBuilder.Append("</env:Body></env:Envelope>");
			XmlElement xmlElement = DataService.Process(stringBuilder.ToString()).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					if (text == "A")
					{
						Logging.Write("ERROR", "Add Contract template error: " + xElement2.Value);
					}
					else
					{
						Logging.Write("ERROR", "Update Contract template error: " + xElement2.Value);
					}
					return xElement2.Value;
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					int num = Function.ParseInt(xElement3.Value);
					if (text == "A")
					{
						Logging.Write("WATCH", $"Add Contract template - {num}");
					}
					else
					{
						Logging.Write("WATCH", $"Update Contract template - {num}");
					}
					return string.Empty;
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

	private string Mapping(DataRow row, EnumCommandType commandType, string sessionId)
	{
		try
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (commandType == EnumCommandType.Update)
			{
				stringBuilder.Append("<dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">");
			}
			else
			{
				stringBuilder.Append("<dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">");
			}
			stringBuilder.Append("<BOM>\r\n                          <BO>\r\n                          <AdmInfo>\r\n                            <Object>oContractTemplates</Object>\r\n                          </AdmInfo> ");
			if (commandType == EnumCommandType.Update)
			{
				stringBuilder.Append("<QueryParams><TemplateName>" + Function.ToString(row["OldName"]) + "</TemplateName></QueryParams>");
			}
			stringBuilder.Append("<ContractTemplates>\r\n                                <row> ");
			stringBuilder.Append("<TemplateName>" + Function.ToString(row["TmpltName"]) + "</TemplateName>");
			if (Function.ToString(row["Deleted"]) == "Y")
			{
				stringBuilder.Append("<TemplateIsDeleted>tYES</TemplateIsDeleted>");
			}
			else
			{
				stringBuilder.Append("<TemplateIsDeleted>tNO</TemplateIsDeleted>");
			}
			if (Function.ToString(row["Renewal"]) == "Y")
			{
				stringBuilder.Append("<TemplateIsRenewal>tYES</TemplateIsRenewal>");
			}
			else
			{
				stringBuilder.Append("<TemplateIsRenewal>tNO</TemplateIsRenewal>");
			}
			stringBuilder.Append("<RemindBeforeRenewal>" + Function.ToString(row["RemindVal"]) + "</RemindBeforeRenewal>");
			switch (Function.ToString(row["RemindUnit"]))
			{
			case "D":
				stringBuilder.Append("<RemindUnit>reu_Days</RemindUnit>");
				break;
			case "W":
				stringBuilder.Append("<RemindUnit>reu_Weeks</RemindUnit>");
				break;
			case "M":
				stringBuilder.Append("<RemindUnit>reu_Month</RemindUnit>");
				break;
			}
			stringBuilder.Append("<DurationOfCoverage>" + Function.ToString(row["Duration"]) + "</DurationOfCoverage>");
			stringBuilder.Append("<ResolutionTime>" + Function.ToString(row["ResponsVal"]) + "</ResolutionTime>");
			string text = Function.ToString(row["ResponsUnt"]);
			if (!(text == "D"))
			{
				if (text == "H")
				{
					stringBuilder.Append("<ResolutionUnit>rsu_Hours</ResolutionUnit>");
				}
			}
			else
			{
				stringBuilder.Append("<ResolutionUnit>rsu_Days</ResolutionUnit>");
			}
			stringBuilder.Append("<Description>" + Function.RawText(row["Descriptio"]) + "</Description>");
			switch (Function.ToString(row["CntrctType"]))
			{
			case "C":
				stringBuilder.Append("<ContractType>ct_Customer</ContractType>");
				break;
			case "G":
				stringBuilder.Append("<ContractType>ct_ItemGroup</ContractType>");
				break;
			case "S":
				stringBuilder.Append("<ContractType>ct_SerialNumber</ContractType>");
				break;
			}
			if (Function.ToString(row["MonEnabled"]) == "Y")
			{
				stringBuilder.Append("<MondayEnabled>tYES</MondayEnabled>");
			}
			else
			{
				stringBuilder.Append("<MondayEnabled>tNO</MondayEnabled>");
			}
			if (Function.ToString(row["TueEnabled"]) == "Y")
			{
				stringBuilder.Append("<TuesdayEnabled>tYES</TuesdayEnabled>");
			}
			else
			{
				stringBuilder.Append("<TuesdayEnabled>tNO</TuesdayEnabled>");
			}
			if (Function.ToString(row["WedEnabled"]) == "Y")
			{
				stringBuilder.Append("<WednesdayEnabled>tYES</WednesdayEnabled>");
			}
			else
			{
				stringBuilder.Append("<WednesdayEnabled>tNO</WednesdayEnabled>");
			}
			if (Function.ToString(row["ThuEnabled"]) == "Y")
			{
				stringBuilder.Append("<ThursdayEnabled>tYES</ThursdayEnabled>");
			}
			else
			{
				stringBuilder.Append("<ThursdayEnabled>tNO</ThursdayEnabled>");
			}
			if (Function.ToString(row["FriEnabled"]) == "Y")
			{
				stringBuilder.Append("<FridayEnabled>tYES</FridayEnabled>");
			}
			else
			{
				stringBuilder.Append("<FridayEnabled>tNO</FridayEnabled>");
			}
			if (Function.ToString(row["SatEnabled"]) == "Y")
			{
				stringBuilder.Append("<SaturdayEnabled>tYES</SaturdayEnabled>");
			}
			else
			{
				stringBuilder.Append("<SaturdayEnabled>tNO</SaturdayEnabled>");
			}
			if (Function.ToString(row["SunEnabled"]) == "Y")
			{
				stringBuilder.Append("<SundayEnabled>tYES</SundayEnabled>");
			}
			else
			{
				stringBuilder.Append("<SaturdayEnabled>tNO</SaturdayEnabled>");
			}
			stringBuilder.Append("<MondayStart>" + Function.ToString(row["MonStart"]) + "</MondayStart>");
			stringBuilder.Append("<MondayEnd>" + Function.ToString(row["MonEnd"]) + "</MondayEnd>");
			stringBuilder.Append("<TuesdayStart>" + Function.ToString(row["TueStart"]) + "</TuesdayStart>");
			stringBuilder.Append("<TuesdayEnd>" + Function.ToString(row["TueEnd"]) + "</TuesdayEnd>");
			stringBuilder.Append("<WednesdayStart>" + Function.ToString(row["WedStart"]) + "</WednesdayStart>");
			stringBuilder.Append("<WednesdayEnd>" + Function.ToString(row["WedEnd"]) + "</WednesdayEnd>");
			stringBuilder.Append("<ThursdayStart>" + Function.ToString(row["ThuStart"]) + "</ThursdayStart>");
			stringBuilder.Append("<ThursdayEnd>" + Function.ToString(row["ThuEnd"]) + "</ThursdayEnd>");
			stringBuilder.Append("<FridayStart>" + Function.ToString(row["FriStart"]) + "</FridayStart>");
			stringBuilder.Append("<FridayEnd>" + Function.ToString(row["FriEnd"]) + "</FridayEnd>");
			stringBuilder.Append("<SaturdayStart>" + Function.ToString(row["SatStart"]) + "</SaturdayStart>");
			stringBuilder.Append("<SaturdayEnd>" + Function.ToString(row["SatEnd"]) + "</SaturdayEnd>");
			stringBuilder.Append("<SundayStart>" + Function.ToString(row["SunStrart"]) + "</SundayStart>");
			stringBuilder.Append("<SundayEnd>" + Function.ToString(row["SunEnd"]) + "</SundayEnd>");
			if (Function.ToString(row["InclParts"]) == "Y")
			{
				stringBuilder.Append("<IncludeParts>tYES</IncludeParts>");
			}
			else
			{
				stringBuilder.Append("<IncludeParts>tNO</IncludeParts>");
			}
			if (Function.ToString(row["InclWork"]) == "Y")
			{
				stringBuilder.Append("<IncludeLabor>tYES</IncludeLabor>");
			}
			else
			{
				stringBuilder.Append("<IncludeLabor>tNO</IncludeLabor>");
			}
			if (Function.ToString(row["InclTravel"]) == "Y")
			{
				stringBuilder.Append("<IncludeTravel>tYES</IncludeTravel>");
			}
			else
			{
				stringBuilder.Append("<IncludeTravel>tNO</IncludeTravel>");
			}
			stringBuilder.Append("<Remarks>" + Function.ToString(row["Remark"]) + "</Remarks>");
			if (Function.ToString(row["InclHldays"]) == "Y")
			{
				stringBuilder.Append("<IncludeHolidays>tYES</IncludeHolidays>");
			}
			else
			{
				stringBuilder.Append("<IncludeHolidays>tNO</IncludeHolidays>");
			}
			stringBuilder.Append("<ResponseValue>" + Function.ToString(row["ResponseV"]) + "</ResponseValue>");
			text = Function.ToString(row["ResponseU"]);
			if (!(text == "H"))
			{
				if (text == "D")
				{
					stringBuilder.Append("<ResponseUnit>boru_Day</ResponseUnit>");
				}
			}
			else
			{
				stringBuilder.Append("<ResponseUnit>boru_Hour</ResponseUnit>");
			}
			List<UserFieldsImpl> listField = _unitOfWork.Common.GetListField("OCTT");
			for (int i = 0; i < listField.Count; i++)
			{
				if (!row.Table.Columns.Contains(listField[i].Name))
				{
					continue;
				}
				switch (listField[i].Type)
				{
				case BoFieldTypes.db_Alpha:
					listField[i].Value = Function.ToString(row[listField[i].Name]);
					break;
				case BoFieldTypes.db_Memo:
					listField[i].Value = Function.ToString(row[listField[i].Name]);
					break;
				case BoFieldTypes.db_Numeric:
					if (listField[i].SubType == BoFldSubTypes.st_Time)
					{
						listField[i].Value = Function.ParseInt(row[listField[i].Name]).ToString("0000");
					}
					else
					{
						listField[i].Value = Function.ParseInt(row[listField[i].Name]);
					}
					break;
				case BoFieldTypes.db_Date:
					listField[i].Value = Function.ParseDateTime(row[listField[i].Name]);
					break;
				case BoFieldTypes.db_Float:
					listField[i].Value = Function.ParseDouble(row[listField[i].Name]);
					break;
				}
			}
			stringBuilder.Append(DataService.MappingUdf(listField));
			stringBuilder.Append("</row>\r\n                            </ContractTemplates>");
			stringBuilder.Append(" </BO>\r\n                          </BOM>");
			if (commandType == EnumCommandType.Update)
			{
				stringBuilder.Append(" </dis:UpdateObject>");
			}
			else
			{
				stringBuilder.Append("</dis:AddObject>");
			}
			return stringBuilder.ToString();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "";
		}
	}
}
