using Apzon.DataServices.Implements;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaIndustryRepository : IndustryRepository
{
	public HanaIndustryRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, unitOfWork, dataService)
	{
	}
}
