using System.Data;
using Apzon.Commons;

namespace Apzon.Services.Interfaces.Administrator.SystemInitialization;

public interface IDocumentSettings
{
	DataSet LoadDoc();

	string Save(DataSet dt);

	DataTable GetDataSettingByObjectType(ApzObjectTypes objectType);
}
