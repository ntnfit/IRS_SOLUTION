using System.Data;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IInternalReconciliationsClient
{
	DataSet GetStructure();

	HttpResult Add(InternalReconciliationImpl doc);
}
