using System.Data;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Mrps;

public interface IJetstarPreOrder
{
	DataTable Search(string type, string searchDate);

	DataTable Run(string type);

	HttpResult SaveData(DataTable dt, Enums.TransactionType tranType);

	DataSet GetOpenPoByMrpCode(int mrpCode);

	DataTable GetBp();
}
