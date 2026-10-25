public class ConfigLogin
{
	public string Remeber { get; set; }

	public string Domain { get; set; }

	public string Database { get; set; }

	public string User { get; set; }

	public string RePass { get; set; }

	public string PassWord { get; set; }

	public ConfigLogin()
	{
	}

	public ConfigLogin(string remember, string database, string user, string rePass, string passWord)
	{
		Remeber = remember;
		Database = database;
		User = user;
		RePass = rePass;
		PassWord = passWord;
	}
}
