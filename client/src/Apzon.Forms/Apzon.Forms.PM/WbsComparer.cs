using System.Collections.Generic;
using System.Data;
using Apzon.Commons;

namespace Apzon.Forms.PM;

internal class WbsComparer : IComparer<DataRow>
{
	public int Compare(DataRow x, DataRow y)
	{
		if (x != null && y != null)
		{
			return Function.CompareString(Function.ToString(x["Wbs"]), Function.ToString(y["Wbs"]));
		}
		return 0;
	}
}
