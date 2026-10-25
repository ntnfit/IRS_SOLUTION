using System.Data;

namespace Apzon.Services.Interfaces.Administrator.ApprovalProcess;

public interface IApprovalStages
{
	DataTable GetData();

	DataTable GetDataLines(int stageCode);

	DataTable SearchData(int stageCode, string stageName);

	int GetNewCode();

	DataTable GetUserInfo(string userName);

	DataTable GetRecord(string position, string stageCode);
}
