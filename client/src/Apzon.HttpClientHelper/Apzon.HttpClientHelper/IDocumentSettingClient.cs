using System.Data;
using System.Threading.Tasks;
using Apzon.Commons;

namespace Apzon.HttpClientHelper;

public interface IDocumentSettingClient
{
	Task<DataSet> GetDataForm();

	Task<string> Save(DataSet dsSave);

	Task<DataTable> GetDataSettingByObjectType(ApzObjectTypes objectType);
}
