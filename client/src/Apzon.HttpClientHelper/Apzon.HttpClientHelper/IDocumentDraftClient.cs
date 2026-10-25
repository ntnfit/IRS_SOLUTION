using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface IDocumentDraftClient
{
	Task<string> CheckIsIns(int docEntry);
}
