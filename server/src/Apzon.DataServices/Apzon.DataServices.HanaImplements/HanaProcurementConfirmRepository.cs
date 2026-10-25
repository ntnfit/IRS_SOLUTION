using Apzon.DataServices.Implements;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaProcurementConfirmRepository : ProcurementConfirmRepository
{
	public HanaProcurementConfirmRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}
}
