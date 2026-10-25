using Apzon.Services.Repository.Opportunity;

namespace Apzon.Services.HanaRepository.Opportunity;

public class HanaActivitiesForOpportunityRepository : ActivitiesForOpportunityRepository
{
	public HanaActivitiesForOpportunityRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetSearchQuery(string position)
	{
		string result = "";
		switch (position)
		{
		case "F":
			result = "Select top 1 \"ClgCode\" From OCLG order by \"ClgCode\" asc;";
			break;
		case "P":
			result = "if exists(select 1 from OCLG where \"ClgCode\" < @clgCode)\r\n                            THEN\r\n                                Select top 1 \"ClgCode\" From OCLG where \"ClgCode\" < @clgCode order by \"ClgCode\" desc;\r\n                            ELSE\r\n                                Select top 1 \"ClgCode\" From OCLG order by \"ClgCode\" desc;\r\n                            END IF;";
			break;
		case "N":
			result = "if exists(select 1 from OCLG where \"ClgCode\" > @clgCode)\r\n                            THEN\r\n                                Select top 1 \"ClgCode\" From OCLG where \"ClgCode\" > @clgCode order by \"ClgCode\" ASC;\r\n                            ELSE\r\n                                Select top 1 \"ClgCode\" From OCLG order by \"ClgCode\" ASC;\r\n                            END IF;";
			break;
		case "L":
			result = "Select top 1 \"ClgCode\" From OCLG order by \"ClgCode\" desc;";
			break;
		}
		return result;
	}
}
