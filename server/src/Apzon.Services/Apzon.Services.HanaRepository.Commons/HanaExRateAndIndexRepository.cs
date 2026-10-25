using System;
using Apzon.Services.Repository.Commons;

namespace Apzon.Services.HanaRepository.Commons;

public class HanaExRateAndIndexRepository : ExRateAndIndexRepository
{
	public HanaExRateAndIndexRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetRateByCurrencyQuery(DateTime date)
	{
		return string.Concat("DECLARE main NVARCHAR(30);\r\n                select (SELECT TOP 1 \"MainCurncy\" FROM OADM) into main from DUMMY;\r\n                IF main=cur\r\n                then\r\n                    SELECT 1 from dummy;\r\n                ELSE\r\n                    select \"Rate\" from ortt where \"RateDate\"='" + $"{date.Year}-{date.Month}-{date.Day}" + "'", " and \"Currency\"=cur;\r\nEND IF;");
	}
}
