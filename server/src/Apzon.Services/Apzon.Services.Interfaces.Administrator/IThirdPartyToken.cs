using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Administrator;

public interface IThirdPartyToken
{
	DataTable GetData(string tokenName, string remarks, string status);

	HttpResult Saves(DataTable dt, string action);

	HttpResult DeleteDoc(int absEntry);

	HttpResult GetMaxEntry();
}
