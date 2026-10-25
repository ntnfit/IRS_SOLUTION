using System.Data;
using System.Threading.Tasks;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.DataServices;
using Apzon.Entities.Models.Documents;

namespace Apzon.Api.Controllers.Intergrated;

[ValidateLoginSapExceptionHandling]
public class IntegratedController : BaseApiController
{
	[HttpPost]
	public DataTable Search([FromBody] DataTable doc)
	{
		return UnitOfWork.Integrated.Search(doc);
	}

	[HttpGet]
	public DataTable SearchLine(int objType, string id, string isIns, string isRecFromProd)
	{
		return UnitOfWork.Integrated.SearchLine(objType, id, isIns, isRecFromProd);
	}

	[HttpGet]
	public DataTable GetListConfig()
	{
		return UnitOfWork.Integrated.GetListConfig();
	}

	[HttpPost]
	public DataTable CheckExists([FromBody] DataTable dt)
	{
		return UnitOfWork.Integrated.CheckExists(dt);
	}

	[HttpGet]
	public DataSet GetDocGridStruct()
	{
		return UnitOfWork.Integrated.GetDocGridStruct();
	}

	[HttpGet]
	public DataSet GetDocLineGridStruct()
	{
		return UnitOfWork.Integrated.GetDocLineGridStruct();
	}

	[HttpPost]
	public async Task<string> SaveADoc([FromBody] DocumentsImpl doc)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "INVALIDSESSION";
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
			{
				return await unitOfWork.Intergrated.Save(doc, SessionUserInfo.SessionDiServer);
			}
			return "INVALIDSESSION";
		}
		return UnitOfWork.Integrated.SaveADoc(doc);
	}

	[HttpGet]
	public string CheckIRSAuthorizations()
	{
		return UnitOfWork.Integrated.CheckIRSAuthorizations();
	}

	[HttpPost]
	public string MapCodeSaveADoc([FromBody] DataTable doc)
	{
		return UnitOfWork.Integrated.MapCodeSaveADoc(doc);
	}
}
