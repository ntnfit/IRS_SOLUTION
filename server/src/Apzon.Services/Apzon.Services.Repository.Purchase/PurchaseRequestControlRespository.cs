using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.UserFields;
using Apzon.Services.Interfaces.Purchase;
using SAPbobsCOM;

namespace Apzon.Services.Repository.Purchase;

public class PurchaseRequestControlRespository : BaseService<DataTable>, IPurchaseRequestControl
{
	public PurchaseRequestControlRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public List<UserFieldsImpl> GetListField(string tableName)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("APZ_Fields_GetListField", CommandType.StoredProcedure, new SqlParameter("@TableId", tableName));
			List<UserFieldsImpl> list = new List<UserFieldsImpl>();
			if (dataTable.IsNotNull())
			{
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					DataRow dataRow = dataTable.Rows[i];
					UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
					userFieldsImpl.Name = Function.ToString(dataRow["ColumnName"]);
					userFieldsImpl.Description = Function.ToString(dataRow["ColumnName"]);
					switch (Function.ToString(dataRow["DataType"]))
					{
					case "int":
					case "smallint":
					case "char":
					case "nvarchar":
						userFieldsImpl.Type = BoFieldTypes.db_Alpha;
						break;
					case "ntext":
						userFieldsImpl.Type = BoFieldTypes.db_Memo;
						break;
					case "numeric":
						userFieldsImpl.Type = BoFieldTypes.db_Numeric;
						break;
					case "datetime":
						userFieldsImpl.Type = BoFieldTypes.db_Date;
						break;
					case "float":
						userFieldsImpl.Type = BoFieldTypes.db_Float;
						break;
					}
					list.Add(userFieldsImpl);
				}
				return list;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new List<UserFieldsImpl>();
	}
}
