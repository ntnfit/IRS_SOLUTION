using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using Microsoft.AspNet.SignalR.Client;

namespace Apzon.Forms.Helper.HubHelpers;

public class HubClientHelper : IDisposable
{
	private readonly dynamic _father;

	public readonly HubConnection HubConnection;

	private HubClientHelper()
	{
		throw new Exception("HubClient do not use default constructor");
	}

	public HubClientHelper(dynamic fatherForm)
	{
		if (!(fatherForm is Form))
		{
			throw new Exception("Father of hubClient must be a Form");
		}
		_father = fatherForm;
		HubConnection = new HubConnection($"{Configuration.ApiServer}SignalR", useDefaultUrl: false);
		HubConnection.Headers.Add("sessionId", GlobalData.SessionId);
	}

	public async Task CreateSocket(string hubName, Dictionary<string, string> method)
	{
		if (HubConnection.State != ConnectionState.Disconnected)
		{
			HubConnection.Stop();
		}
		foreach (string key in method.Keys)
		{
			IHubProxy proxy = HubConnection.CreateHubProxy(hubName);
			dynamic type = _father.GetType();
			dynamic theMethod = type.GetMethod(method[key]);
			proxy.On(key, () =>
			{
				try
				{
					if (_father == null || string.IsNullOrEmpty(_father.Text))
					{
						Dispose();
					}
					else
					{
						theMethod.Invoke(_father, new object[0]);
					}
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
				}
			});
		}
		if (HubConnection.State == ConnectionState.Disconnected)
		{
			await HubConnection.Start();
		}
	}

	public async Task CreateSocket<T>(string hubName, Dictionary<string, string> method)
	{
		if (HubConnection.State != ConnectionState.Disconnected)
		{
			HubConnection.Stop();
		}
		foreach (string key in method.Keys)
		{
			IHubProxy proxy = HubConnection.CreateHubProxy(hubName);
			dynamic type = _father.GetType();
			dynamic theMethod = type.GetMethod(method[key]);
			proxy.On(key, (T t) =>
			{
				try
				{
					if (_father == null || string.IsNullOrEmpty(_father.Text))
					{
						Dispose();
					}
					else
					{
						theMethod.Invoke(_father, new object[1] { t });
					}
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
				}
			});
		}
		if (HubConnection.State == ConnectionState.Disconnected)
		{
			await HubConnection.Start();
		}
	}

	public async Task CreateSocket<T1, T2>(string hubName, Dictionary<string, string> method)
	{
		if (HubConnection.State != ConnectionState.Disconnected)
		{
			HubConnection.Stop();
		}
		foreach (string key in method.Keys)
		{
			IHubProxy proxy = HubConnection.CreateHubProxy(hubName);
			dynamic type = _father.GetType();
			dynamic theMethod = type.GetMethod(method[key]);
			proxy.On(key, (T1 t1, T2 t2) =>
			{
				try
				{
					if (_father == null || string.IsNullOrEmpty(_father.Text))
					{
						Dispose();
					}
					else
					{
						theMethod.Invoke(_father, new object[2] { t1, t2 });
					}
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
				}
			});
		}
		if (HubConnection.State == ConnectionState.Disconnected)
		{
			await HubConnection.Start();
		}
	}

	public async Task CreateSocket<T1, T2, T3>(string hubName, Dictionary<string, string> method)
	{
		if (HubConnection.State != ConnectionState.Disconnected)
		{
			HubConnection.Stop();
		}
		foreach (string key in method.Keys)
		{
			IHubProxy proxy = HubConnection.CreateHubProxy(hubName);
			dynamic type = _father.GetType();
			dynamic theMethod = type.GetMethod(method[key]);
			proxy.On(key, (T1 t1, T2 t2, T3 t3) =>
			{
				try
				{
					if (_father == null || string.IsNullOrEmpty(_father.Text))
					{
						Dispose();
					}
					else
					{
						theMethod.Invoke(_father, new object[3] { t1, t2, t3 });
					}
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
				}
			});
		}
		if (HubConnection.State == ConnectionState.Disconnected)
		{
			await HubConnection.Start();
		}
	}

	public async Task CreateSocket<T1, T2, T3, T4>(string hubName, Dictionary<string, string> method)
	{
		if (HubConnection.State != ConnectionState.Disconnected)
		{
			HubConnection.Stop();
		}
		foreach (string key in method.Keys)
		{
			IHubProxy proxy = HubConnection.CreateHubProxy(hubName);
			dynamic type = _father.GetType();
			dynamic theMethod = type.GetMethod(method[key]);
			proxy.On(key, (T1 t1, T2 t2, T3 t3, T4 t4) =>
			{
				try
				{
					if (_father == null || string.IsNullOrEmpty(_father.Text))
					{
						Dispose();
					}
					else
					{
						theMethod.Invoke(_father, new object[4] { t1, t2, t3, t4 });
					}
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
				}
			});
		}
		if (HubConnection.State == ConnectionState.Disconnected)
		{
			await HubConnection.Start();
		}
	}

	public async Task CreateSocket<T1, T2, T3, T4, T5>(string hubName, Dictionary<string, string> method)
	{
		if (HubConnection.State != ConnectionState.Disconnected)
		{
			HubConnection.Stop();
		}
		foreach (string key in method.Keys)
		{
			IHubProxy proxy = HubConnection.CreateHubProxy(hubName);
			dynamic type = _father.GetType();
			dynamic theMethod = type.GetMethod(method[key]);
			proxy.On(key, (T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) =>
			{
				try
				{
					if (_father == null || string.IsNullOrEmpty(_father.Text))
					{
						Dispose();
					}
					else
					{
						theMethod.Invoke(_father, new object[5] { t1, t2, t3, t4, t5 });
					}
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
				}
			});
		}
		if (HubConnection.State == ConnectionState.Disconnected)
		{
			await HubConnection.Start();
		}
	}

	public async Task CreateSocket<T1, T2, T3, T4, T5, T6>(string hubName, Dictionary<string, string> method)
	{
		if (HubConnection.State != ConnectionState.Disconnected)
		{
			HubConnection.Stop();
		}
		foreach (string key in method.Keys)
		{
			IHubProxy proxy = HubConnection.CreateHubProxy(hubName);
			dynamic type = _father.GetType();
			dynamic theMethod = type.GetMethod(method[key]);
			proxy.On(key, (T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6) =>
			{
				try
				{
					if (_father == null || string.IsNullOrEmpty(_father.Text))
					{
						Dispose();
					}
					else
					{
						theMethod.Invoke(_father, new object[6] { t1, t2, t3, t4, t5, t6 });
					}
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
				}
			});
		}
		if (HubConnection.State == ConnectionState.Disconnected)
		{
			await HubConnection.Start();
		}
	}

	public async Task CreateSocket<T1, T2, T3, T4, T5, T6, T7>(string hubName, Dictionary<string, string> method)
	{
		if (HubConnection.State != ConnectionState.Disconnected)
		{
			HubConnection.Stop();
		}
		foreach (string key in method.Keys)
		{
			IHubProxy proxy = HubConnection.CreateHubProxy(hubName);
			dynamic type = _father.GetType();
			dynamic theMethod = type.GetMethod(method[key]);
			proxy.On(key, (T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7) =>
			{
				try
				{
					if (_father == null || string.IsNullOrEmpty(_father.Text))
					{
						Dispose();
					}
					else
					{
						theMethod.Invoke(_father, new object[7] { t1, t2, t3, t4, t5, t6, t7 });
					}
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
				}
			});
		}
		if (HubConnection.State == ConnectionState.Disconnected)
		{
			await HubConnection.Start();
		}
	}

	public void Dispose()
	{
		HubConnection.Dispose();
	}
}
