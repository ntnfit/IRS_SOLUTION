using Apzon.Services.Repository.Purchase;

namespace Apzon.Services.HanaRepository.Purchase;

public class HanaPurchaseRequestRespository : PurchaseRequestRespository
{
	public HanaPurchaseRequestRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetListDepartmentString()
	{
		return "select \"Code\", IFNULL(\"Name\",'') as \"Name\" from OUDP\r\n                               Union all\r\n                               select -1,'' FROM DUMMY order by \"Code\";";
	}

	protected override string GetListBranchString()
	{
		return "select \"Code\", IFNULL(\"Name\",'') as \"Name\" from OUBR\r\n                                Union all\r\n                                select -1,'' from DUMMY order by \"Code\";";
	}
}
