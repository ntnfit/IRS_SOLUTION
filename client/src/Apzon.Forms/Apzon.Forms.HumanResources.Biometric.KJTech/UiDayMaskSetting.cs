namespace Apzon.Forms.HumanResources.Biometric.KJTech;

public class UiDayMaskSetting
{
	private DayMaskSetting setting;

	public bool Holiday
	{
		get
		{
			return (setting.DMask & 1) == 1;
		}
		set
		{
			if (value)
			{
				setting.DMask |= 1;
			}
			else
			{
				setting.DMask &= -2;
			}
		}
	}

	public bool Monday
	{
		get
		{
			return (setting.DMask & 4) == 4;
		}
		set
		{
			if (value)
			{
				setting.DMask |= 4;
			}
			else
			{
				setting.DMask &= -5;
			}
		}
	}

	public bool TuesDay
	{
		get
		{
			return (setting.DMask & 8) == 8;
		}
		set
		{
			if (value)
			{
				setting.DMask |= 8;
			}
			else
			{
				setting.DMask &= -9;
			}
		}
	}

	public bool WendsDay
	{
		get
		{
			return (setting.DMask & 0x10) == 16;
		}
		set
		{
			if (value)
			{
				setting.DMask |= 16;
			}
			else
			{
				setting.DMask &= -17;
			}
		}
	}

	public bool ThursDay
	{
		get
		{
			return (setting.DMask & 0x20) == 32;
		}
		set
		{
			if (value)
			{
				setting.DMask |= 32;
			}
			else
			{
				setting.DMask &= -33;
			}
		}
	}

	public bool Friday
	{
		get
		{
			return (setting.DMask & 0x40) == 64;
		}
		set
		{
			if (value)
			{
				setting.DMask |= 64;
			}
			else
			{
				setting.DMask &= -65;
			}
		}
	}

	public bool Saturday
	{
		get
		{
			return (setting.DMask & 0x80) == 128;
		}
		set
		{
			if (value)
			{
				setting.DMask |= 128;
			}
			else
			{
				setting.DMask &= -129;
			}
		}
	}

	public bool Sunday
	{
		get
		{
			return (setting.DMask & 2) == 2;
		}
		set
		{
			if (value)
			{
				setting.DMask |= 2;
			}
			else
			{
				setting.DMask &= -3;
			}
		}
	}

	public UiDayMaskSetting(DayMaskSetting setting)
	{
		this.setting = setting;
	}
}
