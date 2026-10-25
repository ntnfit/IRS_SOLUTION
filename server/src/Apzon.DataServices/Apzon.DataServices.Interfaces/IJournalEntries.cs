using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Financials;

namespace Apzon.DataServices.Interfaces;

public interface IJournalEntries : IRepository<JournalEntriesImpl>
{
	HttpResult SaveData(JournalEntriesImpl doc, string sessionId);

	string Mapping(JournalEntriesImpl model, EnumCommandType commandType);

	HttpResult CreateJournalVoucher(JournalEntriesImpl lstJE, string sessionId);

	HttpResult CancelJE(int transId, string sessionId);
}
