namespace Apzon.Entities.Models.DMS;

public class SettingReportLineImpl
{
	public int DocEntry { get; set; }

	public int LineNum { get; set; }

	public int FatherNum { get; set; }

	public string ItemType { get; set; }

	public string fieldName { get; set; }

	public int size { get; set; }

	public int textBefore { get; set; }

	public int textAfter { get; set; }

	public string color { get; set; }

	public string alignItems { get; set; }

	public int justifyContent { get; set; }

	public int fontSize { get; set; }

	public int fontWeight { get; set; }

	public string paddingTop { get; set; }

	public string paddingBottom { get; set; }

	public string paddingLeft { get; set; }

	public string paddingRight { get; set; }
}
