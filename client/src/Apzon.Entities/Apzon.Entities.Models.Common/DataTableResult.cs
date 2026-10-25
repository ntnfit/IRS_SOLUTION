using System.Data;

namespace Apzon.Entities.Models.Common;

public class DataTableResult
{
	public DataTable Data { get; set; }

	public string Message { get; set; }

	public DataTableResult()
	{
	}

	public DataTableResult(string msg)
	{
		Message = msg;
		Data = new DataTable();
	}

	public DataTableResult(string msg, DataTable data)
	{
		Message = msg;
		Data = data;
	}

	public DataTableResult(DataTable data)
	{
		Message = "";
		Data = data;
	}
}
