using System;
using DevExpress.XtraEditors;

namespace Apzon.Forms.HumanResources.Salary;

internal class CustomFormatter : IFormatProvider, ICustomFormatter
{
	private ProgressBarControl _ProgressBar;

	private string _formatStr;

	private string _completeStr;

	public ProgressBarControl ProgressBar
	{
		get
		{
			return _ProgressBar;
		}
		set
		{
			_ProgressBar = value;
		}
	}

	public CustomFormatter(ProgressBarControl progressBarControl, string inProgressString, string completeString)
	{
		_ProgressBar = progressBarControl;
		_formatStr = inProgressString;
		_completeStr = completeString;
	}

	public object GetFormat(Type type)
	{
		return this;
	}

	public string Format(string format, object arg, IFormatProvider formatProvider)
	{
		ProgressBarControl progressBar = ((CustomFormatter)formatProvider).ProgressBar;
		if (progressBar.Position == progressBar.Properties.Maximum && progressBar.Properties.Maximum != 0)
		{
			return _completeStr;
		}
		return $"{progressBar.Position} of {progressBar.Properties.Maximum} Sent";
	}
}
