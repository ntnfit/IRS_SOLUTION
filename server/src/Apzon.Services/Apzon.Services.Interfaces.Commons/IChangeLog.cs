using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Commons;

public interface IChangeLog
{
	int GetDocEntryItemDetail(int id);

	DataTable GetLog(int docEntry);

	DataTable GetChangeLogRow(int docEntry, int logId, int type);

	DataTable GetChangeLog(GetChangeLogParams pr);

	DataTable GetChangeLogDifferences(GetChangeLogParams pr);
}
