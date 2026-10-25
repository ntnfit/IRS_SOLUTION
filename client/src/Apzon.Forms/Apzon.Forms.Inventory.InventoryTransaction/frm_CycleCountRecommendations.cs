using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
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
using Apzon.Forms.Commons;
using Apzon.Forms.UserControls;
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

namespace Apzon.Forms.Inventory.InventoryTransaction;

public class frm_CycleCountRecommendations : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private LabelLine labelLine1;

	private LabelLine labelLine2;

	private LabelLine labelLine3;

	private LabelLine labelLine4;

	private LabelLine labelLine5;

	private LabelLine labelLine6;

	private LabelLine labelLine7;

	private SimpleButtonEx btn_Ok;

	private SimpleButtonEx btn_Cancel;

	private CheckedComboBoxEdit checked_WhsCode;

	private LookUpEdit look_FromCycleCode;

	private LookUpEdit look_ToCycleCode;

	private GridControlEx grd_Item;

	private GridViewEx gv_Item;

	private GridColumnEx ItemNo;

	private GridColumnEx WhsCode;

	private GridColumnEx SublevelCode;

	private GridColumnEx CycleCode;

	private GridColumnEx Recurrence;

	private GridColumnEx NextDate;

	private GridColumnEx Time;

	private GridColumnEx Counted;

	private GridColumnEx ItemDescription;

	private RepositoryItemCheckEdit check_Counted;

	private SimpleButtonEx btn_InventoryCounting;

	private SimpleButtonEx btn_Huy;

	private LookUpEdit look_ToItemCode;

	private LookUpEdit look_FromItemCode;

	private RepositoryItemButtonEditCFL btn_ItemCode;

	private RepositoryItemButtonEditCFL btn_WhsCode;

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
			btn_Huy.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Huy.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_CycleCountRecommendations()
	{
		InitializeComponent();
	}

	private void labelLine5_Load(object sender, EventArgs e)
	{
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
		{
			Close();
		}
	}

	private async void btn_Ok_Click(object sender, EventArgs e)
	{
		try
		{
			string whs = Function.ToString(checked_WhsCode.EditValue);
			int fromCycleCode = Function.ParseInt(look_FromCycleCode.EditValue);
			int toCycleCode = Function.ParseInt(look_ToCycleCode.EditValue);
			string fromItemCode = Function.ToString(look_FromItemCode.EditValue);
			string toItemCode = Function.ToString(look_ToItemCode.EditValue);
			DataTable dataSource = await GetListDataLine(whs, fromCycleCode, toCycleCode, fromItemCode, toItemCode);
			grd_Item.DataSource = dataSource;
			grd_Item.DataSourceTable().AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task frm_CycleCountRecommendations_Load(object sender, EventArgs e)
	{
		_ = 4;
		try
		{
			btn_ItemCode.ValueMember = "ItemCode";
			btn_ItemCode.TypeCFL = Enums.TypeCFL.ListItems;
			btn_ItemCode.TypeDrillDown = ApzObjectTypes.oItems;
			btn_ItemCode.RefreshDataSource();
			btn_WhsCode.ValueMember = "WhsCode";
			btn_WhsCode.TypeCFL = Enums.TypeCFL.Warehouse;
			btn_WhsCode.RefreshDataSource();
			btn_WhsCode.TypeDrillDown = ApzObjectTypes.oWarehouses;
			gv_Item.Columns["NextDate"].DisplayFormat.FormatType = FormatType.DateTime;
			gv_Item.Columns["NextDate"].DisplayFormat.FormatString = GlobalData.DateTimeFormat;
			await SetDataToCycleCode();
			await SetDataFromCycleCode();
			await SetDataFromItemCode();
			await SetDataToItemCode();
			await SetDataWhs();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Item_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		try
		{
			if (e.Info.IsRowIndicator && e.RowHandle >= 0)
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

	private void btn_InventoryCounting_Click(object sender, EventArgs e)
	{
		try
		{
			int[] selectedRows = gv_Item.GetSelectedRows();
			DataTable dataTable = grd_Item.DataSourceTable.Copy();
			DataTable dataTable2 = dataTable.Clone();
			string value = "";
			int[] array = selectedRows;
			foreach (int index in array)
			{
				DataRow row = dataTable.Rows[index];
				dataTable2.ImportRow(row);
				dataTable2.AcceptChanges();
			}
			DataTable dataTable3 = new DataTable();
			dataTable3.Columns.Add("DocEntry", typeof(int));
			dataTable3.Columns.Add("LineNum", typeof(int));
			dataTable3.Columns.Add("ItemCode", typeof(string));
			dataTable3.Columns.Add("ItemDesc", typeof(string));
			dataTable3.Columns.Add("WhsCode", typeof(string));
			dataTable3.Columns.Add("InWhsQty", typeof(decimal));
			dataTable3.Columns.Add("Counted", typeof(string));
			dataTable3.Columns.Add("UomQty", typeof(decimal));
			dataTable3.Columns.Add("CountQty", typeof(decimal));
			dataTable3.Columns.Add("Difference", typeof(decimal));
			dataTable3.Columns.Add("DiffPercen", typeof(decimal));
			dataTable3.Columns.Add("UomCode", typeof(string));
			if (dataTable2.Rows.Count > 0 && dataTable2 != null)
			{
				for (int j = 0; j < dataTable2.Rows.Count; j++)
				{
					DataRow dataRow = dataTable3.NewRow();
					dataRow["DocEntry"] = 0;
					dataRow["LineNum"] = 0;
					dataRow["ItemCode"] = Function.ToString(dataTable2.Rows[j]["ItemCode"]);
					dataRow["ItemDesc"] = Function.ToString(dataTable2.Rows[j]["ItemName"]);
					dataRow["WhsCode"] = Function.ToString(dataTable2.Rows[j]["WhsCode"]);
					dataRow["InWhsQty"] = 0;
					dataRow["Counted"] = "N";
					dataRow["UomQty"] = 0;
					dataRow["CountQty"] = 0;
					dataRow["Difference"] = 0;
					dataRow["DiffPercen"] = 0;
					dataRow["UomCode"] = "";
					string value2 = Function.ToString(dataTable2.Rows[j]["WhsCode"]);
					dataTable3.Rows.Add(dataRow);
					for (int k = j + 1; k < dataTable2.Rows.Count; k++)
					{
						if (!Function.ToString(dataTable2.Rows[k]["WhsCode"]).Equals(value2))
						{
							value = "Bạn chỉ được chọn 1 WhsCode";
						}
					}
					dataTable3.AcceptChanges();
				}
				if (!string.IsNullOrEmpty(value))
				{
					WinCommons.Error(Enums.MessageCode.YouOnlySelectedOneWhs, "");
					return;
				}
				string whsCode = Function.ToString(dataTable3.Rows[0]["WhsCode"]);
				frm_StockCounting frm_StockCounting2 = new frm_StockCounting(dataTable3, whsCode);
				frm_StockCounting2.MdiParent = MdiParent;
				frm_StockCounting2.Show();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.ItemCodeCannotBeNull, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Item_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "Hour")
			{
				e.DisplayText = Function.ParseIntToTimeSpanString(Function.ParseInt(gv_Item.GetRowCellValue(e.ListSourceRowIndex, "Hour")));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Huy_Click(object sender, EventArgs e)
	{
		Close();
	}

	public async Task SetDataFromCycleCode()
	{
		DataTable dataSource = await _httpClient.CycleCountRecommendation.GetListCycleCode();
		look_FromCycleCode.Properties.DataSource = dataSource;
		look_FromCycleCode.Properties.DisplayMember = "Name";
		look_FromCycleCode.Properties.ValueMember = "Code";
	}

	public async Task SetDataToCycleCode()
	{
		DataTable dataSource = await _httpClient.CycleCountRecommendation.GetListCycleCode();
		look_ToCycleCode.Properties.DataSource = dataSource;
		look_ToCycleCode.Properties.DisplayMember = "Name";
		look_ToCycleCode.Properties.ValueMember = "Code";
	}

	public async Task SetDataFromItemCode()
	{
		DataTable dataSource = await _httpClient.CycleCountRecommendation.GetListItemCode();
		look_FromItemCode.Properties.DataSource = dataSource;
		look_FromItemCode.Properties.DisplayMember = "ItemCode";
		look_FromItemCode.Properties.ValueMember = "ItemCode";
	}

	public async Task SetDataToItemCode()
	{
		DataTable dataSource = await _httpClient.CycleCountRecommendation.GetListItemCode();
		look_ToItemCode.Properties.DataSource = dataSource;
		look_ToItemCode.Properties.DisplayMember = "ItemCode";
		look_ToItemCode.Properties.ValueMember = "ItemCode";
	}

	private async Task<List<string>> GetListWhs()
	{
		List<string> lst = new List<string>();
		try
		{
			DataTable dataTable = await _httpClient.CycleCountRecommendation.GetWarehouse();
			if (dataTable.IsNotNull())
			{
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					lst.Add(Function.ToString(dataTable.Rows[i]["WhsCode"]));
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		return lst;
	}

	private async Task SetDataWhs()
	{
		try
		{
			RepositoryItemCheckedComboBoxEdit properties = checked_WhsCode.Properties;
			properties.DataSource = await GetListWhs();
			checked_WhsCode.EditValue = "";
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task<DataTable> GetListDataLine(string whs, int fromCycleCode, int toCycleCode, string fromItemCode, string toItemCode)
	{
		try
		{
			DataTable dataTable = await _httpClient.CycleCountRecommendation.GetListDataLine(whs, fromCycleCode, toCycleCode, fromItemCode, toItemCode);
			foreach (DataRow row in dataTable.Rows)
			{
				switch (Function.ParseInt(row["Frequency"]))
				{
				case 1:
					row["Frequency"] = "Daily";
					break;
				case 2:
					row["Frequency"] = "Weekly";
					break;
				case 3:
					row["Frequency"] = "Monthly";
					break;
				case 4:
					row["Frequency"] = "Annually";
					break;
				case 0:
					row["Frequency"] = "None";
					break;
				}
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Inventory.InventoryTransaction.frm_CycleCountRecommendations));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		this.btn_Ok = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.checked_WhsCode = new DevExpress.XtraEditors.CheckedComboBoxEdit();
		this.labelLine7 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.look_FromCycleCode = new DevExpress.XtraEditors.LookUpEdit();
		this.look_ToCycleCode = new DevExpress.XtraEditors.LookUpEdit();
		this.grd_Item = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Item = new Apzon.Controls.Grid.GridViewEx();
		this.ItemNo = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_ItemCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.ItemDescription = new Apzon.Controls.Grid.GridColumnEx();
		this.WhsCode = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_WhsCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.SublevelCode = new Apzon.Controls.Grid.GridColumnEx();
		this.CycleCode = new Apzon.Controls.Grid.GridColumnEx();
		this.Recurrence = new Apzon.Controls.Grid.GridColumnEx();
		this.NextDate = new Apzon.Controls.Grid.GridColumnEx();
		this.Time = new Apzon.Controls.Grid.GridColumnEx();
		this.Counted = new Apzon.Controls.Grid.GridColumnEx();
		this.check_Counted = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.btn_InventoryCounting = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Huy = new Apzon.Controls.Controls.SimpleButtonEx();
		this.look_ToItemCode = new DevExpress.XtraEditors.LookUpEdit();
		this.look_FromItemCode = new DevExpress.XtraEditors.LookUpEdit();
		((System.ComponentModel.ISupportInitialize)this.checked_WhsCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_FromCycleCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_ToCycleCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Item).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Item).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ItemCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_WhsCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.check_Counted).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_ToItemCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_FromItemCode.Properties).BeginInit();
		base.SuspendLayout();
		this.btn_Ok.EditControl = null;
		this.btn_Ok.EditValue = "Run";
		this.btn_Ok.FieldName = null;
		this.btn_Ok.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Ok.ImageOptions.Image");
		this.btn_Ok.IsSearchControl = false;
		this.btn_Ok.Location = new System.Drawing.Point(13, 101);
		this.btn_Ok.Name = "btn_Ok";
		this.btn_Ok.ReadOnly = false;
		this.btn_Ok.SettingInVisible = false;
		this.btn_Ok.SettingReadonly = false;
		this.btn_Ok.Size = new System.Drawing.Size(75, 23);
		this.btn_Ok.TabIndex = 18;
		this.btn_Ok.Text = "Run";
		this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(91, 391);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 21;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.checked_WhsCode.EditValue = "";
		this.checked_WhsCode.Location = new System.Drawing.Point(145, 65);
		this.checked_WhsCode.Name = "checked_WhsCode";
		this.checked_WhsCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.checked_WhsCode.Size = new System.Drawing.Size(251, 20);
		this.checked_WhsCode.TabIndex = 22;
		this.labelLine7.EditControl = null;
		this.labelLine7.EditValue = "Whs Code";
		this.labelLine7.FieldName = null;
		this.labelLine7.HideWhenEmpty = false;
		this.labelLine7.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine7.Location = new System.Drawing.Point(12, 65);
		this.labelLine7.Name = "labelLine7";
		this.labelLine7.ReadOnly = false;
		this.labelLine7.SettingInVisible = false;
		this.labelLine7.SettingReadonly = false;
		this.labelLine7.ShowUnderLine = true;
		this.labelLine7.Size = new System.Drawing.Size(60, 20);
		this.labelLine7.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine7.TabIndex = 11;
		this.labelLine7.Utext = "Whs Code";
		this.labelLine6.EditControl = null;
		this.labelLine6.EditValue = "To";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(267, 39);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ReadOnly = false;
		this.labelLine6.SettingInVisible = false;
		this.labelLine6.SettingReadonly = false;
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(23, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 5;
		this.labelLine6.Utext = "To";
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "From";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(109, 13);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(29, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 4;
		this.labelLine5.Utext = "From";
		this.labelLine5.Load += new System.EventHandler(this.labelLine5_Load);
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "To";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(267, 13);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(23, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 3;
		this.labelLine4.Utext = "To";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Item Code";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(13, 39);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(60, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 2;
		this.labelLine3.Utext = "Item Code";
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "From";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(109, 39);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(29, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 1;
		this.labelLine2.Utext = "From";
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Cycle Code";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(13, 13);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(60, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 0;
		this.labelLine1.Utext = "Cycle Code";
		this.look_FromCycleCode.Location = new System.Drawing.Point(145, 12);
		this.look_FromCycleCode.Name = "look_FromCycleCode";
		this.look_FromCycleCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_FromCycleCode.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.look_FromCycleCode.Properties.DisplayMember = "Name";
		this.look_FromCycleCode.Properties.NullText = "";
		this.look_FromCycleCode.Properties.ValueMember = "Code";
		this.look_FromCycleCode.Size = new System.Drawing.Size(100, 20);
		this.look_FromCycleCode.TabIndex = 23;
		this.look_ToCycleCode.Location = new System.Drawing.Point(296, 12);
		this.look_ToCycleCode.Name = "look_ToCycleCode";
		this.look_ToCycleCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_ToCycleCode.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.look_ToCycleCode.Properties.NullText = "";
		this.look_ToCycleCode.Size = new System.Drawing.Size(100, 20);
		this.look_ToCycleCode.TabIndex = 26;
		this.grd_Item.AllowPasteExcel = true;
		this.grd_Item.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Item.ClipboardData = null;
		this.grd_Item.Location = new System.Drawing.Point(12, 130);
		this.grd_Item.MainView = this.gv_Item;
		this.grd_Item.Name = "grd_Item";
		this.grd_Item.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.check_Counted, this.btn_ItemCode, this.btn_WhsCode });
		this.grd_Item.Size = new System.Drawing.Size(840, 241);
		this.grd_Item.TabIndex = 27;
		this.grd_Item.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Item });
		this.gv_Item.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Item.ColumnKey = null;
		this.gv_Item.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[9] { this.ItemNo, this.ItemDescription, this.WhsCode, this.SublevelCode, this.CycleCode, this.Recurrence, this.NextDate, this.Time, this.Counted });
		this.gv_Item.DisableFormSetting = false;
		this.gv_Item.DisplayException = false;
		this.gv_Item.EditColumn = null;
		this.gv_Item.FieldCurrency = null;
		this.gv_Item.FieldRate = null;
		this.gv_Item.FormId = "oCycleCountRsc";
		this.gv_Item.GridControl = this.grd_Item;
		this.gv_Item.IgnoreFilter = false;
		this.gv_Item.IndexEditRow = -1;
		this.gv_Item.IndicatorWidth = 30;
		this.gv_Item.IsApzTable = false;
		this.gv_Item.IsInsertNewRow = false;
		this.gv_Item.IsLoadingData = false;
		this.gv_Item.IsMappingContextAction = false;
		this.gv_Item.ItemId = "OCCR";
		this.gv_Item.LstColumnAllowBatch = null;
		this.gv_Item.LstColumnNotAllowBatch = null;
		this.gv_Item.LstColumnNotEditable = null;
		this.gv_Item.LstColumNotSetting = null;
		this.gv_Item.Name = "gv_Item";
		this.gv_Item.OptionsSelection.MultiSelect = true;
		this.gv_Item.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_Item.OptionsView.ShowGroupPanel = false;
		this.gv_Item.UseBatchMod = false;
		this.gv_Item.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Item_CustomDrawRowIndicator);
		this.gv_Item.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gv_Item_CustomColumnDisplayText);
		this.ItemNo.Caption = "Item No";
		this.ItemNo.ColumnEdit = this.btn_ItemCode;
		this.ItemNo.CurrencyDateField = null;
		this.ItemNo.CurrencyField = null;
		this.ItemNo.CurrencyFixed = null;
		this.ItemNo.CurrencyFixedRate = 0.0;
		this.ItemNo.CurrencyRateField = null;
		this.ItemNo.FieldName = "ItemCode";
		this.ItemNo.Name = "ItemNo";
		this.ItemNo.PrimaryKey = true;
		this.ItemNo.UnitField = null;
		this.ItemNo.Visible = true;
		this.ItemNo.VisibleIndex = 1;
		this.btn_ItemCode.AutoHeight = false;
		this.btn_ItemCode.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.btn_ItemCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_ItemCode.ContainerControl = this;
		this.btn_ItemCode.DisplayMember = "ItemCode";
		this.btn_ItemCode.DisplayMember2 = "";
		this.btn_ItemCode.LoadDataSourceWhenEnter = false;
		this.btn_ItemCode.Name = "btn_ItemCode";
		this.btn_ItemCode.SeparateCharacterForMulti = ';';
		this.btn_ItemCode.SeparateString = ", ";
		this.btn_ItemCode.UsingAutoCompleteBox = false;
		this.btn_ItemCode.UsingDataSource = true;
		this.btn_ItemCode.ValidateWhenNoDataSource = false;
		this.btn_ItemCode.ValueMember = "ItemCode";
		this.ItemDescription.Caption = "Item Description";
		this.ItemDescription.CurrencyDateField = null;
		this.ItemDescription.CurrencyField = null;
		this.ItemDescription.CurrencyFixed = null;
		this.ItemDescription.CurrencyFixedRate = 0.0;
		this.ItemDescription.CurrencyRateField = null;
		this.ItemDescription.FieldName = "ItemName";
		this.ItemDescription.Name = "ItemDescription";
		this.ItemDescription.PrimaryKey = false;
		this.ItemDescription.UnitField = null;
		this.ItemDescription.Visible = true;
		this.ItemDescription.VisibleIndex = 2;
		this.WhsCode.Caption = "Ware House";
		this.WhsCode.ColumnEdit = this.btn_WhsCode;
		this.WhsCode.CurrencyDateField = null;
		this.WhsCode.CurrencyField = null;
		this.WhsCode.CurrencyFixed = null;
		this.WhsCode.CurrencyFixedRate = 0.0;
		this.WhsCode.CurrencyRateField = null;
		this.WhsCode.FieldName = "WhsCode";
		this.WhsCode.Name = "WhsCode";
		this.WhsCode.PrimaryKey = false;
		this.WhsCode.UnitField = null;
		this.WhsCode.Visible = true;
		this.WhsCode.VisibleIndex = 3;
		this.btn_WhsCode.AutoHeight = false;
		this.btn_WhsCode.AutoLoadDataSource = true;
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions4.Image");
		this.btn_WhsCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_WhsCode.ContainerControl = this;
		this.btn_WhsCode.DisplayMember = "WhsCode";
		this.btn_WhsCode.DisplayMember2 = "";
		this.btn_WhsCode.LoadDataSourceWhenEnter = false;
		this.btn_WhsCode.Name = "btn_WhsCode";
		this.btn_WhsCode.SeparateCharacterForMulti = ';';
		this.btn_WhsCode.SeparateString = ", ";
		this.btn_WhsCode.UsingAutoCompleteBox = false;
		this.btn_WhsCode.UsingDataSource = true;
		this.btn_WhsCode.ValidateWhenNoDataSource = false;
		this.btn_WhsCode.ValueMember = "WhsCode";
		this.SublevelCode.Caption = "Sublevel Code";
		this.SublevelCode.CurrencyDateField = null;
		this.SublevelCode.CurrencyField = null;
		this.SublevelCode.CurrencyFixed = null;
		this.SublevelCode.CurrencyFixedRate = 0.0;
		this.SublevelCode.CurrencyRateField = null;
		this.SublevelCode.Name = "SublevelCode";
		this.SublevelCode.PrimaryKey = false;
		this.SublevelCode.UnitField = null;
		this.CycleCode.Caption = "Cycle Code";
		this.CycleCode.CurrencyDateField = null;
		this.CycleCode.CurrencyField = null;
		this.CycleCode.CurrencyFixed = null;
		this.CycleCode.CurrencyFixedRate = 0.0;
		this.CycleCode.CurrencyRateField = null;
		this.CycleCode.FieldName = "Name";
		this.CycleCode.Name = "CycleCode";
		this.CycleCode.PrimaryKey = false;
		this.CycleCode.UnitField = null;
		this.CycleCode.Visible = true;
		this.CycleCode.VisibleIndex = 4;
		this.Recurrence.Caption = "Recurrence";
		this.Recurrence.CurrencyDateField = null;
		this.Recurrence.CurrencyField = null;
		this.Recurrence.CurrencyFixed = null;
		this.Recurrence.CurrencyFixedRate = 0.0;
		this.Recurrence.CurrencyRateField = null;
		this.Recurrence.FieldName = "Frequency";
		this.Recurrence.Name = "Recurrence";
		this.Recurrence.PrimaryKey = false;
		this.Recurrence.UnitField = null;
		this.Recurrence.Visible = true;
		this.Recurrence.VisibleIndex = 5;
		this.NextDate.Caption = "Date";
		this.NextDate.CurrencyDateField = null;
		this.NextDate.CurrencyField = null;
		this.NextDate.CurrencyFixed = null;
		this.NextDate.CurrencyFixedRate = 0.0;
		this.NextDate.CurrencyRateField = null;
		this.NextDate.FieldName = "NextDate";
		this.NextDate.Name = "NextDate";
		this.NextDate.PrimaryKey = false;
		this.NextDate.UnitField = null;
		this.NextDate.Visible = true;
		this.NextDate.VisibleIndex = 6;
		this.Time.Caption = "Time";
		this.Time.CurrencyDateField = null;
		this.Time.CurrencyField = null;
		this.Time.CurrencyFixed = null;
		this.Time.CurrencyFixedRate = 0.0;
		this.Time.CurrencyRateField = null;
		this.Time.FieldName = "Hour";
		this.Time.Name = "Time";
		this.Time.PrimaryKey = false;
		this.Time.UnitField = null;
		this.Time.Visible = true;
		this.Time.VisibleIndex = 7;
		this.Counted.Caption = "Counted";
		this.Counted.ColumnEdit = this.check_Counted;
		this.Counted.CurrencyDateField = null;
		this.Counted.CurrencyField = null;
		this.Counted.CurrencyFixed = null;
		this.Counted.CurrencyFixedRate = 0.0;
		this.Counted.CurrencyRateField = null;
		this.Counted.FieldName = "Counted";
		this.Counted.Name = "Counted";
		this.Counted.PrimaryKey = false;
		this.Counted.UnitField = null;
		this.Counted.Visible = true;
		this.Counted.VisibleIndex = 8;
		this.check_Counted.AutoHeight = false;
		this.check_Counted.Name = "check_Counted";
		this.check_Counted.ValueChecked = "Y";
		this.check_Counted.ValueGrayed = "N";
		this.check_Counted.ValueUnchecked = "N";
		this.btn_InventoryCounting.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_InventoryCounting.EditControl = null;
		this.btn_InventoryCounting.EditValue = "Inventory Counting";
		this.btn_InventoryCounting.FieldName = null;
		this.btn_InventoryCounting.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_InventoryCounting.ImageOptions.Image");
		this.btn_InventoryCounting.IsSearchControl = false;
		this.btn_InventoryCounting.Location = new System.Drawing.Point(719, 391);
		this.btn_InventoryCounting.Name = "btn_InventoryCounting";
		this.btn_InventoryCounting.ReadOnly = false;
		this.btn_InventoryCounting.SettingInVisible = false;
		this.btn_InventoryCounting.SettingReadonly = false;
		this.btn_InventoryCounting.Size = new System.Drawing.Size(130, 23);
		this.btn_InventoryCounting.TabIndex = 28;
		this.btn_InventoryCounting.Text = "Inventory Counting";
		this.btn_InventoryCounting.Click += new System.EventHandler(this.btn_InventoryCounting_Click);
		this.btn_Huy.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Huy.EditControl = null;
		this.btn_Huy.EditValue = "OK";
		this.btn_Huy.FieldName = null;
		this.btn_Huy.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Huy.ImageOptions.Image");
		this.btn_Huy.IsSearchControl = false;
		this.btn_Huy.Location = new System.Drawing.Point(12, 391);
		this.btn_Huy.Name = "btn_Huy";
		this.btn_Huy.ReadOnly = false;
		this.btn_Huy.SettingInVisible = false;
		this.btn_Huy.SettingReadonly = false;
		this.btn_Huy.Size = new System.Drawing.Size(75, 23);
		this.btn_Huy.TabIndex = 29;
		this.btn_Huy.Text = "OK";
		this.btn_Huy.Click += new System.EventHandler(this.btn_Huy_Click);
		this.look_ToItemCode.Location = new System.Drawing.Point(296, 39);
		this.look_ToItemCode.Name = "look_ToItemCode";
		this.look_ToItemCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_ToItemCode.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ItemName", "Name", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ItemCode", "Value")
		});
		this.look_ToItemCode.Properties.DisplayMember = "ItemCode";
		this.look_ToItemCode.Properties.NullText = "";
		this.look_ToItemCode.Properties.ValueMember = "ItemCode";
		this.look_ToItemCode.Size = new System.Drawing.Size(100, 20);
		this.look_ToItemCode.TabIndex = 25;
		this.look_FromItemCode.Location = new System.Drawing.Point(145, 39);
		this.look_FromItemCode.Name = "look_FromItemCode";
		this.look_FromItemCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_FromItemCode.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ItemName", "Name", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ItemCode", "Value")
		});
		this.look_FromItemCode.Properties.DisplayMember = "ItemCode";
		this.look_FromItemCode.Properties.NullText = "";
		this.look_FromItemCode.Properties.ValueMember = "ItemCode";
		this.look_FromItemCode.Size = new System.Drawing.Size(100, 20);
		this.look_FromItemCode.TabIndex = 24;
		base.AcceptButton = this.btn_Huy;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btn_Cancel;
		base.ClientSize = new System.Drawing.Size(852, 418);
		base.Controls.Add(this.btn_Huy);
		base.Controls.Add(this.btn_InventoryCounting);
		base.Controls.Add(this.grd_Item);
		base.Controls.Add(this.look_ToCycleCode);
		base.Controls.Add(this.look_ToItemCode);
		base.Controls.Add(this.look_FromItemCode);
		base.Controls.Add(this.look_FromCycleCode);
		base.Controls.Add(this.checked_WhsCode);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Ok);
		base.Controls.Add(this.labelLine7);
		base.Controls.Add(this.labelLine6);
		base.Controls.Add(this.labelLine5);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.labelLine1);
		base.FormId = "oCycleCountRsc";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_CycleCountRecommendations";
		this.Text = "Cycle Count Recommendations - Selection  Criteria";
		base.OnLoadAsync += new Apzon.Controls.Form.LoadAsyncEventHandler(this.frm_CycleCountRecommendations_Load);
		((System.ComponentModel.ISupportInitialize)this.checked_WhsCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_FromCycleCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_ToCycleCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Item).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Item).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ItemCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_WhsCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.check_Counted).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_ToItemCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_FromItemCode.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
