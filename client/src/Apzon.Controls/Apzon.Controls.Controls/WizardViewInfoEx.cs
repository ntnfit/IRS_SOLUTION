using DevExpress.XtraWizard;

namespace Apzon.Controls.Controls;

public class WizardViewInfoEx : WizardViewInfo
{
	internal WizardAppearanceCollection AppearancesInternal => PaintAppearance;

	public WizardViewInfoEx(WizardControl control)
		: base(control)
	{
	}

	protected override WizardModelBase CreateWizardModelCore(WizardStyle style)
	{
		if (style == WizardStyle.Wizard97)
		{
			return new Wizard97ModelEx(this);
		}
		return base.CreateWizardModelCore(style);
	}

	internal int GetDividerSizeInternal()
	{
		return GetDividerSize();
	}
}
