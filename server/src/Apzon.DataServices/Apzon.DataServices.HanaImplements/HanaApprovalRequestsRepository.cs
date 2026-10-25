using System;
using System.Data;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Services;
using Newtonsoft.Json;
using SAPbobsCOM;

namespace Apzon.DataServices.HanaImplements;

public class HanaApprovalRequestsRepository : ApprovalRequestsRepository
{
	public HanaApprovalRequestsRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	public HanaApprovalRequestsRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork dataServiceWork, IDataService dataService)
		: base(serviceUnitOfWork, dataServiceWork, dataService)
	{
	}

	protected override void ProcessApprovalDraft(string sessionId, int wddCode, string userName, string password, string decision, string remarks)
	{
		string text = "{\r\n                                            \"Code\":" + wddCode + ",\r\n                                            \"ApprovalRequestDecisions\": [\r\n                                            {";
		text += Function.GetJsonString("ApproverUserName", userName);
		text += Function.GetJsonString("ApproverPassword", EncodePassword.DecryptString(password, RSA.PasswordPhrase));
		text += Function.GetJsonString("Status", decision);
		text += Function.GetJsonString("Remarks", remarks);
		text += "}\r\n],\r\n                                    }";
		HttpResult httpResult = DataService.UpdateObject(ApzObjectTypes.oApprovalRequests, sessionId, text, wddCode);
		if (!string.IsNullOrEmpty(httpResult.Message))
		{
			throw new Exception(httpResult.Message);
		}
	}

	public override DataSet GetApprovalRequest(int wddCode, string sessionId)
	{
		HttpResult dataObjectFromService = DataService.GetDataObjectFromService(ApzObjectTypes.oApprovalRequests, sessionId, Enums.HttpAction.Get, wddCode);
		if (!string.IsNullOrEmpty(dataObjectFromService.Message))
		{
			Logging.Write("ERROR", JsonConvert.SerializeObject(dataObjectFromService));
			throw new Exception("Could not get approval request");
		}
		dynamic val = JsonConvert.DeserializeObject(Function.ToString(dataObjectFromService.Content));
		DataTable dataTable = new DataTable("ApprovalRequest");
		dataTable.Columns.Add("CurrentStage", typeof(int));
		DataRow dataRow = dataTable.NewRow();
		dataRow["CurrentStage"] = (object)Function.ParseInt(val.CurrentStage);
		dataTable.Rows.Add(dataRow);
		dataTable.AcceptChanges();
		DataTable dataTable2 = new DataTable("ApprovalRequestLine");
		dataTable2.Columns.Add("UserID", typeof(int));
		dataTable2.Columns.Add("Status", typeof(string));
		dataTable2.Columns.Add("StageCode", typeof(int));
		for (int i = 0; i < val.ApprovalRequestLines.Count; i++)
		{
			dynamic val2 = val.ApprovalRequestLines[i];
			DataRow dataRow2 = dataTable2.NewRow();
			dataRow2["UserID"] = (object)Function.ParseInt(val2.UserID);
			dataRow2["Status"] = (object)Function.ToString(val2.Status);
			dataRow2["StageCode"] = (object)Function.ParseInt(val2.StageCode);
			dataTable2.Rows.Add(dataRow2);
		}
		dataTable2.AcceptChanges();
		return new DataSet
		{
			Tables = { dataTable, dataTable2 }
		};
	}

	protected override string GetUpdateExUrlQuery()
	{
		return "insert into APZ_EXRL(\"DocEntry\",\"Token\",\"Status\") SELECT ifnull((SELECT Max(\"DocEntry\") FROM APZ_EXRL),0)+1,:Guid,'N' from DUMMY;";
	}

	public override string CancelApprovalRequest(int wddCode, string sessionId)
	{
		string text = "{\r\n";
		text += Function.GetJsonString("Code", wddCode);
		text += Function.GetJsonString("Status", Function.ToString(BoApprovalRequestStatusEnum.arsCancelled));
		text += "}";
		HttpResult httpResult = DataService.UpdateObject(ApzObjectTypes.oApprovalRequests, sessionId, text, wddCode);
		if (!string.IsNullOrEmpty(httpResult.Message))
		{
			return httpResult.Message;
		}
		return "";
	}
}
