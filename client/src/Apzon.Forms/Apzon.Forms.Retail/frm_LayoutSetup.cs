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
using Apzon.Controls.Form;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.UserControls.POS_UI;
using Apzon.HttpClientHelper;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraBars.Docking2010.Views.Widget;
using DevExpress.XtraEditors;

namespace Apzon.Forms.Retail;

public class frm_LayoutSetup : XtraFormEx
{
	private int WidgetEntry = -100;

	private const string TableHeaderName = "APZ_OWID";

	private const string TableDocumentsName = "APZ_WID1";

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private DataTable _dtDocuments;

	private DataTable _dtHeader;

	private IContainer components;

	private ucWidgetLayout ucWidgetLayout1;

	private SimpleButton btn_SaveLayout;

	public frm_LayoutSetup()
	{
		InitializeComponent();
	}

	private async void btn_SaveLayout_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = _dtDocuments.Clone();
			dataTable.TableName = "APZ_WID1";
			foreach (Document item in (IEnumerable<BaseDocument>)ucWidgetLayout1.widgetView1.Documents)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["AbsEntry"] = WidgetEntry;
				dataRow["PageId"] = 0;
				dataRow["ItemId"] = ucWidgetLayout1.widgetView1.Documents.IndexOf(item);
				dataRow["ObjType"] = item.ControlTypeName;
				dataRow["ObjCode"] = item.ControlName;
				dataRow["Caption"] = item.Caption;
				dataRow["ColumnIndex"] = item.ColumnIndex;
				dataRow["ColumnSpan"] = item.ColumnSpan;
				dataRow["Height"] = item.Height;
				dataRow["Width"] = item.Width;
				dataRow["RowIndex"] = item.RowIndex;
				dataRow["RowSpan"] = item.RowSpan;
				dataRow["ToolTip"] = item.ToolTip;
				dataRow["FreeLayoutHeight"] = item.FreeLayoutHeight.UnitValue;
				dataRow["FreeLayoutWidth"] = item.FreeLayoutWidth.UnitValue;
				_ = ucWidgetLayout1.widgetView1.LayoutMode;
				dataTable.Rows.Add(dataRow);
			}
			if (_dtHeader != null)
			{
				DataTable dataTable2 = _dtHeader.Clone();
				DataRow dataRow2 = dataTable2.NewRow();
				dataRow2["AbsEntry"] = WidgetEntry;
				dataRow2["Caption"] = Text;
				dataRow2["LayoutMode"] = (int)ucWidgetLayout1.widgetView1.LayoutMode;
				dataTable2.TableName = "APZ_OWID";
				dataTable2.Rows.Add(dataRow2);
				DataSet dataSet = new DataSet();
				dataSet.Tables.AddRange(new DataTable[2]
				{
					dataTable2.Copy(),
					dataTable.Copy()
				});
				HttpResult httpResult = await _httpClient.Widget.Save(dataSet);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
				else
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.CanNotGetDataHeader, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void frm_LayoutSetup_Load(object sender, EventArgs e)
	{
		_dtDocuments = WinFuntions.GetTableStructure("APZ_WID1");
		_dtHeader = WinFuntions.GetTableStructure("APZ_OWID");
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
		new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Retail.frm_LayoutSetup));
		this.ucWidgetLayout1 = new Apzon.Forms.UserControls.POS_UI.ucWidgetLayout();
		this.btn_SaveLayout = new DevExpress.XtraEditors.SimpleButton();
		base.SuspendLayout();
		this.ucWidgetLayout1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.ucWidgetLayout1.Location = new System.Drawing.Point(0, -2);
		this.ucWidgetLayout1.Name = "ucWidgetLayout1";
		this.ucWidgetLayout1.Size = new System.Drawing.Size(1211, 708);
		this.ucWidgetLayout1.TabIndex = 0;
		this.btn_SaveLayout.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_SaveLayout.Location = new System.Drawing.Point(1217, 271);
		this.btn_SaveLayout.Name = "btn_SaveLayout";
		this.btn_SaveLayout.Size = new System.Drawing.Size(75, 78);
		this.btn_SaveLayout.TabIndex = 1;
		this.btn_SaveLayout.Text = "Save Layout";
		this.btn_SaveLayout.Click += new System.EventHandler(this.btn_SaveLayout_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1299, 708);
		base.Controls.Add(this.btn_SaveLayout);
		base.Controls.Add(this.ucWidgetLayout1);
		base.Name = "frm_LayoutSetup";
		base.ShowIcon = false;
		this.Text = "POS Layout Setup";
		base.Load += new System.EventHandler(this.frm_LayoutSetup_Load);
		base.ResumeLayout(false);
	}
}
