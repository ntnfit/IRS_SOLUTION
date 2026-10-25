using System.Data;

namespace Apzon.Services.Interfaces.Administrator;

public interface IAuthorToken
{
	string Save(DataTable dt);

	DataTable GetAllEmployees();

	DataTable GetAuthorizedEmp(int userId);
}
