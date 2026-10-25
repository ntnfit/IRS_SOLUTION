using System;

namespace Apzon.Forms.Commons;

internal class StartUpProcess : IProcess, IDisposable
{
	private class StartUpProcessTracker : ProcessTracker
	{
		internal static StartUpProcessTracker Instance = new StartUpProcessTracker();
	}

	private static StartUpProcess process;

	private IDisposable tracker;

	private ProcessStatusEventHandler startCore;

	private ProcessStatusEventHandler runningCore;

	private EventHandler completeCore;

	public static IObservable<string> Status => StartUpProcessTracker.Instance;

	event ProcessStatusEventHandler IProcess.Start
	{
		add
		{
			startCore = (ProcessStatusEventHandler)Delegate.Combine(startCore, value);
		}
		remove
		{
			startCore = (ProcessStatusEventHandler)Delegate.Remove(startCore, value);
		}
	}

	event ProcessStatusEventHandler IProcess.Running
	{
		add
		{
			runningCore = (ProcessStatusEventHandler)Delegate.Combine(runningCore, value);
		}
		remove
		{
			runningCore = (ProcessStatusEventHandler)Delegate.Remove(runningCore, value);
		}
	}

	event EventHandler IProcess.Complete
	{
		add
		{
			completeCore = (EventHandler)Delegate.Combine(completeCore, value);
		}
		remove
		{
			completeCore = (EventHandler)Delegate.Remove(completeCore, value);
		}
	}

	public StartUpProcess()
	{
		process = this;
		tracker = StartUpProcessTracker.Instance.StartTracking(this);
	}

	void IDisposable.Dispose()
	{
		tracker.Dispose();
		process = null;
	}

	public static void OnStart(string status)
	{
		if (process != null)
		{
			process.RaiseStart(status);
		}
	}

	public static void OnRunning(string status)
	{
		if (process != null)
		{
			process.RaiseRunning(status);
		}
	}

	public static void OnComplete()
	{
		if (process != null)
		{
			process.RaiseComplete();
		}
	}

	private void RaiseStart(string status)
	{
		if (startCore != null)
		{
			startCore(this, new ProcessStatusEventArgs(status));
		}
	}

	private void RaiseRunning(string status)
	{
		if (runningCore != null)
		{
			runningCore(this, new ProcessStatusEventArgs(status));
		}
	}

	private void RaiseComplete()
	{
		if (completeCore != null)
		{
			completeCore(this, EventArgs.Empty);
		}
	}
}
