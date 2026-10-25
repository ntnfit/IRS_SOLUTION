using Apzon.Services.Repository.DMS;

namespace Apzon.Services.HanaRepository.DMS;

public class HanaGeographyRepository : GeographyRepository
{
	public HanaGeographyRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetCountryWithGgpCodeQuery()
	{
		return "select IFNULL(\"CountryCode\", '') from APZ_D_GGP1 where \"Code\" = @ggpCode;";
	}
}
