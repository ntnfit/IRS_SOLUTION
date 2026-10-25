using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Payments;

namespace Apzon.DataServices.Interfaces;

public interface IPaymentsDrafts : IRepository<IncomingPaymentsImpl>
{
	HttpResult Save(IncomingPaymentsImpl doc, string sessionId);
}
