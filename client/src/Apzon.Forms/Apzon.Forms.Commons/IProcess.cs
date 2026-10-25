using System;

namespace Apzon.Forms.Commons;

public interface IProcess
{
	event ProcessStatusEventHandler Start;

	event ProcessStatusEventHandler Running;

	event EventHandler Complete;
}
