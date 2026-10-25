using System.Data;

namespace Apzon.Services.Interfaces.Opportunity;

public interface IActivitySubjects
{
	int GetMaxActivitySubject();

	string SaveDoc(DataTable dt);

	DataTable GetAll(string madk);

	DataTable ShowData(string madk);

	DataTable GetUserSignByUserCode_tableOUSR(string madk);

	string DeleteDoc(int code);
}
