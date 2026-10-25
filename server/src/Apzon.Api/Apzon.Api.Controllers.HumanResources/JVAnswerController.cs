using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.HumanResources;

public class JVAnswerController : BaseApiController
{
	[HttpGet]
	public DataTable LoadQuestion()
	{
		return UnitOfWork.JVAnswer.LoadQuestion();
	}

	[HttpGet]
	public DataTable LoadAnswer(int id)
	{
		return UnitOfWork.JVAnswer.LoadAnswer(id);
	}

	[HttpPost]
	public HttpResult SaveQuestion(DataSet ds)
	{
		return UnitOfWork.JVAnswer.SaveData(ds);
	}
}
