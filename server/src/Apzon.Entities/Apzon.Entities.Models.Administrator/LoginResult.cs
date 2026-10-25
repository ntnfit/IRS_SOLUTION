namespace Apzon.Entities.Models.Administrator;

public class LoginResult
{
	public object Content { get; set; }

	public string Message { get; set; }

	public UserLicenseImpl LicenseInfo { get; set; }

	public LoginResult()
	{
	}

	public LoginResult(string msg)
	{
		Message = msg;
	}

	public LoginResult(string msg, object obj)
	{
		Message = msg;
		Content = obj;
	}

	public LoginResult(object obj)
	{
		Message = "";
		Content = obj;
	}

	public LoginResult(string msg, object obj, UserLicenseImpl licenseInfo)
	{
		Message = msg;
		Content = obj;
		LicenseInfo = licenseInfo;
	}
}
