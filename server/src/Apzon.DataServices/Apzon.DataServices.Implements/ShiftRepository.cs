using System;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.PlanningProduction;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class ShiftRepository : Repository<Shift>, IShift, IRepository<Shift>
{
	public ShiftRepository(Apzon.Services.IUnitOfWork unitOfWork, IDataService dataService)
		: base(unitOfWork, dataService)
	{
	}

	public string Save(Shift obj)
	{
		try
		{
			return string.Empty;
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	public string Delete(string code)
	{
		try
		{
			return string.Empty;
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}
}
