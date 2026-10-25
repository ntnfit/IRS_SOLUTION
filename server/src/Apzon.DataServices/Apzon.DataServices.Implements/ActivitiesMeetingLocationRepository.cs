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

public class ActivitiesMeetingLocationRepository : Repository<DataTable>, IActivitiesMeetingLocation, IDisposable
{
	protected readonly IUnitOfWork _unitOfWork;

	protected ApzObjectTypes _objectType = ApzObjectTypes.oActivityLocations;

	protected string _keyFieldName = "Code";

	protected Dictionary<string, string> DicFieldTags = new Dictionary<string, string> { { "Name", "Name" } };

	public ActivitiesMeetingLocationRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
		_unitOfWork = unitOfWork;
	}

	public string Delete(int code, string sessionId)
	{
		try
		{
			return _unitOfWork.Master.Remove(code, sessionId, _objectType).Message;
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
