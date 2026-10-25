using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
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
using Apzon.Entities.Models.Production;

namespace Apzon.Api.Controllers.Production;

[ValidateLoginSapExceptionHandling]
public class ProductionOderController : BaseApiController
{
	[HttpPost]
	public DataTable Search([FromBody] DataSet source)
	{
		return UnitOfWork.ProductionOrder.Search(source.Tables[0], source.Tables[1]);
	}

	[HttpGet]
	public DataTable GetADoc(int docEntry)
	{
		return UnitOfWork.ProductionOrder.GetADoc(docEntry);
	}

	[HttpPost]
	public DataSet SearchLine([FromBody] ProductionOder_LineImpl_WOR1 line)
	{
		return UnitOfWork.ProductionOrder.SearchLine(line);
	}

	[HttpGet]
	public DataTable GetIsIssued(int docEntry)
	{
		return UnitOfWork.ProductionOrder.GetIsIssued(docEntry);
	}

	[HttpGet]
	public DataTable GetOITWQuantity(string code, string whsCode)
	{
		return UnitOfWork.ProductionOrder.GetOITWQuantity(code, whsCode);
	}

	[HttpGet]
	public DataTable GetItemNameUoM(string itemCode)
	{
		return UnitOfWork.ProductionOrder.GetItemNameUoM(itemCode);
	}

	[HttpGet]
	public DataTable GetMaxDocEntry()
	{
		return UnitOfWork.ProductionOrder.GetMaxDocEntry();
	}

	[HttpGet]
	public DataTable Check_FirstRecord()
	{
		return UnitOfWork.ProductionOrder.Check_FirstRecord();
	}

	[HttpGet]
	public DataTable Check_LastRecord()
	{
		return UnitOfWork.ProductionOrder.Check_LastRecord();
	}

	[HttpPost]
	public DataTable GetItem_ITT1([FromBody] BillOfMasterials_LineImpl_ITT1 line)
	{
		return UnitOfWork.ProductionOrder.GetItem_ITT1(line);
	}

	[HttpGet]
	public DataTable GetList_SalesOder()
	{
		return UnitOfWork.ProductionOrder.GetList_SalesOder();
	}

	[HttpGet]
	public DataTable GetDataItemProduction(string _TranType)
	{
		return UnitOfWork.ProductionOrder.GetDataItemProduction(_TranType);
	}

	[HttpGet]
	public DataTable GetDataItemManualProduction(string strSearch)
	{
		return UnitOfWork.ProductionOrder.GetDataItemManualProduction(strSearch);
	}

	[HttpGet]
	public DataTable GetListStandardOrders()
	{
		return UnitOfWork.ProductionOrder.GetListStandardOrders();
	}

	[HttpGet]
	public DataTable GetTypeByDocEntry(string docEntry)
	{
		return UnitOfWork.ProductionOrder.GetTypeByDocEntryDataTable(docEntry);
	}

	[HttpPost]
	public async Task<HttpResult> ReceiptForBatch([FromBody] Tuple<int, DocumentsImpl, DocumentsImpl, string, string, DataTable> model)
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
				}
				IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
				if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
				{
					return await unitOfWork.ProductionOrders.ReceiptForBatch(model, SessionUserInfo.SessionDiServer);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult(Enums.MessageCode.Error, Resources.Msg_UnknownError);
	}

	[HttpPost]
	public async Task<HttpResult> PackingBatch([FromBody] Tuple<DocumentsImpl, DocumentsImpl, ProductionOder_Impl_OWOR> model)
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
				}
				IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
				if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
				{
					return await unitOfWork.ProductionOrders.PackingBatch(model, SessionUserInfo.SessionDiServer);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult(Enums.MessageCode.Error, Resources.Msg_UnknownError);
	}

	[HttpPost]
	public async Task<HttpResult> ConfirmBatch([FromBody] Tuple<DocumentsImpl, DocumentsImpl, ProductionOder_Impl_OWOR> model)
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
				}
				IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
				if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
				{
					return await unitOfWork.ProductionOrders.ConfirmBatch(model, SessionUserInfo.SessionDiServer);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult(Enums.MessageCode.Error, Resources.Msg_UnknownError);
	}

	[HttpPost]
	public HttpResult SaveProductionPlan([FromBody] Tuple<List<ProductionOder_Impl_OWOR>, DocumentsImpl, DataTable> model)
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
				}
				IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
				if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
				{
					return unitOfWork.ProductionOrders.SaveProductionPlan(model, SessionUserInfo.SessionDiServer);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult();
	}

	[HttpGet]
	public DataTable GetItemDetails(string type, string code)
	{
		return UnitOfWork.ProductionOrder.GetItemDetails(type, code);
	}

	[HttpGet]
	public DataTable GetActiveDimensionCount()
	{
		return UnitOfWork.BillOfMasterials.GetActiveDimensionCount();
	}

	[HttpGet]
	public DataTable ShowUsers()
	{
		return UnitOfWork.ProductionOrder.ShowUsers();
	}

	[HttpPost]
	public DataTable ValidateDocData([FromBody] DataTable dt)
	{
		return UnitOfWork.ProductionOrder.ValidateDocData(dt);
	}

	[HttpGet]
	public DataTable GetSalesOrderEntry(int docNum)
	{
		return UnitOfWork.ProductionOrder.GetSalesOrderEntry(docNum);
	}

	[HttpGet]
	public int GetDocNum(int series)
	{
		try
		{
			return UnitOfWork.ProductionOrder.GetDocNum(series);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return 0;
		}
	}

	[HttpGet]
	public DataSet GetIssueComponents(int docEntry)
	{
		return UnitOfWork.ProductionOrder.GetIssueComponents(docEntry);
	}

	[HttpGet]
	public DataSet GetTransferComponents(int docEntry, double parentQty)
	{
		return UnitOfWork.ProductionOrder.GetTransferComponents(docEntry, parentQty);
	}

	[HttpPost]
	public HttpResult SaveDoc([FromBody] ProductionOder_Impl_OWOR doc)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			return BrokerService.GetUnitOfWork(SessionId).ProductionOrders.Save(doc, SessionUserInfo.SessionDiServer);
		}
		return new HttpResult(Enums.MessageCode.ThisFunctionIsNotSupported);
	}

	[HttpPost]
	public HttpResult SaveDataSet([FromBody] DataSet ds)
	{
		try
		{
			return Global.LoginSap ? new HttpResult(Enums.MessageCode.Error, "This function is not supported") : UnitOfWork.ProductionOrder.SaveDataSet(ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	[HttpPost]
	public async Task<HttpResult> SaveList([FromBody] List<ProductionOder_Impl_OWOR> oDocs)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
			{
				return await unitOfWork.ProductionOrders.SaveList(oDocs, SessionUserInfo.SessionDiServer);
			}
		}
		return new HttpResult(Enums.MessageCode.Success);
	}

	[HttpPost]
	public HttpResult UpdateStatus([FromBody] List<ProductionOder_Impl_OWOR> oDocs)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
			{
				return unitOfWork.ProductionOrders.UpdateStatus(oDocs, SessionUserInfo.SessionDiServer);
			}
		}
		return new HttpResult(Enums.MessageCode.Error, Resources.Msg_UnknownError);
	}

	[HttpPost]
	public HttpResult ProPlanning(List<ProductionOder_Impl_OWOR> lst)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			return BrokerService.GetUnitOfWork(SessionId).ProductionOrders.SaveProPlanning(lst, SessionUserInfo.SessionDiServer);
		}
		return new HttpResult(Enums.MessageCode.ThisFunctionIsNotSupported);
	}

	[HttpPost]
	public HttpResult CancelDoc([FromBody] int docEntry)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
			{
				return unitOfWork.ProductionOrders.CancelDoc(docEntry, SessionUserInfo.SessionDiServer);
			}
		}
		return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
	}

	[HttpGet]
	public DataTable GetRecord(string position, string docEntry)
	{
		return UnitOfWork.ProductionOrder.GetRecord(position, docEntry);
	}

	[HttpGet]
	public DataTable GetFirstRecord()
	{
		return UnitOfWork.ProductionOrder.GetFirstRecord();
	}

	[HttpGet]
	public DataTable GetLastRecord()
	{
		return UnitOfWork.ProductionOrder.GetLastRecord();
	}

	[HttpGet]
	public DataTable GetPreviousRecord(string docEntry)
	{
		return UnitOfWork.ProductionOrder.GetPreviousRecord(docEntry);
	}

	[HttpGet]
	public DataTable GetNextRecord(string docEntry)
	{
		return UnitOfWork.ProductionOrder.GetNextRecord(docEntry);
	}

	[HttpGet]
	public DataTable FindDataNextLast(int id, int type)
	{
		return UnitOfWork.ProductionOrder.FindDataNextLast(id, type);
	}

	[HttpGet]
	public DataTable GetByBaseEntry(int baseEntry, ApzObjectTypes objectType)
	{
		try
		{
			return UnitOfWork.ProductionOrder.GetByBaseEntry(baseEntry, objectType);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	[HttpGet]
	public HttpResult CheckBaseProduction(int entry, ApzObjectTypes objtype)
	{
		try
		{
			return UnitOfWork.ProductionOrder.CheckBaseProduction(entry, objtype);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	[HttpGet]
	public string UpdateStatusPO(int docEntry, string status)
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
				return unitOfWork.ProductionOrders.UpdateStatusByDI(docEntry, status, SessionUserInfo.SessionDiServer);
			}
			return "INVALIDSESSION";
		}
		return "INVALIDSESSION";
	}

	[HttpGet]
	public string UpdateStatusByDI(int docEntry, string status)
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
				return unitOfWork.ProductionOrders.UpdateStatusByDI(docEntry, status, SessionUserInfo.SessionDiServer);
			}
		}
		return "This function is not supported";
	}

	[HttpPost]
	public async Task<HttpResult> ConfirmProduction([FromBody] Tuple<int, DocumentsImpl, DocumentsImpl, string, string, DataTable, string> model)
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
				}
				IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
				if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
				{
					return await unitOfWork.ProductionOrders.ConfirmProduction(model, SessionUserInfo.SessionDiServer);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult(Enums.MessageCode.Error, Resources.Msg_UnknownError);
	}

	[HttpGet]
	public DataSet GetProductionByBarcode(string barcode)
	{
		return UnitOfWork.ProductionOrder.GetProductionByBarcode(barcode);
	}

	[HttpGet]
	public DataSetResult GetDataByBarcode(string barcode, string userName, int branch)
	{
		try
		{
			return UnitOfWork.ProductionOrder.GetDataByBarcode(barcode, userName, branch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSetResult();
	}

	[HttpGet]
	public DataTable CheckByDocNum(string type, int docNum)
	{
		return UnitOfWork.ProductionOrder.CheckByDocNum(type, docNum);
	}

	public DataTable GetReferencedDocument(string FromDocEntry)
	{
		return UnitOfWork.ProductionOrder.GetReferencedDocument(FromDocEntry);
	}
}
