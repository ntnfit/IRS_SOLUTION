using System.Data;

namespace Apzon.DataServices.Interfaces;

public interface IFreightSetup
{
	string Save(DataTable doc, string sessionId);
}
