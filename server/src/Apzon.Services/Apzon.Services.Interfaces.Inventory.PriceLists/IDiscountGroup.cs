using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Inventory.PriceLists;

public interface IDiscountGroup
{
	DataTable GetTableColumns(string table);

	HttpResult SaveDoc(DataSet ds);

	DataTable Search(DataTable dt);

	DataTable SearchLine(int entry);

	DataSet LoadTabPagesData();

	DataTable GenDocTable();

	DataTable GetAllBPEntry();
}
