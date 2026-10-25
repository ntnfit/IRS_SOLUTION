using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace Apzon.Forms.Inventory.PriceLists;

public class frm_STDPriceLists : XtraFormEx
{
	private string _privilege = "";

	private bool _isLoading;

	private List<int> _lstChanged = new List<int>();

	private IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private DataTable _dtOriginal = new DataTable();

	private IContainer components;

	private RadioGroupEx radioGroup1;

	private GridControlEx grd_Doc;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx bt_OK;

	private GridColumnEx gcListNum;

	private GridColumnEx gcListName;

	private GridColumnEx gcBASE_NUM;

	private RepositoryItemLookUpEdit rep_luk_basePricelist;

	private GridColumnEx gcFactor;

	private GridColumnEx gcRoundSys;

	private RepositoryItemLookUpEdit rep_luk_RoudingMethod;

	private GridColumnEx gcRoundRule;

	private RepositoryItemLookUpEdit rep_luk_RoudingRule;

	private GridColumnEx gcExtAmount;

	private RepositoryItemTextEdit rep_ted_EndingInterVal;

	private GridColumnEx gcRndFrmtInt;

	private GridColumnEx gcRndFrmtDec;

	private GridColumnEx gcGroupCode;

	private RepositoryItemLookUpEdit rep_luk_GroupCode;

	private GridColumnEx gcIsGrossPrc;

	private GridColumnEx gcPrimCurr;

	private RepositoryItemLookUpEdit rep_luk_PrimCurr;

	private GridColumnEx gridColumn2;

	private RepositoryItemCheckEdit rep_chk_IsGrossPrice;

	private ContextMenuStrip ctx_Doc;

	private ToolStripMenuItem addRowToolStripMenuItem;

	private GridViewEx gv_Doc;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn3;

	private GridColumnEx gridColumn4;

	private RepositoryItemCheckEdit rep_ValidFor;

	private GridColumnEx gridColumn5;

	private GridColumnEx gridColumn6;

	private RepositoryItemDateEdit rep_ValidDate;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private SimpleButtonEx btn_InitCurrency;

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
			bt_OK.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			bt_OK.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_STDPriceLists()
	{
		InitializeComponent();
		SettingLanguage();
		OnLoadAsync += frm_PriceLists_Load;
	}

	private void RefreshData()
	{
		_isLoading = true;
		LoadData();
		LoadComBoBox();
		_lstChanged.Clear();
		_isLoading = false;
	}

	private void LoadData()
	{
		try
		{
			DataTable dataTable = _httpClient.PriceListSetup.LoadData();
			DataRow dataRow = dataTable.NewRow();
			dataRow["ListNum"] = -2;
			dataRow["ListName"] = "Last Evaluated Price";
			dataTable.Rows.InsertAt(dataRow, 0);
			DataRow dataRow2 = dataTable.NewRow();
			dataRow2["ListNum"] = -1;
			dataRow2["ListName"] = "Last Purchase Price";
			dataTable.Rows.InsertAt(dataRow2, 1);
			dataTable.AcceptChanges();
			grd_Doc.DataSource = dataTable;
			grd_Doc.RefreshDataSource();
			_dtOriginal = new DataTable();
			_dtOriginal.Columns.Add("ListNum", typeof(int));
			_dtOriginal.Columns.Add("Factor", typeof(double));
			_dtOriginal.Columns.Add("BASE_NUM", typeof(int));
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				DataRow dataRow3 = _dtOriginal.NewRow();
				dataRow3["ListNum"] = Function.ParseInt(dataTable.Rows[i]["ListNum"]);
				dataRow3["Factor"] = Function.ParseDouble(dataTable.Rows[i]["Factor"]);
				dataRow3["BASE_NUM"] = Function.ParseInt(dataTable.Rows[i]["BASE_NUM"]);
				_dtOriginal.Rows.Add(dataRow3);
			}
			_dtOriginal.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadComBoBox()
	{
		try
		{
			DataTable groupNumPriceList = _httpClient.PriceListSetup.GetGroupNumPriceList();
			rep_luk_basePricelist.DataSource = groupNumPriceList;
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Name", typeof(string));
			dataTable.Columns.Add("Code", typeof(short));
			DataRow dataRow = dataTable.NewRow();
			dataRow["Name"] = "No Rounding";
			dataRow["Code"] = Function.ParseShort(0);
			dataTable.Rows.Add(dataRow);
			DataRow dataRow2 = dataTable.NewRow();
			dataRow2["Name"] = "Round to Full Decimal Amount";
			dataRow2["Code"] = Function.ParseShort(1);
			dataTable.Rows.Add(dataRow2);
			DataRow dataRow3 = dataTable.NewRow();
			dataRow3["Name"] = "Round to Full Amount";
			dataRow3["Code"] = Function.ParseShort(2);
			dataTable.Rows.Add(dataRow3);
			DataRow dataRow4 = dataTable.NewRow();
			dataRow4["Name"] = "Round to Full Tens Amount";
			dataRow4["Code"] = Function.ParseShort(3);
			dataTable.Rows.Add(dataRow4);
			DataRow dataRow5 = dataTable.NewRow();
			dataRow5["Name"] = "Fixed Ending";
			dataRow5["Code"] = Function.ParseShort(4);
			dataTable.Rows.Add(dataRow5);
			DataRow dataRow6 = dataTable.NewRow();
			dataRow6["Name"] = "Fixed Interval";
			dataRow6["Code"] = Function.ParseShort(5);
			dataTable.Rows.Add(dataRow6);
			dataTable.AcceptChanges();
			rep_luk_RoudingMethod.DataSource = dataTable;
			DataTable dataTable2 = new DataTable();
			dataTable2.Columns.Add("Name", typeof(string));
			dataTable2.Columns.Add("Code", typeof(short));
			DataRow dataRow7 = dataTable2.NewRow();
			dataRow7["Name"] = "Group 1";
			dataRow7["Code"] = Function.ParseShort(1);
			dataTable2.Rows.Add(dataRow7);
			DataRow dataRow8 = dataTable2.NewRow();
			dataRow8["Name"] = "Group 2";
			dataRow8["Code"] = Function.ParseShort(2);
			dataTable2.Rows.Add(dataRow8);
			DataRow dataRow9 = dataTable2.NewRow();
			dataRow9["Name"] = "Group 3";
			dataRow9["Code"] = Function.ParseShort(3);
			dataTable2.Rows.Add(dataRow9);
			DataRow dataRow10 = dataTable2.NewRow();
			dataRow10["Name"] = "Group 4";
			dataRow10["Code"] = Function.ParseShort(4);
			dataTable2.Rows.Add(dataRow10);
			dataTable2.AcceptChanges();
			rep_luk_GroupCode.DataSource = dataTable2;
			DataTable dataTable3 = new DataTable();
			dataTable3.Columns.Add("Name", typeof(string));
			dataTable3.Columns.Add("Code", typeof(string));
			DataRow dataRow11 = dataTable3.NewRow();
			dataRow11["Name"] = "Round to Closest";
			dataRow11["Code"] = "R";
			dataTable3.Rows.Add(dataRow11);
			DataRow dataRow12 = dataTable3.NewRow();
			dataRow12["Name"] = "Round Up";
			dataRow12["Code"] = "C";
			dataTable3.Rows.Add(dataRow12);
			DataRow dataRow13 = dataTable3.NewRow();
			dataRow13["Name"] = "Round Down";
			dataRow13["Code"] = "F";
			dataTable3.Rows.Add(dataRow13);
			dataTable3.AcceptChanges();
			rep_luk_RoudingRule.DataSource = dataTable3;
			DataTable dataTable4 = new DataTable();
			dataTable4.Columns.Add("Name", typeof(string));
			dataTable4.Columns.Add("Code", typeof(string));
			foreach (CurrencyCodesImpl currencyCode in GlobalData.CurrencyCodes)
			{
				DataRow dataRow14 = dataTable4.NewRow();
				dataRow14["Name"] = currencyCode.CurrName;
				dataRow14["Code"] = currencyCode.CurrCode;
				dataTable4.Rows.Add(dataRow14);
			}
			dataTable4.AcceptChanges();
			rep_luk_PrimCurr.Columns.Clear();
			rep_luk_PrimCurr.Columns.Add(new LookUpColumnInfo("Code", "Code"));
			rep_luk_PrimCurr.Columns.Add(new LookUpColumnInfo("Name", "Name"));
			rep_luk_PrimCurr.DataSource = dataTable4;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task frm_PriceLists_Load(object sender, EventArgs e)
	{
		try
		{
			_isLoading = true;
			if (GlobalData.LoginSap)
			{
				btn_InitCurrency.Visible = false;
			}
			await WinFuntions.LoadUdfColumns(gv_Doc, "OPLN");
			RefreshData();
			SetFormatControlValue();
			_isLoading = false;
		}
		catch (Exception ex)
		{
			_isLoading = false;
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_ValidFor_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			BaseEdit obj = sender as BaseEdit;
			gv_Doc.PostEditor();
			if (Function.ToString(obj.EditValue) == "N")
			{
				gv_Doc.SetFocusedRowCellValue("ValidFrom", DBNull.Value);
				gv_Doc.SetFocusedRowCellValue("ValidTo", DBNull.Value);
			}
			gv_Doc.PostEditor();
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
			object focusedRowCellValue = gv_Doc.GetFocusedRowCellValue("ListNum");
			int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("RoundSys"));
			if (Function.ParseInt(focusedRowCellValue) == -1 || Function.ParseInt(focusedRowCellValue) == -2 || gv_Doc.FocusedColumn.FieldName.ToUpper() == "LISTNUM" || gv_Doc.FocusedColumn.FieldName.ToUpper() == "DIFFBSFACTOR" || (num < 4 && gv_Doc.FocusedColumn.FieldName.ToUpper() == "EXTAMOUNT") || (num == 0 && gv_Doc.FocusedColumn.FieldName.ToUpper() == "ROUNDRULE"))
			{
				e.Cancel = true;
			}
			if ((gv_Doc.FocusedColumn.FieldName.ToUpper() == "VALIDFROM" || gv_Doc.FocusedColumn.FieldName.ToUpper() == "VALIDTO") && Function.ToString(gv_Doc.GetFocusedRowCellValue("ValidFor")) != "Y")
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

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void gv_Doc_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Update)
			{
				GridView gridView = (GridView)sender;
				Point pt = gridView.GridControl.PointToClient(Control.MousePosition);
				DoRowDoubleClick(gridView, pt);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
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
			FormMode = Enums.FormViewModeType.Update;
			int listNum = Function.ParseInt(gv_Doc.GetRowCellValue(e.RowHandle, "ListNum"));
			if (listNum != -1 && listNum != -2 && !_lstChanged.Any((int t) => t == listNum))
			{
				_lstChanged.Add(listNum);
			}
			if (e.Column.FieldName == "ListName" && listNum != -1 && listNum != -2)
			{
				DataTable obj = rep_luk_basePricelist.DataSource as DataTable;
				DataRow dataRow = obj.AsEnumerable().FirstOrDefault((DataRow r) => Function.ParseInt(r["ListNum"]) == listNum);
				if (dataRow != null)
				{
					dataRow["ListName"] = Function.ToString(e.Value);
				}
				obj.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bt_OK_Click(object sender, EventArgs e)
	{
		bt_OK.Enabled = false;
		try
		{
			if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				if (grd_Doc.DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
				{
					DataTable dtSave = dataTable.Copy();
					for (int num = dtSave.Rows.Count - 1; num >= 0; num--)
					{
						DataRow row = dtSave.Rows[num];
						if (Function.ParseInt(row["ListNum"]) == -1 || Function.ParseInt(row["ListNum"]) == -2 || !_lstChanged.Any((int t) => t == Function.ParseInt(row["ListNum"])))
						{
							dtSave.Rows.Remove(row);
						}
					}
					dtSave.AcceptChanges();
					DataView defaultView = dtSave.DefaultView;
					defaultView.Sort = "ListNum asc";
					DataTable dataTable2 = defaultView.ToTable();
					if (ValidateData(dtSave))
					{
						if (radioGroup1.SelectedIndex == 1)
						{
							frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListItems, true, string.Empty, "", Enums.ItemObjectType.NoItemType, null, "");
							frm_CFL2.ShowDialog(this);
							if (frm_CFL2.ChooseItems != null && frm_CFL2.ChooseItems.Count > 0)
							{
								bool flag = false;
								double num2 = 0.0;
								string text = "";
								string text2 = "";
								int i;
								for (i = 0; i < dtSave.Rows.Count; i++)
								{
									DataRow changeBasePLOrFactorRow = _dtOriginal.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["ListNum"]) == Function.ParseInt(dtSave.Rows[i]["ListNum"]) && (Function.ParseInt(t["BASE_NUM"]) != Function.ParseInt(dtSave.Rows[i]["BASE_NUM"]) || Function.ParseDouble(t["Factor"]) != Function.ParseDouble(dtSave.Rows[i]["Factor"])));
									if (changeBasePLOrFactorRow != null)
									{
										flag = true;
										num2 = Function.ParseDouble(changeBasePLOrFactorRow["Factor"]);
										DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["ListNum"]) == Function.ParseInt(dtSave.Rows[i]["BASE_NUM"]));
										if (dataRow != null)
										{
											text = Function.ToString(dataRow["ListName"]);
										}
										DataRow dataRow2 = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["ListNum"]) == Function.ParseInt(changeBasePLOrFactorRow["ListNum"]));
										if (dataRow2 != null)
										{
											text2 = Function.ToString(dataRow2["ListName"]);
										}
										break;
									}
								}
								DataTable dataTable3 = new DataTable();
								dataTable3.Columns.Add(new DataColumn("ItemCode", typeof(string)));
								if (flag && XtraMessageBoxEx.Show(Enums.MessageCode.PriceListConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, text2, text, Function.ToString(num2)) == DialogResult.Yes)
								{
									foreach (DataRow chooseItem in frm_CFL2.ChooseItems)
									{
										DataRow dataRow3 = dataTable3.NewRow();
										dataRow3[0] = Function.ToString(chooseItem["ItemCode"]);
										dataTable3.Rows.Add(dataRow3);
									}
								}
								dataTable3.AcceptChanges();
								DataSet dataSet = new DataSet();
								dataSet.Tables.Clear();
								dataTable2.TableName = "DOC";
								DataSet dataSet2 = dataTable2.DataSet;
								if (dataSet2 != null)
								{
									dataSet2.Tables.Remove("DOC");
									dataSet2.AcceptChanges();
								}
								dataTable3.TableName = "ITEM";
								DataSet dataSet3 = dataTable3.DataSet;
								if (dataSet3 != null)
								{
									dataSet3.Tables.Remove("ITEM");
									dataSet3.AcceptChanges();
								}
								dataSet.Tables.Add(dataTable2);
								dataSet.Tables.Add(dataTable3);
								dataSet.AcceptChanges();
								SaveDataSelection(dataSet);
							}
						}
						else
						{
							SaveData(dataTable2);
						}
					}
				}
			}
			else if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		bt_OK.Enabled = true;
	}

	private void addRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			int num = 0;
			if (rep_luk_basePricelist.DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
			{
				num = dataTable.Rows.Count + 1;
				DataRow dataRow = dataTable.NewRow();
				dataRow["ListNum"] = Function.ParseShort(num * -1);
				dataRow["ListName"] = "New Price List " + num;
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
			}
			DataTable obj = grd_Doc.DataSource as DataTable;
			DataRow dataRow2 = obj.NewRow();
			dataRow2["ListNum"] = num * -1;
			dataRow2["ListName"] = "New Price List " + num;
			dataRow2["RoundSys"] = ((DataTable)rep_luk_RoudingMethod.DataSource).Rows[0]["Code"];
			dataRow2["RoundRule"] = ((DataTable)rep_luk_RoudingRule.DataSource).Rows[0]["Code"];
			dataRow2["GroupCode"] = ((DataTable)rep_luk_GroupCode.DataSource).Rows[0]["Code"];
			dataRow2["ValidFor"] = "Y";
			dataRow2["ValidFrom"] = DBNull.Value;
			dataRow2["ValidTo"] = DBNull.Value;
			dataRow2["Factor"] = 1;
			dataRow2["PrimCurr"] = GlobalData.MainCurrency;
			dataRow2["AddCurr1"] = GlobalData.MainCurrency;
			dataRow2["AddCurr2"] = GlobalData.MainCurrency;
			dataRow2["BASE_NUM"] = Function.ParseShort(num * -1);
			obj.Rows.Add(dataRow2);
			obj.AcceptChanges();
			grd_Doc.RefreshDataSource();
			FormMode = Enums.FormViewModeType.Update;
			_lstChanged.Add(num * -1);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_Doc.GetRowCellValue(e.RowHandle, "RoundSys"));
			if ((Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "ValidFor")) != "Y" && (e.Column.FieldName == "ValidFrom" || e.Column.FieldName == "ValidTo")) || e.RowHandle == 0 || e.RowHandle == 1 || e.Column.FieldName == "ListNum" || e.Column.FieldName.ToUpper() == "DIFFBSFACTOR" || (num < 4 && e.Column.FieldName.ToUpper() == "EXTAMOUNT") || (num == 0 && e.Column.FieldName.ToUpper() == "ROUNDRULE"))
			{
				e.Appearance.BackColor = StyleColor.GridDisable;
			}
			if (e.RowHandle == 0 || e.RowHandle == 1)
			{
				e.Appearance.BackColor = StyleColor.GridDisable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "ListNum")
			{
				int num = Function.ParseInt(e.Value);
				if (num == -1 || num == -2)
				{
					e.DisplayText = "";
				}
				else if (num < 0)
				{
					e.DisplayText = (num * -1).ToString();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			Function.ToString(gv_Doc.GetFocusedRowCellValue("ListName"));
			int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("ListNum"));
			if (num != -1 && num != -2 && num <= 0)
			{
				gv_Doc.DeleteRow(gv_Doc.FocusedRowHandle);
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				string text = _httpClient.PriceListSetup.DeletePriceList(num);
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Success(Enums.MessageCode.DeleteSuccess, "");
					RefreshData();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_InitCurrency_Click(object sender, EventArgs e)
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.PriceListConfirmInitForPriceList, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes && string.IsNullOrEmpty(_httpClient.PriceListSetup.CorrectPriceCurrency()))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SettingLanguage()
	{
		try
		{
			bt_OK.Text = Resources.ButtonOK;
			btn_Cancel.Text = Resources.ButtonCancel;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetFormatControlValue()
	{
		try
		{
			string dateTimeFormat = GlobalData.DateTimeFormat;
			rep_ValidDate.Mask.MaskType = MaskType.DateTime;
			rep_ValidDate.Mask.EditMask = dateTimeFormat;
			rep_ValidDate.Mask.UseMaskAsDisplayFormat = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void DoRowDoubleClick(GridView view, Point pt)
	{
		try
		{
			GridHitInfo gridHitInfo = view.CalcHitInfo(pt);
			if ((!gridHitInfo.InRow && !gridHitInfo.InRowCell) || gridHitInfo.Column != null)
			{
				return;
			}
			int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("ListNum"));
			if (num < 1)
			{
				if (num == -1 || num == -2)
				{
					frm_SystemPriceListDetail frm_SystemPriceListDetail2 = new frm_SystemPriceListDetail((SystemPriceList)num);
					frm_SystemPriceListDetail2.MdiParent = MdiParent;
					frm_SystemPriceListDetail2.Show();
				}
			}
			else
			{
				if (!(grd_Doc.DataSource is DataTable dataTable) || dataTable.Rows.Count == 0)
				{
					return;
				}
				DataTable obj = rep_luk_basePricelist.DataSource as DataTable;
				string filterExpression = "ListNum=" + num;
				DataRow[] array = obj.Select(filterExpression);
				frm_STDPriceListDetails frm_STDPriceListDetails2 = new frm_STDPriceListDetails
				{
					_ListNum = Function.ParseInt(num),
					DtPriceList = dataTable.Clone()
				};
				frm_STDPriceListDetails2.DtPriceList.ImportRow(dataTable.Rows[gv_Doc.GetFocusedDataSourceRowIndex()]);
				frm_STDPriceListDetails2.DtPriceList.AcceptChanges();
				if (array[0] != null && array[0]["ListName"] != DBNull.Value)
				{
					frm_STDPriceListDetails2._ListName = array[0]["ListName"].ToString();
				}
				frm_STDPriceListDetails2.MdiParent = MdiParent;
				if (radioGroup1.SelectedIndex == 1)
				{
					frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListItems, true, string.Empty, "", Enums.ItemObjectType.NoItemType, null, "");
					frm_CFL2.ShowDialog(this);
					if (frm_CFL2.ChooseItems != null && frm_CFL2.ChooseItems.Count > 0)
					{
						StringBuilder stringBuilder = new StringBuilder("[ItemCode] in (");
						for (int i = 0; i < frm_CFL2.ChooseItems.Count; i++)
						{
							stringBuilder.Append("'" + frm_CFL2.ChooseItems[i]["ItemCode"]?.ToString() + "',");
						}
						stringBuilder.Length--;
						stringBuilder.Append(")");
						frm_STDPriceListDetails2._strSearch = stringBuilder.ToString();
						frm_STDPriceListDetails2.Show();
					}
				}
				else
				{
					frm_STDPriceListDetails2.Show();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SaveData(DataTable dt)
	{
		try
		{
			string text = _httpClient.PriceListSetup.SaveDoc(dt);
			if (string.IsNullOrWhiteSpace(text))
			{
				WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				RefreshData();
				FormMode = Enums.FormViewModeType.View;
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SaveDataSelection(DataSet ds)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(_httpClient.PriceListSetup.SaveDocSelection(ds)))
			{
				WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				RefreshData();
				FormMode = Enums.FormViewModeType.View;
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.SaveError, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private bool ValidateData(DataTable dt)
	{
		try
		{
			if (dt == null || dt.Rows.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.NoDataToSave, "");
				return false;
			}
			DataRow dataRow = dt.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseDouble(t["Factor"]) <= 0.0);
			if (dataRow != null)
			{
				WinCommons.Error(Enums.MessageCode.InvalidFactor, "");
				int rowHandle = gv_Doc.GetRowHandle(dt.Rows.IndexOf(dataRow));
				gv_Doc.FocusedRowHandle = rowHandle;
				gv_Doc.FocusedColumn = gv_Doc.Columns["Factor"];
				gv_Doc.ShowEditor();
				return false;
			}
			DataTable dataTable = (grd_Doc.DataSource as DataTable).Copy();
			for (int num = 0; num < dataTable.Rows.Count; num++)
			{
				int listNum = Function.ParseInt(dataTable.Rows[num]["ListNum"]);
				string listName = Function.ToString(dataTable.Rows[num]["ListName"]);
				if (dataTable.AsEnumerable().LastOrDefault((DataRow t) => string.Compare(Function.ToString(t["ListName"]), listName, ignoreCase: true) == 0 && Function.ParseInt(t["ListNum"]) != listNum) != null)
				{
					WinCommons.Error(Enums.MessageCode.APriceListWithNamAlreadyExists, "");
					int rowHandle2 = gv_Doc.GetRowHandle(dt.Rows.IndexOf(dataRow));
					gv_Doc.FocusedRowHandle = rowHandle2;
					gv_Doc.FocusedColumn = gv_Doc.Columns["ListName"];
					gv_Doc.ShowEditor();
					return false;
				}
			}
			for (int num2 = dataTable.Rows.Count - 1; num2 >= 0; num2--)
			{
				DataRow dataRow2 = dataTable.Rows[num2];
				if (Function.ParseInt(dataRow2["ListNum"]) == -1 || Function.ParseInt(dataRow2["ListNum"]) == -2)
				{
					dataTable.Rows.Remove(dataRow2);
				}
			}
			dataTable.AcceptChanges();
			for (int num3 = 0; num3 < dt.Rows.Count; num3++)
			{
				int depth = 0;
				int num4 = Function.ParseInt(dt.Rows[num3]["ListNum"]);
				if (GetLastBaseNum(Function.ParseInt(dt.Rows[num3]["BASE_NUM"]), dataTable, num4, ref depth) == num4 && depth > 0)
				{
					WinCommons.Error(Enums.MessageCode.PriceListInfiniteLoopInPriceListDependenciesWithCode, "", Function.ToString(dt.Rows[num3]["ListName"]));
					return false;
				}
			}
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return false;
		}
	}

	private int GetLastBaseNum(int basePLNum, DataTable dtSource, int listNum, ref int depth)
	{
		DataRow dataRow = dtSource.AsEnumerable().FirstOrDefault((DataRow r) => Function.ParseInt(r["ListNum"]) == basePLNum);
		if (dataRow != null && basePLNum != listNum)
		{
			if (Function.ParseInt(dataRow["BASE_NUM"]) != basePLNum)
			{
				depth++;
				return GetLastBaseNum(Function.ParseInt(dataRow["BASE_NUM"]), dtSource, listNum, ref depth);
			}
			return basePLNum;
		}
		return basePLNum;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Inventory.PriceLists.frm_STDPriceLists));
		this.radioGroup1 = new Apzon.Controls.Editor.RadioGroupEx();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.ctx_Doc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gcListNum = new Apzon.Controls.Grid.GridColumnEx();
		this.gcListName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcBASE_NUM = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_luk_basePricelist = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcFactor = new Apzon.Controls.Grid.GridColumnEx();
		this.gcRoundSys = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_luk_RoudingMethod = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcRoundRule = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_luk_RoudingRule = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcExtAmount = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ted_EndingInterVal = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.gcRndFrmtInt = new Apzon.Controls.Grid.GridColumnEx();
		this.gcRndFrmtDec = new Apzon.Controls.Grid.GridColumnEx();
		this.gcGroupCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_luk_GroupCode = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcIsGrossPrc = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_chk_IsGrossPrice = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gcPrimCurr = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_luk_PrimCurr = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ValidFor = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ValidDate = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.bt_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_InitCurrency = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.radioGroup1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.ctx_Doc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_basePricelist).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_RoudingMethod).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_RoudingRule).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ted_EndingInterVal).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_GroupCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_chk_IsGrossPrice).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_PrimCurr).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ValidFor).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ValidDate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ValidDate.CalendarTimeProperties).BeginInit();
		base.SuspendLayout();
		this.radioGroup1.EditControl = null;
		this.radioGroup1.FieldName = null;
		this.radioGroup1.IsSearchControl = false;
		this.radioGroup1.Location = new System.Drawing.Point(7, 4);
		this.radioGroup1.Name = "radioGroup1";
		this.radioGroup1.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.radioGroup1.Properties.Appearance.Options.UseBackColor = true;
		this.radioGroup1.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.radioGroup1.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[2]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Update Entire Price List"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Update By Selection")
		});
		this.radioGroup1.SettingInVisible = false;
		this.radioGroup1.SettingReadonly = false;
		this.radioGroup1.Size = new System.Drawing.Size(295, 49);
		this.radioGroup1.TabIndex = 0;
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.ctx_Doc;
		this.grd_Doc.Location = new System.Drawing.Point(7, 52);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[9] { this.rep_ted_EndingInterVal, this.rep_luk_basePricelist, this.rep_luk_RoudingMethod, this.rep_luk_RoudingRule, this.rep_luk_GroupCode, this.rep_luk_PrimCurr, this.rep_chk_IsGrossPrice, this.rep_ValidFor, this.rep_ValidDate });
		this.grd_Doc.Size = new System.Drawing.Size(875, 389);
		this.grd_Doc.TabIndex = 1;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.ctx_Doc.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addRowToolStripMenuItem, this.deleteRowToolStripMenuItem });
		this.ctx_Doc.Name = "ctx_Doc";
		this.ctx_Doc.Size = new System.Drawing.Size(134, 48);
		this.addRowToolStripMenuItem.Name = "addRowToolStripMenuItem";
		this.addRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.addRowToolStripMenuItem.Text = "Add Row";
		this.addRowToolStripMenuItem.Click += new System.EventHandler(this.addRowToolStripMenuItem_Click);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[18]
		{
			this.gcListNum, this.gcListName, this.gcBASE_NUM, this.gcFactor, this.gcRoundSys, this.gcRoundRule, this.gcExtAmount, this.gcRndFrmtInt, this.gcRndFrmtDec, this.gcGroupCode,
			this.gcIsGrossPrc, this.gcPrimCurr, this.gridColumn2, this.gridColumn1, this.gridColumn3, this.gridColumn4, this.gridColumn5, this.gridColumn6
		});
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oPriceLists";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "OPLN";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Doc_RowCellStyle);
		this.gv_Doc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Doc_ShowingEditor);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
		this.gv_Doc.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gv_Doc_CustomColumnDisplayText);
		this.gv_Doc.DoubleClick += new System.EventHandler(this.gv_Doc_DoubleClick);
		this.gcListNum.Caption = "#";
		this.gcListNum.CurrencyDateField = null;
		this.gcListNum.CurrencyField = null;
		this.gcListNum.CurrencyFixed = null;
		this.gcListNum.CurrencyFixedRate = 0.0;
		this.gcListNum.CurrencyRateField = null;
		this.gcListNum.FieldName = "ListNum";
		this.gcListNum.Name = "gcListNum";
		this.gcListNum.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcListNum.PrimaryKey = true;
		this.gcListNum.UnitField = null;
		this.gcListNum.Visible = true;
		this.gcListNum.VisibleIndex = 0;
		this.gcListName.Caption = "Price List Name";
		this.gcListName.CurrencyDateField = null;
		this.gcListName.CurrencyField = null;
		this.gcListName.CurrencyFixed = null;
		this.gcListName.CurrencyFixedRate = 0.0;
		this.gcListName.CurrencyRateField = null;
		this.gcListName.FieldName = "ListName";
		this.gcListName.Name = "gcListName";
		this.gcListName.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcListName.PrimaryKey = false;
		this.gcListName.UnitField = null;
		this.gcListName.Visible = true;
		this.gcListName.VisibleIndex = 1;
		this.gcListName.Width = 102;
		this.gcBASE_NUM.Caption = "Base Price List";
		this.gcBASE_NUM.ColumnEdit = this.rep_luk_basePricelist;
		this.gcBASE_NUM.CurrencyDateField = null;
		this.gcBASE_NUM.CurrencyField = null;
		this.gcBASE_NUM.CurrencyFixed = null;
		this.gcBASE_NUM.CurrencyFixedRate = 0.0;
		this.gcBASE_NUM.CurrencyRateField = null;
		this.gcBASE_NUM.FieldName = "BASE_NUM";
		this.gcBASE_NUM.Name = "gcBASE_NUM";
		this.gcBASE_NUM.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcBASE_NUM.PrimaryKey = false;
		this.gcBASE_NUM.UnitField = null;
		this.gcBASE_NUM.Visible = true;
		this.gcBASE_NUM.VisibleIndex = 3;
		this.gcBASE_NUM.Width = 104;
		this.rep_luk_basePricelist.AutoHeight = false;
		this.rep_luk_basePricelist.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_luk_basePricelist.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ListName", "Name")
		});
		this.rep_luk_basePricelist.DisplayMember = "ListName";
		this.rep_luk_basePricelist.Name = "rep_luk_basePricelist";
		this.rep_luk_basePricelist.NullText = "";
		this.rep_luk_basePricelist.ValueMember = "ListNum";
		this.gcFactor.Caption = "Default Factor";
		this.gcFactor.CurrencyDateField = null;
		this.gcFactor.CurrencyField = null;
		this.gcFactor.CurrencyFixed = null;
		this.gcFactor.CurrencyFixedRate = 0.0;
		this.gcFactor.CurrencyRateField = null;
		this.gcFactor.FieldName = "Factor";
		this.gcFactor.Name = "gcFactor";
		this.gcFactor.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcFactor.PrimaryKey = false;
		this.gcFactor.UnitField = null;
		this.gcFactor.Visible = true;
		this.gcFactor.VisibleIndex = 4;
		this.gcFactor.Width = 92;
		this.gcRoundSys.Caption = "Rounding Method";
		this.gcRoundSys.ColumnEdit = this.rep_luk_RoudingMethod;
		this.gcRoundSys.CurrencyDateField = null;
		this.gcRoundSys.CurrencyField = null;
		this.gcRoundSys.CurrencyFixed = null;
		this.gcRoundSys.CurrencyFixedRate = 0.0;
		this.gcRoundSys.CurrencyRateField = null;
		this.gcRoundSys.FieldName = "RoundSys";
		this.gcRoundSys.Name = "gcRoundSys";
		this.gcRoundSys.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcRoundSys.PrimaryKey = false;
		this.gcRoundSys.UnitField = null;
		this.gcRoundSys.Visible = true;
		this.gcRoundSys.VisibleIndex = 5;
		this.gcRoundSys.Width = 109;
		this.rep_luk_RoudingMethod.AutoHeight = false;
		this.rep_luk_RoudingMethod.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_luk_RoudingMethod.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rep_luk_RoudingMethod.DisplayMember = "Name";
		this.rep_luk_RoudingMethod.Name = "rep_luk_RoudingMethod";
		this.rep_luk_RoudingMethod.NullText = "";
		this.rep_luk_RoudingMethod.ValueMember = "Code";
		this.gcRoundRule.Caption = "Rounding Rule";
		this.gcRoundRule.ColumnEdit = this.rep_luk_RoudingRule;
		this.gcRoundRule.CurrencyDateField = null;
		this.gcRoundRule.CurrencyField = null;
		this.gcRoundRule.CurrencyFixed = null;
		this.gcRoundRule.CurrencyFixedRate = 0.0;
		this.gcRoundRule.CurrencyRateField = null;
		this.gcRoundRule.FieldName = "RoundRule";
		this.gcRoundRule.Name = "gcRoundRule";
		this.gcRoundRule.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcRoundRule.PrimaryKey = false;
		this.gcRoundRule.UnitField = null;
		this.gcRoundRule.Visible = true;
		this.gcRoundRule.VisibleIndex = 6;
		this.gcRoundRule.Width = 90;
		this.rep_luk_RoudingRule.AutoHeight = false;
		this.rep_luk_RoudingRule.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_luk_RoudingRule.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rep_luk_RoudingRule.DisplayMember = "Name";
		this.rep_luk_RoudingRule.Name = "rep_luk_RoudingRule";
		this.rep_luk_RoudingRule.NullText = "";
		this.rep_luk_RoudingRule.ValueMember = "Code";
		this.gcExtAmount.Caption = "Ending/Interval";
		this.gcExtAmount.ColumnEdit = this.rep_ted_EndingInterVal;
		this.gcExtAmount.CurrencyDateField = null;
		this.gcExtAmount.CurrencyField = null;
		this.gcExtAmount.CurrencyFixed = null;
		this.gcExtAmount.CurrencyFixedRate = 0.0;
		this.gcExtAmount.CurrencyRateField = null;
		this.gcExtAmount.FieldName = "ExtAmount";
		this.gcExtAmount.Name = "gcExtAmount";
		this.gcExtAmount.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcExtAmount.PrimaryKey = false;
		this.gcExtAmount.UnitField = null;
		this.gcExtAmount.Visible = true;
		this.gcExtAmount.VisibleIndex = 7;
		this.gcExtAmount.Width = 98;
		this.rep_ted_EndingInterVal.AutoHeight = false;
		this.rep_ted_EndingInterVal.Mask.BeepOnError = true;
		this.rep_ted_EndingInterVal.Mask.EditMask = "\\d{3}\\.\\d{3}";
		this.rep_ted_EndingInterVal.Mask.IgnoreMaskBlank = false;
		this.rep_ted_EndingInterVal.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
		this.rep_ted_EndingInterVal.Name = "rep_ted_EndingInterVal";
		this.gcRndFrmtInt.Caption = "Ending/Interval";
		this.gcRndFrmtInt.CurrencyDateField = null;
		this.gcRndFrmtInt.CurrencyField = null;
		this.gcRndFrmtInt.CurrencyFixed = null;
		this.gcRndFrmtInt.CurrencyFixedRate = 0.0;
		this.gcRndFrmtInt.CurrencyRateField = null;
		this.gcRndFrmtInt.FieldName = "RndFrmtInt";
		this.gcRndFrmtInt.Name = "gcRndFrmtInt";
		this.gcRndFrmtInt.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcRndFrmtInt.PrimaryKey = false;
		this.gcRndFrmtInt.UnitField = null;
		this.gcRndFrmtInt.Width = 97;
		this.gcRndFrmtDec.Caption = "Ending/Interval";
		this.gcRndFrmtDec.CurrencyDateField = null;
		this.gcRndFrmtDec.CurrencyField = null;
		this.gcRndFrmtDec.CurrencyFixed = null;
		this.gcRndFrmtDec.CurrencyFixedRate = 0.0;
		this.gcRndFrmtDec.CurrencyRateField = null;
		this.gcRndFrmtDec.FieldName = "RndFrmtDec";
		this.gcRndFrmtDec.Name = "gcRndFrmtDec";
		this.gcRndFrmtDec.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcRndFrmtDec.PrimaryKey = false;
		this.gcRndFrmtDec.UnitField = null;
		this.gcRndFrmtDec.Width = 99;
		this.gcGroupCode.Caption = "Authorization Group";
		this.gcGroupCode.ColumnEdit = this.rep_luk_GroupCode;
		this.gcGroupCode.CurrencyDateField = null;
		this.gcGroupCode.CurrencyField = null;
		this.gcGroupCode.CurrencyFixed = null;
		this.gcGroupCode.CurrencyFixedRate = 0.0;
		this.gcGroupCode.CurrencyRateField = null;
		this.gcGroupCode.FieldName = "GroupCode";
		this.gcGroupCode.Name = "gcGroupCode";
		this.gcGroupCode.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcGroupCode.PrimaryKey = false;
		this.gcGroupCode.UnitField = null;
		this.gcGroupCode.Visible = true;
		this.gcGroupCode.VisibleIndex = 8;
		this.gcGroupCode.Width = 112;
		this.rep_luk_GroupCode.AutoHeight = false;
		this.rep_luk_GroupCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_luk_GroupCode.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rep_luk_GroupCode.DisplayMember = "Name";
		this.rep_luk_GroupCode.Name = "rep_luk_GroupCode";
		this.rep_luk_GroupCode.NullText = "";
		this.rep_luk_GroupCode.ValueMember = "Code";
		this.gcIsGrossPrc.Caption = "Gross Price";
		this.gcIsGrossPrc.ColumnEdit = this.rep_chk_IsGrossPrice;
		this.gcIsGrossPrc.CurrencyDateField = null;
		this.gcIsGrossPrc.CurrencyField = null;
		this.gcIsGrossPrc.CurrencyFixed = null;
		this.gcIsGrossPrc.CurrencyFixedRate = 0.0;
		this.gcIsGrossPrc.CurrencyRateField = null;
		this.gcIsGrossPrc.FieldName = "IsGrossPrc";
		this.gcIsGrossPrc.Name = "gcIsGrossPrc";
		this.gcIsGrossPrc.OptionsColumn.AllowEdit = false;
		this.gcIsGrossPrc.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcIsGrossPrc.PrimaryKey = false;
		this.gcIsGrossPrc.UnitField = null;
		this.gcIsGrossPrc.Visible = true;
		this.gcIsGrossPrc.VisibleIndex = 9;
		this.gcIsGrossPrc.Width = 80;
		this.rep_chk_IsGrossPrice.AutoHeight = false;
		this.rep_chk_IsGrossPrice.DisplayValueChecked = "Y";
		this.rep_chk_IsGrossPrice.DisplayValueGrayed = "N";
		this.rep_chk_IsGrossPrice.DisplayValueUnchecked = "N";
		this.rep_chk_IsGrossPrice.Name = "rep_chk_IsGrossPrice";
		this.rep_chk_IsGrossPrice.ValueChecked = "Y";
		this.rep_chk_IsGrossPrice.ValueGrayed = "N";
		this.rep_chk_IsGrossPrice.ValueUnchecked = "N";
		this.gcPrimCurr.Caption = "Primary Default Currency";
		this.gcPrimCurr.ColumnEdit = this.rep_luk_PrimCurr;
		this.gcPrimCurr.CurrencyDateField = null;
		this.gcPrimCurr.CurrencyField = null;
		this.gcPrimCurr.CurrencyFixed = null;
		this.gcPrimCurr.CurrencyFixedRate = 0.0;
		this.gcPrimCurr.CurrencyRateField = null;
		this.gcPrimCurr.FieldName = "PrimCurr";
		this.gcPrimCurr.Name = "gcPrimCurr";
		this.gcPrimCurr.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcPrimCurr.PrimaryKey = false;
		this.gcPrimCurr.UnitField = null;
		this.gcPrimCurr.Visible = true;
		this.gcPrimCurr.VisibleIndex = 10;
		this.gcPrimCurr.Width = 131;
		this.rep_luk_PrimCurr.AutoHeight = false;
		this.rep_luk_PrimCurr.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_luk_PrimCurr.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rep_luk_PrimCurr.DisplayMember = "Name";
		this.rep_luk_PrimCurr.Name = "rep_luk_PrimCurr";
		this.rep_luk_PrimCurr.NullText = "";
		this.rep_luk_PrimCurr.ValueMember = "Code";
		this.gridColumn2.Caption = "Differ Base Price List/Factor";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "DiffBsFactor";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.OptionsColumn.AllowEdit = false;
		this.gridColumn2.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn2.OptionsColumn.ReadOnly = true;
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 2;
		this.gridColumn2.Width = 144;
		this.gridColumn1.Caption = "Additional Currency 1";
		this.gridColumn1.ColumnEdit = this.rep_luk_PrimCurr;
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "AddCurr1";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 11;
		this.gridColumn3.Caption = "Additional Currency 2";
		this.gridColumn3.ColumnEdit = this.rep_luk_PrimCurr;
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "AddCurr2";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 12;
		this.gridColumn4.Caption = "Active";
		this.gridColumn4.ColumnEdit = this.rep_ValidFor;
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "ValidFor";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 13;
		this.rep_ValidFor.AutoHeight = false;
		this.rep_ValidFor.DisplayValueChecked = "Y";
		this.rep_ValidFor.DisplayValueGrayed = "N";
		this.rep_ValidFor.DisplayValueUnchecked = "N";
		this.rep_ValidFor.Name = "rep_ValidFor";
		this.rep_ValidFor.ValueChecked = "Y";
		this.rep_ValidFor.ValueGrayed = "N";
		this.rep_ValidFor.ValueUnchecked = "N";
		this.rep_ValidFor.EditValueChanged += new System.EventHandler(this.rep_ValidFor_EditValueChanged);
		this.gridColumn5.Caption = "Valid From";
		this.gridColumn5.ColumnEdit = this.rep_ValidDate;
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.FieldName = "ValidFrom";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.UnitField = null;
		this.gridColumn5.Visible = true;
		this.gridColumn5.VisibleIndex = 14;
		this.rep_ValidDate.AutoHeight = false;
		this.rep_ValidDate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ValidDate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ValidDate.Name = "rep_ValidDate";
		this.gridColumn6.Caption = "Valid To";
		this.gridColumn6.ColumnEdit = this.rep_ValidDate;
		this.gridColumn6.CurrencyDateField = null;
		this.gridColumn6.CurrencyField = null;
		this.gridColumn6.CurrencyFixed = null;
		this.gridColumn6.CurrencyFixedRate = 0.0;
		this.gridColumn6.CurrencyRateField = null;
		this.gridColumn6.FieldName = "ValidTo";
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.PrimaryKey = false;
		this.gridColumn6.UnitField = null;
		this.gridColumn6.Visible = true;
		this.gridColumn6.VisibleIndex = 15;
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(86, 447);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 4;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.bt_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.bt_OK.EditControl = null;
		this.bt_OK.EditValue = "OK";
		this.bt_OK.FieldName = null;
		this.bt_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bt_OK.ImageOptions.Image");
		this.bt_OK.IsSearchControl = false;
		this.bt_OK.Location = new System.Drawing.Point(7, 447);
		this.bt_OK.Name = "bt_OK";
		this.bt_OK.ReadOnly = false;
		this.bt_OK.SettingInVisible = false;
		this.bt_OK.SettingReadonly = false;
		this.bt_OK.Size = new System.Drawing.Size(75, 23);
		this.bt_OK.TabIndex = 3;
		this.bt_OK.Text = "OK";
		this.bt_OK.Click += new System.EventHandler(this.bt_OK_Click);
		this.btn_InitCurrency.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_InitCurrency.EditControl = null;
		this.btn_InitCurrency.EditValue = "Initialize currency";
		this.btn_InitCurrency.FieldName = null;
		this.btn_InitCurrency.IsSearchControl = false;
		this.btn_InitCurrency.Location = new System.Drawing.Point(776, 447);
		this.btn_InitCurrency.Name = "btn_InitCurrency";
		this.btn_InitCurrency.ReadOnly = false;
		this.btn_InitCurrency.SettingInVisible = false;
		this.btn_InitCurrency.SettingReadonly = false;
		this.btn_InitCurrency.Size = new System.Drawing.Size(106, 23);
		this.btn_InitCurrency.TabIndex = 5;
		this.btn_InitCurrency.Text = "Initialize currency";
		this.btn_InitCurrency.Click += new System.EventHandler(this.btn_InitCurrency_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(890, 476);
		base.Controls.Add(this.btn_InitCurrency);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.bt_OK);
		base.Controls.Add(this.grd_Doc);
		base.Controls.Add(this.radioGroup1);
		base.FormId = "oPriceLists";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_STDPriceLists";
		this.Text = "Price Lists";
		((System.ComponentModel.ISupportInitialize)this.radioGroup1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.ctx_Doc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_basePricelist).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_RoudingMethod).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_RoudingRule).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ted_EndingInterVal).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_GroupCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_chk_IsGrossPrice).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_PrimCurr).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ValidFor).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ValidDate.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ValidDate).EndInit();
		base.ResumeLayout(false);
	}
}
