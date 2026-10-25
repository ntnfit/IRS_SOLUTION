using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Interfaces.FormInventory;
using Apzon.Forms.UserControls;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Inventory.InventoryTransaction;

public class frm_InventoryTranferRequest : VirFormInventoryTranfer
{
	public frm_InventoryTranferRequest()
	{
		InitContructure();
	}

	public frm_InventoryTranferRequest(string docEntry, ApzObjectTypes fromTypes)
	{
		FromDocEntry = docEntry;
		FromType = fromTypes;
		InitContructure();
	}

	private void InitContructure()
	{
		ChangeLogType = Enums.ChangeLog.clInventoryTransferRequests;
		FormDocumentTable = "OWTQ";
		FormLineTable = "WTQ1";
		FormLayoutCode = "WTQ1";
		FormController = "TranferRequest";
		base.Text = "Inventory Transfer Request";
		FormId = "oInventoryTransferRequest";
		JournalRemarksDefault = "Inventory Transfer Request -";
		ApzObjectTypes objectType = (ucBatchAndSerial1.FormObjectType = ApzObjectTypes.oInventoryTransferRequest);
		ObjectType = objectType;
		ucBatchAndSerial1.TransType = Enums.BatchSerialTransType.OutAlocate;
		gv_Doc.FormId = FormId;
		GridViewEx gridViewEx = gv_Lines;
		ucAttachment ucAttachment2 = ucAttachment1;
		string text = (gv_Logistic.FormId = FormId);
		string formId2 = (ucAttachment2.FormId = text);
		gridViewEx.FormId = formId2;
		gv_Doc.ItemId = "OWTQ";
		gv_Lines.ItemId = "WTQ1";
		gv_Logistic.ItemId = "OLGT";
		ucBatchAndSerial1.FormId = FormId;
		ucBatchAndSerial1.ItemId = "OBTN";
		ctmCancel_Doc.Visible = false;
		ctm_Reverse.Visible = false;
	}

	public override void gv_Lines_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			base.gv_Lines_ShowingEditor(sender, e);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public override void gv_Doc_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add && (gv_Doc.FocusedColumn.FieldName == gcDocCardCode.FieldName || gv_Doc.FocusedColumn.FieldName == gcDocCardName.FieldName || gv_Doc.FocusedColumn.FieldName == gcDocSeries.FieldName || gv_Doc.FocusedColumn.FieldName == gcDocNum.FieldName || gv_Doc.FocusedColumn.FieldName == gcDocStatus.FieldName))
			{
				e.Cancel = true;
			}
			if (FormMode != Enums.FormViewModeType.Add && Function.ToString(gv_Doc.GetFocusedRowCellValue(gcDocStatus)).Equals("C") && !(gv_Doc.FocusedColumn.FieldName == gcDocCntctCode.FieldName) && !(gv_Doc.FocusedColumn.FieldName == gcDocTaxDate.FieldName) && !(gv_Doc.FocusedColumn.FieldName == gcDocComments.FieldName) && !(gv_Doc.FocusedColumn.FieldName == gcDocJournalRemarks.FieldName) && !gv_Doc.FocusedColumn.FieldName.StartsWith("U_"))
			{
				e.Cancel = true;
			}
			base.gv_Doc_ShowingEditor(sender, e);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public override void gv_Doc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			base.gv_Doc_RowCellStyle(sender, e);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override void gv_Lines_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add && Function.ToString(gv_Lines.GetRowCellValue(e.RowHandle, "LineStatus")).Equals("C"))
			{
				e.Appearance.BackColor = Color.Gainsboro;
			}
			base.gv_Lines_RowCellStyle(sender, e);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override void LoadButtonCopy()
	{
		try
		{
			btnCopyFrom.Text = "Copy To";
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(frm_InventoryTranferRequest));
			btnCopyFrom.ImageOptions.Image = (Image)componentResourceManager.GetObject("btnCopyFrom.ImageOptions.Image");
			DXPopupMenu dXPopupMenu = new DXPopupMenu();
			DXMenuItem dXMenuItem = new DXMenuItem();
			dXMenuItem.Caption = "Inventory Transfer";
			dXMenuItem.Click += (object s, EventArgs e) =>
			{
				WinCommons._OpenProcessing();
				try
				{
					LoadCopyTo();
				}
				catch (Exception ex2)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
					WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex2.Message);
				}
				WinCommons._CloseProcessing();
			};
			dXPopupMenu.Items.Add(dXMenuItem);
			btnCopyFrom.DropDownControl = dXPopupMenu;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected override void LoadDocStatusControl()
	{
		try
		{
			base.LoadDocStatusControl();
			List<Common.Table_Code> dataSource = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Name = "All",
					Code = "A"
				},
				new Common.Table_Code
				{
					Name = "Open",
					Code = "O"
				},
				new Common.Table_Code
				{
					Name = "Close",
					Code = "C"
				}
			};
			txtDocStatus.Properties.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadCopyTo()
	{
		try
		{
			if (Function.ToString(gv_Doc.GetFocusedRowCellValue(gcDocStatus)).Equals("C"))
			{
				WinCommons.Error(Enums.MessageCode.ThisDocumentIsClose, "");
				return;
			}
			if (gv_Doc.FocusedRowHandle < 0)
			{
				WinCommons.Error(Enums.MessageCode.NoDocumentToCopyTo, "");
				return;
			}
			frm_InventoryTranfer frm_InventoryTranfer2 = new frm_InventoryTranfer(Function.ToString(gv_Doc.GetFocusedRowCellValue("DocEntry")), ApzObjectTypes.oInventoryTransferRequest);
			frm_InventoryTranfer2.MdiParent = MdiParent;
			frm_InventoryTranfer2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override int CheckDataDocSave()
	{
		if (string.IsNullOrEmpty(Function.ToString(gv_Doc.GetFocusedRowCellValue(gcDocDueDate))))
		{
			WinCommons.Error(Enums.MessageCode.EnterDueDateInventoryTransferRequest, "");
			gv_Doc.FocusedColumn = gv_Doc.Columns[gcDocDueDate.FieldName];
			gv_Doc.ShowEditor();
			return 1;
		}
		return base.CheckDataDocSave();
	}

	protected override void EnableDisableControl(bool isEnable)
	{
		try
		{
			base.EnableDisableControl(isEnable);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override void insertRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (!Function.ToString(gv_Doc.GetFocusedRowCellValue(gcDocStatus)).Equals("C"))
			{
				base.insertRowToolStripMenuItem_Click(sender, e);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override void toolStripMenuItem_AddNewDocLine_Click(object sender, EventArgs e)
	{
		try
		{
			if (!Function.ToString(gv_Doc.GetFocusedRowCellValue(gcDocStatus)).Equals("C"))
			{
				base.toolStripMenuItem_AddNewDocLine_Click(sender, e);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override void deleteToolStripMenuItem_DocLine_Click(object sender, EventArgs e)
	{
		try
		{
			if (!Function.ToString(gv_Doc.GetFocusedRowCellValue(gcDocStatus)).Equals("C"))
			{
				base.deleteToolStripMenuItem_DocLine_Click(sender, e);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override void cmtDuplicate_Line_Click(object sender, EventArgs e)
	{
		try
		{
			if (!Function.ToString(gv_Doc.GetFocusedRowCellValue(gcDocStatus)).Equals("C"))
			{
				base.cmtDuplicate_Line_Click(sender, e);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override void LoadFormView()
	{
		switch (FormMode)
		{
		case Enums.FormViewModeType.View:
		{
			gcDocCardCode.OptionsColumn.ReadOnly = true;
			gcDocCardName.OptionsColumn.ReadOnly = true;
			gcDocSeries.OptionsColumn.ReadOnly = true;
			gcDocNum.OptionsColumn.ReadOnly = true;
			rDocCustomerCode.Buttons[0].Visible = false;
			rDocSeries.Buttons[0].Visible = false;
			cmtDuplicate_Return.Visible = true;
			saveAsDraftToolStripMenuItem.Visible = false;
			saveAsDraftToolStripMenuItem1.Visible = false;
			ToolStripMenuItem toolStripMenuItem3 = ctmCancel_Doc;
			bool visible = (cmtClose_Doc.Visible = true);
			toolStripMenuItem3.Visible = visible;
			btn_Save.Text = "OK";
			btn_Save.ImageOptions.Image = null;
			btn_Save.DropDownArrowStyle = DropDownArrowStyle.Hide;
			break;
		}
		case Enums.FormViewModeType.Update:
		{
			gcDocCardCode.OptionsColumn.ReadOnly = true;
			gcDocCardName.OptionsColumn.ReadOnly = true;
			gcDocSeries.OptionsColumn.ReadOnly = true;
			gcDocNum.OptionsColumn.ReadOnly = true;
			rDocCustomerCode.Buttons[0].Visible = false;
			rDocSeries.Buttons[0].Visible = false;
			cmtDuplicate_Return.Visible = true;
			saveAsDraftToolStripMenuItem.Visible = false;
			saveAsDraftToolStripMenuItem1.Visible = false;
			ToolStripMenuItem toolStripMenuItem4 = ctmCancel_Doc;
			bool visible = (cmtClose_Doc.Visible = true);
			toolStripMenuItem4.Visible = visible;
			string text2 = _saveAction switch
			{
				Enums.SaveAction.SaveAndNew => "New", 
				Enums.SaveAction.SaveAndView => "View", 
				Enums.SaveAction.SaveAndClose => "Close", 
				_ => "Search", 
			};
			btn_Save.Text = "Update && " + text2;
			if (btn_Save.ImageOptions.Image == null)
			{
				btn_Save.ImageOptions.Image = _btnSaveImg;
			}
			btn_Save.DropDownArrowStyle = DropDownArrowStyle.SplitButton;
			break;
		}
		case Enums.FormViewModeType.Find:
		{
			grd_Doc.DataSourceTable.Rows.Clear();
			grd_Lines.DataSourceTable.Rows.Clear();
			grd_Doc.DataSourceTable.AcceptChanges();
			grd_Lines.DataSourceTable.AcceptChanges();
			contextMenuStrip_Document.Enabled = false;
			contextMenuStrip1_DocLine.Enabled = false;
			cmtDuplicate_Return.Visible = true;
			closeLineToolStripMenuItem.Enabled = false;
			saveAsDraftToolStripMenuItem.Visible = false;
			saveAsDraftToolStripMenuItem1.Visible = false;
			ToolStripMenuItem toolStripMenuItem2 = ctmCancel_Doc;
			bool visible = (cmtClose_Doc.Visible = true);
			toolStripMenuItem2.Visible = visible;
			btn_Save.Text = "OK";
			break;
		}
		case Enums.FormViewModeType.Add:
		{
			gcDocCardCode.OptionsColumn.ReadOnly = false;
			gcDocCardName.OptionsColumn.ReadOnly = false;
			gcDocSeries.OptionsColumn.ReadOnly = false;
			gcDocNum.OptionsColumn.ReadOnly = false;
			rDocCustomerCode.Buttons[0].Visible = true;
			rDocSeries.Buttons[0].Visible = true;
			baseDocumentToolStripMenuItem.Enabled = false;
			targetDocumentToolStripMenuItem.Enabled = false;
			closeLineToolStripMenuItem.Enabled = false;
			cmtDuplicate_Return.Visible = false;
			saveAsDraftToolStripMenuItem.Visible = true;
			saveAsDraftToolStripMenuItem1.Visible = true;
			ToolStripMenuItem toolStripMenuItem = ctmCancel_Doc;
			bool visible = (cmtClose_Doc.Visible = false);
			toolStripMenuItem.Visible = visible;
			string text = _saveAction switch
			{
				Enums.SaveAction.SaveAndNew => "New", 
				Enums.SaveAction.SaveAndView => "View", 
				Enums.SaveAction.SaveAndClose => "Close", 
				_ => "Search", 
			};
			btn_Save.Text = "Add && " + text;
			if (btn_Save.ImageOptions.Image == null)
			{
				btn_Save.ImageOptions.Image = _btnSaveImg;
			}
			btn_Save.DropDownArrowStyle = DropDownArrowStyle.SplitButton;
			break;
		}
		}
	}

	protected override void NoAuthSetControlSearch()
	{
		ToolStripMenuItem toolStripMenuItem = saveAsDraftToolStripMenuItem;
		bool visible = (batchSerialNumberToolStripMenuItem.Visible = true);
		toolStripMenuItem.Visible = visible;
		btnCopyFrom.SettingReadonly = false;
		btnCopyFrom.Enabled = true;
	}
}
