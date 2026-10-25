using System;
using System.Data;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class WeightRepository : Repository<DataTable>, IWeight, IDisposable
{
	public WeightRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	public virtual int CheckExists(int unitCode, string sessionId)
	{
		return 0;
	}

	public virtual string Mapping(DataRow doc, string sessionId)
	{
		return "";
	}

	public virtual HttpResult Save(DataTable dt, string sessionId)
	{
		return new HttpResult();
	}

	public virtual HttpResult Remove(DataTable dt, string sessionId)
	{
		return new HttpResult();
	}

	public void Dispose()
	{
	}
}
