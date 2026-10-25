using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Production;

namespace Apzon.Api.Controllers.Service;

public class ServiceCallController : BaseApiController
{
	[HttpGet]
	public DataTable GetSeries()
	{
		return UnitOfWork.ServiceCall.GetSeries();
	}

	[HttpGet]
	public DataTable GetCallStatus()
	{
		return UnitOfWork.ServiceCall.GetCallStatus();
	}

	[HttpGet]
	public DataTable GetOrigin()
	{
		return UnitOfWork.ServiceCall.GetOrigin();
	}

	[HttpGet]
	public DataTable GetProblemType()
	{
		return UnitOfWork.ServiceCall.GetProblemType();
	}

	[HttpGet]
	public DataTable GetProblemSubType()
	{
		return UnitOfWork.ServiceCall.GetProblemSubType();
	}

	[HttpGet]
	public DataTable GetCallType()
	{
		return UnitOfWork.ServiceCall.GetCallType();
	}

	[HttpGet]
	public DataTable GetAssignee()
	{
		return UnitOfWork.ServiceCall.GetAssignee();
	}

	[HttpGet]
	public DataTable GetLocation()
	{
		return UnitOfWork.ServiceCall.GetLocation();
	}

	[HttpGet]
	public DataTable GetNewAbsEntry()
	{
		return UnitOfWork.ServiceCall.GetNewAbsEntry();
	}

	[HttpGet]
	public DataTable GetAttachment(int callID)
	{
		return UnitOfWork.ServiceCall.GetAttachment(callID);
	}

	[HttpGet]
	public DataTable GetCountries()
	{
		return UnitOfWork.ServiceCall.GetCountries();
	}

	[HttpGet]
	public DataTable GetState(string countryCode)
	{
		return UnitOfWork.ServiceCall.GetState(countryCode);
	}

	[HttpGet]
	public DataTable GetContactPerson()
	{
		return UnitOfWork.ServiceCall.GetContactPerson();
	}

	[HttpGet]
	public DataTable GetContactPerson(string cardCode)
	{
		return UnitOfWork.ServiceCall.GetContactPerson(cardCode);
	}

	[HttpGet]
	public DataTable GetAddrType(string addressID)
	{
		return UnitOfWork.ServiceCall.GetAddrType(addressID);
	}

	[HttpPost]
	public DataTable GetDoc([FromBody] DataTable dts)
	{
		return UnitOfWork.ServiceCall.GetDoc(dts);
	}

	[HttpGet]
	public DataTable GetLineGeneral(int callID)
	{
		return UnitOfWork.ServiceCall.GetLineGeneral(callID);
	}

	[HttpGet]
	public DataTable GetDocNum(int series)
	{
		return UnitOfWork.ServiceCall.GetDocNum(series);
	}

	[HttpGet]
	public DataTable GetCallID()
	{
		return UnitOfWork.ServiceCall.GetCallID();
	}

	[HttpGet]
	public DataTable GetListItemGroup()
	{
		return UnitOfWork.ServiceCall.GetListItemGroup();
	}

	[HttpGet]
	public DataTable GetItemsInfo(string itemCode)
	{
		return UnitOfWork.ServiceCall.GetItemsInfo(itemCode);
	}

	[HttpGet]
	public DataTable GetItemsInfo()
	{
		return UnitOfWork.ServiceCall.GetItemsInfo();
	}

	[HttpGet]
	public DataTable GetActivities(int callID, int docNum)
	{
		return UnitOfWork.ServiceCall.GetActivities(callID, docNum);
	}

	[HttpGet]
	public DataTable GetAdressID(string cardCode)
	{
		return UnitOfWork.ServiceCall.GetAdressID(cardCode);
	}

	[HttpGet]
	public DataTable GetStreet(string cardCode, string adressID)
	{
		return UnitOfWork.ServiceCall.GetStreet(cardCode, adressID);
	}

	[HttpGet]
	public DataTable GetSolutions(int callID, int sltCode)
	{
		return UnitOfWork.ServiceCall.GetSolutions(callID, sltCode);
	}

	[HttpGet]
	public DataTable GetHistory(int callID)
	{
		return UnitOfWork.ServiceCall.GetHistory(callID);
	}

	[HttpPost]
	public string Save(Tuple<DataSet, List<AttachmentFile_Impl>> data)
	{
		try
		{
			if (data.Item1.Tables.Count < 5)
			{
				return "Thiếu dữ liệu!";
			}
			DataRow doc = data.Item1.Tables[0].Rows[0];
			DataTable line = data.Item1.Tables[1];
			DataTable activiti = data.Item1.Tables[2];
			DataTable expenses = data.Item1.Tables[3];
			DataTable solution = data.Item1.Tables[4];
			List<AttachmentFile_Impl> item = data.Item2;
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return "";
				}
				return BrokerService.GetUnitOfWork(SessionId).ServiceCall.Save(doc, line, activiti, expenses, solution, item, SessionUserInfo.SessionDiServer);
			}
			return UnitOfWork.ServiceCall.Save(doc, line, activiti, expenses, solution, item);
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}
}
