using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Administrator;

public class MailSettingController : BaseApiController
{
	[HttpPost]
	public string SaveDoc([FromBody] DataSet ds)
	{
		return UnitOfWork.MailSetting.SaveDoc(ds);
	}

	[HttpGet]
	public DataTable GetDoc()
	{
		return UnitOfWork.MailSetting.GetDoc();
	}

	[HttpGet]
	public string DeleteDoc(string code)
	{
		return UnitOfWork.MailSetting.DeleteDoc(code);
	}
}
