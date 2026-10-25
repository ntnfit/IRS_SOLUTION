using DevExpress.XtraEditors;
using DevExpress.XtraTreeList.StyleFormatConditions;

namespace Apzon.Controls.Tree;

public class TreeItemExpressionFormatRule
{
	private TreeListFormatRule treeListFormatRule;

	public FormatConditionRuleExpression Rule => treeListFormatRule.Rule as FormatConditionRuleExpression;

	public bool IsExpressionCondition => Rule != null;

	public TreeListFormatRule TreeListFormatRule => treeListFormatRule;

	public TreeItemExpressionFormatRule(TreeListFormatRule treeListFormatRule)
	{
		this.treeListFormatRule = treeListFormatRule;
	}

	public override string ToString()
	{
		if (!IsExpressionCondition)
		{
			return base.ToString();
		}
		if (string.IsNullOrEmpty(Rule.Expression))
		{
			return "";
		}
		return Rule.Expression;
	}
}
