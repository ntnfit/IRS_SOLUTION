using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Administrator.SystemInitialization;

public class MenuStructureController : BaseApiController
{
	[HttpGet]
	public DataSet GetListMenu(string lngCode = "")
	{
		return UnitOfWork.MenuStructure.GetListMenu(lngCode);
	}

	[HttpPost]
	public string AddNewFolder([FromBody] DataTable dt)
	{
		return UnitOfWork.MenuStructure.AddNewFolder(dt);
	}

	[HttpPost]
	public string RelocateFolder([FromBody] DataTable dt)
	{
		return UnitOfWork.MenuStructure.RelocateFolder(dt);
	}

	[HttpPost]
	public string Rename([FromBody] DataTable dt)
	{
		return UnitOfWork.MenuStructure.Rename(dt);
	}

	[HttpPost]
	public string DeleteFolder([FromBody] int id)
	{
		return UnitOfWork.MenuStructure.DeleteFolder(id);
	}

	[HttpGet]
	public DataTable GetListCustomizeMenu()
	{
		return UnitOfWork.MenuStructure.GetListCustomizeMenu();
	}
}
