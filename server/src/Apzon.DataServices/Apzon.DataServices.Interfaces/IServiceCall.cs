using System.Collections.Generic;
using System.Data;
using Apzon.Entities.Models.Production;

namespace Apzon.DataServices.Interfaces;

public interface IServiceCall : IRepository<DataTable>
{
	string Save(DataRow doc, DataTable line, DataTable activiti, DataTable expenses, DataTable solution, List<AttachmentFile_Impl> attachment, string sessionId);
}
