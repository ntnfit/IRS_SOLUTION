using System;
using System.Collections.Generic;
using Apzon.Commons;
using Apzon.Commons.Enums;

namespace Apzon.Entities.Models.Common;

public class WorkflowProcess<T> : IWorkflowProcess where T : class
{
	public Type TypeOfClass => typeof(T);

	public T Content { get; set; }

	public ApzObjectTypes ObjectType { get; set; }

	public Enums.ContextActionActionTypeEx ActionType { get; set; }

	public Enums.ContextActionMethod Method { get; set; }

	public List<IWorkflowProcess> SubProcess { get; set; }

	public List<IWorkflowProcess> SubProcessGroup { get; set; }
}
