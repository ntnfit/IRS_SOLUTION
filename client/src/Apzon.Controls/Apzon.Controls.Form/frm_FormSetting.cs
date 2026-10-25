using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls.Grid;
using Apzon.Controls.Tree;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace Apzon.Controls.Form;

public class frm_FormSetting : XtraFormEx
{
	private GridControlEx _grdFormSetting;

	private GridColumnEx _gridColumn1;

	private GridColumnEx _gridColumn10;

	private GridColumnEx _gridColumn11;

	private GridColumnEx _gridColumn2;

	private GridColumnEx _gridColumn3;

	private GridColumnEx _gridColumn4;

	private GridColumnEx gcWidth;

	private GridColumnEx _gridColumn6;

	private GridColumnEx _gridColumn7;

	private GridColumnEx _gridColumn8;

	private RepositoryItemCalcEdit _repositoryItemCalcEdit1;

	private SimpleButton btn_Cancel;

	private SimpleButton btn_CopyFormSetting;

	private SimpleButton btn_OK;

	private IContainer components;

	private GridHitInfo downHitInfo;

	private GridColumnEx gcFixed;

	private GridViewEx gv_FormSetting;

	private ContextMenuStrip contextMenuStrip;

	private ToolStripMenuItem restoreDefaultToolStripMenuItem;

	private ToolStripMenuItem restoreDefaultAllUserToolStripMenuItem;

	private ToolStripMenuItem copyToolStripMenuItem;

	private RepositoryItemLookUpEdit rep_Fixed;

	private GridColumnEx cl_IsReadOnly;

	private GridColumnEx gcGroupIndex;

	private RepositoryItemCalcEdit repositoryItemCalcEdit1;

	private GridColumnEx gc_CurrField;

	public bool ReloadAfterUpdate = true;

	public bool IsSettingForm;

	public Enums.ChangeLog ChangeLogType = Enums.ChangeLog.clFormSetting;

	public DataTable DtSource { get; set; }

	public GridControlEx Grid { get; set; }

	public TreeEx Tree { get; set; }

	public BandedGridViewEx BandView { get; set; }

	public Tuple<TreeEx, string> ItemTree { get; set; }

	public Tuple<GridViewEx, string> Item { get; set; }

	public string FormSettingId { get; set; }

	private bool IsUpdate { get; set; }

	public XtraFormEx BaseForm { get; set; }

	public frm_FormSetting()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_FormSetting"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_FormSetting" && x.authoriza == "F"))
			{
				Controls.DisableAllControl();
				btn_Cancel.Enabled = true;
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private void DrawCheckBoxHeader(string caption, Graphics graphic, Rectangle rec, bool isCheck)
	{
		try
		{
			CheckEditViewInfo checkEditViewInfo = null;
			CheckEditPainter checkEditPainter = null;
			ControlGraphicsInfoArgs controlGraphicsInfoArgs = null;
			RepositoryItemCheckEdit repositoryItemCheckEdit = new RepositoryItemCheckEdit();
			checkEditViewInfo = repositoryItemCheckEdit.CreateViewInfo() as CheckEditViewInfo;
			checkEditPainter = repositoryItemCheckEdit.CreatePainter() as CheckEditPainter;
			repositoryItemCheckEdit.Caption = caption;
			repositoryItemCheckEdit.GlyphAlignment = HorzAlignment.Near;
			repositoryItemCheckEdit.Caption = caption;
			if (checkEditViewInfo != null && checkEditPainter != null)
			{
				checkEditViewInfo.EditValue = isCheck;
				checkEditViewInfo.Bounds = rec;
				checkEditViewInfo.PaintAppearance.ForeColor = Color.Black;
				checkEditViewInfo.CalcViewInfo(graphic);
				controlGraphicsInfoArgs = new ControlGraphicsInfoArgs(checkEditViewInfo, new GraphicsCache(graphic), rec);
				checkEditPainter.Draw(controlGraphicsInfoArgs);
				controlGraphicsInfoArgs.Cache.Dispose();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ShowCheckBoxHeader(ColumnHeaderCustomDrawEventArgs e)
	{
		try
		{
			bool isCheck = IsAllRowChecked(e.Column.FieldName);
			e.Info.Caption = string.Empty;
			e.Info.InnerElements.Clear();
			e.Info.Appearance.ForeColor = Color.Black;
			e.Painter.DrawObject(e.Info);
			DrawCheckBoxHeader(e.Column.Caption, e.Graphics, e.Bounds, isCheck);
			e.Handled = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void UpdateCheckAllValue(string fieldName, bool allowCheck, bool checkValue)
	{
		try
		{
			if (!allowCheck)
			{
				return;
			}
			for (int i = 0; i < gv_FormSetting.RowCount; i++)
			{
				DataRow dataRow = gv_FormSetting.GetDataRow(i);
				if (!Function.ParseBool(dataRow["AllowChSet"]))
				{
					continue;
				}
				if (fieldName.Equals("Visible"))
				{
					if (checkValue)
					{
						if (!Function.ParseBool(dataRow["NeverEditable"]))
						{
							dataRow["Editable"] = true;
							dataRow["IsReadOnly"] = false;
						}
					}
					else
					{
						dataRow["Editable"] = false;
						dataRow["IsReadOnly"] = false;
					}
				}
				else if (fieldName.Equals("Editable"))
				{
					if (!Function.ParseBool(dataRow["Visible"]))
					{
						dataRow["Editable"] = !checkValue;
						dataRow["IsReadOnly"] = checkValue;
					}
					else
					{
						dataRow["Editable"] = checkValue;
						dataRow["IsReadOnly"] = !checkValue;
					}
				}
				else if (fieldName.Equals("IsReadOnly"))
				{
					dataRow["Editable"] = !checkValue;
				}
				dataRow[fieldName] = checkValue;
			}
			_grdFormSetting.DataSourceTable.AcceptChanges();
			_grdFormSetting.RefreshDataSource();
			FormMode = Enums.FormViewModeType.Update;
			btn_OK.Text = Resources.ButtonUpdate;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private bool IsAllRowChecked(string fieldName)
	{
		try
		{
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < gv_FormSetting.RowCount; i++)
			{
				DataRow dataRow = gv_FormSetting.GetDataRow(i);
				if (gv_FormSetting.ColumnsEx[fieldName].ColumnType == typeof(bool) && Function.ParseBool(dataRow[fieldName]))
				{
					num++;
				}
				num2++;
			}
			return num == num2;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return false;
		}
	}

	private void frm_FormSetting_Load(object sender, EventArgs e)
	{
		try
		{
			btn_OK.Text = ControlText.ButtonOk;
			FormMode = Enums.FormViewModeType.View;
			_grdFormSetting.DataSource = DtSource;
			gv_FormSetting.OptionsCustomization.AllowSort = false;
			gv_FormSetting.OptionsView.ShowGroupPanel = false;
			rep_Fixed.SetDataSource(new Dictionary<string, string>
			{
				{ "N", "None" },
				{ "L", "Left" },
				{ "R", "Right" }
			});
			if (IsSettingForm)
			{
				GridColumnEx gridColumnEx = gcWidth;
				GridColumnEx gridColumnEx2 = gcFixed;
				bool flag = (gcGroupIndex.Visible = false);
				bool visible = (gridColumnEx2.Visible = flag);
				gridColumnEx.Visible = visible;
			}
			gv_FormSetting.CustomDrawColumnHeader += Gv_FormSetting_CustomDrawColumnHeader;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void Gv_FormSetting_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
	{
		try
		{
			if (e.Column != null && e.Column.ColumnType == Type.GetType("System.Boolean"))
			{
				ShowCheckBoxHeader(e);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_FormSetting_MouseDown(object sender, MouseEventArgs e)
	{
		try
		{
			GridViewEx gridViewEx = sender as GridViewEx;
			downHitInfo = null;
			if (gridViewEx == null)
			{
				return;
			}
			GridHitInfo gridHitInfo = gridViewEx.CalcHitInfo(new Point(e.X, e.Y));
			if (e.Button == MouseButtons.Right)
			{
				if (gridHitInfo.Column != null)
				{
					copyToolStripMenuItem.Visible = gridHitInfo.Column.ColumnType == Type.GetType("System.Boolean");
				}
			}
			else if (e.Button == MouseButtons.Left && gridHitInfo.InColumn && gridHitInfo.Column != null && gridHitInfo.Column.ColumnType == Type.GetType("System.Boolean"))
			{
				string fieldName = gridHitInfo.Column.FieldName;
				bool flag = IsAllRowChecked(fieldName);
				if (fieldName.Equals("Editable"))
				{
					if (IsAllRowChecked("Visible"))
					{
						UpdateCheckAllValue(fieldName, allowCheck: true, !flag);
					}
					else
					{
						UpdateCheckAllValue(fieldName, allowCheck: false, checkValue: false);
					}
				}
				else
				{
					UpdateCheckAllValue(fieldName, allowCheck: true, !flag);
				}
			}
			if (Control.ModifierKeys == Keys.None && e.Button == MouseButtons.Left && gridHitInfo.InRow && gridHitInfo.RowHandle != -2147483647)
			{
				downHitInfo = gridHitInfo;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_FormSetting_MouseMove(object sender, MouseEventArgs e)
	{
		try
		{
			GridViewEx gridViewEx = sender as GridViewEx;
			if (e.Button == MouseButtons.Left && downHitInfo != null)
			{
				Size dragSize = SystemInformation.DragSize;
				Rectangle rectangle = new Rectangle(new Point(downHitInfo.HitPoint.X - dragSize.Width / 2, downHitInfo.HitPoint.Y - dragSize.Height / 2), dragSize);
				if (!rectangle.Contains(new Point(e.X, e.Y)))
				{
					gridViewEx?.GridControl.DoDragDrop(downHitInfo, DragDropEffects.All);
					downHitInfo = null;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void grd_FormSetting_DragOver(object sender, DragEventArgs e)
	{
		try
		{
			if (e.Data.GetDataPresent(typeof(GridHitInfo)) && e.Data.GetData(typeof(GridHitInfo)) is GridHitInfo gridHitInfo && sender is GridControlEx { MainView: GridViewEx mainView } gridControlEx)
			{
				GridHitInfo gridHitInfo2 = mainView.CalcHitInfo(gridControlEx.PointToClient(new Point(e.X, e.Y)));
				if (gridHitInfo2.InRow && gridHitInfo2.RowHandle != gridHitInfo.RowHandle && gridHitInfo2.RowHandle != -2147483647)
				{
					e.Effect = DragDropEffects.Move;
				}
				else
				{
					e.Effect = DragDropEffects.None;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void grd_FormSetting_DragDrop(object sender, DragEventArgs e)
	{
		try
		{
			if (!(sender is GridControlEx gridControlEx))
			{
				return;
			}
			GridViewEx gridViewEx = gridControlEx.MainView as GridViewEx;
			GridHitInfo gridHitInfo = e.Data.GetData(typeof(GridHitInfo)) as GridHitInfo;
			if (gridViewEx != null)
			{
				GridHitInfo gridHitInfo2 = gridViewEx.CalcHitInfo(gridControlEx.PointToClient(new Point(e.X, e.Y)));
				if (gridHitInfo != null)
				{
					int rowHandle = gridHitInfo.RowHandle;
					int rowHandle2 = gridHitInfo2.RowHandle;
					MoveRow(rowHandle, rowHandle2);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void MoveRow(int sourceRow, int targetRow)
	{
		try
		{
			if (sourceRow != targetRow)
			{
				DataRow dataRow = DtSource.Rows[sourceRow];
				DataRow dataRow2 = DtSource.NewRow();
				dataRow2.ItemArray = dataRow.ItemArray;
				DtSource.Rows.Remove(dataRow);
				DtSource.Rows.InsertAt(dataRow2, targetRow);
				DtSource.AcceptChanges();
				gv_FormSetting.FocusedRowHandle = targetRow;
				btn_OK.Text = ControlText.ButtonUpdate;
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.View)
		{
			if (IsUpdate)
			{
				if (BandView == null && GlobalData.FormSettings.ContainsKey(FormSettingId))
				{
					if (Grid != null && GlobalData.FormSettings[FormSettingId].Tables.Contains(Item.Item2))
					{
						GlobalData.FormSettings[FormSettingId].Tables.Remove(Item.Item2);
					}
					else if (Tree != null && GlobalData.FormSettings[FormSettingId].Tables.Contains(Tree.ItemId))
					{
						GlobalData.FormSettings[FormSettingId].Tables.Remove(Tree.ItemId);
					}
					else if (BaseForm != null && GlobalData.FormSettings[FormSettingId].Tables.Contains(FormSettingId))
					{
						GlobalData.FormSettings[FormSettingId].Tables.Remove(FormSettingId);
					}
				}
				if (ReloadAfterUpdate)
				{
					if (Grid != null)
					{
						if (BandView == null)
						{
							await Item.Item1.ReloadFormSetting(Enums.PivotFormSetting.None, ReloadAfterUpdate);
						}
						else
						{
							await BandView.ReloadFormSetting();
						}
					}
					if (Tree != null)
					{
						await Tree.ReloadFormSetting();
					}
					if (BaseForm != null)
					{
						BaseForm.DataSourceTittle = _grdFormSetting.DataSourceTable;
					}
				}
			}
			Close();
			return;
		}
		WinCommons._OpenProcessing();
		try
		{
			btn_OK.Enabled = false;
			btn_Cancel.Enabled = false;
			IsUpdate = true;
			List<FormSetting> list = new List<FormSetting>();
			if (DtSource != null && DtSource.Rows.Count > 0)
			{
				for (int i = 0; i < DtSource.Rows.Count; i++)
				{
					if (!Function.ToString(DtSource.Rows[i]["IsSystem"]).Equals("N") || Function.ParseBool(DtSource.Rows[i]["Visible"]) || !Function.ToString(DtSource.Rows[i]["ColumnId"]).Equals(Function.ToString(DtSource.Rows[i]["ColumnName"])))
					{
						FormSetting item = new FormSetting
						{
							FormId = Function.ToString(DtSource.Rows[i]["FormId"]),
							ItemId = Function.ToString(DtSource.Rows[i]["ItemId"]),
							ColumnId = Function.ToString(DtSource.Rows[i]["ColumnId"]),
							ColumnName = Function.ToString(DtSource.Rows[i]["ColumnName"]),
							UserSign = GlobalData.UserName,
							Width = Function.ParseInt(DtSource.Rows[i]["Width"]),
							Visible = Function.ParseBool(DtSource.Rows[i]["Visible"]),
							VisIndex = i,
							Editable = Function.ParseBool(DtSource.Rows[i]["Editable"]),
							IsReadOnly = !Function.ParseBool(DtSource.Rows[i]["Editable"]),
							AllowChSet = Function.ParseBool(DtSource.Rows[i]["AllowChSet"]),
							VisSetting = Function.ParseBool(DtSource.Rows[i]["VisSetting"]),
							NeverEditable = false,
							Fixed = Function.ToString(DtSource.Rows[i]["Fixed"]),
							UpdateDate = DateTime.Today,
							UserUpdate = GlobalData.UserID,
							LngCode = Function.ToString(DtSource.Rows[i]["LngCode"]),
							GroupIndex = Function.ParseInt(DtSource.Rows[i]["GroupIndex"]),
							CurrField = (DtSource.Columns.Contains("CurrField") ? Function.ToString(DtSource.Rows[i]["CurrField"]) : null)
						};
						list.Add(item);
					}
				}
			}
			if (list.Count > 0)
			{
				using HttpClientHelper<FormSetting> helper = new HttpClientHelper<FormSetting>(Configuration.ApiServer, "FormSetting");
				string text = await helper.PostParamsAsJsonAsync("SaveData", list);
				if (!string.IsNullOrEmpty(text))
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
				else
				{
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
					btn_OK.Text = ControlText.ButtonOk;
					FormMode = Enums.FormViewModeType.View;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		btn_OK.Enabled = true;
		btn_Cancel.Enabled = true;
		WinCommons._CloseProcessing();
	}

	private void gv_FormSetting_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (!(e.Column.FieldName == "Visible") && !(e.Column.FieldName == "Editable") && !(e.Column.FieldName == "IsReadOnly"))
			{
				return;
			}
			int dataSourceRowIndex = gv_FormSetting.GetDataSourceRowIndex(e.RowHandle);
			if (e.Column.FieldName.Equals("Visible"))
			{
				if ((bool)e.Value)
				{
					if (!(bool)DtSource.Rows[dataSourceRowIndex]["NeverEditable"])
					{
						DtSource.Rows[dataSourceRowIndex]["Editable"] = true;
						DtSource.Rows[dataSourceRowIndex]["IsReadOnly"] = false;
					}
				}
				else
				{
					DtSource.Rows[dataSourceRowIndex]["Editable"] = false;
					DtSource.Rows[dataSourceRowIndex]["IsReadOnly"] = false;
				}
			}
			else if (e.Column.FieldName.Equals("Editable"))
			{
				DtSource.Rows[dataSourceRowIndex]["IsReadOnly"] = !(bool)e.Value;
			}
			else if (e.Column.FieldName.Equals("IsReadOnly"))
			{
				DtSource.Rows[dataSourceRowIndex]["Editable"] = !(bool)e.Value;
			}
			DtSource.AcceptChanges();
			if (FormMode != Enums.FormViewModeType.Update)
			{
				FormMode = Enums.FormViewModeType.Update;
				btn_OK.Text = ControlText.ButtonUpdate;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", "gv_FormSetting_CellValueChanging", ex.ToString());
		}
	}

	private void gv_FormSetting_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Update)
			{
				FormMode = Enums.FormViewModeType.Update;
				btn_OK.Text = ControlText.ButtonUpdate;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void restoreDefaultToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ = 3;
		try
		{
			DataTable dataSourceTable = _grdFormSetting.DataSourceTable;
			if (!dataSourceTable.IsNotNull() || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRestoreFormsettingCurrentUser, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			HttpResult httpResult = await HttpClient.FormSettingClient.RestoreDefault(GlobalData.UserName, Function.ToString(dataSourceTable.Rows[0]["FormId"]), Function.ToString(dataSourceTable.Rows[0]["ItemId"]));
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				return;
			}
			WinCommons.Success(Enums.MessageCode.RestoreSuccess, "");
			if (Grid != null)
			{
				if (BandView == null)
				{
					await Item.Item1.ReloadFormSetting();
				}
				else
				{
					await BandView.ReloadFormSetting();
				}
			}
			if (Tree != null)
			{
				await Tree.ReloadFormSetting();
			}
			if (BaseForm != null)
			{
				BaseForm.DataSourceTittle = _grdFormSetting.DataSourceTable;
			}
			Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void copyToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			Clipboard.SetText(Function.ToString(gv_FormSetting.GetRowCellValue(gv_FormSetting.FocusedRowHandle, gv_FormSetting.FocusedColumn)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void restoreDefaultAllUserToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			DataTable dataSourceTable = _grdFormSetting.DataSourceTable;
			if (!dataSourceTable.IsNotNull() || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRestoreFormsettingAllUser, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			HttpResult httpResult = await HttpClient.FormSettingClient.RestoreDefaultAllUser(Function.ToString(dataSourceTable.Rows[0]["FormId"]), Function.ToString(dataSourceTable.Rows[0]["ItemId"]));
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				return;
			}
			WinCommons.Success(Enums.MessageCode.RestoreSuccess, "");
			if (Grid != null)
			{
				if (BandView == null)
				{
					await Item.Item1.ReloadFormSetting();
				}
				else
				{
					BandView.ReloadFormSetting();
				}
			}
			if (Tree != null)
			{
				await Tree.ReloadFormSetting();
			}
			if (BaseForm != null)
			{
				BaseForm.DataSourceTittle = _grdFormSetting.DataSourceTable;
			}
			Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void simpleButton1_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void btn_CopyFormSetting_Click(object sender, EventArgs e)
	{
		try
		{
			frm_FormSetting_Copy frm_FormSetting_Copy2 = new frm_FormSetting_Copy();
			if (Tree != null)
			{
				frm_FormSetting_Copy2.ItemTree = new Tuple<TreeEx, string>(Tree, Tree.ItemId);
			}
			else
			{
				frm_FormSetting_Copy2.Item = Item;
			}
			frm_FormSetting_Copy2.FormId = FormSettingId;
			frm_FormSetting_Copy2.UserName = GlobalData.UserName;
			frm_FormSetting_Copy2.ShowDialog();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_FormSetting_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (((gv_FormSetting.FocusedColumn.FieldName.Equals("Visible") || gv_FormSetting.FocusedColumn.FieldName.Equals("Editable") || gv_FormSetting.FocusedColumn.FieldName.Equals("IsReadOnly")) && !Function.ParseBool(gv_FormSetting.GetFocusedRowCellValue("AllowChSet"))) || (gv_FormSetting.FocusedColumn.FieldName.Equals("Editable") && Function.ParseBool(gv_FormSetting.GetFocusedRowCellValue("NeverEditable"))) || (gv_FormSetting.FocusedColumn.FieldName.Equals("Editable") && !Function.ParseBool(gv_FormSetting.GetFocusedRowCellValue("Visible"))))
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Controls.Form.frm_FormSetting));
		this._grdFormSetting = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.restoreDefaultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.restoreDefaultAllUserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_FormSetting = new Apzon.Controls.Grid.GridViewEx();
		this._gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this._gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this._gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this._gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcWidth = new Apzon.Controls.Grid.GridColumnEx();
		this._repositoryItemCalcEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this._gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this._gridColumn7 = new Apzon.Controls.Grid.GridColumnEx();
		this._gridColumn8 = new Apzon.Controls.Grid.GridColumnEx();
		this._gridColumn10 = new Apzon.Controls.Grid.GridColumnEx();
		this._gridColumn11 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcFixed = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Fixed = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.cl_IsReadOnly = new Apzon.Controls.Grid.GridColumnEx();
		this.gcGroupIndex = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gc_CurrField = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_OK = new DevExpress.XtraEditors.SimpleButton();
		this.btn_Cancel = new DevExpress.XtraEditors.SimpleButton();
		this.btn_CopyFormSetting = new DevExpress.XtraEditors.SimpleButton();
		((System.ComponentModel.ISupportInitialize)this._grdFormSetting).BeginInit();
		this.contextMenuStrip.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_FormSetting).BeginInit();
		((System.ComponentModel.ISupportInitialize)this._repositoryItemCalcEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Fixed).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).BeginInit();
		base.SuspendLayout();
		this._grdFormSetting.AllowDrop = true;
		this._grdFormSetting.AllowPasteExcel = true;
		this._grdFormSetting.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this._grdFormSetting.ContextMenuStrip = this.contextMenuStrip;
		this._grdFormSetting.Location = new System.Drawing.Point(0, 0);
		this._grdFormSetting.MainView = this.gv_FormSetting;
		this._grdFormSetting.Name = "_grdFormSetting";
		this._grdFormSetting.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this._repositoryItemCalcEdit1, this.rep_Fixed, this.repositoryItemCalcEdit1 });
		this._grdFormSetting.Size = new System.Drawing.Size(686, 373);
		this._grdFormSetting.TabIndex = 0;
		this._grdFormSetting.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_FormSetting });
		this._grdFormSetting.DragDrop += new System.Windows.Forms.DragEventHandler(this.grd_FormSetting_DragDrop);
		this._grdFormSetting.DragOver += new System.Windows.Forms.DragEventHandler(this.grd_FormSetting_DragOver);
		this.contextMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.copyToolStripMenuItem, this.restoreDefaultToolStripMenuItem, this.restoreDefaultAllUserToolStripMenuItem });
		this.contextMenuStrip.Name = "contextMenuStrip";
		this.contextMenuStrip.Size = new System.Drawing.Size(237, 76);
		this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
		this.copyToolStripMenuItem.Size = new System.Drawing.Size(236, 24);
		this.copyToolStripMenuItem.Text = "Copy";
		this.copyToolStripMenuItem.Visible = false;
		this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
		this.restoreDefaultToolStripMenuItem.Name = "restoreDefaultToolStripMenuItem";
		this.restoreDefaultToolStripMenuItem.Size = new System.Drawing.Size(236, 24);
		this.restoreDefaultToolStripMenuItem.Text = "Restore Default";
		this.restoreDefaultToolStripMenuItem.Click += new System.EventHandler(this.restoreDefaultToolStripMenuItem_Click);
		this.restoreDefaultAllUserToolStripMenuItem.Name = "restoreDefaultAllUserToolStripMenuItem";
		this.restoreDefaultAllUserToolStripMenuItem.Size = new System.Drawing.Size(236, 24);
		this.restoreDefaultAllUserToolStripMenuItem.Text = "Restore Default All User";
		this.restoreDefaultAllUserToolStripMenuItem.Click += new System.EventHandler(this.restoreDefaultAllUserToolStripMenuItem_Click);
		this.gv_FormSetting.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_FormSetting.ColumnKey = null;
		this.gv_FormSetting.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[14]
		{
			this._gridColumn1, this._gridColumn2, this._gridColumn3, this._gridColumn4, this.gcWidth, this._gridColumn6, this._gridColumn7, this._gridColumn8, this._gridColumn10, this._gridColumn11,
			this.gcFixed, this.cl_IsReadOnly, this.gcGroupIndex, this.gc_CurrField
		});
		this.gv_FormSetting.DisableFormSetting = true;
		this.gv_FormSetting.DisplayException = false;
		this.gv_FormSetting.FieldCurrency = null;
		this.gv_FormSetting.FieldRate = null;
		this.gv_FormSetting.FormId = "oFormSetting";
		this.gv_FormSetting.GridControl = this._grdFormSetting;
		this.gv_FormSetting.IndicatorWidth = 35;
		this.gv_FormSetting.IsApzTable = false;
		this.gv_FormSetting.IsInsertNewRow = false;
		this.gv_FormSetting.IsLoadingData = false;
		this.gv_FormSetting.ItemId = "OFORMSETTING";
		this.gv_FormSetting.LstColumnAllowBatch = null;
		this.gv_FormSetting.LstColumnNotAllowBatch = null;
		this.gv_FormSetting.LstColumnNotEditable = null;
		this.gv_FormSetting.LstColumNotSetting = null;
		this.gv_FormSetting.Name = "gv_FormSetting";
		this.gv_FormSetting.OptionsView.ColumnAutoWidth = false;
		this.gv_FormSetting.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.gv_FormSetting.OptionsView.ShowGroupPanel = false;
		this.gv_FormSetting.UseBatchMod = false;
		this.gv_FormSetting.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_FormSetting_RowCellStyle);
		this.gv_FormSetting.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_FormSetting_ShowingEditor);
		this.gv_FormSetting.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_FormSetting_CellValueChanging);
		this.gv_FormSetting.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_FormSetting_CellValueChanged);
		this.gv_FormSetting.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gv_FormSetting_MouseDown);
		this.gv_FormSetting.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gv_FormSetting_MouseMove);
		this._gridColumn1.Caption = "FormId";
		this._gridColumn1.CurrencyDateField = null;
		this._gridColumn1.CurrencyField = null;
		this._gridColumn1.CurrencyFixed = null;
		this._gridColumn1.CurrencyFixedRate = 0.0;
		this._gridColumn1.CurrencyRateField = null;
		this._gridColumn1.FieldName = "FormId";
		this._gridColumn1.Name = "_gridColumn1";
		this._gridColumn1.PrimaryKey = false;
		this._gridColumn1.UnitField = null;
		this._gridColumn2.Caption = "ItemId";
		this._gridColumn2.CurrencyDateField = null;
		this._gridColumn2.CurrencyField = null;
		this._gridColumn2.CurrencyFixed = null;
		this._gridColumn2.CurrencyFixedRate = 0.0;
		this._gridColumn2.CurrencyRateField = null;
		this._gridColumn2.FieldName = "ItemId";
		this._gridColumn2.Name = "_gridColumn2";
		this._gridColumn2.PrimaryKey = false;
		this._gridColumn2.UnitField = null;
		this._gridColumn3.Caption = "ColumnId";
		this._gridColumn3.CurrencyDateField = null;
		this._gridColumn3.CurrencyField = null;
		this._gridColumn3.CurrencyFixed = null;
		this._gridColumn3.CurrencyFixedRate = 0.0;
		this._gridColumn3.CurrencyRateField = null;
		this._gridColumn3.FieldName = "ColumnId";
		this._gridColumn3.Name = "_gridColumn3";
		this._gridColumn3.OptionsColumn.AllowEdit = false;
		this._gridColumn3.PrimaryKey = false;
		this._gridColumn3.UnitField = null;
		this._gridColumn3.Visible = true;
		this._gridColumn3.VisibleIndex = 0;
		this._gridColumn3.Width = 121;
		this._gridColumn4.Caption = "UserSign";
		this._gridColumn4.CurrencyDateField = null;
		this._gridColumn4.CurrencyField = null;
		this._gridColumn4.CurrencyFixed = null;
		this._gridColumn4.CurrencyFixedRate = 0.0;
		this._gridColumn4.CurrencyRateField = null;
		this._gridColumn4.FieldName = "UserSign";
		this._gridColumn4.Name = "_gridColumn4";
		this._gridColumn4.PrimaryKey = false;
		this._gridColumn4.UnitField = null;
		this.gcWidth.Caption = "Width";
		this.gcWidth.ColumnEdit = this._repositoryItemCalcEdit1;
		this.gcWidth.CurrencyDateField = null;
		this.gcWidth.CurrencyField = null;
		this.gcWidth.CurrencyFixed = null;
		this.gcWidth.CurrencyFixedRate = 0.0;
		this.gcWidth.CurrencyRateField = null;
		this.gcWidth.FieldName = "Width";
		this.gcWidth.Name = "gcWidth";
		this.gcWidth.PrimaryKey = false;
		this.gcWidth.UnitField = null;
		this.gcWidth.Visible = true;
		this.gcWidth.VisibleIndex = 2;
		this.gcWidth.Width = 100;
		this._repositoryItemCalcEdit1.AutoHeight = false;
		this._repositoryItemCalcEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this._repositoryItemCalcEdit1.Name = "_repositoryItemCalcEdit1";
		this._gridColumn6.Caption = "Visible";
		this._gridColumn6.CurrencyDateField = null;
		this._gridColumn6.CurrencyField = null;
		this._gridColumn6.CurrencyFixed = null;
		this._gridColumn6.CurrencyFixedRate = 0.0;
		this._gridColumn6.CurrencyRateField = null;
		this._gridColumn6.FieldName = "Visible";
		this._gridColumn6.Name = "_gridColumn6";
		this._gridColumn6.PrimaryKey = false;
		this._gridColumn6.UnitField = null;
		this._gridColumn6.Visible = true;
		this._gridColumn6.VisibleIndex = 3;
		this._gridColumn7.Caption = "Visible Index";
		this._gridColumn7.CurrencyDateField = null;
		this._gridColumn7.CurrencyField = null;
		this._gridColumn7.CurrencyFixed = null;
		this._gridColumn7.CurrencyFixedRate = 0.0;
		this._gridColumn7.CurrencyRateField = null;
		this._gridColumn7.FieldName = "VisIndex";
		this._gridColumn7.Name = "_gridColumn7";
		this._gridColumn7.PrimaryKey = false;
		this._gridColumn7.UnitField = null;
		this._gridColumn8.Caption = "Editable";
		this._gridColumn8.CurrencyDateField = null;
		this._gridColumn8.CurrencyField = null;
		this._gridColumn8.CurrencyFixed = null;
		this._gridColumn8.CurrencyFixedRate = 0.0;
		this._gridColumn8.CurrencyRateField = null;
		this._gridColumn8.FieldName = "Editable";
		this._gridColumn8.Name = "_gridColumn8";
		this._gridColumn8.PrimaryKey = false;
		this._gridColumn8.UnitField = null;
		this._gridColumn8.Visible = true;
		this._gridColumn8.VisibleIndex = 4;
		this._gridColumn10.Caption = "DataType";
		this._gridColumn10.CurrencyDateField = null;
		this._gridColumn10.CurrencyField = null;
		this._gridColumn10.CurrencyFixed = null;
		this._gridColumn10.CurrencyFixedRate = 0.0;
		this._gridColumn10.CurrencyRateField = null;
		this._gridColumn10.FieldName = "DataType";
		this._gridColumn10.Name = "_gridColumn10";
		this._gridColumn10.PrimaryKey = false;
		this._gridColumn10.UnitField = null;
		this._gridColumn11.Caption = "ColumnName";
		this._gridColumn11.CurrencyDateField = null;
		this._gridColumn11.CurrencyField = null;
		this._gridColumn11.CurrencyFixed = null;
		this._gridColumn11.CurrencyFixedRate = 0.0;
		this._gridColumn11.CurrencyRateField = null;
		this._gridColumn11.FieldName = "ColumnName";
		this._gridColumn11.Name = "_gridColumn11";
		this._gridColumn11.PrimaryKey = false;
		this._gridColumn11.UnitField = null;
		this._gridColumn11.Visible = true;
		this._gridColumn11.VisibleIndex = 1;
		this._gridColumn11.Width = 186;
		this.gcFixed.Caption = "Fixed";
		this.gcFixed.ColumnEdit = this.rep_Fixed;
		this.gcFixed.CurrencyDateField = null;
		this.gcFixed.CurrencyField = null;
		this.gcFixed.CurrencyFixed = null;
		this.gcFixed.CurrencyFixedRate = 0.0;
		this.gcFixed.CurrencyRateField = null;
		this.gcFixed.FieldName = "Fixed";
		this.gcFixed.Name = "gcFixed";
		this.gcFixed.PrimaryKey = false;
		this.gcFixed.UnitField = null;
		this.gcFixed.Visible = true;
		this.gcFixed.VisibleIndex = 5;
		this.gcFixed.Width = 105;
		this.rep_Fixed.AutoHeight = false;
		this.rep_Fixed.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Fixed.DisplayMember = "Name";
		this.rep_Fixed.Name = "rep_Fixed";
		this.rep_Fixed.NullText = "";
		this.rep_Fixed.ValueMember = "Code";
		this.cl_IsReadOnly.Caption = "ReadOnly";
		this.cl_IsReadOnly.CurrencyDateField = null;
		this.cl_IsReadOnly.CurrencyField = null;
		this.cl_IsReadOnly.CurrencyFixed = null;
		this.cl_IsReadOnly.CurrencyFixedRate = 0.0;
		this.cl_IsReadOnly.CurrencyRateField = null;
		this.cl_IsReadOnly.FieldName = "IsReadOnly";
		this.cl_IsReadOnly.Name = "cl_IsReadOnly";
		this.cl_IsReadOnly.PrimaryKey = false;
		this.cl_IsReadOnly.UnitField = null;
		this.gcGroupIndex.Caption = "Group Index";
		this.gcGroupIndex.ColumnEdit = this.repositoryItemCalcEdit1;
		this.gcGroupIndex.CurrencyDateField = null;
		this.gcGroupIndex.CurrencyField = null;
		this.gcGroupIndex.CurrencyFixed = null;
		this.gcGroupIndex.CurrencyFixedRate = 0.0;
		this.gcGroupIndex.CurrencyRateField = null;
		this.gcGroupIndex.FieldName = "GroupIndex";
		this.gcGroupIndex.Name = "gcGroupIndex";
		this.gcGroupIndex.PrimaryKey = false;
		this.gcGroupIndex.UnitField = null;
		this.gcGroupIndex.Visible = true;
		this.gcGroupIndex.VisibleIndex = 6;
		this.repositoryItemCalcEdit1.AutoHeight = false;
		this.repositoryItemCalcEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit1.Name = "repositoryItemCalcEdit1";
		this.gc_CurrField.Caption = "Currency";
		this.gc_CurrField.CurrencyDateField = null;
		this.gc_CurrField.CurrencyField = null;
		this.gc_CurrField.CurrencyFixed = null;
		this.gc_CurrField.CurrencyFixedRate = 0.0;
		this.gc_CurrField.CurrencyRateField = null;
		this.gc_CurrField.FieldName = "CurrField";
		this.gc_CurrField.Name = "gc_CurrField";
		this.gc_CurrField.PrimaryKey = false;
		this.gc_CurrField.UnitField = null;
		this.gc_CurrField.Visible = true;
		this.gc_CurrField.VisibleIndex = 7;
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_OK.Appearance.Options.UseBackColor = true;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.Location = new System.Drawing.Point(5, 379);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.Size = new System.Drawing.Size(90, 25);
		this.btn_OK.TabIndex = 1;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Cancel.Appearance.Options.UseBackColor = true;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.Location = new System.Drawing.Point(100, 379);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.Size = new System.Drawing.Size(90, 25);
		this.btn_Cancel.TabIndex = 2;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.simpleButton1_Click);
		this.btn_CopyFormSetting.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_CopyFormSetting.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_CopyFormSetting.Appearance.Options.UseBackColor = true;
		this.btn_CopyFormSetting.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_CopyFormSetting.ImageOptions.Image");
		this.btn_CopyFormSetting.Location = new System.Drawing.Point(544, 379);
		this.btn_CopyFormSetting.Name = "btn_CopyFormSetting";
		this.btn_CopyFormSetting.Size = new System.Drawing.Size(139, 25);
		this.btn_CopyFormSetting.TabIndex = 3;
		this.btn_CopyFormSetting.Text = "Copy Form Setting";
		this.btn_CopyFormSetting.Click += new System.EventHandler(this.btn_CopyFormSetting_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(7f, 16f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(688, 407);
		base.Controls.Add(this.btn_CopyFormSetting);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this._grdFormSetting);
		base.DisableContextAction = true;
		base.DisableLoadScript = true;
		base.FormId = "oFormSetting";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_FormSetting.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_FormSetting";
		base.ShowInTaskbar = false;
		this.Text = "Form Setting";
		base.Load += new System.EventHandler(this.frm_FormSetting_Load);
		((System.ComponentModel.ISupportInitialize)this._grdFormSetting).EndInit();
		this.contextMenuStrip.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_FormSetting).EndInit();
		((System.ComponentModel.ISupportInitialize)this._repositoryItemCalcEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Fixed).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).EndInit();
		base.ResumeLayout(false);
	}

	public override void CHANGELOG()
	{
		try
		{
			string text = "";
			text = FormSettingId + "," + ((Tree != null) ? ItemTree.Item2 : Item.Item2) + "," + GlobalData.UserName;
			Enums.ChangeLog changeLogType = ChangeLogType;
			GetChangeLogParams getChangeLogParams = new GetChangeLogParams();
			getChangeLogParams.PrimaryKey = text;
			getChangeLogParams.Object = changeLogType;
			getChangeLogParams.IsApzTable = true;
			Type type = Type.GetType("Apzon.Forms.Commons.frm_ChangeLogList, Apzon.Forms");
			if (type != null)
			{
				dynamic val = Activator.CreateInstance(type);
				val.GetChangeLogParams = getChangeLogParams;
				val.MdiParent = MdiParent;
				val.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
	}

	private void gv_FormSetting_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (!Function.ParseBool(gv_FormSetting.GetRowCellValue(e.RowHandle, "AllowChSet")) && (e.Column.FieldName == "Visible" || e.Column.FieldName == "Editable"))
			{
				e.Appearance.BackColor = StyleColor.GridDisable;
			}
			else if (Function.ParseBool(gv_FormSetting.GetRowCellValue(e.RowHandle, "NeverEditable")) && e.Column.FieldName == "Editable")
			{
				e.Appearance.BackColor = StyleColor.GridDisable;
			}
			else if (e.Column.FieldName == "Editable" && !Function.ParseBool(gv_FormSetting.GetRowCellValue(e.RowHandle, "Visible")))
			{
				e.Appearance.BackColor = StyleColor.GridDisable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
	}
}
