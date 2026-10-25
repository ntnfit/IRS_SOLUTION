using System.Data;

namespace Apzon.Services.Interfaces.Opportunity;

public interface IActivityType
{
	int GetMaxActivityType();

	string SaveDoc(DataTable dt);

	DataTable GetAll();

	DataTable ShowData();

	DataTable GetUserSignByUserCode_tableOUSR(string madk);

	string DeleteDoc(int code);
}
