using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Entities.Models;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.DMS;
using Apzon.Entities.Models.MasterData;
using Apzon.Entities.Models.MobileApp;
using Apzon.Entities.Models.UserFields;

namespace Apzon.Api.Controllers.DMS;

[ValidateLoginSapExceptionHandling]
public class VisitStoreController : BaseApiController
{
	[HttpGet]
	public ResponseResult GetListPotentialCustomers(string userCode)
	{
		try
		{
			return UnitOfWork.VisitStore.GetListPotentialCustomers(userCode);
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
	public ResponseResult SyncDataDetailStore([FromBody] DataTable listBP)
	{
		string userName = SessionUserInfo.UserName;
		return UnitOfWork.VisitStore.SyncDataDetailStore(listBP, userName);
	}

	[HttpGet]
	public DataTable GetBusinessTypeDataByCardCode(string cardCode)
	{
		try
		{
			return UnitOfWork.VisitStore.GetBusinessTypeDataByCardCode(cardCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpGet]
	public ResponseResult DMSHomeGetData()
	{
		try
		{
			DataTable content = UnitOfWork.VisitStore.DMSHomeGetData(SessionUserInfo.UserName);
			return new ResponseResult
			{
				Message = "",
				Content = content,
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
	public DataTable GetVisitStoreStep()
	{
		try
		{
			return UnitOfWork.VisitStore.GetVisitStoreStep();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	[HttpGet]
	public DataTable GetListVisitHistory(string fromDate, string toDate, int userSign, string listCardCode, int pageIndex)
	{
		try
		{
			return UnitOfWork.VisitStore.GetListVisitHistory(fromDate, toDate, userSign, listCardCode, pageIndex);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	[HttpGet]
	public ResponseResult GetVisitHistoryDetail(string visitId)
	{
		try
		{
			DataSet visitHistoryDetail = UnitOfWork.VisitStore.GetVisitHistoryDetail(visitId);
			if (visitHistoryDetail.Tables.Count == 7)
			{
				visitHistoryDetail.Tables[0].TableName = "Detail";
				visitHistoryDetail.Tables[1].TableName = "Status";
				visitHistoryDetail.Tables[2].TableName = "Image";
				visitHistoryDetail.Tables[3].TableName = "UpdateInventory";
				visitHistoryDetail.Tables[4].TableName = "Competitor";
				visitHistoryDetail.Tables[5].TableName = "CompetitorProduct";
				visitHistoryDetail.Tables[6].TableName = "Comment";
			}
			return new ResponseResult
			{
				Message = "",
				Content = visitHistoryDetail
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message
			};
		}
	}

	[HttpGet]
	public DataTable SearchDataUpdateInventory(string cardCode, string searchKey)
	{
		try
		{
			return UnitOfWork.VisitStore.SearchDataUpdateInventory(cardCode, searchKey);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	[HttpGet]
	public DataTable GetListCompetitor(string cardCode)
	{
		try
		{
			return UnitOfWork.VisitStore.GetListCompetitor(cardCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	[HttpGet]
	public DataTable GetOrderByVisitId(string orderType, string visitId)
	{
		try
		{
			return UnitOfWork.VisitStore.GetOrderByVisitId(orderType, visitId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	[HttpGet]
	public DataTable GetListCompetitorProductAvailable(string competitorCode)
	{
		try
		{
			return UnitOfWork.VisitStore.GetListCompetitorProductAvailable(competitorCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	[HttpPost]
	public async Task<ResponseResult> FinishVisitStore(FinishVisitStoreImpl data)
	{
		_ = 1;
		try
		{
			if (UnitOfWork == null)
			{
				return new ResponseResult
				{
					Message = "Invalid SesionId",
					MessageCode = "401"
				};
			}
			if (data.DataHeader.IsNotNull() && data.DataHeader.ExistsColumn("StoreLatitude") && data.DataHeader.ExistsColumn("StoreLongitude") && (string.IsNullOrEmpty(Function.ToString(data.DataHeader.Rows[0]["StoreLatitude"])) || string.IsNullOrEmpty(Function.ToString(data.DataHeader.Rows[0]["StoreLongitude"]))))
			{
				string[] array = Function.ToString(data.DataHeader.Rows[0]["GeoLocation"]).Replace(")", "").Replace("(", "")
					.Split(':');
				if (array.Length != 2)
				{
					return new ResponseResult
					{
						Message = "APZ Location finish visit error!",
						MessageCode = "402"
					};
				}
				string cardCode = Function.ToString(data.DataHeader.Rows[0]["StoreCode"]);
				string newLatitude = Function.ToString(array[0]);
				string newLongitude = Function.ToString(array[1]);
				string newAddress = Function.ToString(data.DataHeader.Rows[0]["Address"]);
				string province = "";
				DataTable dataConfigDMS = UnitOfWork.Commons.GetDataConfigDMS("i-DMS");
				if (dataConfigDMS.IsNotNull() && dataConfigDMS.Rows[0].IsExistsColumn("GoogleApiKey"))
				{
					try
					{
						HttpClient httpClient = new HttpClient();
						string requestUri = "https://maps.googleapis.com/maps/api/geocode/json?latlng=" + newLatitude + "," + newLongitude + "&language=vi&key=" + Function.ToString(dataConfigDMS.Rows[0]["GoogleApiKey"]);
						HttpResponseMessage resultGoogleGeoCode = await httpClient.GetAsync(requestUri);
						await resultGoogleGeoCode.Content.ReadAsStringAsync();
						Task<GoogleApiGeoCodeResultImpl> task = resultGoogleGeoCode.Content.ReadAsAsync<GoogleApiGeoCodeResultImpl>();
						if (task.Result.results.Count > 0)
						{
							GoogleApiGeoCodeResultFormatedAddressImpl googleApiGeoCodeResultFormatedAddressImpl = task.Result.results[0];
							newAddress = Function.ToString(googleApiGeoCodeResultFormatedAddressImpl.formatted_address);
							AddressComponent addressComponent = googleApiGeoCodeResultFormatedAddressImpl.address_components.Find((AddressComponent x) => x.types.FindIndex((string y) => y == "administrative_area_level_1") >= 0);
							if (addressComponent != null)
							{
								province = addressComponent.long_name;
							}
						}
					}
					catch (Exception ex)
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
						return new ResponseResult
						{
							Message = ex.Message,
							MessageCode = "107"
						};
					}
				}
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter
				{
					ParameterName = "@cardCode",
					Value = cardCode
				});
				DataTable dataTable = UnitOfWork.Commons.ExecuteDataTable("SELECT * FROM OCRD WHERE \"CardCode\"=@cardCode;", CommandType.Text, list.ToArray());
				BPImpls bPImpls = new BPImpls();
				if (dataTable.IsNotNull())
				{
					bPImpls = dataTable.Rows[0].ToBusinessPartner();
				}
				bPImpls.CardCode = cardCode;
				bPImpls.OldCardCode = cardCode;
				bPImpls.CardName = Function.ToString(data.DataHeader.Rows[0]["StoreName"]);
				bPImpls.validFor = "Y";
				bPImpls.validFor = "Y";
				List<UserFieldsImpl> listUserFields = bPImpls.ListUserFields;
				listUserFields.RemoveAll((UserFieldsImpl x) => x.Name == "U_Latitude" || x.Name == "U_Longitude" || x.Name == "U_MapAddress" || x.Name == "U_Province");
				listUserFields.Add(new UserFieldsImpl
				{
					Name = "U_Latitude",
					Value = newLatitude
				});
				listUserFields.Add(new UserFieldsImpl
				{
					Name = "U_Longitude",
					Value = newLongitude
				});
				listUserFields.Add(new UserFieldsImpl
				{
					Name = "U_MapAddress",
					Value = newAddress
				});
				listUserFields.Add(new UserFieldsImpl
				{
					Name = "U_Province",
					Value = province
				});
				bPImpls.ListUserFields = listUserFields;
				if (newAddress.Length > 99)
				{
					bPImpls.Addresses = new List<AddressesImpl>
					{
						new AddressesImpl
						{
							Address = "DMS update store location Bill",
							Street = newAddress.Substring(0, 99),
							LineNum = -1,
							AdresType = "B"
						},
						new AddressesImpl
						{
							Address = "DMS update store location Ship",
							Street = newAddress.Substring(0, 99),
							LineNum = -1,
							AdresType = "S"
						}
					};
				}
				else
				{
					bPImpls.Addresses = new List<AddressesImpl>
					{
						new AddressesImpl
						{
							Address = "DMS update store location Bill",
							Street = newAddress,
							LineNum = -1,
							AdresType = "B"
						},
						new AddressesImpl
						{
							Address = "DMS update store location Ship",
							Street = newAddress,
							LineNum = -1,
							AdresType = "S"
						}
					};
				}
				DataTable dataTable2 = UnitOfWork.ContactPerson.ShowContactPerson(cardCode);
				List<ContactPersonImpl> list2 = new List<ContactPersonImpl>();
				for (int num = 0; num < dataTable2.Rows.Count; num++)
				{
					ContactPersonImpl item = Function.DataRowToEntity<ContactPersonImpl>(dataTable2.Rows[num]);
					list2.Add(item);
				}
				bPImpls.ListContactPerson = list2;
				if (data.DataHeader.ExistsColumn("E_Mail"))
				{
					bPImpls.E_Mail = Function.ToString(data.DataHeader.Rows[0]["E_Mail"]);
				}
				if (data.DataHeader.ExistsColumn("SlpCode"))
				{
					bPImpls.SlpCode = Function.ParseInt(data.DataHeader.Rows[0]["SlpCode"]);
				}
				if (data.DataHeader.ExistsColumn("IntrntSite"))
				{
					bPImpls.IntrntSite = Function.ToString(data.DataHeader.Rows[0]["IntrntSite"]);
				}
				string text;
				if (Global.LoginSap)
				{
					if (string.IsNullOrEmpty(SessionId))
					{
					}
					text = BrokerService.GetUnitOfWork(SessionId).BusinessPartners.Save(bPImpls, SessionUserInfo.SessionDiServer).Message;
				}
				else
				{
					text = "This function not support.";
				}
				if (!string.IsNullOrEmpty(text))
				{
					return new ResponseResult
					{
						Message = "APZ Update location store fail: " + text,
						MessageCode = "403"
					};
				}
			}
			HttpResult httpResult = UnitOfWork.VisitStore.FinishVisitStore(data);
			DataTable table = new DataTable();
			if (httpResult.Message == "")
			{
				return new ResponseResult
				{
					Message = "",
					MessageCode = "200"
				};
			}
			return new ResponseResult
			{
				Message = httpResult.Message,
				Content = new
				{
					Table0 = table
				},
				MessageCode = "406"
			};
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			return new ResponseResult
			{
				Message = ex2.Message,
				MessageCode = "107"
			};
		}
	}

	[HttpGet]
	public DataTable GetVisitSchedule(string userCode, string date)
	{
		try
		{
			return UnitOfWork.VisitStore.GetVisitSchedule(userCode, date);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}
}
