using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Production;

namespace Apzon.Api.Controllers.Production;

public class MaterialShortageReportController : BaseApiController
{
	[HttpPost]
	public DataTable Run([FromBody] MaterialShortageReport_Impl input)
	{
		return UnitOfWork.MaterialShortageReport.Run(input);
	}

	[HttpGet]
	public DataSet SearchData(Enums.NavigateType type, int code)
	{
		return UnitOfWork.MaterialShortageReport.SearchData(type, code);
	}

	[HttpGet]
	public HttpResult GetNewId()
	{
		return UnitOfWork.MaterialShortageReport.GetNewId();
	}

	[HttpGet]
	public DataTable LoadAtpType()
	{
		return UnitOfWork.MaterialShortageReport.LoadAtpType();
	}

	[HttpGet]
	public DataTable GetDocStructure()
	{
		return UnitOfWork.MaterialShortageReport.GetDocStructure();
	}

	[HttpPost]
	public HttpResult AddNew(DataSet ds)
	{
		return UnitOfWork.MaterialShortageReport.Save(ds, Enums.TransactionType.A, "");
	}

	[HttpPost]
	public HttpResult Update(DataSet ds)
	{
		return UnitOfWork.MaterialShortageReport.Save(ds, Enums.TransactionType.U, "");
	}

	[HttpGet]
	public DataTable GetStructureGenPurchase()
	{
		return UnitOfWork.MaterialShortageReport.GetStructureGenPurchase();
	}

	[HttpGet]
	public DataTable GetSeriesOrderAndRequest(string type)
	{
		return UnitOfWork.Schedule.GetSeriesOrderAndRequest(type);
	}

	[HttpGet]
	public DataTable GetDataGenPurchase(string input, int code, string user)
	{
		return UnitOfWork.MaterialShortageReport.GetDataGenPurchase(input, code, user);
	}

	[HttpGet]
	public string CheckInputData(string type, int code, string genType)
	{
		return UnitOfWork.MaterialShortageReport.CheckInputData(type, code, genType);
	}

	[HttpGet]
	public DataSet GetListMaterialShortageReport(int bplId, string type, string status)
	{
		return UnitOfWork.MaterialShortageReport.GetListMaterialShortageReport(bplId, type, status);
	}
}
