using Apzon.Services.Repository.Administrator.ApprovalProcess;

namespace Apzon.Services.SqlRepository.Administrator.ApprovalProcess;

public class SqlApprovalTemplatesRepository : ApprovalTemplatesRepository
{
	public SqlApprovalTemplatesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
