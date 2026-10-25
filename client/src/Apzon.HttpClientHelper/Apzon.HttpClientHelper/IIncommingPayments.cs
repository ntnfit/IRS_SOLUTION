using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Payments;

namespace Apzon.HttpClientHelper;

public interface IIncommingPayments
{
	Task<DataTable> GetByInvoiceNo(string barcode);

	Task<HttpResult> Save(IncomingPaymentsImpl doc);

	Task<HttpResult> Save(List<IncomingPaymentsImpl> doc);

	Task<DataSet> GetListInvoice(DataTable dtSearch);

	Task<DataSet> GetListPayment(DataTable dtSearch);
}
