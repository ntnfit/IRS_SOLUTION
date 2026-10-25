using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaManufacturerRepository : ManufacturerRepository
{
	public HanaManufacturerRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, unitOfWork, dataService)
	{
	}

	public virtual bool CheckExists(string firmCode, string sessionId)
	{
		string query = "select 1 from OMRC where \"FirmCode\"=@firmCode;";
		try
		{
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@firmCode", firmCode)).Rows.Count > 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return false;
		}
	}

	public virtual string Mapping(DataRow doc, string sessionId)
	{
		string text = "{";
		try
		{
			text += Function.GetJsonString("Code", Function.ParseInt(doc["FirmCode"]));
			text += Function.GetJsonString("ManufacturerName", doc["FirmName"]);
			return text + "}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "";
		}
	}

	public override HttpResult Save(DataTable dt, string sessionId)
	{
		string text = "";
		List<BodyRequestObject> list = new List<BodyRequestObject>();
		try
		{
			foreach (DataRow row in dt.Rows)
			{
				if (CheckExists(Function.ToString(row["FirmCode"]), sessionId))
				{
					text = Mapping(row, sessionId);
					list.Add(new BodyRequestObject
					{
						BodyObject = text,
						Action = Enums.HttpAction.Update,
						ObjectType = ApzObjectTypes.oManufacturers,
						Key = Function.ParseInt(row["FirmCode"]),
						OutObjectKey = true
					});
				}
				else
				{
					text = Mapping(row, sessionId);
					list.Add(new BodyRequestObject
					{
						Action = Enums.HttpAction.AddNew,
						ObjectType = ApzObjectTypes.oManufacturers,
						BodyObject = text
					});
				}
			}
			return DataService.ProcessMutiTransaction(sessionId, list);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult Delete(DataTable dt, string sessionId)
	{
		List<BodyRequestObject> list = new List<BodyRequestObject>();
		try
		{
			foreach (DataRow row in dt.Rows)
			{
				list.Add(new BodyRequestObject
				{
					Action = Enums.HttpAction.Delete,
					ObjectType = ApzObjectTypes.oManufacturers,
					Key = Function.ParseInt(row["FirmCode"]),
					OutObjectKey = true
				});
			}
			return DataService.ProcessMutiTransaction(sessionId, list);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
