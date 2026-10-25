using Apzon.Services.Repository.BusinessPartner;

namespace Apzon.Services.SqlRepository.BusinessPartner;

public class SqlCommentCustomerRepository : CommentCustomerRepository
{
	public SqlCommentCustomerRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
