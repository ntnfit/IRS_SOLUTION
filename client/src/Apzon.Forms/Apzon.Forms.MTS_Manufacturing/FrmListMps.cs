using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.PlanningProduction;
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Menu;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.MTS_Manufacturing;

public class FrmListMps : XtraFormEx
{
	public delegate void ShowForms(XtraFormEx frm);

	private HttpClientHelper<Mps> _mpsClientHelper = new HttpClientHelper<Mps>(Configuration.ApiServer, "Mps");

	public string mpsCode = string.Empty;

	public ShowForms ShowChildForm;

	private IContainer components;

	private GroupControl groupControl1;

	private SimpleButtonEx btnCancel;

	private SimpleButtonEx btSearch;

	private DateEditEx endDate;

	private DateEditEx beginDate;

	private ComboBoxEditEx cbStatus;

	private TextEditEx txtCode;

	private LabelLine labelLine4;

	private LabelLine labelLine3;

	private LabelLine labelLine2;

	private LabelLine labelLine1;

	private GroupControl groupControl2;

	private GridControlEx grdControlMPS;

	private GridViewEx grdViewMPS;

	private GridColumnEx gColId;

	private GridColumnEx gColCode;

	private GridColumnEx gColCreateDate;

	private GridColumnEx gColBeginDate;

	private GridColumnEx gColEndDate;

	private GridColumnEx gColStatus;

	private GridColumnEx gColNotes;

	public FrmListMps()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "FrmListMps"))
		{
			InitializeComponent();
			WinFuntions.CustomizeAppearanceGridView(grdViewMPS);
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "FrmListMps" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btnCancel.Enabled = true;
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		try
		{
			_mpsClientHelper.Dispose();
		}
		catch (Exception)
		{
		}
		base.Dispose(disposing);
	}

	private async void btSearch_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		await RefreshData();
		WinCommons._CloseProcessing();
	}

	private async Task RefreshData()
	{
		_ = 1;
		try
		{
			string text = string.Empty;
			if (!string.IsNullOrEmpty(txtCode.Text))
			{
				text = txtCode.Text.Trim().ToUpper();
			}
			DateTime dateTime = beginDate.DateTime;
			DateTime dateTime2 = endDate.DateTime;
			new List<Mps>();
			int status = 0;
			if (cbStatus.SelectedItem != null && !object.Equals(cbStatus.SelectedItem, string.Empty))
			{
				status = Convert.ToInt32(((Common.ListItems)cbStatus.SelectedItem).Value);
			}
			List<Mps> dataSource = ((!(dateTime2.Date == DateTime.MinValue)) ? (await GetListMps(text, status, dateTime, dateTime2)) : (await GetListMps(text, status, dateTime, DateTime.Now)));
			grdControlMPS.DataSource = dataSource;
			grdControlMPS.RefreshDataSource();
			grdControlMPS.Refresh();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<List<Mps>> GetListMps(string mpsCode, int status, DateTime bDate, DateTime eDate)
	{
		try
		{
			return await _mpsClientHelper.PostListParameter("GetListMps", new { mpsCode, status, bDate, eDate });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new List<Mps>();
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void grdViewMPS_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
	{
		if (e.Column.FieldName.Equals("Status") && e.Value != null)
		{
			if (e.Value.ToString() == 1.ToString())
			{
				e.DisplayText = "Draft";
			}
			else if (e.Value.ToString() == 2.ToString())
			{
				e.DisplayText = "Aprroved";
			}
			else if (e.Value.ToString() == 3.ToString())
			{
				e.DisplayText = "Open";
			}
			else if (e.Value.ToString() == 4.ToString())
			{
				e.DisplayText = "Close";
			}
			else if (e.Value.ToString() == (-1).ToString())
			{
				e.DisplayText = "Reject";
			}
			else if (e.Value.ToString() == 5.ToString())
			{
				e.DisplayText = "Done";
			}
		}
	}

	private void grdViewMPS_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		try
		{
			if (e.Info.IsRowIndicator)
			{
				e.Info.DisplayText = (e.RowHandle + 1).ToString();
			}
		}
		catch (Exception)
		{
		}
	}

	private async void grdViewMPS_DoubleClick(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		await ViewDetail();
		WinCommons._CloseProcessing();
	}

	private async Task ViewDetail()
	{
		WinCommons._OpenProcessing();
		if (grdViewMPS.FocusedRowHandle >= 0)
		{
			Convert.ToInt32(grdViewMPS.GetRowCellValue(grdViewMPS.FocusedRowHandle, "Status").ToString());
			string text = grdViewMPS.GetRowCellValue(grdViewMPS.FocusedRowHandle, "MpsCode").ToString();
			FrmMps frmMps = new FrmMps(Convert.ToInt32(grdViewMPS.GetRowCellValue(grdViewMPS.FocusedRowHandle, "Id").ToString()), text);
			ShowChildForm(frmMps);
			frmMps.FormClosing += frm_FormClosing;
		}
		WinCommons._CloseProcessing();
	}

	private async void frm_FormClosing(object sender, FormClosingEventArgs e)
	{
		WinCommons._OpenProcessing();
		await RefreshData();
		WinCommons._CloseProcessing();
	}

	private void grdViewMPS_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
	{
		try
		{
			GridViewMenu gridViewMenu = new GridViewMenu(grdViewMPS);
			string[] array = new string[2] { "View Detail", "Delete Row" };
			foreach (string caption in array)
			{
				DXMenuItem dXMenuItem = new DXMenuItem();
				dXMenuItem.Caption = caption;
				dXMenuItem.Tag = grdViewMPS.GetRowCellValue(e.HitInfo.RowHandle, gColId);
				dXMenuItem.Visible = true;
				dXMenuItem.Click += menuItem_Click;
				gridViewMenu.Items.Add(dXMenuItem);
			}
			if (e.MenuType == GridMenuType.Row)
			{
				e.Allow = true;
				gridViewMenu.Show(e.Point);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void menuItem_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (!(sender is DXMenuItem dXMenuItem))
			{
				return;
			}
			if (dXMenuItem.Caption.Trim().Equals("View Detail"))
			{
				ViewDetail();
			}
			else if (dXMenuItem.Caption.Trim().Equals("Delete Row"))
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					string id = dXMenuItem.Tag.ToString();
					string text = await DeleteMps(id);
					if (string.IsNullOrEmpty(text))
					{
						await RefreshData();
						WinCommons.Success(Enums.MessageCode.Success, "");
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, text);
					}
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.FormAccessDenied, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<string> DeleteMps(string id)
	{
		try
		{
			return await _mpsClientHelper.PostAsJsonAsync("DeleteData", new Mps
			{
				Id = Function.ParseLong(id)
			});
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	private void InitializeComponent()
	{
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.MTS_Manufacturing.FrmListMps));
		this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btSearch = new Apzon.Controls.Controls.SimpleButtonEx();
		this.endDate = new Apzon.Controls.Editor.DateEditEx();
		this.beginDate = new Apzon.Controls.Editor.DateEditEx();
		this.cbStatus = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.txtCode = new Apzon.Controls.Editor.TextEditEx();
		this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
		this.grdControlMPS = new Apzon.Controls.Grid.GridControlEx();
		this.grdViewMPS = new Apzon.Controls.Grid.GridViewEx();
		this.gColId = new Apzon.Controls.Grid.GridColumnEx();
		this.gColCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gColCreateDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gColBeginDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gColEndDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gColStatus = new Apzon.Controls.Grid.GridColumnEx();
		this.gColNotes = new Apzon.Controls.Grid.GridColumnEx();
		((System.ComponentModel.ISupportInitialize)this.groupControl1).BeginInit();
		this.groupControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.endDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.endDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.beginDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.beginDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbStatus.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl2).BeginInit();
		this.groupControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdControlMPS).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdViewMPS).BeginInit();
		base.SuspendLayout();
		this.groupControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.groupControl1.Appearance.Options.UseFont = true;
		this.groupControl1.Controls.Add(this.labelLine4);
		this.groupControl1.Controls.Add(this.labelLine3);
		this.groupControl1.Controls.Add(this.labelLine2);
		this.groupControl1.Controls.Add(this.labelLine1);
		this.groupControl1.Controls.Add(this.btnCancel);
		this.groupControl1.Controls.Add(this.btSearch);
		this.groupControl1.Controls.Add(this.endDate);
		this.groupControl1.Controls.Add(this.beginDate);
		this.groupControl1.Controls.Add(this.cbStatus);
		this.groupControl1.Controls.Add(this.txtCode);
		this.groupControl1.Dock = System.Windows.Forms.DockStyle.Top;
		this.groupControl1.Location = new System.Drawing.Point(0, 0);
		this.groupControl1.Name = "groupControl1";
		this.groupControl1.Size = new System.Drawing.Size(627, 73);
		this.groupControl1.TabIndex = 1;
		this.groupControl1.Text = "MPS Infomation";
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "To Date";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(275, 46);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(116, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 20;
		this.labelLine4.Utext = "To Date";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Status";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(275, 24);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(116, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 19;
		this.labelLine3.Utext = "Status";
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "From Date";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(7, 46);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(116, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 18;
		this.labelLine2.Utext = "From Date";
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "MPS Code";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(7, 25);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(116, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 17;
		this.labelLine1.Utext = "MPS Code";
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(547, 43);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 16;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.btSearch.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btSearch.EditControl = null;
		this.btSearch.EditValue = "Search";
		this.btSearch.FieldName = null;
		this.btSearch.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btSearch.ImageOptions.Image");
		this.btSearch.IsSearchControl = false;
		this.btSearch.Location = new System.Drawing.Point(547, 21);
		this.btSearch.Name = "btSearch";
		this.btSearch.ReadOnly = false;
		this.btSearch.SettingInVisible = false;
		this.btSearch.SettingReadonly = false;
		this.btSearch.Size = new System.Drawing.Size(75, 23);
		this.btSearch.TabIndex = 4;
		this.btSearch.Text = "Search";
		this.btSearch.Click += new System.EventHandler(this.btSearch_Click);
		this.endDate.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.endDate.EditControl = null;
		this.endDate.EditValue = null;
		this.endDate.FieldName = null;
		this.endDate.Location = new System.Drawing.Point(391, 45);
		this.endDate.Name = "endDate";
		this.endDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.endDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.endDate.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.endDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.endDate.Properties.EditFormat.FormatString = "dd/MM/yyyy";
		this.endDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.endDate.Properties.IsSearchControl = false;
		this.endDate.Properties.Mask.EditMask = "dd/MM/yyyy";
		this.endDate.SettingInVisible = false;
		this.endDate.SettingReadonly = false;
		this.endDate.Size = new System.Drawing.Size(150, 20);
		this.endDate.TabIndex = 3;
		this.beginDate.EditControl = null;
		this.beginDate.EditValue = null;
		this.beginDate.FieldName = null;
		this.beginDate.Location = new System.Drawing.Point(123, 45);
		this.beginDate.Name = "beginDate";
		this.beginDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.beginDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.beginDate.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.beginDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.beginDate.Properties.EditFormat.FormatString = "dd/MM/yyyy";
		this.beginDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.beginDate.Properties.IsSearchControl = false;
		this.beginDate.Properties.Mask.EditMask = "dd/MM/yyyy";
		this.beginDate.SettingInVisible = false;
		this.beginDate.SettingReadonly = false;
		this.beginDate.Size = new System.Drawing.Size(150, 20);
		this.beginDate.TabIndex = 1;
		this.cbStatus.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.cbStatus.EditControl = null;
		this.cbStatus.FieldName = null;
		this.cbStatus.IsSearchControl = false;
		this.cbStatus.Location = new System.Drawing.Point(391, 22);
		this.cbStatus.Name = "cbStatus";
		this.cbStatus.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbStatus.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.cbStatus.SettingInVisible = false;
		this.cbStatus.SettingReadonly = false;
		this.cbStatus.Size = new System.Drawing.Size(150, 20);
		this.cbStatus.TabIndex = 2;
		this.txtCode.EditControl = null;
		this.txtCode.FieldName = null;
		this.txtCode.IsSearchControl = false;
		this.txtCode.Location = new System.Drawing.Point(123, 22);
		this.txtCode.Name = "txtCode";
		this.txtCode.SettingInVisible = false;
		this.txtCode.SettingReadonly = false;
		this.txtCode.Size = new System.Drawing.Size(150, 20);
		this.txtCode.TabIndex = 0;
		this.groupControl2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.groupControl2.Controls.Add(this.grdControlMPS);
		this.groupControl2.Location = new System.Drawing.Point(0, 72);
		this.groupControl2.Name = "groupControl2";
		this.groupControl2.Size = new System.Drawing.Size(627, 290);
		this.groupControl2.TabIndex = 2;
		this.groupControl2.Text = "List Of MPS";
		this.grdControlMPS.AllowPasteExcel = true;
		this.grdControlMPS.ClipboardData = null;
		this.grdControlMPS.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdControlMPS.Location = new System.Drawing.Point(2, 23);
		this.grdControlMPS.MainView = this.grdViewMPS;
		this.grdControlMPS.Name = "grdControlMPS";
		this.grdControlMPS.Size = new System.Drawing.Size(623, 265);
		this.grdControlMPS.TabIndex = 0;
		this.grdControlMPS.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.grdViewMPS });
		this.grdViewMPS.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.grdViewMPS.ColumnKey = null;
		this.grdViewMPS.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.gColId, this.gColCode, this.gColCreateDate, this.gColBeginDate, this.gColEndDate, this.gColStatus, this.gColNotes });
		this.grdViewMPS.DisableFormSetting = false;
		this.grdViewMPS.DisplayException = false;
		this.grdViewMPS.EditColumn = null;
		this.grdViewMPS.FieldCurrency = null;
		this.grdViewMPS.FieldRate = null;
		this.grdViewMPS.FormId = null;
		this.grdViewMPS.GridControl = this.grdControlMPS;
		this.grdViewMPS.IgnoreFilter = false;
		this.grdViewMPS.IndexEditRow = -1;
		this.grdViewMPS.IndicatorWidth = 30;
		this.grdViewMPS.IsApzTable = false;
		this.grdViewMPS.IsInsertNewRow = false;
		this.grdViewMPS.IsLoadingData = false;
		this.grdViewMPS.IsMappingContextAction = false;
		this.grdViewMPS.ItemId = null;
		this.grdViewMPS.LstColumnAllowBatch = null;
		this.grdViewMPS.LstColumnNotAllowBatch = null;
		this.grdViewMPS.LstColumnNotEditable = null;
		this.grdViewMPS.LstColumNotSetting = null;
		this.grdViewMPS.Name = "grdViewMPS";
		this.grdViewMPS.OptionsBehavior.Editable = false;
		this.grdViewMPS.OptionsView.ShowGroupPanel = false;
		this.grdViewMPS.UseBatchMod = false;
		this.grdViewMPS.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.grdViewMPS_CustomDrawRowIndicator);
		this.grdViewMPS.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.grdViewMPS_PopupMenuShowing);
		this.grdViewMPS.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.grdViewMPS_CustomColumnDisplayText);
		this.grdViewMPS.DoubleClick += new System.EventHandler(this.grdViewMPS_DoubleClick);
		this.gColId.Caption = "Id";
		this.gColId.CurrencyDateField = null;
		this.gColId.CurrencyField = null;
		this.gColId.CurrencyFixed = null;
		this.gColId.CurrencyFixedRate = 0.0;
		this.gColId.CurrencyRateField = null;
		this.gColId.FieldName = "Id";
		this.gColId.Name = "gColId";
		this.gColId.OptionsColumn.AllowEdit = false;
		this.gColId.PrimaryKey = false;
		this.gColId.UnitField = null;
		this.gColCode.Caption = "MPS Code";
		this.gColCode.CurrencyDateField = null;
		this.gColCode.CurrencyField = null;
		this.gColCode.CurrencyFixed = null;
		this.gColCode.CurrencyFixedRate = 0.0;
		this.gColCode.CurrencyRateField = null;
		this.gColCode.FieldName = "MpsCode";
		this.gColCode.Name = "gColCode";
		this.gColCode.OptionsColumn.AllowEdit = false;
		this.gColCode.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
		this.gColCode.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gColCode.PrimaryKey = true;
		this.gColCode.UnitField = null;
		this.gColCode.Visible = true;
		this.gColCode.VisibleIndex = 0;
		this.gColCode.Width = 108;
		this.gColCreateDate.Caption = "Create Date";
		this.gColCreateDate.CurrencyDateField = null;
		this.gColCreateDate.CurrencyField = null;
		this.gColCreateDate.CurrencyFixed = null;
		this.gColCreateDate.CurrencyFixedRate = 0.0;
		this.gColCreateDate.CurrencyRateField = null;
		this.gColCreateDate.FieldName = "CreateDate";
		this.gColCreateDate.Name = "gColCreateDate";
		this.gColCreateDate.OptionsColumn.AllowEdit = false;
		this.gColCreateDate.PrimaryKey = false;
		this.gColCreateDate.UnitField = null;
		this.gColCreateDate.Visible = true;
		this.gColCreateDate.VisibleIndex = 1;
		this.gColCreateDate.Width = 83;
		this.gColBeginDate.Caption = "From Date";
		this.gColBeginDate.CurrencyDateField = null;
		this.gColBeginDate.CurrencyField = null;
		this.gColBeginDate.CurrencyFixed = null;
		this.gColBeginDate.CurrencyFixedRate = 0.0;
		this.gColBeginDate.CurrencyRateField = null;
		this.gColBeginDate.FieldName = "BeginDate";
		this.gColBeginDate.Name = "gColBeginDate";
		this.gColBeginDate.OptionsColumn.AllowEdit = false;
		this.gColBeginDate.PrimaryKey = false;
		this.gColBeginDate.UnitField = null;
		this.gColBeginDate.Visible = true;
		this.gColBeginDate.VisibleIndex = 2;
		this.gColBeginDate.Width = 84;
		this.gColEndDate.Caption = "To Date";
		this.gColEndDate.CurrencyDateField = null;
		this.gColEndDate.CurrencyField = null;
		this.gColEndDate.CurrencyFixed = null;
		this.gColEndDate.CurrencyFixedRate = 0.0;
		this.gColEndDate.CurrencyRateField = null;
		this.gColEndDate.FieldName = "EndDate";
		this.gColEndDate.Name = "gColEndDate";
		this.gColEndDate.OptionsColumn.AllowEdit = false;
		this.gColEndDate.PrimaryKey = false;
		this.gColEndDate.UnitField = null;
		this.gColEndDate.Visible = true;
		this.gColEndDate.VisibleIndex = 3;
		this.gColEndDate.Width = 85;
		this.gColStatus.Caption = "Status";
		this.gColStatus.CurrencyDateField = null;
		this.gColStatus.CurrencyField = null;
		this.gColStatus.CurrencyFixed = null;
		this.gColStatus.CurrencyFixedRate = 0.0;
		this.gColStatus.CurrencyRateField = null;
		this.gColStatus.FieldName = "Status";
		this.gColStatus.Name = "gColStatus";
		this.gColStatus.OptionsColumn.AllowEdit = false;
		this.gColStatus.PrimaryKey = false;
		this.gColStatus.UnitField = null;
		this.gColStatus.Visible = true;
		this.gColStatus.VisibleIndex = 4;
		this.gColStatus.Width = 80;
		this.gColNotes.Caption = "Remark";
		this.gColNotes.CurrencyDateField = null;
		this.gColNotes.CurrencyField = null;
		this.gColNotes.CurrencyFixed = null;
		this.gColNotes.CurrencyFixedRate = 0.0;
		this.gColNotes.CurrencyRateField = null;
		this.gColNotes.FieldName = "Notes";
		this.gColNotes.Name = "gColNotes";
		this.gColNotes.OptionsColumn.AllowEdit = false;
		this.gColNotes.PrimaryKey = false;
		this.gColNotes.UnitField = null;
		this.gColNotes.Visible = true;
		this.gColNotes.VisibleIndex = 5;
		this.gColNotes.Width = 209;
		base.Appearance.Options.UseFont = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(5f, 9f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(627, 361);
		base.Controls.Add(this.groupControl2);
		base.Controls.Add(this.groupControl1);
		this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, 0);
		base.IconOptions.ShowIcon = false;
		base.Margin = new System.Windows.Forms.Padding(2);
		base.Name = "FrmListMps";
		base.ShowInTaskbar = false;
		this.Text = "List Of Mps";
		((System.ComponentModel.ISupportInitialize)this.groupControl1).EndInit();
		this.groupControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.endDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.endDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.beginDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.beginDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbStatus.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl2).EndInit();
		this.groupControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdControlMPS).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdViewMPS).EndInit();
		base.ResumeLayout(false);
	}
}
