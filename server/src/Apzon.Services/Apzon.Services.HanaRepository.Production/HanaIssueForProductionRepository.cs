using Apzon.Services.Repository.Production;

namespace Apzon.Services.HanaRepository.Production;

public class HanaIssueForProductionRepository : IssueForProductionRepository
{
	public HanaIssueForProductionRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetMaxFileEntryQuery()
	{
		return "SELECT IFNULL( MAX(\"AbsEntry\"),0) as \"maxCode\"\r\n                                    FROM oatc;";
	}

	protected override string GetLoadAttachFilesQuery(int absEntry)
	{
		return "SELECT * FROM ATC1 where \"AbsEntry\" = " + absEntry + ";";
	}
}
