using System.Data;

namespace Apzon.Services.Interfaces.Administrator;

public interface IState
{
	string Save(DataTable dt);

	bool CheckData(string key, string country);

	DataTable GetCountry(int user);

	DataTable GetListAllState();

	string Remove(string country);
}
