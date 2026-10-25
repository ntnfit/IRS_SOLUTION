using System.Data;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public class InternalReconciliationsClient : IInternalReconciliationsClient
{
	private const string ControllerName = "InternalReconciliations";

	public DataSet GetStructure()
	{
		using HttpClientHelper<DataSet> httpClientHelper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "InternalReconciliations");
		return httpClientHelper.Get("GetStructure");
	}

	public HttpResult Add(InternalReconciliationImpl doc)
	{
		using HttpClientHelper<HttpResult> httpClientHelper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "InternalReconciliations");
		return httpClientHelper.Post("Add", doc);
	}
}
