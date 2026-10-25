using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IGanttControlClient
{
	Task<DataTable> LoadByProject(int projectAbsEntry);

	Task<HttpResult> UpdateTask(DataTable dt);
}
