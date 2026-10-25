using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.DMS;

public interface IChannel
{
	DataTable GetDataDoc(string channelCode, string channelName, string status);

	DataTable GetDataLine(string channelCode);

	DataTable GetRecord(string position, string channelCode);

	HttpResult SaveData(DataSet ds, string type);

	HttpResult Remove(string channelCode);
}
