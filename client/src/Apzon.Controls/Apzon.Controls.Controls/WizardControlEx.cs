using DevExpress.XtraWizard;

namespace Apzon.Controls.Controls;

public class WizardControlEx : WizardControl
{
	private int _titleHeight;

	public int TitleHeight
	{
		get
		{
			return _titleHeight;
		}
		set
		{
			_titleHeight = value;
		}
	}

	protected override WizardViewInfo CreateViewInfo()
	{
		return new WizardViewInfoEx(this);
	}
}
