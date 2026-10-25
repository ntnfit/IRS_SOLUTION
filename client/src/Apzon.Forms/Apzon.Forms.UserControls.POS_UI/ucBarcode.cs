using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Controls.Editor;
using Apzon.Controls.Grid;
using Apzon.HttpClientHelper;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Forms.UserControls.POS_UI;

public class ucBarcode : XtraUserControl
{
	private GridControlEx _retailGrid;

	public string CardCode;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	public ButtonEditEx btnBarCode;

	[Description("The gridcontrol which is associated with this control")]
	[Browsable(true)]
	[EditorBrowsable(EditorBrowsableState.Always)]
	public GridControlEx RetailGrid
	{
		get
		{
			return _retailGrid;
		}
		set
		{
			_retailGrid = value;
			Invalidate();
		}
	}

	public ucBarcode()
	{
		InitializeComponent();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.UserControls.POS_UI.ucBarcode));
		this.btnBarCode = new Apzon.Controls.Editor.ButtonEditEx();
		((System.ComponentModel.ISupportInitialize)this.btnBarCode.Properties).BeginInit();
		base.SuspendLayout();
		this.btnBarCode.Dock = System.Windows.Forms.DockStyle.Fill;
		this.btnBarCode.Location = new System.Drawing.Point(0, 0);
		this.btnBarCode.Name = "btnBarCode";
		this.btnBarCode.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(253, 234, 218);
		this.btnBarCode.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 14.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnBarCode.Properties.Appearance.Options.UseBackColor = true;
		this.btnBarCode.Properties.Appearance.Options.UseFont = true;
		this.btnBarCode.Properties.AutoHeight = false;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions1.Image");
		this.btnBarCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(editorButtonImageOptions, DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, null)
		});
		this.btnBarCode.Properties.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
		this.btnBarCode.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btnBarCode.Properties.NullText = "Barcode";
		this.btnBarCode.Size = new System.Drawing.Size(862, 40);
		this.btnBarCode.TabIndex = 8;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.btnBarCode);
		base.Name = "ucBarcode";
		base.Size = new System.Drawing.Size(862, 40);
		((System.ComponentModel.ISupportInitialize)this.btnBarCode.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
