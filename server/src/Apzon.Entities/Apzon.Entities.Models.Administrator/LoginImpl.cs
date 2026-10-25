using System.ComponentModel.DataAnnotations;

namespace Apzon.Entities.Models.Administrator;

public class LoginImpl
{
	public string Doimain { get; set; }

	public string UserName { get; set; }

	[DataType(DataType.Password)]
	public string Password { get; set; }

	public string DbName { get; set; }

	public string ClientVersion { get; set; }

	public string BuildVersion { get; set; }

	public string ClientLocalIpAdress { get; set; }

	public string ClientMachineName { get; set; }

	public string ClientWindowsLogonName { get; set; }
}
