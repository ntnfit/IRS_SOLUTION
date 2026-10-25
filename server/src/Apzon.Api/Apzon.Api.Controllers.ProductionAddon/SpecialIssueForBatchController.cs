using System.Collections.Generic;
using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.DataServices;
using Apzon.Entities.Models.Documents;

namespace Apzon.Api.Controllers.ProductionAddon;

public class SpecialIssueForBatchController : BaseApiController
{
	[HttpPost]
	public DataTable InsertRowByBatch([FromBody] string batIssue)
	{
		return UnitOfWork.SpecialIssueForBatch.InsertRowByBatch(batIssue);
	}

	[HttpPost]
	public DataTable InsertRowByBatchToReceipt([FromBody] string batIssue)
	{
		return UnitOfWork.SpecialIssueForBatch.InsertRowByBatchToReceipt(batIssue);
	}

	[HttpPost]
	public string SaveDoc([FromBody] List<DocumentsImpl> doc)
	{
		string result = "";
		if (Global.LoginSap)
		{
			new Transaction();
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			BrokerService.GetUnitOfWork(SessionId);
			return result;
		}
		return "Error!";
	}

	public string SaveDocReceipt([FromBody] List<DocumentsImpl> doc)
	{
		if (Global.LoginSap)
		{
			new Transaction();
			string result = "";
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			BrokerService.GetUnitOfWork(SessionId);
			return result;
		}
		return "Error!";
	}
}
