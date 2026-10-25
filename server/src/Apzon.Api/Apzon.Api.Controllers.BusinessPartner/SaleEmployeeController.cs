using System.Data;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.BusinessPartners;

namespace Apzon.Api.Controllers.BusinessPartner;

public class SaleEmployeeController : BaseApiController
{
	[HttpGet]
	public DataTable ShowSaleEmployee()
	{
		return UnitOfWork.SaleEmployee.ShowSaleEmployee();
	}

	[HttpGet]
	public int GetMaxCode()
	{
		return UnitOfWork.SaleEmployee.GetMaxCode();
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string SaveDoc([FromBody] SaleEmployeesImpl doc)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			BrokerService.GetUnitOfWork(SessionId);
			return "Chức năng đang được cập nhật!";
		}
		return UnitOfWork.SaleEmployee.SaveDoc(doc);
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string DeleteDoc([FromBody] int code)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			BrokerService.GetUnitOfWork(SessionId);
			return "Chức năng đang được cập nhật!";
		}
		return UnitOfWork.SaleEmployee.DeleteDoc(code);
	}
}
