using System;

namespace Apzon.HttpClientHelper;

public interface IExRateAndIndexClient
{
	double GetExchangeRate(string curr, DateTime date);
}
