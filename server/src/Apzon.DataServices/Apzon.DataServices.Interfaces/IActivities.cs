using System.Collections.Generic;
using System.Data;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Production;

namespace Apzon.DataServices.Interfaces;

public interface IActivities : IRepository<Common.Table_Code>
{
	string Save(List<AttachmentFile_Impl> Attachments, DataTable dtAction, string sessionId);

	string Delete(int code, string sessionId);

	string SaveActivityInterval(string sessionId, DataTable dt);

	string SaveActivityStatus(string sessionId, int clgCode, int status);

	string SaveActivityAction(string sessionId, int clgCode, string action);
}
