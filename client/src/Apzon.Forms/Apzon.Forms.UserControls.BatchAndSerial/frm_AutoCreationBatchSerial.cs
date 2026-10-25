using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Forms.Helper;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraVerticalGrid;
using DevExpress.XtraVerticalGrid.Events;
using DevExpress.XtraVerticalGrid.Rows;

namespace Apzon.Forms.UserControls.BatchAndSerial;

public class frm_AutoCreationBatchSerial : XtraFormEx
{
	public double LotQty;

	private readonly List<DataTable> _dataRuleCreation = new List<DataTable>();

	public List<string> ListDistNumber = new List<string>();

	public List<string> ListMnfSerial = new List<string>();

	public List<string> ListLotNumber = new List<string>();

	public DateTime? InDate;

	public DateTime? MnfDate;

	public DateTime? ExpDate;

	public string BinLocation = "";

	public string Notes = "";

	private IContainer components;

	private VGridControl vgrd_Creation;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_Cancel;

	private RepositoryItemButtonEditCFL rep_ItemCode;

	private RepositoryItemButtonEditCFL rep_WhsCode;

	private RepositoryItemButtonEdit rep_DistNumber;

	private RepositoryItemButtonEdit rep_MnfSerial;

	private RepositoryItemButtonEdit rep_LotNumber;

	private RepositoryItemDateEditEx rdate;

	private EditorRow row_ItemCode;

	private EditorRow row_ItemName;

	private EditorRow row_WhsCode;

	private EditorRow row_Bin;

	private EditorRow row_Quantity;

	private EditorRow row_NoOfBatch;

	private EditorRow row_DistNumber;

	private EditorRow row_MnfSerial;

	private EditorRow row_LotNumber;

	private EditorRow row_InDate;

	private EditorRow row_MnfDate;

	private EditorRow row_Location;

	private EditorRow row_Notes;

	private EditorRow row_SysNumber;

	private EditorRow row_Cost;

	private EditorRow row_ExpDate;

	private RepositoryItemButtonEditCFL rWhs;

	private RepositoryItemCalcEdit rQuantity;

	private RepositoryItemCalcEdit rNoOfBatch;

	public sealed override string Text
	{
		get
		{
			return base.Text;
		}
		set
		{
			base.Text = value;
		}
	}

	public frm_AutoCreationBatchSerial()
	{
		InitializeComponent();
	}

	public frm_AutoCreationBatchSerial(string itemCode, string itemName, string whsCode, string binLocation, double createQty, string type)
	{
		InitializeComponent();
		if (type.Equals("B"))
		{
			Text = "Automatic Batch Creation";
			row_DistNumber.Properties.Caption = "Batch";
			row_MnfSerial.Properties.Caption = "Batch Attribute 1";
			row_LotNumber.Properties.Caption = "Batch Attribute 2";
		}
		else
		{
			Text = "Automatic Serial Creation";
			row_DistNumber.Properties.Caption = "Serial";
			row_MnfSerial.Properties.Caption = "Manufacturer Serial No.";
			row_LotNumber.Properties.Caption = "Lot Number";
		}
		try
		{
			DataTable dataTable = new DataTable
			{
				Columns = 
				{
					{
						"ItemCode",
						typeof(string)
					},
					{
						"ItemName",
						typeof(string)
					},
					{
						"WhsCode",
						typeof(string)
					},
					{
						"BinAbs",
						typeof(string)
					},
					{
						"LogQty",
						typeof(double)
					},
					{
						"No",
						typeof(int)
					},
					{
						"DistNumber",
						typeof(string)
					},
					{
						"MnfSerial",
						typeof(string)
					},
					{
						"LotNumber",
						typeof(string)
					},
					{
						"InDate",
						typeof(DateTime)
					},
					{
						"MnfDate",
						typeof(DateTime)
					},
					{
						"ExpDate",
						typeof(DateTime)
					},
					{
						"Location",
						typeof(string)
					},
					{
						"Notes",
						typeof(string)
					},
					{
						"SysNumber",
						typeof(int)
					},
					{
						"Cost",
						typeof(double)
					}
				}
			};
			DataRow dataRow = dataTable.NewRow();
			dataRow[0] = itemCode;
			dataRow[1] = itemName;
			dataRow[2] = whsCode;
			dataRow[3] = binLocation;
			dataRow[4] = createQty;
			dataRow[5] = (type.Equals("B") ? 1.0 : createQty);
			dataRow[9] = DateTime.Now.Date;
			dataTable.Rows.Add(dataRow);
			vgrd_Creation.DataSource = dataTable;
			vgrd_Creation.DataSourceTable().AcceptChanges();
			row_NoOfBatch.Properties.ReadOnly = !type.Equals("B");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void frm_AutoCreationBatchSerial_Load(object sender, EventArgs e)
	{
		try
		{
			_dataRuleCreation.Add(new DataTable());
			_dataRuleCreation.Add(new DataTable());
			_dataRuleCreation.Add(new DataTable());
			row_ExpDate.SetFormatControl(Enums.DisplayFormatType.DateTime);
			row_InDate.SetFormatControl(Enums.DisplayFormatType.DateTime);
			row_MnfDate.SetFormatControl(Enums.DisplayFormatType.DateTime);
			row_NoOfBatch.SetFormatControl(Enums.DisplayFormatType.Integer);
			row_Quantity.SetFormatControl(Enums.DisplayFormatType.Amount);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_DistNumber_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_AutomaticStringCreation frm_AutomaticStringCreation2 = new frm_AutomaticStringCreation(_dataRuleCreation[0].Copy());
			frm_AutomaticStringCreation2.ShowDialog(this);
			if (frm_AutomaticStringCreation2.DialogResult == DialogResult.OK)
			{
				_dataRuleCreation[0] = frm_AutomaticStringCreation2.DataSource;
				if (frm_AutomaticStringCreation2.DataSource.Rows.Count > 0)
				{
					vgrd_Creation.SetCellValue(row_DistNumber, 0, frm_AutomaticStringCreation2.FinalString);
				}
				else
				{
					vgrd_Creation.SetCellValue(row_DistNumber, 0, "");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_MnfSerial_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_AutomaticStringCreation frm_AutomaticStringCreation2 = new frm_AutomaticStringCreation(_dataRuleCreation[1].Copy());
			frm_AutomaticStringCreation2.ShowDialog(this);
			if (frm_AutomaticStringCreation2.DialogResult == DialogResult.OK)
			{
				_dataRuleCreation[1] = frm_AutomaticStringCreation2.DataSource;
				if (frm_AutomaticStringCreation2.DataSource.Rows.Count > 0)
				{
					vgrd_Creation.DataSourceTable().Rows[0][row_MnfSerial.Properties.FieldName] = frm_AutomaticStringCreation2.FinalString;
				}
				else
				{
					vgrd_Creation.DataSourceTable().Rows[0][row_MnfSerial.Properties.FieldName] = "";
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_LotNumber_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_AutomaticStringCreation frm_AutomaticStringCreation2 = new frm_AutomaticStringCreation(_dataRuleCreation[2].Copy());
			frm_AutomaticStringCreation2.ShowDialog(this);
			if (frm_AutomaticStringCreation2.DialogResult == DialogResult.OK)
			{
				_dataRuleCreation[2] = frm_AutomaticStringCreation2.DataSource;
				if (frm_AutomaticStringCreation2.DataSource.Rows.Count > 0)
				{
					vgrd_Creation.DataSourceTable().Rows[0][row_LotNumber.Properties.FieldName] = frm_AutomaticStringCreation2.FinalString;
				}
				else
				{
					vgrd_Creation.DataSourceTable().Rows[0][row_LotNumber.Properties.FieldName] = "";
				}
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
		DialogResult = DialogResult.Cancel;
		Close();
	}

	private void btn_OK_Click(object sender, EventArgs e)
	{
		LotQty = Function.ParseDouble(vgrd_Creation.GetCellValue(row_Quantity, 0)) / (double)Function.ParseInt(vgrd_Creation.GetCellValue(row_NoOfBatch, 0));
		InDate = Function.ParseDateTime(vgrd_Creation.GetCellValue(row_InDate, 0));
		MnfDate = Function.ParseDateTime(vgrd_Creation.GetCellValue(row_MnfDate, 0));
		ExpDate = Function.ParseDateTime(vgrd_Creation.GetCellValue(row_ExpDate, 0));
		BinLocation = Function.ToString(vgrd_Creation.GetCellValue(row_Location, 0));
		Notes = Function.ToString(vgrd_Creation.GetCellValue(row_Notes, 0));
		ListDistNumber = new List<string>();
		ListLotNumber = new List<string>();
		ListMnfSerial = new List<string>();
		try
		{
			for (int i = 0; i < Function.ParseInt(vgrd_Creation.GetCellValue(row_NoOfBatch, 0)); i++)
			{
				string text = "";
				for (int j = 0; j < _dataRuleCreation[0].Rows.Count; j++)
				{
					string text2 = Function.ToString(_dataRuleCreation[0].Rows[j][0]);
					if (string.IsNullOrEmpty(text2))
					{
						continue;
					}
					if (Function.ToString(_dataRuleCreation[0].Rows[j][1]).Equals("2"))
					{
						string text3 = Function.ToString(_dataRuleCreation[0].Rows[j][2]);
						if (!(text3 == "2"))
						{
							if (text3 == "3")
							{
								int num = ((i == 0) ? Function.ParseInt(text2) : (Function.ParseInt(text2) - 1));
								text2 = ((text2.Length <= Function.ToString(num).Length) ? Function.ToString(num) : (text2.Substring(0, text2.Length - Function.ToString(num).Length) + num));
								_dataRuleCreation[0].Rows[j][0] = text2;
							}
						}
						else
						{
							int num = ((i == 0) ? Function.ParseInt(text2) : (Function.ParseInt(text2) + 1));
							text2 = ((text2.Length <= Function.ToString(num).Length) ? Function.ToString(num) : (text2.Substring(0, text2.Length - Function.ToString(num).Length) + num));
							_dataRuleCreation[0].Rows[j][0] = text2;
						}
					}
					text += text2;
				}
				ListDistNumber.Add(text);
				text = "";
				for (int k = 0; k < _dataRuleCreation[1].Rows.Count; k++)
				{
					string text4 = Function.ToString(_dataRuleCreation[1].Rows[k][0]);
					if (string.IsNullOrEmpty(text4))
					{
						continue;
					}
					if (Function.ToString(_dataRuleCreation[1].Rows[k][1]).Equals("2"))
					{
						string text3 = Function.ToString(_dataRuleCreation[1].Rows[k][2]);
						if (!(text3 == "2"))
						{
							if (text3 == "3")
							{
								int num = ((i == 0) ? Function.ParseInt(text4) : (Function.ParseInt(text4) - 1));
								text4 = ((text4.Length <= Function.ToString(num).Length) ? Function.ToString(num) : (text4.Substring(0, text4.Length - Function.ToString(num).Length) + num));
								_dataRuleCreation[1].Rows[k][0] = text4;
							}
						}
						else
						{
							int num = ((i == 0) ? Function.ParseInt(text4) : (Function.ParseInt(text4) + 1));
							text4 = ((text4.Length <= Function.ToString(num).Length) ? Function.ToString(num) : (text4.Substring(0, text4.Length - Function.ToString(num).Length) + num));
							_dataRuleCreation[1].Rows[k][0] = text4;
						}
					}
					text += text4;
				}
				ListMnfSerial.Add(text);
				text = "";
				for (int l = 0; l < _dataRuleCreation[2].Rows.Count; l++)
				{
					string text5 = Function.ToString(_dataRuleCreation[2].Rows[l][0]);
					if (string.IsNullOrEmpty(text5))
					{
						continue;
					}
					if (Function.ToString(_dataRuleCreation[2].Rows[l][1]).Equals("2"))
					{
						string text3 = Function.ToString(_dataRuleCreation[2].Rows[l][2]);
						if (!(text3 == "2"))
						{
							if (text3 == "3")
							{
								int num = ((i == 0) ? Function.ParseInt(text5) : (Function.ParseInt(text5) - 1));
								text5 = ((text5.Length <= Function.ToString(num).Length) ? Function.ToString(num) : (text5.Substring(0, text5.Length - Function.ToString(num).Length) + num));
								_dataRuleCreation[2].Rows[l][0] = text5;
							}
						}
						else
						{
							int num = ((i == 0) ? Function.ParseInt(text5) : (Function.ParseInt(text5) + 1));
							text5 = ((text5.Length <= Function.ToString(num).Length) ? Function.ToString(num) : (text5.Substring(0, text5.Length - Function.ToString(num).Length) + num));
							_dataRuleCreation[2].Rows[l][0] = text5;
						}
					}
					text += text5;
				}
				ListLotNumber.Add(text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		DialogResult = DialogResult.OK;
		Close();
	}

	private void vgrd_Creation_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Row.Properties.FieldName.Equals(row_MnfSerial.Properties.FieldName))
			{
				DataTable dataTable = new DataTable();
				dataTable.Columns.Add("String", typeof(string));
				dataTable.Columns.Add("Type", typeof(string));
				dataTable.Columns.Add("Operation", typeof(string));
				DataRow dataRow = dataTable.NewRow();
				dataRow[0] = Function.ToString(e.Value);
				dataRow[1] = "1";
				dataRow[2] = "1";
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				_dataRuleCreation[1] = dataTable;
			}
			else if (e.Row.Properties.FieldName.Equals(row_LotNumber.Properties.FieldName))
			{
				DataTable dataTable2 = new DataTable();
				dataTable2.Columns.Add("String", typeof(string));
				dataTable2.Columns.Add("Type", typeof(string));
				dataTable2.Columns.Add("Operation", typeof(string));
				DataRow dataRow2 = dataTable2.NewRow();
				dataRow2[0] = Function.ToString(e.Value);
				dataRow2[1] = "1";
				dataRow2[2] = "1";
				dataTable2.Rows.Add(dataRow2);
				dataTable2.AcceptChanges();
				_dataRuleCreation[2] = dataTable2;
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.UserControls.BatchAndSerial.frm_AutoCreationBatchSerial));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		this.vgrd_Creation = new DevExpress.XtraVerticalGrid.VGridControl();
		this.rep_ItemCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.rep_WhsCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.rep_DistNumber = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.rep_MnfSerial = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.rep_LotNumber = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.rdate = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.rWhs = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.rQuantity = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.rNoOfBatch = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.row_ItemCode = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.row_ItemName = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.row_WhsCode = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.row_Bin = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.row_Quantity = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.row_NoOfBatch = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.row_DistNumber = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.row_MnfSerial = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.row_LotNumber = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.row_InDate = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.row_MnfDate = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.row_ExpDate = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.row_Location = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.row_Notes = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.row_SysNumber = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.row_Cost = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.vgrd_Creation).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ItemCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_WhsCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DistNumber).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_MnfSerial).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_LotNumber).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rdate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rdate.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rWhs).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rQuantity).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rNoOfBatch).BeginInit();
		base.SuspendLayout();
		this.vgrd_Creation.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.vgrd_Creation.Cursor = System.Windows.Forms.Cursors.SizeWE;
		this.vgrd_Creation.Location = new System.Drawing.Point(1, 1);
		this.vgrd_Creation.Name = "vgrd_Creation";
		this.vgrd_Creation.OptionsBehavior.UseEnterAsTab = true;
		this.vgrd_Creation.OptionsView.ShowVertLines = false;
		this.vgrd_Creation.RecordWidth = 250;
		this.vgrd_Creation.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[9] { this.rep_ItemCode, this.rep_WhsCode, this.rep_DistNumber, this.rep_MnfSerial, this.rep_LotNumber, this.rdate, this.rWhs, this.rQuantity, this.rNoOfBatch });
		this.vgrd_Creation.RowHeaderWidth = 150;
		this.vgrd_Creation.Rows.AddRange(new DevExpress.XtraVerticalGrid.Rows.BaseRow[16]
		{
			this.row_ItemCode, this.row_ItemName, this.row_WhsCode, this.row_Bin, this.row_Quantity, this.row_NoOfBatch, this.row_DistNumber, this.row_MnfSerial, this.row_LotNumber, this.row_InDate,
			this.row_MnfDate, this.row_ExpDate, this.row_Location, this.row_Notes, this.row_SysNumber, this.row_Cost
		});
		this.vgrd_Creation.ShowButtonMode = DevExpress.XtraVerticalGrid.ShowButtonModeEnum.ShowAlways;
		this.vgrd_Creation.Size = new System.Drawing.Size(409, 450);
		this.vgrd_Creation.TabIndex = 0;
		this.vgrd_Creation.CellValueChanged += new DevExpress.XtraVerticalGrid.Events.CellValueChangedEventHandler(this.vgrd_Creation_CellValueChanged);
		this.rep_ItemCode.AutoHeight = false;
		this.rep_ItemCode.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions8.Image");
		this.rep_ItemCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions)
		});
		this.rep_ItemCode.ContainerControl = this;
		this.rep_ItemCode.DisplayMember = "";
		this.rep_ItemCode.DisplayMember2 = "";
		this.rep_ItemCode.LoadDataSourceWhenEnter = false;
		this.rep_ItemCode.Name = "rep_ItemCode";
		this.rep_ItemCode.SeparateString = ", ";
		this.rep_ItemCode.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oItems;
		this.rep_ItemCode.UsingAutoCompleteBox = false;
		this.rep_ItemCode.UsingDataSource = true;
		this.rep_ItemCode.ValueMember = "";
		this.rep_WhsCode.AutoHeight = false;
		this.rep_WhsCode.AutoLoadDataSource = true;
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.rep_WhsCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions2),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2)
		});
		this.rep_WhsCode.ContainerControl = this;
		this.rep_WhsCode.DisplayMember = "";
		this.rep_WhsCode.DisplayMember2 = "";
		this.rep_WhsCode.LoadDataSourceWhenEnter = false;
		this.rep_WhsCode.Name = "rep_WhsCode";
		this.rep_WhsCode.SeparateString = ", ";
		this.rep_WhsCode.UsingAutoCompleteBox = false;
		this.rep_WhsCode.UsingDataSource = true;
		this.rep_WhsCode.ValueMember = "";
		this.rep_DistNumber.AutoHeight = false;
		this.rep_DistNumber.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.rep_DistNumber.Name = "rep_DistNumber";
		this.rep_DistNumber.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_DistNumber_ButtonClick);
		this.rep_MnfSerial.AutoHeight = false;
		this.rep_MnfSerial.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.rep_MnfSerial.Name = "rep_MnfSerial";
		this.rep_MnfSerial.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_MnfSerial_ButtonClick);
		this.rep_LotNumber.AutoHeight = false;
		this.rep_LotNumber.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.rep_LotNumber.Name = "rep_LotNumber";
		this.rep_LotNumber.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_LotNumber_ButtonClick);
		this.rdate.AutoHeight = false;
		this.rdate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rdate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rdate.DisplayFormat.FormatString = "";
		this.rdate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.rdate.EditFormat.FormatString = "";
		this.rdate.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.rdate.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.rdate.Name = "rdate";
		this.rWhs.AutoHeight = false;
		this.rWhs.AutoLoadDataSource = true;
		editorButtonImageOptions3.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions4.Image");
		this.rWhs.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions3),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions3)
		});
		this.rWhs.ContainerControl = this;
		this.rWhs.DisplayMember = "";
		this.rWhs.DisplayMember2 = "";
		this.rWhs.LoadDataSourceWhenEnter = false;
		this.rWhs.Name = "rWhs";
		this.rWhs.SeparateString = ", ";
		this.rWhs.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oWarehouses;
		this.rWhs.UsingAutoCompleteBox = false;
		this.rWhs.UsingDataSource = true;
		this.rWhs.ValueMember = "";
		this.rQuantity.AutoHeight = false;
		this.rQuantity.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rQuantity.Name = "rQuantity";
		this.rNoOfBatch.AutoHeight = false;
		this.rNoOfBatch.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rNoOfBatch.Name = "rNoOfBatch";
		this.row_ItemCode.Height = 25;
		this.row_ItemCode.Name = "row_ItemCode";
		this.row_ItemCode.Properties.Caption = "Item Number";
		this.row_ItemCode.Properties.FieldName = "ItemCode";
		this.row_ItemCode.Properties.ReadOnly = true;
		this.row_ItemCode.Properties.RowEdit = this.rep_ItemCode;
		this.row_ItemName.Height = 25;
		this.row_ItemName.Name = "row_ItemName";
		this.row_ItemName.Properties.Caption = "Item Description";
		this.row_ItemName.Properties.FieldName = "ItemName";
		this.row_ItemName.Properties.ReadOnly = true;
		this.row_WhsCode.Height = 25;
		this.row_WhsCode.Name = "row_WhsCode";
		this.row_WhsCode.Properties.Caption = "Warehouse";
		this.row_WhsCode.Properties.FieldName = "WhsCode";
		this.row_WhsCode.Properties.ReadOnly = true;
		this.row_WhsCode.Properties.RowEdit = this.rWhs;
		this.row_Bin.Height = 25;
		this.row_Bin.Name = "row_Bin";
		this.row_Bin.Properties.Caption = "Bin Location";
		this.row_Bin.Properties.FieldName = "BinAbs";
		this.row_Bin.Properties.RowEdit = this.rep_WhsCode;
		this.row_Quantity.Height = 25;
		this.row_Quantity.Name = "row_Quantity";
		this.row_Quantity.Properties.Caption = "Quantity";
		this.row_Quantity.Properties.FieldName = "LogQty";
		this.row_Quantity.Properties.ReadOnly = false;
		this.row_Quantity.Properties.RowEdit = this.rQuantity;
		this.row_NoOfBatch.Height = 25;
		this.row_NoOfBatch.Name = "row_NoOfBatch";
		this.row_NoOfBatch.Properties.Caption = "No. Of Batches";
		this.row_NoOfBatch.Properties.FieldName = "No";
		this.row_NoOfBatch.Properties.ReadOnly = false;
		this.row_NoOfBatch.Properties.RowEdit = this.rNoOfBatch;
		this.row_DistNumber.Height = 25;
		this.row_DistNumber.Name = "row_DistNumber";
		this.row_DistNumber.Properties.Caption = "Batch";
		this.row_DistNumber.Properties.FieldName = "DistNumber";
		this.row_DistNumber.Properties.ReadOnly = true;
		this.row_DistNumber.Properties.RowEdit = this.rep_DistNumber;
		this.row_MnfSerial.Height = 25;
		this.row_MnfSerial.Name = "row_MnfSerial";
		this.row_MnfSerial.Properties.Caption = "Batch Attribute 1";
		this.row_MnfSerial.Properties.FieldName = "MnfSerial";
		this.row_MnfSerial.Properties.ReadOnly = false;
		this.row_MnfSerial.Properties.RowEdit = this.rep_MnfSerial;
		this.row_LotNumber.Height = 25;
		this.row_LotNumber.Name = "row_LotNumber";
		this.row_LotNumber.Properties.Caption = "Batch Attribute 2";
		this.row_LotNumber.Properties.FieldName = "LotNumber";
		this.row_LotNumber.Properties.ReadOnly = false;
		this.row_LotNumber.Properties.RowEdit = this.rep_LotNumber;
		this.row_InDate.Height = 25;
		this.row_InDate.Name = "row_InDate";
		this.row_InDate.Properties.Caption = "Adminision Date";
		this.row_InDate.Properties.FieldName = "InDate";
		this.row_InDate.Properties.ReadOnly = false;
		this.row_InDate.Properties.RowEdit = this.rdate;
		this.row_MnfDate.Height = 25;
		this.row_MnfDate.Name = "row_MnfDate";
		this.row_MnfDate.Properties.Caption = "Manufacture Date";
		this.row_MnfDate.Properties.FieldName = "MnfDate";
		this.row_MnfDate.Properties.ReadOnly = false;
		this.row_MnfDate.Properties.RowEdit = this.rdate;
		this.row_ExpDate.Height = 25;
		this.row_ExpDate.Name = "row_ExpDate";
		this.row_ExpDate.Properties.Caption = "Expiration Date";
		this.row_ExpDate.Properties.FieldName = "ExpDate";
		this.row_ExpDate.Properties.ReadOnly = false;
		this.row_ExpDate.Properties.RowEdit = this.rdate;
		this.row_Location.Height = 25;
		this.row_Location.Name = "row_Location";
		this.row_Location.Properties.Caption = "Location";
		this.row_Location.Properties.FieldName = "Location";
		this.row_Location.Properties.ReadOnly = false;
		this.row_Notes.Height = 25;
		this.row_Notes.Name = "row_Notes";
		this.row_Notes.Properties.Caption = "Details";
		this.row_Notes.Properties.FieldName = "Notes";
		this.row_Notes.Properties.ReadOnly = false;
		this.row_SysNumber.Height = 25;
		this.row_SysNumber.Name = "row_SysNumber";
		this.row_SysNumber.Properties.Caption = "System Number";
		this.row_SysNumber.Properties.FieldName = "SysNumber";
		this.row_SysNumber.Properties.ReadOnly = true;
		this.row_Cost.Height = 25;
		this.row_Cost.Name = "row_Cost";
		this.row_Cost.Properties.Caption = "Cost";
		this.row_Cost.Properties.FieldName = "Cost";
		this.row_Cost.Properties.ReadOnly = true;
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.Location = new System.Drawing.Point(1, 453);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 1;
		this.btn_OK.Text = "Create";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.Location = new System.Drawing.Point(80, 453);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 2;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(410, 480);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.vgrd_Creation);
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Name = "frm_AutoCreationBatchSerial";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Auto Creation ";
		base.Load += new System.EventHandler(this.frm_AutoCreationBatchSerial_Load);
		((System.ComponentModel.ISupportInitialize)this.vgrd_Creation).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ItemCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_WhsCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DistNumber).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_MnfSerial).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_LotNumber).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rdate.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rdate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rWhs).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rQuantity).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rNoOfBatch).EndInit();
		base.ResumeLayout(false);
	}
}
