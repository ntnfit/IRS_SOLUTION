using Apzon.Services.Repository.Administrator.ApprovalProcess;

namespace Apzon.Services.SqlRepository.Administrator.ApprovalProcess;

public class SqlApprovalStagesRepository : ApprovalStagesRepository
{
	public SqlApprovalStagesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
