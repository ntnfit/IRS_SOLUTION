using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Entities.Models.Administrator;
using Apzon.Global;

namespace Apzon.HttpClientHelper;

internal class GeneralSettingClient : IGeneralSetting
{
	private const string Controller = "GeneralSetting";

	public async Task GetFormatString()
	{
		List<GeneralSettingImpl> list;
		using (HttpClientHelper<GeneralSettingImpl> helper = new HttpClientHelper<GeneralSettingImpl>(Configuration.ApiServer, "GeneralSetting"))
		{
			list = await helper.GetListAsync("GetFormatString");
		}
		if (list != null && list.Count > 0)
		{
			GeneralSettingImpl generalSettingImpl = list[0];
			GlobalData.DateSeparatorFormat = generalSettingImpl.DateSep;
			GlobalData.DateTimeFormat = GetDateFormat(generalSettingImpl.DateFormat);
			GlobalData.TimeFormat = generalSettingImpl.TimeFormat;
			GlobalData.MainCurrency = generalSettingImpl.MainCurncy;
			GlobalData.SystemCurrency = generalSettingImpl.SysCurrncy;
			GlobalData.PercentageRateFormat = new NumberFormatInfo
			{
				NumberDecimalDigits = Function.ParseInt(generalSettingImpl.PercentDec),
				NumberDecimalSeparator = generalSettingImpl.DecSep,
				NumberGroupSeparator = generalSettingImpl.ThousSep
			};
			GlobalData.UnitPriceFormat = new NumberFormatInfo
			{
				NumberDecimalDigits = Function.ParseInt(generalSettingImpl.PriceDec),
				NumberDecimalSeparator = generalSettingImpl.DecSep,
				NumberGroupSeparator = generalSettingImpl.ThousSep
			};
			GlobalData.QuantitiesFormat = new NumberFormatInfo
			{
				NumberDecimalDigits = Function.ParseInt(generalSettingImpl.QtyDec),
				NumberDecimalSeparator = generalSettingImpl.DecSep,
				NumberGroupSeparator = generalSettingImpl.ThousSep
			};
			GlobalData.TotalsAccuracyFormat = new NumberFormatInfo
			{
				NumberDecimalDigits = Function.ParseInt(generalSettingImpl.SumDec),
				NumberDecimalSeparator = generalSettingImpl.DecSep,
				NumberGroupSeparator = generalSettingImpl.ThousSep
			};
			GlobalData.RateAccuracyFormat = new NumberFormatInfo
			{
				NumberDecimalDigits = Function.ParseInt(generalSettingImpl.RateDec),
				NumberDecimalSeparator = generalSettingImpl.DecSep,
				NumberGroupSeparator = generalSettingImpl.ThousSep
			};
			GlobalData.MeasureFormat = new NumberFormatInfo
			{
				NumberDecimalDigits = Function.ParseInt(generalSettingImpl.QtyDec),
				NumberDecimalSeparator = generalSettingImpl.DecSep,
				NumberGroupSeparator = generalSettingImpl.ThousSep
			};
			GlobalData.DocSettingsClosedQuot = generalSettingImpl.ClosedQuot;
			return;
		}
		throw new Exception("Could not load general setting.");
	}

	public static string GetDateFormat(string format)
	{
		try
		{
			return (format.ToUpper() switch
			{
				"DD/MM/YY" => "dd/MM/yy", 
				"DD/MM/CCYY" => "dd/MM/yyyy", 
				"MM/DD/YY" => "MM/dd/yy", 
				"MM/DD/CCYY" => "MM/dd/yyyy", 
				"CCYY/MM/DD" => "yyyy/MM/dd", 
				"DD/Month/YYYY" => "dd/MMMM/yyyy", 
				"YY/MM/DD" => "yy/MM/dd", 
				_ => "dd/MM/yy", 
			}).Replace("/", GlobalData.DateSeparatorFormat ?? "-");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "dd/MM/yyyy HH:mm:ss";
		}
	}

	public GeneralSettingImpl GetAllGeneralSetting()
	{
		try
		{
			GeneralSettingImpl generalSettingImpl;
			using (HttpClientHelper<GeneralSettingImpl> httpClientHelper = new HttpClientHelper<GeneralSettingImpl>(Configuration.ApiServer, "GeneralSetting"))
			{
				generalSettingImpl = httpClientHelper.Get("GetAllGeneralSetting");
			}
			if (generalSettingImpl != null)
			{
				return generalSettingImpl;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
		return new GeneralSettingImpl();
	}

	public async Task<DataTable> GetLengthUnits()
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "GeneralSetting");
		return await http.GetAsync("GetLengthUnits");
	}

	public async Task<DataTable> GetWeightUnits()
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "GeneralSetting");
		return await http.GetAsync("GetWeightUnits");
	}
}
