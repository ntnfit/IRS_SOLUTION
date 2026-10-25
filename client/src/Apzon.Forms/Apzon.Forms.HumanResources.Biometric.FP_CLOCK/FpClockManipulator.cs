using System;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Forms.Commons;
using AxFP_CLOCKLib;

namespace Apzon.Forms.HumanResources.Biometric.FP_CLOCK;

internal class FpClockManipulator
{
	public const int USER = 1280;

	public const int MYMESSAGE = 1281;

	private int m_nCurSelID = 1;

	private bool m_bDeviceOpened;

	public AxFP_CLOCK axFP_CLOCK = new AxFP_CLOCK();

	public FpClockManipulator()
	{
		try
		{
			axFP_CLOCK.CreateControl();
			axFP_CLOCK.Visible = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public bool OpenCommPort()
	{
		return axFP_CLOCK.OpenCommPort(m_nCurSelID);
	}

	public void CloseCommPort()
	{
		axFP_CLOCK.CloseCommPort();
	}

	public bool DisableDevice()
	{
		return axFP_CLOCK.EnableDevice(m_nCurSelID, 0);
	}

	public bool EnableDevice()
	{
		return axFP_CLOCK.EnableDevice(m_nCurSelID, 1);
	}

	public bool SetIPAddress(string ip, int port, string pass = "0")
	{
		return axFP_CLOCK.SetIPAddress(ref ip, port, 0);
	}

	public string GetSerialNo()
	{
		DisableDevice();
		string lpszSerialNumber = "";
		if (axFP_CLOCK.GetSerialNumber(m_nCurSelID, ref lpszSerialNumber))
		{
			m_bDeviceOpened = true;
			axFP_CLOCK.EnableDevice(m_nCurSelID, 1);
		}
		return lpszSerialNumber;
	}

	public string GetErrorString()
	{
		int dwErrorCode = 0;
		axFP_CLOCK.GetLastError(ref dwErrorCode);
		string empty = string.Empty;
		empty = dwErrorCode switch
		{
			1 => "Can not open COM to connect", 
			2 => "Send out to take place a mistake in the data", 
			3 => "Receive to take place a mistake in the data", 
			4 => "The operation fails", 
			5 => "All datas that store inside the machine read to complete", 
			_ => "Operation successful", 
		};
		axFP_CLOCK.EnableDevice(m_nCurSelID, 1);
		return empty;
	}

	public bool ReadAllGLogData()
	{
		return axFP_CLOCK.ReadAllGLogData(m_nCurSelID);
	}

	public DataTable GetAllGLogDataWithSecond(DateTime fromDate, DateTime toDate, int machineID)
	{
		try
		{
			DataTable tableStructure = WinFuntions.GetTableStructure("APZ_TBD1");
			if (ReadAllGLogData())
			{
				FpClockConstants.GeneralLogInfo generalLogInfo = default;
				bool allGLogDataWithSecond;
				do
				{
					allGLogDataWithSecond = axFP_CLOCK.GetAllGLogDataWithSecond(m_nCurSelID, ref generalLogInfo.dwTMachineNumber, ref generalLogInfo.dwEnrollNumber, ref generalLogInfo.dwEMachineNumber, ref generalLogInfo.dwVerifyMode, ref generalLogInfo.dwInout, ref generalLogInfo.dwEvent, ref generalLogInfo.dwYear, ref generalLogInfo.dwMonth, ref generalLogInfo.dwDay, ref generalLogInfo.dwHour, ref generalLogInfo.dwMinute, ref generalLogInfo.dwSecond);
					if (allGLogDataWithSecond)
					{
						DateTime dateTime = new DateTime(generalLogInfo.dwYear, generalLogInfo.dwMonth, generalLogInfo.dwDay, generalLogInfo.dwHour, generalLogInfo.dwMinute, generalLogInfo.dwSecond);
						if (fromDate.Date <= dateTime.Date && dateTime.Date <= toDate.Date)
						{
							DataRow dataRow = tableStructure.NewRow();
							dataRow["MachineNo"] = machineID;
							dataRow["EnrollNo"] = Function.ParseInt(generalLogInfo.dwEnrollNumber);
							dataRow["DateTimeRecord"] = dateTime.ToString();
							dataRow["Source"] = generalLogInfo.dwVerifyMode;
							dataRow["OriginType"] = Function.ToString(generalLogInfo.dwInout);
							dataRow["ClockDate"] = dateTime.Date;
							dataRow["WorkCode"] = 0;
							tableStructure.Rows.Add(dataRow);
						}
					}
				}
				while (allGLogDataWithSecond);
			}
			return tableStructure;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetAllUserID(int machineID)
	{
		try
		{
			DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OTEM");
			if (DisableDevice())
			{
				int dwEnrollNumber = 0;
				int dwEMachineNumber = 0;
				int dwBackupNumber = 0;
				int dwMachinePrivilege = 0;
				int dwEnable = 0;
				if (axFP_CLOCK.ReadAllUserID(m_nCurSelID))
				{
					int num = 0;
					bool allUserID;
					do
					{
						allUserID = axFP_CLOCK.GetAllUserID(m_nCurSelID, ref dwEnrollNumber, ref dwEMachineNumber, ref dwBackupNumber, ref dwMachinePrivilege, ref dwEnable);
						if (allUserID)
						{
							DataRow dataRow = tableStructure.NewRow();
							dataRow["MachineNo"] = machineID;
							dataRow["EnrollNo"] = dwEnrollNumber;
							tableStructure.Rows.Add(dataRow);
							num++;
						}
					}
					while (allUserID);
				}
				string[] cond = new string[2] { "EnrollNo", "MachineNo" };
				DataTable dataTable = (from row in tableStructure.AsEnumerable()
					group row by string.Join("||", cond.Select((string col) => row[col]?.ToString())) into @group
					select @group.First()).CopyToDataTable();
				foreach (DataRow row in dataTable.Rows)
				{
					row["EnrollName"] = GetUserName(Function.ParseInt(row["EnrollNo"]), machineID);
				}
				tableStructure.Clear();
				tableStructure.Merge(dataTable);
				EnableDevice();
				return tableStructure;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataTable();
		}
	}

	private string GetUserName(int EnrollNo, int machineID)
	{
		try
		{
			DisableDevice();
			string text = "";
			object lpszUserName = new VariantWrapper(text);
			if (!axFP_CLOCK.GetUserName(0, m_nCurSelID, EnrollNo, machineID, ref lpszUserName))
			{
				return text;
			}
			EnableDevice();
			return Function.ToString(lpszUserName);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return "";
		}
	}

	public string GetSerialNumber(string ip, int port)
	{
		try
		{
			axFP_CLOCK.OpenCommPort(m_nCurSelID);
			int dwPassWord = 0;
			string lpszSerialNumber = string.Empty;
			if (!axFP_CLOCK.SetIPAddress(ref ip, port, dwPassWord))
			{
				return lpszSerialNumber;
			}
			if (axFP_CLOCK.OpenCommPort(m_nCurSelID))
			{
				DisableDevice();
				axFP_CLOCK.GetSerialNumber(m_nCurSelID, ref lpszSerialNumber);
				return lpszSerialNumber;
			}
			return lpszSerialNumber;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return "";
		}
	}
}
