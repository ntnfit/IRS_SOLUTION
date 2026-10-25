using System;
using System.Data;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.HumanResources;

public class JVQuestionController : BaseApiController
{
	[HttpGet]
	public DataTable LoadQuestionGroup()
	{
		return UnitOfWork.JVQuestion.LoadQuestionGroup();
	}

	[HttpGet]
	public DataTable LoadQuestion()
	{
		return UnitOfWork.JVQuestion.LoadQuestion();
	}

	[HttpGet]
	public DataTable LoadAnswer(int id)
	{
		return UnitOfWork.JVQuestion.LoadAnswer(id);
	}

	[HttpGet]
	public DataTable CheckExist(int ID, string checkTable, string checkField)
	{
		Tuple<int, string, string> iD = new Tuple<int, string, string>(ID, checkTable, checkField);
		return UnitOfWork.JVQuestion.CheckExist(iD);
	}

	[HttpGet]
	public DataTable CheckQuestion(int ID)
	{
		return UnitOfWork.JVQuestion.CheckQuestion(ID);
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string DeleteQuestion([FromBody] int ID)
	{
		return UnitOfWork.JVQuestion.DeleteQuestion(ID);
	}

	[HttpGet]
	public DataTable CheckQuestionGroup(int ID)
	{
		return UnitOfWork.JVQuestion.CheckQuestionGroup(ID);
	}

	[HttpPost]
	public HttpResult SaveQuestion(Tuple<DataTable, string> dt)
	{
		return UnitOfWork.JVQuestion.SaveData(dt);
	}

	[HttpPost]
	public HttpResult SaveQuestionGroup(DataTable dt)
	{
		Tuple<DataTable, string> dt2 = new Tuple<DataTable, string>(dt, "APZ_OQLG");
		return UnitOfWork.JVQuestion.SaveData(dt2);
	}

	[HttpGet]
	public DataTable LoadBenefit()
	{
		return UnitOfWork.JVQuestion.LoadBenefit();
	}

	[HttpGet]
	public DataTable CheckBenefit(int ID)
	{
		return UnitOfWork.JVQuestion.CheckBenefit(ID);
	}

	[HttpPost]
	public HttpResult SaveBenefit(Tuple<DataTable, string> dt)
	{
		return UnitOfWork.JVQuestion.SaveBenefit(dt);
	}
}
