using System;
using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Sales;

public interface ISalesControl
{
	DataSet LoadSalesControl(string userCode, string ownerName, string salesEmp, DateTime fromDate, DateTime toDate, string docStatus, string bpCode, string saleType, string userName, int branch);

	DataSet LoadSalesControl(DataTable dt);

	string UpdateDeltimeConfirm(int docentry, int lineid, DateTime deltime, string type, string remark, string username);

	DataSetResult GetOrderByBarcode(string barcode);
}
