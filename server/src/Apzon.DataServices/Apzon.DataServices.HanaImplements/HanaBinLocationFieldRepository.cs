using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaBinLocationFieldRepository : BinLocationFieldRepository
{
	public HanaBinLocationFieldRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, unitOfWork, dataService)
	{
	}

	public string Mapping(string sessionId, DataRow doc)
	{
		try
		{
			return string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat("{" + Function.GetJsonString("AbsEntry", doc["AbsEntry"]), Function.GetJsonString("Activated", doc["Activated"])), Function.GetJsonString("DefaultFieldName", doc["DftName"])), Function.GetJsonString("FieldNumber", doc["FldNum"])), Function.GetJsonString("FieldType", doc["FldType"])), Function.GetJsonString("Name", doc["DispName"])), "}");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "";
		}
	}

	public override string Save(string sessionId, DataTable doc)
	{
		try
		{
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			foreach (DataRow row in doc.Rows)
			{
				list.Add(new BodyRequestObject
				{
					BodyObject = Mapping(sessionId, row),
					Action = Enums.HttpAction.Update,
					ObjectType = ApzObjectTypes.oBinLocFieldActivation,
					Key = Function.ParseInt(row["AbsEntry"]),
					OutObjectKey = true
				});
			}
			return DataService.ProcessMutiTransaction(sessionId, list).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}
}
