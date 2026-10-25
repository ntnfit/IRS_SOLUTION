using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Forms.Properties;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Commons.Queries;

public class frm_ManagerCategories : XtraFormEx
{
	public RefreshQueryManager RefreshManager;

	private DataSet dtset;

	private List<LabelLine> category = new List<LabelLine>();

	private List<LinkLabel> queryName = new List<LinkLabel>();

	private int showquery;

	private bool _isLoading;

	private bool _flag;

	private IContainer components;

	private LabelLine lbNewCategory;

	private TextEditEx txtQeryCalegory;

	private SimpleButtonEx btnAdd;

	private SimpleButtonEx btnCancel;

	private GridControlEx grd_Content;

	private GridViewEx gv_Content;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumn3;

	private GridColumnEx gridColumn4;

	private GridColumnEx gridColumn5;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Add:
			btnAdd.Text = Apzon.Commons.Properties.Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btnAdd.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btnAdd.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_ManagerCategories()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_ManagerQuery"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_ManagerQuery" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btnAdd.Enabled = false;
			}
			else
			{
				btnAdd.Enabled = true;
			}
			return;
		}
		throw new Exception(Apzon.Commons.Properties.Resources.Msg_FormAccessDenied);
	}

	private async void btnAdd_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			if (string.IsNullOrEmpty(txtQeryCalegory.Text))
			{
				WinCommons.Error(Enums.MessageCode.NoEntryCatalogName, "");
				return;
			}
			_isLoading = true;
			await Save();
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void frm_ManagerCategories_Load(object sender, EventArgs e)
	{
		try
		{
			_isLoading = true;
			await Reload();
			REFRESH();
			gv_Content.FocusedRowHandle = -1;
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LbOnClick(object sender, EventArgs e)
	{
		int num = 1;
		int num2 = category[showquery].Location.Y;
		if (queryName.Count > 0)
		{
			for (int i = showquery + 1; i < category.Count; i++)
			{
				category[i].Location = new Point(20, num2 + num * 25);
				num++;
			}
		}
		for (int num3 = queryName.Count - 1; num3 >= 0; num3--)
		{
			queryName.Remove(queryName[num3]);
		}
		txtQeryCalegory.Text = ((LabelLine)sender).Utext;
		num = 1;
		num2 = ((LabelLine)sender).Location.Y;
		int num4 = -1;
		for (int j = 0; j < dtset.Tables[0].Rows.Count; j++)
		{
			if (Function.ToString(dtset.Tables[0].Rows[j]["catname"]) == ((LabelLine)sender).Utext)
			{
				num4 = Function.ParseInt(dtset.Tables[0].Rows[j]["Categoryid"]);
			}
		}
		if (dtset.Tables[1] != null && dtset.Tables[1].Rows.Count > 0)
		{
			for (int k = 0; k < dtset.Tables[1].Rows.Count; k++)
			{
				if (Function.ParseInt(dtset.Tables[1].Rows[k]["Qcategory"]) == num4)
				{
					LinkLabel linkLabel = new LinkLabel();
					linkLabel.Name = "lb" + k;
					linkLabel.Location = new Point(30, num2 + num * 25);
					linkLabel.Size = new Size(300, 20);
					linkLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
					linkLabel.TabIndex = 0;
					linkLabel.Text = Function.ToString(dtset.Tables[1].Rows[k]["Qname"]);
					num++;
					queryName.Add(linkLabel);
				}
			}
		}
		for (int l = 0; l < category.Count; l++)
		{
			if (category[l].Utext == ((LabelLine)sender).Utext)
			{
				for (int m = l + 1; m < category.Count; m++)
				{
					category[m].Location = new Point(20, num2 + num * 25);
					num++;
				}
				showquery = l;
				break;
			}
		}
	}

	private void gv_Content_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		try
		{
			GetRowData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			int num = Function.ParseInt(gv_Content.GetFocusedRowCellValue("CategoryId"));
			if (num == -2)
			{
				WinCommons.Error(Enums.MessageCode.YouCannotDeleteSystemQueriesCatalog, "");
			}
			else
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteCatalogAndQuery, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					return;
				}
				using HttpClientHelper<string> _help = new HttpClientHelper<string>(Configuration.ApiServer, "Queries");
				string text = await _help.GetAsync("DeleteLine", new
				{
					cateId = num
				});
				if (string.IsNullOrEmpty(text))
				{
					gv_Content.BeginUpdate();
					gv_Content.DeleteSelectedRows();
					gv_Content.EndUpdate();
					WinCommons.Success(Enums.MessageCode.DeleteSuccess, "");
					await Reload();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txtQeryCalegory_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (!_flag && FormMode != Enums.FormViewModeType.Add)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Content_RowClick(object sender, RowClickEventArgs e)
	{
		try
		{
			if (e.RowHandle == 0)
			{
				GetRowData();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task Save()
	{
		_ = 2;
		try
		{
			DataTable tableStructure = WinFuntions.GetTableStructure("OQCN");
			DataRow dataRow = tableStructure.NewRow();
			if (FormMode == Enums.FormViewModeType.Update)
			{
				dataRow["CategoryId"] = gv_Content.GetFocusedRowCellValue("CategoryId");
				if (Function.ParseInt(gv_Content.GetFocusedRowCellValue("CategoryId")) == -2 && !txtQeryCalegory.Text.Equals("System") && !txtQeryCalegory.Text.Contains("(System)"))
				{
					dataRow["CatName"] = txtQeryCalegory.Text + "(System)";
				}
				else
				{
					dataRow["CatName"] = txtQeryCalegory.Text;
				}
				dataRow["UserSign"] = GlobalData.UserID;
			}
			else
			{
				dataRow["CategoryId"] = 0;
				dataRow["CatName"] = txtQeryCalegory.Text;
				dataRow["UserSign"] = GlobalData.UserID;
			}
			tableStructure.Rows.Add(dataRow);
			tableStructure.AcceptChanges();
			using HttpClientHelper<string> http_ = new HttpClientHelper<string>(Configuration.ApiServer, "Commons");
			string text = await http_.PostAsync("SaveQueryCategory", tableStructure);
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				await Reload();
				REFRESH();
				if (RefreshManager != null)
				{
					await RefreshManager();
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task Reload()
	{
		try
		{
			dtset = await GetDataCategories();
			if (dtset.Tables.Count > 0 && dtset.Tables[0].IsNotNull())
			{
				dtset.Tables[0].AcceptChanges();
				grd_Content.DataSource = dtset.Tables[0];
				grd_Content.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataSet> GetDataCategories()
	{
		try
		{
			using HttpClientHelper<DataSet> http_ = new HttpClientHelper<DataSet>(Configuration.ApiServer, "Commons");
			return await http_.GetAsync("GetDataCategories");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
	}

	public async Task REFRESH()
	{
		try
		{
			txtQeryCalegory.EditValue = "";
			FormMode = Enums.FormViewModeType.Add;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GetRowData()
	{
		try
		{
			if (!_isLoading)
			{
				string text = Function.ToString(gv_Content.GetFocusedRowCellValue("CatName"));
				FormMode = Enums.FormViewModeType.Update;
				_flag = true;
				if (Function.ParseInt(gv_Content.GetFocusedRowCellValue("CategoryId")) == -2 && Function.ToString(gv_Content.GetFocusedRowCellValue("CatName")).Contains("(System)"))
				{
					txtQeryCalegory.EditValue = text.Substring(0, text.IndexOf("(System)", 0, StringComparison.Ordinal));
				}
				else
				{
					txtQeryCalegory.EditValue = text;
				}
				_flag = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		this.lbNewCategory = new Apzon.Controls.Controls.LabelLine();
		this.txtQeryCalegory = new Apzon.Controls.Editor.TextEditEx();
		this.btnAdd = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Content = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Content = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		((System.ComponentModel.ISupportInitialize)this.txtQeryCalegory.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Content).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Content).BeginInit();
		base.SuspendLayout();
		this.lbNewCategory.EditControl = null;
		this.lbNewCategory.EditValue = "Category Name:";
		this.lbNewCategory.FieldName = null;
		this.lbNewCategory.HideWhenEmpty = false;
		this.lbNewCategory.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbNewCategory.Location = new System.Drawing.Point(2, 13);
		this.lbNewCategory.Name = "lbNewCategory";
		this.lbNewCategory.ReadOnly = false;
		this.lbNewCategory.SettingInVisible = false;
		this.lbNewCategory.SettingReadonly = false;
		this.lbNewCategory.ShowUnderLine = true;
		this.lbNewCategory.Size = new System.Drawing.Size(115, 20);
		this.lbNewCategory.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbNewCategory.TabIndex = 0;
		this.lbNewCategory.Utext = "Category Name:";
		this.txtQeryCalegory.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.txtQeryCalegory.EditControl = null;
		this.txtQeryCalegory.FieldName = null;
		this.txtQeryCalegory.IsSearchControl = false;
		this.txtQeryCalegory.Location = new System.Drawing.Point(115, 10);
		this.txtQeryCalegory.Name = "txtQeryCalegory";
		this.txtQeryCalegory.SettingInVisible = false;
		this.txtQeryCalegory.SettingReadonly = false;
		this.txtQeryCalegory.Size = new System.Drawing.Size(307, 20);
		this.txtQeryCalegory.TabIndex = 1;
		this.txtQeryCalegory.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.txtQeryCalegory_EditValueChanging);
		this.btnAdd.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnAdd.EditControl = null;
		this.btnAdd.EditValue = "OK";
		this.btnAdd.FieldName = null;
		this.btnAdd.ImageOptions.Image = Apzon.Forms.Properties.Resources.apply_16x16;
		this.btnAdd.IsSearchControl = false;
		this.btnAdd.Location = new System.Drawing.Point(2, 324);
		this.btnAdd.Name = "btnAdd";
		this.btnAdd.ReadOnly = false;
		this.btnAdd.SettingInVisible = false;
		this.btnAdd.SettingReadonly = false;
		this.btnAdd.Size = new System.Drawing.Size(70, 23);
		this.btnAdd.TabIndex = 3;
		this.btnAdd.Text = "OK";
		this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = Apzon.Forms.Properties.Resources.cancel_16x16;
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(75, 324);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(70, 23);
		this.btnCancel.TabIndex = 4;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.grd_Content.AllowPasteExcel = true;
		this.grd_Content.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Content.ClipboardData = null;
		this.grd_Content.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Content.Location = new System.Drawing.Point(2, 41);
		this.grd_Content.MainView = this.gv_Content;
		this.grd_Content.Name = "grd_Content";
		this.grd_Content.Size = new System.Drawing.Size(420, 278);
		this.grd_Content.TabIndex = 5;
		this.grd_Content.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Content });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.deleteRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(134, 26);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.gv_Content.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Content.ColumnKey = null;
		this.gv_Content.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.gridColumn1, this.gridColumn2, this.gridColumn3, this.gridColumn4, this.gridColumn5 });
		this.gv_Content.DisableFormSetting = false;
		this.gv_Content.DisplayException = false;
		this.gv_Content.EditColumn = null;
		this.gv_Content.FieldCurrency = null;
		this.gv_Content.FieldRate = null;
		this.gv_Content.FormId = "oManagerCategories";
		this.gv_Content.GridControl = this.grd_Content;
		this.gv_Content.IgnoreFilter = false;
		this.gv_Content.IndexEditRow = -1;
		this.gv_Content.IndicatorWidth = 35;
		this.gv_Content.IsApzTable = false;
		this.gv_Content.IsInsertNewRow = false;
		this.gv_Content.IsLoadingData = false;
		this.gv_Content.IsMappingContextAction = false;
		this.gv_Content.ItemId = "OCATAGORIES";
		this.gv_Content.LstColumnAllowBatch = null;
		this.gv_Content.LstColumnNotAllowBatch = null;
		this.gv_Content.LstColumnNotEditable = null;
		this.gv_Content.LstColumNotSetting = null;
		this.gv_Content.Name = "gv_Content";
		this.gv_Content.OptionsView.ShowColumnHeaders = false;
		this.gv_Content.OptionsView.ShowGroupPanel = false;
		this.gv_Content.OptionsView.ShowIndicator = false;
		this.gv_Content.UseBatchMod = false;
		this.gv_Content.RowClick += new DevExpress.XtraGrid.Views.Grid.RowClickEventHandler(this.gv_Content_RowClick);
		this.gv_Content.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gv_Content_FocusedRowChanged);
		this.gridColumn1.Caption = "gridColumn1";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "CategoryId";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.AllowEdit = false;
		this.gridColumn1.OptionsColumn.ReadOnly = true;
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.gridColumn2.Caption = "gridColumn2";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "CatName";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.OptionsColumn.AllowEdit = false;
		this.gridColumn2.OptionsColumn.ReadOnly = true;
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 0;
		this.gridColumn3.Caption = "gridColumn3";
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "PermMask";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.OptionsColumn.AllowEdit = false;
		this.gridColumn3.OptionsColumn.ReadOnly = true;
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn4.Caption = "gridColumn4";
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "DataSource";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.OptionsColumn.AllowEdit = false;
		this.gridColumn4.OptionsColumn.ReadOnly = true;
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn5.Caption = "gridColumn5";
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.FieldName = "UserSign";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.OptionsColumn.AllowEdit = false;
		this.gridColumn5.OptionsColumn.ReadOnly = true;
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.UnitField = null;
		base.AcceptButton = this.btnAdd;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(429, 350);
		base.Controls.Add(this.grd_Content);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btnAdd);
		base.Controls.Add(this.txtQeryCalegory);
		base.Controls.Add(this.lbNewCategory);
		base.FormId = "oManagerCategories";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ManagerCategories";
		this.Text = "Manager Categories";
		base.Load += new System.EventHandler(this.frm_ManagerCategories_Load);
		((System.ComponentModel.ISupportInitialize)this.txtQeryCalegory.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Content).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Content).EndInit();
		base.ResumeLayout(false);
	}
}
