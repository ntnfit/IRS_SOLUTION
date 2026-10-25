using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Administrator.Setup.Inventory;

public class AttrCodeSetupController : BaseApiController
{
	[HttpGet]
	public DataTable GetAttribute()
	{
		return UnitOfWork.AttrCodeSetup.GetAttribute();
	}

	[HttpGet]
	public DataTable GetAttributeByFldNum(int fldNum)
	{
		return UnitOfWork.AttrCodeSetup.GetAttributeByFldNum(fldNum);
	}

	[HttpPost]
	public string Save([FromBody] DataTable doc)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).BinLocationAttribute.Save(SessionUserInfo.SessionDiServer, doc);
		}
		return "This function not support.";
	}

	[HttpGet]
	public string CheckExistInBinLoc(int fldAbs, string attrValue)
	{
		return UnitOfWork.AttrCodeSetup.CheckExistedInBinLoc(fldAbs, attrValue);
	}
}
