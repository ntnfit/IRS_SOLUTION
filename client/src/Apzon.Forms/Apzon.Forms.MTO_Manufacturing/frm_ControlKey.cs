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
using Apzon.Forms.Commons.CFL;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.MTO_Manufacturing;

public class frm_ControlKey : XtraFormEx
{
	private DataTable _dtDocs = new DataTable();

	private DataTable _dtDoc = new DataTable();

	private string _entry = "";

	private bool _addRow;

	private int _rowHand = -1;

	private bool _edittingLine;

	private const string Controller = "ControlKey";

	private IContainer components;

	private Panel panel1;

	private SimpleButtonEx btRefresh;

	private SimpleButtonEx btCancel;

	private SimpleButtonEx btSave;

	private SplitContainerControl splitContainerControl1;

	private GroupControl groupControl1;

	private GridControlEx grdDocs;

	private GridViewEx gvDocs;

	private GridColumnEx clKeyCode;

	private GridColumnEx clKeyName;

	private GridColumnEx clType;

	private GridColumnEx clGroupID;

	private GridColumnEx clStatus;

	private GridColumnEx clConfirm;

	private GridColumnEx clRemark;

	private GroupControl groupControl2;

	private GridControlEx grdDoc;

	private GridViewEx gvDoc;

	private GridColumnEx clIndicatorCode;

	private GridColumnEx clChecked;

	private GridColumnEx clRemar;

	private GridColumnEx clKeyCod;

	private GridColumnEx clIndicatorName;

	private ContextMenuStrip contextDocs;

	private ToolStripMenuItem duplicateToolStripMenuItem;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private RepositoryItemButtonEdit repKeyCode;

	private RepositoryItemButtonEdit repGroupID;

	private RepositoryItemCheckEdit repcheck;

	private RepositoryItemLookUpEdit repStatus;

	private RepositoryItemLookUpEdit repType;

	private RepositoryItemMemoExEdit repositoryItemMemoExEdit1;

	public frm_ControlKey()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_ControlKey"))
		{
			InitializeComponent();
			FormClosing += frm_ControlKey_FormClosing;
			SettingLanguage();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_ControlKey" && x.authoriza == "F"))
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
		Text = Resources.Titel_frmControlKey;
		btSave.Text = Resources.ButtonSave;
		btCancel.Text = Resources.ButtonCancel;
		btRefresh.Text = Resources.ButtonRefresh;
		clKeyCode.Caption = Resources.lb_ControlKey;
		clKeyName.Caption = Resources.lb_Description;
		clType.Caption = Resources.lb_Type;
		clGroupID.Caption = Resources.lb_Group;
		clStatus.Caption = Resources.lb_Status;
		clConfirm.Caption = Resources.lb_Confirmations;
		clRemark.Caption = Resources.lb_Remark;
		clIndicatorName.Caption = Resources.lb_IndicatorName;
		clChecked.Caption = Resources.lb_Checked;
		clRemar.Caption = Resources.lb_Remark;
	}

	private void frm_ControlKey_Load(object sender, EventArgs e)
	{
		try
		{
			GetDocs();
			GetInItVaLue();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GetInItVaLue()
	{
		try
		{
			repcheck.ValueChecked = "Y";
			repcheck.ValueUnchecked = "N";
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
			repStatus.DataSource = list;
			List<Common.Table_ID> list2 = new List<Common.Table_ID>();
			list2.Add(new Common.Table_ID
			{
				Name = "Internal",
				ID = "I"
			});
			list2.Add(new Common.Table_ID
			{
				Name = "Other",
				ID = "O"
			});
			repType.DataSource = list2;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void GetDocs()
	{
		try
		{
			_dtDocs = await AwaitGetDocs();
			grdDocs.DataSource = _dtDocs;
			grdDocs.RefreshDataSource();
			AddNewRowDocs();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> AwaitGetDocs()
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ControlKey");
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

	private async void GetDoc()
	{
		try
		{
			string keycode = Function.ToString(gvDocs.GetRowCellValue(gvDocs.FocusedRowHandle, "KeyCode"));
			WinCommons._OpenProcessing();
			_dtDoc = await AwaitGetDocSaved(keycode);
			if (!_dtDoc.Columns.Contains("KeyCode"))
			{
				_dtDoc.Columns.Add("KeyCode", typeof(string)).SetOrdinal(0);
				_dtDoc.AcceptChanges();
				for (int i = 0; i < _dtDoc.Count(); i++)
				{
					_dtDoc.Rows[i]["KeyCode"] = keycode;
					_dtDoc.AcceptChanges();
				}
			}
			WinCommons._CloseProcessing();
			grdDoc.DataSource = _dtDoc;
			grdDoc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void AddNewRowDoc()
	{
		try
		{
			string text = Function.ToString(gvDocs.GetRowCellValue(gvDocs.FocusedRowHandle, "KeyCode"));
			DataTable dtTable = grdDoc.DataSource as DataTable;
			if (dtTable == null || dtTable.Count() == 0)
			{
				if (!dtTable.Columns.Contains("KeyCode"))
				{
					dtTable.Columns.Add("KeyCode", typeof(string));
				}
				if (!dtTable.Columns.Contains("IndicatorCode"))
				{
					dtTable.Columns.Add("IndicatorCode", typeof(int));
				}
				if (!dtTable.Columns.Contains("IndicatorName"))
				{
					dtTable.Columns.Add("IndicatorName", typeof(string));
				}
				if (!dtTable.Columns.Contains("Remark"))
				{
					dtTable.Columns.Add("Remark", typeof(string));
				}
				if (!dtTable.Columns.Contains("Checked"))
				{
					dtTable.Columns.Add("Checked", typeof(string));
				}
				DataRow dataRow = dtTable.NewRow();
				dataRow["KeyCode"] = text;
				dataRow["IndicatorCode"] = Function.ParseInt(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, "IndicatorCode")) + 1;
				dataRow["IndicatorName"] = "";
				dataRow["Remark"] = "";
				dataRow["Checked"] = "N";
				dtTable.Rows.Add(dataRow);
				dtTable.AcceptChanges();
				grdDoc.DataSource = dtTable;
				grdDocs.RefreshDataSource();
			}
			else if (!string.IsNullOrEmpty(Function.ToString(dtTable.Rows[dtTable.Count() - 1]["IndicatorName"])))
			{
				if (!dtTable.Columns.Contains("KeyCode"))
				{
					dtTable.Columns.Add("KeyCode", typeof(string));
				}
				if (!dtTable.Columns.Contains("IndicatorCode"))
				{
					dtTable.Columns.Add("IndicatorCode", typeof(int));
				}
				if (!dtTable.Columns.Contains("IndicatorName"))
				{
					dtTable.Columns.Add("IndicatorName", typeof(string));
				}
				if (!dtTable.Columns.Contains("Remark"))
				{
					dtTable.Columns.Add("Remark", typeof(string));
				}
				if (!dtTable.Columns.Contains("Checked"))
				{
					dtTable.Columns.Add("Checked", typeof(string));
				}
				DataRow row = dtTable.NewRow();
				row["KeyCode"] = text;
				if (_addRow)
				{
					row["IndicatorCode"] = Function.ParseInt(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, "IndicatorCode")) + 1;
					_addRow = false;
				}
				else
				{
					DataRow dataRow2 = row;
					dataRow2["IndicatorCode"] = await GetMaxIndicatorCode(text) + 1;
				}
				row["IndicatorName"] = "";
				row["Remark"] = "";
				row["Checked"] = "N";
				dtTable.Rows.Add(row);
				dtTable.AcceptChanges();
				grdDoc.DataSource = dtTable;
				grdDocs.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
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

	private async Task<int> GetMaxIndicatorCode(string keycode)
	{
		try
		{
			int result;
			using (HttpClientHelper<ControlKeyImpl> _helper = new HttpClientHelper<ControlKeyImpl>(Configuration.ApiServer, "ControlKey"))
			{
				result = await _helper.GetIntAsync("GetMaxIndicatorCode", new { keycode });
			}
			return result;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return 0;
		}
	}

	private async Task<DataTable> AwaitGetDoc()
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ControlKey");
			DataTable dataTable = await _helper.GetDataTableAsync("GetDoc");
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

	private async Task<DataTable> AwaitGetDocSaved(string keycode)
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ControlKey");
			DataTable dataTable = await _helper.PostAsync("GetDocSaved", keycode);
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

	private async void DeleteRowDoc()
	{
		try
		{
			string value = Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, "KeyCode"));
			int num = Function.ParseInt(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, "IndicatorCode"));
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("KeyCode", typeof(string));
			dataTable.Columns.Add("IndicatorCode", typeof(int));
			DataRow dataRow = dataTable.NewRow();
			dataRow["KeyCode"] = value;
			dataRow["IndicatorCode"] = num;
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			if (await DeleteDoc(dataTable) != "")
			{
				XtraMessageBox.Show("Delete Error!");
			}
			else
			{
				XtraMessageBox.Show("Delete Success!");
			}
			GetDoc();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<string> DeleteDoc(DataTable dts)
	{
		try
		{
			string result;
			using (HttpClientHelper<ControlKeyImpl> _helper = new HttpClientHelper<ControlKeyImpl>(Configuration.ApiServer, "ControlKey"))
			{
				result = await _helper.PostParamsAsJsonAsync("DeleteDoc", dts);
			}
			return result;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return "";
		}
	}

	private async void DeleteRowDocEmpty(string ids)
	{
		try
		{
			using HttpClientHelper<ControlKeyImpl> _helper = new HttpClientHelper<ControlKeyImpl>(Configuration.ApiServer, "ControlKey");
			string text = await _helper.PostParamsAsJsonAsync("DeleteDocsAlone", ids);
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				XtraMessageBox.Show("Delete Success !");
				GetDocs();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
				XtraMessageBox.Show("Delete Error !");
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void DeleteRow(string ids)
	{
		try
		{
			using HttpClientHelper<ControlKeyImpl> _helper = new HttpClientHelper<ControlKeyImpl>(Configuration.ApiServer, "ControlKey");
			string text = await _helper.PostParamsAsJsonAsync("DeleteDocs", ids);
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Success(Enums.MessageCode.DeleteSuccess, "");
				GetDocs();
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

	private async Task<string> WaitSaveData(DataSet dtFather)
	{
		try
		{
			using HttpClientHelper<ControlKeyImpl> _helper = new HttpClientHelper<ControlKeyImpl>(Configuration.ApiServer, "ControlKey");
			return await _helper.PostParamsAsJsonAsync("SaveDocs", dtFather);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return null;
		}
	}

	private void gvDocs_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			if (_rowHand != gvDocs.FocusedRowHandle && _edittingLine)
			{
				DialogResult dialogResult = XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				if (DialogResult.Yes == dialogResult)
				{
					_rowHand = gvDocs.FocusedRowHandle;
					GetDoc();
					_edittingLine = false;
				}
				else
				{
					gvDocs.FocusedRowHandle = _rowHand;
				}
			}
			else
			{
				_rowHand = gvDocs.FocusedRowHandle;
				GetDoc();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void repGroupID_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = grdDocs.DataSource as DataTable;
			ButtonEdit buttonEdit = sender as ButtonEdit;
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.GroupControlKey, multiSelect: false, "");
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				GroupControlKeyImpl groupControlKeyImpl = (GroupControlKeyImpl)frm_ChooseFromList2.ChooseItem;
				dataTable.Rows[gvDocs.GetDataSourceRowIndex(gvDocs.FocusedRowHandle)]["GroupID"] = groupControlKeyImpl.Code;
				dataTable.AcceptChanges();
				buttonEdit.EditValue = groupControlKeyImpl.Code;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Warning, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void gvDocs_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gvDoc_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private async void btSave_Click(object sender, EventArgs e)
	{
		try
		{
			if (!string.IsNullOrEmpty(Function.ToString(gvDocs.GetFocusedRowCellValue("KeyCode"))))
			{
				if (DialogResult.Yes != XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo))
				{
					return;
				}
				WinCommons._OpenProcessing();
				_edittingLine = false;
				DataTable dataTable = grdDocs.DataSource as DataTable;
				DataTable dataTable2 = grdDoc.DataSource as DataTable;
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					for (int i = 0; i < dataTable.Count(); i++)
					{
						if (string.IsNullOrEmpty(Function.ToString(dataTable.Rows[i]["KeyCode"])))
						{
							dataTable.Rows.RemoveAt(i);
							i--;
						}
					}
					dataTable.AcceptChanges();
				}
				if (dataTable2 != null && dataTable2.Rows.Count > 0)
				{
					for (int j = 0; j < dataTable2.Count(); j++)
					{
						if (string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[j]["IndicatorName"])))
						{
							dataTable2.Rows.RemoveAt(j);
							j--;
						}
					}
					dataTable2.AcceptChanges();
				}
				DataTable dataTable3 = new DataTable();
				if (!dataTable3.Columns.Contains("KeyCode"))
				{
					dataTable3.Columns.Add("KeyCode", typeof(string));
				}
				if (!dataTable3.Columns.Contains("IndicatorCode"))
				{
					dataTable3.Columns.Add("IndicatorCode", typeof(string));
				}
				if (!dataTable3.Columns.Contains("IndicatorName"))
				{
					dataTable3.Columns.Add("IndicatorName", typeof(string));
				}
				if (!dataTable3.Columns.Contains("Remark"))
				{
					dataTable3.Columns.Add("Remark", typeof(string));
				}
				if (!dataTable3.Columns.Contains("Checked"))
				{
					dataTable3.Columns.Add("Checked", typeof(string));
				}
				for (int k = 0; k < dataTable2.Rows.Count; k++)
				{
					DataRow dataRow = dataTable3.NewRow();
					DataRow dataRow2 = dataTable2.Rows[k];
					dataRow["KeyCode"] = gvDocs.GetRowCellValue(gvDocs.FocusedRowHandle, "KeyCode");
					dataRow["IndicatorCode"] = dataRow2["IndicatorCode"];
					dataRow["IndicatorName"] = dataRow2["IndicatorName"];
					dataRow["Remark"] = dataRow2["Remark"];
					dataRow["Checked"] = dataRow2["Checked"];
					dataTable3.Rows.Add(dataRow);
					dataTable3.AcceptChanges();
				}
				DataSet ds = new DataSet();
				ds.Tables.AddRange(new DataTable[2] { dataTable, dataTable3 });
				string text = await WaitSaveData(ds);
				ds.Reset();
				ds.Clear();
				if (text == "")
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					GetDocs();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
				WinCommons._CloseProcessing();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.ErrorRowIsSelectedHaveKeyEmpty, "");
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void deleteRowToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		try
		{
			DialogResult dialogResult = XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteThisRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			if (DialogResult.Yes == dialogResult)
			{
				DeleteRowDoc();
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

	private void groupControl1_Paint(object sender, PaintEventArgs e)
	{
	}

	private void btRefresh_Click(object sender, EventArgs e)
	{
		try
		{
			GetDocs();
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
			DataTable dataTable = grdDocs.DataSource as DataTable;
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				dataTable.Columns.Add("KeyCode", typeof(string));
				dataTable.Columns.Add("KeyName", typeof(string));
				dataTable.Columns.Add("Type", typeof(string));
				dataTable.Columns.Add("GroupID", typeof(string));
				dataTable.Columns.Add("Status", typeof(string));
				dataTable.Columns.Add("Confirm", typeof(string));
				dataTable.Columns.Add("Remark", typeof(string));
			}
			DataRow dataRow = dataTable.NewRow();
			dataRow["KeyCode"] = "";
			dataRow["KeyName"] = "";
			dataRow["Type"] = "I";
			dataRow["GroupID"] = "";
			dataRow["Status"] = "Y";
			dataRow["Confirm"] = "";
			dataRow["Remark"] = "";
			dataTable.Rows.Add(dataRow);
			grdDocs.DataSource = dataTable;
			grdDocs.RefreshDataSource();
			gvDocs.FocusedRowHandle = dataTable.Rows.Count - 1;
			GridColumn focusedColumn = gvDocs.Columns.Where((GridColumn p) => p.FieldName == "KeyCode").FirstOrDefault();
			gvDocs.FocusedColumn = focusedColumn;
			gvDocs.ShowEditor();
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
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				dataTable.Columns.Add("KeyCode", typeof(string));
				dataTable.Columns.Add("KeyName", typeof(string));
				dataTable.Columns.Add("Type", typeof(string));
				dataTable.Columns.Add("GroupID", typeof(string));
				dataTable.Columns.Add("Status", typeof(string));
				dataTable.Columns.Add("Confirm", typeof(string));
				dataTable.Columns.Add("Remark", typeof(string));
				DataRow dataRow = dataTable.NewRow();
				dataRow["KeyCode"] = "";
				dataRow["KeyName"] = "";
				dataRow["Type"] = "I";
				dataRow["GroupID"] = "";
				dataRow["Status"] = "Y";
				dataRow["Confirm"] = "";
				dataRow["Remark"] = "";
				dataTable.Rows.Add(dataRow);
				grdDocs.DataSource = dataTable;
				grdDocs.RefreshDataSource();
			}
			else if (!string.IsNullOrEmpty(Function.ToString(gvDocs.GetRowCellValue(gvDocs.RowCount - 1, "KeyCode"))))
			{
				DataRow dataRow2 = dataTable.NewRow();
				dataRow2["KeyCode"] = "";
				dataRow2["KeyName"] = "";
				dataRow2["Type"] = "I";
				dataRow2["GroupID"] = "";
				dataRow2["Status"] = "Y";
				dataRow2["Confirm"] = "";
				dataRow2["Remark"] = "";
				dataTable.Rows.Add(dataRow2);
				grdDocs.DataSource = dataTable;
				grdDocs.RefreshDataSource();
			}
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
			DataTable dataTable = grdDocs.DataSource as DataTable;
			DataRow dataRow = dataTable.NewRow();
			dataRow["KeyCode"] = "";
			dataRow["KeyName"] = "";
			dataRow["Type"] = gvDocs.GetFocusedRowCellValue("Type");
			dataRow["GroupID"] = gvDocs.GetFocusedRowCellValue("GroupID");
			dataRow["Status"] = gvDocs.GetFocusedRowCellValue("Status");
			dataRow["Confirm"] = gvDocs.GetFocusedRowCellValue("Confirm");
			dataRow["Remark"] = "";
			dataTable.Rows.InsertAt(dataRow, gvDocs.RowCount - 1);
			grdDocs.DataSource = dataTable;
			grdDocs.RefreshDataSource();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void repKeyCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			ButtonEdit buttonEdit = sender as ButtonEdit;
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.ControlKey, multiSelect: false, "");
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				ControlKeyImpl controlKeyImpl = (ControlKeyImpl)frm_ChooseFromList2.ChooseItem;
				DataRow dataRow = _dtDocs.Rows[gvDocs.GetDataSourceRowIndex(gvDocs.FocusedRowHandle)];
				dataRow["KeyCode"] = controlKeyImpl.KeyCode;
				dataRow["KeyName"] = controlKeyImpl.KeyName;
				_dtDocs.AcceptChanges();
				buttonEdit.EditValue = controlKeyImpl.KeyCode;
			}
			AddNewRowDoc();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Warning, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void gvDocs_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			_edittingLine = true;
			_ = grdDocs.DataSource;
			if (e.Column.FieldName == "KeyCode")
			{
				if (!string.IsNullOrEmpty(Function.ToString(gvDocs.GetFocusedRowCellValue("KeyCode"))))
				{
					AddNewRowDocs();
				}
				else
				{
					DeleteRowEndEmpty();
				}
			}
			string text = Function.ToString(gvDocs.GetRowCellValue(gvDocs.FocusedRowHandle, "KeyCode"));
			if (!(_entry == text))
			{
				text = _entry;
				WinCommons._OpenProcessing();
				GetDoc();
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvDoc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			_edittingLine = true;
			if (e.Column.FieldName == "IndicatorName")
			{
				_addRow = true;
				AddNewRowDoc();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (gvDocs.GetSelectedRows() == null || gvDocs.GetSelectedRows().Count() == 0)
			{
				WinCommons.Error(Enums.MessageCode.NoRecord, "");
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				string keyCode = Function.ToString(gvDocs.GetFocusedRowCellValue("KeyCode"));
				if (string.IsNullOrEmpty(keyCode))
				{
					WinCommons.Error(Enums.MessageCode.RowIsEmpty, "");
				}
				else if (Function.ParseInt((await CheckExistAsync(keyCode)).Rows[0][0]) == 0)
				{
					DeleteRow(keyCode);
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.ThisControlKeyCodeExistedInRoutingOrPlanning, "");
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> CheckExistAsync(string keycode)
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ControlKey");
			return await _helper.PostAsync("CheckExistKeyCode", keycode);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

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

	private void gvDoc_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			if (e.RowHandle == gvDoc.FocusedRowHandle)
			{
				e.Appearance.BackColor = StyleColor.GridEditting;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_ControlKey_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (_edittingLine)
			{
				if (XtraMessageBox.Show(this, "Are you sure?", "Do you still want ... ?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
				{
					e.Cancel = true;
				}
				_edittingLine = false;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvDocs_ShowingEditor(object sender, CancelEventArgs e)
	{
	}

	private void gvDocs_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.MTO_Manufacturing.frm_ControlKey));
		this.panel1 = new System.Windows.Forms.Panel();
		this.btRefresh = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btSave = new Apzon.Controls.Controls.SimpleButtonEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
		this.grdDocs = new Apzon.Controls.Grid.GridControlEx();
		this.contextDocs = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.duplicateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvDocs = new Apzon.Controls.Grid.GridViewEx();
		this.clKeyCode = new Apzon.Controls.Grid.GridColumnEx();
		this.clKeyName = new Apzon.Controls.Grid.GridColumnEx();
		this.clType = new Apzon.Controls.Grid.GridColumnEx();
		this.repType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.clGroupID = new Apzon.Controls.Grid.GridColumnEx();
		this.repGroupID = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.clStatus = new Apzon.Controls.Grid.GridColumnEx();
		this.repStatus = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.clConfirm = new Apzon.Controls.Grid.GridColumnEx();
		this.clRemark = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoExEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.repKeyCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.clKeyCod = new Apzon.Controls.Grid.GridColumnEx();
		this.clIndicatorName = new Apzon.Controls.Grid.GridColumnEx();
		this.clIndicatorCode = new Apzon.Controls.Grid.GridColumnEx();
		this.clChecked = new Apzon.Controls.Grid.GridColumnEx();
		this.repcheck = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.clRemar = new Apzon.Controls.Grid.GridColumnEx();
		this.panel1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.groupControl1).BeginInit();
		this.groupControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdDocs).BeginInit();
		this.contextDocs.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvDocs).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repGroupID).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repStatus).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repKeyCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl2).BeginInit();
		this.groupControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repcheck).BeginInit();
		base.SuspendLayout();
		this.panel1.Controls.Add(this.btRefresh);
		this.panel1.Controls.Add(this.btCancel);
		this.panel1.Controls.Add(this.btSave);
		this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panel1.Location = new System.Drawing.Point(0, 278);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(574, 33);
		this.panel1.TabIndex = 53;
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
		this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.groupControl1);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.groupControl2);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(574, 278);
		this.splitContainerControl1.SplitterPosition = 149;
		this.splitContainerControl1.TabIndex = 54;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.groupControl1.Controls.Add(this.grdDocs);
		this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.groupControl1.Location = new System.Drawing.Point(0, 0);
		this.groupControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.groupControl1.Name = "groupControl1";
		this.groupControl1.Size = new System.Drawing.Size(574, 149);
		this.groupControl1.TabIndex = 0;
		this.groupControl1.Text = "General";
		this.groupControl1.Paint += new System.Windows.Forms.PaintEventHandler(this.groupControl1_Paint);
		this.grdDocs.AllowPasteExcel = true;
		this.grdDocs.ClipboardData = null;
		this.grdDocs.ContextMenuStrip = this.contextDocs;
		this.grdDocs.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdDocs.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grdDocs.Location = new System.Drawing.Point(2, 23);
		this.grdDocs.MainView = this.gvDocs;
		this.grdDocs.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grdDocs.Name = "grdDocs";
		this.grdDocs.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.repKeyCode, this.repGroupID, this.repStatus, this.repType, this.repositoryItemMemoExEdit1 });
		this.grdDocs.Size = new System.Drawing.Size(570, 124);
		this.grdDocs.TabIndex = 0;
		this.grdDocs.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDocs });
		this.contextDocs.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.contextDocs.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.duplicateToolStripMenuItem, this.deleteRowToolStripMenuItem });
		this.contextDocs.Name = "contextMenuStrip1";
		this.contextDocs.Size = new System.Drawing.Size(134, 48);
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
		this.gvDocs.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.clKeyCode, this.clKeyName, this.clType, this.clGroupID, this.clStatus, this.clConfirm, this.clRemark });
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
		this.gvDocs.OptionsSelection.MultiSelect = true;
		this.gvDocs.OptionsView.ColumnAutoWidth = false;
		this.gvDocs.OptionsView.ShowGroupPanel = false;
		this.gvDocs.UseBatchMod = false;
		this.gvDocs.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvDocs_CustomDrawRowIndicator);
		this.gvDocs.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gvDocs_RowStyle);
		this.gvDocs.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gvDocs_CustomRowCellEdit);
		this.gvDocs.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gvDocs_ShowingEditor);
		this.gvDocs.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gvDocs_FocusedRowObjectChanged);
		this.gvDocs.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDocs_CellValueChanged);
		this.clKeyCode.Caption = "Control Key";
		this.clKeyCode.CurrencyDateField = null;
		this.clKeyCode.CurrencyField = null;
		this.clKeyCode.CurrencyFixed = null;
		this.clKeyCode.CurrencyFixedRate = 0.0;
		this.clKeyCode.CurrencyRateField = null;
		this.clKeyCode.FieldName = "KeyCode";
		this.clKeyCode.Name = "clKeyCode";
		this.clKeyCode.PrimaryKey = true;
		this.clKeyCode.UnitField = null;
		this.clKeyCode.Visible = true;
		this.clKeyCode.VisibleIndex = 0;
		this.clKeyCode.Width = 85;
		this.clKeyName.Caption = "Description";
		this.clKeyName.CurrencyDateField = null;
		this.clKeyName.CurrencyField = null;
		this.clKeyName.CurrencyFixed = null;
		this.clKeyName.CurrencyFixedRate = 0.0;
		this.clKeyName.CurrencyRateField = null;
		this.clKeyName.FieldName = "KeyName";
		this.clKeyName.Name = "clKeyName";
		this.clKeyName.PrimaryKey = false;
		this.clKeyName.UnitField = null;
		this.clKeyName.Visible = true;
		this.clKeyName.VisibleIndex = 1;
		this.clKeyName.Width = 174;
		this.clType.Caption = "Type";
		this.clType.ColumnEdit = this.repType;
		this.clType.CurrencyDateField = null;
		this.clType.CurrencyField = null;
		this.clType.CurrencyFixed = null;
		this.clType.CurrencyFixedRate = 0.0;
		this.clType.CurrencyRateField = null;
		this.clType.FieldName = "Type";
		this.clType.Name = "clType";
		this.clType.PrimaryKey = false;
		this.clType.UnitField = null;
		this.clType.Visible = true;
		this.clType.VisibleIndex = 2;
		this.repType.AutoHeight = false;
		this.repType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repType.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "ID", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Type")
		});
		this.repType.DisplayMember = "Name";
		this.repType.Name = "repType";
		this.repType.ValueMember = "ID";
		this.clGroupID.Caption = "Group";
		this.clGroupID.ColumnEdit = this.repGroupID;
		this.clGroupID.CurrencyDateField = null;
		this.clGroupID.CurrencyField = null;
		this.clGroupID.CurrencyFixed = null;
		this.clGroupID.CurrencyFixedRate = 0.0;
		this.clGroupID.CurrencyRateField = null;
		this.clGroupID.FieldName = "GroupID";
		this.clGroupID.Name = "clGroupID";
		this.clGroupID.PrimaryKey = false;
		this.clGroupID.UnitField = null;
		this.clGroupID.Visible = true;
		this.clGroupID.VisibleIndex = 3;
		this.repGroupID.AutoHeight = false;
		this.repGroupID.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.repGroupID.Name = "repGroupID";
		this.repGroupID.Click += new System.EventHandler(this.repGroupID_Click);
		this.clStatus.Caption = "Status";
		this.clStatus.ColumnEdit = this.repStatus;
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
		this.clStatus.VisibleIndex = 4;
		this.repStatus.AutoHeight = false;
		this.repStatus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repStatus.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Status"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "ID")
		});
		this.repStatus.DisplayMember = "Name";
		this.repStatus.Name = "repStatus";
		this.repStatus.ValueMember = "ID";
		this.clConfirm.Caption = "Confirmations";
		this.clConfirm.CurrencyDateField = null;
		this.clConfirm.CurrencyField = null;
		this.clConfirm.CurrencyFixed = null;
		this.clConfirm.CurrencyFixedRate = 0.0;
		this.clConfirm.CurrencyRateField = null;
		this.clConfirm.FieldName = "Confirm";
		this.clConfirm.Name = "clConfirm";
		this.clConfirm.PrimaryKey = false;
		this.clConfirm.UnitField = null;
		this.clConfirm.Visible = true;
		this.clConfirm.VisibleIndex = 5;
		this.clConfirm.Width = 90;
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
		this.clRemark.VisibleIndex = 6;
		this.clRemark.Width = 165;
		this.repositoryItemMemoExEdit1.AutoHeight = false;
		this.repositoryItemMemoExEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit1.Name = "repositoryItemMemoExEdit1";
		this.repositoryItemMemoExEdit1.ShowIcon = false;
		this.repKeyCode.AutoHeight = false;
		this.repKeyCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.repKeyCode.Name = "repKeyCode";
		this.repKeyCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repKeyCode_ButtonClick);
		this.groupControl2.Controls.Add(this.grdDoc);
		this.groupControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.groupControl2.Location = new System.Drawing.Point(0, 0);
		this.groupControl2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.groupControl2.Name = "groupControl2";
		this.groupControl2.Size = new System.Drawing.Size(574, 119);
		this.groupControl2.TabIndex = 1;
		this.groupControl2.Text = "Indicator";
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdDoc.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grdDoc.Location = new System.Drawing.Point(2, 23);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.repcheck });
		this.grdDoc.Size = new System.Drawing.Size(570, 94);
		this.grdDoc.TabIndex = 1;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.clKeyCod, this.clIndicatorName, this.clIndicatorCode, this.clChecked, this.clRemar });
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = null;
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 40;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.IsMappingContextAction = false;
		this.gvDoc.ItemId = null;
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsView.ColumnAutoWidth = false;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvDoc_CustomDrawRowIndicator);
		this.gvDoc.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gvDoc_RowStyle);
		this.gvDoc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanged);
		this.clKeyCod.Caption = "KeyCode";
		this.clKeyCod.CurrencyDateField = null;
		this.clKeyCod.CurrencyField = null;
		this.clKeyCod.CurrencyFixed = null;
		this.clKeyCod.CurrencyFixedRate = 0.0;
		this.clKeyCod.CurrencyRateField = null;
		this.clKeyCod.FieldName = "KeyCode";
		this.clKeyCod.Name = "clKeyCod";
		this.clKeyCod.PrimaryKey = true;
		this.clKeyCod.UnitField = null;
		this.clKeyCod.Width = 176;
		this.clIndicatorName.Caption = "IndicatorName";
		this.clIndicatorName.CurrencyDateField = null;
		this.clIndicatorName.CurrencyField = null;
		this.clIndicatorName.CurrencyFixed = null;
		this.clIndicatorName.CurrencyFixedRate = 0.0;
		this.clIndicatorName.CurrencyRateField = null;
		this.clIndicatorName.FieldName = "IndicatorName";
		this.clIndicatorName.Name = "clIndicatorName";
		this.clIndicatorName.OptionsColumn.ReadOnly = true;
		this.clIndicatorName.PrimaryKey = false;
		this.clIndicatorName.UnitField = null;
		this.clIndicatorName.Visible = true;
		this.clIndicatorName.VisibleIndex = 1;
		this.clIndicatorName.Width = 166;
		this.clIndicatorCode.Caption = "Indicator Code";
		this.clIndicatorCode.CurrencyDateField = null;
		this.clIndicatorCode.CurrencyField = null;
		this.clIndicatorCode.CurrencyFixed = null;
		this.clIndicatorCode.CurrencyFixedRate = 0.0;
		this.clIndicatorCode.CurrencyRateField = null;
		this.clIndicatorCode.FieldName = "IndicatorCode";
		this.clIndicatorCode.Name = "clIndicatorCode";
		this.clIndicatorCode.OptionsColumn.ReadOnly = true;
		this.clIndicatorCode.PrimaryKey = false;
		this.clIndicatorCode.UnitField = null;
		this.clIndicatorCode.Visible = true;
		this.clIndicatorCode.VisibleIndex = 0;
		this.clIndicatorCode.Width = 176;
		this.clChecked.Caption = "Checked";
		this.clChecked.ColumnEdit = this.repcheck;
		this.clChecked.CurrencyDateField = null;
		this.clChecked.CurrencyField = null;
		this.clChecked.CurrencyFixed = null;
		this.clChecked.CurrencyFixedRate = 0.0;
		this.clChecked.CurrencyRateField = null;
		this.clChecked.FieldName = "Checked";
		this.clChecked.Name = "clChecked";
		this.clChecked.PrimaryKey = false;
		this.clChecked.UnitField = null;
		this.clChecked.Visible = true;
		this.clChecked.VisibleIndex = 3;
		this.clChecked.Width = 59;
		this.repcheck.AutoHeight = false;
		this.repcheck.Name = "repcheck";
		this.repcheck.ValueChecked = "Y";
		this.repcheck.ValueUnchecked = "N";
		this.clRemar.Caption = "Remark";
		this.clRemar.CurrencyDateField = null;
		this.clRemar.CurrencyField = null;
		this.clRemar.CurrencyFixed = null;
		this.clRemar.CurrencyFixedRate = 0.0;
		this.clRemar.CurrencyRateField = null;
		this.clRemar.FieldName = "Remark";
		this.clRemar.Name = "clRemar";
		this.clRemar.OptionsColumn.ReadOnly = true;
		this.clRemar.PrimaryKey = false;
		this.clRemar.UnitField = null;
		this.clRemar.Visible = true;
		this.clRemar.VisibleIndex = 2;
		this.clRemar.Width = 212;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(574, 311);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.panel1);
		base.IconOptions.ShowIcon = false;
		base.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		base.Name = "frm_ControlKey";
		base.ShowInTaskbar = false;
		this.Text = "Control Key Overview";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_ControlKey_FormClosing);
		base.Load += new System.EventHandler(this.frm_ControlKey_Load);
		this.panel1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.groupControl1).EndInit();
		this.groupControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdDocs).EndInit();
		this.contextDocs.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvDocs).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repGroupID).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repStatus).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repKeyCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl2).EndInit();
		this.groupControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repcheck).EndInit();
		base.ResumeLayout(false);
	}
}
