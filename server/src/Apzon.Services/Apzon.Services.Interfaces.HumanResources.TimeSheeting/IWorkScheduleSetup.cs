using System.Data;

namespace Apzon.Services.Interfaces.HumanResources.TimeSheeting;

public interface IWorkScheduleSetup
{
	DataSet GetLine(int schId);

	DataTable GetAll();

	string Save(DataSet ds);

	string Remove(int schId);
}
