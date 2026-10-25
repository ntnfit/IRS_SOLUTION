using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;

namespace Apzon.Forms.Administrator.Plugin;

public class PluginLoader
{
	public void LoadPlugins(PluginManagement z)
	{
		try
		{
			AggregateCatalog aggregateCatalog = new AggregateCatalog();
			DirectoryCatalog item = new DirectoryCatalog(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Extensions", "*.dll");
			AssemblyCatalog item2 = new AssemblyCatalog(Assembly.GetExecutingAssembly());
			aggregateCatalog.Catalogs.Add(item);
			aggregateCatalog.Catalogs.Add(item2);
			new CompositionContainer(aggregateCatalog).ComposeParts(z);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void LoadPlugins(PluginManagement z, string pluginName)
	{
		try
		{
			AggregateCatalog aggregateCatalog = new AggregateCatalog();
			DirectoryCatalog item = new DirectoryCatalog(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Extensions\\" + pluginName, "*.dll");
			AssemblyCatalog item2 = new AssemblyCatalog(Assembly.GetExecutingAssembly());
			aggregateCatalog.Catalogs.Add(item);
			aggregateCatalog.Catalogs.Add(item2);
			using CompositionContainer container = new CompositionContainer(aggregateCatalog);
			container.ComposeParts(z);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
