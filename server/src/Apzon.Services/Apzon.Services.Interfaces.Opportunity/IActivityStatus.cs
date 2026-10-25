using System.Data;

namespace Apzon.Services.Interfaces.Opportunity;

public interface IActivityStatus
{
	int GetMaxActivityStatus();

	string SaveDoc(DataTable dt);

	DataTable GetAll();

	DataTable ShowData();

	DataTable GetUserSignByUserCode_tableOUSR(string madk);

	DataTable GetDefaultTable();

	string Remove(int id);
}
