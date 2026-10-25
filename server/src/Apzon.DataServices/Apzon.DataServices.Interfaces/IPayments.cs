using System.Collections.Generic;
using Apzon.Commons;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Payments;

namespace Apzon.DataServices.Interfaces;

public interface IPayments : IRepository<IncomingPaymentsImpl>
{
	HttpResult Save(ApzObjectTypes objType, IncomingPaymentsImpl doc, string sessionId, bool isDraft);

	HttpResult SaveListDoc(ApzObjectTypes objType, List<IncomingPaymentsImpl> docs, string sessionId);

	HttpResult SaveDraftToDocument(int docEntry, string sessionId);
}
