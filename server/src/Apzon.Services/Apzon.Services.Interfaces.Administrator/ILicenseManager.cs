using System.Data;

namespace Apzon.Services.Interfaces.Administrator;

public interface ILicenseManager
{
	string GetServerDate();

	string Save(string bpcode, string licenseNo, string hardwareKey, string file, string fileId, string licenseType, string dateCreate, DataTable dtLicecseInfo);

	DataTable GetUser();

	DataTable GetLicenseUser(string licenseNo, string licenseType);

	DataSet GetLicense(string hardwareKey);

	string SaveUser(DataTable dt, string database, string licenseId);

	DataTable GetOffline();

	string UpdateSession(string session);

	string DeleteAllLicenseUser(string hardwareKey);

	void UpdateDataTrial(string userName);

	void UpdateSessionUser(string session, string userName, string database, string clientIp, string clientMachine, string clientLogon);
}
