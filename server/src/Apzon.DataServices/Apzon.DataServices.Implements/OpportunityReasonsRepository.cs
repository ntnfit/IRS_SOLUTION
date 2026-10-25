using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class OpportunityReasonsRepository : Repository<DataTable>, IOpportunityReasons, IDisposable
{
	private readonly IUnitOfWork _unitOfWork;

	protected ApzObjectTypes _objectType = ApzObjectTypes.oOpportunityReasons;

	private string _serviceName = "SalesOpportunityReasonsSetupService";

	private string _keyFieldName = "Num";

	protected Dictionary<string, string> DicFieldTags = new Dictionary<string, string>
	{
		{ "SequenceNo", "Num" },
		{ "Description", "Descript" },
		{ "Sort", "SortOrder" }
	};

	public OpportunityReasonsRepository(Apzon.Services.IUnitOfWork servUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(servUnitOfWork, dataService)
	{
		_unitOfWork = unitOfWork;
	}

	public string Delete(int code, string sessionId)
	{
		try
		{
			return _unitOfWork.Master.RemoveService(code, sessionId, _objectType, _serviceName).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public HttpResult Save(DataTable dt, string sessionId)
	{
		try
		{
			return _unitOfWork.Master.SaveService(dt, sessionId, _objectType, _serviceName, _keyFieldName, DicFieldTags, deleteOldRows: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void Dispose()
	{
		throw new NotImplementedException();
	}
}
