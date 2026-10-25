using System;
using System.Data;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Administrator.Business_Partners;

namespace Apzon.Services.Repository.Administrator.Business_Partner;

public class BusinessPartnerPropertiesRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), IBusinessPartnerProperties
{
	private string TempTableTab = "#dtOCQG";

	public DataTable LoadDoc()
	{
		try
		{
			string query = "SELECT * FROM OCQG ORDER BY \"GroupCode\";";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public HttpResult Save(DataTable dtDoc)
	{
		return Process(ApzObjectTypes.oBusinessPartnerProperties, -1, Enums.TransactionType.A, dtDoc);
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable document, ref string objId, ref string objNum)
	{
		objId = "";
		objNum = "";
		try
		{
			CreateTempTable(document, TempTableTab);
			BulkCopy(document, TempTableTab);
			object obj = ExecuteScalar("DELETE dbo.OCQG\r\n                                     INSERT INTO dbo.OCQG\r\n                                     SELECT * FROM #dtOCQG", CommandType.Text);
			objId = Function.ToString(obj);
			objNum = objId;
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
