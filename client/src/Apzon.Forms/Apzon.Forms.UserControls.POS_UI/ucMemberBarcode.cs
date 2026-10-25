using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Forms.UserControls.POS_UI;

public class ucMemberBarcode : XtraUserControl
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public Action<DataRow> GetMemberCode;

	public Action ForceOk;

	public DataRow MemRow;

	private IContainer components;

	private TableLayoutPanel tableLayoutPanel1;

	private LabelControlEx labelControlEx1;

	private SimpleButton btnCancel;

	private LabelControlEx labelControlEx2;

	private ButtonEditEx btnBarCode;

	public ucMemberBarcode()
	{
		InitializeComponent();
		btnBarCode.KeyDown += BtnBarCodeOnKeyDown;
		btnCancel.Click += BtnCancelOnClick;
		Load += delegate
		{
			btnBarCode.Focus();
		};
	}

	private void BtnCancelOnClick(object sender, EventArgs e)
	{
		try
		{
			GetMemberCode?.Invoke(null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void BtnBarCodeOnKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Return)
			{
				SUBMIT();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async void SUBMIT()
	{
		try
		{
			string text = Function.ToString(btnBarCode.EditValue);
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			DataSet dataSet = await _httpClient.RetailChooseFromList.Customer(GlobalData.UserName, GlobalData.StoreCode, text);
			if (dataSet.IsNotNull() && dataSet.Tables.Count > 1)
			{
				DataTable dataTable = dataSet.Tables[1];
				if (dataTable.Rows.Count > 0)
				{
					MemRow = dataTable.NewRow();
					DataRow dataRow = dataTable.Rows[0];
					MemRow.ItemArray = dataRow.ItemArray.Clone() as object[];
					GetMemberCode?.Invoke(dataTable.Rows[0]);
					ForceOk?.Invoke();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, PoinOfSale.msg_CusNotFound);
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, PoinOfSale.msg_CusNotFound);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btnBarCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		SUBMIT();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.UserControls.POS_UI.ucMemberBarcode));
		this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
		this.labelControlEx1 = new Apzon.Controls.Controls.LabelControlEx();
		this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
		this.btnBarCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.labelControlEx2 = new Apzon.Controls.Controls.LabelControlEx();
		this.tableLayoutPanel1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.btnBarCode.Properties).BeginInit();
		base.SuspendLayout();
		this.tableLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(215, 228, 189);
		this.tableLayoutPanel1.ColumnCount = 2;
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50f));
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50f));
		this.tableLayoutPanel1.Controls.Add(this.labelControlEx1, 0, 0);
		this.tableLayoutPanel1.Controls.Add(this.btnCancel, 1, 3);
		this.tableLayoutPanel1.Controls.Add(this.btnBarCode, 0, 2);
		this.tableLayoutPanel1.Controls.Add(this.labelControlEx2, 0, 1);
		this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
		this.tableLayoutPanel1.Name = "tableLayoutPanel1";
		this.tableLayoutPanel1.RowCount = 4;
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30f));
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10f));
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30f));
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30f));
		this.tableLayoutPanel1.Size = new System.Drawing.Size(600, 450);
		this.tableLayoutPanel1.TabIndex = 17;
		this.labelControlEx1.Appearance.Font = new System.Drawing.Font("Tahoma", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.labelControlEx1.Appearance.Options.UseFont = true;
		this.labelControlEx1.Appearance.Options.UseTextOptions = true;
		this.labelControlEx1.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.tableLayoutPanel1.SetColumnSpan(this.labelControlEx1, 2);
		this.labelControlEx1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.labelControlEx1.FieldName = "labelControlEx1";
		this.labelControlEx1.Location = new System.Drawing.Point(3, 3);
		this.labelControlEx1.Name = "labelControlEx1";
		this.labelControlEx1.Size = new System.Drawing.Size(594, 129);
		this.labelControlEx1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx1.TabIndex = 1;
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(228, 108, 10);
		this.btnCancel.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		this.btnCancel.Appearance.ForeColor = System.Drawing.Color.Yellow;
		this.btnCancel.Appearance.Options.UseBackColor = true;
		this.btnCancel.Appearance.Options.UseFont = true;
		this.btnCancel.Appearance.Options.UseForeColor = true;
		this.btnCancel.Location = new System.Drawing.Point(482, 396);
		this.btnCancel.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btnCancel.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(115, 51);
		this.btnCancel.TabIndex = 13;
		this.btnCancel.Text = "Quay lại";
		this.btnCancel.Visible = false;
		this.tableLayoutPanel1.SetColumnSpan(this.btnBarCode, 2);
		this.btnBarCode.Dock = System.Windows.Forms.DockStyle.Fill;
		this.btnBarCode.FieldName = null;
		this.btnBarCode.Location = new System.Drawing.Point(3, 183);
		this.btnBarCode.Name = "btnBarCode";
		this.btnBarCode.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(253, 234, 218);
		this.btnBarCode.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnBarCode.Properties.Appearance.Options.UseBackColor = true;
		this.btnBarCode.Properties.Appearance.Options.UseFont = true;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions1.Image");
		this.btnBarCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(editorButtonImageOptions, DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, null)
		});
		this.btnBarCode.Properties.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
		this.btnBarCode.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btnBarCode.Properties.NullText = "Barcode / Manegtic Card";
		this.btnBarCode.Size = new System.Drawing.Size(594, 40);
		this.btnBarCode.TabIndex = 14;
		this.btnBarCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnBarCode_ButtonClick);
		this.labelControlEx2.Appearance.Font = new System.Drawing.Font("Tahoma", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.labelControlEx2.Appearance.Options.UseFont = true;
		this.labelControlEx2.Appearance.Options.UseTextOptions = true;
		this.labelControlEx2.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.tableLayoutPanel1.SetColumnSpan(this.labelControlEx2, 2);
		this.labelControlEx2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.labelControlEx2.FieldName = "labelControlEx2";
		this.labelControlEx2.Location = new System.Drawing.Point(3, 138);
		this.labelControlEx2.Name = "labelControlEx2";
		this.labelControlEx2.Size = new System.Drawing.Size(594, 39);
		this.labelControlEx2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx2.TabIndex = 15;
		this.labelControlEx2.Text = "Quét mã thẻ thành viên";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.tableLayoutPanel1);
		base.Name = "ucMemberBarcode";
		base.Size = new System.Drawing.Size(600, 450);
		this.tableLayoutPanel1.ResumeLayout(false);
		this.tableLayoutPanel1.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.btnBarCode.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
