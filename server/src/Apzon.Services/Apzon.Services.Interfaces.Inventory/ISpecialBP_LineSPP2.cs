using System;
using System.Collections.Generic;
using System.Data;
using Apzon.Entities.Models.Items;

namespace Apzon.Services.Interfaces.Inventory;

public interface ISpecialBP_LineSPP2
{
	bool CheckLineSPP2(string itemCode, string cardCode, int lineNum, int spp2LNum);

	string SaveDoc(List<SpecialBP_LineSPP2Impl> LineSPP2);

	DataTable GetMaxSPP2LNum(string itemCode, string cardCode, int lineNum);

	DataTable SearchLineSPP2(string itemCode, string cardCode, int SPP1LineNum);

	string DeleteLineSPP2(List<Tuple<string, string>> obj);
}
