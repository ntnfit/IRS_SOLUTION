using System.Data;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.PackingManagement;

public interface IPackingOrder
{
	new DataTable GetType();

	DataTable GetPOCode();

	DataTable GetWhs();

	DataTable POGetDataDoc(int POCode, Enums.NavigateType type);

	HttpResult SaveDocDataSet(DataSet ds, string action);

	string DelPO(int POCode);
}
