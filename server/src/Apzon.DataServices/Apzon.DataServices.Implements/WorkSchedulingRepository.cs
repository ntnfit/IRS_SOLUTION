using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.PlanningProduction;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class WorkSchedulingRepository : Repository<WorkScheduling>, IWorkScheduling, IRepository<WorkScheduling>
{
	public WorkSchedulingRepository(Apzon.Services.IUnitOfWork unitOfWork, IDataService dataService)
		: base(unitOfWork, dataService)
	{
	}
}
