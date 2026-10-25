using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Forms.UserControls;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Forms;

public class frm_Test : XtraFormEx
{
	private IContainer components;

	private ImageSlider imgThanhnv;

	private SpinEditEx spinEditEx1;

	private RadioGroupEx radioGroupEx1;

	private MemoEditEx memoEditEx1;

	private MemoExEditEx memoExEditEx1;

	private LookUpEditUDF lookUpEditUDF1;

	public frm_Test()
	{
		InitializeComponent();
	}

	private void frm_Test_Load(object sender, EventArgs e)
	{
		string[] files = Directory.GetFiles("D:\\Picture");
		for (int i = 0; i < files.Length; i++)
		{
			Image image = Image.FromFile(files[i]);
			imgThanhnv.Images.Add(image);
		}
	}

	private void simpleButton1_Click(object sender, EventArgs e)
	{
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
		new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.frm_Test));
		this.imgThanhnv = new DevExpress.XtraEditors.Controls.ImageSlider();
		this.spinEditEx1 = new Apzon.Controls.Editor.SpinEditEx();
		this.radioGroupEx1 = new Apzon.Controls.Editor.RadioGroupEx();
		this.memoEditEx1 = new Apzon.Controls.Editor.MemoEditEx();
		this.memoExEditEx1 = new Apzon.Controls.Editor.MemoExEditEx();
		this.lookUpEditUDF1 = new Apzon.Forms.UserControls.LookUpEditUDF();
		((System.ComponentModel.ISupportInitialize)this.imgThanhnv).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.spinEditEx1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.radioGroupEx1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.memoEditEx1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.memoExEditEx1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lookUpEditUDF1.Properties).BeginInit();
		base.SuspendLayout();
		this.imgThanhnv.AutoSlide = DevExpress.XtraEditors.Controls.AutoSlide.Forward;
		this.imgThanhnv.Cursor = System.Windows.Forms.Cursors.Default;
		this.imgThanhnv.Location = new System.Drawing.Point(400, 108);
		this.imgThanhnv.Name = "imgThanhnv";
		this.imgThanhnv.Size = new System.Drawing.Size(353, 238);
		this.imgThanhnv.TabIndex = 0;
		this.imgThanhnv.Text = "imageSlider1";
		this.spinEditEx1.EditValue = new decimal(new int[4]);
		this.spinEditEx1.FieldName = null;
		this.spinEditEx1.IsSearchControl = false;
		this.spinEditEx1.Location = new System.Drawing.Point(89, 68);
		this.spinEditEx1.Name = "spinEditEx1";
		this.spinEditEx1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.spinEditEx1.Size = new System.Drawing.Size(100, 20);
		this.spinEditEx1.TabIndex = 1;
		this.radioGroupEx1.FieldName = null;
		this.radioGroupEx1.IsSearchControl = false;
		this.radioGroupEx1.Location = new System.Drawing.Point(109, 180);
		this.radioGroupEx1.Name = "radioGroupEx1";
		this.radioGroupEx1.Size = new System.Drawing.Size(100, 96);
		this.radioGroupEx1.TabIndex = 2;
		this.memoEditEx1.FieldName = null;
		this.memoEditEx1.IsSearchControl = false;
		this.memoEditEx1.Location = new System.Drawing.Point(400, 29);
		this.memoEditEx1.Name = "memoEditEx1";
		this.memoEditEx1.Size = new System.Drawing.Size(100, 96);
		this.memoEditEx1.TabIndex = 3;
		this.memoExEditEx1.FieldName = null;
		this.memoExEditEx1.IsSearchControl = false;
		this.memoExEditEx1.Location = new System.Drawing.Point(708, 30);
		this.memoExEditEx1.Name = "memoExEditEx1";
		this.memoExEditEx1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.memoExEditEx1.Size = new System.Drawing.Size(100, 20);
		this.memoExEditEx1.TabIndex = 4;
		this.lookUpEditUDF1.FieldName = null;
		this.lookUpEditUDF1.IsSearchControl = false;
		this.lookUpEditUDF1.Location = new System.Drawing.Point(845, 71);
		this.lookUpEditUDF1.Name = "lookUpEditUDF1";
		this.lookUpEditUDF1.Properties.AutoHeight = false;
		this.lookUpEditUDF1.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.lookUpEditUDF1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lookUpEditUDF1.Properties.DisplayMember = "Name";
		this.lookUpEditUDF1.Properties.NullText = "";
		this.lookUpEditUDF1.Properties.ShowFooter = false;
		this.lookUpEditUDF1.Properties.ShowHeader = false;
		this.lookUpEditUDF1.Properties.ValueMember = "ID";
		this.lookUpEditUDF1.Size = new System.Drawing.Size(100, 20);
		this.lookUpEditUDF1.TabIndex = 5;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1089, 523);
		base.Controls.Add(this.lookUpEditUDF1);
		base.Controls.Add(this.memoExEditEx1);
		base.Controls.Add(this.memoEditEx1);
		base.Controls.Add(this.radioGroupEx1);
		base.Controls.Add(this.spinEditEx1);
		base.Controls.Add(this.imgThanhnv);
		base.Name = "frm_Test";
		this.Text = "frm_Test";
		base.Load += new System.EventHandler(this.frm_Test_Load);
		((System.ComponentModel.ISupportInitialize)this.imgThanhnv).EndInit();
		((System.ComponentModel.ISupportInitialize)this.spinEditEx1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.radioGroupEx1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.memoEditEx1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.memoExEditEx1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lookUpEditUDF1.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
