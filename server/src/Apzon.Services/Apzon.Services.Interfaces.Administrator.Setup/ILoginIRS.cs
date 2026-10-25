using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Administrator.Setup;

public interface ILoginIRS
{
	HttpResult LoginIrs(string user, string pass, string dbName);

	HttpResult CheckLicenseIrs(string hardwareKeyServer, string user, string dbName, string clientVersion);

	string CheckAdministrator(string userName, string superUser);

	HttpResult CheckTrialDatabase(string username, string dbName);
}
