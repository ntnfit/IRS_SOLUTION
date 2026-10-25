using System.Collections.Generic;
using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.DataServices;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Administrator.Setup.General;

public class VatGroupsController : BaseApiController
{
	[HttpGet]
	public List<VatGroupsImpl> Cfl()
	{
		return UnitOfWork.VatGroups.Cfl();
	}

	[HttpGet]
	public List<VatGroupsImpl> Cfl(string Category)
	{
		return UnitOfWork.VatGroups.Cfl(Category);
	}

	[HttpGet]
	public DataTable GetVatGroupI()
	{
		return UnitOfWork.VatGroups.GetVatGroupI();
	}

	[HttpGet]
	public DataTable GetVatGroupO()
	{
		return UnitOfWork.VatGroups.GetVatGroupO();
	}

	[HttpGet]
	public DataTable GetRate(VatGroupsImpl doc)
	{
		return UnitOfWork.VatGroups.GetRate(doc);
	}

	[HttpGet]
	public DataTable ShowVatGroupI()
	{
		return UnitOfWork.VatGroups.ShowVatGroupI();
	}

	[HttpGet]
	public DataTable ShowVatGroupO()
	{
		return UnitOfWork.VatGroups.ShowVatGroupO();
	}

	[HttpPost]
	public DataTable SearchVatGroupI([FromBody] VatGroupsImpl doc)
	{
		return UnitOfWork.VatGroups.SearchVatGroupI(doc);
	}

	[HttpPost]
	public DataTable SearchVatGroupO([FromBody] VatGroupsImpl doc)
	{
		return UnitOfWork.VatGroups.SearchVatGroupO(doc);
	}

	[HttpGet]
	public DataTable ShowRateByCodeVatGroup(string codeVatGroup)
	{
		return UnitOfWork.VatGroups.ShowRateByCodeVatGroup(codeVatGroup);
	}

	[HttpPost]
	public HttpResult SaveVat([FromBody] DataSet ds)
	{
		return UnitOfWork.VatGroups.SaveVat(ds);
	}

	[HttpPost]
	public string SaveSap([FromBody] List<VatGroupsImpl> doc)
	{
		if (string.IsNullOrEmpty(SessionId))
		{
			return "Invalid session";
		}
		IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
		if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
		{
			return unitOfWork.VatGroups.Save(doc, SessionUserInfo.SessionDiServer);
		}
		return "Invalid session";
	}

	[HttpGet]
	public DataTable GetData()
	{
		return UnitOfWork.VatGroups.GetData();
	}

	[HttpGet]
	public string CheckCode(string code)
	{
		return UnitOfWork.VatGroups.CheckCode(code);
	}

	[HttpGet]
	public DataTable GetAllLine_VTG1()
	{
		return UnitOfWork.VatGroups.GetAllLine_VTG1();
	}

	[HttpGet]
	public DataTable GetDataOVTG(string vatType)
	{
		return UnitOfWork.VatGroups.GetDataOVTG(vatType);
	}

	[HttpPost]
	public string Delete([FromBody] string code)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
			{
				return unitOfWork.VatGroups.Delete(code, SessionUserInfo.SessionDiServer);
			}
			return "Invalid session";
		}
		return UnitOfWork.VatGroups.Delete(code);
	}

	[HttpGet]
	public bool Exists(string code)
	{
		return UnitOfWork.VatGroups.Exists(code);
	}

	[HttpGet]
	public DataTable GetStructHeader()
	{
		return UnitOfWork.VatGroups.GetStructHeader();
	}

	[HttpGet]
	public DataTable GetStructLines()
	{
		return UnitOfWork.VatGroups.GetStructLines();
	}
}
