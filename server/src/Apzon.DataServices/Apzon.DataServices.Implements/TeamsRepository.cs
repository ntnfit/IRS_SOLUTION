using System;
using System.Data;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class TeamsRepository : Repository<DataTable>, ITeams
{
	public TeamsRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	public virtual HttpResult Save(DataTable dtSource, string sessionId)
	{
		throw new NotImplementedException();
	}

	public virtual HttpResult Delete(int id, string sessionId)
	{
		throw new NotImplementedException();
	}
}
