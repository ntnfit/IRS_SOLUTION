using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaApprovalTemplatesRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService) : ApprovalTemplatesRepository(serviceUnitOfWork, unitOfWork, dataService)
{
	private ApzObjectTypes _objType = ApzObjectTypes.oApprovalTemplates;

	public override string Mapping(DataSet ds, EnumCommandType commandType, string sessionId)
	{
		try
		{
			DataRow dataRow = ds.Tables[0].Rows[0];
			DataTable dataTable = ds.Tables[1];
			DataTable dataTable2 = ds.Tables[2];
			DataTable dataTable3 = ds.Tables[3];
			DataTable dataTable4 = ds.Tables[4];
			DataTable dataTable5 = ds.Tables[5];
			string text = "{";
			text += Function.GetJsonString("Code", Function.ToString(dataRow["WtmCode"]));
			text += Function.GetJsonString("Name", Function.ToString(dataRow["Name"]));
			text += Function.GetJsonString("Remarks", Function.ToString(dataRow["Remarks"]));
			text = ((!Function.ToString(dataRow["Conds"]).Equals("Y")) ? (text + Function.GetJsonString("UseTerms", "tNO")) : (text + Function.GetJsonString("UseTerms", "tYES")));
			text = ((!Function.ToString(dataRow["Active"]).Equals("Y")) ? (text + Function.GetJsonString("IsActive", "tNO")) : (text + Function.GetJsonString("IsActive", "tYES")));
			text = ((!Function.ToString(dataRow["AppOnUpd"]).Equals("Y")) ? (text + Function.GetJsonString("IsActiveWhenUpdatingDocuments", "tNO")) : (text + Function.GetJsonString("IsActiveWhenUpdatingDocuments", "tYES")));
			text += "\"ApprovalTemplateUsers\":[";
			for (int i = 0; i < dataTable.Rows.Count - 1; i++)
			{
				text += "{";
				text += Function.GetJsonString("UserID", Function.ParseInt(dataTable.Rows[i]["UserID"]));
				text += "},";
			}
			text += "],";
			text += "\"ApprovalTemplateStages\":[";
			for (int j = 0; j < dataTable3.Rows.Count - 1; j++)
			{
				text += "{";
				text += Function.GetJsonString("SortID", Function.ParseInt(dataTable3.Rows[j]["SortId"]));
				text += Function.GetJsonString("ApprovalStageCode", Function.ParseInt(dataTable3.Rows[j]["WstCode"]));
				text += Function.GetJsonString("Remarks", Function.ToString(dataTable3.Rows[j]["WstRemarks"]));
				text += "},";
			}
			text += "],";
			text += "\"ApprovalTemplateDocuments\":[";
			for (int k = 0; k < dataTable2.Rows.Count; k++)
			{
				string value = string.Empty;
				switch (Function.ParseInt(dataTable2.Rows[k]["TransType"]))
				{
				case 23:
					value = "atdtQuotation";
					break;
				case 17:
					value = "atdtOrder";
					break;
				case 15:
					value = "atdtDelivery";
					break;
				case 16:
					value = "atdtReturns";
					break;
				case 203:
					value = "atdtArDownPayment";
					break;
				case 13:
					value = "atdtArInvoice";
					break;
				case 14:
					value = "atdtArCreditMemo";
					break;
				case 132:
					value = "atdtCorrectionInvoice";
					break;
				case 22:
					value = "atdtPurchaseOrder";
					break;
				case 20:
					value = "atdtGoodsReceiptPO";
					break;
				case 21:
					value = "atdtGoodsReturns";
					break;
				case 204:
					value = "atdtApDownPayment";
					break;
				case 18:
					value = "atdtApInvoice";
					break;
				case 19:
					value = "atdtApCreditMemo";
					break;
				case 59:
					value = "atdtGoodsReceipt";
					break;
				case 60:
					value = "atdtGoodsIssue";
					break;
				case 67:
					value = "atdtInventoryTransfer";
					break;
				case 540000006:
					value = "atdtPurchaseQuotation";
					break;
				case 1250000001:
					value = "atdtInventoryTransferRequest";
					break;
				case 46:
					value = "atdtOutgoingPayment";
					break;
				case 1470000065:
					value = "atdtInventoryCounting";
					break;
				case 10000071:
					value = "atdtInventoryPosting";
					break;
				case 310000001:
					value = "atdtInventoryOpeningBalance";
					break;
				}
				if (!string.IsNullOrEmpty(value))
				{
					text += "{";
					text += Function.GetJsonString("DocumentType", value);
					text += "},";
				}
			}
			text += "],";
			text += "\"ApprovalTemplateTerms\":[";
			for (int l = 0; l < dataTable4.Rows.Count; l++)
			{
				if (!Function.ToString(dataTable4.Rows[l]["Checked"]).Equals("N"))
				{
					text += "{";
					text += Function.GetJsonString("ConditionType", Function.ParseInt(dataTable4.Rows[l]["CondId"]));
					text += Function.GetJsonString("OperationType", Function.ParseInt(dataTable4.Rows[l]["opCode"]));
					text += Function.GetJsonString("Value", Function.ToString(dataTable4.Rows[l]["opValue"]));
					text += "},";
				}
			}
			text += "],";
			text += "\"ApprovalTemplateQueries\":[";
			for (int m = 0; m < dataTable5.Rows.Count - 1; m++)
			{
				if (!string.IsNullOrEmpty(Function.ToString(dataTable5.Rows[m]["QueryName"])))
				{
					text += "{";
					text += Function.GetJsonString("QueryID", Function.ParseInt(dataTable5.Rows[m]["QueryId"]));
					text += "},";
				}
			}
			text += "],";
			return text + "}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return string.Empty;
		}
	}

	public override HttpResult Save(Tuple<DataSet, string> tuple, string sessionId)
	{
		try
		{
			EnumCommandType enumCommandType = (tuple.Item2.Equals("A") ? EnumCommandType.Add : EnumCommandType.Update);
			string bodyObject = Mapping(tuple.Item1, enumCommandType, sessionId);
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			if (enumCommandType == EnumCommandType.Add)
			{
				list.Add(new BodyRequestObject
				{
					Action = Enums.HttpAction.AddNew,
					BodyObject = bodyObject,
					ObjectType = _objType,
					OutObjectKey = true
				});
			}
			else
			{
				string obj = Function.ToString(tuple.Item1.Tables[0].Rows[0]["WtmCode"]);
				list.Add(new BodyRequestObject
				{
					Action = Enums.HttpAction.Update,
					ObjectType = _objType,
					OutObjectKey = true,
					BodyObject = bodyObject,
					Key = Function.ParseInt(obj)
				});
			}
			return DataService.ProcessMutiTransaction(sessionId, list);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult Delete(int templateCode, string sessionId)
	{
		try
		{
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			list.Add(new BodyRequestObject
			{
				Action = Enums.HttpAction.Delete,
				ObjectType = _objType,
				Key = templateCode,
				OutObjectKey = true
			});
			return DataService.ProcessMutiTransaction(sessionId, list);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
