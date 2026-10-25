using System.Data;

namespace Apzon.DataServices.Interfaces;

public interface IProcurementConfirm : IRepository<DataTable>
{
	DataSet LoadPreferredVendor(string baseType, string userName, string branch);

	DataSet LoadSalesOrderLine(string docEntry, string userName, string branch);

	DataSet LoadItemLine(string baseType, string docEntry, string userName, string branch);
}
