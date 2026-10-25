using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaUserTablesDefinedRepository : UserTablesDefinedRepository
{
	public HanaUserTablesDefinedRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, unitOfWork, dataService)
	{
	}

	protected override string GetListTablesUDTQuery()
	{
		return "select STRING_AGG(\"TableName\", ',') as \"TableName\" from OUTB;";
	}

	protected override string Mapping(DataRow row, EnumCommandType commandType, string sessionId)
	{
		try
		{
			string text = "{";
			text += Function.GetJsonString("TableName", Function.ToString(row["TableName"]));
			text += Function.GetJsonString("TableDescription", Function.ToString(row["Descr"]));
			switch (Function.ParseInt(row["ObjectType"]))
			{
			case 0:
				text += Function.GetJsonString("TableType", "bott_NoObject");
				break;
			case 1:
				text += Function.GetJsonString("TableType", "bott_MasterData");
				break;
			case 2:
				text += Function.GetJsonString("TableType", "bott_MasterDataLines");
				break;
			case 3:
				text += Function.GetJsonString("TableType", "bott_Document");
				break;
			case 4:
				text += Function.GetJsonString("TableType", "bott_DocumentLines");
				break;
			}
			text = ((!Function.ToString(row["Archivable"]).Equals("Y")) ? (text + Function.GetJsonString("Archivable", "tNO")) : (text + Function.GetJsonString("Archivable", (Function.ParseInt(row["ObjectType"]) == 0) ? "tNO" : "tYES")));
			text += Function.GetJsonString("ArchiveDateField", Function.ToString(row["ArchivDate"]));
			return text + "}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return string.Empty;
		}
	}

	public override string SaveData(DataTable dt, string sessionId)
	{
		try
		{
			ApzObjectTypes objectType = ApzObjectTypes.oUserTables;
			string empty = string.Empty;
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			foreach (DataRow row in dt.Rows)
			{
				empty = Mapping(row, EnumCommandType.Update, sessionId);
				if (!CheckTableUDTExists(Function.ToString(row["TableName"])))
				{
					list.Add(new BodyRequestObject
					{
						Action = Enums.HttpAction.AddNew,
						BodyObject = empty,
						ObjectType = objectType
					});
				}
				else
				{
					empty = Mapping(row, EnumCommandType.Update, sessionId);
					list.Add(new BodyRequestObject
					{
						Action = Enums.HttpAction.Update,
						BodyObject = empty,
						Key = Function.ToString(row["TableName"]),
						ObjectType = objectType,
						OutObjectKey = true
					});
				}
			}
			List<string> listTablesUDT = GetListTablesUDT();
			List<string> list2 = (from t in dt.AsEnumerable()
				select Function.ToString(t["TableName"])).ToList();
			if (listTablesUDT.Count > 0)
			{
				foreach (string item in listTablesUDT)
				{
					if (!string.IsNullOrEmpty(item) && !list2.Contains(item))
					{
						list.Add(new BodyRequestObject
						{
							Action = Enums.HttpAction.Delete,
							Key = item,
							ObjectType = objectType,
							OutObjectKey = true
						});
					}
				}
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
