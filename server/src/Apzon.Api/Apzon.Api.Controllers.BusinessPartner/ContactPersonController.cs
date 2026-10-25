using System;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.MobileApp;

namespace Apzon.Api.Controllers.BusinessPartner;

[ValidateLoginSapExceptionHandling]
public class ContactPersonController : BaseApiController
{
	private string _messageCodePrefix = "213001";

	[HttpGet]
	public DataTable ShowContactPerson(string code)
	{
		return UnitOfWork.ContactPerson.ShowContactPerson(code);
	}

	[HttpGet]
	public int GetMaxCode()
	{
		return UnitOfWork.ContactPerson.GetMaxCode();
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string SaveDoc([FromBody] ContactPersonImpl doc)
	{
		return UnitOfWork.ContactPerson.SaveDoc(doc);
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string DeleteDoc([FromBody] string code)
	{
		return UnitOfWork.ContactPerson.DeleteDoc(code);
	}

	[HttpPost]
	public ResponseResult UpdateContact(DataTable data)
	{
		try
		{
			data.IsNotNull();
			DataRow dataRow = data.Rows[0];
			string code = Function.ToString(dataRow["cardCode"]);
			string text = Function.ToString(dataRow["cntctCode"]);
			if (UnitOfWork == null)
			{
				return new ResponseResult
				{
					Message = "Invalid SesionId",
					MessageCode = _messageCodePrefix + "401"
				};
			}
			ContactPersonImpl contactPersonImpl = new ContactPersonImpl();
			if (!string.IsNullOrEmpty(text))
			{
				contactPersonImpl = Function.DataRowToEntity<ContactPersonImpl>(UnitOfWork.ContactPerson.ShowContactPerson(code).Select("CntctCode=" + Function.ToString(text))[0]);
			}
			else
			{
				int maxCode = UnitOfWork.ContactPerson.GetMaxCode();
				contactPersonImpl.CntctCode = ((maxCode == 0) ? maxCode : (maxCode + 1));
				contactPersonImpl.CardCode = Function.ToString(dataRow["cardCode"]);
			}
			contactPersonImpl.Active = "Y";
			contactPersonImpl.Cellolar = Function.ToString(dataRow["cellular"]);
			contactPersonImpl.Name = Function.ToString(dataRow["name"]);
			contactPersonImpl.E_MailL = Function.ToString(dataRow["email"]);
			contactPersonImpl.Tel1 = Function.ToString(dataRow["tel1"]);
			contactPersonImpl.Tel2 = Function.ToString(dataRow["tel2"]);
			contactPersonImpl.Address = Function.ToString(dataRow["address"]);
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return new ResponseResult
					{
						MessageCode = _messageCodePrefix + "401",
						Message = "Invalid SessionId"
					};
				}
				BrokerService.GetUnitOfWork(SessionId);
				string text2 = UnitOfWork.ContactPerson.SaveDoc(contactPersonImpl);
				if (string.IsNullOrEmpty(Function.ToString(text2)))
				{
					return new ResponseResult
					{
						MessageCode = "0",
						Message = ""
					};
				}
				return new ResponseResult
				{
					MessageCode = "101",
					Message = Function.ToString(text2)
				};
			}
			return new ResponseResult
			{
				MessageCode = _messageCodePrefix + "405",
				Message = "Function not support"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = _messageCodePrefix + "107"
			};
		}
	}
}
