using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaBusinessPartnerCatalogNumbersRepository : BusinessPartnerCatalogNumbersRepository
{
	public HanaBusinessPartnerCatalogNumbersRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, unitOfWork, dataService)
	{
	}

	public override HttpResult Save(DataTable model, ApzObjectTypes objtype, string sessionId)
	{
		try
		{
			DataTable dtExists = GetExisted(model, objtype);
			if (dtExists == null)
			{
				return new HttpResult(Enums.MessageCode.Error, "Can not get data to save");
			}
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			int i;
			for (i = 0; i < model.Rows.Count; i++)
			{
				if (!string.IsNullOrEmpty(Function.ToString(model.Rows[i]["ItemCode"])) || !string.IsNullOrEmpty(Function.ToString(model.Rows[i]["CardCode"])))
				{
					Enums.HttpAction httpAction = ((!dtExists.AsEnumerable().Any((DataRow t) => Function.ToString(t["ItemCode"]).Equals(Function.ToString(model.Rows[i]["ItemCode"])) && Function.ToString(t["CardCode"]).Equals(Function.ToString(model.Rows[i]["CardCode"])) && Function.ToString(t["Substitute"]).Equals(Function.ToString(model.Rows[i]["Substitute"])))) ? Enums.HttpAction.AddNew : Enums.HttpAction.Update);
					BodyRequestObject bodyRequestObject = new BodyRequestObject();
					bodyRequestObject.Action = httpAction;
					bodyRequestObject.ObjectType = ApzObjectTypes.oAlternateCatNum;
					if (httpAction == Enums.HttpAction.Update)
					{
						bodyRequestObject.Key = new
						{
							ItemCode = Function.ToString(model.Rows[i]["ItemCode"]),
							CardCode = Function.ToString(model.Rows[i]["CardCode"]),
							Substitute = Function.ToString(model.Rows[i]["Substitute"])
						};
					}
					bodyRequestObject.BodyObject = Mapping(model.Rows[i], (httpAction == Enums.HttpAction.AddNew) ? EnumCommandType.Add : EnumCommandType.Update);
					list.Add(bodyRequestObject);
				}
			}
			int i2;
			for (i2 = 0; i2 < dtExists.Rows.Count; i2++)
			{
				if (!model.AsEnumerable().Any((DataRow t) => Function.ToString(t["ItemCode"]).Equals(Function.ToString(dtExists.Rows[i2]["ItemCode"])) && Function.ToString(t["CardCode"]).Equals(Function.ToString(dtExists.Rows[i2]["CardCode"])) && Function.ToString(t["Substitute"]).Equals(Function.ToString(dtExists.Rows[i2]["Substitute"]))))
				{
					BodyRequestObject bodyRequestObject2 = new BodyRequestObject();
					bodyRequestObject2.Action = Enums.HttpAction.Delete;
					bodyRequestObject2.ObjectType = ApzObjectTypes.oAlternateCatNum;
					bodyRequestObject2.Key = new
					{
						ItemCode = Function.ToString(dtExists.Rows[i2]["ItemCode"]),
						CardCode = Function.ToString(dtExists.Rows[i2]["CardCode"]),
						Substitute = Function.ToString(dtExists.Rows[i2]["Substitute"])
					};
					list.Add(bodyRequestObject2);
				}
			}
			HttpResult httpResult = DataService.ProcessMutiTransaction(sessionId, list);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				return new HttpResult(Enums.MessageCode.Error, httpResult.Message);
			}
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override string Mapping(DataRow dataRow, EnumCommandType commandType)
	{
		string text = "{\r\n";
		if (commandType != EnumCommandType.Remove)
		{
			text += Function.GetJsonString("ItemCode", Function.ToString(dataRow["ItemCode"]));
			text += Function.GetJsonString("CardCode", Function.ToString(dataRow["CardCode"]));
			text += Function.GetJsonString("Substitute", Function.ToString(dataRow["Substitute"]));
			if (!string.IsNullOrEmpty(Function.ToString(dataRow["ShowSCN"])) || commandType == EnumCommandType.Update)
			{
				text += Function.GetJsonString("DisplayBPCatalogNumber", (Function.ToString(dataRow["ShowSCN"]) == "N") ? "tNO" : "tYES");
			}
		}
		return text + "}";
	}

	protected override string Mapping(DataRow[] dt)
	{
		string text = "[\r\n";
		foreach (DataRow dataRow in dt)
		{
			if (!string.IsNullOrEmpty(Function.ToString(dataRow["ItemCode"])) || !string.IsNullOrEmpty(Function.ToString(dataRow["CardCode"])))
			{
				text += "{";
				text += Function.GetJsonString("ItemCode", Function.ToString(dataRow["ItemCode"]));
				text += Function.GetJsonString("CardCode", Function.ToString(dataRow["CardCode"]));
				text += Function.GetJsonString("Substitute", Function.ToString(dataRow["Substitute"]));
				if (!string.IsNullOrEmpty(Function.ToString(dataRow["ShowSCN"])))
				{
					text += Function.GetJsonString("DisplayBPCatalogNumber", (Function.ToString(dataRow["ShowSCN"]) == "N") ? "tNO" : "tYES");
				}
				text += "},";
			}
		}
		return text + "]";
	}

	public override string Remove(string code, string subtitle, string item, ApzObjectTypes objtype, string sessionId)
	{
		try
		{
			DataTable dataByCode = GetDataByCode(code, subtitle, item, objtype);
			if (dataByCode.IsNotNull())
			{
				List<BodyRequestObject> list = new List<BodyRequestObject>();
				for (int i = 0; i < dataByCode.Rows.Count; i++)
				{
					list.Add(new BodyRequestObject
					{
						Action = Enums.HttpAction.Delete,
						ObjectType = ApzObjectTypes.oAlternateCatNum,
						Key = new
						{
							ItemCode = Function.ToString(dataByCode.Rows[i]["ItemCode"]),
							CardCode = Function.ToString(dataByCode.Rows[i]["CardCode"]),
							Substitute = Function.ToString(dataByCode.Rows[i]["Substitute"])
						}
					});
				}
				HttpResult httpResult = DataService.ProcessMutiTransaction(sessionId, list);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					return httpResult.Message;
				}
				return string.Empty;
			}
			return "BP Catalog Number does not exist";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return ex.Message;
		}
	}
}
