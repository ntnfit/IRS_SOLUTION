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
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.PlanningProduction;
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Menu;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.MTS_Manufacturing;

public class FrmMrpList : XtraFormEx
{
	public delegate void ShowForms(XtraFormEx frm);

	public ShowForms ShowChildForm;

	private IHttpClientHelper<Mrp> _mrpClientHelper = new HttpClientHelper<Mrp>(Configuration.ApiServer, "Mrp");

	public string mpsCode = string.Empty;

	private IContainer components;

	private ButtonEditEx txt_MpsCode;

	private DateEditEx date_EndDate;

	private DateEditEx date_BeginDate;

	private DateEditEx date_CreateDate;

	private ComboBoxEditEx cbx_Status;

	private TextEditEx txt_MrpCode;

	private TextEditEx txt_MrpName;

	private SimpleButtonEx btnCancel;

	private SimpleButtonEx btnSearch;

	private GroupControl groupControl1;

	private GridColumnEx cl_FixDate;

	private GridColumnEx cl_PoEntry;

	private GridColumnEx cl_Notes;

	private GridColumnEx cl_Whs;

	private GridColumnEx cl_EndDate;

	private GridColumnEx cl_BeginDate;

	private RepositoryItemDateEdit repoDateEdit;

	private GridColumnEx cl_MpsCode;

	private GridColumnEx cl_Status;

	private GridColumnEx cl_CreateDate;

	private GridColumnEx cl_MrpName;

	private GridColumnEx cl_MrpCode;

	private GridColumnEx cl_Id;

	private GridViewEx gv_MRP;

	private GridControlEx grd_MRP;

	private RepositoryItemLookUpEdit repo_LookUpEdit_Status;

	private LabelLine labelLine1;

	private LabelLine labelLine3;

	private LabelLine labelLine2;

	private LabelLine labelLine4;

	private LabelLine labelLine5;

	private LabelLine labelLine6;

	private LabelLine labelLine7;

	public FrmMrpList()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "FrmMrpList"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "FrmMrpList" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btnCancel.Enabled = true;
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private void txt_MpsCode_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		FrmCflMps frmCflMps = new FrmCflMps(this);
		frmCflMps.FormClosing += cfl_FormClosing;
		frmCflMps.ShowDialog();
	}

	private void cfl_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			txt_MpsCode.Text = mpsCode;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void FrmMrpList_Load(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		List<Common.Table_ID> status = FrmMrp.GetStatus();
		WinFuntions._addComboBox(cbx_Status, status);
		GridControlEx gridControlEx = grd_MRP;
		gridControlEx.DataSource = await GetMrp(new Mrp());
		grd_MRP.RefreshDataSource();
		WinCommons._CloseProcessing();
	}

	private async Task<DataTable> GetMrp(Mrp mrp)
	{
		try
		{
			return await _mrpClientHelper.GetDataTableAsJsonAsync("GetMrp", mrp);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataTable();
		}
	}

	private async void btnSearch_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			await LoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private async Task LoadData()
	{
		try
		{
			Mrp mrp = new Mrp();
			mrp.MrpCode = txt_MrpCode.Text;
			mrp.MrpName = txt_MrpName.Text;
			mrp.Status = ((cbx_Status.SelectedIndex >= 0) ? int.Parse(((Common.ListItems)cbx_Status.SelectedItem).Value) : 0);
			mrp.MpsCode = txt_MpsCode.Text;
			if (date_BeginDate.EditValue != null)
			{
				mrp.BeginDate = (DateTime)date_BeginDate.EditValue;
			}
			if (date_EndDate.EditValue != null)
			{
				mrp.EndDate = (DateTime)date_EndDate.EditValue;
			}
			if (date_CreateDate.EditValue != null)
			{
				mrp.CreateDate = (DateTime)date_CreateDate.EditValue;
			}
			DataTable dataSource = await GetMrp(mrp);
			grd_MRP.DataSource = dataSource;
			grd_MRP.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void gv_MRP_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle >= 0)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gv_MRP_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			if (gv_MRP.FocusedRowHandle > -1)
			{
				Mrp mrp = new Mrp();
				mrp.Id = int.Parse(gv_MRP.GetFocusedRowCellValue("Id").ToString());
				mrp.MrpCode = gv_MRP.GetFocusedRowCellValue("MrpCode").ToString();
				mrp.MrpName = gv_MRP.GetFocusedRowCellValue("MrpName").ToString();
				mrp.CreateDate = (DateTime)gv_MRP.GetFocusedRowCellValue("CreateDate");
				mrp.Status = int.Parse(gv_MRP.GetFocusedRowCellValue("Status").ToString());
				mrp.MpsCode = gv_MRP.GetFocusedRowCellValue("MpsCode").ToString();
				mrp.BeginDate = (DateTime)gv_MRP.GetFocusedRowCellValue("BeginDate");
				mrp.EndDate = (DateTime)gv_MRP.GetFocusedRowCellValue("EndDate");
				mrp.Whs = gv_MRP.GetFocusedRowCellValue("Whs").ToString();
				mrp.Notes = gv_MRP.GetFocusedRowCellValue("Notes").ToString();
				mrp.FixDate = (DateTime)gv_MRP.GetFocusedRowCellValue("FixDate");
				FrmMrp frm = new FrmMrp
				{
					obj = mrp
				};
				ShowChildForm(frm);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_MRP_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
	{
		try
		{
			GridViewMenu gridViewMenu = new GridViewMenu(gv_MRP);
			string[] array = new string[3] { "MRP Running", "Delete Row", "View Future" };
			foreach (string caption in array)
			{
				DXMenuItem dXMenuItem = new DXMenuItem();
				dXMenuItem.Caption = caption;
				dXMenuItem.Click += itemMenu_Click;
				gridViewMenu.Items.Add(dXMenuItem);
			}
			if (e.MenuType == GridMenuType.User || e.MenuType == GridMenuType.Row)
			{
				e.Allow = true;
				gridViewMenu.Show(e.Point);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void itemMenu_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			DXMenuItem dXMenuItem = sender as DXMenuItem;
			if (dXMenuItem.Caption.Trim().Equals("MRP Running"))
			{
				if (gv_MRP.FocusedRowHandle > -1)
				{
					Mrp mrp = new Mrp();
					mrp.Id = int.Parse(gv_MRP.GetFocusedRowCellValue("Id").ToString());
					mrp.MrpCode = gv_MRP.GetFocusedRowCellValue("MrpCode").ToString();
					mrp.MrpName = gv_MRP.GetFocusedRowCellValue("MrpName").ToString();
					mrp.CreateDate = (DateTime)gv_MRP.GetFocusedRowCellValue("CreateDate");
					mrp.Status = int.Parse(gv_MRP.GetFocusedRowCellValue("Status").ToString());
					mrp.MpsCode = gv_MRP.GetFocusedRowCellValue("MpsCode").ToString();
					mrp.BeginDate = (DateTime)gv_MRP.GetFocusedRowCellValue("BeginDate");
					mrp.EndDate = (DateTime)gv_MRP.GetFocusedRowCellValue("EndDate");
					mrp.Whs = gv_MRP.GetFocusedRowCellValue("Whs").ToString();
					mrp.Notes = gv_MRP.GetFocusedRowCellValue("Notes").ToString();
					FrmMrp frmMrp = new FrmMrp();
					frmMrp.obj = mrp;
					ShowChildForm(frmMrp);
				}
			}
			else if (dXMenuItem.Caption.Trim().Equals("Delete Row"))
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					string text = null;
					if (gv_MRP.FocusedRowHandle > -1)
					{
						text = gv_MRP.GetFocusedRowCellValue("Id").ToString();
					}
					if (!string.IsNullOrEmpty(text))
					{
						string text2 = await DeleteMRP(text);
						if (string.IsNullOrEmpty(text2))
						{
							await LoadData();
							WinCommons.Success(Enums.MessageCode.DeleteSuccess, "");
						}
						else
						{
							WinCommons.Error(Enums.MessageCode.None, text2);
						}
					}
				}
			}
			else if (dXMenuItem.Caption.Trim().Equals("View Future") && gv_MRP.FocusedRowHandle > -1)
			{
				FrmMrpFuture frm = new FrmMrpFuture(gv_MRP.GetFocusedRowCellValue("Id").ToString());
				ShowChildForm(frm);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private async Task<string> DeleteMRP(string id)
	{
		try
		{
			return await _mrpClientHelper.PostAsJsonAsync("DeleteMrp", new Mrp
			{
				Id = Function.ParseLong(id)
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return ex.Message;
		}
	}

	private void gv_MRP_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			if (e.RowHandle > -1 && gv_MRP.GetRowCellValue(e.RowHandle, cl_Status).ToString().Equals("Aprroved"))
			{
				e.Appearance.BackColor = Color.LightYellow;
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.MTS_Manufacturing.FrmMrpList));
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		this.repoDateEdit = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.txt_MpsCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.date_EndDate = new Apzon.Controls.Editor.DateEditEx();
		this.date_BeginDate = new Apzon.Controls.Editor.DateEditEx();
		this.date_CreateDate = new Apzon.Controls.Editor.DateEditEx();
		this.cbx_Status = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.txt_MrpCode = new Apzon.Controls.Editor.TextEditEx();
		this.txt_MrpName = new Apzon.Controls.Editor.TextEditEx();
		this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
		this.labelLine7 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnSearch = new Apzon.Controls.Controls.SimpleButtonEx();
		this.gv_MRP = new Apzon.Controls.Grid.GridViewEx();
		this.cl_Id = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_MrpCode = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_MrpName = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_CreateDate = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Status = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_MpsCode = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_BeginDate = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_EndDate = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Whs = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Notes = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_PoEntry = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_FixDate = new Apzon.Controls.Grid.GridColumnEx();
		this.grd_MRP = new Apzon.Controls.Grid.GridControlEx();
		this.repo_LookUpEdit_Status = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		((System.ComponentModel.ISupportInitialize)this.repoDateEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repoDateEdit.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_MpsCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_EndDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_EndDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_BeginDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_BeginDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_CreateDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_CreateDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbx_Status.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_MrpCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_MrpName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl1).BeginInit();
		this.groupControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_MRP).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_MRP).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repo_LookUpEdit_Status).BeginInit();
		base.SuspendLayout();
		this.repoDateEdit.AutoHeight = false;
		this.repoDateEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repoDateEdit.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repoDateEdit.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.repoDateEdit.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repoDateEdit.EditFormat.FormatString = "dd/MM/yyyy";
		this.repoDateEdit.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repoDateEdit.Mask.EditMask = "dd/MM/yyyy";
		this.repoDateEdit.Name = "repoDateEdit";
		this.txt_MpsCode.EditControl = null;
		this.txt_MpsCode.FieldName = null;
		this.txt_MpsCode.IsSearchControl = false;
		this.txt_MpsCode.Location = new System.Drawing.Point(131, 69);
		this.txt_MpsCode.Name = "txt_MpsCode";
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions1.Image");
		this.txt_MpsCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txt_MpsCode.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txt_MpsCode_Properties_ButtonClick);
		this.txt_MpsCode.SettingInVisible = false;
		this.txt_MpsCode.SettingReadonly = false;
		this.txt_MpsCode.Size = new System.Drawing.Size(137, 24);
		this.txt_MpsCode.TabIndex = 2;
		this.date_EndDate.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.date_EndDate.EditControl = null;
		this.date_EndDate.EditValue = null;
		this.date_EndDate.FieldName = null;
		this.date_EndDate.Location = new System.Drawing.Point(435, 69);
		this.date_EndDate.Name = "date_EndDate";
		this.date_EndDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_EndDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_EndDate.Properties.CalendarTimeProperties.DisplayFormat.FormatString = "d";
		this.date_EndDate.Properties.CalendarTimeProperties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_EndDate.Properties.CalendarTimeProperties.EditFormat.FormatString = "d";
		this.date_EndDate.Properties.CalendarTimeProperties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_EndDate.Properties.CalendarTimeProperties.Mask.EditMask = "dd/MM/yyyy";
		this.date_EndDate.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.date_EndDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_EndDate.Properties.EditFormat.FormatString = "dd/MM/yyyy";
		this.date_EndDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_EndDate.Properties.IsSearchControl = false;
		this.date_EndDate.Properties.Mask.EditMask = "dd/MM/yyyy";
		this.date_EndDate.SettingInVisible = false;
		this.date_EndDate.SettingReadonly = false;
		this.date_EndDate.Size = new System.Drawing.Size(133, 20);
		this.date_EndDate.TabIndex = 6;
		this.date_BeginDate.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.date_BeginDate.EditControl = null;
		this.date_BeginDate.EditValue = null;
		this.date_BeginDate.FieldName = null;
		this.date_BeginDate.Location = new System.Drawing.Point(435, 46);
		this.date_BeginDate.Name = "date_BeginDate";
		this.date_BeginDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_BeginDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_BeginDate.Properties.CalendarTimeProperties.DisplayFormat.FormatString = "d";
		this.date_BeginDate.Properties.CalendarTimeProperties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_BeginDate.Properties.CalendarTimeProperties.EditFormat.FormatString = "d";
		this.date_BeginDate.Properties.CalendarTimeProperties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_BeginDate.Properties.CalendarTimeProperties.Mask.EditMask = "dd/MM/yyyy";
		this.date_BeginDate.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.date_BeginDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_BeginDate.Properties.EditFormat.FormatString = "dd/MM/yyyy";
		this.date_BeginDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_BeginDate.Properties.IsSearchControl = false;
		this.date_BeginDate.Properties.Mask.EditMask = "dd/MM/yyyy";
		this.date_BeginDate.SettingInVisible = false;
		this.date_BeginDate.SettingReadonly = false;
		this.date_BeginDate.Size = new System.Drawing.Size(133, 20);
		this.date_BeginDate.TabIndex = 5;
		this.date_CreateDate.EditControl = null;
		this.date_CreateDate.EditValue = null;
		this.date_CreateDate.FieldName = null;
		this.date_CreateDate.Location = new System.Drawing.Point(131, 94);
		this.date_CreateDate.Name = "date_CreateDate";
		this.date_CreateDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_CreateDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_CreateDate.Properties.CalendarTimeProperties.DisplayFormat.FormatString = "d";
		this.date_CreateDate.Properties.CalendarTimeProperties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_CreateDate.Properties.CalendarTimeProperties.EditFormat.FormatString = "d";
		this.date_CreateDate.Properties.CalendarTimeProperties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_CreateDate.Properties.CalendarTimeProperties.Mask.EditMask = "dd/MM/yyyy";
		this.date_CreateDate.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.date_CreateDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_CreateDate.Properties.EditFormat.FormatString = "dd/MM/yyyy";
		this.date_CreateDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_CreateDate.Properties.IsSearchControl = false;
		this.date_CreateDate.Properties.Mask.EditMask = "dd/MM/yyyy";
		this.date_CreateDate.SettingInVisible = false;
		this.date_CreateDate.SettingReadonly = false;
		this.date_CreateDate.Size = new System.Drawing.Size(137, 20);
		this.date_CreateDate.TabIndex = 3;
		this.cbx_Status.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.cbx_Status.EditControl = null;
		this.cbx_Status.FieldName = null;
		this.cbx_Status.IsSearchControl = false;
		this.cbx_Status.Location = new System.Drawing.Point(435, 23);
		this.cbx_Status.Name = "cbx_Status";
		this.cbx_Status.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbx_Status.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.cbx_Status.SettingInVisible = false;
		this.cbx_Status.SettingReadonly = false;
		this.cbx_Status.Size = new System.Drawing.Size(133, 20);
		this.cbx_Status.TabIndex = 4;
		this.txt_MrpCode.EditControl = null;
		this.txt_MrpCode.FieldName = null;
		this.txt_MrpCode.IsSearchControl = false;
		this.txt_MrpCode.Location = new System.Drawing.Point(131, 23);
		this.txt_MrpCode.Name = "txt_MrpCode";
		this.txt_MrpCode.SettingInVisible = false;
		this.txt_MrpCode.SettingReadonly = false;
		this.txt_MrpCode.Size = new System.Drawing.Size(137, 20);
		this.txt_MrpCode.TabIndex = 0;
		this.txt_MrpName.EditControl = null;
		this.txt_MrpName.FieldName = null;
		this.txt_MrpName.IsSearchControl = false;
		this.txt_MrpName.Location = new System.Drawing.Point(131, 46);
		this.txt_MrpName.Name = "txt_MrpName";
		this.txt_MrpName.SettingInVisible = false;
		this.txt_MrpName.SettingReadonly = false;
		this.txt_MrpName.Size = new System.Drawing.Size(137, 20);
		this.txt_MrpName.TabIndex = 1;
		this.groupControl1.Controls.Add(this.labelLine7);
		this.groupControl1.Controls.Add(this.labelLine6);
		this.groupControl1.Controls.Add(this.labelLine5);
		this.groupControl1.Controls.Add(this.labelLine4);
		this.groupControl1.Controls.Add(this.labelLine3);
		this.groupControl1.Controls.Add(this.labelLine2);
		this.groupControl1.Controls.Add(this.labelLine1);
		this.groupControl1.Controls.Add(this.txt_MpsCode);
		this.groupControl1.Controls.Add(this.date_EndDate);
		this.groupControl1.Controls.Add(this.date_BeginDate);
		this.groupControl1.Controls.Add(this.date_CreateDate);
		this.groupControl1.Controls.Add(this.cbx_Status);
		this.groupControl1.Controls.Add(this.txt_MrpCode);
		this.groupControl1.Controls.Add(this.txt_MrpName);
		this.groupControl1.Controls.Add(this.btnCancel);
		this.groupControl1.Controls.Add(this.btnSearch);
		this.groupControl1.Dock = System.Windows.Forms.DockStyle.Top;
		this.groupControl1.Location = new System.Drawing.Point(0, 0);
		this.groupControl1.Name = "groupControl1";
		this.groupControl1.Size = new System.Drawing.Size(580, 120);
		this.groupControl1.TabIndex = 2;
		this.groupControl1.Text = "Search Information";
		this.labelLine7.EditControl = null;
		this.labelLine7.EditValue = "To Date";
		this.labelLine7.FieldName = null;
		this.labelLine7.HideWhenEmpty = false;
		this.labelLine7.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine7.Location = new System.Drawing.Point(312, 71);
		this.labelLine7.Name = "labelLine7";
		this.labelLine7.ReadOnly = false;
		this.labelLine7.SettingInVisible = false;
		this.labelLine7.SettingReadonly = false;
		this.labelLine7.ShowUnderLine = true;
		this.labelLine7.Size = new System.Drawing.Size(123, 20);
		this.labelLine7.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine7.TabIndex = 15;
		this.labelLine7.Utext = "To Date";
		this.labelLine6.EditControl = null;
		this.labelLine6.EditValue = "From Date";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(312, 49);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ReadOnly = false;
		this.labelLine6.SettingInVisible = false;
		this.labelLine6.SettingReadonly = false;
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(123, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 14;
		this.labelLine6.Utext = "From Date";
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Status";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(312, 26);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(123, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 13;
		this.labelLine5.Utext = "Status";
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Create Date";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(8, 96);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(123, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 12;
		this.labelLine4.Utext = "Create Date";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Code MPS";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(8, 73);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(123, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 11;
		this.labelLine3.Utext = "Code MPS";
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Description";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(8, 46);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(123, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 10;
		this.labelLine2.Utext = "Description";
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Code MRP";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(8, 26);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(123, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 9;
		this.labelLine1.Utext = "Code MRP";
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(493, 94);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 8;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.btnSearch.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnSearch.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnSearch.EditControl = null;
		this.btnSearch.EditValue = "Search";
		this.btnSearch.FieldName = null;
		this.btnSearch.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSearch.ImageOptions.Image");
		this.btnSearch.IsSearchControl = false;
		this.btnSearch.Location = new System.Drawing.Point(412, 94);
		this.btnSearch.Name = "btnSearch";
		this.btnSearch.ReadOnly = false;
		this.btnSearch.SettingInVisible = false;
		this.btnSearch.SettingReadonly = false;
		this.btnSearch.Size = new System.Drawing.Size(75, 23);
		this.btnSearch.TabIndex = 7;
		this.btnSearch.Text = "Search";
		this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
		this.gv_MRP.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_MRP.ColumnKey = null;
		this.gv_MRP.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[12]
		{
			this.cl_Id, this.cl_MrpCode, this.cl_MrpName, this.cl_CreateDate, this.cl_Status, this.cl_MpsCode, this.cl_BeginDate, this.cl_EndDate, this.cl_Whs, this.cl_Notes,
			this.cl_PoEntry, this.cl_FixDate
		});
		this.gv_MRP.DisableFormSetting = false;
		this.gv_MRP.DisplayException = false;
		this.gv_MRP.EditColumn = null;
		this.gv_MRP.FieldCurrency = null;
		this.gv_MRP.FieldRate = null;
		this.gv_MRP.FormId = null;
		this.gv_MRP.GridControl = this.grd_MRP;
		this.gv_MRP.IgnoreFilter = false;
		this.gv_MRP.IndexEditRow = -1;
		this.gv_MRP.IndicatorWidth = 30;
		this.gv_MRP.IsApzTable = false;
		this.gv_MRP.IsInsertNewRow = false;
		this.gv_MRP.IsLoadingData = false;
		this.gv_MRP.IsMappingContextAction = false;
		this.gv_MRP.ItemId = null;
		this.gv_MRP.LstColumnAllowBatch = null;
		this.gv_MRP.LstColumnNotAllowBatch = null;
		this.gv_MRP.LstColumnNotEditable = null;
		this.gv_MRP.LstColumNotSetting = null;
		this.gv_MRP.Name = "gv_MRP";
		this.gv_MRP.OptionsBehavior.Editable = false;
		this.gv_MRP.OptionsView.ColumnAutoWidth = false;
		this.gv_MRP.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.gv_MRP.OptionsView.ShowGroupPanel = false;
		this.gv_MRP.UseBatchMod = false;
		this.gv_MRP.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_MRP_CustomDrawRowIndicator);
		this.gv_MRP.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gv_MRP_RowStyle);
		this.gv_MRP.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gv_MRP_PopupMenuShowing);
		this.gv_MRP.DoubleClick += new System.EventHandler(this.gv_MRP_DoubleClick);
		this.cl_Id.Caption = "ID";
		this.cl_Id.CurrencyDateField = null;
		this.cl_Id.CurrencyField = null;
		this.cl_Id.CurrencyFixed = null;
		this.cl_Id.CurrencyFixedRate = 0.0;
		this.cl_Id.CurrencyRateField = null;
		this.cl_Id.FieldName = "Id";
		this.cl_Id.Name = "cl_Id";
		this.cl_Id.PrimaryKey = false;
		this.cl_Id.UnitField = null;
		this.cl_MrpCode.Caption = "MRP Code";
		this.cl_MrpCode.CurrencyDateField = null;
		this.cl_MrpCode.CurrencyField = null;
		this.cl_MrpCode.CurrencyFixed = null;
		this.cl_MrpCode.CurrencyFixedRate = 0.0;
		this.cl_MrpCode.CurrencyRateField = null;
		this.cl_MrpCode.FieldName = "MrpCode";
		this.cl_MrpCode.Name = "cl_MrpCode";
		this.cl_MrpCode.PrimaryKey = false;
		this.cl_MrpCode.UnitField = null;
		this.cl_MrpCode.Visible = true;
		this.cl_MrpCode.VisibleIndex = 0;
		this.cl_MrpCode.Width = 103;
		this.cl_MrpName.Caption = "Description";
		this.cl_MrpName.CurrencyDateField = null;
		this.cl_MrpName.CurrencyField = null;
		this.cl_MrpName.CurrencyFixed = null;
		this.cl_MrpName.CurrencyFixedRate = 0.0;
		this.cl_MrpName.CurrencyRateField = null;
		this.cl_MrpName.FieldName = "MrpName";
		this.cl_MrpName.Name = "cl_MrpName";
		this.cl_MrpName.PrimaryKey = false;
		this.cl_MrpName.UnitField = null;
		this.cl_MrpName.Visible = true;
		this.cl_MrpName.VisibleIndex = 1;
		this.cl_MrpName.Width = 142;
		this.cl_CreateDate.Caption = "Ngày tạo";
		this.cl_CreateDate.ColumnEdit = this.repoDateEdit;
		this.cl_CreateDate.CurrencyDateField = null;
		this.cl_CreateDate.CurrencyField = null;
		this.cl_CreateDate.CurrencyFixed = null;
		this.cl_CreateDate.CurrencyFixedRate = 0.0;
		this.cl_CreateDate.CurrencyRateField = null;
		this.cl_CreateDate.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.cl_CreateDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.cl_CreateDate.FieldName = "CreateDate";
		this.cl_CreateDate.Name = "cl_CreateDate";
		this.cl_CreateDate.PrimaryKey = false;
		this.cl_CreateDate.UnitField = null;
		this.cl_Status.Caption = "Status";
		this.cl_Status.CurrencyDateField = null;
		this.cl_Status.CurrencyField = null;
		this.cl_Status.CurrencyFixed = null;
		this.cl_Status.CurrencyFixedRate = 0.0;
		this.cl_Status.CurrencyRateField = null;
		this.cl_Status.FieldName = "StatusName";
		this.cl_Status.Name = "cl_Status";
		this.cl_Status.PrimaryKey = false;
		this.cl_Status.UnitField = null;
		this.cl_Status.Visible = true;
		this.cl_Status.VisibleIndex = 3;
		this.cl_Status.Width = 72;
		this.cl_MpsCode.Caption = "MPS Code";
		this.cl_MpsCode.CurrencyDateField = null;
		this.cl_MpsCode.CurrencyField = null;
		this.cl_MpsCode.CurrencyFixed = null;
		this.cl_MpsCode.CurrencyFixedRate = 0.0;
		this.cl_MpsCode.CurrencyRateField = null;
		this.cl_MpsCode.FieldName = "MpsCode";
		this.cl_MpsCode.Name = "cl_MpsCode";
		this.cl_MpsCode.PrimaryKey = false;
		this.cl_MpsCode.UnitField = null;
		this.cl_MpsCode.Visible = true;
		this.cl_MpsCode.VisibleIndex = 2;
		this.cl_BeginDate.Caption = "From Date";
		this.cl_BeginDate.ColumnEdit = this.repoDateEdit;
		this.cl_BeginDate.CurrencyDateField = null;
		this.cl_BeginDate.CurrencyField = null;
		this.cl_BeginDate.CurrencyFixed = null;
		this.cl_BeginDate.CurrencyFixedRate = 0.0;
		this.cl_BeginDate.CurrencyRateField = null;
		this.cl_BeginDate.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.cl_BeginDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.cl_BeginDate.FieldName = "BeginDate";
		this.cl_BeginDate.Name = "cl_BeginDate";
		this.cl_BeginDate.PrimaryKey = false;
		this.cl_BeginDate.UnitField = null;
		this.cl_BeginDate.Visible = true;
		this.cl_BeginDate.VisibleIndex = 4;
		this.cl_BeginDate.Width = 81;
		this.cl_EndDate.Caption = "To Date";
		this.cl_EndDate.CurrencyDateField = null;
		this.cl_EndDate.CurrencyField = null;
		this.cl_EndDate.CurrencyFixed = null;
		this.cl_EndDate.CurrencyFixedRate = 0.0;
		this.cl_EndDate.CurrencyRateField = null;
		this.cl_EndDate.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.cl_EndDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.cl_EndDate.FieldName = "EndDate";
		this.cl_EndDate.Name = "cl_EndDate";
		this.cl_EndDate.PrimaryKey = false;
		this.cl_EndDate.UnitField = null;
		this.cl_EndDate.Visible = true;
		this.cl_EndDate.VisibleIndex = 5;
		this.cl_EndDate.Width = 76;
		this.cl_Whs.Caption = "Kho";
		this.cl_Whs.CurrencyDateField = null;
		this.cl_Whs.CurrencyField = null;
		this.cl_Whs.CurrencyFixed = null;
		this.cl_Whs.CurrencyFixedRate = 0.0;
		this.cl_Whs.CurrencyRateField = null;
		this.cl_Whs.FieldName = "Whs";
		this.cl_Whs.Name = "cl_Whs";
		this.cl_Whs.PrimaryKey = false;
		this.cl_Whs.UnitField = null;
		this.cl_Notes.Caption = "Remarks";
		this.cl_Notes.CurrencyDateField = null;
		this.cl_Notes.CurrencyField = null;
		this.cl_Notes.CurrencyFixed = null;
		this.cl_Notes.CurrencyFixedRate = 0.0;
		this.cl_Notes.CurrencyRateField = null;
		this.cl_Notes.FieldName = "Notes";
		this.cl_Notes.Name = "cl_Notes";
		this.cl_Notes.PrimaryKey = false;
		this.cl_Notes.UnitField = null;
		this.cl_Notes.Visible = true;
		this.cl_Notes.VisibleIndex = 6;
		this.cl_Notes.Width = 149;
		this.cl_PoEntry.Caption = "POEntry";
		this.cl_PoEntry.CurrencyDateField = null;
		this.cl_PoEntry.CurrencyField = null;
		this.cl_PoEntry.CurrencyFixed = null;
		this.cl_PoEntry.CurrencyFixedRate = 0.0;
		this.cl_PoEntry.CurrencyRateField = null;
		this.cl_PoEntry.FieldName = "PoEntry";
		this.cl_PoEntry.Name = "cl_PoEntry";
		this.cl_PoEntry.PrimaryKey = false;
		this.cl_PoEntry.UnitField = null;
		this.cl_FixDate.Caption = "Fix Date";
		this.cl_FixDate.CurrencyDateField = null;
		this.cl_FixDate.CurrencyField = null;
		this.cl_FixDate.CurrencyFixed = null;
		this.cl_FixDate.CurrencyFixedRate = 0.0;
		this.cl_FixDate.CurrencyRateField = null;
		this.cl_FixDate.FieldName = "FixDate";
		this.cl_FixDate.Name = "cl_FixDate";
		this.cl_FixDate.PrimaryKey = false;
		this.cl_FixDate.UnitField = null;
		this.grd_MRP.AllowPasteExcel = true;
		this.grd_MRP.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_MRP.ClipboardData = null;
		this.grd_MRP.Location = new System.Drawing.Point(0, 125);
		this.grd_MRP.MainView = this.gv_MRP;
		this.grd_MRP.Name = "grd_MRP";
		this.grd_MRP.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.repo_LookUpEdit_Status, this.repoDateEdit });
		this.grd_MRP.Size = new System.Drawing.Size(581, 315);
		this.grd_MRP.TabIndex = 3;
		this.grd_MRP.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_MRP });
		this.repo_LookUpEdit_Status.AutoHeight = false;
		this.repo_LookUpEdit_Status.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repo_LookUpEdit_Status.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Text", "Status")
		});
		this.repo_LookUpEdit_Status.Name = "repo_LookUpEdit_Status";
		this.repo_LookUpEdit_Status.NullText = "";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(580, 439);
		base.Controls.Add(this.groupControl1);
		base.Controls.Add(this.grd_MRP);
		base.IconOptions.ShowIcon = false;
		base.Name = "FrmMrpList";
		base.ShowInTaskbar = false;
		this.Text = "List Of MRP";
		base.Load += new System.EventHandler(this.FrmMrpList_Load);
		((System.ComponentModel.ISupportInitialize)this.repoDateEdit.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repoDateEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_MpsCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_EndDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_EndDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_BeginDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_BeginDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_CreateDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_CreateDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbx_Status.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_MrpCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_MrpName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl1).EndInit();
		this.groupControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_MRP).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_MRP).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repo_LookUpEdit_Status).EndInit();
		base.ResumeLayout(false);
	}
}
