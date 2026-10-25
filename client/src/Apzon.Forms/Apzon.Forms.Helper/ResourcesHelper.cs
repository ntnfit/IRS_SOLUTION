using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Entities.Models.Common;
using Apzon.Global;

namespace Apzon.Forms.Helper;

public static class ResourcesHelper
{
	public static string GetValue(this ComponentResourceManager resources, string code)
	{
		return resources.GetString(code, CultureInfo.CreateSpecificCulture(GlobalData.CURRENTLANGUAGE));
	}

	public static string LoadCurrentLanguage()
	{
		try
		{
			string startupFolderName = Function.GetStartupFolderName();
			string pathRoot = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
			string text = string.Format("{0}\\Users\\All Users\\APZON\\{2}\\{1}\\Config\\Language.xml", pathRoot, Environment.UserName, startupFolderName);
			if (!File.Exists(text))
			{
				text = Application.StartupPath + "\\Language.xml";
			}
			if (!File.Exists(text))
			{
				return "";
			}
			List<Common.ListItems> list = (from t in XDocument.Load(text).Descendants("Lan")
				where t.Attribute("User") == null || Function.ToString(t.Attribute("User")?.Value) == GlobalData.UserName
				select new Common.ListItems
				{
					Value = Function.ToString(t.Attribute("User")?.Value),
					Text = Function.ToString(t.Attribute("Lang")?.Value)
				}).ToList();
			if (list.Count == 1)
			{
				return list[0].Text;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}

	public static void SaveCurrentLanguage()
	{
		try
		{
			string startupFolderName = Function.GetStartupFolderName();
			string pathRoot = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
			string text = string.Format("{0}\\Users\\All Users\\APZON\\{2}\\{1}\\Config", pathRoot, Environment.UserName, startupFolderName);
			string text2 = $"{text}\\Language.xml";
			XDocument xDocument = new XDocument();
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			if (!File.Exists(text2))
			{
				xDocument = new XDocument(new XElement("Lans", new XElement("Lan", new XAttribute("User", GlobalData.UserName), new XAttribute("Lang", GlobalData.CURRENTLANGUAGE))));
				xDocument.Save(text2);
				return;
			}
			xDocument = XDocument.Load(text2);
			XElement xElement = xDocument.Descendants("Lan").FirstOrDefault((XElement t) => Function.ToString(t.Attribute("User")?.Value) == GlobalData.UserName);
			if (xElement == null)
			{
				XElement content = new XElement("Lan", new XAttribute("User", GlobalData.UserName), new XAttribute("Lang", GlobalData.CURRENTLANGUAGE));
				xDocument.Root.Add(content);
			}
			else
			{
				xElement.Attribute("Lang").SetValue(GlobalData.CURRENTLANGUAGE);
			}
			xDocument.Save(text2);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}
}
