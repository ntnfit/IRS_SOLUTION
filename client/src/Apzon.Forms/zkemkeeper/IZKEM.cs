using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace zkemkeeper;

[ComImport]
[CompilerGenerated]
[Guid("102F4206-E43D-4FC9-BAB0-331CFFE4D25B")]
[TypeIdentifier]
public interface IZKEM
{
	[DispId(1)]
	bool ReadMark
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1)]
		[param: In]
		set;
	}

	[DispId(2)]
	int CommPort
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(2)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(2)]
		[param: In]
		set;
	}

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(3)]
	bool ClearAdministrators([In] int dwMachineNumber);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(4)]
	bool DeleteEnrollData([In] int dwMachineNumber, [In] int dwEnrollNumber, [In] int dwEMachineNumber, [In] int dwBackupNumber);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(5)]
	bool ReadSuperLogData([In] int dwMachineNumber);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(6)]
	bool ReadAllSLogData([In] int dwMachineNumber);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(7)]
	bool ReadGeneralLogData([In] int dwMachineNumber);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(8)]
	bool ReadAllGLogData([In] int dwMachineNumber);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(9)]
	bool EnableUser([In] int dwMachineNumber, [In] int dwEnrollNumber, [In] int dwEMachineNumber, [In] int dwBackupNumber, [In] bool bFlag);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(10)]
	bool EnableDevice([In] int dwMachineNumber, [In] bool bFlag);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(11)]
	bool GetDeviceStatus([In] int dwMachineNumber, [In] int dwStatus, [In][Out] ref int dwValue);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(12)]
	bool GetDeviceInfo([In] int dwMachineNumber, [In] int dwInfo, [In][Out] ref int dwValue);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(13)]
	bool SetDeviceInfo([In] int dwMachineNumber, [In] int dwInfo, [In] int dwValue);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(14)]
	bool SetDeviceTime([In] int dwMachineNumber);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(15)]
	void PowerOnAllDevice();

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(16)]
	bool PowerOffDevice([In] int dwMachineNumber);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(17)]
	bool ModifyPrivilege([In] int dwMachineNumber, [In] int dwEnrollNumber, [In] int dwEMachineNumber, [In] int dwBackupNumber, [In] int dwMachinePrivilege);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(18)]
	void GetLastError([In][Out] ref int dwErrorCode);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(19)]
	bool GetEnrollData([In] int dwMachineNumber, [In] int dwEnrollNumber, [In] int dwEMachineNumber, [In] int dwBackupNumber, [In][Out] ref int dwMachinePrivilege, [In][Out] ref int dwEnrollData, [In][Out] ref int dwPassWord);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(20)]
	bool SetEnrollData([In] int dwMachineNumber, [In] int dwEnrollNumber, [In] int dwEMachineNumber, [In] int dwBackupNumber, [In] int dwMachinePrivilege, [In] ref int dwEnrollData, [In] int dwPassWord);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(21)]
	bool GetDeviceTime([In] int dwMachineNumber, [In][Out] ref int dwYear, [In][Out] ref int dwMonth, [In][Out] ref int dwDay, [In][Out] ref int dwHour, [In][Out] ref int dwMinute, [In][Out] ref int dwSecond);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(22)]
	bool GetGeneralLogData([In] int dwMachineNumber, [In][Out] ref int dwTMachineNumber, [In][Out] ref int dwEnrollNumber, [In][Out] ref int dwEMachineNumber, [In][Out] ref int dwVerifyMode, [In][Out] ref int dwInOutMode, [In][Out] ref int dwYear, [In][Out] ref int dwMonth, [In][Out] ref int dwDay, [In][Out] ref int dwHour, [In][Out] ref int dwMinute);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(23)]
	bool GetSuperLogData([In] int dwMachineNumber, [In][Out] ref int dwTMachineNumber, [In][Out] ref int dwSEnrollNumber, [In][Out] ref int Params4, [In][Out] ref int Params1, [In][Out] ref int Params2, [In][Out] ref int dwManipulation, [In] ref int Params3, [In][Out] ref int dwYear, [In][Out] ref int dwMonth, [In][Out] ref int dwDay, [In][Out] ref int dwHour, [In][Out] ref int dwMinute);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(24)]
	bool GetAllSLogData([In] int dwMachineNumber, [In][Out] ref int dwTMachineNumber, [In][Out] ref int dwSEnrollNumber, [In][Out] ref int Params4, [In][Out] ref int Params1, [In][Out] ref int Params2, [In][Out] ref int dwManipulation, [In][Out] ref int Params3, [In][Out] ref int dwYear, [In][Out] ref int dwMonth, [In][Out] ref int dwDay, [In][Out] ref int dwHour, [In][Out] ref int dwMinute);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(25)]
	bool GetAllGLogData([In] int dwMachineNumber, [In][Out] ref int dwTMachineNumber, [In][Out] ref int dwEnrollNumber, [In][Out] ref int dwEMachineNumber, [In][Out] ref int dwVerifyMode, [In][Out] ref int dwInOutMode, [In][Out] ref int dwYear, [In][Out] ref int dwMonth, [In][Out] ref int dwDay, [In][Out] ref int dwHour, [In][Out] ref int dwMinute);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(26)]
	void ConvertPassword([In] int dwSrcPSW, [In][Out] ref int dwDestPSW, [In] int dwLength);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(27)]
	bool ReadAllUserID([In] int dwMachineNumber);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(28)]
	bool GetAllUserID([In] int dwMachineNumber, [In][Out] ref int dwEnrollNumber, [In][Out] ref int dwEMachineNumber, [In][Out] ref int dwBackupNumber, [In][Out] ref int dwMachinePrivilege, [In][Out] ref int dwEnable);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(29)]
	bool GetSerialNumber([In] int dwMachineNumber, [MarshalAs(UnmanagedType.BStr)] out string dwSerialNumber);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(30)]
	bool ClearKeeperData([In] int dwMachineNumber);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(32)]
	int GetBackupNumber([In] int dwMachineNumber);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(33)]
	bool GetProductCode([In] int dwMachineNumber, [MarshalAs(UnmanagedType.BStr)] out string lpszProductCode);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(34)]
	bool GetFirmwareVersion([In] int dwMachineNumber, [In][Out][MarshalAs(UnmanagedType.BStr)] ref string strVersion);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(35)]
	bool GetSDKVersion([In][Out][MarshalAs(UnmanagedType.BStr)] ref string strVersion);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(36)]
	bool ClearGLog([In] int dwMachineNumber);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(37)]
	int GetFPTempLength([In] ref byte dwEnrollData);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(38)]
	bool Connect_Com([In] int ComPort, [In] int MachineNumber, [In] int BaudRate);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(39)]
	bool Connect_Net([In][MarshalAs(UnmanagedType.BStr)] string IPAdd, [In] int Port);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(40)]
	void Disconnect();

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(41)]
	bool SetUserInfo([In] int dwMachineNumber, [In] int dwEnrollNumber, [In][MarshalAs(UnmanagedType.BStr)] string Name, [In][MarshalAs(UnmanagedType.BStr)] string Password, [In] int Privilege, [In] bool Enabled);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(42)]
	bool GetUserInfo([In] int dwMachineNumber, [In] int dwEnrollNumber, [In][Out][MarshalAs(UnmanagedType.BStr)] ref string Name, [In][Out][MarshalAs(UnmanagedType.BStr)] ref string Password, [In][Out] ref int Privilege, [In][Out] ref bool Enabled);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(43)]
	bool SetDeviceIP([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string IPAddr);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(44)]
	bool GetDeviceIP([In] int dwMachineNumber, [In][Out][MarshalAs(UnmanagedType.BStr)] ref string IPAddr);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(45)]
	bool GetUserTmp([In] int dwMachineNumber, [In] int dwEnrollNumber, [In] int dwFingerIndex, [In][Out] ref byte TmpData, [In][Out] ref int TmpLength);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(46)]
	bool SetUserTmp([In] int dwMachineNumber, [In] int dwEnrollNumber, [In] int dwFingerIndex, [In] ref byte TmpData);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(47)]
	bool GetAllUserInfo([In] int dwMachineNumber, [In][Out] ref int dwEnrollNumber, [In][Out][MarshalAs(UnmanagedType.BStr)] ref string Name, [In][Out][MarshalAs(UnmanagedType.BStr)] ref string Password, [In][Out] ref int Privilege, [In][Out] ref bool Enabled);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(48)]
	bool DelUserTmp([In] int dwMachineNumber, [In] int dwEnrollNumber, [In] int dwFingerIndex);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(49)]
	bool RefreshData([In] int dwMachineNumber);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(50)]
	bool FPTempConvert([In] ref byte TmpData1, [In][Out] ref byte TmpData2, [In][Out] ref int Size);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(51)]
	bool SetCommPassword([In] int CommKey);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(52)]
	bool GetUserGroup([In] int dwMachineNumber, [In] int dwEnrollNumber, [In][Out] ref int UserGrp);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(53)]
	bool SetUserGroup([In] int dwMachineNumber, [In] int dwEnrollNumber, [In] int UserGrp);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(54)]
	bool GetTZInfo([In] int dwMachineNumber, [In] int TZIndex, [In][Out][MarshalAs(UnmanagedType.BStr)] ref string TZ);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(55)]
	bool SetTZInfo([In] int dwMachineNumber, [In] int TZIndex, [In][MarshalAs(UnmanagedType.BStr)] string TZ);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(56)]
	bool GetUnlockGroups([In] int dwMachineNumber, [In][Out][MarshalAs(UnmanagedType.BStr)] ref string Grps);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(57)]
	bool SetUnlockGroups([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string Grps);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(58)]
	bool GetGroupTZs([In] int dwMachineNumber, [In] int GroupIndex, [In][Out] ref int TZs);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(59)]
	bool SetGroupTZs([In] int dwMachineNumber, [In] int GroupIndex, [In] ref int TZs);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(60)]
	bool GetUserTZs([In] int dwMachineNumber, [In] int dwEnrollNumber, [In][Out] ref int TZs);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(61)]
	bool SetUserTZs([In] int dwMachineNumber, [In] int dwEnrollNumber, [In] ref int TZs);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(62)]
	bool ACUnlock([In] int dwMachineNumber, [In] int Delay);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(63)]
	bool GetACFun([In][Out] ref int ACFun);

	[DispId(64)]
	int ConvertBIG5
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(64)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(64)]
		[param: In]
		set;
	}

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(65)]
	bool GetGeneralLogDataStr([In] int dwMachineNumber, [In][Out] ref int dwEnrollNumber, [In][Out] ref int dwVerifyMode, [In][Out] ref int dwInOutMode, [In][Out][MarshalAs(UnmanagedType.BStr)] ref string TimeStr);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(66)]
	bool GetUserTmpStr([In] int dwMachineNumber, [In] int dwEnrollNumber, [In] int dwFingerIndex, [In][Out][MarshalAs(UnmanagedType.BStr)] ref string TmpData, [In][Out] ref int TmpLength);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(67)]
	bool SetUserTmpStr([In] int dwMachineNumber, [In] int dwEnrollNumber, [In] int dwFingerIndex, [In][MarshalAs(UnmanagedType.BStr)] string TmpData);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(68)]
	bool GetEnrollDataStr([In] int dwMachineNumber, [In] int dwEnrollNumber, [In] int dwEMachineNumber, [In] int dwBackupNumber, [In][Out] ref int dwMachinePrivilege, [In][Out][MarshalAs(UnmanagedType.BStr)] ref string dwEnrollData, [In][Out] ref int dwPassWord);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(69)]
	bool SetEnrollDataStr([In] int dwMachineNumber, [In] int dwEnrollNumber, [In] int dwEMachineNumber, [In] int dwBackupNumber, [In] int dwMachinePrivilege, [In][MarshalAs(UnmanagedType.BStr)] string dwEnrollData, [In] int dwPassWord);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(70)]
	bool GetGroupTZStr([In] int dwMachineNumber, [In] int GroupIndex, [In][Out][MarshalAs(UnmanagedType.BStr)] ref string TZs);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(71)]
	bool SetGroupTZStr([In] int dwMachineNumber, [In] int GroupIndex, [In][MarshalAs(UnmanagedType.BStr)] string TZs);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(72)]
	bool GetUserTZStr([In] int dwMachineNumber, [In] int dwEnrollNumber, [In][Out][MarshalAs(UnmanagedType.BStr)] ref string TZs);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(73)]
	bool SetUserTZStr([In] int dwMachineNumber, [In] int dwEnrollNumber, [In][MarshalAs(UnmanagedType.BStr)] string TZs);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(74)]
	bool FPTempConvertStr([In][MarshalAs(UnmanagedType.BStr)] string TmpData1, [In][Out][MarshalAs(UnmanagedType.BStr)] ref string TmpData2, [In] ref int Size);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(75)]
	int GetFPTempLengthStr([In][MarshalAs(UnmanagedType.BStr)] string dwEnrollData);

	[DispId(76)]
	int BASE64
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(76)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(76)]
		[param: In]
		set;
	}

	[DispId(78)]
	uint PIN2
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(78)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(78)]
		[param: In]
		set;
	}

	[DispId(79)]
	int AccGroup
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(79)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(79)]
		[param: In]
		set;
	}

	// C# has no syntax for parameterized property 'AccTimeZones'.
	// Its 'property:' attributes below are ignored by the compiler (CS0657).
	[property: DispId(80)]
	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(80)]
	int get_AccTimeZones([In] int Index);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(80)]
	void set_AccTimeZones([In] int Index, [In] int pVal);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(81)]
	bool GetUserInfoByPIN2([In] int dwMachineNumber, [In][Out][MarshalAs(UnmanagedType.BStr)] ref string Name, [In][Out][MarshalAs(UnmanagedType.BStr)] ref string Password, [In][Out] ref int Privilege, [In][Out] ref bool Enabled);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(82)]
	bool GetUserInfoByCard([In] int dwMachineNumber, [In][Out][MarshalAs(UnmanagedType.BStr)] ref string Name, [In][Out][MarshalAs(UnmanagedType.BStr)] ref string Password, [In][Out] ref int Privilege, [In][Out] ref bool Enabled);

	// C# has no syntax for parameterized property 'CardNumber'.
	// Its 'property:' attributes below are ignored by the compiler (CS0657).
	[property: DispId(83)]
	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(83)]
	int get_CardNumber([In] int Index);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(83)]
	void set_CardNumber([In] int Index, [In] int pVal);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(86)]
	bool CaptureImage([In] bool FullImage, [In][Out] ref int Width, [In][Out] ref int Height, [In][Out] ref byte Image, [In][MarshalAs(UnmanagedType.BStr)] string ImageFile);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(87)]
	bool UpdateFirmware([In][MarshalAs(UnmanagedType.BStr)] string FirmwareFile);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(88)]
	bool StartEnroll([In] int UserID, [In] int FingerID);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(89)]
	bool StartVerify([In] int UserID, [In] int FingerID);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(90)]
	bool StartIdentify();

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(91)]
	bool CancelOperation();

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(92)]
	bool QueryState([In][Out] ref int State);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(93)]
	bool BackupData([In][MarshalAs(UnmanagedType.BStr)] string DataFile);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(94)]
	bool RestoreData([In][MarshalAs(UnmanagedType.BStr)] string DataFile);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(95)]
	bool WriteLCD([In] int Row, [In] int Col, [In][MarshalAs(UnmanagedType.BStr)] string Text);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(96)]
	bool ClearLCD();

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(97)]
	bool Beep([In] int DelayMS);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(98)]
	bool PlayVoice([In] int Position, [In] int Length);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(99)]
	bool PlayVoiceByIndex([In] int Index);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(100)]
	bool EnableClock([In] int Enabled);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(101)]
	bool GetUserIDByPIN2([In] int PIN2, [In][Out] ref int UserID);

	[DispId(102)]
	int PINWidth
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(102)]
		get;
	}

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(103)]
	bool GetPIN2([In] int UserID, [In][Out] ref int PIN2);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(104)]
	bool FPTempConvertNew([In] ref byte TmpData1, [In][Out] ref byte TmpData2, [In][Out] ref int Size);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(105)]
	bool FPTempConvertNewStr([In][MarshalAs(UnmanagedType.BStr)] string TmpData1, [In][Out][MarshalAs(UnmanagedType.BStr)] ref string TmpData2, [In][Out] ref int Size);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(106)]
	bool ReadAllTemplate([In] int dwMachineNumber);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(107)]
	bool DisableDeviceWithTimeOut([In] int dwMachineNumber, [In] int TimeOutSec);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(108)]
	bool SetDeviceTime2([In] int dwMachineNumber, [In] int dwYear, [In] int dwMonth, [In] int dwDay, [In] int dwHour, [In] int dwMinute, [In] int dwSecond);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(109)]
	bool ClearSLog([In] int dwMachineNumber);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(110)]
	bool RestartDevice([In] int dwMachineNumber);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(111)]
	bool GetDeviceMAC([In] int dwMachineNumber, [In][Out][MarshalAs(UnmanagedType.BStr)] ref string sMAC);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(112)]
	bool SetDeviceMAC([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string sMAC);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(113)]
	bool GetWiegandFmt([In] int dwMachineNumber, [In][Out][MarshalAs(UnmanagedType.BStr)] ref string sWiegandFmt);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(114)]
	bool SetWiegandFmt([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string sWiegandFmt);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(115)]
	bool ClearSMS([In] int dwMachineNumber);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(116)]
	bool GetSMS([In] int dwMachineNumber, [In] int ID, [In][Out] ref int Tag, [In][Out] ref int ValidMinutes, [In][Out][MarshalAs(UnmanagedType.BStr)] ref string StartTime, [In][Out][MarshalAs(UnmanagedType.BStr)] ref string Content);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(117)]
	bool SetSMS([In] int dwMachineNumber, [In] int ID, [In] int Tag, [In] int ValidMinutes, [In][MarshalAs(UnmanagedType.BStr)] string StartTime, [In][MarshalAs(UnmanagedType.BStr)] string Content);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(118)]
	bool DeleteSMS([In] int dwMachineNumber, [In] int ID);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(119)]
	bool SetUserSMS([In] int dwMachineNumber, [In] int dwEnrollNumber, [In] int SMSID);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(120)]
	bool DeleteUserSMS([In] int dwMachineNumber, [In] int dwEnrollNumber, [In] int SMSID);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(121)]
	bool GetCardFun([In] int dwMachineNumber, [In][Out] ref int CardFun);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(122)]
	bool ClearUserSMS([In] int dwMachineNumber);

	[DispId(123)]
	int MachineNumber
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(123)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(123)]
		[param: In]
		set;
	}

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(124)]
	bool SetDeviceCommPwd([In] int dwMachineNumber, [In] int CommKey);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(128)]
	bool GetDoorState([In] int MachineNumber, [In][Out] ref int State);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(129)]
	bool GetVendor([In][Out][MarshalAs(UnmanagedType.BStr)] ref string strVendor);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(130)]
	bool GetSensorSN([In] int dwMachineNumber, [In][Out][MarshalAs(UnmanagedType.BStr)] ref string SensorSN);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(131)]
	bool ReadCustData([In] int dwMachineNumber, [In][Out][MarshalAs(UnmanagedType.BStr)] ref string CustData);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(132)]
	bool WriteCustData([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string CustData);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(133)]
	bool BeginBatchUpdate([In] int dwMachineNumber, [In] int UpdateFlag);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(134)]
	bool BatchUpdate([In] int dwMachineNumber);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(135)]
	bool ClearData([In] int dwMachineNumber, [In] int DataFlag);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(136)]
	bool GetDataFile([In] int dwMachineNumber, [In] int DataFlag, [In][MarshalAs(UnmanagedType.BStr)] string FileName);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(137)]
	bool WriteCard([In] int dwMachineNumber, [In] int dwEnrollNumber, [In] int dwFingerIndex1, [In] ref byte TmpData1, [In] int dwFingerIndex2, [In] ref byte TmpData2, [In] int dwFingerIndex3, [In] ref byte TmpData3, [In] int dwFingerIndex4, [In] ref byte TmpData4);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(138)]
	bool GetGeneralExtLogData([In] int dwMachineNumber, [In][Out] ref int dwEnrollNumber, [In][Out] ref int dwVerifyMode, [In][Out] ref int dwInOutMode, [In][Out] ref int dwYear, [In][Out] ref int dwMonth, [In][Out] ref int dwDay, [In][Out] ref int dwHour, [In][Out] ref int dwMinute, [In][Out] ref int dwSecond, [In][Out] ref int dwWorkCode, [In][Out] ref int dwReserved);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(139)]
	bool EmptyCard([In] int dwMachineNumber);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(140)]
	bool GetDeviceStrInfo([In] int dwMachineNumber, [In] int dwInfo, [MarshalAs(UnmanagedType.BStr)] out string Value);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(141)]
	bool GetSysOption([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string Option, [MarshalAs(UnmanagedType.BStr)] out string Value);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(142)]
	bool SetUserInfoEx([In] int dwMachineNumber, [In] int dwEnrollNumber, [In] int VerifyStyle, [In] ref byte Reserved);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(143)]
	bool GetUserInfoEx([In] int dwMachineNumber, [In] int dwEnrollNumber, out int VerifyStyle, out byte Reserved);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(144)]
	bool DeleteUserInfoEx([In] int dwMachineNumber, [In] int dwEnrollNumber);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(145)]
	bool SSR_GetGeneralLogData([In] int dwMachineNumber, [MarshalAs(UnmanagedType.BStr)] out string dwEnrollNumber, out int dwVerifyMode, out int dwInOutMode, out int dwYear, out int dwMonth, out int dwDay, out int dwHour, out int dwMinute, out int dwSecond, [In][Out] ref int dwWorkCode);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(146)]
	bool SSR_GetAllUserInfo([In] int dwMachineNumber, [MarshalAs(UnmanagedType.BStr)] out string dwEnrollNumber, [MarshalAs(UnmanagedType.BStr)] out string Name, [MarshalAs(UnmanagedType.BStr)] out string Password, out int Privilege, out bool Enabled);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(147)]
	bool SSR_GetUserInfo([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string dwEnrollNumber, [MarshalAs(UnmanagedType.BStr)] out string Name, [MarshalAs(UnmanagedType.BStr)] out string Password, out int Privilege, out bool Enabled);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(148)]
	bool SSR_GetUserTmp([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string dwEnrollNumber, [In] int dwFingerIndex, out byte TmpData, out int TmpLength);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(149)]
	bool SSR_GetUserTmpStr([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string dwEnrollNumber, [In] int dwFingerIndex, [MarshalAs(UnmanagedType.BStr)] out string TmpData, out int TmpLength);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(150)]
	bool SSR_DeleteEnrollData([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string dwEnrollNumber, [In] int dwBackupNumber);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(151)]
	bool SSR_SetUserInfo([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string dwEnrollNumber, [In][MarshalAs(UnmanagedType.BStr)] string Name, [In][MarshalAs(UnmanagedType.BStr)] string Password, [In] int Privilege, [In] bool Enabled);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(152)]
	bool SSR_SetUserTmp([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string dwEnrollNumber, [In] int dwFingerIndex, [In] ref byte TmpData);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(153)]
	bool SSR_SetUserTmpStr([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string dwEnrollNumber, [In] int dwFingerIndex, [In][MarshalAs(UnmanagedType.BStr)] string TmpData);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(154)]
	bool SSR_DelUserTmp([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string dwEnrollNumber, [In] int dwFingerIndex);

	// C# has no syntax for parameterized property 'STR_CardNumber'.
	// Its 'property:' attributes below are ignored by the compiler (CS0657).
	[property: DispId(155)]
	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(155)]
	[return: MarshalAs(UnmanagedType.BStr)]
	string get_STR_CardNumber([In] int Index);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(155)]
	void set_STR_CardNumber([In] int Index, [In][MarshalAs(UnmanagedType.BStr)] string pVal);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(156)]
	bool SetWorkCode([In] int WorkCodeID, [In] int AWorkCode);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(157)]
	bool GetWorkCode([In] int WorkCodeID, out int AWorkCode);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(158)]
	bool DeleteWorkCode([In] int WorkCodeID);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(159)]
	bool ClearWorkCode();

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(160)]
	bool ReadAttRule([In] int dwMachineNumber);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(161)]
	bool ReadDPTInfo([In] int dwMachineNumber);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(162)]
	bool SaveTheDataToFile([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string TheFilePath, [In] int FileFlag);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(163)]
	bool ReadTurnInfo([In] int dwMachineNumber);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(164)]
	bool SSR_OutPutHTMLRep([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string dwEnrollNumber, [In][MarshalAs(UnmanagedType.BStr)] string AttFile, [In][MarshalAs(UnmanagedType.BStr)] string UserFile, [In][MarshalAs(UnmanagedType.BStr)] string DeptFile, [In][MarshalAs(UnmanagedType.BStr)] string TimeClassFile, [In][MarshalAs(UnmanagedType.BStr)] string AttruleFile, [In] int BYear, [In] int BMonth, [In] int BDay, [In] int BHour, [In] int BMinute, [In] int BSecond, [In] int EYear, [In] int EMonth, [In] int EDay, [In] int EHour, [In] int EMinute, [In] int ESecond, [In][MarshalAs(UnmanagedType.BStr)] string TempPath, [In][MarshalAs(UnmanagedType.BStr)] string OutFileName, [In] int HTMLFlag, [In] int resv1, [In][MarshalAs(UnmanagedType.BStr)] string resv2);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(165)]
	bool ReadAOptions([In][MarshalAs(UnmanagedType.BStr)] string AOption, [MarshalAs(UnmanagedType.BStr)] out string AValue);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(166)]
	bool ReadRTLog([In] int dwMachineNumber);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(167)]
	bool GetRTLog([In] int dwMachineNumber);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(168)]
	bool GetHIDEventCardNumAsStr([MarshalAs(UnmanagedType.BStr)] out string strHIDEventCardNum);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(169)]
	bool GetStrCardNumber([MarshalAs(UnmanagedType.BStr)] out string ACardNumber);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(170)]
	bool SetStrCardNumber([In][MarshalAs(UnmanagedType.BStr)] string ACardNumber);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(171)]
	bool RegEvent([In] int dwMachineNumber, [In] int EventMask);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(172)]
	bool CancelBatchUpdate([In] int dwMachineNumber);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(173)]
	bool SetSysOption([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string Option, [In][MarshalAs(UnmanagedType.BStr)] string Value);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(174)]
	bool Connect_Modem([In] int ComPort, [In] int MachineNumber, [In] int BaudRate, [In][MarshalAs(UnmanagedType.BStr)] string Telephone);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(175)]
	bool UseGroupTimeZone();

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(176)]
	bool SetHoliday([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string Holiday);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(177)]
	bool GetHoliday([In] int dwMachineNumber, [In][Out][MarshalAs(UnmanagedType.BStr)] ref string Holiday);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(178)]
	bool SetDaylight([In] int dwMachineNumber, [In] int Support, [In][MarshalAs(UnmanagedType.BStr)] string BeginTime, [In][MarshalAs(UnmanagedType.BStr)] string EndTime);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(179)]
	bool GetDaylight([In] int dwMachineNumber, [In][Out] ref int Support, [In][Out][MarshalAs(UnmanagedType.BStr)] ref string BeginTime, [In][Out][MarshalAs(UnmanagedType.BStr)] ref string EndTime);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(180)]
	bool SSR_SetUnLockGroup([In] int dwMachineNumber, [In] int CombNo, [In] int Group1, [In] int Group2, [In] int Group3, [In] int Group4, [In] int Group5);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(181)]
	bool SSR_GetUnLockGroup([In] int dwMachineNumber, [In] int CombNo, [In][Out] ref int Group1, [In][Out] ref int Group2, [In][Out] ref int Group3, [In][Out] ref int Group4, [In][Out] ref int Group5);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(182)]
	bool SSR_SetGroupTZ([In] int dwMachineNumber, [In] int GroupNo, [In] int Tz1, [In] int Tz2, [In] int Tz3, [In] int VaildHoliday, [In] int VerifyStyle);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(183)]
	bool SSR_GetGroupTZ([In] int dwMachineNumber, [In] int GroupNo, [In][Out] ref int Tz1, [In][Out] ref int Tz2, [In][Out] ref int Tz3, [In][Out] ref int VaildHoliday, [In][Out] ref int VerifyStyle);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(184)]
	bool SSR_GetHoliday([In] int dwMachineNumber, [In] int HolidayID, [In][Out] ref int BeginMonth, [In][Out] ref int BeginDay, [In][Out] ref int EndMonth, [In][Out] ref int EndDay, [In][Out] ref int TimeZoneID);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(185)]
	bool SSR_SetHoliday([In] int dwMachineNumber, [In] int HolidayID, [In] int BeginMonth, [In] int BeginDay, [In] int EndMonth, [In] int EndDay, [In] int TimeZoneID);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(186)]
	bool GetPlatform([In] int dwMachineNumber, [In][Out][MarshalAs(UnmanagedType.BStr)] ref string Platform);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(187)]
	bool SSR_SetUserSMS([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string dwEnrollNumber, [In] int SMSID);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(188)]
	bool SSR_DeleteUserSMS([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string dwEnrollNumber, [In] int SMSID);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(189)]
	bool IsTFTMachine([In] int dwMachineNumber);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(190)]
	bool SSR_EnableUser([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string dwEnrollNumber, [In] bool bFlag);

	[DispId(191)]
	int SSRPin
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(191)]
		get;
	}

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(192)]
	bool SendCMDMsg([In] int dwMachineNumber, [In] int Param1, [In] int Param2);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(193)]
	bool SendFile([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string FileName);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(194)]
	bool SetLanguageByID([In] int dwMachineNumber, [In] int LanguageID, [In][MarshalAs(UnmanagedType.BStr)] string Language);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(195)]
	bool ReadFile([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string FileName, [In][MarshalAs(UnmanagedType.BStr)] string FilePath);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(196)]
	bool SetLastCount([In] int count);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(197)]
	bool SetCustomizeAttState([In] int dwMachineNumber, [In] int StateID, [In] int NewState);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(198)]
	bool DelCustomizeAttState([In] int dwMachineNumber, [In] int StateID);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(199)]
	bool EnableCustomizeAttState([In] int dwMachineNumber, [In] int StateID, [In] int Enable);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(200)]
	bool SetCustomizeVoice([In] int dwMachineNumber, [In] int VoiceID, [In][MarshalAs(UnmanagedType.BStr)] string FileName);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(201)]
	bool DelCustomizeVoice([In] int dwMachineNumber, [In] int VoiceID);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(202)]
	bool EnableCustomizeVoice([In] int dwMachineNumber, [In] int VoiceID, [In] int Enable);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(203)]
	bool StartEnrollEx([In][MarshalAs(UnmanagedType.BStr)] string UserID, [In] int FingerID, [In] int Flag);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(215)]
	bool SSR_SetUserTmpExt([In] int dwMachineNumber, [In] int IsDeleted, [In][MarshalAs(UnmanagedType.BStr)] string dwEnrollNumber, [In] int dwFingerIndex, [In] ref byte TmpData);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(216)]
	bool SSR_DelUserTmpExt([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string dwEnrollNumber, [In] int dwFingerIndex);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(217)]
	bool SSR_DeleteEnrollDataExt([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string dwEnrollNumber, [In] int dwBackupNumber);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(218)]
	bool SSR_GetWorkCode([In] int AWorkCode, [MarshalAs(UnmanagedType.BStr)] out string Name);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(219)]
	bool SSR_SetWorkCode([In] int AWorkCode, [In][MarshalAs(UnmanagedType.BStr)] string Name);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(220)]
	bool SSR_DeleteWorkCode([In] int PIN);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(221)]
	bool SSR_ClearWorkCode();

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(222)]
	bool SSR_GetSuperLogData([In] int MachineNumber, out int Number, [MarshalAs(UnmanagedType.BStr)] out string Admin, [MarshalAs(UnmanagedType.BStr)] out string User, out int Manipulation, [MarshalAs(UnmanagedType.BStr)] out string Time, out int Params1, out int Params2, out int Params3);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(223)]
	bool SSR_SetShortkey([In] int ShortKeyID, [In] int ShortKeyFun, [In] int StateCode, [In][MarshalAs(UnmanagedType.BStr)] string StateName, [In] int StateAutoChange, [In][MarshalAs(UnmanagedType.BStr)] string StateAutoChangeTime);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(224)]
	bool SSR_GetShortkey([In] int ShortKeyID, [In][Out] ref int ShortKeyFun, [In][Out] ref int StateCode, [In][Out][MarshalAs(UnmanagedType.BStr)] ref string StateName, [In][Out] ref int AutoChange, [In][Out][MarshalAs(UnmanagedType.BStr)] ref string AutoChangeTime);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(225)]
	bool Connect_USB([In] int MachineNumber);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(226)]
	bool GetSuperLogData2([In] int dwMachineNumber, [In][Out] ref int dwTMachineNumber, [In][Out] ref int dwSEnrollNumber, [In][Out] ref int Params4, [In][Out] ref int Params1, [In][Out] ref int Params2, [In][Out] ref int dwManipulation, [In][Out] ref int Params3, [In][Out] ref int dwYear, [In][Out] ref int dwMonth, [In][Out] ref int dwDay, [In][Out] ref int dwHour, [In][Out] ref int dwMinute, [In][Out] ref int dwSecs);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(230)]
	bool GetUserFace([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string dwEnrollNumber, [In] int dwFaceIndex, [In][Out] ref byte TmpData, [In][Out] ref int TmpLength);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(231)]
	bool SetUserFace([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string dwEnrollNumber, [In] int dwFaceIndex, [In] ref byte TmpData, [In] int TmpLength);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(232)]
	bool DelUserFace([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string dwEnrollNumber, [In] int dwFaceIndex);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(233)]
	bool GetUserFaceStr([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string dwEnrollNumber, [In] int dwFaceIndex, [In][Out][MarshalAs(UnmanagedType.BStr)] ref string TmpData, [In][Out] ref int TmpLength);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(234)]
	bool SetUserFaceStr([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string dwEnrollNumber, [In] int dwFaceIndex, [In][MarshalAs(UnmanagedType.BStr)] string TmpData, [In] int TmpLength);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(235)]
	bool GetUserTmpEx([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string dwEnrollNumber, [In] int dwFingerIndex, out int Flag, out byte TmpData, out int TmpLength);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(236)]
	bool GetUserTmpExStr([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string dwEnrollNumber, [In] int dwFingerIndex, out int Flag, [MarshalAs(UnmanagedType.BStr)] out string TmpData, out int TmpLength);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(237)]
	bool SetUserTmpEx([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string dwEnrollNumber, [In] int dwFingerIndex, [In] int Flag, [In] ref byte TmpData);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(238)]
	bool SetUserTmpExStr([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string dwEnrollNumber, [In] int dwFingerIndex, [In] int Flag, [In][MarshalAs(UnmanagedType.BStr)] string TmpData);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(239)]
	bool MergeTemplate([In][Out] IntPtr Templates, [In] int FingerCount, [In][Out] ref byte TemplateDest, [In][Out] ref int FingerSize);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(240)]
	bool SplitTemplate([In] ref byte Template, [In][Out] IntPtr Templates, [In][Out] ref int FingerCount, [In][Out] ref int FingerSize);

	[DispId(241)]
	int PullMode
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(241)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(241)]
		[param: In]
		set;
	}

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(242)]
	bool ReadUserAllTemplate([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string dwEnrollNumber);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(243)]
	bool UpdateFile([In][MarshalAs(UnmanagedType.BStr)] string FileName);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(244)]
	bool ReadLastestLogData([In] int dwMachineNumber, [In] int NewLog, [In] int dwYear, [In] int dwMonth, [In] int dwDay, [In] int dwHour, [In] int dwMinute, [In] int dwSecond);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(245)]
	bool SetOptionCommPwd([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string CommKey);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(246)]
	bool ReadSuperLogDataEx([In] int dwMachineNumber, [In] int dwYear_S, [In] int dwMonth_S, [In] int dwDay_S, [In] int dwHour_S, [In] int dwMinute_S, [In] int dwSecond_S, [In] int dwYear_E, [In] int dwMonth_E, [In] int dwDay_E, [In] int dwHour_E, [In] int dwMinute_E, [In] int dwSecond_E, [In] int dwLogIndex);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(247)]
	bool GetSuperLogDataEx([In] int dwMachineNumber, [In][Out][MarshalAs(UnmanagedType.BStr)] ref string EnrollNumber, [In][Out] ref int Params4, [In][Out] ref int Params1, [In][Out] ref int Params2, [In][Out] ref int dwManipulation, [In][Out] ref int Params3, [In][Out] ref int dwYear, [In][Out] ref int dwMonth, [In][Out] ref int dwDay, [In][Out] ref int dwHour, [In][Out] ref int dwMinute, [In][Out] ref int dwSecond);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(248)]
	bool GetPhotoByName([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string PhotoName, out byte PhotoData, out int PhotoLength);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(249)]
	bool GetPhotoNamesByTime([In] int dwMachineNumber, [In] int iFlag, [In][MarshalAs(UnmanagedType.BStr)] string sTime, [In][MarshalAs(UnmanagedType.BStr)] string eTime, [MarshalAs(UnmanagedType.BStr)] out string AllPhotoName);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(250)]
	bool ClearPhotoByTime([In] int dwMachineNumber, [In] int iFlag, [In][MarshalAs(UnmanagedType.BStr)] string sTime, [In][MarshalAs(UnmanagedType.BStr)] string eTime);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(251)]
	bool GetPhotoCount([In] int dwMachineNumber, out int count, [In] int iFlag);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(252)]
	bool ClearDataEx([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string TableName);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(253)]
	bool GetDataFileEx([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string SourceFileName, [In][MarshalAs(UnmanagedType.BStr)] string DestFileName);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(254)]
	bool SSR_SetDeviceData([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string TableName, [In][MarshalAs(UnmanagedType.BStr)] string Datas, [In][MarshalAs(UnmanagedType.BStr)] string Options);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(255)]
	bool SSR_GetDeviceData([In] int dwMachineNumber, [MarshalAs(UnmanagedType.BStr)] out string Buffer, [In] int BufferSize, [In][MarshalAs(UnmanagedType.BStr)] string TableName, [In][MarshalAs(UnmanagedType.BStr)] string FiledNames, [In][MarshalAs(UnmanagedType.BStr)] string Filter, [In][MarshalAs(UnmanagedType.BStr)] string Options);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(256)]
	bool UpdateLogo([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string FileName);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(257)]
	bool SetCommuTimeOut([In] int timeOut);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(258)]
	bool SendFileByType([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string FileName, [In] int iType);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(259)]
	bool SetCommProType([In] int proType);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(270)]
	bool SetCompatOldFirmware([In] int nCompatOkdFirmware);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(271)]
	bool Connect_P4P([In][MarshalAs(UnmanagedType.BStr)] string uid);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(272)]
	bool SetDeviceTableData([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string TableName, [In][MarshalAs(UnmanagedType.BStr)] string Datas, [In][MarshalAs(UnmanagedType.BStr)] string Options, out int count);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(273)]
	void GetConnectStatus([In][Out] ref int dwErrorCode);

	[DispId(274)]
	int MaxP4PConnect
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(274)]
		get;
	}

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(275)]
	bool SetManufacturerData([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string Name, [In][MarshalAs(UnmanagedType.BStr)] string Value);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(276)]
	int GetDeviceStatusEx([In] int dwMachineNumber);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(277)]
	void CancelByUser();

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(278)]
	int SSR_GetDeviceDataCount([In][MarshalAs(UnmanagedType.BStr)] string TableName, [In][MarshalAs(UnmanagedType.BStr)] string Filter, [In][MarshalAs(UnmanagedType.BStr)] string Options);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(279)]
	bool SSR_DeleteDeviceData([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string TableName, [In][MarshalAs(UnmanagedType.BStr)] string Datas, [In][MarshalAs(UnmanagedType.BStr)] string Options);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(500)]
	bool ReadTimeGLogData([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string sTime, [In][MarshalAs(UnmanagedType.BStr)] string eTime);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(501)]
	bool DeleteAttlogBetweenTheDate([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string sTime, [In][MarshalAs(UnmanagedType.BStr)] string eTime);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(502)]
	bool DeleteAttlogByTime([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string sTime);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(503)]
	bool ReadNewGLogData([In] int dwMachineNumber);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(506)]
	bool IsNewFirmwareMachine([In] int dwMachineNumber);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(520)]
	bool UploadUserPhoto([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string FileName);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(521)]
	bool DownloadUserPhoto([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string FileName, [In][MarshalAs(UnmanagedType.BStr)] string FilePath);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(522)]
	bool DeleteUserPhoto([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string FileName);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(523)]
	bool GetAllUserPhoto([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string dlDir);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(540)]
	bool SetBellSchDataEx([In] int dwMachineNumber, [In] int weekDay, [In] int Index, [In] int Enable, [In] int Hour, [In] int min, [In] int voice, [In] int way, [In] int InerBellDelay, [In] int ExtBellDelay);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(541)]
	bool GetBellSchDataEx([In] int dwMachineNumber, [In] int weekDay, [In] int Index, out int Enable, out int Hour, out int min, out int voice, out int way, out int InerBellDelay, out int ExtBellDelay);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(542)]
	bool GetDayBellSchCount([In] int dwMachineNumber, out int DayBellCnt);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(543)]
	bool GetMaxBellIDInBellSchData([In] int dwMachineNumber, out int MaxBellID);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(544)]
	bool ReadAllBellSchData([In] int dwMachineNumber);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(545)]
	bool GetEachBellInfo([In] int dwMachineNumber, out int weekDay, out int Index, out int Enable, out int Hour, out int min, out int voice, out int way, out int InerBellDelay, out int ExtBellDelay);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(560)]
	bool SetUserValidDate([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string UserID, [In] int Expires, [In] int ValidCount, [In][MarshalAs(UnmanagedType.BStr)] string StartDate, [In][MarshalAs(UnmanagedType.BStr)] string EndDate);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(561)]
	bool GetUserValidDate([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string UserID, out int Expires, out int ValidCount, [MarshalAs(UnmanagedType.BStr)] out string StartDate, [MarshalAs(UnmanagedType.BStr)] out string EndDate);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(564)]
	bool SetUserValidDateBatch([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string Datas);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(565)]
	bool GetUserValidDateBatch([In] int dwMachineNumber, [MarshalAs(UnmanagedType.BStr)] out string Buffer, [In] int BufferSize);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(570)]
	bool SetUserVerifyStyle([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string dwEnrollNumber, [In] int VerifyStyle, [In] ref byte Reserved);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(571)]
	bool GetUserVerifyStyle([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string dwEnrollNumber, out int VerifyStyle, out byte Reserved);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(574)]
	bool SetUserVerifyStyleBatch([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string Datas, [In] ref byte Reserved);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(575)]
	bool GetUserVerifyStyleBatch([In] int dwMachineNumber, [MarshalAs(UnmanagedType.BStr)] out string Buffer, [In] int BufferSize, out byte Reserved);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(580)]
	bool GetDeviceFirmwareVersion([In] int dwMachineNumber, [In][Out][MarshalAs(UnmanagedType.BStr)] ref string strVersion);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(280)]
	bool SendFileEx([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string FileName, [In][MarshalAs(UnmanagedType.BStr)] string FilePath);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(590)]
	bool UploadTheme([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string FileName, [In][MarshalAs(UnmanagedType.BStr)] string InDevName);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(591)]
	bool UploadPicture([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string FileName, [In][MarshalAs(UnmanagedType.BStr)] string InDevName);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(592)]
	bool DeletePicture([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string FileName);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(593)]
	bool DownloadPicture([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string FileName, [In][MarshalAs(UnmanagedType.BStr)] string FilePath);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(595)]
	bool TurnOffAlarm([In] int dwMachineNumber);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(596)]
	bool CloseAlarm([In] int dwMachineNumber);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(600)]
	bool SSR_SetWorkCodeEx([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string WorkCodeNum, [In][MarshalAs(UnmanagedType.BStr)] string Name);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(601)]
	bool SSR_GetWorkCodeEx([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string WorkCodeNum, [MarshalAs(UnmanagedType.BStr)] out string Name);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(602)]
	bool SSR_DeleteWorkCodeEx([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string WorkCodeNum);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(603)]
	bool SSR_GetGeneralLogDataEx([In] int dwMachineNumber, [MarshalAs(UnmanagedType.BStr)] out string dwEnrollNumber, out int dwVerifyMode, out int dwInOutMode, out int dwYear, out int dwMonth, out int dwDay, out int dwHour, out int dwMinute, out int dwSecond, [MarshalAs(UnmanagedType.BStr)] out string dwWorkCode);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(604)]
	bool SSR_SetWorkCodeExBatch([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string Datas);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(605)]
	bool SSR_GetWorkCodeExBatch([In] int dwMachineNumber, [MarshalAs(UnmanagedType.BStr)] out string Buffer, [In] int BufferSize);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(606)]
	bool SSR_GetWorkCodeExByID([In] int dwMachineNumber, [In] int WorkCodeID, [MarshalAs(UnmanagedType.BStr)] out string WorkCodeNum, [MarshalAs(UnmanagedType.BStr)] out string Name);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(607)]
	bool SSR_GetWorkCodeIDByName([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string workcodeName, out int WorkCodeID);

	[DispId(610)]
	int BatchDataMode
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(610)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(610)]
		[param: In]
		set;
	}

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(620)]
	bool SetEventMode([In] int nType);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(625)]
	bool GetAllSFIDName([In] int dwMachineNumber, [MarshalAs(UnmanagedType.BStr)] out string ShortkeyIDName, [In] int BufferSize1, [MarshalAs(UnmanagedType.BStr)] out string FunctionIDName, [In] int BufferSize2);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(626)]
	bool SetShortkey([In] int dwMachineNumber, [In] int ShortKeyID, [In][MarshalAs(UnmanagedType.BStr)] string ShortKeyName, [In][MarshalAs(UnmanagedType.BStr)] string FunctionName, [In] int ShortKeyFun, [In] int StateCode, [In][MarshalAs(UnmanagedType.BStr)] string StateName, [In][MarshalAs(UnmanagedType.BStr)] string Description, [In] int StateAutoChange, [In][MarshalAs(UnmanagedType.BStr)] string StateAutoChangeTime);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(627)]
	bool GetShortkey([In] int dwMachineNumber, [In] int ShortKeyID, [In][Out][MarshalAs(UnmanagedType.BStr)] ref string ShortKeyName, [In][Out][MarshalAs(UnmanagedType.BStr)] ref string FunctionName, [In][Out] ref int ShortKeyFun, [In][Out] ref int StateCode, [In][Out][MarshalAs(UnmanagedType.BStr)] ref string StateName, [In][Out][MarshalAs(UnmanagedType.BStr)] ref string Description, [In][Out] ref int AutoChange, [In][Out][MarshalAs(UnmanagedType.BStr)] ref string AutoChangeTime);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(630)]
	bool GetAllAppFun([In] int dwMachineNumber, [MarshalAs(UnmanagedType.BStr)] out string AppName, [MarshalAs(UnmanagedType.BStr)] out string FunofAppName);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(631)]
	bool GetAllRole([In] int dwMachineNumber, [MarshalAs(UnmanagedType.BStr)] out string RoleName);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(632)]
	bool GetAppOfRole([In] int dwMachineNumber, [In] int Permission, [MarshalAs(UnmanagedType.BStr)] out string AppName);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(633)]
	bool GetFunOfRole([In] int dwMachineNumber, [In] int Permission, [MarshalAs(UnmanagedType.BStr)] out string FunName);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(634)]
	bool SetPermOfAppFun([In] int dwMachineNumber, [In] int Permission, [In][MarshalAs(UnmanagedType.BStr)] string AppName, [In][MarshalAs(UnmanagedType.BStr)] string FunName);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(635)]
	bool DeletePermOfAppFun([In] int dwMachineNumber, [In] int Permission, [In][MarshalAs(UnmanagedType.BStr)] string AppName, [In][MarshalAs(UnmanagedType.BStr)] string FunName);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(636)]
	bool IsUserDefRoleEnable([In] int dwMachineNumber, [In] int Permission, out bool Enable);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(281)]
	bool SearchDevice([In][MarshalAs(UnmanagedType.BStr)] string commType, [In][MarshalAs(UnmanagedType.BStr)] string address, [MarshalAs(UnmanagedType.BStr)] out string DevBuffer, [In] int DevBufferSize);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(641)]
	bool SetUserIDCardInfo([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string strEnrollNumber, [In] ref byte IDCardData, [In] int DataLen);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(642)]
	bool GetUserIDCardInfo([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string strEnrollNumber, out byte IDCardData, [In][Out] ref int DataLen);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(643)]
	bool DelUserIDCardInfo([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string strEnrollNumber);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(644)]
	bool GetPhotoByNameToFile([In] int dwMachineNumber, [In][MarshalAs(UnmanagedType.BStr)] string PhotoName, [In][MarshalAs(UnmanagedType.BStr)] string LocalFileName);
}
