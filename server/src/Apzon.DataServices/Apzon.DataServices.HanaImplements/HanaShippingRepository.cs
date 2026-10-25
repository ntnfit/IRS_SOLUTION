using Apzon.DataServices.Implements;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaShippingRepository : ShippingRepository
{
	public HanaShippingRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, unitOfWork, dataService)
	{
	}
}
