using System;
using System.Collections.Generic;
using System.Data;
using Apzon.Entities.Models.Documents;

namespace Apzon.Services.Interfaces.Purchase;

public interface IMKPurchaseOrder
{
	DataSet GetDoc(int docEntry);

	string Save(Tuple<PurchaseOrder, List<PurchaseOrderLine>> doc);

	string UpdateStatus(DataSet ds);

	string Approval(DataTable dt);
}
