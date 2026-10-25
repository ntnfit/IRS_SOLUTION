using System.Data;

namespace Apzon.Services.Interfaces.HumanResources;

public interface IEmpRelatives
{
	DataSet GetDetail(int relaId, int empId);

	DataTable GetAllRecords(int empId);

	string SaveDocDataSet(DataSet ds, string action);

	bool CheckDoc(int relaId, int empId);

	string DeleteDoc(int relaId, int empId);

	int GetNextId(int empId);

	DataTable ExistFileInDoc(int relaId, int empId, string fileName, string fileExt);

	string GetRecord(int relaId, int empId, int type);

	DataTable Search(DataTable dtDoc);
}
