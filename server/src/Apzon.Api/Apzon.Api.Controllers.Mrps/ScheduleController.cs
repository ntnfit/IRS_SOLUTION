using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Production;

namespace Apzon.Api.Controllers.Mrps;

public class ScheduleController : BaseApiController
{
	[HttpGet]
	public DataSet Run(string mrpCode, string scheduleType, string fdate, string tdate, string isDetailSchedule)
	{
		try
		{
			return UnitOfWork.Schedule.Run(mrpCode, scheduleType, fdate, tdate, isDetailSchedule);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	[DeflateCompression]
	public DataTable GetRunSchedule(string mrpNo)
	{
		try
		{
			return UnitOfWork.Schedule.SP_APZ_RunSchedule(mrpNo);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	[HttpGet]
	[DeflateCompression]
	public DataTable GetSchedule(string scheduleNo)
	{
		try
		{
			return UnitOfWork.Schedule.SP_APZ_GetScheduling(scheduleNo);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	[HttpGet]
	[DeflateCompression]
	public DataTable GetScheduleGenPO(string scheduleNo)
	{
		try
		{
			return UnitOfWork.Schedule.GetScheduleGenPO(scheduleNo);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	[HttpGet]
	[DeflateCompression]
	public DataTable GetHoliday(DateTime fDate, DateTime tDate)
	{
		try
		{
			return UnitOfWork.Schedule.SP_APZ_GetHoliDay(fDate, tDate);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	[HttpGet]
	[DeflateCompression]
	public DataTable CheckPOInShift(string itemCode, DateTime date, string shift, string psCode)
	{
		try
		{
			return UnitOfWork.Schedule.CheckPOInShift(itemCode, date, shift, psCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return null;
	}

	[HttpGet]
	[DeflateCompression]
	public DataTable GetSearch(string scheduleNo, DateTime fDate, DateTime tDate, string status)
	{
		try
		{
			return UnitOfWork.Schedule.SP_APZ_Search(scheduleNo, fDate, tDate, status);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	[HttpGet]
	public int GetMaxDocEntry()
	{
		try
		{
			return UnitOfWork.Schedule.GetMaxDocEntry("apz_OPRS");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return 1;
	}

	[HttpPost]
	public HttpResult Remove([FromBody] int docEntry)
	{
		return UnitOfWork.Schedule.Save(null, Enums.TransactionType.D, Function.ToString(docEntry));
	}

	[HttpPost]
	public HttpResult Save([FromBody] DataSet model)
	{
		return UnitOfWork.Schedule.Save(model, Enums.TransactionType.A, "");
	}

	[HttpPost]
	public HttpResult Update([FromBody] DataSet model)
	{
		return UnitOfWork.Schedule.Save(model, Enums.TransactionType.U, "");
	}

	[HttpGet]
	public DataSet GetDataLine(int id)
	{
		return UnitOfWork.Schedule.GetDataLine(id);
	}

	[HttpGet]
	public DataTable GetDataProductionOrder(string itemCode, string version, string sourceType, int sourceId, int sourceLine)
	{
		return UnitOfWork.Schedule.GetDataProductionOrder(itemCode, version, sourceType, sourceId, sourceLine);
	}

	[HttpGet]
	public DataTable GetDataMrp(int mrpCode)
	{
		return UnitOfWork.Schedule.GetDataMrp(mrpCode);
	}

	[HttpGet]
	public DataTable GetWhs(string inputType, int inputId)
	{
		return UnitOfWork.Schedule.GetWhs(inputType, inputId);
	}

	[HttpGet]
	public DataSet GetDataStructureGenProduction()
	{
		return UnitOfWork.Schedule.GetDataStructureGenProduction();
	}

	[HttpGet]
	public DataTable GetDataStructureAutoreport()
	{
		return UnitOfWork.Schedule.GetDataStructureAutoreport();
	}

	[HttpGet]
	public DataTable GetStructureGenPurchase()
	{
		return UnitOfWork.Schedule.GetStructureGenPurchase();
	}

	[HttpGet]
	public DataTable GetStructureGenTransfer()
	{
		return UnitOfWork.Schedule.GetStructureGenTransfer();
	}

	[HttpGet]
	[DeflateCompression]
	public DataTable GetCheckBarcode(string barcode)
	{
		try
		{
			return UnitOfWork.Schedule.SP_APZ_ProductionGetCheck(barcode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	[HttpGet]
	[DeflateCompression]
	public DataTable GetBarcodeManual(string barcode)
	{
		try
		{
			return UnitOfWork.Schedule.GetBarCodeManual(barcode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	[HttpGet]
	[DeflateCompression]
	public DataTable GetDataGenProduction(string id, string type, string inputType)
	{
		try
		{
			return UnitOfWork.Schedule.GetDataGenProduction(id, type, inputType);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	[HttpPost]
	public string UpdateStatus([FromBody] ProductionOder_Impl_OWOR obj)
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return "INVALIDSESSION";
				}
				IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
				List<ProductionOder_Impl_OWOR> oDocs = new List<ProductionOder_Impl_OWOR> { obj };
				return unitOfWork.ProductionOrders.UpdateStatus(oDocs, SessionUserInfo.SessionDiServer).Message;
			}
			return UnitOfWork.ProductionOrder.UpdateStatus(obj);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	[HttpGet]
	public DataTable GetDataDoc(int id, Enums.NavigateType type)
	{
		return UnitOfWork.Schedule.GetDataDoc(id, type);
	}

	[HttpGet]
	public DataSet GetTablePO()
	{
		return UnitOfWork.Schedule.GetTablePO();
	}

	[HttpGet]
	public DataTable GetDataGenPurchase(string input, int code, string user)
	{
		return UnitOfWork.Schedule.GetDataGenPurchase(input, code, user);
	}

	[HttpGet]
	public DataTable GetDataInventoryTransferFormSchedule(string input, int code, string user)
	{
		return UnitOfWork.Schedule.GetDataInventoryTransferFormSchedule(input, code, user);
	}

	[HttpGet]
	public DataTable GetSeriesOrderAndRequest(string type)
	{
		return UnitOfWork.Schedule.GetSeriesOrderAndRequest(type);
	}

	[HttpGet]
	public HttpResult GetNewIdSchedule()
	{
		return UnitOfWork.Schedule.GetNewIdSchedule();
	}

	[HttpGet]
	public DataTable GetdataBpItem(string bpCode, string itemCode, string shipDate)
	{
		return UnitOfWork.Schedule.GetdataBpItem(bpCode, itemCode, shipDate);
	}

	[HttpGet]
	public DataTable GetDocStructure()
	{
		return UnitOfWork.Schedule.GetDocStructure();
	}

	[HttpGet]
	public string CheckInputData(string type, int code, string genType)
	{
		return UnitOfWork.Schedule.CheckInputData(type, code, genType);
	}
}
