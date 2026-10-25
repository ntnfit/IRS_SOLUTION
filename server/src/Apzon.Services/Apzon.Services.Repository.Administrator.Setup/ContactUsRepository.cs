using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Administrator.Setup;

namespace Apzon.Services.Repository.Administrator.Setup;

public class ContactUsRepository : BaseService<ContactUsImpl>, IContactUs
{
	private const string temp = "#APZ_D_CTUS";

	public ContactUsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public HttpResult Save(ContactUsImpl entities)
	{
		try
		{
			return Process(ApzObjectTypes.oContactUs, -1, Enums.TransactionType.A, entities);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, ContactUsImpl entities, ref string objId, ref string objNum)
	{
		new HttpResult();
		if (transtype == Enums.TransactionType.A)
		{
			return SaveData(entities, transtype, ref objId, ref objNum);
		}
		return new HttpResult(Enums.MessageCode.Error, "Could not execute function");
	}

	public HttpResult SaveData(ContactUsImpl entites, Enums.TransactionType tranType, ref string objId, ref string objNum)
	{
		try
		{
			Function.ToString(tranType);
			DataTable tableStruct = GetTableStruct("APZ_D_CTUS");
			if (!tableStruct.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			_ = from DataColumn t in tableStruct.Columns
				select t.ColumnName;
			DataTable dataTable = tableStruct.Clone();
			DataRow dataRow = dataTable.Rows.Add();
			dataRow["Id"] = 0;
			dataRow["Name"] = entites.Name;
			dataRow["Address"] = entites.Address;
			dataRow["Email"] = entites.Email;
			dataRow["Phone"] = entites.Phone;
			dataRow["Remark"] = entites.Remark;
			dataRow["CreatedTime"] = DateTime.Now;
			CreateTempTable(dataTable, "#APZ_D_CTUS");
			BulkCopy(dataTable, "#APZ_D_CTUS");
			DataTable dataTable2 = ExecuteDataTable("APZ_D_ContactUs_Save", CommandType.StoredProcedure);
			if (dataTable2 != null && dataTable2.Rows.Count > 0)
			{
				if (string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[0][1])))
				{
					objId = Function.ToString(dataTable2.Rows[0][0]);
					objNum = objId;
					return new HttpResult(Enums.MessageCode.Success, "", objId);
				}
				return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable2.Rows[0][0])}-{Function.ToString(dataTable2.Rows[0][1])}");
			}
			return new HttpResult(Enums.MessageCode.Error, "Data is not valid");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
