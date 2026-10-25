using System.Data;

namespace Apzon.Services.Interfaces.ProductionAddon;

public interface IListPlant
{
	DataTable GetDocs();

	string SaveDocs(DataTable dt);

	string DeleteDocs(string ids);

	DataTable GetListGroupControlKey();
}
