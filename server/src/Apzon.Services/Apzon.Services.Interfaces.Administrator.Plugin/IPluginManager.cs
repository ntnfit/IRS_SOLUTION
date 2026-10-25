using System.Data;

namespace Apzon.Services.Interfaces.Administrator.Plugin;

public interface IPluginManager
{
	DataTable GetData();

	string Save(DataTable dt);

	string Delete(string plugName);

	string Update(DataTable dt);
}
