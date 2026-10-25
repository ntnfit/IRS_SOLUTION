using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms;

public class frm_LogViewer : XtraFormEx
{
	private IContainer components;

	private GridControlEx grd_LogViewer;

	private GridViewEx gv_LogViewer;

	public frm_LogViewer()
	{
		InitializeComponent();
	}

	private void frm_LogViewer_Load(object sender, EventArgs e)
	{
		int totalWidth = 8;
		DataTable dataTable = CreateTable();
		try
		{
			DirectoryInfo[] directories = new DirectoryInfo(Environment.CurrentDirectory + "\\Log").GetDirectories();
			foreach (DirectoryInfo directoryInfo in directories)
			{
				DirectoryInfo[] directories2 = directoryInfo.GetDirectories();
				foreach (DirectoryInfo directoryInfo2 in directories2)
				{
					FileInfo[] files = directoryInfo2.GetFiles();
					foreach (FileInfo fileInfo in files)
					{
						StreamReader streamReader = File.OpenText(fileInfo.FullName);
						string text;
						while ((text = streamReader.ReadLine()) != null)
						{
							string text2 = text.Replace("".PadRight(totalWidth, ' '), "\t");
							char[] separator = new char[1] { '\t' };
							string[] array = text2.Split(separator);
							int num;
							for (num = 0; num < array.Length; num++)
							{
								DataRow dataRow = dataTable.NewRow();
								dataRow["Year"] = directoryInfo.Name;
								dataRow["Month"] = directoryInfo2.Name;
								dataRow["Name"] = fileInfo.Name.Replace(".txt", " ");
								if (num < array.Length)
								{
									dataRow["Type"] = array[num].ToString().Replace("\t", " ");
								}
								if (num + 1 < array.Length)
								{
									dataRow["Date"] = array[num + 1].ToString().Replace("\t", " ");
								}
								if (num + 2 < array.Length)
								{
									dataRow["Time"] = array[num + 2].ToString().Replace("\t", " ");
								}
								if (num + 3 < array.Length)
								{
									dataRow["Form"] = array[num + 3].ToString().Replace("\t", " ");
								}
								if (num + 4 < array.Length)
								{
									dataRow["Message"] = array[num + 4].ToString().Replace("\t", " ");
								}
								dataTable.Rows.Add(dataRow);
								num += 6;
							}
							dataTable.AcceptChanges();
						}
						streamReader.Close();
						grd_LogViewer.DataSource = dataTable;
						gv_LogViewer.OptionsView.ColumnAutoWidth = false;
						gv_LogViewer.Columns["Year"].GroupIndex = 0;
						gv_LogViewer.Columns["Month"].GroupIndex = 1;
						gv_LogViewer.Columns["Name"].GroupIndex = 2;
						gv_LogViewer.Columns["Type"].Width = 180;
						gv_LogViewer.Columns["Date"].Width = 100;
						gv_LogViewer.Columns["Time"].Width = 100;
						gv_LogViewer.Columns["Form"].Width = 350;
						gv_LogViewer.Columns["Message"].Width = 300;
						gv_LogViewer.Columns["Type"].OptionsFilter.FilterPopupMode = FilterPopupMode.List;
						gv_LogViewer.Columns["Date"].OptionsFilter.FilterPopupMode = FilterPopupMode.List;
						gv_LogViewer.Columns["Time"].OptionsFilter.FilterPopupMode = FilterPopupMode.List;
						gv_LogViewer.Columns["Form"].OptionsFilter.FilterPopupMode = FilterPopupMode.List;
						gv_LogViewer.Columns["Message"].OptionsFilter.FilterPopupMode = FilterPopupMode.List;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private DataTable CreateTable()
	{
		try
		{
			DataTable dataTable = new DataTable();
			DataColumn column = new DataColumn
			{
				DataType = Type.GetType("System.String"),
				ColumnName = "Year"
			};
			dataTable.Columns.Add(column);
			column = new DataColumn
			{
				DataType = Type.GetType("System.String"),
				ColumnName = "Month"
			};
			dataTable.Columns.Add(column);
			column = new DataColumn
			{
				DataType = Type.GetType("System.String"),
				ColumnName = "Name"
			};
			dataTable.Columns.Add(column);
			column = new DataColumn
			{
				DataType = Type.GetType("System.String"),
				ColumnName = "Type"
			};
			dataTable.Columns.Add(column);
			column = new DataColumn
			{
				DataType = Type.GetType("System.String"),
				ColumnName = "Date"
			};
			dataTable.Columns.Add(column);
			column = new DataColumn
			{
				DataType = Type.GetType("System.String"),
				ColumnName = "Time"
			};
			dataTable.Columns.Add(column);
			column = new DataColumn
			{
				DataType = Type.GetType("System.String"),
				ColumnName = "Form"
			};
			dataTable.Columns.Add(column);
			column = new DataColumn
			{
				DataType = Type.GetType("System.String"),
				ColumnName = "Message"
			};
			dataTable.Columns.Add(column);
			return dataTable;
		}
		catch (Exception ex)
		{
			throw new Exception(ex.Message);
		}
	}

	private void gv_LogViewer_RowCellStyle(object sender, RowCellStyleEventArgs e)
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
		this.grd_LogViewer = new Apzon.Controls.Grid.GridControlEx();
		this.gv_LogViewer = new Apzon.Controls.Grid.GridViewEx();
		((System.ComponentModel.ISupportInitialize)this.grd_LogViewer).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_LogViewer).BeginInit();
		base.SuspendLayout();
		this.grd_LogViewer.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_LogViewer.Location = new System.Drawing.Point(0, 0);
		this.grd_LogViewer.MainView = this.gv_LogViewer;
		this.grd_LogViewer.Name = "grd_LogViewer";
		this.grd_LogViewer.Size = new System.Drawing.Size(817, 456);
		this.grd_LogViewer.TabIndex = 3;
		this.grd_LogViewer.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_LogViewer });
		this.gv_LogViewer.GridControl = this.grd_LogViewer;
		this.gv_LogViewer.IndicatorWidth = 30;
		this.gv_LogViewer.Name = "gv_LogViewer";
		this.gv_LogViewer.OptionsDetail.DetailMode = DevExpress.XtraGrid.Views.Grid.DetailMode.Default;
		this.gv_LogViewer.OptionsView.ShowGroupPanel = false;
		this.gv_LogViewer.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_LogViewer_RowCellStyle);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(817, 456);
		base.Controls.Add(this.grd_LogViewer);
		base.Name = "frm_LogViewer";
		base.ShowIcon = false;
		this.Text = "Log Viewer";
		base.Load += new System.EventHandler(this.frm_LogViewer_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_LogViewer).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_LogViewer).EndInit();
		base.ResumeLayout(false);
	}
}
