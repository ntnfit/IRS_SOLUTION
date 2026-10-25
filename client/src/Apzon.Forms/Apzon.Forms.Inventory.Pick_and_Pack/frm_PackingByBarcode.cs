using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
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
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.UserFields;
using Apzon.Forms.Commons;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.HttpClientHelper;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Menu;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Inventory.Pick_and_Pack;

public class frm_PackingByBarcode : XtraFormEx
{
	private const string UdoCode = "OPKB";

	public string UdoTableName = "@OPKB";

	public DataTable UdoTable = new DataTable();

	private const string FormController = "PickAndPack";

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private List<UserFieldsImpl> lstUdf = new List<UserFieldsImpl>();

	private IContainer components;

	private LabelLine labelLine3;

	private DateEditEx date_Doc;

	private LabelLine labelLine4;

	private TextEditEx txt_Batch;

	private GridControlEx grd_Counting;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx simpleButton2;

	private BackgroundWorker backgroundWorker1;

	private GridViewEx gv_Counting;

	private SimpleButtonEx simpleButtonEx1;

	public DateTime GetDate { get; set; }

	public frm_PackingByBarcode()
	{
		InitializeComponent();
	}

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			simpleButtonEx1.Enabled = true;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Add:
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	private async void frm_PackingByBarcode_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			DateTime today = DateTime.Today;
			date_Doc.EditValue = today;
			GetDate = today;
			lstUdf = await _httpClient.Udf.GetListField("@OPKB");
			UdoTable = await _httpClient.Udo.GetUdoByCode(new Common.Table_Code
			{
				Name = "OUDO",
				Code = "OPKB"
			});
			ResetData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void txt_Batch_KeyDown(object sender, KeyEventArgs e)
	{
		_ = 1;
		try
		{
			if (e.KeyCode != Keys.Return || string.IsNullOrEmpty(txt_Batch.Text))
			{
				return;
			}
			txt_Batch.Enabled = false;
			WinCommons._OpenProcessing();
			string barcode = Function.ToString(txt_Batch.Text);
			DataSetResult dataSetResult = await _httpClient.PackingBatch.GetDataByBarcode(barcode, "");
			if (!string.IsNullOrEmpty(dataSetResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.Error, dataSetResult.Message);
				txt_Batch.Text = string.Empty;
				txt_Batch.Enabled = true;
				txt_Batch.Focus();
				return;
			}
			DataSet ds = dataSetResult.Data;
			if (ds.IsHasStructure())
			{
				if (gv_Counting.Columns.Count <= 0)
				{
					await GridViewHelper.GenerateGrid(grd_Counting, gv_Counting, ds);
				}
				else
				{
					DataTable dataSourceTable = grd_Counting.DataSourceTable;
					if (dataSourceTable.IsNotNull() && ds.Tables[1].IsNotNull())
					{
						foreach (DataRow row in ds.Tables[1].Rows)
						{
							string arg = Function.ToString(row["U_ItemCode"]);
							string arg2 = Function.ToString(row["U_LotNo"]);
							string arg3 = Function.ToString(row["U_TemKey"]);
							if (dataSourceTable.Select($"U_ItemCode='{arg}' AND U_LotNo='{arg2}' AND U_TemKey='{arg3}'").Length != 0)
							{
								WinCommons.Error(Enums.MessageCode.Error, "Tem đã được bắn!");
								continue;
							}
							DataRow dataRow2 = dataSourceTable.NewRow();
							dataRow2.ItemArray = row.ItemArray;
							dataSourceTable.Rows.Add(dataRow2);
							gv_Counting.FocusedRowHandle = dataSourceTable.Rows.IndexOf(dataRow2);
						}
						dataSourceTable.AcceptChanges();
					}
					else
					{
						grd_Counting.DataSource = ds.Tables[1];
						grd_Counting.RefreshDataSource();
					}
				}
				FormMode = Enums.FormViewModeType.Add;
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.Error, "Không tìm thấy bản ghi nào.");
			}
			WinCommons._CloseProcessing();
			ds.Tables.Clear();
			ds.Dispose();
			txt_Batch.Text = string.Empty;
			txt_Batch.Enabled = true;
			txt_Batch.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			txt_Batch.Enabled = true;
		}
	}

	private async void btn_Transfer_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			if (XtraMessageBox.Show(Apzon.Commons.Properties.Resources.xtraMsg_SaveChange, Apzon.Commons.Properties.Resources.Msg, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				return;
			}
			if (UdoTable == null || UdoTable.Rows.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.Error, "UDO Table is invalid.");
				return;
			}
			btn_OK.Enabled = false;
			DataTable dataTable = grd_Counting.DatasourceTable();
			if (!dataTable.IsNotNull())
			{
				WinCommons.Warning(Enums.MessageCode.Error, "Data is Empty!");
				return;
			}
			WinCommons._OpenProcessing();
			DataSet dataSet = new DataSet();
			string tableName = Function.ToString(UdoTable.Rows[0]["TableName"]);
			dataSet.Tables.Add(UdoTable.Copy());
			DataTable dataTable2 = new DataTable(tableName);
			dataTable2.Columns.Add("DocEntry", typeof(int));
			dataTable2.Columns.Add("CreateDate", typeof(DateTime));
			dataTable2.Columns.Add("U_BoxCode", typeof(string));
			DataRow dataRow = dataTable2.NewRow();
			dataRow["DocEntry"] = 0;
			dataRow["CreateDate"] = date_Doc.EditValue;
			dataRow["U_BoxCode"] = "";
			dataTable2.Rows.Add(dataRow);
			dataSet.Tables.Add(dataTable2);
			DataTable dataTable3 = dataTable.Copy();
			dataTable3.TableName = "PKB1";
			foreach (string item in (from DataColumn t in dataTable3.Columns
				where !t.ColumnName.Equals("DocEntry") && !t.ColumnName.Equals("LineNum") && !t.ColumnName.StartsWith("U_")
				select t.ColumnName).ToList())
			{
				dataTable3.Columns.Remove(item);
			}
			dataSet.Tables.Add(dataTable3);
			HttpResult httpResult = await _httpClient.Udo.Save(dataSet);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				XtraMessageBox.Show(httpResult.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			else
			{
				WinCommons.Success(Enums.MessageCode.Success, Apzon.Commons.Properties.Resources.Msg_SaveSuccess);
				grd_Counting.DataSource = null;
				grd_Counting.RefreshDataSource();
				await WinFuntions.PrintLayout("", Function.ParseInt(httpResult.Content), "OPKB");
				FormMode = Enums.FormViewModeType.View;
				txt_Batch.Text = "";
				txt_Batch.Focus();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		btn_OK.Enabled = true;
		WinCommons._CloseProcessing();
	}

	private void simpleButton2_Click(object sender, EventArgs e)
	{
		try
		{
			grd_Counting.DataSource = null;
			txt_Batch.Text = "";
			txt_Batch.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ResetData()
	{
		try
		{
			txt_Batch.Text = "";
			FormMode = Enums.FormViewModeType.View;
			txt_Batch.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Counting_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
	{
		try
		{
			GridViewMenu gridViewMenu = new GridViewMenu((GridView)sender);
			DXMenuItem dXMenuItem = new DXMenuItem();
			dXMenuItem.Caption = "Remove Row";
			dXMenuItem.Click += gv_CountingMenu_Click;
			gridViewMenu.Items.Add(dXMenuItem);
			if (e.MenuType == GridMenuType.User || e.MenuType == GridMenuType.Row)
			{
				e.Allow = true;
				gridViewMenu.Show(e.Point);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.Error, ex.Message);
		}
	}

	private void gv_CountingMenu_Click(object sender, EventArgs e)
	{
		try
		{
			if ((sender as DXMenuItem).Caption.Equals("Remove Row") && gv_Counting.FocusedRowHandle > -1)
			{
				gv_Counting.DeleteRow(gv_Counting.FocusedRowHandle);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void simpleButtonEx1_Click(object sender, EventArgs e)
	{
		Close();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Inventory.Pick_and_Pack.frm_PackingByBarcode));
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.date_Doc = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Batch = new Apzon.Controls.Editor.TextEditEx();
		this.grd_Counting = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Counting = new Apzon.Controls.Grid.GridViewEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButton2 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
		this.simpleButtonEx1 = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.date_Doc.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_Doc.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Batch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Counting).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Counting).BeginInit();
		base.SuspendLayout();
		this.labelLine3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Doc Date";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(655, 9);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(80, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 4;
		this.labelLine3.Utext = "Doc Date";
		this.date_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.date_Doc.EditControl = null;
		this.date_Doc.EditValue = null;
		this.date_Doc.FieldName = null;
		this.date_Doc.Location = new System.Drawing.Point(734, 6);
		this.date_Doc.Name = "date_Doc";
		this.date_Doc.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_Doc.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_Doc.Properties.IsSearchControl = false;
		this.date_Doc.SettingInVisible = false;
		this.date_Doc.SettingReadonly = false;
		this.date_Doc.Size = new System.Drawing.Size(142, 20);
		this.date_Doc.TabIndex = 5;
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "QR Code";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(5, 8);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(105, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 11;
		this.labelLine4.Utext = "QR Code";
		this.txt_Batch.EditControl = null;
		this.txt_Batch.FieldName = null;
		this.txt_Batch.IsSearchControl = false;
		this.txt_Batch.Location = new System.Drawing.Point(110, 6);
		this.txt_Batch.Name = "txt_Batch";
		this.txt_Batch.SettingInVisible = false;
		this.txt_Batch.SettingReadonly = false;
		this.txt_Batch.Size = new System.Drawing.Size(241, 20);
		this.txt_Batch.TabIndex = 10;
		this.txt_Batch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_Batch_KeyDown);
		this.grd_Counting.AllowPasteExcel = true;
		this.grd_Counting.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Counting.ClipboardData = null;
		this.grd_Counting.Location = new System.Drawing.Point(0, 32);
		this.grd_Counting.MainView = this.gv_Counting;
		this.grd_Counting.Name = "grd_Counting";
		this.grd_Counting.Size = new System.Drawing.Size(876, 398);
		this.grd_Counting.TabIndex = 12;
		this.grd_Counting.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Counting });
		this.gv_Counting.Appearance.FocusedRow.BackColor = System.Drawing.Color.Transparent;
		this.gv_Counting.Appearance.FocusedRow.ForeColor = System.Drawing.Color.Transparent;
		this.gv_Counting.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Counting.Appearance.FocusedRow.Options.UseForeColor = true;
		this.gv_Counting.Appearance.SelectedRow.BackColor = System.Drawing.Color.Transparent;
		this.gv_Counting.Appearance.SelectedRow.ForeColor = System.Drawing.Color.Transparent;
		this.gv_Counting.Appearance.SelectedRow.Options.UseBackColor = true;
		this.gv_Counting.Appearance.SelectedRow.Options.UseForeColor = true;
		this.gv_Counting.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Counting.ColumnKey = null;
		this.gv_Counting.DisableFormSetting = false;
		this.gv_Counting.DisplayException = false;
		this.gv_Counting.EditColumn = null;
		this.gv_Counting.FieldCurrency = null;
		this.gv_Counting.FieldRate = null;
		this.gv_Counting.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFullFocus;
		this.gv_Counting.FormId = null;
		this.gv_Counting.GridControl = this.grd_Counting;
		this.gv_Counting.IgnoreFilter = false;
		this.gv_Counting.IndexEditRow = -1;
		this.gv_Counting.IndicatorWidth = 35;
		this.gv_Counting.IsApzTable = false;
		this.gv_Counting.IsInsertNewRow = false;
		this.gv_Counting.IsLoadingData = false;
		this.gv_Counting.IsMappingContextAction = false;
		this.gv_Counting.ItemId = null;
		this.gv_Counting.LstColumnAllowBatch = null;
		this.gv_Counting.LstColumnNotAllowBatch = null;
		this.gv_Counting.LstColumnNotEditable = null;
		this.gv_Counting.LstColumNotSetting = null;
		this.gv_Counting.Name = "gv_Counting";
		this.gv_Counting.OptionsView.ShowGroupPanel = false;
		this.gv_Counting.UseBatchMod = false;
		this.gv_Counting.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gv_Counting_PopupMenuShowing);
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(5, 434);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 15;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_Transfer_Click);
		this.simpleButton2.EditControl = null;
		this.simpleButton2.EditValue = "Clear";
		this.simpleButton2.FieldName = null;
		this.simpleButton2.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("simpleButton2.ImageOptions.Image");
		this.simpleButton2.IsSearchControl = false;
		this.simpleButton2.Location = new System.Drawing.Point(357, 6);
		this.simpleButton2.Name = "simpleButton2";
		this.simpleButton2.ReadOnly = false;
		this.simpleButton2.SettingInVisible = false;
		this.simpleButton2.SettingReadonly = false;
		this.simpleButton2.Size = new System.Drawing.Size(75, 20);
		this.simpleButton2.TabIndex = 19;
		this.simpleButton2.Text = "Clear";
		this.simpleButton2.Click += new System.EventHandler(this.simpleButton2_Click);
		this.simpleButtonEx1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.simpleButtonEx1.EditControl = null;
		this.simpleButtonEx1.EditValue = "Cancel";
		this.simpleButtonEx1.FieldName = null;
		this.simpleButtonEx1.ImageOptions.Image = Apzon.Forms.Properties.Resources.cancel_16x16;
		this.simpleButtonEx1.IsSearchControl = false;
		this.simpleButtonEx1.Location = new System.Drawing.Point(84, 434);
		this.simpleButtonEx1.Name = "simpleButtonEx1";
		this.simpleButtonEx1.ReadOnly = false;
		this.simpleButtonEx1.SettingInVisible = false;
		this.simpleButtonEx1.SettingReadonly = false;
		this.simpleButtonEx1.Size = new System.Drawing.Size(75, 23);
		this.simpleButtonEx1.TabIndex = 20;
		this.simpleButtonEx1.Text = "Cancel";
		this.simpleButtonEx1.ToolTip = "Thoát";
		this.simpleButtonEx1.Click += new System.EventHandler(this.simpleButtonEx1_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(877, 460);
		base.Controls.Add(this.simpleButtonEx1);
		base.Controls.Add(this.simpleButton2);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.grd_Counting);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.txt_Batch);
		base.Controls.Add(this.date_Doc);
		base.Controls.Add(this.labelLine3);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_PackingByBarcode";
		base.ShowInTaskbar = false;
		this.Text = "Packing Box";
		base.Load += new System.EventHandler(this.frm_PackingByBarcode_Load);
		((System.ComponentModel.ISupportInitialize)this.date_Doc.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_Doc.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Batch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Counting).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Counting).EndInit();
		base.ResumeLayout(false);
	}
}
