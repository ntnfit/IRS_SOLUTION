using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.UserFields;
using Newtonsoft.Json;
using RestSharp;
using SAPbobsCOM;

namespace Apzon.DataServices;

public class HanaService : IDataService
{
	private readonly string _serviceAddress;

	private const string BatchHeader = "Batch_Header";

	private const string BatchBody = "Batch_Body";

	public static List<DiConnection> LstDiConnections { get; set; }

	public HanaService(string serviceAddress)
	{
		_serviceAddress = serviceAddress;
	}

	public HttpResult GetBusinessObjectTemplate(string sessionId, string objType)
	{
		return new HttpResult(Enums.MessageCode.Success);
	}

	public HttpResult GetServiceMetaData(string sessionId, string servicename)
	{
		return new HttpResult(Enums.MessageCode.Success);
	}

	public HttpResult BatchProcess(string xmlObject, int trytime = 0)
	{
		try
		{
			return new HttpResult(Enums.MessageCode.Success);
		}
		finally
		{
			GC.Collect();
		}
	}

	public HttpResult Process(string xmlObject, int trytime = 0)
	{
		return new HttpResult(Enums.MessageCode.Success);
	}

	public HttpResult ProcessJson(ApzObjectTypes objectType, string sessionId, string jsonObject, Enums.HttpAction action, object objectKey = null, string udoCode = "", bool isTryProcess = false, bool replaceOnPatch = true)
	{
		IRestResponse responseService = GetResponseService(objectType, sessionId, jsonObject, objectKey, action, udoCode, isServiceAction: false, replaceOnPatch);
		if (!string.IsNullOrEmpty(responseService.ErrorMessage))
		{
			return new HttpResult(Enums.MessageCode.Error, responseService.ErrorMessage);
		}
		if (responseService.StatusCode == HttpStatusCode.OK || responseService.StatusCode == HttpStatusCode.Accepted || responseService.StatusCode == HttpStatusCode.Created || responseService.StatusCode == HttpStatusCode.NoContent)
		{
			Parameter parameter = responseService.Headers.FirstOrDefault((Parameter t) => t.Name.Equals("Location"));
			if (parameter == null)
			{
				return new HttpResult(Enums.MessageCode.Success, "", objectKey);
			}
			string location = Function.ToString(parameter.Value);
			return new HttpResult(Enums.MessageCode.Success, "", GetIdObjectFromLocation(location));
		}
		LayerMessageObject layerMessageObject = JsonConvert.DeserializeObject<LayerMessageObject>(responseService.Content);
		if (!isTryProcess && layerMessageObject.error.code == 301)
		{
			ReConnect(sessionId);
			return ProcessJson(objectType, sessionId, jsonObject, action, objectKey, udoCode, isTryProcess: true, replaceOnPatch);
		}
		return new HttpResult(Enums.MessageCode.Error, $"{layerMessageObject.error.code}-{layerMessageObject.error.message.value}");
	}

	public HttpResult ProcessServiceJson(ApzObjectTypes objectType, string sessionId, string jsonObject, Enums.HttpAction action, object objectKey = null, string udoCode = "", bool isTryProcess = false)
	{
		IRestResponse responseService = GetResponseService(objectType, sessionId, jsonObject, objectKey, action, udoCode, isServiceAction: true);
		if (!string.IsNullOrEmpty(responseService.ErrorMessage))
		{
			return new HttpResult(Enums.MessageCode.Error, responseService.ErrorMessage);
		}
		if (responseService.StatusCode == HttpStatusCode.OK || responseService.StatusCode == HttpStatusCode.Accepted || responseService.StatusCode == HttpStatusCode.Created || responseService.StatusCode == HttpStatusCode.NoContent)
		{
			Parameter parameter = responseService.Headers.FirstOrDefault((Parameter t) => t.Name.Equals("Location"));
			if (parameter == null)
			{
				return new HttpResult(Enums.MessageCode.Success, "", objectKey);
			}
			string location = Function.ToString(parameter.Value);
			return new HttpResult(Enums.MessageCode.Success, "", GetIdObjectFromLocation(location));
		}
		LayerMessageObject layerMessageObject = JsonConvert.DeserializeObject<LayerMessageObject>(responseService.Content);
		if (!isTryProcess && layerMessageObject.error.code == 301)
		{
			ReConnect(sessionId);
			return ProcessJson(objectType, sessionId, jsonObject, action, objectKey, udoCode, isTryProcess: true);
		}
		return new HttpResult(Enums.MessageCode.Error, $"{layerMessageObject.error.code}-{layerMessageObject.error.message.value}");
	}

	public HttpResult AddObject(ApzObjectTypes objectTypes, string sessionId, string xmlObject, int trytime = 0, bool isTryProcess = false)
	{
		IRestResponse responseService = GetResponseService(objectTypes, sessionId, xmlObject, "", Enums.HttpAction.AddNew);
		if (!string.IsNullOrEmpty(responseService.ErrorMessage))
		{
			return new HttpResult(Enums.MessageCode.Error, responseService.ErrorMessage);
		}
		if (responseService.StatusCode == HttpStatusCode.OK || responseService.StatusCode == HttpStatusCode.Accepted || responseService.StatusCode == HttpStatusCode.Created || responseService.StatusCode == HttpStatusCode.NoContent)
		{
			Parameter parameter = responseService.Headers.FirstOrDefault((Parameter t) => t.Name.Equals("Location"));
			if (parameter == null)
			{
				return new HttpResult(Enums.MessageCode.Success);
			}
			string location = Function.ToString(parameter.Value);
			return new HttpResult(Enums.MessageCode.Success, "", GetIdObjectFromLocation(location));
		}
		LayerMessageObject layerMessageObject = JsonConvert.DeserializeObject<LayerMessageObject>(responseService.Content);
		if (!isTryProcess && layerMessageObject.error.code == 301)
		{
			ReConnect(sessionId);
			return AddObject(objectTypes, sessionId, xmlObject, trytime, isTryProcess: true);
		}
		return new HttpResult(Enums.MessageCode.Error, $"{layerMessageObject.error.code}-{layerMessageObject.error.message.value}");
	}

	public HttpResult UpdateObject(ApzObjectTypes objectTypes, string sessionId, string xmlObject, object objectKey = null, int trytime = 0, bool isTryProcess = false)
	{
		IRestResponse responseService = GetResponseService(objectTypes, sessionId, xmlObject, objectKey, Enums.HttpAction.Update);
		if (!string.IsNullOrEmpty(responseService.ErrorMessage))
		{
			return new HttpResult(Enums.MessageCode.Error, responseService.ErrorMessage);
		}
		if (responseService.StatusCode == HttpStatusCode.OK || responseService.StatusCode == HttpStatusCode.Accepted || responseService.StatusCode == HttpStatusCode.Created || responseService.StatusCode == HttpStatusCode.NoContent)
		{
			Parameter parameter = responseService.Headers.FirstOrDefault((Parameter t) => t.Name.Equals("Location"));
			if (parameter == null)
			{
				return new HttpResult(Enums.MessageCode.Success, "", objectKey);
			}
			string location = Function.ToString(parameter.Value);
			return new HttpResult(Enums.MessageCode.Success, "", GetIdObjectFromLocation(location));
		}
		LayerMessageObject layerMessageObject = JsonConvert.DeserializeObject<LayerMessageObject>(responseService.Content);
		if (!isTryProcess && layerMessageObject.error.code == 301)
		{
			ReConnect(sessionId);
			return UpdateObject(objectTypes, sessionId, xmlObject, objectKey, trytime, isTryProcess: true);
		}
		return new HttpResult(Enums.MessageCode.Error, $"{layerMessageObject.error.code}-{layerMessageObject.error.message.value}");
	}

	public HttpResult CloseObject(ApzObjectTypes objectType, string sessionId, object objectKey)
	{
		IRestResponse responseService = GetResponseService(objectType, sessionId, "", objectKey, Enums.HttpAction.Close);
		if (!string.IsNullOrEmpty(responseService.ErrorMessage))
		{
			return new HttpResult(Enums.MessageCode.Error, responseService.ErrorMessage);
		}
		if (responseService.StatusCode == HttpStatusCode.OK || responseService.StatusCode == HttpStatusCode.Accepted || responseService.StatusCode == HttpStatusCode.Created || responseService.StatusCode == HttpStatusCode.NoContent)
		{
			return new HttpResult(Enums.MessageCode.Success, "", responseService.Content);
		}
		LayerMessageObject layerMessageObject = JsonConvert.DeserializeObject<LayerMessageObject>(responseService.Content);
		return new HttpResult(Enums.MessageCode.Error, $"{layerMessageObject.error.code}-{layerMessageObject.error.message.value}");
	}

	public HttpResult RemoveObject(ApzObjectTypes objectType, string sessionId, object objectKey)
	{
		IRestResponse responseService = GetResponseService(objectType, sessionId, "", objectKey, Enums.HttpAction.Delete);
		if (!string.IsNullOrEmpty(responseService.ErrorMessage))
		{
			return new HttpResult(Enums.MessageCode.Error, responseService.ErrorMessage);
		}
		if (responseService.StatusCode == HttpStatusCode.OK || responseService.StatusCode == HttpStatusCode.Accepted || responseService.StatusCode == HttpStatusCode.Created || responseService.StatusCode == HttpStatusCode.NoContent)
		{
			return new HttpResult(Enums.MessageCode.Success, "", responseService.Content);
		}
		LayerMessageObject layerMessageObject = JsonConvert.DeserializeObject<LayerMessageObject>(responseService.Content);
		return new HttpResult(Enums.MessageCode.Error, $"{layerMessageObject.error.code}-{layerMessageObject.error.message.value}");
	}

	public HttpResult AddServiceObject(ApzObjectTypes objectTypes, string sessionId, string xmlObject, int trytime = 0, bool isTryProcess = false)
	{
		IRestResponse responseService = GetResponseService(objectTypes, sessionId, xmlObject, "", Enums.HttpAction.AddNew, "", isServiceAction: true);
		if (!string.IsNullOrEmpty(responseService.ErrorMessage))
		{
			return new HttpResult(Enums.MessageCode.Error, responseService.ErrorMessage);
		}
		if (responseService.StatusCode == HttpStatusCode.OK || responseService.StatusCode == HttpStatusCode.Accepted || responseService.StatusCode == HttpStatusCode.Created || responseService.StatusCode == HttpStatusCode.NoContent)
		{
			Parameter parameter = responseService.Headers.FirstOrDefault((Parameter t) => t.Name.Equals("Location"));
			if (parameter == null)
			{
				return new HttpResult(Enums.MessageCode.Success);
			}
			string location = Function.ToString(parameter.Value);
			return new HttpResult(Enums.MessageCode.Success, "", GetIdObjectFromLocation(location));
		}
		LayerMessageObject layerMessageObject = JsonConvert.DeserializeObject<LayerMessageObject>(responseService.Content);
		if (!isTryProcess && layerMessageObject.error.code == 301)
		{
			ReConnect(sessionId);
			return AddServiceObject(objectTypes, sessionId, xmlObject, trytime, isTryProcess: true);
		}
		return new HttpResult(Enums.MessageCode.Error, $"{layerMessageObject.error.code}-{layerMessageObject.error.message.value}");
	}

	public HttpResult UpdateServiceObject(ApzObjectTypes objectTypes, string sessionId, string xmlObject, object objectKey = null, int trytime = 0, bool isTryProcess = false)
	{
		IRestResponse responseService = GetResponseService(objectTypes, sessionId, xmlObject, objectKey, Enums.HttpAction.Update, "", isServiceAction: true);
		if (!string.IsNullOrEmpty(responseService.ErrorMessage))
		{
			return new HttpResult(Enums.MessageCode.Error, responseService.ErrorMessage);
		}
		if (responseService.StatusCode == HttpStatusCode.OK || responseService.StatusCode == HttpStatusCode.Accepted || responseService.StatusCode == HttpStatusCode.Created || responseService.StatusCode == HttpStatusCode.NoContent)
		{
			Parameter parameter = responseService.Headers.FirstOrDefault((Parameter t) => t.Name.Equals("Location"));
			if (parameter == null)
			{
				return new HttpResult(Enums.MessageCode.Success, "", objectKey);
			}
			string location = Function.ToString(parameter.Value);
			return new HttpResult(Enums.MessageCode.Success, "", GetIdObjectFromLocation(location));
		}
		LayerMessageObject layerMessageObject = JsonConvert.DeserializeObject<LayerMessageObject>(responseService.Content);
		if (!isTryProcess && layerMessageObject.error.code == 301)
		{
			ReConnect(sessionId);
			return UpdateObject(objectTypes, sessionId, xmlObject, objectKey, trytime, isTryProcess: true);
		}
		return new HttpResult(Enums.MessageCode.Error, $"{layerMessageObject.error.code}-{layerMessageObject.error.message.value}");
	}

	public HttpResult RemoveServiceObject(ApzObjectTypes objectType, string sessionId, string jsonObject)
	{
		IRestResponse responseService = GetResponseService(objectType, sessionId, jsonObject, "", Enums.HttpAction.Delete, "", isServiceAction: true);
		if (!string.IsNullOrEmpty(responseService.ErrorMessage))
		{
			return new HttpResult(Enums.MessageCode.Error, responseService.ErrorMessage);
		}
		if (responseService.StatusCode == HttpStatusCode.OK || responseService.StatusCode == HttpStatusCode.Accepted || responseService.StatusCode == HttpStatusCode.Created || responseService.StatusCode == HttpStatusCode.NoContent)
		{
			return new HttpResult(Enums.MessageCode.Success, "", responseService.Content);
		}
		LayerMessageObject layerMessageObject = JsonConvert.DeserializeObject<LayerMessageObject>(responseService.Content);
		return new HttpResult(Enums.MessageCode.Error, $"{layerMessageObject.error.code}-{layerMessageObject.error.message.value}");
	}

	public HttpResult GetDataObjectFromService(ApzObjectTypes objectType, string sessionId, Enums.HttpAction action = Enums.HttpAction.Get, object objectKey = null, string bodyString = "", bool isServiceAction = false, bool isTryProcess = false)
	{
		IRestResponse responseService = GetResponseService(objectType, sessionId, bodyString, objectKey, action, "", isServiceAction);
		if (!string.IsNullOrEmpty(responseService.ErrorMessage))
		{
			return new HttpResult(Enums.MessageCode.Error, responseService.ErrorMessage);
		}
		if (responseService.StatusCode == HttpStatusCode.OK || responseService.StatusCode == HttpStatusCode.Accepted || responseService.StatusCode == HttpStatusCode.Created || responseService.StatusCode == HttpStatusCode.NoContent)
		{
			return new HttpResult(Enums.MessageCode.Success, "", responseService.Content);
		}
		LayerMessageObject layerMessageObject = JsonConvert.DeserializeObject<LayerMessageObject>(responseService.Content);
		if (!isTryProcess && layerMessageObject.error.code == 301)
		{
			ReConnect(sessionId);
			return GetDataObjectFromService(objectType, sessionId, action, objectKey, bodyString, isServiceAction, isTryProcess: true);
		}
		return new HttpResult(Enums.MessageCode.Error, $"{layerMessageObject.error.code}-{layerMessageObject.error.message.value}");
	}

	public HttpResult ProcessMutiTransaction(string sessionId, List<BodyRequestObject> lstBodyRequestObjects, bool isTryProcess = false, bool replaceOnPatch = true)
	{
		if (lstBodyRequestObjects == null || !lstBodyRequestObjects.Any())
		{
			throw new Exception("Object request can not be null");
		}
		if (lstBodyRequestObjects.Count == 1)
		{
			BodyRequestObject bodyRequestObject = lstBodyRequestObjects[0];
			return ProcessJson(bodyRequestObject.ObjectType, sessionId, bodyRequestObject.BodyObject, bodyRequestObject.Action, bodyRequestObject.Key, bodyRequestObject.UdoCode, isTryProcess: false, replaceOnPatch);
		}
		IRestResponse responseBatchService = GetResponseBatchService(sessionId, lstBodyRequestObjects, replaceOnPatch);
		if (!string.IsNullOrEmpty(responseBatchService.ErrorMessage))
		{
			return new HttpResult(Enums.MessageCode.Error, responseBatchService.ErrorMessage);
		}
		if ((responseBatchService.StatusCode == HttpStatusCode.OK || responseBatchService.StatusCode == HttpStatusCode.Accepted || responseBatchService.StatusCode == HttpStatusCode.Created) && !responseBatchService.Content.Contains("\"error\" : {"))
		{
			List<BodyRequestObject> list = lstBodyRequestObjects.Where((BodyRequestObject t) => t.OutObjectKey).ToList();
			if (list == null)
			{
				return new HttpResult(Enums.MessageCode.Success, "", responseBatchService.Content);
			}
			if (list.Count > 0)
			{
				string text = string.Empty;
				int num = 0;
				for (int num2 = 0; num2 < list.Count; num2++)
				{
					GetHanaServiceName(list[num2].ObjectType, "", Enums.HttpAction.AddNew, list[num2].UdoCode);
					num = responseBatchService.Content.IndexOf("Content-ID: " + (num2 + 1), num, StringComparison.Ordinal);
					if (num > -1)
					{
						int num3 = responseBatchService.Content.IndexOf(")\r\n", num, StringComparison.Ordinal);
						if (num < num3)
						{
							text = ((num2 == list.Count - 1) ? (text + GetIdObjectFromLocation(responseBatchService.Content.Substring(num, num3 - num + 1))) : (text + GetIdObjectFromLocation(responseBatchService.Content.Substring(num, num3 - num + 1)) + ","));
							num = num3;
						}
					}
				}
				return new HttpResult(Enums.MessageCode.Success, "", text);
			}
			return new HttpResult(Enums.MessageCode.Success);
		}
		int num4 = responseBatchService.Content.IndexOf('{');
		int num5 = responseBatchService.Content.LastIndexOf('}');
		if (num4 >= num5)
		{
			return new HttpResult(Enums.MessageCode.Success, responseBatchService.Content);
		}
		LayerMessageObject layerMessageObject = JsonConvert.DeserializeObject<LayerMessageObject>(responseBatchService.Content.Substring(num4, num5 - num4 + 1));
		if (!isTryProcess && layerMessageObject.error.code == 301)
		{
			ReConnect(sessionId);
			return ProcessMutiTransaction(sessionId, lstBodyRequestObjects, isTryProcess: true, replaceOnPatch);
		}
		return new HttpResult(Enums.MessageCode.Error, $"{layerMessageObject.error.code}-{layerMessageObject.error.message.value}");
	}

	public HttpResult GetByKey(string sessionId, string xmlObject, int trytime = 0)
	{
		return new HttpResult(Enums.MessageCode.Success);
	}

	public HttpResult Login(string dataBaseServer, string dataBaseName, string dataBaseType, string dataBaseUserName, string dataBasePassword, string companyUserName, string companyPassword, string language, string licenseServer)
	{
		try
		{
			string jsonString = $"{{\"CompanyDB\": \"{dataBaseName}\",\"Password\":\"{companyPassword}\",\"UserName\":\"{companyUserName}\"}}";
			IRestResponse responseService = GetResponseService(ApzObjectTypes.oLogin, "", jsonString, "", Enums.HttpAction.Login);
			if (responseService.StatusCode == HttpStatusCode.OK || responseService.StatusCode == HttpStatusCode.Accepted || responseService.StatusCode == HttpStatusCode.Created)
			{
				SessionObj result = JsonConvert.DeserializeObject<SessionObj>(responseService.Content);
				if (string.IsNullOrEmpty(result.SessionId))
				{
					return new HttpResult(Enums.MessageCode.Success, "Can not login to server. Session response is null");
				}
				if (LstDiConnections != null && LstDiConnections.Exists((DiConnection t) => t.Id.Split(',').Contains(result.SessionId)))
				{
					return Login(dataBaseServer, dataBaseName, dataBaseType, dataBaseUserName, dataBasePassword, companyUserName, companyPassword, language, licenseServer);
				}
				return new HttpResult
				{
					Content = result.SessionId
				};
			}
			if (responseService.StatusCode == HttpStatusCode.Unauthorized)
			{
				LayerMessageObject layerMessageObject = JsonConvert.DeserializeObject<LayerMessageObject>(responseService.Content);
				return new HttpResult(Enums.MessageCode.Error, $"{layerMessageObject.error.code}-{layerMessageObject.error.message.value}");
			}
			return new HttpResult(Enums.MessageCode.Error, $"Login failed. {responseService.ErrorMessage}");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(0).ToString(), ex.ToString());
			return new HttpResult(Enums.MessageCode.Error, "[ERROR] " + ex.Message);
		}
	}

	public object LogOut(string sSessionId)
	{
		IRestResponse responseService = GetResponseService(ApzObjectTypes.oLogout, sSessionId, "", "", Enums.HttpAction.Logout);
		CloseConnection(sSessionId);
		return responseService.Content;
	}

	public string GetCurrentSessionId(string xmlObject)
	{
		return string.Empty;
	}

	public string UpdateSessionId(string sessionId, string xmlObject)
	{
		return xmlObject;
	}

	public string ReConnect(string sessionId)
	{
		if (LstDiConnections != null)
		{
			DiConnection diConnection = LstDiConnections.FirstOrDefault((DiConnection t) => t?.Id.Split(',').Contains(sessionId) ?? false);
			if (diConnection != null)
			{
				string text = Function.ToString(Login(diConnection.DbServer, diConnection.DbName, diConnection.DbTypeStr, diConnection.DbUserName, diConnection.DbPwd, diConnection.UserName, diConnection.UserPwd, diConnection.Lang, diConnection.LicServer).Content);
				if (string.IsNullOrEmpty(text) || !text.ToUpper().Contains("ERROR"))
				{
					diConnection.SessionId = text;
					Logging.Write("WATCH", $"API auto ReConnect\t{diConnection.DbName}\t{diConnection.UserName}", text);
					return text;
				}
				Logging.Write("ERROR", $"API auto ReConnect Error\t{diConnection.DbName}\t{diConnection.UserName}\t{text}");
			}
			else
			{
				Logging.Write("ERROR", "Unknow Session. " + sessionId);
			}
		}
		else
		{
			Logging.Write("ERROR", "DI Connections is empty");
		}
		return string.Empty;
	}

	public static string OpenConnection(string sessionId)
	{
		if (LstDiConnections == null)
		{
			return sessionId;
		}
		DiConnection diConnection = LstDiConnections.FirstOrDefault((DiConnection t) => t?.Id.Split(',').Contains(sessionId) ?? false);
		if (diConnection == null)
		{
			return sessionId;
		}
		return diConnection.SessionId;
	}

	public static void CloseConnection(string sessionId)
	{
		if (LstDiConnections != null)
		{
			DiConnection diConnection = LstDiConnections.FirstOrDefault((DiConnection t) => t?.Id.Split(',').Contains(sessionId) ?? false);
			if (diConnection != null)
			{
				LstDiConnections.Remove(diConnection);
			}
		}
	}

	public async Task<HttpResult> ProcessAsync(string xmlObject, int trytime = 0)
	{
		return await Task.FromResult(new HttpResult
		{
			Content = Process(xmlObject, trytime)
		});
	}

	public static string GetServiceActionName(ApzObjectTypes objectType, Enums.HttpAction action)
	{
		switch (objectType)
		{
		case ApzObjectTypes.oAlternativeItems:
			switch (action)
			{
			case Enums.HttpAction.AddNew:
				return "AlternativeItemsService_AddItem";
			case Enums.HttpAction.Update:
				return "AlternativeItemsService_UpdateItem";
			case Enums.HttpAction.Delete:
				return "AlternativeItemsService_DeleteItem";
			}
			break;
		case ApzObjectTypes.oChangeLog:
			switch (action)
			{
			case Enums.HttpAction.GetChangeLog:
				return "ChangeLogsService_GetChangeLog";
			case Enums.HttpAction.GetChangeLogDiff:
				return "ChangeLogsService_GetChangeLogDifferences";
			}
			break;
		case ApzObjectTypes.oDrafts:
			if (action == Enums.HttpAction.SaveDraftToDocument)
			{
				return "DraftsService_SaveDraftToDocument";
			}
			break;
		case ApzObjectTypes.oJournalVouchers:
			if (action == Enums.HttpAction.AddNew)
			{
				return "JournalVouchersService_Add";
			}
			break;
		}
		throw new Exception("Service not support");
	}

	public static string GetHanaServiceName(ApzObjectTypes objectType, object objectKey, Enums.HttpAction action, string udoCode = "", bool isServiceAction = false)
	{
		if (isServiceAction)
		{
			return GetServiceActionName(objectType, action);
		}
		string text = "";
		switch (objectType)
		{
		case ApzObjectTypes.oLogin:
			text = "Login";
			break;
		case ApzObjectTypes.oLogout:
			text = "Logout";
			break;
		case ApzObjectTypes.oChartOfAccounts:
			text = "ChartOfAccounts";
			break;
		case ApzObjectTypes.oBusinessPartnerGroups:
			text = "BusinessPartnerGroups";
			break;
		case ApzObjectTypes.oActivitySubject:
			text = "Activities";
			break;
		case ApzObjectTypes.oSalesStages:
			text = "SalesStages";
			break;
		case ApzObjectTypes.oActivityTypes:
			text = "ActivityTypes";
			break;
		case ApzObjectTypes.oActivityLocations:
			text = "ActivityLocations";
			break;
		case ApzObjectTypes.oDrafts:
			text = "Drafts";
			break;
		case ApzObjectTypes.oDeductionTaxHierarchies:
			text = "DeductionTaxHierarchies";
			break;
		case ApzObjectTypes.oDeductionTaxGroups:
			text = "DeductionTaxGroups";
			break;
		case ApzObjectTypes.oUsers:
			text = "Users";
			break;
		case ApzObjectTypes.oDistributionRule:
			text = "DistributionRules";
			break;
		case ApzObjectTypes.oApprovalStages:
			text = "ApprovalStages";
			break;
		case ApzObjectTypes.oApprovalTemplates:
			text = "ApprovalTemplates";
			break;
		case ApzObjectTypes.oApprovalRequests:
			text = "ApprovalRequests";
			break;
		case ApzObjectTypes.oAdditionalExpenses:
			text = "AdditionalExpenses";
			break;
		case ApzObjectTypes.oInventoryTransferRequest:
			text = "InventoryTransferRequests";
			break;
		case ApzObjectTypes.oSalesTaxAuthorities:
			text = "SalesTaxAuthorities";
			break;
		case ApzObjectTypes.oSalesTaxAuthoritiesTypes:
			text = "SalesTaxAuthoritiesTypes";
			break;
		case ApzObjectTypes.oSalesTaxCodes:
			text = "SalesTaxCodes";
			break;
		case ApzObjectTypes.oCountries:
			text = "Countries";
			break;
		case ApzObjectTypes.oInvoices:
			text = "Invoices";
			break;
		case ApzObjectTypes.oQueryCategories:
			text = "QueryCategories";
			break;
		case ApzObjectTypes.oFactoringIndicators:
			text = "FactoringIndicators";
			break;
		case ApzObjectTypes.oCreditNotes:
			text = "CreditNotes";
			break;
		case ApzObjectTypes.oPaymentsDrafts:
			text = "PaymentDrafts";
			break;
		case ApzObjectTypes.oAccountSegmentations:
			text = "AccountSegmentations";
			break;
		case ApzObjectTypes.oAccountSegmentationCategories:
			text = "AccountSegmentationCategories";
			break;
		case ApzObjectTypes.oWarehouseLocations:
			text = "WarehouseLocations";
			break;
		case ApzObjectTypes.oForms1099:
			text = "Forms1099";
			break;
		case ApzObjectTypes.oInventoryCycles:
			text = "InventoryCycles";
			break;
		case ApzObjectTypes.oWizardPaymentMethods:
			text = "WizardPaymentMethods";
			break;
		case ApzObjectTypes.oPurchaseRequest:
			text = "PurchaseRequests";
			break;
		case ApzObjectTypes.oDeliveryNotes:
			text = "DeliveryNotes";
			break;
		case ApzObjectTypes.oBPPriorities:
			text = "BPPriorities";
			break;
		case ApzObjectTypes.oDunningLetters:
			text = "DunningLetters";
			break;
		case ApzObjectTypes.oUserFields:
			text = "UserFieldsMD";
			break;
		case ApzObjectTypes.oUserTables:
			text = "UserTablesMD";
			break;
		case ApzObjectTypes.oPickLists:
			text = "PickLists";
			break;
		case ApzObjectTypes.oPaymentRunExport:
			text = "PaymentRunExport";
			break;
		case ApzObjectTypes.oReturns:
			text = "Returns";
			break;
		case ApzObjectTypes.oReturnRequest:
			text = "ReturnRequest";
			break;
		case ApzObjectTypes.oUserQueries:
			text = "UserQueries";
			break;
		case ApzObjectTypes.oMaterialRevaluation:
			text = "MaterialRevaluation";
			break;
		case ApzObjectTypes.oCorrectionInvoice:
			text = "CorrectionInvoice";
			break;
		case ApzObjectTypes.oCorrectionInvoiceReversal:
			text = "CorrectionInvoiceReversal";
			break;
		case ApzObjectTypes.oCorrectionPurchaseInvoice:
			text = "CorrectionPurchaseInvoice";
			break;
		case ApzObjectTypes.oCorrectionPurchaseInvoiceReversal:
			text = "CorrectionPurchaseInvoiceReversal";
			break;
		case ApzObjectTypes.oOrders:
			text = "Orders";
			break;
		case ApzObjectTypes.oContractTemplates:
			text = "ContractTemplates";
			break;
		case ApzObjectTypes.oEmployeesInfo:
			text = "EmployeesInfo";
			break;
		case ApzObjectTypes.oCustomerEquipmentCards:
			text = "CustomerEquipmentCards";
			break;
		case ApzObjectTypes.oWithholdingTaxCodes:
			text = "WithholdingTaxCodes";
			break;
		case ApzObjectTypes.oPurchaseInvoices:
			text = "PurchaseInvoices";
			break;
		case ApzObjectTypes.oBillOfExchangeTransactions:
			text = "BillOfExchangeTransaction";
			break;
		case ApzObjectTypes.oKnowledgeBaseSolutions:
			text = "KnowledgeBaseSolutions";
			break;
		case ApzObjectTypes.oPurchaseCreditNotes:
			text = "PurchaseCreditNotes";
			break;
		case ApzObjectTypes.oServiceContracts:
			text = "ServiceContracts";
			break;
		case ApzObjectTypes.oServiceCalls:
			text = "ServiceCalls";
			break;
		case ApzObjectTypes.oUserKeys:
			text = "UserKeysMD";
			break;
		case ApzObjectTypes.oQueue:
			text = "Queue";
			break;
		case ApzObjectTypes.oSalesForecast:
			text = "SalesForecast";
			break;
		case ApzObjectTypes.oBusinessPartners:
			text = "BusinessPartners";
			break;
		case ApzObjectTypes.oBusinessPartnerProperties:
			text = "BusinessPartnerProperties";
			break;
		case ApzObjectTypes.oBusinessPartnerPriorities:
			text = "BPPriorities";
			break;
		case ApzObjectTypes.oPurchaseDeliveryNotes:
			text = "PurchaseDeliveryNotes";
			break;
		case ApzObjectTypes.oTerritories:
			text = "Territories";
			break;
		case ApzObjectTypes.oIndustries:
			text = "Industries";
			break;
		case ApzObjectTypes.oProductionOrders:
			text = "ProductionOrders";
			break;
		case ApzObjectTypes.oDownPayments:
			text = "DownPayments";
			break;
		case ApzObjectTypes.oPurchaseDownPayments:
			text = "PurchaseDownPayments";
			break;
		case ApzObjectTypes.oPackagesTypes:
			text = "PackagesTypes";
			break;
		case ApzObjectTypes.oUserObjectsMD:
			text = "UserObjectsMD";
			break;
		case ApzObjectTypes.oPurchaseReturns:
			text = "PurchaseReturns";
			break;
		case ApzObjectTypes.oGoodsReturnRequest:
			text = "GoodsReturnRequest";
			break;
		case ApzObjectTypes.oTeams:
			text = "Teams";
			break;
		case ApzObjectTypes.oRoles:
			text = "EmployeeRolesSetup";
			break;
		case ApzObjectTypes.oRelationships:
			text = "Relationships";
			break;
		case ApzObjectTypes.oUserPermissionTree:
			text = "UserPermissionTree";
			break;
		case ApzObjectTypes.oActivityStatus:
			text = "ActivityStatuses";
			break;
		case ApzObjectTypes.oChooseFromList:
			text = "ChooseFromList";
			break;
		case ApzObjectTypes.oFormattedSearches:
			text = "FormattedSearches";
			break;
		case ApzObjectTypes.oPurchaseOrders:
			text = "PurchaseOrders";
			break;
		case ApzObjectTypes.oAttachments2:
			text = "Attachments2";
			break;
		case ApzObjectTypes.oUserLanguages:
			text = "UserLanguages";
			break;
		case ApzObjectTypes.oMultiLanguageTranslations:
			text = "MultiLanguageTranslations";
			break;
		case ApzObjectTypes.oDynamicSystemStrings:
			text = "DynamicSystemStrings";
			break;
		case ApzObjectTypes.oQuotations:
			text = "Quotations";
			break;
		case ApzObjectTypes.oHouseBankAccounts:
			text = "HouseBankAccounts";
			break;
		case ApzObjectTypes.oIncomingPayments:
		case ApzObjectTypes.oIncomingPayment:
			text = "IncomingPayments";
			break;
		case ApzObjectTypes.oOutgoingPayment:
			text = "VendorPayments";
			break;
		case ApzObjectTypes.oBusinessPlaces:
			text = "BusinessPlaces";
			break;
		case ApzObjectTypes.oLocalEra:
			text = "LocalEra";
			break;
		case ApzObjectTypes.oSalesTaxInvoice:
			text = "SalesTaxInvoices";
			break;
		case ApzObjectTypes.oPurchaseTaxInvoice:
			text = "PurchaseTaxInvoices";
			break;
		case ApzObjectTypes.oBanks:
			text = "Banks";
			break;
		case ApzObjectTypes.oJournalEntries:
			text = "JournalEntries";
			break;
		case ApzObjectTypes.oContacts:
			text = "Contacts";
			break;
		case ApzObjectTypes.oCreditCards:
			text = "CreditCards";
			break;
		case ApzObjectTypes.oCurrencyCodes:
			text = "Currencies";
			break;
		case ApzObjectTypes.oItems:
			text = "Items";
			break;
		case ApzObjectTypes.oPaymentTermsTypes:
			text = "PaymentTermsTypes";
			break;
		case ApzObjectTypes.oBankPages:
			text = "BankPages";
			break;
		case ApzObjectTypes.oManufacturers:
			text = "Manufacturers";
			break;
		case ApzObjectTypes.oVendorPayments:
			text = "VendorPayments";
			break;
		case ApzObjectTypes.oLandedCostsCodes:
			text = "LandedCostsCodes";
			break;
		case ApzObjectTypes.oShippingTypes:
			text = "ShippingTypes";
			break;
		case ApzObjectTypes.oVatGroups:
			text = "VatGroups";
			break;
		case ApzObjectTypes.oLengthMeasures:
			text = "LengthMeasures";
			break;
		case ApzObjectTypes.oWeightMeasures:
			text = "WeightMeasures";
			break;
		case ApzObjectTypes.oItemGroups:
			text = "ItemGroups";
			break;
		case ApzObjectTypes.oSalesPersons:
			text = "SalesPersons";
			break;
		case ApzObjectTypes.oPurchaseQuotations:
			text = "PurchaseQuotations";
			break;
		case ApzObjectTypes.oCustomsGroups:
			text = "CustomsGroups";
			break;
		case ApzObjectTypes.oChecksforPayment:
			text = "ChecksforPayment";
			break;
		case ApzObjectTypes.oInventoryGenEntry:
			text = "InventoryGenEntries";
			break;
		case ApzObjectTypes.oPriceLists:
			text = "PriceLists";
			break;
		case ApzObjectTypes.oInventoryGenExit:
			text = "InventoryGenExits";
			break;
		case ApzObjectTypes.oProfitCenters:
			text = "ProfitCenters";
			break;
		case ApzObjectTypes.oProjectCodes:
			text = "Projects";
			break;
		case ApzObjectTypes.oWarehouses:
			text = "Warehouses";
			break;
		case ApzObjectTypes.oCommissionGroups:
			text = "CommissionGroups";
			break;
		case ApzObjectTypes.oProductTrees:
			text = "ProductTrees";
			break;
		case ApzObjectTypes.oStockTransferDraft:
			text = "StockTransferDrafts";
			break;
		case ApzObjectTypes.oStockTransfer:
			text = "StockTransfers";
			break;
		case ApzObjectTypes.oWorkOrders:
			text = "WorkOrders";
			break;
		case ApzObjectTypes.oSpecialPrices:
			text = "SpecialPrices";
			break;
		case ApzObjectTypes.oCreditPaymentMethods:
			text = "CreditPaymentMethods";
			break;
		case ApzObjectTypes.oCreditCardPayments:
			text = "CreditCardPayments";
			break;
		case ApzObjectTypes.oAlternateCatNum:
			text = "AlternateCatNum";
			break;
		case ApzObjectTypes.oBudget:
			text = "Budgets";
			break;
		case ApzObjectTypes.oBudgetDistribution:
			text = "BudgetDistributions";
			break;
		case ApzObjectTypes.oItemProperties:
			text = "ItemProperties";
			break;
		case ApzObjectTypes.oAlertManagements:
			text = "AlertManagements";
			break;
		case ApzObjectTypes.oMessages:
			text = "Messages";
			break;
		case ApzObjectTypes.oBudgetScenarios:
			text = "BudgetScenarios";
			break;
		case ApzObjectTypes.oUserDefaultGroups:
			text = "UserDefaultGroups";
			break;
		case ApzObjectTypes.oSalesOpportunities:
			text = "SalesOpportunities";
			break;
		case ApzObjectTypes.oBlanketAgreement:
			text = "BlanketAgreements";
			break;
		case ApzObjectTypes.oBatchDetails:
			text = "BatchNumberDetails";
			break;
		case ApzObjectTypes.oSerialNumberDetails:
			text = "SerialNumberDetails";
			break;
		case ApzObjectTypes.oDimension:
			text = "Dimensions";
			break;
		case ApzObjectTypes.oDiscountGroups:
			text = "EnhancedDiscountGroups";
			break;
		case ApzObjectTypes.oVehicle:
			text = "VEHICLE";
			break;
		case ApzObjectTypes.oNone:
			text = udoCode;
			break;
		case ApzObjectTypes.oOpportunityCompetitor:
			text = "SalesOpportunityCompetitorsSetup";
			break;
		case ApzObjectTypes.oOpportunityPartners:
			text = "PartnersSetups";
			break;
		case ApzObjectTypes.oOpportunityReasons:
			text = "SalesOpportunityReasonsSetup";
			break;
		case ApzObjectTypes.oInterest:
			text = "SalesOpportunityInterestsSetup";
			break;
		case ApzObjectTypes.oUserDefinedTexts:
			text = "PredefinedTexts";
			break;
		case ApzObjectTypes.oBinLocFieldActivation:
			text = "BinLocationFields";
			break;
		case ApzObjectTypes.oWarehouseSublevel:
			text = "WarehouseSublevelCodes";
			break;
		case ApzObjectTypes.oBinLocAttribute:
			text = "BinLocationAttributes";
			break;
		case ApzObjectTypes.oBinLocation:
			text = "BinLocations";
			break;
		case ApzObjectTypes.oPurchaseLandedCost:
			text = "LandedCosts";
			break;
		case ApzObjectTypes.oInternalReconciliations:
			text = "InternalReconciliations";
			break;
		case ApzObjectTypes.oUoM:
			text = "UnitOfMeasurements";
			break;
		case ApzObjectTypes.oUoMDefGroups:
			text = "UnitOfMeasurementGroups";
			break;
		default:
			throw new Exception("Service not support");
		}
		switch (action)
		{
		case Enums.HttpAction.Cancel:
			if (objectType == ApzObjectTypes.oBlanketAgreement)
			{
				return $"{text}{GetSuffixes(objectKey)}/CancelBlanketAgreement";
			}
			return $"{text}{GetSuffixes(objectKey)}/Cancel";
		case Enums.HttpAction.Close:
			return $"{text}{GetSuffixes(objectKey)}/Close";
		case Enums.HttpAction.Update:
		case Enums.HttpAction.Delete:
			return $"{text}{GetSuffixes(objectKey)}";
		case Enums.HttpAction.Reopen:
			return $"{text}{GetSuffixes(objectKey)}/Reopen";
		case Enums.HttpAction.CreateCancellationDocument:
			return $"{text}{GetSuffixes(objectKey)}/CreateCancellationDocument";
		case Enums.HttpAction.SaveDraftToDocument:
			return $"{text}{GetSuffixes(objectKey)}/SaveDraftToDocument";
		case Enums.HttpAction.CreateLinkedItem:
			return $"{text}{GetSuffixes(objectKey)}/CreateLinkedItem";
		case Enums.HttpAction.Get:
			return $"{text}{GetSuffixes(objectKey)}";
		default:
			return text;
		}
	}

	public static Method GetHttpMethodByAction(Enums.HttpAction action, bool isServiceAction)
	{
		if (isServiceAction)
		{
			return Method.POST;
		}
		return action switch
		{
			Enums.HttpAction.Get => Method.GET, 
			Enums.HttpAction.Delete => Method.DELETE, 
			Enums.HttpAction.Update => Method.PATCH, 
			_ => Method.POST, 
		};
	}

	public static string GetSuffixes(object value)
	{
		string text = "";
		if (value is string)
		{
			if (!string.IsNullOrEmpty(Function.ToString(value)))
			{
				text = $"'{value}'";
			}
		}
		else
		{
			PropertyInfo[] array = value?.GetType().GetProperties();
			if (array != null && array.Length != 0)
			{
				List<string> list = new List<string>();
				Type type = value.GetType();
				PropertyInfo[] array2 = array;
				foreach (PropertyInfo propertyInfo in array2)
				{
					object value2 = type.GetProperty(propertyInfo.Name).GetValue(value, null);
					list.Add(propertyInfo.Name + "=" + ((value2 is string) ? $"'{Function.ToString(value2)}'" : Function.ToString(value2)));
				}
				text = string.Join(",", list.ToArray());
			}
		}
		if (string.IsNullOrEmpty(text))
		{
			text = Function.ToString(value);
		}
		return $"({text})";
	}

	private IRestResponse GetResponseService(ApzObjectTypes objectTypes, string sessionId, string jsonString, object objectKey, Enums.HttpAction action, string udoCode = "", bool isServiceAction = false, bool replaceOnPatch = true)
	{
		RestClient restClient = new RestClient($"https://{_serviceAddress}/b1s/v1/{GetHanaServiceName(objectTypes, objectKey, action, udoCode, isServiceAction)}");
		RestRequest restRequest = new RestRequest(GetHttpMethodByAction(action, isServiceAction));
		restRequest.AddHeader("Cache-Control", "no-cache");
		restRequest.AddHeader("Accept", "application/json");
		restRequest.AddHeader("Prefer", "return-no-content");
		restRequest.AddHeader("Content-Type", "application/json;odata=minimalmetadata;charset=utf8");
		if (replaceOnPatch)
		{
			restRequest.AddHeader("B1S-ReplaceCollectionsOnPatch", "true");
		}
		restRequest.Timeout = 1800000;
		sessionId = OpenConnection(sessionId);
		if (!string.IsNullOrEmpty(sessionId))
		{
			restRequest.AddCookie("B1SESSION", sessionId);
		}
		if (!string.IsNullOrEmpty(jsonString))
		{
			restRequest.AddParameter("undefined", jsonString, ParameterType.RequestBody);
		}
		return restClient.Execute(restRequest);
	}

	private IRestResponse GetResponseBatchService(string sessionId, List<BodyRequestObject> lstBodyRequestObjects, bool replaceOnPatch = true)
	{
		RestClient restClient = new RestClient($"https://{_serviceAddress}/b1s/v1/$batch");
		RestRequest restRequest = new RestRequest(Method.POST);
		restRequest.AddHeader("Cache-Control", "no-cache");
		restRequest.AddHeader("Accept", "application/json");
		restRequest.AddHeader("Content-Type", string.Format("multipart/mixed; boundary={0}", "Batch_Header"));
		if (replaceOnPatch)
		{
			restRequest.AddHeader("B1S-ReplaceCollectionsOnPatch", "true");
		}
		restRequest.Timeout = 1800000;
		sessionId = OpenConnection(sessionId);
		if (!string.IsNullOrEmpty(sessionId))
		{
			restRequest.AddCookie("B1SESSION", sessionId);
		}
		string batchBodyRequest = GetBatchBodyRequest(lstBodyRequestObjects, replaceOnPatch);
		if (!string.IsNullOrEmpty(batchBodyRequest))
		{
			restRequest.AddParameter(string.Format("multipart/mixed; boundary={0}", "Batch_Header"), batchBodyRequest, ParameterType.RequestBody);
		}
		return restClient.Execute(restRequest);
	}

	private string GetBatchBodyRequest(List<BodyRequestObject> lstBodyRequestObjects, bool replaceOnPatch = true)
	{
		string text = string.Format("--{0}\r\nContent-Type: multipart/mixed:boundary={1}", "Batch_Header", "Batch_Body");
		for (int i = 0; i < lstBodyRequestObjects.Count; i++)
		{
			text += string.Format("\r\n\r\n--{0}\r\nContent-Type: application/http \r\nContent-Transfer-Encoding:binary\r\nContent-ID: {1}\r\n\r\n{2} /b1s/v1/{3}\r\nContent-Type: application/json\r\n{5}\r\n\r\n{4}", "Batch_Body", i + 1, GetHttpMethodByAction(lstBodyRequestObjects[i].Action, lstBodyRequestObjects[i].IsServiceAction), GetHanaServiceName(lstBodyRequestObjects[i].ObjectType, lstBodyRequestObjects[i].Key, lstBodyRequestObjects[i].Action, lstBodyRequestObjects[i].UdoCode), string.IsNullOrEmpty(lstBodyRequestObjects[i].BodyObject) ? "{}" : lstBodyRequestObjects[i].BodyObject, replaceOnPatch ? "B1S-ReplaceCollectionsOnPatch=true" : "");
		}
		return string.Format("{0}\r\n\r\n--{1}--\r\n--{2}--", text, "Batch_Body", "Batch_Header");
	}

	private string GetIdObjectFromLocation(string location)
	{
		int num = location.LastIndexOf('(');
		int num2 = location.LastIndexOf(')');
		string text = "";
		if (num < num2 - 1)
		{
			text = location.Substring(num + 1, num2 - num - 1);
			if (text.StartsWith("'"))
			{
				text = text.Substring(1);
			}
			if (text.EndsWith("'"))
			{
				text = text.Substring(0, text.Length - 1);
			}
		}
		return text;
	}

	public virtual string MappingUdf(List<UserFieldsImpl> lst, EnumCommandType commandType = EnumCommandType.Add, bool ignoreUpdateEmptyValue = false, List<string> lstFieldNotValidated = null)
	{
		string text = string.Empty;
		try
		{
			if (lst != null && lst.Count > 0)
			{
				foreach (UserFieldsImpl item in lst)
				{
					if (item == null || string.IsNullOrEmpty(item.Name))
					{
						continue;
					}
					if (!item.Name.StartsWith("U_"))
					{
						item.Name = "U_" + item.Name;
					}
					switch (item.Type)
					{
					case BoFieldTypes.db_Alpha:
					case BoFieldTypes.db_Memo:
						if (commandType == EnumCommandType.Add || !string.IsNullOrEmpty(Function.ToString(item.Value)) || !ignoreUpdateEmptyValue)
						{
							text = ((lstFieldNotValidated == null || !lstFieldNotValidated.Contains(item.Name)) ? (text + string.Format("\"{0}\": {1},", item.Name, (!string.IsNullOrEmpty(Function.ToString(item.Value))) ? JsonConvert.ToString(item.Value) : "null")) : (text + $"\"{item.Name}\": {JsonConvert.ToString(Function.ToString(item.Value))},"));
						}
						break;
					case BoFieldTypes.db_Date:
						if (Function.ParseDateTime(item.Value).HasValue && Convert.ToDateTime(item.Value) > new DateTime(1900, 1, 1))
						{
							text += string.Format("\"{0}\": \"{1}\",", item.Name, Convert.ToDateTime(Function.ToString(item.Value)).ToString("yyyyMMdd"));
						}
						else if (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update)
						{
							text += string.Format("\"{0}\": {1},", item.Name, "null");
						}
						break;
					case BoFieldTypes.db_Numeric:
					case BoFieldTypes.db_Float:
						if (!((Math.Abs(Function.ParseDouble(item.Value)) < 1E-07) & ignoreUpdateEmptyValue) || commandType != EnumCommandType.Update)
						{
							text = ((item.SubType != BoFldSubTypes.st_Time && (!Function.IsInt(Function.ToString(item.Value)) || Function.ToString(item.Value).Length > 4)) ? (text + string.Format("\"{0}\": {1},", item.Name, (item.Value == null) ? "null" : Function.ParseDecimal(item.Value).ToString())) : (text + string.Format("\"{0}\": {1},", item.Name, (item.Value == null) ? "null" : Function.ParseInt(item.Value).ToString())));
						}
						break;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return text;
	}

	public virtual string MappingUdf(DataRow dataRow, EnumCommandType commandType = EnumCommandType.Add, bool ignoreUpdateEmptyValue = false)
	{
		string text = string.Empty;
		try
		{
			List<DataColumn> list = (from DataColumn t in dataRow.Table.Columns
				where Function.ToString(t.ColumnName).StartsWith("U_")
				select t).ToList();
			if (list.Count > 0)
			{
				foreach (DataColumn item in list)
				{
					if (item == null || string.IsNullOrEmpty(item.ColumnName))
					{
						continue;
					}
					object obj = dataRow[item];
					if (item.DataType == typeof(string) && (!string.IsNullOrEmpty(Function.ToString(obj)) || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update)))
					{
						text += Function.GetJsonString(item.ColumnName, Function.ToString(obj));
					}
					if (item.DataType == typeof(DateTime) || item.DataType == typeof(DateTime?))
					{
						if (Function.ParseDateTime(obj).HasValue && Convert.ToDateTime(obj) > new DateTime(1900, 1, 1))
						{
							text += string.Format("\"{0}\": \"{1}\",", item.ColumnName, Convert.ToDateTime(Function.ToString(obj)).ToString("yyyyMMdd"));
						}
						else if (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update)
						{
							text += string.Format("\"{0}\": {1},", item.ColumnName, "null");
						}
					}
					if (new Type[6]
					{
						typeof(double),
						typeof(double?),
						typeof(float),
						typeof(float?),
						typeof(decimal),
						typeof(decimal?)
					}.Contains(item.DataType) && (Math.Abs(Function.ParseDecimal(obj)) > 0m || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update)))
					{
						text += Function.GetJsonString(item.ColumnName, Function.ParseDecimal(obj));
					}
					if (new Type[6]
					{
						typeof(short),
						typeof(short?),
						typeof(int),
						typeof(int?),
						typeof(long),
						typeof(long?)
					}.Contains(item.DataType))
					{
						text += Function.GetJsonString(item.ColumnName, Function.ParseInt(obj));
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return text;
	}
}
