using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.Nodes.Operations;

namespace Apzon.Forms.Administrator;

public class TreeListChangeChildNodesOperation : TreeListOperation
{
	private TreeListColumn column;

	private string state;

	private TreeListNode parentNode;

	public TreeListChangeChildNodesOperation(TreeListColumn column, TreeListNode node, string state)
	{
		this.column = column;
		this.state = state;
		parentNode = node;
	}

	public override void Execute(TreeListNode node)
	{
		if (node.HasAsParent(parentNode))
		{
			node.SetValue(column, state);
		}
	}

	public override bool NeedsVisitChildren(TreeListNode node)
	{
		return true;
	}
}
