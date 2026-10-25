using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaTeamsRepository : TeamsRepository
{
	public HanaTeamsRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	public override HttpResult Save(DataTable dtSource, string sessionId)
	{
		HttpResult httpResult = new HttpResult();
		try
		{
			DataTable dtCurrentSource = ServiceUnitOfWork.States.GetTeamSetup();
			if (!dtSource.IsNotNull() && !dtCurrentSource.IsNotNull())
			{
				httpResult = new HttpResult(Enums.MessageCode.SaveError);
				httpResult.Message = "Please enter data first";
			}
			else
			{
				List<BodyRequestObject> list = new List<BodyRequestObject>();
				if (dtCurrentSource.IsNotNull())
				{
					List<int> list2 = (from t in dtCurrentSource.AsEnumerable()
						where !dtSource.IsNotNull() || dtSource.AsEnumerable().All((DataRow x) => !Function.ToString(x["teamID"]).Equals(Function.ToString(t["teamID"])))
						select Function.ParseInt(t["teamID"])).ToList();
					for (int num = 0; num < list2.Count; num++)
					{
						list.Add(new BodyRequestObject
						{
							ObjectType = ApzObjectTypes.oTeams,
							OutObjectKey = true,
							Key = list2[num],
							Action = Enums.HttpAction.Delete
						});
					}
					if (dtSource.IsNotNull())
					{
						List<DataRow> list3 = (from t in dtSource.AsEnumerable()
							where dtCurrentSource.AsEnumerable().Any((DataRow x) => Function.ToString(x["teamID"]).Equals(Function.ToString(t["teamID"])))
							select t).ToList();
						for (int num2 = 0; num2 < list3.Count; num2++)
						{
							list.Add(new BodyRequestObject
							{
								ObjectType = ApzObjectTypes.oTeams,
								OutObjectKey = true,
								Key = Function.ParseInt(list3[num2]["teamID"]),
								Action = Enums.HttpAction.Update,
								BodyObject = Mapping(list3[num2])
							});
						}
					}
				}
				if (dtSource.IsNotNull())
				{
					List<DataRow> list4 = (from t in dtSource.AsEnumerable()
						where !dtCurrentSource.IsNotNull() || dtCurrentSource.AsEnumerable().All((DataRow x) => !Function.ToString(x["teamID"]).Equals(Function.ToString(t["teamID"])))
						select t).ToList();
					for (int num3 = 0; num3 < list4.Count; num3++)
					{
						list.Add(new BodyRequestObject
						{
							ObjectType = ApzObjectTypes.oTeams,
							OutObjectKey = true,
							Action = Enums.HttpAction.AddNew,
							BodyObject = Mapping(list4[num3])
						});
					}
				}
				HttpResult httpResult2 = DataService.ProcessMutiTransaction(sessionId, list);
				if (!string.IsNullOrEmpty(httpResult2.Message))
				{
					Logging.Write("ERROR", "Save Teams " + httpResult2.Message);
					httpResult = new HttpResult(Enums.MessageCode.SaveError);
					httpResult.Message = httpResult2.Message;
				}
				else
				{
					httpResult = new HttpResult(Enums.MessageCode.Success);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			httpResult = new HttpResult(Enums.MessageCode.ExceptionMessage);
			httpResult.Message = ex.Message;
		}
		return httpResult;
	}

	private string Mapping(DataRow rowData)
	{
		string text = "{";
		if (Function.ParseInt(rowData["teamID"]) > 0)
		{
			text += Function.GetJsonString("TeamID", Function.ParseInt(rowData["teamID"]));
		}
		text += Function.GetJsonString("TeamName", Function.ToString(rowData["name"]));
		text += Function.GetJsonString("Description", Function.ToString(rowData["descriptio"]));
		return text + "}";
	}

	public override HttpResult Delete(int id, string sessionId)
	{
		try
		{
			return DataService.RemoveObject(ApzObjectTypes.oTeams, sessionId, id);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage)
			{
				Message = ex.Message
			};
		}
	}
}
