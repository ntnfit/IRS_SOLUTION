using System;
using System.Data;
using System.Threading.Tasks;

namespace Apzon.DataServices.Interfaces;

public interface IAlternativeItem
{
	Task<string> Save(string sessionId, Tuple<string, DataTable> tuple);

	Task<string> DeleteData(string sessionId, string itemNo);
}
