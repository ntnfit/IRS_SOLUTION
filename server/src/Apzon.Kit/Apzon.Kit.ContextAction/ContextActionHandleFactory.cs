using System.Data;
using Apzon.Commons.Enums;

namespace Apzon.Kit.ContextAction;

public static class ContextActionHandleFactory
{
	public static IContextActionHandle GetActionHandle()
	{
		return new ContextActionHandle();
	}

	public static IContextActionHandle GetActionHandle(DataTable document, DataTable lines, DataTable batchserial)
	{
		return new ContextActionHandle(document, lines, batchserial);
	}

	public static IContextActionHandle GetActionHandle(Enums.ContextActionActionTypeEx actionType, DataTable dtItems, DataTable dtItemPrices, DataTable dtItemWarehouses)
	{
		return new ContextActionHandle(actionType, dtItems, dtItemPrices, dtItemWarehouses);
	}

	public static IContextActionHandle GetActionHandle(Enums.ContextActionActionTypeEx actionType, DataTable dtHeader)
	{
		return new ContextActionHandle(actionType, dtHeader);
	}

	public static IContextActionHandle GetActionHandle(Enums.ContextActionActionTypeEx actionType, DataTable dtHeader, DataTable dtLine)
	{
		return new ContextActionHandle(actionType, dtHeader, dtLine);
	}
}
