using System.Data;

namespace Apzon.Services.Interfaces.ProductionAddon;

public interface IReportCompleteForProduction
{
	DataTable GetDocsByBarCode(string batIssue);
}
