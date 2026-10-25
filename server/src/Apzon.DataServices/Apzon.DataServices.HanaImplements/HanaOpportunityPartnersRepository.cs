using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaOpportunityPartnersRepository : OpportunityPartnersRepository
{
	private readonly IUnitOfWork _unitOfWork;

	private string _serviceName = "PartnersSetupsService";

	private string _keyFieldName = "PrtId";

	private ApzObjectTypes _objectType = ApzObjectTypes.oOpportunityPartners;

	public HanaOpportunityPartnersRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, unitOfWork, dataService)
	{
		_unitOfWork = unitOfWork;
	}

	public override HttpResult Save(DataTable dt, string sessionId)
	{
		try
		{
			HttpResult httpResult = _unitOfWork.Master.SaveService(dt, sessionId, _objectType, _serviceName, _keyFieldName, DicFieldTags, deleteOldRows: true);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				return httpResult;
			}
			using (Transaction transaction = new Transaction())
			{
				DataRow[] source = dt.Select("PrtId <> 0");
				DataTable dataTable = new DataTable();
				if (source.Any())
				{
					dataTable = source.CopyToDataTable();
				}
				if (dataTable.IsNotNull())
				{
					List<SqlParameter> list = new List<SqlParameter>();
					string text = "declare checkBp int;\r\n\t\t\t\t\t\t\tdeclare checkRl int;";
					for (int i = 0; i < dataTable.Rows.Count; i++)
					{
						string text2 = Function.ToString(dataTable.Rows[i]["RelatCard"]);
						int num = Function.ParseInt(dataTable.Rows[i]["DefaultORL"]);
						int num2 = Function.ParseInt(dataTable.Rows[i]["PrtId"]);
						text += $"\r\n\t\t\t\t\t\t\tselect IFNULL((SELECT 1 FROM ocrd b WHERE b.\"CardCode\" = '{text2}'),0) into checkBp from DUMMY;\r\n\t\t\t\t\t\t\tselect IFNULL((SELECT 1 FROM oorl b WHERE b.\"OrlCode\" = {num}),0) into checkRl from DUMMY;\r\n\t\t\t\t\t\t\tIF(checkBp = 0 AND ltrim(rtrim(IFNULL('{text2}', ''))) <> '') THEN SELECT '1 - Related BP Code ''{text2}'' does not exist' FROM DUMMY;\r\n\t\t\t\t\t\t\tRETURN;\r\n\t\t\t\t\t\t\tEND IF;\r\n\t\t\t\t\t\t\tIF(checkRl = 0  AND IFNULL({num}, 0) <> 0 AND IFNULL({num}, 0) <> -1) THEN SELECT '1 - Default Relationship Code ''{num}'' does not exist' FROM DUMMY;\r\n\t\t\t\t\t\t\tRETURN;\r\n\t\t\t\t\t\t\tEND IF;\r\n\t\t\t\t\t\t\tDELETE FROM OPRT WHERE \"PrtId\" = {num2};\r\n\t\t\t\t\t\t\tINSERT INTO OPRT(\"PrtId\", \"Name\", \"RelatnType\", \"RelatCard\", \"Memo\", \"DataSource\", \"UserSign\", \"DefaultORL\") VALUES(@PrtId{i}, @Name{i}, @RelatnType{i}, @RelatCard{i}, @Memo{i}, @DataSource{i}, @UserSign{i}, @DefaultORL{i});\r\n\t\t\t\t\t\t\t";
						list.AddRange(new SqlParameter[8]
						{
							new SqlParameter("@PrtId" + i, Function.ParseInt(dataTable.Rows[i]["PrtId"])),
							new SqlParameter("@Name" + i, Function.ToString(dataTable.Rows[i]["Name"])),
							new SqlParameter("@RelatnType" + i, Function.ToString(dataTable.Rows[i]["RelatnType"])),
							new SqlParameter("@RelatCard" + i, Function.ToString(dataTable.Rows[i]["RelatCard"])),
							new SqlParameter("@Memo" + i, Function.ToString(dataTable.Rows[i]["Memo"])),
							new SqlParameter("@DataSource" + i, Function.ToString(dataTable.Rows[i]["DataSource"])),
							new SqlParameter("@UserSign" + i, Function.ParseShort(dataTable.Rows[i]["UserSign"])),
							new SqlParameter("@DefaultORL" + i, Function.ParseInt(dataTable.Rows[i]["DefaultORL"]))
						});
					}
					if (text.EndsWith(","))
					{
						text = text.Remove(text.Length - 1);
					}
					DataTable dataTable2 = ExecuteDataTable(text, CommandType.Text, list.ToArray());
					if (dataTable2 != null && dataTable2.Rows.Count > 0)
					{
						transaction.Rollback();
						return new HttpResult(Enums.MessageCode.Error, Function.ToString(dataTable2.Rows[0][0]));
					}
					transaction.Commit();
					return new HttpResult(Enums.MessageCode.Success);
				}
			}
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
