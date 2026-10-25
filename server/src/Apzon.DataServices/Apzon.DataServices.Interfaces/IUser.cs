using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;

namespace Apzon.DataServices.Interfaces;

public interface IUser : IRepository<UsersImpl>
{
	HttpResult Login(string dbServer, string dbName, string dbTypeStr, string userName, string userPwd, string language, string licServer, string dbuserName, string dbPwd, string sessionId);

	HttpResult Logout(string sessionId);
}
