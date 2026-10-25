using Apzon.Commons.Enums;
using Apzon.Global;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Controls.Form;

public class CustomizeDialogButtonText : Localizer
{
	public override string GetLocalizedString(StringId buttonId)
	{
		return buttonId switch
		{
			StringId.XtraMessageBoxYesButtonText => GlobalData.MessageDataCenter[Enums.MessageCode.Yes], 
			StringId.XtraMessageBoxNoButtonText => GlobalData.MessageDataCenter[Enums.MessageCode.No], 
			StringId.XtraMessageBoxAbortButtonText => GlobalData.MessageDataCenter[Enums.MessageCode.Abort], 
			StringId.XtraMessageBoxCancelButtonText => GlobalData.MessageDataCenter[Enums.MessageCode.Cancel], 
			StringId.XtraMessageBoxIgnoreButtonText => GlobalData.MessageDataCenter[Enums.MessageCode.Ignore], 
			StringId.XtraMessageBoxOkButtonText => GlobalData.MessageDataCenter[Enums.MessageCode.Ok], 
			StringId.XtraMessageBoxRetryButtonText => GlobalData.MessageDataCenter[Enums.MessageCode.Retry], 
			_ => base.GetLocalizedString(buttonId), 
		};
	}
}
