using System;
using System.Data;

namespace Apzon.Services.Interfaces.Mrps;

public interface ISalesForeCast
{
	DataTable GetDocs();

	DataTable GetDoc(int AbsID);

	DataTable GetInfor(int AbsID);

	int GetMaxAbsID();

	DataTable GetDetail(Tuple<int, DateTime, DateTime> tup);

	DataTable Search(DataTable dt);

	DataTable GetQuickAnalysis(DataTable dt);

	DataTable GetYearForQuickAnalysis();

	string SaveDocs(DataSet ds);

	string Savedoc(DataTable dt);

	string DeleteRowInGeneral(int AbsID);

	string DeleteDocs(string ids);
}
