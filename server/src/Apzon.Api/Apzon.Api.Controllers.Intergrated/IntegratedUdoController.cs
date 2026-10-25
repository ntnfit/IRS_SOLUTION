using System.Data;
using System.Threading.Tasks;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Documents;

namespace Apzon.Api.Controllers.Intergrated;

[ValidateLoginSapExceptionHandling]
public class IntegratedUdoController : BaseApiController
{
	[HttpGet]
	public DataTable GetAllChildCode(string udoCode)
	{
		return UnitOfWork.IntegratedUdo.GetAllChildCode(udoCode);
	}

	[HttpGet]
	public DataTable LoadUdoCombo()
	{
		return UnitOfWork.IntegratedUdo.LoadUdoCombo();
	}

	[HttpPost]
	public DataTable Search([FromBody] DataTable doc)
	{
		return UnitOfWork.IntegratedUdo.Search(doc);
	}

	[HttpPost]
	public DataSet SearchLine(DataTable dt)
	{
		return UnitOfWork.IntegratedUdo.SearchLine(dt);
	}

	[HttpGet]
	public bool CheckExists(string id, string code, string type)
	{
		return UnitOfWork.IntegratedUdo.CheckExists(id, code, type);
	}

	[HttpGet]
	public DataSet GetDocGridStruct(string udoCode)
	{
		return UnitOfWork.IntegratedUdo.GetDocGridStruct(udoCode);
	}

	[HttpGet]
	public DataSet GetDocLineGridStruct(string udoCode)
	{
		return UnitOfWork.IntegratedUdo.GetDocLineGridStruct(udoCode);
	}

	[HttpPost]
	public async Task<string> SaveADoc([FromBody] DocumentsImpl doc)
	{
		return "";
	}
}
