using System.Data;

namespace Apzon.Services.Interfaces.Opportunity;

public interface IActivityMeetingLocation
{
	int GetMaxActivityMeetingLocation();

	string SaveDoc(DataTable dt);

	DataTable GetAll();

	DataTable ShowData();

	DataTable GetUserSignByUserCode_tableOUSR(string madk);

	string DeleteDoc(int code);
}
