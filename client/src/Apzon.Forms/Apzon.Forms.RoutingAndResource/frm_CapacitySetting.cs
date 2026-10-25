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
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraTab;

namespace Apzon.Forms.RoutingAndResource;

public class frm_CapacitySetting : XtraFormEx
{
	private IContainer components;

	private XtraTabControlEx xtraTabSetting;

	private XtraTabPageEx xtraPageGeneral;

	private LookUpEditEx txtDragType;

	private LabelLine lbDragType;

	private SimpleButtonEx btnSave;

	private SimpleButtonEx btnCancel;

	private XtraTabPageEx xtraPageTooltipSetting;

	private GridControlEx grdDoc;

	private GridViewEx gvDoc;

	private LabelLine lbEx5;

	private LabelLine lbEx4;

	private LabelLine lbEx3;

	private LabelLine lbEx2;

	private LabelLine lbEx1;

	private GridColumnEx gcColName;

	private RepositoryItemLookUpEdit rColName;

	private GridColumnEx gcColText;

	private GridColumnEx gcVisOrder;

	private RepositoryItemCalcEdit rcal;

	private LabelLine lbExension;

	private CalcEditEx txtRsPerPag;

	private CalcEditEx txtTimeSpaWdth;

	private CalcEditEx txtApHeight;

	private LabelLine lbResourceHeight;

	private LabelLine lbTimeSpaWidth;

	private LabelLine lbApWidth;

	private LabelLine lbSizeSetting;

	private LabelLine lbColorSetting;

	private ColorEditEx txtClosePoColor;

	private ColorEditEx txtOpenPoColor;

	private ColorEditEx txtCompColor;

	private ColorEditEx txtNotCopColor;

	private LabelLine lbClosePoColor;

	private LabelLine lbOpePoColor;

	private LabelLine lbCompColor;

	private LabelLine lbNotCopColor;

	private MemoExEditEx txtEx1Qry;

	private MemoExEditEx txtEx2Qry;

	private MemoExEditEx txtEx3Qry;

	private MemoExEditEx txtEx4Qry;

	private MemoExEditEx txtEx5Qry;

	private ContextMenuStrip contextMenuStripTooltip;

	private ToolStripMenuItem addRowToolStripMenuItem;

	private ToolStripMenuItem insertRowToolStripMenuItem;

	private ToolStripMenuItem deleteRowsToolStripMenuItem;

	private ColorEditEx txtCancelPo;

	private LabelLine lbCancel;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			btnCancel.Enabled = true;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
		case Enums.FormViewModeType.Add:
			btnSave.Text = "Save";
			break;
		case Enums.FormViewModeType.View:
			btnSave.Text = "OK";
			break;
		case Enums.FormViewModeType.Busy:
			break;
		}
	}

	public frm_CapacitySetting()
	{
		InitializeComponent();
	}

	private void frm_CapacitySetting_Load(object sender, EventArgs e)
	{
		try
		{
			InitDataInput();
			LoadDataSetting();
			FormMode = Enums.FormViewModeType.View;
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

	private void btnSave_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			DataTable dataTable = DataSource as DataTable;
			DataTable dataTable2 = grdDoc.DataSourceTable.Copy();
			if (!dataTable.IsNotNull() || !dataTable2.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.SaveDataCanNotBeNull, "");
				return;
			}
			dataTable.TableName = "General";
			dataTable2.TableName = "tooltip";
			DataSet dataSet = new DataSet();
			for (int i = 0; i < dataTable2.Rows.Count; i++)
			{
				if (string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[i]["ColName"])))
				{
					dataTable2.Rows.RemoveAt(i);
					i--;
				}
			}
			for (int j = 0; j < dataTable2.Rows.Count; j++)
			{
				dataTable2.Rows[j]["VisOrder"] = j;
			}
			dataTable2.AcceptChanges();
			dataSet.Tables.AddRange(new DataTable[2]
			{
				dataTable.Copy(),
				dataTable2
			});
			HttpResult httpResult = HttpClient.CapacitySettingClient.SaveData(dataSet);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				return;
			}
			WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void frm_CapacitySetting_OnControlValueChanged(object sender, ControlChangedEventArgs e)
	{
		try
		{
			if (!IsSetDataSouceControl && FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Add;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void addRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grdDoc.DataSourceTable;
			DataRow dataRow = dataSourceTable.NewRow();
			dataRow["DocEntry"] = 1;
			dataRow["LineNum"] = GetNewLineNum(dataSourceTable);
			dataSourceTable.Rows.Add(dataRow);
			dataSourceTable.AcceptChanges();
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Add;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void insertRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grdDoc.DataSourceTable;
			DataRow dataRow = dataSourceTable.NewRow();
			dataRow["DocEntry"] = 1;
			dataRow["LineNum"] = GetNewLineNum(dataSourceTable);
			dataSourceTable.Rows.InsertAt(dataRow, gvDoc.GetFocusedDataSourceRowIndex());
			dataSourceTable.AcceptChanges();
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Add;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void deleteRowsToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteSelectedRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				List<DataRow> listDataSourceSelectedRow = gvDoc.GetListDataSourceSelectedRow();
				DataTable dataSourceTable = grdDoc.DataSourceTable;
				for (int i = 0; i < listDataSourceSelectedRow.Count; i++)
				{
					dataSourceTable.Rows.Remove(listDataSourceSelectedRow[i]);
				}
				grdDoc.DataSource = dataSourceTable;
				grdDoc.DataSourceTable.AcceptChanges();
				grdDoc.RefreshDataSource();
				if (FormMode == Enums.FormViewModeType.View)
				{
					FormMode = Enums.FormViewModeType.Add;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvDoc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		if (!IsSetDataSouceControl && FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Add;
		}
	}

	private void InitDataInput()
	{
		List<Common.Table_Code> list = new List<Common.Table_Code>();
		foreach (object value in Enum.GetValues(typeof(DragType)))
		{
			list.Add(new Common.Table_Code
			{
				Code = Function.ToString((int)value),
				Name = Function.ToString(value)
			});
		}
		txtDragType.Properties.DataSource = list;
		rColName.DataSource = HttpClient.CapacitySettingClient.GetColumnSetting();
	}

	private void LoadDataSetting()
	{
		try
		{
			DataSet dataSetting = HttpClient.CapacitySettingClient.GetDataSetting();
			if (dataSetting == null || dataSetting.Tables.Count != 2)
			{
				WinCommons.Error(Enums.MessageCode.CannotLoadDataSetting, "");
				return;
			}
			if (!dataSetting.Tables[0].IsNotNull() || !dataSetting.Tables[1].IsNotNull())
			{
				DataRow dataRow = dataSetting.Tables[0].NewRow();
				dataRow["DocEntry"] = 1;
				dataRow["DragType"] = "F";
				dataRow["NotCompColor"] = -1;
				dataRow["CompColor"] = -16776961;
				dataRow["GenPoColor"] = -64;
				dataRow["CloseColor"] = -8355712;
				dataRow["CloseColor"] = -16777216;
				dataRow["ApWdth"] = 20;
				dataRow["TlvSpaWdth"] = 2;
				dataRow["RsPerPag"] = 8;
				dataSetting.Tables[0].Rows.Add(dataRow);
				dataSetting.Tables[0].AcceptChanges();
				DataTable dataTable = dataSetting.Tables[1];
				DataRow dataRow2 = dataTable.NewRow();
				dataRow2["DocEntry"] = 1;
				dataRow2["LineNum"] = 0;
				dataRow2["ColName"] = "ItemName";
				dataRow2["ColText"] = "Item Name";
				dataRow2["VisOrder"] = 0;
				dataTable.Rows.Add(dataRow2);
				dataRow2 = dataTable.NewRow();
				dataRow2["DocEntry"] = 1;
				dataRow2["LineNum"] = 1;
				dataRow2["ColName"] = "OpeName";
				dataRow2["ColText"] = "Operation";
				dataRow2["VisOrder"] = 1;
				dataTable.Rows.Add(dataRow2);
				dataRow2 = dataTable.NewRow();
				dataRow2["DocEntry"] = 1;
				dataRow2["LineNum"] = 2;
				dataRow2["ColName"] = "Shift";
				dataRow2["ColText"] = "Work Shift";
				dataRow2["VisOrder"] = 2;
				dataTable.Rows.Add(dataRow2);
				dataRow2 = dataTable.NewRow();
				dataRow2["DocEntry"] = 1;
				dataRow2["LineNum"] = 3;
				dataRow2["ColName"] = "Qty";
				dataRow2["ColText"] = "Quantity";
				dataRow2["VisOrder"] = 3;
				dataTable.Rows.Add(dataRow2);
				dataRow2 = dataTable.NewRow();
				dataRow2["DocEntry"] = 1;
				dataRow2["LineNum"] = 4;
				dataRow2["ColName"] = "TimeRun";
				dataRow2["ColText"] = "Quantity";
				dataRow2["VisOrder"] = 4;
				dataTable.Rows.Add(dataRow2);
			}
			IsSetDataSouceControl = true;
			DataSource = dataSetting.Tables[0];
			grdDoc.DataSource = dataSetting.Tables[1];
			IsSetDataSouceControl = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private int GetNewLineNum(DataTable dtTable)
	{
		if (!dtTable.IsNotNull())
		{
			return 0;
		}
		return dtTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["LineNum"])) + 1;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.RoutingAndResource.frm_CapacitySetting));
		this.xtraTabSetting = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraPageGeneral = new Apzon.Controls.Controls.XtraTabPageEx();
		this.txtCancelPo = new Apzon.Controls.Editor.ColorEditEx();
		this.lbCancel = new Apzon.Controls.Controls.LabelLine();
		this.txtRsPerPag = new Apzon.Controls.Editor.CalcEditEx();
		this.txtTimeSpaWdth = new Apzon.Controls.Editor.CalcEditEx();
		this.txtApHeight = new Apzon.Controls.Editor.CalcEditEx();
		this.lbResourceHeight = new Apzon.Controls.Controls.LabelLine();
		this.lbTimeSpaWidth = new Apzon.Controls.Controls.LabelLine();
		this.lbApWidth = new Apzon.Controls.Controls.LabelLine();
		this.lbSizeSetting = new Apzon.Controls.Controls.LabelLine();
		this.lbColorSetting = new Apzon.Controls.Controls.LabelLine();
		this.txtClosePoColor = new Apzon.Controls.Editor.ColorEditEx();
		this.txtOpenPoColor = new Apzon.Controls.Editor.ColorEditEx();
		this.txtCompColor = new Apzon.Controls.Editor.ColorEditEx();
		this.txtNotCopColor = new Apzon.Controls.Editor.ColorEditEx();
		this.lbClosePoColor = new Apzon.Controls.Controls.LabelLine();
		this.lbOpePoColor = new Apzon.Controls.Controls.LabelLine();
		this.lbCompColor = new Apzon.Controls.Controls.LabelLine();
		this.lbNotCopColor = new Apzon.Controls.Controls.LabelLine();
		this.lbExension = new Apzon.Controls.Controls.LabelLine();
		this.lbEx5 = new Apzon.Controls.Controls.LabelLine();
		this.lbEx4 = new Apzon.Controls.Controls.LabelLine();
		this.lbEx3 = new Apzon.Controls.Controls.LabelLine();
		this.lbEx2 = new Apzon.Controls.Controls.LabelLine();
		this.lbEx1 = new Apzon.Controls.Controls.LabelLine();
		this.txtDragType = new Apzon.Controls.Editor.LookUpEditEx();
		this.lbDragType = new Apzon.Controls.Controls.LabelLine();
		this.txtEx1Qry = new Apzon.Controls.Editor.MemoExEditEx();
		this.txtEx2Qry = new Apzon.Controls.Editor.MemoExEditEx();
		this.txtEx3Qry = new Apzon.Controls.Editor.MemoExEditEx();
		this.txtEx4Qry = new Apzon.Controls.Editor.MemoExEditEx();
		this.txtEx5Qry = new Apzon.Controls.Editor.MemoExEditEx();
		this.xtraPageTooltipSetting = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStripTooltip = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.insertRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteRowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.gcColName = new Apzon.Controls.Grid.GridColumnEx();
		this.rColName = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcColText = new Apzon.Controls.Grid.GridColumnEx();
		this.gcVisOrder = new Apzon.Controls.Grid.GridColumnEx();
		this.rcal = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.btnSave = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.xtraTabSetting).BeginInit();
		this.xtraTabSetting.SuspendLayout();
		this.xtraPageGeneral.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txtCancelPo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtRsPerPag.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtTimeSpaWdth.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtApHeight.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtClosePoColor.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtOpenPoColor.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtCompColor.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtNotCopColor.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtDragType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtEx1Qry.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtEx2Qry.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtEx3Qry.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtEx4Qry.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtEx5Qry.Properties).BeginInit();
		this.xtraPageTooltipSetting.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		this.contextMenuStripTooltip.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rColName).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rcal).BeginInit();
		base.SuspendLayout();
		this.xtraTabSetting.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.xtraTabSetting.IsSearchControl = false;
		this.xtraTabSetting.Location = new System.Drawing.Point(1, 2);
		this.xtraTabSetting.Name = "xtraTabSetting";
		this.xtraTabSetting.SelectedTabPage = this.xtraPageGeneral;
		this.xtraTabSetting.Size = new System.Drawing.Size(503, 457);
		this.xtraTabSetting.TabIndex = 0;
		this.xtraTabSetting.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.xtraPageGeneral, this.xtraPageTooltipSetting });
		this.xtraPageGeneral.Controls.Add(this.txtCancelPo);
		this.xtraPageGeneral.Controls.Add(this.lbCancel);
		this.xtraPageGeneral.Controls.Add(this.txtRsPerPag);
		this.xtraPageGeneral.Controls.Add(this.txtTimeSpaWdth);
		this.xtraPageGeneral.Controls.Add(this.txtApHeight);
		this.xtraPageGeneral.Controls.Add(this.lbResourceHeight);
		this.xtraPageGeneral.Controls.Add(this.lbTimeSpaWidth);
		this.xtraPageGeneral.Controls.Add(this.lbApWidth);
		this.xtraPageGeneral.Controls.Add(this.lbSizeSetting);
		this.xtraPageGeneral.Controls.Add(this.lbColorSetting);
		this.xtraPageGeneral.Controls.Add(this.txtClosePoColor);
		this.xtraPageGeneral.Controls.Add(this.txtOpenPoColor);
		this.xtraPageGeneral.Controls.Add(this.txtCompColor);
		this.xtraPageGeneral.Controls.Add(this.txtNotCopColor);
		this.xtraPageGeneral.Controls.Add(this.lbClosePoColor);
		this.xtraPageGeneral.Controls.Add(this.lbOpePoColor);
		this.xtraPageGeneral.Controls.Add(this.lbCompColor);
		this.xtraPageGeneral.Controls.Add(this.lbNotCopColor);
		this.xtraPageGeneral.Controls.Add(this.lbExension);
		this.xtraPageGeneral.Controls.Add(this.lbEx5);
		this.xtraPageGeneral.Controls.Add(this.lbEx4);
		this.xtraPageGeneral.Controls.Add(this.lbEx3);
		this.xtraPageGeneral.Controls.Add(this.lbEx2);
		this.xtraPageGeneral.Controls.Add(this.lbEx1);
		this.xtraPageGeneral.Controls.Add(this.txtDragType);
		this.xtraPageGeneral.Controls.Add(this.lbDragType);
		this.xtraPageGeneral.Controls.Add(this.txtEx1Qry);
		this.xtraPageGeneral.Controls.Add(this.txtEx2Qry);
		this.xtraPageGeneral.Controls.Add(this.txtEx3Qry);
		this.xtraPageGeneral.Controls.Add(this.txtEx4Qry);
		this.xtraPageGeneral.Controls.Add(this.txtEx5Qry);
		this.xtraPageGeneral.FieldName = "";
		this.xtraPageGeneral.IsSearchControl = false;
		this.xtraPageGeneral.Name = "xtraPageGeneral";
		this.xtraPageGeneral.Size = new System.Drawing.Size(501, 428);
		this.xtraPageGeneral.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraPageGeneral.Text = "General setting";
		this.txtCancelPo.EditValue = System.Drawing.Color.Empty;
		this.txtCancelPo.FieldName = "CancelColor";
		this.txtCancelPo.IsSearchControl = false;
		this.txtCancelPo.Location = new System.Drawing.Point(204, 295);
		this.txtCancelPo.Name = "txtCancelPo";
		this.txtCancelPo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtCancelPo.Properties.StoreColorAsInteger = true;
		this.txtCancelPo.Size = new System.Drawing.Size(100, 20);
		this.txtCancelPo.TabIndex = 11;
		this.lbCancel.FieldName = null;
		this.lbCancel.HideWhenEmpty = false;
		this.lbCancel.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbCancel.Location = new System.Drawing.Point(94, 298);
		this.lbCancel.Name = "lbCancel";
		this.lbCancel.ShowUnderLine = true;
		this.lbCancel.Size = new System.Drawing.Size(110, 20);
		this.lbCancel.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbCancel.TabIndex = 0;
		this.lbCancel.Utext = "Cancel Production";
		this.txtRsPerPag.FieldName = "RsPerPag";
		this.txtRsPerPag.IsSearchControl = false;
		this.txtRsPerPag.Location = new System.Drawing.Point(204, 399);
		this.txtRsPerPag.Name = "txtRsPerPag";
		this.txtRsPerPag.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtRsPerPag.Properties.IsSearchControl = false;
		this.txtRsPerPag.Size = new System.Drawing.Size(100, 20);
		this.txtRsPerPag.TabIndex = 14;
		this.txtTimeSpaWdth.FieldName = "TlvSpaWdth";
		this.txtTimeSpaWdth.IsSearchControl = false;
		this.txtTimeSpaWdth.Location = new System.Drawing.Point(204, 376);
		this.txtTimeSpaWdth.Name = "txtTimeSpaWdth";
		this.txtTimeSpaWdth.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtTimeSpaWdth.Properties.IsSearchControl = false;
		this.txtTimeSpaWdth.Size = new System.Drawing.Size(100, 20);
		this.txtTimeSpaWdth.TabIndex = 13;
		this.txtApHeight.FieldName = "ApHeight";
		this.txtApHeight.IsSearchControl = false;
		this.txtApHeight.Location = new System.Drawing.Point(204, 353);
		this.txtApHeight.Name = "txtApHeight";
		this.txtApHeight.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtApHeight.Properties.IsSearchControl = false;
		this.txtApHeight.Size = new System.Drawing.Size(100, 20);
		this.txtApHeight.TabIndex = 12;
		this.lbResourceHeight.FieldName = null;
		this.lbResourceHeight.HideWhenEmpty = false;
		this.lbResourceHeight.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbResourceHeight.Location = new System.Drawing.Point(94, 402);
		this.lbResourceHeight.Name = "lbResourceHeight";
		this.lbResourceHeight.ShowUnderLine = true;
		this.lbResourceHeight.Size = new System.Drawing.Size(110, 20);
		this.lbResourceHeight.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbResourceHeight.TabIndex = 107;
		this.lbResourceHeight.Utext = "Resource Per Page";
		this.lbTimeSpaWidth.FieldName = null;
		this.lbTimeSpaWidth.HideWhenEmpty = false;
		this.lbTimeSpaWidth.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbTimeSpaWidth.Location = new System.Drawing.Point(94, 379);
		this.lbTimeSpaWidth.Name = "lbTimeSpaWidth";
		this.lbTimeSpaWidth.ShowUnderLine = true;
		this.lbTimeSpaWidth.Size = new System.Drawing.Size(110, 20);
		this.lbTimeSpaWidth.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbTimeSpaWidth.TabIndex = 108;
		this.lbTimeSpaWidth.Utext = "Time Space Width";
		this.lbApWidth.FieldName = null;
		this.lbApWidth.HideWhenEmpty = false;
		this.lbApWidth.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbApWidth.Location = new System.Drawing.Point(94, 356);
		this.lbApWidth.Name = "lbApWidth";
		this.lbApWidth.ShowUnderLine = true;
		this.lbApWidth.Size = new System.Drawing.Size(110, 20);
		this.lbApWidth.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbApWidth.TabIndex = 109;
		this.lbApWidth.Utext = "Appointment Height";
		this.lbSizeSetting.FieldName = null;
		this.lbSizeSetting.HideWhenEmpty = false;
		this.lbSizeSetting.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbSizeSetting.Location = new System.Drawing.Point(45, 322);
		this.lbSizeSetting.Name = "lbSizeSetting";
		this.lbSizeSetting.ShowUnderLine = true;
		this.lbSizeSetting.Size = new System.Drawing.Size(152, 20);
		this.lbSizeSetting.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbSizeSetting.TabIndex = 110;
		this.lbSizeSetting.Utext = "Size Setting";
		this.lbColorSetting.FieldName = null;
		this.lbColorSetting.HideWhenEmpty = false;
		this.lbColorSetting.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbColorSetting.Location = new System.Drawing.Point(45, 177);
		this.lbColorSetting.Name = "lbColorSetting";
		this.lbColorSetting.ShowUnderLine = true;
		this.lbColorSetting.Size = new System.Drawing.Size(152, 20);
		this.lbColorSetting.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbColorSetting.TabIndex = 111;
		this.lbColorSetting.Utext = "Color Setting";
		this.txtClosePoColor.EditValue = System.Drawing.Color.Empty;
		this.txtClosePoColor.FieldName = "CloseColor";
		this.txtClosePoColor.IsSearchControl = false;
		this.txtClosePoColor.Location = new System.Drawing.Point(204, 272);
		this.txtClosePoColor.Name = "txtClosePoColor";
		this.txtClosePoColor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtClosePoColor.Properties.StoreColorAsInteger = true;
		this.txtClosePoColor.Size = new System.Drawing.Size(100, 20);
		this.txtClosePoColor.TabIndex = 10;
		this.txtOpenPoColor.EditValue = System.Drawing.Color.Empty;
		this.txtOpenPoColor.FieldName = "GenPoColor";
		this.txtOpenPoColor.IsSearchControl = false;
		this.txtOpenPoColor.Location = new System.Drawing.Point(204, 249);
		this.txtOpenPoColor.Name = "txtOpenPoColor";
		this.txtOpenPoColor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtOpenPoColor.Properties.StoreColorAsInteger = true;
		this.txtOpenPoColor.Size = new System.Drawing.Size(100, 20);
		this.txtOpenPoColor.TabIndex = 9;
		this.txtCompColor.EditValue = System.Drawing.Color.Empty;
		this.txtCompColor.FieldName = "CompColor";
		this.txtCompColor.IsSearchControl = false;
		this.txtCompColor.Location = new System.Drawing.Point(204, 226);
		this.txtCompColor.Name = "txtCompColor";
		this.txtCompColor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtCompColor.Properties.StoreColorAsInteger = true;
		this.txtCompColor.Size = new System.Drawing.Size(100, 20);
		this.txtCompColor.TabIndex = 8;
		this.txtNotCopColor.EditValue = System.Drawing.Color.Empty;
		this.txtNotCopColor.FieldName = "NotCompColor";
		this.txtNotCopColor.IsSearchControl = false;
		this.txtNotCopColor.Location = new System.Drawing.Point(204, 203);
		this.txtNotCopColor.Name = "txtNotCopColor";
		this.txtNotCopColor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtNotCopColor.Properties.StoreColorAsInteger = true;
		this.txtNotCopColor.Size = new System.Drawing.Size(100, 20);
		this.txtNotCopColor.TabIndex = 7;
		this.lbClosePoColor.FieldName = null;
		this.lbClosePoColor.HideWhenEmpty = false;
		this.lbClosePoColor.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbClosePoColor.Location = new System.Drawing.Point(94, 275);
		this.lbClosePoColor.Name = "lbClosePoColor";
		this.lbClosePoColor.ShowUnderLine = true;
		this.lbClosePoColor.Size = new System.Drawing.Size(110, 20);
		this.lbClosePoColor.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbClosePoColor.TabIndex = 112;
		this.lbClosePoColor.Utext = "Close Production";
		this.lbOpePoColor.FieldName = null;
		this.lbOpePoColor.HideWhenEmpty = false;
		this.lbOpePoColor.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbOpePoColor.Location = new System.Drawing.Point(94, 252);
		this.lbOpePoColor.Name = "lbOpePoColor";
		this.lbOpePoColor.ShowUnderLine = true;
		this.lbOpePoColor.Size = new System.Drawing.Size(110, 20);
		this.lbOpePoColor.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbOpePoColor.TabIndex = 113;
		this.lbOpePoColor.Utext = "Open Production";
		this.lbCompColor.FieldName = null;
		this.lbCompColor.HideWhenEmpty = false;
		this.lbCompColor.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbCompColor.Location = new System.Drawing.Point(94, 229);
		this.lbCompColor.Name = "lbCompColor";
		this.lbCompColor.ShowUnderLine = true;
		this.lbCompColor.Size = new System.Drawing.Size(110, 20);
		this.lbCompColor.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbCompColor.TabIndex = 114;
		this.lbCompColor.Utext = "Complete";
		this.lbNotCopColor.FieldName = null;
		this.lbNotCopColor.HideWhenEmpty = false;
		this.lbNotCopColor.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbNotCopColor.Location = new System.Drawing.Point(94, 206);
		this.lbNotCopColor.Name = "lbNotCopColor";
		this.lbNotCopColor.ShowUnderLine = true;
		this.lbNotCopColor.Size = new System.Drawing.Size(110, 20);
		this.lbNotCopColor.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbNotCopColor.TabIndex = 115;
		this.lbNotCopColor.Utext = "Not Complete";
		this.lbExension.FieldName = null;
		this.lbExension.HideWhenEmpty = false;
		this.lbExension.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbExension.Location = new System.Drawing.Point(45, 11);
		this.lbExension.Name = "lbExension";
		this.lbExension.ShowUnderLine = true;
		this.lbExension.Size = new System.Drawing.Size(157, 20);
		this.lbExension.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbExension.TabIndex = 11;
		this.lbExension.Utext = "Extension";
		this.lbEx5.FieldName = null;
		this.lbEx5.HideWhenEmpty = false;
		this.lbEx5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbEx5.Location = new System.Drawing.Point(94, 152);
		this.lbEx5.Name = "lbEx5";
		this.lbEx5.ShowUnderLine = true;
		this.lbEx5.Size = new System.Drawing.Size(110, 20);
		this.lbEx5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbEx5.TabIndex = 0;
		this.lbEx5.Utext = "Extension 5";
		this.lbEx4.FieldName = null;
		this.lbEx4.HideWhenEmpty = false;
		this.lbEx4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbEx4.Location = new System.Drawing.Point(94, 129);
		this.lbEx4.Name = "lbEx4";
		this.lbEx4.ShowUnderLine = true;
		this.lbEx4.Size = new System.Drawing.Size(110, 20);
		this.lbEx4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbEx4.TabIndex = 0;
		this.lbEx4.Utext = "Extension 4";
		this.lbEx3.FieldName = null;
		this.lbEx3.HideWhenEmpty = false;
		this.lbEx3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbEx3.Location = new System.Drawing.Point(94, 106);
		this.lbEx3.Name = "lbEx3";
		this.lbEx3.ShowUnderLine = true;
		this.lbEx3.Size = new System.Drawing.Size(110, 20);
		this.lbEx3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbEx3.TabIndex = 0;
		this.lbEx3.Utext = "Extension 3";
		this.lbEx2.FieldName = null;
		this.lbEx2.HideWhenEmpty = false;
		this.lbEx2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbEx2.Location = new System.Drawing.Point(94, 83);
		this.lbEx2.Name = "lbEx2";
		this.lbEx2.ShowUnderLine = true;
		this.lbEx2.Size = new System.Drawing.Size(110, 20);
		this.lbEx2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbEx2.TabIndex = 0;
		this.lbEx2.Utext = "Extension 2";
		this.lbEx1.FieldName = null;
		this.lbEx1.HideWhenEmpty = false;
		this.lbEx1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbEx1.Location = new System.Drawing.Point(94, 60);
		this.lbEx1.Name = "lbEx1";
		this.lbEx1.ShowUnderLine = true;
		this.lbEx1.Size = new System.Drawing.Size(110, 20);
		this.lbEx1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbEx1.TabIndex = 0;
		this.lbEx1.Utext = "Extension 1";
		this.txtDragType.FieldName = "DragType";
		this.txtDragType.IsSearchControl = false;
		this.txtDragType.Location = new System.Drawing.Point(204, 34);
		this.txtDragType.Name = "txtDragType";
		this.txtDragType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtDragType.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.txtDragType.Properties.DisplayMember = "Name";
		this.txtDragType.Properties.NullText = "";
		this.txtDragType.Properties.ShowHeader = false;
		this.txtDragType.Properties.ValueMember = "Code";
		this.txtDragType.Size = new System.Drawing.Size(154, 20);
		this.txtDragType.TabIndex = 1;
		this.lbDragType.FieldName = null;
		this.lbDragType.HideWhenEmpty = false;
		this.lbDragType.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbDragType.Location = new System.Drawing.Point(94, 37);
		this.lbDragType.Name = "lbDragType";
		this.lbDragType.ShowUnderLine = true;
		this.lbDragType.Size = new System.Drawing.Size(110, 20);
		this.lbDragType.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbDragType.TabIndex = 0;
		this.lbDragType.Utext = "Drag Type";
		this.txtEx1Qry.FieldName = "Ex1Qry";
		this.txtEx1Qry.IsSearchControl = false;
		this.txtEx1Qry.Location = new System.Drawing.Point(204, 57);
		this.txtEx1Qry.Name = "txtEx1Qry";
		this.txtEx1Qry.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtEx1Qry.Size = new System.Drawing.Size(154, 20);
		this.txtEx1Qry.TabIndex = 2;
		this.txtEx2Qry.FieldName = "Ex2Qry";
		this.txtEx2Qry.IsSearchControl = false;
		this.txtEx2Qry.Location = new System.Drawing.Point(204, 80);
		this.txtEx2Qry.Name = "txtEx2Qry";
		this.txtEx2Qry.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtEx2Qry.Size = new System.Drawing.Size(154, 20);
		this.txtEx2Qry.TabIndex = 3;
		this.txtEx3Qry.FieldName = "Ex3Qry";
		this.txtEx3Qry.IsSearchControl = false;
		this.txtEx3Qry.Location = new System.Drawing.Point(204, 103);
		this.txtEx3Qry.Name = "txtEx3Qry";
		this.txtEx3Qry.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtEx3Qry.Size = new System.Drawing.Size(154, 20);
		this.txtEx3Qry.TabIndex = 4;
		this.txtEx4Qry.FieldName = "Ex4Qry";
		this.txtEx4Qry.IsSearchControl = false;
		this.txtEx4Qry.Location = new System.Drawing.Point(204, 126);
		this.txtEx4Qry.Name = "txtEx4Qry";
		this.txtEx4Qry.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtEx4Qry.Size = new System.Drawing.Size(154, 20);
		this.txtEx4Qry.TabIndex = 5;
		this.txtEx5Qry.FieldName = "Ex5Qry";
		this.txtEx5Qry.IsSearchControl = false;
		this.txtEx5Qry.Location = new System.Drawing.Point(204, 149);
		this.txtEx5Qry.Name = "txtEx5Qry";
		this.txtEx5Qry.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtEx5Qry.Size = new System.Drawing.Size(154, 20);
		this.txtEx5Qry.TabIndex = 6;
		this.xtraPageTooltipSetting.Controls.Add(this.grdDoc);
		this.xtraPageTooltipSetting.FieldName = "";
		this.xtraPageTooltipSetting.IsSearchControl = false;
		this.xtraPageTooltipSetting.Name = "xtraPageTooltipSetting";
		this.xtraPageTooltipSetting.Size = new System.Drawing.Size(501, 428);
		this.xtraPageTooltipSetting.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraPageTooltipSetting.Text = "Tooltip Setting";
		this.grdDoc.AllowPasteExcel = false;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.ContextMenuStrip = this.contextMenuStripTooltip;
		this.grdDoc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdDoc.Location = new System.Drawing.Point(0, 0);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rColName, this.rcal });
		this.grdDoc.Size = new System.Drawing.Size(501, 428);
		this.grdDoc.TabIndex = 0;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.contextMenuStripTooltip.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.addRowToolStripMenuItem, this.insertRowToolStripMenuItem, this.deleteRowsToolStripMenuItem });
		this.contextMenuStripTooltip.Name = "contextMenuStripTooltip";
		this.contextMenuStripTooltip.Size = new System.Drawing.Size(139, 70);
		this.addRowToolStripMenuItem.Name = "addRowToolStripMenuItem";
		this.addRowToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
		this.addRowToolStripMenuItem.Text = "Add Row";
		this.addRowToolStripMenuItem.Click += new System.EventHandler(this.addRowToolStripMenuItem_Click);
		this.insertRowToolStripMenuItem.Name = "insertRowToolStripMenuItem";
		this.insertRowToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
		this.insertRowToolStripMenuItem.Text = "Insert Row";
		this.insertRowToolStripMenuItem.Click += new System.EventHandler(this.insertRowToolStripMenuItem_Click);
		this.deleteRowsToolStripMenuItem.Name = "deleteRowsToolStripMenuItem";
		this.deleteRowsToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
		this.deleteRowsToolStripMenuItem.Text = "Delete Rows";
		this.deleteRowsToolStripMenuItem.Click += new System.EventHandler(this.deleteRowsToolStripMenuItem_Click);
		this.gvDoc.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gvDoc.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gcColName, this.gcColText, this.gcVisOrder });
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = "oCapacitySetting";
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 35;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.ItemId = "APZ_OCSI";
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gvDoc.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gvDoc.OptionsPrint.AutoWidth = false;
		this.gvDoc.OptionsPrint.UsePrintStyles = false;
		this.gvDoc.OptionsView.BestFitMaxRowCount = 100;
		this.gvDoc.OptionsView.ColumnAutoWidth = false;
		this.gvDoc.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gvDoc.OptionsView.ShowGroupedColumns = true;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanged);
		this.gcColName.Caption = "Column Name";
		this.gcColName.ColumnEdit = this.rColName;
		this.gcColName.CurrencyDateField = null;
		this.gcColName.CurrencyField = null;
		this.gcColName.CurrencyFixed = null;
		this.gcColName.CurrencyFixedRate = 0.0;
		this.gcColName.CurrencyRateField = null;
		this.gcColName.FieldName = "ColName";
		this.gcColName.Name = "gcColName";
		this.gcColName.PrimaryKey = false;
		this.gcColName.UnitField = null;
		this.gcColName.Visible = true;
		this.gcColName.VisibleIndex = 0;
		this.gcColName.Width = 96;
		this.rColName.AutoHeight = false;
		this.rColName.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rColName.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rColName.DisplayMember = "Name";
		this.rColName.Name = "rColName";
		this.rColName.NullText = "";
		this.rColName.ShowHeader = false;
		this.rColName.ValueMember = "Code";
		this.gcColText.Caption = "Display Text";
		this.gcColText.CurrencyDateField = null;
		this.gcColText.CurrencyField = null;
		this.gcColText.CurrencyFixed = null;
		this.gcColText.CurrencyFixedRate = 0.0;
		this.gcColText.CurrencyRateField = null;
		this.gcColText.FieldName = "ColText";
		this.gcColText.Name = "gcColText";
		this.gcColText.PrimaryKey = false;
		this.gcColText.UnitField = null;
		this.gcColText.Visible = true;
		this.gcColText.VisibleIndex = 1;
		this.gcColText.Width = 183;
		this.gcVisOrder.Caption = "Visible Order";
		this.gcVisOrder.ColumnEdit = this.rcal;
		this.gcVisOrder.CurrencyDateField = null;
		this.gcVisOrder.CurrencyField = null;
		this.gcVisOrder.CurrencyFixed = null;
		this.gcVisOrder.CurrencyFixedRate = 0.0;
		this.gcVisOrder.CurrencyRateField = null;
		this.gcVisOrder.FieldName = "VisOrder";
		this.gcVisOrder.Name = "gcVisOrder";
		this.gcVisOrder.PrimaryKey = false;
		this.gcVisOrder.UnitField = null;
		this.gcVisOrder.Visible = true;
		this.gcVisOrder.VisibleIndex = 2;
		this.rcal.AutoHeight = false;
		this.rcal.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rcal.Name = "rcal";
		this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnSave.FieldName = null;
		this.btnSave.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSave.ImageOptions.Image");
		this.btnSave.IsSearchControl = false;
		this.btnSave.Location = new System.Drawing.Point(344, 460);
		this.btnSave.Name = "btnSave";
		this.btnSave.Size = new System.Drawing.Size(75, 23);
		this.btnSave.TabIndex = 300;
		this.btnSave.Text = "OK";
		this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(425, 460);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 301;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(506, 487);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btnSave);
		base.Controls.Add(this.xtraTabSetting);
		base.FormId = "oCapacitySetting";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_CapacitySetting";
		this.Text = "Capacity Setting Infomation";
		base.OnControlValueChanged += new Apzon.Controls.Form.ControlValueChanged(this.frm_CapacitySetting_OnControlValueChanged);
		base.Load += new System.EventHandler(this.frm_CapacitySetting_Load);
		((System.ComponentModel.ISupportInitialize)this.xtraTabSetting).EndInit();
		this.xtraTabSetting.ResumeLayout(false);
		this.xtraPageGeneral.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txtCancelPo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtRsPerPag.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtTimeSpaWdth.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtApHeight.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtClosePoColor.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtOpenPoColor.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtCompColor.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtNotCopColor.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtDragType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtEx1Qry.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtEx2Qry.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtEx3Qry.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtEx4Qry.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtEx5Qry.Properties).EndInit();
		this.xtraPageTooltipSetting.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		this.contextMenuStripTooltip.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rColName).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rcal).EndInit();
		base.ResumeLayout(false);
	}
}
