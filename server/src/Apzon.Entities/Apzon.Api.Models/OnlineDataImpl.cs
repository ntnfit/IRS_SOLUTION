namespace Apzon.Api.Models;

public class OnlineDataImpl
{
	public string Guid { get; set; }

	public string ServerAddress { get; set; }

	public string Domain { get; set; }

	public string DatabaseName { get; set; }

	public object Data { get; set; }
}
