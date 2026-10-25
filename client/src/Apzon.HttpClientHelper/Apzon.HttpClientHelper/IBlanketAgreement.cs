using System;
using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Documents;

namespace Apzon.HttpClientHelper;

public interface IBlanketAgreement
{
	Task<DataTable> GetLine(int absId);

	Task<DataTable> GetDoc(DocumentSBAImpl condiDoc);

	DataTable GetBlanketAgreementDetails(Tuple<int, int> tupe);

	Task<string> Cancel(int DocEntry);
}
