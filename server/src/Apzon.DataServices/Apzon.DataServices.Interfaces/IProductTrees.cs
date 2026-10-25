using System.Data;
using Apzon.Entities.Models.Production;

namespace Apzon.DataServices.Interfaces;

public interface IProductTrees : IRepository<BillOfMasterials_Impl_OITT>
{
	string Save(BillOfMasterials_Impl_OITT doc, string sessionId);

	bool Exists(string treeCode, string sessionId);

	string SaveBomVersion(DataSet dtset, string sessionId);

	string DeleteDoc(string code, string sessionId);
}
