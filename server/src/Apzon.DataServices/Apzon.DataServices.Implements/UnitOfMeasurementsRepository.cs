using System;
using System.Data;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class UnitOfMeasurementsRepository : Repository<DataTable>, IUnitOfMeasurements
{
	public UnitOfMeasurementsRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	public virtual HttpResult SaveData(DataTable dtSave, string sessionId)
	{
		throw new NotImplementedException();
	}
}
