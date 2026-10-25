namespace Apzon.Entities.Models.HumanResources.Biometric;

public static class Constants
{
	public static readonly int FP_SIZE_COM = 1416;

	public static readonly int ENROLL_DATA_SIZE = 32 + FP_SIZE_COM * 2;

	public static readonly int PHOTO_SIZE = 8192;

	public static readonly int USER_INFO_SIZE = 20;

	public static readonly int GLOG_BY_ID = 1;

	public static readonly int GLOG_BY_CD = 2;

	public static readonly int GLOG_BY_FP = 4;

	public static readonly int GLOG_BY_DURESS_BIT = 8;

	public static readonly int GLOG_BY_LIMITTIME = 16;

	public static readonly int GLOG_BY_ANTIPASS = 32;

	public static readonly int GLOG_BY_TIMEZONE = 64;

	public static readonly int GLOG_BY_AREA = 128;

	public static readonly int COMPANY_NAME_FONT_BOLD = 2;

	public static readonly int COMPANY_NAME_FONT_ITALIC = 1;

	public static readonly int DB_HOLIDAY_SIZE = 16;

	public static readonly int DB_HOLIDAY_MAX = 256;

	public static readonly int DB_TZONE_SIZE = 24;

	public static readonly int DB_TZONE_MAX = 2048;

	public const int DMASK_HOL = 1;

	public const int DMASK_SUN = 2;

	public const int DMASK_MON = 4;

	public const int DMASK_TUE = 8;

	public const int DMASK_WED = 16;

	public const int DMASK_THU = 32;

	public const int DMASK_FRI = 64;

	public const int DMASK_SAT = 128;

	public const int DMASK_ALL = 255;

	public static readonly int DB_DAYLIGHT_SIZE = 24;

	public static readonly int DB_DAYLIGHT_MAX = 4;

	public static readonly int DB_TMODE_SIZE = 28;

	public static readonly int DB_TMODE_MAX = 10;

	public static readonly int DB_BELLTIME_SIZE = 20;

	public static readonly int DB_BELLTIME_MAX = 20;

	public static readonly int DB_AUTODOOR_SIZE = 24;

	public static readonly int DB_AUTODOOR_MAX = 6;

	public static readonly int DB_AUTOKEY_SIZE = 28;

	public static readonly int DB_AUTOKEY_MAX = 5;

	public static readonly int DB_NOACTKEY_SIZE = 16;

	public const int VS_NONE = 0;

	public const int VS_CLOSED = 1;

	public const int VS_RINGING = 2;

	public const int VS_OPENED = 3;

	public const int VOIP_IMAGE_WIDTH = 320;

	public const int VOIP_IMAGE_HEIGHT = 240;

	public const int PHONE_COUNT = 100;
}
