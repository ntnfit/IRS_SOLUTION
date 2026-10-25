using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaUserDefinedTextsRepository : Repository<DataTable>, IUserDefinedTexts, IDisposable
{
	public HanaUserDefinedTextsRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	public string Save(string sessionId, DataTable dtSave)
	{
		try
		{
			if (!dtSave.IsNotNull())
			{
				return "Data save can not be null";
			}
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			for (int i = 0; i < dtSave.Rows.Count; i++)
			{
				if (Exists(Function.ParseInt(dtSave.Rows[i]["AbsEntry"])))
				{
					list.Add(new BodyRequestObject
					{
						ObjectType = ApzObjectTypes.oUserDefinedTexts,
						Action = Enums.HttpAction.Update,
						BodyObject = Mapping(dtSave.Rows[i], EnumCommandType.Update),
						Key = Function.ParseInt(dtSave.Rows[i]["AbsEntry"])
					});
				}
				else
				{
					list.Add(new BodyRequestObject
					{
						ObjectType = ApzObjectTypes.oUserDefinedTexts,
						Action = Enums.HttpAction.AddNew,
						BodyObject = Mapping(dtSave.Rows[i], EnumCommandType.Add)
					});
				}
			}
			HttpResult httpResult = DataService.ProcessMutiTransaction(sessionId, list);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				return httpResult.Message;
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
	}

	private string Mapping(DataRow row, EnumCommandType commandType)
	{
		return string.Concat(string.Concat("{" + Function.GetJsonString("TextCode", Function.ToString(row["TextCode"])), Function.GetJsonString("Text", Function.ToString(row["Text"]))), "}");
	}

	public string Remove(string sessionId, int entry)
	{
		try
		{
			HttpResult httpResult = DataService.ProcessMutiTransaction(sessionId, new List<BodyRequestObject>
			{
				new BodyRequestObject
				{
					Action = Enums.HttpAction.Delete,
					ObjectType = ApzObjectTypes.oUserDefinedTexts,
					Key = entry
				}
			});
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				return httpResult.Message;
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
	}

	public bool Exists(int absEntry)
	{
		try
		{
			string text = ExecuteScarlar("Select 1 from OPDT where \"AbsEntry\" = @absEntry;", CommandType.Text, new SqlParameter("@absEntry", absEntry));
			if (!string.IsNullOrEmpty(text) && text.Equals("1"))
			{
				return true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return false;
	}

	public void Dispose()
	{
	}
}
