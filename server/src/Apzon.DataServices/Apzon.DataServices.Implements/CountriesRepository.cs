using System;
using System.Data;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class CountriesRepository : Repository<DataTable>, ICountries, IDisposable
{
	public CountriesRepository(Apzon.Services.IUnitOfWork serviceUOW, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUOW, dataService)
	{
	}

	public virtual string Mapping(DataRow doc, string sessionId)
	{
		return "";
	}

	public virtual bool CheckExists(string code)
	{
		return false;
	}

	public virtual HttpResult Save(DataTable dt, string sessionId)
	{
		return new HttpResult();
	}

	public virtual HttpResult Remove(string code, string sessionId)
	{
		return new HttpResult();
	}

	public void Dispose()
	{
	}
}
