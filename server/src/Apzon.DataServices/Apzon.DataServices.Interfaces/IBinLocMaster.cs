using System.Data;

namespace Apzon.DataServices.Interfaces;

public interface IBinLocMaster
{
	string Save(string sessionId, DataTable doc, string action);

	string Delete(string sessionId, int absEntry);
}
