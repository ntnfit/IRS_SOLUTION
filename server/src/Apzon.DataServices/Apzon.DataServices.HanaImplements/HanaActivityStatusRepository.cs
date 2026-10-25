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

public class HanaActivityStatusRepository : ActivityStatusRepository
{
	public HanaActivityStatusRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	public override HttpResult Save(DataTable dtSave, string sessionId)
	{
		try
		{
			new HttpResult();
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			DataTable dataTable = ExecuteDataTable("select \"statusID\" ID from OCLA;", CommandType.Text);
			if (dataTable.IsNotNull())
			{
				List<DataRow> list2 = (from t in dataTable.AsEnumerable()
					where !dtSave.AsEnumerable().Any((DataRow x) => Function.ParseInt(x["statusID"]) == Function.ParseInt(t["ID"]))
					select t).ToList();
				if (list2.Any())
				{
					foreach (DataRow item in list2)
					{
						list.Add(new BodyRequestObject
						{
							ObjectType = ApzObjectTypes.oActivityStatus,
							OutObjectKey = true,
							Action = Enums.HttpAction.Delete,
							Key = Function.ParseInt(item["ID"])
						});
					}
				}
			}
			if (dtSave.IsNotNull())
			{
				int i;
				for (i = 0; i < dtSave.Rows.Count; i++)
				{
					if (dataTable.IsNotNull() && dataTable.AsEnumerable().Any((DataRow t) => Function.ParseInt(t["ID"]) == Function.ParseInt(dtSave.Rows[i]["statusID"])))
					{
						list.Add(new BodyRequestObject
						{
							ObjectType = ApzObjectTypes.oActivityStatus,
							OutObjectKey = true,
							Action = Enums.HttpAction.Update,
							BodyObject = Mapping(dtSave.Rows[i]),
							Key = Function.ParseInt(dtSave.Rows[i]["statusID"])
						});
					}
					else
					{
						list.Add(new BodyRequestObject
						{
							ObjectType = ApzObjectTypes.oActivityStatus,
							OutObjectKey = true,
							Action = Enums.HttpAction.AddNew,
							BodyObject = Mapping(dtSave.Rows[i])
						});
					}
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

	private string Mapping(DataRow rowMapping)
	{
		return string.Concat(string.Concat(string.Concat("{" + Function.GetJsonString("StatusName", Function.ToString(rowMapping["name"])), Function.GetJsonString("StatusDescription", Function.ToString(rowMapping["descriptio"]))), DataService.MappingUdf(rowMapping)), "}");
	}
}
