using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.MasterData;
using Apzon.Entities.Models.MobileApp;

namespace Apzon.Api.Controllers.Inventory;

[ValidateLoginSapExceptionHandling]
public class ItemsMasterController : BaseApiController
{
	[HttpGet]
	public HttpResult Navigate(int type, string currentCode, string user, int branch)
	{
		try
		{
			return UnitOfWork.Items.Navigate(type, currentCode, user, branch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	[HttpGet]
	public DataSet GetByCode(string itemCode, string user, int branch)
	{
		try
		{
			return UnitOfWork.Items.GetByCode(itemCode, user, branch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	[HttpPost]
	public DataTable Find([FromBody] Tuple<DataTable, string> model)
	{
		try
		{
			DataTable item = model.Item1;
			string item2 = model.Item2;
			return UnitOfWork.Items.Find(item, item2);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	[HttpGet]
	public DataTable GetItemStruct()
	{
		try
		{
			return UnitOfWork.Items.GetItemStruct();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpPost]
	public DataTable GetADoc([FromBody] string itmCode)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "INVALIDSESSION");
				return new DataTable();
			}
			return UnitOfWork.Items.GetADoc(itmCode);
		}
		return UnitOfWork.Items.GetADoc(itmCode);
	}

	[HttpGet]
	public DataTable GetUoMGroup()
	{
		return UnitOfWork.Items.GetUoMGroup();
	}

	[HttpGet]
	public string IsExistsTransItemWarehouse(string itemCode, string whsCode)
	{
		return UnitOfWork.Items.IsExistsTransItemWarehouse(itemCode, whsCode);
	}

	[HttpGet]
	public string IsStockInItemWarehouse(string itemCode, string whsCode)
	{
		return UnitOfWork.Items.IsStockInItemWarehouse(itemCode, whsCode);
	}

	[HttpGet]
	public DataTable GetPriceList()
	{
		return UnitOfWork.Items.GetPriceList();
	}

	[HttpGet]
	public DataTable LoadUomGroup()
	{
		try
		{
			return UnitOfWork.Items.LoadUomGroup();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpGet]
	public DataTable LoadPriceList()
	{
		try
		{
			return UnitOfWork.Items.LoadPriceList();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpGet]
	public DataTable LoadManufactures()
	{
		try
		{
			return UnitOfWork.Items.LoadManufactures();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpGet]
	public DataTable LoadShippingType()
	{
		try
		{
			return UnitOfWork.Items.LoadShippingType();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpGet]
	public DataTable LoadCustomGroup()
	{
		try
		{
			return UnitOfWork.Items.LoadCustomGroup();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpGet]
	public DataTable LoadVatGroup(string category)
	{
		try
		{
			return UnitOfWork.Items.LoadVatGroup(category);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpGet]
	public DataTable LoadProperties()
	{
		try
		{
			return UnitOfWork.Items.LoadProperties();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpGet]
	public DataTable LoadSeries(string user)
	{
		try
		{
			return UnitOfWork.Items.LoadSeries(user);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable Get()
	{
		return UnitOfWork.Items.Table();
	}

	[HttpGet]
	public DataSet GetItemProperties(string itemCode)
	{
		return UnitOfWork.Items.GetItemProperties(itemCode);
	}

	[HttpGet]
	public DataTable GetItemDetails(string itemCode, int BPLId)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "INVALIDSESSION");
				return new DataTable();
			}
			string userName = SessionUserInfo.UserName;
			return UnitOfWork.Items.GetItemDetails(itemCode, userName, BPLId);
		}
		string userName2 = SessionUserInfo.UserName;
		return UnitOfWork.Items.GetItemDetails(itemCode, userName2, BPLId);
	}

	[HttpGet]
	public List<ItemsImpl> Cfl()
	{
		return UnitOfWork.Items.Cfl();
	}

	[HttpGet]
	public DataTable GetItemByItemCode(string ItemCode)
	{
		return UnitOfWork.Items.GetItemByItemCode(ItemCode);
	}

	[HttpGet]
	public DataTable GetPrice(string itemCode)
	{
		return UnitOfWork.Items.GetPrice(itemCode);
	}

	[HttpGet]
	public DataTable GetItemWarehouse(string itemCode)
	{
		return UnitOfWork.Items.GetItemWarehouse(itemCode);
	}

	[HttpPost]
	public DataTable CheckExistItem([FromBody] string ItemCode)
	{
		return UnitOfWork.Items.CheckExistItem(ItemCode);
	}

	[HttpPost]
	public DataTable Search([FromBody] Tuple<ItemsImpl, DataTable> tpl)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "INVALIDSESSION");
				return new DataTable();
			}
			string userName = SessionUserInfo.UserName;
			tpl.Item1.USER_CODE = userName;
			return UnitOfWork.Items.Search(tpl);
		}
		string userName2 = SessionUserInfo.UserName;
		tpl.Item1.USER_CODE = userName2;
		return UnitOfWork.Items.Search(tpl);
	}

	[HttpGet]
	public DataTable GetAnItemByItemCode(string itemCode)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "INVALIDSESSION");
				return new DataTable();
			}
			string userName = SessionUserInfo.UserName;
			return UnitOfWork.Items.GetAnItemByItemCode(itemCode, userName);
		}
		string userName2 = SessionUserInfo.UserName;
		return UnitOfWork.Items.GetAnItemByItemCode(itemCode, userName2);
	}

	[HttpGet]
	public DataTable SearchLine(string itemCode)
	{
		return UnitOfWork.Items.SearchLine(itemCode);
	}

	[HttpGet]
	public DataTable SearchOITW(string itemCode)
	{
		return UnitOfWork.Items.SearchOITW(itemCode);
	}

	[HttpGet]
	public DataTable SearchITM12(string itemCode)
	{
		return UnitOfWork.Items.SearchITM12(itemCode);
	}

	[HttpPost]
	public string SaveDoc([FromBody] ItemsImpl doc)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).Items.Save(doc, SessionUserInfo.SessionDiServer);
		}
		return "[APZ-IRS].[SaveDoc] - This function not supported when not login SAP";
	}

	[HttpPost]
	public string SaveIgnoreEmpty([FromBody] ItemsImpl doc)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).Items.Save(doc, SessionUserInfo.SessionDiServer, ignoreEmptyValue: true);
		}
		return "[APZ-IRS].[SaveDoc] - This function not supported when not login SAP";
	}

	[HttpPost]
	public string SaveDocDataSet([FromBody] DataSet ds)
	{
		if (!Global.LoginSap)
		{
			return UnitOfWork.Items.SaveDoc(ds);
		}
		return "[APZ-IRS].[SaveDocDataSet] - This function not supported when login SAP";
	}

	[HttpPost]
	public string UpdateUoMPrices([FromBody] List<Items_UoMPricesImpl> lst)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).Items.UpdateUoMPrices(lst, SessionUserInfo.SessionDiServer);
		}
		return "[APZ-IRS].[SaveDoc] - This function not supported when not login SAP";
	}

	[HttpGet]
	public DataTable GetRecord(string itmCode, string typeRecord, int BPLId, bool isAssetMaster = false)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "INVALIDSESSION");
				return new DataTable();
			}
			string userName = SessionUserInfo.UserName;
			return UnitOfWork.Items.GetRecord(itmCode, typeRecord, userName, BPLId, isAssetMaster);
		}
		string userName2 = SessionUserInfo.UserName;
		return UnitOfWork.Items.GetRecord(itmCode, typeRecord, userName2, BPLId, isAssetMaster);
	}

	[HttpGet]
	public DataTable GetFirstRecord()
	{
		return UnitOfWork.Items.GetFirstRecord();
	}

	[HttpGet]
	public DataTable GetPreviousRecord(string item)
	{
		return UnitOfWork.Items.GetPreviousRecord(item);
	}

	[HttpGet]
	public DataTable GetNextRecord(string item)
	{
		return UnitOfWork.Items.GetNextRecord(item);
	}

	[HttpGet]
	public DataTable GetLastRecord()
	{
		return UnitOfWork.Items.GetLastRecord();
	}

	[HttpPost]
	public DataTable GetNext([FromBody] ItemsImpl doc)
	{
		return UnitOfWork.Items.GetNext(doc);
	}

	[HttpPost]
	public DataTable GetPrevious([FromBody] ItemsImpl doc)
	{
		return UnitOfWork.Items.GetPrevious(doc);
	}

	[HttpGet]
	public DataTable GetFirst(ItemsImpl doc)
	{
		return UnitOfWork.Items.GetFirst(doc);
	}

	[HttpGet]
	public DataTable GetLast(ItemsImpl doc)
	{
		return UnitOfWork.Items.GetLast(doc);
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string DeleteOITW(string itemCode, string whsCode)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			BrokerService.GetUnitOfWork(SessionId);
			return "Chức năng đang được xây dựng!";
		}
		return UnitOfWork.Items.DeleteOITW(itemCode, whsCode);
	}

	[HttpGet]
	public DataTable ShowWareHouse()
	{
		return UnitOfWork.Items.ShowWareHouse();
	}

	[HttpPost]
	public DataTable FindDataByShip([FromBody] ItemsImpl doc)
	{
		return UnitOfWork.Items.FindDataByShip(doc);
	}

	[HttpGet]
	public DataTable Check_FirstRecord()
	{
		return UnitOfWork.Items.Check_FirstRecord();
	}

	[HttpGet]
	public DataTable Check_LastRecord()
	{
		return UnitOfWork.Items.Check_LastRecord();
	}

	[HttpGet]
	public bool CheckItem(string itemCode)
	{
		return UnitOfWork.Items.CheckDoc(itemCode);
	}

	[HttpPost]
	public DataTable GetOITM([FromBody] ItemsImpl doc)
	{
		if (string.IsNullOrEmpty(SessionId))
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "INVALIDSESSION");
			return new DataTable();
		}
		string userName = SessionUserInfo.UserName;
		doc.USER_CODE = userName;
		return UnitOfWork.Items.GetOITM(doc);
	}

	[HttpGet]
	public DataTable GetAllDivision()
	{
		return UnitOfWork.Items.GetAllDivision();
	}

	[HttpGet]
	public DataTable GetCatetory(string divCode)
	{
		try
		{
			return UnitOfWork.Items.GetCategory(divCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	[HttpGet]
	public DataTable GetModel(string group)
	{
		try
		{
			return UnitOfWork.Items.GetModel(group);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	[HttpGet]
	public DataTable GetItem(string model, string color, string size)
	{
		try
		{
			return UnitOfWork.Items.GetItem(model, color, size);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	[HttpGet]
	public DataTable GetModelByDivision(string divCode, int? pageNum, int? pageSize, string catId, string model)
	{
		try
		{
			return UnitOfWork.Items.GetModelByDivision(divCode, pageNum, pageSize, catId, model);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	[HttpGet]
	public DataTable LoadUDFDataSAP(string itemCode)
	{
		return UnitOfWork.Items.LoadUDFDataSAP(itemCode);
	}

	[HttpGet]
	public DataTable GetShowOCYC()
	{
		return UnitOfWork.Items.GetOCYC();
	}

	[HttpGet]
	public HttpResult GetItemsPerUnit(string itmCode, string type)
	{
		return UnitOfWork.Items.GetItemsPerUnit(itmCode, type);
	}

	[HttpPost]
	public DataTable ReportItemSummary([FromBody] Tuple<DateTime, DateTime, string, string, string, string> model)
	{
		try
		{
			DateTime item = model.Item1;
			DateTime item2 = model.Item2;
			string item3 = model.Item3;
			string item4 = model.Item4;
			string item5 = model.Item5;
			string item6 = model.Item6;
			return UnitOfWork.Items.ReportItemSummary(item, item2, item3, item4, item5, item6);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	[HttpGet]
	public DataTable ExistFileInDoc(string itemCode, string fileName, string fileExt)
	{
		return UnitOfWork.Items.ExistFileInDoc(itemCode, fileName, fileExt);
	}

	[HttpGet]
	public DataTable GetItemLineTableStructure()
	{
		return UnitOfWork.Items.GetItemLineTableStructure();
	}

	[HttpGet]
	public DataTable GetVolumeUnit()
	{
		return UnitOfWork.Items.GetVolumeUnit();
	}

	[HttpGet]
	public DataTable GetLghtAndWghtDisplay()
	{
		return UnitOfWork.Items.GetLghtAndWghtDisplay();
	}

	[HttpGet]
	public DataTable GetUoMCombo(int ugpEntry, string itemCode, string type)
	{
		return UnitOfWork.Items.GetUoMCombo(ugpEntry, itemCode, type);
	}

	[HttpGet]
	public DataTable GetUomInItemByUomGroup(int ugpEntry, string itemCode)
	{
		return UnitOfWork.Items.GetUomInItemByUomGroup(ugpEntry, itemCode);
	}

	[HttpGet]
	public DataTable GetUoMByUomGroup(int ugpEntry)
	{
		return UnitOfWork.Items.GetUoMByUomGroup(ugpEntry);
	}

	[HttpGet]
	public DataTable LoadBaseUoM(int ugrpEntry)
	{
		return UnitOfWork.Items.LoadBaseUoM(ugrpEntry);
	}

	[HttpGet]
	public DataTable ItemsMasterGetListCustomsGroup()
	{
		return UnitOfWork.Items.ItemsMasterGetListCustomsGroup();
	}

	[HttpGet]
	public DataTable GetItemType(string itemCode)
	{
		return UnitOfWork.Items.GetItemType(itemCode);
	}

	[HttpGet]
	public DataTable GetUoMPricesByItmCodePrList(int? priceList, string itemCode, int ugpEntry)
	{
		return UnitOfWork.Items.GetUoMPricesByItmCodePrList(priceList, itemCode, ugpEntry);
	}

	[HttpGet]
	public DataTable UoMPricesLoadPriceList(int listNum)
	{
		return UnitOfWork.Items.UoMPricesLoadPriceList(listNum);
	}

	[HttpGet]
	public DataTable UoMPricesLoadComBo(string itemCode)
	{
		return UnitOfWork.Items.UoMPricesLoadComBo(itemCode);
	}

	[HttpGet]
	public DataTable UoMInItemGetRowsDetails(int uomEntry, int ugpEntry)
	{
		return UnitOfWork.Items.UoMInItemGetRowsDetails(uomEntry, ugpEntry);
	}

	[HttpGet]
	public DataTable GetTop1(string itemCode)
	{
		return UnitOfWork.Items.GetTop1(itemCode);
	}

	[HttpGet]
	public string IsLinkedTrans(string itemCode)
	{
		return UnitOfWork.Items.IsLinkedTrans(itemCode);
	}

	[HttpGet]
	public string RemoveItem(string itemCode)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).Items.Remove(itemCode, SessionUserInfo.SessionDiServer);
		}
		return UnitOfWork.Items.RemoveItem(itemCode);
	}

	[HttpGet]
	public DataTable GetItemGroupData(int itmsGrpCod)
	{
		return UnitOfWork.Items.GetItemGroupData(itmsGrpCod);
	}

	[HttpGet]
	public DataTable GetProductionData(string itemCode)
	{
		return UnitOfWork.Items.GetProductionData(itemCode);
	}

	[HttpGet]
	public DataTable GetPreferredVendor(string itemCode)
	{
		return UnitOfWork.Items.GetPreferredVendor(itemCode);
	}

	[HttpGet]
	public DataTable GetAPreferredVendorData(string itemCode, string vendorCode)
	{
		return UnitOfWork.Items.GetAPreferredVendorData(itemCode, vendorCode);
	}

	[HttpGet]
	public bool ItemHaveQuantity(string itemCode)
	{
		return UnitOfWork.Items.ItemHaveQuantity(itemCode);
	}

	[HttpGet]
	public int GetDefaultLengthCode()
	{
		return UnitOfWork.Items.GetDefaultLengthCode();
	}

	[HttpGet]
	public ResponseResult GetAllDataItem()
	{
		try
		{
			if (UnitOfWork == null)
			{
				return new ResponseResult
				{
					Message = "Invalid SesionId",
					Content = new
					{
						Table0 = "401"
					},
					MessageCode = "Lỗi401"
				};
			}
			DataTable allDataItem = UnitOfWork.Items.GetAllDataItem();
			allDataItem.TableName = "Items";
			return new ResponseResult
			{
				Message = "",
				Content = new
				{
					Table0 = allDataItem
				},
				MessageCode = "0"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				Content = new
				{
					Table0 = "107,error"
				},
				MessageCode = "Lỗi107"
			};
		}
	}

	[HttpGet]
	public ResponseResult GetDataItemByItemCode(string itemCode)
	{
		try
		{
			if (UnitOfWork == null)
			{
				return new ResponseResult
				{
					Message = "Invalid SesionId",
					Content = new
					{
						Table0 = "401"
					},
					MessageCode = "Lỗi401"
				};
			}
			DataTable dataItemByItemCode = UnitOfWork.Items.GetDataItemByItemCode(itemCode);
			dataItemByItemCode.TableName = "ItemsDetail";
			return new ResponseResult
			{
				Message = "",
				Content = new
				{
					Table0 = dataItemByItemCode
				},
				MessageCode = "0"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				Content = new
				{
					Table0 = "107,error"
				},
				MessageCode = "Lỗi107"
			};
		}
	}

	[HttpGet]
	public ResponseResult GetDataItemGroup()
	{
		try
		{
			if (UnitOfWork == null)
			{
				return new ResponseResult
				{
					Message = "Invalid SesionId",
					Content = new
					{
						Table0 = "401"
					},
					MessageCode = "Lỗi401"
				};
			}
			DataTable dataItemGroup = UnitOfWork.Items.GetDataItemGroup();
			dataItemGroup.TableName = "ItemsGroup";
			return new ResponseResult
			{
				Message = "",
				Content = new
				{
					Table0 = dataItemGroup
				},
				MessageCode = "0"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				Content = new
				{
					Table0 = "107, error"
				},
				MessageCode = "Lỗi107"
			};
		}
	}

	[HttpGet]
	public ResponseResult SearchWishItemMaster(string userCode)
	{
		try
		{
			if (UnitOfWork == null)
			{
				return new ResponseResult
				{
					Message = "Invalid SesionId",
					Content = new
					{
						Table0 = "401"
					},
					MessageCode = "Lỗi401"
				};
			}
			DataTable dataTable = UnitOfWork.Items.SearchWishItemMaster(userCode);
			dataTable.TableName = "ItemsGroup";
			return new ResponseResult
			{
				Message = "",
				Content = new
				{
					Table0 = dataTable
				},
				MessageCode = "0"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				Content = new
				{
					Table0 = "107, error"
				},
				MessageCode = "Lỗi107"
			};
		}
	}

	[HttpGet]
	public ResponseResult SearchItemMaster(string itemCode, int ItmsGrpCod)
	{
		try
		{
			if (UnitOfWork == null)
			{
				return new ResponseResult
				{
					Message = "Invalid SesionId",
					Content = new
					{
						Table0 = "401"
					},
					MessageCode = "Lỗi401"
				};
			}
			DataTable dataTable = UnitOfWork.Items.SearchItemMaster(itemCode, ItmsGrpCod);
			dataTable.TableName = "ItemsGroup";
			return new ResponseResult
			{
				Message = "",
				Content = new
				{
					Table0 = dataTable
				},
				MessageCode = "0"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				Content = new
				{
					Table0 = "107, error"
				},
				MessageCode = "Lỗi107"
			};
		}
	}

	[HttpGet]
	public DataSet GetSaleItems(string userName, string cardCode)
	{
		return UnitOfWork.Items.GetSaleItems(userName, cardCode);
	}
}
