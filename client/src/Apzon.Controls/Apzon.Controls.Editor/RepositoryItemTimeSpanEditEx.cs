using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;

namespace Apzon.Controls.Editor;

[UserRepositoryItem("RegisterCustomTimeSpanEdit")]
public class RepositoryItemTimeSpanEditEx : RepositoryItemTimeSpanEdit
{
	public const string CustomEditName = "TimeSpanEditEx";

	public bool IsSearchControl { get; set; }

	public override string EditorTypeName => "TimeSpanEditEx";

	static RepositoryItemTimeSpanEditEx()
	{
		RegisterTimeSpanEditEx();
	}

	public override string GetDisplayText(object editValue)
	{
		return base.GetDisplayText(editValue);
	}

	public override string GetDisplayText(FormatInfo format, object editValue)
	{
		if (Mask.UseMaskAsDisplayFormat)
		{
			format.FormatString = Mask.EditMask.ToLowerInvariant().Replace(":", "\\:");
		}
		return base.GetDisplayText(format, editValue);
	}

	public static void RegisterTimeSpanEditEx()
	{
		Image image = null;
		EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo("TimeSpanEditEx", typeof(TimeSpanEditEx), typeof(RepositoryItemTimeSpanEditEx), typeof(TimeSpanEditExViewInfo), new TimeSpanEditExPainter(), designTimeVisible: true, image));
	}

	public override void Assign(RepositoryItem item)
	{
		BeginUpdate();
		try
		{
			base.Assign(item);
			_ = item is RepositoryItemTimeSpanEditEx;
		}
		finally
		{
			EndUpdate();
		}
	}
}
