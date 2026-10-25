using System;
using System.Data;

namespace Apzon.Services.Interfaces.Production;

public interface IProductionForBatch
{
	DataSet Search(DataTable dt);

	DataSet SearchPlanning(DataTable dt);

	DataSet SearchProductionOrder(DataTable dt);

	DataSet GetProductionRequest(int id);

	DataSet GetPlanning(string batch, int docEntry);

	int GetPage(int current, int type, string username, int branch);

	DataSet GetByStatus(string status, DateTime? fromDate, DateTime? toDate, string username, string branch);

	DataSet GetBatchSelected(int docEntry);

	DataSet GetOperation(int docEntry, int fatherId);

	string UpdateDelRequestBatch(DataTable dtBatch);

	DataTable LoadBatchStatusQC();

	DataTable LoadWhs(string userName, int branch);
}
