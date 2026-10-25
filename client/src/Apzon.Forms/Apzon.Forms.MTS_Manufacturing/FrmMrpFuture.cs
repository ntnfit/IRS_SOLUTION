using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using Apzon.Commons.Properties;
using Apzon.Controls.Form;
using Apzon.Entities.Models.Administrator;
using Apzon.Forms.Commons;
using Apzon.Global;

namespace Apzon.Forms.MTS_Manufacturing;

public class FrmMrpFuture : XtraFormEx
{
	private DataTable tableMRP = new DataTable();

	private int _colNumber_MRP;

	private string _idMRP;

	private IContainer components;

	public FrmMrpFuture(string idMRP)
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "FrmMrpFuture"))
		{
			InitializeComponent();
			_idMRP = idMRP;
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "FrmMrpFuture" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
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
		this.components = new System.ComponentModel.Container();
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.Text = "FrmMrpFuture";
	}
}
