using System;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Sales;

public class AtpController : BaseApiController
{
	[HttpPost]
	public DataSet SearchData([FromBody] DataTable dtSearch)
	{
		try
		{
			return UnitOfWork.Atp.Search(dtSearch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public DataTable GetDataAtpType()
	{
		return UnitOfWork.Atp.GetDataAtpType();
	}

	[HttpGet]
	public HttpResult GetNewIdAtp()
	{
		return UnitOfWork.Atp.GetNewIdAtp();
	}

	[HttpGet]
	public DataTable GetStructureInput()
	{
		return UnitOfWork.Atp.GetStructureInput();
	}

	[HttpGet]
	public DataTable GetDocStructure()
	{
		return UnitOfWork.Atp.GetDocStructure();
	}

	[HttpPost]
	public DataTable RunAtp([FromBody] DataSet ds)
	{
		return UnitOfWork.Atp.RunAtp(ds);
	}

	[HttpPost]
	public HttpResult Save([FromBody] DataSet ds)
	{
		return UnitOfWork.Atp.Save(ds, Enums.TransactionType.A, "");
	}

	[HttpPost]
	public HttpResult AtpUpdate([FromBody] DataSet ds)
	{
		return UnitOfWork.Atp.Save(ds, Enums.TransactionType.U, "");
	}

	[HttpGet]
	public DataSet SearchData(Enums.NavigateType type, int code)
	{
		return UnitOfWork.Atp.SearchData(type, code);
	}

	[HttpGet]
	public DataTable GetDataGenPurchase(string input, int code, string user)
	{
		return UnitOfWork.Atp.GetDataGenPurchase(input, code, user);
	}

	[HttpGet]
	public DataTable GetStructureGenPurchase()
	{
		return UnitOfWork.Atp.GetStructureGenPurchase();
	}

	[HttpGet]
	public DataTable GetSeriesOrderAndRequest(string type)
	{
		return UnitOfWork.Schedule.GetSeriesOrderAndRequest(type);
	}

	[HttpGet]
	public HttpResult GetNewIdScheduleForAtp()
	{
		return UnitOfWork.Atp.GetNewIdScheduleForAtp();
	}

	[HttpGet]
	public DataSet GetDataAtpForSchedle(int code)
	{
		return UnitOfWork.Atp.GetDataAtpForSchedle(code);
	}

	[HttpGet]
	public DataSet RunSchedule(string code, string type)
	{
		return UnitOfWork.Atp.RunSchedule(code, type);
	}

	[HttpGet]
	public DataSet GetDataLineSchedule(int id)
	{
		return UnitOfWork.Atp.GetDataLineSchedule(id);
	}

	[HttpGet]
	public DataTable GetDocScheduleStructure()
	{
		return UnitOfWork.Atp.GetDocScheduleStructure();
	}

	[HttpPost]
	public HttpResult SaveSchedule(DataSet dtSet)
	{
		return UnitOfWork.Atp.SaveSchedule(dtSet, Enums.TransactionType.A, "");
	}

	[HttpPost]
	public HttpResult UpdateSchedule(DataSet dtSet)
	{
		return UnitOfWork.Atp.SaveSchedule(dtSet, Enums.TransactionType.U, "");
	}

	[HttpGet]
	public DataTable GetDataDocSchedule(int code, Enums.NavigateType type)
	{
		return UnitOfWork.Atp.GetDataDocSchedule(code, type);
	}

	[HttpGet]
	public DataSet GetDataStructureGenProduction()
	{
		return UnitOfWork.Atp.GetDataStructureGenProduction();
	}

	[HttpGet]
	public DataTable GetDataGenProduction(string id, string type, string inputType)
	{
		try
		{
			return UnitOfWork.Atp.GetDataGenProduction(id, type);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	[HttpGet]
	public string CheckInputData(string type, int code, string genType)
	{
		return UnitOfWork.Atp.CheckInputData(type, code, genType);
	}
}
