using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Administrator.Setup;

public interface IWorkingTable
{
	DataSet GetWorkingTableLine(string id);

	DataSet GetWorkingTableDoc(string type, string user);

	DataSet GetDataStepStatus(string id);

	DataTable GetUserSettings(string username);

	HttpResult GetTopDraft(string username);

	string MarkReadMessage(int docEntry, string username);

	HttpResult GetDraftApprovalStatus(int draftKey);
}
