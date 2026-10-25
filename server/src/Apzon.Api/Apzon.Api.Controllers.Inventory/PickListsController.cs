using System.Threading.Tasks;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Inventory;

namespace Apzon.Api.Controllers.Inventory;

public class PickListsController : BaseApiController
{
	[HttpPost]
	public async Task<HttpResult> Save([FromBody] PickListsImpl doc)
	{
		if (string.IsNullOrEmpty(SessionId))
		{
			return new HttpResult
			{
				Message = "INVALIDSESSION",
				Content = null
			};
		}
		return await BrokerService.GetUnitOfWork(SessionId).PickLists.Save(doc, SessionUserInfo.SessionDiServer);
	}
}
