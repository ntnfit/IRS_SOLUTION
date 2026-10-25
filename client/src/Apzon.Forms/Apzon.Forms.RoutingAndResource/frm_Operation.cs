using System;
using System.Collections.Generic;
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
using Apzon.Entities.Models.Common;
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
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace Apzon.Forms.RoutingAndResource;

public class frm_Operation : XtraFormEx
{
	private bool _edit;

	private int hotTrackRow = int.MinValue;

	private IContainer components;

	private Panel panel1;

	private SimpleButtonEx btRefresh;

	private SimpleButtonEx btCancel;

	private SimpleButtonEx btSave;

	private GridControlEx grdOperation;

	private GridViewEx gvOperation;

	private GridColumnEx clCode;

	private GridColumnEx clName;

	private GridColumnEx clStatus;

	private GridColumnEx clRemark;

	private GridColumnEx clAlternative;

	private ContextMenuStrip ctxDocs;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private ToolStripMenuItem duplicateToolStripMenuItem;

	private RepositoryItemComboBox repStatus;

	private RepositoryItemLookUpEdit repoStatus;

	private RepositoryItemMemoExEdit repositoryItemMemoExEdit1;

	private int HotTrackRow
	{
		get
		{
			return hotTrackRow;
		}
		set
		{
			if (hotTrackRow != value)
			{
				int rowHandle = hotTrackRow;
				hotTrackRow = value;
				if (gvOperation.ActiveEditor != null)
				{
					gvOperation.PostEditor();
				}
				gvOperation.RefreshRow(rowHandle);
				gvOperation.RefreshRow(hotTrackRow);
				if (hotTrackRow >= 0)
				{
					grdOperation.Cursor = Cursors.Hand;
				}
				else
				{
					grdOperation.Cursor = Cursors.Default;
				}
			}
		}
	}

	public frm_Operation()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_Operation"))
		{
			InitializeComponent();
			SettingLanguage();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_Operation" && x.authoriza == "F"))
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
		Text = Resources.Titel_frmOperation;
		btCancel.Text = Resources.ButtonCancel;
		btSave.Text = Resources.ButtonSave;
		btRefresh.Text = Resources.ButtonRefresh;
		clCode.Caption = Resources.lb_Code;
		clName.Caption = Resources.lb_Name;
		clStatus.Caption = Resources.lb_Status;
		clRemark.Caption = Resources.Doc_gvDoc_Remark;
		clAlternative.Caption = Resources.lb_AlternativeOperation;
	}

	private void LoadInIt()
	{
		try
		{
			List<Common.Table_ID> list = new List<Common.Table_ID>();
			list.Add(new Common.Table_ID
			{
				Name = "Active",
				ID = "Y"
			});
			list.Add(new Common.Table_ID
			{
				Name = "Inactive",
				ID = "N"
			});
			repoStatus.DataSource = list;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_Operation_Load(object sender, EventArgs e)
	{
		LoadDocs();
		LoadInIt();
	}

	private async void LoadDocs()
	{
		DataTable dataSource = await GetDocs();
		grdOperation.DataSource = dataSource;
		grdOperation.RefreshDataSource();
		AddNewRowDocs();
	}

	private async Task<DataTable> GetDocs()
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Operation");
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

	private void DeleteRowEndEmpty()
	{
		try
		{
			DataTable dataTable = grdOperation.DataSource as DataTable;
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				dataTable.Rows.RemoveAt(dataTable.Rows.Count - 1);
				dataTable.AcceptChanges();
			}
			grdOperation.DataSource = dataTable;
			grdOperation.RefreshDataSource();
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
			DataTable dataTable = grdOperation.DataSource as DataTable;
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				if (!dataTable.Columns.Contains("Code"))
				{
					dataTable.Columns.Add("Code", typeof(string));
				}
				if (!dataTable.Columns.Contains("Name"))
				{
					dataTable.Columns.Add("Name", typeof(string));
				}
				if (!dataTable.Columns.Contains("Status"))
				{
					dataTable.Columns.Add("Status", typeof(string));
				}
				if (!dataTable.Columns.Contains("Remark"))
				{
					dataTable.Columns.Add("Remark", typeof(string));
				}
				if (!dataTable.Columns.Contains("Alternative"))
				{
					dataTable.Columns.Add("Alternative", typeof(string));
				}
				DataRow dataRow = dataTable.NewRow();
				dataRow["Code"] = "";
				dataRow["Name"] = "";
				dataRow["Status"] = "Y";
				dataRow["Remark"] = "";
				dataRow["Alternative"] = "";
				dataTable.Rows.Add(dataRow);
				grdOperation.DataSource = dataTable;
				grdOperation.RefreshDataSource();
			}
			else if (!string.IsNullOrEmpty(Function.ToString(gvOperation.GetRowCellValue(gvOperation.RowCount - 1, "Code"))))
			{
				DataRow dataRow2 = dataTable.NewRow();
				dataRow2["Code"] = "";
				dataRow2["Name"] = "";
				dataRow2["Status"] = "Y";
				dataRow2["Remark"] = "";
				dataRow2["Alternative"] = "";
				dataTable.Rows.Add(dataRow2);
				grdOperation.DataSource = dataTable;
				grdOperation.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void DuplicateRowDocs()
	{
		try
		{
			if (grdOperation.DataSource is DataTable dataTable)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["Code"] = "";
				dataRow["Name"] = "";
				dataRow["Status"] = gvOperation.GetRowCellValue(gvOperation.FocusedRowHandle, "Status");
				dataRow["Remark"] = "";
				dataRow["Alternative"] = gvOperation.GetRowCellValue(gvOperation.FocusedRowHandle, "Alternative");
				dataTable.Rows.InsertAt(dataRow, gvOperation.RowCount - 1);
				grdOperation.DataSource = dataTable;
				grdOperation.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void DeleteRowDocs()
	{
		try
		{
			if (!string.IsNullOrEmpty(Function.ToString(gvOperation.GetFocusedRowCellValue("Code"))))
			{
				DataTable dataTable = grdOperation.DataSource as DataTable;
				dataTable.Rows.RemoveAt(gvOperation.FocusedRowHandle);
				dataTable.AcceptChanges();
				grdOperation.DataSource = dataTable;
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

	private async void DeleteRowDocsAwait(string ids)
	{
		try
		{
			using HttpClientHelper<OperationImpl> _helper = new HttpClientHelper<OperationImpl>(Configuration.ApiServer, "Operation");
			string text = await _helper.PostParamsAsJsonAsync("DeleteDocs", ids);
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

	private async void Save()
	{
	}

	private async Task<string> WaitSaveData(DataTable dtFather)
	{
		try
		{
			using HttpClientHelper<ControlKeyImpl> _helper = new HttpClientHelper<ControlKeyImpl>(Configuration.ApiServer, "Operation");
			return await _helper.PostParamsAsJsonAsync("SaveDocs", dtFather);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return null;
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
		DuplicateRowDocs();
	}

	private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DeleteRowDocs();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvOperation_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		Close();
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

	private async void btSave_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = grdOperation.DataSource as DataTable;
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					if (string.IsNullOrEmpty(Function.ToString(dataTable.Rows[i]["Code"])))
					{
						dataTable.Rows.RemoveAt(i);
						i--;
						dataTable.AcceptChanges();
					}
				}
			}
			WinCommons._OpenProcessing();
			string text = await WaitSaveData(dataTable);
			WinCommons._CloseProcessing();
			if (text == "")
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

	private void gvOperation_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			if (e.RowHandle == gvOperation.FocusedRowHandle)
			{
				e.Appearance.BackColor = StyleColor.GridEditting;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvOperation_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			_edit = true;
			DataTable dataTable = grdOperation.DataSource as DataTable;
			if (e.Column.FieldName == "Code")
			{
				if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[dataTable.Count() - 1]["Code"])))
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

	private void frm_Operation_FormClosing(object sender, FormClosingEventArgs e)
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

	private void gvOperation_MouseLeave(object sender, EventArgs e)
	{
		HotTrackRow = int.MinValue;
	}

	private void gvOperation_MouseUp(object sender, MouseEventArgs e)
	{
	}

	private void gvOperation_MouseMove(object sender, MouseEventArgs e)
	{
		GridHitInfo gridHitInfo = (sender as GridView).CalcHitInfo(new Point(e.X, e.Y));
		if (gridHitInfo.InRowCell)
		{
			HotTrackRow = gridHitInfo.RowHandle;
		}
		else
		{
			HotTrackRow = int.MinValue;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.RoutingAndResource.frm_Operation));
		this.panel1 = new System.Windows.Forms.Panel();
		this.btRefresh = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btSave = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grdOperation = new Apzon.Controls.Grid.GridControlEx();
		this.ctxDocs = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.duplicateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvOperation = new Apzon.Controls.Grid.GridViewEx();
		this.clCode = new Apzon.Controls.Grid.GridColumnEx();
		this.clName = new Apzon.Controls.Grid.GridColumnEx();
		this.clStatus = new Apzon.Controls.Grid.GridColumnEx();
		this.repoStatus = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.clRemark = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoExEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.clAlternative = new Apzon.Controls.Grid.GridColumnEx();
		this.repStatus = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
		this.panel1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdOperation).BeginInit();
		this.ctxDocs.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvOperation).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repoStatus).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repStatus).BeginInit();
		base.SuspendLayout();
		this.panel1.Controls.Add(this.btRefresh);
		this.panel1.Controls.Add(this.btCancel);
		this.panel1.Controls.Add(this.btSave);
		this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panel1.Location = new System.Drawing.Point(0, 290);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(590, 33);
		this.panel1.TabIndex = 51;
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
		this.grdOperation.AllowPasteExcel = true;
		this.grdOperation.ClipboardData = null;
		this.grdOperation.ContextMenuStrip = this.ctxDocs;
		this.grdOperation.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdOperation.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grdOperation.Location = new System.Drawing.Point(0, 0);
		this.grdOperation.MainView = this.gvOperation;
		this.grdOperation.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grdOperation.Name = "grdOperation";
		this.grdOperation.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.repStatus, this.repoStatus, this.repositoryItemMemoExEdit1 });
		this.grdOperation.Size = new System.Drawing.Size(590, 290);
		this.grdOperation.TabIndex = 52;
		this.grdOperation.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvOperation });
		this.ctxDocs.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.duplicateToolStripMenuItem, this.deleteRowToolStripMenuItem });
		this.ctxDocs.Name = "contextMenuStrip1";
		this.ctxDocs.Size = new System.Drawing.Size(134, 48);
		this.duplicateToolStripMenuItem.Name = "duplicateToolStripMenuItem";
		this.duplicateToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.duplicateToolStripMenuItem.Text = "Duplicate";
		this.duplicateToolStripMenuItem.Click += new System.EventHandler(this.duplicateToolStripMenuItem_Click);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.gvOperation.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvOperation.ColumnKey = null;
		this.gvOperation.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.clCode, this.clName, this.clStatus, this.clRemark, this.clAlternative });
		this.gvOperation.DisableFormSetting = false;
		this.gvOperation.DisplayException = false;
		this.gvOperation.EditColumn = null;
		this.gvOperation.FieldCurrency = null;
		this.gvOperation.FieldRate = null;
		this.gvOperation.FormId = null;
		this.gvOperation.GridControl = this.grdOperation;
		this.gvOperation.IgnoreFilter = false;
		this.gvOperation.IndexEditRow = -1;
		this.gvOperation.IndicatorWidth = 40;
		this.gvOperation.IsApzTable = false;
		this.gvOperation.IsInsertNewRow = false;
		this.gvOperation.IsLoadingData = false;
		this.gvOperation.IsMappingContextAction = false;
		this.gvOperation.ItemId = null;
		this.gvOperation.LstColumnAllowBatch = null;
		this.gvOperation.LstColumnNotAllowBatch = null;
		this.gvOperation.LstColumnNotEditable = null;
		this.gvOperation.LstColumNotSetting = null;
		this.gvOperation.Name = "gvOperation";
		this.gvOperation.OptionsSelection.MultiSelect = true;
		this.gvOperation.OptionsView.ColumnAutoWidth = false;
		this.gvOperation.OptionsView.ShowGroupPanel = false;
		this.gvOperation.UseBatchMod = false;
		this.gvOperation.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvOperation_CustomDrawRowIndicator);
		this.gvOperation.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gvOperation_RowStyle);
		this.gvOperation.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvOperation_CellValueChanged);
		this.gvOperation.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gvOperation_MouseUp);
		this.gvOperation.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gvOperation_MouseMove);
		this.gvOperation.MouseLeave += new System.EventHandler(this.gvOperation_MouseLeave);
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
		this.clStatus.Caption = "Status";
		this.clStatus.ColumnEdit = this.repoStatus;
		this.clStatus.CurrencyDateField = null;
		this.clStatus.CurrencyField = null;
		this.clStatus.CurrencyFixed = null;
		this.clStatus.CurrencyFixedRate = 0.0;
		this.clStatus.CurrencyRateField = null;
		this.clStatus.FieldName = "Status";
		this.clStatus.Name = "clStatus";
		this.clStatus.PrimaryKey = false;
		this.clStatus.UnitField = null;
		this.clStatus.Visible = true;
		this.clStatus.VisibleIndex = 2;
		this.clStatus.Width = 63;
		this.repoStatus.AutoHeight = false;
		this.repoStatus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repoStatus.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Status"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "ID")
		});
		this.repoStatus.DisplayMember = "Name";
		this.repoStatus.Name = "repoStatus";
		this.repoStatus.ValueMember = "ID";
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
		this.clRemark.VisibleIndex = 4;
		this.clRemark.Width = 132;
		this.repositoryItemMemoExEdit1.AutoHeight = false;
		this.repositoryItemMemoExEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit1.Name = "repositoryItemMemoExEdit1";
		this.repositoryItemMemoExEdit1.ShowIcon = false;
		this.clAlternative.Caption = "Alternative Operation";
		this.clAlternative.ColumnEdit = this.repositoryItemMemoExEdit1;
		this.clAlternative.CurrencyDateField = null;
		this.clAlternative.CurrencyField = null;
		this.clAlternative.CurrencyFixed = null;
		this.clAlternative.CurrencyFixedRate = 0.0;
		this.clAlternative.CurrencyRateField = null;
		this.clAlternative.FieldName = "Alternative";
		this.clAlternative.Name = "clAlternative";
		this.clAlternative.PrimaryKey = false;
		this.clAlternative.UnitField = null;
		this.clAlternative.Visible = true;
		this.clAlternative.VisibleIndex = 3;
		this.clAlternative.Width = 161;
		this.repStatus.AutoHeight = false;
		this.repStatus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repStatus.Items.AddRange(new object[2] { "Active", "Inactive" });
		this.repStatus.Name = "repStatus";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(590, 323);
		base.Controls.Add(this.grdOperation);
		base.Controls.Add(this.panel1);
		base.IconOptions.ShowIcon = false;
		base.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		base.Name = "frm_Operation";
		base.ShowInTaskbar = false;
		this.Text = "Operation";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_Operation_FormClosing);
		base.Load += new System.EventHandler(this.frm_Operation_Load);
		this.panel1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdOperation).EndInit();
		this.ctxDocs.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvOperation).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repoStatus).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repStatus).EndInit();
		base.ResumeLayout(false);
	}
}
