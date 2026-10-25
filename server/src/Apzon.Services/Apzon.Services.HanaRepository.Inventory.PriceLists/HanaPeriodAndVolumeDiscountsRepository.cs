using Apzon.Services.Repository.Inventory;

namespace Apzon.Services.HanaRepository.Inventory.PriceLists;

public class HanaPeriodAndVolumeDiscountsRepository : PeriodAndVolumeDiscountsRepository
{
	public HanaPeriodAndVolumeDiscountsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetDeleteDocsQuery()
	{
		return "select string_agg(\"ItemCode\" ||',') from OSPP a where \"ListNum\"=@ListNum and \"ItemCode\" not in (";
	}

	protected override string GetDeleteDocsAndQuery()
	{
		return ") and (select count(\"CardCode\") from ocrd where \"CardCode\"=a.\"CardCode\")=0;";
	}
}
