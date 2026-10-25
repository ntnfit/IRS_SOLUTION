using System.Collections.Generic;
using System.Data;
using Apzon.Entities.Models.Items;

namespace Apzon.DataServices.Interfaces;

public interface ISpecialPrices : IRepository<SpecialBP_Impl>
{
	string SaveDoc(DataSet docs, string sessionId);

	string Delete(string itemCode, string cardCode, string sessionId);

	string SaveDataAreas(List<SpecialBP_LineSPP1Impl> docs, string sessionId);

	string SaveQuantityAreas(List<SpecialBP_LineSPP2Impl> docs, string sessionId);
}
