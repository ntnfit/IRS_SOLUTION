using System.Data;
using System.Web.Http;
using System.Web.Http.Controllers;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.Api.Controllers.Administrator.Setup;

public class ThirdPartyTokenController : BaseApiController
{
	private IUnitOfWork _unitOfWork;

	protected override void Initialize(HttpControllerContext controllerContext)
	{
		base.Initialize(controllerContext);
		_unitOfWork = UnitOfWorkFactory.GetUnitOfWork(Global.CommonConnectionString, Global.DatabaseSystemType);
	}

	[HttpGet]
	public DataTable GetTokens(string tokenName, string remarks, string status)
	{
		return _unitOfWork.ThirdPartyToken.GetData(tokenName, remarks, status);
	}

	[HttpGet]
	public HttpResult GetMaxAbsEntry()
	{
		return _unitOfWork.ThirdPartyToken.GetMaxEntry();
	}

	[HttpPost]
	public HttpResult SaveTokens([FromUri] string action, [FromBody] DataTable dt)
	{
		return _unitOfWork.ThirdPartyToken.Saves(dt, action);
	}

	[HttpPost]
	public HttpResult DeleteDoc([FromBody] int absEntry)
	{
		return _unitOfWork.ThirdPartyToken.DeleteDoc(absEntry);
	}
}
