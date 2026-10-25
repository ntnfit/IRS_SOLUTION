using System.Data;

namespace Apzon.Services.Interfaces.HumanResources.TimeSheeting;

public interface IWorkScheduleRegister
{
	DataTable GetDetail(int empId);

	DataTable Search(DataTable dtSearch);

	string Save(DataTable dt);

	string Remove(int lineNum, int empId);
}
