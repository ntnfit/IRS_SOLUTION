using System.Data;

namespace Apzon.Services.Interfaces.DMS;

public interface IOutlet
{
	DataTable GetList(string userCode);
}
