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
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.UserFields;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Inventory.Item_Management;

public class frm_BatchesControl : XtraFormEx
{
	private const string FormController = "BatchesControl";

	private List<UserFieldsImpl> _lstUDF;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private int argNo;

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	private PanelControl panelControl1;

	private PanelControl panelControl2;

	private SimpleButtonEx btn_Search;

	private ButtonEditEx txt_SalesBlanket;

	private LabelLine labelLine1;

	private LabelLine lb_BusinessPartner;

	private ButtonEditEx txt_SONo;

	private LabelLine labelLine3;

	private ButtonEditEx txt_BPCode;

	private LabelLine labelLine2;

	private GridControlEx grd_Batch;

	private GridViewEx gv_Batch;

	private LookUpEdit cbx_OptionSearch;

	private SimpleButtonEx btn_Update;

	private SimpleButtonEx btn_Cancel;

	private ButtonEditEx txt_Desc;

	public frm_BatchesControl()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_BatchesControl"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_BatchesControl" && x.authoriza == "F"))
			{
				btn_Update.Enabled = false;
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private async void BatchesControl_Load(object sender, EventArgs e)
	{
		try
		{
			List<Common.Table_Code> list = new List<Common.Table_Code>();
			list.Add(new Common.Table_Code
			{
				Name = "Sales Blanket",
				Code = "B"
			});
			list.Add(new Common.Table_Code
			{
				Name = "Sales Order",
				Code = "S"
			});
			list.Add(new Common.Table_Code
			{
				Name = "Customer Code",
				Code = "C"
			});
			list.Add(new Common.Table_Code
			{
				Name = "All",
				Code = "A"
			});
			cbx_OptionSearch.Properties.DataSource = list;
			cbx_OptionSearch.Properties.DisplayMember = "Name";
			cbx_OptionSearch.Properties.ValueMember = "Code";
			_lstUDF = await _httpClient.Udf.GetListField("OBTN");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_BPCode_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Tab && string.IsNullOrEmpty(txt_BPCode.Text))
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Bp, false, txt_BPCode.Text, "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ShowDialog(this);
				DataRow chooseItem = frm_CFL2.ChooseItem;
				if (chooseItem != null && chooseItem.IsExistsColumn("CardCode"))
				{
					txt_BPCode.Text = Function.ToString(chooseItem["CardCode"]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_BPCode_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Bp, false, txt_BPCode.Text, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			DataRow chooseItem = frm_CFL2.ChooseItem;
			if (chooseItem != null && chooseItem.IsExistsColumn("CardCode"))
			{
				txt_BPCode.Text = Function.ToString(chooseItem["CardCode"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Search_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			WinCommons._OpenProcessing();
			btn_Search.Enabled = false;
			bool upEn = btn_Update.Enabled;
			btn_Update.Enabled = false;
			cbx_OptionSearch.Enabled = false;
			argNo = Function.ParseInt(txt_SalesBlanket.EditValue);
			string text = Function.ToString(cbx_OptionSearch.EditValue);
			string text2 = Function.ToString(txt_SalesBlanket.EditValue);
			string text3 = Function.ToString(txt_SONo.EditValue);
			string text4 = Function.ToString(txt_BPCode.EditValue);
			if (text.Equals("B") && string.IsNullOrEmpty(text2))
			{
				WinCommons._CloseProcessing();
				XtraMessageBox.Show("Chưa chọn Sales order!", "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
				return;
			}
			if (text.Equals("S") && string.IsNullOrEmpty(text3))
			{
				WinCommons._CloseProcessing();
				XtraMessageBox.Show("Chưa chọn Sales order!", "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
				return;
			}
			if (text.Equals("C") && string.IsNullOrEmpty(text4))
			{
				WinCommons._CloseProcessing();
				XtraMessageBox.Show("Chưa chọn Sales order!", "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
				return;
			}
			DataSet ds = await LoadBatch(text, text2, text3, text4, GlobalData.UserName, GlobalData.DefaultBranch);
			if (ds.IsNotNull())
			{
				await GridViewHelper.GenerateGrid(grd_Batch, gv_Batch, ds, _lstUDF, expand: true);
			}
			btn_Search.Enabled = true;
			btn_Update.Enabled = upEn;
			cbx_OptionSearch.Enabled = true;
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void cbx_OptionSearch_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			switch (Function.ToString(cbx_OptionSearch.EditValue))
			{
			case "B":
				txt_SalesBlanket.Enabled = true;
				txt_BPCode.Enabled = false;
				txt_SONo.Enabled = false;
				btn_Update.Enabled = true;
				break;
			case "S":
				txt_SalesBlanket.Enabled = false;
				txt_BPCode.Enabled = false;
				txt_SONo.Enabled = true;
				btn_Update.Enabled = true;
				break;
			case "C":
				txt_SalesBlanket.Enabled = false;
				txt_BPCode.Enabled = true;
				txt_SONo.Enabled = false;
				btn_Update.Enabled = false;
				break;
			default:
				txt_SalesBlanket.Enabled = true;
				txt_BPCode.Enabled = true;
				txt_SONo.Enabled = true;
				btn_Update.Enabled = true;
				break;
			}
			if (gv_Batch.Columns.Count > 0)
			{
				gv_Batch.Columns.Clear();
			}
			grd_Batch.DataSource = new DataTable();
			grd_Batch.RefreshDataSource();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_SalesBlanket_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Tab && string.IsNullOrEmpty(txt_SalesBlanket.Text))
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.UserDefinedCfl, false, txt_SalesBlanket.Text, "Apz_Cfl_LoadSalesBlanket", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ShowDialog(this);
				DataRow chooseItem = frm_CFL2.ChooseItem;
				if (chooseItem != null && chooseItem.IsExistsColumn("AgrNo"))
				{
					txt_SalesBlanket.Text = Function.ToString(chooseItem["AgrNo"]);
				}
				if (chooseItem != null && chooseItem.IsExistsColumn("Descript"))
				{
					txt_Desc.Text = Function.ToString(chooseItem["Descript"]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_SalesBlanket_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.UserDefinedCfl, false, txt_SalesBlanket.Text, "Apz_Cfl_LoadSalesBlanket", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			DataRow chooseItem = frm_CFL2.ChooseItem;
			if (chooseItem != null && chooseItem.IsExistsColumn("AgrNo"))
			{
				txt_SalesBlanket.Text = Function.ToString(chooseItem["AgrNo"]);
			}
			if (chooseItem != null && chooseItem.IsExistsColumn("Descript"))
			{
				txt_Desc.Text = Function.ToString(chooseItem["Descript"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataSet> LoadBatch(string option, string sb, string so, string bp, string userName, int branch)
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "BatchesControl");
			return await helper.GetAsync("LoadBatch", new { option, sb, so, bp, userName, branch });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
	}

	private void gv_Batch_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (gv_Batch.IsExistColumn("Checked") && e.Column.FieldName != "Checked")
			{
				gv_Batch.SetRowCellValue(e.RowHandle, "Checked", true);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void btn_Update_Click(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			string text = Function.ToString(cbx_OptionSearch.EditValue);
			DataRow[] array = grd_Batch.DataSourceTable.Select($"Checked='{true}' AND DistNumber <> ''");
			if ((array == null || array.Length == 0) && !text.Equals("B"))
			{
				XtraMessageBox.Show("Chưa chọn hàng!", "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
				return;
			}
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			btn_Search.Enabled = false;
			btn_Update.Enabled = false;
			string text2 = "";
			switch (text)
			{
			case "B":
			{
				string text3 = "";
				DataRow[] array2 = array;
				foreach (DataRow dataRow in array2)
				{
					text3 = text3 + "," + Function.ToString(dataRow["DistNumber"]);
				}
				if (!string.IsNullOrEmpty(text3) && text3.StartsWith(","))
				{
					text3 = text3.Substring(1, text3.Length - 1);
				}
				text2 = await UpdateBatchSalesBlanket(text3, argNo);
				break;
			}
			case "S":
				text2 = await UpdateBatchForSales(array.CopyToDataTable());
				break;
			case "A":
				text2 = await UpdateBatchDetails(array.CopyToDataTable());
				break;
			}
			if (string.IsNullOrEmpty(text2))
			{
				WinCommons.writeStatus(Enums.MessageCode.SaveSuccess, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
				btn_Search.Enabled = true;
				btn_Update.Enabled = true;
				btn_Search_Click(null, null);
			}
			else
			{
				WinCommons.writeStatus(Enums.MessageCode.None, Enums.MessageType.Error, Msg_Time._TimeMedium, text2);
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		btn_Search.Enabled = true;
		btn_Update.Enabled = true;
	}

	private async Task<string> UpdateBatchSalesBlanket(string batches, int argNo)
	{
		try
		{
			using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "BatchesControl");
			return await helper.GetAsync("UpdateBatchSalesBlanket", new { batches, argNo });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private async Task<string> UpdateBatchForSales(DataTable dtBatch)
	{
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BatchesControl");
			return await _help.PostAsJsonAsync("UpdateBatchForSales", dtBatch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private async Task<string> UpdateBatchDetails(DataTable dtBatch)
	{
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BatchesControl");
			return await _help.PostAsJsonAsync("UpdateBatchDetails", dtBatch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Inventory.Item_Management.frm_BatchesControl));
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.txt_Desc = new Apzon.Controls.Editor.ButtonEditEx();
		this.cbx_OptionSearch = new DevExpress.XtraEditors.LookUpEdit();
		this.txt_SONo = new Apzon.Controls.Editor.ButtonEditEx();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.txt_BPCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.txt_SalesBlanket = new Apzon.Controls.Editor.ButtonEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.lb_BusinessPartner = new Apzon.Controls.Controls.LabelLine();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Update = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Batch = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Batch = new Apzon.Controls.Grid.GridViewEx();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_Desc.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbx_OptionSearch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_SONo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BPCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_SalesBlanket.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl2).BeginInit();
		this.panelControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Batch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Batch).BeginInit();
		base.SuspendLayout();
		this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.panelControl1);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.panelControl2);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(1054, 637);
		this.splitContainerControl1.SplitterPosition = 67;
		this.splitContainerControl1.TabIndex = 0;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.panelControl1.Controls.Add(this.txt_Desc);
		this.panelControl1.Controls.Add(this.cbx_OptionSearch);
		this.panelControl1.Controls.Add(this.txt_SONo);
		this.panelControl1.Controls.Add(this.labelLine3);
		this.panelControl1.Controls.Add(this.txt_BPCode);
		this.panelControl1.Controls.Add(this.labelLine2);
		this.panelControl1.Controls.Add(this.txt_SalesBlanket);
		this.panelControl1.Controls.Add(this.labelLine1);
		this.panelControl1.Controls.Add(this.lb_BusinessPartner);
		this.panelControl1.Controls.Add(this.btn_Search);
		this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl1.Location = new System.Drawing.Point(0, 0);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(1054, 67);
		this.panelControl1.TabIndex = 0;
		this.txt_Desc.EditControl = null;
		this.txt_Desc.FieldName = null;
		this.txt_Desc.IsSearchControl = false;
		this.txt_Desc.Location = new System.Drawing.Point(595, 11);
		this.txt_Desc.Name = "txt_Desc";
		this.txt_Desc.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 6.5f);
		this.txt_Desc.Properties.Appearance.Options.UseFont = true;
		this.txt_Desc.Properties.ReadOnly = true;
		this.txt_Desc.SettingInVisible = false;
		this.txt_Desc.SettingReadonly = false;
		this.txt_Desc.Size = new System.Drawing.Size(172, 18);
		this.txt_Desc.TabIndex = 70;
		this.cbx_OptionSearch.Location = new System.Drawing.Point(116, 9);
		this.cbx_OptionSearch.Name = "cbx_OptionSearch";
		this.cbx_OptionSearch.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbx_OptionSearch.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.cbx_OptionSearch.Properties.DisplayMember = "Name";
		this.cbx_OptionSearch.Properties.NullText = "";
		this.cbx_OptionSearch.Properties.ShowHeader = false;
		this.cbx_OptionSearch.Properties.ValueMember = "Code";
		this.cbx_OptionSearch.Size = new System.Drawing.Size(172, 20);
		this.cbx_OptionSearch.TabIndex = 68;
		this.cbx_OptionSearch.EditValueChanged += new System.EventHandler(this.cbx_OptionSearch_EditValueChanged);
		this.txt_SONo.EditControl = null;
		this.txt_SONo.FieldName = null;
		this.txt_SONo.IsSearchControl = false;
		this.txt_SONo.Location = new System.Drawing.Point(116, 34);
		this.txt_SONo.Name = "txt_SONo";
		this.txt_SONo.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 6.5f);
		this.txt_SONo.Properties.Appearance.Options.UseFont = true;
		this.txt_SONo.SettingInVisible = false;
		this.txt_SONo.SettingReadonly = false;
		this.txt_SONo.Size = new System.Drawing.Size(172, 18);
		this.txt_SONo.TabIndex = 67;
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Sales Blanket";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(313, 11);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(107, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 66;
		this.labelLine3.Utext = "Sales Blanket";
		this.txt_BPCode.EditControl = null;
		this.txt_BPCode.FieldName = null;
		this.txt_BPCode.IsSearchControl = false;
		this.txt_BPCode.Location = new System.Drawing.Point(417, 34);
		this.txt_BPCode.Name = "txt_BPCode";
		this.txt_BPCode.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 6.5f);
		this.txt_BPCode.Properties.Appearance.Options.UseFont = true;
		this.txt_BPCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.txt_BPCode.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txt_BPCode_Properties_ButtonClick);
		this.txt_BPCode.SettingInVisible = false;
		this.txt_BPCode.SettingReadonly = false;
		this.txt_BPCode.Size = new System.Drawing.Size(172, 18);
		this.txt_BPCode.TabIndex = 65;
		this.txt_BPCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_BPCode_KeyDown);
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "BP Code";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(313, 34);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(107, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 64;
		this.labelLine2.Utext = "BP Code";
		this.txt_SalesBlanket.EditControl = null;
		this.txt_SalesBlanket.FieldName = null;
		this.txt_SalesBlanket.IsSearchControl = false;
		this.txt_SalesBlanket.Location = new System.Drawing.Point(417, 11);
		this.txt_SalesBlanket.Name = "txt_SalesBlanket";
		this.txt_SalesBlanket.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 6.5f);
		this.txt_SalesBlanket.Properties.Appearance.Options.UseFont = true;
		this.txt_SalesBlanket.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.txt_SalesBlanket.Properties.DisplayFormat.FormatString = "n0";
		this.txt_SalesBlanket.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txt_SalesBlanket.Properties.EditFormat.FormatString = "n0";
		this.txt_SalesBlanket.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txt_SalesBlanket.Properties.Mask.EditMask = "n0";
		this.txt_SalesBlanket.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txt_SalesBlanket_Properties_ButtonClick);
		this.txt_SalesBlanket.SettingInVisible = false;
		this.txt_SalesBlanket.SettingReadonly = false;
		this.txt_SalesBlanket.Size = new System.Drawing.Size(172, 18);
		this.txt_SalesBlanket.TabIndex = 62;
		this.txt_SalesBlanket.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_SalesBlanket_KeyDown);
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "SO No";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(12, 34);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(107, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 61;
		this.labelLine1.Utext = "SO No";
		this.lb_BusinessPartner.EditControl = null;
		this.lb_BusinessPartner.EditValue = "Option search";
		this.lb_BusinessPartner.FieldName = null;
		this.lb_BusinessPartner.HideWhenEmpty = false;
		this.lb_BusinessPartner.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_BusinessPartner.Location = new System.Drawing.Point(12, 12);
		this.lb_BusinessPartner.Name = "lb_BusinessPartner";
		this.lb_BusinessPartner.ReadOnly = false;
		this.lb_BusinessPartner.SettingInVisible = false;
		this.lb_BusinessPartner.SettingReadonly = false;
		this.lb_BusinessPartner.ShowUnderLine = true;
		this.lb_BusinessPartner.Size = new System.Drawing.Size(107, 20);
		this.lb_BusinessPartner.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_BusinessPartner.TabIndex = 60;
		this.lb_BusinessPartner.Utext = "Option search";
		this.btn_Search.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Search.Appearance.Options.UseBackColor = true;
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = false;
		this.btn_Search.Location = new System.Drawing.Point(595, 34);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(99, 23);
		this.btn_Search.TabIndex = 22;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.panelControl2.Controls.Add(this.btn_Cancel);
		this.panelControl2.Controls.Add(this.btn_Update);
		this.panelControl2.Controls.Add(this.grd_Batch);
		this.panelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl2.Location = new System.Drawing.Point(0, 0);
		this.panelControl2.Name = "panelControl2";
		this.panelControl2.Size = new System.Drawing.Size(1054, 560);
		this.panelControl2.TabIndex = 1;
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(149, 537);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(90, 23);
		this.btn_Cancel.TabIndex = 36;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.ToolTip = "Thoát";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_Update.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Update.EditControl = null;
		this.btn_Update.EditValue = "Update Batch Detal";
		this.btn_Update.FieldName = null;
		this.btn_Update.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Update.ImageOptions.Image");
		this.btn_Update.IsSearchControl = false;
		this.btn_Update.Location = new System.Drawing.Point(12, 537);
		this.btn_Update.Name = "btn_Update";
		this.btn_Update.ReadOnly = false;
		this.btn_Update.SettingInVisible = false;
		this.btn_Update.SettingReadonly = false;
		this.btn_Update.Size = new System.Drawing.Size(131, 23);
		this.btn_Update.TabIndex = 35;
		this.btn_Update.Text = "Update Batch Detal";
		this.btn_Update.Click += new System.EventHandler(this.btn_Update_Click);
		this.grd_Batch.AllowPasteExcel = true;
		this.grd_Batch.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Batch.ClipboardData = null;
		this.grd_Batch.Location = new System.Drawing.Point(2, 0);
		this.grd_Batch.MainView = this.gv_Batch;
		this.grd_Batch.Name = "grd_Batch";
		this.grd_Batch.Size = new System.Drawing.Size(1052, 531);
		this.grd_Batch.TabIndex = 0;
		this.grd_Batch.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Batch });
		this.gv_Batch.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Batch.ColumnKey = null;
		this.gv_Batch.DisableFormSetting = false;
		this.gv_Batch.DisplayException = false;
		this.gv_Batch.EditColumn = null;
		this.gv_Batch.FieldCurrency = null;
		this.gv_Batch.FieldRate = null;
		this.gv_Batch.FormId = "oBatchControl";
		this.gv_Batch.GridControl = this.grd_Batch;
		this.gv_Batch.IgnoreFilter = false;
		this.gv_Batch.IndexEditRow = -1;
		this.gv_Batch.IndicatorWidth = 35;
		this.gv_Batch.IsApzTable = false;
		this.gv_Batch.IsInsertNewRow = false;
		this.gv_Batch.IsLoadingData = false;
		this.gv_Batch.IsMappingContextAction = false;
		this.gv_Batch.ItemId = "OBATCH";
		this.gv_Batch.LstColumnAllowBatch = null;
		this.gv_Batch.LstColumnNotAllowBatch = null;
		this.gv_Batch.LstColumnNotEditable = null;
		this.gv_Batch.LstColumNotSetting = null;
		this.gv_Batch.Name = "gv_Batch";
		this.gv_Batch.UseBatchMod = false;
		this.gv_Batch.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Batch_CellValueChanged);
		base.AcceptButton = this.btn_Search;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1054, 637);
		base.Controls.Add(this.splitContainerControl1);
		base.FormId = "oBatchControl";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_BatchesControl";
		base.ShowInTaskbar = false;
		this.Text = "Batches Control";
		base.Load += new System.EventHandler(this.BatchesControl_Load);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_Desc.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbx_OptionSearch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_SONo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BPCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_SalesBlanket.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl2).EndInit();
		this.panelControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Batch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Batch).EndInit();
		base.ResumeLayout(false);
	}
}
