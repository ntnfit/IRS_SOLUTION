using Apzon.Services.Repository.Administrator.ApprovalProcess;

namespace Apzon.Services.HanaRepository.Administrator.ApprovalProcess;

public class HanaApprovalTemplatesRepository : ApprovalTemplatesRepository
{
	public HanaApprovalTemplatesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetNewCodeQuery()
	{
		return "select IFNULL(MAX(\"WtmCode\"), 0) + 1 from OWTM;";
	}

	protected override string GetSearchDataQuery()
	{
		return "select * from OWTM where (IFNULL(\"Name\", '') = '' or \"Name\" like N'%' || @wtmName || '%') and (IFNULL(\"Remarks\", '') = '' or \"Remarks\" like N'%' || @wtmRemarks || '%');";
	}
}
