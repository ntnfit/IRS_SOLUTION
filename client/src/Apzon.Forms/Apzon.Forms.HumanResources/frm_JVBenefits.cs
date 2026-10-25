using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Entities.Models.Common;
using Apzon.HttpClientHelper;

namespace Apzon.Forms.HumanResources;

public class frm_JVBenefits : frm_JVInterviewStatus
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	public frm_JVBenefits()
	{
		FormController = "JVQuestion";
		tableName = "APZ_OBNF";
		FormId = "oBenefits";
		gridView1.ItemId = "APZ_OBNF";
		gridView1.FormId = "oBenefits";
		Text = "Benefits";
	}

	protected override void LoadResult(ref DataTable dt)
	{
		try
		{
			dt = _httpClient.JVQuestion.LoadBenefit();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected override HttpResult SaveResult(Tuple<DataTable, string> dt)
	{
		try
		{
			return _httpClient.JVQuestion.SaveBenefit(dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new HttpResult(Enums.MessageCode.Error, "Error");
		}
	}

	protected override DataTable DeleteResult(int _ID)
	{
		try
		{
			return _httpClient.JVQuestion.CheckBenefit(_ID);
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
		this.Text = "Benefits";
	}
}
