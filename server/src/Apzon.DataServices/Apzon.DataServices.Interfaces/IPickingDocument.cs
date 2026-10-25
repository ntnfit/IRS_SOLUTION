using System.Collections.Generic;
using System.Data;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;

namespace Apzon.DataServices.Interfaces;

public interface IPickingDocument : IRepository<DocumentsImpl>
{
	HttpResult Save(DataSet ds, string sessionid);

	HttpResult SaveList(List<DataSet> model, string sessionId);

	HttpResult Cancel(int docEntry, string sessionId);
}
