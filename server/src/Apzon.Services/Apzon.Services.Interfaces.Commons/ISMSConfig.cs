using System.Data;

namespace Apzon.Services.Interfaces.Commons;

public interface ISMSConfig
{
	DataTable GetConfig();

	string SaveConfig(DataTable dt);
}
