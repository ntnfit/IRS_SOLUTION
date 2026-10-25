using System;
using System.Data;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Services;
using SAPbobsCOM;

namespace Apzon.DataServices.HanaImplements;

public class HanaInventoryCyclesRepository : InventoryCyclesRepository
{
	public HanaInventoryCyclesRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	public override HttpResult Add(DataTable dtSave, string sessionId)
	{
		if (!dtSave.IsNotNull())
		{
			return new HttpResult(Enums.MessageCode.DataIsEmpty, "Save data cannot be empty");
		}
		return DataService.AddObject(ApzObjectTypes.oInventoryCycles, sessionId, Mapping(dtSave.Rows[0]));
	}

	public override HttpResult Delete(DataTable dtSave, string sessionId)
	{
		if (!dtSave.IsNotNull())
		{
			return new HttpResult(Enums.MessageCode.DataIsEmpty, "Save data cannot be empty");
		}
		return DataService.RemoveObject(ApzObjectTypes.oInventoryCycles, sessionId, Function.ParseInt(dtSave.Rows[0]["Code"]));
	}

	public override HttpResult Update(DataTable dtSave, string sessionId)
	{
		if (!dtSave.IsNotNull())
		{
			return new HttpResult(Enums.MessageCode.DataIsEmpty, "Save data cannot be empty");
		}
		return DataService.UpdateObject(ApzObjectTypes.oInventoryCycles, sessionId, Mapping(dtSave.Rows[0]), Function.ParseInt(dtSave.Rows[0]["Code"]));
	}

	private string Mapping(DataRow row)
	{
		string text = "{";
		if (!string.IsNullOrEmpty(Function.ToString(row["Code"])))
		{
			text += Function.GetJsonString("CycleCode", Function.ToString(row["Code"]));
		}
		text += Function.GetJsonString("CycleName", Function.ToString(row["Name"]));
		if (Function.ParseInt(row["Day"]) > 0)
		{
			text += Function.GetJsonString("Day", Function.ParseInt(row["Day"]));
		}
		string text2 = Function.ToString(row["EndType"]);
		if (text2 == "C")
		{
			text += Function.GetJsonString("endType", EndTypeEnum.etByCounter.ToString());
		}
		else
		{
			text = ((!(text2 == "D")) ? (text + Function.GetJsonString("endType", EndTypeEnum.etNoEndDate.ToString())) : (text + Function.GetJsonString("endType", EndTypeEnum.etByDate.ToString())));
		}
		string text3 = Function.ToString(row["Frequency"]);
		switch (text3)
		{
		case "0":
			text += Function.GetJsonString("Frequency", BoFrequency.bof_Daily.ToString());
			break;
		case "1":
			text += Function.GetJsonString("Frequency", BoFrequency.bof_Weekly.ToString());
			break;
		case "2":
			text += Function.GetJsonString("Frequency", BoFrequency.bof_Every4Weeks.ToString());
			break;
		case "3":
			text += Function.GetJsonString("Frequency", BoFrequency.bof_Monthly.ToString());
			break;
		case "4":
			text += Function.GetJsonString("Frequency", BoFrequency.bof_Quarterly.ToString());
			break;
		case "5":
			text += Function.GetJsonString("Frequency", BoFrequency.bof_HalfYearly.ToString());
			break;
		case "6":
			text += Function.GetJsonString("Frequency", BoFrequency.bof_Annually.ToString());
			break;
		case "7":
			text += Function.GetJsonString("Frequency", BoFrequency.bof_OneTime.ToString());
			break;
		case "8":
			text += Function.GetJsonString("Frequency", BoFrequency.bof_EveryXDays.ToString());
			break;
		}
		text = ((!(Function.ToString(row["Friday"]) == "Y")) ? (text + Function.GetJsonString("Friday", BoYesNoEnum.tNO.ToString())) : (text + Function.GetJsonString("Friday", BoYesNoEnum.tYES.ToString())));
		if (Function.ParseInt(row["Hour"]) > 0)
		{
			text += Function.GetJsonString("Hour", Function.ParseInt(row["Hour"]));
		}
		if (Function.ParseInt(row["Interval"]) > 0)
		{
			text += Function.GetJsonString("Interval", Function.ParseInt(row["Interval"]));
		}
		if (Function.ParseInt(row["MaxOccur"]) > 0)
		{
			text += Function.GetJsonString("MaxOccurrence", Function.ParseInt(row["MaxOccur"]));
		}
		text = ((!(Function.ToString(row["Monday"]) == "Y")) ? (text + Function.GetJsonString("Monday", BoYesNoEnum.tNO.ToString())) : (text + Function.GetJsonString("Monday", BoYesNoEnum.tYES.ToString())));
		if (text3 == "0" || text3 == "7")
		{
			DateTime? dateTime = Function.ParseDateTime(row["NextDate"]);
			if (dateTime.HasValue)
			{
				text += Function.GetJsonString("NextCountingDate", dateTime.Value.ToString("yyyyMMdd"));
			}
		}
		if (Function.ParseInt(row["DayInMonth"]) > 0)
		{
			text += Function.GetJsonString("RecurrenceDayInMonth", Function.ParseInt(row["DayInMonth"]));
		}
		switch (Function.ParseInt(row["DayOfWeek"]))
		{
		case 0:
			text += Function.GetJsonString("RecurrenceDayOfWeek", RecurrenceDayOfWeekEnum.rdowDay.ToString());
			break;
		case 1:
			text += Function.GetJsonString("RecurrenceDayOfWeek", RecurrenceDayOfWeekEnum.rdowWeekDay.ToString());
			break;
		case 2:
			text += Function.GetJsonString("RecurrenceDayOfWeek", RecurrenceDayOfWeekEnum.rdowWeekendDay.ToString());
			break;
		case 3:
			text += Function.GetJsonString("RecurrenceDayOfWeek", RecurrenceDayOfWeekEnum.rdowSun.ToString());
			break;
		case 4:
			text += Function.GetJsonString("RecurrenceDayOfWeek", RecurrenceDayOfWeekEnum.rdowMon.ToString());
			break;
		case 5:
			text += Function.GetJsonString("RecurrenceDayOfWeek", RecurrenceDayOfWeekEnum.rdowTue.ToString());
			break;
		case 6:
			text += Function.GetJsonString("RecurrenceDayOfWeek", RecurrenceDayOfWeekEnum.rdowWed.ToString());
			break;
		case 7:
			text += Function.GetJsonString("RecurrenceDayOfWeek", RecurrenceDayOfWeekEnum.rdowThu.ToString());
			break;
		case 8:
			text += Function.GetJsonString("RecurrenceDayOfWeek", RecurrenceDayOfWeekEnum.rdowFri.ToString());
			break;
		case 9:
			text += Function.GetJsonString("RecurrenceDayOfWeek", RecurrenceDayOfWeekEnum.rdowSat.ToString());
			break;
		}
		if (Function.ParseInt(row["Month"]) > 0)
		{
			text += Function.GetJsonString("RecurrenceMonth", Function.ParseInt(row["Month"]));
		}
		switch (Function.ParseInt(row["Week"]))
		{
		case 0:
			text += Function.GetJsonString("RecurrenceSequenceSpecifier", "rsFirst");
			break;
		case 1:
			text += Function.GetJsonString("RecurrenceSequenceSpecifier", "rsSecond");
			break;
		case 2:
			text += Function.GetJsonString("RecurrenceSequenceSpecifier", "rsThird");
			break;
		case 3:
			text += Function.GetJsonString("RecurrenceSequenceSpecifier", "rsFourth");
			break;
		case 4:
			text += Function.GetJsonString("RecurrenceSequenceSpecifier", "rsLast");
			break;
		}
		text = ((!(Function.ToString(row["SubOption"]) == "2")) ? (text + Function.GetJsonString("RepeatOption", RepeatOptionEnum.roByDate.ToString())) : (text + Function.GetJsonString("RepeatOption", RepeatOptionEnum.roByWeekDay.ToString())));
		text = ((!(Function.ToString(row["Saturday"]) == "Y")) ? (text + Function.GetJsonString("Saturday", BoYesNoEnum.tNO.ToString())) : (text + Function.GetJsonString("Saturday", BoYesNoEnum.tYES.ToString())));
		DateTime? dateTime2 = Function.ParseDateTime(row["SeEndDat"]);
		if (dateTime2.HasValue)
		{
			text += Function.GetJsonString("SeriesEndDate", dateTime2.Value.ToString("yyyyMMdd"));
		}
		text = ((!(Function.ToString(row["Sunday"]) == "Y")) ? (text + Function.GetJsonString("Sunday", BoYesNoEnum.tNO.ToString())) : (text + Function.GetJsonString("Sunday", BoYesNoEnum.tYES.ToString())));
		text = ((!(Function.ToString(row["Thursday"]) == "Y")) ? (text + Function.GetJsonString("Thursday", BoYesNoEnum.tNO.ToString())) : (text + Function.GetJsonString("Thursday", BoYesNoEnum.tYES.ToString())));
		text = ((!(Function.ToString(row["Tuesday"]) == "Y")) ? (text + Function.GetJsonString("Tuesday", BoYesNoEnum.tNO.ToString())) : (text + Function.GetJsonString("Tuesday", BoYesNoEnum.tYES.ToString())));
		text = ((!(Function.ToString(row["Wednesday"]) == "Y")) ? (text + Function.GetJsonString("Wednesday", BoYesNoEnum.tNO.ToString())) : (text + Function.GetJsonString("Wednesday", BoYesNoEnum.tYES.ToString())));
		text += DataService.MappingUdf(row);
		return text + "}";
	}
}
