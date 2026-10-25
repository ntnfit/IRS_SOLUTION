using System.Collections.Generic;
using System.Data;
using Apzon.Commons;

namespace Apzon.Services.Interfaces.Commons;

public interface IDiagram
{
	void GetDiagram(ApzObjectTypes type, string objId, ref DataTable dt, List<string> lstChecked = null, bool isreturn = false);
}
