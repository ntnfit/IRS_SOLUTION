using System;
using System.Data;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class CurrenciesRepository : Repository<DataTable>, ICurrencies, IDisposable
{
	public CurrenciesRepository(Apzon.Services.IUnitOfWork serviceUOW, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUOW, dataService)
	{
	}

	public virtual string Mapping(DataRow doc, string sessionId)
	{
		return string.Empty;
	}

	public virtual HttpResult Save(string sessionId, DataTable dt)
	{
		return new HttpResult();
	}

	public virtual bool CheckExists(string currCode, string sessionId)
	{
		return false;
	}

	public virtual HttpResult Remove(string currCode, string sessionId)
	{
		return new HttpResult();
	}

	public void Dispose()
	{
	}
}
