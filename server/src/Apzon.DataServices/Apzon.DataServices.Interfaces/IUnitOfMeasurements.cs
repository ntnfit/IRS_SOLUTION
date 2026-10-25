using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.DataServices.Interfaces;

public interface IUnitOfMeasurements
{
	HttpResult SaveData(DataTable dtSave, string sessionId);
}
