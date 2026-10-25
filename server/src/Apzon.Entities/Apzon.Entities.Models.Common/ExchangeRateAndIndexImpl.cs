using System;

namespace Apzon.Entities.Models.Common;

public class ExchangeRateAndIndexImpl
{
	public DateTime RateDate { get; set; }

	public string Currency { get; set; }

	public decimal Rate { get; set; }

	public string DataSource { get; set; }

	public int UserSign { get; set; }

	public string MonthName { get; set; }
}
