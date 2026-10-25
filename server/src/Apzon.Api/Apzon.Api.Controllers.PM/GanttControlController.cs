using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.PM;

public class GanttControlController : BaseApiController
{
	[HttpGet]
	public DataTable LoadByProject(int projectAbsEntry)
	{
		return UnitOfWork.GanttControl.LoadByProject(projectAbsEntry);
	}

	[HttpPost]
	public HttpResult UpdateTask([FromBody] DataTable dt)
	{
		return UnitOfWork.GanttControl.UpdateTask(dt);
	}
}
