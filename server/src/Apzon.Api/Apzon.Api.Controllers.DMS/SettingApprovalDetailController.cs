using System;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.MobileApp;

namespace Apzon.Api.Controllers.DMS;

[ValidateLoginSapExceptionHandling]
public class SettingApprovalDetailController : BaseApiController
{
	[HttpGet]
	public DataTable GetDataHeaderById(int Id)
	{
		return UnitOfWork.SettingApprovalDetail.GetDataHeaderById(Id);
	}

	[HttpGet]
	public DataTable LoadData(int id, string fromDate, string toDate, string objectType)
	{
		return UnitOfWork.SettingApprovalDetail.LoadData(id, fromDate, toDate, objectType);
	}

	[HttpGet]
	public DataTable GetAllLineById(int Id)
	{
		return UnitOfWork.SettingApprovalDetail.GetAllLineById(Id);
	}

	[HttpPost]
	public HttpResult Add([FromBody] DataSet ds)
	{
		return UnitOfWork.SettingApprovalDetail.Save(ds, "A");
	}

	[HttpPost]
	public HttpResult Update([FromBody] DataSet ds)
	{
		return UnitOfWork.SettingApprovalDetail.Save(ds, "U");
	}

	[HttpPost]
	public string DeleteDocs([FromBody] string Id)
	{
		return UnitOfWork.SettingApprovalDetail.DeleteDocs(Id);
	}

	[HttpGet]
	public ResponseResult GetDataById(int docEntry)
	{
		try
		{
			return UnitOfWork.SettingApprovalDetail.GetDataById(docEntry);
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
	public ResponseResult GetDataSource(string docNo)
	{
		try
		{
			return UnitOfWork.SettingApprovalDetail.GetDataSource(docNo);
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
}
