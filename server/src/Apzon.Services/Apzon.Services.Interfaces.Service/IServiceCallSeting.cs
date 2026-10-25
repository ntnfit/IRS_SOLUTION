using System.Data;

namespace Apzon.Services.Interfaces.Service;

public interface IServiceCallSeting
{
	string Save(string name, string email, string pass, int service, string userName);

	DataTable GetService(string user);

	DataTable GetSetting(string user);
}
