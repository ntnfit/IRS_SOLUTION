using System.Data;

namespace Apzon.Services.Interfaces.HumanResources.TimeSheeting;

public interface IHolidaySetup
{
	DataTable GetLine(string hldCode);

	DataTable GetAll();

	string Save(DataSet ds);

	string Remove(string hldCode);
}
