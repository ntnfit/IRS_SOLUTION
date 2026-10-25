using DevExpress.XtraTreeList.Columns;

namespace Apzon.Controls.Tree;

public class TreeListColumnCollectionEx : TreeListColumnCollection
{
	public TreeListColumnCollectionEx(TreeEx owner)
		: base(owner)
	{
	}

	public TreeListColumnEx AddEx()
	{
		TreeListColumnEx treeListColumnEx = new TreeListColumnEx();
		Add(treeListColumnEx);
		return treeListColumnEx;
	}

	protected override TreeListColumn CreateColumn()
	{
		return new TreeListColumnEx();
	}
}
