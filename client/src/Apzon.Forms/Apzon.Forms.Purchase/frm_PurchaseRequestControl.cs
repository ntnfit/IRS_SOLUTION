using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Controls;
using Apzon.Controls.Form;
using Apzon.Entities.Models.UserFields;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using SAPbobsCOM;

namespace Apzon.Forms.Purchase;

public class frm_PurchaseRequestControl : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private LayoutControl layoutControl1;

	private LayoutControlGroup layoutControlGroup1;

	public frm_PurchaseRequestControl()
	{
		InitializeComponent();
		OnLoadAsync += frm_PurchaseRequestControl_Load;
	}

	private async Task frm_PurchaseRequestControl_Load(object sender, EventArgs e)
	{
		await LoadUdfControl();
	}

	private async Task LoadUdfControl()
	{
		try
		{
			List<UserFieldsImpl> list = await _httpClient.PurchaseRequestControl.GetListField("OPRQ");
			for (int i = 0; i < list.Count; i++)
			{
				UserFieldsImpl userFieldsImpl = list[i];
				LayoutControlItem layoutControlItem = layoutControl1.AddItem();
				layoutControlItem.Text = userFieldsImpl.Description;
				layoutControlItem.TextToControlDistance = 10;
				layoutControlItem.AppearanceItemCaption.Font = new Font(layoutControlItem.AppearanceItemCaption.Font.FontFamily, layoutControlItem.AppearanceItemCaption.Font.Size, FontStyle.Underline);
				AddControlToLayoutItem(userFieldsImpl, layoutControlItem);
				if (i % 2 != 0)
				{
					layoutControl1.Items[i + 1].Move(layoutControl1.Items[i], InsertType.Right);
					layoutControl1.Items[i + 1].Padding = new DevExpress.XtraLayout.Utils.Padding(40, 0, 0, 0);
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddControlToLayoutItem(UserFieldsImpl item, LayoutControlItem layoutItem)
	{
		try
		{
			_ = layoutControl1.Width / 4;
			switch (item.Type)
			{
			case BoFieldTypes.db_Alpha:
			{
				TextEdit textEdit = new TextEdit();
				textEdit.Name = item.Name;
				textEdit.StyleController = layoutControl1;
				layoutItem.Control = textEdit;
				break;
			}
			case BoFieldTypes.db_Date:
			{
				DateEdit dateEdit = new DateEdit();
				dateEdit.Name = item.Name;
				dateEdit.Properties.DisplayFormat.FormatType = FormatType.DateTime;
				dateEdit.Properties.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
				dateEdit.StyleController = layoutControl1;
				layoutItem.Control = dateEdit;
				dateEdit.StyleController = layoutControl1;
				layoutItem.Control = dateEdit;
				break;
			}
			case BoFieldTypes.db_Float:
			{
				CalcEdit calcEdit2 = new CalcEdit();
				calcEdit2.Name = item.Name;
				calcEdit2.Properties.DisplayFormat.FormatType = FormatType.Numeric;
				calcEdit2.Properties.DisplayFormat.Format = GlobalData.UnitPriceFormat;
				calcEdit2.StyleController = layoutControl1;
				layoutItem.Control = calcEdit2;
				break;
			}
			case BoFieldTypes.db_Memo:
			{
				MemoExEdit memoExEdit = new MemoExEdit();
				memoExEdit.Name = item.Name;
				memoExEdit.StyleController = layoutControl1;
				layoutItem.Control = memoExEdit;
				break;
			}
			case BoFieldTypes.db_Numeric:
			{
				CalcEdit calcEdit = new CalcEdit();
				calcEdit.Name = item.Name;
				calcEdit.StyleController = layoutControl1;
				calcEdit.Properties.DisplayFormat.FormatType = FormatType.Numeric;
				calcEdit.Properties.DisplayFormat.Format = GlobalData.UnitPriceFormat;
				layoutItem.Control = calcEdit;
				break;
			}
			}
		}
		catch (Exception ex)
		{
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
		new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Purchase.frm_PurchaseRequestControl));
		this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		base.SuspendLayout();
		this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl1.Location = new System.Drawing.Point(0, 0);
		this.layoutControl1.Name = "layoutControl1";
		this.layoutControl1.Root = this.layoutControlGroup1;
		this.layoutControl1.Size = new System.Drawing.Size(834, 447);
		this.layoutControl1.TabIndex = 0;
		this.layoutControl1.Text = "layoutControl1";
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
		this.layoutControlGroup1.Name = "layoutControlGroup1";
		this.layoutControlGroup1.Size = new System.Drawing.Size(834, 447);
		this.layoutControlGroup1.TextVisible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(834, 447);
		base.Controls.Add(this.layoutControl1);
		base.Name = "frm_PurchaseRequestControl";
		base.ShowIcon = false;
		this.Text = "Purchase Request Control";
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		base.ResumeLayout(false);
	}
}
