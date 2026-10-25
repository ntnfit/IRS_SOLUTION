using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Interfaces.Mrp;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Forms.Production;

public class frm_GenPurchaseFromMaterialShortage : VirGenPurchase
{
	private IContainer components;

	public frm_GenPurchaseFromMaterialShortage()
	{
		InitComponents();
	}

	public void InitComponents()
	{
		Controller = "MaterialShortageReport";
		Text = "Purchase From Material Shortage Report";
		FormMode = Enums.FormViewModeType.View;
		string formId = (gvDoc.FormId = "GenPurchaseFromMaterialShortage");
		FormId = formId;
		gvDoc.ItemId = "Lines";
	}

	protected override void LoadDataInputType()
	{
		try
		{
			List<Common.Table_Code> dataSource = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Name = "Material Shortage",
					Code = "M"
				}
			};
			txtInput.Properties.DataSource = dataSource;
			txtInput.EditValue = "M";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override void Code_ChooseFromList(object sender, ButtonPressedEventArgs e)
	{
		if (!Function.ToString(txtInput.EditValue).Equals("M"))
		{
			return;
		}
		if (e.Button.Index == 0)
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.MaterialShortageReport, false, Function.ToString(txtCode.EditValue), "", Enums.ItemObjectType.NoItemType, null, "")
			{
				MaterialShortageType = "P",
				MaterialShortageStatus = "A"
			};
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				txtCode.EditValue = Function.ToString(frm_CFL2.ChooseItem["Code"]);
			}
		}
		else
		{
			int num = Function.ParseInt(txtCode.EditValue);
			if (num != 0)
			{
				this.DrillDown(ApzObjectTypes.oMaterialShortageReport, ApzObjectTypes.oMaterialShortageReport, Function.ToString(num));
			}
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
		this.components = new System.ComponentModel.Container();
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.Text = "frm_GenPurchaseFromAtp";
	}
}
