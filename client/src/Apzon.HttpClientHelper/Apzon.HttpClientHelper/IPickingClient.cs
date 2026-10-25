using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IPickingClient
{
	Task<DataSet> LoadSalesOrder(DataTable dtSearch);

	Task<DataSet> LoadPickingList(DataTable dtSearch);

	Task<DataSet> LoadPicked(DataTable dtSearch);

	Task<DataSet> LoadPickedLine(int docentry);

	Task<DataSet> LoadBatchBarcode(string barcode, string itemcode, string whs, int docNo, int docRow, int line);

	Task<DataSet> LoadBatchLine(int docEntry, int lineNum);

	Task<DataTable> GetDoc(DataTable dt);

	Task<DataTable> GetLines(int docentry);

	Task<int> GetNextId();

	Task<HttpResult> Close(int docentry);

	Task<HttpResult> Cancel(int docentry);

	Task<HttpResult> Save(DataSet ds);

	Task<DataTable> GetBatch(int absEntry, int pickentry);

	Task<DataTable> GetConfirmed(int absEntry, int pickentry);

	Task<DataTable> AutoSelectBatch(string itemcode, string whsCode, double quantity, int linenum, int docentry, int orderNum, int orderLine);

	Task<DataSet> CreateNewInstance();

	Task<HttpResult> SaveList(List<DataSet> lst);

	Task<DataSet> GetListBatchSerial(string itemCode, string whsCode, int pickentry, int docEntry, int orderNum, int orderLine);
}
