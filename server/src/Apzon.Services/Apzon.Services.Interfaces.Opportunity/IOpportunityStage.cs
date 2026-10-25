using System.Collections.Generic;
using System.Data;

namespace Apzon.Services.Interfaces.Opportunity;

public interface IOpportunityStage
{
	DataTable GetDocs();

	int GetNewId();

	string SaveDoc(DataTable dt);

	int GetNewNum();

	string DeleteDocs(List<int> lst);

	DataTable GetDoc(int StepId);

	string DeleteDoc(int num);
}
