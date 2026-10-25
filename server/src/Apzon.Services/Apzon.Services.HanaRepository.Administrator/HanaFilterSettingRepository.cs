using Apzon.Services.Repository.Administrator;

namespace Apzon.Services.HanaRepository.Administrator;

public class HanaFilterSettingRepository : FilterSettingRepository
{
	public HanaFilterSettingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetFilterSelectQuery()
	{
		return "SELECT \"Code\",\"Name\",a.\"FilterText\",a.\"ObjType\" FROM APZ_FILTER a \r\n                                    INNER JOIN APZ_FilterAssign b ON a.\"Code\"=b.\"FilterCode\" AND a.\"ObjType\"=b.\"ObjType\"\r\n                                    WHERE b.\"UserName\"=UserName AND a.\"ObjType\"=ObjType AND a.\"Valid\"=1;";
	}
}
