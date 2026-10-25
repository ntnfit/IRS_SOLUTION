using System;
using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public class ChannelClient : IChannelClient
{
	private const string _controller = "Channel";

	public async Task<DataTable> GetDataDoc(string channelCode, string channelName, string status)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Channel");
		return await helper.GetAsync("GetDataDoc", new { channelCode, channelName, status });
	}

	public async Task<DataTable> GetDataLine(string channelCode)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Channel");
		return await helper.GetAsync("GetDataLine", new { channelCode });
	}

	public DataTable GetRecord(string position, string channelCode)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Channel");
		return httpClientHelper.Get("GetRecord", new { position, channelCode });
	}

	public async Task<HttpResult> SaveData(Tuple<DataSet, string> tupData)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "Channel");
		return await helper.PostAsync("SaveData", tupData);
	}

	public async Task<HttpResult> Remove(Tuple<string> tupData)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "Channel");
		return await helper.PostAsync("Remove", tupData);
	}
}
