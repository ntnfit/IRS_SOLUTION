using System.Data;

namespace Apzon.Services.Interfaces.Commons;

public interface IApprovalDecision
{
	DataTable GetDataByWddCode(int wddCode);

	DataTable GetSourceMailContent(int wddCode);
}
