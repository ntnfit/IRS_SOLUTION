using System.ComponentModel;

namespace Apzon.Forms.HumanResources.Biometric.FP_CLOCK;

public class FpClockConstants
{
	public enum CURDEVICETYPE
	{
		[Description("this means connection through COM port")]
		DEVICE_COM,
		[Description("this means connection through net work")]
		DEVICE_NET,
		[Description("this means connection through DEVICE_P2S")]
		DEVICE_P2S,
		[Description("this means connection through USB")]
		DEVICE_USB
	}

	public enum DoorStatus
	{
		FORCEOPEN = 1,
		FORCECLOSE,
		SOFTWAREOPEN,
		RESTORETOAUTO,
		REBOOT_FPA_MACHINE,
		DEASSERT_ALARM
	}

	public struct TimeInfo
	{
		public int nYear;

		public int nMonth;

		public int nDay;

		public int nHour;

		public int nMinute;

		public int nDayofWeek;

		public TimeInfo(int init)
		{
			nYear = (nMonth = (nDay = (nHour = (nMinute = (nDayofWeek = init)))));
		}
	}

	public struct PasstimeInfo
	{
		public byte bSHour;

		public byte bSMinute;

		public byte bEHour;

		public byte bEMinute;
	}

	public struct GeneralLogInfo
	{
		public int dwTMachineNumber;

		public int dwEnrollNumber;

		public int dwEMachineNumber;

		public int dwVerifyMode;

		public int dwInout;

		public int dwEvent;

		public int dwYear;

		public int dwMonth;

		public int dwDay;

		public int dwHour;

		public int dwMinute;

		public int dwSecond;
	}

	public struct SuperLogInfo
	{
		public int dwTMachineNumber;

		public int dwSEnrollNumber;

		public int dwSEMachineNumber;

		public int dwGEnrollNumber;

		public int dwGEMachineNumber;

		public int dwFingerNumber;

		public int dwManipulation;

		public int dwYear;

		public int dwMonth;

		public int dwDay;

		public int dwHour;

		public int dwMinute;
	}
}
