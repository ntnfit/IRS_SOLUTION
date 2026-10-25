using System.Data;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.RoutingAndResource;

public interface IResourceWorkSchedule
{
	DataTable SearchData(string code, string status, Enums.NavigateType type);

	DataTable GetDataLine(string code, int year);

	HttpResult Save(string code, DataSet dtSave, Enums.TransactionType type);
}
