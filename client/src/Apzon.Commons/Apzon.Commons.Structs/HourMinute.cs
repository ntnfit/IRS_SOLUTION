using System;

namespace Apzon.Commons.Structs;

public class HourMinute
{
	private readonly DateTime dateTime;

	public HourMinute(int hour, int minute)
	{
		dateTime = new DateTime(1900, 1, 1, hour, minute, 0);
	}
}
