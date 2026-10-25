using System;
using System.Collections.Generic;

namespace Apzon.Forms.Commons;

internal abstract class ProcessTracker : IObservable<string>
{
	private class TrackingContext : IDisposable
	{
		private IProcess process;

		private ProcessTracker tracker;

		public TrackingContext(IProcess process, ProcessTracker tracker)
		{
			process.Start += tracker.process_Start;
			process.Running += tracker.process_Running;
			process.Complete += tracker.process_Complete;
			this.process = process;
			this.tracker = tracker;
		}

		void IDisposable.Dispose()
		{
			process.Start -= tracker.process_Start;
			process.Running -= tracker.process_Running;
			process.Complete -= tracker.process_Complete;
			tracker = null;
			process = null;
		}
	}

	private class Subscription : IDisposable
	{
		private readonly IObserver<string> observer;

		private readonly ProcessTracker tracker;

		public Subscription(ProcessTracker tracker, IObserver<string> observer)
		{
			if (!tracker.observers.Contains(observer))
			{
				tracker.observers.Add(observer);
			}
			this.tracker = tracker;
			this.observer = observer;
		}

		void IDisposable.Dispose()
		{
			tracker.observers.Remove(observer);
		}
	}

	private readonly IList<IObserver<string>> observers;

	protected ProcessTracker()
	{
		observers = new List<IObserver<string>>();
	}

	IDisposable IObservable<string>.Subscribe(IObserver<string> observer)
	{
		return new Subscription(this, observer);
	}

	public IDisposable StartTracking(IProcess process)
	{
		return new TrackingContext(process, this);
	}

	private void process_Start(object sender, ProcessStatusEventArgs e)
	{
		foreach (IObserver<string> observer in observers)
		{
			observer.OnNext(e.Status);
		}
	}

	private void process_Running(object sender, ProcessStatusEventArgs e)
	{
		foreach (IObserver<string> observer in observers)
		{
			observer.OnNext(e.Status);
		}
	}

	private void process_Complete(object sender, EventArgs e)
	{
		foreach (IObserver<string> observer in observers)
		{
			observer.OnCompleted();
		}
	}
}
