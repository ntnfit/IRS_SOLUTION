using System.Data;

namespace Apzon.Services.Interfaces.Administrator;

public interface ICalendarSetting
{
	DataSet GetDoc(string user);

	DataTable GetGeneSettings(string user);

	string SaveDoc(DataSet ds);

	string DeleteDoc(string code);
}
