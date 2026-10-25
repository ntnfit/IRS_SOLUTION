using System.Data;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Purchase;

[ValidateLoginSapExceptionHandling]
public class ProcurementConfirmController : BaseApiController
{
	[HttpGet]
	public DataSet LoadPreferredVendor(string baseType, string userName, string branch)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return null;
			}
			return BrokerService.GetUnitOfWork(SessionId).ProcurementConfirm.LoadPreferredVendor(baseType, userName, branch);
		}
		return null;
	}

	[HttpGet]
	public DataSet LoadItemLine(string baseType, string docEntry, string userName, string branch)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return null;
			}
			return BrokerService.GetUnitOfWork(SessionId).ProcurementConfirm.LoadItemLine(baseType, docEntry, userName, branch);
		}
		return null;
	}
}
