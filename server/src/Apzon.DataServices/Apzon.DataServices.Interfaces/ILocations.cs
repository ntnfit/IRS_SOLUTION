using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.DataServices.Interfaces;

public interface ILocations
{
	HttpResult Save(DataTable dtSave, string sessionId);
}
