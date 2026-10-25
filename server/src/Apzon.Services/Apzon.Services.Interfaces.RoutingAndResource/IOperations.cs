using System.Data;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.RoutingAndResource;

public interface IOperations
{
	DataTable SearchData(DataTable dt);

	DataTable GetDataLine(string opeCode);

	DataTable OperationsGetDataDoc(string OpeCode, Enums.NavigateType type);

	HttpResult SaveData(DataTable dt);

	HttpResult DeleteData(DataTable dt);
}
