using System;
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
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Administrator.SetUp.SalesEmployeeBuyers;

public class frm_SalesEmployeeBuyers : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private int _dfltSlpCode;

	private DataTable _dtOriginal = new DataTable();

	private IContainer components;

	private GridControlEx grd_OSLP;

	private GridViewEx gv_OSLP;

	private GridColumnEx gcSlpCode;

	private GridColumnEx gcSlpName;

	private GridColumnEx gcGroupCode;

	private GridColumnEx gcCommission;

	private GridColumnEx gcMemo;

	private GridColumnEx gcActive;

	private GridColumnEx gcTelephone;

	private RepositoryItemCheckEdit repositoryItemCheckEdit1;

	private GridColumnEx gcMobil;

	private GridColumnEx gcFax;

	private GridColumnEx gc;

	private GridColumnEx gcEmpID;

	private GridColumnEx gcLocked;

	private GridColumnEx gcDataSource;

	private GridColumnEx gcUserSign;

	private SimpleButtonEx btOK;

	private SimpleButtonEx bt_Cancel;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem removeToolStripMenuItem;

	private RepositoryItemLookUpEdit rILUE_CommissionGroup;

	private GridColumnEx gridColumn1;

	private SimpleButtonEx btn_SetDefault;

	private GridColumnEx gridColumn2;

	private RepositoryItemButtonEdit rep_bte_EmpName;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			btOK.Enabled = false;
			bt_Cancel.Enabled = true;
			return;
		}
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btOK.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btOK.Text = Resources.ButtonOK;
			break;
		}
	}

	private async Task frm_SalesEmployeeBuyers_Load(object sender, EventArgs e)
	{
		try
		{
			frm_SalesEmployeeBuyers frm_SalesEmployeeBuyers2 = this;
			string formId = (gv_OSLP.FormId = "oSalesPersons");
			frm_SalesEmployeeBuyers2.FormId = formId;
			gv_OSLP.ItemId = "OSLP";
			LoadCombo();
			LoadData();
			await WinFuntions.LoadUdfColumns(gv_OSLP, "OSLP");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_OSLP_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		try
		{
			if (e.Info.IsRowIndicator)
			{
				e.Info.DisplayText = (e.RowHandle + 1).ToString();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bt_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void btOK_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else
			{
				Save();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_OSLP.GetFocusedRowCellValue("SlpCode"));
			if (num == -1)
			{
				WinCommons.Error(Enums.MessageCode.CannotRemoveARecordCreatedBySystem, "");
			}
			else if (num > 0)
			{
				WinCommons._OpenProcessing();
				string text = _httpClient.SalesEmployeeBuyerClient.DeleteRow(num);
				if (string.IsNullOrEmpty(text))
				{
					gv_OSLP.DeleteSelectedRows();
					WinCommons.Success(Enums.MessageCode.OperationCompletedSuccess, "");
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
			else
			{
				gv_OSLP.DeleteSelectedRows();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private void gv_OSLP_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "SlpName")
			{
				AddNewRow();
			}
			if (e.Column.FieldName == "empID2")
			{
				int slpCode = Function.ParseInt(gv_OSLP.GetRowCellValue(e.RowHandle, "SlpCode"));
				if (!_httpClient.SalesEmployeeBuyerClient.IsExistsSlpCode(slpCode))
				{
					WinCommons.Error(Enums.MessageCode.CannotAssociateWithEmployeeUpdateSalesEmployeeFirst, "");
					return;
				}
				gv_OSLP.SetRowCellValue(e.RowHandle, "empID1", 0);
				if (grd_OSLP.DataSource is DataTable dataTable && dataTable.Rows.Count > 0 && !string.IsNullOrWhiteSpace(Function.ToString(e.Value)))
				{
					for (int i = 0; i < dataTable.Rows.Count; i++)
					{
						if (Function.ToString(dataTable.Rows[i]["empID2"]).ToUpper() == Function.ToString(e.Value).ToUpper() && i != gv_OSLP.GetDataSourceRowIndex(e.RowHandle))
						{
							WinCommons.Error(Enums.MessageCode.SalesEmployeeAlreadyAssociatedWithAnotherEmployeeOrAnotherSalesEmployee, "");
						}
					}
				}
			}
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_bte_EmpName_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			int slpCode = Function.ParseInt(gv_OSLP.GetFocusedRowCellValue("SlpCode"));
			if (!_httpClient.SalesEmployeeBuyerClient.IsExistsSlpCode(slpCode))
			{
				WinCommons.Error(Enums.MessageCode.CannotAssociateWithEmployeeUpdateSalesEmployeeFirst, "");
				return;
			}
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.EmployeeListForSalesEmpSetup, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem == null)
			{
				return;
			}
			int num = Function.ParseInt(frm_CFL2.ChooseItem["empID"]);
			if (num > 0)
			{
				for (int i = 0; i < gv_OSLP.DataRowCount; i++)
				{
					if (Function.ParseInt(gv_OSLP.GetRowCellValue(i, "empID1")) == num)
					{
						WinCommons.Error(Enums.MessageCode.SalesEmployeeAlreadyAssociatedWithAnotherEmployeeOrAnotherSalesEmployee, "");
						return;
					}
				}
			}
			gv_OSLP.SetFocusedRowCellValue("empID2", Function.ToString(frm_CFL2.ChooseItem["lastName"]) + ", " + Function.ToString(frm_CFL2.ChooseItem["firstName"]));
			gv_OSLP.SetFocusedRowCellValue("empID1", num);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_SetDefault_Click(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_OSLP.GetFocusedRowCellValue("SlpCode"));
			if (num != 0)
			{
				string text = _httpClient.SalesEmployeeBuyerClient.SetDefaultCode(num);
				if (string.IsNullOrEmpty(text))
				{
					_dfltSlpCode = num;
					gv_OSLP.RefreshData();
					WinCommons.Success(Enums.MessageCode.OperationCompletedSuccess, "");
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_OSLP_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (Function.ParseInt(gv_OSLP.GetFocusedRowCellValue("SlpCode")) == -1)
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_OSLP_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_OSLP.GetRowCellValue(e.RowHandle, "SlpCode"));
			if (num == -1)
			{
				e.Appearance.BackColor = StyleColor.GridDisable;
			}
			if (num == _dfltSlpCode)
			{
				e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public frm_SalesEmployeeBuyers()
	{
		try
		{
			InitializeComponent();
			OnLoadAsync += frm_SalesEmployeeBuyers_Load;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadCombo()
	{
		try
		{
			DataTable oCOG = _httpClient.SalesEmployeeBuyerClient.GetOCOG();
			rILUE_CommissionGroup.DataSource = oCOG;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadData()
	{
		try
		{
			_dfltSlpCode = _httpClient.SalesEmployeeBuyerClient.GetDefaultCode();
			DataTable oSLP = _httpClient.SalesEmployeeBuyerClient.GetOSLP();
			if (oSLP != null && oSLP.Rows.Count > 0)
			{
				_dtOriginal = oSLP.Copy();
				grd_OSLP.DataSource = oSLP;
				grd_OSLP.RefreshDataSource();
				AddNewRow();
			}
			else
			{
				_dtOriginal = null;
				grd_OSLP.DataSource = null;
				grd_OSLP.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void AddNewRow()
	{
		try
		{
			DataTable dataTable = grd_OSLP.DataSource as DataTable;
			if (dataTable == null || dataTable.Columns.Count == 0)
			{
				dataTable = WinFuntions.GetTableStructure("OSLP");
				dataTable.Columns.Add(new DataColumn("empID1", typeof(int)));
				dataTable.AcceptChanges();
			}
			if (!dataTable.AsEnumerable().Any((DataRow t) => Function.ParseInt(t["SlpCode"]) == 0))
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["SlpCode"] = 0;
				dataRow["SlpName"] = string.Empty;
				dataRow["Memo"] = string.Empty;
				dataRow["Commission"] = 0;
				dataRow["GroupCode"] = 0;
				dataRow["Locked"] = "N";
				dataRow["DataSource"] = "N";
				dataRow["UserSign"] = GlobalData.UserID;
				dataRow["EmpID"] = 0;
				dataRow["Active"] = "Y";
				dataRow["Telephone"] = string.Empty;
				dataRow["Mobil"] = string.Empty;
				dataRow["Fax"] = string.Empty;
				dataRow["Email"] = string.Empty;
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				grd_OSLP.DataSource = dataTable;
				grd_OSLP.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override void CustomFormSetting()
	{
		try
		{
			if (Function.ToString(GlobalData.GeneralSetting.SlpCommUse) == "Y")
			{
				gv_OSLP.Columns["GroupCode"].Visible = true;
				gv_OSLP.Columns["Commission"].Visible = true;
			}
			else
			{
				gv_OSLP.Columns["GroupCode"].Visible = false;
				gv_OSLP.Columns["Commission"].Visible = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void Save()
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				btOK.Enabled = false;
				DataTable dataTable = grd_OSLP.DataSource as DataTable;
				if (!ValidateData())
				{
					return;
				}
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					DataTable changeRow = GetChangeRow();
					if (changeRow != null && changeRow.Rows.Count > 0)
					{
						DataTable dataTable2 = changeRow.Copy();
						for (int num = dataTable2.Rows.Count - 1; num >= 0; num--)
						{
							if ((Function.ParseInt(dataTable2.Rows[num]["SlpCode"]) == 0 && string.IsNullOrWhiteSpace(Function.ToString(dataTable2.Rows[num]["SlpName"]))) || Function.ParseInt(dataTable2.Rows[num]["SlpCode"]) == -1)
							{
								dataTable2.Rows.Remove(dataTable2.Rows[num]);
							}
						}
						dataTable2.AcceptChanges();
						string text = _httpClient.SalesEmployeeBuyerClient.Save(dataTable2);
						if (string.IsNullOrEmpty(text))
						{
							LoadData();
							WinCommons.Success(Enums.MessageCode.OperationCompletedSuccess, "");
							FormMode = Enums.FormViewModeType.View;
						}
						else
						{
							WinCommons.Error(Enums.MessageCode.None, text);
						}
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.NoChangedRowToSavefrm_SalesEmployeeBuyers, "");
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		btOK.Enabled = true;
		WinCommons._CloseProcessing();
	}

	private DataTable GetChangeRow()
	{
		DataTable dataTable = new DataTable();
		try
		{
			DataTable dataTable2 = grd_OSLP.DataSource as DataTable;
			dataTable = dataTable2.Clone();
			foreach (DataRow row in dataTable2.Rows)
			{
				int slpCode = Function.ParseInt(row["SlpCode"]);
				if (slpCode == 0)
				{
					dataTable.ImportRow(row);
					dataTable.AcceptChanges();
					continue;
				}
				DataRow dataRow2 = _dtOriginal.AsEnumerable().FirstOrDefault((DataRow r1) => Function.ParseInt(r1["SlpCode"]) == slpCode);
				if (dataRow2 == null)
				{
					continue;
				}
				bool flag = false;
				foreach (DataColumn column in dataTable2.Columns)
				{
					if (Function.ToString(dataRow2[column.ColumnName]) != Function.ToString(row[column.ColumnName]))
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					dataTable.Rows.Add(row.ItemArray);
					dataTable.AcceptChanges();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return dataTable;
	}

	private bool ValidateData()
	{
		try
		{
			if (!(grd_OSLP.DataSource is DataTable dataTable) || dataTable.Rows.Count <= 1)
			{
				WinCommons.Error(Enums.MessageCode.NoDataToSave, "");
				return false;
			}
			DataTable dataTable2 = dataTable.Copy();
			for (int i = 1; i < dataTable2.Rows.Count; i++)
			{
				if (Function.ParseInt(dataTable2.Rows[i]["SlpCode"]) > 0 && string.IsNullOrWhiteSpace(Function.ToString(dataTable2.Rows[i]["SlpName"])))
				{
					WinCommons.Error(Enums.MessageCode.NotEnoughDataSalesEmployeeName, "");
					return false;
				}
			}
			for (int num = dataTable2.Rows.Count - 1; num >= 0; num--)
			{
				if (Function.ParseInt(dataTable2.Rows[num]["SlpCode"]) == 0 && string.IsNullOrWhiteSpace(Function.ToString(dataTable2.Rows[num]["SlpName"])))
				{
					dataTable2.Rows.Remove(dataTable2.Rows[num]);
				}
			}
			dataTable2.AcceptChanges();
			if (dataTable2.Rows.Count <= 1)
			{
				WinCommons.Error(Enums.MessageCode.NoDataToSave, "");
				return false;
			}
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return false;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.SetUp.SalesEmployeeBuyers.frm_SalesEmployeeBuyers));
		this.grd_OSLP = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_OSLP = new Apzon.Controls.Grid.GridViewEx();
		this.gcSlpCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcSlpName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcGroupCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rILUE_CommissionGroup = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcCommission = new Apzon.Controls.Grid.GridColumnEx();
		this.gcMemo = new Apzon.Controls.Grid.GridColumnEx();
		this.gcActive = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gcTelephone = new Apzon.Controls.Grid.GridColumnEx();
		this.gcMobil = new Apzon.Controls.Grid.GridColumnEx();
		this.gcFax = new Apzon.Controls.Grid.GridColumnEx();
		this.gc = new Apzon.Controls.Grid.GridColumnEx();
		this.gcEmpID = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLocked = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDataSource = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUserSign = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_bte_EmpName = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.btOK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.bt_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_SetDefault = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_OSLP).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_OSLP).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rILUE_CommissionGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_EmpName).BeginInit();
		base.SuspendLayout();
		this.grd_OSLP.AllowPasteExcel = true;
		this.grd_OSLP.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_OSLP.ClipboardData = null;
		this.grd_OSLP.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_OSLP.Location = new System.Drawing.Point(2, 2);
		this.grd_OSLP.MainView = this.gv_OSLP;
		this.grd_OSLP.Name = "grd_OSLP";
		this.grd_OSLP.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.repositoryItemCheckEdit1, this.rILUE_CommissionGroup, this.rep_bte_EmpName });
		this.grd_OSLP.Size = new System.Drawing.Size(745, 377);
		this.grd_OSLP.TabIndex = 0;
		this.grd_OSLP.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_OSLP });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(118, 26);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.gv_OSLP.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_OSLP.ColumnKey = null;
		this.gv_OSLP.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[16]
		{
			this.gcSlpCode, this.gcSlpName, this.gcGroupCode, this.gcCommission, this.gcMemo, this.gcActive, this.gcTelephone, this.gcMobil, this.gcFax, this.gc,
			this.gcEmpID, this.gcLocked, this.gcDataSource, this.gcUserSign, this.gridColumn1, this.gridColumn2
		});
		this.gv_OSLP.DisableFormSetting = false;
		this.gv_OSLP.DisplayException = false;
		this.gv_OSLP.EditColumn = null;
		this.gv_OSLP.FieldCurrency = null;
		this.gv_OSLP.FieldRate = null;
		this.gv_OSLP.FormId = "oSaleEmployeeBuyerSetup";
		this.gv_OSLP.GridControl = this.grd_OSLP;
		this.gv_OSLP.IgnoreFilter = false;
		this.gv_OSLP.IndexEditRow = -1;
		this.gv_OSLP.IndicatorWidth = 30;
		this.gv_OSLP.IsApzTable = false;
		this.gv_OSLP.IsInsertNewRow = false;
		this.gv_OSLP.IsLoadingData = false;
		this.gv_OSLP.IsMappingContextAction = false;
		this.gv_OSLP.ItemId = "OSLP";
		this.gv_OSLP.LstColumnAllowBatch = null;
		this.gv_OSLP.LstColumnNotAllowBatch = null;
		this.gv_OSLP.LstColumnNotEditable = null;
		this.gv_OSLP.LstColumNotSetting = null;
		this.gv_OSLP.Name = "gv_OSLP";
		this.gv_OSLP.OptionsView.ColumnAutoWidth = false;
		this.gv_OSLP.OptionsView.ShowGroupPanel = false;
		this.gv_OSLP.UseBatchMod = false;
		this.gv_OSLP.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_OSLP_CustomDrawRowIndicator);
		this.gv_OSLP.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_OSLP_RowCellStyle);
		this.gv_OSLP.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_OSLP_ShowingEditor);
		this.gv_OSLP.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_OSLP_CellValueChanged);
		this.gcSlpCode.Caption = "SlpCode";
		this.gcSlpCode.CurrencyDateField = null;
		this.gcSlpCode.CurrencyField = null;
		this.gcSlpCode.CurrencyFixed = null;
		this.gcSlpCode.CurrencyFixedRate = 0.0;
		this.gcSlpCode.CurrencyRateField = null;
		this.gcSlpCode.FieldName = "SlpCode";
		this.gcSlpCode.Name = "gcSlpCode";
		this.gcSlpCode.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcSlpCode.PrimaryKey = true;
		this.gcSlpCode.UnitField = null;
		this.gcSlpName.Caption = "Sales Employee Name";
		this.gcSlpName.CurrencyDateField = null;
		this.gcSlpName.CurrencyField = null;
		this.gcSlpName.CurrencyFixed = null;
		this.gcSlpName.CurrencyFixedRate = 0.0;
		this.gcSlpName.CurrencyRateField = null;
		this.gcSlpName.FieldName = "SlpName";
		this.gcSlpName.Name = "gcSlpName";
		this.gcSlpName.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcSlpName.PrimaryKey = false;
		this.gcSlpName.UnitField = null;
		this.gcSlpName.Visible = true;
		this.gcSlpName.VisibleIndex = 0;
		this.gcSlpName.Width = 117;
		this.gcGroupCode.Caption = "Commission Group";
		this.gcGroupCode.ColumnEdit = this.rILUE_CommissionGroup;
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
		this.gcGroupCode.Width = 123;
		this.rILUE_CommissionGroup.AutoHeight = false;
		this.rILUE_CommissionGroup.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rILUE_CommissionGroup.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("GroupName", "GroupName"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("GroupCode", "GroupCode", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rILUE_CommissionGroup.DisplayMember = "GroupName";
		this.rILUE_CommissionGroup.Name = "rILUE_CommissionGroup";
		this.rILUE_CommissionGroup.NullText = "";
		this.rILUE_CommissionGroup.ValueMember = "GroupCode";
		this.gcCommission.Caption = "Commission %";
		this.gcCommission.CurrencyDateField = null;
		this.gcCommission.CurrencyField = null;
		this.gcCommission.CurrencyFixed = null;
		this.gcCommission.CurrencyFixedRate = 0.0;
		this.gcCommission.CurrencyRateField = null;
		this.gcCommission.FieldName = "Commission";
		this.gcCommission.Name = "gcCommission";
		this.gcCommission.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcCommission.PrimaryKey = false;
		this.gcCommission.UnitField = null;
		this.gcCommission.Width = 106;
		this.gcMemo.Caption = "Remarks";
		this.gcMemo.CurrencyDateField = null;
		this.gcMemo.CurrencyField = null;
		this.gcMemo.CurrencyFixed = null;
		this.gcMemo.CurrencyFixedRate = 0.0;
		this.gcMemo.CurrencyRateField = null;
		this.gcMemo.FieldName = "Memo";
		this.gcMemo.Name = "gcMemo";
		this.gcMemo.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcMemo.PrimaryKey = false;
		this.gcMemo.UnitField = null;
		this.gcMemo.Visible = true;
		this.gcMemo.VisibleIndex = 1;
		this.gcActive.Caption = "Active";
		this.gcActive.ColumnEdit = this.repositoryItemCheckEdit1;
		this.gcActive.CurrencyDateField = null;
		this.gcActive.CurrencyField = null;
		this.gcActive.CurrencyFixed = null;
		this.gcActive.CurrencyFixedRate = 0.0;
		this.gcActive.CurrencyRateField = null;
		this.gcActive.FieldName = "Active";
		this.gcActive.Name = "gcActive";
		this.gcActive.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcActive.PrimaryKey = false;
		this.gcActive.UnitField = null;
		this.gcActive.Visible = true;
		this.gcActive.VisibleIndex = 2;
		this.repositoryItemCheckEdit1.AutoHeight = false;
		this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
		this.repositoryItemCheckEdit1.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.repositoryItemCheckEdit1.ValueChecked = "Y";
		this.repositoryItemCheckEdit1.ValueUnchecked = "N";
		this.gcTelephone.Caption = "Telephone";
		this.gcTelephone.CurrencyDateField = null;
		this.gcTelephone.CurrencyField = null;
		this.gcTelephone.CurrencyFixed = null;
		this.gcTelephone.CurrencyFixedRate = 0.0;
		this.gcTelephone.CurrencyRateField = null;
		this.gcTelephone.FieldName = "Telephone";
		this.gcTelephone.Name = "gcTelephone";
		this.gcTelephone.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcTelephone.PrimaryKey = false;
		this.gcTelephone.UnitField = null;
		this.gcTelephone.Visible = true;
		this.gcTelephone.VisibleIndex = 3;
		this.gcMobil.Caption = "Mobile";
		this.gcMobil.CurrencyDateField = null;
		this.gcMobil.CurrencyField = null;
		this.gcMobil.CurrencyFixed = null;
		this.gcMobil.CurrencyFixedRate = 0.0;
		this.gcMobil.CurrencyRateField = null;
		this.gcMobil.FieldName = "Mobil";
		this.gcMobil.Name = "gcMobil";
		this.gcMobil.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcMobil.PrimaryKey = false;
		this.gcMobil.UnitField = null;
		this.gcMobil.Visible = true;
		this.gcMobil.VisibleIndex = 4;
		this.gcFax.Caption = "Fax";
		this.gcFax.CurrencyDateField = null;
		this.gcFax.CurrencyField = null;
		this.gcFax.CurrencyFixed = null;
		this.gcFax.CurrencyFixedRate = 0.0;
		this.gcFax.CurrencyRateField = null;
		this.gcFax.FieldName = "Fax";
		this.gcFax.Name = "gcFax";
		this.gcFax.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcFax.PrimaryKey = false;
		this.gcFax.UnitField = null;
		this.gcFax.Visible = true;
		this.gcFax.VisibleIndex = 5;
		this.gc.Caption = "E-Mail";
		this.gc.CurrencyDateField = null;
		this.gc.CurrencyField = null;
		this.gc.CurrencyFixed = null;
		this.gc.CurrencyFixedRate = 0.0;
		this.gc.CurrencyRateField = null;
		this.gc.FieldName = "Email";
		this.gc.Name = "gc";
		this.gc.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gc.PrimaryKey = false;
		this.gc.UnitField = null;
		this.gc.Visible = true;
		this.gc.VisibleIndex = 6;
		this.gcEmpID.Caption = "Employee";
		this.gcEmpID.CurrencyDateField = null;
		this.gcEmpID.CurrencyField = null;
		this.gcEmpID.CurrencyFixed = null;
		this.gcEmpID.CurrencyFixedRate = 0.0;
		this.gcEmpID.CurrencyRateField = null;
		this.gcEmpID.FieldName = "empID1";
		this.gcEmpID.Name = "gcEmpID";
		this.gcEmpID.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcEmpID.PrimaryKey = false;
		this.gcEmpID.UnitField = null;
		this.gcLocked.Caption = "Locked";
		this.gcLocked.ColumnEdit = this.repositoryItemCheckEdit1;
		this.gcLocked.CurrencyDateField = null;
		this.gcLocked.CurrencyField = null;
		this.gcLocked.CurrencyFixed = null;
		this.gcLocked.CurrencyFixedRate = 0.0;
		this.gcLocked.CurrencyRateField = null;
		this.gcLocked.FieldName = "Locked";
		this.gcLocked.Name = "gcLocked";
		this.gcLocked.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcLocked.PrimaryKey = false;
		this.gcLocked.UnitField = null;
		this.gcDataSource.Caption = "DataSource";
		this.gcDataSource.CurrencyDateField = null;
		this.gcDataSource.CurrencyField = null;
		this.gcDataSource.CurrencyFixed = null;
		this.gcDataSource.CurrencyFixedRate = 0.0;
		this.gcDataSource.CurrencyRateField = null;
		this.gcDataSource.FieldName = "DataSource";
		this.gcDataSource.Name = "gcDataSource";
		this.gcDataSource.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcDataSource.PrimaryKey = false;
		this.gcDataSource.UnitField = null;
		this.gcUserSign.Caption = "UserSign";
		this.gcUserSign.CurrencyDateField = null;
		this.gcUserSign.CurrencyField = null;
		this.gcUserSign.CurrencyFixed = null;
		this.gcUserSign.CurrencyFixedRate = 0.0;
		this.gcUserSign.CurrencyRateField = null;
		this.gcUserSign.FieldName = "UserSign";
		this.gcUserSign.Name = "gcUserSign";
		this.gcUserSign.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcUserSign.PrimaryKey = false;
		this.gcUserSign.UnitField = null;
		this.gridColumn1.Caption = "EmpID";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "EmpID";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.gridColumn2.Caption = "Employee";
		this.gridColumn2.ColumnEdit = this.rep_bte_EmpName;
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "empID2";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 7;
		this.rep_bte_EmpName.AutoHeight = false;
		this.rep_bte_EmpName.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.rep_bte_EmpName.Name = "rep_bte_EmpName";
		this.rep_bte_EmpName.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_bte_EmpName_ButtonPressed);
		this.btOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btOK.EditControl = null;
		this.btOK.EditValue = "OK";
		this.btOK.FieldName = null;
		this.btOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btOK.ImageOptions.Image");
		this.btOK.IsSearchControl = false;
		this.btOK.Location = new System.Drawing.Point(2, 382);
		this.btOK.Name = "btOK";
		this.btOK.ReadOnly = false;
		this.btOK.SettingInVisible = false;
		this.btOK.SettingReadonly = false;
		this.btOK.Size = new System.Drawing.Size(75, 23);
		this.btOK.TabIndex = 1;
		this.btOK.Text = "OK";
		this.btOK.Click += new System.EventHandler(this.btOK_Click);
		this.bt_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.bt_Cancel.EditControl = null;
		this.bt_Cancel.EditValue = "Cancel";
		this.bt_Cancel.FieldName = null;
		this.bt_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bt_Cancel.ImageOptions.Image");
		this.bt_Cancel.IsSearchControl = false;
		this.bt_Cancel.Location = new System.Drawing.Point(80, 382);
		this.bt_Cancel.Name = "bt_Cancel";
		this.bt_Cancel.ReadOnly = false;
		this.bt_Cancel.SettingInVisible = false;
		this.bt_Cancel.SettingReadonly = false;
		this.bt_Cancel.Size = new System.Drawing.Size(75, 23);
		this.bt_Cancel.TabIndex = 1;
		this.bt_Cancel.Text = "Cancel";
		this.bt_Cancel.Click += new System.EventHandler(this.bt_Cancel_Click);
		this.btn_SetDefault.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_SetDefault.EditControl = null;
		this.btn_SetDefault.EditValue = "&Set as Default";
		this.btn_SetDefault.FieldName = null;
		this.btn_SetDefault.IsSearchControl = false;
		this.btn_SetDefault.Location = new System.Drawing.Point(647, 382);
		this.btn_SetDefault.Name = "btn_SetDefault";
		this.btn_SetDefault.ReadOnly = false;
		this.btn_SetDefault.SettingInVisible = false;
		this.btn_SetDefault.SettingReadonly = false;
		this.btn_SetDefault.Size = new System.Drawing.Size(100, 23);
		this.btn_SetDefault.TabIndex = 2;
		this.btn_SetDefault.Text = "&Set as Default";
		this.btn_SetDefault.Click += new System.EventHandler(this.btn_SetDefault_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(749, 409);
		base.CancelButton = this.bt_Cancel;
		base.Controls.Add(this.btn_SetDefault);
		base.Controls.Add(this.bt_Cancel);
		base.Controls.Add(this.btOK);
		base.Controls.Add(this.grd_OSLP);
		base.FormId = "oSaleEmployeeBuyerSetup";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_SalesEmployeeBuyers";
		this.Text = "Sales Employees/Buyers - Setup";
		((System.ComponentModel.ISupportInitialize)this.grd_OSLP).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_OSLP).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rILUE_CommissionGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_EmpName).EndInit();
		base.ResumeLayout(false);
	}
}
