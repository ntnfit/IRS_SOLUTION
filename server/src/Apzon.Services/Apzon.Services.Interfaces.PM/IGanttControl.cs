using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.PM;

public interface IGanttControl
{
	DataTable LoadByProject(int projectAbsEntry);

	HttpResult UpdateTask(DataTable dt);
}
