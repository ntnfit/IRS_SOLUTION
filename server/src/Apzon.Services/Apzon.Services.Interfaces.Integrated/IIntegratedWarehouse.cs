using System.Data;

namespace Apzon.Services.Interfaces.Integrated;

public interface IIntegratedWarehouse
{
	DataSet Search(DataTable doc);

	DataTable CheckExists(DataTable dt);
}
