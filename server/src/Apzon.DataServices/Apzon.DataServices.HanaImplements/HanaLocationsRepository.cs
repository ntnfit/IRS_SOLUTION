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

public class HanaLocationsRepository : LocationsRepository
{
	public HanaLocationsRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	public override HttpResult Save(DataTable dtSource, string sessionId)
	{
		try
		{
			DataTable dtCurrentSource = ExecuteDataTable("Select \"Code\" from OLCT;", CommandType.Text);
			if (!dtSource.IsNotNull() && !dtCurrentSource.IsNotNull())
			{
				return new HttpResult(Enums.MessageCode.SaveError, "Please enter data first");
			}
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			if (dtCurrentSource.IsNotNull())
			{
				List<int> list2 = (from t in dtCurrentSource.AsEnumerable()
					where !dtSource.IsNotNull() || dtSource.AsEnumerable().All((DataRow x) => !Function.ToString(x["Code"]).Equals(Function.ToString(t["Code"])))
					select Function.ParseInt(t["Code"])).ToList();
				for (int num = 0; num < list2.Count; num++)
				{
					list.Add(new BodyRequestObject
					{
						ObjectType = ApzObjectTypes.oWarehouseLocations,
						OutObjectKey = true,
						Key = list2[num],
						Action = Enums.HttpAction.Delete
					});
				}
				if (dtSource.IsNotNull())
				{
					List<DataRow> list3 = (from t in dtSource.AsEnumerable()
						where dtCurrentSource.AsEnumerable().Any((DataRow x) => Function.ToString(x["Code"]).Equals(Function.ToString(t["Code"])))
						select t).ToList();
					for (int num2 = 0; num2 < list3.Count; num2++)
					{
						list.Add(new BodyRequestObject
						{
							ObjectType = ApzObjectTypes.oWarehouseLocations,
							OutObjectKey = true,
							Key = Function.ParseInt(list3[num2]["Code"]),
							Action = Enums.HttpAction.Update,
							BodyObject = Mapping(list3[num2])
						});
					}
				}
			}
			if (dtSource.IsNotNull())
			{
				List<DataRow> list4 = (from t in dtSource.AsEnumerable()
					where !dtCurrentSource.IsNotNull() || dtCurrentSource.AsEnumerable().All((DataRow x) => !Function.ToString(x["Code"]).Equals(Function.ToString(t["Code"])))
					select t).ToList();
				for (int num3 = 0; num3 < list4.Count; num3++)
				{
					list.Add(new BodyRequestObject
					{
						ObjectType = ApzObjectTypes.oWarehouseLocations,
						OutObjectKey = true,
						Action = Enums.HttpAction.AddNew,
						BodyObject = Mapping(list4[num3])
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

	private string Mapping(DataRow rowData)
	{
		string text = "{";
		if (Function.ParseInt(rowData["Code"]) > 0)
		{
			text += Function.GetJsonString("Code", Function.ParseInt(rowData["Code"]));
		}
		text += Function.GetJsonString("Name", Function.ToString(rowData["Location"]));
		return text + "}";
	}
}
