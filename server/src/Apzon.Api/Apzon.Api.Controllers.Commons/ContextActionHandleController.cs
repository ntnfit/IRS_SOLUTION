using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.ApzJson;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.DataServices;
using Apzon.Entities.Models;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.MasterData;
using Apzon.Kit.ContextAction;
using Apzon.Services;
using Newtonsoft.Json;

namespace Apzon.Api.Controllers.Commons;

public class ContextActionHandleController : BaseApiController
{
	public const int ACTION_SUCCESS = 200;

	public const int ACTION_ERROR = 1;

	public const int INVALID_TOKEN = 2;

	public const int SERVICE_NOT_FOUND = 3;

	public const int ACTION_NOT_FOUND = 4;

	public const int INVALID_USER = 5;

	public const int INVALID_CONFIG = 6;

	public const int NOT_SOURCE = 7;

	public const int INVALID_SOURCE = 8;

	public const int COULD_NOT_GET_DATA = 9;

	[HttpPost]
	[ActionName("0")]
	public async Task<HttpResponseMessage> HandleTable(string name, string method)
	{
		_ = 1;
		try
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "INVALIDSESSION");
				string message = $"INVALID TOKEN {name}/{method}";
				return GetResponseMessage(new ResponseMessage
				{
					Code = 2,
					Message = message
				}, HttpStatusCode.Unauthorized);
			}
			if (UnitOfWork == null)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "INVALIDSESSION");
				string message2 = $"INVALID TOKEN {name}/{method}";
				return GetResponseMessage(new ResponseMessage
				{
					Code = 2,
					Message = message2
				}, HttpStatusCode.Unauthorized);
			}
			DataTable byName = UnitOfWork.PublishService.GetByName(name, method);
			if (!byName.IsNotNull())
			{
				string message3 = $"Service not found {name}/{method}";
				return GetResponseMessage(new ResponseMessage
				{
					Code = 3,
					Message = message3
				}, HttpStatusCode.NotFound);
			}
			DataTable dataTable = JsonConvert.DeserializeObject<DataTable>(Request.Content.ReadAsStringAsync().Result, new JsonConverter[1]
			{
				new ApzJsonClientObjectToDataTable()
			});
			string outputType = Function.ToString(byName.Rows[0]["OputType"]);
			string outputQuery = Function.ToString(byName.Rows[0]["OputQuery"]);
			DataSet dataByEntry = UnitOfWork.ContextAction.GetDataByEntry(Function.ParseInt(byName.Rows[0]["ActionEntry"]), "PublishService", Function.ToString(byName.Rows[0]["AbsEntry"]));
			if (!dataByEntry.IsNotNull())
			{
				string message4 = $"Action not found {name}/{method}";
				return GetResponseMessage(new ResponseMessage
				{
					Code = 4,
					Message = message4
				}, HttpStatusCode.NotFound);
			}
			string userName = SessionUserInfo.UserName;
			DataTable userLogin = UnitOfWork.Users.GetUserLogin(userName);
			if (!userLogin.IsNotNull())
			{
				string message5 = $"INVALID USER {name}/{method}";
				return GetResponseMessage(new ResponseMessage
				{
					Code = 5,
					Message = message5
				}, HttpStatusCode.Unauthorized);
			}
			int num = Function.ParseInt(userLogin.Rows[0]["USERID"]);
			IContextActionHandle actionHandle = ContextActionHandleFactory.GetActionHandle();
			DataTable dataTable2 = dataByEntry.Tables[0];
			if (!dataTable2.IsNotNull())
			{
				string message6 = $"Thông tin cấu hình không đúng - Action - Header {name}/{method}";
				return GetResponseMessage(new ResponseMessage
				{
					Code = 6,
					Message = message6
				}, HttpStatusCode.NotFound);
			}
			DataTable jsonStructureById = UnitOfWork.PublishService.GetJsonStructureById(Function.ParseInt(byName.Rows[0]["AbsEntry"]));
			if (jsonStructureById.IsNotNull())
			{
				DataRow dataRow = jsonStructureById.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["FatherNum"]) == -1 && !Function.ToString(t["JsonType"]).Equals("F"));
				if (dataRow != null)
				{
					DataTable dataTable3 = new DataTable();
					GetDataTableStructureFromObjectConfig(dataTable3, jsonStructureById, Function.ParseInt(dataRow["ConfigID"]), "");
					for (int num2 = 0; num2 < dataTable.Rows.Count; num2++)
					{
						dataTable3.ImportRow(dataTable.Rows[num2]);
					}
					dataTable = dataTable3;
				}
			}
			Enums.ContextActionActionTypeEx contextActionActionTypeEx = (Enums.ContextActionActionTypeEx)Function.ParseInt(dataTable2.Rows[0]["ActionTypeEx"]);
			Enums.ContextActionSourceType contextActionSourceType = (Enums.ContextActionSourceType)(Function.ParseChar(dataTable2.Rows[0]["SourceType"]) ?? 'N');
			char? c = Function.ParseChar(dataTable2.Rows[0]["DataRange"]);
			if (c.HasValue)
			{
				c.GetValueOrDefault();
			}
			c = Function.ParseChar(dataTable2.Rows[0]["Method"]);
			if (c.HasValue)
			{
				c.GetValueOrDefault();
			}
			Enums.ContextActionDataFrom contextActionDataFrom = (Enums.ContextActionDataFrom)(Function.ParseChar(dataTable2.Rows[0]["DataFrom"]) ?? 'T');
			string text = Function.ToString(dataTable2.Rows[0]["DataString"]);
			Function.ToString(dataTable2.Rows[0]["ObjectId"]);
			DataTable dataTable4 = new DataTable();
			switch (contextActionSourceType)
			{
			case Enums.ContextActionSourceType.Database:
			{
				ContextActionQuery query = actionHandle.GetQuery(dataByEntry, dataTable);
				string text2 = "";
				switch (contextActionDataFrom)
				{
				case Enums.ContextActionDataFrom.DefinedQuery:
				{
					int queryId = Function.ParseInt(text);
					DataTable queryById = UnitOfWork.Query.GetQueryById(queryId);
					if (queryById.IsNotNull())
					{
						text2 = Function.ToString(queryById.Rows[0]["QString"]);
					}
					break;
				}
				case Enums.ContextActionDataFrom.QueryText:
					text2 = text;
					break;
				}
				if (string.IsNullOrEmpty(text2))
				{
					string message7 = $"Could not get source data - Data String is Empty {name}/{method}";
					return GetResponseMessage(new ResponseMessage
					{
						Code = 7,
						Message = message7
					});
				}
				text2 = Regex.Replace(text2, "@CURRENTUSER", $"N'{userName}'", RegexOptions.IgnoreCase);
				text2 = Regex.Replace(text2, "$[USER]", $"N'{num}'", RegexOptions.IgnoreCase);
				DataSet dataSet;
				if (query.DataParamType == "T")
				{
					string tableName = query.TemporaryTable.TableName;
					query.TemporaryTable.TableName = (string.IsNullOrEmpty(tableName) ? "tbltemp" : tableName);
					dataSet = UnitOfWork.Commons.ExecuteTemporyTable(text2, query.TemporaryTable);
				}
				else
				{
					dataSet = UnitOfWork.Commons.ExecuteSql(text2, query.Paramaters);
				}
				if (dataSet != null && dataSet.Tables.Count > 1)
				{
					DataTable dataTable5 = dataSet.Tables["Message"];
					if (dataTable5.IsNotNull())
					{
						string text3 = Function.ToString(dataTable5.Rows[0]["MsgType"]);
						string text4 = Function.ToString(dataTable5.Rows[0]["Message"]);
						if (text3 == "5xx")
						{
							text4 += $"{name}/{method}";
							return GetMessageError(text4);
						}
					}
					dataTable4 = dataSet.Tables[1];
					break;
				}
				string message8 = $"Could not get source data {name}/{method}";
				return GetResponseMessage(new ResponseMessage
				{
					Code = 7,
					Message = message8
				});
			}
			case Enums.ContextActionSourceType.UI:
				dataTable4 = dataTable;
				break;
			}
			switch (contextActionActionTypeEx)
			{
			case Enums.ContextActionActionTypeEx.PhysicalTable:
			{
				string text9 = Function.ToString(dataTable2.Rows[0]["PhysicalTableName"]);
				string[] array = text9.Split('.');
				if (array.Length > 1)
				{
					string text10 = array[0];
					text9 = array[1];
					using Apzon.Services.IUnitOfWork unitOfWork8 = Apzon.Services.UnitOfWorkFactory.GetUnitOfWork($"Server = {Global.CommonDbServer};CS=\"{text10}\"; UserID = {Global.CommonDbUserName}; Password = {Global.CommonDbPassword}", Enums.DatabaseSystemType.HANA);
					HttpResult httpResult7 = unitOfWork8.ContextAction.SavePhysicalData(text9, dataByEntry, dataTable4);
					if (!string.IsNullOrWhiteSpace(httpResult7.Message))
					{
						string msg2 = string.Format(httpResult7.Message + "{0}/{1}", name, method);
						return GetMessageError(msg2);
					}
					return GetResponseContent(httpResult7.Content);
				}
				HttpResult httpResult8 = UnitOfWork.ContextAction.SavePhysicalData(text9, dataByEntry, dataTable4);
				if (!string.IsNullOrWhiteSpace(httpResult8.Message))
				{
					string msg3 = string.Format(httpResult8.Message + "{0}/{1}", name, method);
					return GetMessageError(msg3);
				}
				return GetResponseContent(httpResult8.Content);
			}
			case Enums.ContextActionActionTypeEx.Task:
			{
				DataTable dtStruct = UnitOfWork.SubActivities.GetStruct();
				DataSet sourceTask = actionHandle.GetSourceTask(dataByEntry, dataTable4, dtStruct);
				if (sourceTask.IsNotNull())
				{
					sourceTask.Tables[0].Rows[0]["AssignedBy"] = num;
					HttpResult httpResult5 = UnitOfWork.SubActivities.AddTask(sourceTask);
					if (!string.IsNullOrWhiteSpace(httpResult5.Message))
					{
						string msg = string.Format(httpResult5.Message + "{0}/{1}", name, method);
						return GetMessageError(msg);
					}
					object responseObjectForIncommingService = GetResponseObjectForIncommingService(httpResult5.Content, httpResult5.Content, outputType, outputQuery);
					return GetResponseContent(responseObjectForIncommingService);
				}
				string message29 = $"INVALID DATA {name}/{method}";
				return GetResponseMessage(new ResponseMessage
				{
					Code = 8,
					Message = message29
				});
			}
			case Enums.ContextActionActionTypeEx.MaketingDocument:
			{
				DataTable document = UnitOfWork.Documents.Search(new DocumentsImpl
				{
					DocEntry = -1
				}, ApzObjectTypes.oCreditNotes);
				DataTable dataTable7 = UnitOfWork.Documents.TableLine(-1, ApzObjectTypes.oCreditNotes);
				if (dataTable7.ExistsColumn("LineNum"))
				{
					dataTable7.Columns["LineNum"].DefaultValue = -1;
				}
				if (!dataTable7.ExistsColumn("SpecifiedPriceBefDi"))
				{
					dataTable7.Columns.Add("SpecifiedPriceBefDi", typeof(bool));
				}
				DataTable batchserial = UnitOfWork.Documents.LoadBatchNumberStruct(ApzObjectTypes.oOrders);
				actionHandle = ContextActionHandleFactory.GetActionHandle(document, dataTable7, batchserial);
				if (!DocumentsHelper.BatchSerialNumbersStruct.IsNotNull())
				{
					DocumentsHelper.BatchSerialNumbersStruct = UnitOfWork.Documents.LoadBatchNumberStruct(ApzObjectTypes.oOrders);
				}
				List<DocumentsImpl> documents = actionHandle.GetMappingDocument(dataTable4, dataByEntry);
				Enums.ContextActionDocumentType objtype = (Enums.ContextActionDocumentType)Function.ParseInt(dataTable2.Rows[0]["ObjType"]);
				ApzObjectTypes objtype2 = actionHandle.GetObjectTypesFromContext(objtype);
				if (documents.Count > 0)
				{
					if (!Global.LoginSap)
					{
						break;
					}
					Apzon.DataServices.IUnitOfWork unitOfWork4 = BrokerService.GetUnitOfWork(SessionId);
					if (Global.SessionUserInfo != null && Global.SessionUserInfo.Count >= 0)
					{
						if (string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
						{
							string message23 = $"INVALID SESSION ID{name}/{method}";
							return GetResponseMessage(new ResponseMessage
							{
								Code = 2,
								Message = message23
							});
						}
						List<ResponseMessage> results = new List<ResponseMessage>();
						foreach (DocumentsImpl item in documents)
						{
							HttpResult httpResult4 = await unitOfWork4.Documents.SaveDocument(item, SessionUserInfo.SessionDiServer, objtype2);
							ResponseMessage responseMessage6 = new ResponseMessage
							{
								Code = ((!string.IsNullOrEmpty(httpResult4.Message)) ? 1 : 200),
								Message = httpResult4.Message,
								Content = httpResult4.Content
							};
							if (responseMessage6.Code == 200)
							{
								responseMessage6.Content = GetResponseObjectForIncommingService(httpResult4.Content, httpResult4.Content, outputType, outputQuery);
							}
							if (documents.Count == 1)
							{
								return GetResponseObject(responseMessage6);
							}
							results.Add(responseMessage6);
						}
						HttpResponseMessage httpResponseMessage6 = Request.CreateResponse(HttpStatusCode.OK);
						httpResponseMessage6.Content = new StringContent(JsonConvert.SerializeObject(results), Encoding.UTF8, "application/json");
						return httpResponseMessage6;
					}
					string message24 = $"INVALID SESSION ID{name}/{method}";
					return GetResponseMessage(new ResponseMessage
					{
						Code = 2,
						Message = message24
					});
				}
				string message25 = $"Could not get documents data {name}/{method}";
				return GetResponseMessage(new ResponseMessage
				{
					Code = 9,
					Message = message25
				});
			}
			case Enums.ContextActionActionTypeEx.Items:
			{
				DataTable aDoc = UnitOfWork.Items.GetADoc("");
				DataTable price = UnitOfWork.Items.GetPrice("");
				DataTable itemWarehouse = UnitOfWork.Items.GetItemWarehouse("");
				actionHandle = ContextActionHandleFactory.GetActionHandle(Enums.ContextActionActionTypeEx.Items, aDoc, price, itemWarehouse);
				List<ItemsImpl> mappingItem = actionHandle.GetMappingItem(dataTable4, dataByEntry);
				if (mappingItem != null && mappingItem.Count > 0)
				{
					if (!Global.LoginSap)
					{
						break;
					}
					Apzon.DataServices.IUnitOfWork unitOfWork5 = BrokerService.GetUnitOfWork(SessionId);
					if (Global.SessionUserInfo != null && Global.SessionUserInfo.Count >= 0)
					{
						if (string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
						{
							string message20 = $"INVALID SESSION ID{name}/{method}";
							return GetResponseMessage(new ResponseMessage
							{
								Code = 2,
								Message = message20
							});
						}
						List<ResponseMessage> list4 = new List<ResponseMessage>();
						foreach (ItemsImpl item2 in mappingItem)
						{
							item2.APZ_SAVE_ACTION = "S";
							string text7 = unitOfWork5.Items.Save(item2, SessionUserInfo.SessionDiServer, ignoreEmptyValue: true);
							ResponseMessage responseMessage5 = new ResponseMessage
							{
								Code = ((!string.IsNullOrEmpty(text7)) ? 1 : 200),
								Message = text7
							};
							if (responseMessage5.Code == 200)
							{
								responseMessage5.Content = GetResponseObjectForIncommingService(item2.ItemCode, item2.ItemCode, outputType, outputQuery);
							}
							if (mappingItem.Count == 1)
							{
								return GetResponseObject(responseMessage5);
							}
							list4.Add(responseMessage5);
						}
						HttpResponseMessage httpResponseMessage5 = Request.CreateResponse(HttpStatusCode.OK);
						httpResponseMessage5.Content = new StringContent(JsonConvert.SerializeObject(list4), Encoding.UTF8, "application/json");
						return httpResponseMessage5;
					}
					string message21 = $"INVALID SESSION ID{name}/{method}";
					return GetResponseMessage(new ResponseMessage
					{
						Code = 2,
						Message = message21
					});
				}
				string message22 = $"Could not get items data {name}/{method}";
				return GetResponseMessage(new ResponseMessage
				{
					Code = 9,
					Message = message22
				});
			}
			case Enums.ContextActionActionTypeEx.BusinessBusinessPartner:
			{
				DataTable aDoc2 = UnitOfWork.BusinessMasterData.GetADoc("", "");
				DataTable addressByCardCode = UnitOfWork.BusinessMasterData.GetAddressByCardCode("");
				DataTable dtItemWarehouses = UnitOfWork.ContactPerson.ShowContactPerson("");
				actionHandle = ContextActionHandleFactory.GetActionHandle(Enums.ContextActionActionTypeEx.BusinessBusinessPartner, aDoc2, addressByCardCode, dtItemWarehouses);
				List<BPImpls> mappingBusinesPartner = actionHandle.GetMappingBusinesPartner(dataTable4, dataByEntry);
				if (mappingBusinesPartner != null && mappingBusinesPartner.Count > 0)
				{
					if (!Global.LoginSap)
					{
						break;
					}
					Apzon.DataServices.IUnitOfWork unitOfWork9 = BrokerService.GetUnitOfWork(SessionId);
					if (Global.SessionUserInfo != null && Global.SessionUserInfo.Count >= 0)
					{
						if (string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
						{
							string message33 = $"INVALID SESSION ID{name}/{method}";
							return GetResponseMessage(new ResponseMessage
							{
								Code = 2,
								Message = message33
							});
						}
						List<ResponseMessage> list6 = new List<ResponseMessage>();
						foreach (BPImpls item3 in mappingBusinesPartner)
						{
							item3.OldCardCode = item3.CardCode;
							HttpResult httpResult9 = unitOfWork9.BusinessPartners.Save(item3, SessionUserInfo.SessionDiServer);
							ResponseMessage responseMessage9 = new ResponseMessage
							{
								Code = ((!string.IsNullOrEmpty(httpResult9.Message)) ? 1 : 200),
								Message = httpResult9.Message,
								Content = httpResult9.Content
							};
							if (responseMessage9.Code == 200)
							{
								responseMessage9.Content = GetResponseObjectForIncommingService(httpResult9.Content, httpResult9.Content, outputType, outputQuery);
							}
							if (mappingBusinesPartner.Count == 1)
							{
								return GetResponseObject(responseMessage9);
							}
							list6.Add(responseMessage9);
						}
						HttpResponseMessage httpResponseMessage9 = Request.CreateResponse(HttpStatusCode.OK);
						httpResponseMessage9.Content = new StringContent(JsonConvert.SerializeObject(list6), Encoding.UTF8, "application/json");
						return httpResponseMessage9;
					}
					string message34 = $"INVALID SESSION ID{name}/{method}";
					return GetResponseMessage(new ResponseMessage
					{
						Code = 2,
						Message = message34
					});
				}
				string message35 = $"Could not get bussiness partner data {name}/{method}";
				return GetResponseMessage(new ResponseMessage
				{
					Code = 9,
					Message = message35
				});
			}
			case Enums.ContextActionActionTypeEx.ChartOfAccount:
			{
				DataTable dtHeader = UnitOfWork.ChartOfAccount.GetStruct();
				actionHandle = ContextActionHandleFactory.GetActionHandle(Enums.ContextActionActionTypeEx.ChartOfAccount, dtHeader);
				List<ChartOfAccountsImpl> mappingChartOfAccount = actionHandle.GetMappingChartOfAccount(dataTable4, dataByEntry);
				if (mappingChartOfAccount != null && mappingChartOfAccount.Count > 0)
				{
					if (!Global.LoginSap)
					{
						break;
					}
					Apzon.DataServices.IUnitOfWork unitOfWork3 = BrokerService.GetUnitOfWork(SessionId);
					if (Global.SessionUserInfo != null && Global.SessionUserInfo.Count >= 0)
					{
						if (string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
						{
							string message14 = $"INVALID SESSION ID{name}/{method}";
							return GetResponseMessage(new ResponseMessage
							{
								Code = 2,
								Message = message14
							});
						}
						List<ResponseMessage> list3 = new List<ResponseMessage>();
						foreach (ChartOfAccountsImpl item4 in mappingChartOfAccount)
						{
							HttpResult httpResult3 = unitOfWork3.ChartOfAccount.Save(new List<ChartOfAccountsImpl> { item4 }, SessionUserInfo.SessionDiServer);
							ResponseMessage responseMessage3 = new ResponseMessage
							{
								Code = ((!string.IsNullOrEmpty(httpResult3.Message)) ? 1 : 200),
								Message = httpResult3.Message,
								Content = httpResult3.Content
							};
							if (responseMessage3.Code == 200)
							{
								responseMessage3.Content = GetResponseObjectForIncommingService(httpResult3.Content, httpResult3.Content, outputType, outputQuery);
							}
							if (mappingChartOfAccount.Count == 1)
							{
								return GetResponseObject(responseMessage3);
							}
							list3.Add(responseMessage3);
						}
						HttpResponseMessage httpResponseMessage3 = Request.CreateResponse(HttpStatusCode.OK);
						httpResponseMessage3.Content = new StringContent(JsonConvert.SerializeObject(list3), Encoding.UTF8, "application/json");
						return httpResponseMessage3;
					}
					string message15 = $"INVALID SESSION ID{name}/{method}";
					return GetResponseMessage(new ResponseMessage
					{
						Code = 2,
						Message = message15
					});
				}
				string message16 = $"Could not get chart of account data {name}/{method}";
				return GetResponseMessage(new ResponseMessage
				{
					Code = 9,
					Message = message16
				});
			}
			case Enums.ContextActionActionTypeEx.DistributionRule:
			{
				DataTable dtHeader2 = UnitOfWork.DistributeRule.GetStruct();
				DataTable structRow = UnitOfWork.DistributeRule.GetStructRow();
				actionHandle = ContextActionHandleFactory.GetActionHandle(Enums.ContextActionActionTypeEx.DistributionRule, dtHeader2, structRow);
				List<DataSet> distributerule = actionHandle.GetMappingDistributeRule(dataTable4, dataByEntry);
				if (distributerule != null && distributerule.Count > 0)
				{
					if (!Global.LoginSap)
					{
						break;
					}
					Apzon.DataServices.IUnitOfWork unitOfWork4 = BrokerService.GetUnitOfWork(SessionId);
					if (Global.SessionUserInfo != null && Global.SessionUserInfo.Count >= 0)
					{
						if (string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
						{
							string message17 = $"INVALID SESSION ID{name}/{method}";
							return GetResponseMessage(new ResponseMessage
							{
								Code = 2,
								Message = message17
							});
						}
						List<ResponseMessage> results = new List<ResponseMessage>();
						foreach (DataSet bp in distributerule)
						{
							string text6 = await unitOfWork4.DistributionRule.Save(SessionUserInfo.SessionDiServer, bp);
							ResponseMessage responseMessage4 = new ResponseMessage
							{
								Code = ((!string.IsNullOrEmpty(text6)) ? 1 : 200),
								Message = text6
							};
							if (responseMessage4.Code == 200)
							{
								responseMessage4.Content = GetResponseObjectForIncommingService(bp.Tables[0].Rows[0]["OcrCode"], bp.Tables[0].Rows[0]["OcrCode"], outputType, outputQuery);
							}
							if (distributerule.Count == 1)
							{
								return GetResponseObject(responseMessage4);
							}
							results.Add(responseMessage4);
						}
						HttpResponseMessage httpResponseMessage4 = Request.CreateResponse(HttpStatusCode.OK);
						httpResponseMessage4.Content = new StringContent(JsonConvert.SerializeObject(results), Encoding.UTF8, "application/json");
						return httpResponseMessage4;
					}
					string message18 = $"INVALID SESSION ID{name}/{method}";
					return GetResponseMessage(new ResponseMessage
					{
						Code = 2,
						Message = message18
					});
				}
				string message19 = $"Could not get distribute rule data {name}/{method}";
				return GetResponseMessage(new ResponseMessage
				{
					Code = 9,
					Message = message19
				});
			}
			case Enums.ContextActionActionTypeEx.VatGroup:
			{
				DataTable structHeader = UnitOfWork.VatGroups.GetStructHeader();
				DataTable structLines = UnitOfWork.VatGroups.GetStructLines();
				actionHandle = ContextActionHandleFactory.GetActionHandle(Enums.ContextActionActionTypeEx.VatGroup, structHeader, structLines);
				List<VatGroupsImpl> mappingVatGroups = actionHandle.GetMappingVatGroups(dataTable4, dataByEntry);
				if (mappingVatGroups != null && mappingVatGroups.Count > 0)
				{
					if (!Global.LoginSap)
					{
						break;
					}
					Apzon.DataServices.IUnitOfWork unitOfWork6 = BrokerService.GetUnitOfWork(SessionId);
					if (Global.SessionUserInfo != null && Global.SessionUserInfo.Count >= 0)
					{
						if (string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
						{
							string message26 = $"INVALID SESSION ID{name}/{method}";
							return GetResponseMessage(new ResponseMessage
							{
								Code = 2,
								Message = message26
							});
						}
						List<ResponseMessage> list5 = new List<ResponseMessage>();
						foreach (VatGroupsImpl item5 in mappingVatGroups)
						{
							string text8 = unitOfWork6.VatGroups.Save(new List<VatGroupsImpl> { item5 }, SessionUserInfo.SessionDiServer);
							ResponseMessage responseMessage7 = new ResponseMessage
							{
								Code = ((!string.IsNullOrEmpty(text8)) ? 1 : 200),
								Message = text8,
								Content = item5.Code
							};
							if (responseMessage7.Code == 200)
							{
								responseMessage7.Content = GetResponseObjectForIncommingService(item5.Code, item5.Code, outputType, outputQuery);
							}
							if (mappingVatGroups.Count == 1)
							{
								return GetResponseObject(responseMessage7);
							}
							list5.Add(responseMessage7);
						}
						HttpResponseMessage httpResponseMessage7 = Request.CreateResponse(HttpStatusCode.OK);
						httpResponseMessage7.Content = new StringContent(JsonConvert.SerializeObject(list5), Encoding.UTF8, "application/json");
						return httpResponseMessage7;
					}
					string message27 = $"INVALID SESSION ID{name}/{method}";
					return GetResponseMessage(new ResponseMessage
					{
						Code = 2,
						Message = message27
					});
				}
				string message28 = $"Could not get vat groups data {name}/{method}";
				return GetResponseMessage(new ResponseMessage
				{
					Code = 9,
					Message = message28
				});
			}
			case Enums.ContextActionActionTypeEx.SalesPersons:
			{
				DataTable structure = UnitOfWork.SalesPersons.GetStructure();
				actionHandle = ContextActionHandleFactory.GetActionHandle();
				if (dataByEntry == null || dataByEntry.Tables.Count < 2)
				{
					return GetResponseMessage(new ResponseMessage
					{
						Code = 6,
						Message = $"Can not get config data {name}/{method}"
					});
				}
				DataTable dt = actionHandle.GetmappingSourceSaleEmployee(structure, dataTable4, dataByEntry.Tables[2]);
				if (dt.IsNotNull())
				{
					if (Global.LoginSap)
					{
						Apzon.DataServices.IUnitOfWork unitOfWork7 = BrokerService.GetUnitOfWork(SessionId);
						if (Global.SessionUserInfo != null && Global.SessionUserInfo.Count >= 0)
						{
							if (string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
							{
								string message30 = $"INVALID SESSION ID{name}/{method}";
								return GetResponseMessage(new ResponseMessage
								{
									Code = 2,
									Message = message30
								});
							}
							HttpResult httpResult6 = unitOfWork7.SalesPersons.AddDoc(SessionUserInfo.SessionDiServer, dt);
							ResponseMessage responseMessage8 = new ResponseMessage
							{
								Code = ((!string.IsNullOrEmpty(httpResult6.Message)) ? 1 : 200),
								Message = httpResult6.Message,
								Content = httpResult6.Content
							};
							if (responseMessage8.Code == 200)
							{
								responseMessage8.Content = GetResponseObjectForIncommingService(httpResult6.Content, httpResult6.Content, outputType, outputQuery);
							}
							HttpResponseMessage httpResponseMessage8 = Request.CreateResponse(HttpStatusCode.OK);
							httpResponseMessage8.Content = new StringContent(JsonConvert.SerializeObject(responseMessage8), Encoding.UTF8, "application/json");
							return httpResponseMessage8;
						}
						string message31 = $"INVALID SESSION ID{name}/{method}";
						return GetResponseMessage(new ResponseMessage
						{
							Code = 2,
							Message = message31
						});
					}
					break;
				}
				string message32 = $"Could not get vat groups data {name}/{method}";
				return GetResponseMessage(new ResponseMessage
				{
					Code = 9,
					Message = message32
				});
			}
			case Enums.ContextActionActionTypeEx.UserDefinedObject:
			{
				string code = Function.ToString(dataTable2.Rows[0]["UdoCode"]);
				DataTable udoByCode = UnitOfWork.UDOs.GetUdoByCode(new Common.Table_Code
				{
					Name = "OUDO",
					Code = code
				});
				actionHandle = ContextActionHandleFactory.GetActionHandle(Enums.ContextActionActionTypeEx.UserDefinedObject, udoByCode);
				List<DataSet> mappingUserDefinedObject = actionHandle.GetMappingUserDefinedObject(dataTable4, dataByEntry);
				if (mappingUserDefinedObject.Count <= 0)
				{
					break;
				}
				Apzon.DataServices.IUnitOfWork unitOfWork10 = BrokerService.GetUnitOfWork(SessionId);
				if (Global.SessionUserInfo != null && Global.SessionUserInfo.Count >= 0)
				{
					if (string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
					{
						string message36 = $"INVALID SESSION ID{name}/{method}";
						return GetResponseMessage(new ResponseMessage
						{
							Code = 2,
							Message = message36
						});
					}
					List<ResponseMessage> list7 = new List<ResponseMessage>();
					foreach (DataSet item6 in mappingUserDefinedObject)
					{
						HttpResult httpResult10 = unitOfWork10.UDOs.Save(item6, SessionUserInfo.SessionDiServer);
						ResponseMessage responseMessage10 = new ResponseMessage
						{
							Code = ((!string.IsNullOrEmpty(httpResult10.Message)) ? 1 : 200),
							Message = httpResult10.Message,
							Content = httpResult10.Content
						};
						if (responseMessage10.Code == 200)
						{
							responseMessage10.Content = GetResponseObjectForIncommingService(httpResult10.Content, httpResult10.Content, outputType, outputQuery);
						}
						if (mappingUserDefinedObject.Count == 1)
						{
							return GetResponseObject(responseMessage10);
						}
						list7.Add(responseMessage10);
					}
					HttpResponseMessage httpResponseMessage10 = Request.CreateResponse(HttpStatusCode.OK);
					httpResponseMessage10.Content = new StringContent(JsonConvert.SerializeObject(list7), Encoding.UTF8, "application/json");
					return httpResponseMessage10;
				}
				string message37 = $"INVALID SESSION ID{name}/{method}";
				return GetResponseMessage(new ResponseMessage
				{
					Code = 2,
					Message = message37
				});
			}
			case Enums.ContextActionActionTypeEx.DoQuery:
			{
				Enums.ContextActionReportFrom contextActionReportFrom = (Enums.ContextActionReportFrom)(Function.ParseChar(dataTable2.Rows[0]["QueryFrom"]) ?? 'Q');
				DataTable dataTable6 = dataByEntry.Tables[1];
				if (contextActionReportFrom != Enums.ContextActionReportFrom.DefinedQuery && contextActionReportFrom != Enums.ContextActionReportFrom.QueryText)
				{
					break;
				}
				if (!string.IsNullOrWhiteSpace(Function.ToString(dataTable2.Rows[0]["TempTblNameDoQuery"])))
				{
					ContextActionDoQuery contextActionDoQuery = new ContextActionDoQuery();
					if (!dataTable4.IsNotNull())
					{
						return GetResponseMessage(new ResponseMessage
						{
							Code = 8,
							Message = "Could not get input data."
						}, HttpStatusCode.BadRequest);
					}
					if (string.IsNullOrWhiteSpace(Function.ToString(dataTable2.Rows[0]["QueryString"])))
					{
						return GetResponseMessage(new ResponseMessage
						{
							Code = 1,
							Message = "Could not get query."
						}, HttpStatusCode.InternalServerError);
					}
					contextActionDoQuery.TempTableData = dataTable4;
					contextActionDoQuery.TempTableName = Function.ToString(dataTable2.Rows[0]["TempTblNameDoQuery"]);
					contextActionDoQuery.QueryText = Function.ToString(dataTable2.Rows[0]["QueryString"]);
					Apzon.DataServices.IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
					if (Global.SessionUserInfo != null && Global.SessionUserInfo.Count >= 0)
					{
						if (string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
						{
							string message9 = $"INVALID SESSION ID{name}/{method}";
							return GetResponseMessage(new ResponseMessage
							{
								Code = 2,
								Message = message9
							}, HttpStatusCode.Unauthorized);
						}
						HttpResult httpResult = unitOfWork.Common.DoQueryWithTempTable(contextActionDoQuery, SessionUserInfo.SessionDiServer, SessionUserInfo.UserName);
						ResponseMessage responseMessage = new ResponseMessage
						{
							Code = ((!string.IsNullOrEmpty(httpResult.Message)) ? 1 : 200),
							Message = httpResult.Message,
							Content = httpResult.Content
						};
						if (responseMessage.Code == 200)
						{
							responseMessage.Content = GetResponseObjectForIncommingService(httpResult.Content, httpResult.Content, outputType, outputQuery);
						}
						HttpResponseMessage httpResponseMessage = Request.CreateResponse((responseMessage.Code == 200) ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
						httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject(responseMessage), Encoding.UTF8, "application/json");
						return httpResponseMessage;
					}
					string message10 = $"INVALID SESSION ID{name}/{method}";
					return GetResponseMessage(new ResponseMessage
					{
						Code = 2,
						Message = message10
					}, HttpStatusCode.Unauthorized);
				}
				if (!dataTable6.IsNotNull())
				{
					break;
				}
				List<ContextActionDoQuery> list = new List<ContextActionDoQuery>();
				EnumerableRowCollection<DataRow> enumerableRowCollection = from t in dataTable6.AsEnumerable()
					where Function.ToString(t["ParamType"]) == "Q"
					select t;
				foreach (DataRow row in dataTable4.DefaultView.ToTable(distinct: true, enumerableRowCollection.Select((DataRow t) => Function.ToString(t["SourceField"])).Distinct().ToArray()).Rows)
				{
					ContextActionDoQuery contextActionDoQuery2 = new ContextActionDoQuery();
					contextActionDoQuery2.QueryText = Function.ToString(dataTable2.Rows[0]["QueryString"]);
					List<QueryParamater> list2 = new List<QueryParamater>();
					foreach (DataRow item7 in enumerableRowCollection)
					{
						QueryParamater queryParamater = new QueryParamater();
						queryParamater.ParaName = Function.ToString(item7["ParamName"]);
						string text5 = Function.ToString(item7["SourceField"]);
						if (!string.IsNullOrEmpty(text5))
						{
							object value = ((row[text5] == DBNull.Value) ? Function.ToString(item7["DefaultValue"]) : row[text5]);
							queryParamater.Value = value;
						}
						list2.Add(queryParamater);
					}
					contextActionDoQuery2.Paramaters = list2;
					list.Add(contextActionDoQuery2);
				}
				if (list.Count > 0)
				{
					Apzon.DataServices.IUnitOfWork unitOfWork2 = BrokerService.GetUnitOfWork(SessionId);
					if (Global.SessionUserInfo != null && Global.SessionUserInfo.Count >= 0)
					{
						if (string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
						{
							string message11 = $"INVALID SESSION ID{name}/{method}";
							return GetResponseMessage(new ResponseMessage
							{
								Code = 2,
								Message = message11
							}, HttpStatusCode.Unauthorized);
						}
						HttpResult httpResult2 = unitOfWork2.Common.DoQuery(list, SessionUserInfo.SessionDiServer, SessionUserInfo.UserName);
						ResponseMessage responseMessage2 = new ResponseMessage
						{
							Code = ((!string.IsNullOrEmpty(httpResult2.Message)) ? 1 : 200),
							Message = httpResult2.Message,
							Content = httpResult2.Content
						};
						if (responseMessage2.Code == 200)
						{
							responseMessage2.Content = GetResponseObjectForIncommingService(httpResult2.Content, httpResult2.Content, outputType, outputQuery);
						}
						HttpResponseMessage httpResponseMessage2 = Request.CreateResponse((responseMessage2.Code == 200) ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
						httpResponseMessage2.Content = new StringContent(JsonConvert.SerializeObject(responseMessage2), Encoding.UTF8, "application/json");
						return httpResponseMessage2;
					}
					string message12 = $"INVALID SESSION ID{name}/{method}";
					return GetResponseMessage(new ResponseMessage
					{
						Code = 2,
						Message = message12
					}, HttpStatusCode.Unauthorized);
				}
				string message13 = "Could not get query info.";
				return GetResponseMessage(new ResponseMessage
				{
					Code = 1,
					Message = message13
				}, HttpStatusCode.InternalServerError);
			}
			}
			return new HttpResponseMessage(HttpStatusCode.NoContent);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			string error = "An unexpected error occurred.";
			if (ex is DbException)
			{
				string text11 = Apzon.Services.UnitOfWorkFactory.GetUnitOfWork(string.Empty, Global.DatabaseSystemType).ConvertErrorMessage(error);
				if (!string.IsNullOrEmpty(text11))
				{
					var value2 = new
					{
						Code = -99,
						Message = text11
					};
					return new HttpResponseMessage(HttpStatusCode.BadRequest)
					{
						Content = new StringContent(JsonConvert.SerializeObject(value2), Encoding.UTF8, "application/json")
					};
				}
			}
			return new HttpResponseMessage(HttpStatusCode.InternalServerError);
		}
	}

	private void GetDataTableStructureFromObjectConfig(DataTable source, DataTable configObj, int fatherId, string fatherName)
	{
		EnumerableRowCollection<DataRow> enumerableRowCollection = from t in configObj.AsEnumerable()
			where Function.ParseInt(t["FatherNum"]) == fatherId
			select t;
		if (!enumerableRowCollection.Any())
		{
			return;
		}
		foreach (DataRow item in enumerableRowCollection)
		{
			string text = (string.IsNullOrEmpty(fatherName) ? Function.ToString(item["Name"]) : string.Format("{0}.{1}", fatherName, Function.ToString(item["Name"])));
			if (Function.ToString(item["JsonType"]).Equals("F"))
			{
				if (!source.ExistsColumn(text))
				{
					switch ((Enums.DisplayFormatType)Function.ParseInt(item["DataType"]))
					{
					case Enums.DisplayFormatType.Qty:
					case Enums.DisplayFormatType.Price:
					case Enums.DisplayFormatType.Amount:
					case Enums.DisplayFormatType.Rate:
					case Enums.DisplayFormatType.Percent:
					case Enums.DisplayFormatType.Unit:
						source.Columns.Add(text, typeof(double));
						break;
					case Enums.DisplayFormatType.DateTime:
						source.Columns.Add(text, typeof(DateTime));
						break;
					case Enums.DisplayFormatType.Integer:
						source.Columns.Add(text, typeof(int));
						break;
					default:
						source.Columns.Add(text, typeof(string));
						break;
					}
					if (Function.ToString(item["DfltValue"]) != "")
					{
						source.Columns[text].DefaultValue = item["DfltValue"];
					}
				}
			}
			else
			{
				GetDataTableStructureFromObjectConfig(source, configObj, Function.ParseInt(item["ConfigID"]), text);
			}
		}
	}

	private object GetResponseObjectForIncommingService(object content, object objCode, string outputType, string outputQuery)
	{
		if (!string.IsNullOrEmpty(outputQuery))
		{
			try
			{
				DataTable dataTable = UnitOfWork.Commons.ExecuteDataTable(outputQuery, CommandType.Text, new SqlParameter("@objCode", objCode));
				if (!dataTable.IsNotNull())
				{
					return null;
				}
				if (!(outputType == "F"))
				{
					if (outputType == "O")
					{
						DataRow dataRow = dataTable.Rows[0];
						dynamic val = new ExpandoObject();
						for (int i = 0; i < dataTable.Columns.Count; i++)
						{
							string columnName = dataTable.Columns[i].ColumnName;
							((IDictionary<string, object>)val).Add(columnName, dataRow[columnName]);
						}
						return val;
					}
					return dataTable;
				}
				return dataTable.Rows[0][0];
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			}
			return null;
		}
		return content;
	}

	private HttpResponseMessage GetResponseObject(ResponseMessage msg)
	{
		HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);
		httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject(msg), Encoding.UTF8, "application/json");
		return httpResponseMessage;
	}

	[HttpPost]
	[ActionName("1")]
	public HttpResponseMessage HandleDataSet(string name, string method)
	{
		try
		{
			HttpUtility.ParseQueryString(Request.RequestUri.Query);
			JsonConvert.DeserializeObject<DataSet>(Request.Content.ReadAsStringAsync().Result);
			return new HttpResponseMessage(HttpStatusCode.NoContent);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new HttpResponseMessage(HttpStatusCode.InternalServerError);
		}
	}

	private HttpResponseMessage GetMessageError(string msg)
	{
		ResponseMessage responseMessage = new ResponseMessage();
		responseMessage.Code = 1;
		responseMessage.Message = msg;
		Logging.Write("TRACE", msg);
		HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);
		httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject(responseMessage), Encoding.UTF8, "application/json");
		return httpResponseMessage;
	}

	private HttpResponseMessage GetResponseContent(object obj)
	{
		ResponseMessage responseMessage = new ResponseMessage();
		responseMessage.Code = 200;
		responseMessage.Content = obj;
		HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);
		httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject(responseMessage), Encoding.UTF8, "application/json");
		return httpResponseMessage;
	}

	private HttpResponseMessage GetResponseMessage(ResponseMessage res, HttpStatusCode statusCode = HttpStatusCode.OK)
	{
		HttpResponseMessage httpResponseMessage = Request.CreateResponse(statusCode);
		httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject(res), Encoding.UTF8, "application/json");
		return httpResponseMessage;
	}
}
