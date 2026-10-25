using System;
using DevExpress.XtraSplashScreen;

namespace Apzon.Forms.Commons.Helper;

internal class DataGenerationProgress : IObserver<string>
{
	void IObserver<string>.OnNext(string status)
	{
		SplashScreenManager.Default.SendCommand(DemoSplashScreenBase.SplashScreenCommand.UpdateLabel, status);
	}

	void IObserver<string>.OnCompleted()
	{
	}

	void IObserver<string>.OnError(Exception error)
	{
		throw error;
	}
}
