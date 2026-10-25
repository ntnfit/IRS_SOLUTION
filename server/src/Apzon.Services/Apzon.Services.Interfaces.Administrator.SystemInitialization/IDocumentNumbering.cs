using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Administrator.SystemInitialization;

public interface IDocumentNumbering
{
	DataTable LoadDoc(int userId);

	HttpResult Save(DataSet dt);

	DataTable GetDataSeries(int userId);
}
