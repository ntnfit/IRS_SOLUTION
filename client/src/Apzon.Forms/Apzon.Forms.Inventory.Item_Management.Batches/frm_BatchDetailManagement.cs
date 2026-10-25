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
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.UserFields;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Global;
using DevExpress.XtraEditors;
using DevExpress.XtraVerticalGrid;
using DevExpress.XtraVerticalGrid.Events;
using DevExpress.XtraVerticalGrid.Rows;

namespace Apzon.Forms.Inventory.Item_Management.Batches;

public class frm_BatchDetailManagement : XtraFormEx
{
	private const string FormController = "BatchDetail";

	private int currentAbsentry;

	private List<UserFieldsImpl> lstUdf = new List<UserFieldsImpl>();

	private IContainer components;

	private VGridControlEx vgrd_BatchDetail;

	private SimpleButton btn_OK;

	private SimpleButton btn_Cancel;

	private ContextMenuStrip ctx_Batch;

	private ToolStripMenuItem batchToolStripMenuItem;

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
			btn_OK.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_OK.Text = Resources.ButtonOK;
			break;
		case Enums.FormViewModeType.Find:
			btn_OK.Text = Resources.ButtonFind;
			break;
		case Enums.FormViewModeType.Add:
			break;
		}
	}

	public frm_BatchDetailManagement()
	{
		InitializeComponent();
	}

	public async Task MOVE_NEXT()
	{
		try
		{
			int type = 3;
			int currentId = currentAbsentry;
			await Navigate(currentId, type);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task MOVE_FIRST()
	{
		try
		{
			int type = 1;
			int currentId = currentAbsentry;
			await Navigate(currentId, type);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task MOVE_PREVIOUS()
	{
		try
		{
			int type = 2;
			int currentId = currentAbsentry;
			await Navigate(currentId, type);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task MOVE_LAST()
	{
		try
		{
			int type = 4;
			int currentId = currentAbsentry;
			await Navigate(currentId, type);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task MODE_FIND()
	{
		try
		{
			int type = 5;
			int currentId = 0;
			await Navigate(currentId, type);
			vgrd_BatchDetail.Rows.GetRowByFieldName("ItemCode").Properties.ReadOnly = false;
			vgrd_BatchDetail.Rows.GetRowByFieldName("DistNumber").Properties.ReadOnly = false;
			FormMode = Enums.FormViewModeType.Find;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			int type = 5;
			int currentId = currentAbsentry;
			await Navigate(currentId, type);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task Navigate(int currentId, int type)
	{
		WinCommons._OpenProcessing();
		try
		{
			if (FormMode == Enums.FormViewModeType.Find)
			{
				vgrd_BatchDetail.Rows.GetRowByFieldName("ItemCode").Properties.ReadOnly = true;
				vgrd_BatchDetail.Rows.GetRowByFieldName("DistNumber").Properties.ReadOnly = true;
			}
			int num = ((type != 5) ? (await HttpClient.BatchDetails.GetPage(currentId, type, GlobalData.UserName, GlobalData.DefaultBranch)) : currentId);
			currentAbsentry = num;
			DataSet ds = await HttpClient.BatchDetails.GetBatchDetail(currentAbsentry);
			if (ds.IsHasStructure())
			{
				await VGridControlHelper.GenerateVGridControl(vgrd_BatchDetail, ds, lstUdf);
			}
			FormMode = Enums.FormViewModeType.View;
			vgrd_BatchDetail.Refresh();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void frm_BatchDetailManagement_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			vgrd_BatchDetail.ContextMenuStrip = ctx_Batch;
			FormId = ApzObjectTypes.oBatchDetails.ToString();
			FormObjectType = ApzObjectTypes.oBatchDetails;
			vgrd_BatchDetail.RecordCellStyle += vgrd_BatchDetail_RecordCellStyle;
			vgrd_BatchDetail.FormId = ApzObjectTypes.oBatchDetails.ToString();
			vgrd_BatchDetail.ItemId = "OBTN";
			vgrd_BatchDetail.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
			lstUdf = await HttpClient.Udf.GetListField("OBTN");
			await MODE_FIND();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		_ = 4;
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else if (FormMode == Enums.FormViewModeType.Find)
			{
				foreach (Form openForm in Application.OpenForms)
				{
					if (!openForm.IsDisposed && !openForm.Disposing && openForm.GetType() == typeof(frm_CFL) && openForm.Text.Equals("List of Batch Numbers Master Data"))
					{
						openForm.Activate();
						return;
					}
				}
				WinCommons._OpenProcessing();
				BaseRow rowByFieldName = vgrd_BatchDetail.Rows.GetRowByFieldName("ItemCode");
				BaseRow rowByFieldName2 = vgrd_BatchDetail.Rows.GetRowByFieldName("DistNumber");
				BaseRow rowByFieldName3 = vgrd_BatchDetail.Rows.GetRowByFieldName("Status");
				BaseRow rowByFieldName4 = vgrd_BatchDetail.Rows.GetRowByFieldName("MnfSerial");
				BaseRow rowByFieldName5 = vgrd_BatchDetail.Rows.GetRowByFieldName("LotNumber");
				string text = Function.ToString(vgrd_BatchDetail.GetCellValue(rowByFieldName, 0));
				string text2 = Function.ToString(vgrd_BatchDetail.GetCellValue(rowByFieldName2, 0));
				string text3 = Function.ToString(vgrd_BatchDetail.GetCellValue(rowByFieldName3, 0));
				string text4 = Function.ToString(vgrd_BatchDetail.GetCellValue(rowByFieldName4, 0));
				string text5 = Function.ToString(vgrd_BatchDetail.GetCellValue(rowByFieldName5, 0));
				if (string.IsNullOrEmpty(text) && string.IsNullOrEmpty(text2) && string.IsNullOrEmpty(text3) && string.IsNullOrEmpty(text4) && string.IsNullOrEmpty(text5))
				{
					return;
				}
				DataSet ds = await HttpClient.BatchDetails.GetBatchDetail(-1, text, text3, text2, text4, text5);
				if (ds.IsHasStructure())
				{
					await VGridControlHelper.GenerateVGridControl(vgrd_BatchDetail, ds, lstUdf);
				}
				if (!ds.IsHasStructure() || !ds.Tables[1].IsNotNull())
				{
					return;
				}
				if (ds.Tables[1].Rows.Count > 1)
				{
					frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.UserDefinedData, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
					frm_CFL2.Text = "List of Batch Numbers Master Data";
					frm_CFL2.DataSource = ds;
					frm_CFL2.ShowDialog(this);
					DataRow chooseItem = frm_CFL2.ChooseItem;
					if (chooseItem != null && chooseItem.IsExistsColumn("AbsEntry"))
					{
						currentAbsentry = Function.ParseInt(chooseItem["AbsEntry"]);
					}
				}
				else
				{
					currentAbsentry = Function.ParseInt(ds.Tables[1].Rows[0]["AbsEntry"]);
				}
				if (currentAbsentry > 0)
				{
					await Navigate(currentAbsentry, 5);
					FormMode = Enums.FormViewModeType.View;
				}
			}
			else
			{
				if (FormMode != Enums.FormViewModeType.Update)
				{
					return;
				}
				DataTable dataTable = vgrd_BatchDetail.DataSourceTable();
				if (dataTable.IsNotNull() && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					WinCommons._OpenProcessing();
					DataSet ds = new DataSet
					{
						Tables = { dataTable.Copy() }
					};
					string text6 = await HttpClient.BatchDetails.Save(ds);
					if (string.IsNullOrEmpty(text6))
					{
						await REFRESH_DATA();
						WinCommons.Success(Enums.MessageCode.Success, "");
						FormMode = Enums.FormViewModeType.View;
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, text6);
					}
					ds.DisposeDataSet();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
	}

	private void vgrd_BatchDetail_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Find)
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

	private void vgrd_BatchDetail_RecordCellStyle(object sender, GetCustomRowCellStyleEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Find && (e.Row.Properties.FieldName.Equals("DistNumber") || e.Row.Properties.FieldName.Equals("ItemCode") || e.Row.Properties.FieldName.Equals("Status") || e.Row.Properties.FieldName.Equals("MnfSerial") || e.Row.Properties.FieldName.Equals("LotNumber")))
			{
				e.Appearance.BackColor = Color.FromArgb(250, 221, 122);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void batchToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			string id = Function.ToString(vgrd_BatchDetail.GetCellValue(vgrd_BatchDetail.Rows.GetRowByFieldName("DistNumber"), vgrd_BatchDetail.FocusedRecord));
			this.DrillDown(ApzObjectTypes.oBatchDetailsTransactionReport, ApzObjectTypes.oBatchDetailsTransactionReport, id);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void vgrd_BatchDetail_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
	{
		try
		{
			batchToolStripMenuItem.Visible = FormMode != Enums.FormViewModeType.Find;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Inventory.Item_Management.Batches.frm_BatchDetailManagement));
		this.vgrd_BatchDetail = new Apzon.Controls.Grid.VGridControlEx();
		this.btn_OK = new DevExpress.XtraEditors.SimpleButton();
		this.btn_Cancel = new DevExpress.XtraEditors.SimpleButton();
		this.ctx_Batch = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.batchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		((System.ComponentModel.ISupportInitialize)this.vgrd_BatchDetail).BeginInit();
		this.ctx_Batch.SuspendLayout();
		base.SuspendLayout();
		this.vgrd_BatchDetail.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.vgrd_BatchDetail.Cursor = System.Windows.Forms.Cursors.Default;
		this.vgrd_BatchDetail.FormId = "oBatchDetailManagement";
		this.vgrd_BatchDetail.ItemId = "ODATA";
		this.vgrd_BatchDetail.Location = new System.Drawing.Point(0, 0);
		this.vgrd_BatchDetail.Name = "vgrd_BatchDetail";
		this.vgrd_BatchDetail.ObjectType = Apzon.Commons.ApzObjectTypes.oNone;
		this.vgrd_BatchDetail.RecordWidth = 250;
		this.vgrd_BatchDetail.RowHeaderWidth = 150;
		this.vgrd_BatchDetail.Size = new System.Drawing.Size(405, 470);
		this.vgrd_BatchDetail.TabIndex = 0;
		this.vgrd_BatchDetail.CellValueChanged += new DevExpress.XtraVerticalGrid.Events.CellValueChangedEventHandler(this.vgrd_BatchDetail_CellValueChanged);
		this.vgrd_BatchDetail.PopupMenuShowing += new DevExpress.XtraVerticalGrid.Events.PopupMenuShowingEventHandler(this.vgrd_BatchDetail_PopupMenuShowing);
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.Location = new System.Drawing.Point(5, 480);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 1;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.Location = new System.Drawing.Point(84, 480);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 2;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.ctx_Batch.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.batchToolStripMenuItem });
		this.ctx_Batch.Name = "contextMenuStrip1";
		this.ctx_Batch.Size = new System.Drawing.Size(258, 26);
		this.batchToolStripMenuItem.Name = "batchToolStripMenuItem";
		this.batchToolStripMenuItem.Size = new System.Drawing.Size(257, 22);
		this.batchToolStripMenuItem.Text = "Batch Number Transactions Report";
		this.batchToolStripMenuItem.Click += new System.EventHandler(this.batchToolStripMenuItem_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(404, 511);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.vgrd_BatchDetail);
		base.FormId = "oBatchDetailManagement";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_BatchDetailManagement";
		base.ShowInTaskbar = false;
		this.Text = "Batch Details";
		base.Load += new System.EventHandler(this.frm_BatchDetailManagement_Load);
		((System.ComponentModel.ISupportInitialize)this.vgrd_BatchDetail).EndInit();
		this.ctx_Batch.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
