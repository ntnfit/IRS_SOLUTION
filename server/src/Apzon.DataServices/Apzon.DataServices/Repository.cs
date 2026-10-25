using System.Data;
using System.Data.SqlClient;
using Apzon.Commons;
using Apzon.Services;

namespace Apzon.DataServices;

public class Repository<TEntity> where TEntity : class
{
	public Apzon.Services.IUnitOfWork ServiceUnitOfWork;

	public IDataService DataService;

	public Repository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService)
	{
		ServiceUnitOfWork = serviceUnitOfWork;
		DataService = dataService;
	}

	public DataTable ExecuteDataTable(string query, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
	{
		return ServiceUnitOfWork.Commons.ExecuteDataTable(query, commandType, parameters);
	}

	public int ExecuteNonQuery(string query, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
	{
		return ServiceUnitOfWork.Commons.ExecuteNonQuery(query, commandType, parameters);
	}

	public DataSet ExecuteData(string query, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
	{
		return ServiceUnitOfWork.Commons.ExecuteData(query, commandType, parameters);
	}

	public string ExecuteScarlar(string query, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
	{
		return Function.ToString(ServiceUnitOfWork.Commons.ExecuteScalar(query, commandType, parameters));
	}

	protected string GetSessionId(string sess)
	{
		string result = "";
		if (!string.IsNullOrEmpty(sess))
		{
			string[] array = sess.Split(',');
			if (array != null && array.Length > 1)
			{
				_ = sess.Split(',')[0];
				result = sess.Split(',')[1];
			}
			else
			{
				result = sess;
			}
		}
		return result;
	}
}
