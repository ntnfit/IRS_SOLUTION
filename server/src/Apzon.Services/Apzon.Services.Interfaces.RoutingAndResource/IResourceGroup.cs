using System.Data;

namespace Apzon.Services.Interfaces.RoutingAndResource;

public interface IResourceGroup
{
	DataTable SearchData(string resGroupName);
}
