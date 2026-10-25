namespace Apzon.Entities.Models.HumanResources.Biometric;

public class GLogData
{
	public int no;

	public int vEnrollNumber;

	public int vGranted;

	public int vMethod;

	public int vDoorMode;

	public int vFunNumber;

	public int vSensor;

	public int vYear;

	public int vMonth;

	public int vDay;

	public int vHour;

	public int vMinute;

	public int vSecond;

	public int No => no;

	public string Result
	{
		get
		{
			if (vGranted != 1)
			{
				return "Denied";
			}
			return "Granted";
		}
	}

	public string ID
	{
		get
		{
			if (vEnrollNumber != -1)
			{
				return $"{vEnrollNumber:D8}";
			}
			return "NONE";
		}
	}

	public string DoorMode => vDoorMode switch
	{
		0 => "Any", 
		1 => "Finger", 
		2 => "CD or FP", 
		3 => "ID&FP or CD", 
		4 => "ID&FP or ID&CD", 
		5 => "ID&FP or CD&FP", 
		6 => "Open", 
		7 => "Close", 
		8 => "Card", 
		9 => "ID or FP", 
		10 => "ID or CD", 
		11 => "ID&CD", 
		12 => "CD&FP", 
		13 => "ID&FP", 
		14 => "ID&CD&FP", 
		_ => "Unknown", 
	};

	public string Sensor
	{
		get
		{
			if (vSensor != 1)
			{
				return "Close";
			}
			return "Open";
		}
	}

	public string Function
	{
		get
		{
			if (vFunNumber == 40)
			{
				return "NONE";
			}
			return "F" + (vFunNumber / 10 + 1) + "-" + vFunNumber % 10;
		}
	}

	public string Method
	{
		get
		{
			string text = "";
			switch (vMethod & (Constants.GLOG_BY_ID | Constants.GLOG_BY_CD | Constants.GLOG_BY_FP))
			{
			case 0:
				text = "by CD2";
				break;
			case 1:
				text = "by ID";
				break;
			case 2:
				text = "by CD";
				break;
			case 3:
				text = "by ID&CD";
				break;
			case 4:
				text = "by FP";
				break;
			case 5:
				text = "by ID&FP";
				break;
			case 6:
				text = "by CD&FP";
				break;
			case 7:
				text = "by ID&CD&FP";
				break;
			}
			if ((vMethod & Constants.GLOG_BY_DURESS_BIT) == Constants.GLOG_BY_DURESS_BIT)
			{
				text = "[DURESS]";
			}
			if ((vMethod & Constants.GLOG_BY_LIMITTIME) == Constants.GLOG_BY_LIMITTIME)
			{
				text += " [LT]";
			}
			if ((vMethod & Constants.GLOG_BY_ANTIPASS) == Constants.GLOG_BY_ANTIPASS)
			{
				text += " [AP]";
			}
			if ((vMethod & Constants.GLOG_BY_TIMEZONE) == Constants.GLOG_BY_TIMEZONE)
			{
				text += " [TZ]";
			}
			if ((vMethod & Constants.GLOG_BY_AREA) == Constants.GLOG_BY_AREA)
			{
				text += " [FACE]";
			}
			return text;
		}
	}

	public string Time => $"{vYear:D4}-{vMonth:D2}-{vDay:D2} {vHour:D2}:{vMinute:D2}:{vSecond & 0xFF:D2}";

	public bool CapturedPhoto => ((vSecond >> 8) & 0xFF) == 1;
}
