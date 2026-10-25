using System;
using System.Data;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.PackingManagement;

public interface ICreateTypeCase
{
	DataTable GetTypeCase();

	string DelTC(string TypeCode);

	HttpResult SaveTCDataSet(DataSet ds, string action);

	DataTable TypeCaseGetDataDoc(Enums.NavigateType type, string TypeCode);

	DataTable Search(string TypeCode, string TypeName, DateTime CreateDate);
}
