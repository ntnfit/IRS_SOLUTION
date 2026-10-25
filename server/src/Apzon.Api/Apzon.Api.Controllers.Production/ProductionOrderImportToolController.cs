using System.Data;
using System.Threading.Tasks;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons.Enums;
using Apzon.DataServices;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;

namespace Apzon.Api.Controllers.Production;

[ValidateLoginSapExceptionHandling]
public class ProductionOrderImportToolController : BaseApiController
{
	[HttpPost]
	public DataTable Search([FromBody] DataTable dt)
	{
		return UnitOfWork.ProductionOrderImportTool.Search(dt);
	}

	[HttpGet]
	public DataTable GetTableStructure(string tabName)
	{
		return UnitOfWork.ProductionOrderImportTool.GetTableStructure(tabName);
	}

	[HttpGet]
	public DataTable GetAllBatchSerial(string itemCode, string fromWhsCode, string toWhsCode)
	{
		return UnitOfWork.ProductionOrderImportTool.GetAllBatchSerial(itemCode, fromWhsCode, toWhsCode);
	}

	[HttpGet]
	public DataTable GetTransferedBinData(int docEntry, int lineNum)
	{
		return UnitOfWork.ProductionOrderImportTool.GetTransferedBinData(docEntry, lineNum);
	}

	[HttpGet]
	public DataTable GetAllBinData(string itemCode, string whsCode)
	{
		return UnitOfWork.ProductionOrderImportTool.GetAllBinData(itemCode, whsCode);
	}

	[HttpGet]
	public bool IsBinWarehouse(string whsCode)
	{
		return UnitOfWork.ProductionOrderImportTool.IsBinWarehouse(whsCode);
	}

	[HttpGet]
	public DataTable GetBinWarehouse()
	{
		return UnitOfWork.ProductionOrderImportTool.GetBinWarehouse();
	}

	[HttpGet]
	public DataTable GetBOMHeader(string code)
	{
		return UnitOfWork.ProductionOrderImportTool.GetBOMHeader(code);
	}

	[HttpGet]
	public DataTable GetItemCodeAndName()
	{
		return UnitOfWork.ProductionOrderImportTool.GetItemCodeAndName();
	}

	[HttpGet]
	public DataTable RCGetProductionDetails(string barCode)
	{
		return UnitOfWork.ProductionOrderImportTool.RCGetProductionDetails(barCode);
	}

	[HttpGet]
	public DataTable GetEditTableEnableColumns()
	{
		return UnitOfWork.ProductionOrderImportTool.GetEditTableEnableColumns();
	}

	[HttpPost]
	public async Task<HttpResult> SaveFG([FromBody] DataTable dt)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "Invalid session");
			}
			return await BrokerService.GetUnitOfWork(SessionId).ProductionOrderImportTool.SaveFG(dt, SessionUserInfo.SessionDiServer);
		}
		return new HttpResult(Enums.MessageCode.Error, "[Azon API] This function not support.");
	}

	[HttpPost]
	public async Task<string> SaveFGSerial(DataSet ds)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return await BrokerService.GetUnitOfWork(SessionId).ProductionOrderImportTool.SaveFGSerial(ds, SessionUserInfo.SessionDiServer);
		}
		return "[Azon API] This function not support.";
	}

	[HttpPost]
	public string SaveTransfer([FromBody] DocumentsImpl model)
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
				return unitOfWork.StockTransfer.Save(model, SessionUserInfo.SessionDiServer, isDraft: false).Message;
			}
			return "INVALIDSESSION";
		}
		return "INVALIDSESSION";
	}

	[HttpPost]
	public async Task<string> SaveComp([FromBody] DataTable dt)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return await BrokerService.GetUnitOfWork(SessionId).ProductionOrderImportTool.SaveComp(dt, SessionUserInfo.SessionDiServer);
		}
		return "[Azon API] This function not support.";
	}

	[HttpPost]
	public string SaveInteruptCodeRC([FromBody] string intrCode)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).ProductionOrderImportTool.SaveInteruptCodeRC(intrCode, SessionUserInfo.SessionDiServer);
		}
		return "[Azon API] This function not support.";
	}

	[HttpPost]
	public string SaveRC([FromBody] DataTable dt)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).ProductionOrderImportTool.SaveRC(dt, SessionUserInfo.SessionDiServer);
		}
		return "[Azon API] This function not support.";
	}

	[HttpPost]
	public string ReportCompletion([FromBody] DataTable dt)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).ProductionOrderImportTool.ReportCompletion(dt, SessionUserInfo.SessionDiServer).Result;
		}
		return "[Azon API] This function not support.";
	}

	[HttpPost]
	public string SaveRCFromPC([FromBody] DataTable dt)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).ProductionOrderImportTool.SaveRCFromPC(dt, SessionUserInfo.SessionDiServer);
		}
		return "[Azon API] This function not support.";
	}

	[HttpPost]
	public string SaveClose([FromBody] DataTable dt)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).ProductionOrderImportTool.SaveClose(dt, SessionUserInfo.SessionDiServer);
		}
		return "[Azon API] This function not support.";
	}

	[HttpPost]
	public HttpResult SaveTempFG([FromBody] DataTable dt)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "Invalid session");
			}
			return BrokerService.GetUnitOfWork(SessionId).ProductionOrderImportTool.SaveTempFG(dt, SessionUserInfo.SessionDiServer, SessionUserInfo.UserName);
		}
		return new HttpResult(Enums.MessageCode.Error, "[Azon API] This function not support.");
	}

	[HttpGet]
	public string UpdateDataForBarcodeApp(string weekNo)
	{
		return UnitOfWork.ProductionOrderImportTool.UpdateDataForBarcodeApp(weekNo);
	}

	[HttpPost]
	public string WithdrawOfGooods([FromBody] DataTable dt)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).ProductionOrderImportTool.WithdrawOfGooods(dt, SessionUserInfo.SessionDiServer);
		}
		return "[API] This function not support.";
	}

	[HttpGet]
	public DataSet IssNdRecQtySearch(string date)
	{
		return UnitOfWork.ProductionOrderImportTool.IssNdRecQtySearch(date);
	}
}
