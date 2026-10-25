using System.Data;

namespace Apzon.Services.Interfaces.Administrator.Setup;

public interface ITerritory
{
	DataTable Search(string id);

	DataTable GetData();

	string SaveData(DataTable dt);

	bool ExistsLinkedObject(int id);
}
