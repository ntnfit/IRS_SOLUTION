using System.Data;

namespace Apzon.Entities.Models.Common;

public class DataSetResult
{
	public DataSet Data { get; set; }

	public string Message { get; set; }

	public DataSetResult()
	{
	}

	public DataSetResult(string msg)
	{
		Message = msg;
		Data = new DataSet();
	}

	public DataSetResult(string msg, DataSet data)
	{
		Message = msg;
		Data = data;
	}

	public DataSetResult(DataSet data)
	{
		Message = "";
		Data = data;
	}
}
