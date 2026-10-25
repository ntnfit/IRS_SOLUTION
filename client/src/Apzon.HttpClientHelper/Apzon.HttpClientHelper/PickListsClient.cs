using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Inventory;

namespace Apzon.HttpClientHelper;

internal class PickListsClient : IPickListsClient
{
	private const string FormController = "PickLists";

	public async Task<HttpResult> Save(PickListsImpl doc)
	{
		try
		{
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "PickLists");
			return await helper.PostAsync("Save", doc);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
