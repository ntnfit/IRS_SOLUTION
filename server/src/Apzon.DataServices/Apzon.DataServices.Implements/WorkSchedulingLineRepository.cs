using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.PlanningProduction;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class WorkSchedulingLineRepository : Repository<WorkSchedulingLines>, IWorkSchedulingLine, IRepository<WorkSchedulingLines>
{
	public WorkSchedulingLineRepository(Apzon.Services.IUnitOfWork unitOfWork, IDataService dataService)
		: base(unitOfWork, dataService)
	{
	}
}
