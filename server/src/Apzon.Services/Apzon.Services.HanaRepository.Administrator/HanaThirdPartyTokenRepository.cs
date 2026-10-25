using System;
using System.Data;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Repository.Administrator;

namespace Apzon.Services.HanaRepository.Administrator;

public class HanaThirdPartyTokenRepository : ThirdPartyTokenRepository
{
	public HanaThirdPartyTokenRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public override HttpResult GetMaxEntry()
	{
		try
		{
			object content = ExecuteScalar("Select max(\"AbsEntry\") from APZ_OTPT", CommandType.Text);
			return new HttpResult
			{
				Content = content
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult
			{
				Content = null,
				Message = "Failed!",
				MessageCode = Enums.MessageCode.Success
			};
		}
	}
}
