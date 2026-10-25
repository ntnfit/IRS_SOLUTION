using System.Collections.Generic;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Production;

namespace Apzon.DataServices.Interfaces;

public interface IAttachment2 : IRepository<List<AttachmentFile_Impl>>
{
	HttpResult Add(List<AttachmentFile_Impl> lstFile, string sesstionId);

	HttpResult Update(int absEntry, List<AttachmentFile_Impl> lstFile, string sesstionId);
}
