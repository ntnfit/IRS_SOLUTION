using System.Data;
using Apzon.Commons.Enums;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class InternalReconciliationsRepository : Repository<DataSet>, IInternalReconciliations
{
	public InternalReconciliationsRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	public virtual HttpResult Add(InternalReconciliationImpl doc, string sessionId)
	{
		return new HttpResult(Enums.MessageCode.Error, "Function not support");
	}
}
