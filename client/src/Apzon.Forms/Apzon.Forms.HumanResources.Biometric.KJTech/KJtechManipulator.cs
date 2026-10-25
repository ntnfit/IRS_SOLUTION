using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Entities.Models.HumanResources.Biometric;
using Apzon.Forms.Commons;
using Apzon.HttpClientHelper;
using AxSFC3KPCLib;

namespace Apzon.Forms.HumanResources.Biometric.KJTech;

internal class KJtechManipulator
{
	public AxSFC3KPC SFC3KPC1;

	public KJtechManipulator()
	{
		try
		{
			KJtechSDK kJtechSDK = new KJtechSDK();
			SFC3KPC1 = (AxSFC3KPC)kJtechSDK.Controls["SFC3KPC1"];
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public string TestConnect(int machineId, string ip, int port, int password = 0)
	{
		if (SFC3KPC1.ConnectTcpip(machineId, ip, port, password))
		{
			SFC3KPC1.Disconnect(machineId);
			return "";
		}
		return GetErrorString();
	}

	public string Connect(int machineId, string ip, int port, int password = 0)
	{
		if (SFC3KPC1.ConnectTcpip(machineId, ip, port, password))
		{
			return "";
		}
		return GetErrorString();
	}

	public void Disconnect(int machineId)
	{
		SFC3KPC1.Disconnect(machineId);
	}

	public string GetErrorString()
	{
		int dwErrorCode = 0;
		SFC3KPC1.GetLastError(ref dwErrorCode);
		return dwErrorCode switch
		{
			0 => "No Error", 
			1 => "Can 't open com port", 
			2 => "Can 't set com port", 
			3 => "Error in creating socket", 
			4 => "Error in setting socket option", 
			5 => "Error in connecting", 
			6 => "Error in reconnecting", 
			7 => "The Password (or the machine ID) is incorrect", 
			8 => "Error in allocating memory in socket dll", 
			101 => "Can 't send data to device", 
			102 => "Can 't read data from device", 
			103 => "Error in parameter", 
			104 => "Invalid Data", 
			105 => "The scope of data is incorrect", 
			501 => "Can't operate the device properly (or none data)", 
			502 => "All data have been read", 
			503 => "Double access to FP(or CARD) data", 
			504 => "Error in allocating memory in device", 
			505 => "Invalid ID", 
			506 => "All fingerprints are enrolled at the terminal", 
			507 => "Fingerprint is already enrolled for this position", 
			508 => "Invalid step number", 
			509 => "Fingerprint duplicated while enrolling image", 
			510 => "An error occurred while enrolling Nth image", 
			511 => "Could not load Fingerprint image file", 
			512 => "An error occurred while enrolling fingerprint image", 
			513 => "Cannot open background bitmap", 
			514 => "Invalid background bitmap", 
			515 => "Bitmap dimension is invalid", 
			516 => "Bitmap color depth is incorrect. It should be 24bit", 
			517 => "This bitmap is compressed format. It should be an uncompressed bitmap", 
			_ => "Unknown", 
		};
	}

	public async Task<DataTable> GetAllUserTable(int machineNumber, string ip, int port)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BiometricDevice");
			return await helper.GetAsync("GetAllUserTable", new { machineNumber, ip, port });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	public DataTable GetAllUserTable_OLD2(int machineNumber)
	{
		DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OTEM");
		Logging.Write("WATCH", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "StartReadGeneralLogData...");
		SFC3KPC1.StartReadGeneralLogData(machineNumber);
		Logging.Write("WATCH", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), GetErrorString());
		Logging.Write("WATCH", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "ReadGeneralLogData...");
		SFC3KPC1.ReadGeneralLogData(machineNumber);
		Logging.Write("WATCH", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), GetErrorString());
		Logging.Write("WATCH", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetGeneralLogData...");
		int num = 1;
		List<GLogData> list = new List<GLogData>();
		while (true)
		{
			GLogData gLogData = new GLogData();
			if (!SFC3KPC1.GetGeneralLogData(machineNumber, ref gLogData.vEnrollNumber, ref gLogData.vGranted, ref gLogData.vMethod, ref gLogData.vDoorMode, ref gLogData.vFunNumber, ref gLogData.vSensor, ref gLogData.vYear, ref gLogData.vMonth, ref gLogData.vDay, ref gLogData.vHour, ref gLogData.vMinute, ref gLogData.vSecond))
			{
				break;
			}
			gLogData.no = num;
			list.Add(gLogData);
			num++;
		}
		Logging.Write("WATCH", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Read GLogData OK");
		foreach (GLogData item in (from m in (from m in list
				group m by m.ID into m
				select m.First()).Distinct()
			where m.Result.Equals("Granted") && m.Method.Equals("by FP")
			select m).ToList())
		{
			DataRow dataRow = tableStructure.NewRow();
			dataRow["MachineNo"] = machineNumber;
			dataRow["EnrollNo"] = item.ID;
			dataRow["EnrollName"] = "";
			tableStructure.Rows.Add(dataRow);
		}
		return tableStructure;
	}

	public DataTable GetAllUserTable_OLD(int machineNumber)
	{
		DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OTEM");
		_ = string.Empty;
		_ = string.Empty;
		_ = string.Empty;
		_ = string.Empty;
		int dwEnrollNumber = 0;
		int dwEnrollData = 0;
		if (SFC3KPC1.ReadAllUserID(machineNumber))
		{
			while (SFC3KPC1.GetAllUserID(machineNumber, ref dwEnrollNumber, ref dwEnrollData))
			{
				DataRow dataRow = tableStructure.NewRow();
				dataRow["MachineNo"] = machineNumber;
				dataRow["EnrollNo"] = dwEnrollNumber;
				dataRow["EnrollName"] = "";
				tableStructure.Rows.Add(dataRow);
			}
		}
		return tableStructure;
	}

	public DataTable GetLogDataTable_OLD(int machineNumber, string fromTime, string toTime, DateTime fromDate, DateTime toDate)
	{
		DataTable tableStructure = WinFuntions.GetTableStructure("APZ_TBD1");
		tableStructure.Columns.Add("EnrollName", typeof(string));
		Logging.Write("WATCH", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "StartReadGeneralLogData...");
		SFC3KPC1.StartReadGeneralLogData(machineNumber);
		Logging.Write("WATCH", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), GetErrorString());
		Logging.Write("WATCH", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "ReadGeneralLogData...");
		SFC3KPC1.ReadGeneralLogData(machineNumber);
		Logging.Write("WATCH", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), GetErrorString());
		Logging.Write("WATCH", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetGeneralLogData...");
		while (true)
		{
			GLogData gLogData = new GLogData();
			if (!SFC3KPC1.GetGeneralLogData(machineNumber, ref gLogData.vEnrollNumber, ref gLogData.vGranted, ref gLogData.vMethod, ref gLogData.vDoorMode, ref gLogData.vFunNumber, ref gLogData.vSensor, ref gLogData.vYear, ref gLogData.vMonth, ref gLogData.vDay, ref gLogData.vHour, ref gLogData.vMinute, ref gLogData.vSecond))
			{
				break;
			}
			DateTime dateTime = Function.ParseDateTimes(gLogData.Time);
			if (fromDate.Date <= dateTime.Date && dateTime.Date <= toDate.Date && gLogData.Result.Equals("Granted"))
			{
				string value = "";
				DataRow dataRow = tableStructure.NewRow();
				dataRow["MachineNo"] = machineNumber;
				dataRow["EnrollNo"] = int.Parse(gLogData.ID);
				dataRow["EnrollName"] = value;
				dataRow["DateTimeRecord"] = dateTime.ToString();
				dataRow["Source"] = gLogData.vDoorMode;
				dataRow["ClockDate"] = dateTime.Date;
				tableStructure.Rows.Add(dataRow);
			}
		}
		Logging.Write("WATCH", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Read GLogData OK");
		return tableStructure;
	}

	public async Task<DataTable> GetLogDataTable(int machineNumber, string ip, int port, DateTime fromDate, DateTime toDate)
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add(new DataColumn("machineNumber", typeof(string)));
			dataTable.Columns.Add(new DataColumn("ip", typeof(string)));
			dataTable.Columns.Add(new DataColumn("port", typeof(int)));
			dataTable.Columns.Add(new DataColumn("fromdate", typeof(DateTime)));
			dataTable.Columns.Add(new DataColumn("todate", typeof(DateTime)));
			DataRow dataRow = dataTable.NewRow();
			dataTable.Rows.Add(dataRow);
			dataRow["machineNumber"] = machineNumber;
			dataRow["ip"] = ip;
			dataRow["port"] = port;
			dataRow["fromdate"] = fromDate;
			dataRow["todate"] = toDate;
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BiometricDevice");
			return await helper.PostAsync("GetLogDataTable", dataTable);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}
}
