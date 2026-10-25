using System.Data;

namespace Apzon.DataServices.Interfaces;

public interface IBinLocationAttribute
{
	string Save(string sessionId, DataTable doc);
}
