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

public class MrpController : BaseApiController
{
	private readonly IUnitOfWork _unitOfWork;

	[HttpGet]
	public DataTable SP_APZ_GetMRP(long mrpId)
	{
		try
		{
			return _unitOfWork.Mrp.SP_APZ_GetMRP(mrpId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpGet]
	public string GetItemByMpsCode(string mpsCode)
	{
		try
		{
			return _unitOfWork.Mrp.GetItemByMpsCode(mpsCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return string.Empty;
		}
	}

	[HttpPost]
	public DataTable MrpRun([FromBody] Tuple<Mrp, string> model)
	{
		try
		{
			Mrp item = model.Item1;
			string item2 = model.Item2;
			return _unitOfWork.Mrp.MrpRun(item, item2);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpGet]
	public DataTable GetOitmByItemCode(string itemCode)
	{
		try
		{
			return _unitOfWork.Mrp.GetOitmByItemCode(itemCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpGet]
	public DataTable SP_APZ_GetMRPCapactity(long mrpId)
	{
		try
		{
			return _unitOfWork.Mrp.SP_APZ_GetMRPCapactity(mrpId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpGet]
	public DataTable SP_APZ_MRPRun_Capacity(string mrpCode, DateTime fromDate, DateTime toDate)
	{
		try
		{
			return _unitOfWork.Mrp.SP_APZ_MRPRun_Capacity(mrpCode, fromDate, toDate);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpGet]
	public string UpdateStatus(string id, string status)
	{
		try
		{
			return _unitOfWork.Mrp.UpdateStatus(id, status);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	[HttpGet]
	public DataTable GetMrpByCode(string mrpCode)
	{
		try
		{
			return _unitOfWork.Mrp.GetMrpByCode(mrpCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpPost]
	public string Create([FromBody] Tuple<Mrp, List<Mrp1>, List<Mrp2>> model)
	{
		try
		{
			Mrp item = model.Item1;
			List<Mrp1> item2 = model.Item2;
			List<Mrp2> item3 = model.Item3;
			return _unitOfWork.Mrp.Create(item, item2, item3);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	[HttpPost]
	public string CreateData([FromBody] Tuple<Mrp, List<Mrp2>> model)
	{
		try
		{
			Mrp item = model.Item1;
			List<Mrp2> item2 = model.Item2;
			return _unitOfWork.Mrp.Create(item, item2);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	[HttpPost]
	public string Update([FromBody] Tuple<Mrp, List<Mrp1>, List<Mrp2>> model)
	{
		try
		{
			Mrp item = model.Item1;
			List<Mrp1> item2 = model.Item2;
			List<Mrp2> item3 = model.Item3;
			return _unitOfWork.Mrp.UpdateData(item, item2, item3);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	[HttpPost]
	public string UpdateData([FromBody] Tuple<Mrp, List<Mrp2>> model)
	{
		try
		{
			Mrp item = model.Item1;
			List<Mrp2> item2 = model.Item2;
			return _unitOfWork.Mrp.UpdateData(item, item2);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	[HttpGet]
	public int GetMaxId(string tableName, string fieldId)
	{
		try
		{
			return _unitOfWork.Mrp.GetMaxId(tableName, fieldId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return 0;
		}
	}

	[HttpGet]
	public string GetCodeMrp()
	{
		try
		{
			return _unitOfWork.Mrp.GetCodeMrp();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return string.Empty;
		}
	}

	[HttpGet]
	public DataTable GetMrpHeader()
	{
		try
		{
			return _unitOfWork.Mrp.GetMrpHeader();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpGet]
	public DataTable SP_APZ_MRPRun_SAP(string id, DateTime beginDate, DateTime endDate, string mpsCode)
	{
		try
		{
			return _unitOfWork.Mrp.SP_APZ_MRPRun_SAP(id, beginDate, endDate, mpsCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpGet]
	public DataTable usp_BOM_GetChangedItemByCode(string itemCode, DateTime dateChanged, double qty)
	{
		try
		{
			return _unitOfWork.Mrp.usp_BOM_GetChangedItemByCode(itemCode, dateChanged, qty);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpGet]
	public DataTable GetProductionOrder()
	{
		try
		{
			return _unitOfWork.Mrp.GetProductionOrder();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpPost]
	public DataTable GetMrp([FromBody] Mrp mrp)
	{
		try
		{
			return _unitOfWork.Mrp.GetMrp(mrp);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpPost]
	public string DeleteMrp([FromBody] Mrp mrp)
	{
		try
		{
			return _unitOfWork.Mrp.DeleteMrp(mrp);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	[HttpGet]
	public DataTable GetMrpByCode_CFL(string mrpCode)
	{
		try
		{
			return _unitOfWork.Mrp.GetMrpByCode_CFL(mrpCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpGet]
	public DataTable GetData_CFL(string status)
	{
		try
		{
			return _unitOfWork.Mrp.GetData_CFL(status);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}
}
