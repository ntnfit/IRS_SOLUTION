using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Interfaces.FormInventory;
using Apzon.Forms.UserControls.BatchAndSerial;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Inventory.InventoryTransaction;

public class frm_InventoryTranfer : VirFormInventoryTranfer
{
	public frm_InventoryTranfer(string docEntry, ApzObjectTypes fromTypes)
	{
		FromDocEntry = docEntry;
		FromType = fromTypes;
		InitContructure();
	}

	public frm_InventoryTranfer()
	{
		InitContructure();
	}

	public void InitContructure()
	{
		ChangeLogType = Enums.ChangeLog.clStockTransfers;
		FormDocumentTable = "OWTR";
		FormLineTable = "WTR1";
		FormController = "InventoryTranfer";
		base.Text = "Inventory Transfer";
		FormLayoutCode = "WTR1";
		FormId = "oInventoryTransfer";
		JournalRemarksDefault = "Inventory Transfers -";
		ApzObjectTypes objectType = (ucBatchAndSerial1.FormObjectType = ApzObjectTypes.oStockTransfer);
		ObjectType = objectType;
		ucBatchAndSerial1.TransType = Enums.BatchSerialTransType.Out;
		GridViewEx gridViewEx = gv_Doc;
		GridViewEx gridViewEx2 = gv_Lines;
		ucBatchAndSerial ucBatchAndSerial2 = ucBatchAndSerial1;
		GridViewEx gridViewEx3 = gv_Logistic;
		string text = (ucAttachment1.FormId = FormId);
		string text2 = (gridViewEx3.FormId = text);
		string text4 = (ucBatchAndSerial2.FormId = text2);
		string formId2 = (gridViewEx2.FormId = text4);
		gridViewEx.FormId = formId2;
		gv_Doc.ItemId = "OWTR";
		gv_Lines.ItemId = "WTR1";
		ucBatchAndSerial1.ItemId = "OBTN";
		gv_Logistic.ItemId = "OLGT";
		cmtClose_Doc.Visible = false;
		closeLineToolStripMenuItem.Visible = false;
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
			if (FormMode != Enums.FormViewModeType.Add && !e.Column.FieldName.StartsWith("U_") && !new string[3] { "TaxDate", "JrnlMemo", "Comments" }.Contains(e.Column.FieldName))
			{
				e.Column.AppearanceCell.BackColor = Color.Gainsboro;
			}
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
			if (FormMode != Enums.FormViewModeType.Add)
			{
				e.Appearance.BackColor = Color.WhiteSmoke;
			}
			base.gv_Lines_RowCellStyle(sender, e);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override async Task LoadDataDoc(Enums.NavigateType type)
	{
		try
		{
			toolStripMenuItem_AddNewDocLine.Enabled = false;
			insertRowToolStripMenuItem.Enabled = false;
			deleteToolStripMenuItem_DocLine.Enabled = false;
			cmtDuplicate_Line.Enabled = false;
			await base.LoadDataDoc(type);
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
			base.LoadButtonCopy();
			DXPopupMenu dXPopupMenu = new DXPopupMenu();
			DXMenuItem dXMenuItem = new DXMenuItem();
			dXMenuItem.Caption = "Inventory Transfer Request";
			dXMenuItem.Click += async (object s, EventArgs e) =>
			{
				WinCommons._OpenProcessing();
				try
				{
					frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.TransferRequestCopyFrom, true, string.Empty, "", Enums.ItemObjectType.NoItemType, null, "");
					frm_CFL2.FromWhsCode = Function.ToString(gv_Doc.GetFocusedRowCellValue(gcDocFromWhs));
					frm_CFL2.ToWhsCode = Function.ToString(gv_Doc.GetFocusedRowCellValue(gcDocToWhs));
					frm_CFL2.ShowDialog(this);
					if (frm_CFL2.ChooseItems != null && frm_CFL2.ChooseItems.Count > 0)
					{
						string text = "";
						for (int i = 0; i < frm_CFL2.ChooseItems.Count; i++)
						{
							text = text + Function.ToString(frm_CFL2.ChooseItems[i]["DocEntry"]) + ",";
						}
						FromType = ApzObjectTypes.oInventoryTransferRequest;
						await LoadDataCopyFrom(text.Remove(text.Length - 1));
					}
				}
				catch (Exception ex2)
				{
					WinCommons.ErrorLineNumber(ex2);
				}
				WinCommons._CloseProcessing();
			};
			dXPopupMenu.Items.Add(dXMenuItem);
			btnCopyFrom.DropDownControl = dXPopupMenu;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
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
					Name = "Cancel",
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

	protected override void LoadFormView()
	{
		switch (FormMode)
		{
		case Enums.FormViewModeType.View:
			gv_Doc.SetViewModeButtonEdit(isEditable: false);
			gv_Lines.SetViewModeButtonEdit(isEditable: false, isHeader: false);
			gcDocCardCode.OptionsColumn.ReadOnly = true;
			gcDocCardName.OptionsColumn.ReadOnly = true;
			gcDocSeries.OptionsColumn.ReadOnly = true;
			gcDocNum.OptionsColumn.ReadOnly = true;
			rDocCustomerCode.Buttons[0].Visible = false;
			rDocSeries.Buttons[0].Visible = false;
			saveAsDraftToolStripMenuItem.Visible = false;
			saveAsDraftToolStripMenuItem1.Visible = false;
			ctmCancel_Doc.Visible = true;
			rItemCodeLine.Buttons[0].Visible = false;
			rLineWareHouse.Buttons[0].Visible = false;
			rLineBarCode.Buttons[0].Visible = false;
			rLineDistrRule.Buttons[0].Visible = false;
			rep_LineProject.Buttons[0].Visible = false;
			rLineStatus.Buttons[0].Visible = false;
			rLineTaxCode.Buttons[0].Visible = false;
			rLineUoMCode.Buttons[0].Visible = false;
			rLineUoMGroup.Buttons[0].Visible = false;
			rLineUseBaseUn.Buttons[0].Visible = false;
			rLineWareHouse.Buttons[0].Visible = false;
			deleteToolStripMenuItem_DocLine.Enabled = false;
			cmtDuplicate_Return.Visible = true;
			ctm_Reverse.Enabled = true;
			btn_Save.Text = "OK";
			btn_Save.ImageOptions.Image = null;
			btn_Save.DropDownArrowStyle = DropDownArrowStyle.Hide;
			break;
		case Enums.FormViewModeType.Update:
		{
			gv_Doc.SetViewModeButtonEdit(isEditable: false);
			gv_Lines.SetViewModeButtonEdit(isEditable: false, isHeader: false);
			gcDocCardCode.OptionsColumn.ReadOnly = true;
			gcDocCardName.OptionsColumn.ReadOnly = true;
			gcDocSeries.OptionsColumn.ReadOnly = true;
			gcDocNum.OptionsColumn.ReadOnly = true;
			rDocCustomerCode.Buttons[0].Visible = false;
			rDocSeries.Buttons[0].Visible = false;
			rItemCodeLine.Buttons[0].Visible = false;
			rLineWareHouse.Buttons[0].Visible = false;
			rLineBarCode.Buttons[0].Visible = false;
			rLineDistrRule.Buttons[0].Visible = false;
			rep_LineProject.Buttons[0].Visible = false;
			rLineStatus.Buttons[0].Visible = false;
			rLineTaxCode.Buttons[0].Visible = false;
			rLineUoMCode.Buttons[0].Visible = false;
			rLineUoMGroup.Buttons[0].Visible = false;
			rLineUseBaseUn.Buttons[0].Visible = false;
			rLineWareHouse.Buttons[0].Visible = false;
			cmtDuplicate_Return.Visible = true;
			deleteToolStripMenuItem_DocLine.Enabled = false;
			saveAsDraftToolStripMenuItem.Visible = false;
			saveAsDraftToolStripMenuItem1.Visible = false;
			ctmCancel_Doc.Visible = true;
			ctm_Reverse.Enabled = true;
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
			gv_Doc.SetViewModeButtonEdit(isEditable: false);
			gv_Lines.SetViewModeButtonEdit(isEditable: false, isHeader: false);
			grd_Doc.DataSourceTable.Rows.Clear();
			grd_Lines.DataSourceTable.Rows.Clear();
			grd_Doc.DataSourceTable.AcceptChanges();
			grd_Lines.DataSourceTable.AcceptChanges();
			contextMenuStrip_Document.Enabled = false;
			contextMenuStrip1_DocLine.Enabled = false;
			closeLineToolStripMenuItem.Enabled = false;
			deleteToolStripMenuItem_DocLine.Enabled = false;
			cmtDuplicate_Return.Visible = true;
			saveAsDraftToolStripMenuItem.Visible = false;
			saveAsDraftToolStripMenuItem1.Visible = false;
			ctmCancel_Doc.Visible = true;
			ctm_Reverse.Enabled = false;
			btn_Save.Text = "OK";
			break;
		case Enums.FormViewModeType.Add:
		{
			gv_Doc.SetViewModeButtonEdit(isEditable: true);
			gv_Lines.SetViewModeButtonEdit(isEditable: true, isHeader: false);
			gcDocCardCode.OptionsColumn.ReadOnly = false;
			gcDocCardName.OptionsColumn.ReadOnly = false;
			gcDocSeries.OptionsColumn.ReadOnly = false;
			gcDocNum.OptionsColumn.ReadOnly = false;
			rDocCustomerCode.Buttons[0].Visible = true;
			rDocSeries.Buttons[0].Visible = true;
			baseDocumentToolStripMenuItem.Enabled = false;
			targetDocumentToolStripMenuItem.Enabled = false;
			saveAsDraftToolStripMenuItem.Visible = true;
			saveAsDraftToolStripMenuItem1.Visible = true;
			ctmCancel_Doc.Visible = false;
			cmtDuplicate_Return.Visible = false;
			closeLineToolStripMenuItem.Enabled = false;
			deleteToolStripMenuItem_DocLine.Enabled = true;
			ctm_Reverse.Enabled = false;
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

	public override void CustomFormSetting()
	{
	}
}
