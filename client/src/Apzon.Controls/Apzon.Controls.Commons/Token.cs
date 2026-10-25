namespace Apzon.Controls.Commons;

public class Token
{
	public string Name { get; set; }

	public DataTypeCtd DataType { get; set; }

	public TypeSyntax Type { get; set; }

	public DataTypeCtd TagetType { get; set; }

	public dynamic Value { get; set; }

	public int LineNum { get; set; }
}
