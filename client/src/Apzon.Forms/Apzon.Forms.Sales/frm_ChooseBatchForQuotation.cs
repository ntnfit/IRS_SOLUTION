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
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.UserFields;
using Apzon.Forms.Commons;
using Apzon.Forms.Helper;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Sales;

public class frm_ChooseBatchForQuotation : XtraFormEx
{
	public delegate void DelegateFillBatchs(string batchs);

	public DataTable _tableTinhGiay = new DataTable();

	private DataTable _tableGhepKho = new DataTable();

	private DataTable _tableDaChon = new DataTable();

	public DataTable _tableChonGiay = new DataTable();

	private DataSet ds;

	public bool isChonGiayClick;

	private const string FormController = "TinhGiay";

	public DelegateFillBatchs DelegateFillBatch;

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	private SplitContainerControl splitContainerControl2;

	private GroupControl groupControl1;

	private GridControlEx grd_DSTinhGiay;

	private GridViewEx gv_DSTinhGiay;

	private GroupControl groupControl2;

	private SimpleButtonEx btn_ChonGhepKho;

	private GridControlEx grd_DSGhepKho;

	private GridViewEx gv_DSGhepKho;

	private GroupControl groupControl3;

	private SplitContainerControl splitContainerControl3;

	private GroupControl groupControl5;

	private SimpleButtonEx btn_ChayTinhGiay;

	private GridControlEx grd_Kho;

	private GridViewEx gv_Kho;

	private GridColumnEx clkh_WhsCode;

	private GridColumnEx clkh_WhsName;

	private GroupControl groupControl4;

	private SimpleButtonEx btn_ChonGiay;

	private GridControlEx grd_DSCuon;

	private GridViewEx gv_DSCuon;

	private GridColumnEx cltg_ItemCode;

	private GridColumnEx cltg_ItemName;

	private GridColumnEx cltg_Batch;

	private GridColumnEx cltg_QtyM;

	private GridColumnEx cltg_Kho;

	private GridColumnEx cltg_Dai;

	private GridColumnEx cltg_QtyPCS;

	private GridColumnEx gridColumn10;

	private GridColumnEx cltg_XoayKho;

	private GridColumnEx cltg_H1;

	private GridColumnEx cltg_H2;

	private GridColumnEx cltg_Joins;

	private GridColumnEx cltg_KhongGhep;

	private GridColumnEx cltg_HXa;

	private GridColumnEx cltg_GhepKho;

	private GridColumnEx clgk_ItemCode;

	private GridColumnEx clgk_ItemName;

	private GridColumnEx clgk_TotalQty;

	private GridColumnEx clgk_TotalWidth;

	private GridColumnEx clgk_H1;

	private GridColumnEx clgk_H2;

	private GridColumnEx clgk_Joins;

	private GridColumnEx clgk_ChieuDaiTinhGiay;

	private GridColumnEx clgk_XenToiDa;

	private GridColumnEx clgk_HHToiThieu;

	private GridColumnEx clgk_Check;

	private RepositoryItemCheckEdit repo_Check;

	private GridColumnEx clcg_ItemCode;

	private GridColumnEx clcg_ItemName;

	private GridColumnEx clcg_Batch;

	private GridColumnEx clcg_Kho;

	private GridColumnEx clcg_Dai;

	private GridColumnEx clcg_QtyReq;

	private GridColumnEx clcg_QtyInv;

	private GridColumnEx clcg_Joins;

	private GridColumnEx clcg_Whse;

	private GridColumnEx clcg_DangTon;

	private GridColumnEx clcg_KhoThuHoi;

	private GridColumnEx clcg_DaiThuHoi;

	private GridColumnEx clcg_LeKho;

	private GridColumnEx clcg_H2;

	private GridColumnEx clcg_QtyTTinh;

	private GridColumnEx clcg_Check;

	private RepositoryItemCheckEdit repositoryItemCheckEdit3;

	private RepositoryItemCalcEdit repositoryItemCalcEdit2;

	private RepositoryItemCheckEdit repositoryItemCheckEdit5;

	private RepositoryItemCalcEdit repositoryItemCalcEdit1;

	private GridColumnEx cltg_ID;

	private GridColumnEx clcg_ID;

	private GridColumnEx clgk_Batch;

	private GridColumnEx clgk_ID;

	private GridColumnEx clgk_TotalLenght;

	private RepositoryItemSpinEdit rep_Spin_N2;

	private GridColumnEx clkh_Status;

	private RepositoryItemLookUpEdit repo_LookUp_Status;

	private RepositoryItemMemoEdit repositoryItemMemoEdit1;

	private RepositoryItemMemoExEdit repositoryItemMemoExEdit1;

	private GridColumnEx clgk_QtyPcs;

	private GridColumnEx clgk_Uom;

	private SimpleButtonEx btn_XemTonKho;

	private SplitContainerControl splitContainerControl4;

	private GroupControl groupControl6;

	private GridControlEx grd_DSDaChon;

	private GridViewEx gv_DSDaChon;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumn3;

	private GridColumnEx gridColumn4;

	private GridColumnEx gridColumn5;

	private RepositoryItemCalcEdit repositoryItemCalcEdit3;

	private GridColumnEx gridColumn6;

	private GridColumnEx gridColumn7;

	private GridColumnEx gridColumn8;

	private GridColumnEx gridColumn9;

	private GridColumnEx gridColumn11;

	private GridColumnEx gridColumn12;

	private GridColumnEx gridColumn13;

	private GridColumnEx gridColumn14;

	private GridColumnEx gridColumn15;

	private GridColumnEx gridColumn16;

	private RepositoryItemCheckEdit repositoryItemCheckEdit1;

	private GridColumnEx gridColumn17;

	private GridColumnEx gridColumn18;

	private GridColumnEx cl_SoLuongDaChon;

	private GridColumnEx cl_SoLuongConLai;

	public frm_ChooseBatchForQuotation()
	{
		InitializeComponent();
	}

	public frm_ChooseBatchForQuotation(DataTable dtTinhGiay)
	{
		InitializeComponent();
		if (!dtTinhGiay.IsNotNull())
		{
			return;
		}
		_tableTinhGiay = CreateTableTinhGiay();
		for (int i = 0; i < dtTinhGiay.Rows.Count; i++)
		{
			DataRow dataRow = _tableTinhGiay.Rows.Add();
			dataRow["ID"] = i;
			if (dtTinhGiay.Rows[i].IsExistsColumn("U_ItemCode"))
			{
				dataRow["ItemCode"] = Function.ToString(dtTinhGiay.Rows[i]["U_ItemCode"]);
				dataRow["ItemName"] = (dtTinhGiay.Rows[i].IsExistsColumn("U_ItemCode") ? Function.ToString(dtTinhGiay.Rows[i]["U_ItemName"]) : "");
				dataRow["Batch"] = "";
				dataRow["Qty"] = (dtTinhGiay.Rows[i].IsExistsColumn("U_Quantity") ? Function.ParseDouble(dtTinhGiay.Rows[i]["U_Quantity"]) : 0.0);
				dataRow["Width"] = (dtTinhGiay.Rows[i].IsExistsColumn("U_Width") ? Function.ParseDouble(dtTinhGiay.Rows[i]["U_Width"]) : 0.0);
				dataRow["Length"] = (dtTinhGiay.Rows[i].IsExistsColumn("U_Length") ? Function.ParseDouble(dtTinhGiay.Rows[i]["U_Length"]) : 0.0);
				dataRow["QtyPcs"] = (dtTinhGiay.Rows[i].IsExistsColumn("U_QtyUoM") ? Function.ParseDouble(dtTinhGiay.Rows[i]["U_QtyUoM"]) : 0.0);
				dataRow["Uom"] = (dtTinhGiay.Rows[i].IsExistsColumn("U_UnitUoM") ? Function.ToString(dtTinhGiay.Rows[i]["U_UnitUoM"]) : "");
				dataRow["XoayKho"] = dtTinhGiay.Rows[i].IsExistsColumn("XoayKho") && Function.ParseBool(dtTinhGiay.Rows[i]["XoayKho"]);
				dataRow["H1"] = dtTinhGiay.Rows[i].IsExistsColumn("H1") && Function.ParseBool(dtTinhGiay.Rows[i]["H1"]);
				dataRow["H2"] = dtTinhGiay.Rows[i].IsExistsColumn("H2") && Function.ParseBool(dtTinhGiay.Rows[i]["H2"]);
				dataRow["Joins"] = (dtTinhGiay.Rows[i].IsExistsColumn("Joins") ? Function.ParseDouble(dtTinhGiay.Rows[i]["Joins"]) : 0.0);
				dataRow["KhongGhep"] = dtTinhGiay.Rows[i].IsExistsColumn("KhongGhep") && Function.ParseBool(dtTinhGiay.Rows[i]["KhongGhep"]);
				dataRow["Hxa"] = dtTinhGiay.Rows[i].IsExistsColumn("Hxa") && Function.ParseBool(dtTinhGiay.Rows[i]["Hxa"]);
				dataRow["GhepKho"] = dtTinhGiay.Rows[i].IsExistsColumn("GhepKho") && Function.ParseBool(dtTinhGiay.Rows[i]["GhepKho"]);
			}
		}
	}

	private async void frm_TinhGiay_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (_tableTinhGiay != null && _tableTinhGiay.Rows.Count > 0)
			{
				grd_DSTinhGiay.DataSource = _tableTinhGiay;
				grd_DSTinhGiay.RefreshDataSource();
			}
			LoadRepoGrid();
			DataTable dataSource = await LoadDsKho(GlobalData.DefaultBranch);
			grd_Kho.DataSource = dataSource;
			grd_Kho.RefreshDataSource();
			await WinFuntions.LoadUdfColumns(gv_Kho, "OWHS");
			btn_ChayTinhGiay.Enabled = false;
			btn_XemTonKho.Enabled = false;
			btn_ChonGiay.Enabled = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_DSTinhGiay_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column == cltg_GhepKho && Function.ParseBool(gv_DSTinhGiay.GetRowCellValue(gv_DSTinhGiay.FocusedRowHandle, cltg_GhepKho)))
			{
				gv_DSTinhGiay.SetRowCellValue(e.RowHandle, cltg_KhongGhep, false);
			}
			else if (e.Column == cltg_KhongGhep && Function.ParseBool(gv_DSTinhGiay.GetRowCellValue(gv_DSTinhGiay.FocusedRowHandle, cltg_KhongGhep)))
			{
				gv_DSTinhGiay.SetRowCellValue(e.RowHandle, cltg_GhepKho, false);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_ChonGhepKho_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			DataTable dataTable = (DataTable)grd_DSTinhGiay.DataSource;
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				DataTable dataTable2 = dataTable.Copy();
				dataTable2.Columns.Remove("SoLuongDaChon");
				dataTable2.Columns.Remove("SoLuongConLai");
				DataTable dataSource = await TinhGhepKho(dataTable2);
				grd_DSGhepKho.DataSource = dataSource;
				grd_DSGhepKho.RefreshDataSource();
				btn_ChayTinhGiay.Enabled = true;
				btn_XemTonKho.Enabled = true;
				btn_ChonGiay.Enabled = false;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void btn_ChonGiay_Click(object sender, EventArgs e)
	{
		try
		{
			if (grd_DSTinhGiay.DataSource != null)
			{
				_tableChonGiay = (DataTable)grd_DSTinhGiay.DataSource;
				if (_tableChonGiay != null && _tableChonGiay.Rows.Count > 0)
				{
					isChonGiayClick = true;
				}
			}
			else
			{
				XtraMessageBox.Show("Chưa chọn item chọn giấy!", Resources.Msg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		Close();
	}

	private void gv_DSCuon_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			string batch = "";
			if (gvExistColumn(gv_DSCuon, "Batch"))
			{
				batch = gv_DSCuon.GetRowCellDisplayText(e.RowHandle, "Batch");
			}
			string rowCellDisplayText = gv_DSCuon.GetRowCellDisplayText(e.RowHandle, "Checked");
			int focusedRowHandle = gv_DSTinhGiay.FocusedRowHandle;
			if (focusedRowHandle >= 0 && rowCellDisplayText == "Checked")
			{
				string text = Function.ToString(gv_DSTinhGiay.GetRowCellValue(focusedRowHandle, cltg_Batch));
				if (string.IsNullOrEmpty(text))
				{
					text = batch;
				}
				else
				{
					text = text + ";" + batch;
					text = string.Join(";", text.Split(';').Distinct());
				}
				gv_DSTinhGiay.SetRowCellValue(focusedRowHandle, cltg_Batch, text);
				DataRow dataRow = gv_DSTinhGiay.GetDataRow(focusedRowHandle);
				double num = Function.ParseDouble(dataRow["Width"]);
				double num2 = Function.ParseDouble(dataRow["Length"]);
				double num3 = Function.ParseDouble(dataRow["QtyPcs"]);
				double num4 = Function.ParseDouble(dataRow["SoLuongDaChon"]);
				if (Function.ParseBool(dataRow["XoayKho"]))
				{
					num = num2 * 100.0;
					num2 = Function.ParseDouble(dataRow["Width"]) / 100.0;
				}
				DataRow dataRow2 = gv_DSCuon.GetDataRow(e.RowHandle);
				double num5 = Function.ParseDouble(dataRow2["Width"]);
				double num6 = Function.ParseDouble(dataRow2["Length"]);
				double num7 = Function.ParseDouble(dataRow2["QtyPcs"]);
				double num8 = (double)(Convert.ToInt32(num5 / num) * Convert.ToInt32(num6 / num2)) * num7;
				dataRow["SoLuongDaChon"] = num4 + num8;
				dataRow["SoLuongConLai"] = num3 - Function.ParseDouble(dataRow["SoLuongDaChon"]);
				_tableDaChon.ImportRow(gv_DSCuon.GetDataRow(e.RowHandle));
				grd_DSDaChon.DataSource = _tableDaChon;
				grd_DSDaChon.RefreshDataSource();
				((DataTable)grd_DSCuon.DataSource).Rows.Remove(gv_DSCuon.GetDataRow(e.RowHandle));
			}
			else if (focusedRowHandle >= 0 && rowCellDisplayText == "Unchecked")
			{
				string text2 = Function.ToString(gv_DSTinhGiay.GetRowCellValue(focusedRowHandle, cltg_Batch));
				if (!string.IsNullOrEmpty(text2))
				{
					string[] array = (from o in text2.Split(';')
						where !o.Contains(batch)
						select o).Distinct().ToArray();
					if (array.Length != 0)
					{
						text2 = array[0];
						for (int num9 = 1; num9 < array.Length; num9++)
						{
							text2 = text2 + ";" + array[num9];
						}
					}
					else
					{
						text2 = "";
					}
				}
				gv_DSTinhGiay.SetRowCellValue(focusedRowHandle, cltg_Batch, text2);
				if (_tableDaChon.IsNotNull())
				{
					DataRow[] array2 = _tableDaChon.Select($"Batch = '{batch}'");
					if (array2 != null && array2.Length != 0)
					{
						_tableDaChon.Rows.Remove(array2[0]);
					}
				}
			}
			DataTable dataTable = (DataTable)grd_DSTinhGiay.DataSource;
			for (int num10 = 0; num10 < dataTable.Rows.Count; num10++)
			{
				if (num10 != focusedRowHandle)
				{
					string text3 = Function.ToString(dataTable.Rows[num10]["Batch"]);
					dataTable.Rows[num10]["Batch"] = string.Join(";", (from o in text3.Split(';')
						where !o.Contains(batch)
						select o).Distinct());
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_ChayTinhGiay_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			WinCommons._OpenProcessing();
			string text = "";
			if (grd_Kho.DataSource != null && grd_DSGhepKho.DataSource != null)
			{
				if (gv_Kho.GetSelectedRows() != null && gv_Kho.GetSelectedRows().Length != 0)
				{
					for (int i = 0; i < gv_Kho.GetSelectedRows().Length; i++)
					{
						int rowHandle = gv_Kho.GetSelectedRows()[i];
						if (!string.IsNullOrEmpty(gv_Kho.GetRowCellValue(rowHandle, clkh_WhsCode).ToString()))
						{
							text = text + gv_Kho.GetRowCellValue(rowHandle, clkh_WhsCode)?.ToString() + ",";
						}
					}
					if (!string.IsNullOrEmpty(text))
					{
						text = text.Substring(0, text.Length - 1);
					}
				}
				DataTable dataTable = (DataTable)grd_DSGhepKho.DataSource;
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					ds = new DataSet();
					ds.Tables.Clear();
					ds = await TinhChonGiay(dataTable, text);
					if (ds.IsHasStructure())
					{
						List<UserFieldsImpl> lstUDF = await LoadUDF("QUT1");
						GridViewHelper.GenerateGrid(grd_DSCuon, gv_DSCuon, ds.Tables[0], ds.Tables[1], null, lstUDF, expand: true);
						if (!_tableDaChon.IsNotNull())
						{
							_tableDaChon = ds.Tables[1].Copy();
							GridViewHelper.GenerateGrid(grd_DSDaChon, gv_DSDaChon, ds.Tables[0], _tableDaChon, null, lstUDF, expand: true);
							_tableDaChon.Rows.Clear();
						}
						else
						{
							for (int j = 0; j < _tableDaChon.Rows.Count; j++)
							{
								DataRow[] array = ds.Tables[1].Select(string.Format("Batch = '{0}'", _tableDaChon.Rows[j]["Batch"]));
								if (array != null && array.Length != 0)
								{
									ds.Tables[1].Rows.Remove(array[0]);
								}
							}
						}
					}
					else
					{
						grd_DSCuon.DataSource = new DataTable();
						grd_DSCuon.RefreshDataSource();
					}
				}
				btn_ChonGiay.Enabled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private async void btn_XemTonKho_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			WinCommons._OpenProcessing();
			string text = "";
			if (grd_Kho.DataSource != null && grd_DSGhepKho.DataSource != null)
			{
				if (gv_Kho.GetSelectedRows() != null && gv_Kho.GetSelectedRows().Length != 0)
				{
					for (int i = 0; i < gv_Kho.GetSelectedRows().Length; i++)
					{
						int rowHandle = gv_Kho.GetSelectedRows()[i];
						if (!string.IsNullOrEmpty(gv_Kho.GetRowCellValue(rowHandle, clkh_WhsCode).ToString()))
						{
							text = text + gv_Kho.GetRowCellValue(rowHandle, clkh_WhsCode)?.ToString() + ",";
						}
					}
					if (!string.IsNullOrEmpty(text))
					{
						text = text.Substring(0, text.Length - 1);
					}
				}
				DataTable dataTable = (DataTable)grd_DSGhepKho.DataSource;
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					ds = new DataSet();
					ds.Tables.Clear();
					ds = await XemTonKho(dataTable, text);
					if (ds.IsHasStructure())
					{
						List<UserFieldsImpl> lstUDF = await LoadUDF("QUT1");
						GridViewHelper.GenerateGrid(grd_DSCuon, gv_DSCuon, ds.Tables[0], ds.Tables[1], null, lstUDF, expand: true);
						if (!_tableDaChon.IsNotNull())
						{
							_tableDaChon = ds.Tables[1].Copy();
							GridViewHelper.GenerateGrid(grd_DSDaChon, gv_DSDaChon, ds.Tables[0], _tableDaChon, null, lstUDF, expand: true);
							_tableDaChon.Rows.Clear();
						}
						else
						{
							for (int j = 0; j < _tableDaChon.Rows.Count; j++)
							{
								DataRow[] array = ds.Tables[1].Select(string.Format("Batch = '{0}'", _tableDaChon.Rows[j]["Batch"]));
								if (array != null && array.Length != 0)
								{
									ds.Tables[1].Rows.Remove(array[0]);
								}
							}
						}
					}
					else
					{
						grd_DSCuon.DataSource = new DataTable();
						grd_DSCuon.RefreshDataSource();
					}
				}
				btn_ChonGiay.Enabled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void gv_DSTinhGiay_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gv_DSGhepKho_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gv_Kho_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gv_DSCuon_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gv_Kho_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			if (e.RowHandle > -1 && gv_Kho.GetRowCellValue(e.RowHandle, clkh_Status).ToString().Equals("2"))
			{
				e.Appearance.BackColor = Color.Gray;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_DSCuon_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			string text = "";
			int focusedRowHandle = gv_DSCuon.FocusedRowHandle;
			if (gvExistColumn(gv_DSCuon, "Batch"))
			{
				text = gv_DSCuon.GetRowCellDisplayText(focusedRowHandle, "Batch");
			}
			int focusedRowHandle2 = gv_DSTinhGiay.FocusedRowHandle;
			if (focusedRowHandle2 >= 0)
			{
				string text2 = Function.ToString(gv_DSTinhGiay.GetRowCellValue(focusedRowHandle2, cltg_Batch));
				if (string.IsNullOrEmpty(text2))
				{
					text2 = text;
				}
				else
				{
					text2 = text2 + ";" + text;
					text2 = string.Join(";", text2.Split(';').Distinct());
				}
				gv_DSTinhGiay.SetRowCellValue(focusedRowHandle2, cltg_Batch, text2);
				DataRow dataRow = gv_DSTinhGiay.GetDataRow(focusedRowHandle2);
				double num = Function.ParseDouble(dataRow["Width"]);
				double num2 = Function.ParseDouble(dataRow["Length"]);
				double num3 = Function.ParseDouble(dataRow["QtyPcs"]);
				double num4 = Function.ParseDouble(dataRow["SoLuongDaChon"]);
				if (Function.ParseBool(dataRow["XoayKho"]))
				{
					num = num2 * 100.0;
					num2 = Function.ParseDouble(dataRow["Width"]) / 100.0;
				}
				DataRow dataRow2 = gv_DSCuon.GetDataRow(focusedRowHandle);
				double num5 = Function.ParseDouble(dataRow2["Width"]);
				double num6 = Function.ParseDouble(dataRow2["Length"]);
				double num7 = Function.ParseDouble(dataRow2["QtyPcs"]);
				double num8 = (double)(Convert.ToInt32(num5 / num) * Convert.ToInt32(num6 / num2)) * num7;
				dataRow["SoLuongDaChon"] = num4 + num8;
				dataRow["SoLuongConLai"] = num3 - Function.ParseDouble(dataRow["SoLuongDaChon"]);
				_tableDaChon.ImportRow(gv_DSCuon.GetDataRow(focusedRowHandle));
				grd_DSDaChon.DataSource = _tableDaChon;
				grd_DSDaChon.RefreshDataSource();
				((DataTable)grd_DSCuon.DataSource).Rows.Remove(gv_DSCuon.GetDataRow(focusedRowHandle));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private DataTable CreateTableTinhGiay()
	{
		DataTable dataTable = new DataTable();
		try
		{
			dataTable.Columns.Add("ID", typeof(int));
			dataTable.Columns.Add("ItemCode", typeof(string));
			dataTable.Columns.Add("ItemName", typeof(string));
			dataTable.Columns.Add("Batch", typeof(string));
			dataTable.Columns.Add("Qty", typeof(double));
			dataTable.Columns.Add("Width", typeof(double));
			dataTable.Columns.Add("Length", typeof(double));
			dataTable.Columns.Add("QtyPcs", typeof(double));
			dataTable.Columns.Add("Uom", typeof(string));
			dataTable.Columns.Add("XoayKho", typeof(bool));
			dataTable.Columns.Add("H1", typeof(bool));
			dataTable.Columns.Add("H2", typeof(bool));
			dataTable.Columns.Add("Joins", typeof(double));
			dataTable.Columns.Add("KhongGhep", typeof(bool));
			dataTable.Columns.Add("Hxa", typeof(bool));
			dataTable.Columns.Add("GhepKho", typeof(bool));
			dataTable.Columns.Add("SoLuongDaChon", typeof(double));
			dataTable.Columns.Add("SoLuongConLai", typeof(double));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return dataTable;
	}

	private async Task<List<UserFieldsImpl>> LoadUDF(string tableName)
	{
		List<UserFieldsImpl> lst = new List<UserFieldsImpl>();
		try
		{
			using HttpClientHelper<UserFieldsImpl> helper = new HttpClientHelper<UserFieldsImpl>(Configuration.ApiServer, "UDF");
			lst = await helper.GetListAsyncByPr("GetListField", new { tableName });
			if (lst != null && lst.Count > 0)
			{
				return lst;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return lst;
	}

	private async Task<DataTable> LoadDsKho(int branchId)
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "TinhGiay");
			return await _helper.GetAsync("GetWarehouseForBatch", new { branchId });
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async Task<DataTable> TinhGhepKho(DataTable dtTinhGiay)
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "TinhGiay");
			return await _helper.PostAsync("TinhGhepKho", dtTinhGiay);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async Task<DataSet> TinhChonGiay(DataTable dtGhepKho, string whs)
	{
		try
		{
			using HttpClientHelper<DataSet> _helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "TinhGiay");
			return await _helper.PostAsync("TinhChonGiay", new Tuple<DataTable, string>(dtGhepKho, whs));
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
	}

	private async Task<DataSet> XemTonKho(DataTable dtGhepKho, string whs)
	{
		try
		{
			using HttpClientHelper<DataSet> _helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "TinhGiay");
			return await _helper.PostAsync("XemTonKho", new Tuple<DataTable, string>(dtGhepKho, whs));
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
	}

	private void LoadRepoGrid()
	{
		try
		{
			List<Common.Table_ID> list = new List<Common.Table_ID>();
			list.Add(new Common.Table_ID
			{
				Name = "Không áp dụng",
				ID = "0"
			});
			list.Add(new Common.Table_ID
			{
				Name = "Áp dụng",
				ID = "1"
			});
			list.Add(new Common.Table_ID
			{
				Name = "Tham khảo",
				ID = "2"
			});
			repo_LookUp_Status.DataSource = list;
			repo_LookUp_Status.DisplayMember = "Name";
			repo_LookUp_Status.ValueMember = "ID";
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private bool gvExistColumn(GridView view, string fieldName)
	{
		try
		{
			return view.Columns.Where((GridColumn t) => t.FieldName == fieldName) != null;
		}
		catch (Exception)
		{
			return false;
		}
	}

	private void frm_ChooseBatchForQuotation_FormClosed(object sender, FormClosedEventArgs e)
	{
		try
		{
			if (ds.IsNotNull())
			{
				ds.Tables.Clear();
				ds.Dispose();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_DSDaChon_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			string batch = "";
			int focusedRowHandle = gv_DSDaChon.FocusedRowHandle;
			if (gvExistColumn(gv_DSDaChon, "Batch"))
			{
				batch = gv_DSDaChon.GetRowCellDisplayText(focusedRowHandle, "Batch");
			}
			int focusedRowHandle2 = gv_DSTinhGiay.FocusedRowHandle;
			if (focusedRowHandle2 >= 0)
			{
				string text = Function.ToString(gv_DSTinhGiay.GetRowCellValue(focusedRowHandle2, cltg_Batch));
				if (!string.IsNullOrEmpty(text))
				{
					string[] array = (from o in text.Split(';')
						where !o.Contains(batch)
						select o).Distinct().ToArray();
					if (array.Length != 0)
					{
						text = array[0];
						for (int num = 1; num < array.Length; num++)
						{
							text = text + ";" + array[num];
						}
					}
					else
					{
						text = "";
					}
				}
				gv_DSTinhGiay.SetRowCellValue(focusedRowHandle2, cltg_Batch, text);
				((DataTable)grd_DSCuon.DataSource).ImportRow(gv_DSDaChon.GetDataRow(focusedRowHandle));
				((DataTable)grd_DSDaChon.DataSource).Rows.Remove(gv_DSDaChon.GetDataRow(focusedRowHandle));
			}
			DataTable dataTable = (DataTable)grd_DSTinhGiay.DataSource;
			for (int num2 = 0; num2 < dataTable.Rows.Count; num2++)
			{
				if (num2 != focusedRowHandle2)
				{
					string text2 = Function.ToString(dataTable.Rows[num2]["Batch"]);
					dataTable.Rows[num2]["Batch"] = string.Join(";", (from o in text2.Split(';')
						where !o.Contains(batch)
						select o).Distinct());
				}
			}
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Sales.frm_ChooseBatchForQuotation));
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.splitContainerControl4 = new DevExpress.XtraEditors.SplitContainerControl();
		this.splitContainerControl2 = new DevExpress.XtraEditors.SplitContainerControl();
		this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
		this.grd_DSTinhGiay = new Apzon.Controls.Grid.GridControlEx();
		this.gv_DSTinhGiay = new Apzon.Controls.Grid.GridViewEx();
		this.cltg_ID = new Apzon.Controls.Grid.GridColumnEx();
		this.cltg_ItemCode = new Apzon.Controls.Grid.GridColumnEx();
		this.cltg_ItemName = new Apzon.Controls.Grid.GridColumnEx();
		this.cltg_Batch = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoExEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.cltg_QtyM = new Apzon.Controls.Grid.GridColumnEx();
		this.cltg_Kho = new Apzon.Controls.Grid.GridColumnEx();
		this.cltg_Dai = new Apzon.Controls.Grid.GridColumnEx();
		this.cltg_QtyPCS = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn10 = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_SoLuongDaChon = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_SoLuongConLai = new Apzon.Controls.Grid.GridColumnEx();
		this.cltg_XoayKho = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCheckEdit5 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.cltg_H1 = new Apzon.Controls.Grid.GridColumnEx();
		this.cltg_H2 = new Apzon.Controls.Grid.GridColumnEx();
		this.cltg_Joins = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.cltg_KhongGhep = new Apzon.Controls.Grid.GridColumnEx();
		this.cltg_HXa = new Apzon.Controls.Grid.GridColumnEx();
		this.cltg_GhepKho = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
		this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
		this.btn_ChonGhepKho = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_DSGhepKho = new Apzon.Controls.Grid.GridControlEx();
		this.gv_DSGhepKho = new Apzon.Controls.Grid.GridViewEx();
		this.clgk_ID = new Apzon.Controls.Grid.GridColumnEx();
		this.clgk_ItemCode = new Apzon.Controls.Grid.GridColumnEx();
		this.clgk_ItemName = new Apzon.Controls.Grid.GridColumnEx();
		this.clgk_Batch = new Apzon.Controls.Grid.GridColumnEx();
		this.clgk_TotalQty = new Apzon.Controls.Grid.GridColumnEx();
		this.clgk_TotalLenght = new Apzon.Controls.Grid.GridColumnEx();
		this.clgk_TotalWidth = new Apzon.Controls.Grid.GridColumnEx();
		this.clgk_QtyPcs = new Apzon.Controls.Grid.GridColumnEx();
		this.clgk_Uom = new Apzon.Controls.Grid.GridColumnEx();
		this.clgk_H1 = new Apzon.Controls.Grid.GridColumnEx();
		this.clgk_H2 = new Apzon.Controls.Grid.GridColumnEx();
		this.clgk_Joins = new Apzon.Controls.Grid.GridColumnEx();
		this.clgk_ChieuDaiTinhGiay = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Spin_N2 = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
		this.clgk_XenToiDa = new Apzon.Controls.Grid.GridColumnEx();
		this.repo_Check = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.clgk_HHToiThieu = new Apzon.Controls.Grid.GridColumnEx();
		this.clgk_Check = new Apzon.Controls.Grid.GridColumnEx();
		this.groupControl6 = new DevExpress.XtraEditors.GroupControl();
		this.grd_DSDaChon = new Apzon.Controls.Grid.GridControlEx();
		this.gv_DSDaChon = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn11 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn12 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn13 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn14 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn15 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn16 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gridColumn17 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn18 = new Apzon.Controls.Grid.GridColumnEx();
		this.groupControl3 = new DevExpress.XtraEditors.GroupControl();
		this.splitContainerControl3 = new DevExpress.XtraEditors.SplitContainerControl();
		this.groupControl5 = new DevExpress.XtraEditors.GroupControl();
		this.btn_XemTonKho = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_ChayTinhGiay = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Kho = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Kho = new Apzon.Controls.Grid.GridViewEx();
		this.clkh_WhsCode = new Apzon.Controls.Grid.GridColumnEx();
		this.clkh_WhsName = new Apzon.Controls.Grid.GridColumnEx();
		this.clkh_Status = new Apzon.Controls.Grid.GridColumnEx();
		this.repo_LookUp_Status = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.groupControl4 = new DevExpress.XtraEditors.GroupControl();
		this.btn_ChonGiay = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_DSCuon = new Apzon.Controls.Grid.GridControlEx();
		this.gv_DSCuon = new Apzon.Controls.Grid.GridViewEx();
		this.clcg_ID = new Apzon.Controls.Grid.GridColumnEx();
		this.clcg_ItemCode = new Apzon.Controls.Grid.GridColumnEx();
		this.clcg_ItemName = new Apzon.Controls.Grid.GridColumnEx();
		this.clcg_Batch = new Apzon.Controls.Grid.GridColumnEx();
		this.clcg_Kho = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.clcg_Dai = new Apzon.Controls.Grid.GridColumnEx();
		this.clcg_QtyReq = new Apzon.Controls.Grid.GridColumnEx();
		this.clcg_QtyInv = new Apzon.Controls.Grid.GridColumnEx();
		this.clcg_Joins = new Apzon.Controls.Grid.GridColumnEx();
		this.clcg_Whse = new Apzon.Controls.Grid.GridColumnEx();
		this.clcg_DangTon = new Apzon.Controls.Grid.GridColumnEx();
		this.clcg_KhoThuHoi = new Apzon.Controls.Grid.GridColumnEx();
		this.clcg_DaiThuHoi = new Apzon.Controls.Grid.GridColumnEx();
		this.clcg_LeKho = new Apzon.Controls.Grid.GridColumnEx();
		this.clcg_H2 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCheckEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.clcg_QtyTTinh = new Apzon.Controls.Grid.GridColumnEx();
		this.clcg_Check = new Apzon.Controls.Grid.GridColumnEx();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl4).BeginInit();
		this.splitContainerControl4.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).BeginInit();
		this.splitContainerControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.groupControl1).BeginInit();
		this.groupControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_DSTinhGiay).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_DSTinhGiay).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl2).BeginInit();
		this.groupControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_DSGhepKho).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_DSGhepKho).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Spin_N2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repo_Check).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl6).BeginInit();
		this.groupControl6.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_DSDaChon).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_DSDaChon).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl3).BeginInit();
		this.groupControl3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl3).BeginInit();
		this.splitContainerControl3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.groupControl5).BeginInit();
		this.groupControl5.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Kho).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Kho).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repo_LookUp_Status).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl4).BeginInit();
		this.groupControl4.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_DSCuon).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_DSCuon).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit3).BeginInit();
		base.SuspendLayout();
		this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.splitContainerControl4);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.groupControl3);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(897, 524);
		this.splitContainerControl1.SplitterPosition = 323;
		this.splitContainerControl1.TabIndex = 0;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.splitContainerControl4.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl4.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl4.Name = "splitContainerControl4";
		this.splitContainerControl4.Panel1.Controls.Add(this.splitContainerControl2);
		this.splitContainerControl4.Panel1.Text = "Panel1";
		this.splitContainerControl4.Panel2.Controls.Add(this.groupControl6);
		this.splitContainerControl4.Panel2.Text = "Panel2";
		this.splitContainerControl4.Size = new System.Drawing.Size(897, 323);
		this.splitContainerControl4.SplitterPosition = 605;
		this.splitContainerControl4.TabIndex = 1;
		this.splitContainerControl4.Text = "splitContainerControl4";
		this.splitContainerControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl2.Horizontal = false;
		this.splitContainerControl2.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.splitContainerControl2.Name = "splitContainerControl2";
		this.splitContainerControl2.Panel1.Controls.Add(this.groupControl1);
		this.splitContainerControl2.Panel1.Text = "Panel1";
		this.splitContainerControl2.Panel2.Controls.Add(this.groupControl2);
		this.splitContainerControl2.Panel2.Text = "Panel2";
		this.splitContainerControl2.Size = new System.Drawing.Size(605, 323);
		this.splitContainerControl2.SplitterPosition = 159;
		this.splitContainerControl2.TabIndex = 0;
		this.splitContainerControl2.Text = "splitContainerControl2";
		this.groupControl1.AppearanceCaption.Font = new System.Drawing.Font("Tahoma", 7.8f, System.Drawing.FontStyle.Bold);
		this.groupControl1.AppearanceCaption.Options.UseFont = true;
		this.groupControl1.Controls.Add(this.grd_DSTinhGiay);
		this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.groupControl1.Location = new System.Drawing.Point(0, 0);
		this.groupControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.groupControl1.Name = "groupControl1";
		this.groupControl1.Size = new System.Drawing.Size(605, 159);
		this.groupControl1.TabIndex = 1;
		this.groupControl1.Text = "Danh sách mặt hàng tính giấy";
		this.grd_DSTinhGiay.AllowPasteExcel = true;
		this.grd_DSTinhGiay.ClipboardData = null;
		this.grd_DSTinhGiay.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_DSTinhGiay.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grd_DSTinhGiay.Location = new System.Drawing.Point(2, 23);
		this.grd_DSTinhGiay.MainView = this.gv_DSTinhGiay;
		this.grd_DSTinhGiay.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grd_DSTinhGiay.Name = "grd_DSTinhGiay";
		this.grd_DSTinhGiay.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.repositoryItemCalcEdit2, this.repositoryItemCheckEdit5, this.repositoryItemMemoEdit1, this.repositoryItemMemoExEdit1 });
		this.grd_DSTinhGiay.Size = new System.Drawing.Size(601, 134);
		this.grd_DSTinhGiay.TabIndex = 0;
		this.grd_DSTinhGiay.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_DSTinhGiay });
		this.gv_DSTinhGiay.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_DSTinhGiay.ColumnKey = null;
		this.gv_DSTinhGiay.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[18]
		{
			this.cltg_ID, this.cltg_ItemCode, this.cltg_ItemName, this.cltg_Batch, this.cltg_QtyM, this.cltg_Kho, this.cltg_Dai, this.cltg_QtyPCS, this.gridColumn10, this.cl_SoLuongDaChon,
			this.cl_SoLuongConLai, this.cltg_XoayKho, this.cltg_H1, this.cltg_H2, this.cltg_Joins, this.cltg_KhongGhep, this.cltg_HXa, this.cltg_GhepKho
		});
		this.gv_DSTinhGiay.DisableFormSetting = false;
		this.gv_DSTinhGiay.DisplayException = false;
		this.gv_DSTinhGiay.EditColumn = null;
		this.gv_DSTinhGiay.FieldCurrency = null;
		this.gv_DSTinhGiay.FieldRate = null;
		this.gv_DSTinhGiay.FormId = null;
		this.gv_DSTinhGiay.GridControl = this.grd_DSTinhGiay;
		this.gv_DSTinhGiay.IgnoreFilter = false;
		this.gv_DSTinhGiay.IndexEditRow = -1;
		this.gv_DSTinhGiay.IndicatorWidth = 35;
		this.gv_DSTinhGiay.IsApzTable = false;
		this.gv_DSTinhGiay.IsInsertNewRow = false;
		this.gv_DSTinhGiay.IsLoadingData = false;
		this.gv_DSTinhGiay.IsMappingContextAction = false;
		this.gv_DSTinhGiay.ItemId = null;
		this.gv_DSTinhGiay.LstColumnAllowBatch = null;
		this.gv_DSTinhGiay.LstColumnNotAllowBatch = null;
		this.gv_DSTinhGiay.LstColumnNotEditable = null;
		this.gv_DSTinhGiay.LstColumNotSetting = null;
		this.gv_DSTinhGiay.Name = "gv_DSTinhGiay";
		this.gv_DSTinhGiay.OptionsView.ColumnAutoWidth = false;
		this.gv_DSTinhGiay.OptionsView.RowAutoHeight = true;
		this.gv_DSTinhGiay.OptionsView.ShowGroupPanel = false;
		this.gv_DSTinhGiay.UseBatchMod = false;
		this.gv_DSTinhGiay.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_DSTinhGiay_CustomDrawRowIndicator);
		this.gv_DSTinhGiay.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_DSTinhGiay_CellValueChanged);
		this.cltg_ID.Caption = "ID";
		this.cltg_ID.CurrencyDateField = null;
		this.cltg_ID.CurrencyField = null;
		this.cltg_ID.CurrencyFixed = null;
		this.cltg_ID.CurrencyFixedRate = 0.0;
		this.cltg_ID.CurrencyRateField = null;
		this.cltg_ID.FieldName = "ID";
		this.cltg_ID.Name = "cltg_ID";
		this.cltg_ID.PrimaryKey = true;
		this.cltg_ID.UnitField = null;
		this.cltg_ItemCode.Caption = "Mã hàng";
		this.cltg_ItemCode.CurrencyDateField = null;
		this.cltg_ItemCode.CurrencyField = null;
		this.cltg_ItemCode.CurrencyFixed = null;
		this.cltg_ItemCode.CurrencyFixedRate = 0.0;
		this.cltg_ItemCode.CurrencyRateField = null;
		this.cltg_ItemCode.FieldName = "ItemCode";
		this.cltg_ItemCode.Name = "cltg_ItemCode";
		this.cltg_ItemCode.PrimaryKey = false;
		this.cltg_ItemCode.UnitField = null;
		this.cltg_ItemCode.Visible = true;
		this.cltg_ItemCode.VisibleIndex = 0;
		this.cltg_ItemCode.Width = 120;
		this.cltg_ItemName.Caption = "Tên hàng";
		this.cltg_ItemName.CurrencyDateField = null;
		this.cltg_ItemName.CurrencyField = null;
		this.cltg_ItemName.CurrencyFixed = null;
		this.cltg_ItemName.CurrencyFixedRate = 0.0;
		this.cltg_ItemName.CurrencyRateField = null;
		this.cltg_ItemName.FieldName = "ItemName";
		this.cltg_ItemName.Name = "cltg_ItemName";
		this.cltg_ItemName.PrimaryKey = false;
		this.cltg_ItemName.UnitField = null;
		this.cltg_ItemName.Width = 150;
		this.cltg_Batch.Caption = "Batch No";
		this.cltg_Batch.ColumnEdit = this.repositoryItemMemoExEdit1;
		this.cltg_Batch.CurrencyDateField = null;
		this.cltg_Batch.CurrencyField = null;
		this.cltg_Batch.CurrencyFixed = null;
		this.cltg_Batch.CurrencyFixedRate = 0.0;
		this.cltg_Batch.CurrencyRateField = null;
		this.cltg_Batch.FieldName = "Batch";
		this.cltg_Batch.Name = "cltg_Batch";
		this.cltg_Batch.PrimaryKey = false;
		this.cltg_Batch.UnitField = null;
		this.cltg_Batch.Visible = true;
		this.cltg_Batch.VisibleIndex = 1;
		this.cltg_Batch.Width = 110;
		this.repositoryItemMemoExEdit1.AutoHeight = false;
		this.repositoryItemMemoExEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit1.Name = "repositoryItemMemoExEdit1";
		this.repositoryItemMemoExEdit1.ShowIcon = false;
		this.cltg_QtyM.Caption = "Qty (m2)";
		this.cltg_QtyM.CurrencyDateField = null;
		this.cltg_QtyM.CurrencyField = null;
		this.cltg_QtyM.CurrencyFixed = null;
		this.cltg_QtyM.CurrencyFixedRate = 0.0;
		this.cltg_QtyM.CurrencyRateField = null;
		this.cltg_QtyM.FieldName = "Qty";
		this.cltg_QtyM.Name = "cltg_QtyM";
		this.cltg_QtyM.PrimaryKey = false;
		this.cltg_QtyM.UnitField = null;
		this.cltg_QtyM.Visible = true;
		this.cltg_QtyM.VisibleIndex = 8;
		this.cltg_QtyM.Width = 80;
		this.cltg_Kho.Caption = "Khổ (cm)";
		this.cltg_Kho.CurrencyDateField = null;
		this.cltg_Kho.CurrencyField = null;
		this.cltg_Kho.CurrencyFixed = null;
		this.cltg_Kho.CurrencyFixedRate = 0.0;
		this.cltg_Kho.CurrencyRateField = null;
		this.cltg_Kho.FieldName = "Width";
		this.cltg_Kho.Name = "cltg_Kho";
		this.cltg_Kho.PrimaryKey = false;
		this.cltg_Kho.UnitField = null;
		this.cltg_Kho.Visible = true;
		this.cltg_Kho.VisibleIndex = 2;
		this.cltg_Kho.Width = 80;
		this.cltg_Dai.Caption = "Dài (m)";
		this.cltg_Dai.CurrencyDateField = null;
		this.cltg_Dai.CurrencyField = null;
		this.cltg_Dai.CurrencyFixed = null;
		this.cltg_Dai.CurrencyFixedRate = 0.0;
		this.cltg_Dai.CurrencyRateField = null;
		this.cltg_Dai.FieldName = "Length";
		this.cltg_Dai.Name = "cltg_Dai";
		this.cltg_Dai.PrimaryKey = false;
		this.cltg_Dai.UnitField = null;
		this.cltg_Dai.Visible = true;
		this.cltg_Dai.VisibleIndex = 3;
		this.cltg_Dai.Width = 80;
		this.cltg_QtyPCS.Caption = "Qty (pcs)";
		this.cltg_QtyPCS.CurrencyDateField = null;
		this.cltg_QtyPCS.CurrencyField = null;
		this.cltg_QtyPCS.CurrencyFixed = null;
		this.cltg_QtyPCS.CurrencyFixedRate = 0.0;
		this.cltg_QtyPCS.CurrencyRateField = null;
		this.cltg_QtyPCS.FieldName = "QtyPcs";
		this.cltg_QtyPCS.Name = "cltg_QtyPCS";
		this.cltg_QtyPCS.PrimaryKey = false;
		this.cltg_QtyPCS.UnitField = null;
		this.cltg_QtyPCS.Visible = true;
		this.cltg_QtyPCS.VisibleIndex = 4;
		this.cltg_QtyPCS.Width = 80;
		this.gridColumn10.Caption = "Đơn vị ";
		this.gridColumn10.CurrencyDateField = null;
		this.gridColumn10.CurrencyField = null;
		this.gridColumn10.CurrencyFixed = null;
		this.gridColumn10.CurrencyFixedRate = 0.0;
		this.gridColumn10.CurrencyRateField = null;
		this.gridColumn10.FieldName = "Uom";
		this.gridColumn10.Name = "gridColumn10";
		this.gridColumn10.OptionsColumn.AllowEdit = false;
		this.gridColumn10.PrimaryKey = false;
		this.gridColumn10.UnitField = null;
		this.gridColumn10.Visible = true;
		this.gridColumn10.VisibleIndex = 5;
		this.gridColumn10.Width = 60;
		this.cl_SoLuongDaChon.Caption = "SL đã chọn";
		this.cl_SoLuongDaChon.CurrencyDateField = null;
		this.cl_SoLuongDaChon.CurrencyField = null;
		this.cl_SoLuongDaChon.CurrencyFixed = null;
		this.cl_SoLuongDaChon.CurrencyFixedRate = 0.0;
		this.cl_SoLuongDaChon.CurrencyRateField = null;
		this.cl_SoLuongDaChon.FieldName = "SoLuongDaChon";
		this.cl_SoLuongDaChon.Name = "cl_SoLuongDaChon";
		this.cl_SoLuongDaChon.PrimaryKey = false;
		this.cl_SoLuongDaChon.UnitField = null;
		this.cl_SoLuongDaChon.Visible = true;
		this.cl_SoLuongDaChon.VisibleIndex = 6;
		this.cl_SoLuongConLai.Caption = "SL còn lại";
		this.cl_SoLuongConLai.CurrencyDateField = null;
		this.cl_SoLuongConLai.CurrencyField = null;
		this.cl_SoLuongConLai.CurrencyFixed = null;
		this.cl_SoLuongConLai.CurrencyFixedRate = 0.0;
		this.cl_SoLuongConLai.CurrencyRateField = null;
		this.cl_SoLuongConLai.FieldName = "SoLuongConLai";
		this.cl_SoLuongConLai.Name = "cl_SoLuongConLai";
		this.cl_SoLuongConLai.PrimaryKey = false;
		this.cl_SoLuongConLai.UnitField = null;
		this.cl_SoLuongConLai.Visible = true;
		this.cl_SoLuongConLai.VisibleIndex = 7;
		this.cltg_XoayKho.Caption = "Xoay khổ";
		this.cltg_XoayKho.ColumnEdit = this.repositoryItemCheckEdit5;
		this.cltg_XoayKho.CurrencyDateField = null;
		this.cltg_XoayKho.CurrencyField = null;
		this.cltg_XoayKho.CurrencyFixed = null;
		this.cltg_XoayKho.CurrencyFixedRate = 0.0;
		this.cltg_XoayKho.CurrencyRateField = null;
		this.cltg_XoayKho.FieldName = "XoayKho";
		this.cltg_XoayKho.Name = "cltg_XoayKho";
		this.cltg_XoayKho.PrimaryKey = false;
		this.cltg_XoayKho.UnitField = null;
		this.cltg_XoayKho.Visible = true;
		this.cltg_XoayKho.VisibleIndex = 9;
		this.cltg_XoayKho.Width = 60;
		this.repositoryItemCheckEdit5.AutoHeight = false;
		this.repositoryItemCheckEdit5.Name = "repositoryItemCheckEdit5";
		this.cltg_H1.Caption = "H1";
		this.cltg_H1.ColumnEdit = this.repositoryItemCheckEdit5;
		this.cltg_H1.CurrencyDateField = null;
		this.cltg_H1.CurrencyField = null;
		this.cltg_H1.CurrencyFixed = null;
		this.cltg_H1.CurrencyFixedRate = 0.0;
		this.cltg_H1.CurrencyRateField = null;
		this.cltg_H1.FieldName = "H1";
		this.cltg_H1.Name = "cltg_H1";
		this.cltg_H1.PrimaryKey = false;
		this.cltg_H1.UnitField = null;
		this.cltg_H1.Width = 60;
		this.cltg_H2.Caption = "H2";
		this.cltg_H2.ColumnEdit = this.repositoryItemCheckEdit5;
		this.cltg_H2.CurrencyDateField = null;
		this.cltg_H2.CurrencyField = null;
		this.cltg_H2.CurrencyFixed = null;
		this.cltg_H2.CurrencyFixedRate = 0.0;
		this.cltg_H2.CurrencyRateField = null;
		this.cltg_H2.FieldName = "H2";
		this.cltg_H2.Name = "cltg_H2";
		this.cltg_H2.PrimaryKey = false;
		this.cltg_H2.UnitField = null;
		this.cltg_H2.Width = 60;
		this.cltg_Joins.Caption = "Mối nối";
		this.cltg_Joins.ColumnEdit = this.repositoryItemCalcEdit2;
		this.cltg_Joins.CurrencyDateField = null;
		this.cltg_Joins.CurrencyField = null;
		this.cltg_Joins.CurrencyFixed = null;
		this.cltg_Joins.CurrencyFixedRate = 0.0;
		this.cltg_Joins.CurrencyRateField = null;
		this.cltg_Joins.FieldName = "Joins";
		this.cltg_Joins.Name = "cltg_Joins";
		this.cltg_Joins.PrimaryKey = false;
		this.cltg_Joins.UnitField = null;
		this.cltg_Joins.Width = 60;
		this.repositoryItemCalcEdit2.AutoHeight = false;
		this.repositoryItemCalcEdit2.DisplayFormat.FormatString = "n2";
		this.repositoryItemCalcEdit2.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.repositoryItemCalcEdit2.EditFormat.FormatString = "n2";
		this.repositoryItemCalcEdit2.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.repositoryItemCalcEdit2.Mask.EditMask = "n2";
		this.repositoryItemCalcEdit2.Name = "repositoryItemCalcEdit2";
		this.cltg_KhongGhep.Caption = "Không ghép";
		this.cltg_KhongGhep.ColumnEdit = this.repositoryItemCheckEdit5;
		this.cltg_KhongGhep.CurrencyDateField = null;
		this.cltg_KhongGhep.CurrencyField = null;
		this.cltg_KhongGhep.CurrencyFixed = null;
		this.cltg_KhongGhep.CurrencyFixedRate = 0.0;
		this.cltg_KhongGhep.CurrencyRateField = null;
		this.cltg_KhongGhep.FieldName = "KhongGhep";
		this.cltg_KhongGhep.Name = "cltg_KhongGhep";
		this.cltg_KhongGhep.PrimaryKey = false;
		this.cltg_KhongGhep.UnitField = null;
		this.cltg_KhongGhep.Width = 60;
		this.cltg_HXa.Caption = "H. Xả";
		this.cltg_HXa.ColumnEdit = this.repositoryItemCheckEdit5;
		this.cltg_HXa.CurrencyDateField = null;
		this.cltg_HXa.CurrencyField = null;
		this.cltg_HXa.CurrencyFixed = null;
		this.cltg_HXa.CurrencyFixedRate = 0.0;
		this.cltg_HXa.CurrencyRateField = null;
		this.cltg_HXa.FieldName = "Hxa";
		this.cltg_HXa.Name = "cltg_HXa";
		this.cltg_HXa.PrimaryKey = false;
		this.cltg_HXa.UnitField = null;
		this.cltg_HXa.Width = 60;
		this.cltg_GhepKho.Caption = "Ghép khổ";
		this.cltg_GhepKho.ColumnEdit = this.repositoryItemCheckEdit5;
		this.cltg_GhepKho.CurrencyDateField = null;
		this.cltg_GhepKho.CurrencyField = null;
		this.cltg_GhepKho.CurrencyFixed = null;
		this.cltg_GhepKho.CurrencyFixedRate = 0.0;
		this.cltg_GhepKho.CurrencyRateField = null;
		this.cltg_GhepKho.FieldName = "GhepKho";
		this.cltg_GhepKho.Name = "cltg_GhepKho";
		this.cltg_GhepKho.PrimaryKey = false;
		this.cltg_GhepKho.UnitField = null;
		this.cltg_GhepKho.Visible = true;
		this.cltg_GhepKho.VisibleIndex = 10;
		this.cltg_GhepKho.Width = 60;
		this.repositoryItemMemoEdit1.Name = "repositoryItemMemoEdit1";
		this.groupControl2.AppearanceCaption.Font = new System.Drawing.Font("Tahoma", 7.8f, System.Drawing.FontStyle.Bold);
		this.groupControl2.AppearanceCaption.Options.UseFont = true;
		this.groupControl2.Controls.Add(this.btn_ChonGhepKho);
		this.groupControl2.Controls.Add(this.grd_DSGhepKho);
		this.groupControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.groupControl2.Location = new System.Drawing.Point(0, 0);
		this.groupControl2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.groupControl2.Name = "groupControl2";
		this.groupControl2.Size = new System.Drawing.Size(605, 154);
		this.groupControl2.TabIndex = 2;
		this.groupControl2.Text = "Danh sách ghép khổ";
		this.btn_ChonGhepKho.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_ChonGhepKho.EditControl = null;
		this.btn_ChonGhepKho.EditValue = "Chọn - Ghép khổ";
		this.btn_ChonGhepKho.FieldName = null;
		this.btn_ChonGhepKho.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_ChonGhepKho.ImageOptions.Image");
		this.btn_ChonGhepKho.IsSearchControl = false;
		this.btn_ChonGhepKho.Location = new System.Drawing.Point(484, 0);
		this.btn_ChonGhepKho.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.btn_ChonGhepKho.Name = "btn_ChonGhepKho";
		this.btn_ChonGhepKho.ReadOnly = false;
		this.btn_ChonGhepKho.SettingInVisible = false;
		this.btn_ChonGhepKho.SettingReadonly = false;
		this.btn_ChonGhepKho.Size = new System.Drawing.Size(117, 19);
		this.btn_ChonGhepKho.TabIndex = 0;
		this.btn_ChonGhepKho.Text = "Chọn - Ghép khổ";
		this.btn_ChonGhepKho.Click += new System.EventHandler(this.btn_ChonGhepKho_Click);
		this.grd_DSGhepKho.AllowPasteExcel = true;
		this.grd_DSGhepKho.ClipboardData = null;
		this.grd_DSGhepKho.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_DSGhepKho.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grd_DSGhepKho.Location = new System.Drawing.Point(2, 23);
		this.grd_DSGhepKho.MainView = this.gv_DSGhepKho;
		this.grd_DSGhepKho.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grd_DSGhepKho.Name = "grd_DSGhepKho";
		this.grd_DSGhepKho.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.repo_Check, this.rep_Spin_N2 });
		this.grd_DSGhepKho.Size = new System.Drawing.Size(601, 129);
		this.grd_DSGhepKho.TabIndex = 0;
		this.grd_DSGhepKho.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_DSGhepKho });
		this.gv_DSGhepKho.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_DSGhepKho.ColumnKey = null;
		this.gv_DSGhepKho.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[16]
		{
			this.clgk_ID, this.clgk_ItemCode, this.clgk_ItemName, this.clgk_Batch, this.clgk_TotalQty, this.clgk_TotalLenght, this.clgk_TotalWidth, this.clgk_QtyPcs, this.clgk_Uom, this.clgk_H1,
			this.clgk_H2, this.clgk_Joins, this.clgk_ChieuDaiTinhGiay, this.clgk_XenToiDa, this.clgk_HHToiThieu, this.clgk_Check
		});
		this.gv_DSGhepKho.DisableFormSetting = false;
		this.gv_DSGhepKho.DisplayException = false;
		this.gv_DSGhepKho.EditColumn = null;
		this.gv_DSGhepKho.FieldCurrency = null;
		this.gv_DSGhepKho.FieldRate = null;
		this.gv_DSGhepKho.FormId = null;
		this.gv_DSGhepKho.GridControl = this.grd_DSGhepKho;
		this.gv_DSGhepKho.IgnoreFilter = false;
		this.gv_DSGhepKho.IndexEditRow = -1;
		this.gv_DSGhepKho.IndicatorWidth = 35;
		this.gv_DSGhepKho.IsApzTable = false;
		this.gv_DSGhepKho.IsInsertNewRow = false;
		this.gv_DSGhepKho.IsLoadingData = false;
		this.gv_DSGhepKho.IsMappingContextAction = false;
		this.gv_DSGhepKho.ItemId = null;
		this.gv_DSGhepKho.LstColumnAllowBatch = null;
		this.gv_DSGhepKho.LstColumnNotAllowBatch = null;
		this.gv_DSGhepKho.LstColumnNotEditable = null;
		this.gv_DSGhepKho.LstColumNotSetting = null;
		this.gv_DSGhepKho.Name = "gv_DSGhepKho";
		this.gv_DSGhepKho.OptionsView.ColumnAutoWidth = false;
		this.gv_DSGhepKho.OptionsView.ShowGroupPanel = false;
		this.gv_DSGhepKho.UseBatchMod = false;
		this.gv_DSGhepKho.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_DSGhepKho_CustomDrawRowIndicator);
		this.clgk_ID.Caption = "ID";
		this.clgk_ID.CurrencyDateField = null;
		this.clgk_ID.CurrencyField = null;
		this.clgk_ID.CurrencyFixed = null;
		this.clgk_ID.CurrencyFixedRate = 0.0;
		this.clgk_ID.CurrencyRateField = null;
		this.clgk_ID.FieldName = "ID";
		this.clgk_ID.Name = "clgk_ID";
		this.clgk_ID.PrimaryKey = true;
		this.clgk_ID.UnitField = null;
		this.clgk_ItemCode.Caption = "Mã hàng";
		this.clgk_ItemCode.CurrencyDateField = null;
		this.clgk_ItemCode.CurrencyField = null;
		this.clgk_ItemCode.CurrencyFixed = null;
		this.clgk_ItemCode.CurrencyFixedRate = 0.0;
		this.clgk_ItemCode.CurrencyRateField = null;
		this.clgk_ItemCode.FieldName = "ItemCode";
		this.clgk_ItemCode.Name = "clgk_ItemCode";
		this.clgk_ItemCode.PrimaryKey = false;
		this.clgk_ItemCode.UnitField = null;
		this.clgk_ItemCode.Visible = true;
		this.clgk_ItemCode.VisibleIndex = 0;
		this.clgk_ItemCode.Width = 120;
		this.clgk_ItemName.Caption = "Item Name";
		this.clgk_ItemName.CurrencyDateField = null;
		this.clgk_ItemName.CurrencyField = null;
		this.clgk_ItemName.CurrencyFixed = null;
		this.clgk_ItemName.CurrencyFixedRate = 0.0;
		this.clgk_ItemName.CurrencyRateField = null;
		this.clgk_ItemName.FieldName = "ItemName";
		this.clgk_ItemName.Name = "clgk_ItemName";
		this.clgk_ItemName.PrimaryKey = false;
		this.clgk_ItemName.UnitField = null;
		this.clgk_ItemName.Width = 150;
		this.clgk_Batch.Caption = "Batch No";
		this.clgk_Batch.CurrencyDateField = null;
		this.clgk_Batch.CurrencyField = null;
		this.clgk_Batch.CurrencyFixed = null;
		this.clgk_Batch.CurrencyFixedRate = 0.0;
		this.clgk_Batch.CurrencyRateField = null;
		this.clgk_Batch.FieldName = "Batch";
		this.clgk_Batch.Name = "clgk_Batch";
		this.clgk_Batch.PrimaryKey = false;
		this.clgk_Batch.UnitField = null;
		this.clgk_Batch.Visible = true;
		this.clgk_Batch.VisibleIndex = 1;
		this.clgk_Batch.Width = 111;
		this.clgk_TotalQty.Caption = "Total m2";
		this.clgk_TotalQty.CurrencyDateField = null;
		this.clgk_TotalQty.CurrencyField = null;
		this.clgk_TotalQty.CurrencyFixed = null;
		this.clgk_TotalQty.CurrencyFixedRate = 0.0;
		this.clgk_TotalQty.CurrencyRateField = null;
		this.clgk_TotalQty.FieldName = "Qty";
		this.clgk_TotalQty.Name = "clgk_TotalQty";
		this.clgk_TotalQty.PrimaryKey = false;
		this.clgk_TotalQty.UnitField = null;
		this.clgk_TotalQty.Visible = true;
		this.clgk_TotalQty.VisibleIndex = 6;
		this.clgk_TotalQty.Width = 80;
		this.clgk_TotalLenght.Caption = "Total dài(m)";
		this.clgk_TotalLenght.CurrencyDateField = null;
		this.clgk_TotalLenght.CurrencyField = null;
		this.clgk_TotalLenght.CurrencyFixed = null;
		this.clgk_TotalLenght.CurrencyFixedRate = 0.0;
		this.clgk_TotalLenght.CurrencyRateField = null;
		this.clgk_TotalLenght.FieldName = "Length";
		this.clgk_TotalLenght.Name = "clgk_TotalLenght";
		this.clgk_TotalLenght.PrimaryKey = false;
		this.clgk_TotalLenght.UnitField = null;
		this.clgk_TotalLenght.Visible = true;
		this.clgk_TotalLenght.VisibleIndex = 3;
		this.clgk_TotalWidth.Caption = "Total khổ (cm)";
		this.clgk_TotalWidth.CurrencyDateField = null;
		this.clgk_TotalWidth.CurrencyField = null;
		this.clgk_TotalWidth.CurrencyFixed = null;
		this.clgk_TotalWidth.CurrencyFixedRate = 0.0;
		this.clgk_TotalWidth.CurrencyRateField = null;
		this.clgk_TotalWidth.FieldName = "Width";
		this.clgk_TotalWidth.Name = "clgk_TotalWidth";
		this.clgk_TotalWidth.PrimaryKey = false;
		this.clgk_TotalWidth.UnitField = null;
		this.clgk_TotalWidth.Visible = true;
		this.clgk_TotalWidth.VisibleIndex = 2;
		this.clgk_TotalWidth.Width = 100;
		this.clgk_QtyPcs.Caption = "QtyPcs";
		this.clgk_QtyPcs.CurrencyDateField = null;
		this.clgk_QtyPcs.CurrencyField = null;
		this.clgk_QtyPcs.CurrencyFixed = null;
		this.clgk_QtyPcs.CurrencyFixedRate = 0.0;
		this.clgk_QtyPcs.CurrencyRateField = null;
		this.clgk_QtyPcs.FieldName = "QtyPcs";
		this.clgk_QtyPcs.Name = "clgk_QtyPcs";
		this.clgk_QtyPcs.PrimaryKey = false;
		this.clgk_QtyPcs.UnitField = null;
		this.clgk_QtyPcs.Visible = true;
		this.clgk_QtyPcs.VisibleIndex = 4;
		this.clgk_Uom.Caption = "Uom";
		this.clgk_Uom.CurrencyDateField = null;
		this.clgk_Uom.CurrencyField = null;
		this.clgk_Uom.CurrencyFixed = null;
		this.clgk_Uom.CurrencyFixedRate = 0.0;
		this.clgk_Uom.CurrencyRateField = null;
		this.clgk_Uom.FieldName = "Uom";
		this.clgk_Uom.Name = "clgk_Uom";
		this.clgk_Uom.PrimaryKey = false;
		this.clgk_Uom.UnitField = null;
		this.clgk_Uom.Visible = true;
		this.clgk_Uom.VisibleIndex = 5;
		this.clgk_H1.Caption = "Total H1";
		this.clgk_H1.CurrencyDateField = null;
		this.clgk_H1.CurrencyField = null;
		this.clgk_H1.CurrencyFixed = null;
		this.clgk_H1.CurrencyFixedRate = 0.0;
		this.clgk_H1.CurrencyRateField = null;
		this.clgk_H1.DisplayFormat.FormatString = "n0";
		this.clgk_H1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.clgk_H1.FieldName = "H1";
		this.clgk_H1.Name = "clgk_H1";
		this.clgk_H1.PrimaryKey = false;
		this.clgk_H1.UnitField = null;
		this.clgk_H1.Visible = true;
		this.clgk_H1.VisibleIndex = 7;
		this.clgk_H1.Width = 80;
		this.clgk_H2.Caption = "Total H2";
		this.clgk_H2.CurrencyDateField = null;
		this.clgk_H2.CurrencyField = null;
		this.clgk_H2.CurrencyFixed = null;
		this.clgk_H2.CurrencyFixedRate = 0.0;
		this.clgk_H2.CurrencyRateField = null;
		this.clgk_H2.DisplayFormat.FormatString = "n0";
		this.clgk_H2.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.clgk_H2.FieldName = "H2";
		this.clgk_H2.Name = "clgk_H2";
		this.clgk_H2.PrimaryKey = false;
		this.clgk_H2.UnitField = null;
		this.clgk_H2.Visible = true;
		this.clgk_H2.VisibleIndex = 8;
		this.clgk_H2.Width = 80;
		this.clgk_Joins.Caption = "Join";
		this.clgk_Joins.CurrencyDateField = null;
		this.clgk_Joins.CurrencyField = null;
		this.clgk_Joins.CurrencyFixed = null;
		this.clgk_Joins.CurrencyFixedRate = 0.0;
		this.clgk_Joins.CurrencyRateField = null;
		this.clgk_Joins.DisplayFormat.FormatString = "n0";
		this.clgk_Joins.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.clgk_Joins.FieldName = "Joins";
		this.clgk_Joins.Name = "clgk_Joins";
		this.clgk_Joins.PrimaryKey = false;
		this.clgk_Joins.UnitField = null;
		this.clgk_Joins.Visible = true;
		this.clgk_Joins.VisibleIndex = 9;
		this.clgk_Joins.Width = 80;
		this.clgk_ChieuDaiTinhGiay.Caption = "C. dài tính giấy";
		this.clgk_ChieuDaiTinhGiay.ColumnEdit = this.rep_Spin_N2;
		this.clgk_ChieuDaiTinhGiay.CurrencyDateField = null;
		this.clgk_ChieuDaiTinhGiay.CurrencyField = null;
		this.clgk_ChieuDaiTinhGiay.CurrencyFixed = null;
		this.clgk_ChieuDaiTinhGiay.CurrencyFixedRate = 0.0;
		this.clgk_ChieuDaiTinhGiay.CurrencyRateField = null;
		this.clgk_ChieuDaiTinhGiay.DisplayFormat.FormatString = "n2";
		this.clgk_ChieuDaiTinhGiay.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.clgk_ChieuDaiTinhGiay.FieldName = "ChieuDaiTinhGiay";
		this.clgk_ChieuDaiTinhGiay.Name = "clgk_ChieuDaiTinhGiay";
		this.clgk_ChieuDaiTinhGiay.PrimaryKey = false;
		this.clgk_ChieuDaiTinhGiay.UnitField = null;
		this.clgk_ChieuDaiTinhGiay.Visible = true;
		this.clgk_ChieuDaiTinhGiay.VisibleIndex = 10;
		this.clgk_ChieuDaiTinhGiay.Width = 80;
		this.rep_Spin_N2.AutoHeight = false;
		this.rep_Spin_N2.DisplayFormat.FormatString = "n2";
		this.rep_Spin_N2.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.rep_Spin_N2.EditFormat.FormatString = "n2";
		this.rep_Spin_N2.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.rep_Spin_N2.Mask.EditMask = "n2";
		this.rep_Spin_N2.Name = "rep_Spin_N2";
		this.clgk_XenToiDa.Caption = "Xén tối đa";
		this.clgk_XenToiDa.ColumnEdit = this.repo_Check;
		this.clgk_XenToiDa.CurrencyDateField = null;
		this.clgk_XenToiDa.CurrencyField = null;
		this.clgk_XenToiDa.CurrencyFixed = null;
		this.clgk_XenToiDa.CurrencyFixedRate = 0.0;
		this.clgk_XenToiDa.CurrencyRateField = null;
		this.clgk_XenToiDa.FieldName = "XenToiDa";
		this.clgk_XenToiDa.Name = "clgk_XenToiDa";
		this.clgk_XenToiDa.PrimaryKey = false;
		this.clgk_XenToiDa.UnitField = null;
		this.clgk_XenToiDa.Visible = true;
		this.clgk_XenToiDa.VisibleIndex = 11;
		this.clgk_XenToiDa.Width = 80;
		this.repo_Check.AutoHeight = false;
		this.repo_Check.Name = "repo_Check";
		this.clgk_HHToiThieu.Caption = "HH Tối thiểu";
		this.clgk_HHToiThieu.ColumnEdit = this.repo_Check;
		this.clgk_HHToiThieu.CurrencyDateField = null;
		this.clgk_HHToiThieu.CurrencyField = null;
		this.clgk_HHToiThieu.CurrencyFixed = null;
		this.clgk_HHToiThieu.CurrencyFixedRate = 0.0;
		this.clgk_HHToiThieu.CurrencyRateField = null;
		this.clgk_HHToiThieu.FieldName = "HHToiThieu";
		this.clgk_HHToiThieu.Name = "clgk_HHToiThieu";
		this.clgk_HHToiThieu.PrimaryKey = false;
		this.clgk_HHToiThieu.UnitField = null;
		this.clgk_HHToiThieu.Visible = true;
		this.clgk_HHToiThieu.VisibleIndex = 12;
		this.clgk_HHToiThieu.Width = 80;
		this.clgk_Check.ColumnEdit = this.repo_Check;
		this.clgk_Check.CurrencyDateField = null;
		this.clgk_Check.CurrencyField = null;
		this.clgk_Check.CurrencyFixed = null;
		this.clgk_Check.CurrencyFixedRate = 0.0;
		this.clgk_Check.CurrencyRateField = null;
		this.clgk_Check.FieldName = "Check";
		this.clgk_Check.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Right;
		this.clgk_Check.Name = "clgk_Check";
		this.clgk_Check.PrimaryKey = false;
		this.clgk_Check.UnitField = null;
		this.clgk_Check.Visible = true;
		this.clgk_Check.VisibleIndex = 13;
		this.clgk_Check.Width = 35;
		this.groupControl6.AppearanceCaption.Font = new System.Drawing.Font("Tahoma", 7.8f, System.Drawing.FontStyle.Bold);
		this.groupControl6.AppearanceCaption.Options.UseFont = true;
		this.groupControl6.AppearanceCaption.Options.UseTextOptions = true;
		this.groupControl6.AppearanceCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.groupControl6.Controls.Add(this.grd_DSDaChon);
		this.groupControl6.Dock = System.Windows.Forms.DockStyle.Fill;
		this.groupControl6.Location = new System.Drawing.Point(0, 0);
		this.groupControl6.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.groupControl6.Name = "groupControl6";
		this.groupControl6.Size = new System.Drawing.Size(282, 323);
		this.groupControl6.TabIndex = 1;
		this.groupControl6.Text = "Danh sách cuộn đã chọn";
		this.grd_DSDaChon.AllowPasteExcel = true;
		this.grd_DSDaChon.ClipboardData = null;
		this.grd_DSDaChon.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_DSDaChon.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grd_DSDaChon.Location = new System.Drawing.Point(2, 23);
		this.grd_DSDaChon.MainView = this.gv_DSDaChon;
		this.grd_DSDaChon.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grd_DSDaChon.Name = "grd_DSDaChon";
		this.grd_DSDaChon.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.repositoryItemCheckEdit1, this.repositoryItemCalcEdit3 });
		this.grd_DSDaChon.Size = new System.Drawing.Size(278, 298);
		this.grd_DSDaChon.TabIndex = 0;
		this.grd_DSDaChon.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_DSDaChon });
		this.gv_DSDaChon.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_DSDaChon.ColumnKey = null;
		this.gv_DSDaChon.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[17]
		{
			this.gridColumn1, this.gridColumn2, this.gridColumn3, this.gridColumn4, this.gridColumn5, this.gridColumn6, this.gridColumn7, this.gridColumn8, this.gridColumn9, this.gridColumn11,
			this.gridColumn12, this.gridColumn13, this.gridColumn14, this.gridColumn15, this.gridColumn16, this.gridColumn17, this.gridColumn18
		});
		this.gv_DSDaChon.DisableFormSetting = false;
		this.gv_DSDaChon.DisplayException = false;
		this.gv_DSDaChon.EditColumn = null;
		this.gv_DSDaChon.FieldCurrency = null;
		this.gv_DSDaChon.FieldRate = null;
		this.gv_DSDaChon.FormId = null;
		this.gv_DSDaChon.GridControl = this.grd_DSDaChon;
		this.gv_DSDaChon.IgnoreFilter = false;
		this.gv_DSDaChon.IndexEditRow = -1;
		this.gv_DSDaChon.IndicatorWidth = 35;
		this.gv_DSDaChon.IsApzTable = false;
		this.gv_DSDaChon.IsInsertNewRow = false;
		this.gv_DSDaChon.IsLoadingData = false;
		this.gv_DSDaChon.IsMappingContextAction = false;
		this.gv_DSDaChon.ItemId = null;
		this.gv_DSDaChon.LstColumnAllowBatch = null;
		this.gv_DSDaChon.LstColumnNotAllowBatch = null;
		this.gv_DSDaChon.LstColumnNotEditable = null;
		this.gv_DSDaChon.LstColumNotSetting = null;
		this.gv_DSDaChon.Name = "gv_DSDaChon";
		this.gv_DSDaChon.OptionsView.ColumnAutoWidth = false;
		this.gv_DSDaChon.OptionsView.ShowGroupPanel = false;
		this.gv_DSDaChon.UseBatchMod = false;
		this.gv_DSDaChon.DoubleClick += new System.EventHandler(this.gv_DSDaChon_DoubleClick);
		this.gridColumn1.Caption = "ID";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "ID";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.PrimaryKey = true;
		this.gridColumn1.UnitField = null;
		this.gridColumn2.Caption = "Mã hàng";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "ItemCode";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 0;
		this.gridColumn2.Width = 80;
		this.gridColumn3.Caption = "Tên hàng";
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "ItemName";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Width = 150;
		this.gridColumn4.Caption = "Mã cuộn";
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "Batch";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 1;
		this.gridColumn4.Width = 80;
		this.gridColumn5.Caption = "Khổ (cm)";
		this.gridColumn5.ColumnEdit = this.repositoryItemCalcEdit3;
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.DisplayFormat.FormatString = "n2";
		this.gridColumn5.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn5.FieldName = "Width";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.UnitField = null;
		this.gridColumn5.Visible = true;
		this.gridColumn5.VisibleIndex = 2;
		this.gridColumn5.Width = 60;
		this.repositoryItemCalcEdit3.AutoHeight = false;
		this.repositoryItemCalcEdit3.DisplayFormat.FormatString = "n2";
		this.repositoryItemCalcEdit3.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.repositoryItemCalcEdit3.EditFormat.FormatString = "n2";
		this.repositoryItemCalcEdit3.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.repositoryItemCalcEdit3.Mask.EditMask = "n2";
		this.repositoryItemCalcEdit3.Name = "repositoryItemCalcEdit3";
		this.gridColumn6.Caption = "Dài (m)";
		this.gridColumn6.ColumnEdit = this.repositoryItemCalcEdit3;
		this.gridColumn6.CurrencyDateField = null;
		this.gridColumn6.CurrencyField = null;
		this.gridColumn6.CurrencyFixed = null;
		this.gridColumn6.CurrencyFixedRate = 0.0;
		this.gridColumn6.CurrencyRateField = null;
		this.gridColumn6.DisplayFormat.FormatString = "n2";
		this.gridColumn6.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn6.FieldName = "Length";
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.PrimaryKey = false;
		this.gridColumn6.UnitField = null;
		this.gridColumn6.Visible = true;
		this.gridColumn6.VisibleIndex = 3;
		this.gridColumn6.Width = 60;
		this.gridColumn7.Caption = "SL cần";
		this.gridColumn7.ColumnEdit = this.repositoryItemCalcEdit3;
		this.gridColumn7.CurrencyDateField = null;
		this.gridColumn7.CurrencyField = null;
		this.gridColumn7.CurrencyFixed = null;
		this.gridColumn7.CurrencyFixedRate = 0.0;
		this.gridColumn7.CurrencyRateField = null;
		this.gridColumn7.DisplayFormat.FormatString = "n2";
		this.gridColumn7.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn7.FieldName = "QtyReq";
		this.gridColumn7.Name = "gridColumn7";
		this.gridColumn7.PrimaryKey = false;
		this.gridColumn7.UnitField = null;
		this.gridColumn7.Visible = true;
		this.gridColumn7.VisibleIndex = 4;
		this.gridColumn7.Width = 80;
		this.gridColumn8.Caption = "SL tồn";
		this.gridColumn8.ColumnEdit = this.repositoryItemCalcEdit3;
		this.gridColumn8.CurrencyDateField = null;
		this.gridColumn8.CurrencyField = null;
		this.gridColumn8.CurrencyFixed = null;
		this.gridColumn8.CurrencyFixedRate = 0.0;
		this.gridColumn8.CurrencyRateField = null;
		this.gridColumn8.DisplayFormat.FormatString = "n2";
		this.gridColumn8.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn8.FieldName = "QtyInv";
		this.gridColumn8.Name = "gridColumn8";
		this.gridColumn8.PrimaryKey = false;
		this.gridColumn8.UnitField = null;
		this.gridColumn8.Visible = true;
		this.gridColumn8.VisibleIndex = 5;
		this.gridColumn8.Width = 80;
		this.gridColumn9.Caption = "Mối nối";
		this.gridColumn9.ColumnEdit = this.repositoryItemCalcEdit3;
		this.gridColumn9.CurrencyDateField = null;
		this.gridColumn9.CurrencyField = null;
		this.gridColumn9.CurrencyFixed = null;
		this.gridColumn9.CurrencyFixedRate = 0.0;
		this.gridColumn9.CurrencyRateField = null;
		this.gridColumn9.FieldName = "Joins";
		this.gridColumn9.Name = "gridColumn9";
		this.gridColumn9.PrimaryKey = false;
		this.gridColumn9.UnitField = null;
		this.gridColumn9.Visible = true;
		this.gridColumn9.VisibleIndex = 6;
		this.gridColumn9.Width = 50;
		this.gridColumn11.Caption = "Kho";
		this.gridColumn11.CurrencyDateField = null;
		this.gridColumn11.CurrencyField = null;
		this.gridColumn11.CurrencyFixed = null;
		this.gridColumn11.CurrencyFixedRate = 0.0;
		this.gridColumn11.CurrencyRateField = null;
		this.gridColumn11.FieldName = "Whse";
		this.gridColumn11.Name = "gridColumn11";
		this.gridColumn11.PrimaryKey = false;
		this.gridColumn11.UnitField = null;
		this.gridColumn11.Visible = true;
		this.gridColumn11.VisibleIndex = 7;
		this.gridColumn11.Width = 80;
		this.gridColumn12.Caption = "Dạng tồn";
		this.gridColumn12.CurrencyDateField = null;
		this.gridColumn12.CurrencyField = null;
		this.gridColumn12.CurrencyFixed = null;
		this.gridColumn12.CurrencyFixedRate = 0.0;
		this.gridColumn12.CurrencyRateField = null;
		this.gridColumn12.FieldName = "DangTon";
		this.gridColumn12.Name = "gridColumn12";
		this.gridColumn12.PrimaryKey = false;
		this.gridColumn12.UnitField = null;
		this.gridColumn12.Visible = true;
		this.gridColumn12.VisibleIndex = 8;
		this.gridColumn12.Width = 110;
		this.gridColumn13.Caption = "Khổ thu hồi";
		this.gridColumn13.ColumnEdit = this.repositoryItemCalcEdit3;
		this.gridColumn13.CurrencyDateField = null;
		this.gridColumn13.CurrencyField = null;
		this.gridColumn13.CurrencyFixed = null;
		this.gridColumn13.CurrencyFixedRate = 0.0;
		this.gridColumn13.CurrencyRateField = null;
		this.gridColumn13.DisplayFormat.FormatString = "n2";
		this.gridColumn13.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn13.FieldName = "KhoThuHoi";
		this.gridColumn13.Name = "gridColumn13";
		this.gridColumn13.PrimaryKey = false;
		this.gridColumn13.UnitField = null;
		this.gridColumn13.Visible = true;
		this.gridColumn13.VisibleIndex = 9;
		this.gridColumn13.Width = 110;
		this.gridColumn14.Caption = "Dài thu hồi";
		this.gridColumn14.ColumnEdit = this.repositoryItemCalcEdit3;
		this.gridColumn14.CurrencyDateField = null;
		this.gridColumn14.CurrencyField = null;
		this.gridColumn14.CurrencyFixed = null;
		this.gridColumn14.CurrencyFixedRate = 0.0;
		this.gridColumn14.CurrencyRateField = null;
		this.gridColumn14.DisplayFormat.FormatString = "n2";
		this.gridColumn14.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn14.FieldName = "DaiThuHoi";
		this.gridColumn14.Name = "gridColumn14";
		this.gridColumn14.PrimaryKey = false;
		this.gridColumn14.UnitField = null;
		this.gridColumn14.Visible = true;
		this.gridColumn14.VisibleIndex = 10;
		this.gridColumn14.Width = 110;
		this.gridColumn15.Caption = "Lẻ khổ";
		this.gridColumn15.ColumnEdit = this.repositoryItemCalcEdit3;
		this.gridColumn15.CurrencyDateField = null;
		this.gridColumn15.CurrencyField = null;
		this.gridColumn15.CurrencyFixed = null;
		this.gridColumn15.CurrencyFixedRate = 0.0;
		this.gridColumn15.CurrencyRateField = null;
		this.gridColumn15.DisplayFormat.FormatString = "n2";
		this.gridColumn15.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn15.FieldName = "LeKho";
		this.gridColumn15.Name = "gridColumn15";
		this.gridColumn15.PrimaryKey = false;
		this.gridColumn15.UnitField = null;
		this.gridColumn15.Visible = true;
		this.gridColumn15.VisibleIndex = 11;
		this.gridColumn15.Width = 100;
		this.gridColumn16.Caption = "H2";
		this.gridColumn16.ColumnEdit = this.repositoryItemCheckEdit1;
		this.gridColumn16.CurrencyDateField = null;
		this.gridColumn16.CurrencyField = null;
		this.gridColumn16.CurrencyFixed = null;
		this.gridColumn16.CurrencyFixedRate = 0.0;
		this.gridColumn16.CurrencyRateField = null;
		this.gridColumn16.FieldName = "H2";
		this.gridColumn16.Name = "gridColumn16";
		this.gridColumn16.PrimaryKey = false;
		this.gridColumn16.UnitField = null;
		this.gridColumn16.Visible = true;
		this.gridColumn16.VisibleIndex = 12;
		this.gridColumn16.Width = 50;
		this.repositoryItemCheckEdit1.AutoHeight = false;
		this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
		this.gridColumn17.Caption = "SL T. Tính";
		this.gridColumn17.ColumnEdit = this.repositoryItemCalcEdit3;
		this.gridColumn17.CurrencyDateField = null;
		this.gridColumn17.CurrencyField = null;
		this.gridColumn17.CurrencyFixed = null;
		this.gridColumn17.CurrencyFixedRate = 0.0;
		this.gridColumn17.CurrencyRateField = null;
		this.gridColumn17.DisplayFormat.FormatString = "n2";
		this.gridColumn17.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn17.FieldName = "QtyTTinh";
		this.gridColumn17.Name = "gridColumn17";
		this.gridColumn17.PrimaryKey = false;
		this.gridColumn17.UnitField = null;
		this.gridColumn17.Visible = true;
		this.gridColumn17.VisibleIndex = 13;
		this.gridColumn17.Width = 110;
		this.gridColumn18.Caption = " Check";
		this.gridColumn18.ColumnEdit = this.repositoryItemCheckEdit1;
		this.gridColumn18.CurrencyDateField = null;
		this.gridColumn18.CurrencyField = null;
		this.gridColumn18.CurrencyFixed = null;
		this.gridColumn18.CurrencyFixedRate = 0.0;
		this.gridColumn18.CurrencyRateField = null;
		this.gridColumn18.FieldName = "Checked";
		this.gridColumn18.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Right;
		this.gridColumn18.Name = "gridColumn18";
		this.gridColumn18.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn18.OptionsColumn.FixedWidth = true;
		this.gridColumn18.PrimaryKey = false;
		this.gridColumn18.UnitField = null;
		this.gridColumn18.Visible = true;
		this.gridColumn18.VisibleIndex = 14;
		this.gridColumn18.Width = 35;
		this.groupControl3.AppearanceCaption.Font = new System.Drawing.Font("Tahoma", 7.8f, System.Drawing.FontStyle.Bold);
		this.groupControl3.AppearanceCaption.Options.UseFont = true;
		this.groupControl3.AppearanceCaption.Options.UseTextOptions = true;
		this.groupControl3.AppearanceCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.groupControl3.Controls.Add(this.splitContainerControl3);
		this.groupControl3.Dock = System.Windows.Forms.DockStyle.Fill;
		this.groupControl3.Location = new System.Drawing.Point(0, 0);
		this.groupControl3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.groupControl3.Name = "groupControl3";
		this.groupControl3.Size = new System.Drawing.Size(897, 191);
		this.groupControl3.TabIndex = 0;
		this.groupControl3.Text = "Danh sách chọn hàng trong kho";
		this.splitContainerControl3.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl3.Location = new System.Drawing.Point(2, 23);
		this.splitContainerControl3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.splitContainerControl3.Name = "splitContainerControl3";
		this.splitContainerControl3.Panel1.Controls.Add(this.groupControl5);
		this.splitContainerControl3.Panel1.Text = "Panel1";
		this.splitContainerControl3.Panel2.Controls.Add(this.groupControl4);
		this.splitContainerControl3.Panel2.Text = "Panel2";
		this.splitContainerControl3.Size = new System.Drawing.Size(893, 166);
		this.splitContainerControl3.SplitterPosition = 260;
		this.splitContainerControl3.TabIndex = 0;
		this.splitContainerControl3.Text = "splitContainerControl3";
		this.groupControl5.AppearanceCaption.Font = new System.Drawing.Font("Tahoma", 7.8f, System.Drawing.FontStyle.Bold);
		this.groupControl5.AppearanceCaption.Options.UseFont = true;
		this.groupControl5.AppearanceCaption.Options.UseTextOptions = true;
		this.groupControl5.AppearanceCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.groupControl5.Controls.Add(this.btn_XemTonKho);
		this.groupControl5.Controls.Add(this.btn_ChayTinhGiay);
		this.groupControl5.Controls.Add(this.grd_Kho);
		this.groupControl5.Dock = System.Windows.Forms.DockStyle.Fill;
		this.groupControl5.Location = new System.Drawing.Point(0, 0);
		this.groupControl5.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.groupControl5.Name = "groupControl5";
		this.groupControl5.Size = new System.Drawing.Size(260, 166);
		this.groupControl5.TabIndex = 1;
		this.groupControl5.Text = "Chọn kho";
		this.btn_XemTonKho.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_XemTonKho.EditControl = null;
		this.btn_XemTonKho.EditValue = "Xem tồn kho";
		this.btn_XemTonKho.FieldName = null;
		this.btn_XemTonKho.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_XemTonKho.ImageOptions.Image");
		this.btn_XemTonKho.IsSearchControl = false;
		this.btn_XemTonKho.Location = new System.Drawing.Point(167, 143);
		this.btn_XemTonKho.Name = "btn_XemTonKho";
		this.btn_XemTonKho.ReadOnly = false;
		this.btn_XemTonKho.SettingInVisible = false;
		this.btn_XemTonKho.SettingReadonly = false;
		this.btn_XemTonKho.Size = new System.Drawing.Size(88, 23);
		this.btn_XemTonKho.TabIndex = 3;
		this.btn_XemTonKho.Text = "Xem tồn kho";
		this.btn_XemTonKho.Click += new System.EventHandler(this.btn_XemTonKho_Click);
		this.btn_ChayTinhGiay.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_ChayTinhGiay.EditControl = null;
		this.btn_ChayTinhGiay.EditValue = "Chạy Tính Giấy";
		this.btn_ChayTinhGiay.FieldName = null;
		this.btn_ChayTinhGiay.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_ChayTinhGiay.ImageOptions.Image");
		this.btn_ChayTinhGiay.IsSearchControl = false;
		this.btn_ChayTinhGiay.Location = new System.Drawing.Point(9, 145);
		this.btn_ChayTinhGiay.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.btn_ChayTinhGiay.Name = "btn_ChayTinhGiay";
		this.btn_ChayTinhGiay.ReadOnly = false;
		this.btn_ChayTinhGiay.SettingInVisible = false;
		this.btn_ChayTinhGiay.SettingReadonly = false;
		this.btn_ChayTinhGiay.Size = new System.Drawing.Size(117, 19);
		this.btn_ChayTinhGiay.TabIndex = 2;
		this.btn_ChayTinhGiay.Text = "Chạy Tính Giấy";
		this.btn_ChayTinhGiay.Click += new System.EventHandler(this.btn_ChayTinhGiay_Click);
		this.grd_Kho.AllowPasteExcel = false;
		this.grd_Kho.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Kho.ClipboardData = null;
		this.grd_Kho.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grd_Kho.Location = new System.Drawing.Point(-1, 20);
		this.grd_Kho.MainView = this.gv_Kho;
		this.grd_Kho.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grd_Kho.Name = "grd_Kho";
		this.grd_Kho.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.repo_LookUp_Status });
		this.grd_Kho.Size = new System.Drawing.Size(256, 121);
		this.grd_Kho.TabIndex = 0;
		this.grd_Kho.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Kho });
		this.gv_Kho.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Kho.ColumnKey = null;
		this.gv_Kho.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.clkh_WhsCode, this.clkh_WhsName, this.clkh_Status });
		this.gv_Kho.DisableFormSetting = false;
		this.gv_Kho.DisplayException = false;
		this.gv_Kho.EditColumn = null;
		this.gv_Kho.FieldCurrency = null;
		this.gv_Kho.FieldRate = null;
		this.gv_Kho.FormId = null;
		this.gv_Kho.GridControl = this.grd_Kho;
		this.gv_Kho.IgnoreFilter = false;
		this.gv_Kho.IndexEditRow = -1;
		this.gv_Kho.IndicatorWidth = 40;
		this.gv_Kho.IsApzTable = false;
		this.gv_Kho.IsInsertNewRow = false;
		this.gv_Kho.IsLoadingData = false;
		this.gv_Kho.IsMappingContextAction = false;
		this.gv_Kho.ItemId = null;
		this.gv_Kho.LstColumnAllowBatch = null;
		this.gv_Kho.LstColumnNotAllowBatch = null;
		this.gv_Kho.LstColumnNotEditable = null;
		this.gv_Kho.LstColumNotSetting = null;
		this.gv_Kho.Name = "gv_Kho";
		this.gv_Kho.OptionsSelection.CheckBoxSelectorColumnWidth = 35;
		this.gv_Kho.OptionsSelection.MultiSelect = true;
		this.gv_Kho.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_Kho.OptionsSelection.ResetSelectionClickOutsideCheckboxSelector = true;
		this.gv_Kho.OptionsSelection.ShowCheckBoxSelectorInPrintExport = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Kho.OptionsView.ColumnAutoWidth = false;
		this.gv_Kho.OptionsView.ShowGroupPanel = false;
		this.gv_Kho.UseBatchMod = false;
		this.gv_Kho.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Kho_CustomDrawRowIndicator);
		this.gv_Kho.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gv_Kho_RowStyle);
		this.clkh_WhsCode.Caption = "Whse Code";
		this.clkh_WhsCode.CurrencyDateField = null;
		this.clkh_WhsCode.CurrencyField = null;
		this.clkh_WhsCode.CurrencyFixed = null;
		this.clkh_WhsCode.CurrencyFixedRate = 0.0;
		this.clkh_WhsCode.CurrencyRateField = null;
		this.clkh_WhsCode.FieldName = "WhsCode";
		this.clkh_WhsCode.Name = "clkh_WhsCode";
		this.clkh_WhsCode.PrimaryKey = true;
		this.clkh_WhsCode.UnitField = null;
		this.clkh_WhsCode.Visible = true;
		this.clkh_WhsCode.VisibleIndex = 1;
		this.clkh_WhsCode.Width = 70;
		this.clkh_WhsName.Caption = "Whse Name";
		this.clkh_WhsName.CurrencyDateField = null;
		this.clkh_WhsName.CurrencyField = null;
		this.clkh_WhsName.CurrencyFixed = null;
		this.clkh_WhsName.CurrencyFixedRate = 0.0;
		this.clkh_WhsName.CurrencyRateField = null;
		this.clkh_WhsName.FieldName = "WhsName";
		this.clkh_WhsName.Name = "clkh_WhsName";
		this.clkh_WhsName.PrimaryKey = false;
		this.clkh_WhsName.UnitField = null;
		this.clkh_WhsName.Visible = true;
		this.clkh_WhsName.VisibleIndex = 2;
		this.clkh_WhsName.Width = 150;
		this.clkh_Status.Caption = "Status";
		this.clkh_Status.ColumnEdit = this.repo_LookUp_Status;
		this.clkh_Status.CurrencyDateField = null;
		this.clkh_Status.CurrencyField = null;
		this.clkh_Status.CurrencyFixed = null;
		this.clkh_Status.CurrencyFixedRate = 0.0;
		this.clkh_Status.CurrencyRateField = null;
		this.clkh_Status.FieldName = "U_Status";
		this.clkh_Status.Name = "clkh_Status";
		this.clkh_Status.PrimaryKey = false;
		this.clkh_Status.UnitField = null;
		this.clkh_Status.Visible = true;
		this.clkh_Status.VisibleIndex = 3;
		this.repo_LookUp_Status.AutoHeight = false;
		this.repo_LookUp_Status.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repo_LookUp_Status.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "ID"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.repo_LookUp_Status.Name = "repo_LookUp_Status";
		this.groupControl4.AppearanceCaption.Font = new System.Drawing.Font("Tahoma", 7.8f, System.Drawing.FontStyle.Bold);
		this.groupControl4.AppearanceCaption.Options.UseFont = true;
		this.groupControl4.AppearanceCaption.Options.UseTextOptions = true;
		this.groupControl4.AppearanceCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.groupControl4.Controls.Add(this.btn_ChonGiay);
		this.groupControl4.Controls.Add(this.grd_DSCuon);
		this.groupControl4.Dock = System.Windows.Forms.DockStyle.Fill;
		this.groupControl4.Location = new System.Drawing.Point(0, 0);
		this.groupControl4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.groupControl4.Name = "groupControl4";
		this.groupControl4.Size = new System.Drawing.Size(623, 166);
		this.groupControl4.TabIndex = 0;
		this.groupControl4.Text = "Danh sách cuộn phù hợp";
		this.btn_ChonGiay.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_ChonGiay.EditControl = null;
		this.btn_ChonGiay.EditValue = "Chọn Giấy";
		this.btn_ChonGiay.FieldName = null;
		this.btn_ChonGiay.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_ChonGiay.ImageOptions.Image");
		this.btn_ChonGiay.IsSearchControl = false;
		this.btn_ChonGiay.Location = new System.Drawing.Point(532, 145);
		this.btn_ChonGiay.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.btn_ChonGiay.Name = "btn_ChonGiay";
		this.btn_ChonGiay.ReadOnly = false;
		this.btn_ChonGiay.SettingInVisible = false;
		this.btn_ChonGiay.SettingReadonly = false;
		this.btn_ChonGiay.Size = new System.Drawing.Size(91, 19);
		this.btn_ChonGiay.TabIndex = 1;
		this.btn_ChonGiay.Text = "Chọn Giấy";
		this.btn_ChonGiay.Click += new System.EventHandler(this.btn_ChonGiay_Click);
		this.grd_DSCuon.AllowPasteExcel = false;
		this.grd_DSCuon.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_DSCuon.ClipboardData = null;
		this.grd_DSCuon.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grd_DSCuon.Location = new System.Drawing.Point(1, 20);
		this.grd_DSCuon.MainView = this.gv_DSCuon;
		this.grd_DSCuon.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grd_DSCuon.Name = "grd_DSCuon";
		this.grd_DSCuon.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.repositoryItemCheckEdit3, this.repositoryItemCalcEdit1 });
		this.grd_DSCuon.Size = new System.Drawing.Size(625, 121);
		this.grd_DSCuon.TabIndex = 0;
		this.grd_DSCuon.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_DSCuon });
		this.gv_DSCuon.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_DSCuon.ColumnKey = null;
		this.gv_DSCuon.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[17]
		{
			this.clcg_ID, this.clcg_ItemCode, this.clcg_ItemName, this.clcg_Batch, this.clcg_Kho, this.clcg_Dai, this.clcg_QtyReq, this.clcg_QtyInv, this.clcg_Joins, this.clcg_Whse,
			this.clcg_DangTon, this.clcg_KhoThuHoi, this.clcg_DaiThuHoi, this.clcg_LeKho, this.clcg_H2, this.clcg_QtyTTinh, this.clcg_Check
		});
		this.gv_DSCuon.DisableFormSetting = false;
		this.gv_DSCuon.DisplayException = false;
		this.gv_DSCuon.EditColumn = null;
		this.gv_DSCuon.FieldCurrency = null;
		this.gv_DSCuon.FieldRate = null;
		this.gv_DSCuon.FormId = null;
		this.gv_DSCuon.GridControl = this.grd_DSCuon;
		this.gv_DSCuon.IgnoreFilter = false;
		this.gv_DSCuon.IndexEditRow = -1;
		this.gv_DSCuon.IndicatorWidth = 35;
		this.gv_DSCuon.IsApzTable = false;
		this.gv_DSCuon.IsInsertNewRow = false;
		this.gv_DSCuon.IsLoadingData = false;
		this.gv_DSCuon.IsMappingContextAction = false;
		this.gv_DSCuon.ItemId = null;
		this.gv_DSCuon.LstColumnAllowBatch = null;
		this.gv_DSCuon.LstColumnNotAllowBatch = null;
		this.gv_DSCuon.LstColumnNotEditable = null;
		this.gv_DSCuon.LstColumNotSetting = null;
		this.gv_DSCuon.Name = "gv_DSCuon";
		this.gv_DSCuon.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
		this.gv_DSCuon.OptionsSelection.ShowCheckBoxSelectorInColumnHeader = DevExpress.Utils.DefaultBoolean.True;
		this.gv_DSCuon.OptionsView.ColumnAutoWidth = false;
		this.gv_DSCuon.OptionsView.ShowGroupPanel = false;
		this.gv_DSCuon.UseBatchMod = false;
		this.gv_DSCuon.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_DSCuon_CustomDrawRowIndicator);
		this.gv_DSCuon.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_DSCuon_CellValueChanged);
		this.gv_DSCuon.DoubleClick += new System.EventHandler(this.gv_DSCuon_DoubleClick);
		this.clcg_ID.Caption = "ID";
		this.clcg_ID.CurrencyDateField = null;
		this.clcg_ID.CurrencyField = null;
		this.clcg_ID.CurrencyFixed = null;
		this.clcg_ID.CurrencyFixedRate = 0.0;
		this.clcg_ID.CurrencyRateField = null;
		this.clcg_ID.FieldName = "ID";
		this.clcg_ID.Name = "clcg_ID";
		this.clcg_ID.PrimaryKey = true;
		this.clcg_ID.UnitField = null;
		this.clcg_ItemCode.Caption = "Mã hàng";
		this.clcg_ItemCode.CurrencyDateField = null;
		this.clcg_ItemCode.CurrencyField = null;
		this.clcg_ItemCode.CurrencyFixed = null;
		this.clcg_ItemCode.CurrencyFixedRate = 0.0;
		this.clcg_ItemCode.CurrencyRateField = null;
		this.clcg_ItemCode.FieldName = "ItemCode";
		this.clcg_ItemCode.Name = "clcg_ItemCode";
		this.clcg_ItemCode.PrimaryKey = false;
		this.clcg_ItemCode.UnitField = null;
		this.clcg_ItemCode.Visible = true;
		this.clcg_ItemCode.VisibleIndex = 0;
		this.clcg_ItemCode.Width = 80;
		this.clcg_ItemName.Caption = "Tên hàng";
		this.clcg_ItemName.CurrencyDateField = null;
		this.clcg_ItemName.CurrencyField = null;
		this.clcg_ItemName.CurrencyFixed = null;
		this.clcg_ItemName.CurrencyFixedRate = 0.0;
		this.clcg_ItemName.CurrencyRateField = null;
		this.clcg_ItemName.FieldName = "ItemName";
		this.clcg_ItemName.Name = "clcg_ItemName";
		this.clcg_ItemName.PrimaryKey = false;
		this.clcg_ItemName.UnitField = null;
		this.clcg_ItemName.Width = 150;
		this.clcg_Batch.Caption = "Mã cuộn";
		this.clcg_Batch.CurrencyDateField = null;
		this.clcg_Batch.CurrencyField = null;
		this.clcg_Batch.CurrencyFixed = null;
		this.clcg_Batch.CurrencyFixedRate = 0.0;
		this.clcg_Batch.CurrencyRateField = null;
		this.clcg_Batch.FieldName = "Batch";
		this.clcg_Batch.Name = "clcg_Batch";
		this.clcg_Batch.PrimaryKey = false;
		this.clcg_Batch.UnitField = null;
		this.clcg_Batch.Visible = true;
		this.clcg_Batch.VisibleIndex = 1;
		this.clcg_Batch.Width = 80;
		this.clcg_Kho.Caption = "Khổ (cm)";
		this.clcg_Kho.ColumnEdit = this.repositoryItemCalcEdit1;
		this.clcg_Kho.CurrencyDateField = null;
		this.clcg_Kho.CurrencyField = null;
		this.clcg_Kho.CurrencyFixed = null;
		this.clcg_Kho.CurrencyFixedRate = 0.0;
		this.clcg_Kho.CurrencyRateField = null;
		this.clcg_Kho.DisplayFormat.FormatString = "n2";
		this.clcg_Kho.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.clcg_Kho.FieldName = "Width";
		this.clcg_Kho.Name = "clcg_Kho";
		this.clcg_Kho.PrimaryKey = false;
		this.clcg_Kho.UnitField = null;
		this.clcg_Kho.Visible = true;
		this.clcg_Kho.VisibleIndex = 2;
		this.clcg_Kho.Width = 60;
		this.repositoryItemCalcEdit1.AutoHeight = false;
		this.repositoryItemCalcEdit1.DisplayFormat.FormatString = "n2";
		this.repositoryItemCalcEdit1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.repositoryItemCalcEdit1.EditFormat.FormatString = "n2";
		this.repositoryItemCalcEdit1.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.repositoryItemCalcEdit1.Mask.EditMask = "n2";
		this.repositoryItemCalcEdit1.Name = "repositoryItemCalcEdit1";
		this.clcg_Dai.Caption = "Dài (m)";
		this.clcg_Dai.ColumnEdit = this.repositoryItemCalcEdit1;
		this.clcg_Dai.CurrencyDateField = null;
		this.clcg_Dai.CurrencyField = null;
		this.clcg_Dai.CurrencyFixed = null;
		this.clcg_Dai.CurrencyFixedRate = 0.0;
		this.clcg_Dai.CurrencyRateField = null;
		this.clcg_Dai.DisplayFormat.FormatString = "n2";
		this.clcg_Dai.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.clcg_Dai.FieldName = "Length";
		this.clcg_Dai.Name = "clcg_Dai";
		this.clcg_Dai.PrimaryKey = false;
		this.clcg_Dai.UnitField = null;
		this.clcg_Dai.Visible = true;
		this.clcg_Dai.VisibleIndex = 3;
		this.clcg_Dai.Width = 60;
		this.clcg_QtyReq.Caption = "SL cần";
		this.clcg_QtyReq.ColumnEdit = this.repositoryItemCalcEdit1;
		this.clcg_QtyReq.CurrencyDateField = null;
		this.clcg_QtyReq.CurrencyField = null;
		this.clcg_QtyReq.CurrencyFixed = null;
		this.clcg_QtyReq.CurrencyFixedRate = 0.0;
		this.clcg_QtyReq.CurrencyRateField = null;
		this.clcg_QtyReq.DisplayFormat.FormatString = "n2";
		this.clcg_QtyReq.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.clcg_QtyReq.FieldName = "QtyReq";
		this.clcg_QtyReq.Name = "clcg_QtyReq";
		this.clcg_QtyReq.PrimaryKey = false;
		this.clcg_QtyReq.UnitField = null;
		this.clcg_QtyReq.Visible = true;
		this.clcg_QtyReq.VisibleIndex = 4;
		this.clcg_QtyReq.Width = 80;
		this.clcg_QtyInv.Caption = "SL tồn";
		this.clcg_QtyInv.ColumnEdit = this.repositoryItemCalcEdit1;
		this.clcg_QtyInv.CurrencyDateField = null;
		this.clcg_QtyInv.CurrencyField = null;
		this.clcg_QtyInv.CurrencyFixed = null;
		this.clcg_QtyInv.CurrencyFixedRate = 0.0;
		this.clcg_QtyInv.CurrencyRateField = null;
		this.clcg_QtyInv.DisplayFormat.FormatString = "n2";
		this.clcg_QtyInv.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.clcg_QtyInv.FieldName = "QtyInv";
		this.clcg_QtyInv.Name = "clcg_QtyInv";
		this.clcg_QtyInv.PrimaryKey = false;
		this.clcg_QtyInv.UnitField = null;
		this.clcg_QtyInv.Visible = true;
		this.clcg_QtyInv.VisibleIndex = 5;
		this.clcg_QtyInv.Width = 80;
		this.clcg_Joins.Caption = "Mối nối";
		this.clcg_Joins.ColumnEdit = this.repositoryItemCalcEdit1;
		this.clcg_Joins.CurrencyDateField = null;
		this.clcg_Joins.CurrencyField = null;
		this.clcg_Joins.CurrencyFixed = null;
		this.clcg_Joins.CurrencyFixedRate = 0.0;
		this.clcg_Joins.CurrencyRateField = null;
		this.clcg_Joins.FieldName = "Joins";
		this.clcg_Joins.Name = "clcg_Joins";
		this.clcg_Joins.PrimaryKey = false;
		this.clcg_Joins.UnitField = null;
		this.clcg_Joins.Visible = true;
		this.clcg_Joins.VisibleIndex = 6;
		this.clcg_Joins.Width = 50;
		this.clcg_Whse.Caption = "Kho";
		this.clcg_Whse.CurrencyDateField = null;
		this.clcg_Whse.CurrencyField = null;
		this.clcg_Whse.CurrencyFixed = null;
		this.clcg_Whse.CurrencyFixedRate = 0.0;
		this.clcg_Whse.CurrencyRateField = null;
		this.clcg_Whse.FieldName = "Whse";
		this.clcg_Whse.Name = "clcg_Whse";
		this.clcg_Whse.PrimaryKey = false;
		this.clcg_Whse.UnitField = null;
		this.clcg_Whse.Visible = true;
		this.clcg_Whse.VisibleIndex = 7;
		this.clcg_Whse.Width = 80;
		this.clcg_DangTon.Caption = "Dạng tồn";
		this.clcg_DangTon.CurrencyDateField = null;
		this.clcg_DangTon.CurrencyField = null;
		this.clcg_DangTon.CurrencyFixed = null;
		this.clcg_DangTon.CurrencyFixedRate = 0.0;
		this.clcg_DangTon.CurrencyRateField = null;
		this.clcg_DangTon.FieldName = "DangTon";
		this.clcg_DangTon.Name = "clcg_DangTon";
		this.clcg_DangTon.PrimaryKey = false;
		this.clcg_DangTon.UnitField = null;
		this.clcg_DangTon.Visible = true;
		this.clcg_DangTon.VisibleIndex = 8;
		this.clcg_DangTon.Width = 110;
		this.clcg_KhoThuHoi.Caption = "Khổ thu hồi";
		this.clcg_KhoThuHoi.ColumnEdit = this.repositoryItemCalcEdit1;
		this.clcg_KhoThuHoi.CurrencyDateField = null;
		this.clcg_KhoThuHoi.CurrencyField = null;
		this.clcg_KhoThuHoi.CurrencyFixed = null;
		this.clcg_KhoThuHoi.CurrencyFixedRate = 0.0;
		this.clcg_KhoThuHoi.CurrencyRateField = null;
		this.clcg_KhoThuHoi.DisplayFormat.FormatString = "n2";
		this.clcg_KhoThuHoi.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.clcg_KhoThuHoi.FieldName = "KhoThuHoi";
		this.clcg_KhoThuHoi.Name = "clcg_KhoThuHoi";
		this.clcg_KhoThuHoi.PrimaryKey = false;
		this.clcg_KhoThuHoi.UnitField = null;
		this.clcg_KhoThuHoi.Visible = true;
		this.clcg_KhoThuHoi.VisibleIndex = 9;
		this.clcg_KhoThuHoi.Width = 110;
		this.clcg_DaiThuHoi.Caption = "Dài thu hồi";
		this.clcg_DaiThuHoi.ColumnEdit = this.repositoryItemCalcEdit1;
		this.clcg_DaiThuHoi.CurrencyDateField = null;
		this.clcg_DaiThuHoi.CurrencyField = null;
		this.clcg_DaiThuHoi.CurrencyFixed = null;
		this.clcg_DaiThuHoi.CurrencyFixedRate = 0.0;
		this.clcg_DaiThuHoi.CurrencyRateField = null;
		this.clcg_DaiThuHoi.DisplayFormat.FormatString = "n2";
		this.clcg_DaiThuHoi.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.clcg_DaiThuHoi.FieldName = "DaiThuHoi";
		this.clcg_DaiThuHoi.Name = "clcg_DaiThuHoi";
		this.clcg_DaiThuHoi.PrimaryKey = false;
		this.clcg_DaiThuHoi.UnitField = null;
		this.clcg_DaiThuHoi.Visible = true;
		this.clcg_DaiThuHoi.VisibleIndex = 10;
		this.clcg_DaiThuHoi.Width = 110;
		this.clcg_LeKho.Caption = "Lẻ khổ";
		this.clcg_LeKho.ColumnEdit = this.repositoryItemCalcEdit1;
		this.clcg_LeKho.CurrencyDateField = null;
		this.clcg_LeKho.CurrencyField = null;
		this.clcg_LeKho.CurrencyFixed = null;
		this.clcg_LeKho.CurrencyFixedRate = 0.0;
		this.clcg_LeKho.CurrencyRateField = null;
		this.clcg_LeKho.DisplayFormat.FormatString = "n2";
		this.clcg_LeKho.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.clcg_LeKho.FieldName = "LeKho";
		this.clcg_LeKho.Name = "clcg_LeKho";
		this.clcg_LeKho.PrimaryKey = false;
		this.clcg_LeKho.UnitField = null;
		this.clcg_LeKho.Visible = true;
		this.clcg_LeKho.VisibleIndex = 11;
		this.clcg_LeKho.Width = 100;
		this.clcg_H2.Caption = "H2";
		this.clcg_H2.ColumnEdit = this.repositoryItemCheckEdit3;
		this.clcg_H2.CurrencyDateField = null;
		this.clcg_H2.CurrencyField = null;
		this.clcg_H2.CurrencyFixed = null;
		this.clcg_H2.CurrencyFixedRate = 0.0;
		this.clcg_H2.CurrencyRateField = null;
		this.clcg_H2.FieldName = "H2";
		this.clcg_H2.Name = "clcg_H2";
		this.clcg_H2.PrimaryKey = false;
		this.clcg_H2.UnitField = null;
		this.clcg_H2.Visible = true;
		this.clcg_H2.VisibleIndex = 12;
		this.clcg_H2.Width = 50;
		this.repositoryItemCheckEdit3.AutoHeight = false;
		this.repositoryItemCheckEdit3.Name = "repositoryItemCheckEdit3";
		this.clcg_QtyTTinh.Caption = "SL T. Tính";
		this.clcg_QtyTTinh.ColumnEdit = this.repositoryItemCalcEdit1;
		this.clcg_QtyTTinh.CurrencyDateField = null;
		this.clcg_QtyTTinh.CurrencyField = null;
		this.clcg_QtyTTinh.CurrencyFixed = null;
		this.clcg_QtyTTinh.CurrencyFixedRate = 0.0;
		this.clcg_QtyTTinh.CurrencyRateField = null;
		this.clcg_QtyTTinh.DisplayFormat.FormatString = "n2";
		this.clcg_QtyTTinh.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.clcg_QtyTTinh.FieldName = "QtyTTinh";
		this.clcg_QtyTTinh.Name = "clcg_QtyTTinh";
		this.clcg_QtyTTinh.PrimaryKey = false;
		this.clcg_QtyTTinh.UnitField = null;
		this.clcg_QtyTTinh.Visible = true;
		this.clcg_QtyTTinh.VisibleIndex = 13;
		this.clcg_QtyTTinh.Width = 110;
		this.clcg_Check.Caption = " Check";
		this.clcg_Check.ColumnEdit = this.repositoryItemCheckEdit3;
		this.clcg_Check.CurrencyDateField = null;
		this.clcg_Check.CurrencyField = null;
		this.clcg_Check.CurrencyFixed = null;
		this.clcg_Check.CurrencyFixedRate = 0.0;
		this.clcg_Check.CurrencyRateField = null;
		this.clcg_Check.FieldName = "Checked";
		this.clcg_Check.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Right;
		this.clcg_Check.Name = "clcg_Check";
		this.clcg_Check.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.clcg_Check.OptionsColumn.FixedWidth = true;
		this.clcg_Check.PrimaryKey = false;
		this.clcg_Check.UnitField = null;
		this.clcg_Check.Visible = true;
		this.clcg_Check.VisibleIndex = 14;
		this.clcg_Check.Width = 35;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(897, 524);
		base.Controls.Add(this.splitContainerControl1);
		base.IconOptions.ShowIcon = false;
		base.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		base.Name = "frm_ChooseBatchForQuotation";
		this.Text = "Tính giấy";
		base.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frm_ChooseBatchForQuotation_FormClosed);
		base.Load += new System.EventHandler(this.frm_TinhGiay_Load);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl4).EndInit();
		this.splitContainerControl4.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).EndInit();
		this.splitContainerControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.groupControl1).EndInit();
		this.groupControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_DSTinhGiay).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_DSTinhGiay).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl2).EndInit();
		this.groupControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_DSGhepKho).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_DSGhepKho).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Spin_N2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repo_Check).EndInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl6).EndInit();
		this.groupControl6.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_DSDaChon).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_DSDaChon).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl3).EndInit();
		this.groupControl3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl3).EndInit();
		this.splitContainerControl3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.groupControl5).EndInit();
		this.groupControl5.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Kho).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Kho).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repo_LookUp_Status).EndInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl4).EndInit();
		this.groupControl4.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_DSCuon).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_DSCuon).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit3).EndInit();
		base.ResumeLayout(false);
	}
}
