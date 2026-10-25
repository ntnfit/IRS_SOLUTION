using Apzon.Services.Repository.Administrator.ApprovalProcess;

namespace Apzon.Services.HanaRepository.Administrator.ApprovalProcess;

public class HanaApprovalStagesRepository : ApprovalStagesRepository
{
	public HanaApprovalStagesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetNewCodeQuery()
	{
		return "select IFNULL(MAX(\"WstCode\"), 0) + 1 from OWST;";
	}
}
