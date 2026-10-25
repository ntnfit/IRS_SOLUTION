using DevExpress.XtraEditors;
using DevExpress.XtraPivotGrid;

namespace Apzon.Controls.Tree;

public class PivotItemExpressionFormatRule
{
	private PivotGridFormatRule pivotFormatRule;

	public bool IsExpressionCondition => Rule != null;

	public FormatConditionRuleExpression Rule => pivotFormatRule.Rule as FormatConditionRuleExpression;

	public PivotGridFormatRule PivotFormatRule => pivotFormatRule;

	public PivotItemExpressionFormatRule(PivotGridFormatRule pivotFormatRule)
	{
		this.pivotFormatRule = pivotFormatRule;
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
