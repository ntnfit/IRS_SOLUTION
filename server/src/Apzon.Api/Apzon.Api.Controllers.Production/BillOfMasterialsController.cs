using System;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Production;

namespace Apzon.Api.Controllers.Production;

[ValidateLoginSapExceptionHandling]
public class BillOfMasterialsController : BaseApiController
{
	[HttpPost]
	public DataTable Search([FromBody] DataSet source)
	{
		return UnitOfWork.BillOfMasterials.Search(source.Tables[0], source.Tables[1]);
	}

	[HttpGet]
	public DataTable SearchADoc(string code)
	{
		return UnitOfWork.BillOfMasterials.SearchADoc(code);
	}

	[HttpGet]
	public DataTable GetSearchTableDoc()
	{
		return UnitOfWork.BillOfMasterials.GetSearchTableDoc();
	}

	[HttpGet]
	public DataTable GetSearchTableLine()
	{
		return UnitOfWork.BillOfMasterials.GetSearchTableLine();
	}

	[HttpPost]
	public DataTable SearchLine([FromBody] BillOfMasterials_LineImpl_ITT1 line)
	{
		return UnitOfWork.BillOfMasterials.SearchLine(line);
	}

	[HttpGet]
	public DataTable GetItemCFL()
	{
		return UnitOfWork.BillOfMasterials.GetItemCFL();
	}

	[HttpGet]
	public DataTable ShowITT1(string code)
	{
		return UnitOfWork.BillOfMasterials.ShowITT1(code);
	}

	[HttpGet]
	public DataTable ShowOITT(string BoMCode)
	{
		return UnitOfWork.BillOfMasterials.ShowITT1(BoMCode);
	}

	[HttpGet]
	public DataTable GetItemName(string itemCode)
	{
		return UnitOfWork.BillOfMasterials.GetItemName(itemCode);
	}

	[HttpGet]
	public DataTable CheckValidItemType(string code)
	{
		return UnitOfWork.BillOfMasterials.CheckValidItemType(code);
	}

	[HttpGet]
	public string DeleteDoc(string code)
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
				return unitOfWork.ProductTrees.DeleteDoc(code, SessionUserInfo.SessionDiServer);
			}
		}
		return "This function is not supported";
	}

	[HttpGet]
	public DataTable GetList_BillOfMster()
	{
		return UnitOfWork.BillOfMasterials.GetList_BillOfMster();
	}

	[HttpGet]
	public DataTable GetChildItemsInBom(string code)
	{
		return UnitOfWork.BillOfMasterials.GetChildItemsInBom(code);
	}

	[HttpGet]
	public DataTable GetItemPriceByPriceList(string itemCode, int priceList)
	{
		return UnitOfWork.BillOfMasterials.GetItemPriceByPriceList(itemCode, priceList);
	}

	[HttpGet]
	public DataTable GetActiveDimensionCount()
	{
		return UnitOfWork.BillOfMasterials.GetActiveDimensionCount();
	}

	[HttpGet]
	public DataTable GetItemDetails(string type, string code)
	{
		return UnitOfWork.BillOfMasterials.GetItemDetails(type, code);
	}

	[HttpGet]
	public DataTable IsValidCode(int type, string code)
	{
		return UnitOfWork.BillOfMasterials.IsValidCode(type, code);
	}

	[HttpGet]
	public bool IsABOM(string code)
	{
		return UnitOfWork.BillOfMasterials.IsABOM(code);
	}

	[HttpPost]
	public string SaveDoc([FromBody] BillOfMasterials_Impl_OITT line)
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
				return unitOfWork.ProductTrees.Save(line, SessionUserInfo.SessionDiServer);
			}
			return "INVALIDSESSION";
		}
		return "This method must be log in to SAP - (IRS)";
	}

	[HttpPost]
	public string SaveDocNoSAP([FromBody] DataSet ds)
	{
		if (Global.LoginSap)
		{
			return "This method is invalid when log in SAP - (IRS)";
		}
		return UnitOfWork.BillOfMasterials.SaveDocNoSAP(ds, Enums.TransactionType.A, "");
	}

	[HttpPost]
	public string SaveDocNoSAPUpdate([FromBody] DataSet ds)
	{
		if (Global.LoginSap)
		{
			return "This method is invalid when log in SAP - (IRS)";
		}
		return UnitOfWork.BillOfMasterials.SaveDocNoSAPUpdate(ds);
	}

	[HttpGet]
	public DataTable GetRawMaterial(string itemCode)
	{
		return UnitOfWork.BillOfMasterials.GetRawMaterial(itemCode);
	}

	[HttpGet]
	public DataTable GetFirstRecord()
	{
		return UnitOfWork.BillOfMasterials.GetFirstRecord();
	}

	[HttpGet]
	public DataTable GetLastRecord()
	{
		return UnitOfWork.BillOfMasterials.GetLastRecord();
	}

	[HttpGet]
	public DataTable GetNextRecord(string value)
	{
		return UnitOfWork.BillOfMasterials.GetNextRecord(value);
	}

	[HttpGet]
	public DataTable GetPreviousRecord(string value)
	{
		return UnitOfWork.BillOfMasterials.GetPreviousRecord(value);
	}

	[HttpGet]
	public DataTable getBoMByItemCode(string code)
	{
		return UnitOfWork.BillOfMasterials.getBoMByItemCode(code);
	}

	[HttpGet]
	public DataTable GetLineByFather(string father, string username, int branch)
	{
		return UnitOfWork.BillOfMasterials.GetLineByFather(father, username, branch);
	}

	[HttpGet]
	public DataTable GetRecord(string p, string c)
	{
		return UnitOfWork.BillOfMasterials.GetRecord(p, c);
	}

	[HttpGet]
	public DataTable GetDataDocBomVersion(string code, string version)
	{
		return UnitOfWork.BomVersion.GetDataDocBomVersion(code, version);
	}

	[HttpGet]
	public DataTable GetDataLineBomVersion(int id)
	{
		return UnitOfWork.BomVersion.GetDataLineBomVersion(id);
	}

	[HttpGet]
	public DataTable GetColumnsDocBomVersion()
	{
		return UnitOfWork.BomVersion.GetColumnsDocBomVersion();
	}

	[HttpGet]
	public DataTable GetColumnsLineBomVersion()
	{
		return UnitOfWork.BomVersion.GetColumnsLineBomVersion();
	}

	[HttpGet]
	public DataSet GetDataBomSap(string productionNo)
	{
		return UnitOfWork.BomVersion.GetDataBomSap(productionNo);
	}

	[HttpPost]
	public HttpResult Save([FromBody] DataSet dtset)
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
					string text = unitOfWork.ProductTrees.SaveBomVersion(dtset, SessionUserInfo.SessionDiServer);
					if (!string.IsNullOrEmpty(text))
					{
						return new HttpResult(Enums.MessageCode.Error, text);
					}
					return UnitOfWork.BomVersion.Save(dtset, Enums.TransactionType.A, "", Global.LoginSap);
				}
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			return UnitOfWork.BomVersion.Save(dtset, Enums.TransactionType.A, "", Global.LoginSap);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	[HttpPost]
	public HttpResult Update([FromBody] DataSet dtset)
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
					string text = unitOfWork.ProductTrees.SaveBomVersion(dtset, SessionUserInfo.SessionDiServer);
					if (!string.IsNullOrEmpty(text))
					{
						return new HttpResult(Enums.MessageCode.Error, text);
					}
					return UnitOfWork.BomVersion.Save(dtset, Enums.TransactionType.U, "", Global.LoginSap);
				}
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			return UnitOfWork.BomVersion.Save(dtset, Enums.TransactionType.U, "", Global.LoginSap);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	[HttpPost]
	public HttpResult DeleteBomVersion([FromBody] int id)
	{
		return UnitOfWork.BomVersion.Save(null, Enums.TransactionType.D, Function.ToString(id), Global.LoginSap);
	}

	[HttpGet]
	public string GetNewVersionBom(string itemcode)
	{
		return UnitOfWork.BomVersion.GetNewVersionBom(itemcode);
	}

	[HttpGet]
	public DataTable LoadDataNextLast(Enums.NavigateType type, string code)
	{
		return UnitOfWork.BomVersion.LoadDataNextLast(type, code);
	}

	[HttpGet]
	public DataTable GetDataLineBomVersion(string fatherCode, string version)
	{
		return UnitOfWork.BillOfMasterials.GetDataLineBomVersion(fatherCode, version);
	}

	[HttpGet]
	public DataTable GetAllChilds()
	{
		return UnitOfWork.BillOfMasterials.GetAllChilds();
	}

	[HttpGet]
	public DataTable CopyChildItemsInBom(string itemCode, int baseType, int baseEntry, int baseRow)
	{
		return UnitOfWork.BillOfMasterials.CopyChildItemsInBom(itemCode, baseType, baseEntry, baseRow);
	}
}
