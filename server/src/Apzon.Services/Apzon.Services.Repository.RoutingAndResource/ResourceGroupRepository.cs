using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Interfaces.RoutingAndResource;

namespace Apzon.Services.Repository.RoutingAndResource;

public class ResourceGroupRepository : BaseService<DataTable>, IResourceGroup
{
	public ResourceGroupRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable SearchData(string resGroupName)
	{
		try
		{
			return ExecuteDataTable("APZ_ResourceGroup_SearchData", CommandType.StoredProcedure, new SqlParameter("@resGroupName", resGroupName));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return new DataTable();
	}
}
