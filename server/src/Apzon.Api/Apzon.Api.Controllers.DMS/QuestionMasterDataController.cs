using System.Data;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.DMS;

[ValidateLoginSapExceptionHandling]
public class QuestionMasterDataController : BaseApiController
{
	[HttpGet]
	public DataTable LoadData(int id, string fromDate, string toDate, string status)
	{
		return UnitOfWork.QuestionMasterData.LoadData(id, fromDate, toDate, status);
	}

	[HttpGet]
	public DataTable GetDataById(int Id)
	{
		return UnitOfWork.QuestionMasterData.GetDataById(Id);
	}

	[HttpPost]
	public HttpResult Add([FromBody] DataSet ds)
	{
		return UnitOfWork.QuestionMasterData.Save(ds, "A");
	}

	[HttpPost]
	public HttpResult Update([FromBody] DataSet ds)
	{
		return UnitOfWork.QuestionMasterData.Save(ds, "U");
	}

	[HttpPost]
	public string DeleteDocs([FromBody] string Id)
	{
		return UnitOfWork.QuestionMasterData.DeleteDocs(Id);
	}

	[HttpGet]
	public int GetNo()
	{
		return UnitOfWork.QuestionMasterData.GetNo();
	}

	[HttpGet]
	public DataTable FindNextLastFrist(int id, int type)
	{
		return UnitOfWork.QuestionMasterData.FindNextLastFrist(id, type);
	}
}
