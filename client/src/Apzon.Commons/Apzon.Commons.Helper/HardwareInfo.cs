using System;
using System.Diagnostics;
using System.Management;

namespace Apzon.Commons.Helper;

public static class HardwareInfo
{
	public static string GetProcessorId()
	{
		try
		{
			ManagementObjectCollection instances = new ManagementClass("win32_processor").GetInstances();
			string result = string.Empty;
			using (ManagementObjectCollection.ManagementObjectEnumerator managementObjectEnumerator = instances.GetEnumerator())
			{
				if (managementObjectEnumerator.MoveNext())
				{
					result = ((ManagementObject)managementObjectEnumerator.Current).Properties["processorID"].Value.ToString();
				}
			}
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}

	public static string GetHDDSerialNo()
	{
		ManagementObjectCollection instances = new ManagementClass("Win32_LogicalDisk").GetInstances();
		string text = "";
		foreach (ManagementObject item in instances)
		{
			text += Convert.ToString(item["VolumeSerialNumber"]);
		}
		return text;
	}

	public static string GetMACAddress()
	{
		ManagementObjectCollection instances = new ManagementClass("Win32_NetworkAdapterConfiguration").GetInstances();
		string text = string.Empty;
		foreach (ManagementObject item in instances)
		{
			if (text == string.Empty && (bool)item["IPEnabled"])
			{
				text = item["MacAddress"].ToString();
			}
			item.Dispose();
		}
		return text.Replace(":", "");
	}

	public static string GetBoardMaker()
	{
		foreach (ManagementObject item in new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BaseBoard").Get())
		{
			try
			{
				return item.GetPropertyValue("Manufacturer").ToString();
			}
			catch
			{
			}
		}
		return "Board Maker: Unknown";
	}

	public static string GetBoardProductId()
	{
		foreach (ManagementObject item in new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BaseBoard").Get())
		{
			try
			{
				return item.GetPropertyValue("Product").ToString();
			}
			catch
			{
			}
		}
		return "Product: Unknown";
	}

	public static string GetCdRomDrive()
	{
		foreach (ManagementObject item in new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_CDROMDrive").Get())
		{
			try
			{
				return item.GetPropertyValue("Drive").ToString();
			}
			catch
			{
			}
		}
		return "CD ROM Drive Letter: Unknown";
	}

	public static string GetBIOSmaker()
	{
		foreach (ManagementObject item in new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BIOS").Get())
		{
			try
			{
				return item.GetPropertyValue("Manufacturer").ToString();
			}
			catch
			{
			}
		}
		return "BIOS Maker: Unknown";
	}

	public static string GetBIOSserNo()
	{
		foreach (ManagementObject item in new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BIOS").Get())
		{
			try
			{
				return item.GetPropertyValue("SerialNumber").ToString();
			}
			catch
			{
			}
		}
		return "BIOS Serial Number: Unknown";
	}

	public static string GetBIOScaption()
	{
		foreach (ManagementObject item in new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BIOS").Get())
		{
			try
			{
				return item.GetPropertyValue("Caption").ToString();
			}
			catch
			{
			}
		}
		return "BIOS Caption: Unknown";
	}

	public static string GetAccountName()
	{
		foreach (ManagementObject item in new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_UserAccount").Get())
		{
			try
			{
				return item.GetPropertyValue("Name").ToString();
			}
			catch
			{
			}
		}
		return "User Account Name: Unknown";
	}

	public static string GetPhysicalMemory()
	{
		ManagementScope scope = new ManagementScope();
		ObjectQuery query = new ObjectQuery("SELECT Capacity FROM Win32_PhysicalMemory");
		ManagementObjectCollection managementObjectCollection = new ManagementObjectSearcher(scope, query).Get();
		long num = 0L;
		long num2 = 0L;
		foreach (ManagementObject item in managementObjectCollection)
		{
			num2 = Convert.ToInt64(item["Capacity"]);
			num += num2;
		}
		return num / 1024 / 1024 + "MB";
	}

	public static string GetNoRamSlots()
	{
		int num = 0;
		ManagementScope scope = new ManagementScope();
		ObjectQuery query = new ObjectQuery("SELECT MemoryDevices FROM Win32_PhysicalMemoryArray");
		foreach (ManagementObject item in new ManagementObjectSearcher(scope, query).Get())
		{
			num = Convert.ToInt32(item["MemoryDevices"]);
		}
		return num.ToString();
	}

	public static string GetCPUManufacturer()
	{
		string text = string.Empty;
		foreach (ManagementObject instance in new ManagementClass("Win32_Processor").GetInstances())
		{
			if (text == string.Empty)
			{
				text = instance.Properties["Manufacturer"].Value.ToString();
			}
		}
		return text;
	}

	public static int GetCPUCurrentClockSpeed()
	{
		int num = 0;
		foreach (ManagementObject instance in new ManagementClass("Win32_Processor").GetInstances())
		{
			if (num == 0)
			{
				num = Convert.ToInt32(instance.Properties["CurrentClockSpeed"].Value.ToString());
			}
		}
		return num;
	}

	public static string GetDefaultIPGateway()
	{
		ManagementObjectCollection instances = new ManagementClass("Win32_NetworkAdapterConfiguration").GetInstances();
		string text = string.Empty;
		foreach (ManagementObject item in instances)
		{
			if (text == string.Empty && (bool)item["IPEnabled"])
			{
				text = item["DefaultIPGateway"].ToString();
			}
			item.Dispose();
		}
		return text.Replace(":", "");
	}

	public static double? GetCpuSpeedInGHz()
	{
		double? result = null;
		using (ManagementClass managementClass = new ManagementClass("Win32_Processor"))
		{
			using ManagementObjectCollection.ManagementObjectEnumerator managementObjectEnumerator = managementClass.GetInstances().GetEnumerator();
			if (managementObjectEnumerator.MoveNext())
			{
				ManagementObject managementObject = (ManagementObject)managementObjectEnumerator.Current;
				result = 0.001 * (double)(uint)managementObject.Properties["CurrentClockSpeed"].Value;
				return result;
			}
		}
		return result;
	}

	public static string GetCurrentLanguage()
	{
		foreach (ManagementObject item in new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BIOS").Get())
		{
			try
			{
				return item.GetPropertyValue("CurrentLanguage").ToString();
			}
			catch
			{
			}
		}
		return "BIOS Maker: Unknown";
	}

	public static string GetOSInformation()
	{
		foreach (ManagementObject item in new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem").Get())
		{
			try
			{
				return ((string)item["Caption"]).Trim() + ", " + (string)item["Version"] + ", " + (string)item["OSArchitecture"];
			}
			catch
			{
			}
		}
		return "BIOS Maker: Unknown";
	}

	public static string GetProcessorInformation()
	{
		ManagementObjectCollection instances = new ManagementClass("win32_processor").GetInstances();
		string result = string.Empty;
		foreach (ManagementObject item in instances)
		{
			string text = (string)item["Name"];
			text = text.Replace("(TM)", "™").Replace("(tm)", "™").Replace("(R)", "®")
				.Replace("(r)", "®")
				.Replace("(C)", "©")
				.Replace("(c)", "©")
				.Replace("    ", " ")
				.Replace("  ", " ");
			result = text + ", " + (string)item["Caption"] + ", " + (string)item["SocketDesignation"];
		}
		return result;
	}

	public static string GetComputerName()
	{
		ManagementObjectCollection instances = new ManagementClass("Win32_ComputerSystem").GetInstances();
		string result = string.Empty;
		foreach (ManagementObject item in instances)
		{
			result = (string)item["Name"];
		}
		return result;
	}
}
