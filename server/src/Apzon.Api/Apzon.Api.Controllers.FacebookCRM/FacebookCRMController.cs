using System;
using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.FacebookCRM;

public class FacebookCRMController : BaseApiController
{
	[HttpGet]
	public DataTable LoadMessages(string pageId)
	{
		return UnitOfWork.FacebookCRM.LoadMessages(pageId);
	}

	[HttpGet]
	public DataTable LoadMessagesForCustomer(string pageId, string customerId)
	{
		return UnitOfWork.FacebookCRM.LoadMessagesForCustomer(pageId, customerId);
	}

	[HttpGet]
	public DataTable LoadCustomers(string pageId)
	{
		return UnitOfWork.FacebookCRM.LoadCustomers(pageId);
	}

	[HttpGet]
	public DataTable LoadPageInfo(string pageId)
	{
		return UnitOfWork.FacebookCRM.LoadPageInfo(pageId);
	}

	[HttpGet]
	public DataTable LoadPageInfoFromUser(string userId)
	{
		return UnitOfWork.FacebookCRM.LoadPageInfoFromUser(userId);
	}

	[HttpGet]
	public DataTable LoadOrderAndItemCode()
	{
		return UnitOfWork.FacebookCRM.LoadOrderAndItemCode();
	}

	[HttpGet]
	public int GetDocNumByDocEntry(int docEntry)
	{
		return UnitOfWork.FacebookCRM.GetDocNumByDocEntry(docEntry);
	}

	[HttpPost]
	public string SaveUser([FromBody] DataTable dt)
	{
		return UnitOfWork.FacebookCRM.SaveUser(dt);
	}

	[HttpPost]
	public string SavePageList([FromBody] DataTable dt)
	{
		return UnitOfWork.FacebookCRM.SavePageList(dt);
	}

	[HttpPost]
	public string SaveMessages([FromBody] DataTable dt)
	{
		return UnitOfWork.FacebookCRM.SaveMessages(dt);
	}

	[HttpPost]
	public string SaveCustomers([FromBody] DataSet dt)
	{
		return UnitOfWork.FacebookCRM.SaveCustomers(dt);
	}

	[HttpPost]
	public string ActiveMessages([FromBody] DataTable dt)
	{
		return UnitOfWork.FacebookCRM.ActiveMessages(dt);
	}

	[HttpPost]
	public string ChangePageStatus([FromBody] Tuple<string, DateTime> pageId)
	{
		return UnitOfWork.FacebookCRM.ChangePageStatus(pageId.Item1, pageId.Item2);
	}
}
