using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Packing_Management;

public class frm_PrintBillWrap : XtraFormEx
{
	private IContainer components;

	private PanelControl panelControl1;

	private SimpleButton simpleBtn_Print;

	private LabelControl labelCtrl_PackingOrder;

	private PanelControl panelControl3;

	private SimpleButton simpleBtn_RemoveContainer;

	private SimpleButton simpleBtn_ComplContainer;

	private LabelControl labelCtrl_Mathung;

	private LabelControl labelCtrl_BarCode;

	private SimpleButton simpleBtn_RemoveItem;

	private GridControlEx gridControl_Case;

	private GridViewEx gridView_Case;

	private GridControlEx gridControl_Item;

	private GridViewEx gridView_Item;

	private GridColumnEx CaseCode;

	private GridColumnEx gridColumnEx1;

	private ButtonEditCFL bte_PO;

	private TextEditEx txtBox_CodeCase;

	private TextEditEx txtBox_Code;

	private LabelControl labelControl_ItemNum;

	private LabelControlEx label_Amount;

	private LabelControlEx AmountPO;

	private GridColumnEx LỉneNumber;

	private string _LineNum { get; set; }

	private string _CaseCode { get; set; }

	private int _LineNumber { get; set; }

	private string _BundleCode { get; set; }

	private int _AmountItem { get; set; }

	public int _AmountPO { get; set; }

	public frm_PrintBillWrap()
	{
		InitializeComponent();
	}

	public async void frm_PrintBillWrap_Load(object sender, EventArgs e)
	{
		labelControl_ItemNum.Text = "";
		try
		{
			RepositoryItemButtonEditCFL properties = bte_PO.Properties;
			properties.HandleChooseItem = (Action<DataRow>)Delegate.Combine(properties.HandleChooseItem, (Action<DataRow>)((DataRow chooseItem) =>
			{
				if (chooseItem != null)
				{
					bte_PO.EditValue = chooseItem["POCode"];
					string amountofPO = chooseItem["AmountItem"].ToString();
					showAmountPO(amountofPO);
					GetCase();
					txtBox_CodeCase.Focus();
				}
			}));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void GetCase()
	{
		try
		{
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "PrintBillWrap");
			int pOCode = Function.ParseInt(bte_PO.Text.Trim());
			DataTable dataSource = await http.GetAsync("GetCase", new
			{
				POCode = pOCode
			});
			gridControl_Case.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void Check(int a)
	{
		try
		{
			object obj = (gridControl_Case.DataSource as DataTable).Rows[a]["Amount"];
			_AmountItem = gridView_Item.RowCount;
			if (_AmountItem >= Function.ParseInt(obj))
			{
				txtBox_CodeCase.Focus();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void showAmountPO(string AmountofPO)
	{
		try
		{
			_AmountPO = gridView_Case.RowCount;
			AmountPO.Text = "Amount of PO " + AmountofPO;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void showAmountItem()
	{
		try
		{
			_AmountItem = gridView_Item.RowCount;
			if (_AmountItem < 10)
			{
				labelControl_ItemNum.Text = "0" + _AmountItem;
			}
			else
			{
				labelControl_ItemNum.Text = _AmountItem.ToString();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void GetItemByLineNumber(int _LineNumber)
	{
		using (HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Traceability"))
		{
			DataTable dataSource = await http.GetAsync("GetItemByLineNumber", new
			{
				LineNumber = _LineNumber
			});
			gridControl_Item.DataSource = dataSource;
		}
		showAmountItem();
		int focusedRowHandle = gridView_Case.FocusedRowHandle;
		Check(focusedRowHandle);
		object obj = (gridControl_Case.DataSource as DataTable).Rows[focusedRowHandle]["Amount"];
		label_Amount.Text = "Amount Case " + obj.ToString();
	}

	public async void FocusedRowObjectChange_Case(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			_LineNumber = Function.ParseInt(gridView_Case.GetRowCellValue(gridView_Case.FocusedRowHandle, "LineNumber"));
			GetItemByLineNumber(_LineNumber);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void simpleBtn_RemoveContainer_Click(object sender, EventArgs e)
	{
		try
		{
			if (XtraMessageBox.Show(Resources.xtraMsg_Delete, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return;
			}
			_LineNumber = Function.ParseInt(gridView_Case.GetRowCellValue(gridView_Case.FocusedRowHandle, "LineNumber"));
			using (HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "PrintBillWrap"))
			{
				WinCommons._OpenProcessing();
				string text = await http.GetAsync("DelCase", new
				{
					LineNumber = _LineNumber
				});
				if (string.IsNullOrWhiteSpace(text))
				{
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
				WinCommons._CloseProcessing();
			}
			GetCase();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void simpleBtn_RemoveItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (XtraMessageBox.Show(Resources.xtraMsg_Delete, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				_LineNum = Function.ToString(gridView_Item.GetRowCellValue(gridView_Item.FocusedRowHandle, "LineNum"));
				using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "PrintBillWrap");
				WinCommons._OpenProcessing();
				string text = await http.GetAsync("DelItem", new
				{
					LineNum = _LineNum
				});
				if (string.IsNullOrWhiteSpace(text))
				{
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
				WinCommons._CloseProcessing();
			}
			_LineNumber = Function.ParseInt(gridView_Case.GetRowCellValue(gridView_Case.FocusedRowHandle, "LineNumber"));
			GetItemByLineNumber(_LineNumber);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void ItemCode_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			_LineNumber = Function.ParseInt(gridView_Case.GetRowCellValue(gridView_Case.FocusedRowHandle, "LineNumber"));
			if (e.KeyCode != Keys.Return)
			{
				return;
			}
			if (txtBox_Code.Text.Trim() != null)
			{
				_CaseCode = Function.ToString(gridView_Case.GetRowCellValue(gridView_Case.FocusedRowHandle, "CaseCode"));
				_BundleCode = txtBox_Code.Text;
				WinCommons._OpenProcessing();
				using (HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "PrintBillWrap"))
				{
					string text = await http.PostAsync("InsertItem", new Tuple<string, string, int>(_CaseCode, _BundleCode, _LineNumber));
					txtBox_Code.Text = null;
					if (string.IsNullOrWhiteSpace(text))
					{
						WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, text);
					}
					WinCommons._CloseProcessing();
				}
				GetItemByLineNumber(_LineNumber);
			}
			else
			{
				XtraMessageBox.Show("Barcode must fill");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void Case_KeyDown(object sender, KeyEventArgs e)
	{
		_ = 1;
		try
		{
			if (e.KeyCode != Keys.Return)
			{
				return;
			}
			if (bte_PO.Text != "Choose PO" && txtBox_CodeCase.Text.Trim() != null)
			{
				int userID = GlobalData.UserID;
				int item = int.Parse(bte_PO.Text);
				_CaseCode = txtBox_CodeCase.Text;
				using (HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "PrintBillWrap"))
				{
					WinCommons._OpenProcessing();
					string text = await http.PostAsync("InsertCase", new Tuple<string, int, int>(_CaseCode, item, userID));
					if (string.IsNullOrWhiteSpace(text))
					{
						WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, text);
						XtraMessageBox.Show("Add Case Fail, Check your case code again");
					}
				}
				WinCommons._CloseProcessing();
				using HttpClientHelper<DataTable> http2 = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Traceability");
				DataTable dataSource = await http2.GetAsync("GetCase");
				gridControl_Case.DataSource = dataSource;
			}
			else
			{
				XtraMessageBox.Show("Choose PO then fill Case code");
			}
			txtBox_CodeCase.Text = null;
			GetCase();
			txtBox_Code.Focus();
			_AmountPO = gridView_Case.RowCount;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void simpleBtn_ComplContainer_Click(object sender, EventArgs e)
	{
		try
		{
			txtBox_CodeCase.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void frm_PrintBillWrap_FormClosing(object sender, FormClosingEventArgs e)
	{
	}

	private void bte_PO_EditValueChanged(object sender, EventArgs e)
	{
		GetCase();
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
		new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Packing_Management.frm_PrintBillWrap));
		DevExpress.XtraGrid.GridLevelNode gridLevelNode = new DevExpress.XtraGrid.GridLevelNode();
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.AmountPO = new Apzon.Controls.Controls.LabelControlEx();
		this.label_Amount = new Apzon.Controls.Controls.LabelControlEx();
		this.bte_PO = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.simpleBtn_Print = new DevExpress.XtraEditors.SimpleButton();
		this.labelCtrl_PackingOrder = new DevExpress.XtraEditors.LabelControl();
		this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
		this.labelControl_ItemNum = new DevExpress.XtraEditors.LabelControl();
		this.simpleBtn_RemoveContainer = new DevExpress.XtraEditors.SimpleButton();
		this.simpleBtn_ComplContainer = new DevExpress.XtraEditors.SimpleButton();
		this.labelCtrl_Mathung = new DevExpress.XtraEditors.LabelControl();
		this.labelCtrl_BarCode = new DevExpress.XtraEditors.LabelControl();
		this.simpleBtn_RemoveItem = new DevExpress.XtraEditors.SimpleButton();
		this.gridControl_Case = new Apzon.Controls.Grid.GridControlEx();
		this.gridView_Case = new Apzon.Controls.Grid.GridViewEx();
		this.CaseCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gridControl_Item = new Apzon.Controls.Grid.GridControlEx();
		this.gridView_Item = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.txtBox_CodeCase = new Apzon.Controls.Editor.TextEditEx();
		this.txtBox_Code = new Apzon.Controls.Editor.TextEditEx();
		this.LỉneNumber = new Apzon.Controls.Grid.GridColumnEx();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.bte_PO.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl3).BeginInit();
		this.panelControl3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gridControl_Case).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView_Case).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridControl_Item).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView_Item).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtBox_CodeCase.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtBox_Code.Properties).BeginInit();
		base.SuspendLayout();
		this.panelControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panelControl1.Controls.Add(this.AmountPO);
		this.panelControl1.Controls.Add(this.label_Amount);
		this.panelControl1.Controls.Add(this.bte_PO);
		this.panelControl1.Controls.Add(this.simpleBtn_Print);
		this.panelControl1.Controls.Add(this.labelCtrl_PackingOrder);
		this.panelControl1.Location = new System.Drawing.Point(1, 0);
		this.panelControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(681, 50);
		this.panelControl1.TabIndex = 0;
		this.AmountPO.FieldName = null;
		this.AmountPO.Location = new System.Drawing.Point(243, 18);
		this.AmountPO.Name = "AmountPO";
		this.AmountPO.Size = new System.Drawing.Size(67, 13);
		this.AmountPO.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.AmountPO.TabIndex = 34;
		this.AmountPO.Text = "Amount of PO";
		this.label_Amount.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.label_Amount.FieldName = "Amount";
		this.label_Amount.Location = new System.Drawing.Point(382, 19);
		this.label_Amount.Name = "label_Amount";
		this.label_Amount.Size = new System.Drawing.Size(64, 13);
		this.label_Amount.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.label_Amount.TabIndex = 33;
		this.label_Amount.Text = "Amount Case";
		this.bte_PO.EnterMoveNextControl = true;
		this.bte_PO.FieldName = "POCode";
		this.bte_PO.IsSearchControl = false;
		this.bte_PO.Location = new System.Drawing.Point(101, 15);
		this.bte_PO.Name = "bte_PO";
		this.bte_PO.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.bte_PO.Properties.Appearance.Options.UseFont = true;
		this.bte_PO.Properties.AutoHeight = false;
		this.bte_PO.Properties.AutoLoadDataSource = true;
		this.bte_PO.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
		this.bte_PO.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_PO.Properties.ContainerControl = this;
		this.bte_PO.Properties.DisplayMember = "";
		this.bte_PO.Properties.DisplayMember2 = "";
		this.bte_PO.Properties.LoadDataSourceWhenEnter = false;
		this.bte_PO.Properties.SeparateCharacterForMulti = ';';
		this.bte_PO.Properties.SeparateString = ", ";
		this.bte_PO.Properties.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.GetPOforWrap;
		this.bte_PO.Properties.UsingAutoCompleteBox = false;
		this.bte_PO.Properties.UsingDataSource = true;
		this.bte_PO.Properties.ValidateWhenNoDataSource = false;
		this.bte_PO.Properties.ValueMember = "";
		this.bte_PO.Size = new System.Drawing.Size(131, 21);
		this.bte_PO.TabIndex = 31;
		this.bte_PO.ToolTip = "Input PO by keyboard or choose";
		this.bte_PO.EditValueChanged += new System.EventHandler(this.bte_PO_EditValueChanged);
		this.simpleBtn_Print.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.simpleBtn_Print.Appearance.Font = new System.Drawing.Font("Tahoma", 7.8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.simpleBtn_Print.Appearance.Options.UseFont = true;
		this.simpleBtn_Print.ImageOptions.ImageUri.Uri = "Print;Size16x16";
		this.simpleBtn_Print.Location = new System.Drawing.Point(578, 10);
		this.simpleBtn_Print.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.simpleBtn_Print.Name = "simpleBtn_Print";
		this.simpleBtn_Print.Size = new System.Drawing.Size(95, 32);
		this.simpleBtn_Print.TabIndex = 9;
		this.simpleBtn_Print.Text = "Print \r\nBarcode Case";
		this.labelCtrl_PackingOrder.Appearance.Font = new System.Drawing.Font("Tahoma", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.labelCtrl_PackingOrder.Appearance.Options.UseFont = true;
		this.labelCtrl_PackingOrder.Location = new System.Drawing.Point(9, 18);
		this.labelCtrl_PackingOrder.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.labelCtrl_PackingOrder.Name = "labelCtrl_PackingOrder";
		this.labelCtrl_PackingOrder.Size = new System.Drawing.Size(76, 14);
		this.labelCtrl_PackingOrder.TabIndex = 1;
		this.labelCtrl_PackingOrder.Text = "Packing Order";
		this.panelControl3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panelControl3.Appearance.BackColor = System.Drawing.Color.White;
		this.panelControl3.Appearance.Options.UseBackColor = true;
		this.panelControl3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.panelControl3.Controls.Add(this.labelControl_ItemNum);
		this.panelControl3.Location = new System.Drawing.Point(244, 56);
		this.panelControl3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.panelControl3.Name = "panelControl3";
		this.panelControl3.Size = new System.Drawing.Size(215, 224);
		this.panelControl3.TabIndex = 2;
		this.labelControl_ItemNum.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom;
		this.labelControl_ItemNum.Appearance.Font = new System.Drawing.Font("Calibri", 80.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.labelControl_ItemNum.Appearance.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.labelControl_ItemNum.Appearance.Options.UseFont = true;
		this.labelControl_ItemNum.Appearance.Options.UseForeColor = true;
		this.labelControl_ItemNum.LineLocation = DevExpress.XtraEditors.LineLocation.Center;
		this.labelControl_ItemNum.LineVisible = true;
		this.labelControl_ItemNum.Location = new System.Drawing.Point(55, 42);
		this.labelControl_ItemNum.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.labelControl_ItemNum.Name = "labelControl_ItemNum";
		this.labelControl_ItemNum.Size = new System.Drawing.Size(108, 131);
		this.labelControl_ItemNum.TabIndex = 5;
		this.labelControl_ItemNum.Text = "01";
		this.simpleBtn_RemoveContainer.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.simpleBtn_RemoveContainer.ImageOptions.ImageUri.Uri = "DeleteDataSource;Size16x16";
		this.simpleBtn_RemoveContainer.Location = new System.Drawing.Point(9, 248);
		this.simpleBtn_RemoveContainer.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.simpleBtn_RemoveContainer.Name = "simpleBtn_RemoveContainer";
		this.simpleBtn_RemoveContainer.Size = new System.Drawing.Size(90, 32);
		this.simpleBtn_RemoveContainer.TabIndex = 3;
		this.simpleBtn_RemoveContainer.Text = "Remove \r\nContainer";
		this.simpleBtn_RemoveContainer.Click += new System.EventHandler(this.simpleBtn_RemoveContainer_Click);
		this.simpleBtn_ComplContainer.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.simpleBtn_ComplContainer.ImageOptions.ImageUri.Uri = "Apply;Size16x16";
		this.simpleBtn_ComplContainer.Location = new System.Drawing.Point(140, 248);
		this.simpleBtn_ComplContainer.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.simpleBtn_ComplContainer.Name = "simpleBtn_ComplContainer";
		this.simpleBtn_ComplContainer.Size = new System.Drawing.Size(93, 32);
		this.simpleBtn_ComplContainer.TabIndex = 4;
		this.simpleBtn_ComplContainer.Text = "Complete \r\nContainer";
		this.simpleBtn_ComplContainer.Click += new System.EventHandler(this.simpleBtn_ComplContainer_Click);
		this.labelCtrl_Mathung.Appearance.Font = new System.Drawing.Font("Tahoma", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.labelCtrl_Mathung.Appearance.Options.UseFont = true;
		this.labelCtrl_Mathung.Location = new System.Drawing.Point(9, 61);
		this.labelCtrl_Mathung.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.labelCtrl_Mathung.Name = "labelCtrl_Mathung";
		this.labelCtrl_Mathung.Size = new System.Drawing.Size(55, 14);
		this.labelCtrl_Mathung.TabIndex = 7;
		this.labelCtrl_Mathung.Text = "Mã Thùng";
		this.labelCtrl_BarCode.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelCtrl_BarCode.Appearance.Font = new System.Drawing.Font("Tahoma", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.labelCtrl_BarCode.Appearance.Options.UseFont = true;
		this.labelCtrl_BarCode.Location = new System.Drawing.Point(470, 61);
		this.labelCtrl_BarCode.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.labelCtrl_BarCode.Name = "labelCtrl_BarCode";
		this.labelCtrl_BarCode.Size = new System.Drawing.Size(45, 14);
		this.labelCtrl_BarCode.TabIndex = 8;
		this.labelCtrl_BarCode.Text = "BarCode";
		this.simpleBtn_RemoveItem.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.simpleBtn_RemoveItem.ImageOptions.ImageUri.Uri = "Delete;Size16x16";
		this.simpleBtn_RemoveItem.Location = new System.Drawing.Point(579, 248);
		this.simpleBtn_RemoveItem.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.simpleBtn_RemoveItem.Name = "simpleBtn_RemoveItem";
		this.simpleBtn_RemoveItem.Size = new System.Drawing.Size(93, 32);
		this.simpleBtn_RemoveItem.TabIndex = 13;
		this.simpleBtn_RemoveItem.Text = "Remove\r\nItem";
		this.simpleBtn_RemoveItem.Click += new System.EventHandler(this.simpleBtn_RemoveItem_Click);
		this.gridControl_Case.AllowPasteExcel = false;
		this.gridControl_Case.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.gridControl_Case.ClipboardData = null;
		this.gridControl_Case.Location = new System.Drawing.Point(9, 81);
		this.gridControl_Case.MainView = this.gridView_Case;
		this.gridControl_Case.Name = "gridControl_Case";
		this.gridControl_Case.Size = new System.Drawing.Size(224, 163);
		this.gridControl_Case.TabIndex = 14;
		this.gridControl_Case.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gridView_Case });
		this.gridView_Case.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gridView_Case.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gridView_Case.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView_Case.ColumnKey = null;
		this.gridView_Case.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.CaseCode, this.LỉneNumber });
		this.gridView_Case.DisableFormSetting = false;
		this.gridView_Case.DisplayException = false;
		this.gridView_Case.EditColumn = null;
		this.gridView_Case.FieldCurrency = null;
		this.gridView_Case.FieldRate = null;
		this.gridView_Case.FormId = null;
		this.gridView_Case.GridControl = this.gridControl_Case;
		this.gridView_Case.IndexEditRow = -1;
		this.gridView_Case.IndicatorWidth = 35;
		this.gridView_Case.IsApzTable = false;
		this.gridView_Case.IsInsertNewRow = false;
		this.gridView_Case.IsLoadingData = false;
		this.gridView_Case.ItemId = null;
		this.gridView_Case.LstColumnAllowBatch = null;
		this.gridView_Case.LstColumnNotAllowBatch = null;
		this.gridView_Case.LstColumNotSetting = null;
		this.gridView_Case.Name = "gridView_Case";
		this.gridView_Case.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gridView_Case.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gridView_Case.OptionsPrint.AutoWidth = false;
		this.gridView_Case.OptionsPrint.UsePrintStyles = false;
		this.gridView_Case.OptionsView.BestFitMaxRowCount = 100;
		this.gridView_Case.OptionsView.ColumnAutoWidth = false;
		this.gridView_Case.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridView_Case.OptionsView.ShowGroupedColumns = true;
		this.gridView_Case.OptionsView.ShowGroupPanel = false;
		this.gridView_Case.UseBatchMod = false;
		this.gridView_Case.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.FocusedRowObjectChange_Case);
		this.CaseCode.Caption = "CaseCode";
		this.CaseCode.CurrencyDateField = null;
		this.CaseCode.CurrencyField = null;
		this.CaseCode.CurrencyFixed = null;
		this.CaseCode.CurrencyFixedRate = 0.0;
		this.CaseCode.CurrencyRateField = null;
		this.CaseCode.FieldName = "CaseCode";
		this.CaseCode.Name = "CaseCode";
		this.CaseCode.PrimaryKey = false;
		this.CaseCode.UnitField = null;
		this.CaseCode.Visible = true;
		this.CaseCode.VisibleIndex = 0;
		this.CaseCode.Width = 375;
		this.gridControl_Item.AllowPasteExcel = false;
		this.gridControl_Item.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.gridControl_Item.ClipboardData = null;
		gridLevelNode.RelationName = "Level1";
		this.gridControl_Item.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[1] { gridLevelNode });
		this.gridControl_Item.Location = new System.Drawing.Point(470, 78);
		this.gridControl_Item.MainView = this.gridView_Item;
		this.gridControl_Item.Name = "gridControl_Item";
		this.gridControl_Item.Size = new System.Drawing.Size(202, 163);
		this.gridControl_Item.TabIndex = 15;
		this.gridControl_Item.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gridView_Item });
		this.gridView_Item.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gridView_Item.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gridView_Item.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView_Item.ColumnKey = null;
		this.gridView_Item.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[1] { this.gridColumnEx1 });
		this.gridView_Item.DisableFormSetting = false;
		this.gridView_Item.DisplayException = false;
		this.gridView_Item.EditColumn = null;
		this.gridView_Item.FieldCurrency = null;
		this.gridView_Item.FieldRate = null;
		this.gridView_Item.FormId = null;
		this.gridView_Item.GridControl = this.gridControl_Item;
		this.gridView_Item.IndexEditRow = -1;
		this.gridView_Item.IndicatorWidth = 35;
		this.gridView_Item.IsApzTable = false;
		this.gridView_Item.IsInsertNewRow = false;
		this.gridView_Item.IsLoadingData = false;
		this.gridView_Item.ItemId = null;
		this.gridView_Item.LstColumnAllowBatch = null;
		this.gridView_Item.LstColumnNotAllowBatch = null;
		this.gridView_Item.LstColumNotSetting = null;
		this.gridView_Item.Name = "gridView_Item";
		this.gridView_Item.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gridView_Item.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gridView_Item.OptionsPrint.AutoWidth = false;
		this.gridView_Item.OptionsPrint.UsePrintStyles = false;
		this.gridView_Item.OptionsView.BestFitMaxRowCount = 100;
		this.gridView_Item.OptionsView.ColumnAutoWidth = false;
		this.gridView_Item.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridView_Item.OptionsView.ShowGroupedColumns = true;
		this.gridView_Item.OptionsView.ShowGroupPanel = false;
		this.gridView_Item.UseBatchMod = false;
		this.gridColumnEx1.Caption = "Bundle Code";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "BundleCode";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 0;
		this.gridColumnEx1.Width = 298;
		this.txtBox_CodeCase.FieldName = null;
		this.txtBox_CodeCase.IsSearchControl = false;
		this.txtBox_CodeCase.Location = new System.Drawing.Point(102, 59);
		this.txtBox_CodeCase.Name = "txtBox_CodeCase";
		this.txtBox_CodeCase.Size = new System.Drawing.Size(131, 20);
		this.txtBox_CodeCase.TabIndex = 32;
		this.txtBox_CodeCase.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Case_KeyDown);
		this.txtBox_Code.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txtBox_Code.FieldName = null;
		this.txtBox_Code.IsSearchControl = false;
		this.txtBox_Code.Location = new System.Drawing.Point(521, 55);
		this.txtBox_Code.Name = "txtBox_Code";
		this.txtBox_Code.Size = new System.Drawing.Size(151, 20);
		this.txtBox_Code.TabIndex = 33;
		this.txtBox_Code.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ItemCode_KeyDown);
		this.LỉneNumber.Caption = "LỉneNumber";
		this.LỉneNumber.CurrencyDateField = null;
		this.LỉneNumber.CurrencyField = null;
		this.LỉneNumber.CurrencyFixed = null;
		this.LỉneNumber.CurrencyFixedRate = 0.0;
		this.LỉneNumber.CurrencyRateField = null;
		this.LỉneNumber.FieldName = "LỉneNumber";
		this.LỉneNumber.Name = "LỉneNumber";
		this.LỉneNumber.PrimaryKey = false;
		this.LỉneNumber.UnitField = null;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(682, 287);
		base.Controls.Add(this.txtBox_Code);
		base.Controls.Add(this.txtBox_CodeCase);
		base.Controls.Add(this.gridControl_Item);
		base.Controls.Add(this.gridControl_Case);
		base.Controls.Add(this.simpleBtn_RemoveItem);
		base.Controls.Add(this.labelCtrl_Mathung);
		base.Controls.Add(this.labelCtrl_BarCode);
		base.Controls.Add(this.simpleBtn_ComplContainer);
		base.Controls.Add(this.simpleBtn_RemoveContainer);
		base.Controls.Add(this.panelControl3);
		base.Controls.Add(this.panelControl1);
		base.FormId = "oPrintBillWrap";
		base.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		base.Name = "frm_PrintBillWrap";
		base.ShowIcon = false;
		this.Text = "frm_PrintBillWrap";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_PrintBillWrap_FormClosing);
		base.Load += new System.EventHandler(this.frm_PrintBillWrap_Load);
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		this.panelControl1.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.bte_PO.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl3).EndInit();
		this.panelControl3.ResumeLayout(false);
		this.panelControl3.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.gridControl_Case).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView_Case).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridControl_Item).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView_Item).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtBox_CodeCase.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtBox_Code.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
