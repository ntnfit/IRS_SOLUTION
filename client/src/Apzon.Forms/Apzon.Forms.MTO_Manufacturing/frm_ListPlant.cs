using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Production_Addon;
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.MTO_Manufacturing;

public class frm_ListPlant : XtraFormEx
{
	private bool _edit;

	private IContainer components;

	private GridControlEx grdDocs;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem duplicateToolStripMenuItem;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private GridViewEx gvDocs;

	private GridColumnEx clCode;

	private GridColumnEx clName;

	private GridColumnEx clRemark;

	private Panel panel1;

	private SimpleButtonEx btRefresh;

	private SimpleButtonEx btCancel;

	private SimpleButtonEx btSave;

	private RepositoryItemMemoEdit repositoryItemMemoEdit1;

	private RepositoryItemMemoExEdit repositoryItemMemoExEdit1;

	private void gvDocs_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			if (e.RowHandle == gvDocs.FocusedRowHandle)
			{
				e.Appearance.BackColor = StyleColor.GridEditting;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvDocs_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			_edit = true;
			if (e.Column.FieldName == "Code")
			{
				if (!string.IsNullOrEmpty(Function.ToString(gvDocs.GetRowCellValue(gvDocs.FocusedRowHandle, "Code"))))
				{
					AddNewRowDocs();
				}
				else
				{
					DeleteRowEndEmpty();
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_ListPlant_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (_edit)
			{
				if (XtraMessageBox.Show(this, "Are you sure?", "Do you still want ... ?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
				{
					e.Cancel = true;
				}
				_edit = false;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public frm_ListPlant()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_ListPlant"))
		{
			InitializeComponent();
			SettingLanguage();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_ListPlant" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btCancel.Enabled = true;
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private void SettingLanguage()
	{
		Text = Resources.Titel_frmListPlant;
		btSave.Text = Resources.ButtonSave;
		btCancel.Text = Resources.ButtonCancel;
		btRefresh.Text = Resources.ButtonRefresh;
		clCode.Caption = Resources.lb_Code;
		clName.Caption = Resources.lb_Name;
		clRemark.Caption = Resources.Doc_gvDoc_Remark;
	}

	private void DeleteRowEndEmpty()
	{
		try
		{
			DataTable dataTable = grdDocs.DataSource as DataTable;
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				dataTable.Rows.RemoveAt(dataTable.Rows.Count - 1);
				dataTable.AcceptChanges();
			}
			grdDocs.DataSource = dataTable;
			grdDocs.RefreshDataSource();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddNewRowDocs()
	{
		try
		{
			DataTable dataTable = grdDocs.DataSource as DataTable;
			if (dataTable == null || dataTable.Count() < 1)
			{
				if (!dataTable.Columns.Contains("Code"))
				{
					dataTable.Columns.Add("Code", typeof(string));
				}
				if (!dataTable.Columns.Contains("Name"))
				{
					dataTable.Columns.Add("Name", typeof(string));
				}
				if (!dataTable.Columns.Contains("Remark"))
				{
					dataTable.Columns.Add("Remark", typeof(string));
				}
				DataRow dataRow = dataTable.NewRow();
				dataRow["Code"] = "";
				dataRow["Name"] = "";
				dataRow["Remark"] = "";
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				grdDocs.DataSource = dataTable;
				grdDocs.RefreshDataSource();
			}
			else if (!string.IsNullOrEmpty(Function.ToString(gvDocs.GetRowCellValue(gvDocs.RowCount - 1, "Code"))))
			{
				DataRow dataRow2 = dataTable.NewRow();
				dataRow2["Code"] = "";
				dataRow2["Name"] = "";
				dataRow2["Remark"] = "";
				dataTable.Rows.Add(dataRow2);
				dataTable.AcceptChanges();
				grdDocs.DataSource = dataTable;
				grdDocs.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void LoadDocs()
	{
		try
		{
			DataTable dataSource = await AsyncGetDocs();
			grdDocs.DataSource = dataSource;
			grdDocs.RefreshDataSource();
			AddNewRowDocs();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> AsyncGetDocs()
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ListPlant");
			DataTable dataTable = await _helper.GetDataTableAsync("GetDocs");
			if (dataTable == null)
			{
				return new DataTable();
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async Task<string> AwaitSave(DataTable dtFather)
	{
		try
		{
			using HttpClientHelper<ListPlantImpl> _helper = new HttpClientHelper<ListPlantImpl>(Configuration.ApiServer, "ListPlant");
			return await _helper.PostParamsAsJsonAsync("SaveDocs", dtFather);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return null;
		}
	}

	private async void DeleteRow(string ids)
	{
		try
		{
			using HttpClientHelper<GroupControlKeyImpl> _helper = new HttpClientHelper<GroupControlKeyImpl>(Configuration.ApiServer, "ListPlant");
			string text = await _helper.PostParamsAsJsonAsync("DeleteDocs", ids);
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				XtraMessageBox.Show(Resources.Msg_DeleteSuccess);
				LoadDocs();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
				XtraMessageBox.Show(Resources.Msg_DeleteError);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void Save()
	{
	}

	private void DuplicateDocs()
	{
		try
		{
			DataTable dataTable = grdDocs.DataSource as DataTable;
			DataRow dataRow = dataTable.NewRow();
			dataRow["Code"] = "";
			dataRow["Name"] = gvDocs.GetRowCellValue(gvDocs.FocusedRowHandle, "Name");
			dataRow["Remark"] = gvDocs.GetRowCellValue(gvDocs.FocusedRowHandle, "Remark");
			dataTable.Rows.InsertAt(dataRow, gvDocs.RowCount - 1);
			dataTable.AcceptChanges();
			grdDocs.DataSource = dataTable;
			grdDocs.RefreshDataSource();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_ListPlant_Load(object sender, EventArgs e)
	{
		try
		{
			LoadDocs();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void addNewRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			AddNewRowDocs();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DuplicateDocs();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (!string.IsNullOrEmpty(Function.ToString(gvDocs.GetFocusedRowCellValue("Code"))))
			{
				DataTable dataTable = grdDocs.DataSource as DataTable;
				dataTable.Rows.RemoveAt(gvDocs.FocusedRowHandle);
				dataTable.AcceptChanges();
				grdDocs.DataSource = dataTable;
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.RowIsEmptyCannotDelete, "");
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btSave_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = grdDocs.DataSource as DataTable;
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				if (string.IsNullOrEmpty(Function.ToString(dataTable.Rows[i]["Code"])))
				{
					dataTable.Rows.RemoveAt(i);
					i--;
				}
			}
			WinCommons._OpenProcessing();
			string text = await AwaitSave(dataTable);
			WinCommons._CloseProcessing();
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				LoadDocs();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		try
		{
			Close();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btRefresh_Click(object sender, EventArgs e)
	{
		try
		{
			LoadDocs();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvDocs_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.MTO_Manufacturing.frm_ListPlant));
		this.grdDocs = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.duplicateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvDocs = new Apzon.Controls.Grid.GridViewEx();
		this.clCode = new Apzon.Controls.Grid.GridColumnEx();
		this.clName = new Apzon.Controls.Grid.GridColumnEx();
		this.clRemark = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoExEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.repositoryItemMemoEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
		this.panel1 = new System.Windows.Forms.Panel();
		this.btRefresh = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btSave = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grdDocs).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvDocs).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit1).BeginInit();
		this.panel1.SuspendLayout();
		base.SuspendLayout();
		this.grdDocs.AllowPasteExcel = true;
		this.grdDocs.ClipboardData = null;
		this.grdDocs.ContextMenuStrip = this.contextMenuStrip1;
		this.grdDocs.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdDocs.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grdDocs.Location = new System.Drawing.Point(0, 0);
		this.grdDocs.MainView = this.gvDocs;
		this.grdDocs.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grdDocs.Name = "grdDocs";
		this.grdDocs.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.repositoryItemMemoEdit1, this.repositoryItemMemoExEdit1 });
		this.grdDocs.Size = new System.Drawing.Size(558, 311);
		this.grdDocs.TabIndex = 56;
		this.grdDocs.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDocs });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.duplicateToolStripMenuItem, this.deleteRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(134, 48);
		this.duplicateToolStripMenuItem.Name = "duplicateToolStripMenuItem";
		this.duplicateToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.duplicateToolStripMenuItem.Text = "Duplicate";
		this.duplicateToolStripMenuItem.Click += new System.EventHandler(this.duplicateToolStripMenuItem_Click);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.gvDocs.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDocs.ColumnKey = null;
		this.gvDocs.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.clCode, this.clName, this.clRemark });
		this.gvDocs.DisableFormSetting = false;
		this.gvDocs.DisplayException = false;
		this.gvDocs.EditColumn = null;
		this.gvDocs.FieldCurrency = null;
		this.gvDocs.FieldRate = null;
		this.gvDocs.FormId = null;
		this.gvDocs.GridControl = this.grdDocs;
		this.gvDocs.IgnoreFilter = false;
		this.gvDocs.IndexEditRow = -1;
		this.gvDocs.IndicatorWidth = 40;
		this.gvDocs.IsApzTable = false;
		this.gvDocs.IsInsertNewRow = false;
		this.gvDocs.IsLoadingData = false;
		this.gvDocs.IsMappingContextAction = false;
		this.gvDocs.ItemId = null;
		this.gvDocs.LstColumnAllowBatch = null;
		this.gvDocs.LstColumnNotAllowBatch = null;
		this.gvDocs.LstColumnNotEditable = null;
		this.gvDocs.LstColumNotSetting = null;
		this.gvDocs.Name = "gvDocs";
		this.gvDocs.OptionsView.ColumnAutoWidth = false;
		this.gvDocs.OptionsView.ShowGroupPanel = false;
		this.gvDocs.UseBatchMod = false;
		this.gvDocs.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvDocs_CustomDrawRowIndicator);
		this.gvDocs.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gvDocs_RowStyle);
		this.gvDocs.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDocs_CellValueChanged);
		this.clCode.Caption = "Code";
		this.clCode.CurrencyDateField = null;
		this.clCode.CurrencyField = null;
		this.clCode.CurrencyFixed = null;
		this.clCode.CurrencyFixedRate = 0.0;
		this.clCode.CurrencyRateField = null;
		this.clCode.FieldName = "Code";
		this.clCode.Name = "clCode";
		this.clCode.PrimaryKey = true;
		this.clCode.UnitField = null;
		this.clCode.Visible = true;
		this.clCode.VisibleIndex = 0;
		this.clCode.Width = 184;
		this.clName.Caption = "Name";
		this.clName.CurrencyDateField = null;
		this.clName.CurrencyField = null;
		this.clName.CurrencyFixed = null;
		this.clName.CurrencyFixedRate = 0.0;
		this.clName.CurrencyRateField = null;
		this.clName.FieldName = "Name";
		this.clName.Name = "clName";
		this.clName.PrimaryKey = false;
		this.clName.UnitField = null;
		this.clName.Visible = true;
		this.clName.VisibleIndex = 1;
		this.clName.Width = 199;
		this.clRemark.Caption = "Remark";
		this.clRemark.ColumnEdit = this.repositoryItemMemoExEdit1;
		this.clRemark.CurrencyDateField = null;
		this.clRemark.CurrencyField = null;
		this.clRemark.CurrencyFixed = null;
		this.clRemark.CurrencyFixedRate = 0.0;
		this.clRemark.CurrencyRateField = null;
		this.clRemark.FieldName = "Remark";
		this.clRemark.Name = "clRemark";
		this.clRemark.PrimaryKey = false;
		this.clRemark.UnitField = null;
		this.clRemark.Visible = true;
		this.clRemark.VisibleIndex = 2;
		this.clRemark.Width = 177;
		this.repositoryItemMemoExEdit1.AutoHeight = false;
		this.repositoryItemMemoExEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit1.Name = "repositoryItemMemoExEdit1";
		this.repositoryItemMemoExEdit1.ShowIcon = false;
		this.repositoryItemMemoEdit1.Name = "repositoryItemMemoEdit1";
		this.panel1.Controls.Add(this.btRefresh);
		this.panel1.Controls.Add(this.btCancel);
		this.panel1.Controls.Add(this.btSave);
		this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panel1.Location = new System.Drawing.Point(0, 311);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(558, 33);
		this.panel1.TabIndex = 55;
		this.panel1.Tag = "ss";
		this.btRefresh.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btRefresh.EditControl = null;
		this.btRefresh.EditValue = "Refresh";
		this.btRefresh.FieldName = null;
		this.btRefresh.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btRefresh.ImageOptions.Image");
		this.btRefresh.IsSearchControl = false;
		this.btRefresh.Location = new System.Drawing.Point(216, 8);
		this.btRefresh.Name = "btRefresh";
		this.btRefresh.ReadOnly = false;
		this.btRefresh.SettingInVisible = false;
		this.btRefresh.SettingReadonly = false;
		this.btRefresh.Size = new System.Drawing.Size(99, 19);
		this.btRefresh.TabIndex = 44;
		this.btRefresh.Text = "Refresh";
		this.btRefresh.Click += new System.EventHandler(this.btRefresh_Click);
		this.btCancel.Appearance.Font = new System.Drawing.Font("Tahoma", 7.5f);
		this.btCancel.Appearance.Options.UseFont = true;
		this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(112, 8);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(99, 19);
		this.btCancel.TabIndex = 14;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.btSave.Appearance.Font = new System.Drawing.Font("Tahoma", 7.5f);
		this.btSave.Appearance.Options.UseFont = true;
		this.btSave.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btSave.EditControl = null;
		this.btSave.EditValue = "Save";
		this.btSave.FieldName = null;
		this.btSave.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btSave.ImageOptions.Image");
		this.btSave.IsSearchControl = false;
		this.btSave.Location = new System.Drawing.Point(9, 8);
		this.btSave.Name = "btSave";
		this.btSave.ReadOnly = false;
		this.btSave.SettingInVisible = false;
		this.btSave.SettingReadonly = false;
		this.btSave.Size = new System.Drawing.Size(99, 19);
		this.btSave.TabIndex = 13;
		this.btSave.Text = "Save";
		this.btSave.Click += new System.EventHandler(this.btSave_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(558, 344);
		base.Controls.Add(this.grdDocs);
		base.Controls.Add(this.panel1);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ListPlant";
		this.Text = "List Plant";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_ListPlant_FormClosing);
		base.Load += new System.EventHandler(this.frm_ListPlant_Load);
		((System.ComponentModel.ISupportInitialize)this.grdDocs).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvDocs).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit1).EndInit();
		this.panel1.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
