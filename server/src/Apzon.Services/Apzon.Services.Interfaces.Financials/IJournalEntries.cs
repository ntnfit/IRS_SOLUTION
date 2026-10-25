using System.Data;
using Apzon.Entities.Models.Financials;

namespace Apzon.Services.Interfaces.Financials;

public interface IJournalEntries : IService<JournalEntriesImpl>
{
	string Costing(JournalEntriesImpl doc);

	string Import(JournalEntriesImpl JeLuong, AccountingSalaryImpl AccountingSalary);

	string Cancel(int docEntry);

	int GetMax();

	DataTable GetDoc(int transId);

	DataTable GetLines(int transId);
}
