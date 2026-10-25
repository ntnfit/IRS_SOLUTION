using System.Data;

namespace Apzon.Services.Interfaces.Administrator;

public interface IAdministrator
{
	DataTable GetCodeOcyc();

	DataTable GetListCountry();

	DataTable GetListState(string country);

	DataTable GetCodeOcycItemGroups();
}
