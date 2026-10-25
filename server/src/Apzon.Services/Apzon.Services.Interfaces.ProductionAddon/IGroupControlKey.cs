using System.Data;

namespace Apzon.Services.Interfaces.ProductionAddon;

public interface IGroupControlKey
{
	DataTable GetDocs();

	string SaveDocs(DataTable dt);

	string DeleteDocs(string ids);

	DataTable GetListGroupControlKey();
}
