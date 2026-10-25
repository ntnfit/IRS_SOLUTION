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
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Administrator.SetUp.Inventory;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Administrator;

public class frm_ItemGroups : XtraFormEx
{
	private const string FormController = "ItemGroups";

	private DataTable _dtStructItemGroup;

	private List<Tuple<string, DataTable>> _lstWhsNBin = new List<Tuple<string, DataTable>>();

	private RepositoryItemButtonEditCFL bte;

	public short ChangedId;

	public bool IsOpenFromBP;

	private IContainer components;

	private GridControlEx grdDoc;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private GridViewEx gvDoc;

	public SimpleButtonEx btn_Save;

	public SimpleButtonEx btnCancel;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumn3;

	private GridColumnEx gridColumn4;

	private GridColumnEx gridColumn5;

	private GridColumnEx gridColumn6;

	private GridColumnEx gridColumn7;

	private GridColumnEx gridColumn8;

	private GridColumnEx gridColumn9;

	private GridColumnEx gridColumn10;

	private GridColumnEx gridColumn11;

	private GridColumnEx gridColumn13;

	private GridColumnEx gridColumn14;

	private GridColumnEx gridColumn15;

	private GridColumnEx gridColumn16;

	private GridColumnEx gridColumn17;

	private GridColumnEx gridColumn18;

	private GridColumnEx gridColumn19;

	private GridColumnEx gridColumn20;

	private GridColumnEx gridColumn21;

	private GridColumnEx gridColumn22;

	private GridColumnEx gridColumn23;

	private GridColumnEx gridColumn24;

	private GridColumnEx gridColumn25;

	private GridColumnEx gridColumn26;

	private GridColumnEx gridColumn27;

	private GridColumnEx gridColumn28;

	private GridColumnEx gridColumn29;

	private GridColumnEx gridColumn30;

	private GridColumnEx gridColumn31;

	private GridColumnEx gridColumn32;

	private GridColumnEx gridColumn33;

	private GridColumnEx gridColumn34;

	private GridColumnEx gridColumn35;

	private GridColumnEx gridColumn36;

	private GridColumnEx gridColumn37;

	private GridColumnEx gridColumn38;

	private GridColumnEx gridColumn39;

	private GridColumnEx gridColumn40;

	private GridColumnEx gridColumn41;

	private GridColumnEx gridColumn42;

	private GridColumnEx gridColumn43;

	private GridColumnEx gridColumn44;

	private GridColumnEx gridColumn45;

	private GridColumnEx gridColumn46;

	private GridColumnEx gridColumn47;

	private GridColumnEx gridColumn48;

	private GridColumnEx gridColumn49;

	private GridColumnEx gridColumn50;

	private GridColumnEx gridColumn51;

	private GridColumnEx gridColumn52;

	private GridColumnEx gridColumn53;

	private GridColumnEx gridColumn54;

	private GridColumnEx gridColumn55;

	private GridColumnEx gridColumn56;

	private GridColumnEx gridColumn57;

	private GridColumnEx gridColumn58;

	private GridColumnEx gridColumn59;

	private GridColumnEx gridColumn60;

	private GridColumnEx gridColumn61;

	private GridColumnEx gridColumn62;

	private GridColumnEx gridColumn63;

	private GridColumnEx gridColumn64;

	private GridColumnEx gridColumn65;

	private GridColumnEx gridColumn66;

	private GridColumnEx gridColumn67;

	private GridColumnEx gridColumn68;

	private GridColumnEx gridColumn69;

	private GridColumnEx gridColumn70;

	private GridColumnEx gridColumn71;

	private GridColumnEx gridColumn72;

	private GridColumnEx gridColumn73;

	private GridColumnEx gridColumn74;

	private GridColumnEx gridColumn75;

	private GridColumnEx gridColumn76;

	private GridColumnEx gridColumn77;

	private GridColumnEx gridColumn78;

	private GridColumnEx gridColumn79;

	private GridColumnEx gridColumn80;

	private GridColumnEx gridColumn81;

	private RepositoryItemLookUpEditCombo rep_PlaningSys;

	private RepositoryItemLookUpEditCombo rep_InvntSys;

	private RepositoryItemLookUpEditCombo rep_PrcrmntMtd;

	private RepositoryItemLookUpEditCombo rep_OrdrIntrvl;

	private GridColumnEx gridColumn12;

	private LabelLine labelLine1;

	private GridControlEx grd_Whs;

	private GridViewEx gv_Whs;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx12;

	private GridColumnEx gridColumnEx4;

	private GridColumnEx gridColumnEx5;

	private GridColumnEx gridColumnEx6;

	private GridColumnEx gridColumnEx7;

	private GridColumnEx gridColumnEx8;

	private GridColumnEx gridColumnEx9;

	private GridColumnEx gridColumnEx10;

	private GridColumnEx gridColumnEx11;

	private RepositoryItemCheckEdit rep_chk_DftBinEnfd;

	private SplitContainerControl splitContainerControl1;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			btnCancel.Enabled = true;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public async Task REFRESH_DATA()
	{
		await LoadData();
	}

	private async Task frm_ItemGroups_Load(object sender, EventArgs e)
	{
		_ = 3;
		try
		{
			_dtStructItemGroup = await WinFuntions.GetTableStructureAsync("OITB");
			await LoadInItForm();
			await LoadData();
			await LoadListWarehouseBinData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void Bte_Validating(object sender, CancelEventArgs e)
	{
		try
		{
			if (!e.Cancel)
			{
				string whsCode = Function.ToString(gv_Whs.GetFocusedRowCellValue("WhsCode"));
				ButtonEditCFL buttonEditCFL = sender as ButtonEditCFL;
				int binCode = Function.ParseInt(buttonEditCFL.EditValue);
				Tuple<string, DataTable> tuple = _lstWhsNBin.FirstOrDefault((Tuple<string, DataTable> t) => t.Item1.ToLower() == whsCode.ToLower());
				if (tuple != null && tuple.Item2 != null && binCode > 0 && !tuple.Item2.AsEnumerable().Any((DataRow s) => Function.ParseInt(s["AbsEntry"]) == binCode))
				{
					e.Cancel = true;
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
			int num = Function.ParseInt(gvDoc.GetFocusedRowCellValue("ItmsGrpCod"));
			if (num <= 0)
			{
				return;
			}
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "ItemGroups");
			string text = httpClientHelper.Post("DeleteDoc", num);
			if (!string.IsNullOrEmpty(text))
			{
				WinCommons.Error(Enums.MessageCode.None, text);
				return;
			}
			WinCommons.Success(Enums.MessageCode.RemoveSuccess, "");
			gvDoc.BeginUpdate();
			gvDoc.DeleteSelectedRows();
			gvDoc.EndUpdate();
			DataTable dataTable = (DataTable)grdDoc.DataSource;
			dataTable.AcceptChanges();
			grdDoc.DataSource = dataTable;
			grdDoc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btnCancel_Click(object sender, EventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
		{
			WinCommons._OpenProcessing();
			await SaveData();
			WinCommons._CloseProcessing();
		}
		Close();
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					WinCommons._OpenProcessing();
					await SaveData();
					WinCommons._CloseProcessing();
				}
			}
			else if (FormMode == Enums.FormViewModeType.View)
			{
				if (IsOpenFromBP)
				{
					ChangedId = Function.ParseShort(gvDoc.GetFocusedRowCellValue("ItmsGrpCod")).Value;
				}
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvDoc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void rep_OrdrIntrvl_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 1)
			{
				frm_InventoryCycles frm_InventoryCycles2 = new frm_InventoryCycles();
				frm_InventoryCycles2.MdiParent = MdiParent;
				frm_InventoryCycles2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvDoc_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				e.Allow = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvDoc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			int code = Function.ParseInt(gvDoc.GetFocusedRowCellValue("ItmsGrpCod"));
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemGroups");
			DataTable dataSource = httpClientHelper.Get("GetItemGroupWarehouse", new { code });
			grd_Whs.DataSource = dataSource;
			grd_Whs.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void frm_ItemGroups_Resize(object sender, EventArgs e)
	{
		if (GlobalData.LoginSap)
		{
			splitContainerControl1.SplitterPosition = splitContainerControl1.Height / 2;
		}
	}

	private void gv_Whs_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
			if (e.Column.FieldName == "DftBinAbs" && Function.ParseInt(e.Value) <= 0)
			{
				gv_Whs.SetFocusedRowCellValue("DftBinEnfd", "N");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Whs_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
	}

	private void gv_Whs_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
	}

	private void gv_Whs_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (gv_Whs.FocusedColumn.FieldName == "DftBinEnfd" && Function.ParseInt(gv_Whs.GetFocusedRowCellValue("DftBinAbs")) <= 0)
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

	public frm_ItemGroups()
	{
		InitializeComponent();
		OnLoadAsync += frm_ItemGroups_Load;
	}

	private async Task LoadInItForm()
	{
		try
		{
			rep_PlaningSys.SetDataSource(new string[2, 2]
			{
				{ "M", "MRP" },
				{ "N", "None" }
			}, isStringCode: true);
			rep_PrcrmntMtd.SetDataSource(new string[2, 2]
			{
				{ "B", "Buy" },
				{ "M", "Make" }
			}, isStringCode: true);
			using (HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Administrator"))
			{
				DataTable dataSource = await helper.GetAsync("GetCodeOcycItemGroups");
				rep_OrdrIntrvl.DataSource = dataSource;
			}
			rep_InvntSys.SetDataSource(new string[3, 2]
			{
				{ "A", "Moving Average" },
				{ "S", "Standard" },
				{ "F", "FIFO" }
			}, isStringCode: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadData()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemGroups");
			FormMode = Enums.FormViewModeType.View;
			DataTable dataSource = await helper.GetAsync("LoadDoc");
			grdDoc.DataSource = dataSource;
			grdDoc.RefreshDataSource();
			AddNew();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task SaveData()
	{
		try
		{
			DataTable dataTable = (grdDoc.DataSource as DataTable).Copy();
			DataRow[] array = (from t in dataTable.AsEnumerable()
				where !string.IsNullOrEmpty(Function.ToString(t["ItmsGrpNam"]))
				select t).ToArray();
			if (array.Length != 0)
			{
				dataTable = array.CopyToDataTable();
			}
			List<IGrouping<string, DataRow>> list = (from i in dataTable.AsEnumerable()
				group i by Function.ToString(i.Field<string>("ItmsGrpNam")) into gr
				where gr.Count() > 1
				select gr).ToList();
			if (list != null && list.Count > 0)
			{
				string text = Function.ToString(list[0].FirstOrDefault()["ItmsGrpNam"]);
				WinCommons.Error(Enums.MessageCode.DuplicateGroupName, "", text);
				return;
			}
			DataTable dataTable2 = dataTable.Clone();
			dataTable2.ImportRow(gvDoc.GetFocusedDataRow());
			dataTable2.AcceptChanges();
			DataTable dataTable3 = new DataTable();
			if (grd_Whs.DataSource is DataTable dataTable4 && dataTable4.Rows.Count > 0)
			{
				DataTable dataTable5 = dataTable4.Copy();
				dataTable5.Columns.Remove("WhsName");
				dataTable5.AcceptChanges();
				dataTable3 = dataTable5.Clone();
				for (int num = 0; num < dataTable5.Rows.Count; num++)
				{
					if (Function.ParseInt(dataTable5.Rows[num]["DftBinAbs"]) > 0)
					{
						dataTable3.ImportRow(dataTable5.Rows[num]);
					}
				}
				dataTable3.AcceptChanges();
			}
			DataSet dataSet = new DataSet();
			dataSet.Tables.AddRange(new DataTable[2] { dataTable2, dataTable3 });
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "ItemGroups");
			HttpResult httpResult = helper.Post("SaveOITB", dataSet);
			if (httpResult != null && string.IsNullOrEmpty(httpResult.Message))
			{
				if (IsOpenFromBP)
				{
					ChangedId = Function.ParseShort(gvDoc.GetFocusedRowCellValue("ItmsGrpCod")).Value;
					if (ChangedId == 0)
					{
						ChangedId = -1;
					}
					Close();
				}
				else
				{
					await LoadData();
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void AddNew()
	{
		try
		{
			DataTable dataTable = grdDoc.DataSource as DataTable;
			if (dataTable == null || dataTable.Columns.Count == 0)
			{
				dataTable = _dtStructItemGroup.Clone();
			}
			if (!dataTable.AsEnumerable().Any((DataRow r) => Function.ParseShort(r["ItmsGrpCod"]) == 0))
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["ItmsGrpCod"] = Convert.ToInt16(0);
				dataRow["Locked"] = "N";
				dataRow["PlaningSys"] = "N";
				dataRow["PrcrmntMtd"] = "B";
				dataRow["DataSource"] = "N";
				dataRow["Alert"] = "N";
				dataRow["Object"] = "52";
				dataRow["ItemClass"] = "2";
				dataRow["OSvcCode"] = "-1";
				dataRow["ISvcCode"] = "-1";
				dataRow["ServiceGrp"] = "-1";
				dataRow["NCMCode"] = "-1";
				dataRow["MatType"] = "1";
				dataRow["MatGrp"] = "-1";
				dataRow["ProductSrc"] = "0";
				dataRow["CompoWH"] = "B";
				dataRow["UserSign"] = GlobalData.UserID;
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				grdDoc.DataSource = dataTable;
				grdDoc.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private RepositoryItemButtonEditCFL CreateLookUpEditDfltBinAbs(CustomRowCellEditEventArgs e)
	{
		try
		{
			string whsCode = Function.ToString(gv_Whs.GetRowCellValue(e.RowHandle, "WhsCode"));
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = new RepositoryItemButtonEditCFL();
			repositoryItemButtonEditCFL.ValueMember = "AbsEntry";
			repositoryItemButtonEditCFL.DisplayMember = "BinCode";
			repositoryItemButtonEditCFL.Buttons.Clear();
			repositoryItemButtonEditCFL.Buttons.Add(new EditorButton(ButtonPredefines.Search));
			repositoryItemButtonEditCFL.TypeCFL = Enums.TypeCFL.BinOfItemGroupsWarehouse;
			repositoryItemButtonEditCFL.BeforeShowCFL = (Action<frm_CFL>)Delegate.Combine(repositoryItemButtonEditCFL.BeforeShowCFL, (Action<frm_CFL>)((frm_CFL frm) =>
			{
				frm.Params = new List<string> { whsCode };
			}));
			repositoryItemButtonEditCFL.RefreshDataSource();
			return repositoryItemButtonEditCFL;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new RepositoryItemButtonEditCFL();
		}
	}

	private async Task LoadListWarehouseBinData()
	{
		_ = 1;
		try
		{
			DataTable dtAllBin = new DataTable();
			DataTable dtStruct = new DataTable();
			using (HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemGroups"))
			{
				DataTable dt = await helper.GetAsync("GetItemGroupWarehouse", new
				{
					code = "-1"
				});
				if (dt != null && dt.Rows.Count > 0)
				{
					for (int i = 0; i < dt.Rows.Count; i++)
					{
						string whsCode = Function.ToString(dt.Rows[i]["WhsCode"]);
						DataSet dataSet = await HttpClientHelperFactory.GetHttpClient().Cfl.GetBinOfItemGroupsWarehouse(whsCode);
						if (dataSet == null || dataSet.Tables.Count != 2)
						{
							continue;
						}
						_lstWhsNBin.Add(new Tuple<string, DataTable>(whsCode, dataSet.Tables[1]));
						if (dtAllBin.Columns.Count == 0)
						{
							if (dataSet.Tables[1] != null)
							{
								dtAllBin = dataSet.Tables[1].Copy();
							}
						}
						else
						{
							for (int j = 0; j < dataSet.Tables[1].Rows.Count; j++)
							{
								dtAllBin.ImportRow(dataSet.Tables[1].Rows[j]);
							}
						}
						if (dtStruct.Columns.Count == 0 && dataSet.Tables[0] != null)
						{
							dtStruct = dataSet.Tables[0].Copy();
						}
					}
					dtAllBin.AcceptChanges();
				}
			}
			bte = new RepositoryItemButtonEditCFL
			{
				ValueMember = "AbsEntry",
				DisplayMember = "BinCode"
			};
			bte.Buttons.Clear();
			bte.Buttons.Add(new EditorButton(ButtonPredefines.Search));
			bte.TypeCFL = Enums.TypeCFL.BinOfItemGroupsWarehouse;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = bte;
			repositoryItemButtonEditCFL.BeforeShowCFL = (Action<frm_CFL>)Delegate.Combine(repositoryItemButtonEditCFL.BeforeShowCFL, (Action<frm_CFL>)((frm_CFL frm) =>
			{
				DataSet dataSet2 = new DataSet();
				dataSet2.Tables.AddRange(new DataTable[2]
				{
					dtStruct.Copy(),
					dtAllBin.Copy()
				});
				frm.DataSource = dataSet2;
			}));
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL2 = bte;
			repositoryItemButtonEditCFL2.AfterShowCFL = (Action<frm_CFL>)Delegate.Combine(repositoryItemButtonEditCFL2.AfterShowCFL, new Action<frm_CFL>(AfterShowCFL1));
			bte.Validating += Bte_Validating;
			bte.RefreshDataSource();
			gv_Whs.Columns["DftBinAbs"].ColumnEdit = bte;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void AfterShowCFL1(frm_CFL frm)
	{
		frm.gv_CFL.CustomRowFilter += Gv_CFL_CustomRowFilter;
	}

	private void Gv_CFL_CustomRowFilter(object sender, RowFilterEventArgs e)
	{
		GridView obj = sender as GridView;
		string whsCode = Function.ToString(gv_Whs.GetFocusedRowCellValue("WhsCode"));
		object listSourceRowCellValue = obj.GetListSourceRowCellValue(e.ListSourceRow, "AbsEntry");
		int binCode = Function.ParseInt(listSourceRowCellValue);
		Tuple<string, DataTable> tuple = _lstWhsNBin.FirstOrDefault((Tuple<string, DataTable> t) => t.Item1.ToLower() == whsCode.ToLower());
		if (tuple != null && tuple.Item2 != null && !tuple.Item2.AsEnumerable().Any((DataRow s) => Function.ParseInt(s["AbsEntry"]) == binCode))
		{
			e.Visible = false;
			e.Handled = true;
		}
	}

	private void grdDoc_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
			{
				ExtenseionMethods.PasteFromExcelToDataTable(grdDoc, gvDoc, new List<string> { "ItmsGrpNam" });
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.frm_ItemGroups));
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn11 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn12 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn13 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn14 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn15 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn16 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn17 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn18 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn19 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn20 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn21 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn22 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn23 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn24 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn25 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn26 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn27 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn28 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn29 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn30 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn31 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn32 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_InvntSys = new Apzon.Forms.UserControls.RepositoryItemLookUpEditCombo();
		this.gridColumn33 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_PlaningSys = new Apzon.Forms.UserControls.RepositoryItemLookUpEditCombo();
		this.gridColumn34 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_PrcrmntMtd = new Apzon.Forms.UserControls.RepositoryItemLookUpEditCombo();
		this.gridColumn35 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_OrdrIntrvl = new Apzon.Forms.UserControls.RepositoryItemLookUpEditCombo();
		this.gridColumn36 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn37 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn38 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn39 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn40 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn41 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn42 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn43 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn44 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn45 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn46 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn47 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn48 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn49 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn50 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn51 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn52 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn53 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn54 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn55 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn56 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn57 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn58 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn59 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn60 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn61 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn62 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn63 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn64 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn65 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn66 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn67 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn68 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn69 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn70 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn71 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn72 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn73 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn74 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn75 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn76 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn77 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn78 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn79 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn80 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn81 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.grd_Whs = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Whs = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx12 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_chk_DftBinEnfd = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx11 = new Apzon.Controls.Grid.GridColumnEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_InvntSys).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PlaningSys).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PrcrmntMtd).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_OrdrIntrvl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Whs).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Whs).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_chk_DftBinEnfd).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		base.SuspendLayout();
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.ContextMenuStrip = this.contextMenuStrip1;
		this.grdDoc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdDoc.Location = new System.Drawing.Point(0, 0);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.rep_PlaningSys, this.rep_PrcrmntMtd, this.rep_OrdrIntrvl, this.rep_InvntSys });
		this.grdDoc.Size = new System.Drawing.Size(749, 171);
		this.grdDoc.TabIndex = 80;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.grdDoc.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grdDoc_EditorKeyDown);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.deleteRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(134, 26);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[81]
		{
			this.gridColumn1, this.gridColumn2, this.gridColumn3, this.gridColumn4, this.gridColumn5, this.gridColumn6, this.gridColumn7, this.gridColumn8, this.gridColumn9, this.gridColumn10,
			this.gridColumn11, this.gridColumn12, this.gridColumn13, this.gridColumn14, this.gridColumn15, this.gridColumn16, this.gridColumn17, this.gridColumn18, this.gridColumn19, this.gridColumn20,
			this.gridColumn21, this.gridColumn22, this.gridColumn23, this.gridColumn24, this.gridColumn25, this.gridColumn26, this.gridColumn27, this.gridColumn28, this.gridColumn29, this.gridColumn30,
			this.gridColumn31, this.gridColumn32, this.gridColumn33, this.gridColumn34, this.gridColumn35, this.gridColumn36, this.gridColumn37, this.gridColumn38, this.gridColumn39, this.gridColumn40,
			this.gridColumn41, this.gridColumn42, this.gridColumn43, this.gridColumn44, this.gridColumn45, this.gridColumn46, this.gridColumn47, this.gridColumn48, this.gridColumn49, this.gridColumn50,
			this.gridColumn51, this.gridColumn52, this.gridColumn53, this.gridColumn54, this.gridColumn55, this.gridColumn56, this.gridColumn57, this.gridColumn58, this.gridColumn59, this.gridColumn60,
			this.gridColumn61, this.gridColumn62, this.gridColumn63, this.gridColumn64, this.gridColumn65, this.gridColumn66, this.gridColumn67, this.gridColumn68, this.gridColumn69, this.gridColumn70,
			this.gridColumn71, this.gridColumn72, this.gridColumn73, this.gridColumn74, this.gridColumn75, this.gridColumn76, this.gridColumn77, this.gridColumn78, this.gridColumn79, this.gridColumn80,
			this.gridColumn81
		});
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = "oItemGroups";
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 40;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.IsMappingContextAction = false;
		this.gvDoc.ItemId = "OITB";
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsView.ColumnAutoWidth = false;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gvDoc_FocusedRowObjectChanged);
		this.gvDoc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanged);
		this.gvDoc.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gvDoc_BeforeLeaveRow);
		this.gridColumn1.Caption = "gridColumn1";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "ItmsGrpCod";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.PrimaryKey = true;
		this.gridColumn1.UnitField = null;
		this.gridColumn2.Caption = "Item Group Name";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "ItmsGrpNam";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 0;
		this.gridColumn2.Width = 160;
		this.gridColumn3.Caption = "gridColumn3";
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "Locked";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn4.Caption = "gridColumn4";
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "DataSource";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn5.Caption = "gridColumn5";
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.FieldName = "UserSign";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.UnitField = null;
		this.gridColumn6.Caption = "gridColumn6";
		this.gridColumn6.CurrencyDateField = null;
		this.gridColumn6.CurrencyField = null;
		this.gridColumn6.CurrencyFixed = null;
		this.gridColumn6.CurrencyFixedRate = 0.0;
		this.gridColumn6.CurrencyRateField = null;
		this.gridColumn6.FieldName = "BalInvntAc";
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.PrimaryKey = false;
		this.gridColumn6.UnitField = null;
		this.gridColumn7.Caption = "gridColumn7";
		this.gridColumn7.CurrencyDateField = null;
		this.gridColumn7.CurrencyField = null;
		this.gridColumn7.CurrencyFixed = null;
		this.gridColumn7.CurrencyFixedRate = 0.0;
		this.gridColumn7.CurrencyRateField = null;
		this.gridColumn7.FieldName = "SaleCostAc";
		this.gridColumn7.Name = "gridColumn7";
		this.gridColumn7.PrimaryKey = false;
		this.gridColumn7.UnitField = null;
		this.gridColumn8.Caption = "gridColumn8";
		this.gridColumn8.CurrencyDateField = null;
		this.gridColumn8.CurrencyField = null;
		this.gridColumn8.CurrencyFixed = null;
		this.gridColumn8.CurrencyFixedRate = 0.0;
		this.gridColumn8.CurrencyRateField = null;
		this.gridColumn8.FieldName = "TransferAc";
		this.gridColumn8.Name = "gridColumn8";
		this.gridColumn8.PrimaryKey = false;
		this.gridColumn8.UnitField = null;
		this.gridColumn9.Caption = "gridColumn9";
		this.gridColumn9.CurrencyDateField = null;
		this.gridColumn9.CurrencyField = null;
		this.gridColumn9.CurrencyFixed = null;
		this.gridColumn9.CurrencyFixedRate = 0.0;
		this.gridColumn9.CurrencyRateField = null;
		this.gridColumn9.FieldName = "RevenuesAc";
		this.gridColumn9.Name = "gridColumn9";
		this.gridColumn9.PrimaryKey = false;
		this.gridColumn9.UnitField = null;
		this.gridColumn10.Caption = "gridColumn10";
		this.gridColumn10.CurrencyDateField = null;
		this.gridColumn10.CurrencyField = null;
		this.gridColumn10.CurrencyFixed = null;
		this.gridColumn10.CurrencyFixedRate = 0.0;
		this.gridColumn10.CurrencyRateField = null;
		this.gridColumn10.FieldName = "VarianceAc";
		this.gridColumn10.Name = "gridColumn10";
		this.gridColumn10.PrimaryKey = false;
		this.gridColumn10.UnitField = null;
		this.gridColumn11.Caption = "gridColumn11";
		this.gridColumn11.CurrencyDateField = null;
		this.gridColumn11.CurrencyField = null;
		this.gridColumn11.CurrencyFixed = null;
		this.gridColumn11.CurrencyFixedRate = 0.0;
		this.gridColumn11.CurrencyRateField = null;
		this.gridColumn11.FieldName = "DecreasAc";
		this.gridColumn11.Name = "gridColumn11";
		this.gridColumn11.PrimaryKey = false;
		this.gridColumn11.UnitField = null;
		this.gridColumn12.Caption = "gridColumn12";
		this.gridColumn12.CurrencyDateField = null;
		this.gridColumn12.CurrencyField = null;
		this.gridColumn12.CurrencyFixed = null;
		this.gridColumn12.CurrencyFixedRate = 0.0;
		this.gridColumn12.CurrencyRateField = null;
		this.gridColumn12.FieldName = "IncreasAc";
		this.gridColumn12.Name = "gridColumn12";
		this.gridColumn12.PrimaryKey = false;
		this.gridColumn12.UnitField = null;
		this.gridColumn13.Caption = "gridColumn13";
		this.gridColumn13.CurrencyDateField = null;
		this.gridColumn13.CurrencyField = null;
		this.gridColumn13.CurrencyFixed = null;
		this.gridColumn13.CurrencyFixedRate = 0.0;
		this.gridColumn13.CurrencyRateField = null;
		this.gridColumn13.FieldName = "ReturnAc";
		this.gridColumn13.Name = "gridColumn13";
		this.gridColumn13.PrimaryKey = false;
		this.gridColumn13.UnitField = null;
		this.gridColumn14.Caption = "gridColumn14";
		this.gridColumn14.CurrencyDateField = null;
		this.gridColumn14.CurrencyField = null;
		this.gridColumn14.CurrencyFixed = null;
		this.gridColumn14.CurrencyFixedRate = 0.0;
		this.gridColumn14.CurrencyRateField = null;
		this.gridColumn14.FieldName = "ExpensesAc";
		this.gridColumn14.Name = "gridColumn14";
		this.gridColumn14.PrimaryKey = false;
		this.gridColumn14.UnitField = null;
		this.gridColumn15.Caption = "gridColumn15";
		this.gridColumn15.CurrencyDateField = null;
		this.gridColumn15.CurrencyField = null;
		this.gridColumn15.CurrencyFixed = null;
		this.gridColumn15.CurrencyFixedRate = 0.0;
		this.gridColumn15.CurrencyRateField = null;
		this.gridColumn15.FieldName = "EURevenuAc";
		this.gridColumn15.Name = "gridColumn15";
		this.gridColumn15.PrimaryKey = false;
		this.gridColumn15.UnitField = null;
		this.gridColumn16.Caption = "gridColumn16";
		this.gridColumn16.CurrencyDateField = null;
		this.gridColumn16.CurrencyField = null;
		this.gridColumn16.CurrencyFixed = null;
		this.gridColumn16.CurrencyFixedRate = 0.0;
		this.gridColumn16.CurrencyRateField = null;
		this.gridColumn16.FieldName = "EUExpensAc";
		this.gridColumn16.Name = "gridColumn16";
		this.gridColumn16.PrimaryKey = false;
		this.gridColumn16.UnitField = null;
		this.gridColumn17.Caption = "gridColumn17";
		this.gridColumn17.CurrencyDateField = null;
		this.gridColumn17.CurrencyField = null;
		this.gridColumn17.CurrencyFixed = null;
		this.gridColumn17.CurrencyFixedRate = 0.0;
		this.gridColumn17.CurrencyRateField = null;
		this.gridColumn17.FieldName = "FrRevenuAc";
		this.gridColumn17.Name = "gridColumn17";
		this.gridColumn17.PrimaryKey = false;
		this.gridColumn17.UnitField = null;
		this.gridColumn18.Caption = "gridColumn18";
		this.gridColumn18.CurrencyDateField = null;
		this.gridColumn18.CurrencyField = null;
		this.gridColumn18.CurrencyFixed = null;
		this.gridColumn18.CurrencyFixedRate = 0.0;
		this.gridColumn18.CurrencyRateField = null;
		this.gridColumn18.FieldName = "FrExpensAc";
		this.gridColumn18.Name = "gridColumn18";
		this.gridColumn18.PrimaryKey = false;
		this.gridColumn18.UnitField = null;
		this.gridColumn19.Caption = "gridColumn19";
		this.gridColumn19.CurrencyDateField = null;
		this.gridColumn19.CurrencyField = null;
		this.gridColumn19.CurrencyFixed = null;
		this.gridColumn19.CurrencyFixedRate = 0.0;
		this.gridColumn19.CurrencyRateField = null;
		this.gridColumn19.FieldName = "ExmptIncom";
		this.gridColumn19.Name = "gridColumn19";
		this.gridColumn19.PrimaryKey = false;
		this.gridColumn19.UnitField = null;
		this.gridColumn20.Caption = "gridColumn20";
		this.gridColumn20.CurrencyDateField = null;
		this.gridColumn20.CurrencyField = null;
		this.gridColumn20.CurrencyFixed = null;
		this.gridColumn20.CurrencyFixedRate = 0.0;
		this.gridColumn20.CurrencyRateField = null;
		this.gridColumn20.FieldName = "CycleCode";
		this.gridColumn20.Name = "gridColumn20";
		this.gridColumn20.PrimaryKey = false;
		this.gridColumn20.UnitField = null;
		this.gridColumn21.Caption = "gridColumn21";
		this.gridColumn21.CurrencyDateField = null;
		this.gridColumn21.CurrencyField = null;
		this.gridColumn21.CurrencyFixed = null;
		this.gridColumn21.CurrencyFixedRate = 0.0;
		this.gridColumn21.CurrencyRateField = null;
		this.gridColumn21.FieldName = "Alert";
		this.gridColumn21.Name = "gridColumn21";
		this.gridColumn21.PrimaryKey = false;
		this.gridColumn21.UnitField = null;
		this.gridColumn22.Caption = "gridColumn22";
		this.gridColumn22.CurrencyDateField = null;
		this.gridColumn22.CurrencyField = null;
		this.gridColumn22.CurrencyFixed = null;
		this.gridColumn22.CurrencyFixedRate = 0.0;
		this.gridColumn22.CurrencyRateField = null;
		this.gridColumn22.FieldName = "PriceDifAc";
		this.gridColumn22.Name = "gridColumn22";
		this.gridColumn22.PrimaryKey = false;
		this.gridColumn22.UnitField = null;
		this.gridColumn23.Caption = "gridColumn23";
		this.gridColumn23.CurrencyDateField = null;
		this.gridColumn23.CurrencyField = null;
		this.gridColumn23.CurrencyFixed = null;
		this.gridColumn23.CurrencyFixedRate = 0.0;
		this.gridColumn23.CurrencyRateField = null;
		this.gridColumn23.FieldName = "ExchangeAc";
		this.gridColumn23.Name = "gridColumn23";
		this.gridColumn23.PrimaryKey = false;
		this.gridColumn23.UnitField = null;
		this.gridColumn24.Caption = "gridColumn24";
		this.gridColumn24.CurrencyDateField = null;
		this.gridColumn24.CurrencyField = null;
		this.gridColumn24.CurrencyFixed = null;
		this.gridColumn24.CurrencyFixedRate = 0.0;
		this.gridColumn24.CurrencyRateField = null;
		this.gridColumn24.FieldName = "BalanceAcc";
		this.gridColumn24.Name = "gridColumn24";
		this.gridColumn24.PrimaryKey = false;
		this.gridColumn24.UnitField = null;
		this.gridColumn25.Caption = "gridColumn25";
		this.gridColumn25.CurrencyDateField = null;
		this.gridColumn25.CurrencyField = null;
		this.gridColumn25.CurrencyFixed = null;
		this.gridColumn25.CurrencyFixedRate = 0.0;
		this.gridColumn25.CurrencyRateField = null;
		this.gridColumn25.FieldName = "PurchaseAc";
		this.gridColumn25.Name = "gridColumn25";
		this.gridColumn25.PrimaryKey = false;
		this.gridColumn25.UnitField = null;
		this.gridColumn26.Caption = "gridColumn26";
		this.gridColumn26.CurrencyDateField = null;
		this.gridColumn26.CurrencyField = null;
		this.gridColumn26.CurrencyFixed = null;
		this.gridColumn26.CurrencyFixedRate = 0.0;
		this.gridColumn26.CurrencyRateField = null;
		this.gridColumn26.FieldName = "PAReturnAc";
		this.gridColumn26.Name = "gridColumn26";
		this.gridColumn26.PrimaryKey = false;
		this.gridColumn26.UnitField = null;
		this.gridColumn27.Caption = "gridColumn27";
		this.gridColumn27.CurrencyDateField = null;
		this.gridColumn27.CurrencyField = null;
		this.gridColumn27.CurrencyFixed = null;
		this.gridColumn27.CurrencyFixedRate = 0.0;
		this.gridColumn27.CurrencyRateField = null;
		this.gridColumn27.FieldName = "PurchOfsAc";
		this.gridColumn27.Name = "gridColumn27";
		this.gridColumn27.PrimaryKey = false;
		this.gridColumn27.UnitField = null;
		this.gridColumn28.Caption = "gridColumn28";
		this.gridColumn28.CurrencyDateField = null;
		this.gridColumn28.CurrencyField = null;
		this.gridColumn28.CurrencyFixed = null;
		this.gridColumn28.CurrencyFixedRate = 0.0;
		this.gridColumn28.CurrencyRateField = null;
		this.gridColumn28.FieldName = "ShpdGdsAct";
		this.gridColumn28.Name = "gridColumn28";
		this.gridColumn28.PrimaryKey = false;
		this.gridColumn28.UnitField = null;
		this.gridColumn29.Caption = "gridColumn29";
		this.gridColumn29.CurrencyDateField = null;
		this.gridColumn29.CurrencyField = null;
		this.gridColumn29.CurrencyFixed = null;
		this.gridColumn29.CurrencyFixedRate = 0.0;
		this.gridColumn29.CurrencyRateField = null;
		this.gridColumn29.FieldName = "VatRevAct";
		this.gridColumn29.Name = "gridColumn29";
		this.gridColumn29.PrimaryKey = false;
		this.gridColumn29.UnitField = null;
		this.gridColumn30.Caption = "gridColumn30";
		this.gridColumn30.CurrencyDateField = null;
		this.gridColumn30.CurrencyField = null;
		this.gridColumn30.CurrencyFixed = null;
		this.gridColumn30.CurrencyFixedRate = 0.0;
		this.gridColumn30.CurrencyRateField = null;
		this.gridColumn30.FieldName = "DecresGlAc";
		this.gridColumn30.Name = "gridColumn30";
		this.gridColumn30.PrimaryKey = false;
		this.gridColumn30.UnitField = null;
		this.gridColumn31.Caption = "gridColumn31";
		this.gridColumn31.CurrencyDateField = null;
		this.gridColumn31.CurrencyField = null;
		this.gridColumn31.CurrencyFixed = null;
		this.gridColumn31.CurrencyFixedRate = 0.0;
		this.gridColumn31.CurrencyRateField = null;
		this.gridColumn31.FieldName = "IncresGlAc";
		this.gridColumn31.Name = "gridColumn31";
		this.gridColumn31.PrimaryKey = false;
		this.gridColumn31.UnitField = null;
		this.gridColumn32.Caption = "Default Valuation Method";
		this.gridColumn32.ColumnEdit = this.rep_InvntSys;
		this.gridColumn32.CurrencyDateField = null;
		this.gridColumn32.CurrencyField = null;
		this.gridColumn32.CurrencyFixed = null;
		this.gridColumn32.CurrencyFixedRate = 0.0;
		this.gridColumn32.CurrencyRateField = null;
		this.gridColumn32.FieldName = "InvntSys";
		this.gridColumn32.Name = "gridColumn32";
		this.gridColumn32.PrimaryKey = false;
		this.gridColumn32.UnitField = null;
		this.gridColumn32.Visible = true;
		this.gridColumn32.VisibleIndex = 1;
		this.gridColumn32.Width = 138;
		this.rep_InvntSys.AutoHeight = false;
		this.rep_InvntSys.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.rep_InvntSys.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_InvntSys.DisplayMember = "Name";
		this.rep_InvntSys.IsSearchControl = false;
		this.rep_InvntSys.Name = "rep_InvntSys";
		this.rep_InvntSys.NullText = "";
		this.rep_InvntSys.ShowFooter = false;
		this.rep_InvntSys.ShowHeader = false;
		this.rep_InvntSys.ValueMember = "Code";
		this.gridColumn33.Caption = "Planning Method";
		this.gridColumn33.ColumnEdit = this.rep_PlaningSys;
		this.gridColumn33.CurrencyDateField = null;
		this.gridColumn33.CurrencyField = null;
		this.gridColumn33.CurrencyFixed = null;
		this.gridColumn33.CurrencyFixedRate = 0.0;
		this.gridColumn33.CurrencyRateField = null;
		this.gridColumn33.FieldName = "PlaningSys";
		this.gridColumn33.Name = "gridColumn33";
		this.gridColumn33.PrimaryKey = false;
		this.gridColumn33.UnitField = null;
		this.gridColumn33.Visible = true;
		this.gridColumn33.VisibleIndex = 2;
		this.gridColumn33.Width = 169;
		this.rep_PlaningSys.AutoHeight = false;
		this.rep_PlaningSys.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.rep_PlaningSys.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_PlaningSys.DisplayMember = "Name";
		this.rep_PlaningSys.IsSearchControl = false;
		this.rep_PlaningSys.Name = "rep_PlaningSys";
		this.rep_PlaningSys.NullText = "";
		this.rep_PlaningSys.ShowFooter = false;
		this.rep_PlaningSys.ShowHeader = false;
		this.rep_PlaningSys.ValueMember = "Code";
		this.gridColumn34.Caption = "Procurement Method";
		this.gridColumn34.ColumnEdit = this.rep_PrcrmntMtd;
		this.gridColumn34.CurrencyDateField = null;
		this.gridColumn34.CurrencyField = null;
		this.gridColumn34.CurrencyFixed = null;
		this.gridColumn34.CurrencyFixedRate = 0.0;
		this.gridColumn34.CurrencyRateField = null;
		this.gridColumn34.FieldName = "PrcrmntMtd";
		this.gridColumn34.Name = "gridColumn34";
		this.gridColumn34.PrimaryKey = false;
		this.gridColumn34.UnitField = null;
		this.gridColumn34.Visible = true;
		this.gridColumn34.VisibleIndex = 3;
		this.gridColumn34.Width = 136;
		this.rep_PrcrmntMtd.AutoHeight = false;
		this.rep_PrcrmntMtd.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.rep_PrcrmntMtd.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_PrcrmntMtd.DisplayMember = "Name";
		this.rep_PrcrmntMtd.IsSearchControl = false;
		this.rep_PrcrmntMtd.Name = "rep_PrcrmntMtd";
		this.rep_PrcrmntMtd.NullText = "";
		this.rep_PrcrmntMtd.ShowFooter = false;
		this.rep_PrcrmntMtd.ShowHeader = false;
		this.rep_PrcrmntMtd.ValueMember = "Code";
		this.gridColumn35.Caption = "Order Interval";
		this.gridColumn35.ColumnEdit = this.rep_OrdrIntrvl;
		this.gridColumn35.CurrencyDateField = null;
		this.gridColumn35.CurrencyField = null;
		this.gridColumn35.CurrencyFixed = null;
		this.gridColumn35.CurrencyFixedRate = 0.0;
		this.gridColumn35.CurrencyRateField = null;
		this.gridColumn35.FieldName = "OrdrIntrvl";
		this.gridColumn35.Name = "gridColumn35";
		this.gridColumn35.PrimaryKey = false;
		this.gridColumn35.UnitField = null;
		this.gridColumn35.Visible = true;
		this.gridColumn35.VisibleIndex = 4;
		this.gridColumn35.Width = 123;
		this.rep_OrdrIntrvl.AutoHeight = false;
		this.rep_OrdrIntrvl.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_OrdrIntrvl.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_OrdrIntrvl.DisplayMember = "Name";
		this.rep_OrdrIntrvl.IsSearchControl = false;
		this.rep_OrdrIntrvl.Name = "rep_OrdrIntrvl";
		this.rep_OrdrIntrvl.NullText = "";
		this.rep_OrdrIntrvl.ShowFooter = false;
		this.rep_OrdrIntrvl.ShowHeader = false;
		this.rep_OrdrIntrvl.ValueMember = "Code";
		this.rep_OrdrIntrvl.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_OrdrIntrvl_ButtonPressed);
		this.gridColumn36.Caption = "Order Multiple";
		this.gridColumn36.CurrencyDateField = null;
		this.gridColumn36.CurrencyField = null;
		this.gridColumn36.CurrencyFixed = null;
		this.gridColumn36.CurrencyFixedRate = 0.0;
		this.gridColumn36.CurrencyRateField = null;
		this.gridColumn36.FieldName = "OrdrMulti";
		this.gridColumn36.Name = "gridColumn36";
		this.gridColumn36.PrimaryKey = false;
		this.gridColumn36.UnitField = null;
		this.gridColumn36.Visible = true;
		this.gridColumn36.VisibleIndex = 5;
		this.gridColumn36.Width = 133;
		this.gridColumn37.Caption = "Minimum Order Qty";
		this.gridColumn37.CurrencyDateField = null;
		this.gridColumn37.CurrencyField = null;
		this.gridColumn37.CurrencyFixed = null;
		this.gridColumn37.CurrencyFixedRate = 0.0;
		this.gridColumn37.CurrencyRateField = null;
		this.gridColumn37.FieldName = "MinOrdrQty";
		this.gridColumn37.Name = "gridColumn37";
		this.gridColumn37.PrimaryKey = false;
		this.gridColumn37.UnitField = null;
		this.gridColumn37.Visible = true;
		this.gridColumn37.VisibleIndex = 6;
		this.gridColumn37.Width = 142;
		this.gridColumn38.Caption = "Lead Time";
		this.gridColumn38.CurrencyDateField = null;
		this.gridColumn38.CurrencyField = null;
		this.gridColumn38.CurrencyFixed = null;
		this.gridColumn38.CurrencyFixedRate = 0.0;
		this.gridColumn38.CurrencyRateField = null;
		this.gridColumn38.FieldName = "LeadTime";
		this.gridColumn38.Name = "gridColumn38";
		this.gridColumn38.PrimaryKey = false;
		this.gridColumn38.UnitField = null;
		this.gridColumn38.Visible = true;
		this.gridColumn38.VisibleIndex = 7;
		this.gridColumn38.Width = 147;
		this.gridColumn39.Caption = "gridColumn39";
		this.gridColumn39.CurrencyDateField = null;
		this.gridColumn39.CurrencyField = null;
		this.gridColumn39.CurrencyFixed = null;
		this.gridColumn39.CurrencyFixedRate = 0.0;
		this.gridColumn39.CurrencyRateField = null;
		this.gridColumn39.FieldName = "StokRvlAct";
		this.gridColumn39.Name = "gridColumn39";
		this.gridColumn39.PrimaryKey = false;
		this.gridColumn39.UnitField = null;
		this.gridColumn40.Caption = "gridColumn40";
		this.gridColumn40.CurrencyDateField = null;
		this.gridColumn40.CurrencyField = null;
		this.gridColumn40.CurrencyFixed = null;
		this.gridColumn40.CurrencyFixedRate = 0.0;
		this.gridColumn40.CurrencyRateField = null;
		this.gridColumn40.FieldName = "StkOffsAct";
		this.gridColumn40.Name = "gridColumn40";
		this.gridColumn40.PrimaryKey = false;
		this.gridColumn40.UnitField = null;
		this.gridColumn41.Caption = "gridColumn41";
		this.gridColumn41.CurrencyDateField = null;
		this.gridColumn41.CurrencyField = null;
		this.gridColumn41.CurrencyFixed = null;
		this.gridColumn41.CurrencyFixedRate = 0.0;
		this.gridColumn41.CurrencyRateField = null;
		this.gridColumn41.FieldName = "WipAcct";
		this.gridColumn41.Name = "gridColumn41";
		this.gridColumn41.PrimaryKey = false;
		this.gridColumn41.UnitField = null;
		this.gridColumn42.Caption = "gridColumn42";
		this.gridColumn42.CurrencyDateField = null;
		this.gridColumn42.CurrencyField = null;
		this.gridColumn42.CurrencyFixed = null;
		this.gridColumn42.CurrencyFixedRate = 0.0;
		this.gridColumn42.CurrencyRateField = null;
		this.gridColumn42.FieldName = "WipVarAcct";
		this.gridColumn42.Name = "gridColumn42";
		this.gridColumn42.PrimaryKey = false;
		this.gridColumn42.UnitField = null;
		this.gridColumn43.Caption = "gridColumn43";
		this.gridColumn43.CurrencyDateField = null;
		this.gridColumn43.CurrencyField = null;
		this.gridColumn43.CurrencyFixed = null;
		this.gridColumn43.CurrencyFixedRate = 0.0;
		this.gridColumn43.CurrencyRateField = null;
		this.gridColumn43.FieldName = "CostRvlAct";
		this.gridColumn43.Name = "gridColumn43";
		this.gridColumn43.PrimaryKey = false;
		this.gridColumn43.UnitField = null;
		this.gridColumn44.Caption = "gridColumn44";
		this.gridColumn44.CurrencyDateField = null;
		this.gridColumn44.CurrencyField = null;
		this.gridColumn44.CurrencyFixed = null;
		this.gridColumn44.CurrencyFixedRate = 0.0;
		this.gridColumn44.CurrencyRateField = null;
		this.gridColumn44.FieldName = "CstOffsAct";
		this.gridColumn44.Name = "gridColumn44";
		this.gridColumn44.PrimaryKey = false;
		this.gridColumn44.UnitField = null;
		this.gridColumn45.Caption = "gridColumn45";
		this.gridColumn45.CurrencyDateField = null;
		this.gridColumn45.CurrencyField = null;
		this.gridColumn45.CurrencyFixed = null;
		this.gridColumn45.CurrencyFixedRate = 0.0;
		this.gridColumn45.CurrencyRateField = null;
		this.gridColumn45.FieldName = "ExpClrAct";
		this.gridColumn45.Name = "gridColumn45";
		this.gridColumn45.PrimaryKey = false;
		this.gridColumn45.UnitField = null;
		this.gridColumn46.Caption = "gridColumn46";
		this.gridColumn46.CurrencyDateField = null;
		this.gridColumn46.CurrencyField = null;
		this.gridColumn46.CurrencyFixed = null;
		this.gridColumn46.CurrencyFixedRate = 0.0;
		this.gridColumn46.CurrencyRateField = null;
		this.gridColumn46.FieldName = "ExpOfstAct";
		this.gridColumn46.Name = "gridColumn46";
		this.gridColumn46.PrimaryKey = false;
		this.gridColumn46.UnitField = null;
		this.gridColumn47.Caption = "gridColumn47";
		this.gridColumn47.CurrencyDateField = null;
		this.gridColumn47.CurrencyField = null;
		this.gridColumn47.CurrencyFixed = null;
		this.gridColumn47.CurrencyFixedRate = 0.0;
		this.gridColumn47.CurrencyRateField = null;
		this.gridColumn47.FieldName = "Object";
		this.gridColumn47.Name = "gridColumn47";
		this.gridColumn47.PrimaryKey = false;
		this.gridColumn47.UnitField = null;
		this.gridColumn48.Caption = "gridColumn48";
		this.gridColumn48.CurrencyDateField = null;
		this.gridColumn48.CurrencyField = null;
		this.gridColumn48.CurrencyFixed = null;
		this.gridColumn48.CurrencyFixedRate = 0.0;
		this.gridColumn48.CurrencyRateField = null;
		this.gridColumn48.FieldName = "logInstanc";
		this.gridColumn48.Name = "gridColumn48";
		this.gridColumn48.PrimaryKey = false;
		this.gridColumn48.UnitField = null;
		this.gridColumn49.Caption = "gridColumn49";
		this.gridColumn49.CurrencyDateField = null;
		this.gridColumn49.CurrencyField = null;
		this.gridColumn49.CurrencyFixed = null;
		this.gridColumn49.CurrencyFixedRate = 0.0;
		this.gridColumn49.CurrencyRateField = null;
		this.gridColumn49.FieldName = "createDate";
		this.gridColumn49.Name = "gridColumn49";
		this.gridColumn49.PrimaryKey = false;
		this.gridColumn49.UnitField = null;
		this.gridColumn50.Caption = "gridColumn50";
		this.gridColumn50.CurrencyDateField = null;
		this.gridColumn50.CurrencyField = null;
		this.gridColumn50.CurrencyFixed = null;
		this.gridColumn50.CurrencyFixedRate = 0.0;
		this.gridColumn50.CurrencyRateField = null;
		this.gridColumn50.FieldName = "userSign2";
		this.gridColumn50.Name = "gridColumn50";
		this.gridColumn50.PrimaryKey = false;
		this.gridColumn50.UnitField = null;
		this.gridColumn51.Caption = "gridColumn51";
		this.gridColumn51.CurrencyDateField = null;
		this.gridColumn51.CurrencyField = null;
		this.gridColumn51.CurrencyFixed = null;
		this.gridColumn51.CurrencyFixedRate = 0.0;
		this.gridColumn51.CurrencyRateField = null;
		this.gridColumn51.FieldName = "updateDate";
		this.gridColumn51.Name = "gridColumn51";
		this.gridColumn51.PrimaryKey = false;
		this.gridColumn51.UnitField = null;
		this.gridColumn52.Caption = "gridColumn52";
		this.gridColumn52.CurrencyDateField = null;
		this.gridColumn52.CurrencyField = null;
		this.gridColumn52.CurrencyFixed = null;
		this.gridColumn52.CurrencyFixedRate = 0.0;
		this.gridColumn52.CurrencyRateField = null;
		this.gridColumn52.FieldName = "ARCMAct";
		this.gridColumn52.Name = "gridColumn52";
		this.gridColumn52.PrimaryKey = false;
		this.gridColumn52.UnitField = null;
		this.gridColumn53.Caption = "gridColumn53";
		this.gridColumn53.CurrencyDateField = null;
		this.gridColumn53.CurrencyField = null;
		this.gridColumn53.CurrencyFixed = null;
		this.gridColumn53.CurrencyFixedRate = 0.0;
		this.gridColumn53.CurrencyRateField = null;
		this.gridColumn53.FieldName = "ARCMFrnAct";
		this.gridColumn53.Name = "gridColumn53";
		this.gridColumn53.PrimaryKey = false;
		this.gridColumn53.UnitField = null;
		this.gridColumn54.Caption = "gridColumn54";
		this.gridColumn54.CurrencyDateField = null;
		this.gridColumn54.CurrencyField = null;
		this.gridColumn54.CurrencyFixed = null;
		this.gridColumn54.CurrencyFixedRate = 0.0;
		this.gridColumn54.CurrencyRateField = null;
		this.gridColumn54.FieldName = "ARCMEUAct";
		this.gridColumn54.Name = "gridColumn54";
		this.gridColumn54.PrimaryKey = false;
		this.gridColumn54.UnitField = null;
		this.gridColumn55.Caption = "gridColumn55";
		this.gridColumn55.CurrencyDateField = null;
		this.gridColumn55.CurrencyField = null;
		this.gridColumn55.CurrencyFixed = null;
		this.gridColumn55.CurrencyFixedRate = 0.0;
		this.gridColumn55.CurrencyRateField = null;
		this.gridColumn55.FieldName = "ARCMExpAct";
		this.gridColumn55.Name = "gridColumn55";
		this.gridColumn55.PrimaryKey = false;
		this.gridColumn55.UnitField = null;
		this.gridColumn56.Caption = "gridColumn56";
		this.gridColumn56.CurrencyDateField = null;
		this.gridColumn56.CurrencyField = null;
		this.gridColumn56.CurrencyFixed = null;
		this.gridColumn56.CurrencyFixedRate = 0.0;
		this.gridColumn56.CurrencyRateField = null;
		this.gridColumn56.FieldName = "APCMAct";
		this.gridColumn56.Name = "gridColumn56";
		this.gridColumn56.PrimaryKey = false;
		this.gridColumn56.UnitField = null;
		this.gridColumn57.Caption = "gridColumn57";
		this.gridColumn57.CurrencyDateField = null;
		this.gridColumn57.CurrencyField = null;
		this.gridColumn57.CurrencyFixed = null;
		this.gridColumn57.CurrencyFixedRate = 0.0;
		this.gridColumn57.CurrencyRateField = null;
		this.gridColumn57.FieldName = "APCMFrnAct";
		this.gridColumn57.Name = "gridColumn57";
		this.gridColumn57.PrimaryKey = false;
		this.gridColumn57.UnitField = null;
		this.gridColumn58.Caption = "gridColumn58";
		this.gridColumn58.CurrencyDateField = null;
		this.gridColumn58.CurrencyField = null;
		this.gridColumn58.CurrencyFixed = null;
		this.gridColumn58.CurrencyFixedRate = 0.0;
		this.gridColumn58.CurrencyRateField = null;
		this.gridColumn58.FieldName = "APCMEUAct";
		this.gridColumn58.Name = "gridColumn58";
		this.gridColumn58.PrimaryKey = false;
		this.gridColumn58.UnitField = null;
		this.gridColumn59.Caption = "gridColumn59";
		this.gridColumn59.CurrencyDateField = null;
		this.gridColumn59.CurrencyField = null;
		this.gridColumn59.CurrencyFixed = null;
		this.gridColumn59.CurrencyFixedRate = 0.0;
		this.gridColumn59.CurrencyRateField = null;
		this.gridColumn59.FieldName = "RevRetAct";
		this.gridColumn59.Name = "gridColumn59";
		this.gridColumn59.PrimaryKey = false;
		this.gridColumn59.UnitField = null;
		this.gridColumn60.Caption = "gridColumn60";
		this.gridColumn60.CurrencyDateField = null;
		this.gridColumn60.CurrencyField = null;
		this.gridColumn60.CurrencyFixed = null;
		this.gridColumn60.CurrencyFixedRate = 0.0;
		this.gridColumn60.CurrencyRateField = null;
		this.gridColumn60.FieldName = "ItemClass";
		this.gridColumn60.Name = "gridColumn60";
		this.gridColumn60.PrimaryKey = false;
		this.gridColumn60.UnitField = null;
		this.gridColumn61.Caption = "gridColumn61";
		this.gridColumn61.CurrencyDateField = null;
		this.gridColumn61.CurrencyField = null;
		this.gridColumn61.CurrencyFixed = null;
		this.gridColumn61.CurrencyFixedRate = 0.0;
		this.gridColumn61.CurrencyRateField = null;
		this.gridColumn61.FieldName = "OSvcCode";
		this.gridColumn61.Name = "gridColumn61";
		this.gridColumn61.PrimaryKey = false;
		this.gridColumn61.UnitField = null;
		this.gridColumn62.Caption = "gridColumn62";
		this.gridColumn62.CurrencyDateField = null;
		this.gridColumn62.CurrencyField = null;
		this.gridColumn62.CurrencyFixed = null;
		this.gridColumn62.CurrencyFixedRate = 0.0;
		this.gridColumn62.CurrencyRateField = null;
		this.gridColumn62.FieldName = "ISvcCode";
		this.gridColumn62.Name = "gridColumn62";
		this.gridColumn62.PrimaryKey = false;
		this.gridColumn62.UnitField = null;
		this.gridColumn63.Caption = "gridColumn63";
		this.gridColumn63.CurrencyDateField = null;
		this.gridColumn63.CurrencyField = null;
		this.gridColumn63.CurrencyFixed = null;
		this.gridColumn63.CurrencyFixedRate = 0.0;
		this.gridColumn63.CurrencyRateField = null;
		this.gridColumn63.FieldName = "ServiceGrp";
		this.gridColumn63.Name = "gridColumn63";
		this.gridColumn63.PrimaryKey = false;
		this.gridColumn63.UnitField = null;
		this.gridColumn64.Caption = "gridColumn64";
		this.gridColumn64.CurrencyDateField = null;
		this.gridColumn64.CurrencyField = null;
		this.gridColumn64.CurrencyFixed = null;
		this.gridColumn64.CurrencyFixedRate = 0.0;
		this.gridColumn64.CurrencyRateField = null;
		this.gridColumn64.FieldName = "NCMCode";
		this.gridColumn64.Name = "gridColumn64";
		this.gridColumn64.PrimaryKey = false;
		this.gridColumn64.UnitField = null;
		this.gridColumn65.Caption = "gridColumn65";
		this.gridColumn65.CurrencyDateField = null;
		this.gridColumn65.CurrencyField = null;
		this.gridColumn65.CurrencyFixed = null;
		this.gridColumn65.CurrencyFixedRate = 0.0;
		this.gridColumn65.CurrencyRateField = null;
		this.gridColumn65.FieldName = "MatType";
		this.gridColumn65.Name = "gridColumn65";
		this.gridColumn65.PrimaryKey = false;
		this.gridColumn65.UnitField = null;
		this.gridColumn66.Caption = "gridColumn66";
		this.gridColumn66.CurrencyDateField = null;
		this.gridColumn66.CurrencyField = null;
		this.gridColumn66.CurrencyFixed = null;
		this.gridColumn66.CurrencyFixedRate = 0.0;
		this.gridColumn66.CurrencyRateField = null;
		this.gridColumn66.FieldName = "MatGrp";
		this.gridColumn66.Name = "gridColumn66";
		this.gridColumn66.PrimaryKey = false;
		this.gridColumn66.UnitField = null;
		this.gridColumn67.Caption = "gridColumn67";
		this.gridColumn67.CurrencyDateField = null;
		this.gridColumn67.CurrencyField = null;
		this.gridColumn67.CurrencyFixed = null;
		this.gridColumn67.CurrencyFixedRate = 0.0;
		this.gridColumn67.CurrencyRateField = null;
		this.gridColumn67.FieldName = "ProductSrc";
		this.gridColumn67.Name = "gridColumn67";
		this.gridColumn67.PrimaryKey = false;
		this.gridColumn67.UnitField = null;
		this.gridColumn68.Caption = "gridColumn68";
		this.gridColumn68.CurrencyDateField = null;
		this.gridColumn68.CurrencyField = null;
		this.gridColumn68.CurrencyFixed = null;
		this.gridColumn68.CurrencyFixedRate = 0.0;
		this.gridColumn68.CurrencyRateField = null;
		this.gridColumn68.FieldName = "NegStckAct";
		this.gridColumn68.Name = "gridColumn68";
		this.gridColumn68.PrimaryKey = false;
		this.gridColumn68.UnitField = null;
		this.gridColumn69.Caption = "gridColumn69";
		this.gridColumn69.CurrencyDateField = null;
		this.gridColumn69.CurrencyField = null;
		this.gridColumn69.CurrencyFixed = null;
		this.gridColumn69.CurrencyFixedRate = 0.0;
		this.gridColumn69.CurrencyRateField = null;
		this.gridColumn69.FieldName = "StkInTnAct";
		this.gridColumn69.Name = "gridColumn69";
		this.gridColumn69.PrimaryKey = false;
		this.gridColumn69.UnitField = null;
		this.gridColumn70.Caption = "gridColumn70";
		this.gridColumn70.CurrencyDateField = null;
		this.gridColumn70.CurrencyField = null;
		this.gridColumn70.CurrencyFixed = null;
		this.gridColumn70.CurrencyFixedRate = 0.0;
		this.gridColumn70.CurrencyRateField = null;
		this.gridColumn70.FieldName = "PurBalAct";
		this.gridColumn70.Name = "gridColumn70";
		this.gridColumn70.PrimaryKey = false;
		this.gridColumn70.UnitField = null;
		this.gridColumn71.Caption = "gridColumn71";
		this.gridColumn71.CurrencyDateField = null;
		this.gridColumn71.CurrencyField = null;
		this.gridColumn71.CurrencyFixed = null;
		this.gridColumn71.CurrencyFixedRate = 0.0;
		this.gridColumn71.CurrencyRateField = null;
		this.gridColumn71.FieldName = "WhICenAct";
		this.gridColumn71.Name = "gridColumn71";
		this.gridColumn71.PrimaryKey = false;
		this.gridColumn71.UnitField = null;
		this.gridColumn72.Caption = "gridColumn72";
		this.gridColumn72.CurrencyDateField = null;
		this.gridColumn72.CurrencyField = null;
		this.gridColumn72.CurrencyFixed = null;
		this.gridColumn72.CurrencyFixedRate = 0.0;
		this.gridColumn72.CurrencyRateField = null;
		this.gridColumn72.FieldName = "WhOCenAct";
		this.gridColumn72.Name = "gridColumn72";
		this.gridColumn72.PrimaryKey = false;
		this.gridColumn72.UnitField = null;
		this.gridColumn73.Caption = "gridColumn73";
		this.gridColumn73.CurrencyDateField = null;
		this.gridColumn73.CurrencyField = null;
		this.gridColumn73.CurrencyFixed = null;
		this.gridColumn73.CurrencyFixedRate = 0.0;
		this.gridColumn73.CurrencyRateField = null;
		this.gridColumn73.FieldName = "WipOffset";
		this.gridColumn73.Name = "gridColumn73";
		this.gridColumn73.PrimaryKey = false;
		this.gridColumn73.UnitField = null;
		this.gridColumn74.Caption = "gridColumn74";
		this.gridColumn74.CurrencyDateField = null;
		this.gridColumn74.CurrencyField = null;
		this.gridColumn74.CurrencyFixed = null;
		this.gridColumn74.CurrencyFixedRate = 0.0;
		this.gridColumn74.CurrencyRateField = null;
		this.gridColumn74.FieldName = "StockOffst";
		this.gridColumn74.Name = "gridColumn74";
		this.gridColumn74.PrimaryKey = false;
		this.gridColumn74.UnitField = null;
		this.gridColumn75.Caption = "gridColumn75";
		this.gridColumn75.CurrencyDateField = null;
		this.gridColumn75.CurrencyField = null;
		this.gridColumn75.CurrencyFixed = null;
		this.gridColumn75.CurrencyFixedRate = 0.0;
		this.gridColumn75.CurrencyRateField = null;
		this.gridColumn75.FieldName = "UgpEntry";
		this.gridColumn75.Name = "gridColumn75";
		this.gridColumn75.PrimaryKey = false;
		this.gridColumn75.UnitField = null;
		this.gridColumn76.Caption = "gridColumn76";
		this.gridColumn76.CurrencyDateField = null;
		this.gridColumn76.CurrencyField = null;
		this.gridColumn76.CurrencyFixed = null;
		this.gridColumn76.CurrencyFixedRate = 0.0;
		this.gridColumn76.CurrencyRateField = null;
		this.gridColumn76.FieldName = "IUoMEntry";
		this.gridColumn76.Name = "gridColumn76";
		this.gridColumn76.PrimaryKey = false;
		this.gridColumn76.UnitField = null;
		this.gridColumn77.Caption = "Tolerance Days";
		this.gridColumn77.CurrencyDateField = null;
		this.gridColumn77.CurrencyField = null;
		this.gridColumn77.CurrencyFixed = null;
		this.gridColumn77.CurrencyFixedRate = 0.0;
		this.gridColumn77.CurrencyRateField = null;
		this.gridColumn77.FieldName = "ToleranDay";
		this.gridColumn77.Name = "gridColumn77";
		this.gridColumn77.PrimaryKey = false;
		this.gridColumn77.UnitField = null;
		this.gridColumn77.Visible = true;
		this.gridColumn77.VisibleIndex = 8;
		this.gridColumn77.Width = 171;
		this.gridColumn78.Caption = "gridColumn78";
		this.gridColumn78.CurrencyDateField = null;
		this.gridColumn78.CurrencyField = null;
		this.gridColumn78.CurrencyFixed = null;
		this.gridColumn78.CurrencyFixedRate = 0.0;
		this.gridColumn78.CurrencyRateField = null;
		this.gridColumn78.FieldName = "RuleCode";
		this.gridColumn78.Name = "gridColumn78";
		this.gridColumn78.PrimaryKey = false;
		this.gridColumn78.UnitField = null;
		this.gridColumn79.Caption = "gridColumn79";
		this.gridColumn79.CurrencyDateField = null;
		this.gridColumn79.CurrencyField = null;
		this.gridColumn79.CurrencyFixed = null;
		this.gridColumn79.CurrencyFixedRate = 0.0;
		this.gridColumn79.CurrencyRateField = null;
		this.gridColumn79.FieldName = "CompoWH";
		this.gridColumn79.Name = "gridColumn79";
		this.gridColumn79.PrimaryKey = false;
		this.gridColumn79.UnitField = null;
		this.gridColumn80.Caption = "gridColumn80";
		this.gridColumn80.CurrencyDateField = null;
		this.gridColumn80.CurrencyField = null;
		this.gridColumn80.CurrencyFixed = null;
		this.gridColumn80.CurrencyFixedRate = 0.0;
		this.gridColumn80.CurrencyRateField = null;
		this.gridColumn80.FieldName = "FreeChrgSA";
		this.gridColumn80.Name = "gridColumn80";
		this.gridColumn80.PrimaryKey = false;
		this.gridColumn80.UnitField = null;
		this.gridColumn81.Caption = "gridColumn81";
		this.gridColumn81.CurrencyDateField = null;
		this.gridColumn81.CurrencyField = null;
		this.gridColumn81.CurrencyFixed = null;
		this.gridColumn81.CurrencyFixedRate = 0.0;
		this.gridColumn81.CurrencyRateField = null;
		this.gridColumn81.FieldName = "FreeChrgPU";
		this.gridColumn81.Name = "gridColumn81";
		this.gridColumn81.PrimaryKey = false;
		this.gridColumn81.UnitField = null;
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = Apzon.Forms.Properties.Resources.apply_16x16;
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(1, 398);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(99, 20);
		this.btn_Save.TabIndex = 84;
		this.btn_Save.Text = "OK";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnCancel.Appearance.Options.UseBackColor = true;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(102, 398);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(99, 20);
		this.btnCancel.TabIndex = 83;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Default Bin Locations";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(7, 2);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(114, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 85;
		this.labelLine1.Utext = "Default Bin Locations";
		this.grd_Whs.AllowPasteExcel = true;
		this.grd_Whs.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Whs.ClipboardData = null;
		this.grd_Whs.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Whs.Location = new System.Drawing.Point(0, 28);
		this.grd_Whs.MainView = this.gv_Whs;
		this.grd_Whs.Name = "grd_Whs";
		this.grd_Whs.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_chk_DftBinEnfd });
		this.grd_Whs.Size = new System.Drawing.Size(749, 181);
		this.grd_Whs.TabIndex = 86;
		this.grd_Whs.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Whs });
		this.gv_Whs.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Whs.ColumnKey = null;
		this.gv_Whs.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[12]
		{
			this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx12, this.gridColumnEx4, this.gridColumnEx5, this.gridColumnEx6, this.gridColumnEx7, this.gridColumnEx8, this.gridColumnEx9,
			this.gridColumnEx10, this.gridColumnEx11
		});
		this.gv_Whs.DisableFormSetting = false;
		this.gv_Whs.DisplayException = false;
		this.gv_Whs.EditColumn = null;
		this.gv_Whs.FieldCurrency = null;
		this.gv_Whs.FieldRate = null;
		this.gv_Whs.FormId = "oItemGroups";
		this.gv_Whs.GridControl = this.grd_Whs;
		this.gv_Whs.IgnoreFilter = false;
		this.gv_Whs.IndexEditRow = -1;
		this.gv_Whs.IndicatorWidth = 40;
		this.gv_Whs.IsApzTable = false;
		this.gv_Whs.IsInsertNewRow = false;
		this.gv_Whs.IsLoadingData = false;
		this.gv_Whs.IsMappingContextAction = false;
		this.gv_Whs.ItemId = "OWHS";
		this.gv_Whs.LstColumnAllowBatch = null;
		this.gv_Whs.LstColumnNotAllowBatch = null;
		this.gv_Whs.LstColumnNotEditable = null;
		this.gv_Whs.LstColumNotSetting = null;
		this.gv_Whs.Name = "gv_Whs";
		this.gv_Whs.OptionsView.ColumnAutoWidth = false;
		this.gv_Whs.OptionsView.ShowGroupPanel = false;
		this.gv_Whs.UseBatchMod = false;
		this.gv_Whs.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gv_Whs_CustomRowCellEdit);
		this.gv_Whs.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Whs_ShowingEditor);
		this.gv_Whs.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Whs_CellValueChanged);
		this.gv_Whs.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Whs_CellValueChanging);
		this.gridColumnEx1.Caption = "Internal Number";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "AbsEntry";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = true;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx2.Caption = "Item Group Code";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "ItmsGrpCod";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx3.Caption = "Warehouse Code";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "WhsCode";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 0;
		this.gridColumnEx3.Width = 97;
		this.gridColumnEx12.Caption = "Warehouse Name";
		this.gridColumnEx12.CurrencyDateField = null;
		this.gridColumnEx12.CurrencyField = null;
		this.gridColumnEx12.CurrencyFixed = null;
		this.gridColumnEx12.CurrencyFixedRate = 0.0;
		this.gridColumnEx12.CurrencyRateField = null;
		this.gridColumnEx12.FieldName = "WhsName";
		this.gridColumnEx12.Name = "gridColumnEx12";
		this.gridColumnEx12.PrimaryKey = false;
		this.gridColumnEx12.UnitField = null;
		this.gridColumnEx12.Visible = true;
		this.gridColumnEx12.VisibleIndex = 1;
		this.gridColumnEx12.Width = 102;
		this.gridColumnEx4.Caption = "Default Bin Internal Number";
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "DftBinAbs";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 2;
		this.gridColumnEx4.Width = 151;
		this.gridColumnEx5.Caption = "Default Bin Enforced [Y/N]";
		this.gridColumnEx5.ColumnEdit = this.rep_chk_DftBinEnfd;
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "DftBinEnfd";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 3;
		this.gridColumnEx5.Width = 152;
		this.rep_chk_DftBinEnfd.AutoHeight = false;
		this.rep_chk_DftBinEnfd.DisplayValueChecked = "Y";
		this.rep_chk_DftBinEnfd.DisplayValueGrayed = "N";
		this.rep_chk_DftBinEnfd.DisplayValueUnchecked = "N";
		this.rep_chk_DftBinEnfd.Name = "rep_chk_DftBinEnfd";
		this.rep_chk_DftBinEnfd.ValueChecked = "Y";
		this.rep_chk_DftBinEnfd.ValueGrayed = "N";
		this.rep_chk_DftBinEnfd.ValueUnchecked = "N";
		this.gridColumnEx6.Caption = "Data Source";
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "DataSource";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx7.Caption = "User Signature";
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "UserSign";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.UnitField = null;
		this.gridColumnEx8.Caption = "Log Instance";
		this.gridColumnEx8.CurrencyDateField = null;
		this.gridColumnEx8.CurrencyField = null;
		this.gridColumnEx8.CurrencyFixed = null;
		this.gridColumnEx8.CurrencyFixedRate = 0.0;
		this.gridColumnEx8.CurrencyRateField = null;
		this.gridColumnEx8.FieldName = "LogInstanc";
		this.gridColumnEx8.Name = "gridColumnEx8";
		this.gridColumnEx8.PrimaryKey = false;
		this.gridColumnEx8.UnitField = null;
		this.gridColumnEx9.Caption = "Create Date";
		this.gridColumnEx9.CurrencyDateField = null;
		this.gridColumnEx9.CurrencyField = null;
		this.gridColumnEx9.CurrencyFixed = null;
		this.gridColumnEx9.CurrencyFixedRate = 0.0;
		this.gridColumnEx9.CurrencyRateField = null;
		this.gridColumnEx9.FieldName = "CreateDate";
		this.gridColumnEx9.Name = "gridColumnEx9";
		this.gridColumnEx9.PrimaryKey = false;
		this.gridColumnEx9.UnitField = null;
		this.gridColumnEx10.Caption = "Updating User";
		this.gridColumnEx10.CurrencyDateField = null;
		this.gridColumnEx10.CurrencyField = null;
		this.gridColumnEx10.CurrencyFixed = null;
		this.gridColumnEx10.CurrencyFixedRate = 0.0;
		this.gridColumnEx10.CurrencyRateField = null;
		this.gridColumnEx10.FieldName = "UserSign2";
		this.gridColumnEx10.Name = "gridColumnEx10";
		this.gridColumnEx10.PrimaryKey = false;
		this.gridColumnEx10.UnitField = null;
		this.gridColumnEx11.Caption = "Date of Update";
		this.gridColumnEx11.CurrencyDateField = null;
		this.gridColumnEx11.CurrencyField = null;
		this.gridColumnEx11.CurrencyFixed = null;
		this.gridColumnEx11.CurrencyFixedRate = 0.0;
		this.gridColumnEx11.CurrencyRateField = null;
		this.gridColumnEx11.FieldName = "UpdateDate";
		this.gridColumnEx11.Name = "gridColumnEx11";
		this.gridColumnEx11.PrimaryKey = false;
		this.gridColumnEx11.UnitField = null;
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(3, 4);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grdDoc);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.grd_Whs);
		this.splitContainerControl1.Panel2.Controls.Add(this.labelLine1);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(749, 388);
		this.splitContainerControl1.SplitterPosition = 171;
		this.splitContainerControl1.TabIndex = 88;
		this.splitContainerControl1.Text = "splitContainerControl1";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(754, 425);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.btnCancel);
		base.FormId = "oItemGroups";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_ItemGroups.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ItemGroups";
		this.Text = "Item Groups";
		base.Resize += new System.EventHandler(this.frm_ItemGroups_Resize);
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_InvntSys).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PlaningSys).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PrcrmntMtd).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_OrdrIntrvl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Whs).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Whs).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_chk_DftBinEnfd).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
