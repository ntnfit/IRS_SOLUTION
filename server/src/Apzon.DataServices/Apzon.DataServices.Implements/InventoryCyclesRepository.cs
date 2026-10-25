using System;
using System.Data;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class InventoryCyclesRepository : Repository<DataTable>, IInventoryCycles
{
	public InventoryCyclesRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	public virtual HttpResult Add(DataTable dtSave, string sessionId)
	{
		throw new NotImplementedException();
	}

	public virtual HttpResult Delete(DataTable dtSave, string sessionId)
	{
		throw new NotImplementedException();
	}

	public virtual HttpResult Update(DataTable dtSave, string sessionId)
	{
		throw new NotImplementedException();
	}
}
