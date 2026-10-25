using System;
using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IChannelClient
{
	Task<DataTable> GetDataDoc(string channelCode, string channelName, string status);

	Task<DataTable> GetDataLine(string channelCode);

	DataTable GetRecord(string position, string channelCode);

	Task<HttpResult> SaveData(Tuple<DataSet, string> tupData);

	Task<HttpResult> Remove(Tuple<string> tupData);
}
