using System.Data;
using Apzon.Commons;

namespace Apzon.HttpClientHelper;

public class BinLocMasterDataClient : IBinLocMasterDataClient
{
	public const string Controller = "BinLocMaster";

	public DataTable GetDataBinLocation(int absEntry)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BinLocMaster");
		return httpClientHelper.Get("GetDataBinLocation", new { absEntry });
	}

	public DataTable GetDataBinLocation(string binCode, string whsCode)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BinLocMaster");
		return httpClientHelper.Get("GetDataBinLocation", new { binCode, whsCode });
	}

	public DataTable GetBatchBinAllowcateLineByDocEntry(string docEntry, ApzObjectTypes objectType)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BinLocMaster");
		return httpClientHelper.Get("GetBatchBinAllowcateLineByDocEntry", new { docEntry, objectType });
	}

	public DataTable GetDataCopyBinAllowcationLine(string lstDocentry, ApzObjectTypes objectType)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BinLocMaster");
		return httpClientHelper.Get("GetDataCopyBinAllowcationLine", new { lstDocentry, objectType });
	}

	public DataTable GetAllBinCode()
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BinLocMaster");
		return httpClientHelper.Get("GetAllBinCode");
	}

	public DataTable GetBinDefault(string tranType, string whsCode, string itemCode, double qty)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BinLocMaster");
		return httpClientHelper.Get("GetBinDefault", new { tranType, whsCode, itemCode, qty });
	}

	public DataTable GetBinDefaultBatch(string tranType, string whsCode, string itemCode, int absEntry, double qty)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BinLocMaster");
		return httpClientHelper.Get("GetBinDefaultBatch", new { tranType, whsCode, itemCode, absEntry, qty });
	}
}
