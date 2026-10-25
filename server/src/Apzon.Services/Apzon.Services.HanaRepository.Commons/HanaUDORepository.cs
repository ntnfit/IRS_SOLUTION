using Apzon.Services.Repository.Commons;

namespace Apzon.Services.HanaRepository.Commons;

public class HanaUDORepository : UDORepository
{
	public HanaUDORepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetMaxIdGetQuery(string tableName)
	{
		return $"SELECT IFNULL((SELECT MAX(DocEntry) FROM  \"@{tableName}\"),0) ID from DUMMY;";
	}
}
