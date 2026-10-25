using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Financials;

namespace Apzon.HttpClientHelper;

public interface IJournalEntryClient
{
	Task<DataTable> GetDoc(int transId);

	Task<DataTable> GetLines(int transId);

	Task<HttpResult> SaveDoc(JournalEntriesImpl doc);

	Task<HttpResult> SaveDoc(JournalEntriesImpl doc, string sessionId);

	Task<HttpResult> CancelJE(int transId);
}
