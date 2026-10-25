using Apzon.Commons;

namespace Apzon.HttpClientHelper;

public static class HttpClientHelperFactory
{
	public static IHttpClient GetHttpClient()
	{
		return new HttpClientRepository();
	}

	public static IHttpClient GetHttpClient(ApzObjectTypes type)
	{
		return new HttpClientRepository(type);
	}
}
