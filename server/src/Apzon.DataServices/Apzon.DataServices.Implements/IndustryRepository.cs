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

public class IndustryRepository : Repository<DataTable>, IIndustry, IDisposable
{
	private readonly IUnitOfWork _unitOfWork;

	private ApzObjectTypes _objectType = ApzObjectTypes.oIndustries;

	private string _keyFieldName = "IndCode";

	protected Dictionary<string, string> DicFieldTags = new Dictionary<string, string>
	{
		{ "IndustryName", "IndName" },
		{ "IndustryDescription", "IndDesc" }
	};

	public IndustryRepository(Apzon.Services.IUnitOfWork serviceWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceWork, dataService)
	{
		_unitOfWork = unitOfWork;
	}

	public string Delete(int indCode, string sessionId)
	{
		try
		{
			return _unitOfWork.Master.Remove(indCode, sessionId, _objectType).Message;
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
			return _unitOfWork.Master.Save(dt, sessionId, _objectType, _keyFieldName, DicFieldTags, deleteOldRows: true);
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
