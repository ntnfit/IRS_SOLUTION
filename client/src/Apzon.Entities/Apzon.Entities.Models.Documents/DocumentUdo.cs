using System.Data;

namespace Apzon.Entities.Models.Documents;

public class DocumentUdo
{
	public DataTable UdoTable { get; set; }

	public DataTable dtFather { get; set; }

	public DataSet dsChild { get; set; }

	public DocumentUdo()
	{
	}

	public DocumentUdo(string udocode, string udoname)
	{
		UdoTable = new DataTable();
		UdoTable.Columns.Add("Code", typeof(string));
		UdoTable.Columns.Add("Name", typeof(string));
		DataRow dataRow = UdoTable.NewRow();
		dataRow["Code"] = udocode;
		dataRow["Name"] = udoname;
		UdoTable.Rows.Add(dataRow);
	}
}
