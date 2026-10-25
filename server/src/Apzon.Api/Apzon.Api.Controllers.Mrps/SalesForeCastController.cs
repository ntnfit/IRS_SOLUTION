using System;
using System.Data;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Mrps;

public class SalesForeCastController : BaseApiController
{
	[HttpGet]
	public DataTable GetDocs()
	{
		return UnitOfWork.SalesForeCast.GetDocs();
	}

	[HttpPost]
	public DataTable Search(DataTable dt)
	{
		return UnitOfWork.SalesForeCast.Search(dt);
	}

	[HttpPost]
	public DataTable GetDetail(Tuple<int, DateTime, DateTime> tup)
	{
		return UnitOfWork.SalesForeCast.GetDetail(tup);
	}

	[HttpGet]
	public DataTable GetDoc(int id)
	{
		return UnitOfWork.SalesForeCast.GetDoc(id);
	}

	[HttpPost]
	public DataTable GetQuickAnalysis(DataTable dt)
	{
		return UnitOfWork.SalesForeCast.GetQuickAnalysis(dt);
	}

	[HttpGet]
	public DataTable GetInfor(int ID)
	{
		return UnitOfWork.SalesForeCast.GetInfor(ID);
	}

	[HttpGet]
	public int GetMaxAbsID()
	{
		return UnitOfWork.SalesForeCast.GetMaxAbsID();
	}

	[HttpGet]
	public DataTable GetYearForQuickAnalysis()
	{
		return UnitOfWork.SalesForeCast.GetYearForQuickAnalysis();
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string SaveDocs(DataSet ds)
	{
		return UnitOfWork.SalesForeCast.SaveDocs(ds);
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string Savedoc(DataTable dt)
	{
		return UnitOfWork.SalesForeCast.Savedoc(dt);
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string DeleteRowInGeneral([FromBody] int AbsID)
	{
		return UnitOfWork.SalesForeCast.DeleteRowInGeneral(AbsID);
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string DeleteDocs([FromBody] string ids)
	{
		return UnitOfWork.SalesForeCast.DeleteDocs(ids);
	}
}
