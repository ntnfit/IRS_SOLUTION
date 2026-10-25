using System.Collections.Generic;
using System.Data;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Production;

namespace Apzon.Services.Interfaces.Commons;

public interface IFile
{
	HttpResult DMSSaveAttachment(DataTable dt);

	HttpResult SaveAttachmentFiles(List<AttachmentFile_Impl> lstFiles, Dictionary<string, object> code, string table);

	void UpdateTargetPath(List<AttachmentFile_Impl> lstFiles, int atcEntry);
}
