using System;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.DataServices;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;

namespace Apzon.Api.Controllers.Inventory.InventoryTransaction;

[ValidateLoginSapExceptionHandling]
public class InventoryCountingBinController : BaseApiController
{
	[HttpGet]
	public DataTable SearchDataByBarCode(string barcode, string whs, string bin)
	{
		return UnitOfWork.InventoryCoutingBin.SearchDataByBarCode(barcode, whs, bin);
	}

	[HttpGet]
	public string GetRecord(int docEntry, int type)
	{
		return UnitOfWork.InventoryCoutingBin.GetRecord(docEntry, type);
	}

	[HttpGet]
	public DataSet TransferGet(string batchNo, string bin)
	{
		return UnitOfWork.InventoryCoutingBin.TransferGet(batchNo, bin);
	}

	[HttpGet]
	public DataSet GetBinInstock(string bin, string period)
	{
		try
		{
			return UnitOfWork.InventoryCoutingBin.GetBinInstock(bin, period);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public DataTable LoadAllBin(string itemCode, string whsCode, int batchSerialAbs, int tranType, string binCode)
	{
		return UnitOfWork.InventoryCoutingBin.LoadAllBin(itemCode, whsCode, batchSerialAbs, tranType, binCode);
	}

	[HttpGet]
	public DataTable GetPeriods()
	{
		return UnitOfWork.InventoryCoutingBin.GetPeriods();
	}

	[HttpGet]
	public DataTable CheckBatch(string batch, string bin, string period)
	{
		return UnitOfWork.InventoryCoutingBin.CheckBatch(batch, bin, period);
	}

	[HttpGet]
	public int GetPage(int current, int type)
	{
		try
		{
			return UnitOfWork.InventoryCoutingBin.GetPage(current, type);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return 0;
	}

	[HttpGet]
	public DataSet GetById(int id)
	{
		try
		{
			return UnitOfWork.InventoryCoutingBin.GetById(id);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public DataSet GetCountingReport(string period, string bin)
	{
		try
		{
			return UnitOfWork.InventoryCoutingBin.GetCountingReport(period, bin);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpPost]
	[QueueModifiedAction]
	public HttpResult Posting([FromBody] Tuple<DocumentsImpl, DocumentsImpl, DocumentsImpl> model)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "Invalid session");
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
			{
				return unitOfWork.StockTransfer.Posting(model, SessionUserInfo.SessionDiServer);
			}
		}
		return new HttpResult(Enums.MessageCode.Error, Resources.Msg_FunctionNotSupported);
	}

	[HttpGet]
	public DataTableResult GetWarehouseRequest(string docEntry)
	{
		try
		{
			return UnitOfWork.InventoryCoutingBin.GetWarehouseRequest(docEntry);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTableResult();
	}

	[HttpGet]
	public DataSetResult GetDataByBarcode(string barcode, string whs, string fromWhs, int requestId = 0)
	{
		try
		{
			return UnitOfWork.InventoryCoutingBin.GetDataByBarcode(barcode, whs, fromWhs, requestId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSetResult();
	}

	[HttpGet]
	public DataSetResult GetDataSummaryByBarcode(string barcode, string whs, string fromWhs)
	{
		try
		{
			return UnitOfWork.InventoryCoutingBin.GetDataSummaryByBarcode(barcode, whs, fromWhs);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSetResult();
	}
}
