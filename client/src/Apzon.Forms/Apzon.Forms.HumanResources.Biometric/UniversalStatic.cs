using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Apzon.Forms.HumanResources.Biometric;

internal class UniversalStatic
{
	public const string acx_Disconnect = "Disconnected";

	public const string acx_Connect = "Conncected";

	public static bool ValidateIP(string addrString)
	{
		if (IPAddress.TryParse(addrString, out var _))
		{
			return true;
		}
		return false;
	}

	public static void DrawLineInFooter(Control control, Color color, int thickness)
	{
		int height = control.Height;
		DrawLine(control, color, 0, height, control.Width, height, thickness);
	}

	public static void DrawLine(Control control, Color color, int x, int y, int x1, int y1, int thickness)
	{
		control.CreateGraphics().DrawLine(new Pen(color, thickness), x, y, x1, y1);
	}

	public static bool PingTheDevice(string ipAdd)
	{
		try
		{
			IPAddress address = IPAddress.Parse(ipAdd);
			Ping ping = new Ping();
			PingOptions options = new PingOptions
			{
				DontFragment = true
			};
			string s = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
			byte[] bytes = Encoding.ASCII.GetBytes(s);
			int timeout = 120;
			if (ping.Send(address, timeout, bytes, options).Status == IPStatus.Success)
			{
				return true;
			}
			return false;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static string SplitPascal(string text)
	{
		return new Regex("([A-Z]+[a-z]+)").Replace(text, (Match m) => ((m.Value.Length > 3) ? m.Value : m.Value.ToLower()) + " ");
	}

	public static Image StringToImage(string imageString)
	{
		return Image.FromStream(new MemoryStream(Convert.FromBase64String(imageString)), useEmbeddedColorManagement: true, validateImageData: true);
	}

	public static string IntegerValidation(char[] enteredString, string actualString)
	{
		foreach (char item in enteredString.AsEnumerable())
		{
			if (char.IsDigit(item))
			{
				actualString += item;
				continue;
			}
			actualString.Replace(item, ' ');
			actualString.Trim();
		}
		return actualString;
	}

	public static void ValidateInteger(TextBox tbx)
	{
		string empty = string.Empty;
		char[] enteredString = tbx.Text.ToCharArray();
		tbx.Text = IntegerValidation(enteredString, empty);
		tbx.Select(tbx.Text.Length, 0);
	}

	public static void ChangeGridProperties(DataGridView dgvMaster)
	{
		dgvMaster.DefaultCellStyle.Font = new Font("Segoe UI", 8f);
		dgvMaster.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
		dgvMaster.RowHeadersWidth = 10;
		dgvMaster.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
		dgvMaster.RowHeadersVisible = false;
		dgvMaster.RowTemplate.DefaultCellStyle.Padding = new Padding(5, 0, 10, 0);
		foreach (DataGridViewColumn column in dgvMaster.Columns)
		{
			column.Resizable = DataGridViewTriState.False;
			column.ReadOnly = true;
		}
		dgvMaster.AllowUserToOrderColumns = true;
		dgvMaster.BackgroundColor = SystemColors.Control;
		dgvMaster.BorderStyle = BorderStyle.Fixed3D;
	}
}
