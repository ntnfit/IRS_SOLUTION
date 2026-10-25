using Apzon.Services.Repository.Mrps;

namespace Apzon.Services.HanaRepository.Mrps;

public class HanaItemQuotaArrangementRepository : ItemQuotaArrangementRepository
{
	public HanaItemQuotaArrangementRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string QueryGetDocs()
	{
		return "Select * From APZ_OIQA where LOWER(\"ItemCode\") like '%' || LOWER(@ItemCode) || '%';";
	}
}
