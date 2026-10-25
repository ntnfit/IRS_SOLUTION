using System;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;

namespace Apzon.Api.Controllers.Inventory.ItemManagement;

public class SerialNumberDetailsController : BaseApiController
{
	[HttpGet]
	public DataTable Search(string itemCode, string itemName, string mnfSerial, string serialNum, string lotNum)
	{
		return UnitOfWork.SerialNumberDetails.Search(itemCode, itemName, mnfSerial, serialNum, lotNum);
	}

	[HttpGet]
	public DataTable LoadAnEntry(int absEntry)
	{
		return UnitOfWork.SerialNumberDetails.LoadAnEntry(absEntry);
	}

	[HttpGet]
	public string GetUniqueSerialNo()
	{
		return UnitOfWork.SerialNumberDetails.GetUniqueSerialNo();
	}

	[HttpGet]
	public DataTable GetSearialNumbers(string itemCode)
	{
		return UnitOfWork.SerialNumberDetails.GetSearialNumbers(itemCode);
	}

	[HttpGet]
	public DataTable GetQuantity(int absEntry)
	{
		return UnitOfWork.SerialNumberDetails.GetQuantity(absEntry);
	}

	[HttpPost]
	public string SaveSerialNumberDetail(DataTable dtSave)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).SerialNumberDetails.Save(dtSave, SessionUserInfo.SessionDiServer);
		}
		return UnitOfWork.SerialNumberDetails.SaveSerialNumberDetail(dtSave);
	}

	[HttpGet]
	public DataTable GetByAbsEntry(int absEntry)
	{
		return UnitOfWork.SerialNumberDetails.GetByAbsEntry(absEntry);
	}

	[HttpGet]
	public int GetPage(int currentId, int type, string username, int branch)
	{
		try
		{
			return UnitOfWork.SerialNumberDetails.GetPage(currentId, type, username, branch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return 0;
	}

	[HttpGet]
	public DataSet GetSerialDetail(int absEntry, string itemCode, string status, string distNumber, string mnfSerial, string lotNumber)
	{
		try
		{
			return UnitOfWork.SerialNumberDetails.GetSerialDetail(absEntry, itemCode, status, distNumber, mnfSerial, lotNumber);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}
}
