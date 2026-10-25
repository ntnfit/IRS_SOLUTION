using System;
using System.Collections.Generic;
using System.Data;
using Apzon.Entities.Models.Items;

namespace Apzon.Services.Interfaces;

public interface ISpecialBP_LineSPP1
{
	bool CheckLineSPP1(string itemCode, string cardCode, int lineNum);

	DataTable SearchLineSPP1(string itemCode, string cardCode);

	string SaveDoc(List<SpecialBP_LineSPP1Impl> LineSPP1);

	DataTable GetMaxLineNum(string itemCode, string cardCode);

	string DeleteLineSPP1(List<Tuple<string, string>> obj);
}
