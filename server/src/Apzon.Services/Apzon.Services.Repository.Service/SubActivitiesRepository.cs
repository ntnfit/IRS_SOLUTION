using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Service;
using SAPbobsCOM;

namespace Apzon.Services.Repository.Service;

public class SubActivitiesRepository(IDatabaseService databaseService) : BaseService<DataSet>(databaseService), ISubActivities
{
	private string processType = "";

	private int _saveEventType;

	private int _removeEventType;

	private List<int> additionNotifyUser = new List<int>();

	private int commentCode;

	private string TempTableTab = "#dtAPZ_OCLG";

	private string TempNote = "#APZ_Note";

	private string TemComment = "#APZ_Comment";

	private string TempEvent = "#APZ_Event";

	private string TempAtt = "#APZ_Attachment";

	private string TempUnreadComment = "#dtAPZ_CLG3";

	public DataTable GetAll(string userCode)
	{
		try
		{
			string query = "APZ_SubActivities_GetByCode";
			List<SqlParameter> list = new List<SqlParameter>();
			SqlParameter item = new SqlParameter("@UserName", userCode ?? "");
			SqlParameter item2 = new SqlParameter("@ObjTypeTask", Function.ToString(1470000128));
			SqlParameter item3 = new SqlParameter("@ObjTypeEvent", Function.ToString(1470000129));
			SqlParameter item4 = new SqlParameter("@ObjTypeNote", Function.ToString(19000102));
			list.Add(item);
			list.Add(item2);
			list.Add(item3);
			list.Add(item4);
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetCalendarResources(string user)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@user", user);
			return ExecuteDataTable("APZ_SubActivities_GetCalResources", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetUsers  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetByCode(int clgCode, string userCode)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@UserName", userCode));
			list.Add(new SqlParameter("@ObjTypeTask", Function.ToString(1470000128)));
			list.Add(new SqlParameter("@ObjTypeEvent", Function.ToString(1470000129)));
			list.Add(new SqlParameter("@ObjTypeNote", Function.ToString(19000102)));
			list.Add(new SqlParameter("@ActionType", ""));
			list.Add(new SqlParameter("@ClgCode", Function.ToString(clgCode)));
			return ExecuteDataTable("APZ_SubActivities_GetByCode", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public HttpResult GetNotesByClgCode(string clgCode)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@clgCode", Function.ToString(clgCode)));
			DataTable content = ExecuteDataTable("select \"Notes\" from APZ_OCLG where \"ClgCode\" = @clgCode;", CommandType.Text, list.ToArray());
			return new HttpResult
			{
				Message = "",
				MessageCode = Enums.MessageCode.Success,
				Content = content
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetNotesByClgCode  FAILED ", ex.Message);
			return new HttpResult
			{
				Message = ex.Message,
				MessageCode = Enums.MessageCode.ExceptionMessage
			};
		}
	}

	public DataTable GetBySeriesNum(int seriesNum, string userCode)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@UserName", userCode));
			list.Add(new SqlParameter("@ObjTypeTask", Function.ToString(1470000128)));
			list.Add(new SqlParameter("@ObjTypeEvent", Function.ToString(1470000129)));
			list.Add(new SqlParameter("@ObjTypeNote", Function.ToString(19000102)));
			list.Add(new SqlParameter("@ActionType", ""));
			list.Add(new SqlParameter("@ClgCode", SqlDbType.BigInt));
			list.Add(new SqlParameter("@SeriesNum", Function.ToString(seriesNum)));
			return ExecuteDataTable("APZ_SubActivities_GetByCode", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDocByAction(string action, string userCode)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@UserName", userCode));
			list.Add(new SqlParameter("@ObjTypeTask", Function.ToString(1470000128)));
			list.Add(new SqlParameter("@ObjTypeEvent", Function.ToString(1470000129)));
			list.Add(new SqlParameter("@ObjTypeNote", Function.ToString(19000102)));
			list.Add(new SqlParameter("@ActionType", Function.ToString(action)));
			return ExecuteDataTable("APZ_SubActivities_GetByCode", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDocByResource(string resourceId, string userCode)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ResourceId", Function.ToString(resourceId)));
			list.Add(new SqlParameter("@UserName", userCode));
			list.Add(new SqlParameter("@ObjTypeTask", Function.ToString(1470000128)));
			list.Add(new SqlParameter("@ObjTypeEvent", Function.ToString(1470000129)));
			list.Add(new SqlParameter("@ObjTypeNote", Function.ToString(19000102)));
			return ExecuteDataTable("APZ_SubActivities_GetByCode", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable DMSGetDocByInterval(DataTable dtSearch)
	{
		try
		{
			if (dtSearch.IsNotNull())
			{
				string value = Function.ToString(dtSearch.Rows[0]["userCode"]);
				DateTime dateTime = Function.ParseDateTimes(dtSearch.Rows[0]["startDate"]);
				DateTime dateTime2 = Function.ParseDateTimes(dtSearch.Rows[0]["endDate"]);
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@UserName", value));
				list.Add(new SqlParameter("@ObjTypeTask", Function.ToString(1470000128)));
				list.Add(new SqlParameter("@ObjTypeEvent", Function.ToString(1470000129)));
				list.Add(new SqlParameter("@ObjTypeNote", Function.ToString(19000102)));
				list.Add(new SqlParameter("@ActionType", ""));
				if (dtSearch.IsExistsColumn("clgCode"))
				{
					list.Add(new SqlParameter("@ClgCode", (!string.IsNullOrEmpty(Function.ToString(dtSearch.Rows[0]["clgCode"]))) ? Function.ParseInt(dtSearch.Rows[0]["clgCode"]) : 0));
				}
				else
				{
					list.Add(new SqlParameter("@ClgCode", SqlDbType.BigInt));
				}
				list.Add(new SqlParameter("@SeriesNum", SqlDbType.BigInt));
				list.Add(string.IsNullOrEmpty(Function.ToString(dtSearch.Rows[0]["startDate"])) ? new SqlParameter("@StartDate", DBNull.Value) : new SqlParameter("@StartDate", dateTime));
				list.Add(string.IsNullOrEmpty(Function.ToString(dtSearch.Rows[0]["endDate"])) ? new SqlParameter("@EndDate", DBNull.Value) : new SqlParameter("@EndDate", dateTime2));
				return ExecuteDataTable("APZ_D_SubActivities_GetByCode", CommandType.StoredProcedure, list.ToArray());
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDocByInterval(DataTable dtSearch)
	{
		try
		{
			if (dtSearch.IsNotNull())
			{
				string value = Function.ToString(dtSearch.Rows[0]["userCode"]);
				DateTime dateTime = Function.ParseDateTimes(dtSearch.Rows[0]["startDate"]);
				DateTime dateTime2 = Function.ParseDateTimes(dtSearch.Rows[0]["endDate"]);
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@UserName", value));
				list.Add(new SqlParameter("@ObjTypeTask", Function.ToString(1470000128)));
				list.Add(new SqlParameter("@ObjTypeEvent", Function.ToString(1470000129)));
				list.Add(new SqlParameter("@ObjTypeNote", Function.ToString(19000102)));
				list.Add(new SqlParameter("@ActionType", ""));
				list.Add(new SqlParameter("@ClgCode", SqlDbType.BigInt));
				list.Add(new SqlParameter("@SeriesNum", SqlDbType.BigInt));
				list.Add(string.IsNullOrEmpty(Function.ToString(dtSearch.Rows[0]["startDate"])) ? new SqlParameter("@StartDate", DBNull.Value) : new SqlParameter("@StartDate", dateTime));
				list.Add(string.IsNullOrEmpty(Function.ToString(dtSearch.Rows[0]["endDate"])) ? new SqlParameter("@EndDate", DBNull.Value) : new SqlParameter("@EndDate", dateTime2));
				return ExecuteDataTable("APZ_SubActivities_GetByCode", CommandType.StoredProcedure, list.ToArray());
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable SearchByIntervalMobile(DataTable dtSearch)
	{
		try
		{
			if (dtSearch.IsNotNull())
			{
				string value = Function.ToString(dtSearch.Rows[0]["userCode"]);
				string value2 = Function.ToString(dtSearch.Rows[0]["details"]);
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@UserName", value));
				list.Add(new SqlParameter("@ObjTypeTask", Function.ToString(1470000128)));
				list.Add(new SqlParameter("@ObjTypeEvent", Function.ToString(1470000129)));
				list.Add(new SqlParameter("@ObjTypeNote", Function.ToString(19000102)));
				list.Add(new SqlParameter("@Details", value2));
				return ExecuteDataTable("APZ_D_SubActivities_SearchByDetails", CommandType.StoredProcedure, list.ToArray());
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public string SaveActivityInterval(DataTable dt)
	{
		try
		{
			processType = "CalendarSaveInterval";
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(dt.Copy());
			return Process(ApzObjectTypes.oTaskManagement, -1, Enums.TransactionType.A, dataSet).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SaveInterval  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public DataTable LoadTask(string userName, string taskStatus = "")
	{
		try
		{
			string query = "APZ_SubActivities_LoadTask";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@UserName", userName));
			list.Add(new SqlParameter("@TaskStatus", taskStatus));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadTask  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable LoadTaskAndSubTask(string userName, ApzObjectTypes objType, string objId)
	{
		try
		{
			string query = "APZ_SubActivities_LoadTaskAndSubTask";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@UserName", userName));
			string value = Function.ToString((int)objType);
			list.Add(new SqlParameter("@ObjType", value));
			list.Add(new SqlParameter("@ObjId", Function.ToString(objId)));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadTaskAndSubTask  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable LoadTaskByObjectType(string userName, ApzObjectTypes objType, string objId)
	{
		try
		{
			string query = "APZ_SubActivities_LoadTaskByObjectType";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@UserName", userName));
			string value = Function.ToString((int)objType);
			list.Add(new SqlParameter("@ObjType", value));
			list.Add(new SqlParameter("@ObjId", Function.ToString(objId)));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadTaskByObjectType  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable LoadSingleTask(int clgCode, string userName)
	{
		try
		{
			string query = "APZ_SubActivities_LoadSingleTask";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ClgCode", clgCode));
			list.Add(new SqlParameter("@UserName", userName));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadSingleTask  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable LoadSubTask(int clgCode, string userName)
	{
		try
		{
			string query = "APZ_SubActivities_LoadSubTask";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ClgCode", clgCode));
			list.Add(new SqlParameter("@UserName", userName));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadSubTask  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable LoadSubNode(int clgCode, string userName)
	{
		try
		{
			string query = "APZ_SubActivities_LoadSubTask";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ClgCode", clgCode));
			list.Add(new SqlParameter("@UserName", userName));
			list.Add(new SqlParameter("@LoadForNode", 'Y'));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadSubNode  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public string GetCardTypeByCardCode(string cardCode)
	{
		try
		{
			string query = "select CASE WHEN  B.\"CardType\" = 'C' then 'Customer'\r\n\t\t\t                               WHEN  B.\"CardType\" = 'L' then 'Lead'\r\n\t\t\t                               WHEN  B.\"CardType\" = 'S' then 'Vendor'\t\r\n                                      END AS \"CardType\"\r\n                               from  OCRD B\r\n                               where B.\"CardCode\" =@CardCode;";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@CardCode", cardCode));
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ToString(dataTable.Rows[0]["CardType"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetListContactPerson  FAILED ", ex.Message);
		}
		return "";
	}

	public DataTable LoadParentTask(int parentTask)
	{
		try
		{
			string query = "APZ_SubActivities_GetParentTask";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@parentTask", parentTask));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadParentTask  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable LoadTaskManagementSetting(string userSign)
	{
		try
		{
			string query = "SELECT * FROM APZ_CLG2 WHERE \"UserSign\"=@UserSign;";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@UserSign", userSign));
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadTaskManagementSetting  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public HttpResult AddTask(DataSet dt)
	{
		try
		{
			processType = "AddTask";
			DataTable dataTable = dt.Tables[0];
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				SetAutoValueForTask(dataTable.Rows[i], isAdd: true);
			}
			return Process(ApzObjectTypes.oTaskManagement, -1, Enums.TransactionType.A, dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult AddMuntiTask(DataTable dt)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string value = "";
				string value2 = "";
				if (dt.IsNotNull())
				{
					if (string.IsNullOrWhiteSpace(value2))
					{
						for (int i = 0; i < dt.Rows.Count; i++)
						{
							DataTable dataTable = dt.Copy().Clone();
							DataRow row = dt.Rows[i];
							dataTable.ImportRow(row);
							CreateTempTable(dataTable, TempTableTab);
							BulkCopy(dataTable, TempTableTab);
							if (!dataTable.IsNotNull())
							{
								return new HttpResult(Enums.MessageCode.Error, "Data null");
							}
							if (string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["Details"])))
							{
								return new HttpResult(Enums.MessageCode.Error, "Detail null");
							}
							string query = "APZ_SubActivities_AddTask";
							value2 = Function.ToString(ExecuteNonQuery(query, CommandType.StoredProcedure));
							if (string.IsNullOrWhiteSpace(value2))
							{
								value = value2;
								break;
							}
							ExecuteNonQuery("drop table \"#dtAPZ_OCLG\";", CommandType.Text);
						}
					}
				}
				else
				{
					value = "Data null";
				}
				if (string.IsNullOrWhiteSpace(value))
				{
					dbTransaction.Commit();
					return new HttpResult();
				}
				dbTransaction.Rollback();
				return new HttpResult(Enums.MessageCode.Error, "Data is not valid");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult SaveTask(DataSet dt)
	{
		try
		{
			processType = "SaveTask";
			DataTable dataTable = dt.Tables[0];
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				SetAutoValueForTask(dataTable.Rows[i]);
			}
			return Process(ApzObjectTypes.oTaskManagement, -1, Enums.TransactionType.U, dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public static void SetAutoValueForTask(DataRow dr, bool isAdd = false)
	{
		try
		{
			if (Function.ToString(dr["Closed"]) == "Y")
			{
				dr["CloseDate"] = DateTime.Now;
				dr["status"] = 2;
			}
			else
			{
				dr["status"] = 1;
			}
			if (isAdd)
			{
				dr["CntctDate"] = DateTime.Today;
				dr["CntctTime"] = Function.ParseTimeSpanToInt(DateTime.Now.TimeOfDay);
			}
			else
			{
				dr["UpdateDate"] = DateTime.Today;
				dr["UpdateTime"] = Function.ParseTimeSpanToInt(DateTime.Now.TimeOfDay);
			}
			if (string.IsNullOrWhiteSpace(Function.ToString(dr["GroupUser"])) && string.IsNullOrWhiteSpace(Function.ToString(dr["GroupEmployee"])) && !string.IsNullOrWhiteSpace(Function.ToString(dr["AssignedBy"])))
			{
				dr["GroupUser"] = dr["AssignedBy"];
			}
			string value = ResMapping(dr);
			dr["ResourceIdsInfo"] = value;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.Message);
		}
	}

	public static string ResMapping(DataRow dr)
	{
		string result = "";
		try
		{
			List<string> list = new List<string>();
			string text = Function.ToString(dr["GroupUser"]);
			string text2 = Function.ToString(dr["GroupEmployee"]);
			if (text != "")
			{
				string text3 = text;
				if (!string.IsNullOrWhiteSpace(text3))
				{
					string[] array = Function.ToString(text3).Split(';');
					foreach (string text4 in array)
					{
						if (!string.IsNullOrWhiteSpace(text4))
						{
							list.Add("U" + Function.ToString(text4));
						}
					}
				}
			}
			if (text == "")
			{
				string text3 = text2;
				if (!string.IsNullOrWhiteSpace(text3))
				{
					string[] array = Function.ToString(text3).Split(';');
					foreach (string text5 in array)
					{
						if (!string.IsNullOrWhiteSpace(text5))
						{
							list.Add("E" + Function.ToString(text5));
						}
					}
				}
			}
			ResourceIds resourceIds = new ResourceIds
			{
				ListResourceId = new List<ResourceId>()
			};
			foreach (string item2 in list)
			{
				ResourceId item = new ResourceId
				{
					Type = "System.String",
					Value = item2
				};
				resourceIds.ListResourceId.Add(item);
			}
			XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
			xmlSerializerNamespaces.Add("", "");
			XmlWriterSettings settings = new XmlWriterSettings
			{
				Indent = true,
				OmitXmlDeclaration = true
			};
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(ResourceIds));
			using StringWriter stringWriter = new StringWriter();
			using XmlWriter xmlWriter = XmlWriter.Create(stringWriter, settings);
			xmlSerializer.Serialize(xmlWriter, resourceIds, xmlSerializerNamespaces);
			result = stringWriter.ToString();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.Message);
		}
		return result;
	}

	public HttpResult ChangeTaskStatus(DataSet dt)
	{
		processType = "ChangeTaskStatus";
		return Process(ApzObjectTypes.oTaskManagement, -1, Enums.TransactionType.A, dt);
	}

	public HttpResult SaveSubTaskDetails(DataSet dt)
	{
		processType = "SaveSubTaskDetails";
		return Process(ApzObjectTypes.oTaskManagement, -1, Enums.TransactionType.A, dt);
	}

	public HttpResult SaveSubTaskAssigner(DataSet dt)
	{
		processType = "SaveSubTaskAssigner";
		return Process(ApzObjectTypes.oTaskManagement, -1, Enums.TransactionType.A, dt);
	}

	public HttpResult SaveTaskPosition(DataSet dt)
	{
		processType = "SaveTaskPosition";
		return Process(ApzObjectTypes.oTaskManagement, -1, Enums.TransactionType.A, dt);
	}

	public HttpResult SaveTaskManagementSetting(DataSet dt)
	{
		processType = "SaveTaskManagementSetting";
		return Process(ApzObjectTypes.oTaskManagement, -1, Enums.TransactionType.A, dt);
	}

	public string RemoveTask(DataTable dt)
	{
		try
		{
			processType = "RemoveTask";
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(dt);
			return Process(ApzObjectTypes.oTaskManagement, -1, Enums.TransactionType.D, dataSet).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Save  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public DataTable LoadNote(string userCode, int projectCode)
	{
		try
		{
			string query = "APZ_SubActivities_LoadNote";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@UserName", userCode));
			list.Add(new SqlParameter("@ObjType", Function.ToString(19000102)));
			list.Add(new SqlParameter("@ClgCode", DBNull.Value));
			list.Add(new SqlParameter("@StartDate", DBNull.Value));
			list.Add(new SqlParameter("@EndDate", DBNull.Value));
			if (projectCode != -1)
			{
				list.Add(new SqlParameter("@ProjectCode", projectCode));
			}
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable LoadNoteByObjectType(string userName, ApzObjectTypes objTypeNote, ApzObjectTypes objType, string objId)
	{
		try
		{
			string query = "APZ_SubActivities_LoadNoteByObjType";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@UserName", userName));
			string value = Function.ToString((int)objTypeNote);
			list.Add(new SqlParameter("@objTypeNote", value));
			string value2 = Function.ToString((int)objType);
			list.Add(new SqlParameter("@ObjType", value2));
			list.Add(new SqlParameter("@ObjId", Function.ToString(objId)));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadNoteByObjectType  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable LoadNoteByCode(int clgCode)
	{
		try
		{
			string query = "SELECT * FROM APZ_OCLG WHERE \"Action\" = 'E' and \"ClgCode\" = @clgCode;";
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@clgCode", clgCode)
			};
			return ExecuteDataTable(query, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable LoadNoteByInterval(DataTable dtSearch)
	{
		try
		{
			if (dtSearch.IsNotNull())
			{
				string value = Function.ToString(dtSearch.Rows[0]["userCode"]);
				DateTime dateTime = Function.ParseDateTimes(dtSearch.Rows[0]["startDate"]);
				DateTime dateTime2 = Function.ParseDateTimes(dtSearch.Rows[0]["endDate"]);
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@UserName", value));
				list.Add(new SqlParameter("@ObjType", Function.ToString(19000102)));
				list.Add(new SqlParameter("@clgCode", DBNull.Value));
				list.Add(string.IsNullOrEmpty(Function.ToString(dtSearch.Rows[0]["startDate"])) ? new SqlParameter("@StartDate", DBNull.Value) : new SqlParameter("@StartDate", dateTime));
				list.Add(string.IsNullOrEmpty(Function.ToString(dtSearch.Rows[0]["endDate"])) ? new SqlParameter("@EndDate", DBNull.Value) : new SqlParameter("@EndDate", dateTime2));
				return ExecuteDataTable("APZ_SubActivities_LoadNote", CommandType.StoredProcedure, list.ToArray());
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public string DeleteNote(DataTable dt)
	{
		DataSet dataSet = new DataSet();
		dataSet.Tables.Add(dt);
		processType = "DeleteNote";
		return Process(ApzObjectTypes.oTaskManagement, -1, Enums.TransactionType.D, dataSet).Message;
	}

	public HttpResult SaveNote(DataSet dt)
	{
		processType = "SaveNote";
		return Process(ApzObjectTypes.oNoteManagement, -1, Enums.TransactionType.A, dt);
	}

	public string PushAppNotification(string NoteId)
	{
		try
		{
			string query = "APZ_SubActivities_PushAppNotification";
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@clgCode", NoteId)
			};
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable.IsNotNull())
			{
				return Function.ToString(dataTable.Rows[0][1]);
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "PushAppNotification  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public DataTable LoadEvent(string userCode, int projectCode)
	{
		try
		{
			string query = "APZ_SubActivities_LoadEvent";
			List<SqlParameter> list = new List<SqlParameter>();
			SqlParameter item = new SqlParameter("@UserName", userCode);
			SqlParameter item2 = new SqlParameter("@ObjType", Function.ToString(1470000129));
			list.Add(item);
			list.Add(item2);
			list.Add(new SqlParameter("@clgCode", DBNull.Value));
			list.Add(new SqlParameter("@StartDate", DBNull.Value));
			list.Add(new SqlParameter("@EndDate", DBNull.Value));
			if (projectCode != -1)
			{
				list.Add(new SqlParameter("@ProjectCode", projectCode));
			}
			list.Add(new SqlParameter("@LimitData", DBNull.Value));
			list.Add(new SqlParameter("@Skip", DBNull.Value));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable LoadEventByObjectType(string userName, ApzObjectTypes objTypeEvent, ApzObjectTypes objType, string objId)
	{
		try
		{
			string query = "APZ_SubActivities_LoadEventByObjType";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@UserName", userName));
			string value = Function.ToString((int)objTypeEvent);
			list.Add(new SqlParameter("@ObjTypeEvent", value));
			string value2 = Function.ToString((int)objType);
			list.Add(new SqlParameter("@ObjType", value2));
			list.Add(new SqlParameter("@ObjId", Function.ToString(objId)));
			ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadEventByObjectType  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable LoadEventByCode(int clgCode, string userCode)
	{
		try
		{
			string query = "APZ_SubActivities_LoadEvent";
			List<SqlParameter> list = new List<SqlParameter>();
			SqlParameter item = new SqlParameter("@UserName", userCode);
			SqlParameter item2 = new SqlParameter("@ObjType", Function.ToString(1470000129));
			list.Add(item);
			list.Add(item2);
			list.Add(new SqlParameter("@ClgCode", Function.ParseInt(clgCode)));
			list.Add(new SqlParameter("@StartDate", DBNull.Value));
			list.Add(new SqlParameter("@EndDate", DBNull.Value));
			list.Add(new SqlParameter("@ProjectCode", DBNull.Value));
			list.Add(new SqlParameter("@LimitData", DBNull.Value));
			list.Add(new SqlParameter("@Skip", DBNull.Value));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable LoadEventByOpportunity(ApzObjectTypes objType, int oprId, string bpCode)
	{
		try
		{
			string query = "APZ_SubActivities_LoadEventByOpportunity";
			return ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@objType", (int)objType), new SqlParameter("@oprId", oprId), new SqlParameter("@bpCode", bpCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable LoadEventByInterval(DataTable dtSearch)
	{
		try
		{
			if (dtSearch.IsNotNull())
			{
				string value = Function.ToString(dtSearch.Rows[0]["userCode"]);
				DateTime dateTime = Function.ParseDateTimes(dtSearch.Rows[0]["startDate"]);
				DateTime dateTime2 = Function.ParseDateTimes(dtSearch.Rows[0]["endDate"]);
				int num = 0;
				int num2 = 0;
				if (dtSearch.ExistsColumn("skip"))
				{
					num = Function.ParseInt(dtSearch.Rows[0]["skip"]);
				}
				if (dtSearch.ExistsColumn("limit"))
				{
					num2 = Function.ParseInt(dtSearch.Rows[0]["limit"]);
				}
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@UserName", value));
				list.Add(new SqlParameter("@ObjType", Function.ToString(1470000129)));
				list.Add(new SqlParameter("@clgCode", SqlDbType.BigInt));
				list.Add(string.IsNullOrEmpty(Function.ToString(dtSearch.Rows[0]["startDate"])) ? new SqlParameter("@StartDate", DBNull.Value) : new SqlParameter("@StartDate", dateTime));
				list.Add(string.IsNullOrEmpty(Function.ToString(dtSearch.Rows[0]["endDate"])) ? new SqlParameter("@EndDate", DBNull.Value) : new SqlParameter("@EndDate", dateTime2));
				list.Add(new SqlParameter("@ProjectCode", null));
				list.Add(new SqlParameter("@Skip", num));
				list.Add(new SqlParameter("@Limit", num2));
				return ExecuteDataTable("APZ_SubActivities_LoadEvent", CommandType.StoredProcedure, list.ToArray());
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public HttpResult AddEvent(DataSet ds)
	{
		processType = "AddEvent";
		return Process(ApzObjectTypes.oEventManagement, -1, Enums.TransactionType.A, ds);
	}

	public HttpResult SaveEvent(DataSet ds, int type)
	{
		processType = "SaveEvent";
		_saveEventType = type;
		return Process(ApzObjectTypes.oEventManagement, -1, Enums.TransactionType.U, ds);
	}

	public string RemoveEvent(DataTable dt, int type)
	{
		DataSet dataSet = new DataSet();
		dataSet.Tables.Add(dt);
		_removeEventType = type;
		processType = "RemoveEvent";
		return Process(ApzObjectTypes.oEventManagement, -1, Enums.TransactionType.D, dataSet).Message;
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet document, ref string objId, ref string objNum)
	{
		objId = "";
		objNum = "";
		string text = "";
		object obj = null;
		try
		{
			if (document.Tables.Count > 0)
			{
				switch (processType)
				{
				case "SaveTask":
				{
					DataTable tableStruct = GetTableStruct("APZ_OCLG");
					DataTable dataTable2 = document.Tables[0];
					if (!tableStruct.IsNotNull())
					{
						throw new Exception("Could not get data structure");
					}
					IEnumerable<string> source = from DataColumn t in tableStruct.Columns
						select t.ColumnName;
					dataTable2 = dataTable2.DefaultView.ToTable(distinct: false, source.ToArray());
					CreateTempTable(dataTable2, TempTableTab);
					BulkCopy(dataTable2, TempTableTab);
					if (document.Tables.Contains("Attachment"))
					{
						CreateTempTable(document.Tables["Attachment"], TempAtt);
						BulkCopy(document.Tables["Attachment"], TempAtt);
					}
					text = "APZ_SubActivities_SaveTask";
					obj = ExecuteScalar(text, CommandType.StoredProcedure);
					break;
				}
				case "AddTask":
				{
					DataTable tableStruct3 = GetTableStruct("APZ_OCLG");
					DataTable dataTable4 = document.Tables[0];
					if (!tableStruct3.IsNotNull())
					{
						throw new Exception("Could not get data structure");
					}
					IEnumerable<string> source3 = from DataColumn t in tableStruct3.Columns
						select t.ColumnName;
					dataTable4 = dataTable4.DefaultView.ToTable(distinct: false, source3.ToArray());
					CreateTempTable(dataTable4, TempTableTab);
					BulkCopy(dataTable4, TempTableTab);
					if (document.Tables.Contains("Attachment"))
					{
						CreateTempTable(document.Tables["Attachment"], TempAtt);
						BulkCopy(document.Tables["Attachment"], TempAtt);
					}
					if (!document.Tables[0].IsNotNull())
					{
						return new HttpResult();
					}
					if (string.IsNullOrEmpty(Function.ToString(document.Tables[0].Rows[0]["Details"])))
					{
						return new HttpResult();
					}
					text = "APZ_SubActivities_AddTask";
					obj = ExecuteScalar(text, CommandType.StoredProcedure);
					break;
				}
				case "AddMuntiTask":
				{
					DataTable tableStruct5 = GetTableStruct("APZ_OCLG");
					DataTable dataTable6 = document.Tables[0];
					if (!tableStruct5.IsNotNull())
					{
						throw new Exception("Could not get data structure");
					}
					IEnumerable<string> source5 = from DataColumn t in tableStruct5.Columns
						select t.ColumnName;
					dataTable6 = dataTable6.DefaultView.ToTable(distinct: false, source5.ToArray());
					CreateTempTable(dataTable6, TempTableTab);
					BulkCopy(dataTable6, TempTableTab);
					if (!document.Tables[0].IsNotNull())
					{
						return new HttpResult();
					}
					if (string.IsNullOrEmpty(Function.ToString(document.Tables[0].Rows[0]["Details"])))
					{
						return new HttpResult();
					}
					text = "APZ_SubActivities_GenTaskProject";
					obj = ExecuteScalar(text, CommandType.StoredProcedure);
					break;
				}
				case "ChangeTaskStatus":
					CreateTempTable(document.Tables[0], TempTableTab);
					BulkCopy(document.Tables[0], TempTableTab);
					text = "APZ_SubActivities_ChangeTaskStatus";
					obj = ExecuteScalar(text, CommandType.StoredProcedure);
					break;
				case "RemoveTask":
					text = "APZ_SubActivities_RemoveTask";
					obj = ExecuteScalar(text, CommandType.StoredProcedure, new SqlParameter("@ClgCode", Function.ParseInt(document.Tables[0].Rows[0]["ClgCode"])));
					if (document.Tables[0].IsNotNull())
					{
						int num7 = Function.ParseInt(document.Tables[0].Rows[0]["ClgCode"]);
						Logging.Write("TRACE", "Delete Task ID " + num7);
					}
					break;
				case "SaveSubTaskDetails":
					CreateTempTable(document.Tables[0], TempTableTab);
					BulkCopy(document.Tables[0], TempTableTab);
					text = "APZ_SubActivities_SaveSubTaskDetails";
					obj = ExecuteScalar(text, CommandType.StoredProcedure);
					break;
				case "SaveSubTaskAssigner":
					CreateTempTable(document.Tables[0], TempTableTab);
					BulkCopy(document.Tables[0], TempTableTab);
					text = "APZ_SubActivities_SaveSubTaskAssigner";
					obj = ExecuteScalar(text, CommandType.StoredProcedure);
					break;
				case "SaveTaskPosition":
					CreateTempTable(document.Tables[0], TempTableTab);
					BulkCopy(document.Tables[0], TempTableTab);
					text = "APZ_SubActivities_SaveTaskPosition";
					obj = ExecuteScalar(text, CommandType.StoredProcedure);
					break;
				case "SaveTaskManagementSetting":
					CreateTempTable(document.Tables[0], "#dtAPZ_CLG2");
					BulkCopy(document.Tables[0], "#dtAPZ_CLG2");
					text = "APZ_SubActivities_SaveTaskManagementSetting";
					obj = ExecuteScalar(text, CommandType.StoredProcedure);
					break;
				case "SaveNote":
				{
					DataTable dataTable7 = document.Tables[0];
					DataTable tableStruct6 = GetTableStruct("APZ_OCLG");
					if (!tableStruct6.IsNotNull())
					{
						throw new Exception("Cannot get table struct!");
					}
					tableStruct6.Rows.Clear();
					for (int num8 = 0; num8 < dataTable7.Rows.Count; num8++)
					{
						tableStruct6.ImportRow(dataTable7.Rows[num8]);
					}
					CreateTempTable(tableStruct6, TempNote);
					BulkCopy(tableStruct6, TempNote);
					if (document.Tables.Contains("Attachment"))
					{
						CreateTempTable(document.Tables["Attachment"], TempAtt);
						BulkCopy(document.Tables["Attachment"], TempAtt);
					}
					text = "APZ_SubActivities_SaveNote";
					obj = ExecuteScalar(text, CommandType.StoredProcedure);
					break;
				}
				case "DeleteNote":
					CreateTempTable(document.Tables[0], TempNote);
					BulkCopy(document.Tables[0], TempNote);
					text = "APZ_SubActivities_DeleteNote";
					obj = ExecuteScalar(text, CommandType.StoredProcedure);
					if (document.Tables[0].IsNotNull())
					{
						int num5 = Function.ParseInt(document.Tables[0].Rows[0]["ClgCode"]);
						Logging.Write("TRACE", "Delete Note ID " + num5);
					}
					break;
				case "SaveEvent":
				{
					DataTable dataTable5 = document.Tables[0];
					if (document.Tables[0].IsNotNull())
					{
						if (dataTable5.ExistsColumn("ResName"))
						{
							dataTable5.Columns.Remove("ResName");
						}
						if (dataTable5.ExistsColumn("ResourceId"))
						{
							dataTable5.Columns.Remove("ResourceId");
						}
						if (dataTable5.ExistsColumn("startDateTime"))
						{
							dataTable5.Columns.Remove("startDateTime");
						}
						if (dataTable5.ExistsColumn("endDateTime"))
						{
							dataTable5.Columns.Remove("endDateTime");
						}
						if (dataTable5.ExistsColumn("LocationCode"))
						{
							dataTable5.Columns.Remove("LocationCode");
						}
						if (dataTable5.ExistsColumn("ProjectCode"))
						{
							dataTable5.Columns.Remove("ProjectCode");
						}
					}
					DataTable tableStruct4 = GetTableStruct("APZ_OCLG");
					if (!tableStruct4.IsNotNull())
					{
						throw new Exception("Could not get data structure");
					}
					IEnumerable<string> source4 = from DataColumn t in tableStruct4.Columns
						select t.ColumnName;
					dataTable5 = dataTable5.DefaultView.ToTable(distinct: false, source4.ToArray());
					CreateTempTable(dataTable5, TempEvent);
					BulkCopy(dataTable5, TempEvent);
					string value2 = null;
					string value3 = null;
					string value4 = null;
					if (document.Tables.Count >= 2)
					{
						if (document.Tables[1].IsNotNull())
						{
							value2 = (string.IsNullOrEmpty(Function.ToString(document.Tables[1].Rows[0]["DateChanged"])) ? "N" : Function.ToString(document.Tables[1].Rows[0]["DateChanged"]));
							value3 = (string.IsNullOrEmpty(Function.ToString(document.Tables[1].Rows[0]["TimeChanged"])) ? "N" : Function.ToString(document.Tables[1].Rows[0]["TimeChanged"]));
							value4 = Function.ToString(document.Tables[1].Rows[0]["NewRecurrenceInfo"]);
						}
						if (document.Tables.Contains("Attachment"))
						{
							CreateTempTable(document.Tables["Attachment"], TempAtt);
							BulkCopy(document.Tables["Attachment"], TempAtt);
						}
					}
					text = "APZ_SubActivities_SaveEvent";
					List<SqlParameter> list2 = new List<SqlParameter>();
					SqlParameter item = new SqlParameter("@type", _saveEventType);
					SqlParameter item2 = new SqlParameter("@checkDateChange", value2);
					SqlParameter item3 = new SqlParameter("@checkTimeChange", value3);
					SqlParameter item4 = new SqlParameter("@newOriRecurrenceInfo", value4);
					list2.Add(item);
					list2.Add(item2);
					list2.Add(item3);
					list2.Add(item4);
					obj = ExecuteScalar(text, CommandType.StoredProcedure, list2.ToArray());
					break;
				}
				case "AddEvent":
				{
					DataTable dataTable3 = document.Tables[0];
					if (document.Tables[0].IsNotNull())
					{
						if (dataTable3.ExistsColumn("ResName"))
						{
							dataTable3.Columns.Remove("ResName");
						}
						if (dataTable3.ExistsColumn("ResourceId"))
						{
							dataTable3.Columns.Remove("ResourceId");
						}
						if (dataTable3.ExistsColumn("startDateTime"))
						{
							dataTable3.Columns.Remove("startDateTime");
						}
						if (dataTable3.ExistsColumn("endDateTime"))
						{
							dataTable3.Columns.Remove("endDateTime");
						}
						if (dataTable3.ExistsColumn("LocationCode"))
						{
							dataTable3.Columns.Remove("LocationCode");
						}
						if (dataTable3.ExistsColumn("ProjectCode"))
						{
							dataTable3.Columns.Remove("ProjectCode");
						}
					}
					DataTable tableStruct2 = GetTableStruct("APZ_OCLG");
					if (!tableStruct2.IsNotNull())
					{
						throw new Exception("Could not get data structure");
					}
					IEnumerable<string> source2 = from DataColumn t in tableStruct2.Columns
						select t.ColumnName;
					dataTable3 = dataTable3.DefaultView.ToTable(distinct: false, source2.ToArray());
					CreateTempTable(dataTable3, TempEvent);
					BulkCopy(dataTable3, TempEvent);
					if (document.Tables.Contains("Attachment"))
					{
						CreateTempTable(document.Tables["Attachment"], TempAtt);
						BulkCopy(document.Tables["Attachment"], TempAtt);
					}
					text = "APZ_SubActivities_SaveEvent";
					SqlParameter sqlParameter = new SqlParameter("@type", Function.ParseInt(0));
					obj = ExecuteScalar(text, CommandType.StoredProcedure, sqlParameter);
					break;
				}
				case "RemoveEvent":
				{
					CreateTempTable(document.Tables[0], TempEvent);
					BulkCopy(document.Tables[0], TempEvent);
					text = "APZ_SubActivities_RemoveEvent";
					SqlParameter sqlParameter2 = new SqlParameter("@type", _removeEventType);
					obj = ExecuteScalar(text, CommandType.StoredProcedure, sqlParameter2);
					if (document.Tables[0].IsNotNull())
					{
						int num6 = Function.ParseInt(document.Tables[0].Rows[0]["ClgCode"]);
						Logging.Write("TRACE", "Delete Event ID " + num6);
					}
					break;
				}
				case "SaveComment":
				{
					CreateTempTable(document.Tables[0], TemComment);
					BulkCopy(document.Tables[0], TemComment);
					text = "APZ_SubActivities_SaveComment";
					List<SqlParameter> list4 = new List<SqlParameter>();
					if (additionNotifyUser.Count > 0)
					{
						string value5 = string.Join(";", additionNotifyUser);
						list4.Add(new SqlParameter("@AdditionNotifyUser", value5));
					}
					obj = ExecuteScalar(text, CommandType.StoredProcedure, list4.ToArray());
					break;
				}
				case "ClearUnreadComment":
					CreateTempTable(document.Tables[0], TempUnreadComment);
					BulkCopy(document.Tables[0], TempUnreadComment);
					text = "exec ('update a set \"Unread\" = b.\"Unread\" from APZ_CLG3 a inner join \"#dtAPZ_CLG3\" b ON a.\"CmtCode\" = b.\"CmtCode\" AND a.\"UserSign\" = b.\"UserSign\";');";
					obj = ExecuteScalar(text, CommandType.Text);
					break;
				case "MarkAsRead":
				{
					text = " UPDATE APZ_CLG3 SET \"Unread\"='N' WHERE \"CmtCode\"=@CmtCode;";
					List<SqlParameter> list3 = new List<SqlParameter>();
					if (commentCode > 0)
					{
						list3.Add(new SqlParameter("@CmtCode", commentCode));
					}
					obj = ExecuteScalar(text, CommandType.Text, list3.ToArray());
					break;
				}
				case "CalendarSaveInterval":
				{
					DataTable dataTable = document.Tables[0];
					if (dataTable.IsNotNull())
					{
						DateTime value = Function.ParseDateTimes(dataTable.Rows[0]["newStart"]);
						DateTime dateTime = Function.ParseDateTimes(dataTable.Rows[0]["newEnd"]);
						int num = Function.ParseInt(dataTable.Rows[0]["startTime"]);
						int num2 = Function.ParseInt(dataTable.Rows[0]["endTime"]);
						int num3 = Function.ParseInt(dataTable.Rows[0]["ClgCode"]);
						string text2 = Function.ToString(dataTable.Rows[0]["GroupUser"]);
						string text3 = Function.ToString(dataTable.Rows[0]["GroupEmployee"]);
						string text4 = Function.ToString(dataTable.Rows[0]["resourceIdsInfo"]);
						string text5;
						if (dateTime.Subtract(value).TotalMinutes < 60.0)
						{
							text5 = "M";
						}
						else
						{
							text5 = ((!(dateTime.Subtract(value).TotalMinutes >= 1440.0)) ? "H" : "D");
						}
						decimal num4;
						if (text5 == Function.ToString(BoDurations.du_Minuts))
						{
							num4 = Function.ParseDecimal(dateTime.Subtract(value).TotalMinutes);
						}
						else
						{
							num4 = ((!(text5 == Function.ToString(BoDurations.du_Days))) ? Function.ParseDecimal(dateTime.Subtract(value).TotalHours) : Function.ParseDecimal(dateTime.Subtract(value).TotalDays));
						}
						List<SqlParameter> list = new List<SqlParameter>();
						list.Add(new SqlParameter("@clgCode", num3));
						list.Add(new SqlParameter("@durType", text5));
						list.Add(new SqlParameter("@duration", num4));
						list.Add(new SqlParameter("@recontact", value.Date.ToString("yyyyMMdd")));
						list.Add(new SqlParameter("@startTime", Function.ToString(num)));
						list.Add(new SqlParameter("@endDate", dateTime.Date.ToString("yyyyMMdd")));
						list.Add(new SqlParameter("@endTime", Function.ToString(num2)));
						list.Add(new SqlParameter("@groupUser", Function.ToString(text2)));
						list.Add(new SqlParameter("@groupEmp", Function.ToString(text3)));
						list.Add(new SqlParameter("@resourceIdsInfo", Function.ToString(text4)));
						obj = ExecuteScalar("APZ_SubActivities_SaveInterval", CommandType.StoredProcedure, list.ToArray());
						break;
					}
					return new HttpResult(Enums.MessageCode.Error, "No data to save.");
				}
				}
				string[] array = Function.ToString(obj).Split(',');
				if (array.Length > 1)
				{
					objId = Function.ToString(array[0]);
					objNum = Function.ToString(obj);
				}
				else
				{
					objId = Function.ToString(obj);
					objNum = objId;
				}
				return new HttpResult();
			}
			return new HttpResult(Enums.MessageCode.Error, "No data to save.");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataSet LoadComment(int Id, string type, int userSign)
	{
		try
		{
			string query = "APZ_SubActivities_LoadComment";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Id", Id));
			list.Add(new SqlParameter("@ObjType", type));
			list.Add(new SqlParameter("@UserSign", userSign));
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public string DeleteComment(int id)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				SqlParameter sqlParameter = new SqlParameter("@Id", id);
				if (ExecuteNonQuery("DELETE FROM APZ_CLG1 WHERE \"CmtCode\" = @Id; DELETE FROM APZ_CLG3 WHERE \"CmtCode\" = @Id;", CommandType.Text, sqlParameter) > 0)
				{
					dbTransaction.Commit();
					return "";
				}
				dbTransaction.Rollback();
				return "Failed to delete";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Delete FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public string RemoveCommentById(int Id, string type)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				List<SqlParameter> list = new List<SqlParameter>
				{
					new SqlParameter("@Id", Id),
					new SqlParameter("@ObjType", type)
				};
				string text = Function.ToString(ExecuteScalar("APZ_SubActivities_RemoveComment", CommandType.StoredProcedure, list.ToArray()));
				if (text == "")
				{
					dbTransaction.Commit();
					return "";
				}
				dbTransaction.Rollback();
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Delete FAILED ", text);
				return "Failed to remove comment";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Delete FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public HttpResult MarkAsRead(int comment)
	{
		processType = "MarkAsRead";
		DataSet dataSet = new DataSet();
		dataSet.Tables.Add(new DataTable());
		commentCode = comment;
		return Process(ApzObjectTypes.oComment, -1, Enums.TransactionType.A, dataSet);
	}

	public HttpResult SaveComment(Tuple<DataTable, List<int>> dt)
	{
		processType = "SaveComment";
		DataSet dataSet = new DataSet();
		dataSet.Tables.Add(dt.Item1);
		additionNotifyUser = dt.Item2;
		return Process(ApzObjectTypes.oComment, -1, Enums.TransactionType.A, dataSet);
	}

	public string ClearUnreadComment(DataTable dt)
	{
		processType = "ClearUnreadComment";
		DataSet dataSet = new DataSet();
		dataSet.Tables.Add(dt);
		return Process(ApzObjectTypes.oComment, -1, Enums.TransactionType.A, dataSet).Message;
	}

	public DataTable GetUnlockedUsers()
	{
		try
		{
			return ExecuteDataTable("SELECT USERID,U_NAME,\"E_Mail\" from OUSR where \"Locked\" = 'N' and GROUPS != 99;", CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetRequesterByUser", ex.Message.ToString());
			return new DataTable();
		}
	}

	public virtual DataTable GetActiveEmployees()
	{
		try
		{
			return ExecuteDataTable("SELECT isnull(lastName,'') + \r\n\t\t\t\t\t\t\t\tCase When Isnull(b.middleName,'') != '' Then ' ' +  Isnull(b.middleName,'') Else '' End +\r\n\t\t\t\t\t\t\t\tCase When Isnull(b.firstName,'') != '' Then ', ' +  Isnull(b.firstName,'') Else '' End as empName ,isnull( empID,0) as empID,email,userId  \r\n\t\t\t\t\t\t\t\t\tFROM OHEM where Active = 'Y' ", CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetRequesterByEmployee", ex.Message.ToString());
			return new DataTable();
		}
	}

	public DataTable GetAllNotification(int userId)
	{
		try
		{
			return ExecuteDataTable("APZ_SubActivities_GetAllNotification", CommandType.StoredProcedure, new SqlParameter("@userId", userId));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDataById(int id)
	{
		try
		{
			return ExecuteDataTable("select * from APZ_OCLG where \"ClgCode\" = @clgCode;", CommandType.Text, new SqlParameter("@clgCode", id));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public virtual string GetUserComment(string objType, int id)
	{
		try
		{
			return Function.ToString(ExecuteScalar("select USER_CODE + ';' from OUSR a INNER JOIN (select distinct UserSign from APZ_CLG1 where ObjType = @objType and ID = @id) b ON a.USERID = b.UserSign for xml path('')", CommandType.Text, new SqlParameter("@id", id), new SqlParameter("@objType", objType)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}

	public DataTable GetStruct()
	{
		try
		{
			return ExecuteDataTable("SELECT TOP 0 * FROM \"APZ_OCLG\";", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetEmployeeByUserId(int userId)
	{
		try
		{
			return ExecuteDataTable("SELECT \"empID\" FROM OHEM Where \"userId\" = @_userId;", CommandType.Text, new SqlParameter("@_userId", userId));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public string CheckExists(int oclg_code)
	{
		try
		{
			string query = "select \"ClgCode\" from apz_oclg where \"ClgCode\" = " + oclg_code;
			return Function.ToString(ExecuteScalar(query, CommandType.Text));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}
}
