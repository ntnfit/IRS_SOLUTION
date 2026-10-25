using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.MobileApp;

namespace Apzon.Api.Controllers.DMS;

[ValidateLoginSapExceptionHandling]
public class SettingReportController : BaseApiController
{
	[HttpGet]
	public DataTable GetDefaultDataSourceReport(string code)
	{
		return UnitOfWork.SettingReport.GetDefaultDataSourceReport(code);
	}

	[HttpGet]
	public DataTable GetDataDashboardById(int docEntry, int id)
	{
		return UnitOfWork.SettingReport.GetDataDashboardById(docEntry, id);
	}

	[HttpGet]
	public DataTable LoadData(int id, string fromDate, string toDate, string report)
	{
		return UnitOfWork.SettingReport.LoadData(id, fromDate, toDate, report);
	}

	[HttpGet]
	public DataTable GetDataChartById(int Id)
	{
		return UnitOfWork.SettingReport.GetDataChartById(Id);
	}

	[HttpGet]
	public DataTable GetAllLineById(int Id)
	{
		return UnitOfWork.SettingReport.GetAllLineById(Id);
	}

	[HttpPost]
	public HttpResult Add([FromBody] DataSet ds)
	{
		return UnitOfWork.SettingReport.Save(ds, "A");
	}

	[HttpPost]
	public HttpResult Update([FromBody] DataSet ds)
	{
		return UnitOfWork.SettingReport.Save(ds, "U");
	}

	[HttpPost]
	public string DeleteDocs([FromBody] string Id)
	{
		return UnitOfWork.SettingReport.DeleteDocs(Id);
	}

	[HttpGet]
	public int GetNo()
	{
		return UnitOfWork.SettingReport.GetNo();
	}

	[HttpGet]
	public DataTable FindNextLastFrist(int id, int type)
	{
		return UnitOfWork.SettingReport.FindNextLastFrist(id, type);
	}

	[HttpGet]
	public DataTable GetColumnStoreReport(string code)
	{
		return UnitOfWork.SettingReport.GetColumnStoreReport(code);
	}

	[HttpGet]
	public ResponseResult GetDataById(int docEntry)
	{
		try
		{
			return UnitOfWork.SettingReport.GetDataById(docEntry);
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
	public DataTable GetAllData()
	{
		try
		{
			string userName = SessionUserInfo.UserName;
			return UnitOfWork.SettingReport.GetAllData(userName);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpGet]
	public DataTable GetAllDataByUserCode(string userCode)
	{
		return UnitOfWork.SettingReport.GetAllData(userCode);
	}

	[HttpPost]
	public DataTable DataSourceReport([FromBody] DataTable parameter)
	{
		try
		{
			return UnitOfWork.SettingReport.DataSourceReportByStore(parameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpGet]
	public DataSet GetParameterByCode(string code)
	{
		try
		{
			return UnitOfWork.SettingReport.GetParameterByCode(code);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	[HttpGet]
	public DataTable DataSourceCFL(string parName, string valuepar, string storeCFL)
	{
		try
		{
			return UnitOfWork.SettingReport.DataSourceCFL(parName, valuepar, storeCFL);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpGet]
	public DataTable GetReportByCode(string code)
	{
		try
		{
			return UnitOfWork.SettingReport.GetReportByCode(code);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpGet]
	public DataTable GetDataStoreNotParameter(string store)
	{
		try
		{
			return UnitOfWork.SettingReport.GetDataStoreNotParameter(store);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	private List<Tuple<string, int, string, List<Common.ParameterValue>>> GetUserProcedure(DataTable dt)
	{
		try
		{
			List<Tuple<string, int, string, List<Common.ParameterValue>>> list = new List<Tuple<string, int, string, List<Common.ParameterValue>>>();
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				string text = Function.ToString(dt.Rows[i]["SubType"]);
				if (string.IsNullOrEmpty(text))
				{
					continue;
				}
				List<Common.ParameterValue> list2 = new List<Common.ParameterValue>();
				string[] array = text.Split(' ');
				if (array.Length > 1 && array[1].StartsWith("@"))
				{
					text = array[0];
					string[] array2 = array[1].Split(',');
					for (int j = 0; j < array2.Length; j++)
					{
						string para = array2[j];
						if (array2[j].Contains("="))
						{
							int num = array2[j].IndexOf('=');
							list2.Add(new Common.ParameterValue(array2[j].Substring(0, num), array2[j].Substring(num + 1).Replace("'", "")));
						}
						else if (para.ToLower().Contains("username") || para.ToLower().Contains("usercode"))
						{
							list2.Add(new Common.ParameterValue(array2[j], SessionUserInfo.UserName));
						}
						else if ((from dr in dt.Rows.OfType<DataRow>()
							select dr.Field<string>("ParamName")).ToList().Any((string item) => item == para))
						{
							int index = (from dr in dt.Rows.OfType<DataRow>()
								select dr.Field<string>("ParamName")).ToList().FindIndex((string item) => item == para);
							object value = dt.Rows[index]["DefaultValue"];
							list2.Add(new Common.ParameterValue(array2[j], value));
						}
						else
						{
							list2.Add(new Common.ParameterValue(array2[j], array2[j]));
						}
					}
				}
				list.Add(Tuple.Create(Function.ToString(dt.Rows[i]["Code"]), Function.ParseInt(dt.Rows[i]["LineNum"]), text, list2));
			}
			return list;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	[HttpPost]
	public ResponseResult GetAllDataParamReport([FromBody] DataTable dt)
	{
		try
		{
			List<Tuple<string, int, string, List<Common.ParameterValue>>> userProcedure = GetUserProcedure(dt);
			List<Tuple<string, int, string, DataTable>> list = new List<Tuple<string, int, string, DataTable>>();
			for (int i = 0; i < userProcedure.Count; i++)
			{
				string item = userProcedure[i].Item3;
				List<Common.ParameterValue> item2 = userProcedure[i].Item4;
				DataSet dataSet = UnitOfWork.Commons.ExecuteStore(item, item2);
				if (dataSet.IsNotNull())
				{
					list.Add(Tuple.Create(userProcedure[i].Item1, userProcedure[i].Item2, userProcedure[i].Item3, dataSet.Tables[0]));
				}
				else
				{
					list.Add(Tuple.Create(userProcedure[i].Item1, userProcedure[i].Item2, userProcedure[i].Item3, new DataTable()));
				}
			}
			return new ResponseResult
			{
				Content = list,
				Message = "",
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
	public DataTable GetUserReportParams(int docEntry, string userCode)
	{
		return UnitOfWork.SettingReport.GetUserReportParams(docEntry, userCode);
	}

	[HttpPost]
	public ResponseResult SaveUserReportParams([FromBody] DataTable dt)
	{
		return UnitOfWork.SettingReport.SaveUserReportParams(dt);
	}

	[HttpPost]
	public ResponseResult ExecuteReport([FromBody] Tuple<string, List<Common.ParameterValue>> param)
	{
		try
		{
			DataSet dataSet = UnitOfWork.Commons.ExecuteStore(param.Item1, param.Item2);
			DataTable content = new DataTable();
			if (dataSet.IsNotNull())
			{
				content = dataSet.Tables[0];
			}
			return new ResponseResult
			{
				Content = content,
				Message = "",
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
	public ResponseResult LoadDataCfl([FromBody] DataTable dt)
	{
		try
		{
			string value = Function.ToString(dt.Rows[0]["CurrentParamName"]);
			List<Tuple<string, int, string, List<Common.ParameterValue>>> list = new List<Tuple<string, int, string, List<Common.ParameterValue>>>();
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				string text = Function.ToString(dt.Rows[i]["SubType"]);
				if (!text.Contains(value))
				{
					continue;
				}
				List<Common.ParameterValue> list2 = new List<Common.ParameterValue>();
				string[] array = text.Split(' ');
				if (array.Length > 1 && array[1].StartsWith("@"))
				{
					text = array[0];
					string[] array2 = array[1].Split(',');
					for (int j = 0; j < array2.Length; j++)
					{
						string para = array2[j];
						if (array2[j].Contains("="))
						{
							int num = array2[j].IndexOf('=');
							list2.Add(new Common.ParameterValue(array2[j].Substring(0, num), array2[j].Substring(num + 1).Replace("'", "")));
						}
						else if (para.ToLower().Contains("username") || para.ToLower().Contains("usercode"))
						{
							list2.Add(new Common.ParameterValue(array2[j], SessionUserInfo.UserName));
						}
						else if ((from dr in dt.Rows.OfType<DataRow>()
							select dr.Field<string>("ParamName")).ToList().Any((string text2) => text2 == para))
						{
							int index = (from dr in dt.Rows.OfType<DataRow>()
								select dr.Field<string>("ParamName")).ToList().FindIndex((string text2) => text2 == para);
							object value2 = dt.Rows[index]["CurrentValue"];
							list2.Add(new Common.ParameterValue(array2[j], value2));
						}
						else
						{
							list2.Add(new Common.ParameterValue(array2[j], array2[j]));
						}
					}
				}
				list.Add(Tuple.Create(Function.ToString(dt.Rows[i]["Code"]), Function.ParseInt(dt.Rows[i]["LineNum"]), text, list2));
			}
			List<Tuple<string, int, string, DataTable>> list3 = new List<Tuple<string, int, string, DataTable>>();
			for (int num2 = 0; num2 < list.Count; num2++)
			{
				string item = list[num2].Item3;
				List<Common.ParameterValue> item2 = list[num2].Item4;
				DataSet dataSet = UnitOfWork.Commons.ExecuteStore(item, item2);
				if (dataSet.IsNotNull())
				{
					list3.Add(Tuple.Create(list[num2].Item1, list[num2].Item2, list[num2].Item3, dataSet.Tables[0]));
				}
				else
				{
					list3.Add(Tuple.Create(list[num2].Item1, list[num2].Item2, list[num2].Item3, new DataTable()));
				}
			}
			return new ResponseResult
			{
				Content = list3,
				Message = "",
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
}
