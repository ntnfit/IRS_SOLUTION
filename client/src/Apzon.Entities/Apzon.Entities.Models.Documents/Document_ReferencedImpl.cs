namespace Apzon.Entities.Models.Documents;

public class Document_ReferencedImpl
{
	public int LineNum { get; set; }

	public string RefType { get; set; }

	public int RefDocEntr { get; set; }

	public int RefDocNum { get; set; }

	public int ExtDocNum { get; set; }

	public int RefObjType { get; set; }

	public string Remark { get; set; }
}
