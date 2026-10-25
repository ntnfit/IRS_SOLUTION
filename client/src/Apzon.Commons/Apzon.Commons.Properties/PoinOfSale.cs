using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Apzon.Commons.Properties;

[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
public class PoinOfSale
{
	private static ResourceManager resourceMan;

	private static CultureInfo resourceCulture;

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static ResourceManager ResourceManager
	{
		get
		{
			if (resourceMan == null)
			{
				resourceMan = new ResourceManager("Apzon.Commons.Properties.PoinOfSale", typeof(PoinOfSale).Assembly);
			}
			return resourceMan;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static CultureInfo Culture
	{
		get
		{
			return resourceCulture;
		}
		set
		{
			resourceCulture = value;
		}
	}

	public static string msg_BillRptNotFound => ResourceManager.GetString("msg_BillRptNotFound", resourceCulture);

	public static string msg_ChangeDue => ResourceManager.GetString("msg_ChangeDue", resourceCulture);

	public static string msg_CusNotFound => ResourceManager.GetString("msg_CusNotFound", resourceCulture);

	public static string msg_DialogAbort => ResourceManager.GetString("msg_DialogAbort", resourceCulture);

	public static string msg_DialogCancel => ResourceManager.GetString("msg_DialogCancel", resourceCulture);

	public static string msg_DialogIgnore => ResourceManager.GetString("msg_DialogIgnore", resourceCulture);

	public static string msg_DialogNo => ResourceManager.GetString("msg_DialogNo", resourceCulture);

	public static string msg_DialogOk => ResourceManager.GetString("msg_DialogOk", resourceCulture);

	public static string msg_DialogRetry => ResourceManager.GetString("msg_DialogRetry", resourceCulture);

	public static string msg_DialogYes => ResourceManager.GetString("msg_DialogYes", resourceCulture);

	public static string msg_DocEntNotExist => ResourceManager.GetString("msg_DocEntNotExist", resourceCulture);

	public static string msg_ExitConfirm => ResourceManager.GetString("msg_ExitConfirm", resourceCulture);

	public static string msg_GetStoreErr => ResourceManager.GetString("msg_GetStoreErr", resourceCulture);

	public static string msg_GetTableStrucErr => ResourceManager.GetString("msg_GetTableStrucErr", resourceCulture);

	public static string msg_Hold => ResourceManager.GetString("msg_Hold", resourceCulture);

	public static string msg_HoldSuccess => ResourceManager.GetString("msg_HoldSuccess", resourceCulture);

	public static string msg_InvalidRefundQty => ResourceManager.GetString("msg_InvalidRefundQty", resourceCulture);

	public static string msg_ItemCodeNotExist => ResourceManager.GetString("msg_ItemCodeNotExist", resourceCulture);

	public static string msg_ItemCodeNotInList => ResourceManager.GetString("msg_ItemCodeNotInList", resourceCulture);

	public static string msg_ItemNotInRefund => ResourceManager.GetString("msg_ItemNotInRefund", resourceCulture);

	public static string msg_MemberNotChosen => ResourceManager.GetString("msg_MemberNotChosen", resourceCulture);

	public static string msg_NoItemInList => ResourceManager.GetString("msg_NoItemInList", resourceCulture);

	public static string msg_NoRefundLines => ResourceManager.GetString("msg_NoRefundLines", resourceCulture);

	public static string msg_OpenDrawerErr => ResourceManager.GetString("msg_OpenDrawerErr", resourceCulture);

	public static string msg_PaySuccess => ResourceManager.GetString("msg_PaySuccess", resourceCulture);

	public static string msg_Print => ResourceManager.GetString("msg_Print", resourceCulture);

	public static string msg_ShutDown => ResourceManager.GetString("msg_ShutDown", resourceCulture);

	public static string msg_SignOutConfirm => ResourceManager.GetString("msg_SignOutConfirm", resourceCulture);

	public static string msg_VoidCurrentTrans => ResourceManager.GetString("msg_VoidCurrentTrans", resourceCulture);

	public static string msg_VoidTransErr => ResourceManager.GetString("msg_VoidTransErr", resourceCulture);

	public static string msg_VoidTransSuc => ResourceManager.GetString("msg_VoidTransSuc", resourceCulture);

	internal PoinOfSale()
	{
	}
}
