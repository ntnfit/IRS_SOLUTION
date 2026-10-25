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

public class HanaBatchNumberDetailsRepository : BatchNumberDetailsRepository
{
	public HanaBatchNumberDetailsRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, unitOfWork, dataService)
	{
	}

	public override string Save(DataSet ds, string sessionId)
	{
		try
		{
			if (ds.Tables.Count > 0 && ds.Tables[0].IsNotNull())
			{
				int num = Function.ParseInt(ds.Tables[0].Rows[0]["AbsEntry"]);
				if (num <= 0)
				{
					return "No matching record found";
				}
				string jsonObject = Mapping(ds.Tables[0].Rows[0]);
				HttpResult httpResult = DataService.ProcessJson(ApzObjectTypes.oBatchDetails, sessionId, jsonObject, Enums.HttpAction.Update, num);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					return httpResult.Message;
				}
				return "";
			}
			return "Data is empty";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string Mapping(DataRow dr)
	{
		try
		{
			string text = "{ ";
			if (Function.ParseInt(dr["AbsEntry"]) > 0)
			{
				text += Function.GetJsonString("DocEntry", Function.ParseInt(dr["AbsEntry"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(dr["ItemCode"])))
			{
				text += Function.GetJsonString("ItemCode", Function.ToString(dr["ItemCode"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(dr["itemName"])))
			{
				text += Function.GetJsonString("ItemDescription", Function.ParseInt(dr["itemName"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(dr["DistNumber"])))
			{
				text += Function.GetJsonString("Batch", Function.ToString(dr["DistNumber"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(dr["MnfSerial"])))
			{
				text += Function.GetJsonString("BatchAttribute1", Function.ToString(dr["MnfSerial"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(dr["LotNumber"])))
			{
				text += Function.GetJsonString("BatchAttribute2", Function.ToString(dr["LotNumber"]));
			}
			if (Function.ParseInt(dr["Status"]) >= 0)
			{
				text = Function.ParseInt(dr["Status"]) switch
				{
					1 => text + Function.GetJsonString("Status", "bdsStatus_NotAccessible"), 
					2 => text + Function.GetJsonString("Status", "bdsStatus_Locked"), 
					_ => text + Function.GetJsonString("Status", "bdsStatus_Released"), 
				};
			}
			if (Function.ParseDateTime(dr["InDate"]).HasValue)
			{
				text += Function.GetJsonString("AdmissionDate", Convert.ToDateTime(dr["InDate"]).ToString("yyyy-MM-dd"));
			}
			if (Function.ParseDateTime(dr["MnfDate"]).HasValue)
			{
				text += Function.GetJsonString("ManufacturingDate", Convert.ToDateTime(dr["MnfDate"]).ToString("yyyy-MM-dd"));
			}
			if (Function.ParseDateTime(dr["ExpDate"]).HasValue)
			{
				text += Function.GetJsonString("ExpirationDate", Convert.ToDateTime(dr["ExpDate"]).ToString("yyyy-MM-dd"));
			}
			if (!string.IsNullOrEmpty(Function.ToString(dr["Notes"])))
			{
				text += Function.GetJsonString("Details", Function.ToString(dr["Notes"]));
			}
			if (Function.ParseInt(dr["SysNumber"]) > 0)
			{
				text += Function.GetJsonString("SystemNumber", Function.ParseInt(dr["SysNumber"]));
			}
			text += DataService.MappingUdf(dr);
			return text + " }";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}
}
