using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Production;

namespace Apzon.Api.Controllers.Opportunity;

[ValidateLoginSapExceptionHandling]
public class ActivitiesForOpportunityController : BaseApiController
{
	[HttpPost]
	public DataTable SearchDoc([FromBody] DataTable dt)
	{
		return UnitOfWork.ActivitiesForOpportunity.SearchDoc(dt);
	}

	[HttpPost]
	public DataTable SearchActivityOverviewBP([FromBody] DataTable dt)
	{
		return UnitOfWork.ActivitiesForOpportunity.SearchActivityOverviewBP(dt);
	}

	[HttpGet]
	public DataTable LoadOneDoc(int clgCode)
	{
		return UnitOfWork.ActivitiesForOpportunity.LoadOneDoc(clgCode);
	}

	[HttpGet]
	public string GetLinkedObject(int clgCode)
	{
		return UnitOfWork.ActivitiesForOpportunity.GetLinkedObject(clgCode);
	}

	[HttpPost]
	public string SaveDoc([FromBody] Tuple<List<AttachmentFile_Impl>, DataTable> tpl)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).Activities.Save(tpl.Item1, tpl.Item2, SessionUserInfo.SessionDiServer);
		}
		return UnitOfWork.ActivitiesForOpportunity.SaveDoc(tpl.Item2, tpl.Item1);
	}

	[HttpGet]
	public int GetMaxActivity_ClgCode()
	{
		return UnitOfWork.ActivitiesForOpportunity.GetMaxActivity_ClgCode();
	}

	[HttpGet]
	public int GetMinClgCode()
	{
		return UnitOfWork.ActivitiesForOpportunity.GetMinClgCode();
	}

	[HttpGet]
	public DataTable GetDefaultTable()
	{
		return UnitOfWork.ActivitiesForOpportunity.GetDefaultTable();
	}

	[HttpGet]
	public int CheckExistAttachment(int clgCode)
	{
		return UnitOfWork.ActivitiesForOpportunity.CheckExistAttachment(clgCode);
	}

	[HttpGet]
	public DataTable ExistFileInDoc(int clgCode, string fileName, string fileExt)
	{
		return UnitOfWork.ActivitiesForOpportunity.ExistFileInDoc(clgCode, fileName, fileExt);
	}

	[HttpGet]
	public DataTable GetFirstRecord()
	{
		return UnitOfWork.ActivitiesForOpportunity.GetFirstRecord();
	}

	[HttpGet]
	public DataTable GetLastRecord()
	{
		return UnitOfWork.ActivitiesForOpportunity.GetLastRecord();
	}

	[HttpGet]
	public DataTable GetNextRecord(string id)
	{
		return UnitOfWork.ActivitiesForOpportunity.GetNextRecord(id);
	}

	[HttpGet]
	public DataTable GetPrevious(string id)
	{
		return UnitOfWork.ActivitiesForOpportunity.GetPreviousRecord(id);
	}

	[HttpGet]
	public int GetRecord(string position, int clgCode)
	{
		return UnitOfWork.ActivitiesForOpportunity.GetRecord(position, clgCode);
	}

	[HttpGet]
	public bool CheckDoc(string clgCode)
	{
		return UnitOfWork.ActivitiesForOpportunity.CheckDoc(clgCode);
	}

	[HttpPost]
	public string DeleteOCLG([FromBody] int id)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).Activities.Delete(id, SessionUserInfo.SessionDiServer);
		}
		return UnitOfWork.ActivitiesForOpportunity.Delete(id);
	}

	[HttpGet]
	public DataTable GetActivityByClgCode(int clgCode)
	{
		return UnitOfWork.ActivitiesForOpportunity.GetActivityByClgCode(clgCode);
	}

	[HttpGet]
	public DataTable GetCountries()
	{
		return UnitOfWork.ActivitiesForOpportunity.GetCountries();
	}

	[HttpGet]
	public DataTable GetStates(string country)
	{
		return UnitOfWork.ActivitiesForOpportunity.GetStates(country);
	}
}
