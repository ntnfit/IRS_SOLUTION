using System;
using System.Data;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class ActivityStatusRepository : Repository<DataTable>, IActivityStatus
{
	public ActivityStatusRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	public virtual HttpResult Save(DataTable dtSave, string sessionId)
	{
		throw new NotImplementedException();
	}
}
