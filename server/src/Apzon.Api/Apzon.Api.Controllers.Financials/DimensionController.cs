using System.Data;
using System.Threading.Tasks;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.DataServices;

namespace Apzon.Api.Controllers.Financials;

public class DimensionController : BaseApiController
{
	[HttpPost]
	public async Task<string> Save([FromBody] DataTable dtDoc)
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
				return await unitOfWork.Dimension.Save(SessionUserInfo.SessionDiServer, dtDoc);
			}
			return "INVALIDSESSION";
		}
		return UnitOfWork.Dimension.Save(dtDoc);
	}

	[HttpGet]
	public DataTable GetData()
	{
		return UnitOfWork.Dimension.GetData();
	}

	[HttpGet]
	public DataTable GetDisableDim()
	{
		return UnitOfWork.Dimension.GetDisableDim();
	}
}
