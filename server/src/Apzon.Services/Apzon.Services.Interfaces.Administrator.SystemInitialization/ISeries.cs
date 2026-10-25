using System.Data;

namespace Apzon.Services.Interfaces.Administrator.SystemInitialization;

public interface ISeries
{
	DataTable LoadDoc(string objCode, string docSubType, int userId);

	string Save(DataTable dtDoc);
}
