using System.Data;

namespace Apzon.Services.Interfaces.Service;

public interface Isolution
{
	DataTable Search(int sltCode, string itemCode, int status);

	DataTable GetLine(int atcEntry);

	DataTable GetSolutionStatuses();

	DataTable GetOwner();

	DataTable FindDataNextLast(int id, int type);

	string Save(DataSet ds, string action);

	string DeleteDocs(int id);

	int GetMaxSltCode();
}
