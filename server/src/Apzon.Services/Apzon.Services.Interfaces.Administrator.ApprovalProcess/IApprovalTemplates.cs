using System.Data;

namespace Apzon.Services.Interfaces.Administrator.ApprovalProcess;

public interface IApprovalTemplates
{
	DataTable GetData();

	DataSet GetDataLines(int templateCode);

	DataTable SearchData(string templateName, string templateRemarks);

	int GetNewCode();

	DataTable GetUserInfo(string userName);

	DataTable GetRecord(string position, string templateCode);

	DataTable GetApprovalStageInfo(string stageName);

	int GetQueryId(string queryName);
}
