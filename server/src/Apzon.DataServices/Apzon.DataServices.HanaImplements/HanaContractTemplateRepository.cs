using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Production;
using Apzon.Entities.Models.UserFields;
using Apzon.Services;
using SAPbobsCOM;

namespace Apzon.DataServices.HanaImplements;

public class HanaContractTemplateRepository : ContractTemplateRepository
{
	public HanaContractTemplateRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, unitOfWork, dataService)
	{
	}

	protected override HttpResult SaveAttachment(List<AttachmentFile_Impl> lstFile, int absEntry, string tmplName, string sessionId, bool addNew)
	{
		if (addNew || absEntry <= 0)
		{
			return _unitOfWork.Attachment2.Add(lstFile, sessionId);
		}
		return _unitOfWork.Attachment2.Update(absEntry, lstFile, sessionId);
	}

	public override string SaveDoc(DataSet ds, string sessionId)
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
			string xmlObject = Mapping(ds.Tables[0].Rows[0]);
			return ((text == "A") ? DataService.AddObject(ApzObjectTypes.oContractTemplates, sessionId, xmlObject) : DataService.UpdateObject(ApzObjectTypes.oContractTemplates, sessionId, xmlObject)).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string Mapping(DataRow row)
	{
		try
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("{");
			stringBuilder.Append(Function.GetJsonString("TemplateName", Function.ToString(row["TmpltName"])));
			if (Function.ToString(row["Deleted"]) == "Y")
			{
				stringBuilder.Append(Function.GetJsonString("TemplateIsDeleted", "tYES"));
			}
			else
			{
				stringBuilder.Append(Function.GetJsonString("TemplateIsDeleted", "tNO"));
			}
			if (Function.ToString(row["Renewal"]) == "Y")
			{
				stringBuilder.Append(Function.GetJsonString("TemplateIsRenewal", "tYES"));
			}
			else
			{
				stringBuilder.Append(Function.GetJsonString("TemplateIsRenewal", "tNO"));
			}
			stringBuilder.Append(Function.GetJsonString("RemindBeforeRenewal", Function.ToString(row["RemindVal"])));
			switch (Function.ToString(row["RemindUnit"]))
			{
			case "D":
				stringBuilder.Append(Function.GetJsonString("RemindUnit", "reu_Days"));
				break;
			case "W":
				stringBuilder.Append(Function.GetJsonString("RemindUnit", "reu_Weeks"));
				break;
			case "M":
				stringBuilder.Append(Function.GetJsonString("RemindUnit", "reu_Month"));
				break;
			}
			stringBuilder.Append(Function.GetJsonString("DurationOfCoverage", Function.ToString(row["Duration"])));
			stringBuilder.Append(Function.GetJsonString("ResolutionTime", Function.ToString(row["ResponsVal"])));
			string text = Function.ToString(row["ResponsUnt"]);
			if (!(text == "D"))
			{
				if (text == "H")
				{
					stringBuilder.Append(Function.GetJsonString("ResolutionUnit", "rsu_Hours"));
				}
			}
			else
			{
				stringBuilder.Append(Function.GetJsonString("ResolutionUnit", "rsu_Days"));
			}
			stringBuilder.Append(Function.GetJsonString("Description", Function.ToString(row["Descriptio"])));
			switch (Function.ToString(row["CntrctType"]))
			{
			case "C":
				stringBuilder.Append(Function.GetJsonString("ContractType", "ct_Customer"));
				break;
			case "G":
				stringBuilder.Append(Function.GetJsonString("ContractType", "ct_ItemGroup"));
				break;
			case "S":
				stringBuilder.Append(Function.GetJsonString("ContractType", "ct_SerialNumber"));
				break;
			}
			if (Function.ToString(row["MonEnabled"]) == "Y")
			{
				stringBuilder.Append(Function.GetJsonString("MondayEnabled", "tYES"));
			}
			else
			{
				stringBuilder.Append(Function.GetJsonString("MondayEnabled", "tNO"));
			}
			if (Function.ToString(row["TueEnabled"]) == "Y")
			{
				stringBuilder.Append(Function.GetJsonString("TuesdayEnabled", "tYES"));
			}
			else
			{
				stringBuilder.Append(Function.GetJsonString("TuesdayEnabled", "tNO"));
			}
			if (Function.ToString(row["WedEnabled"]) == "Y")
			{
				stringBuilder.Append(Function.GetJsonString("WednesdayEnabled", "tYES"));
			}
			else
			{
				stringBuilder.Append(Function.GetJsonString("WednesdayEnabled", "tNO"));
			}
			if (Function.ToString(row["ThuEnabled"]) == "Y")
			{
				stringBuilder.Append(Function.GetJsonString("ThursdayEnabled", "tYES"));
			}
			else
			{
				stringBuilder.Append(Function.GetJsonString("ThursdayEnabled", "tNO"));
			}
			if (Function.ToString(row["FriEnabled"]) == "Y")
			{
				stringBuilder.Append(Function.GetJsonString("FridayEnabled", "tYES"));
			}
			else
			{
				stringBuilder.Append(Function.GetJsonString("FridayEnabled", "tNO"));
			}
			if (Function.ToString(row["SatEnabled"]) == "Y")
			{
				stringBuilder.Append(Function.GetJsonString("SaturdayEnabled", "tYES"));
			}
			else
			{
				stringBuilder.Append(Function.GetJsonString("SaturdayEnabled", "tNO"));
			}
			if (Function.ToString(row["SunEnabled"]) == "Y")
			{
				stringBuilder.Append(Function.GetJsonString("SundayEnabled", "tYES"));
			}
			else
			{
				stringBuilder.Append(Function.GetJsonString("SaturdayEnabled", "tNO"));
			}
			stringBuilder.Append(Function.GetJsonString("MondayStart", Function.ToString(row["MonStart"])));
			stringBuilder.Append(Function.GetJsonString("MondayEnd", Function.ToString(row["MonEnd"])));
			stringBuilder.Append(Function.GetJsonString("TuesdayStart", Function.ToString(row["TueStart"])));
			stringBuilder.Append(Function.GetJsonString("TuesdayEnd", Function.ToString(row["TueEnd"])));
			stringBuilder.Append(Function.GetJsonString("WednesdayStart", Function.ToString(row["WedStart"])));
			stringBuilder.Append(Function.GetJsonString("WednesdayEnd", Function.ToString(row["WedEnd"])));
			stringBuilder.Append(Function.GetJsonString("ThursdayStart", Function.ToString(row["ThuStart"])));
			stringBuilder.Append(Function.GetJsonString("ThursdayEnd", Function.ToString(row["ThuEnd"])));
			stringBuilder.Append(Function.GetJsonString("FridayStart", Function.ToString(row["FriStart"])));
			stringBuilder.Append(Function.GetJsonString("FridayEnd", Function.ToString(row["FriEnd"])));
			stringBuilder.Append(Function.GetJsonString("SaturdayStart", Function.ToString(row["SatStart"])));
			stringBuilder.Append(Function.GetJsonString("SaturdayEnd", Function.ToString(row["SatEnd"])));
			stringBuilder.Append(Function.GetJsonString("SundayStart", Function.ToString(row["SunStrart"])));
			stringBuilder.Append(Function.GetJsonString("SundayEnd", Function.ToString(row["SunEnd"])));
			if (Function.ToString(row["InclParts"]) == "Y")
			{
				stringBuilder.Append(Function.GetJsonString("IncludeParts", "tYES"));
			}
			else
			{
				stringBuilder.Append(Function.GetJsonString("IncludeParts", "tNO"));
			}
			if (Function.ToString(row["InclWork"]) == "Y")
			{
				stringBuilder.Append(Function.GetJsonString("IncludeLabor", "tYES"));
			}
			else
			{
				stringBuilder.Append(Function.GetJsonString("IncludeLabor", "tNO"));
			}
			if (Function.ToString(row["InclTravel"]) == "Y")
			{
				stringBuilder.Append(Function.GetJsonString("IncludeTravel", "tYES"));
			}
			else
			{
				stringBuilder.Append(Function.GetJsonString("IncludeTravel", "tNO"));
			}
			stringBuilder.Append(Function.GetJsonString("Remarks", Function.ToString(row["Remark"])));
			if (Function.ToString(row["InclHldays"]) == "Y")
			{
				stringBuilder.Append(Function.GetJsonString("IncludeHolidays", "tYES"));
			}
			else
			{
				stringBuilder.Append(Function.GetJsonString("IncludeHolidays", "tNO"));
			}
			stringBuilder.Append(Function.GetJsonString("ResponseValue", Function.ToString(row["ResponseV"])));
			text = Function.ToString(row["ResponseU"]);
			if (!(text == "H"))
			{
				if (text == "D")
				{
					stringBuilder.Append(Function.GetJsonString("ResponseUnit", "boru_Day"));
				}
			}
			else
			{
				stringBuilder.Append(Function.GetJsonString("ResponseUnit", "boru_Hour"));
			}
			if (Function.ParseInt(row["AtcEntry"]) > 0)
			{
				stringBuilder.Append(Function.GetJsonString("AttachmentEntry", Function.ParseInt(row["AtcEntry"])));
			}
			else
			{
				stringBuilder.Append(Function.GetJsonString("AttachmentEntry", null));
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
			stringBuilder.Append("}");
			return stringBuilder.ToString();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "";
		}
	}
}
