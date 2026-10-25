using System;
using System.Data;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaUnitOfMeasureGroupsRepository : UnitOfMeasureGroupsRepository
{
	public HanaUnitOfMeasureGroupsRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, unitOfWork, dataService)
	{
	}

	private string Mapping(DataRow row, DataTable dt)
	{
		try
		{
			string text = "{";
			if (row != null && dt.IsNotNull())
			{
				text += Function.GetJsonString("AbsEntry", Function.ParseInt(row["UgpEntry"]));
				text += Function.GetJsonString("Code", Function.ToString(row["UgpCode"]));
				text += Function.GetJsonString("Name", Function.ToString(row["UgpName"]));
				text += Function.GetJsonString("BaseUoM", Function.ToString(row["BaseUom"]));
				text += "\"UoMGroupDefinitionCollection\":[";
				foreach (DataRow row2 in dt.Rows)
				{
					text += "{";
					text += Function.GetJsonString("AlternateUoM", Function.ParseInt(row2["UomEntry"]));
					text += Function.GetJsonString("AlternateQuantity", Function.ParseDecimal(row2["AltQty"]));
					text += Function.GetJsonString("BaseQuantity", Function.ParseDecimal(row2["BaseQty"]));
					text += Function.GetJsonString("WeightFactor", Function.ParseInt(row2["WghtFactor"]));
					text += Function.GetJsonString("UdfFactor", Function.ParseInt(row2["UdfFactor"]));
					text += "},";
				}
				text += "]";
				text += "}";
			}
			return text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return string.Empty;
		}
	}

	public override HttpResult SaveData(Tuple<DataTable, DataTable, Enums.HttpAction> tupData, string sessionId)
	{
		try
		{
			if (tupData.Item3 == Enums.HttpAction.Delete)
			{
				return DataService.RemoveObject(ApzObjectTypes.oUoMDefGroups, sessionId, Function.ParseInt(tupData.Item1.Rows[0]["UgpEntry"]));
			}
			if (tupData.Item3 == Enums.HttpAction.AddNew)
			{
				return DataService.AddObject(ApzObjectTypes.oUoMDefGroups, sessionId, Mapping(tupData.Item1.Rows[0], tupData.Item2));
			}
			return DataService.UpdateObject(ApzObjectTypes.oUoMDefGroups, sessionId, Mapping(tupData.Item1.Rows[0], tupData.Item2), Function.ParseInt(tupData.Item1.Rows[0]["UgpEntry"]));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
