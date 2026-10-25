using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
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
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.ItemsMaster;
using Apzon.Forms.Properties;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.DMS;

public class frm_Channel : XtraFormEx
{
	private Dictionary<string, string> _dicStatus = new Dictionary<string, string>
	{
		{ "Y", "Active" },
		{ "N", "Inactive" }
	};

	private IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_Cancel;

	private LookUpEditEx lk_Status;

	private SimpleButtonEx btn_Search;

	private LabelLine lbl_ChnlCode;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private GridControlEx grd_Lines;

	private GridViewEx gv_Lines;

	private LabelLine lbl_Status;

	private LabelLine lbl_ChnnlName;

	private ButtonEditEx btn_ChnlCode;

	private GridColumnEx gc_Code;

	private GridColumnEx gc_Name;

	private GridColumnEx gc_Status;

	private RepositoryItemLookUpEdit rep_Status;

	private GridColumnEx gc_Remarks;

	private GridColumnEx gc_UserSign;

	private GridColumnEx gc_Loginstanc;

	private GridColumnEx gc_CreateDate;

	private RepositoryItemDateEdit rep_CreateDate;

	private GridColumnEx gc_CreateTime;

	private RepositoryItemHourEdit rep_CreateTime;

	private GridColumnEx gc_UpdateDate;

	private RepositoryItemDateEdit repositoryItemDateEdit1;

	private GridColumnEx gc_UpdateTime;

	private RepositoryItemHourEdit repositoryItemHourEdit1;

	private GridColumnEx gc_AllItem;

	private RepositoryItemCheckEdit rep_AllItm;

	private RepositoryItemCheckEdit repositoryItemCheckEdit1;

	private GridColumnEx gc_ItmCode;

	private GridColumnEx gc_ItmName;

	private GridColumnEx gc_FrgnName;

	private GridColumnEx gc_ChnCode;

	private GridColumnEx gc_Status1;

	private RepositoryItemLookUpEdit rep_Status1;

	private GridColumnEx gc_ItmRemarks;

	private GridColumnEx gc_CreateDate1;

	private GridColumnEx gc_CreateTime1;

	private GridColumnEx gc_UpdateDate1;

	private GridColumnEx gc_UpdateTime1;

	private ButtonEditEx btn_ChnnlName;

	private ContextMenuStrip contextMenuStrip1;

	private ContextMenuStrip contextMenuStrip2;

	private ToolStripMenuItem menu_AddDoc;

	private ToolStripMenuItem menu_RemoveDoc;

	private ToolStripMenuItem menu_DuplicateDoc;

	private ToolStripMenuItem menu_AddLine;

	private ToolStripMenuItem menu_RemoveLine;

	private RepositoryItemButtonEdit rep_Item;

	private GridColumnEx gc_ItmDesc;

	private GridColumnEx gc_Active;

	private RepositoryItemCheckEdit rep_Active;

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

	public frm_Channel()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_Channel"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_Channel" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				contextMenuStrip1.Enabled = false;
				contextMenuStrip2.Enabled = false;
				btn_Cancel.Enabled = true;
			}
			return;
		}
		throw new Exception(Apzon.Commons.Properties.Resources.Msg_FormAccessDenied);
	}

	public async Task REFRESH()
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			addNewToolStripMenuItem_Click(null, null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_FIRST()
	{
		try
		{
			WinCommons._OpenProcessing();
			GetRecord(Enums.NavigateType.First, Function.ToString(gv_Doc.GetRowCellValue(gv_Doc.FocusedRowHandle, "ChannelCode")));
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_PREVIOUS()
	{
		try
		{
			WinCommons._OpenProcessing();
			GetRecord(Enums.NavigateType.Previous, Function.ToString(gv_Doc.GetRowCellValue(gv_Doc.FocusedRowHandle, "ChannelCode")));
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_NEXT()
	{
		try
		{
			WinCommons._OpenProcessing();
			GetRecord(Enums.NavigateType.Next, Function.ToString(gv_Doc.GetRowCellValue(gv_Doc.FocusedRowHandle, "ChannelCode")));
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_LAST()
	{
		try
		{
			WinCommons._OpenProcessing();
			GetRecord(Enums.NavigateType.Last, Function.ToString(gv_Doc.GetRowCellValue(gv_Doc.FocusedRowHandle, "ChannelCode")));
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
		WinCommons._CloseProcessing();
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			await LoadData();
			btn_ChnlCode.EditValue = null;
			btn_ChnnlName.EditValue = null;
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void InitData()
	{
		try
		{
			lk_Status.SetDataSource(_dicStatus);
			rep_Status.DataSource = _dicStatus;
			rep_Status1.DataSource = _dicStatus;
			DataTable tableStructure = WinFuntions.GetTableStructure("APZ_D_OCHN");
			DataTable tableStructure2 = WinFuntions.GetTableStructure("APZ_D_CHN1");
			if (tableStructure.Columns.Count == 0 && tableStructure2.Columns.Count == 0)
			{
				WinFuntions.DisableAllControl(Controls);
				btn_Cancel.Enabled = true;
				contextMenuStrip1.Enabled = false;
				contextMenuStrip2.Enabled = false;
				WinCommons.Error(Enums.MessageCode.CannotLoadStructure, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadData(string channelCode = "", string channelName = "", string status = "")
	{
		try
		{
			DataTable dataTable = await _httpClient.ChannelClient.GetDataDoc(channelCode, channelName, status);
			if (dataTable.IsNotNull())
			{
				string text = Function.ToString(dataTable.Rows[0]["AllItem"]);
				ShowGridItems(text.Equals("Y"));
			}
			grd_Doc.DataSource = dataTable;
			grd_Doc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataLine(string channelCode)
	{
		try
		{
			GridControlEx gridControlEx = grd_Lines;
			gridControlEx.DataSource = await _httpClient.ChannelClient.GetDataLine(channelCode);
			grd_Lines.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void AddNewDoc()
	{
		try
		{
			DataTable dataTable = grd_Doc.DataSourceTable.Clone();
			DataRow dataRow = dataTable.NewRow();
			dataRow["Status"] = "Y";
			dataRow["AllItem"] = "N";
			dataTable.Rows.Add(dataRow);
			grd_Doc.DataSource = dataTable;
			grd_Doc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void AddNewLine(bool isNew)
	{
		try
		{
			DataTable dataTable = (isNew ? grd_Lines.DataSourceTable.Clone() : grd_Lines.DataSourceTable);
			if (dataTable.Columns.Count == 0)
			{
				dataTable = WinFuntions.GetTableStructure("APZ_D_CHN1");
			}
			DataRow dataRow = dataTable.NewRow();
			dataRow["Status"] = "Y";
			dataRow["Active"] = "N";
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			grd_Lines.DataSource = dataTable;
			grd_Lines.RefreshDataSource();
			gv_Lines.FocusedRowHandle = gv_Lines.RowCount - 1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void UpdateFormView()
	{
		if (FormMode != Enums.FormViewModeType.Add)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void ShowGridItems(bool isAll)
	{
		try
		{
			if (isAll)
			{
				splitContainerControl1.PanelVisibility = SplitPanelVisibility.Panel1;
				grd_Lines.DataSourceTable.Clear();
				grd_Lines.DataSourceTable.AcceptChanges();
			}
			else
			{
				splitContainerControl1.PanelVisibility = SplitPanelVisibility.Both;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<string> SaveData(Enums.FormViewModeType formViewMode)
	{
		try
		{
			DataTable dataTable = gv_Doc.GetListDataSourceSelectedRow().CopyToDataTable();
			DataTable dataSourceTable = grd_Lines.DataSourceTable;
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(dataTable.Copy());
			dataSet.Tables.Add(dataSourceTable.Copy());
			dataSet.AcceptChanges();
			HttpResult httpResult = await _httpClient.ChannelClient.SaveData(new Tuple<DataSet, string>(dataSet, (formViewMode == Enums.FormViewModeType.Add) ? "A" : "U"));
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				return httpResult.Message;
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return ex.Message;
		}
	}

	private void GetRecord(Enums.NavigateType type, string channelCode)
	{
		try
		{
			string position = string.Empty;
			DataTable dataTable = new DataTable();
			switch (type)
			{
			case Enums.NavigateType.First:
				position = "F";
				break;
			case Enums.NavigateType.Last:
				position = "L";
				break;
			case Enums.NavigateType.Previous:
				position = "P";
				break;
			case Enums.NavigateType.Next:
				position = "N";
				break;
			}
			dataTable = _httpClient.ChannelClient.GetRecord(position, channelCode);
			if (dataTable.IsNotNull())
			{
				grd_Doc.DataSource = dataTable;
				grd_Doc.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ShowChannelCfl(bool isChannelCode)
	{
		try
		{
			using frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Channel, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				if (isChannelCode)
				{
					btn_ChnlCode.Text = Function.ToString(frm_CFL2.ChooseItem[0]);
				}
				else
				{
					btn_ChnnlName.Text = Function.ToString(frm_CFL2.ChooseItem[1]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void frm_Channel_Load(object sender, EventArgs e)
	{
		try
		{
			InitData();
			await LoadData();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gv_Lines_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void addNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add)
			{
				AddNewDoc();
				AddNewLine(isNew: true);
				ShowGridItems(isAll: false);
				FormMode = Enums.FormViewModeType.Add;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void addNewToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		try
		{
			AddNewLine(isNew: false);
			UpdateFormView();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (gv_Doc.FocusedColumn.Name == gc_Code.Name)
			{
				if (FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Update)
				{
					e.Cancel = true;
				}
				else
				{
					e.Cancel = false;
				}
			}
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

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBox.Show((FormMode == Enums.FormViewModeType.Add) ? "Do you want to save?" : "Do you want to save changes?", "System Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				btn_OK.Enabled = false;
				string text = await SaveData(FormMode);
				if (!string.IsNullOrEmpty(text))
				{
					WinCommons.Error(Enums.MessageCode.None, text);
					btn_OK.Enabled = true;
					return;
				}
				FormMode = Enums.FormViewModeType.View;
				await LoadData();
				WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		btn_OK.Enabled = true;
		WinCommons._CloseProcessing();
	}

	private void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.Name == gc_AllItem.Name)
			{
				bool flag = Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, gc_AllItem.FieldName)) == "Y";
				if (flag)
				{
					for (int i = 0; i < gv_Doc.RowCount; i++)
					{
						if (gv_Doc.FocusedRowHandle != i)
						{
							gv_Doc.SetRowCellValue(i, gc_AllItem, "N");
						}
					}
					gv_Doc.RefreshEditor(updateEditorValue: true);
				}
				ShowGridItems(flag);
			}
			UpdateFormView();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Lines_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			UpdateFormView();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void toolStripMenuItem2_Click(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			if (FormMode == Enums.FormViewModeType.View && XtraMessageBox.Show("Do you want to remove this channel?", "System Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				Tuple<string> tupData = new Tuple<string>(Function.ToString(gv_Doc.GetRowCellValue(gv_Doc.FocusedRowHandle, gc_Code.FieldName)));
				HttpResult httpResult = await _httpClient.ChannelClient.Remove(tupData);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					return;
				}
				WinCommons.Success(Enums.MessageCode.Success, "");
				await LoadData();
				await LoadDataLine(Function.ToString(gv_Doc.GetRowCellValue(gv_Doc.FocusedRowHandle, gc_Code.FieldName)));
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void menu_RemoveLine_Click(object sender, EventArgs e)
	{
		try
		{
			if (grd_Doc.DataSourceTable.IsNotNull())
			{
				if (grd_Lines.DataSourceTable.IsNotNull())
				{
					gv_Lines.DeleteDataSourceRowSelected();
					gv_Lines.RefreshData();
				}
				else
				{
					AddNewLine(isNew: false);
				}
				UpdateFormView();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void frm_Channel_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBox.Show("Unsaved data will be lost! Do you want to continue without saving?", "System Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_MouseDown(object sender, MouseEventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grd_Doc.DataSourceTable;
			menu_AddDoc.Enabled = dataSourceTable.Columns.Count > 0;
			menu_RemoveDoc.Enabled = dataSourceTable.IsNotNull();
			menu_DuplicateDoc.Enabled = dataSourceTable.IsNotNull();
			if (FormMode == Enums.FormViewModeType.Add)
			{
				menu_AddDoc.Enabled = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Lines_MouseDown(object sender, MouseEventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grd_Doc.DataSourceTable;
			DataTable dataSourceTable2 = grd_Lines.DataSourceTable;
			if (grd_Doc.DataSourceTable.IsNotNull() && grd_Doc.DataSourceTable.AsEnumerable().FirstOrDefault((DataRow row) => Function.ToString(row["AllItem"]) == "Y") != null)
			{
				menu_AddLine.Enabled = false;
				menu_RemoveLine.Enabled = false;
			}
			else
			{
				menu_AddLine.Enabled = dataSourceTable.IsNotNull();
				menu_RemoveLine.Enabled = dataSourceTable2.IsNotNull();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void menu_DuplicateDoc_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				DataRow dataRow = gv_Doc.GetDataRow(gv_Doc.FocusedRowHandle);
				if (dataRow != null)
				{
					DataTable dataTable = grd_Doc.DataSourceTable.Clone();
					dataRow["ChannelCode"] = string.Empty;
					dataTable.Rows.Add(dataRow.ItemArray);
					dataTable.AcceptChanges();
					grd_Doc.DataSource = dataTable;
					grd_Doc.RefreshDataSource();
					AddNewLine(isNew: true);
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

	private async void btn_Search_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBox.Show("Unsaved data will be lost! Do you want to continue without saving?", "System Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			btn_Search.Enabled = false;
			await LoadData(Function.ToString(btn_ChnlCode.EditValue), Function.ToString(btn_ChnnlName.EditValue), Function.ToString(lk_Status.EditValue));
			await LoadDataLine(Function.ToString(gv_Doc.GetRowCellValue(gv_Doc.FocusedRowHandle, gc_ChnCode.FieldName)));
			if (!grd_Doc.DataSourceTable.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
			}
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
		btn_Search.Enabled = true;
		WinCommons._CloseProcessing();
	}

	private void btn_ChnlCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			ShowChannelCfl(isChannelCode: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_ChnnlName_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			ShowChannelCfl(isChannelCode: false);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_Item_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				if (!string.IsNullOrEmpty(Function.ToString(gv_Lines.GetRowCellValue(gv_Lines.FocusedRowHandle, gc_ItmCode))))
				{
					frm_ItemMasterData frm_ItemMasterData2 = new frm_ItemMasterData();
					frm_ItemMasterData2.MdiParent = MdiParent;
					frm_ItemMasterData2.Show();
				}
				return;
			}
			using frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Items, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ListMandatoryField = "ItemName,ItemDescription,FrgnName";
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				gv_Lines.SetRowCellValue(gv_Lines.FocusedRowHandle, gc_ItmCode, frm_CFL2.ChooseItem["ItemCode"]);
				gv_Lines.SetRowCellValue(gv_Lines.FocusedRowHandle, gc_ItmName, frm_CFL2.ChooseItem["ItemName"]);
				gv_Lines.SetRowCellValue(gv_Lines.FocusedRowHandle, gc_ItmDesc, frm_CFL2.ChooseItem["ItemDescription"]);
				gv_Lines.SetRowCellValue(gv_Lines.FocusedRowHandle, gc_FrgnName, frm_CFL2.ChooseItem["FrgnName"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gv_Doc_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add && (FormMode != Enums.FormViewModeType.Update || XtraMessageBox.Show("Unsaved data will be lost! Do you want to continue without saving?", "System Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.No))
			{
				string channelCode = Function.ToString(gv_Doc.GetRowCellValue(gv_Doc.FocusedRowHandle, gc_ChnCode.FieldName));
				bool isAll = Function.ToString(gv_Doc.GetRowCellValue(e.FocusedRowHandle, gc_AllItem.FieldName)) == "Y";
				ShowGridItems(isAll);
				await LoadDataLine(channelCode);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.DMS.frm_Channel));
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.menu_AddDoc = new System.Windows.Forms.ToolStripMenuItem();
		this.menu_RemoveDoc = new System.Windows.Forms.ToolStripMenuItem();
		this.menu_DuplicateDoc = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gc_Code = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Name = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Status = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Status = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_Remarks = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_UserSign = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Loginstanc = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_CreateDate = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_CreateDate = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gc_CreateTime = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_CreateTime = new Apzon.Controls.Controls.RepositoryItemHourEdit();
		this.gc_UpdateDate = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gc_UpdateTime = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemHourEdit1 = new Apzon.Controls.Controls.RepositoryItemHourEdit();
		this.gc_AllItem = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_AllItm = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.grd_Lines = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.menu_AddLine = new System.Windows.Forms.ToolStripMenuItem();
		this.menu_RemoveLine = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Lines = new Apzon.Controls.Grid.GridViewEx();
		this.gc_Active = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Active = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gc_ItmCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Item = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gc_ItmName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_FrgnName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_ItmDesc = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_ChnCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Status1 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Status1 = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_ItmRemarks = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_CreateDate1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_CreateTime1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_UpdateDate1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_UpdateTime1 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lk_Status = new Apzon.Controls.Editor.LookUpEditEx();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lbl_ChnlCode = new Apzon.Controls.Controls.LabelLine();
		this.lbl_Status = new Apzon.Controls.Controls.LabelLine();
		this.lbl_ChnnlName = new Apzon.Controls.Controls.LabelLine();
		this.btn_ChnlCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.btn_ChnnlName = new Apzon.Controls.Editor.ButtonEditEx();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CreateDate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CreateDate.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CreateTime).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemHourEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_AllItm).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Lines).BeginInit();
		this.contextMenuStrip2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Lines).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Active).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Item).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lk_Status.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ChnlCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ChnnlName.Properties).BeginInit();
		base.SuspendLayout();
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(2, 60);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Doc);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.grd_Lines);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(815, 379);
		this.splitContainerControl1.SplitterPosition = 168;
		this.splitContainerControl1.TabIndex = 7;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[6] { this.rep_Status, this.rep_CreateDate, this.rep_CreateTime, this.repositoryItemDateEdit1, this.repositoryItemHourEdit1, this.rep_AllItm });
		this.grd_Doc.Size = new System.Drawing.Size(815, 168);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.menu_AddDoc, this.menu_RemoveDoc, this.menu_DuplicateDoc });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(125, 70);
		this.menu_AddDoc.Name = "menu_AddDoc";
		this.menu_AddDoc.Size = new System.Drawing.Size(124, 22);
		this.menu_AddDoc.Text = "Add New";
		this.menu_AddDoc.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
		this.menu_RemoveDoc.Name = "menu_RemoveDoc";
		this.menu_RemoveDoc.Size = new System.Drawing.Size(124, 22);
		this.menu_RemoveDoc.Text = "Remove";
		this.menu_RemoveDoc.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
		this.menu_DuplicateDoc.Name = "menu_DuplicateDoc";
		this.menu_DuplicateDoc.Size = new System.Drawing.Size(124, 22);
		this.menu_DuplicateDoc.Text = "Duplicate";
		this.menu_DuplicateDoc.Click += new System.EventHandler(this.menu_DuplicateDoc_Click);
		this.gv_Doc.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Doc.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[11]
		{
			this.gc_Code, this.gc_Name, this.gc_Status, this.gc_Remarks, this.gc_UserSign, this.gc_Loginstanc, this.gc_CreateDate, this.gc_CreateTime, this.gc_UpdateDate, this.gc_UpdateTime,
			this.gc_AllItem
		});
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oChannel";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "APZ_D_OCHN";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Doc.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Doc.OptionsPrint.AutoWidth = false;
		this.gv_Doc.OptionsPrint.UsePrintStyles = false;
		this.gv_Doc.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Doc.OptionsView.ShowGroupedColumns = true;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Doc_CustomDrawRowIndicator);
		this.gv_Doc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Doc_RowCellStyle);
		this.gv_Doc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Doc_ShowingEditor);
		this.gv_Doc.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gv_Doc_FocusedRowChanged);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
		this.gv_Doc.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gv_Doc_MouseDown);
		this.gc_Code.Caption = "Mã kênh";
		this.gc_Code.CurrencyDateField = null;
		this.gc_Code.CurrencyField = null;
		this.gc_Code.CurrencyFixed = null;
		this.gc_Code.CurrencyFixedRate = 0.0;
		this.gc_Code.CurrencyRateField = null;
		this.gc_Code.FieldName = "ChannelCode";
		this.gc_Code.Name = "gc_Code";
		this.gc_Code.PrimaryKey = true;
		this.gc_Code.UnitField = null;
		this.gc_Code.Visible = true;
		this.gc_Code.VisibleIndex = 0;
		this.gc_Code.Width = 85;
		this.gc_Name.Caption = "Tên kênh";
		this.gc_Name.CurrencyDateField = null;
		this.gc_Name.CurrencyField = null;
		this.gc_Name.CurrencyFixed = null;
		this.gc_Name.CurrencyFixedRate = 0.0;
		this.gc_Name.CurrencyRateField = null;
		this.gc_Name.FieldName = "ChannelName";
		this.gc_Name.Name = "gc_Name";
		this.gc_Name.PrimaryKey = false;
		this.gc_Name.UnitField = null;
		this.gc_Name.Visible = true;
		this.gc_Name.VisibleIndex = 1;
		this.gc_Name.Width = 85;
		this.gc_Status.Caption = "Trạng thái";
		this.gc_Status.ColumnEdit = this.rep_Status;
		this.gc_Status.CurrencyDateField = null;
		this.gc_Status.CurrencyField = null;
		this.gc_Status.CurrencyFixed = null;
		this.gc_Status.CurrencyFixedRate = 0.0;
		this.gc_Status.CurrencyRateField = null;
		this.gc_Status.FieldName = "Status";
		this.gc_Status.Name = "gc_Status";
		this.gc_Status.PrimaryKey = false;
		this.gc_Status.UnitField = null;
		this.gc_Status.Visible = true;
		this.gc_Status.VisibleIndex = 2;
		this.rep_Status.AutoHeight = false;
		this.rep_Status.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Status.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Name")
		});
		this.rep_Status.DisplayMember = "Value";
		this.rep_Status.Name = "rep_Status";
		this.rep_Status.NullText = "";
		this.rep_Status.ShowHeader = false;
		this.rep_Status.ValueMember = "Key";
		this.gc_Remarks.Caption = "Diễn giải";
		this.gc_Remarks.CurrencyDateField = null;
		this.gc_Remarks.CurrencyField = null;
		this.gc_Remarks.CurrencyFixed = null;
		this.gc_Remarks.CurrencyFixedRate = 0.0;
		this.gc_Remarks.CurrencyRateField = null;
		this.gc_Remarks.FieldName = "Remarks";
		this.gc_Remarks.Name = "gc_Remarks";
		this.gc_Remarks.PrimaryKey = false;
		this.gc_Remarks.UnitField = null;
		this.gc_Remarks.Visible = true;
		this.gc_Remarks.VisibleIndex = 3;
		this.gc_UserSign.Caption = "User Sign";
		this.gc_UserSign.CurrencyDateField = null;
		this.gc_UserSign.CurrencyField = null;
		this.gc_UserSign.CurrencyFixed = null;
		this.gc_UserSign.CurrencyFixedRate = 0.0;
		this.gc_UserSign.CurrencyRateField = null;
		this.gc_UserSign.FieldName = "UserSign";
		this.gc_UserSign.Name = "gc_UserSign";
		this.gc_UserSign.PrimaryKey = false;
		this.gc_UserSign.UnitField = null;
		this.gc_Loginstanc.Caption = "LogInstance";
		this.gc_Loginstanc.CurrencyDateField = null;
		this.gc_Loginstanc.CurrencyField = null;
		this.gc_Loginstanc.CurrencyFixed = null;
		this.gc_Loginstanc.CurrencyFixedRate = 0.0;
		this.gc_Loginstanc.CurrencyRateField = null;
		this.gc_Loginstanc.FieldName = "Loginstanc";
		this.gc_Loginstanc.Name = "gc_Loginstanc";
		this.gc_Loginstanc.PrimaryKey = false;
		this.gc_Loginstanc.UnitField = null;
		this.gc_CreateDate.Caption = "Create Date";
		this.gc_CreateDate.ColumnEdit = this.rep_CreateDate;
		this.gc_CreateDate.CurrencyDateField = null;
		this.gc_CreateDate.CurrencyField = null;
		this.gc_CreateDate.CurrencyFixed = null;
		this.gc_CreateDate.CurrencyFixedRate = 0.0;
		this.gc_CreateDate.CurrencyRateField = null;
		this.gc_CreateDate.FieldName = "CreateDate";
		this.gc_CreateDate.Name = "gc_CreateDate";
		this.gc_CreateDate.PrimaryKey = false;
		this.gc_CreateDate.UnitField = null;
		this.rep_CreateDate.AutoHeight = false;
		this.rep_CreateDate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_CreateDate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_CreateDate.Name = "rep_CreateDate";
		this.gc_CreateTime.Caption = "Create Time";
		this.gc_CreateTime.ColumnEdit = this.rep_CreateTime;
		this.gc_CreateTime.CurrencyDateField = null;
		this.gc_CreateTime.CurrencyField = null;
		this.gc_CreateTime.CurrencyFixed = null;
		this.gc_CreateTime.CurrencyFixedRate = 0.0;
		this.gc_CreateTime.CurrencyRateField = null;
		this.gc_CreateTime.FieldName = "CreateTime";
		this.gc_CreateTime.Name = "gc_CreateTime";
		this.gc_CreateTime.PrimaryKey = false;
		this.gc_CreateTime.UnitField = null;
		this.rep_CreateTime.AutoHeight = false;
		this.rep_CreateTime.IsSearchControl = false;
		this.rep_CreateTime.MaxLength = 5;
		this.rep_CreateTime.Name = "rep_CreateTime";
		this.gc_UpdateDate.Caption = "Update Date";
		this.gc_UpdateDate.ColumnEdit = this.repositoryItemDateEdit1;
		this.gc_UpdateDate.CurrencyDateField = null;
		this.gc_UpdateDate.CurrencyField = null;
		this.gc_UpdateDate.CurrencyFixed = null;
		this.gc_UpdateDate.CurrencyFixedRate = 0.0;
		this.gc_UpdateDate.CurrencyRateField = null;
		this.gc_UpdateDate.FieldName = "UpdateDate";
		this.gc_UpdateDate.Name = "gc_UpdateDate";
		this.gc_UpdateDate.PrimaryKey = false;
		this.gc_UpdateDate.UnitField = null;
		this.repositoryItemDateEdit1.AutoHeight = false;
		this.repositoryItemDateEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit1.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit1.Name = "repositoryItemDateEdit1";
		this.gc_UpdateTime.Caption = "Update Time";
		this.gc_UpdateTime.ColumnEdit = this.repositoryItemHourEdit1;
		this.gc_UpdateTime.CurrencyDateField = null;
		this.gc_UpdateTime.CurrencyField = null;
		this.gc_UpdateTime.CurrencyFixed = null;
		this.gc_UpdateTime.CurrencyFixedRate = 0.0;
		this.gc_UpdateTime.CurrencyRateField = null;
		this.gc_UpdateTime.FieldName = "UpdateTime";
		this.gc_UpdateTime.Name = "gc_UpdateTime";
		this.gc_UpdateTime.PrimaryKey = false;
		this.gc_UpdateTime.UnitField = null;
		this.repositoryItemHourEdit1.AutoHeight = false;
		this.repositoryItemHourEdit1.IsSearchControl = false;
		this.repositoryItemHourEdit1.MaxLength = 5;
		this.repositoryItemHourEdit1.Name = "repositoryItemHourEdit1";
		this.gc_AllItem.Caption = "Tất cả mặt hàng";
		this.gc_AllItem.ColumnEdit = this.rep_AllItm;
		this.gc_AllItem.CurrencyDateField = null;
		this.gc_AllItem.CurrencyField = null;
		this.gc_AllItem.CurrencyFixed = null;
		this.gc_AllItem.CurrencyFixedRate = 0.0;
		this.gc_AllItem.CurrencyRateField = null;
		this.gc_AllItem.FieldName = "AllItem";
		this.gc_AllItem.Name = "gc_AllItem";
		this.gc_AllItem.PrimaryKey = false;
		this.gc_AllItem.UnitField = null;
		this.gc_AllItem.Visible = true;
		this.gc_AllItem.VisibleIndex = 4;
		this.gc_AllItem.Width = 171;
		this.rep_AllItm.AutoHeight = false;
		this.rep_AllItm.DisplayValueChecked = "Y";
		this.rep_AllItm.DisplayValueUnchecked = "N";
		this.rep_AllItm.Name = "rep_AllItm";
		this.rep_AllItm.ValueChecked = "Y";
		this.rep_AllItm.ValueUnchecked = "N";
		this.grd_Lines.AllowPasteExcel = true;
		this.grd_Lines.ClipboardData = null;
		this.grd_Lines.ContextMenuStrip = this.contextMenuStrip2;
		this.grd_Lines.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Lines.Location = new System.Drawing.Point(0, 0);
		this.grd_Lines.MainView = this.gv_Lines;
		this.grd_Lines.Name = "grd_Lines";
		this.grd_Lines.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.rep_Status1, this.repositoryItemCheckEdit1, this.rep_Item, this.rep_Active });
		this.grd_Lines.Size = new System.Drawing.Size(815, 201);
		this.grd_Lines.TabIndex = 1;
		this.grd_Lines.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Lines });
		this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.menu_AddLine, this.menu_RemoveLine });
		this.contextMenuStrip2.Name = "contextMenuStrip2";
		this.contextMenuStrip2.Size = new System.Drawing.Size(124, 48);
		this.menu_AddLine.Name = "menu_AddLine";
		this.menu_AddLine.Size = new System.Drawing.Size(123, 22);
		this.menu_AddLine.Text = "Add New";
		this.menu_AddLine.Click += new System.EventHandler(this.addNewToolStripMenuItem1_Click);
		this.menu_RemoveLine.Name = "menu_RemoveLine";
		this.menu_RemoveLine.Size = new System.Drawing.Size(123, 22);
		this.menu_RemoveLine.Text = "Remove";
		this.menu_RemoveLine.Click += new System.EventHandler(this.menu_RemoveLine_Click);
		this.gv_Lines.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Lines.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Lines.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Lines.ColumnKey = null;
		this.gv_Lines.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[12]
		{
			this.gc_Active, this.gc_ItmCode, this.gc_ItmName, this.gc_FrgnName, this.gc_ItmDesc, this.gc_ChnCode, this.gc_Status1, this.gc_ItmRemarks, this.gc_CreateDate1, this.gc_CreateTime1,
			this.gc_UpdateDate1, this.gc_UpdateTime1
		});
		this.gv_Lines.DisableFormSetting = false;
		this.gv_Lines.DisplayException = false;
		this.gv_Lines.EditColumn = null;
		this.gv_Lines.FieldCurrency = null;
		this.gv_Lines.FieldRate = null;
		this.gv_Lines.FormId = "oChannel";
		this.gv_Lines.GridControl = this.grd_Lines;
		this.gv_Lines.IgnoreFilter = false;
		this.gv_Lines.IndexEditRow = -1;
		this.gv_Lines.IndicatorWidth = 35;
		this.gv_Lines.IsApzTable = false;
		this.gv_Lines.IsInsertNewRow = false;
		this.gv_Lines.IsLoadingData = false;
		this.gv_Lines.IsMappingContextAction = false;
		this.gv_Lines.ItemId = "APZ_D_CHN1";
		this.gv_Lines.LstColumnAllowBatch = null;
		this.gv_Lines.LstColumnNotAllowBatch = null;
		this.gv_Lines.LstColumnNotEditable = null;
		this.gv_Lines.LstColumNotSetting = null;
		this.gv_Lines.Name = "gv_Lines";
		this.gv_Lines.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Lines.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Lines.OptionsPrint.AutoWidth = false;
		this.gv_Lines.OptionsPrint.UsePrintStyles = false;
		this.gv_Lines.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Lines.OptionsView.ColumnAutoWidth = false;
		this.gv_Lines.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Lines.OptionsView.ShowGroupedColumns = true;
		this.gv_Lines.OptionsView.ShowGroupPanel = false;
		this.gv_Lines.UseBatchMod = false;
		this.gv_Lines.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Lines_CustomDrawRowIndicator);
		this.gv_Lines.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Lines_CellValueChanged);
		this.gv_Lines.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gv_Lines_MouseDown);
		this.gc_Active.ColumnEdit = this.rep_Active;
		this.gc_Active.CurrencyDateField = null;
		this.gc_Active.CurrencyField = null;
		this.gc_Active.CurrencyFixed = null;
		this.gc_Active.CurrencyFixedRate = 0.0;
		this.gc_Active.CurrencyRateField = null;
		this.gc_Active.FieldName = "Active";
		this.gc_Active.Name = "gc_Active";
		this.gc_Active.OptionsColumn.ShowCaption = false;
		this.gc_Active.PrimaryKey = true;
		this.gc_Active.UnitField = null;
		this.gc_Active.Visible = true;
		this.gc_Active.VisibleIndex = 0;
		this.gc_Active.Width = 45;
		this.rep_Active.AutoHeight = false;
		this.rep_Active.DisplayValueChecked = "Y";
		this.rep_Active.DisplayValueUnchecked = "N";
		this.rep_Active.Name = "rep_Active";
		this.rep_Active.ValueChecked = "Y";
		this.rep_Active.ValueUnchecked = "N";
		this.gc_ItmCode.Caption = "Mã hàng";
		this.gc_ItmCode.ColumnEdit = this.rep_Item;
		this.gc_ItmCode.CurrencyDateField = null;
		this.gc_ItmCode.CurrencyField = null;
		this.gc_ItmCode.CurrencyFixed = null;
		this.gc_ItmCode.CurrencyFixedRate = 0.0;
		this.gc_ItmCode.CurrencyRateField = null;
		this.gc_ItmCode.FieldName = "ItemCode";
		this.gc_ItmCode.Name = "gc_ItmCode";
		this.gc_ItmCode.PrimaryKey = true;
		this.gc_ItmCode.UnitField = null;
		this.gc_ItmCode.Visible = true;
		this.gc_ItmCode.VisibleIndex = 1;
		this.gc_ItmCode.Width = 100;
		this.rep_Item.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_Item.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_Item.Name = "rep_Item";
		this.rep_Item.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Item_ButtonClick);
		this.gc_ItmName.Caption = "Tên hàng";
		this.gc_ItmName.CurrencyDateField = null;
		this.gc_ItmName.CurrencyField = null;
		this.gc_ItmName.CurrencyFixed = null;
		this.gc_ItmName.CurrencyFixedRate = 0.0;
		this.gc_ItmName.CurrencyRateField = null;
		this.gc_ItmName.FieldName = "ItemName";
		this.gc_ItmName.Name = "gc_ItmName";
		this.gc_ItmName.OptionsColumn.ReadOnly = true;
		this.gc_ItmName.PrimaryKey = false;
		this.gc_ItmName.UnitField = null;
		this.gc_ItmName.Visible = true;
		this.gc_ItmName.VisibleIndex = 2;
		this.gc_ItmName.Width = 145;
		this.gc_FrgnName.Caption = "Tên nước ngoài";
		this.gc_FrgnName.CurrencyDateField = null;
		this.gc_FrgnName.CurrencyField = null;
		this.gc_FrgnName.CurrencyFixed = null;
		this.gc_FrgnName.CurrencyFixedRate = 0.0;
		this.gc_FrgnName.CurrencyRateField = null;
		this.gc_FrgnName.FieldName = "FrgnName";
		this.gc_FrgnName.Name = "gc_FrgnName";
		this.gc_FrgnName.OptionsColumn.ReadOnly = true;
		this.gc_FrgnName.PrimaryKey = false;
		this.gc_FrgnName.UnitField = null;
		this.gc_FrgnName.Visible = true;
		this.gc_FrgnName.VisibleIndex = 3;
		this.gc_FrgnName.Width = 145;
		this.gc_ItmDesc.Caption = "Mô tả";
		this.gc_ItmDesc.CurrencyDateField = null;
		this.gc_ItmDesc.CurrencyField = null;
		this.gc_ItmDesc.CurrencyFixed = null;
		this.gc_ItmDesc.CurrencyFixedRate = 0.0;
		this.gc_ItmDesc.CurrencyRateField = null;
		this.gc_ItmDesc.FieldName = "ItemDescription";
		this.gc_ItmDesc.Name = "gc_ItmDesc";
		this.gc_ItmDesc.OptionsColumn.ReadOnly = true;
		this.gc_ItmDesc.PrimaryKey = false;
		this.gc_ItmDesc.UnitField = null;
		this.gc_ItmDesc.Visible = true;
		this.gc_ItmDesc.VisibleIndex = 4;
		this.gc_ItmDesc.Width = 145;
		this.gc_ChnCode.Caption = "Channel Code";
		this.gc_ChnCode.CurrencyDateField = null;
		this.gc_ChnCode.CurrencyField = null;
		this.gc_ChnCode.CurrencyFixed = null;
		this.gc_ChnCode.CurrencyFixedRate = 0.0;
		this.gc_ChnCode.CurrencyRateField = null;
		this.gc_ChnCode.FieldName = "ChannelCode";
		this.gc_ChnCode.Name = "gc_ChnCode";
		this.gc_ChnCode.OptionsColumn.ReadOnly = true;
		this.gc_ChnCode.PrimaryKey = false;
		this.gc_ChnCode.UnitField = null;
		this.gc_Status1.Caption = "Trạng thái";
		this.gc_Status1.ColumnEdit = this.rep_Status1;
		this.gc_Status1.CurrencyDateField = null;
		this.gc_Status1.CurrencyField = null;
		this.gc_Status1.CurrencyFixed = null;
		this.gc_Status1.CurrencyFixedRate = 0.0;
		this.gc_Status1.CurrencyRateField = null;
		this.gc_Status1.FieldName = "Status";
		this.gc_Status1.Name = "gc_Status1";
		this.gc_Status1.PrimaryKey = false;
		this.gc_Status1.UnitField = null;
		this.gc_Status1.Visible = true;
		this.gc_Status1.VisibleIndex = 5;
		this.rep_Status1.AutoHeight = false;
		this.rep_Status1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Status1.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Name")
		});
		this.rep_Status1.DisplayMember = "Value";
		this.rep_Status1.Name = "rep_Status1";
		this.rep_Status1.NullText = "";
		this.rep_Status1.ShowHeader = false;
		this.rep_Status1.ValueMember = "Key";
		this.gc_ItmRemarks.Caption = "Diễn giải";
		this.gc_ItmRemarks.CurrencyDateField = null;
		this.gc_ItmRemarks.CurrencyField = null;
		this.gc_ItmRemarks.CurrencyFixed = null;
		this.gc_ItmRemarks.CurrencyFixedRate = 0.0;
		this.gc_ItmRemarks.CurrencyRateField = null;
		this.gc_ItmRemarks.FieldName = "Remarks";
		this.gc_ItmRemarks.Name = "gc_ItmRemarks";
		this.gc_ItmRemarks.PrimaryKey = false;
		this.gc_ItmRemarks.UnitField = null;
		this.gc_ItmRemarks.Visible = true;
		this.gc_ItmRemarks.VisibleIndex = 6;
		this.gc_ItmRemarks.Width = 137;
		this.gc_CreateDate1.Caption = "Create Date";
		this.gc_CreateDate1.CurrencyDateField = null;
		this.gc_CreateDate1.CurrencyField = null;
		this.gc_CreateDate1.CurrencyFixed = null;
		this.gc_CreateDate1.CurrencyFixedRate = 0.0;
		this.gc_CreateDate1.CurrencyRateField = null;
		this.gc_CreateDate1.FieldName = "CreateDate";
		this.gc_CreateDate1.Name = "gc_CreateDate1";
		this.gc_CreateDate1.OptionsColumn.ReadOnly = true;
		this.gc_CreateDate1.PrimaryKey = false;
		this.gc_CreateDate1.UnitField = null;
		this.gc_CreateTime1.Caption = "Create Time";
		this.gc_CreateTime1.CurrencyDateField = null;
		this.gc_CreateTime1.CurrencyField = null;
		this.gc_CreateTime1.CurrencyFixed = null;
		this.gc_CreateTime1.CurrencyFixedRate = 0.0;
		this.gc_CreateTime1.CurrencyRateField = null;
		this.gc_CreateTime1.FieldName = "CreateTime";
		this.gc_CreateTime1.Name = "gc_CreateTime1";
		this.gc_CreateTime1.OptionsColumn.ReadOnly = true;
		this.gc_CreateTime1.PrimaryKey = false;
		this.gc_CreateTime1.UnitField = null;
		this.gc_UpdateDate1.Caption = "Update Date";
		this.gc_UpdateDate1.CurrencyDateField = null;
		this.gc_UpdateDate1.CurrencyField = null;
		this.gc_UpdateDate1.CurrencyFixed = null;
		this.gc_UpdateDate1.CurrencyFixedRate = 0.0;
		this.gc_UpdateDate1.CurrencyRateField = null;
		this.gc_UpdateDate1.FieldName = "UpdateDate";
		this.gc_UpdateDate1.Name = "gc_UpdateDate1";
		this.gc_UpdateDate1.OptionsColumn.ReadOnly = true;
		this.gc_UpdateDate1.PrimaryKey = false;
		this.gc_UpdateDate1.UnitField = null;
		this.gc_UpdateTime1.Caption = "Update Time";
		this.gc_UpdateTime1.CurrencyDateField = null;
		this.gc_UpdateTime1.CurrencyField = null;
		this.gc_UpdateTime1.CurrencyFixed = null;
		this.gc_UpdateTime1.CurrencyFixedRate = 0.0;
		this.gc_UpdateTime1.CurrencyRateField = null;
		this.gc_UpdateTime1.FieldName = "UpdateTime";
		this.gc_UpdateTime1.Name = "gc_UpdateTime1";
		this.gc_UpdateTime1.OptionsColumn.ReadOnly = true;
		this.gc_UpdateTime1.PrimaryKey = false;
		this.gc_UpdateTime1.UnitField = null;
		this.repositoryItemCheckEdit1.AutoHeight = false;
		this.repositoryItemCheckEdit1.DisplayValueChecked = "Y";
		this.repositoryItemCheckEdit1.DisplayValueUnchecked = "N";
		this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
		this.repositoryItemCheckEdit1.ValueChecked = "Y";
		this.repositoryItemCheckEdit1.ValueUnchecked = "N";
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(2, 445);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 20);
		this.btn_OK.TabIndex = 8;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(79, 445);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 20);
		this.btn_Cancel.TabIndex = 9;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.lk_Status.EditControl = null;
		this.lk_Status.FieldName = null;
		this.lk_Status.IsSearchControl = false;
		this.lk_Status.Location = new System.Drawing.Point(93, 30);
		this.lk_Status.Name = "lk_Status";
		this.lk_Status.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lk_Status.Properties.Appearance.Options.UseFont = true;
		this.lk_Status.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(250, 249, 215);
		this.lk_Status.Properties.AppearanceFocused.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lk_Status.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.lk_Status.Properties.AppearanceFocused.Options.UseFont = true;
		this.lk_Status.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lk_Status.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.lk_Status.Properties.DisplayMember = "Name";
		this.lk_Status.Properties.NullText = "";
		this.lk_Status.Properties.ShowHeader = false;
		this.lk_Status.Properties.ValueMember = "Code";
		this.lk_Status.SettingInVisible = false;
		this.lk_Status.SettingReadonly = false;
		this.lk_Status.Size = new System.Drawing.Size(137, 20);
		this.lk_Status.TabIndex = 5;
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Tìm kiếm";
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = false;
		this.btn_Search.Location = new System.Drawing.Point(392, 30);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(75, 20);
		this.btn_Search.TabIndex = 6;
		this.btn_Search.Text = "Tìm kiếm";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.lbl_ChnlCode.EditControl = null;
		this.lbl_ChnlCode.EditValue = "Mã kênh";
		this.lbl_ChnlCode.FieldName = null;
		this.lbl_ChnlCode.HideWhenEmpty = false;
		this.lbl_ChnlCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_ChnlCode.Location = new System.Drawing.Point(5, 10);
		this.lbl_ChnlCode.Name = "lbl_ChnlCode";
		this.lbl_ChnlCode.ReadOnly = false;
		this.lbl_ChnlCode.SettingInVisible = false;
		this.lbl_ChnlCode.SettingReadonly = false;
		this.lbl_ChnlCode.ShowUnderLine = true;
		this.lbl_ChnlCode.Size = new System.Drawing.Size(91, 20);
		this.lbl_ChnlCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_ChnlCode.TabIndex = 0;
		this.lbl_ChnlCode.Utext = "Mã kênh";
		this.lbl_Status.EditControl = null;
		this.lbl_Status.EditValue = "Trạng thái";
		this.lbl_Status.FieldName = null;
		this.lbl_Status.HideWhenEmpty = false;
		this.lbl_Status.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Status.Location = new System.Drawing.Point(5, 33);
		this.lbl_Status.Name = "lbl_Status";
		this.lbl_Status.ReadOnly = false;
		this.lbl_Status.SettingInVisible = false;
		this.lbl_Status.SettingReadonly = false;
		this.lbl_Status.ShowUnderLine = true;
		this.lbl_Status.Size = new System.Drawing.Size(88, 20);
		this.lbl_Status.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Status.TabIndex = 4;
		this.lbl_Status.Utext = "Trạng thái";
		this.lbl_ChnnlName.EditControl = null;
		this.lbl_ChnnlName.EditValue = "Tên kênh";
		this.lbl_ChnnlName.FieldName = null;
		this.lbl_ChnnlName.HideWhenEmpty = false;
		this.lbl_ChnnlName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_ChnnlName.Location = new System.Drawing.Point(243, 7);
		this.lbl_ChnnlName.Name = "lbl_ChnnlName";
		this.lbl_ChnnlName.ReadOnly = false;
		this.lbl_ChnnlName.SettingInVisible = false;
		this.lbl_ChnnlName.SettingReadonly = false;
		this.lbl_ChnnlName.ShowUnderLine = true;
		this.lbl_ChnnlName.Size = new System.Drawing.Size(97, 20);
		this.lbl_ChnnlName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_ChnnlName.TabIndex = 2;
		this.lbl_ChnnlName.Utext = "Tên kênh";
		this.btn_ChnlCode.EditControl = null;
		this.btn_ChnlCode.FieldName = null;
		this.btn_ChnlCode.IsSearchControl = false;
		this.btn_ChnlCode.Location = new System.Drawing.Point(93, 7);
		this.btn_ChnlCode.Name = "btn_ChnlCode";
		this.btn_ChnlCode.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btn_ChnlCode.Properties.Appearance.Options.UseFont = true;
		this.btn_ChnlCode.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(250, 249, 215);
		this.btn_ChnlCode.Properties.AppearanceFocused.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btn_ChnlCode.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.btn_ChnlCode.Properties.AppearanceFocused.Options.UseFont = true;
		this.btn_ChnlCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.btn_ChnlCode.SettingInVisible = false;
		this.btn_ChnlCode.SettingReadonly = false;
		this.btn_ChnlCode.Size = new System.Drawing.Size(137, 20);
		this.btn_ChnlCode.TabIndex = 1;
		this.btn_ChnlCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btn_ChnlCode_ButtonClick);
		this.btn_ChnnlName.EditControl = null;
		this.btn_ChnnlName.FieldName = null;
		this.btn_ChnnlName.IsSearchControl = false;
		this.btn_ChnnlName.Location = new System.Drawing.Point(330, 4);
		this.btn_ChnnlName.Name = "btn_ChnnlName";
		this.btn_ChnnlName.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btn_ChnnlName.Properties.Appearance.Options.UseFont = true;
		this.btn_ChnnlName.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(250, 249, 215);
		this.btn_ChnnlName.Properties.AppearanceFocused.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btn_ChnnlName.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.btn_ChnnlName.Properties.AppearanceFocused.Options.UseFont = true;
		this.btn_ChnnlName.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.btn_ChnnlName.SettingInVisible = false;
		this.btn_ChnnlName.SettingReadonly = false;
		this.btn_ChnnlName.Size = new System.Drawing.Size(137, 20);
		this.btn_ChnnlName.TabIndex = 3;
		this.btn_ChnnlName.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btn_ChnnlName_ButtonClick);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(819, 472);
		base.Controls.Add(this.btn_ChnnlName);
		base.Controls.Add(this.btn_ChnlCode);
		base.Controls.Add(this.lbl_Status);
		base.Controls.Add(this.lbl_ChnnlName);
		base.Controls.Add(this.lbl_ChnlCode);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.lk_Status);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.splitContainerControl1);
		base.FormId = "oChannel";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_Channel";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
		this.Text = "Kênh bán hàng";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_Channel_FormClosing);
		base.Load += new System.EventHandler(this.frm_Channel_Load);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CreateDate.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CreateDate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CreateTime).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemHourEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_AllItm).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Lines).EndInit();
		this.contextMenuStrip2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Lines).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Active).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Item).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lk_Status.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ChnlCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ChnnlName.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
