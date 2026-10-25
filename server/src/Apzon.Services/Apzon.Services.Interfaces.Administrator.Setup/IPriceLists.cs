using System.Collections.Generic;
using System.Data;
using Apzon.Entities.Models.MasterData;

namespace Apzon.Services.Interfaces.Administrator.Setup;

public interface IPriceLists : IService<PriceListImpl>
{
	List<PriceListImpl> Cfl();

	DataTable GetGroupNumPriceList();

	DataTable ShowPriceList();

	DataTable Search(PriceListImpl doc);

	int GetMaxCode();

	bool Check(int idCode);

	string SaveDoc(PriceListImpl doc);

	string DeleteDoc(int idCode);

	DataTable GetPriceList(string itmCode);
}
