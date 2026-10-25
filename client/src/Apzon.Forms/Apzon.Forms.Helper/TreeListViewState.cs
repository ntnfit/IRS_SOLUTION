using System.Collections;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.Nodes.Operations;

namespace Apzon.Forms.Helper;

public class TreeListViewState
{
	private class OperationSaveExpanded : TreeListOperation
	{
		private ArrayList al = new ArrayList();

		public ArrayList Nodes => al;

		public override void Execute(TreeListNode node)
		{
			if (node.HasChildren && node.Expanded)
			{
				al.Add(node.GetValue(node.TreeList.KeyFieldName));
			}
		}
	}

	private ArrayList expanded;

	private ArrayList selected;

	private object focused;

	private object topNode;

	private TreeList treeList;

	public TreeList TreeList
	{
		get
		{
			return treeList;
		}
		set
		{
			treeList = value;
			Clear();
		}
	}

	public TreeListViewState()
		: this(null)
	{
	}

	public TreeListViewState(TreeList treeList)
	{
		this.treeList = treeList;
		expanded = new ArrayList();
		selected = new ArrayList();
	}

	public void Clear()
	{
		expanded.Clear();
		selected.Clear();
		focused = null;
		topNode = null;
	}

	private ArrayList GetExpanded()
	{
		OperationSaveExpanded operationSaveExpanded = new OperationSaveExpanded();
		TreeList.NodesIterator.DoOperation(operationSaveExpanded);
		return operationSaveExpanded.Nodes;
	}

	private ArrayList GetSelected()
	{
		ArrayList arrayList = new ArrayList();
		foreach (TreeListNode item in TreeList.Selection)
		{
			arrayList.Add(item.GetValue(TreeList.KeyFieldName));
		}
		return arrayList;
	}

	public void LoadState()
	{
		TreeList.BeginUpdate();
		try
		{
			TreeList.CollapseAll();
			foreach (object item in expanded)
			{
				TreeListNode treeListNode = TreeList.FindNodeByKeyID(item);
				if (treeListNode != null)
				{
					treeListNode.Expanded = true;
				}
			}
			TreeList.FocusedNode = TreeList.FindNodeByKeyID(focused);
			foreach (object item2 in selected)
			{
				TreeListNode treeListNode = TreeList.FindNodeByKeyID(item2);
				if (treeListNode != null)
				{
					TreeList.Selection.Add(treeListNode);
				}
			}
		}
		finally
		{
			TreeList.EndUpdate();
			TreeListNode treeListNode2 = TreeList.FindNodeByKeyID(topNode);
			if (treeListNode2 == null)
			{
				treeListNode2 = TreeList.FocusedNode;
			}
			TreeList.TopVisibleNodeIndex = TreeList.GetVisibleIndexByNode(treeListNode2);
		}
	}

	public void SaveState()
	{
		if (TreeList.FocusedNode != null)
		{
			expanded = GetExpanded();
			selected = GetSelected();
			focused = TreeList.FocusedNode[TreeList.KeyFieldName];
			topNode = TreeList.GetNodeByVisibleIndex(TreeList.TopVisibleNodeIndex)[TreeList.KeyFieldName];
		}
		else
		{
			Clear();
		}
	}
}
