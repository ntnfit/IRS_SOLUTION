using System;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Api.Hubs;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.PM;

public class TrackingIssueController : BaseApiController
{
	[HttpGet]
	public DataTable Search(string bpCode, string projectCode, int projectOwner, string reportBy, string handleBy, int id, string status, string fromDate, string toDate, string userName = "")
	{
		return UnitOfWork.TrackingIssue.Search(bpCode, projectCode, projectOwner, reportBy, handleBy, id, status, fromDate, toDate, userName);
	}

	[HttpGet]
	public DataSet GetDataById(int Id)
	{
		return UnitOfWork.TrackingIssue.GetDataById(Id);
	}

	[HttpPost]
	public HttpResult Add([FromBody] DataSet ds)
	{
		HttpResult httpResult = UnitOfWork.TrackingIssue.Save(ds, "A");
		if (string.IsNullOrEmpty(httpResult.Message))
		{
			NotifiedObj notifiedFromDataRow = GetNotifiedFromDataRow(ds.Tables[0].Rows[0]);
			notifiedFromDataRow.NotifiedId = Function.ToString(httpResult.Content);
			notifiedFromDataRow.TranType = Enums.TransactionType.A;
			new NotificationClient().SendNotifiedToClient(notifiedFromDataRow);
		}
		return httpResult;
	}

	[HttpPost]
	public HttpResult Update([FromBody] DataSet ds)
	{
		HttpResult httpResult = UnitOfWork.TrackingIssue.Save(ds, "U");
		if (string.IsNullOrEmpty(httpResult.Message))
		{
			NotifiedObj notifiedFromDataRow = GetNotifiedFromDataRow(ds.Tables[0].Rows[0]);
			notifiedFromDataRow.TranType = Enums.TransactionType.A;
			new NotificationClient().SendNotifiedToClient(notifiedFromDataRow);
		}
		return httpResult;
	}

	[HttpPost]
	public string DeleteDocs([FromBody] string Id)
	{
		return UnitOfWork.TrackingIssue.DeleteDocs(Id);
	}

	[HttpPost]
	public HttpResult SaveSearch([FromBody] Tuple<DataSet, string> model)
	{
		return UnitOfWork.TrackingIssue.SaveSearch(model.Item1, model.Item2);
	}

	[HttpGet]
	public DataTable GetDataTypeById(string Id)
	{
		return UnitOfWork.TrackingIssue.GetDataTypeById(Id);
	}

	[HttpGet]
	public DataTable GetDataSearchByUserId(int userId, string formId)
	{
		return UnitOfWork.TrackingIssue.GetDataSearchByUserId(userId, formId);
	}

	[HttpGet]
	public DataTable LoadIssueByProjectId(int projectId)
	{
		return UnitOfWork.TrackingIssue.LoadIssueByProjectId(projectId);
	}

	[HttpGet]
	public DataTable LoadIssueByCallId(int callId)
	{
		return UnitOfWork.TrackingIssue.LoadIssueByCallId(callId);
	}

	[HttpGet]
	public DataTable GetEmployeeByUserId(int userId)
	{
		return UnitOfWork.TrackingIssue.GetEmployeeByUserId(userId);
	}

	private NotifiedObj GetNotifiedFromDataRow(DataRow dt)
	{
		try
		{
			NotifiedObj notifiedObj = new NotifiedObj
			{
				CreateUserCode = UnitOfWork.EmpMasterData.GetListUserOfEmp(Function.ToString(dt["Reporter"])),
				AssignUser = UnitOfWork.EmpMasterData.GetListUserOfEmp(Function.ToString(dt["HandleBy"])).Replace(";", ""),
				DueDate = ((!string.IsNullOrEmpty(Function.ToString(dt["ResponseDate"]))) ? Function.ParseDateTimeMinToMaxValue(dt["ResponseDate"], 0) : Function.ParseDateTimeMinToMaxValue(dt["IssueDate"], 0)),
				NotifiedId = Function.ToString(dt["Id"]),
				Location = "",
				Detail = Function.ToString(dt["Note"]),
				ObjectTypes = ApzObjectTypes.oTrackingIssue
			};
			string text = UnitOfWork.Users.GetLstUserNameByCode(notifiedObj.CreateUserCode);
			if (!string.IsNullOrEmpty(text))
			{
				text = text.Split(';')[0];
			}
			notifiedObj.CreateUserName = text;
			notifiedObj.NotifiedUser = string.Format("{0};{1}", UnitOfWork.Users.GetLstUserNameByCode(notifiedObj.AssignUser), UnitOfWork.TrackingIssue.GetUserHistoryAssign(Function.ParseInt(dt["Id"])));
			return notifiedObj;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new NotifiedObj();
	}

	[HttpPost]
	public HttpResult UpdateStatus([FromBody] DataSet ds)
	{
		try
		{
			HttpResult httpResult = UnitOfWork.TrackingIssue.UpdateStatus(ds);
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				DataSet dataById = UnitOfWork.TrackingIssue.GetDataById(Function.ParseInt(ds.Tables[0].Rows[0]["Id"]));
				NotifiedObj notifiedFromDataRow = GetNotifiedFromDataRow(dataById.Tables[0].Rows[0]);
				notifiedFromDataRow.TranType = ((Function.ParseInt(dataById.Tables[0].Rows[0]["Status"]) >= 6) ? Enums.TransactionType.D : Enums.TransactionType.A);
				new NotificationClient().SendNotifiedToClient(notifiedFromDataRow);
			}
			return httpResult;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	[HttpGet]
	public DataSet GetStructure()
	{
		return UnitOfWork.TrackingIssue.GetStructure();
	}

	[HttpGet]
	public DataTable GetDataStatusById(string Id)
	{
		return UnitOfWork.TrackingIssue.GetDataStatusById(Id);
	}

	[HttpGet]
	public DataSet GetStructureDMS()
	{
		try
		{
			DataSet structure = UnitOfWork.TrackingIssue.GetStructure();
			DataTable structureAtc = UnitOfWork.TrackingIssue.GetStructureAtc1();
			structure.Tables.Add(structureAtc);
			if (structure != null)
			{
				DataTable dataTable = structure.Tables[0];
				DataRow dataRow = dataTable.NewRow();
				dataRow["Status"] = 2;
				dataRow["IssueDate"] = DateTime.Now;
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				DataTable dataTable2 = structure.Tables[1];
				DataRow dataRow2 = dataTable2.NewRow();
				dataRow2["ActionDetail"] = "Created issue";
				dataRow2["TrackingDate"] = DateTime.Now;
				dataTable2.Rows.Add(dataRow2);
				dataTable2.AcceptChanges();
			}
			structure.AcceptChanges();
			return structure;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	[HttpGet]
	public DataTable GetStructureAtc1()
	{
		return UnitOfWork.TrackingIssue.GetStructureAtc1();
	}

	[HttpGet]
	public DataTable GetAllStatus()
	{
		return UnitOfWork.TrackingIssue.GetAllStatus();
	}

	[HttpGet]
	public DataTable GetFileByOjectType(string objectType, string objectCode = "")
	{
		try
		{
			return UnitOfWork.TrackingIssue.GetFileByOjectType(objectType, objectCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpGet]
	public DataTable GetAllUser()
	{
		return UnitOfWork.TrackingIssue.GetAllUser();
	}
}
