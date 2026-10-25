using Apzon.Services.Repository.Promotion;

namespace Apzon.Services.HanaRepository.Promotion;

internal class HanaCheckPromotionRepository : CheckPromotionRepository
{
	public HanaCheckPromotionRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetSaveQuery()
	{
		return "DELETE from APZ_D_PMO4 WHERE GUID=@GUID;\r\n                    exec 'INSERT INTO APZ_D_PMO4 SELECT * FROM #APZ_D_PMO4;';";
	}
}
