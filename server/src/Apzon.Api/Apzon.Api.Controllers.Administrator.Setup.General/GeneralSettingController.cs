using System;
using System.Data;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Administrator.Setup.General;

public class GeneralSettingController : BaseApiController
{
	[HttpGet]
	public DataTable ShowGeneralSetting()
	{
		return UnitOfWork.GeneralSetting.ShowGeneralSetting();
	}

	[HttpGet]
	public DataTable GetFormatString()
	{
		return UnitOfWork.GeneralSetting.GetFormatString();
	}

	[HttpGet]
	public DataTable GetFileSharingUploadPath()
	{
		return UnitOfWork.GeneralSetting.GetFileSharingUploadPath();
	}

	[HttpGet]
	public DataSet GetData(int userId)
	{
		return UnitOfWork.GeneralSetting.GetData(userId);
	}

	[HttpGet]
	public DataTable GetDefaultPaymentTermforCustomer()
	{
		return UnitOfWork.GeneralSetting.GetDefaultPaymentTermforCustomer();
	}

	[HttpGet]
	public DataTable GetDefaultPaymentMethodforCustomer()
	{
		return UnitOfWork.GeneralSetting.GetDefaultPaymentMethodforCustomer();
	}

	[HttpGet]
	public DataTable GetMapService()
	{
		return UnitOfWork.GeneralSetting.GetMapService();
	}

	[HttpGet]
	public DataTable GetDfltWhs()
	{
		return UnitOfWork.GeneralSetting.GetDfltWhs();
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string SaveData([FromBody] Tuple<DataSet, int> tuple)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "INVALIDSESSION";
			}
			BrokerService.GetUnitOfWork(SessionId);
			return "This function is not supported";
		}
		return UnitOfWork.GeneralSetting.SaveData(tuple);
	}

	[HttpGet]
	public DataTable GetAllGeneralSetting()
	{
		return UnitOfWork.GeneralSetting.GetAllGeneralSetting();
	}

	[HttpGet]
	public DataTable GetObjectTypePath()
	{
		return UnitOfWork.GeneralSetting.GetObjectTypePath();
	}

	[HttpGet]
	public DataTable GetLengthUnits()
	{
		return UnitOfWork.GeneralSetting.GetLengthUnits();
	}

	[HttpGet]
	public DataTable GetWeightUnits()
	{
		return UnitOfWork.GeneralSetting.GetWeightUnits();
	}
}
