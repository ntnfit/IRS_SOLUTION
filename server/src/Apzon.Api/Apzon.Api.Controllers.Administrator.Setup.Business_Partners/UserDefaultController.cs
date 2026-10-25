using System.Data;
using System.Web.Http;
using System.Web.Mvc;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Administrator.Setup.Business_Partners;

public class UserDefaultController : BaseApiController
{
	[System.Web.Mvc.HttpGet]
	public DataTable GetOUDG(string code)
	{
		return UnitOfWork.UserDefault.GetOUDG(code);
	}

	[System.Web.Mvc.HttpGet]
	public DataTable GetUDG1(string code, string type)
	{
		return UnitOfWork.UserDefault.GetUDG1(code, type);
	}

	[System.Web.Mvc.HttpGet]
	public DataTable GetUDG2(string code)
	{
		return UnitOfWork.UserDefault.GetUDG2(code);
	}

	[System.Web.Mvc.HttpPost]
	public string Save([FromBody] DataSet ds)
	{
		return UnitOfWork.UserDefault.Save(ds);
	}

	[System.Web.Mvc.HttpPost]
	public string Remove([FromBody] string code)
	{
		return UnitOfWork.UserDefault.Remove(code);
	}

	[System.Web.Mvc.HttpPost]
	public DataTable Search([FromBody] DataTable dtFind)
	{
		return UnitOfWork.UserDefault.Search(dtFind);
	}

	[System.Web.Mvc.HttpGet]
	public DataTable GetTableColumns(string tableName)
	{
		return UnitOfWork.UserDefault.GetTableColumns(tableName);
	}

	[System.Web.Mvc.HttpGet]
	public DataTable GetRecord(string tbl, string key, string TypeRecord)
	{
		return UnitOfWork.UserDefault.GetRecord(tbl, key, TypeRecord);
	}
}
