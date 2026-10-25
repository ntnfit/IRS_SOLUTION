using System.Drawing;
using DevExpress.XtraWizard;

namespace Apzon.Controls.Controls;

public class Wizard97ModelEx : WizardViewInfo.Wizard97Model
{
	private Rectangle interiorHeaderBounds;

	public Wizard97ModelEx(WizardViewInfo viewInfo)
		: base(viewInfo)
	{
	}

	public override Rectangle GetInteriorHeaderBounds()
	{
		return GetInteriorHeaderBounds(useCached: true, null);
	}

	public override Rectangle GetInteriorPageBounds(BaseWizardPage page)
	{
		Rectangle contentBounds = GetContentBounds();
		int num = GetInteriorHeaderBounds(useCached: false, page).Height + ((WizardViewInfoEx)ViewInfo).GetDividerSizeInternal();
		contentBounds.Y += num;
		contentBounds.Height -= num;
		Wizard.AllowPagePadding = false;
		if (Wizard.AllowPagePadding)
		{
			contentBounds.Inflate(-16, -16);
		}
		return contentBounds;
	}

	private Rectangle GetInteriorHeaderBounds(bool useCached, BaseWizardPage page)
	{
		if (useCached)
		{
			_ = interiorHeaderBounds;
		}
		else
		{
			interiorHeaderBounds = GetInteriorHeaderBoundsCore(page);
		}
		return interiorHeaderBounds;
	}

	private Rectangle GetInteriorHeaderBoundsCore(BaseWizardPage page)
	{
		Rectangle contentBounds = GetContentBounds();
		if (page == null)
		{
			page = (WizardPage)Wizard.SelectedPage;
		}
		if (!(page is WizardPage))
		{
			return base.GetInteriorHeaderBounds();
		}
		WizardAppearanceCollection appearancesInternal = ((WizardViewInfoEx)ViewInfo).AppearancesInternal;
		contentBounds.Height = CalcTextSize(page.Text, appearancesInternal.PageTitle, contentBounds.Width).Height;
		_ = contentBounds.Width;
		contentBounds.Height = 0;
		return contentBounds;
	}
}
