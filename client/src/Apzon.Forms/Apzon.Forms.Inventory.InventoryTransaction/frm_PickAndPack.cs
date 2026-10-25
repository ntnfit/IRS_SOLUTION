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
using Apzon.Entities.Models.UserFields;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Menu;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Inventory.InventoryTransaction;

public class frm_PickAndPack : XtraFormEx
{
	private const string UdoCode = "OPKL";

	public string UdoTableName = "@OPKL";

	public DataTable UdoTable = new DataTable();

	private const string FormController = "PickAndPack";

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private List<UserFieldsImpl> lstUdf = new List<UserFieldsImpl>();

	private int maxDoc;

	private bool isPackDocument;

	private IContainer components;

	private SimpleButtonEx btn_Update;

	private TextEditEx txt_BarCode;

	private LabelLine lbl_BarCode;

	private TextEditEx txt_PackNo;

	private LabelLine lbl_PackingNo;

	private SimpleButtonEx btn_Cancel;

	private GridControlEx grd_Line;

	private PanelControl panelControl1;

	private PanelControl panelControl2;

	private PanelControl panelControl3;

	private GridViewEx gv_Line;

	private DateEditEx date_Doc;

	private LabelLine labelLine3;

	private TextEditEx txt_User;

	private LabelLine labelLine2;

	private SimpleButtonEx btn_Refresh;

	private ButtonEditEx bte_ProductionNo;

	private LabelLine labelLine1;

	private TextEditEx txt_DocumentBarcode;

	private LabelLine lbl_DocumentNo;

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
			btn_Update.Text = Apzon.Commons.Properties.Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btn_Update.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Update.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		case Enums.FormViewModeType.Find:
			btn_Update.Text = Apzon.Commons.Properties.Resources.ButtonFind;
			break;
		}
	}

	public frm_PickAndPack()
	{
		InitializeComponent();
	}

	private async void frm_PickAndPack_Load(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			FormMode = Enums.FormViewModeType.Add;
			maxDoc = await _httpClient.Udo.GetMaxId("OPKL") + 1;
			txt_PackNo.Text = Function.ToString(maxDoc);
			txt_User.Text = GlobalData.UserName;
			date_Doc.EditValue = DateTime.Today;
			txt_PackNo.Properties.ReadOnly = true;
			lstUdf = await _httpClient.Udf.GetListField("OBTN");
			UdoTable = await _httpClient.Udo.GetUdoByCode(new Common.Table_Code
			{
				Name = "OUDO",
				Code = "OPKL"
			});
			txt_BarCode.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void txt_BarCode_KeyDown(object sender, KeyEventArgs e)
	{
		_ = 1;
		try
		{
			if (e.KeyCode != Keys.Return || string.IsNullOrEmpty(txt_BarCode.Text) || FormMode != Enums.FormViewModeType.Add)
			{
				return;
			}
			txt_BarCode.Enabled = false;
			if (grd_Line.DataSource != null)
			{
				DataTable dataTable = grd_Line.DataSource as DataTable;
				DataRow[] array = dataTable.Select($"U_DistNumber='{txt_BarCode.Text}'");
				if (array.Length != 0)
				{
					DataRow[] array2 = array;
					foreach (DataRow dataRow in array2)
					{
						dataRow["Checked"] = true;
						gv_Line.FocusedRowHandle = dataTable.Rows.IndexOf(dataRow);
					}
					txt_BarCode.Text = string.Empty;
					txt_BarCode.Enabled = true;
					txt_BarCode.Focus();
					return;
				}
				if (isPackDocument)
				{
					XtraMessageBox.Show("Batch không có trong danh sách!", "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
				}
			}
			if (!isPackDocument)
			{
				WinCommons._OpenProcessing();
				DataSet ds = await GetBatchItem(txt_BarCode.Text);
				if (ds.IsHasStructure())
				{
					if (gv_Line.Columns.Count <= 0)
					{
						await GridViewHelper.GenerateGrid(grd_Line, gv_Line, ds, lstUdf);
						if (!ds.Tables[1].IsNotNull())
						{
							XtraMessageBox.Show("Không tìm thấy batch!", "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
						}
						else
						{
							FormMode = Enums.FormViewModeType.Add;
						}
					}
					else
					{
						DataTable dataSourceTable = grd_Line.DataSourceTable;
						if (dataSourceTable.IsNotNull() && ds.Tables[1].IsNotNull())
						{
							foreach (DataRow row in ds.Tables[1].Rows)
							{
								DataRow dataRow3 = dataSourceTable.NewRow();
								dataRow3.ItemArray = row.ItemArray;
								dataSourceTable.Rows.Add(dataRow3);
							}
							gv_Line.FocusedRowHandle = dataSourceTable.Rows.Count - 1;
							FormMode = Enums.FormViewModeType.Add;
						}
						else if (ds.Tables[1].IsNotNull())
						{
							grd_Line.DataSource = ds.Tables[1];
							grd_Line.RefreshDataSource();
							FormMode = Enums.FormViewModeType.Add;
						}
						else
						{
							XtraMessageBox.Show("Không tìm thấy batch!", "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
						}
					}
					WinCommons._CloseProcessing();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, "Không tìm thấy bản ghi nào.");
				}
				ds.Tables.Clear();
				ds.Dispose();
			}
			txt_BarCode.Text = string.Empty;
			txt_BarCode.Enabled = true;
			txt_BarCode.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_Update_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				return;
			}
			if (FormMode == Enums.FormViewModeType.Find)
			{
				LoadNavigate("S");
				return;
			}
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				return;
			}
			if (UdoTable == null || UdoTable.Rows.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.UdoTableIsInvalid, "");
				return;
			}
			WinCommons._OpenProcessing();
			DataTable dataSourceTable = grd_Line.DataSourceTable;
			DataSet dataSet = new DataSet();
			dataSet.Tables.Clear();
			if (!dataSourceTable.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.None, "Không có batch packing!");
				return;
			}
			if (dataSourceTable.Select("Checked='False'").IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.None, "Có batch chưa được check!");
				return;
			}
			string tableName = Function.ToString(UdoTable.Rows[0]["TableName"]);
			dataSet.Tables.Add(UdoTable.Copy());
			DataTable dataTable = new DataTable(tableName);
			dataTable.Columns.Add("DocEntry", typeof(int));
			dataTable.Columns.Add("U_DocDate", typeof(DateTime));
			DataRow dataRow = dataTable.NewRow();
			dataRow["DocEntry"] = 0;
			dataRow["U_DocDate"] = date_Doc.EditValue;
			dataTable.Rows.Add(dataRow);
			dataSet.Tables.Add(dataTable);
			DataTable dataTable2 = dataSourceTable.Copy();
			dataTable2.TableName = "PKL1";
			foreach (string item in (from DataColumn t in dataTable2.Columns
				where !t.ColumnName.Equals("DocEntry") && !t.ColumnName.Equals("LineNum") && !t.ColumnName.StartsWith("U_")
				select t.ColumnName).ToList())
			{
				dataTable2.Columns.Remove(item);
			}
			dataSet.Tables.Add(dataTable2);
			HttpResult check = await Save(dataSet);
			if (!string.IsNullOrEmpty(check.Message))
			{
				XtraMessageBox.Show(check.Message, "Error", MessageBoxButtons.OK);
			}
			else
			{
				WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				grd_Line.DataSource = null;
				grd_Line.RefreshDataSource();
				await WinFuntions.PrintLayout("", Function.ParseInt(check.Content), "OPKL");
				maxDoc = Function.ParseInt(check.Content) + 1;
				txt_PackNo.Text = maxDoc.ToString();
				WinCommons.Success(Enums.MessageCode.Success, "");
				txt_BarCode.Text = "";
				txt_BarCode.Focus();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void gv_Line_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.View)
			{
				GridViewMenu gridViewMenu = new GridViewMenu((GridView)sender);
				DXMenuItem dXMenuItem = new DXMenuItem
				{
					Caption = "Remove Line"
				};
				dXMenuItem.Click += lineMenu_Click;
				gridViewMenu.Items.Add(dXMenuItem);
				if (e.MenuType == GridMenuType.User || e.MenuType == GridMenuType.Row)
				{
					e.Allow = true;
					gridViewMenu.Show(e.Point);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void lineMenu_Click(object sender, EventArgs e)
	{
		try
		{
			if (!(sender as DXMenuItem).Caption.Equals("Remove Line"))
			{
				return;
			}
			GridView gridView = (GridView)grd_Line.MainView;
			if (gridView.FocusedRowHandle <= -1)
			{
				return;
			}
			if (int.Parse(gridView.GetFocusedRowCellValue("DocEntry").ToString()) > 0)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					gridView.DeleteRow(gridView.FocusedRowHandle);
				}
			}
			else
			{
				gridView.DeleteRow(gridView.FocusedRowHandle);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_Refresh_Click(object sender, EventArgs e)
	{
		try
		{
			if ((grd_Line.DataSource as DataTable).IsNotNull() && FormMode == Enums.FormViewModeType.Add && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				txt_BarCode.Focus();
				return;
			}
			txt_PackNo.Text = Function.ToString(maxDoc);
			txt_PackNo.Properties.ReadOnly = true;
			date_Doc.EditValue = DateTime.Today;
			FormMode = Enums.FormViewModeType.Add;
			grd_Line.DataSource = null;
			grd_Line.RefreshDataSource();
			isPackDocument = false;
			txt_BarCode.Text = "";
			txt_BarCode.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void bte_ProductionNo_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		_ = 1;
		try
		{
			if (string.IsNullOrEmpty(bte_ProductionNo.Text))
			{
				return;
			}
			string storeProcedure = "Apz_PickAndPack_GetBatchFromPO";
			int num = Function.ParseInt(bte_ProductionNo.EditValue);
			List<Common.ParameterValue> list = new List<Common.ParameterValue>();
			list.Add(new Common.ParameterValue("@docentry", num));
			list.Add(new Common.ParameterValue("@batch", ""));
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.UserDefinedProcedure, true, "", storeProcedure, Enums.ItemObjectType.NoItemType, list, "");
			frm_CFL2.ShowDialog(this);
			List<DataRow> drs = frm_CFL2.ChooseItems;
			if (drs.Count <= 0)
			{
				return;
			}
			DataTable dataSourceTable = grd_Line.DataSourceTable;
			if (dataSourceTable.Columns.Count > 0)
			{
				foreach (DataRow item in drs)
				{
					DataRow dataRow = dataSourceTable.NewRow();
					WinFuntions.DataRowToDataRow(item, dataRow);
					dataSourceTable.Rows.Add(dataRow);
				}
				grd_Line.RefreshDataSource();
				return;
			}
			DataSet ds = await GetBatchItem("");
			await GridViewHelper.GenerateGrid(grd_Line, gv_Line, ds, lstUdf);
			DataTable dataSourceTable2 = grd_Line.DataSourceTable;
			foreach (DataRow item2 in drs)
			{
				DataRow dataRow2 = dataSourceTable2.NewRow();
				WinFuntions.DataRowToDataRow(item2, dataRow2);
				dataSourceTable2.Rows.Add(dataRow2);
			}
			grd_Line.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void bte_ProductionNo_KeyDown(object sender, KeyEventArgs e)
	{
		_ = 1;
		try
		{
			if (e.KeyCode != Keys.Return || string.IsNullOrEmpty(bte_ProductionNo.Text))
			{
				return;
			}
			string storeProcedure = "Apz_PickAndPack_GetBatchFromPO";
			int num = Function.ParseInt(bte_ProductionNo.EditValue);
			List<Common.ParameterValue> list = new List<Common.ParameterValue>();
			list.Add(new Common.ParameterValue("@docentry", num));
			list.Add(new Common.ParameterValue("@batch", ""));
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.UserDefinedProcedure, true, "", storeProcedure, Enums.ItemObjectType.NoItemType, list, "");
			frm_CFL2.ShowDialog(this);
			List<DataRow> drs = frm_CFL2.ChooseItems;
			if (drs.Count <= 0)
			{
				return;
			}
			DataTable dt = grd_Line.DataSourceTable;
			if (dt.Columns.Count > 0)
			{
				foreach (DataRow item in drs)
				{
					DataRow dataRow = dt.NewRow();
					WinFuntions.DataRowToDataRow(item, dataRow);
					dt.Rows.Add(dataRow);
				}
				grd_Line.RefreshDataSource();
			}
			else
			{
				DataSet ds = await GetBatchItem("");
				await GridViewHelper.GenerateGrid(grd_Line, gv_Line, ds, lstUdf);
				DataTable dataSourceTable = grd_Line.DataSourceTable;
				foreach (DataRow item2 in drs)
				{
					DataRow dataRow2 = dataSourceTable.NewRow();
					WinFuntions.DataRowToDataRow(item2, dataRow2);
					dataSourceTable.Rows.Add(dataRow2);
				}
				grd_Line.RefreshDataSource();
			}
			gv_Line.FocusedRowHandle = dt.Rows.Count - 1;
			FormMode = Enums.FormViewModeType.Add;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void txt_DocumentBarcode_KeyDown(object sender, KeyEventArgs e)
	{
		_ = 1;
		try
		{
			if (e.KeyCode != Keys.Return || string.IsNullOrEmpty(Function.ToString(txt_DocumentBarcode.Text)))
			{
				return;
			}
			txt_DocumentBarcode.Enabled = false;
			WinCommons._OpenProcessing();
			DataSet ds = await GetBatchDocument(txt_DocumentBarcode.Text);
			if (ds.IsHasStructure())
			{
				if (gv_Line.Columns.Count <= 0)
				{
					await GridViewHelper.GenerateGrid(grd_Line, gv_Line, ds, lstUdf);
					if (!ds.Tables[1].IsNotNull())
					{
						XtraMessageBox.Show("Không tìm thấy batch!", "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
					}
					else
					{
						isPackDocument = true;
						FormMode = Enums.FormViewModeType.Add;
					}
				}
				else
				{
					DataTable dataSourceTable = grd_Line.DataSourceTable;
					if (dataSourceTable.IsNotNull() && ds.Tables[1].IsNotNull())
					{
						foreach (DataRow row in ds.Tables[1].Rows)
						{
							DataRow dataRow2 = dataSourceTable.NewRow();
							dataRow2.ItemArray = row.ItemArray;
							dataSourceTable.Rows.Add(dataRow2);
						}
						gv_Line.FocusedRowHandle = dataSourceTable.Rows.Count - 1;
						FormMode = Enums.FormViewModeType.Add;
						isPackDocument = true;
					}
					else if (ds.Tables[1].IsNotNull())
					{
						grd_Line.DataSource = ds.Tables[1];
						grd_Line.RefreshDataSource();
						FormMode = Enums.FormViewModeType.Add;
						isPackDocument = true;
					}
					else
					{
						XtraMessageBox.Show("Không tìm thấy batch!", "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
					}
				}
				WinCommons._CloseProcessing();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, "Không tìm thấy bản ghi nào.");
			}
			ds.Tables.Clear();
			ds.Dispose();
			txt_DocumentBarcode.Text = string.Empty;
			txt_DocumentBarcode.Enabled = true;
			txt_DocumentBarcode.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			XtraMessageBox.Show($"{new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5)} {ex.Message}");
			WinCommons._CloseProcessing();
		}
	}

	private async Task<HttpResult> Save(DataSet ds)
	{
		try
		{
			using HttpClientHelper<HttpResult> _help = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "PickAndPack");
			return await _help.PostAsync("Save", ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<DataSet> GetBatchItem(string barcode)
	{
		try
		{
			using HttpClientHelper<DataSet> hp = new HttpClientHelper<DataSet>(Configuration.ApiServer, "PickAndPack");
			return await hp.GetAsync("GetBatchItem", new { barcode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
	}

	private async Task<DataSet> GetRecord(int docEntry, string type)
	{
		try
		{
			using HttpClientHelper<DataSet> hp = new HttpClientHelper<DataSet>(Configuration.ApiServer, "PickAndPack");
			return await hp.GetAsync("GetRecord", new { docEntry, type });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
	}

	private async Task<DataSet> GetBatchDocument(string barcode)
	{
		try
		{
			using HttpClientHelper<DataSet> hp = new HttpClientHelper<DataSet>(Configuration.ApiServer, "PickAndPack");
			return await hp.GetAsync("GetBatchDocument", new { barcode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
	}

	private async void LoadNavigate(string type)
	{
		WinCommons._OpenProcessing();
		try
		{
			if (grd_Line.DataSourceTable.IsNotNull() && FormMode == Enums.FormViewModeType.Add && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				txt_BarCode.Focus();
				return;
			}
			int old_DocEntry = 0;
			if (!string.IsNullOrEmpty(txt_PackNo.Text))
			{
				old_DocEntry = Function.ParseInt(txt_PackNo.Text);
			}
			if (gv_Line.Columns.Count <= 0)
			{
				DataSet dataSet = await GetBatchItem("");
				if (dataSet.IsHasStructure())
				{
					dataSet.Tables[1].Rows.Clear();
					await GridViewHelper.GenerateGrid(grd_Line, gv_Line, dataSet, lstUdf);
				}
			}
			DataSet dataSet2 = await GetRecord(old_DocEntry, type);
			if (dataSet2.IsHasStructure() && dataSet2.Tables[0].IsNotNull() && dataSet2.Tables[1].IsNotNull())
			{
				txt_PackNo.Text = Function.ToString(dataSet2.Tables[0].Rows[0]["DocEntry"]);
				date_Doc.EditValue = Function.ParseDateTimes(dataSet2.Tables[0].Rows[0]["U_DocDate"]);
				grd_Line.DataSource = dataSet2.Tables[1];
				grd_Line.RefreshDataSource();
				FormMode = Enums.FormViewModeType.View;
				txt_PackNo.Properties.ReadOnly = true;
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, "Không tìm thấy bản ghi nào!");
				if (FormMode == Enums.FormViewModeType.Find)
				{
					txt_PackNo.Text = "";
					txt_PackNo.Focus();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_FIRST()
	{
		LoadNavigate("F");
	}

	public async Task MOVE_NEXT()
	{
		LoadNavigate("N");
	}

	public async Task MOVE_LAST()
	{
		LoadNavigate("L");
	}

	public async Task MOVE_PREVIOUS()
	{
		LoadNavigate("P");
	}

	public async Task MODE_FIND()
	{
		try
		{
			if (grd_Line.DataSourceTable.IsNotNull() && FormMode == Enums.FormViewModeType.Add && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				txt_BarCode.Focus();
				return;
			}
			grd_Line.DataSource = null;
			FormMode = Enums.FormViewModeType.Find;
			txt_PackNo.Text = string.Empty;
			txt_PackNo.Properties.ReadOnly = false;
			txt_PackNo.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH()
	{
		try
		{
			if ((grd_Line.DataSource as DataTable).IsNotNull() && FormMode == Enums.FormViewModeType.Add && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				txt_BarCode.Focus();
				return;
			}
			txt_PackNo.Text = Function.ToString(maxDoc);
			txt_PackNo.Properties.ReadOnly = true;
			date_Doc.EditValue = DateTime.Today;
			FormMode = Enums.FormViewModeType.Add;
			grd_Line.DataSource = null;
			grd_Line.RefreshDataSource();
			txt_BarCode.Text = "";
			txt_BarCode.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async void PRINTLAYOUT()
	{
		try
		{
			int num = Function.ParseInt(txt_PackNo.Text);
			if (FormMode == Enums.FormViewModeType.Add && num == maxDoc)
			{
				num--;
			}
			await WinFuntions.PrintLayout("", num, "OPKL");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task PRINT()
	{
		try
		{
			int num = Function.ParseInt(txt_PackNo.Text);
			if (FormMode == Enums.FormViewModeType.Add && num == maxDoc)
			{
				num--;
			}
			await WinFuntions.Print("", num, "OPKL");
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Inventory.InventoryTransaction.frm_PickAndPack));
		this.btn_Update = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Line = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Line = new Apzon.Controls.Grid.GridViewEx();
		this.txt_BarCode = new Apzon.Controls.Editor.TextEditEx();
		this.txt_PackNo = new Apzon.Controls.Editor.TextEditEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lbl_BarCode = new Apzon.Controls.Controls.LabelLine();
		this.lbl_PackingNo = new Apzon.Controls.Controls.LabelLine();
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
		this.txt_DocumentBarcode = new Apzon.Controls.Editor.TextEditEx();
		this.lbl_DocumentNo = new Apzon.Controls.Controls.LabelLine();
		this.bte_ProductionNo = new Apzon.Controls.Editor.ButtonEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.btn_Refresh = new Apzon.Controls.Controls.SimpleButtonEx();
		this.date_Doc = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.txt_User = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
		((System.ComponentModel.ISupportInitialize)this.grd_Line).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Line).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BarCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_PackNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl2).BeginInit();
		this.panelControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_DocumentBarcode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_ProductionNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_Doc.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_Doc.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_User.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl3).BeginInit();
		this.panelControl3.SuspendLayout();
		base.SuspendLayout();
		this.btn_Update.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Update.EditControl = null;
		this.btn_Update.EditValue = "OK";
		this.btn_Update.FieldName = null;
		this.btn_Update.ImageOptions.Image = Apzon.Forms.Properties.Resources.apply_16x16;
		this.btn_Update.IsSearchControl = false;
		this.btn_Update.Location = new System.Drawing.Point(12, 7);
		this.btn_Update.Name = "btn_Update";
		this.btn_Update.ReadOnly = false;
		this.btn_Update.SettingInVisible = false;
		this.btn_Update.SettingReadonly = false;
		this.btn_Update.Size = new System.Drawing.Size(75, 23);
		this.btn_Update.TabIndex = 18;
		this.btn_Update.Text = "OK";
		this.btn_Update.Click += new System.EventHandler(this.btn_Update_Click);
		this.grd_Line.AllowPasteExcel = true;
		this.grd_Line.ClipboardData = null;
		this.grd_Line.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Line.Location = new System.Drawing.Point(2, 2);
		this.grd_Line.MainView = this.gv_Line;
		this.grd_Line.Name = "grd_Line";
		this.grd_Line.Size = new System.Drawing.Size(797, 365);
		this.grd_Line.TabIndex = 0;
		this.grd_Line.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Line });
		this.gv_Line.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Line.ColumnKey = null;
		this.gv_Line.DisableFormSetting = false;
		this.gv_Line.DisplayException = false;
		this.gv_Line.EditColumn = null;
		this.gv_Line.FieldCurrency = null;
		this.gv_Line.FieldRate = null;
		this.gv_Line.FormId = "oPickAndPack";
		this.gv_Line.GridControl = this.grd_Line;
		this.gv_Line.IgnoreFilter = false;
		this.gv_Line.IndexEditRow = -1;
		this.gv_Line.IndicatorWidth = 35;
		this.gv_Line.IsApzTable = false;
		this.gv_Line.IsInsertNewRow = false;
		this.gv_Line.IsLoadingData = false;
		this.gv_Line.IsMappingContextAction = false;
		this.gv_Line.ItemId = "OPAP";
		this.gv_Line.LstColumnAllowBatch = null;
		this.gv_Line.LstColumnNotAllowBatch = null;
		this.gv_Line.LstColumnNotEditable = null;
		this.gv_Line.LstColumNotSetting = null;
		this.gv_Line.Name = "gv_Line";
		this.gv_Line.OptionsView.ShowGroupPanel = false;
		this.gv_Line.UseBatchMod = false;
		this.gv_Line.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gv_Line_PopupMenuShowing);
		this.txt_BarCode.EditControl = null;
		this.txt_BarCode.FieldName = null;
		this.txt_BarCode.IsSearchControl = false;
		this.txt_BarCode.Location = new System.Drawing.Point(92, 26);
		this.txt_BarCode.Name = "txt_BarCode";
		this.txt_BarCode.SettingInVisible = false;
		this.txt_BarCode.SettingReadonly = false;
		this.txt_BarCode.Size = new System.Drawing.Size(245, 20);
		this.txt_BarCode.TabIndex = 13;
		this.txt_BarCode.ToolTip = "Bắn barcode tem cần packing";
		this.txt_BarCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_BarCode_KeyDown);
		this.txt_PackNo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_PackNo.EditControl = null;
		this.txt_PackNo.FieldName = null;
		this.txt_PackNo.IsSearchControl = false;
		this.txt_PackNo.Location = new System.Drawing.Point(661, 2);
		this.txt_PackNo.Name = "txt_PackNo";
		this.txt_PackNo.Properties.DisplayFormat.FormatString = "n0";
		this.txt_PackNo.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txt_PackNo.Properties.EditFormat.FormatString = "n0";
		this.txt_PackNo.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txt_PackNo.Properties.Mask.EditMask = "n0";
		this.txt_PackNo.SettingInVisible = false;
		this.txt_PackNo.SettingReadonly = false;
		this.txt_PackNo.Size = new System.Drawing.Size(135, 20);
		this.txt_PackNo.TabIndex = 11;
		this.txt_PackNo.ToolTip = "Số packing";
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = Apzon.Forms.Properties.Resources.cancel_16x16;
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(91, 7);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 19;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.ToolTip = "Thoát";
		this.btn_Cancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.lbl_BarCode.EditControl = null;
		this.lbl_BarCode.EditValue = "Batch (Barcode)";
		this.lbl_BarCode.FieldName = null;
		this.lbl_BarCode.HideWhenEmpty = false;
		this.lbl_BarCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_BarCode.Location = new System.Drawing.Point(5, 28);
		this.lbl_BarCode.Name = "lbl_BarCode";
		this.lbl_BarCode.ReadOnly = false;
		this.lbl_BarCode.SettingInVisible = false;
		this.lbl_BarCode.SettingReadonly = false;
		this.lbl_BarCode.ShowUnderLine = true;
		this.lbl_BarCode.Size = new System.Drawing.Size(89, 20);
		this.lbl_BarCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_BarCode.TabIndex = 12;
		this.lbl_BarCode.Utext = "Batch (Barcode)";
		this.lbl_PackingNo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_PackingNo.EditControl = null;
		this.lbl_PackingNo.EditValue = "Packing No";
		this.lbl_PackingNo.FieldName = null;
		this.lbl_PackingNo.HideWhenEmpty = false;
		this.lbl_PackingNo.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_PackingNo.Location = new System.Drawing.Point(582, 5);
		this.lbl_PackingNo.Name = "lbl_PackingNo";
		this.lbl_PackingNo.ReadOnly = false;
		this.lbl_PackingNo.SettingInVisible = false;
		this.lbl_PackingNo.SettingReadonly = false;
		this.lbl_PackingNo.ShowUnderLine = true;
		this.lbl_PackingNo.Size = new System.Drawing.Size(80, 20);
		this.lbl_PackingNo.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_PackingNo.TabIndex = 10;
		this.lbl_PackingNo.Utext = "Packing No";
		this.panelControl1.Controls.Add(this.btn_Cancel);
		this.panelControl1.Controls.Add(this.btn_Update);
		this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panelControl1.Location = new System.Drawing.Point(0, 425);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(801, 35);
		this.panelControl1.TabIndex = 22;
		this.panelControl2.Controls.Add(this.txt_DocumentBarcode);
		this.panelControl2.Controls.Add(this.lbl_DocumentNo);
		this.panelControl2.Controls.Add(this.bte_ProductionNo);
		this.panelControl2.Controls.Add(this.labelLine1);
		this.panelControl2.Controls.Add(this.btn_Refresh);
		this.panelControl2.Controls.Add(this.date_Doc);
		this.panelControl2.Controls.Add(this.labelLine3);
		this.panelControl2.Controls.Add(this.txt_User);
		this.panelControl2.Controls.Add(this.labelLine2);
		this.panelControl2.Controls.Add(this.txt_PackNo);
		this.panelControl2.Controls.Add(this.lbl_PackingNo);
		this.panelControl2.Controls.Add(this.txt_BarCode);
		this.panelControl2.Controls.Add(this.lbl_BarCode);
		this.panelControl2.Dock = System.Windows.Forms.DockStyle.Top;
		this.panelControl2.Location = new System.Drawing.Point(0, 0);
		this.panelControl2.Name = "panelControl2";
		this.panelControl2.Size = new System.Drawing.Size(801, 56);
		this.panelControl2.TabIndex = 23;
		this.txt_DocumentBarcode.EditControl = null;
		this.txt_DocumentBarcode.FieldName = null;
		this.txt_DocumentBarcode.IsSearchControl = false;
		this.txt_DocumentBarcode.Location = new System.Drawing.Point(293, 2);
		this.txt_DocumentBarcode.Name = "txt_DocumentBarcode";
		this.txt_DocumentBarcode.SettingInVisible = false;
		this.txt_DocumentBarcode.SettingReadonly = false;
		this.txt_DocumentBarcode.Size = new System.Drawing.Size(125, 20);
		this.txt_DocumentBarcode.TabIndex = 62;
		this.txt_DocumentBarcode.ToolTip = "Bắn barcode phiếu hàng ra cổng";
		this.txt_DocumentBarcode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_DocumentBarcode_KeyDown);
		this.lbl_DocumentNo.EditControl = null;
		this.lbl_DocumentNo.EditValue = "Document No";
		this.lbl_DocumentNo.FieldName = null;
		this.lbl_DocumentNo.HideWhenEmpty = false;
		this.lbl_DocumentNo.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_DocumentNo.Location = new System.Drawing.Point(214, 3);
		this.lbl_DocumentNo.Name = "lbl_DocumentNo";
		this.lbl_DocumentNo.ReadOnly = false;
		this.lbl_DocumentNo.SettingInVisible = false;
		this.lbl_DocumentNo.SettingReadonly = false;
		this.lbl_DocumentNo.ShowUnderLine = true;
		this.lbl_DocumentNo.Size = new System.Drawing.Size(82, 20);
		this.lbl_DocumentNo.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_DocumentNo.TabIndex = 61;
		this.lbl_DocumentNo.Utext = "Document No";
		this.bte_ProductionNo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.bte_ProductionNo.EditControl = null;
		this.bte_ProductionNo.FieldName = null;
		this.bte_ProductionNo.IsSearchControl = false;
		this.bte_ProductionNo.Location = new System.Drawing.Point(494, 2);
		this.bte_ProductionNo.Name = "bte_ProductionNo";
		this.bte_ProductionNo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_ProductionNo.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_ProductionNo_Properties_ButtonClick);
		this.bte_ProductionNo.SettingInVisible = false;
		this.bte_ProductionNo.SettingReadonly = false;
		this.bte_ProductionNo.Size = new System.Drawing.Size(82, 20);
		this.bte_ProductionNo.TabIndex = 60;
		this.bte_ProductionNo.ToolTip = "Nhập số lệnh sản xuất để chọn batch";
		this.bte_ProductionNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.bte_ProductionNo_KeyDown);
		this.labelLine1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Production No";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(424, 3);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(70, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 59;
		this.labelLine1.Utext = "Production No";
		this.btn_Refresh.EditControl = null;
		this.btn_Refresh.EditValue = "Clear";
		this.btn_Refresh.FieldName = null;
		this.btn_Refresh.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Refresh.ImageOptions.Image");
		this.btn_Refresh.IsSearchControl = false;
		this.btn_Refresh.Location = new System.Drawing.Point(343, 24);
		this.btn_Refresh.Name = "btn_Refresh";
		this.btn_Refresh.ReadOnly = false;
		this.btn_Refresh.SettingInVisible = false;
		this.btn_Refresh.SettingReadonly = false;
		this.btn_Refresh.Size = new System.Drawing.Size(75, 23);
		this.btn_Refresh.TabIndex = 18;
		this.btn_Refresh.Text = "Clear";
		this.btn_Refresh.ToolTip = "Làm mới";
		this.btn_Refresh.Click += new System.EventHandler(this.btn_Refresh_Click);
		this.date_Doc.EditControl = null;
		this.date_Doc.EditValue = null;
		this.date_Doc.FieldName = null;
		this.date_Doc.Location = new System.Drawing.Point(91, 2);
		this.date_Doc.Name = "date_Doc";
		this.date_Doc.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_Doc.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_Doc.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.date_Doc.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_Doc.Properties.EditFormat.FormatString = "dd/MM/yyyy";
		this.date_Doc.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_Doc.Properties.IsSearchControl = false;
		this.date_Doc.Properties.Mask.EditMask = "dd/MM/yyyy";
		this.date_Doc.Properties.ReadOnly = true;
		this.date_Doc.SettingInVisible = false;
		this.date_Doc.SettingReadonly = false;
		this.date_Doc.Size = new System.Drawing.Size(117, 20);
		this.date_Doc.TabIndex = 17;
		this.date_Doc.ToolTip = "Ngày tạo";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Doc Date";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(5, 2);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(89, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 16;
		this.labelLine3.Utext = "Doc Date";
		this.txt_User.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_User.EditControl = null;
		this.txt_User.FieldName = null;
		this.txt_User.IsSearchControl = false;
		this.txt_User.Location = new System.Drawing.Point(661, 26);
		this.txt_User.Name = "txt_User";
		this.txt_User.Properties.ReadOnly = true;
		this.txt_User.SettingInVisible = false;
		this.txt_User.SettingReadonly = false;
		this.txt_User.Size = new System.Drawing.Size(135, 20);
		this.txt_User.TabIndex = 15;
		this.txt_User.ToolTip = "Username đăng nhập";
		this.labelLine2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "User";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(582, 28);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(80, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 14;
		this.labelLine2.Utext = "User";
		this.panelControl3.Controls.Add(this.grd_Line);
		this.panelControl3.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl3.Location = new System.Drawing.Point(0, 56);
		this.panelControl3.Name = "panelControl3";
		this.panelControl3.Size = new System.Drawing.Size(801, 369);
		this.panelControl3.TabIndex = 24;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(801, 460);
		base.Controls.Add(this.panelControl3);
		base.Controls.Add(this.panelControl2);
		base.Controls.Add(this.panelControl1);
		base.FormId = "oPickAndPack";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_PickAndPack";
		base.ShowInTaskbar = false;
		this.Text = "Pick and pack";
		base.Load += new System.EventHandler(this.frm_PickAndPack_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_Line).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Line).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BarCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_PackNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl2).EndInit();
		this.panelControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_DocumentBarcode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_ProductionNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_Doc.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_Doc.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_User.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl3).EndInit();
		this.panelControl3.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
