using System.Collections.Generic;
using System.Data;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Administrator.Setup;

public interface IItemGroups : IService<ItemGroupsImpl>
{
	List<ItemGroupsImpl> Cfl();

	DataTable GetItemGroup();

	DataTable ShowItemGroup();

	DataTable Search(ItemGroupsImpl doc);

	int GetMaxCode();

	bool Check(int code);

	HttpResult SaveDoc(DataSet ds);

	string DeleteDoc(int code);

	DataTable CheckExistItemGroupCode(int itgrCode);

	DataTable LoadDoc();

	DataTable GetItemGroupWarehouse(int code);
}
