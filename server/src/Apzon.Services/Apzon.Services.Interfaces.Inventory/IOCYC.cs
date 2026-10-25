using System.Data;
using Apzon.Entities.Models.MasterData;

namespace Apzon.Services.Interfaces.Inventory;

public interface IOCYC
{
	DataTable GetOCYC();

	DataTable Search(string key);

	DataTable GetList(string name, string frequency);

	int GetMaxCode();

	string Save(OCYC_Impl doc);
}
