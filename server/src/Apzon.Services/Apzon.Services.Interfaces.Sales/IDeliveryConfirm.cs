using System.Data;

namespace Apzon.Services.Interfaces.Sales;

public interface IDeliveryConfirm
{
	DataSet GetDeliveryOrder(int docEntry);

	DataTable GetBatchDetail(string batch);

	string ConfirmBatch(DataSet ds);
}
