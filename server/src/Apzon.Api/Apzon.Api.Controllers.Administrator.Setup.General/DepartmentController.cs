using System.Data;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Administrator;

namespace Apzon.Api.Controllers.Administrator.Setup.General;

public class DepartmentController : BaseApiController
{
	[HttpGet]
	public DataTable ShowDepartment()
	{
		return UnitOfWork.Department.ShowDepartment();
	}

	[HttpGet]
	public int GetMaxCode()
	{
		return UnitOfWork.Department.GetMaxCode();
	}

	[HttpPost]
	public DataTable Search([FromBody] DepartmentImpl doc)
	{
		return UnitOfWork.Department.Search(doc);
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
			return "Chức năng đang được xây dựng!";
		}
		return UnitOfWork.Department.DeleteDoc(code);
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string SaveDoc([FromBody] DepartmentImpl doc)
	{
		return UnitOfWork.Department.SaveDoc(doc);
	}
}
