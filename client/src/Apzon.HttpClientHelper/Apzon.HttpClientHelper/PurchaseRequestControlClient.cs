using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Entities.Models.UserFields;

namespace Apzon.HttpClientHelper;

internal class PurchaseRequestControlClient : IPurchaseRequestControlClient
{
	private const string FormController = "PurchaseRequestControl";

	public async Task<List<UserFieldsImpl>> GetListField(string tblName)
	{
		try
		{
			using HttpClientHelper<UserFieldsImpl> helper = new HttpClientHelper<UserFieldsImpl>(Configuration.ApiServer, "PurchaseRequestControl");
			return await helper.GetListAsyncByPr("GetListField", new
			{
				tableName = tblName
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}
}
