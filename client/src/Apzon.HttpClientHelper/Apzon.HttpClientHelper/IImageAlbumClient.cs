using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IImageAlbumClient
{
	Task<DataTable> LoadData();

	Task<HttpResult> Saves(DataTable dt);

	Task<string> DeleteDocs(string id);
}
