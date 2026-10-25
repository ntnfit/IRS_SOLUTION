using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaTerritoryRepository : TerritoryRepository
{
	public HanaTerritoryRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	public int ExistsHana(int territryID, string sessionId)
	{
		try
		{
			if (territryID == 0)
			{
				return -1;
			}
			return Function.ParseInt(ExecuteScarlar("select count(1) from OTER where \"territryID\" = @territryID;", CommandType.Text, new SqlParameter("@territryID", territryID)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return -1;
	}

	public override string Mapping(DataTable dt, string sessionId)
	{
		try
		{
			string text = "{";
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				int num = Function.ParseInt(dt.Rows[i]["territryID"]);
				text += Function.GetJsonString("TerritoryID", Function.ToString(num));
				text += Function.GetJsonString("Description", Function.ToString(dt.Rows[i]["descript"]));
				text += Function.GetJsonString("Parent", Function.ParseInt(dt.Rows[i]["parent"]));
				text += Function.GetJsonString("LocationIndex", Function.ParseInt(dt.Rows[i]["lindex"]));
				text = ((!(Function.ToString(dt.Rows[i]["inactive"]) == "Y")) ? (text + Function.GetJsonString("Inactive", "tNO")) : (text + Function.GetJsonString("Inactive", "tYES")));
			}
			text += "}";
			return text.ToString();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "ERR:" + ex.Message;
		}
	}

	public override string GetDeleteXML(DataTable dtSource)
	{
		try
		{
			StringBuilder stringBuilder = new StringBuilder();
			DataTable dataTable = ExecuteDataTable("Select \"territryID\",\"parent\" from OTER;", CommandType.Text);
			int num = 0;
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				Dictionary<int, int> dictionary = new Dictionary<int, int>();
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					int id = Function.ParseInt(dataTable.Rows[i]["territryID"]);
					int value = Function.ParseInt(dataTable.Rows[i]["parent"]);
					if (!dtSource.AsEnumerable().Any((DataRow r) => Function.ParseInt(r["territryID"]) == id))
					{
						dictionary.Add(id, value);
					}
				}
				List<int> lstChild = new List<int>();
				foreach (KeyValuePair<int, int> item in dictionary)
				{
					GetAllChildID(item.Key, ref lstChild, dictionary);
				}
				foreach (int item2 in lstChild)
				{
					dictionary.Remove(item2);
				}
			}
			if (num > 0)
			{
				return stringBuilder.ToString();
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "";
		}
	}

	public override int GetNextKey()
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("Select \"AutoKey\" from ONNM where \"ObjectCode\"='200';", CommandType.Text);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0][0]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return 0;
		}
	}

	public override string SaveData(DataTable dtS, string sessionId)
	{
		try
		{
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			DataView defaultView = dtS.DefaultView;
			defaultView.Sort = "territryID ASC";
			DataTable dataTable = defaultView.ToTable();
			int nextKey = GetNextKey();
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			int num = 0;
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				int id = Function.ParseInt(dataTable.Rows[i]["territryID"]);
				if (ExistsHana(id, sessionId) != 0)
				{
					continue;
				}
				EnumerableRowCollection<DataRow> source = from r in dataTable.AsEnumerable()
					where Function.ParseInt(r["parent"]) == id
					select r;
				if (source.Any())
				{
					DataRow[] array = source.ToArray();
					int value = num + nextKey;
					for (int num2 = 0; num2 < array.Length; num2++)
					{
						dictionary.Add(Function.ParseInt(array[num2]["territryID"]), value);
					}
				}
				num++;
			}
			foreach (KeyValuePair<int, int> item in dictionary)
			{
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow r) => Function.ParseInt(r["territryID"]) == item.Key);
				if (dataRow != null)
				{
					dataRow["parent"] = item.Value;
				}
			}
			dataTable.AcceptChanges();
			for (int num3 = 0; num3 < dataTable.Rows.Count; num3++)
			{
				int territryID = Function.ParseInt(dataTable.Rows[num3]["territryID"]);
				if (ExistsHana(territryID, sessionId) == 0)
				{
					DataTable dataTable2 = dataTable.Copy().Clone();
					DataRow row = dataTable.Rows[num3];
					dataTable2.ImportRow(row);
					string bodyObject = Mapping(dataTable2, sessionId);
					list.Add(new BodyRequestObject
					{
						ObjectType = ApzObjectTypes.oTerritories,
						OutObjectKey = true,
						Action = Enums.HttpAction.AddNew,
						BodyObject = bodyObject
					});
				}
				else
				{
					DataTable dataTable3 = dataTable.Copy().Clone();
					DataRow row2 = dataTable.Rows[num3];
					dataTable3.ImportRow(row2);
					string bodyObject2 = Mapping(dataTable3, sessionId);
					list.Add(new BodyRequestObject
					{
						ObjectType = ApzObjectTypes.oTerritories,
						OutObjectKey = true,
						Action = Enums.HttpAction.Update,
						Key = dataTable3.Rows[0]["territryID"],
						BodyObject = bodyObject2
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
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}
}
