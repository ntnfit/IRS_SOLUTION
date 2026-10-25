using System;
using System.Collections.Generic;

namespace Apzon.Entities.Models.Common;

public class WorkflowProcessGroup : IWorkflowProcess
{
	public List<IWorkflowProcess> Process { get; set; }

	public Type TypeOfClass => typeof(WorkflowProcessGroup);
}
