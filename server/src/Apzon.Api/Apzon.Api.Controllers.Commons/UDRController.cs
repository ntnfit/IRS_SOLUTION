using System;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Commons;

public class UDRController : BaseApiController
{
	[HttpGet]
	public DataTable GetListUDR()
	{
		return UnitOfWork.UDRs.GetListUDR();
	}

	[HttpPost]
	public DataTable GetUDRByCode([FromBody] Common.Table_Code item)
	{
		return UnitOfWork.UDRs.GetUDRByCode(item);
	}

	[HttpGet]
	public DataSetResult GetParamByStore(string storeName, string code, string userName, int branch)
	{
		try
		{
			return UnitOfWork.UDRs.GetParamByStore(storeName, code, userName, branch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSetResult();
	}

	[HttpGet]
	public DataSetResult GetListReport(string userName, int branch)
	{
		try
		{
			return UnitOfWork.UDRs.GetListReport(userName, branch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSetResult();
	}

	[HttpPost]
	public HttpResult Add([FromBody] DataSet ds)
	{
		return UnitOfWork.UDRs.Save(ds, "A");
	}

	[HttpPost]
	public HttpResult Update([FromBody] DataSet ds)
	{
		return UnitOfWork.UDRs.Save(ds, "U");
	}

	[HttpPost]
	public HttpResult Delete([FromBody] DataSet ds)
	{
		return UnitOfWork.UDRs.Save(ds, "D");
	}

	[HttpGet]
	public DataTable GetParamsByCode(string code)
	{
		return UnitOfWork.UDRs.GetParamsByCode(code);
	}

	[HttpGet]
	public DataTable GetQueryByCode(string code)
	{
		return UnitOfWork.UDRs.GetQueryByCode(code);
	}
}
