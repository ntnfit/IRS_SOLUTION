using System;
using System.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.XtraEditors;

namespace Apzon.Forms.Commons.Helper;

internal class LabelTabController
{
	private LabelControl[] labels;

	private object editValueCore;

	public object EditValue
	{
		get
		{
			return editValueCore;
		}
		set
		{
			if (object.Equals(editValueCore, value))
			{
				return;
			}
			editValueCore = value;
			RaiseEditValueChanged();
			LabelControl[] array = labels;
			foreach (LabelControl labelControl in array)
			{
				if (EditValue.Equals(labelControl.Tag))
				{
					labelControl.Font = new Font(labelControl.Font.FontFamily, 10f, FontStyle.Bold);
					labelControl.Appearance.ForeColor = CommonColors.GetQuestionColor(UserLookAndFeel.Default);
				}
				else
				{
					labelControl.Appearance.Reset();
					labelControl.Font = new Font(labelControl.Font.FontFamily, 10f, FontStyle.Regular);
				}
			}
		}
	}

	public event EventHandler EditValueChanged;

	public LabelTabController(object eValue, params LabelControl[] list)
	{
		labels = list;
		EditValue = eValue;
		for (int i = 0; i < list.Length; i++)
		{
			list[i].Click += (object s, EventArgs e) =>
			{
				EditValue = ((LabelControl)s).Tag;
			};
		}
	}

	private void RaiseEditValueChanged()
	{
		EditValueChanged?.Invoke(EditValue, EventArgs.Empty);
	}
}
