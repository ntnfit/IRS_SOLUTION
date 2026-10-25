using System.Collections.Generic;
using Apzon.Entities.Models.Documents;

namespace Apzon.DataServices.Interfaces;

public interface IOutsourcing
{
	string SaveDoc(List<DocumentsImpl> lst, string sessionId);
}
