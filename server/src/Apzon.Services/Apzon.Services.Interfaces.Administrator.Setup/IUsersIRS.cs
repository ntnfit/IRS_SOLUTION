using System.Data;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Administrator.Setup;

public interface IUsersIRS
{
	HttpResult Save(UsersImpl user);

	DataTable GetData();

	DataTable GetPocuse(string user);
}
