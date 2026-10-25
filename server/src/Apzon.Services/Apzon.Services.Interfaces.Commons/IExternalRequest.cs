namespace Apzon.Services.Interfaces.Commons;

public interface IExternalRequest
{
	bool CheckExternalRequestToken(string token);

	void SaveExternalRequestToken(string token, string status, string errMess);
}
