using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Integrated;

public class frm_Integrated_MapCode : XtraFormEx
{
	private string _formController = "Integrated";

	private string _privilege = "N";

	private IContainer components;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private SimpleButtonEx btn_Save;

	private SimpleButtonEx btn_Cancel;

	private Label lbl_Progress;

	private ProgressBar prgBar;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem addRowToolStripMenuItem;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			btn_Cancel.Enabled = true;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btn_Save.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Save.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_Integrated_MapCode()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_Integrated_MapCode"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_Integrated_MapCode" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btn_Cancel.Enabled = true;
				_privilege = "R";
			}
			else
			{
				_privilege = "F";
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private void frm_Integrated_MapCode_Load(object sender, EventArgs e)
	{
		AddNewRow();
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			await SaveData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		try
		{
			Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
	}

	private void grd_Doc_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode != Keys.V || e.Modifiers != Keys.Control)
			{
				return;
			}
			DataObject dataObject = (DataObject)Clipboard.GetDataObject();
			string text = dataObject.GetData(DataFormats.UnicodeText).ToString();
			object data = text;
			if (dataObject.GetDataPresent(DataFormats.Text))
			{
				DataTable dataTable = Function.PasteFromExcel(text);
				if (dataTable != null && (dataTable.Rows.Count > 1 || dataTable.Columns.Count > 1))
				{
					data = dataTable.Rows[0][0];
					int num = gv_Doc.FocusedRowHandle;
					_ = gv_Doc.FocusedRowHandle;
					GridColumn focusedColumn = gv_Doc.FocusedColumn;
					int rowCount = gv_Doc.RowCount;
					int count = dataTable.Rows.Count;
					if (rowCount - num < count)
					{
						DataTable dataTable2 = (DataTable)grd_Doc.DataSource;
						for (int i = 0; i < count - rowCount + num; i++)
						{
							DataRow dataRow = dataTable2.NewRow();
							dataRow["TypeId"] = 0;
							dataRow["TypeName"] = "";
							dataRow["Code"] = "";
							dataRow["Name"] = "";
							dataRow["NewCode"] = "";
							dataRow["NewName"] = "";
							dataTable2.Rows.Add(dataRow);
							dataTable2.AcceptChanges();
						}
						grd_Doc.DataSource = dataTable2;
						grd_Doc.RefreshDataSource();
					}
					int num2 = ((gv_Doc.Columns.Count > dataTable.Columns.Count) ? dataTable.Columns.Count : gv_Doc.Columns.Count);
					for (int j = 0; j < count; j++)
					{
						int colIndex = focusedColumn.VisibleIndex;
						for (int k = 0; k < num2; k++)
						{
							GridColumn column = gv_Doc.Columns.FirstOrDefault((GridColumn t) => t.VisibleIndex == colIndex);
							gv_Doc.SetRowCellValue(num, column, dataTable.Rows[j][k]);
							colIndex++;
						}
						num++;
					}
					AddNewRow();
					Clipboard.Clear();
				}
			}
			Clipboard.SetData(DataFormats.Text, data);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void addRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		AddNewRow();
	}

	private async Task SaveData()
	{
		DisableControls();
		try
		{
			DataTable dt = grd_Doc.DataSource as DataTable;
			DataTable dtSave = dt.Clone();
			if (dt == null || dt.Rows.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.NoDataToSave, "");
				EnableControls();
				return;
			}
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmStartImportData, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				lbl_Progress.Text = "";
				int success = 0;
				int totalLine = gv_Doc.DataRowCount;
				prgBar.Maximum = totalLine + 1;
				prgBar.Minimum = 1;
				prgBar.Step = 1;
				prgBar.Value = 1;
				for (int i = 0; i < dt.Rows.Count; i++)
				{
					if (Function.ParseInt(dt.Rows[i]["TypeId"]) > 0 && !string.IsNullOrWhiteSpace(Function.ToString(dt.Rows[i]["Code"])))
					{
						dtSave.Rows.Clear();
						dtSave.ImportRow(dt.Rows[i]);
						dtSave.AcceptChanges();
						if (string.IsNullOrWhiteSpace(await SaveADoc(dtSave)))
						{
							success++;
						}
					}
					prgBar.PerformStep();
					lbl_Progress.Text = i + 1 + " of " + totalLine;
				}
				lbl_Progress.Text = lbl_Progress.Text + ", " + success + " success.";
				grd_Doc.DataSource = dt;
				grd_Doc.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		EnableControls();
	}

	private void EnableControls()
	{
		try
		{
			btn_Save.Enabled = true;
			btn_Cancel.Enabled = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void DisableControls()
	{
		try
		{
			btn_Save.Enabled = false;
			btn_Cancel.Enabled = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<string> SaveADoc(DataTable dt)
	{
		string result = "";
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, _formController);
			result = await hp.PostAsJsonAsync("MapCodeSaveADoc", dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			result = ex.Message;
		}
		return result;
	}

	private void AddNewRow()
	{
		try
		{
			DataTable dataTable = grd_Doc.DataSource as DataTable;
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				dataTable = GetColumns();
			}
			DataRow dataRow = dataTable.NewRow();
			dataRow["TypeId"] = 0;
			dataRow["TypeName"] = "";
			dataRow["Code"] = "";
			dataRow["Name"] = "";
			dataRow["NewCode"] = "";
			dataRow["NewName"] = "";
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			grd_Doc.DataSource = dataTable;
			grd_Doc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private DataTable GetColumns()
	{
		DataTable dataTable = new DataTable();
		try
		{
			dataTable.Columns.Clear();
			dataTable.Columns.Add(new DataColumn("TypeId", typeof(int)));
			dataTable.Columns.Add(new DataColumn("TypeName", typeof(string)));
			dataTable.Columns.Add(new DataColumn("Code", typeof(string)));
			dataTable.Columns.Add(new DataColumn("Name", typeof(string)));
			dataTable.Columns.Add(new DataColumn("NewCode", typeof(string)));
			dataTable.Columns.Add(new DataColumn("NewName", typeof(string)));
			dataTable.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return dataTable;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Integrated.frm_Integrated_MapCode));
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lbl_Progress = new System.Windows.Forms.Label();
		this.prgBar = new System.Windows.Forms.ProgressBar();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		base.SuspendLayout();
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Doc.Location = new System.Drawing.Point(12, 12);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.Size = new System.Drawing.Size(688, 427);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.grd_Doc.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grd_Doc_EditorKeyDown);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.addRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(123, 26);
		this.addRowToolStripMenuItem.Name = "addRowToolStripMenuItem";
		this.addRowToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
		this.addRowToolStripMenuItem.Text = "Add Row";
		this.addRowToolStripMenuItem.Click += new System.EventHandler(this.addRowToolStripMenuItem_Click);
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = null;
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = null;
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Doc_RowCellStyle);
		this.btn_Save.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "Save";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(12, 449);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(75, 23);
		this.btn_Save.TabIndex = 1;
		this.btn_Save.Text = "Save";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(91, 449);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 2;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.lbl_Progress.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lbl_Progress.AutoSize = true;
		this.lbl_Progress.Location = new System.Drawing.Point(564, 452);
		this.lbl_Progress.Name = "lbl_Progress";
		this.lbl_Progress.Size = new System.Drawing.Size(35, 13);
		this.lbl_Progress.TabIndex = 11;
		this.lbl_Progress.Text = "label1";
		this.prgBar.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.prgBar.Location = new System.Drawing.Point(344, 449);
		this.prgBar.Name = "prgBar";
		this.prgBar.Size = new System.Drawing.Size(213, 20);
		this.prgBar.TabIndex = 10;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(712, 483);
		base.Controls.Add(this.lbl_Progress);
		base.Controls.Add(this.prgBar);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.grd_Doc);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_Integrated_MapCode";
		this.Text = "Integrated Map Code";
		base.Load += new System.EventHandler(this.frm_Integrated_MapCode_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
