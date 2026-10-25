using System.Data;

namespace Apzon.Services.Interfaces.Administrator;

public interface IEmpToken
{
	string Save(DataTable dt);

	DataTable GetEmpTokens(int userId);

	string CheckToken(int empId, string token);
}
