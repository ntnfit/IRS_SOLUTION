using System;
using System.Collections.Generic;
using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Commons;

public interface IExRateAndIndex
{
	string SaveDoc(List<ExchangeRateAndIndexImpl> doc);

	DataTable GetIndexByYear(int year);

	DataTable GetRatesByMonthYear(string month, string year);

	DataTable GetRateByCurrency(string cur, DateTime date);
}
