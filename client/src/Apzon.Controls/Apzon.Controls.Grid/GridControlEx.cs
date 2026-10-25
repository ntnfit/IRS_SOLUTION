using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls.Form;
using Apzon.Controls.Form.ExchangeRateAndIndex;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Registrator;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Controls.Grid;

[ToolboxItem(true)]
public class GridControlEx : GridControl
{
	public bool AllowPasteExcel { get; set; }

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public DataTable ClipboardData { get; set; }

	public override object DataSource
	{
		get
		{
			return base.DataSource;
		}
		set
		{
			DataTable dtSource = value as DataTable;
			(MainView as GridViewEx)?.InitFormatColumnWhenChangeSource(dtSource);
			base.DataSource = value;
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public DataTable DataSourceTable
	{
		get
		{
			if (DataSource is DataTable { Rows: not null } dataTable)
			{
				return dataTable;
			}
			return new DataTable();
		}
	}

	public GridControlEx()
	{
		if (LicenseManager.UsageMode != LicenseUsageMode.Runtime)
		{
			return;
		}
		Load += GridControlEx_LoadAsync;
		Enter += (object sender, EventArgs e) =>
		{
			if (FindForm() is XtraFormEx { Modal: false } && XtraFormEx.EnableDirectionalControl != null)
			{
				Enums.DirectionalControl[] gridToolbarButtons = XtraFormEx.GridToolbarButtons;
				foreach (Enums.DirectionalControl diectioControl in gridToolbarButtons)
				{
					XtraFormEx.EnableDirectionalControl(diectioControl, isEnable: true);
				}
			}
		};
		Leave += (object sender, EventArgs e) =>
		{
			if (FindForm() is XtraFormEx { Modal: false })
			{
				Enums.DirectionalControl[] gridToolbarButtons = XtraFormEx.GridToolbarButtons;
				foreach (Enums.DirectionalControl diectioControl in gridToolbarButtons)
				{
					XtraFormEx.EnableDirectionalControl(diectioControl, isEnable: false);
				}
			}
		};
	}

	private async void GridControlEx_LoadAsync(object sender, EventArgs e)
	{
		if (!(MainView is GridViewEx gridViewEx))
		{
			return;
		}
		if (!string.IsNullOrEmpty(gridViewEx.FormId) && !string.IsNullOrEmpty(gridViewEx.ItemId) && !gridViewEx.DisableFormSetting)
		{
			Visible = false;
			if (!gridViewEx.NotLoadFormsettingAtLoad && gridViewEx.FirstLoadFormSetting == null)
			{
				gridViewEx.FirstLoadFormSetting = gridViewEx.ReloadFormSetting();
			}
			if (gridViewEx.FirstLoadFormSetting != null)
			{
				await gridViewEx.FirstLoadFormSetting;
			}
			Visible = true;
		}
		else
		{
			gridViewEx.HideColumnsInColumnChooser();
		}
	}

	public override BaseView CreateView(string name)
	{
		name = (name.Equals("GridView") ? "GridViewEx" : (name.Equals("BandedGridView") ? "BandedGridViewEx" : name));
		return base.CreateView(name);
	}

	protected override void RegisterAvailableViewsCore(InfoCollection collection)
	{
		base.RegisterAvailableViewsCore(collection);
		collection.Add(new GridViewInfoRegistratorEx());
		collection.Add(new BandViewInfoRegistratorEx());
		collection.Add(new CustomViewInfoRegistratorEx());
		collection.Add(new AdvBandViewInfoRegistratorEx());
	}

	protected override void RaiseEditorKeyDown(KeyEventArgs e)
	{
		if (AllowPasteExcel)
		{
			if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
			{
				if (FocusedView is GridViewEx gv)
				{
					gv.ExcelToDataSource();
				}
				e.Handled = true;
				return;
			}
			if (!e.Handled)
			{
				base.RaiseEditorKeyDown(e);
			}
		}
		if (e.KeyCode == Keys.Tab && MainView is GridViewEx { FocusedColumn: GridColumnEx { IsChangeValue: not false } focusedColumn } gridViewEx)
		{
			XtraFormEx xtraFormEx = FindForm() as XtraFormEx;
			bool checkValidValueStatus = gridViewEx.GetCheckValidValueStatus(gridViewEx.FocusedRowHandle, focusedColumn.FieldName);
			focusedColumn.IsChangeValue = false;
			string text = Function.ToString(gridViewEx.EditingValue);
			string text2 = Function.ToString(gridViewEx.GetFocusedRowCellValue(focusedColumn));
			if (checkValidValueStatus && !string.IsNullOrEmpty(text) && focusedColumn.CflType != Enums.TypeCFL.None && !HttpClientHelperFactory.GetHttpClient().Commons.CheckExistsByCodeObject(focusedColumn.CflType, text))
			{
				if (xtraFormEx != null)
				{
					xtraFormEx.IsCanClose = false;
				}
				if (focusedColumn.ColumnEdit != null)
				{
					if (focusedColumn.ColumnEdit is RepositoryItemButtonEdit repositoryItemButtonEdit)
					{
						MethodInfo[] methods = repositoryItemButtonEdit.GetType().GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic);
						ButtonPressedEventArgs e2 = new ButtonPressedEventArgs(repositoryItemButtonEdit.Buttons[0]);
						List<int> listIndexButtonPress = methods.GetListIndexButtonPress();
						for (int i = 0; i < listIndexButtonPress.Count; i++)
						{
							methods[listIndexButtonPress[i]].Invoke(repositoryItemButtonEdit, new object[1] { e2 });
						}
					}
					if (text.Equals(Function.ToString(gridViewEx.GetFocusedRowCellValue(focusedColumn))) || (gridViewEx.State == GridState.Editing && (string.IsNullOrEmpty(Function.ToString(gridViewEx.GetFocusedRowCellValue(focusedColumn))) || text2.Equals(Function.ToString(gridViewEx.GetFocusedRowCellValue(focusedColumn))))))
					{
						gridViewEx.Focus();
						gridViewEx.FocusedColumn = focusedColumn;
						gridViewEx.ShowEditor();
						focusedColumn.IsChangeValue = true;
						e.Handled = true;
					}
					return;
				}
			}
			if (!string.IsNullOrEmpty(gridViewEx.FieldCurrency) && focusedColumn.FieldName.Equals(gridViewEx.FieldCurrency) && Math.Abs(Function.ParseDouble(gridViewEx.GetFocusedRowCellValue(gridViewEx.FieldRate))) < 1E-08)
			{
				frm_ExchangeRatesAndIndexs frm_ExchangeRatesAndIndexs2 = new frm_ExchangeRatesAndIndexs();
				frm_ExchangeRatesAndIndexs2.Curr = Function.ToString(gridViewEx.GetFocusedRowCellValue(gridViewEx.FieldCurrency));
				frm_ExchangeRatesAndIndexs2._date = DateTime.Now.Date;
				frm_ExchangeRatesAndIndexs2.ShowDialog();
				if (Math.Abs(frm_ExchangeRatesAndIndexs2.rate) < 1E-07)
				{
					gridViewEx.SetFocusedRowCellValue(gridViewEx.FieldRate, 0);
					Focus();
					gridViewEx.FocusedColumn = focusedColumn;
					gridViewEx.ShowEditor();
					return;
				}
				focusedColumn.IsChangeValue = false;
				gridViewEx.SetFocusedRowCellValue(gridViewEx.FieldRate, frm_ExchangeRatesAndIndexs2.rate);
			}
			if (xtraFormEx != null)
			{
				xtraFormEx.IsCanClose = true;
			}
		}
		base.RaiseEditorKeyDown(e);
	}
}
