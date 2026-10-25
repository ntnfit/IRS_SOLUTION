using System.Threading.Tasks;
using Apzon.Entities.Models.Documents;

namespace Apzon.DataServices.Interfaces;

public interface IIntergrated
{
	Task<string> Save(DocumentsImpl doc, string sessionId);
}
