using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.ProductionAddon;

public class ControlKeyController : BaseApiController
{
	[HttpGet]
	public DataTable GetDocs()
	{
		return UnitOfWork.ControlKey.GetDocs();
	}

	[HttpGet]
	public DataTable GetDoc()
	{
		return UnitOfWork.ControlKey.GetDoc();
	}

	[HttpPost]
	public DataTable CheckExistKeyCode([FromBody] string keycode)
	{
		return UnitOfWork.ControlKey.CheckExistKeyCode(keycode);
	}

	[HttpPost]
	public string SaveDocs(DataSet ds)
	{
		return UnitOfWork.ControlKey.SaveDocs(ds);
	}

	[HttpPost]
	public string SaveDoc(DataTable dt)
	{
		return UnitOfWork.ControlKey.SaveDoc(dt);
	}

	[HttpGet]
	public DataTable GetListKeyCode()
	{
		return UnitOfWork.ControlKey.GetListKeyCode();
	}

	[HttpGet]
	public int GetMaxIndicatorID(string keycode)
	{
		return UnitOfWork.ControlKey.GetMaxIndicatorID(keycode);
	}

	[HttpPost]
	public string DeleteDoc([FromBody] DataTable dts)
	{
		return UnitOfWork.ControlKey.DeleteDoc(dts);
	}

	[HttpPost]
	public string DeleteDocs([FromBody] string ids)
	{
		return UnitOfWork.ControlKey.DeleteDocs(ids);
	}

	[HttpPost]
	public string DeleteDocsAlone([FromBody] string ids)
	{
		return UnitOfWork.ControlKey.DeleteDocsAlone(ids);
	}

	[HttpPost]
	public DataTable GetDocSaved([FromBody] string keycode)
	{
		return UnitOfWork.ControlKey.GetDocSaved(keycode);
	}
}
