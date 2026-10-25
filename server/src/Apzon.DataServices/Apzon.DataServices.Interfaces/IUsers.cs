using Apzon.Entities.Models.Administrator;

namespace Apzon.DataServices.Interfaces;

public interface IUsers : IRepository<UsersImpl>
{
	string Save(UsersImpl doc, string sessionId);

	string Delete(string userCode, string sessionId);

	string DeleteDoc(int code, string sessionId);
}
