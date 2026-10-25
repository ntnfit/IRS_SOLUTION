using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Entities.Models.HumanResources.Biometric;
using Newtonsoft.Json;

namespace Apzon.Api.Controllers.HumanResources.TimeSheeting;

public class BiometricDeviceController : BaseApiController
{
	[HttpPost]
	public DataTable GetLogDataTable([FromBody] DataTable dtSearch)
	{
		try
		{
			if (dtSearch == null || dtSearch.Rows.Count == 0)
			{
				return new DataTable();
			}
			string text = Function.ToString(dtSearch.Rows[0]["machineNumber"]);
			string text2 = Function.ToString(dtSearch.Rows[0]["ip"]);
			int num = Function.ParseInt(dtSearch.Rows[0]["port"]);
			DateTime dateTime = Function.ParseDateTimes(dtSearch.Rows[0]["fromDate"]);
			DateTime dateTime2 = Function.ParseDateTimes(dtSearch.Rows[0]["toDate"]);
			DataTable bPTableStructure = UnitOfWork.BusinessMasterData.GetBPTableStructure("APZ_TBD1");
			bPTableStructure.Columns.Add("EnrollName", typeof(string));
			Logging.Write("WATCH", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Start connect to socket server...");
			TcpClient tcpClient = new TcpClient();
			IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(Global.BiometricServer), Global.BiometricPort);
			tcpClient.Connect(remoteEP);
			if (tcpClient.Connected)
			{
				StreamWriter streamWriter = new StreamWriter(tcpClient.GetStream());
				StreamReader streamReader = new StreamReader(tcpClient.GetStream());
				streamWriter.AutoFlush = true;
				Logging.Write("WATCH", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Connected to socket server!");
				streamWriter.WriteLine($"{text}|{text2}|{num}|{dateTime}|{dateTime2}");
				while (tcpClient.Connected)
				{
					string text3 = streamReader.ReadLine();
					if (text3.ToUpper() == "EXIT")
					{
						streamWriter.WriteLine("EXIT");
						break;
					}
					Logging.Write("WATCH", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "recieved data!");
					foreach (GLogData item in JsonConvert.DeserializeObject<List<GLogData>>(text3))
					{
						DateTime dateTime3 = Function.ParseDateTimes(item.Time);
						if (dateTime.Date <= dateTime3.Date && dateTime3.Date <= dateTime2.Date && item.Result.Equals("Granted"))
						{
							string value = "";
							DataRow dataRow = bPTableStructure.NewRow();
							dataRow["MachineNo"] = text;
							dataRow["EnrollNo"] = int.Parse(item.ID);
							dataRow["EnrollName"] = value;
							dataRow["DateTimeRecord"] = dateTime3.ToString();
							dataRow["Source"] = item.vDoorMode;
							dataRow["ClockDate"] = dateTime3.Date;
							bPTableStructure.Rows.Add(dataRow);
						}
					}
				}
				streamWriter.Close();
				streamReader.Close();
				tcpClient.Close();
				Logging.Write("WATCH", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Client disconnect!");
				return bPTableStructure;
			}
			Logging.Write("WATCH", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Client did not connect!");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	[HttpGet]
	public DataTable GetAllUserTable(int machineNumber, string ip, int port)
	{
		try
		{
			DataTable bPTableStructure = UnitOfWork.BusinessMasterData.GetBPTableStructure("APZ_OTEM");
			Logging.Write("WATCH", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Start connect to socket server...");
			TcpClient tcpClient = new TcpClient();
			IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(Global.BiometricServer), Global.BiometricPort);
			tcpClient.Connect(remoteEP);
			if (tcpClient.Connected)
			{
				StreamWriter streamWriter = new StreamWriter(tcpClient.GetStream());
				StreamReader streamReader = new StreamReader(tcpClient.GetStream());
				streamWriter.AutoFlush = true;
				Logging.Write("WATCH", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Connected to socket server!");
				streamWriter.WriteLine($"{machineNumber}|{ip}|{port}");
				while (tcpClient.Connected)
				{
					string text = streamReader.ReadLine();
					if (text.ToUpper() == "EXIT")
					{
						streamWriter.WriteLine("EXIT");
						break;
					}
					Logging.Write("WATCH", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "recieved data!");
					List<GLogData> list = JsonConvert.DeserializeObject<List<GLogData>>(text);
					if (list.Count <= 0)
					{
						continue;
					}
					foreach (GLogData item in (from m in (from m in list
							group m by m.ID into m
							select m.First()).Distinct()
						where m.Result.Equals("Granted") && m.Method.Equals("by FP")
						select m).ToList())
					{
						DataRow dataRow = bPTableStructure.NewRow();
						dataRow["MachineNo"] = machineNumber;
						dataRow["EnrollNo"] = item.ID;
						dataRow["EnrollName"] = "";
						bPTableStructure.Rows.Add(dataRow);
					}
				}
				streamWriter.Close();
				streamReader.Close();
				tcpClient.Close();
				Logging.Write("WATCH", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Client disconnect!");
				return bPTableStructure;
			}
			Logging.Write("WATCH", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Client did not connect!");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}
}
