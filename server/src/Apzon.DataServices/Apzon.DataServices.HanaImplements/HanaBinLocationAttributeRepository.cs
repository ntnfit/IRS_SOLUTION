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

public class HanaBinLocationAttributeRepository : BinLocationAttributeRepository
{
	public HanaBinLocationAttributeRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, unitOfWork, dataService)
	{
	}

	public override string Save(string sessionId, DataTable doc)
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
					Key = Function.ParseInt(row["AbsEntry"]),
					ObjectType = ApzObjectTypes.oBinLocAttribute,
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

	public string Mapping(string sessionId, DataRow doc)
	{
		try
		{
			string text = "{";
			if (Function.ToString(doc["Type"]) != "A")
			{
				text += Function.GetJsonString("AbsEntry", doc["AbsEntry"]);
			}
			if (Function.ToString(doc["Type"]) != "")
			{
				text += Function.GetJsonString("Attribute", doc["FldAbs"]);
				text += Function.GetJsonString("Code", doc["AttrValue"]);
			}
			return text + "}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "";
		}
	}
}
