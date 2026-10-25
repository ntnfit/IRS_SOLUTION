using System;

namespace Apzon.Forms.HumanResources.Biometric.Info;

public class MachineInfo
{
	public int MachineNumber { get; set; }

	public int IndRegID { get; set; }

	public string DateTimeRecord { get; set; }

	public string VerifyMode { get; set; }

	public string InOutMode { get; set; }

	public DateTime DateOnlyRecord => DateTime.Parse(DateTime.Parse(DateTimeRecord).ToString("yyyy-MM-dd"));

	public DateTime TimeOnlyRecord => DateTime.Parse(DateTime.Parse(DateTimeRecord).ToString("hh:mm:ss tt"));
}
