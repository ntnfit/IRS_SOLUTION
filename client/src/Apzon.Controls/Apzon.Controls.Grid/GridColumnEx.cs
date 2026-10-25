using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Global;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;

namespace Apzon.Controls.Grid;

public class GridColumnEx : GridColumn
{
	private Enums.DisplayFormatType _displayFormatType;

	[DefaultValue(Enums.CurrencyColumn.None)]
	public Enums.CurrencyColumn Currency { get; set; }

	public string CurrencyFixed { get; set; }

	public double CurrencyFixedRate { get; set; }

	public string CurrencyField { get; set; }

	public string CurrencyRateField { get; set; }

	[DefaultValue(Enums.DisplayFormatType.None)]
	public Enums.DisplayFormatType DisplayFormatType
	{
		get
		{
			return _displayFormatType;
		}
		set
		{
			_displayFormatType = value;
			if (GlobalData.GeneralSetting != null)
			{
				switch (value)
				{
				case Enums.DisplayFormatType.DateTime:
					DisplayFormat.FormatType = FormatType.DateTime;
					DisplayFormat.FormatString = GlobalData.DateTimeFormat;
					break;
				case Enums.DisplayFormatType.Percent:
					DisplayFormat.FormatType = FormatType.Numeric;
					DisplayFormat.FormatString = $"#,0.{new string('#', GlobalData.GeneralSetting.PercentDec ?? 6)}\\%";
					break;
				case Enums.DisplayFormatType.Qty:
					DisplayFormat.FormatType = FormatType.Numeric;
					DisplayFormat.FormatString = $"#,0.{new string('#', GlobalData.GeneralSetting.QtyDec ?? 6)}";
					break;
				case Enums.DisplayFormatType.Rate:
					DisplayFormat.FormatType = FormatType.Numeric;
					DisplayFormat.FormatString = $"#,0.{new string('#', GlobalData.GeneralSetting.RateDec ?? 6)}";
					break;
				case Enums.DisplayFormatType.Unit:
					DisplayFormat.FormatType = FormatType.Numeric;
					DisplayFormat.FormatString = $"#,0.{new string('#', GlobalData.GeneralSetting.QtyDec ?? 6)}";
					break;
				case Enums.DisplayFormatType.Price:
					DisplayFormat.FormatType = FormatType.Numeric;
					DisplayFormat.FormatString = $"#,0.{new string('#', GlobalData.GeneralSetting.PriceDec.GetValueOrDefault())}";
					break;
				case Enums.DisplayFormatType.Amount:
					DisplayFormat.FormatType = FormatType.Numeric;
					DisplayFormat.FormatString = $"#,0.{new string('#', GlobalData.GeneralSetting.SumDec.GetValueOrDefault())}";
					break;
				case Enums.DisplayFormatType.Integer:
					DisplayFormat.FormatType = FormatType.Numeric;
					DisplayFormat.FormatString = "n0";
					break;
				case Enums.DisplayFormatType.Time:
					break;
				}
			}
		}
	}

	public string CurrencyDateField { get; set; }

	public bool PrimaryKey { get; set; }

	[DefaultValue(false)]
	public bool IsChangeValue { get; set; }

	[DefaultValue(Enums.TypeCFL.None)]
	public Enums.TypeCFL CflType { get; set; }

	[DefaultValue(true)]
	public bool AllowChSet { get; set; }

	public string UnitField { get; set; }

	protected override void OnOptionsChanged(object sender, BaseOptionChangedEventArgs e)
	{
		try
		{
			if (e == null || !e.Name.Equals("ReadOnly") || ColumnEdit == null || (!ColumnEdit.GetEditorType().Name.Equals("ButtonEditCFL") && !ColumnEdit.GetEditorType().Name.Equals("ButtonEdit")))
			{
				return;
			}
			RepositoryItemButtonEdit repositoryItemButtonEdit = ColumnEdit as RepositoryItemButtonEdit;
			ColumnEdit.ReadOnly = Function.ParseBool(e.NewValue);
			if (repositoryItemButtonEdit != null && repositoryItemButtonEdit.Buttons.Count > 0)
			{
				EditorButton editorButton = repositoryItemButtonEdit.Buttons.FirstOrDefault((EditorButton r) => !r.IsLeft);
				if (editorButton != null)
				{
					editorButton.Visible = !ColumnEdit.ReadOnly;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public GridColumnEx()
	{
		AllowChSet = true;
	}
}
