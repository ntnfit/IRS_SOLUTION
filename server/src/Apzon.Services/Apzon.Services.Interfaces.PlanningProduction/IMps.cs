namespace Apzon.Services.Interfaces.PlanningProduction;

public interface IMps
{
	string GetCodeMps();

	string UpdateApprovalStatus(string id, string status);
}
