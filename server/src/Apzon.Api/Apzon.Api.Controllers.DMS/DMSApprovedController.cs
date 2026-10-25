using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.DataServices;
using Apzon.Entities.Models.MobileApp;

namespace Apzon.Api.Controllers.DMS;

[ValidateLoginSapExceptionHandling]
public class DMSApprovedController : BaseApiController
{
	[HttpGet]
	public DataTable GetList()
	{
		try
		{
			string userName = SessionUserInfo.UserName;
			return UnitOfWork.DMSApproved.GetList(userName);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpGet]
	public DataTable GetListV2(string fromDate, string toDate)
	{
		try
		{
			string userName = SessionUserInfo.UserName;
			return UnitOfWork.DMSApproved.GetListV2(userName, fromDate, toDate);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpGet]
	public ResponseResult GetListItem(string objType, string userCode, string fromDate, string toDate, int pageIndex, string typeFilter, string searchText, int wddCode = -1)
	{
		try
		{
			return UnitOfWork.DMSApproved.GetListItem(objType, userCode, fromDate, toDate, pageIndex, typeFilter, searchText, wddCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = "406"
			};
		}
	}

	[HttpGet]
	public ResponseResult GetDetailItem(string userName, string status, string requestType, string wddCode)
	{
		try
		{
			return UnitOfWork.DMSApproved.GetDetailItem(userName, status, requestType, wddCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = "406"
			};
		}
	}

	[HttpGet]
	public ResponseResult GetTemplateDetails(int wddCode, int wtmCode)
	{
		try
		{
			return UnitOfWork.DMSApproved.GetTemplateDetails(wddCode, wtmCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = "406"
			};
		}
	}

	[HttpPost]
	public string SetStatus([FromBody] DataTable dt)
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
				if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
				{
					string userName = SessionUserInfo.UserName;
					List<Tuple<string, string>> list = new List<Tuple<string, string>>();
					for (int i = 0; i < dt.Rows.Count; i++)
					{
						dt.Rows[i]["Remarks"] = "";
						dt.Rows[i]["Password"] = EncodePassword.EncryptString(Function.ToString(dt.Rows[i]["Password"]), RSA.PasswordPhrase);
						DataTable dt2 = dt.Rows[i].ToArray().CopyToDataTable();
						string item = unitOfWork.Documents.ApproveDraft(SessionUserInfo.SessionDiServer, userName, dt2, string.IsNullOrWhiteSpace(Global.AppDataFolder) ? HttpContext.Current.Server.MapPath("~/App_Data/") : Global.AppDataFolder);
						list.Add(Tuple.Create(Function.ToString(dt.Rows[i]["WddCode"]), item));
					}
					string text = "";
					for (int j = 0; j < list.Count; j++)
					{
						if (!string.IsNullOrEmpty(list[j].Item2))
						{
							text += $"DocNo {list[j].Item1} approval process error: {list[j].Item2}\n";
						}
					}
					return text;
				}
				return "INVALIDSESSION";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return Resources.Msg_UnknownError;
	}

	[HttpPost]
	public ResponseResult NewSetStatus([FromBody] DataTable dt)
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return new ResponseResult
					{
						Message = "INVALIDSESSION",
						MessageCode = "405"
					};
				}
				IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
				if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
				{
					string userName = SessionUserInfo.UserName;
					DataTable dataTable = new DataTable();
					dataTable.Columns.Add("WddCode", typeof(string));
					dataTable.Columns.Add("Message", typeof(string));
					for (int i = 0; i < dt.Rows.Count; i++)
					{
						DataRow dataRow = dataTable.NewRow();
						dt.Rows[i]["Remarks"] = "";
						dt.Rows[i]["Password"] = EncodePassword.EncryptString(Function.ToString(dt.Rows[i]["Password"]), RSA.PasswordPhrase);
						DataTable dt2 = dt.Rows[i].ToArray().CopyToDataTable();
						string value = unitOfWork.Documents.ApproveDraft(SessionUserInfo.SessionDiServer, userName, dt2, string.IsNullOrWhiteSpace(Global.AppDataFolder) ? HttpContext.Current.Server.MapPath("~/App_Data/") : Global.AppDataFolder);
						dataRow["WddCode"] = Function.ToString(dt.Rows[i]["WddCode"]);
						dataRow["Message"] = value;
						dataTable.Rows.Add(dataRow.ItemArray);
					}
					dataTable.AcceptChanges();
					string text = "";
					for (int j = 0; j < dataTable.Rows.Count; j++)
					{
						if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[j]["Message"])))
						{
							text += string.Format("DocNo {0} approval process error: {1}\n", Function.ToString(dataTable.Rows[j]["WddCode"]), Function.ToString(dataTable.Rows[j]["Message"]));
						}
					}
					return new ResponseResult
					{
						Message = text,
						MessageCode = "200",
						Content = dataTable
					};
				}
				return new ResponseResult
				{
					Message = "INVALIDSESSION",
					MessageCode = "405"
				};
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = "403"
			};
		}
		return new ResponseResult
		{
			Message = Resources.Msg_UnknownError,
			MessageCode = "404"
		};
	}
}
