using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Apzon.Commons;
using Apzon.Forms.Commons;
using Apzon.Forms.HumanResources.Biometric.Info;

namespace Apzon.Forms.HumanResources.Biometric;

internal class DeviceManipulator
{
	public ZkemClient objZkeeper;

	public ICollection<UserInfo> GetAllUserInfo(ZkemClient objZkeeper, int machineNumber)
	{
		string dwEnrollNumber = string.Empty;
		string Name = string.Empty;
		string Password = string.Empty;
		string TmpData = string.Empty;
		int Privilege = 0;
		int TmpLength = 0;
		int Flag = 0;
		bool Enabled = false;
		ICollection<UserInfo> collection = new List<UserInfo>();
		objZkeeper.ReadAllUserID(machineNumber);
		objZkeeper.ReadAllTemplate(machineNumber);
		while (objZkeeper.SSR_GetAllUserInfo(machineNumber, out dwEnrollNumber, out Name, out Password, out Privilege, out Enabled))
		{
			for (int i = 0; i < 10; i++)
			{
				if (objZkeeper.GetUserTmpExStr(machineNumber, dwEnrollNumber, i, out Flag, out TmpData, out TmpLength))
				{
					UserInfo userInfo = new UserInfo();
					userInfo.MachineNumber = machineNumber;
					userInfo.EnrollNumber = dwEnrollNumber;
					userInfo.Name = Name;
					userInfo.FingerIndex = i;
					userInfo.TmpData = TmpData;
					userInfo.Privelage = Privilege;
					userInfo.Password = Password;
					userInfo.Enabled = Enabled;
					userInfo.iFlag = Flag.ToString();
					collection.Add(userInfo);
				}
			}
		}
		return collection;
	}

	public DataTable GetAllUserTable(ZkemClient objZkeeper, int machineNumber)
	{
		DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OTEM");
		string dwEnrollNumber = string.Empty;
		string Name = string.Empty;
		string Password = string.Empty;
		_ = string.Empty;
		int Privilege = 0;
		bool Enabled = false;
		objZkeeper.ReadAllUserID(machineNumber);
		while (objZkeeper.SSR_GetAllUserInfo(machineNumber, out dwEnrollNumber, out Name, out Password, out Privilege, out Enabled))
		{
			DataRow dataRow = tableStructure.NewRow();
			dataRow["MachineNo"] = machineNumber;
			dataRow["EnrollNo"] = dwEnrollNumber;
			dataRow["EnrollName"] = Name;
			tableStructure.Rows.Add(dataRow);
		}
		return tableStructure;
	}

	public ICollection<MachineInfo> GetLogData(ZkemClient objZkeeper, int machineNumber)
	{
		string dwEnrollNumber = "";
		int dwVerifyMode = 0;
		int dwInOutMode = 0;
		int dwYear = 0;
		int dwMonth = 0;
		int dwDay = 0;
		int dwHour = 0;
		int dwMinute = 0;
		int dwSecond = 0;
		int dwWorkCode = 0;
		ICollection<MachineInfo> collection = new List<MachineInfo>();
		objZkeeper.ReadAllGLogData(machineNumber);
		while (objZkeeper.SSR_GetGeneralLogData(machineNumber, out dwEnrollNumber, out dwVerifyMode, out dwInOutMode, out dwYear, out dwMonth, out dwDay, out dwHour, out dwMinute, out dwSecond, ref dwWorkCode))
		{
			string dateTimeRecord = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
			MachineInfo machineInfo = new MachineInfo();
			machineInfo.MachineNumber = machineNumber;
			machineInfo.IndRegID = int.Parse(dwEnrollNumber);
			machineInfo.DateTimeRecord = dateTimeRecord;
			machineInfo.VerifyMode = Function.ToString(dwVerifyMode);
			machineInfo.InOutMode = Function.ToString(dwInOutMode);
			collection.Add(machineInfo);
		}
		return collection;
	}

	public DataTable GetLogDataTable(ZkemClient objZkeeper, int machineNumber, string fromTime, string toTime, DateTime fromDate, DateTime toDate)
	{
		string dwEnrollNumber1 = "";
		int dwVerifyMode = 0;
		int dwInOutMode = 0;
		int dwYear = 0;
		int dwMonth = 0;
		int dwDay = 0;
		int dwHour = 0;
		int dwMinute = 0;
		int dwSecond = 0;
		int dwWorkCode = 0;
		DataTable tableStructure = WinFuntions.GetTableStructure("APZ_TBD1");
		tableStructure.Columns.Add("EnrollName", typeof(string));
		DataTable allUserTable = GetAllUserTable(objZkeeper, machineNumber);
		int dwErrorCode = 0;
		string errorMsg = "";
		bool flag = objZkeeper.ReadTimeGLogData(machineNumber, fromTime, toTime);
		if (!flag)
		{
			objZkeeper.GetLastError(ref dwErrorCode, ref errorMsg);
			Logging.Write("WATCH", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "ReadTimeGLogData Error Code: " + dwErrorCode, errorMsg);
			flag = objZkeeper.ReadAllGLogData(machineNumber);
		}
		if (!flag)
		{
			objZkeeper.GetLastError(ref dwErrorCode, ref errorMsg);
			Logging.Write("WATCH", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "ReadAllGLogData Error Code: " + dwErrorCode, errorMsg);
		}
		while (objZkeeper.SSR_GetGeneralLogData(machineNumber, out dwEnrollNumber1, out dwVerifyMode, out dwInOutMode, out dwYear, out dwMonth, out dwDay, out dwHour, out dwMinute, out dwSecond, ref dwWorkCode))
		{
			DateTime dateTime = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond);
			if (fromDate.Date <= dateTime.Date && dateTime.Date <= toDate.Date)
			{
				string value = "";
				DataRow dataRow = allUserTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["EnrollNo"]) == Function.ParseInt(dwEnrollNumber1));
				if (dataRow != null)
				{
					value = Function.ToString(dataRow["EnrollName"]);
				}
				DataRow dataRow2 = tableStructure.NewRow();
				dataRow2["MachineNo"] = machineNumber;
				dataRow2["EnrollNo"] = int.Parse(dwEnrollNumber1);
				dataRow2["EnrollName"] = value;
				dataRow2["DateTimeRecord"] = dateTime.ToString();
				dataRow2["Source"] = dwVerifyMode.ToString();
				dataRow2["OriginType"] = dwInOutMode.ToString();
				dataRow2["ClockDate"] = dateTime.Date;
				dataRow2["WorkCode"] = dwWorkCode;
				tableStructure.Rows.Add(dataRow2);
			}
		}
		return tableStructure;
	}

	public ICollection<UserIDInfo> GetAllUserID(ZkemClient objZkeeper, int machineNumber)
	{
		int dwEnrollNumber = 0;
		int dwEMachineNumber = 0;
		int dwBackupNumber = 0;
		int dwMachinePrivilege = 0;
		int dwEnable = 0;
		ICollection<UserIDInfo> collection = new List<UserIDInfo>();
		while (objZkeeper.GetAllUserID(machineNumber, ref dwEnrollNumber, ref dwEMachineNumber, ref dwBackupNumber, ref dwMachinePrivilege, ref dwEnable))
		{
			UserIDInfo userIDInfo = new UserIDInfo();
			userIDInfo.BackUpNumber = dwBackupNumber;
			userIDInfo.Enabled = dwEnable;
			userIDInfo.EnrollNumber = dwEnrollNumber;
			userIDInfo.MachineNumber = dwEMachineNumber;
			userIDInfo.Privelage = dwMachinePrivilege;
			collection.Add(userIDInfo);
		}
		return collection;
	}

	public void GetGeneratLog(ZkemClient objZkeeper, int machineNumber, string enrollNo)
	{
		string Name = null;
		string Password = null;
		int Privilege = 0;
		bool Enabled = false;
		byte[] array = new byte[2000];
		int TmpLength = 0;
		int dwFingerIndex = 0;
		int Flag = 0;
		objZkeeper.ReadAllTemplate(machineNumber);
		while (objZkeeper.SSR_GetUserInfo(machineNumber, enrollNo, out Name, out Password, out Privilege, out Enabled) && !objZkeeper.GetUserTmpEx(machineNumber, enrollNo, dwFingerIndex, out Flag, out array[0], out TmpLength))
		{
		}
	}

	public bool PushUserDataToDevice(ZkemClient objZkeeper, int machineNumber, string enrollNo)
	{
		string empty = string.Empty;
		string empty2 = string.Empty;
		int privilege = 1;
		return objZkeeper.SSR_SetUserInfo(machineNumber, enrollNo, empty, empty2, privilege, Enabled: true);
	}

	public bool UploadFTPTemplate(ZkemClient objZkeeper, int machineNumber, List<UserInfo> lstUserInfo)
	{
		string empty = string.Empty;
		string empty2 = string.Empty;
		string empty3 = string.Empty;
		int num = 0;
		int num2 = 0;
		int num3 = 1;
		int updateFlag = 1;
		string text = "";
		bool flag = false;
		if (objZkeeper.BeginBatchUpdate(machineNumber, updateFlag))
		{
			string text2 = "";
			for (int i = 0; i < lstUserInfo.Count; i++)
			{
				empty = lstUserInfo[i].EnrollNumber;
				empty2 = lstUserInfo[i].Name;
				num = lstUserInfo[i].FingerIndex;
				empty3 = lstUserInfo[i].TmpData;
				num2 = lstUserInfo[i].Privelage;
				text = lstUserInfo[i].Password;
				lstUserInfo[i].Enabled.ToString();
				num3 = Convert.ToInt32(lstUserInfo[i].iFlag);
				flag = true;
				if (empty != text2)
				{
					if (!objZkeeper.SSR_SetUserInfo(machineNumber, empty, empty2, text, num2, flag))
					{
						return false;
					}
					objZkeeper.SetUserTmpExStr(machineNumber, empty, num, num3, empty3);
				}
				else
				{
					objZkeeper.SetUserTmpExStr(machineNumber, empty, num, num3, empty3);
				}
				text2 = empty;
			}
			return true;
		}
		return false;
	}

	public bool ClearGLog(ZkemClient objZkeeper, int machineNumber)
	{
		return objZkeeper.ClearGLog(machineNumber);
	}

	public string FetchDeviceInfo(ZkemClient objZkeeper, int machineNumber)
	{
		StringBuilder stringBuilder = new StringBuilder();
		string strVersion = string.Empty;
		objZkeeper.GetFirmwareVersion(machineNumber, ref strVersion);
		if (strVersion.Trim() != string.Empty)
		{
			stringBuilder.Append("Firmware V: ");
			stringBuilder.Append(strVersion);
			stringBuilder.Append(",");
		}
		strVersion = string.Empty;
		objZkeeper.GetVendor(ref strVersion);
		if (strVersion.Trim() != string.Empty)
		{
			stringBuilder.Append("Vendor: ");
			stringBuilder.Append(strVersion);
			stringBuilder.Append(",");
		}
		string sWiegandFmt = string.Empty;
		objZkeeper.GetWiegandFmt(machineNumber, ref sWiegandFmt);
		strVersion = string.Empty;
		objZkeeper.GetSDKVersion(ref strVersion);
		if (strVersion.Trim() != string.Empty)
		{
			stringBuilder.Append("SDK V: ");
			stringBuilder.Append(strVersion);
			stringBuilder.Append(",");
		}
		strVersion = string.Empty;
		objZkeeper.GetSerialNumber(machineNumber, out strVersion);
		if (strVersion.Trim() != string.Empty)
		{
			stringBuilder.Append("Serial No: ");
			stringBuilder.Append(strVersion);
			stringBuilder.Append(",");
		}
		strVersion = string.Empty;
		objZkeeper.GetDeviceMAC(machineNumber, ref strVersion);
		if (strVersion.Trim() != string.Empty)
		{
			stringBuilder.Append("Device MAC: ");
			stringBuilder.Append(strVersion);
		}
		return stringBuilder.ToString();
	}
}
