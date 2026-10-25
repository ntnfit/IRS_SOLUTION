using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Controls;
using Apzon.HttpClientHelper;

namespace Apzon.Forms.HumanResources;

public class frm_JVInterviewTypes : frm_JVInterviewStatus
{
	private IContainer components;

	public frm_JVInterviewTypes()
	{
		FormController = "JVInterview";
		tableName = "APZ_OHIT";
		Text = "Interview Types";
		FormId = "oInterviewType";
		gridView1.ItemId = "APZ_OHIT";
		gridView1.FormId = "oInterviewType";
	}

	protected override void LoadResult(ref DataTable dt)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			dt = httpClientHelper.Get("LoadInterviewTypes");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected new virtual DataTable DeleteResult(int _ID)
	{
		try
		{
			DataTable result;
			using (HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController))
			{
				result = httpClientHelper.Get("CheckInterviewTypes", new
				{
					ID = _ID
				});
			}
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
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
		this.Text = "Interview Types";
	}
}
