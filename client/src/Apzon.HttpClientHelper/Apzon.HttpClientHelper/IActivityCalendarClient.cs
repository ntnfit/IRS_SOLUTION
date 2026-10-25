using System;
using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface IActivityCalendarClient
{
	Task<DataTable> GetDoc();

	Task<DataTable> GetUsers();

	Task<DataTable> GetDocByUser(int attendUser);

	Task<DataTable> GetDocByEmpl(int attendEmpl);

	Task<DataTable> GetUnlockedUsers();

	Task<DataTable> GetActiveEmployees();

	DataTable GetActivityByCode(int clgCode);

	string SaveActivityInterval(DataTable dt);

	string SaveActivityStatus(Tuple<int, int> tpl);

	string SaveActivityAction(Tuple<int, string> tpl);
}
