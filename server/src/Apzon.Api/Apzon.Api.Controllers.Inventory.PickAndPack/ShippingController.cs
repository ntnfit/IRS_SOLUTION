using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Inventory.PickAndPack;

public class ShippingController : BaseApiController
{
	[HttpPost]
	public DataTable GetDoc([FromBody] DataTable pr)
	{
		return UnitOfWork.Shipping.GetDoc(pr);
	}

	[HttpGet]
	public DataTable GetLines(int id)
	{
		return UnitOfWork.Shipping.GetLines(id);
	}

	[HttpGet]
	public int GetNextId()
	{
		return UnitOfWork.Shipping.GetNextId();
	}

	[HttpGet]
	public DataTable GetLineByPickNo(int id)
	{
		return UnitOfWork.Shipping.GetLineByPickNo(id);
	}

	[HttpGet]
	public DataTable GetLineByPackNo(int id)
	{
		return UnitOfWork.Shipping.GetLineByPackNo(id);
	}

	[HttpGet]
	public DataSet ChoosePickList()
	{
		return UnitOfWork.Shipping.ChoosePickList();
	}

	[HttpGet]
	public DataSet ChoosePacking()
	{
		return UnitOfWork.Shipping.ChoosePacking();
	}

	[HttpGet]
	public DataSet ChooseBatch()
	{
		return UnitOfWork.Shipping.ChooseBatch();
	}

	[HttpPost]
	public async Task<HttpResult> Save([FromBody] DataSet ds)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "INVALID SESSION ID");
			}
			return await BrokerService.GetUnitOfWork(SessionId).Shipping.Save(ds, SessionUserInfo.SessionDiServer);
		}
		return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
	}

	[HttpPost]
	public HttpResult Close([FromBody] int docEntry)
	{
		return UnitOfWork.Shipping.Close(docEntry);
	}

	[HttpPost]
	public HttpResult Cancel([FromBody] int docEntry)
	{
		return UnitOfWork.Shipping.Cancel(docEntry);
	}

	[HttpGet]
	public DataSet ChooseList(ApzObjectTypes type, string cardcode)
	{
		return UnitOfWork.Shipping.ChooseList(type, cardcode);
	}

	[HttpPost]
	public DataTable Copy([FromBody] Tuple<ApzObjectTypes, ApzObjectTypes, List<int>> model)
	{
		return UnitOfWork.Shipping.Copy(model.Item1, model.Item2, model.Item3);
	}

	[HttpPost]
	public DataTable CopyHeader([FromBody] Tuple<ApzObjectTypes, ApzObjectTypes, List<int>> model)
	{
		return UnitOfWork.Shipping.CopyHeader(model.Item1, model.Item2, model.Item3);
	}

	[HttpGet]
	public int GetOrderEntry(ApzObjectTypes type, int entry)
	{
		return UnitOfWork.Shipping.GetOrderEntry(type, entry);
	}
}
