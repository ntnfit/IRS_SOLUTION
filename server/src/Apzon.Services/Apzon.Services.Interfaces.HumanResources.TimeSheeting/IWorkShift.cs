using System.Data;

namespace Apzon.Services.Interfaces.HumanResources.TimeSheeting;

public interface IWorkShift
{
	DataTable GetAll();

	DataTable GetLine(int shiftID);

	string Save(DataSet ds);

	string Remove(int shiftId);
}
