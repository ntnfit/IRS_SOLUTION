using System.Data;

namespace Apzon.Services.Interfaces.Integrated;

public interface IIntegratedBP
{
	DataSet Search(DataTable doc);

	DataSet GetContactEmps(string cardCode);

	DataSet GetAddresses(string cardCode);

	DataTable CheckExists(DataTable dt);
}
