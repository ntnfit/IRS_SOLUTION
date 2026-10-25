using System;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.DataServices;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Mrps;

public class JetstarPreOrderController : BaseApiController
{
	[HttpGet]
	public DataTable Search(string type, string searchDate)
	{
		return UnitOfWork.JetstarPreOrder.Search(type, searchDate);
	}

	[HttpGet]
	public DataTable Run(string type)
	{
		return UnitOfWork.JetstarPreOrder.Run(type);
	}

	[HttpPost]
	public HttpResult SaveData([FromBody] DataTable dt)
	{
		return UnitOfWork.JetstarPreOrder.SaveData(dt, Enums.TransactionType.A);
	}

	[HttpPost]
	public HttpResult UpdateData([FromBody] DataTable dt)
	{
		return UnitOfWork.JetstarPreOrder.SaveData(dt, Enums.TransactionType.U);
	}

	[HttpPost]
	public HttpResult CancelOldPurchaseOrder([FromBody] int mrpCode)
	{
		try
		{
			int num = 0;
			DataSet openPoByMrpCode = UnitOfWork.JetstarPreOrder.GetOpenPoByMrpCode(mrpCode);
			if (openPoByMrpCode == null || openPoByMrpCode.Tables.Count != 2)
			{
				return new HttpResult(Enums.MessageCode.Error, "Can not get data cancel Purchase order");
			}
			if (openPoByMrpCode.Tables[0].IsNotNull() && Function.ParseInt(openPoByMrpCode.Tables[0].Rows[0][0]) > 0)
			{
				num = Function.ParseInt(openPoByMrpCode.Tables[0].Rows[0][0]);
			}
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			for (int i = 0; i < openPoByMrpCode.Tables[1].Rows.Count; i++)
			{
				string message = unitOfWork.Documents.CancelDoc(Function.ParseInt(openPoByMrpCode.Tables[1].Rows[i][0]), SessionUserInfo.SessionDiServer, ApzObjectTypes.oPurchaseOrders).Message;
				if (!string.IsNullOrEmpty(message))
				{
					Logging.Write("ERROR", message);
					return new HttpResult(Enums.MessageCode.Error, message);
				}
			}
			return new HttpResult(Enums.MessageCode.Success, "", num + 1);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	[HttpGet]
	public DataTable GetBp()
	{
		return UnitOfWork.JetstarPreOrder.GetBp();
	}
}
