using System;
using System.Data;
using System.Threading.Tasks;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;

namespace Apzon.Api.Controllers.Inventory.InventoryTransaction;

public class ReceiptIssueBinController : BaseApiController
{
	[HttpPost]
	[QueueModifiedAction]
	public async Task<HttpResult> SaveDoc([FromBody] Tuple<DocumentsImpl, ApzObjectTypes> model)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			DocumentsImpl item = model.Item1;
			ApzObjectTypes item2 = model.Item2;
			return await unitOfWork.Documents.AddNew(item, SessionUserInfo.SessionDiServer, item2);
		}
		return new HttpResult(Enums.MessageCode.Error, "Unknown error");
	}

	[HttpGet]
	public DataSet GetData(string batchNo, string bin, ApzObjectTypes objtype)
	{
		return UnitOfWork.Commons.ReceiptIssueBinData(batchNo, bin, objtype);
	}
}
