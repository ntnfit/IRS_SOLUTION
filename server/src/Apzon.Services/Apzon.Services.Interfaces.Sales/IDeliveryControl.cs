using System.Data;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;

namespace Apzon.Services.Interfaces.Sales;

public interface IDeliveryControl
{
	DataSet Get(DataTable dt);

	DataSet GetProduction(DataTable dt);

	DataSet ListOfDelivery(DataTable dt);

	DataSet ListOfInvoice(DataTable dt);

	DataSet ListInvoiceForPayment(DataTable dt);

	DataTable GetListShipers(string username, int branch);

	DataTable GetListBranchs(string username, int branch);

	DataSet GetBatch(int orderNo, int orderLine);

	DataSet LoadInvoices(int id);

	DataSet LoadItems(int id);

	DataSet LoadSchedule(int id);

	DataTable GetById(int id);

	int GetPage(int currentId, int type, string username, int branch);

	DataSet GetBatchDelivery(string docentry, string userName, int branch);

	DataSet GetInvoice(int docentry);

	DataTable CheckDelivery(string docentry);

	string ReturnDelRequestBatch(DataTable dtBatch);

	string UpdatePrintedInvoice(string docentry, string table, string type);

	DataSet SearchDeliveryOrder(DocumentsImpl doc);

	DataSetResult GetDataByBarcode(DataSet model);

	DataSetResult GetDataByDraftKey(string barcode);
}
