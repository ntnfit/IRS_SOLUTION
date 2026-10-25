using System;
using Apzon.Commons;
using Apzon.Controls;

namespace Apzon.Forms.KPIs;

public class CustomFormatPeriod : IFormatProvider, ICustomFormatter
{
	public string Format(string format, object arg, IFormatProvider formatProvider)
	{
		string result = arg.ToString();
		switch (format)
		{
		case "apz_quarter":
		{
			DateTime date = Function.ParseDateTimes(arg);
			return "Q" + date.GetQuarter() + "/" + date.Year;
		}
		case "apz_month":
			return Function.ParseDateTimes(arg).ToString("MM/yyyy");
		case "apz_year":
			return Function.ParseDateTimes(arg).ToString("yyyy");
		case "apz_week":
			return Function.ParseDateTimes(arg).ToString("dd/MM/yyyy");
		default:
			return result;
		}
	}

	public object GetFormat(Type formatType)
	{
		return this;
	}
}
