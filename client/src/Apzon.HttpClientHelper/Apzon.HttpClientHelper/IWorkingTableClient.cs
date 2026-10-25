using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;

namespace Apzon.HttpClientHelper;

public interface IWorkingTableClient
{
	Task<string> SetStatus(DataTable dt);

	Task<string> CancelRequest(int wddCode);

	Task<HttpResult> SaveDraftToDocument(DocumentsImpl doc);

	Task<HttpResult> GetDraftApprovalStatus(int draftKey);
}
