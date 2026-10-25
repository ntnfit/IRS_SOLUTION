using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Administrator.SystemInitialization;

public class DocumentNumberingController : BaseApiController
{
	[HttpGet]
	public DataTable LoadDoc(int userId)
	{
		return UnitOfWork.DocumentNumbering.LoadDoc(userId);
	}

	[HttpPost]
	public HttpResult Save(DataSet dt)
	{
		if (Global.LoginSap)
		{
			return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
		}
		return UnitOfWork.DocumentNumbering.Save(dt);
	}

	[HttpGet]
	public DataTable GetDataSeries(int userId)
	{
		return UnitOfWork.DocumentNumbering.GetDataSeries(userId);
	}
}
