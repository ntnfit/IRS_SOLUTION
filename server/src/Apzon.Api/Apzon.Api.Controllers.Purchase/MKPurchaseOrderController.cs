using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Entities.Models.Documents;

namespace Apzon.Api.Controllers.Purchase;

public class MKPurchaseOrderController : BaseApiController
{
	public ApzObjectTypes FormObjectTypes = ApzObjectTypes.oPurchaseOrders;

	[HttpGet]
	public DataSet GetDoc(int docEntry)
	{
		return UnitOfWork.MKPurchaseOrder.GetDoc(docEntry);
	}

	[HttpPost]
	public string Save([FromBody] Tuple<PurchaseOrder, List<PurchaseOrderLine>> doc)
	{
		return UnitOfWork.MKPurchaseOrder.Save(doc);
	}

	[HttpPost]
	public string UpdateStatus([FromBody] DataSet ds)
	{
		return UnitOfWork.MKPurchaseOrder.UpdateStatus(ds);
	}

	[HttpPost]
	public string Approval([FromBody] DataTable dt)
	{
		return UnitOfWork.MKPurchaseOrder.Approval(dt);
	}
}
