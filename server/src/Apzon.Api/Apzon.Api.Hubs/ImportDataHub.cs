namespace Apzon.Api.Hubs;

public class ImportDataHub : BaseHub<ImportDataHub>
{
	public void UpdateStatus(string status)
	{
		if (CurrentUser != null)
		{
			for (int i = 0; i < CurrentUser.Count; i++)
			{
				CurrentUser[i].UpdateStatus(status);
			}
		}
	}
}
