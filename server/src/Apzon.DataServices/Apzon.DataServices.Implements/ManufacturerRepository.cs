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

public class ManufacturerRepository : Repository<DataTable>, IManufacturer, IDisposable
{
	protected readonly IUnitOfWork _unitOfWork;

	protected ApzObjectTypes _objectType = ApzObjectTypes.oManufacturers;

	protected string _keyFieldName = "FirmCode";

	protected Dictionary<string, string> DicFieldTags = new Dictionary<string, string> { { "ManufacturerName", "FirmName" } };

	public ManufacturerRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
		_unitOfWork = unitOfWork;
	}

	public virtual string Delete(int indCode, string sessionId)
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

	public virtual HttpResult Delete(DataTable dt, string sessionId)
	{
		return new HttpResult();
	}

	public virtual HttpResult Save(DataTable dt, string sessionId)
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
