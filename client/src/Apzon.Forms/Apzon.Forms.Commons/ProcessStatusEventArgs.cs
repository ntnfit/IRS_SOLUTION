using System;

namespace Apzon.Forms.Commons;

public class ProcessStatusEventArgs : EventArgs
{
	public string Status { get; private set; }

	public ProcessStatusEventArgs(string status)
	{
		Status = status;
	}
}
