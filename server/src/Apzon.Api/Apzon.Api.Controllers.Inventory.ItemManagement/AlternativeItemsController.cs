using System;
using System.Data;
using System.Threading.Tasks;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices;

namespace Apzon.Api.Controllers.Inventory.ItemManagement;

public class AlternativeItemsController : BaseApiController
{
	[HttpGet]
	public DataTable GetItemNameByItemCode(string itemCode)
	{
		return UnitOfWork.AlternativeItems.GetItemNameByItemCode(itemCode);
	}

	[HttpPost]
	public DataTable GetDataDoc(Tuple<string, Enums.NavigateType> tuple)
	{
		return UnitOfWork.AlternativeItems.GetDataDoc(tuple);
	}

	[HttpGet]
	public DataTable GetData(string originItem)
	{
		return UnitOfWork.AlternativeItems.GetData(originItem);
	}

	[HttpPost]
	public async Task<string> Save(Tuple<string, DataTable> tuple)
	{
		if (string.IsNullOrEmpty(SessionId))
		{
			return "INVALIDSESSION";
		}
		IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
		if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
		{
			return await unitOfWork.AlternativeItem.Save(SessionUserInfo.SessionDiServer, tuple);
		}
		return "INVALIDSESSION";
	}

	[HttpPost]
	public async Task<string> DeleteData([FromBody] string itemNo)
	{
		if (string.IsNullOrEmpty(SessionId))
		{
			return "INVALIDSESSION";
		}
		IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
		if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
		{
			return await unitOfWork.AlternativeItem.DeleteData(SessionUserInfo.SessionDiServer, itemNo);
		}
		return "INVALIDSESSION";
	}

	[HttpGet]
	public DataSet GetDataCfl(string itemCode, string userName, int branch, string cardCode, string docDate)
	{
		return UnitOfWork.AlternativeItems.GetDataCfl(itemCode, userName, branch, cardCode, Function.ParseDateTimes(docDate, "yyyyMMdd"));
	}
}
