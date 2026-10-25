using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Inventory.PickAndPack;

public class PickingController : BaseApiController
{
	[HttpPost]
	public DataSet LoadSalesOrder([FromBody] DataTable dtSearch)
	{
		try
		{
			return UnitOfWork.Picking.LoadSalesOrder(dtSearch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public DataSet LoadBatchBarcode(string barcode, string itemcode, string whs, int docNo, int docRow, int line)
	{
		return UnitOfWork.Picking.LoadBatchBarcode(barcode, itemcode, whs, docNo, docRow, line);
	}

	[HttpPost]
	public DataSet LoadPickingList([FromBody] DataTable dtSearch)
	{
		try
		{
			return UnitOfWork.Picking.LoadPickingList(dtSearch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpPost]
	public DataSet LoadPicked([FromBody] DataTable dtSearch)
	{
		try
		{
			return UnitOfWork.Picking.LoadPicked(dtSearch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public DataSet LoadPickedLine(int docentry)
	{
		try
		{
			return UnitOfWork.Picking.LoadPickedLine(docentry);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpPost]
	public DataTable GetDoc([FromBody] DataTable pr)
	{
		return UnitOfWork.Picking.GetDoc(pr);
	}

	[HttpGet]
	public DataTable GetLines(int id)
	{
		return UnitOfWork.Picking.GetLines(id);
	}

	[HttpGet]
	public int GetNextId()
	{
		return UnitOfWork.Picking.GetNextId();
	}

	[HttpGet]
	public DataTable GetLineByPickNo(int id)
	{
		return UnitOfWork.Picking.GetLineByPickNo(id);
	}

	[HttpGet]
	public DataTable GetLineByPackNo(int id)
	{
		return UnitOfWork.Picking.GetLineByPackNo(id);
	}

	[HttpGet]
	public DataSet ChoosePickList()
	{
		return UnitOfWork.Picking.ChoosePickList();
	}

	[HttpGet]
	public DataSet ChoosePacking()
	{
		return UnitOfWork.Picking.ChoosePacking();
	}

	[HttpGet]
	public DataSet ChooseBatch()
	{
		return UnitOfWork.Picking.ChooseBatch();
	}

	[HttpPost]
	public HttpResult Save([FromBody] DataSet ds)
	{
		try
		{
			return UnitOfWork.Picking.Save(ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	[HttpPost]
	public HttpResult SaveList([FromBody] List<DataSet> model)
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return new HttpResult(Enums.MessageCode.Error, "INVALID SESSION ID");
				}
				return BrokerService.GetUnitOfWork(SessionId).PickingDocument.SaveList(model, SessionUserInfo.SessionDiServer);
			}
			return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	[HttpPost]
	public HttpResult Close([FromBody] int docEntry)
	{
		return UnitOfWork.Picking.Close(docEntry);
	}

	[HttpPost]
	public HttpResult Cancel([FromBody] int docEntry)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "INVALID SESSION ID");
			}
			return BrokerService.GetUnitOfWork(SessionId).PickingDocument.Cancel(docEntry, SessionUserInfo.SessionDiServer);
		}
		return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
	}

	[HttpGet]
	public DataSet ChooseList(ApzObjectTypes type)
	{
		return UnitOfWork.Picking.ChooseList(type);
	}

	[HttpPost]
	public DataTable Copy([FromBody] Tuple<ApzObjectTypes, ApzObjectTypes, List<int>> model)
	{
		return UnitOfWork.Picking.Copy(model.Item1, model.Item2, model.Item3);
	}

	[HttpGet]
	public DataTable GetBatch(int absEntry, int pickentry)
	{
		return UnitOfWork.Picking.GetBatch(absEntry, pickentry);
	}

	[HttpGet]
	public DataTable GetConfirmed(int absEntry, int pickentry)
	{
		return UnitOfWork.Picking.GetConfirmed(absEntry, pickentry);
	}

	[HttpGet]
	public DataTable AutoSelectBatch(string itemcode, string whsCode, double quantity, int linenum, int docentry, int orderNum, int orderLine)
	{
		return UnitOfWork.Picking.AutoSelectBatch(itemcode, whsCode, quantity, linenum, docentry, orderNum, orderLine);
	}

	[HttpGet]
	public DataSet CreateNewInstance()
	{
		return UnitOfWork.Picking.CreateNewInstance();
	}

	[HttpGet]
	public DataSet GetListBatchSerial(string itemCode, string whsCode, int pickentry, int docEntry, int orderNum, int orderLine)
	{
		return UnitOfWork.Picking.GetListBatchSerial(itemCode, whsCode, pickentry, docEntry, orderNum, orderLine);
	}

	[HttpGet]
	public DataSet LoadBatchLine(int docEntry, string barcode, string type = "C")
	{
		return UnitOfWork.Picking.LoadBatchLine(docEntry, barcode, type);
	}

	[HttpPost]
	public HttpResult PickedConfirm([FromBody] DataTable dt)
	{
		return UnitOfWork.Picking.PickedConfirm(dt);
	}

	[HttpPost]
	public HttpResult UnpickedConfirm([FromBody] DataTable dt)
	{
		return UnitOfWork.Picking.UnpickedConfirm(dt);
	}
}
