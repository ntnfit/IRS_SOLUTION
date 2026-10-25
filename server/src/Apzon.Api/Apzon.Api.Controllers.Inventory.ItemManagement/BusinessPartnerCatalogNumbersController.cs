using System;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Inventory.ItemManagement;

public class BusinessPartnerCatalogNumbersController : BaseApiController
{
	[HttpGet]
	public DataTable GetItemByCardCode(string cardcode)
	{
		try
		{
			return UnitOfWork.BusinessPartnerCatalogNumbers.GetItemByCardCode(cardcode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetItemByCardCode   ", ex.ToString());
		}
		return new DataTable();
	}

	[HttpGet]
	public DataTable GetBPByItemCode(string itemcode)
	{
		try
		{
			return UnitOfWork.BusinessPartnerCatalogNumbers.GetBPByItemCode(itemcode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetBPByItemCode   ", ex.ToString());
		}
		return new DataTable();
	}

	[HttpGet]
	public DataTable GetBPByCardCode(string cardCode)
	{
		return UnitOfWork.BusinessPartnerCatalogNumbers.GetBPByCardCode(cardCode);
	}

	[HttpGet]
	public DataTable GetBPByCode(string catalogCode, string cardCode)
	{
		return UnitOfWork.BusinessPartnerCatalogNumbers.GetBPByCode(catalogCode, cardCode);
	}

	[HttpPost]
	public HttpResult SaveItem([FromBody] DataTable model)
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
				}
				IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
				if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
				{
					return unitOfWork.BusinessPartnerCatalogNumbers.Save(model, ApzObjectTypes.oItems, SessionUserInfo.SessionDiServer);
				}
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			return UnitOfWork.BusinessPartnerCatalogNumbers.Save(model, ApzObjectTypes.oItems);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "save   ", ex.ToString());
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	[HttpPost]
	public HttpResult SaveBp([FromBody] DataTable model)
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
				}
				IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
				if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
				{
					return unitOfWork.BusinessPartnerCatalogNumbers.Save(model, ApzObjectTypes.oBusinessPartners, SessionUserInfo.SessionDiServer);
				}
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			return UnitOfWork.BusinessPartnerCatalogNumbers.Save(model, ApzObjectTypes.oBusinessPartners);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "save   ", ex.ToString());
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	[HttpPost]
	public string Remove([FromBody] Tuple<string, string, string, string> model)
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return "INVALIDSESSION";
				}
				IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
				if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
				{
					if (model.Item4 == "TabBP")
					{
						return unitOfWork.BusinessPartnerCatalogNumbers.Remove(model.Item1, model.Item2, model.Item3, ApzObjectTypes.oBusinessPartners, SessionUserInfo.SessionDiServer);
					}
					if (model.Item4 == "TabItems")
					{
						return unitOfWork.BusinessPartnerCatalogNumbers.Remove(model.Item1, model.Item2, model.Item3, ApzObjectTypes.oItems, SessionUserInfo.SessionDiServer);
					}
				}
				return "INVALIDSESSION";
			}
			return UnitOfWork.BusinessPartnerCatalogNumbers.Remove(model.Item1, model.Item2, model.Item3, model.Item4);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Remove  ", ex.ToString());
		}
		return "error";
	}

	[HttpGet]
	public string CheckExist(string code, string subtitle, string item, ApzObjectTypes objtype)
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return "N";
				}
				IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
				if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
				{
					return unitOfWork.BusinessPartnerCatalogNumbers.CheckExist(code, subtitle, item, objtype, SessionUserInfo.SessionDiServer);
				}
				return "N";
			}
			return UnitOfWork.BusinessPartnerCatalogNumbers.CheckExist(code, subtitle, item, objtype);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "CheckExist  ", ex.ToString());
		}
		return "N";
	}

	[HttpGet]
	public DataTable GetData(Enums.NavigateType type, Enums.BusinessPartnerCatalogNumbersTableType objectType, string par)
	{
		try
		{
			return UnitOfWork.BusinessPartnerCatalogNumbers.GetData(type, objectType, par);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetData    ", ex.ToString());
		}
		return new DataTable();
	}

	[HttpGet]
	public string GetNameKey(string code, Enums.BusinessPartnerCatalogNumbersTableType type)
	{
		try
		{
			return UnitOfWork.BusinessPartnerCatalogNumbers.GetNameKey(code, type);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetNameKey  ", ex.ToString());
		}
		return "";
	}

	[HttpGet]
	public string GetCardNameByCode(string cardCode)
	{
		return UnitOfWork.BusinessPartnerCatalogNumbers.GetCardNameByCode(cardCode);
	}

	[HttpGet]
	public string GetItemNameByCode(string itemCode)
	{
		return UnitOfWork.BusinessPartnerCatalogNumbers.GetItemNameByCode(itemCode);
	}

	[HttpGet]
	public DataTable GetDataItemByBpCatalog(string subCatNum, string cardCode, string itemCode)
	{
		return UnitOfWork.BusinessPartnerCatalogNumbers.GetDataItemByBpCatalog(subCatNum, cardCode, itemCode);
	}

	[HttpGet]
	public string GetSubCatNumByItemAndCardCode(string itemCode, string cardCode)
	{
		return UnitOfWork.BusinessPartnerCatalogNumbers.GetSubCatNumByItemAndCardCode(itemCode, cardCode);
	}
}
