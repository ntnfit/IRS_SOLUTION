using Apzon.Entities.Models.PlanningProduction;

namespace Apzon.DataServices.Interfaces;

public interface IShift : IRepository<Shift>
{
	string Save(Shift obj);

	string Delete(string code);
}
