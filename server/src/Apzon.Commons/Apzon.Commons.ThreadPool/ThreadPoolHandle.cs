using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Apzon.Commons.ThreadPool;

public class ThreadPoolHandle
{
	private enum TaskState
	{
		Notstarted,
		Processing,
		Completed,
		Aborted
	}

	private class TaskHandle
	{
		public ClientHandle Token;

		public UserTask Task;

		public Action<TaskStatus> Callback;
	}

	private class TaskItem
	{
		public TaskHandle TaskHandle;

		public Thread Handler;

		public TaskState TaskState;

		public DateTime StartTime = DateTime.MaxValue;
	}

	private const int MAX = 8;

	private const int MIN = 3;

	private const int MIN_WAIT = 10;

	private const int MAX_WAIT = 15000;

	private const int CLEANUP_INTERVAL = 60000;

	private const int SCHEDULING_INTERVAL = 10;

	private static readonly ThreadPoolHandle _instance = new ThreadPoolHandle();

	private Queue<TaskHandle> _readyQueue;

	private List<TaskItem> _pool;

	private Thread _taskScheduler;

	private readonly object _syncLock = new object();

	private readonly object _criticalLock = new object();

	public static ThreadPoolHandle Instance => _instance;

	public ThreadPoolHandle()
	{
		InitializeThreadPool();
	}

	private void InitializeThreadPool()
	{
		_readyQueue = new Queue<TaskHandle>();
		_pool = new List<TaskItem>();
		InitPoolWithMinCapacity();
		DateTime lastCleanup = DateTime.Now;
		_taskScheduler = new Thread(() =>
		{
			while (true)
			{
				lock (_syncLock)
				{
					while (_readyQueue.Count > 0 && _readyQueue.Peek().Task == null)
					{
						_readyQueue.Dequeue();
					}
					int count = _readyQueue.Count;
					for (int i = 0; i < count; i++)
					{
						TaskHandle taskHandle = _readyQueue.Peek();
						bool flag = false;
						lock (_criticalLock)
						{
							foreach (TaskItem item in _pool)
							{
								lock (item)
								{
									if (item.TaskState == TaskState.Completed)
									{
										item.TaskHandle = taskHandle;
										item.TaskState = TaskState.Notstarted;
										flag = true;
										_readyQueue.Dequeue();
										break;
									}
								}
							}
							if (!flag && _pool.Count < 8)
							{
								TaskItem taskItem = new TaskItem
								{
									TaskState = TaskState.Notstarted
								};
								taskItem.TaskHandle = taskHandle;
								AddTaskToPool(taskItem);
								flag = true;
								_readyQueue.Dequeue();
							}
						}
						if (!flag)
						{
							break;
						}
					}
				}
				if (DateTime.Now - lastCleanup > TimeSpan.FromMilliseconds(60000.0))
				{
					CleanupPool();
					lastCleanup = DateTime.Now;
				}
				else
				{
					Thread.Yield();
					Thread.Sleep(10);
				}
			}
		});
		_taskScheduler.Priority = ThreadPriority.AboveNormal;
		_taskScheduler.Start();
	}

	private void InitPoolWithMinCapacity()
	{
		for (int i = 0; i <= 3; i++)
		{
			TaskItem taskItem = new TaskItem
			{
				TaskState = TaskState.Notstarted,
				TaskHandle = new TaskHandle
				{
					Task = () =>
					{
					},
					Callback = (TaskStatus taskStatus) =>
					{
					},
					Token = new ClientHandle
					{
						Id = Guid.NewGuid()
					}
				}
			};
			AddTaskToPool(taskItem);
		}
	}

	private void AddTaskToPool(TaskItem taskItem)
	{
		taskItem.Handler = new Thread(() =>
		{
			while (true)
			{
				bool flag = false;
				lock (taskItem)
				{
					if (taskItem.TaskState == TaskState.Aborted)
					{
						break;
					}
					if (taskItem.TaskState == TaskState.Notstarted)
					{
						taskItem.TaskState = TaskState.Processing;
						taskItem.StartTime = DateTime.Now;
						flag = true;
					}
				}
				if (flag)
				{
					TaskStatus taskStatus = new TaskStatus();
					try
					{
						taskItem.TaskHandle.Task();
						taskStatus.Success = true;
					}
					catch (Exception innerException)
					{
						taskStatus.Success = false;
						taskStatus.InnerException = innerException;
					}
					lock (taskItem)
					{
						if (taskItem.TaskHandle.Callback != null && taskItem.TaskState != TaskState.Aborted)
						{
							try
							{
								taskItem.TaskState = TaskState.Completed;
								taskItem.StartTime = DateTime.MaxValue;
								taskItem.TaskHandle.Callback(taskStatus);
							}
							catch
							{
							}
						}
					}
				}
				Thread.Yield();
				Thread.Sleep(10);
			}
		});
		taskItem.Handler.Start();
		lock (_criticalLock)
		{
			_pool.Add(taskItem);
		}
	}

	private void CleanupPool()
	{
		List<TaskItem> list = null;
		lock (_criticalLock)
		{
			list = _pool.Where((TaskItem ti) => ti.TaskHandle.Token.IsSimpleTask && DateTime.Now - ti.StartTime > TimeSpan.FromMilliseconds(15000.0)).ToList();
		}
		foreach (TaskItem item in list)
		{
			CancelUserTask(item.TaskHandle.Token);
		}
		lock (_criticalLock)
		{
			list = _pool.Where((TaskItem ti) => ti.TaskState == TaskState.Aborted).ToList();
			foreach (TaskItem item2 in list)
			{
				try
				{
					item2.Handler.Abort();
					item2.Handler.Priority = ThreadPriority.Lowest;
					item2.Handler.IsBackground = true;
				}
				catch
				{
				}
				_pool.Remove(item2);
			}
			int num = _pool.Count;
			if (num >= 3)
			{
				list = _pool.Where((TaskItem ti) => ti.TaskState == TaskState.Completed).ToList();
				foreach (TaskItem item3 in list)
				{
					item3.Handler.Priority = ThreadPriority.AboveNormal;
					item3.TaskState = TaskState.Aborted;
					_pool.Remove(item3);
					num--;
					if (num == 3)
					{
						break;
					}
				}
			}
			while (_pool.Count < 3)
			{
				TaskItem taskItem = new TaskItem
				{
					TaskState = TaskState.Notstarted
				};
				taskItem.TaskHandle = new TaskHandle
				{
					Task = () =>
					{
					}
				};
				taskItem.TaskHandle.Token = new ClientHandle
				{
					Id = Guid.NewGuid()
				};
				taskItem.TaskHandle.Callback = (TaskStatus taskStatus) =>
				{
				};
				AddTaskToPool(taskItem);
			}
		}
	}

	public ClientHandle QueueUserTask(UserTask task, Action<TaskStatus> callback)
	{
		TaskHandle taskHandle = new TaskHandle
		{
			Task = task,
			Token = new ClientHandle
			{
				Id = Guid.NewGuid()
			},
			Callback = callback
		};
		lock (_syncLock)
		{
			_readyQueue.Enqueue(taskHandle);
		}
		return taskHandle.Token;
	}

	public static void CancelUserTask(ClientHandle clientToken)
	{
		lock (Instance._syncLock)
		{
			TaskHandle taskHandle = Instance._readyQueue.FirstOrDefault((TaskHandle th) => th.Token.Id == clientToken.Id);
			if (taskHandle != null)
			{
				taskHandle.Task = null;
				taskHandle.Callback = null;
				taskHandle.Token = null;
				return;
			}
			_ = Instance._readyQueue.Count;
			TaskItem taskItem = null;
			lock (Instance._criticalLock)
			{
				taskItem = Instance._pool.FirstOrDefault((TaskItem task) => task.TaskHandle.Token.Id == clientToken.Id);
			}
			if (taskItem == null)
			{
				return;
			}
			lock (taskItem)
			{
				if (taskItem.TaskState != TaskState.Completed)
				{
					taskItem.TaskState = TaskState.Aborted;
					taskItem.TaskHandle.Callback = null;
				}
				if (taskItem.TaskState == TaskState.Aborted)
				{
					try
					{
						taskItem.Handler.Abort();
						taskItem.Handler.Priority = ThreadPriority.BelowNormal;
						taskItem.Handler.IsBackground = true;
						return;
					}
					catch
					{
						return;
					}
				}
			}
		}
	}
}
