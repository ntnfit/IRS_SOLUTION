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
public class IntegratedInventoryTransferController : BaseApiController
{
	[HttpPost]
	public DataSet Search([FromBody] DataTable doc)
	{
		return UnitOfWork.IntegratedInventoryTransfer.Search(doc);
	}

	[HttpGet]
	public DataSet SearchLine(string id)
	{
		return UnitOfWork.IntegratedInventoryTransfer.SearchLine(id);
	}

	[HttpGet]
	public DataTable SearchLineForSave(string id)
	{
		return UnitOfWork.IntegratedInventoryTransfer.SearchLineForSave(id);
	}

	[HttpPost]
	public DataTable CheckExists([FromBody] DataTable dt)
	{
		return UnitOfWork.IntegratedInventoryTransfer.CheckExists(dt);
	}

	[HttpPost]
	public string SaveADocTransfer([FromBody] DocumentsImpl doc)
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
				return unitOfWork.StockTransfer.Save(doc, SessionUserInfo.SessionDiServer, isDraft: false).Message;
			}
			return "INVALIDSESSION";
		}
		return "INVALIDSESSION";
	}

	[HttpPost]
	public async Task<string> SaveADocRequest([FromBody] DocumentsImpl doc)
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
				return (await unitOfWork.StockTransferRequest.Save(doc, isDraft: false, SessionUserInfo.SessionDiServer)).Message;
			}
			return "INVALIDSESSION";
		}
		return "INVALIDSESSION";
	}
}
