using Apzon.DataServices.Implements;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaWorkSchedulingLineRepository : WorkSchedulingLineRepository
{
	public HanaWorkSchedulingLineRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}
}
