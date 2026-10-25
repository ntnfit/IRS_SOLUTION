using System;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.MobileApp;

namespace Apzon.Api.Controllers.DMS;

[ValidateLoginSapExceptionHandling]
public class DMSDeliveryController : BaseApiController
{
	[HttpGet]
	public ResponseResult GetListDeliveryHistory(string userCode, string lstCardCode, string lstConfirmStatus, string fromDate, string toDate, int pageIndex)
	{
		try
		{
			DataTable listDeliveryHistory = UnitOfWork.DMSDelivery.GetListDeliveryHistory(userCode, lstCardCode, lstConfirmStatus, fromDate, toDate, pageIndex);
			if (listDeliveryHistory.IsNotNull())
			{
				listDeliveryHistory.TableName = "Data";
			}
			return new ResponseResult
			{
				Message = "",
				Content = listDeliveryHistory,
				MessageCode = "200"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = "400"
			};
		}
	}

	[HttpGet]
	public ResponseResult GetListBusinessPartner()
	{
		try
		{
			DataSet dataSet = UnitOfWork.CFL.LoadBusinessPartner("C", SessionUserInfo.UserName, "");
			DataTable content = new DataTable();
			if (dataSet.IsNotNull() && dataSet.Tables.Count == 2)
			{
				content = dataSet.Tables[1].Copy();
			}
			return new ResponseResult
			{
				Message = "",
				Content = content,
				MessageCode = "200"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = "400"
			};
		}
	}

	[HttpPost]
	public DataSet SearchDeliveryOrder([FromBody] DataTable dt)
	{
		try
		{
			return UnitOfWork.DMSDelivery.SearchDeliveryOrder(dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public DataSet GetLines(int docentry)
	{
		try
		{
			return UnitOfWork.DMSDelivery.GetLines(docentry);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpPost]
	public HttpResult Add([FromBody] DataSet ds)
	{
		return UnitOfWork.DMSDelivery.Save(ds, "A");
	}

	[HttpPost]
	public HttpResult Update([FromBody] DataSet ds)
	{
		return UnitOfWork.DMSDelivery.Save(ds, "U");
	}

	[HttpGet]
	public ResponseResult GetListDeliveryOrder(string userCode, DateTime date, string status)
	{
		try
		{
			DataTable listDeliveryOrder = UnitOfWork.DMSDelivery.GetListDeliveryOrder(userCode, date, status);
			if (listDeliveryOrder.IsNotNull())
			{
				listDeliveryOrder.TableName = "Data";
			}
			return new ResponseResult
			{
				Message = "",
				Content = listDeliveryOrder,
				MessageCode = "200"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = "400"
			};
		}
	}

	[HttpGet]
	public ResponseResult GetDeliveryOrderDetails(string docentry, string cardcode)
	{
		try
		{
			DataSet deliveryOrderDetails = UnitOfWork.DMSDelivery.GetDeliveryOrderDetails(docentry, cardcode);
			if (deliveryOrderDetails.Tables.Count == 2)
			{
				deliveryOrderDetails.Tables[0].TableName = "Header";
				deliveryOrderDetails.Tables[1].TableName = "Detail";
			}
			return new ResponseResult
			{
				Message = "",
				Content = deliveryOrderDetails,
				MessageCode = "200"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = "400"
			};
		}
	}

	[HttpGet]
	public ResponseResult GetLocationNote(string cardcode, string shipCode)
	{
		try
		{
			DataTable locationNote = UnitOfWork.DMSDelivery.GetLocationNote(cardcode, shipCode);
			if (locationNote.IsNotNull())
			{
				locationNote.TableName = "Data";
			}
			return new ResponseResult
			{
				Message = "",
				Content = locationNote,
				MessageCode = "200"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = "400"
			};
		}
	}

	[HttpGet]
	public ResponseResult GetBPNote(string cardCode)
	{
		try
		{
			DataTable bPNote = UnitOfWork.DMSDelivery.GetBPNote(cardCode);
			if (bPNote.IsNotNull())
			{
				bPNote.TableName = "Data";
			}
			return new ResponseResult
			{
				Message = "",
				Content = bPNote,
				MessageCode = "200"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = "400"
			};
		}
	}

	[HttpPost]
	public ResponseResult SaveLocationNote([FromBody] DataTable dt)
	{
		try
		{
			if (!dt.IsNotNull())
			{
				return new ResponseResult
				{
					Message = "Data is null",
					MessageCode = "401"
				};
			}
			return UnitOfWork.DMSDelivery.SaveLocationNote(dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = "107"
			};
		}
	}

	[HttpPost]
	public ResponseResult DeleteLocationNote([FromBody] DataTable dt)
	{
		try
		{
			if (!dt.IsNotNull())
			{
				return new ResponseResult
				{
					Message = "Data is null",
					MessageCode = "401"
				};
			}
			return UnitOfWork.DMSDelivery.SaveLocationNote(dt, Enums.TransactionType.L);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = "107"
			};
		}
	}

	[HttpPost]
	public ResponseResult DeleteBPNote([FromBody] DataTable dt)
	{
		try
		{
			if (!dt.IsNotNull())
			{
				return new ResponseResult
				{
					Message = "Data is null",
					MessageCode = "401"
				};
			}
			return UnitOfWork.DMSDelivery.SaveBPNote(dt, Enums.TransactionType.L);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = "107"
			};
		}
	}

	[HttpPost]
	public ResponseResult SaveBPNote([FromBody] DataTable dt)
	{
		try
		{
			if (!dt.IsNotNull())
			{
				return new ResponseResult
				{
					Message = "Data is null",
					MessageCode = "401"
				};
			}
			return UnitOfWork.DMSDelivery.SaveBPNote(dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = "107"
			};
		}
	}

	[HttpGet]
	public ResponseResult ConfirmLateTime(string docentry, string cardcode, DateTime newTime, string reason)
	{
		try
		{
			return UnitOfWork.DMSDelivery.ConfirmLateTime(docentry, cardcode, newTime, reason);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = "400"
			};
		}
	}

	[HttpPost]
	public ResponseResult ConfirmDelivery([FromBody] DataTable dt)
	{
		try
		{
			if (!dt.IsNotNull())
			{
				return new ResponseResult
				{
					Message = "Data is null",
					MessageCode = "401"
				};
			}
			return UnitOfWork.DMSDelivery.ConfirmDelivery(dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = "107"
			};
		}
	}
}
