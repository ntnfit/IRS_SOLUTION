using System.Data;

namespace Apzon.Services.Interfaces.Administrator;

public interface IActivityCalendar
{
	DataTable GetDoc();

	DataTable GetUsers();

	DataTable GetDocByUser(int attendUser);

	DataTable GetDocByEmpl(int attendEmpl);

	DataTable GetActivityByCode(int clgCode);

	string SaveActivityInterval(DataTable dt);

	string SaveActivityStatus(int clgCode, int status);

	string SaveActivityAction(int clgCode, string action);

	DataTable GetUnlockedUsers();

	DataTable GetActiveEmployees();
}
