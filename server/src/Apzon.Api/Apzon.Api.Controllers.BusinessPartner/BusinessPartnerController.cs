using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Entities.Models.BusinessPartners;

namespace Apzon.Api.Controllers.BusinessPartner;

public class BusinessPartnerController : BaseApiController
{
	[HttpGet]
	public DataTable Get()
	{
		return UnitOfWork.BP.Table();
	}

	[HttpGet]
	public List<BusinessPartnersImpl> ListCustomer()
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "INVALIDSESSION");
					return new List<BusinessPartnersImpl>();
				}
				string userName = SessionUserInfo.UserName;
				return UnitOfWork.BP.GetCustomer(userName);
			}
			return UnitOfWork.BP.GetCustomer();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new List<BusinessPartnersImpl>();
	}

	[HttpGet]
	public List<BusinessPartnersImpl> GetVendor()
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "INVALIDSESSION");
					return new List<BusinessPartnersImpl>();
				}
				string userName = SessionUserInfo.UserName;
				return UnitOfWork.BP.GetVendor(userName);
			}
			return UnitOfWork.BP.GetVendor("manager");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new List<BusinessPartnersImpl>();
	}

	[HttpGet]
	public List<BusinessPartnersImpl> GetLead()
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "INVALIDSESSION");
					return new List<BusinessPartnersImpl>();
				}
				string userName = SessionUserInfo.UserName;
				return UnitOfWork.BP.GetLead(userName);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new List<BusinessPartnersImpl>();
	}

	[HttpGet]
	public List<BusinessPartnersImpl> GetBp()
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "INVALIDSESSION");
					return new List<BusinessPartnersImpl>();
				}
				string userName = SessionUserInfo.UserName;
				return UnitOfWork.BP.GetBp("", userName);
			}
			return UnitOfWork.BP.GetBp("", "manager");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new List<BusinessPartnersImpl>();
	}

	[HttpGet]
	public List<BusinessPartnersImpl> GetListContactPerson(string cardCode)
	{
		return UnitOfWork.BP.GetListContactPerson(cardCode);
	}

	[HttpGet]
	public List<BusinessPartnersImpl> GetContactPerson_byCntctCode(string cardCode, string cntctCode)
	{
		return UnitOfWork.BP.GetContactPerson_byCntctCode(cardCode, cntctCode);
	}

	[HttpGet]
	public List<BusinessPartnersImpl> GetTelePh_ByCotacPerson(string cardCode, string cntctCode)
	{
		return UnitOfWork.BP.GetTelePh_ByCotacPerson(cardCode, cntctCode);
	}

	[HttpGet]
	public DataTable GetTel_ByContactPerson(string cardCode, string cntctCode)
	{
		return UnitOfWork.BP.GetTel_ByContactPerson(cardCode, cntctCode);
	}

	[HttpGet]
	public List<BusinessPartnersImpl> GetCardType_ByCardCode(string cardCode, string clgCode)
	{
		return UnitOfWork.BP.GetCardType_ByCardCode(cardCode, clgCode);
	}

	[HttpGet]
	public DataTable ListBPByCardType(string cardType, int BPLId)
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "INVALIDSESSION");
					return new DataTable();
				}
				string userName = SessionUserInfo.UserName;
				return UnitOfWork.BP.ListBPByCardType(cardType, userName, BPLId);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}
}
