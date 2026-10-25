using System.Data;

namespace Apzon.Services.Interfaces.Administrator.SystemInitialization;

public interface IMenuStructure
{
	DataSet GetListMenu(string lngCode = "");

	string AddNewFolder(DataTable dt);

	string RelocateFolder(DataTable dt);

	string Rename(DataTable dt);

	string DeleteFolder(int id);

	DataTable GetListCustomizeMenu();
}
