using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.DataServices;
using Apzon.Entities.Models.PlanningProduction;

namespace Apzon.Api.Controllers.PlanningProduction;

public class MpsController : BaseApiController
{
	private readonly IUnitOfWork _unitOfWork;

	[HttpGet]
	public string GetCodeMps()
	{
		return UnitOfWork.Mps.GetCodeMps();
	}

	[HttpPost]
	public string UpdateStatus([FromBody] Tuple<string, string> model)
	{
		string item = model.Item1;
		string item2 = model.Item2;
		return _unitOfWork.Mps.UpdateStatus(item, item2);
	}

	[HttpPost]
	public DataTable GetWorkday1([FromBody] Mps model)
	{
		try
		{
			return _unitOfWork.Mps.GetWorkday1(model);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.Message);
			return new DataTable();
		}
	}

	[HttpPost]
	public DataTable GetWorkday2([FromBody] Mps model)
	{
		try
		{
			return _unitOfWork.Mps.GetWorkday2(model);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.Message);
			return new DataTable();
		}
	}

	[HttpPost]
	public DataTable GetMps1([FromBody] Mps model)
	{
		try
		{
			return _unitOfWork.Mps.GetMPS1(model);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.Message);
			return new DataTable();
		}
	}

	[HttpPost]
	public DataTable GetMps2([FromBody] Mps model)
	{
		try
		{
			return _unitOfWork.Mps.GetMPS2(model);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.Message);
			return new DataTable();
		}
	}

	[HttpPost]
	public DataTable RunMps1([FromBody] Mps model)
	{
		try
		{
			return _unitOfWork.Mps.RunMps1(model);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpPost]
	public DataTable RunMps2([FromBody] Mps model)
	{
		try
		{
			return _unitOfWork.Mps.RunMps2(model);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpPost]
	public DataTable RunWorkday1([FromBody] Mps model)
	{
		try
		{
			return _unitOfWork.Mps.RunWorkday1(model);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpPost]
	public DataTable RunWorkday2([FromBody] Mps model)
	{
		try
		{
			return _unitOfWork.Mps.RunWorkday2(model);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpGet]
	public Mps GetOmps(int id, string mpsCode)
	{
		try
		{
			return new Mps();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new Mps();
		}
	}

	[HttpPost]
	public string SaveData([FromBody] Tuple<Mps, DataTable, DataTable> model)
	{
		try
		{
			Mps item = model.Item1;
			DataTable item2 = model.Item2;
			DataTable item3 = model.Item3;
			return _unitOfWork.Mps.Save(item, item2, item3);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	[HttpGet]
	public List<Mps> GetListMps(string mpsCode, int status, DateTime bDate, DateTime eDate)
	{
		try
		{
			return _unitOfWork.Mps.GetListMps(mpsCode, status, bDate, eDate);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new List<Mps>();
		}
	}

	[HttpPost]
	public string DeleteData([FromBody] Mps model)
	{
		try
		{
			return _unitOfWork.Mps.DeleteData(model);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	[HttpGet]
	public DataTable GetMps(string mpsCode, int status, DateTime beginDate, DateTime endDate)
	{
		try
		{
			return _unitOfWork.Mps.GetMps(mpsCode, status, beginDate, endDate);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}
}
