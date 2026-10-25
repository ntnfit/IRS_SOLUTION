using System.Data;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.HumanResources.TimeSheeting;

public interface IAboutRangeTime
{
	DataTable GetData();

	HttpResult Process(ApzObjectTypes objtype, int bplId, Enums.TransactionType transtype, DataTable document, string objId = "", string objCode = "", bool lockNumbering = true);
}
