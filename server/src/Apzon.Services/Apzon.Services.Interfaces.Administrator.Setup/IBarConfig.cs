using System.Data;

namespace Apzon.Services.Interfaces.Administrator.Setup;

public interface IBarConfig
{
	DataTable GetData();

	DataTable GetColumns();

	string Save(DataTable dt);
}
