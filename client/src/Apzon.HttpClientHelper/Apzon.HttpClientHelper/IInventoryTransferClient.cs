using System.Data;
using System.Threading.Tasks;
using Apzon.Commons;

namespace Apzon.HttpClientHelper;

public interface IInventoryTransferClient
{
	Task<DataSet> GetDataLine(int docEntry);

	Task<DataSet> GetDataCopyFrom(string lstDocEntry, ApzObjectTypes fromObj);
}
