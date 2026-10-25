using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;

namespace Apzon.Api.Controllers.Service;

public class ServiceCallsController : BaseApiController
{
	[HttpGet]
	public DataTable LoadServiceCalls(string cardCode, int handleBy, int callType, int callId, string tag, string fromDate, string toDate)
	{
		return UnitOfWork.ServiceCalls.GetServiceCalls(cardCode, handleBy, callType, callId, tag, Function.ParseDateTime(fromDate, "ddMMyyyy"), Function.ParseDateTime(toDate, "ddMMyyyy"));
	}

	[HttpGet]
	public DataTable GetContactPerson(string cardCode)
	{
		return UnitOfWork.ServiceCalls.GetContactPerson(cardCode);
	}

	[HttpGet]
	public DataTable GetSolutionByCallId(int callId)
	{
		return UnitOfWork.ServiceCalls.GetSolutionByCallId(callId);
	}

	[HttpGet]
	public DataTable GetDocumentByCallId(int callId)
	{
		return UnitOfWork.ServiceCalls.GetDocumentByCallId(callId);
	}

	[HttpGet]
	public string RemoveDoc(int callId)
	{
		return UnitOfWork.ServiceCalls.RemoveDoc(callId);
	}

	[HttpPost]
	public string SaveDoc(DataSet dt)
	{
		return UnitOfWork.ServiceCalls.SaveDoc(dt);
	}
}
