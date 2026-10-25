using System.Data;
using Apzon.Entities.Models.Administrator;

namespace Apzon.Services.Interfaces.Administrator.Setup;

public interface IHoliday : IService<HolidayImpl>
{
	DataTable ShowHoliday();

	DataTable Search();

	DataTable SearchLine(string hldCod);

	DataTable GetMaxCode();

	bool CheckHldCode(string hldCode);

	string SaveDoc(HolidayImpl doc);

	string SaveLine(Holiday_LineImpl line);

	string DeleteDoc(string hldCode);

	DataTable GetHoliday(HolidayImpl _docSearch);
}
