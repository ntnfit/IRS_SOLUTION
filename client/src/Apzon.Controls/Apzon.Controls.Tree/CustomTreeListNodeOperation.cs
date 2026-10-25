using System.Collections.Generic;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.Nodes.Operations;

namespace Apzon.Controls.Tree;

public class CustomTreeListNodeOperation : TreeListOperation
{
	private List<TreeListNode> _CheckedNodeCollection = new List<TreeListNode>();

	public List<TreeListNode> CheckedNodeCollection
	{
		get
		{
			return _CheckedNodeCollection;
		}
		set
		{
			_CheckedNodeCollection = value;
		}
	}

	public override void Execute(TreeListNode node)
	{
		if (node.Checked)
		{
			CheckedNodeCollection.Add(node);
		}
	}
}
