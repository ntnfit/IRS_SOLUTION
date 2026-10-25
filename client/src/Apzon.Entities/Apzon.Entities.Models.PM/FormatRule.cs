namespace Apzon.Entities.Models.PM;

public class FormatRule
{
	public string FormId { get; set; }

	public string ItemId { get; set; }

	public string ColumnId { get; set; }

	public string UserCode { get; set; }

	public bool ApplyToRow { get; set; }

	public int ExpressionId { get; set; }

	public string Expression { get; set; }

	public string BackColor { get; set; }

	public string BackColor2 { get; set; }

	public string BorderColor { get; set; }

	public string ForeColor { get; set; }

	public int GradientMode { get; set; }

	public string Image { get; set; }

	public string Font { get; set; }

	public bool Bold { get; set; }

	public bool Italic { get; set; }

	public float Size { get; set; }

	public bool UnderLine { get; set; }

	public bool Strikeout { get; set; }

	public int Unit { get; set; }

	public byte GdiCharSet { get; set; }

	public bool GdiVerticalFont { get; set; }

	public int FontSizeDelta { get; set; }

	public int FontStyleDelta { get; set; }

	public int HAlignment { get; set; }

	public int HotkeyPrefix { get; set; }

	public int Trimming { get; set; }

	public int VAlignment { get; set; }

	public int WordWrap { get; set; }

	public string Measure { get; set; }

	public string RowId { get; set; }
}
