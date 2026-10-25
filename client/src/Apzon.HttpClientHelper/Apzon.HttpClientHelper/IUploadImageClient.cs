using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface IUploadImageClient
{
	Task<DataTable> GetImageList(DataTable dt);

	Task<DataTable> GetItemImageList(DataTable dt);

	Task<DataTable> GetFileByObjectType(string objectType, string objectCode);
}
