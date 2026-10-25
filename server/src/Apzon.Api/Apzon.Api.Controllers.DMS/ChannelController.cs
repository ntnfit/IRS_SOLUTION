using System;
using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.DMS;

public class ChannelController : BaseApiController
{
	[HttpGet]
	public DataTable GetDataDoc(string channelCode, string channelName, string status)
	{
		return UnitOfWork.Channel.GetDataDoc(channelCode, channelName, status);
	}

	[HttpGet]
	public DataTable GetDataLine(string channelCode)
	{
		return UnitOfWork.Channel.GetDataLine(channelCode);
	}

	[HttpGet]
	public DataTable GetRecord(string position, string channelCode)
	{
		return UnitOfWork.Channel.GetRecord(position, channelCode);
	}

	[HttpPost]
	public HttpResult SaveData(Tuple<DataSet, string> tupData)
	{
		return UnitOfWork.Channel.SaveData(tupData.Item1, tupData.Item2);
	}

	[HttpPost]
	public HttpResult Remove(Tuple<string> tupData)
	{
		return UnitOfWork.Channel.Remove(tupData.Item1);
	}
}
