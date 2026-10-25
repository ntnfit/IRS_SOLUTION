using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls.Grid;
using Apzon.Controls.Pivot;
using Apzon.Controls.Tree;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Controls.Form;

public class frm_FormatRule_Copy : XtraFormEx
{
	private IContainer components;

	private GridControlEx grd_Copy;

	private GridViewEx gv_Copy;

	private GridColumn gcUserCode;

	private GridColumn gcName;

	private GridColumn gcDepartment;

	private DropDownButton btnSave;

	private SimpleButton bt_Cancel;

	public Tuple<TreeEx, string> Item { get; set; }

	public Tuple<GridViewEx, string> ItemGrid { get; set; }

	public Tuple<PivotGridControlEx, string> ItemPivot { get; set; }

	public new string FormId { get; set; }

	public string UserName { get; set; }

	public frm_FormatRule_Copy()
	{
		InitializeComponent();
	}

	private void btnSave_Click(object sender, EventArgs e)
	{
		try
		{
			ItemBtnSave_Click(sender, e);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bt_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void frm_FormatRule_Copy_Load(object sender, EventArgs e)
	{
		try
		{
			await LoadData();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadData()
	{
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Auth");
			DataTable dataSource = await hp.GetAsync("GetUserForCopy", new
			{
				user = UserName
			});
			grd_Copy.DataSource = dataSource;
			grd_Copy.RefreshDataSource();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void ItemBtnSaveAll_Click(object sender, EventArgs e)
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmApplyAllFormatConditionForUser, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				await CopyFormatRule(copyall: true);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void ItemBtnSave_Click(object sender, EventArgs e)
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmApplyAllFormatConditionForUser, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				await CopyFormatRule(copyall: false);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task CopyFormatRule(bool copyall)
	{
		try
		{
			WinCommons._OpenProcessing();
			List<string> list = new List<string>();
			int[] selectedRows = gv_Copy.GetSelectedRows();
			if (selectedRows.Length != 0)
			{
				DataTable dataTable = new DataTable();
				dataTable.Columns.Add("ItemId", typeof(string));
				dataTable.Columns.Add("FormId", typeof(string));
				dataTable.Columns.Add("UserName", typeof(string));
				dataTable.Columns.Add("CopyUser", typeof(string));
				dataTable.Columns.Add("CopyAll", typeof(string));
				list.AddRange(from t in selectedRows
					select Function.ToString(gv_Copy.GetRowCellValue(t, "USER_CODE")) into user
					where !string.IsNullOrWhiteSpace(user)
					select user);
				DataRow dataRow = dataTable.NewRow();
				if (Item != null)
				{
					dataRow["ItemId"] = Item.Item2;
				}
				else if (ItemGrid != null)
				{
					dataRow["ItemId"] = ItemGrid.Item2;
				}
				else if (ItemPivot != null)
				{
					dataRow["ItemId"] = ItemPivot.Item2;
				}
				dataRow["FormId"] = FormId;
				dataRow["UserName"] = UserName;
				dataRow["CopyUser"] = string.Join(",", list.ToArray());
				dataRow["CopyAll"] = (copyall ? "Y" : "N");
				dataTable.Rows.Add(dataRow);
				using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "FormatRule");
				string text = await hp.PostAsJsonAsync("CopyFormatRule", dataTable);
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Success(Enums.MessageCode.CopyFormatRuleSuccess, "");
					WinCommons._CloseProcessing();
					Close();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Controls.Form.frm_FormatRule_Copy));
		this.grd_Copy = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Copy = new Apzon.Controls.Grid.GridViewEx();
		this.gcUserCode = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gcName = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gcDepartment = new DevExpress.XtraGrid.Columns.GridColumn();
		this.btnSave = new DevExpress.XtraEditors.DropDownButton();
		this.bt_Cancel = new DevExpress.XtraEditors.SimpleButton();
		((System.ComponentModel.ISupportInitialize)this.grd_Copy).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Copy).BeginInit();
		base.SuspendLayout();
		this.grd_Copy.AllowPasteExcel = false;
		this.grd_Copy.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Copy.ClipboardData = null;
		this.grd_Copy.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
		this.grd_Copy.Location = new System.Drawing.Point(1, 1);
		this.grd_Copy.MainView = this.gv_Copy;
		this.grd_Copy.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
		this.grd_Copy.Name = "grd_Copy";
		this.grd_Copy.Size = new System.Drawing.Size(532, 465);
		this.grd_Copy.TabIndex = 9;
		this.grd_Copy.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Copy });
		this.gv_Copy.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Copy.ColumnKey = null;
		this.gv_Copy.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gcUserCode, this.gcName, this.gcDepartment });
		this.gv_Copy.DetailHeight = 431;
		this.gv_Copy.DisableFormSetting = true;
		this.gv_Copy.DisplayException = false;
		this.gv_Copy.FieldCurrency = null;
		this.gv_Copy.FieldRate = null;
		this.gv_Copy.FormId = "oFormatRuleCopy";
		this.gv_Copy.GridControl = this.grd_Copy;
		this.gv_Copy.IndicatorWidth = 41;
		this.gv_Copy.IsApzTable = false;
		this.gv_Copy.IsInsertNewRow = false;
		this.gv_Copy.IsLoadingData = false;
		this.gv_Copy.ItemId = "OUSR";
		this.gv_Copy.LstColumnAllowBatch = null;
		this.gv_Copy.LstColumnNotAllowBatch = null;
		this.gv_Copy.LstColumnNotEditable = null;
		this.gv_Copy.LstColumNotSetting = null;
		this.gv_Copy.Name = "gv_Copy";
		this.gv_Copy.OptionsSelection.MultiSelect = true;
		this.gv_Copy.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_Copy.OptionsView.ShowDetailButtons = false;
		this.gv_Copy.OptionsView.ShowGroupPanel = false;
		this.gv_Copy.UseBatchMod = false;
		this.gcUserCode.Caption = "User Code";
		this.gcUserCode.FieldName = "USER_CODE";
		this.gcUserCode.MinWidth = 23;
		this.gcUserCode.Name = "gcUserCode";
		this.gcUserCode.OptionsColumn.AllowEdit = false;
		this.gcUserCode.OptionsColumn.ReadOnly = true;
		this.gcUserCode.Visible = true;
		this.gcUserCode.VisibleIndex = 1;
		this.gcUserCode.Width = 87;
		this.gcName.Caption = "Name";
		this.gcName.FieldName = "Rate";
		this.gcName.MinWidth = 23;
		this.gcName.Name = "gcName";
		this.gcName.OptionsColumn.AllowEdit = false;
		this.gcName.OptionsColumn.ReadOnly = true;
		this.gcName.Visible = true;
		this.gcName.VisibleIndex = 3;
		this.gcName.Width = 87;
		this.gcDepartment.Caption = "Department";
		this.gcDepartment.FieldName = "Department";
		this.gcDepartment.MinWidth = 23;
		this.gcDepartment.Name = "gcDepartment";
		this.gcDepartment.OptionsColumn.AllowEdit = false;
		this.gcDepartment.OptionsColumn.ReadOnly = true;
		this.gcDepartment.Visible = true;
		this.gcDepartment.VisibleIndex = 2;
		this.gcDepartment.Width = 87;
		this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnSave.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSave.ImageOptions.Image");
		this.btnSave.Location = new System.Drawing.Point(1, 474);
		this.btnSave.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
		this.btnSave.Name = "btnSave";
		this.btnSave.Size = new System.Drawing.Size(146, 25);
		this.btnSave.TabIndex = 12;
		this.btnSave.Text = "Apply This Form";
		this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
		this.bt_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.bt_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bt_Cancel.ImageOptions.Image");
		this.bt_Cancel.Location = new System.Drawing.Point(154, 474);
		this.bt_Cancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
		this.bt_Cancel.Name = "bt_Cancel";
		this.bt_Cancel.Size = new System.Drawing.Size(102, 25);
		this.bt_Cancel.TabIndex = 13;
		this.bt_Cancel.Text = "Cancel";
		this.bt_Cancel.Click += new System.EventHandler(this.bt_Cancel_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(7f, 16f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(534, 502);
		base.Controls.Add(this.bt_Cancel);
		base.Controls.Add(this.btnSave);
		base.Controls.Add(this.grd_Copy);
		base.DisableContextAction = true;
		base.DisableLoadScript = true;
		this.FormId = "oFormatRuleCopy";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_FormatRule_Copy.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
		base.Name = "frm_FormatRule_Copy";
		this.Text = "Format Rule Copy";
		base.Load += new System.EventHandler(this.frm_FormatRule_Copy_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_Copy).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Copy).EndInit();
		base.ResumeLayout(false);
	}
}
