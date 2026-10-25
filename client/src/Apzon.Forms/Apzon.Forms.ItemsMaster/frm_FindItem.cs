using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.ItemsMaster;

public class frm_FindItem : XtraFormEx
{
	public List<Tuple<GridViewEx, string>> FormItem = new List<Tuple<GridViewEx, string>>();

	public new string FormId = "oFindItem";

	private IContainer components;

	private GridControlEx grd_Item;

	private GridViewEx gv_Item;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_Cancel;

	public DataTable Source { get; set; }

	public object ChoosedItem { get; set; }

	public frm_FindItem()
	{
		InitializeComponent();
	}

	private async void frm_FindItem_Load(object sender, EventArgs e)
	{
		try
		{
			FormItem.Add(new Tuple<GridViewEx, string>(gv_Item, "OITM"));
			if (Source.IsNotNull())
			{
				grd_Item.DataSource = Source;
				grd_Item.RefreshDataSource();
				gv_Item.PopulateColumns();
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

	private void btn_OK_Click(object sender, EventArgs e)
	{
		try
		{
			AddChoosedItem();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Item_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			if (gv_Item.FocusedRowHandle > 0)
			{
				AddChoosedItem();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddChoosedItem()
	{
		string choosedItem = Function.ToString(gv_Item.GetFocusedRowCellValue("ItemCode"));
		ChoosedItem = choosedItem;
		Close();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.ItemsMaster.frm_FindItem));
		this.grd_Item = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Item = new Apzon.Controls.Grid.GridViewEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Item).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Item).BeginInit();
		base.SuspendLayout();
		this.grd_Item.AllowPasteExcel = true;
		this.grd_Item.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Item.ClipboardData = null;
		this.grd_Item.Location = new System.Drawing.Point(0, -1);
		this.grd_Item.MainView = this.gv_Item;
		this.grd_Item.Name = "grd_Item";
		this.grd_Item.Size = new System.Drawing.Size(793, 413);
		this.grd_Item.TabIndex = 0;
		this.grd_Item.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Item });
		this.gv_Item.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Item.ColumnKey = null;
		this.gv_Item.DisableFormSetting = false;
		this.gv_Item.DisplayException = false;
		this.gv_Item.EditColumn = null;
		this.gv_Item.FieldCurrency = null;
		this.gv_Item.FieldRate = null;
		this.gv_Item.FormId = "oFindItem";
		this.gv_Item.GridControl = this.grd_Item;
		this.gv_Item.IgnoreFilter = false;
		this.gv_Item.IndexEditRow = -1;
		this.gv_Item.IndicatorWidth = 35;
		this.gv_Item.IsApzTable = false;
		this.gv_Item.IsInsertNewRow = false;
		this.gv_Item.IsLoadingData = false;
		this.gv_Item.IsMappingContextAction = false;
		this.gv_Item.ItemId = "OITM";
		this.gv_Item.LstColumnAllowBatch = null;
		this.gv_Item.LstColumnNotAllowBatch = null;
		this.gv_Item.LstColumnNotEditable = null;
		this.gv_Item.LstColumNotSetting = null;
		this.gv_Item.Name = "gv_Item";
		this.gv_Item.OptionsView.ColumnAutoWidth = false;
		this.gv_Item.UseBatchMod = false;
		this.gv_Item.DoubleClick += new System.EventHandler(this.gv_Item_DoubleClick);
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(4, 418);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 1;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(83, 418);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 2;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(793, 447);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.grd_Item);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_FindItem.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_FindItem";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "List of Item";
		base.Load += new System.EventHandler(this.frm_FindItem_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_Item).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Item).EndInit();
		base.ResumeLayout(false);
	}
}
