using DevExpress.XtraEditors;
using DevExpress.XtraGrid;

namespace Apzon.Controls.Tree;

public class GridItemExpressionFormatRule
{
	private GridFormatRule gridFormatRule;

	public FormatConditionRuleExpression Rule => gridFormatRule.Rule as FormatConditionRuleExpression;

	public bool IsExpressionCondition => Rule != null;

	public GridFormatRule GridFormatRule => gridFormatRule;

	public GridItemExpressionFormatRule(GridFormatRule gridFormatRule)
	{
		this.gridFormatRule = gridFormatRule;
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
