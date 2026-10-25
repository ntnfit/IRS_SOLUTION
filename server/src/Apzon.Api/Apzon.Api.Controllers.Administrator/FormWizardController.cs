using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Administrator;

public class FormWizardController : BaseApiController
{
	[HttpGet]
	public DataTable GetValid()
	{
		return UnitOfWork.FormWizard.GetValid();
	}

	[HttpGet]
	public DataSet GetById(int templateId)
	{
		return UnitOfWork.FormWizard.GetById(templateId);
	}

	[HttpGet]
	public DataTable GetAll()
	{
		return UnitOfWork.FormWizard.GetAll();
	}

	[HttpPost]
	public HttpResult SaveData(DataSet ds)
	{
		return UnitOfWork.FormWizard.SaveData(ds);
	}

	[HttpPost]
	public HttpResult Remove([FromBody] int absEntry)
	{
		return UnitOfWork.FormWizard.Remove(absEntry);
	}
}
