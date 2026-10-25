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
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Promotions;

public class frm_PromotionCondition : XtraFormEx
{
	private DataTable _dtDoc;

	private DataTable _dtDocOriginal;

	private DataTable _dtLine;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private bool _isLoading;

	private DataTable dtSearch;

	public string _code = "";

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private GridControlEx grd_Line;

	private GridViewEx gv_Line;

	private SimpleButtonEx btn_Ok;

	private SimpleButtonEx btn_Cancel;

	private ContextMenuStrip ctm_Doc;

	private ToolStripMenuItem ctm_Doc_Add;

	private ContextMenuStrip ctm_Line;

	private ToolStripMenuItem ctm_Line_Add;

	private ToolStripMenuItem ctm_Line_Delete;

	private TextEditEx txt_Code;

	private LabelLine labelLine1;

	private SimpleButtonEx btn_Search;

	private LookUpEditEx luk_Status;

	private LabelLine labelLine4;

	private DateEditEx date_toDate;

	private DateEditEx date_fromDate;

	private LabelLine labelLine3;

	private LabelLine labelLine2;

	private ToolStripMenuItem ctm_Doc_Duplicate;

	private ToolStripMenuItem ctm_Doc_Delete;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
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
				btn_Ok.Text = Apzon.Commons.Properties.Resources.ButtonAdd;
				ctm_Doc_Add.Enabled = false;
				ctm_Doc_Duplicate.Enabled = false;
				ctm_Doc_Delete.Enabled = false;
				ctm_Line_Add.Enabled = true;
				ctm_Line_Delete.Enabled = true;
				break;
			case Enums.FormViewModeType.Update:
				btn_Ok.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
				ctm_Doc_Add.Enabled = true;
				ctm_Doc_Duplicate.Enabled = true;
				ctm_Doc_Delete.Enabled = true;
				ctm_Line_Delete.Enabled = true;
				ctm_Line_Add.Enabled = true;
				break;
			case Enums.FormViewModeType.View:
				btn_Ok.Text = Apzon.Commons.Properties.Resources.ButtonOK;
				ctm_Doc_Add.Enabled = true;
				ctm_Doc_Duplicate.Enabled = true;
				ctm_Doc_Delete.Enabled = true;
				ctm_Line_Delete.Enabled = true;
				ctm_Line_Add.Enabled = true;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public frm_PromotionCondition()
	{
		InitializeComponent();
		FormId = "oPromotionCondition";
		gv_Doc.FormId = "oPromotionCondition";
		gv_Doc.ItemId = "APZ_OPCN";
		gv_Line.FormId = "oPromotionCondition";
		gv_Line.ItemId = "APZ_PCN1";
		OnLoadAsync += Frm_PromotionCondition_Load;
	}

	private async Task Frm_PromotionCondition_Load(object sender, EventArgs e)
	{
		_ = 3;
		try
		{
			_isLoading = true;
			luk_Status.SetDataSource(new Dictionary<string, string>
			{
				{ "", "" },
				{ "Y", "Yes" },
				{ "N", "No" }
			});
			new DataTable();
			DateTime dateTime = DateTime.Today.AddDays(-Configuration.SearchDate);
			DateTime dateTime2 = DateTime.Today.AddDays(Configuration.SearchDate);
			date_fromDate.EditValue = dateTime;
			date_toDate.EditValue = dateTime2;
			string value = Function.ToString(luk_Status.EditValue);
			if (!string.IsNullOrEmpty(_code))
			{
				txt_Code.EditValue = _code;
			}
			CreateDataSearch();
			DataRow dataRow = dtSearch.NewRow();
			dataRow["code"] = _code;
			dataRow["fromDate"] = dateTime;
			dataRow["toDate"] = dateTime2;
			dataRow["active"] = value;
			dtSearch.Rows.Add(dataRow);
			DataSet ds = await _httpClient.PromotionCondition.SearchPromotionCondition(dtSearch.Copy());
			await GridViewHelper.GenerateGrid(grd_Doc, gv_Doc, ds);
			await WinFuntions.LoadUdfColumns(gv_Doc, "APZ_OPCN", null, overrideOld: true);
			_dtDocOriginal = grd_Doc.DataSourceTable.Copy();
			if (ds.IsHasStructure() && !ds.Tables[1].IsNotNull())
			{
				await LoadLineById(0);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		_isLoading = false;
	}

	private void CreateDataSearch()
	{
		if (!dtSearch.IsNotNull() || dtSearch.Columns.Count == 0)
		{
			dtSearch = new DataTable();
			dtSearch.Columns.Add(new DataColumn("code", typeof(string)));
			dtSearch.Columns.Add(new DataColumn("fromDate", typeof(DateTime)));
			dtSearch.Columns.Add(new DataColumn("toDate", typeof(DateTime)));
			dtSearch.Columns.Add(new DataColumn("active", typeof(string)));
		}
		else
		{
			dtSearch.Rows.Clear();
		}
	}

	private async void btn_Ok_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			switch (FormMode)
			{
			case Enums.FormViewModeType.Add:
				await Save("A");
				break;
			case Enums.FormViewModeType.Update:
				await Save("U");
				break;
			case Enums.FormViewModeType.View:
				Close();
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task Save(string type)
	{
		if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
		{
			return;
		}
		try
		{
			WinCommons._OpenProcessing();
			DataSet dataSet = new DataSet();
			if (type.Equals("U"))
			{
				DataTable dataSourceTable = grd_Doc.DataSourceTable;
				dataSourceTable.AcceptChanges();
				DataRow[] array = dataSourceTable.Select("DocEntry=" + Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry")));
				if (array.Length != 0)
				{
					_dtDoc = array.CopyToDataTable();
				}
			}
			else
			{
				_dtDoc = grd_Doc.DataSourceTable;
			}
			_dtLine = grd_Line.DataSourceTable().Copy();
			if (_dtLine != null && _dtLine.Rows.Count > 0)
			{
				for (int i = 0; i < _dtLine.Rows.Count; i++)
				{
					if (Function.ToString(_dtLine.Rows[i]["TypeCode"]) == "")
					{
						_dtLine.Rows.RemoveAt(i--);
					}
				}
			}
			_dtDoc.TableName = "dtDoc";
			_dtLine.TableName = "dtLine";
			dataSet.Tables.Add(_dtDoc.Copy());
			dataSet.Tables.Add(_dtLine.Copy());
			HttpResult httpResult = ((!type.Equals("U")) ? (await _httpClient.PromotionCondition.Add(dataSet)) : (await _httpClient.PromotionCondition.Update(dataSet)));
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				FormMode = Enums.FormViewModeType.View;
				if (type.Equals("A"))
				{
					btn_Search_Click(null, null);
				}
				else
				{
					_dtDocOriginal = grd_Doc.DataSourceTable.Copy();
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.View)
		{
			Close();
		}
		else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
		{
			Close();
		}
	}

	private async void gv_Doc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add || !_isLoading)
			{
				int id = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
				await LoadLineById(id);
				AddNewBlankLine();
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ctm_Doc_Add_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.View && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
			{
				return;
			}
			FormMode = Enums.FormViewModeType.Add;
			_isLoading = true;
			grd_Doc.DataSourceTable.Rows.Clear();
			grd_Line.DataSourceTable().Rows.Clear();
			_dtDoc = grd_Doc.DataSourceTable;
			if (_dtDoc == null || _dtDoc.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
			}
			DataRow dataRow = _dtDoc.NewRow();
			dataRow["Code"] = "";
			dataRow["Type"] = "1";
			dataRow["Active"] = "Y";
			_dtDoc.Rows.Add(dataRow);
			gv_Doc.Columns["Code"].OptionsColumn.ReadOnly = false;
			grd_Doc.DataSource = _dtDoc;
			AddNewBlankLine();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		_isLoading = false;
	}

	private void ctm_Doc_Duplicate_Click(object sender, EventArgs e)
	{
		try
		{
			if (gv_Doc.FocusedRowHandle < 0 || (FormMode != Enums.FormViewModeType.View && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes))
			{
				return;
			}
			FormMode = Enums.FormViewModeType.Add;
			_isLoading = true;
			DataRow focusedDataRow = gv_Doc.GetFocusedDataRow();
			DataTable dataTable = grd_Doc.DatasourceTable();
			DataRow dataRow = dataTable.NewRow();
			dataRow.ItemArray = focusedDataRow.ItemArray;
			dataRow["Code"] = "";
			dataTable.Rows.Clear();
			dataTable.Rows.Add(dataRow);
			gv_Doc.Columns["Code"].OptionsColumn.ReadOnly = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		_isLoading = false;
	}

	private async void ctm_Doc_Delete_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (gv_Doc.FocusedRowHandle >= 0 && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				DataSet dataSet = new DataSet();
				string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("Code"));
				DataTable dataTable = grd_Doc.DatasourceTable().Select("Code = '" + text + "'").CopyToDataTable();
				dataTable.TableName = "dtDoc";
				dataSet.Tables.Add(dataTable);
				new HttpResult();
				HttpResult httpResult = await _httpClient.PromotionCondition.Delete(dataSet);
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Success(Enums.MessageCode.None, Apzon.Commons.Properties.Resources.Msg_Success + httpResult.Content);
					await Frm_PromotionCondition_Load(null, null);
					FormMode = Enums.FormViewModeType.View;
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ctm_Line_Add_Click(object sender, EventArgs e)
	{
		AddNewBlankLine();
		DataChanged();
	}

	private void ctm_Line_Delete_Click(object sender, EventArgs e)
	{
		try
		{
			if (gv_Line.DataRowCount > 0)
			{
				grd_Line.BeginUpdate();
				gv_Line.DeleteSelectedRows();
				grd_Line.EndUpdate();
				grd_Line.DatasourceTable().AcceptChanges();
				DataChanged();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadLineById(int id)
	{
		_ = 2;
		try
		{
			WinCommons._OpenProcessing();
			_isLoading = true;
			DataSet ds = await _httpClient.PromotionCondition.GetLines(id);
			await GridViewHelper.GenerateGrid(grd_Line, gv_Line, ds);
			await WinFuntions.LoadUdfColumns(gv_Doc, "APZ_PCN1", null, overrideOld: true);
			if (gv_Line.ColumnsEx.Count > 0 && gv_Line.ColumnsEx.Any((GridColumn m) => m.FieldName.Equals("TypeCode")))
			{
				RepositoryItemButtonEdit repositoryItemButtonEdit = new RepositoryItemButtonEdit();
				repositoryItemButtonEdit.Buttons[0].Kind = ButtonPredefines.Search;
				repositoryItemButtonEdit.Buttons.Add(new EditorButton
				{
					Kind = ButtonPredefines.Glyph,
					Image = Apzon.Forms.Properties.Resources.right,
					IsLeft = true
				});
				repositoryItemButtonEdit.ButtonClick += (object s, ButtonPressedEventArgs e) =>
				{
					try
					{
						int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("Type"));
						if (num > 0)
						{
							List<DataRow> list = new List<DataRow>();
							if (e.Button.Index == 0)
							{
								switch (num)
								{
								case 1:
								case 3:
								{
									frm_CFL frm_CFL7 = new frm_CFL(Enums.TypeCFL.Customer, true, Function.ToString(gv_Line.GetFocusedRowCellValue("TypeCode")), "", Enums.ItemObjectType.NoItemType, null, "");
									frm_CFL7.Text = "List of Business Partners";
									frm_CFL7.ShowDialog();
									list = frm_CFL7.ChooseItems;
									break;
								}
								case 2:
								{
									frm_CFL frm_CFL6 = new frm_CFL(Enums.TypeCFL.UserDefinedProcedure, true, Function.ToString(gv_Line.GetFocusedRowCellValue("TypeCode")), "Apz_PromotionCondition_CFLCustomerGroup", Enums.ItemObjectType.NoItemType, null, "");
									frm_CFL6.Text = "List of Customer Groups";
									frm_CFL6.ShowDialog();
									list = frm_CFL6.ChooseItems;
									break;
								}
								case 4:
								case 6:
								{
									frm_CFL frm_CFL5 = new frm_CFL(Enums.TypeCFL.UserDefinedProcedure, true, Function.ToString(gv_Line.GetFocusedRowCellValue("TypeCode")), "Apz_PromotionCondition_CFLItem", Enums.ItemObjectType.NoItemType, null, "");
									frm_CFL5.Text = "List of Items";
									frm_CFL5.ShowDialog();
									list = frm_CFL5.ChooseItems;
									break;
								}
								case 5:
								case 7:
								{
									frm_CFL frm_CFL4 = new frm_CFL(Enums.TypeCFL.UserDefinedProcedure, true, Function.ToString(gv_Line.GetFocusedRowCellValue("TypeCode")), "Apz_PromotionCondition_CFLItemGroup", Enums.ItemObjectType.NoItemType, null, "");
									frm_CFL4.Text = "List of Items";
									frm_CFL4.ShowDialog();
									list = frm_CFL4.ChooseItems;
									break;
								}
								case 8:
								{
									frm_CFL frm_CFL3 = new frm_CFL(Enums.TypeCFL.UserDefinedProcedure, true, Function.ToString(gv_Line.GetFocusedRowCellValue("TypeCode")), "Apz_PromotionCondition_CFLTerritory", Enums.ItemObjectType.NoItemType, null, "");
									frm_CFL3.Text = "List of Items";
									frm_CFL3.ShowDialog();
									list = frm_CFL3.ChooseItems;
									break;
								}
								case 9:
								{
									frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.UserDefinedProcedure, true, Function.ToString(gv_Line.GetFocusedRowCellValue("TypeCode")), "Apz_PromotionCondition_CFLChannel", Enums.ItemObjectType.NoItemType, null, "");
									frm_CFL2.Text = "List of Items";
									frm_CFL2.ShowDialog();
									list = frm_CFL2.ChooseItems;
									break;
								}
								}
								DataTable dataTable = grd_Line.DatasourceTable();
								if (dataTable != null && list.Count != 0)
								{
									foreach (DataRow item in list)
									{
										string code = Function.ToString(item[0]);
										if (!dataTable.AsEnumerable().Any((DataRow m) => m["TypeCode"].Equals(code)))
										{
											DataRow dataRow = dataTable.NewRow();
											dataRow["TypeCode"] = code;
											dataRow["TypeName"] = item[1];
											dataRow["Rate"] = 100;
											dataTable.Rows.InsertAt(dataRow, 0);
										}
									}
									AddNewBlankLine();
									if (FormMode == Enums.FormViewModeType.View)
									{
										FormMode = Enums.FormViewModeType.Update;
									}
								}
							}
							else if (e.Button.Index == 1)
							{
								XtraFormEx baseForm = (XtraFormEx)gv_Line.GridControl.FindForm();
								switch (num)
								{
								case 1:
								case 3:
									baseForm.DrillDown(ApzObjectTypes.oBusinessPartners, ApzObjectTypes.oBusinessPartners, Function.ToString(gv_Line.GetFocusedRowCellValue("TypeCode")));
									break;
								case 2:
									baseForm.DrillDown(ApzObjectTypes.oCustomsGroups, ApzObjectTypes.oBusinessPartners, Function.ToString(gv_Line.GetFocusedRowCellValue("TypeCode")));
									break;
								case 4:
								case 6:
									baseForm.DrillDown(ApzObjectTypes.oItems, ApzObjectTypes.oBusinessPartners, Function.ToString(gv_Line.GetFocusedRowCellValue("TypeCode")));
									break;
								case 5:
								case 7:
									baseForm.DrillDown(ApzObjectTypes.oItemGroups, ApzObjectTypes.oBusinessPartners, Function.ToString(gv_Line.GetFocusedRowCellValue("TypeCode")));
									break;
								case 8:
								case 9:
									break;
								}
							}
						}
					}
					catch (Exception ex2)
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
						WinCommons.ErrorLineNumber(ex2);
					}
				};
				gv_Line.Columns["TypeCode"].ColumnEdit = repositoryItemButtonEdit;
				(gv_Line.Columns["TypeCode"] as GridColumnEx).PrimaryKey = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		_isLoading = false;
		WinCommons._CloseProcessing();
	}

	private void AddNewBlankLine()
	{
		try
		{
			_dtLine = grd_Line.DataSourceTable();
			if ((_dtLine != null || _dtLine.Columns.Count != 0) && !_dtLine.AsEnumerable().Any((DataRow m) => string.IsNullOrEmpty(Function.ToString(m["TypeCode"]))))
			{
				DataRow dataRow = _dtLine.NewRow();
				dataRow["Rate"] = 100;
				_dtLine.Rows.Add(dataRow);
				_dtLine.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void DataChanged()
	{
		try
		{
			if (!_isLoading && FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			btn_Search_Click(null, null);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Search_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (FormMode == Enums.FormViewModeType.View || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				_isLoading = true;
				grd_Doc.DatasourceTable().Rows.Clear();
				grd_Line.DatasourceTable().Rows.Clear();
				string value = Function.ToString(luk_Status.EditValue);
				DateTime dateTime = (string.IsNullOrWhiteSpace(date_fromDate.Text) ? new DateTime(1900, 1, 1) : Convert.ToDateTime(date_fromDate.EditValue));
				DateTime dateTime2 = (string.IsNullOrWhiteSpace(date_toDate.Text) ? DateTime.Today : Convert.ToDateTime(date_toDate.EditValue));
				CreateDataSearch();
				DataRow dataRow = dtSearch.NewRow();
				dataRow["code"] = txt_Code.Text;
				dataRow["fromDate"] = dateTime;
				dataRow["toDate"] = dateTime2;
				dataRow["active"] = value;
				dtSearch.Rows.Add(dataRow);
				_isLoading = false;
				DataSet ds = await _httpClient.PromotionCondition.SearchPromotionCondition(dtSearch.Copy());
				await GridViewHelper.GenerateGrid(grd_Doc, gv_Doc, ds);
				_dtDocOriginal = grd_Doc.DataSourceTable.Copy();
				gv_Doc.Columns["Code"].OptionsColumn.ReadOnly = true;
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (!_isLoading && FormMode != Enums.FormViewModeType.View)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
				{
					e.Allow = false;
					return;
				}
				grd_Doc.DataSource = _dtDocOriginal.Copy();
				grd_Doc.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			_ = gv_Doc.FocusedRowHandle;
			if (e.Column.FieldName == "DocEntry")
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Line_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "TypeCode")
			{
				FillDataFromCFL(e.RowHandle, Function.ToString(e.Value));
			}
			DataChanged();
			AddNewBlankLine();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void FillDataFromCFL(int row, string value)
	{
		int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("Type"));
		_ = string.Empty;
		string name = string.Empty;
		switch (num)
		{
		case 1:
		case 3:
		{
			string query = "SELECT \"CardCode\", \"CardName\" FROM OCRD WHERE \"CardType\" = 'C' AND \"CardCode\" = '" + value + "';";
			DataSet dataSet = await HttpClient.Cfl.GetDataByQuery(query);
			if (dataSet.IsNotNull() && dataSet.Tables[0].IsNotNull())
			{
				name = Function.ToString(dataSet.Tables[0].Rows[0]["CardName"]);
			}
			break;
		}
		case 2:
		{
			string query = "SELECT \"GroupCode\", \"GroupName\" FROM OCRG WHERE \"GroupType\" = 'C' AND \"GroupCode\"='" + value + "';";
			DataSet dataSet = await HttpClient.Cfl.GetDataByQuery(query);
			if (dataSet.IsNotNull() && dataSet.Tables[0].IsNotNull())
			{
				name = Function.ToString(dataSet.Tables[0].Rows[0]["GroupName"]);
			}
			break;
		}
		case 4:
		case 6:
		{
			string query = "SELECT \"ItemCode\", \"ItemName\", b.\"ItmsGrpNam\" \"ItemGroup\", 'Y' \"Active\"\r\n                                    FROM OITM a LEFT JOIN OITB b ON b.\"ItmsGrpCod\" = a.\"ItmsGrpCod\" \r\n                                    WHERE a.\"ItemCode\" = '" + value + "';";
			DataSet dataSet = await HttpClient.Cfl.GetDataByQuery(query);
			if (dataSet.IsNotNull() && dataSet.Tables[0].IsNotNull())
			{
				name = Function.ToString(dataSet.Tables[0].Rows[0]["ItemName"]);
			}
			break;
		}
		case 5:
		case 7:
		{
			string query = "SELECT \"ItmsGrpCod\" \"Code\", \"ItmsGrpNam\" \"Name\" FROM OITB WHERE \"Locked\" = 'N' AND \"ItmsGrpCod\" = '" + value + "';";
			DataSet dataSet = await HttpClient.Cfl.GetDataByQuery(query);
			if (dataSet.IsNotNull() && dataSet.Tables[0].IsNotNull())
			{
				name = Function.ToString(dataSet.Tables[0].Rows[0]["Name"]);
			}
			break;
		}
		case 8:
		{
			string query = "SELECT \"RouteCode\" \"TerCode\", \"RouteName\" \"TerName\" FROM APZ_D_OSTR WHERE \"RouteCode\" = '" + value + "';";
			DataSet dataSet = await HttpClient.Cfl.GetDataByQuery(query);
			if (dataSet.IsNotNull() && dataSet.Tables[0].IsNotNull())
			{
				name = Function.ToString(dataSet.Tables[0].Rows[0]["TerName"]);
			}
			break;
		}
		case 9:
		{
			string query = "SELECT \"ChannelCode\" \"ChannelCode\", \"ChannelName\" \"ChannelName\" FROM APZ_D_OCHN WHERE \"ChannelCode\" = '" + value + "';";
			DataSet dataSet = await HttpClient.Cfl.GetDataByQuery(query);
			if (dataSet.IsNotNull() && dataSet.Tables[0].IsNotNull())
			{
				name = Function.ToString(dataSet.Tables[0].Rows[0]["ChannelName"]);
			}
			break;
		}
		}
		gv_Line.SetRowCellValue(row, "TypeName", name);
		if (string.IsNullOrWhiteSpace(Function.ToString(gv_Line.GetRowCellValue(row, "Rate"))))
		{
			gv_Line.SetRowCellValue(row, "Rate", 100);
		}
	}

	private void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (_isLoading)
			{
				return;
			}
			if (e.Column.FieldName.Equals("Type"))
			{
				string value = Function.ToString(gv_Doc.GetFocusedRowCellValue(e.Column));
				DataTable dataTable = grd_Line.DataSourceTable();
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					for (int i = 0; i < dataTable.Rows.Count; i++)
					{
						string text = Function.ToString(dataTable.Rows[i]["TypeCode"]);
						if (!string.IsNullOrEmpty(text) && !text.Equals(value))
						{
							dataTable.Rows.RemoveAt(i--);
						}
					}
					grd_Line.RefreshDataSource();
				}
			}
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		DataChanged();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Promotions.frm_PromotionCondition));
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Doc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.ctm_Doc_Add = new System.Windows.Forms.ToolStripMenuItem();
		this.ctm_Doc_Duplicate = new System.Windows.Forms.ToolStripMenuItem();
		this.ctm_Doc_Delete = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.grd_Line = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Line = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.ctm_Line_Add = new System.Windows.Forms.ToolStripMenuItem();
		this.ctm_Line_Delete = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Line = new Apzon.Controls.Grid.GridViewEx();
		this.btn_Ok = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_Code = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.date_fromDate = new Apzon.Controls.Editor.DateEditEx();
		this.date_toDate = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.luk_Status = new Apzon.Controls.Editor.LookUpEditEx();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.ctm_Doc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Line).BeginInit();
		this.ctm_Line.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Line).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Code.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_fromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_fromDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_toDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_toDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Status.Properties).BeginInit();
		base.SuspendLayout();
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(1, 48);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Doc);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.grd_Line);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(1113, 504);
		this.splitContainerControl1.SplitterPosition = 211;
		this.splitContainerControl1.TabIndex = 0;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.ctm_Doc;
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.Size = new System.Drawing.Size(1113, 211);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.ctm_Doc.ImageScalingSize = new System.Drawing.Size(28, 28);
		this.ctm_Doc.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.ctm_Doc_Add, this.ctm_Doc_Duplicate, this.ctm_Doc_Delete });
		this.ctm_Doc.Name = "ctm_Doc";
		this.ctm_Doc.Size = new System.Drawing.Size(125, 70);
		this.ctm_Doc_Add.Name = "ctm_Doc_Add";
		this.ctm_Doc_Add.Size = new System.Drawing.Size(124, 22);
		this.ctm_Doc_Add.Text = "Add New";
		this.ctm_Doc_Add.Click += new System.EventHandler(this.ctm_Doc_Add_Click);
		this.ctm_Doc_Duplicate.Name = "ctm_Doc_Duplicate";
		this.ctm_Doc_Duplicate.Size = new System.Drawing.Size(124, 22);
		this.ctm_Doc_Duplicate.Text = "Duplicate";
		this.ctm_Doc_Duplicate.Click += new System.EventHandler(this.ctm_Doc_Duplicate_Click);
		this.ctm_Doc_Delete.Name = "ctm_Doc_Delete";
		this.ctm_Doc_Delete.Size = new System.Drawing.Size(124, 22);
		this.ctm_Doc_Delete.Text = "Delete";
		this.ctm_Doc_Delete.Click += new System.EventHandler(this.ctm_Doc_Delete_Click);
		this.gv_Doc.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Doc.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "";
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
		this.gv_Doc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Doc_RowCellStyle);
		this.gv_Doc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Doc_FocusedRowObjectChanged);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
		this.gv_Doc.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanging);
		this.gv_Doc.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_Doc_BeforeLeaveRow);
		this.grd_Line.AllowPasteExcel = true;
		this.grd_Line.ClipboardData = null;
		this.grd_Line.ContextMenuStrip = this.ctm_Line;
		this.grd_Line.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Line.Location = new System.Drawing.Point(0, 0);
		this.grd_Line.MainView = this.gv_Line;
		this.grd_Line.Name = "grd_Line";
		this.grd_Line.Size = new System.Drawing.Size(1113, 283);
		this.grd_Line.TabIndex = 0;
		this.grd_Line.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Line });
		this.ctm_Line.ImageScalingSize = new System.Drawing.Size(28, 28);
		this.ctm_Line.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.ctm_Line_Add, this.ctm_Line_Delete });
		this.ctm_Line.Name = "ctm_Line";
		this.ctm_Line.Size = new System.Drawing.Size(134, 48);
		this.ctm_Line_Add.Name = "ctm_Line_Add";
		this.ctm_Line_Add.Size = new System.Drawing.Size(133, 22);
		this.ctm_Line_Add.Text = "Add Row";
		this.ctm_Line_Add.Click += new System.EventHandler(this.ctm_Line_Add_Click);
		this.ctm_Line_Delete.Name = "ctm_Line_Delete";
		this.ctm_Line_Delete.Size = new System.Drawing.Size(133, 22);
		this.ctm_Line_Delete.Text = "Delete Row";
		this.ctm_Line_Delete.Click += new System.EventHandler(this.ctm_Line_Delete_Click);
		this.gv_Line.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Line.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Line.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Line.ColumnKey = null;
		this.gv_Line.DisableFormSetting = false;
		this.gv_Line.DisplayException = false;
		this.gv_Line.EditColumn = null;
		this.gv_Line.FieldCurrency = null;
		this.gv_Line.FieldRate = null;
		this.gv_Line.FormId = "";
		this.gv_Line.GridControl = this.grd_Line;
		this.gv_Line.IgnoreFilter = false;
		this.gv_Line.IndexEditRow = -1;
		this.gv_Line.IndicatorWidth = 35;
		this.gv_Line.IsApzTable = false;
		this.gv_Line.IsInsertNewRow = false;
		this.gv_Line.IsLoadingData = false;
		this.gv_Line.IsMappingContextAction = false;
		this.gv_Line.ItemId = "";
		this.gv_Line.LstColumnAllowBatch = null;
		this.gv_Line.LstColumnNotAllowBatch = null;
		this.gv_Line.LstColumnNotEditable = null;
		this.gv_Line.LstColumNotSetting = null;
		this.gv_Line.Name = "gv_Line";
		this.gv_Line.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Line.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Line.OptionsPrint.AutoWidth = false;
		this.gv_Line.OptionsPrint.UsePrintStyles = false;
		this.gv_Line.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Line.OptionsView.ColumnAutoWidth = false;
		this.gv_Line.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Line.OptionsView.ShowGroupedColumns = true;
		this.gv_Line.OptionsView.ShowGroupPanel = false;
		this.gv_Line.UseBatchMod = false;
		this.gv_Line.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Line_CellValueChanged);
		this.btn_Ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Ok.EditControl = null;
		this.btn_Ok.EditValue = "OK";
		this.btn_Ok.FieldName = null;
		this.btn_Ok.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Ok.ImageOptions.Image");
		this.btn_Ok.IsSearchControl = false;
		this.btn_Ok.Location = new System.Drawing.Point(1, 558);
		this.btn_Ok.Name = "btn_Ok";
		this.btn_Ok.ReadOnly = false;
		this.btn_Ok.SettingInVisible = false;
		this.btn_Ok.SettingReadonly = false;
		this.btn_Ok.Size = new System.Drawing.Size(75, 23);
		this.btn_Ok.TabIndex = 1;
		this.btn_Ok.Text = "OK";
		this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(82, 558);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 2;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.txt_Code.EditControl = null;
		this.txt_Code.FieldName = null;
		this.txt_Code.IsSearchControl = false;
		this.txt_Code.Location = new System.Drawing.Point(52, 18);
		this.txt_Code.Name = "txt_Code";
		this.txt_Code.SettingInVisible = false;
		this.txt_Code.SettingReadonly = false;
		this.txt_Code.Size = new System.Drawing.Size(130, 20);
		this.txt_Code.TabIndex = 3;
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Code";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(7, 18);
		this.labelLine1.Margin = new System.Windows.Forms.Padding(5);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(45, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 4;
		this.labelLine1.Utext = "Code";
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "From Date";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(190, 18);
		this.labelLine2.Margin = new System.Windows.Forms.Padding(5);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(72, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 5;
		this.labelLine2.Utext = "From Date";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "To Date";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(365, 18);
		this.labelLine3.Margin = new System.Windows.Forms.Padding(5);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(72, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 6;
		this.labelLine3.Utext = "To Date";
		this.date_fromDate.EditControl = null;
		this.date_fromDate.EditValue = null;
		this.date_fromDate.FieldName = null;
		this.date_fromDate.Location = new System.Drawing.Point(259, 18);
		this.date_fromDate.Name = "date_fromDate";
		this.date_fromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_fromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_fromDate.Properties.IsSearchControl = false;
		this.date_fromDate.SettingInVisible = false;
		this.date_fromDate.SettingReadonly = false;
		this.date_fromDate.Size = new System.Drawing.Size(100, 20);
		this.date_fromDate.TabIndex = 7;
		this.date_toDate.EditControl = null;
		this.date_toDate.EditValue = null;
		this.date_toDate.FieldName = null;
		this.date_toDate.Location = new System.Drawing.Point(427, 18);
		this.date_toDate.Name = "date_toDate";
		this.date_toDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_toDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_toDate.Properties.IsSearchControl = false;
		this.date_toDate.SettingInVisible = false;
		this.date_toDate.SettingReadonly = false;
		this.date_toDate.Size = new System.Drawing.Size(100, 20);
		this.date_toDate.TabIndex = 8;
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Active";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(541, 18);
		this.labelLine4.Margin = new System.Windows.Forms.Padding(5);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(55, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 9;
		this.labelLine4.Utext = "Active";
		this.luk_Status.EditControl = null;
		this.luk_Status.FieldName = null;
		this.luk_Status.IsSearchControl = false;
		this.luk_Status.Location = new System.Drawing.Point(591, 18);
		this.luk_Status.Name = "luk_Status";
		this.luk_Status.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Status.Properties.NullText = "";
		this.luk_Status.SettingInVisible = false;
		this.luk_Status.SettingReadonly = false;
		this.luk_Status.Size = new System.Drawing.Size(100, 20);
		this.luk_Status.TabIndex = 10;
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = false;
		this.btn_Search.Location = new System.Drawing.Point(719, 16);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(75, 23);
		this.btn_Search.TabIndex = 11;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		base.AcceptButton = this.btn_Ok;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btn_Cancel;
		base.ClientSize = new System.Drawing.Size(1116, 585);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.luk_Status);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.date_toDate);
		base.Controls.Add(this.date_fromDate);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.txt_Code);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Ok);
		base.Controls.Add(this.splitContainerControl1);
		base.FormId = "oQuestionMasterData";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_PromotionCondition";
		this.Text = "Promotion Conditions";
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.ctm_Doc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Line).EndInit();
		this.ctm_Line.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Line).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Code.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_fromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_fromDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_toDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_toDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Status.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
