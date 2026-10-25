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

public class HanaUnitOfMeasurementsRepository : UnitOfMeasurementsRepository
{
	public HanaUnitOfMeasurementsRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	public override HttpResult SaveData(DataTable dtSave, string sessionId)
	{
		try
		{
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			sessionId = GetSessionId(sessionId);
			if (dtSave.IsNotNull())
			{
				DataTable currentData = GetCurrentData();
				if (currentData.IsNotNull())
				{
					List<int> lstUomEntry = (dtSave.IsNotNull() ? (from t in dtSave.AsEnumerable()
						select Function.ParseInt(t["UomEntry"])).ToList() : new List<int>());
					List<DataRow> list2 = (from t in currentData.AsEnumerable()
						where !lstUomEntry.Contains(Function.ParseInt(t["UomEntry"]))
						select t).ToList();
					if (list2.Any())
					{
						foreach (DataRow item in list2)
						{
							list.Add(new BodyRequestObject
							{
								ObjectType = ApzObjectTypes.oUoM,
								OutObjectKey = true,
								Action = Enums.HttpAction.Delete,
								Key = Function.ParseInt(item["UomEntry"])
							});
						}
					}
				}
				if (dtSave.IsNotNull())
				{
					int i;
					for (i = 0; i < dtSave.Rows.Count; i++)
					{
						if (currentData.IsNotNull() && currentData.AsEnumerable().Any((DataRow t) => Function.ToString(t["UomEntry"]).Equals(Function.ToString(dtSave.Rows[i]["UomEntry"]))))
						{
							list.Add(new BodyRequestObject
							{
								ObjectType = ApzObjectTypes.oUoM,
								OutObjectKey = true,
								Action = Enums.HttpAction.Update,
								BodyObject = MappingJson(dtSave.Rows[i]),
								Key = Function.ParseInt(dtSave.Rows[i]["UomEntry"])
							});
						}
						else
						{
							list.Add(new BodyRequestObject
							{
								ObjectType = ApzObjectTypes.oUoM,
								OutObjectKey = true,
								Action = Enums.HttpAction.AddNew,
								BodyObject = MappingJson(dtSave.Rows[i])
							});
						}
					}
				}
				return DataService.ProcessMutiTransaction(sessionId, list);
			}
			return new HttpResult(Enums.MessageCode.DataIsEmpty, "Data not provide");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private string MappingJson(DataRow rowData)
	{
		try
		{
			string text = "{";
			if (!string.IsNullOrEmpty(Function.ToString(rowData["UomCode"])))
			{
				text += Function.GetJsonString("Code", Function.ToString(rowData["UomCode"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(rowData["UomName"])))
			{
				text += Function.GetJsonString("Name", Function.ToString(rowData["UomName"]));
			}
			if (Function.ParseDouble(rowData["Length1"]) > 0.0)
			{
				text += Function.GetJsonString("Length1", Function.ParseDouble(rowData["Length1"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(rowData["Len1Unit"])))
			{
				text += Function.GetJsonString("Length1Unit", Function.ToString(rowData["Len1Unit"]));
			}
			if (Function.ParseDouble(rowData["Length2"]) > 0.0)
			{
				text += Function.GetJsonString("Length2", Function.ParseDouble(rowData["Length2"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(rowData["Len2Unit"])))
			{
				text += Function.GetJsonString("Length2Unit", Function.ToString(rowData["Len2Unit"]));
			}
			if (Function.ParseDouble(rowData["Width1"]) > 0.0)
			{
				text += Function.GetJsonString("Width1", Function.ParseDouble(rowData["Width1"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(rowData["Wdth1Unit"])))
			{
				text += Function.GetJsonString("Width1Unit", Function.ToString(rowData["Wdth1Unit"]));
			}
			if (Function.ParseDouble(rowData["Width2"]) > 0.0)
			{
				text += Function.GetJsonString("Width2", Function.ParseDouble(rowData["Width2"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(rowData["Wdth2Unit"])))
			{
				text += Function.GetJsonString("Width2Unit", Function.ToString(rowData["Wdth2Unit"]));
			}
			if (Function.ParseDouble(rowData["Height1"]) > 0.0)
			{
				text += Function.GetJsonString("Height1", Function.ParseDouble(rowData["Height1"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(rowData["Hght1Unit"])))
			{
				text += Function.GetJsonString("Height1Unit", Function.ToString(rowData["Hght1Unit"]));
			}
			if (Function.ParseDouble(rowData["Height2"]) > 0.0)
			{
				text += Function.GetJsonString("Height2", Function.ParseDouble(rowData["Height2"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(rowData["Hght2Unit"])))
			{
				text += Function.GetJsonString("Height2Unit", Function.ToString(rowData["Hght2Unit"]));
			}
			if (Function.ParseDouble(rowData["Volume"]) > 0.0)
			{
				text += Function.GetJsonString("Volume", Function.ParseDouble(rowData["Volume"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(rowData["VolUnit"])))
			{
				text += Function.GetJsonString("VolumeUnit", Function.ToString(rowData["VolUnit"]));
			}
			if (Function.ParseDouble(rowData["Weight1"]) > 0.0)
			{
				text += Function.GetJsonString("Weight1", Function.ParseDouble(rowData["Weight1"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(rowData["WghtUnit"])))
			{
				text += Function.GetJsonString("Weight1Unit", Function.ToString(rowData["WghtUnit"]));
			}
			if (Function.ParseDouble(rowData["Weight2"]) > 0.0)
			{
				text += Function.GetJsonString("Weight2", Function.ParseDouble(rowData["Weight2"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(rowData["Wght2Unit"])))
			{
				text += Function.GetJsonString("Weight2Unit", Function.ToString(rowData["Wght2Unit"]));
			}
			return text + "}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}

	protected DataTable GetCurrentData()
	{
		try
		{
			string query = "SELECT \"UomEntry\", \"UomCode\" FROM OUOM;";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}
}
