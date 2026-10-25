using System;
using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;

namespace Apzon.Api.Controllers.Commons;

public class DiagramController : BaseApiController
{
	[HttpGet]
	public DataTable GetDiagram(ApzObjectTypes type, string objId)
	{
		try
		{
			DataTable dt = new DataTable();
			UnitOfWork.Diagram.GetDiagram(type, objId, ref dt);
			return dt;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataTable();
		}
	}
}
