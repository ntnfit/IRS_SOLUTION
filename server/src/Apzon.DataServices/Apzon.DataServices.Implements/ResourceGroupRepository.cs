using System;
using System.Data;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.DataServices.Interfaces;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class ResourceGroupRepository : Repository<DataTable>, IResourceGroup
{
	public ResourceGroupRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	public string Save(DataTable dt, string sessionId)
	{
		try
		{
			string objType = "oResourceGroup";
			DataService.GetBusinessObjectTemplate(sessionId, objType);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
		return "";
	}
}
