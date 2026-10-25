using System.Data;

namespace Apzon.DataServices.Interfaces;

public interface IApprovalRequests
{
	string ApproveDraft(string sessionId, string user, int userId, DataTable dt, string serverPath = "");

	string CancelApprovalRequest(int wddCode, string sessionId);
}
