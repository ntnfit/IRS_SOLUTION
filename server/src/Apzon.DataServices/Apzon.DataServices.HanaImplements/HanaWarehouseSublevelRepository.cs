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

public class HanaWarehouseSublevelRepository : WarehouseSublevelRepository
{
	public HanaWarehouseSublevelRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, unitOfWork, dataService)
	{
	}

	public override HttpResult Save(string sessionId, DataTable doc)
	{
		try
		{
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			foreach (DataRow row in doc.Rows)
			{
				string text = Function.ToString(row["Type"]);
				Enums.HttpAction action = ((text == "A") ? Enums.HttpAction.AddNew : ((!(text == "U")) ? Enums.HttpAction.Delete : Enums.HttpAction.Update));
				list.Add(new BodyRequestObject
				{
					BodyObject = Mapping(sessionId, row),
					Action = action,
					ObjectType = ApzObjectTypes.oWarehouseSublevel,
					Key = Function.ParseInt(row["AbsEntry"]),
					OutObjectKey = true
				});
			}
			HttpResult httpResult = DataService.ProcessMutiTransaction(sessionId, list);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				return httpResult;
			}
			return new HttpResult(Enums.MessageCode.Success, "", Function.ParseInt(httpResult.Content));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public string Mapping(string sessionId, DataRow doc)
	{
		try
		{
			string text = Function.ToString(doc["Type"]);
			string text2 = "{";
			if (text != "A")
			{
				text2 += Function.GetJsonString("AbsEntry", doc["AbsEntry"]);
			}
			if (text == "A" || text == "U")
			{
				text2 += Function.GetJsonString("Code", Function.ToString(doc["SLCode"]).ToUpper());
				text2 += Function.GetJsonString("Description", doc["Descr"]);
				text2 += Function.GetJsonString("WarehouseSublevel", doc["FldAbs"]);
			}
			return text2 + "}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "";
		}
	}
}
