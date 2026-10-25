using System;
using System.Data;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Administrator.Setup;

namespace Apzon.Services.Repository.Administrator.Setup;

public class ItemPropertiesRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), IItemProperties, IService<DataTable>
{
	private string TempTableTab = "#dtOITG";

	public string Insert(DataTable entity)
	{
		throw new NotImplementedException();
	}

	public string Update(DataTable entity)
	{
		throw new NotImplementedException();
	}

	public string Remove(DataTable entity)
	{
		throw new NotImplementedException();
	}

	public DataTable GetById(int id)
	{
		throw new NotImplementedException();
	}

	public DataTable LoadDocItemProperties()
	{
		try
		{
			string query = LoadDocItemPropertiesQuery();
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text);
			if (dataTable.IsNotNull())
			{
				return dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	protected virtual string LoadDocItemPropertiesQuery()
	{
		return "SELECT \r\n\t                                        ISNULL(ItmsTypCod,0) AS ItmsTypCod,\r\n\t                                        ISNULL(ItmsGrpNam,'') AS ItmsGrpNam,\r\n\t                                        ISNULL(UserSign,1) AS UserSign\t\r\n\t                                 FROM dbo.oitg ";
	}

	public HttpResult SaveItemProperties(DataTable dtDoc)
	{
		return Process(ApzObjectTypes.oItemProperties, -1, Enums.TransactionType.U, dtDoc);
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable document, ref string objId, ref string objNum)
	{
		objId = "";
		objNum = "";
		try
		{
			CreateTempTable(document, TempTableTab);
			BulkCopy(document, TempTableTab);
			int num = ExecuteNonQuery("APZ_Process_ItemProperties", CommandType.StoredProcedure);
			objId = Function.ToString(num);
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
