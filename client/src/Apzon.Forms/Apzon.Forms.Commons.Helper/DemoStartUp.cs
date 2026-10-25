using System;
using DevExpress.XtraSplashScreen;

namespace Apzon.Forms.Commons.Helper;

internal class DemoStartUp : IObserver<string>
{
	void IObserver<string>.OnCompleted()
	{
		SplashScreenManager.CloseForm(throwExceptionIfAlreadyClosed: false, 1500, AppHelper.MainForm);
	}

	void IObserver<string>.OnNext(string status)
	{
		if (SplashScreenManager.Default == null)
		{
			SplashScreenManager.ShowForm(AppHelper.MainForm, typeof(DemoSplashScreen), useFadeIn: true, useFadeOut: true);
			SplashScreenManager.Default.SendCommand(DemoSplashScreenBase.SplashScreenCommand.UpdateLabelProductText, "Apzon");
			SplashScreenManager.Default.SendCommand(DemoSplashScreenBase.SplashScreenCommand.UpdateLabelDemoText, status);
		}
		else
		{
			SplashScreenManager.Default.SendCommand(DemoSplashScreenBase.SplashScreenCommand.UpdateLabel, status);
		}
	}

	void IObserver<string>.OnError(Exception error)
	{
		throw error;
	}
}
