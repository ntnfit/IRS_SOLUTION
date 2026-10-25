using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

internal class ProjectManagerClient : IProjectManagerClient
{
	private const string FormController = "ProjectManager";

	public async Task<DataTable> LoadData(int absEntry, string code, string status, string bpCode, string startFrom, string startTo, string userName)
	{
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ProjectManager");
			return await hp.GetAsync("LoadData", new { absEntry, code, status, bpCode, startFrom, startTo, userName });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataSet> GetDataById(int Id)
	{
		try
		{
			using HttpClientHelper<DataSet> hp = new HttpClientHelper<DataSet>(Configuration.ApiServer, "ProjectManager");
			return await hp.GetAsync("GetDataById", new { Id });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<HttpResult> Add(DataSet ds)
	{
		try
		{
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "ProjectManager");
			return await helper.PostAsync("Add", ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<HttpResult> Update(DataSet ds)
	{
		try
		{
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "ProjectManager");
			return await helper.PostAsync("Update", ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<string> DeleteDocs(string Id)
	{
		try
		{
			using HttpClientHelper<string> heper = new HttpClientHelper<string>(Configuration.ApiServer, "ProjectManager");
			return await heper.PostParamsAsJsonAsync("DeleteDocs", Id);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetDataTypeById(string Id)
	{
		try
		{
			using HttpClientHelper<DataTable> heper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ProjectManager");
			return await heper.GetAsync("GetDataTypeById", new { Id });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> LoadSubProjectByMasterId(int projectId)
	{
		try
		{
			using HttpClientHelper<DataTable> heper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ProjectManager");
			return await heper.GetAsync("LoadSubProjectByMasterId", new { projectId });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetDataBOMSaleOrder(string code)
	{
		try
		{
			using HttpClientHelper<DataTable> heper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ProjectManager");
			return await heper.GetAsync("GetDataBOMSaleOrder", new { code });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetDataItemProduction(string code)
	{
		try
		{
			using HttpClientHelper<DataTable> heper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ProjectManager");
			return await heper.GetAsync("GetDataItemProduction", new { code });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetDataItemInBom(string code)
	{
		try
		{
			using HttpClientHelper<DataTable> heper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ProjectManager");
			return await heper.GetAsync("GetDataItemInBom", new { code });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetDataItemGroup()
	{
		try
		{
			using HttpClientHelper<DataTable> heper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ProjectManager");
			return await heper.GetAsync("GetDataItemGroup");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<HttpResult> UpdateStatus(int docEntry, string status)
	{
		try
		{
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "ProjectManager");
			return await helper.PostAsync("UpdateStatus", new Tuple<int, string>(docEntry, status));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> FindNextLastFrist(int id, int type)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ProjectManager");
			return await helper.GetAsync("FindNextLastFrist", new { id, type });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataSet> GetDataLineByWbsTemplate(string code)
	{
		try
		{
			using HttpClientHelper<DataSet> heper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "ProjectManager");
			return await heper.GetAsync("GetDataLineByWbsTemplate", new { code });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable GetIdWorkScheduling(int year)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ProjectManager");
			return httpClientHelper.Get("GetIdWorkScheduling", new { year });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable GetDataShift(string code)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ProjectManager");
			return httpClientHelper.Get("GetDataShift", new { code });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetTaskTemplateByCode(string code)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ProjectManager");
			return await helper.GetAsync("GetTaskTemplateByCode", new { code });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetTaskByWbsCode(string baseType, int baseEntry, int baseLine, string wbsCode)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ProjectManager");
			return await helper.GetAsync("GetTaskByWbsCode", new { baseType, baseEntry, baseLine, wbsCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetStruct()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ProjectManager");
			return await helper.GetAsync("GetStruct");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw;
		}
	}

	public string CheckItemInDocument(int Id, int lineNum = -1)
	{
		try
		{
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "ProjectManager");
			return httpClientHelper.Get("CheckItemInDocument", new { Id, lineNum });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable GetShifOrderByTime(string code)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ProjectManager");
			return httpClientHelper.Get("GetShifOrderByTime", new { code });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable GetBrachByEmpID(int empId)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ProjectManager");
			return httpClientHelper.Get("GetBrachByEmpID", new { empId });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public string CheckMltpBrnchs()
	{
		try
		{
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "ProjectManager");
			return httpClientHelper.Get("CheckMltpBrnchs");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetScheduleFromDate(string startDate)
	{
		try
		{
			using HttpClientHelper<DataTable> heper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ProjectManager");
			return await heper.GetAsync("GetScheduleFromDate", new { startDate });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetAllBaseLine(int absEntry)
	{
		try
		{
			using HttpClientHelper<DataTable> heper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ProjectManager");
			return await heper.GetAsync("GetAllBaseLine", new { absEntry });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<HttpResult> SetApproval(string status, int id)
	{
		try
		{
			using HttpClientHelper<HttpResult> heper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "ProjectManager");
			return await heper.PostAsync("SetApproval", new NameID_Short
			{
				ID = id,
				Name = status
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetTreeData(string itemCode)
	{
		using HttpClientHelper<DataTable> heper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ProjectManager");
		return await heper.GetAsync("GetTreeData", new { itemCode });
	}

	public async Task<HttpResult> UndoApproval(int absEntry)
	{
		using HttpClientHelper<HttpResult> heper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "ProjectManager");
		return await heper.PostAsync("UndoApproval", absEntry);
	}
}
