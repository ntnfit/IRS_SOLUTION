using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Commons;

namespace Apzon.Services.Repository.Commons;

public class UserDefinedTextsRepository : BaseService<DataTable>, IUserDefinedTexts
{
	public UserDefinedTextsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetAll()
	{
		try
		{
			return ExecuteDataTable("SELECT * FROM OPDT;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public string Save(DataTable dataTable)
	{
		return Process(ApzObjectTypes.oUserDefinedTexts, -1, Enums.TransactionType.U, dataTable).Message;
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable document, ref string objId, ref string objNum)
	{
		if (transtype == Enums.TransactionType.D)
		{
			string query = "DELETE OPDT WHERE \"AbsEntry\"=@AbsEntry;";
			int num = ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@AbsEntry", objId));
			if (num <= 0)
			{
				return new HttpResult(Enums.MessageCode.Error, num + " row affected");
			}
			return new HttpResult();
		}
		for (int i = 0; i < document.Rows.Count; i++)
		{
			string value = Function.ToString(document.Rows[i]["TextCode"]);
			if (!string.IsNullOrEmpty(value))
			{
				int num2 = Function.ParseInt(document.Rows[i]["AbsEntry"]);
				if (num2 > 0)
				{
					string query2 = "UPDATE OPDT SET \"TextCode\"=@TextCodeValue,\"Text\"=@TextValue WHERE \"AbsEntry\"=@AbsEntry;";
					SqlParameter[] array = new SqlParameter[3];
					SqlParameter sqlParameter = new SqlParameter("@TextValue", SqlDbType.NText);
					sqlParameter.Value = Function.ToString(document.Rows[i]["Text"]);
					array[0] = sqlParameter;
					array[1] = new SqlParameter("@AbsEntry", num2);
					array[2] = new SqlParameter("@TextCodeValue", value);
					ExecuteNonQuery(query2, CommandType.Text, array);
				}
				else
				{
					SqlParameter[] array2 = new SqlParameter[3];
					SqlParameter sqlParameter2 = new SqlParameter("@TextValue", SqlDbType.NText);
					sqlParameter2.Value = Function.ToString(document.Rows[i]["Text"]);
					array2[0] = sqlParameter2;
					array2[1] = new SqlParameter("@ObjectCode", Function.ToString((int)objtype));
					array2[2] = new SqlParameter("@TextCodeValue", value);
					ExecuteNonQuery(GetSaveQuery(), CommandType.Text, array2);
				}
			}
		}
		return new HttpResult();
	}

	protected virtual string GetSaveQuery()
	{
		return "declare @max int =(select AutoKey From ONNM Where ObjectCode=@ObjectCode)\r\n                                INSERT INTO OPDT(AbsEntry,TextCode,Text) VALUES (@max,@TextCode,@Text)\r\n                                UPDATE ONNM SET AutoKey=@max+1 WHERE ObjectCode=@ObjectCode";
	}

	public string Remove(int absEntry)
	{
		return Process(ApzObjectTypes.oUserDefinedTexts, -1, Enums.TransactionType.D, new DataTable(), absEntry.ToString()).Message;
	}
}
