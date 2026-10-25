using System.Data;
using System.Data.SqlClient;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Services.Interfaces.Commons;

namespace Apzon.Services.Repository.Commons;

internal class ApprovalDecisionRepository : BaseService<DataTable>, IApprovalDecision
{
	public ApprovalDecisionRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetDataByWddCode(int wddCode)
	{
		return ExecuteDataTable("SELECT * FROM OWDD WHERE WddCode=@WddCode", CommandType.Text, new SqlParameter("@WddCode", wddCode));
	}

	public DataTable GetSourceMailContent(int wddCode)
	{
		DataTable dataByWddCode = GetDataByWddCode(wddCode);
		if (dataByWddCode.IsNotNull())
		{
			int num = Function.ParseInt(dataByWddCode.Rows[0]["WtmCode"]);
			int num2 = Function.ParseInt(dataByWddCode.Rows[0]["DraftEntry"]);
			string value = Function.ToString(dataByWddCode.Rows[0]["ObjType"]);
			string value2 = Function.ToString(dataByWddCode.Rows[0]["Remarks"]);
			int num3 = Function.ParseInt(dataByWddCode.Rows[0]["OwnerID"]);
			new DataTable();
			string query = "apz_approval_emailcontent";
			return ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@WddCode", wddCode), new SqlParameter("@WtmCode", num), new SqlParameter("@DraftKey", num2), new SqlParameter("@ObjType", value), new SqlParameter("@Remarks", value2), new SqlParameter("@ReqId", num3), new SqlParameter("@ReceiptId", 1));
		}
		return new DataTable();
	}
}
