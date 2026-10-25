using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.DataServices.Interfaces;

public interface IProductionOrderImportTool
{
	Task<HttpResult> SaveFG(DataTable dt, string sessionId);

	Task<string> SaveComp(DataTable dt, string sessionId);

	string SaveTransfer(DataTable dt, string sessionId);

	string SaveRC(DataTable dt, string sessionId);

	Task<string> ReportCompletion(DataTable dt, string sessionId);

	string SaveClose(DataTable dt, string sessionId);

	string SaveRCFromPC(DataTable dt, string sessionId);

	bool IsBinWarehouse(string warehouse);

	string SaveInteruptCodeRC(string intrCode, string sessionId);

	Task<string> SaveFGSerial(DataSet ds, string sessionId);

	HttpResult SaveTempFG(DataTable dt, string sessionId, string user);

	string WithdrawOfGooods(DataTable dt, string sessionId);
}
