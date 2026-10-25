using System;
using System.Data;
using System.Data.SqlClient;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.PM;

namespace Apzon.Services.Repository.PM;

internal class CommentsRepository : BaseService<DataTable>, IComments
{
	protected const string Temp = "#APZ_COMMENTS";

	public CommentsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public HttpResult AddNew(DataTable dt, string user)
	{
		dt.Rows[0]["CREATED_BY"] = user;
		return Process(ApzObjectTypes.oObjectComments, -1, Enums.TransactionType.A, dt);
	}

	public DataTable GetByObject(string objectType, string objectCode, string objectDetail)
	{
		return ExecuteDataTable("SELECT * FROM APZ_COMMENTS WHERE OBJTYPE=@ObjType AND OBJECT_CODE=@ObjectCode AND DETAIL_CODE=@ObjectDetail;", CommandType.Text, new SqlParameter("@ObjType", objectType), new SqlParameter("@ObjectCode", objectCode ?? ""), new SqlParameter("@ObjectDetail", objectDetail ?? ""));
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable document, ref string objId, ref string objNum)
	{
		try
		{
			string value = Function.ToString(document.Rows[0]["COMMENTS"]);
			string value2 = Function.ToString(document.Rows[0]["CREATED_BY"]);
			string text = Guid.NewGuid().ToString();
			ExecuteNonQuery("insert into APZ_COMMENTS(ID,OBJTYPE,OBJECT_CODE,DETAIL_CODE,CREATE_DATE,CREATE_TIME,CREATED_BY,CREATOR_NAME,COMMENTS)\r\n\t                                SELECT @Id,@ObjType,@ObjectCode,@ObjectDetail,@CreateDate,@CreateTime,@UserCode,T0.U_NAME,@Comments FROM OUSR T0 WHERE USER_CODE=@UserCode; ", CommandType.Text, new SqlParameter("@UserCode", value2), new SqlParameter("@Comments", value), new SqlParameter("@Id", text), new SqlParameter("@ObjType", Function.ToString(document.Rows[0]["OBJTYPE"])), new SqlParameter("@ObjectCode", Function.ToString(document.Rows[0]["OBJECT_CODE"])), new SqlParameter("@ObjectDetail", Function.ToString(document.Rows[0]["DETAIL_CODE"])), new SqlParameter("@CreateDate", DateTime.Today.ToString("yyyy-MM-dd")), new SqlParameter("@CreateTime", DateTime.Now));
			objId = (objNum = text);
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new HttpResult
			{
				Message = ex.Message,
				MessageCode = Enums.MessageCode.Error
			};
		}
	}
}
