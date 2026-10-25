using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Forms.Helper;
using Apzon.Forms.Interfaces.Mrp;
using DevExpress.XtraPivotGrid;

namespace Apzon.Forms.Mrps;

public class frm_Schedule : VirSchedule
{
	public frm_Schedule()
	{
		InitComponent();
	}

	public frm_Schedule(int psCode)
		: base(psCode)
	{
		InitComponent();
	}

	public frm_Schedule(int psCode, string itemCode, DateTime? dueDate, string shiftCode)
		: base(psCode, itemCode, dueDate, shiftCode)
	{
		InitComponent();
	}

	private void InitComponent()
	{
		Text = "Schedule";
		EnableDisableControl(isEnable: true);
	}

	protected override void ShowingEditorExtentions(CancelPivotCellEditEventArgs e)
	{
		if (Function.ToString(chkDetailSchedule.EditValue).Equals("Y"))
		{
			e.Cancel = true;
		}
	}

	protected override void pgcDetail_CellDoubleClick(object sender, PivotCellEventArgs e)
	{
		try
		{
			if (e == null || e.ColumnField == null || !Function.ToString(chkDetailSchedule.EditValue).Equals("Y"))
			{
				return;
			}
			if (e.ColumnField.FieldName.Equals(field_Shift.FieldName))
			{
				PivotDrillDownDataSource mrpSource = pgcDetail.CreateDrillDownDataSource(e.ColumnIndex, e.RowIndex);
				if (mrpSource != null && mrpSource.RowCount > 0)
				{
					WinCommons._OpenProcessing();
					DataTable dataTable = pgcDetail.DataSourceTable();
					EnumerableRowCollection<DataRow> source = from t in dataTable.AsEnumerable()
						where Function.ToString(t["ItemCode"]).Equals(Function.ToString(mrpSource[0]["ItemCode"])) && Function.ToString(t["BomVersion"]).Equals(Function.ToString(mrpSource[0]["BomVersion"])) && Function.ToString(t["CardCode"]).Equals(Function.ToString(mrpSource[0]["CardCode"]))
						select t;
					if (!source.Any())
					{
						return;
					}
					List<DataRow> list = source.ToList();
					frm_DrillDownSourceCellMrp frm_DrillDownSourceCellMrp2 = new frm_DrillDownSourceCellMrp(list.CopyToDataTable(), txtFromDate.DateTime, txtFixDate.DateTime, txtToDate.DateTime, Enums.FormViewModeType.Add)
					{
						Text = string.Format("Detailed scheduling for items {0}", Function.ToString(mrpSource[0]["ItemCode"]))
					};
					frm_DrillDownSourceCellMrp2.ShowDialog(this);
					if (frm_DrillDownSourceCellMrp2.DialogResult == DialogResult.OK)
					{
						WinCommons._OpenProcessing();
						for (int num = 0; num < list.Count; num++)
						{
							dataTable.Rows.Remove(list[num]);
						}
						for (int num2 = 0; num2 < frm_DrillDownSourceCellMrp2.SourceDetail.Rows.Count; num2++)
						{
							DataRow dataRow = dataTable.NewRow();
							dataRow.ItemArray = frm_DrillDownSourceCellMrp2.SourceDetail.Rows[num2].ItemArray;
							dataTable.Rows.Add(dataRow);
						}
						SetDataPivot(dataTable);
						WinCommons._CloseProcessing();
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private void InitializeComponent()
	{
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Mrps.frm_Schedule));
		((System.ComponentModel.ISupportInitialize)base.txtInputCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.txtStatus.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.txtFromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.txtFromDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.txtToDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.txtToDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.txtFixDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.txtFixDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.txtType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.txtName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.txtCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.chkDetailSchedule.Properties).BeginInit();
		base.SuspendLayout();
		base.txtInputCode.Properties.Appearance.BackColor = System.Drawing.Color.PapayaWhip;
		base.txtInputCode.Properties.Appearance.Options.UseBackColor = true;
		base.txtInputCode.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		base.txtInputCode.Properties.AppearanceFocused.Options.UseBackColor = true;
		base.txtStatus.EditValue = "D";
		base.txtStatus.Properties.Appearance.BackColor = System.Drawing.Color.PapayaWhip;
		base.txtStatus.Properties.Appearance.Options.UseBackColor = true;
		base.txtStatus.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		base.txtStatus.Properties.AppearanceFocused.Options.UseBackColor = true;
		base.txtFromDate.EditValue = null;
		base.txtFromDate.Properties.Appearance.BackColor = System.Drawing.Color.PapayaWhip;
		base.txtFromDate.Properties.Appearance.Options.UseBackColor = true;
		base.txtFromDate.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		base.txtFromDate.Properties.AppearanceFocused.Options.UseBackColor = true;
		base.txtToDate.EditValue = null;
		base.txtToDate.Properties.Appearance.BackColor = System.Drawing.Color.PapayaWhip;
		base.txtToDate.Properties.Appearance.Options.UseBackColor = true;
		base.txtToDate.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		base.txtToDate.Properties.AppearanceFocused.Options.UseBackColor = true;
		base.btn_Save.EditValue = "Find";
		base.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		base.btn_Save.Text = "Find";
		base.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		base.txtFixDate.EditValue = null;
		base.txtFixDate.Properties.Appearance.BackColor = System.Drawing.Color.PapayaWhip;
		base.txtFixDate.Properties.Appearance.Options.UseBackColor = true;
		base.txtType.EditValue = "O";
		base.txtType.Properties.Appearance.BackColor = System.Drawing.Color.PapayaWhip;
		base.txtType.Properties.Appearance.Options.UseBackColor = true;
		base.txtName.Properties.Appearance.BackColor = System.Drawing.Color.PapayaWhip;
		base.txtName.Properties.Appearance.Options.UseBackColor = true;
		base.txtName.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		base.txtName.Properties.AppearanceFocused.Options.UseBackColor = true;
		base.txtCode.Properties.Appearance.BackColor = System.Drawing.Color.PapayaWhip;
		base.txtCode.Properties.Appearance.Options.UseBackColor = true;
		base.txtCode.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		base.txtCode.Properties.AppearanceFocused.Options.UseBackColor = true;
		base.btnAutoFillData.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnAutoFillData.ImageOptions.Image");
		base.chkDetailSchedule.EditValue = "N";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.ClientSize = new System.Drawing.Size(970, 603);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_Schedule.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_Schedule";
		((System.ComponentModel.ISupportInitialize)base.txtInputCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.txtStatus.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.txtFromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.txtFromDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.txtToDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.txtToDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.txtFixDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.txtFixDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.txtType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.txtName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.txtCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.chkDetailSchedule.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
