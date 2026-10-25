using System.Data;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.PlanningProduction;

public interface IWorkScheduling
{
	DataTable GetDataDoc(int branch);

	DataTable GetDataLine(int docId);

	DataTable LoadShiftData();

	HttpResult Save(DataSet dsSave, Enums.TransactionType tranType, string objId);

	DataTable GetColumnDoc();

	DataTable GetColumnLine();
}
