using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.DMS;

public interface IDMSReport
{
	HttpResult SavesCoverage();

	DataTable GetMarkers();
}
