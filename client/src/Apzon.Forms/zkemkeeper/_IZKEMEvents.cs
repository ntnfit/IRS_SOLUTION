using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace zkemkeeper;

[ComImport]
[CompilerGenerated]
[Guid("CF83B580-5D32-4C65-B44E-BEDC750CDFA8")]
[InterfaceType(2)]
[TypeIdentifier]
public interface _IZKEMEvents
{
	void _VtblGap1_2();

	[MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(3)]
	void OnEnrollFinger([In] int EnrollNumber, [In] int FingerIndex, [In] int ActionResult, [In] int TemplateLength);

	void _VtblGap2_2();

	[MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(6)]
	void OnConnected();

	[MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(7)]
	void OnDisConnected();

	[MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(8)]
	void OnFinger();

	void _VtblGap3_8();

	[MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(17)]
	void OnAttTransactionEx([In][MarshalAs(UnmanagedType.BStr)] string EnrollNumber, [In] int IsInValid, [In] int AttState, [In] int VerifyMethod, [In] int Year, [In] int Month, [In] int Day, [In] int Hour, [In] int Minute, [In] int Second, [In] int WorkCode);
}
