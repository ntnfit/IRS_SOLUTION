using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;

namespace Apzon.DataServices.Interfaces;

public interface IInternalReconciliations
{
	HttpResult Add(InternalReconciliationImpl doc, string sessionId);
}
