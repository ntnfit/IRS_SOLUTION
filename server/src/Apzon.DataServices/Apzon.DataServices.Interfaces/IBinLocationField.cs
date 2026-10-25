using System.Data;

namespace Apzon.DataServices.Interfaces;

public interface IBinLocationField
{
	string Save(string sessionId, DataTable doc);
}
