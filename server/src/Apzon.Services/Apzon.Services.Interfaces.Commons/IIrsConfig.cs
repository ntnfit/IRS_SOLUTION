using System.Data;

namespace Apzon.Services.Interfaces.Commons;

public interface IIrsConfig
{
	DataTable GetTableStructure(string tblName);
}
