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
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Properties;
using Apzon.Forms.SalesOppotunity;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Sales;

public class frm_SalesBlanketAgreement_RowDetails : XtraFormEx
{
	public DataTable dtLine = new DataTable();

	public DataTable dtDetail;

	public DateTime StartDate;

	public DateTime EndDate;

	public DataTable dtClgCode;

	public string CardCode = "";

	public static Dictionary<string, GridViewEx> TblFormular = new Dictionary<string, GridViewEx>();

	private IContainer components;

	private GridControlEx grd_Detail;

	private GridViewEx gv_Detail;

	private GridColumnEx gcAgrNo;

	private GridColumnEx gcAgrLnNum;

	private GridColumnEx gcAgrEfctNum;

	private GridColumnEx gcDatePeriod;

	private GridColumnEx gcFromDate;

	private GridColumnEx gcToDate;

	private PanelControl panelControl1;

	private GridColumnEx gcItemCode;

	private GridColumnEx gcItemName;

	private GridColumnEx gcCallUp;

	private GridColumnEx gcQuantity;

	private GridColumnEx gcWhsCode;

	private GridColumnEx gcFreeTxt;

	private GridColumnEx gcConsumeFCT;

	private GridColumnEx gcActivity;

	private RepositoryItemLookUpEdit rep_Frequency;

	private RepositoryItemDateEdit rep_FromDate;

	private RepositoryItemDateEdit rep_ToDate;

	private RepositoryItemCalcEdit rep_Quantity;

	private RepositoryItemButtonEdit rep_Warehouse;

	private RepositoryItemCheckEdit rep_ConsumeFCT;

	private RepositoryItemButtonEdit rep_Activity;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_OK;

	private GridColumnEx gcAmountLC;

	private GridColumnEx gcAmountFC;

	private PanelControl panelControl2;

	private ContextMenuStrip contextMenuStrip_Detail;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

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

	public frm_SalesBlanketAgreement_RowDetails()
	{
		InitializeComponent();
	}

	private void gridView1_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle >= 0)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private async void frm_SalesBlanketAgreement_RowDetails_Load(object sender, EventArgs e)
	{
		WinFuntions.SetFormatControlValue(this);
		await WinFuntions.LoadUdfColumns(gv_Detail, "OAT2", TblFormular);
		await LoadDetail();
		if (dtClgCode != null)
		{
			CreateDTClgCode();
		}
		rep_Quantity.Mask.MaskType = MaskType.Numeric;
		rep_Quantity.Mask.EditMask = "n" + GlobalData.QuantitiesFormat.NumberDecimalDigits;
		rep_Quantity.Mask.UseMaskAsDisplayFormat = true;
		btn_Cancel.Enabled = false;
	}

	private async void rep_Warehouse_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Warehouse, multiSelect: false, string.Empty);
			frm_ChooseFromList2.ShowDialog(this);
			DataRow dataRow = (DataRow)frm_ChooseFromList2.ChooseItem;
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				gv_Detail.SetFocusedRowCellValue("WhsCode", dataRow["WhsCode"]);
			}
			frm_ChooseFromList2.Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void rep_Activity_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			int agrNo = Function.ParseInt(gv_Detail.GetFocusedRowCellValue("AgrNo"));
			int agrLnNum = Function.ParseInt(gv_Detail.GetFocusedRowCellValue("AgrLnNum"));
			int agrEfctNum = Function.ParseInt(gv_Detail.GetFocusedRowCellValue("AgrEfctNum"));
			List<DataRow> list = (from t in dtClgCode.AsEnumerable()
				where Function.ParseInt(t["AgrNo"]) == agrNo && Function.ParseInt(t["AgrLnNum"]) == agrLnNum && Function.ParseInt(t["AgrEfctNum"]) == agrEfctNum
				select t).ToList();
			if (list != null && list.Count > 0)
			{
				List<int> list2 = new List<int>();
				DataTable dtSource = new DataTable();
				foreach (DataRow item in list)
				{
					list2.Add(Function.ParseInt(item["ActivityID"]));
				}
				string lstClgCodes = string.Join(",", list2.ToArray());
				using (HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SalesBlanketAgreement"))
				{
					dtSource = httpClientHelper.Get("GetActivitiesByActivityID", new { lstClgCodes });
				}
				frm_ActivityListForSalesOpportunity frmlstActivity = new frm_ActivityListForSalesOpportunity(dtSource, CardCode);
				frmlstActivity.Text = "Activities Overview for Blanket Agreement";
				frmlstActivity.MdiParent = MdiParent;
				frmlstActivity.FormClosing += (object s, FormClosingEventArgs events) =>
				{
					try
					{
						if (frmlstActivity.dtActList != null && frmlstActivity.dtActList.Rows.Count > 0)
						{
							MergeDataClgCode(frmlstActivity.dtActList, agrNo, agrLnNum, agrEfctNum);
						}
					}
					catch (Exception ex2)
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
						WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex2.Message);
					}
				};
				frmlstActivity.Show();
				return;
			}
			DataTable dataTable = CheckExistActivity();
			DataTable dtSource2 = new DataTable();
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				List<int> list3 = new List<int>();
				foreach (DataRow row in dataTable.Rows)
				{
					list3.Add(Function.ParseInt(row["ActivityID"]));
				}
				string lstClgCodes2 = string.Join(",", list3.ToArray());
				using (HttpClientHelper<DataTable> httpClientHelper2 = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SalesBlanketAgreement"))
				{
					dtSource2 = httpClientHelper2.Get("GetActivitiesByActivityID", new
					{
						lstClgCodes = lstClgCodes2
					});
				}
				frm_ActivityListForSalesOpportunity frmlstActivity2 = new frm_ActivityListForSalesOpportunity(dtSource2, CardCode);
				frmlstActivity2.Text = "Activities Overview for Blanket Agreement";
				frmlstActivity2.MdiParent = MdiParent;
				frmlstActivity2.FormClosing += (object s, FormClosingEventArgs events) =>
				{
					try
					{
						if (frmlstActivity2.dtActList != null && frmlstActivity2.dtActList.Rows.Count > 0)
						{
							MergeDataClgCode(frmlstActivity2.dtActList, agrNo, agrLnNum, agrEfctNum);
						}
					}
					catch (Exception ex2)
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
						WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex2.Message);
					}
				};
				frmlstActivity2.Show();
				return;
			}
			frm_ActivitiesForOpportunity frmAct = new frm_ActivitiesForOpportunity();
			frmAct.MdiParent = MdiParent;
			frmAct.isLoadFromOpportunity = true;
			frmAct._cardCodefrm_SalesOpportunity = CardCode;
			frmAct.FormClosing += (object s, FormClosingEventArgs events) =>
			{
				try
				{
					if (frmAct.dtCopy.Rows.Count > 0)
					{
						MergeDataClgCode(frmAct.dtCopy.Copy(), agrNo, agrLnNum, agrEfctNum);
					}
				}
				catch (Exception ex2)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
					WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex2.Message);
				}
			};
			frmAct.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gv_Detail_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			FormMode = Enums.FormViewModeType.Update;
			btn_Cancel.Enabled = true;
			if (!string.IsNullOrEmpty(Function.ToString(gv_Detail.GetRowCellValue(e.RowHandle, "FromDate"))) && Function.ParseInt(gv_Detail.GetRowCellValue(e.RowHandle + 1, "AgrEfctNum")) <= 0)
			{
				AddNewDetail();
			}
			if (e.Column.FieldName == "FromDate")
			{
				if (Function.ParseDateTime(e.Value) > EndDate)
				{
					WinCommons.Error(Enums.MessageCode.SpecifyFromDateEarlierOrEqualEndDate, "");
					gv_Detail.SetRowCellValue(e.RowHandle, "FromDate", null);
					return;
				}
				if (Function.ParseDateTime(e.Value) < StartDate)
				{
					WinCommons.Error(Enums.MessageCode.SpecifyFromDateLaterOrEqualStartDate, "");
					gv_Detail.SetRowCellValue(e.RowHandle, "FromDate", null);
					return;
				}
			}
			if (e.Column.FieldName == "ToDate")
			{
				if (Function.ParseDateTime(e.Value) > EndDate)
				{
					WinCommons.Error(Enums.MessageCode.SpecifyToDateEarlierOrEqualEndDate, "");
					gv_Detail.SetRowCellValue(e.RowHandle, "ToDate", null);
				}
				else if (Function.ParseDateTime(e.Value) < StartDate)
				{
					WinCommons.Error(Enums.MessageCode.SpecifyToDateLaterOrEqualStartDate, "");
					gv_Detail.SetRowCellValue(e.RowHandle, "ToDate", null);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else
			{
				if (!CheckUpdate())
				{
					return;
				}
				DataTable dataTable = (DataTable)grd_Detail.DataSource;
				dtDetail = dataTable.Clone();
				foreach (DataRow row in dataTable.Rows)
				{
					if (!string.IsNullOrEmpty(Function.ToString(row["FromDate"])) && !string.IsNullOrEmpty(Function.ToString(row["ToDate"])) && !string.IsNullOrEmpty(Function.ToString(row["Quantity"])))
					{
						DataRow dataRow2 = dtDetail.NewRow();
						dataRow2.ItemArray = row.ItemArray;
						dtDetail.Rows.Add(dataRow2);
					}
				}
				dtDetail.AcceptChanges();
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_Cancel_Click(object sender, EventArgs e)
	{
		dtClgCode = new DataTable();
		dtDetail = new DataTable();
		Close();
	}

	private void grd_Detail_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (!string.IsNullOrEmpty(Function.ToString(gv_Detail.GetDataRow(gv_Detail.RowCount - 1)["FromDate"])) && e.KeyCode == Keys.Tab)
			{
				AddNewDetail();
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
		if (gv_Detail.FocusedRowHandle != gv_Detail.RowCount - 1)
		{
			gv_Detail.DeleteSelectedRows();
			FormMode = Enums.FormViewModeType.Update;
			btn_Cancel.Enabled = true;
		}
	}

	private void gv_Detail_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
	{
		gcQuantity.SummaryItem.DisplayFormat = "{0:n" + GlobalData.QuantitiesFormat.CurrencyDecimalDigits + "}";
	}

	private async Task LoadDetail()
	{
		try
		{
			List<Common.Table_ID> list = new List<Common.Table_ID>();
			list.Add(new Common.Table_ID
			{
				ID = "D",
				Name = "Daily"
			});
			list.Add(new Common.Table_ID
			{
				ID = "W",
				Name = "Weekly"
			});
			list.Add(new Common.Table_ID
			{
				ID = "M",
				Name = "Monthly"
			});
			list.Add(new Common.Table_ID
			{
				ID = "Q",
				Name = "Quarterly"
			});
			list.Add(new Common.Table_ID
			{
				ID = "S",
				Name = "Semi-Annually"
			});
			list.Add(new Common.Table_ID
			{
				ID = "A",
				Name = "Annually"
			});
			list.Add(new Common.Table_ID
			{
				ID = "O",
				Name = "One Time"
			});
			rep_Frequency.DataSource = list;
			int agrNo = Function.ParseInt(dtLine.Rows[0]["AgrNo"]);
			int agrLnNum = Function.ParseInt(dtLine.Rows[0]["AgrLineNum"]);
			if (!dtDetail.IsExistsColumn("ItemCode"))
			{
				dtDetail.Columns.Add("ItemCode", typeof(string));
			}
			if (!dtDetail.IsExistsColumn("ItemName"))
			{
				dtDetail.Columns.Add("ItemName", typeof(string));
			}
			List<DataRow> list2 = (from t in dtDetail.AsEnumerable()
				where Function.ParseInt(t["AgrNo"]) == agrNo && Function.ParseInt(t["AgrLnNum"]) == agrLnNum
				select t).ToList();
			if (list2 != null && list2.Count > 0)
			{
				foreach (DataRow item in list2)
				{
					item["ItemCode"] = Function.ToString(dtLine.Rows[0]["ItemCode"]);
					item["ItemName"] = Function.ToString(dtLine.Rows[0]["ItemName"]);
				}
				grd_Detail.DataSource = list2.CopyToDataTable();
				grd_Detail.RefreshDataSource();
			}
			else
			{
				Tuple<int, int> tupe = new Tuple<int, int>(agrNo, agrLnNum);
				DataTable blanketAgreementDetails = GetBlanketAgreementDetails(tupe);
				blanketAgreementDetails.Columns.Add("ItemCode", typeof(string));
				blanketAgreementDetails.Columns.Add("ItemName", typeof(string));
				blanketAgreementDetails.AcceptChanges();
				foreach (DataRow row in blanketAgreementDetails.Rows)
				{
					row["ItemCode"] = Function.ToString(dtLine.Rows[0]["ItemCode"]);
					row["ItemName"] = Function.ToString(dtLine.Rows[0]["ItemName"]);
				}
				grd_Detail.DataSource = blanketAgreementDetails;
				grd_Detail.RefreshDataSource();
			}
			AddNewDetail();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private DataTable GetBlanketAgreementDetails(Tuple<int, int> tupe)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SalesBlanketAgreement");
			return httpClientHelper.Post("GetBlanketAgreementDetails", tupe);
		}
		catch (Exception)
		{
			return new DataTable();
		}
	}

	private void AddNewDetail()
	{
		try
		{
			DataTable dataTable = new DataTable();
			if (grd_Detail.DataSource is DataTable)
			{
				dataTable = grd_Detail.DataSource as DataTable;
			}
			else
			{
				dataTable.Columns.Add("AgrNo", typeof(int));
				dataTable.Columns.Add("AgrLnNum", typeof(int));
				dataTable.Columns.Add("ItemCode", typeof(string));
				dataTable.Columns.Add("ItemName", typeof(string));
				dataTable.Columns.Add("AgrEfctNum", typeof(int));
				dataTable.Columns.Add("Quantity", typeof(decimal));
				dataTable.Columns.Add("AmountLC", typeof(decimal));
				dataTable.Columns.Add("AmountFC", typeof(decimal));
				dataTable.Columns.Add("DatePeriod", typeof(string));
				dataTable.Columns.Add("CallUp", typeof(string));
				dataTable.Columns.Add("WhsCode", typeof(string));
				dataTable.Columns.Add("FromDate", typeof(DateTime));
				dataTable.Columns.Add("ToDate", typeof(DateTime));
				dataTable.Columns.Add("LogInstanc", typeof(int));
				dataTable.Columns.Add("ConsumeFCT", typeof(string));
				dataTable.Columns.Add("FreeTxt", typeof(string));
			}
			DataRow dataRow = dataTable.NewRow();
			if (dataRow.Table.Columns.Contains("AgrNo"))
			{
				dataRow["AgrNo"] = Function.ParseInt(dtLine.Rows[0]["AgrNo"]);
			}
			if (dataRow.Table.Columns.Contains("AgrLnNum"))
			{
				dataRow["AgrLnNum"] = Function.ParseInt(dtLine.Rows[0]["AgrLineNum"]);
			}
			if (dataRow.Table.Columns.Contains("AgrEfctNum"))
			{
				dataRow["AgrEfctNum"] = dataTable.Rows.Count + 1;
			}
			if (dataRow.Table.Columns.Contains("DatePeriod"))
			{
				dataRow["DatePeriod"] = "M";
			}
			if (dataRow.Table.Columns.Contains("ItemCode") && Function.ToString(dtLine.Rows[0]["ItemCode"]) != null)
			{
				dataRow["ItemCode"] = Function.ToString(dtLine.Rows[0]["ItemCode"]);
			}
			if (dataRow.Table.Columns.Contains("ItemName") && Function.ToString(dtLine.Rows[0]["ItemName"]) != null)
			{
				dataRow["ItemName"] = Function.ToString(dtLine.Rows[0]["ItemName"]);
			}
			if (dataRow.Table.Columns.Contains("ConsumeFCT"))
			{
				dataRow["ConsumeFCT"] = "Y";
			}
			dataTable.Rows.Add(dataRow);
			grd_Detail.DataSource = dataTable;
			grd_Detail.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private DataTable CheckExistActivity()
	{
		try
		{
			int item = Function.ParseInt(gv_Detail.GetFocusedRowCellValue("AgrNo"));
			int item2 = Function.ParseInt(gv_Detail.GetFocusedRowCellValue("AgrLnNum"));
			int item3 = Function.ParseInt(gv_Detail.GetFocusedRowCellValue("AgrEfctNum"));
			Tuple<int, int, int> pr = new Tuple<int, int, int>(item, item2, item3);
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SalesBlanketAgreement");
			return httpClientHelper.Post("CheckExistActivity", pr);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	private bool CheckUpdate()
	{
		try
		{
			decimal num = Function.ParseDecimal(dtLine.Rows[0]["PlanQty"]);
			DataTable dataTable = (DataTable)grd_Detail.DataSource;
			int num2 = 0;
			if (dataTable.Rows.Count == 1)
			{
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					if (!Function.ParseDateTime(dataTable.Rows[i]["FromDate"]).HasValue)
					{
						WinCommons.Error(Enums.MessageCode.SpecifyFromDate, "");
						return false;
					}
					if (!Function.ParseDateTime(dataTable.Rows[i]["ToDate"]).HasValue)
					{
						WinCommons.Error(Enums.MessageCode.SpecifyToDate, "");
						return false;
					}
					if (Function.ParseInt(dataTable.Rows[i]["Quantity"]) <= 0)
					{
						WinCommons.Error(Enums.MessageCode.SpecifyQuantityMustGreater0, "");
						return false;
					}
					num2 += Function.ParseInt(dataTable.Rows[i]["Quantity"]);
				}
			}
			else
			{
				for (int j = 0; j < dataTable.Rows.Count - 1; j++)
				{
					if (!Function.ParseDateTime(dataTable.Rows[j]["FromDate"]).HasValue)
					{
						WinCommons.Error(Enums.MessageCode.SpecifyFromDate, "");
						return false;
					}
					if (!Function.ParseDateTime(dataTable.Rows[j]["ToDate"]).HasValue)
					{
						WinCommons.Error(Enums.MessageCode.SpecifyToDate, "");
						return false;
					}
					if (Function.ParseInt(dataTable.Rows[j]["Quantity"]) <= 0)
					{
						WinCommons.Error(Enums.MessageCode.SpecifyQuantityMustGreater0, "");
						return false;
					}
					num2 += Function.ParseInt(dataTable.Rows[j]["Quantity"]);
				}
			}
			if ((decimal)num2 > num)
			{
				WinCommons.Error(Enums.MessageCode.SpecifyQuantityItemIsSmallerOrEqualPlannedQuantity, "");
				return false;
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

	private void CreateDTClgCode()
	{
		dtClgCode = new DataTable();
		dtClgCode.Columns.Add("AgrNo", typeof(int));
		dtClgCode.Columns.Add("AgrLnNum", typeof(int));
		dtClgCode.Columns.Add("AgrEfctNum", typeof(int));
		dtClgCode.Columns.Add("ActivityID", typeof(int));
		dtClgCode.Columns.Add("LogInstanc", typeof(int));
		dtClgCode.AcceptChanges();
	}

	private void MergeDataClgCode(DataTable dt, int agrNo, int agrLnNum, int agrEfnum)
	{
		if (dtClgCode != null && dtClgCode.Rows.Count > 0)
		{
			List<DataRow> list = (from t in dtClgCode.AsEnumerable()
				where Function.ParseInt(t["AgrNo"]) == agrNo && Function.ParseInt(t["AgrLnNum"]) == agrLnNum && Function.ParseInt(t["AgrEfctNum"]) == agrEfnum
				select t).ToList();
			if (list != null && list.Count > 0)
			{
				foreach (DataRow item in list)
				{
					dtClgCode.Rows.Remove(item);
				}
				dtClgCode.AcceptChanges();
			}
		}
		foreach (DataRow row in dt.Rows)
		{
			DataRow dataRow2 = dtClgCode.NewRow();
			dataRow2["AgrNo"] = agrNo;
			dataRow2["AgrLnNum"] = agrLnNum;
			dataRow2["AgrEfctNum"] = agrEfnum;
			dataRow2["ActivityID"] = Function.ParseInt(row["ClgCode"]);
			dtClgCode.Rows.Add(dataRow2);
		}
		if (dtClgCode != null && dtClgCode.Rows.Count > 0)
		{
			FormMode = Enums.FormViewModeType.Update;
			btn_Cancel.Enabled = true;
		}
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		try
		{
			if (msg.Msg == 256 || msg.Msg == 260)
			{
				switch (keyData)
				{
				case Keys.Return:
					btn_OK_Click(null, null);
					break;
				case Keys.Tab:
					EventTabkey();
					break;
				}
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return false;
	}

	private void EventTabkey()
	{
		try
		{
			if (gv_Detail.FocusedColumn.FieldName == "WhsCode")
			{
				rep_Warehouse_ButtonClick(null, null);
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
		this.components = new System.ComponentModel.Container();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		this.grd_Detail = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip_Detail = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Detail = new Apzon.Controls.Grid.GridViewEx();
		this.gcAgrNo = new Apzon.Controls.Grid.GridColumnEx();
		this.gcAgrLnNum = new Apzon.Controls.Grid.GridColumnEx();
		this.gcAgrEfctNum = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDatePeriod = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Frequency = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcFromDate = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_FromDate = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gcToDate = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ToDate = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gcItemCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcItemName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcCallUp = new Apzon.Controls.Grid.GridColumnEx();
		this.gcQuantity = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Quantity = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcWhsCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Warehouse = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcFreeTxt = new Apzon.Controls.Grid.GridColumnEx();
		this.gcConsumeFCT = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ConsumeFCT = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gcActivity = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Activity = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcAmountLC = new Apzon.Controls.Grid.GridColumnEx();
		this.gcAmountFC = new Apzon.Controls.Grid.GridColumnEx();
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
		((System.ComponentModel.ISupportInitialize)this.grd_Detail).BeginInit();
		this.contextMenuStrip_Detail.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Detail).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Frequency).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_FromDate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_FromDate.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ToDate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ToDate.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Quantity).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Warehouse).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ConsumeFCT).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Activity).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl2).BeginInit();
		this.panelControl2.SuspendLayout();
		base.SuspendLayout();
		this.grd_Detail.ContextMenuStrip = this.contextMenuStrip_Detail;
		this.grd_Detail.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Detail.Location = new System.Drawing.Point(2, 2);
		this.grd_Detail.MainView = this.gv_Detail;
		this.grd_Detail.Name = "grd_Detail";
		this.grd_Detail.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[7] { this.rep_Frequency, this.rep_FromDate, this.rep_ToDate, this.rep_Quantity, this.rep_Warehouse, this.rep_ConsumeFCT, this.rep_Activity });
		this.grd_Detail.Size = new System.Drawing.Size(586, 224);
		this.grd_Detail.TabIndex = 0;
		this.grd_Detail.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Detail });
		this.grd_Detail.KeyDown += new System.Windows.Forms.KeyEventHandler(this.grd_Detail_KeyDown);
		this.contextMenuStrip_Detail.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.deleteRowToolStripMenuItem });
		this.contextMenuStrip_Detail.Name = "contextMenuStrip_Detail";
		this.contextMenuStrip_Detail.Size = new System.Drawing.Size(130, 26);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		DevExpress.XtraGrid.Columns.GridColumnCollection columns = this.gv_Detail.Columns;
		DevExpress.XtraGrid.Columns.GridColumn[] columns2 = new Apzon.Controls.Grid.GridColumnEx[16]
		{
			this.gcAgrNo, this.gcAgrLnNum, this.gcAgrEfctNum, this.gcDatePeriod, this.gcFromDate, this.gcToDate, this.gcItemCode, this.gcItemName, this.gcCallUp, this.gcQuantity,
			this.gcWhsCode, this.gcFreeTxt, this.gcConsumeFCT, this.gcActivity, this.gcAmountLC, this.gcAmountFC
		};
		columns.AddRange(columns2);
		this.gv_Detail.GridControl = this.grd_Detail;
		this.gv_Detail.IndicatorWidth = 35;
		this.gv_Detail.Name = "gv_Detail";
		this.gv_Detail.OptionsView.ColumnAutoWidth = false;
		this.gv_Detail.OptionsView.ShowFooter = true;
		this.gv_Detail.OptionsView.ShowGroupPanel = false;
		this.gv_Detail.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gridView1_CustomDrawRowIndicator);
		this.gv_Detail.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Detail_CellValueChanged);
		this.gv_Detail.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gv_Detail_CustomColumnDisplayText);
		this.gcAgrNo.Caption = "AgrNo";
		this.gcAgrNo.FieldName = "AgrNo";
		this.gcAgrNo.Name = "gcAgrNo";
		this.gcAgrLnNum.Caption = "AgrLnNum";
		this.gcAgrLnNum.FieldName = "AgrLnNum";
		this.gcAgrLnNum.Name = "gcAgrLnNum";
		this.gcAgrEfctNum.Caption = "AgrEfctNum";
		this.gcAgrEfctNum.FieldName = "AgrEfctNum";
		this.gcAgrEfctNum.Name = "gcAgrEfctNum";
		this.gcDatePeriod.AppearanceCell.BackColor = System.Drawing.Color.White;
		this.gcDatePeriod.AppearanceCell.Options.UseBackColor = true;
		this.gcDatePeriod.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.gcDatePeriod.Caption = "Frequency";
		this.gcDatePeriod.ColumnEdit = this.rep_Frequency;
		this.gcDatePeriod.FieldName = "DatePeriod";
		this.gcDatePeriod.Name = "gcDatePeriod";
		this.gcDatePeriod.OptionsFilter.AllowAutoFilter = false;
		this.gcDatePeriod.OptionsFilter.AllowFilter = false;
		this.gcDatePeriod.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcDatePeriod.Visible = true;
		this.gcDatePeriod.VisibleIndex = 0;
		this.rep_Frequency.AutoHeight = false;
		this.rep_Frequency.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.DropDown)
		});
		this.rep_Frequency.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "ID", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default)
		});
		this.rep_Frequency.DisplayMember = "Name";
		this.rep_Frequency.Name = "rep_Frequency";
		this.rep_Frequency.NullText = "";
		this.rep_Frequency.ShowFooter = false;
		this.rep_Frequency.ShowHeader = false;
		this.rep_Frequency.ValueMember = "ID";
		this.gcFromDate.Caption = "From";
		this.gcFromDate.ColumnEdit = this.rep_FromDate;
		this.gcFromDate.FieldName = "FromDate";
		this.gcFromDate.Name = "gcFromDate";
		this.gcFromDate.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowForFocusedCell;
		this.gcFromDate.Visible = true;
		this.gcFromDate.VisibleIndex = 1;
		this.gcFromDate.Width = 70;
		this.rep_FromDate.AutoHeight = false;
		this.rep_FromDate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Down)
		});
		this.rep_FromDate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_FromDate.Name = "rep_FromDate";
		this.gcToDate.Caption = "To";
		this.gcToDate.ColumnEdit = this.rep_ToDate;
		this.gcToDate.FieldName = "ToDate";
		this.gcToDate.Name = "gcToDate";
		this.gcToDate.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowForFocusedCell;
		this.gcToDate.Visible = true;
		this.gcToDate.VisibleIndex = 2;
		this.gcToDate.Width = 70;
		this.rep_ToDate.AutoHeight = false;
		this.rep_ToDate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Down)
		});
		this.rep_ToDate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ToDate.Name = "rep_ToDate";
		this.gcItemCode.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.gcItemCode.AppearanceCell.Options.UseBackColor = true;
		this.gcItemCode.Caption = "Item No.";
		this.gcItemCode.FieldName = "ItemCode";
		this.gcItemCode.Name = "gcItemCode";
		this.gcItemCode.OptionsColumn.AllowEdit = false;
		this.gcItemCode.Visible = true;
		this.gcItemCode.VisibleIndex = 3;
		this.gcItemName.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.gcItemName.AppearanceCell.Options.UseBackColor = true;
		this.gcItemName.Caption = "Item Description";
		this.gcItemName.FieldName = "ItemName";
		this.gcItemName.Name = "gcItemName";
		this.gcItemName.OptionsColumn.AllowEdit = false;
		this.gcItemName.Visible = true;
		this.gcItemName.VisibleIndex = 4;
		this.gcItemName.Width = 120;
		this.gcCallUp.Caption = "Release";
		this.gcCallUp.FieldName = "CallUp";
		this.gcCallUp.Name = "gcCallUp";
		this.gcCallUp.Visible = true;
		this.gcCallUp.VisibleIndex = 5;
		this.gcCallUp.Width = 120;
		this.gcQuantity.Caption = "Quantity";
		this.gcQuantity.ColumnEdit = this.rep_Quantity;
		this.gcQuantity.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gcQuantity.FieldName = "Quantity";
		this.gcQuantity.Name = "gcQuantity";
		this.gcQuantity.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum)
		});
		this.gcQuantity.Visible = true;
		this.gcQuantity.VisibleIndex = 6;
		this.gcQuantity.Width = 60;
		this.rep_Quantity.AutoHeight = false;
		this.rep_Quantity.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.SpinDown, "", -1, true, true, true, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, "", null, null, true)
		});
		this.rep_Quantity.Name = "rep_Quantity";
		this.gcWhsCode.Caption = "Warehouse";
		this.gcWhsCode.ColumnEdit = this.rep_Warehouse;
		this.gcWhsCode.FieldName = "WhsCode";
		this.gcWhsCode.Name = "gcWhsCode";
		this.gcWhsCode.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowForFocusedCell;
		this.gcWhsCode.Visible = true;
		this.gcWhsCode.VisibleIndex = 7;
		this.gcWhsCode.Width = 70;
		this.rep_Warehouse.AutoHeight = false;
		this.rep_Warehouse.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_Warehouse.Name = "rep_Warehouse";
		this.rep_Warehouse.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Warehouse_ButtonClick);
		this.gcFreeTxt.Caption = "Free Text";
		this.gcFreeTxt.FieldName = "FreeTxt";
		this.gcFreeTxt.Name = "gcFreeTxt";
		this.gcFreeTxt.Visible = true;
		this.gcFreeTxt.VisibleIndex = 8;
		this.gcConsumeFCT.Caption = "Consume Forecast";
		this.gcConsumeFCT.ColumnEdit = this.rep_ConsumeFCT;
		this.gcConsumeFCT.FieldName = "ConsumeFCT";
		this.gcConsumeFCT.Name = "gcConsumeFCT";
		this.gcConsumeFCT.Visible = true;
		this.gcConsumeFCT.VisibleIndex = 9;
		this.gcConsumeFCT.Width = 50;
		this.rep_ConsumeFCT.AutoHeight = false;
		this.rep_ConsumeFCT.Name = "rep_ConsumeFCT";
		this.rep_ConsumeFCT.ValueChecked = "Y";
		this.rep_ConsumeFCT.ValueUnchecked = "N";
		this.gcActivity.Caption = "Activity";
		this.gcActivity.ColumnEdit = this.rep_Activity;
		this.gcActivity.Name = "gcActivity";
		this.gcActivity.OptionsColumn.ReadOnly = true;
		this.gcActivity.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcActivity.Visible = true;
		this.gcActivity.VisibleIndex = 10;
		this.gcActivity.Width = 60;
		this.rep_Activity.AutoHeight = false;
		this.rep_Activity.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, DevExpress.XtraEditors.ImageLocation.MiddleCenter, Apzon.Forms.Properties.Resources.right, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, "", null, null, true)
		});
		this.rep_Activity.Name = "rep_Activity";
		this.rep_Activity.ReadOnly = true;
		this.rep_Activity.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.rep_Activity.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Activity_ButtonClick);
		this.gcAmountLC.Caption = "AmountLC";
		this.gcAmountLC.FieldName = "AmountLC";
		this.gcAmountLC.Name = "gcAmountLC";
		this.gcAmountFC.Caption = "AmountFC";
		this.gcAmountFC.FieldName = "AmountFC";
		this.gcAmountFC.Name = "gcAmountFC";
		this.panelControl1.Controls.Add(this.btn_Cancel);
		this.panelControl1.Controls.Add(this.btn_OK);
		this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panelControl1.Location = new System.Drawing.Point(0, 228);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(590, 34);
		this.panelControl1.TabIndex = 1;
		this.btn_Cancel.Image = Apzon.Forms.Properties.Resources.cancel_16x16;
		this.btn_Cancel.Location = new System.Drawing.Point(88, 5);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 0;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_OK.Image = Apzon.Forms.Properties.Resources.apply_16x16;
		this.btn_OK.Location = new System.Drawing.Point(7, 5);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 0;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.panelControl2.Controls.Add(this.grd_Detail);
		this.panelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl2.Location = new System.Drawing.Point(0, 0);
		this.panelControl2.Name = "panelControl2";
		this.panelControl2.Size = new System.Drawing.Size(590, 228);
		this.panelControl2.TabIndex = 2;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(590, 262);
		base.Controls.Add(this.panelControl2);
		base.Controls.Add(this.panelControl1);
		base.Name = "frm_SalesBlanketAgreement_RowDetails";
		base.ShowIcon = false;
		this.Text = "Row Details - Blanket Agreement";
		base.Load += new System.EventHandler(this.frm_SalesBlanketAgreement_RowDetails_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_Detail).EndInit();
		this.contextMenuStrip_Detail.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Detail).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Frequency).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_FromDate.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_FromDate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ToDate.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ToDate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Quantity).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Warehouse).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ConsumeFCT).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Activity).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl2).EndInit();
		this.panelControl2.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
