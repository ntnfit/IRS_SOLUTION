using System.Data;
using Apzon.Entities.Models.MobileApp;

namespace Apzon.Services.Interfaces.Sales;

public interface IDocumentDraft
{
	DataTable GetTableDefault();

	DataTable SearchDoc(DataTable dt);

	ResponseResult DMSSearchDoc(DataTable dt);

	DataTable SearchLine(DataTable dt);

	DataTable SearchListDocEntry(DataTable dt);

	DataTable GetDraftByEntry(int docEntry);

	string CheckIsIns(int docEntry);

	string SaveLogForm(DataTable dt);

	DataTable GetLogForm(string userName);

	DataTable GetApprovalInfo(int docEntry);
}
