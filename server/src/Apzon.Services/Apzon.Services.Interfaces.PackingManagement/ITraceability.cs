using System;
using System.Data;
using Apzon.Commons.Enums;

namespace Apzon.Services.Interfaces.PackingManagement;

public interface ITraceability
{
	DataTable GetCase();

	DataTable GetCaseTracibility();

	DataTable GetUser();

	DataTable GetItemByLineNumber(int LineNumber);

	string Update(DataSet ds);

	DataTable GetLastLineNum();

	DataTable CaseGetDataDoc(string CaseCode, Enums.NavigateType type);

	DataTable Search(string CaseCode, string TypeCode, DateTime PostingDate);
}
