using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.UserFields;
using Apzon.Services;
using SAPbobsCOM;

namespace Apzon.DataServices.HanaImplements;

public class HanaServiceContractRepository : ServiceContractRepository
{
	public HanaServiceContractRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, unitOfWork, dataService)
	{
	}

	protected override HttpResult AddNew(DataSet data, string sessionId, ApzObjectTypes objType, int absEntry)
	{
		try
		{
			string xmlObject = Mapping(data, sessionId, EnumCommandType.Add, objType, absEntry);
			HttpResult httpResult = DataService.AddObject(objType, sessionId, xmlObject);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				if (absEntry > 0)
				{
					DeleteFile(absEntry);
				}
				return new HttpResult(Enums.MessageCode.Error, httpResult.Message);
			}
			int num = Function.ParseInt(httpResult.Content);
			Logging.Write("WATCH", $"Add new document - {objType} - DocEntry - {num}");
			return new HttpResult(Enums.MessageCode.Success, "", num);
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
	}

	protected override string UpdateData(DataSet data, string sessionId, ApzObjectTypes objType, int absEnty)
	{
		try
		{
			string xmlObject = Mapping(data, sessionId, EnumCommandType.Update, objType, absEnty);
			HttpResult httpResult = DataService.UpdateObject(objType, sessionId, xmlObject, Function.ParseInt(data.Tables[0].Rows[0]["ContractID"]));
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				return httpResult.Message;
			}
			int num = Function.ParseInt(httpResult.Content);
			Logging.Write("WATCH", $"update document - {objType} - DocEntry - {num}");
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override string Mapping(DataSet data, string sessionId, EnumCommandType commandType, ApzObjectTypes objType, int absEntry)
	{
		DataTable dataTable = data.Tables[0];
		DataTable dataTable2 = data.Tables[1];
		string text = "{\r\n";
		try
		{
			if (Function.ParseInt(dataTable.Rows[0]["ContractID"]) > 0)
			{
				text += Function.GetJsonString("ContractID", Function.ToString(dataTable.Rows[0]["ContractID"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["CstmrCode"])))
			{
				text += Function.GetJsonString("CustomerCode", Function.ToString(dataTable.Rows[0]["CstmrCode"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["CstmrName"])))
			{
				text += Function.GetJsonString("CustomerName", Function.ToString(dataTable.Rows[0]["CstmrName"]));
			}
			if (Function.ParseInt(dataTable.Rows[0]["CntctCode"]) != 0)
			{
				text += Function.GetJsonString("ContactCode", Function.ParseInt(dataTable.Rows[0]["CntctCode"]));
			}
			int num = Function.ParseInt(dataTable.Rows[0]["Owner"]);
			if (num != 0)
			{
				text += Function.GetJsonString("Owner", num);
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["Status"])))
			{
				text = ((string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["TermDate"])) || !(Function.ParseDateTimes(dataTable.Rows[0]["TermDate"]) > new DateTime(1900, 1, 1))) ? (text + Function.GetJsonString("Status", "scs_Approved")) : (text + Function.GetJsonString("Status", "T")));
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["CntrcTmplt"])))
			{
				text += Function.GetJsonString("ContractTemplate", Function.ToString(dataTable.Rows[0]["CntrcTmplt"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["CntrcType"])))
			{
				text += Function.GetJsonString("ContractType", "ct_Customer");
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["Renewal"])))
			{
				text += Function.GetJsonString("Renewal", (Function.ToString(dataTable.Rows[0]["Renewal"]) == "Y") ? "tYES" : "tNO");
			}
			int num2 = Function.ParseInt(dataTable.Rows[0]["RemindVal"]);
			text = ((num2 <= 0) ? (text + Function.GetJsonString("ReminderTime", string.Empty)) : (text + Function.GetJsonString("ReminderTime", num2)));
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["RemindUnit"])))
			{
				text += Function.GetJsonString("RemindUnit", "reu_Days");
			}
			int num3 = Function.ParseInt(dataTable.Rows[0]["Duration"]);
			if (num3 > 0)
			{
				text += Function.GetJsonString("DurationOfCoverage", num3);
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["StartDate"])) && Function.ParseDateTimes(dataTable.Rows[0]["StartDate"]) > new DateTime(1900, 1, 1))
			{
				text += Function.GetJsonString("StartDate", Function.ParseDateTimes(dataTable.Rows[0]["StartDate"]).ToString("yyyyMMdd"));
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["EndDate"])) && Function.ParseDateTimes(dataTable.Rows[0]["EndDate"]) > new DateTime(1900, 1, 1))
			{
				text += Function.GetJsonString("EndDate", Function.ParseDateTimes(dataTable.Rows[0]["EndDate"]).ToString("yyyyMMdd"));
			}
			int num4 = Function.ParseInt(dataTable.Rows[0]["ResponsVal"]);
			if (num4 > 0)
			{
				text += Function.GetJsonString("ResolutionTime", num4);
			}
			text = ((absEntry <= 0) ? (text + Function.GetJsonString("AttachmentEntry", null)) : (text + Function.GetJsonString("AttachmentEntry", absEntry)));
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["ResponsUnt"])))
			{
				text += Function.GetJsonString("ResolutionUnit", "rsu_Days");
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["Descriptio"])))
			{
				text += Function.GetJsonString("Description", Function.ToString(dataTable.Rows[0]["Descriptio"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["MonEnabled"])))
			{
				text += Function.GetJsonString("MondayEnabled", (Function.ToString(dataTable.Rows[0]["MonEnabled"]) == "Y") ? "tYES" : "tNO");
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["TueEnabled"])))
			{
				text += Function.GetJsonString("TuesdayEnabled", (Function.ToString(dataTable.Rows[0]["TueEnabled"]) == "Y") ? "tYES" : "tNO");
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["WedEnabled"])))
			{
				text += Function.GetJsonString("WednesdayEnabled", (Function.ToString(dataTable.Rows[0]["WedEnabled"]) == "Y") ? "tYES" : "tNO");
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["ThuEnabled"])))
			{
				text += Function.GetJsonString("ThursdayEnabled", (Function.ToString(dataTable.Rows[0]["ThuEnabled"]) == "Y") ? "tYES" : "tNO");
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["FriEnabled"])))
			{
				text += Function.GetJsonString("FridayEnabled", (Function.ToString(dataTable.Rows[0]["FriEnabled"]) == "Y") ? "tYES" : "tNO");
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["SatEnabled"])))
			{
				text += Function.GetJsonString("SaturdayEnabled", (Function.ToString(dataTable.Rows[0]["SatEnabled"]) == "Y") ? "tYES" : "tNO");
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["SunEnabled"])))
			{
				text += Function.GetJsonString("SundayEnabled", (Function.ToString(dataTable.Rows[0]["SunEnabled"]) == "Y") ? "tYES" : "tNO");
			}
			text += Function.GetJsonString("MondayStart", Function.ParseIntToTimeSpanString(Function.ParseInt(dataTable.Rows[0]["MonStart"])));
			text += Function.GetJsonString("MondayEnd", Function.ParseIntToTimeSpanString(Function.ParseInt(dataTable.Rows[0]["MonEnd"])));
			text += Function.GetJsonString("TuesdayStart", Function.ParseIntToTimeSpanString(Function.ParseInt(dataTable.Rows[0]["TueStart"])));
			text += Function.GetJsonString("TuesdayEnd", Function.ParseIntToTimeSpanString(Function.ParseInt(dataTable.Rows[0]["TueEnd"])));
			text += Function.GetJsonString("WednesdayStart", Function.ParseIntToTimeSpanString(Function.ParseInt(dataTable.Rows[0]["WedStart"])));
			text += Function.GetJsonString("WednesdayEnd", Function.ParseIntToTimeSpanString(Function.ParseInt(dataTable.Rows[0]["WedEnd"])));
			text += Function.GetJsonString("ThursdayStart", Function.ParseIntToTimeSpanString(Function.ParseInt(dataTable.Rows[0]["ThuStart"])));
			text += Function.GetJsonString("ThursdayEnd", Function.ParseIntToTimeSpanString(Function.ParseInt(dataTable.Rows[0]["ThuEnd"])));
			text += Function.GetJsonString("FridayStart", Function.ParseIntToTimeSpanString(Function.ParseInt(dataTable.Rows[0]["FriStart"])));
			text += Function.GetJsonString("FridayEnd", Function.ParseIntToTimeSpanString(Function.ParseInt(dataTable.Rows[0]["FriEnd"])));
			text += Function.GetJsonString("SaturdayStart", Function.ParseIntToTimeSpanString(Function.ParseInt(dataTable.Rows[0]["SatStart"])));
			text += Function.GetJsonString("SaturdayEnd", Function.ParseIntToTimeSpanString(Function.ParseInt(dataTable.Rows[0]["SatEnd"])));
			text += Function.GetJsonString("SundayStart", Function.ParseIntToTimeSpanString(Function.ParseInt(dataTable.Rows[0]["SunStrart"])));
			text += Function.GetJsonString("SundayEnd", Function.ParseIntToTimeSpanString(Function.ParseInt(dataTable.Rows[0]["SunEnd"])));
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["InclParts"])))
			{
				text += Function.GetJsonString("IncludeParts", (Function.ToString(dataTable.Rows[0]["InclParts"]) == "Y") ? "tYES" : "tNO");
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["InclWork"])))
			{
				text += Function.GetJsonString("IncludeLabor", (Function.ToString(dataTable.Rows[0]["InclWork"]) == "Y") ? "tYES" : "tNO");
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["InclTravel"])))
			{
				text += Function.GetJsonString("IncludeTravel", (Function.ToString(dataTable.Rows[0]["InclTravel"]) == "Y") ? "tYES" : "tNO");
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["Remarks1"])))
			{
				text += Function.GetJsonString("TemplateRemarks", Function.ToString(dataTable.Rows[0]["Remarks1"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["Remarks2"])))
			{
				text += Function.GetJsonString("Remarks", Function.ToString(dataTable.Rows[0]["Remarks2"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["InclHldays"])))
			{
				text += Function.GetJsonString("IncludeHolidays", (Function.ToString(dataTable.Rows[0]["InclHldays"]) == "Y") ? "tYES" : "tNO");
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["SrvcType"])))
			{
				text += Function.GetJsonString("ServiceType", "bst_Regular");
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["ResponseU"])))
			{
				text += Function.GetJsonString("ResponseUnit", "boru_Day");
			}
			int num5 = Function.ParseInt(dataTable.Rows[0]["ResponseV"]);
			if (num5 > 0)
			{
				text += Function.GetJsonString("ResponseTime", num5);
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["TermDate"])) && Function.ParseDateTimes(dataTable.Rows[0]["TermDate"]) > new DateTime(1900, 1, 1))
			{
				text += Function.GetJsonString("TerminationDate", Function.ParseDateTimes(dataTable.Rows[0]["TermDate"]).ToString("yyyyMMdd"));
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["BPType"])))
			{
				text += Function.GetJsonString("ServiceBPType", Function.ToString(dataTable.Rows[0]["BPType"]));
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
			if (dataTable2 != null && dataTable2.Rows.Count > 0)
			{
				text += "\"ServiceContract_Lines\":[\r\n";
				for (int num7 = 0; num7 < dataTable2.Rows.Count; num7++)
				{
					text += "{";
					text += Function.GetJsonString("LineNum", Function.ParseInt(dataTable2.Rows[num7]["Line"]));
					if (Function.ParseInt(dataTable2.Rows[num7]["ManufSN"]) != 0)
					{
						text += Function.GetJsonString("ManufacturerSerialNum", Function.ParseInt(dataTable2.Rows[num7]["ManufSN"]));
					}
					if (Function.ParseInt(dataTable2.Rows[num7]["InternalSN"]) != 0)
					{
						text += Function.GetJsonString("InternalSerialNum", Function.ParseInt(dataTable2.Rows[num7]["InternalSN"]));
					}
					if (!string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[num7]["ItemCode"])))
					{
						text += Function.GetJsonString("ItemCode", Function.ToString(dataTable2.Rows[num7]["ItemCode"]));
					}
					if (!string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[num7]["ItemName"])))
					{
						text += Function.GetJsonString("ItemName", Function.ToString(dataTable2.Rows[num7]["ItemName"]));
					}
					int num8 = Function.ParseInt(dataTable2.Rows[num7]["ItemGroup"]);
					if (num8 != 0)
					{
						text += Function.GetJsonString("ItemGroup", num8);
					}
					if (!string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[num7]["StartDate"])) && Function.ParseDateTimes(dataTable2.Rows[num7]["StartDate"]) > new DateTime(1900, 1, 1))
					{
						text += Function.GetJsonString("StartDate", Function.ParseDateTimes(dataTable2.Rows[num7]["StartDate"]).ToString("yyyyMMdd"));
					}
					else if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["StartDate"])) && Function.ParseDateTimes(dataTable.Rows[0]["StartDate"]) > new DateTime(1900, 1, 1))
					{
						text += Function.GetJsonString("StartDate", Function.ParseDateTimes(dataTable.Rows[0]["StartDate"]).ToString("yyyyMMdd"));
					}
					if (!string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[num7]["EndDate"])) && Function.ParseDateTimes(dataTable2.Rows[num7]["EndDate"]) > new DateTime(1900, 1, 1))
					{
						text += Function.GetJsonString("EndDate", Function.ParseDateTimes(dataTable2.Rows[num7]["EndDate"]).ToString("yyyyMMdd"));
					}
					else if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["EndDate"])) && Function.ParseDateTimes(dataTable.Rows[0]["EndDate"]) > new DateTime(1900, 1, 1))
					{
						text += Function.GetJsonString("EndDate", Function.ParseDateTimes(dataTable.Rows[0]["EndDate"]).ToString("yyyyMMdd"));
					}
					if (!string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[num7]["TermDate"])) && Function.ParseDateTimes(dataTable2.Rows[num7]["TermDate"]) > new DateTime(1900, 1, 1))
					{
						text += Function.GetJsonString("TerminationDate", Function.ParseDateTimes(dataTable2.Rows[num7]["TermDate"]).ToString("yyyyMMdd"));
					}
					else if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["TermDate"])) && Function.ParseDateTimes(dataTable.Rows[0]["TermDate"]) > new DateTime(1900, 1, 1))
					{
						text += Function.GetJsonString("TerminationDate", Function.ParseDateTimes(dataTable.Rows[0]["TermDate"]).ToString("yyyyMMdd"));
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
					text += "},";
				}
				text += "]";
			}
			text += "}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return text;
	}
}
