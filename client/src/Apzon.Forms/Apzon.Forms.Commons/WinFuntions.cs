using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Controls.Pivot;
using Apzon.Controls.Tree;
using Apzon.Entities.Models;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.UserFields;
using Apzon.Forms.Administrator;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Commons.UDF;
using Apzon.Forms.Commons.UDO;
using Apzon.Forms.Commons.UDT;
using Apzon.Forms.Helper;
using Apzon.Forms.Inventory.InventoryTransaction;
using Apzon.Forms.Properties;
using Apzon.Forms.Purchase;
using Apzon.Forms.Reports;
using Apzon.Forms.Sales;
using Apzon.Forms.UserControls;
using Apzon.Forms.UserControls.BatchAndSerial;
using Apzon.Global;
using Apzon.HttpClientHelper;
using Commons.Helper;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.ReportAppServer.ClientDoc;
using CrystalDecisions.ReportAppServer.Controllers;
using CrystalDecisions.ReportAppServer.DataDefModel;
using CrystalDecisions.ReportAppServer.ReportDefModel;
using CrystalDecisions.Shared;
using DevExpress.Data;
using DevExpress.Data.Extensions;
using DevExpress.DataAccess.Sql;
using DevExpress.Spreadsheet;
using DevExpress.Utils;
using DevExpress.Utils.Win;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.UI;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
using ExcelDataReader;
using Microsoft.Win32;
using SAPbobsCOM;

namespace Apzon.Forms.Commons;

internal class WinFuntions
{
	private delegate void _DaddComboBox(ComboBoxEdit _cb, List<Common.Table_Code> _list, bool _selectFirstItem);

	public class NetworkShareAccesser : IDisposable
	{
		[StructLayout(LayoutKind.Sequential)]
		private class NETRESOURCE
		{
			public int dwScope;

			public int dwType;

			public int dwDisplayType;

			public int dwUsage;

			public string lpLocalName = "";

			public string lpRemoteName = "";

			public string lpComment = "";

			public string lpProvider = "";
		}

		private string _remoteUncName;

		private string _remoteComputerName;

		private const int RESOURCE_CONNECTED = 1;

		private const int RESOURCE_GLOBALNET = 2;

		private const int RESOURCE_REMEMBERED = 3;

		private const int RESOURCETYPE_ANY = 0;

		private const int RESOURCETYPE_DISK = 1;

		private const int RESOURCETYPE_PRINT = 2;

		private const int RESOURCEDISPLAYTYPE_GENERIC = 0;

		private const int RESOURCEDISPLAYTYPE_DOMAIN = 1;

		private const int RESOURCEDISPLAYTYPE_SERVER = 2;

		private const int RESOURCEDISPLAYTYPE_SHARE = 3;

		private const int RESOURCEDISPLAYTYPE_FILE = 4;

		private const int RESOURCEDISPLAYTYPE_GROUP = 5;

		private const int RESOURCEUSAGE_CONNECTABLE = 1;

		private const int RESOURCEUSAGE_CONTAINER = 2;

		private const int CONNECT_INTERACTIVE = 8;

		private const int CONNECT_PROMPT = 16;

		private const int CONNECT_REDIRECT = 128;

		private const int CONNECT_UPDATE_PROFILE = 1;

		private const int CONNECT_COMMANDLINE = 2048;

		private const int CONNECT_CMD_SAVECRED = 4096;

		private const int CONNECT_LOCALDRIVE = 256;

		private const int NO_ERROR = 0;

		private const int ERROR_ACCESS_DENIED = 5;

		private const int ERROR_ALREADY_ASSIGNED = 85;

		private const int ERROR_BAD_DEVICE = 1200;

		private const int ERROR_BAD_NET_NAME = 67;

		private const int ERROR_BAD_PROVIDER = 1204;

		private const int ERROR_CANCELLED = 1223;

		private const int ERROR_EXTENDED_ERROR = 1208;

		private const int ERROR_INVALID_ADDRESS = 487;

		private const int ERROR_INVALID_PARAMETER = 87;

		private const int ERROR_INVALID_PASSWORD = 1216;

		private const int ERROR_MORE_DATA = 234;

		private const int ERROR_NO_MORE_ITEMS = 259;

		private const int ERROR_NO_NET_OR_BAD_PATH = 1203;

		private const int ERROR_NO_NETWORK = 1222;

		private const int ERROR_BAD_PROFILE = 1206;

		private const int ERROR_CANNOT_OPEN_PROFILE = 1205;

		private const int ERROR_DEVICE_IN_USE = 2404;

		private const int ERROR_NOT_CONNECTED = 2250;

		private const int ERROR_OPEN_FILES = 2401;

		public string RemoteComputerName
		{
			get
			{
				return _remoteComputerName;
			}
			set
			{
				_remoteComputerName = value;
				_remoteUncName = "\\\\" + _remoteComputerName;
			}
		}

		public string UserName { get; set; }

		public string Password { get; set; }

		[DllImport("Mpr.dll")]
		private static extern int WNetUseConnection(IntPtr hwndOwner, NETRESOURCE lpNetResource, string lpPassword, string lpUserID, int dwFlags, string lpAccessName, string lpBufferSize, string lpResult);

		[DllImport("Mpr.dll")]
		private static extern int WNetCancelConnection2(string lpName, int dwFlags, bool fForce);

		public static NetworkShareAccesser Access(string remoteComputerName)
		{
			return new NetworkShareAccesser(remoteComputerName);
		}

		public static NetworkShareAccesser Access(string remoteComputerName, string domainOrComuterName, string userName, string password)
		{
			return new NetworkShareAccesser(remoteComputerName, domainOrComuterName + "\\" + userName, password);
		}

		public static NetworkShareAccesser Access(string remoteComputerName, string userName, string password)
		{
			return new NetworkShareAccesser(remoteComputerName, userName, password);
		}

		private NetworkShareAccesser(string remoteComputerName)
		{
			RemoteComputerName = remoteComputerName;
			ConnectToShare(_remoteUncName, null, null, promptUser: true);
		}

		private NetworkShareAccesser(string remoteComputerName, string userName, string password)
		{
			RemoteComputerName = remoteComputerName;
			UserName = userName;
			Password = password;
			ConnectToShare(_remoteUncName, UserName, Password, promptUser: false);
		}

		private void ConnectToShare(string remoteUnc, string username, string password, bool promptUser)
		{
			NETRESOURCE lpNetResource = new NETRESOURCE
			{
				dwType = 1,
				lpRemoteName = remoteUnc
			};
			int num = ((!promptUser) ? WNetUseConnection(IntPtr.Zero, lpNetResource, password, username, 0, null, null, null) : WNetUseConnection(IntPtr.Zero, lpNetResource, "", "", 24, null, null, null));
			if (num != 0)
			{
				throw new Win32Exception(num);
			}
		}

		private void DisconnectFromShare(string remoteUnc)
		{
			int num = WNetCancelConnection2(remoteUnc, 1, fForce: false);
			if (num != 0)
			{
				throw new Win32Exception(num);
			}
		}

		public void Dispose()
		{
			DisconnectFromShare(_remoteUncName);
		}
	}

	private static readonly string[] VietNamChar = new string[16]
	{
		"aAeEoOuUiIdDyY", "áàạảãâấầậẩẫăắằặẳẵ", "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ", "éèẹẻẽêếềệểễ", "ÉÈẸẺẼÊẾỀỆỂỄ", "óòọỏõôốồộổỗơớờợởỡ", "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ", "úùụủũưứừựửữ", "ÚÙỤỦŨƯỨỪỰỬỮ", "íìịỉĩ",
		"ÍÌỊỈĨ", "đ", "Đ", "ýỳỵỷỹ", "ÝỲỴỶỸ", "0123456789_-,BCFGHJKLMNPQRSTVWXZbcfghjklmnpqrstvwxz\b"
	};

	private static byte[] _salt = Encoding.ASCII.GetBytes("tu89geji340t89u2");

	public static XtraFormEx UFrmMain
	{
		get
		{
			FormCollection openForms = Application.OpenForms;
			for (int i = 0; i < openForms.Count; i++)
			{
				Form form = openForms[i];
				if (form.IsMdiContainer)
				{
					return (XtraFormEx)form;
				}
			}
			return null;
		}
	}

	public static string _StartupPath => Application.StartupPath + "\\";

	internal static void AutoFilterCheckBox(GridViewEx gv)
	{
		if (gv.Columns.Count > 0)
		{
			for (int i = 0; i < gv.Columns.Count; i++)
			{
				gv.Columns[i].OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
			}
		}
	}

	internal static void _addComboBox(ComboBoxEdit _cb, List<Common.Table_ID> _list)
	{
		try
		{
			if (_list == null)
			{
				return;
			}
			int num = -1;
			int selectedIndex = -1;
			_cb.Properties.Items.Clear();
			foreach (Common.Table_ID item in _list)
			{
				_cb.Properties.Items.Add(new Common.ListItems(item.Name, item.ID.ToString().Trim()));
				num++;
				if (item.Name.Trim().Equals("None"))
				{
					selectedIndex = num;
				}
			}
			_cb.SelectedIndex = selectedIndex;
		}
		catch (Exception ex)
		{
			XtraMessageBox.Show("lỗi" + ex);
		}
	}

	internal static void _addComboBox(ComboBoxEdit _cb, List<Common.Table_ID> _list, bool _selectFirstItem)
	{
		try
		{
			if (_list == null)
			{
				return;
			}
			_cb.Properties.Items.Clear();
			foreach (Common.Table_ID item in _list)
			{
				_cb.Properties.Items.Add(new Common.ListItems(item.Name, item.ID.ToString().Trim()));
			}
			if (_selectFirstItem)
			{
				_cb.SelectedIndex = 0;
			}
		}
		catch (Exception ex)
		{
			XtraMessageBox.Show("lỗi" + ex);
		}
	}

	internal static void _addComboBoxReposity(RepositoryItemComboBox _cb, List<Common.Table_ID_Code> _list)
	{
		try
		{
			if (_list == null)
			{
				return;
			}
			int num = -1;
			_cb.Items.Clear();
			foreach (Common.Table_ID_Code item in _list)
			{
				_cb.Items.Add(new Common.ListItems(item.Name, item.ID.ToString().Trim()));
				num++;
				item.Name.Trim().Equals("None");
			}
		}
		catch (Exception ex)
		{
			XtraMessageBox.Show(ex.Message);
		}
	}

	internal static void _addComboBox(ComboBoxEdit _cb, List<Common.Table_Code> _list)
	{
		try
		{
			if (_list == null)
			{
				return;
			}
			int num = -1;
			int selectedIndex = -1;
			_cb.Properties.Items.Clear();
			foreach (Common.Table_Code item in _list)
			{
				_cb.Properties.Items.Add(new Common.ListItems(item.Name, item.Code.Trim()));
				num++;
				if (item.Name.Trim().Equals("None"))
				{
					selectedIndex = num;
				}
			}
			_cb.SelectedIndex = selectedIndex;
		}
		catch (Exception ex)
		{
			XtraMessageBox.Show(ex.Message);
		}
	}

	internal static void _addComboBox(ComboBoxEdit _cb, List<Common.Table_Code> _list, bool _selectFirstItem)
	{
		try
		{
			if (_list == null)
			{
				return;
			}
			if (_cb.InvokeRequired)
			{
				_DaddComboBox method = _addComboBox;
				_cb.Invoke(method, _cb, _list, _selectFirstItem);
				return;
			}
			_cb.Properties.Items.Clear();
			foreach (Common.Table_Code item in _list)
			{
				_cb.Properties.Items.Add(new Common.ListItems(item.Name, item.Code.Trim()));
			}
			if (_selectFirstItem)
			{
				_cb.SelectedIndex = 0;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	internal static void _addComboBoxBlank(ComboBoxEdit _cb, List<Common.Table_Code> _list, bool _blank)
	{
		try
		{
			if (_list == null)
			{
				return;
			}
			_cb.Properties.Items.Clear();
			if (_blank)
			{
				_cb.Properties.Items.Add(new Common.ListItems(string.Empty, "-1"));
			}
			foreach (Common.Table_Code item in _list)
			{
				_cb.Properties.Items.Add(new Common.ListItems(item.Name, item.Code.Trim()));
			}
		}
		catch (Exception ex)
		{
			XtraMessageBox.Show(ex.Message);
		}
	}

	internal static void _addComboBox(ComboBoxEdit _cb, List<Common.Table_ID_Code> _list)
	{
		try
		{
			if (_list == null)
			{
				return;
			}
			int num = -1;
			int selectedIndex = -1;
			_cb.Properties.Items.Clear();
			foreach (Common.Table_ID_Code item in _list)
			{
				_cb.Properties.Items.Add(new Common.ListItems(item.Name.Trim(), item.ID.ToString().Trim()));
				num++;
				if (item.Name.Trim().Equals("None"))
				{
					selectedIndex = num;
				}
			}
			_cb.SelectedIndex = selectedIndex;
		}
		catch (Exception ex)
		{
			XtraMessageBox.Show(ex.Message);
		}
	}

	internal static void _addComboBox(ComboBoxEdit _cb, List<Common.Table_ID_Code> _list, bool _selectFirstItem)
	{
		try
		{
			if (_list == null)
			{
				return;
			}
			_cb.Properties.Items.Clear();
			foreach (Common.Table_ID_Code item in _list)
			{
				_cb.Properties.Items.Add(new Common.ListItems(item.Name.Trim(), item.ID.ToString().Trim()));
			}
			if (_selectFirstItem)
			{
				_cb.SelectedIndex = 0;
			}
		}
		catch (Exception ex)
		{
			XtraMessageBox.Show(ex.Message);
		}
	}

	internal static void _addCheckedComboBox(CheckedComboBoxEdit _cb, List<Common.Table_ID> _list)
	{
		try
		{
			if (_list == null)
			{
				return;
			}
			_cb.Properties.Items.Clear();
			foreach (Common.Table_ID item in _list)
			{
				_cb.Properties.Items.Add(item.ID, item.Name);
			}
		}
		catch (Exception ex)
		{
			XtraMessageBox.Show(ex.Message);
		}
	}

	internal static void _addCheckedComboBox(CheckedComboBoxEdit _cb, List<Common.Table_Code> _list)
	{
		try
		{
			if (_list == null)
			{
				return;
			}
			_cb.Properties.Items.Clear();
			foreach (Common.Table_Code item in _list)
			{
				_cb.Properties.Items.Add(item.Code, item.Name);
			}
		}
		catch (Exception ex)
		{
			XtraMessageBox.Show(ex.Message);
		}
	}

	internal static void _addCheckedComboBox(CheckedComboBoxEdit _cb, List<Common.Table_ID_Code> _list)
	{
		try
		{
			if (_list == null)
			{
				return;
			}
			_cb.Properties.Items.Clear();
			foreach (Common.Table_ID_Code item in _list)
			{
				_cb.Properties.Items.Add(item.ID, item.Name);
			}
		}
		catch (Exception ex)
		{
			XtraMessageBox.Show(ex.Message);
		}
	}

	internal static void _addCheckedComboBoxBlank(CheckedComboBoxEdit _cb, List<Common.Table_ID> _list, bool _blank)
	{
		try
		{
			if (_list == null)
			{
				return;
			}
			_cb.Properties.Items.Clear();
			if (_blank)
			{
				_cb.Properties.Items.Add("-1", "Empty");
			}
			foreach (Common.Table_ID item in _list)
			{
				_cb.Properties.Items.Add(item.ID, item.Name);
			}
		}
		catch (Exception ex)
		{
			XtraMessageBox.Show(ex.Message);
		}
	}

	internal static void _addCheckedListBox(CheckedListBoxControl _cb, List<Common.Table_ID> _list)
	{
		try
		{
			if (_list == null)
			{
				return;
			}
			_cb.Items.Clear();
			foreach (Common.Table_ID item in _list)
			{
				_cb.Items.Add(item.ID.ToString(), item.Name);
			}
		}
		catch (Exception ex)
		{
			XtraMessageBox.Show(ex.Message);
		}
	}

	internal static void _addCheckedListBox(CheckedListBoxControl _cb, List<Common.Table_Code> _list)
	{
		try
		{
			if (_list == null)
			{
				return;
			}
			_cb.Items.Clear();
			foreach (Common.Table_Code item in _list)
			{
				_cb.Items.Add(item.Code, item.Code + " - " + item.Name);
			}
		}
		catch (Exception ex)
		{
			XtraMessageBox.Show(ex.Message);
		}
	}

	internal static void _addCheckedListBox(CheckedListBoxControl _cb, List<Common.Table_ID_Code> _list)
	{
		try
		{
			if (_list == null)
			{
				return;
			}
			_cb.Items.Clear();
			foreach (Common.Table_ID_Code item in _list)
			{
				_cb.Items.Add(item.ID.ToString(), item.Code + " - " + item.Name);
			}
		}
		catch (Exception ex)
		{
			XtraMessageBox.Show(ex.Message);
		}
	}

	internal static void LoadApprovalStatus(RepositoryItemLookUpEdit rep_ApprovalStatus)
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Code", typeof(string));
			dataTable.Columns.Add("Name", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow["Code"] = "-";
			dataRow["Name"] = "Without";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = "W";
			dataRow["Name"] = "Pending";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = "Y";
			dataRow["Name"] = "Approved";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = "N";
			dataRow["Name"] = "Rejected";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = "P";
			dataRow["Name"] = "Generated";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = "A";
			dataRow["Name"] = "Generated by Authorizer";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = "C";
			dataRow["Name"] = "Canceled";
			dataTable.Rows.Add(dataRow);
			rep_ApprovalStatus.DataSource = dataTable;
			rep_ApprovalStatus.DisplayMember = "Name";
			rep_ApprovalStatus.ValueMember = "Code";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	internal static void LoadDocumentStatus(RepositoryItemLookUpEdit rep_Status)
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Code", typeof(string));
			dataTable.Columns.Add("Name", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow["Code"] = "O";
			dataRow["Name"] = "Open";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = "C";
			dataRow["Name"] = "Closed";
			dataTable.Rows.Add(dataRow);
			rep_Status.DataSource = dataTable;
			rep_Status.DisplayMember = "Name";
			rep_Status.ValueMember = "Code";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	internal static void _selectItemCheckedListBoxByListValue(CheckedListBoxControl _cb, List<Common.Table_ID> _list)
	{
		for (int i = 0; i < _cb.Items.Count; i++)
		{
			string text = _cb.Items[i].Value.ToString();
			foreach (Common.Table_ID item in _list)
			{
				if (text.Equals(item.ID.ToString().Trim()))
				{
					_cb.Items[i].CheckState = CheckState.Checked;
				}
			}
		}
	}

	internal static void _selectItemCheckedListBoxByListValue(CheckedListBoxControl _cb, int[] _ArrayValue)
	{
		for (int i = 0; i < _cb.Items.Count; i++)
		{
			string text = _cb.Items[i].Value.ToString();
			foreach (int num in _ArrayValue)
			{
				if (text.Equals(num.ToString()))
				{
					_cb.Items[i].CheckState = CheckState.Checked;
				}
			}
		}
	}

	internal static void _selectItemCheckedListBoxByListValue(CheckedListBoxControl _cb, string[] _ArrayValue)
	{
		for (int i = 0; i < _cb.Items.Count; i++)
		{
			string text = _cb.Items[i].Value.ToString();
			foreach (string text2 in _ArrayValue)
			{
				if (text.Equals(text2.Trim()))
				{
					_cb.Items[i].CheckState = CheckState.Checked;
				}
			}
		}
	}

	internal static void _UnCheckedAllItemChekedListBox(CheckedListBoxControl _cb)
	{
		for (int i = 0; i < _cb.Items.Count; i++)
		{
			_cb.Items[i].CheckState = CheckState.Unchecked;
		}
	}

	internal static int[] _getIntValueChecked_CheckedListBox(CheckedListBoxControl _listbox)
	{
		if (_listbox.Text.Equals(""))
		{
			return null;
		}
		int[] array = new int[_listbox.CheckedItems.Count];
		try
		{
			int num = 0;
			for (int i = 0; i < _listbox.Items.Count; i++)
			{
				if (_listbox.Items[i].CheckState == CheckState.Checked)
				{
					array[num] = int.Parse(_listbox.Items[i].Value.ToString());
					num++;
				}
			}
			if (num == 0)
			{
				array = null;
			}
		}
		catch (Exception)
		{
			array = null;
		}
		return array;
	}

	internal static string[] _getStringValueChecked_CheckedListBox(CheckedListBoxControl _listbox)
	{
		if (_listbox.Text.Equals(""))
		{
			return null;
		}
		string[] array = new string[_listbox.CheckedItems.Count];
		try
		{
			int num = 0;
			for (int i = 0; i < _listbox.Items.Count; i++)
			{
				if (_listbox.Items[i].CheckState == CheckState.Checked)
				{
					array[num] = _listbox.Items[i].Value.ToString();
					num++;
				}
			}
		}
		catch (Exception)
		{
			array = null;
		}
		return array;
	}

	internal static void _selectItemComboBoxByValue(ComboBoxEdit _cb, string _value)
	{
		if (_value == null)
		{
			return;
		}
		for (int i = 0; i < _cb.Properties.Items.Count; i++)
		{
			if (((Common.ListItems)_cb.Properties.Items[i]).Value.Trim().Equals(_value.Trim()))
			{
				_cb.SelectedIndex = i;
				break;
			}
		}
	}

	internal static void _selectItemComboBoxByIndex(ComboBoxEdit _cb, int _index)
	{
		if (_index >= 0)
		{
			_cb.SelectedIndex = _index;
		}
	}

	internal static void _selectItemComboBoxByText(ComboBoxEdit _cb, string _text)
	{
		if (_text == null)
		{
			return;
		}
		for (int i = 0; i < _cb.Properties.Items.Count; i++)
		{
			if (((Common.ListItems)_cb.Properties.Items[i]).Text.Trim().Equals(_text.Trim()))
			{
				_cb.SelectedIndex = i;
				break;
			}
		}
	}

	public static string SetControlValue(Control ctl)
	{
		if (ctl is TextEdit)
		{
			new TextEdit().Text = string.Empty;
		}
		if (ctl is ComboBoxEdit)
		{
			new ComboBoxEdit().EditValue = string.Empty;
		}
		if (ctl is CalcEdit)
		{
			new CalcEdit().EditValue = string.Empty;
		}
		if (ctl is DateEdit)
		{
			new DateEdit().EditValue = string.Empty;
		}
		if (ctl is CheckEdit)
		{
			new CheckEdit().EditValue = null;
		}
		return "";
	}

	public static void SetControlsValueAndType_Empty(Control.ControlCollection lstControl)
	{
		foreach (Control item in lstControl)
		{
			SetControlValue(item);
		}
	}

	public static void CustomizeAppearanceGridView(GridViewEx gv)
	{
		gv.Appearance.FocusedRow.Font = new System.Drawing.Font("Arial", 8.25f);
		gv.Appearance.FocusedRow.ForeColor = Color.FromArgb(242, 144, 0);
		gv.Appearance.FocusedRow.Options.UseFont = true;
		gv.Appearance.FocusedRow.Options.UseForeColor = true;
		gv.Appearance.HeaderPanel.Font = new System.Drawing.Font("Arial", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
		gv.Appearance.HeaderPanel.ForeColor = Color.FromArgb(128, 64, 0);
		gv.Appearance.HeaderPanel.Options.UseFont = true;
		gv.Appearance.HeaderPanel.Options.UseForeColor = true;
		gv.Appearance.SelectedRow.ForeColor = Color.FromArgb(242, 144, 0);
		gv.Appearance.SelectedRow.Options.UseForeColor = true;
	}

	public static List<string> GetNumber(string madk)
	{
		try
		{
			List<string> list = new List<string>();
			string item = string.Join("", from c in madk
				where char.IsDigit(c)
				select (c));
			list.Add(item);
			return list;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new List<string>();
		}
	}

	public static List<string> GetChains(string madk)
	{
		try
		{
			List<string> list = new List<string>();
			string item = string.Join("", from c in madk
				where char.IsLetter(c)
				select (c));
			list.Add(item);
			return list;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new List<string>();
		}
	}

	public static List<Tuple<string, string>> GetDocLineUDFNameAndType()
	{
		List<Tuple<string, string>> list = new List<Tuple<string, string>>();
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			DataTable dataTable = httpClientHelper.Get("GetDocLineUDFNameAndType");
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					Tuple<string, string> item = new Tuple<string, string>(Function.ToString(dataTable.Rows[i][0]), Function.ToString(dataTable.Rows[i][1]));
					list.Add(item);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return list;
	}

	public static Dictionary<string, string> GetDocLineUdfNameAndTypeDictionary()
	{
		new List<Tuple<string, string>>();
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			DataTable dataTable = httpClientHelper.Get("GetDocLineUDFNameAndType");
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					dictionary.Add(Function.ToString(dataTable.Rows[i][0]), Function.ToString(dataTable.Rows[i][1]).ToLower());
				}
				return dictionary;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new Dictionary<string, string>();
	}

	public static List<Tuple<string, string>> GetDocUDFNameAndType()
	{
		List<Tuple<string, string>> list = new List<Tuple<string, string>>();
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			DataTable dataTable = httpClientHelper.Get("GetDocUDFNameAndType");
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					Tuple<string, string> item = new Tuple<string, string>(Function.ToString(dataTable.Rows[i][0]), Function.ToString(dataTable.Rows[i][1]).ToLower());
					list.Add(item);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return list;
	}

	public static List<UserFieldsImpl> GetUDFByTableName(DataRow dr, string tableName)
	{
		List<UserFieldsImpl> list = new List<UserFieldsImpl>();
		try
		{
			IHttpClient httClient = HttpClientHelperFactory.GetHttpClient();
			List<UserFieldsImpl> lstUDFData = Task.Run(async () => await httClient.Udf.GetListField(tableName)).Result;
			IEnumerable<DataColumn> enumerable = from DataColumn t in dr.Table.Columns
				where t.ColumnName.StartsWith("U_")
				select t;
			if (lstUDFData != null && lstUDFData.Count > 0 && enumerable != null)
			{
				enumerable.Count();
				_ = 0;
			}
			if (lstUDFData != null && lstUDFData.Count > 0 && enumerable != null && enumerable.Count() > 0)
			{
				int iUDF;
				for (iUDF = lstUDFData.Count - 1; iUDF >= 0; iUDF--)
				{
					if (!enumerable.Any((DataColumn t) => string.Compare(t.ColumnName.Remove(0, 2), lstUDFData[iUDF].Name, ignoreCase: true) == 0))
					{
						lstUDFData.Remove(lstUDFData[iUDF]);
					}
				}
				foreach (UserFieldsImpl item in lstUDFData)
				{
					UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
					userFieldsImpl.Type = item.Type;
					userFieldsImpl.Name = item.Name;
					userFieldsImpl.SubType = item.SubType;
					string columnName = "U_" + item.Name;
					switch (item.Type)
					{
					case BoFieldTypes.db_Alpha:
						userFieldsImpl.Value = Function.ToString(dr[columnName]);
						break;
					case BoFieldTypes.db_Memo:
						userFieldsImpl.Value = Function.ToString(dr[columnName]);
						break;
					case BoFieldTypes.db_Numeric:
						if (item.SubType == BoFldSubTypes.st_Time)
						{
							userFieldsImpl.Value = Function.ParseShort(Function.ParseSAPHourToString(dr[columnName]));
						}
						else
						{
							userFieldsImpl.Value = Function.ParseDouble(dr[columnName]);
						}
						break;
					case BoFieldTypes.db_Date:
						if (item.SubType == BoFldSubTypes.st_Time)
						{
							userFieldsImpl.Value = Function.ParseShort(Function.ParseSAPHourToString(dr[columnName]));
						}
						else
						{
							userFieldsImpl.Value = Function.ParseDateTime(dr[columnName]);
						}
						break;
					case BoFieldTypes.db_Float:
						userFieldsImpl.Value = Function.ParseFloat(dr[columnName]);
						break;
					}
					list.Add(userFieldsImpl);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return list;
	}

	internal static void CopyToDocument(ApzObjectTypes fromtype, ApzObjectTypes totype, XtraFormEx frmcopy, GridViewEx gv, bool isReserveInvoice = false, bool isCancel = false)
	{
		WinCommons._OpenProcessing();
		try
		{
			if (GlobalData.IsMultiBranch)
			{
				int branch = Function.ParseInt(gv.GetFocusedRowCellValue("BPLId"));
				if (branch != 0 && GlobalData.BranchList.IsNotNull() && GlobalData.BranchList.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["BPLId"]) == branch) == null)
				{
					string text = Function.ToString(gv.GetFocusedRowCellDisplayText("BPLId"));
					WinCommons.Error(Enums.MessageCode.YouAreNotAssignedToBranchAndCannotCopyBaseDocumentToTargetDocument, "", text);
					WinCommons._CloseProcessing();
					return;
				}
			}
			int[] selectedRows = gv.GetSelectedRows();
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			for (int num = 0; num < selectedRows.Length; num++)
			{
				string item = Function.ToString(gv.GetRowCellValue(selectedRows[num], "CardCode"));
				if (list.Count > 0)
				{
					if (!list.Contains(item))
					{
						WinCommons.Error(Enums.MessageCode.ChooseFromOneBp, "");
						WinCommons._CloseProcessing();
						return;
					}
				}
				else
				{
					list.Add(item);
				}
				list2.Add(Function.ToString(gv.GetRowCellValue(selectedRows[num], "DocEntry")));
			}
			if (list2.Count > 0)
			{
				string text2 = string.Join(",", list2.ToArray());
				switch (totype)
				{
				case ApzObjectTypes.oQuotations:
				{
					frm_SalesQuotationForBatch frm_SalesQuotationForBatch2 = new frm_SalesQuotationForBatch();
					frm_SalesQuotationForBatch2.Fromtype = fromtype;
					frm_SalesQuotationForBatch2.Fromentry = text2;
					frm_SalesQuotationForBatch2.Formcopy = frmcopy;
					frm_SalesQuotationForBatch2.Cancellation = isCancel;
					frm_SalesQuotationForBatch2.MdiParent = frmcopy.MdiParent;
					frm_SalesQuotationForBatch2.Show();
					break;
				}
				case ApzObjectTypes.oOrders:
				{
					frm_SalesOrder frm_SalesOrder2 = new frm_SalesOrder();
					frm_SalesOrder2.Fromtype = fromtype;
					frm_SalesOrder2.Fromentry = text2;
					frm_SalesOrder2.Formcopy = frmcopy;
					frm_SalesOrder2.Cancellation = isCancel;
					frm_SalesOrder2.MdiParent = frmcopy.MdiParent;
					frm_SalesOrder2.Show();
					break;
				}
				case ApzObjectTypes.oDeliveryNotes:
				{
					frm_Delivery frm_Delivery2 = new frm_Delivery();
					frm_Delivery2.Fromtype = fromtype;
					frm_Delivery2.Fromentry = text2;
					frm_Delivery2.Formcopy = frmcopy;
					frm_Delivery2.Cancellation = isCancel;
					frm_Delivery2.MdiParent = frmcopy.MdiParent;
					frm_Delivery2.Show();
					break;
				}
				case ApzObjectTypes.oCreditNotes:
				{
					frm_ARCreditMemos frm_ARCreditMemos2 = new frm_ARCreditMemos();
					frm_ARCreditMemos2.Fromtype = fromtype;
					frm_ARCreditMemos2.Fromentry = text2;
					frm_ARCreditMemos2.Formcopy = frmcopy;
					frm_ARCreditMemos2.Cancellation = isCancel;
					frm_ARCreditMemos2.MdiParent = frmcopy.MdiParent;
					frm_ARCreditMemos2.Show();
					break;
				}
				case ApzObjectTypes.oReturnRequest:
				{
					frm_ReturnRequest frm_ReturnRequest2 = new frm_ReturnRequest();
					frm_ReturnRequest2.Fromtype = fromtype;
					frm_ReturnRequest2.Fromentry = text2;
					frm_ReturnRequest2.Formcopy = frmcopy;
					frm_ReturnRequest2.Cancellation = isCancel;
					frm_ReturnRequest2.MdiParent = frmcopy.MdiParent;
					frm_ReturnRequest2.Show();
					break;
				}
				case ApzObjectTypes.oReturns:
				{
					frm_Return frm_Return2 = new frm_Return();
					frm_Return2.Fromtype = fromtype;
					frm_Return2.Fromentry = text2;
					frm_Return2.Formcopy = frmcopy;
					frm_Return2.Cancellation = isCancel;
					frm_Return2.MdiParent = frmcopy.MdiParent;
					frm_Return2.Show();
					break;
				}
				case ApzObjectTypes.oInvoices:
					if (!isReserveInvoice)
					{
						frm_ARInvoices frm_ARInvoices2 = new frm_ARInvoices();
						frm_ARInvoices2.Fromtype = fromtype;
						frm_ARInvoices2.Fromentry = text2;
						frm_ARInvoices2.Formcopy = frmcopy;
						frm_ARInvoices2.Cancellation = isCancel;
						frm_ARInvoices2.MdiParent = frmcopy.MdiParent;
						frm_ARInvoices2.Show();
					}
					else
					{
						frm_AR_ReserveInvoices frm_AR_ReserveInvoices3 = new frm_AR_ReserveInvoices();
						frm_AR_ReserveInvoices3.Fromtype = fromtype;
						frm_AR_ReserveInvoices3.Fromentry = text2;
						frm_AR_ReserveInvoices3.Formcopy = frmcopy;
						frm_AR_ReserveInvoices3.Cancellation = isCancel;
						frm_AR_ReserveInvoices3.MdiParent = frmcopy.MdiParent;
						frm_AR_ReserveInvoices3.Show();
					}
					break;
				case ApzObjectTypes.oReserveInvoices:
				{
					frm_AR_ReserveInvoices frm_AR_ReserveInvoices2 = new frm_AR_ReserveInvoices();
					frm_AR_ReserveInvoices2.Fromtype = fromtype;
					frm_AR_ReserveInvoices2.Fromentry = text2;
					frm_AR_ReserveInvoices2.Formcopy = frmcopy;
					frm_AR_ReserveInvoices2.Cancellation = isCancel;
					frm_AR_ReserveInvoices2.MdiParent = frmcopy.MdiParent;
					frm_AR_ReserveInvoices2.Show();
					break;
				}
				case ApzObjectTypes.oDownPayments:
				{
					frm_DownPaymentRequest frm_DownPaymentRequest2 = new frm_DownPaymentRequest();
					frm_DownPaymentRequest2.Fromtype = fromtype;
					frm_DownPaymentRequest2.Fromentry = text2;
					frm_DownPaymentRequest2.Formcopy = frmcopy;
					frm_DownPaymentRequest2.Cancellation = isCancel;
					frm_DownPaymentRequest2.MdiParent = frmcopy.MdiParent;
					frm_DownPaymentRequest2.Show();
					break;
				}
				case ApzObjectTypes.oDownPaymentInvoice:
				{
					frm_DownPaymentInvoice frm_DownPaymentInvoice2 = new frm_DownPaymentInvoice();
					frm_DownPaymentInvoice2.Fromtype = fromtype;
					frm_DownPaymentInvoice2.Fromentry = text2;
					frm_DownPaymentInvoice2.Formcopy = frmcopy;
					frm_DownPaymentInvoice2.Cancellation = isCancel;
					frm_DownPaymentInvoice2.MdiParent = frmcopy.MdiParent;
					frm_DownPaymentInvoice2.Show();
					break;
				}
				case ApzObjectTypes.oPurchaseRequest:
				{
					frm_PurchaseRequest frm_PurchaseRequest2 = new frm_PurchaseRequest();
					frm_PurchaseRequest2.Fromtype = fromtype;
					frm_PurchaseRequest2.Fromentry = text2;
					frm_PurchaseRequest2.Formcopy = frmcopy;
					frm_PurchaseRequest2.Cancellation = isCancel;
					frm_PurchaseRequest2.MdiParent = frmcopy.MdiParent;
					frm_PurchaseRequest2.Show();
					break;
				}
				case ApzObjectTypes.oPurchaseQuotations:
				{
					frm_PurchaseQuotation frm_PurchaseQuotation2 = new frm_PurchaseQuotation();
					frm_PurchaseQuotation2.Fromtype = fromtype;
					frm_PurchaseQuotation2.Fromentry = text2;
					frm_PurchaseQuotation2.Formcopy = frmcopy;
					frm_PurchaseQuotation2.Cancellation = isCancel;
					frm_PurchaseQuotation2.MdiParent = frmcopy.MdiParent;
					frm_PurchaseQuotation2.Show();
					break;
				}
				case ApzObjectTypes.oPurchaseCreditNotes:
				{
					frm_APCreditMemos frm_APCreditMemos2 = new frm_APCreditMemos();
					frm_APCreditMemos2.Fromtype = fromtype;
					frm_APCreditMemos2.Fromentry = text2;
					frm_APCreditMemos2.Formcopy = frmcopy;
					frm_APCreditMemos2.Cancellation = isCancel;
					frm_APCreditMemos2.MdiParent = frmcopy.MdiParent;
					frm_APCreditMemos2.Show();
					break;
				}
				case ApzObjectTypes.oPurchaseOrders:
				{
					frm_PurchaseOder frm_PurchaseOder2 = new frm_PurchaseOder();
					frm_PurchaseOder2.Fromtype = fromtype;
					frm_PurchaseOder2.Fromentry = text2;
					frm_PurchaseOder2.Formcopy = frmcopy;
					frm_PurchaseOder2.Cancellation = isCancel;
					frm_PurchaseOder2.MdiParent = frmcopy.MdiParent;
					frm_PurchaseOder2.Show();
					break;
				}
				case ApzObjectTypes.oPurchaseDeliveryNotes:
				{
					frm_GoodReceiptPO frm_GoodReceiptPO2 = new frm_GoodReceiptPO();
					frm_GoodReceiptPO2.Fromtype = fromtype;
					frm_GoodReceiptPO2.Fromentry = text2;
					frm_GoodReceiptPO2.Formcopy = frmcopy;
					frm_GoodReceiptPO2.Cancellation = isCancel;
					frm_GoodReceiptPO2.MdiParent = frmcopy.MdiParent;
					frm_GoodReceiptPO2.Show();
					break;
				}
				case ApzObjectTypes.oPurchaseInvoices:
					if (!isReserveInvoice)
					{
						frm_APInvoices frm_APInvoices2 = new frm_APInvoices();
						frm_APInvoices2.Fromtype = fromtype;
						frm_APInvoices2.Fromentry = text2;
						frm_APInvoices2.Formcopy = frmcopy;
						frm_APInvoices2.Cancellation = isCancel;
						frm_APInvoices2.MdiParent = frmcopy.MdiParent;
						frm_APInvoices2.Show();
					}
					else
					{
						frm_APReserveInvoices frm_APReserveInvoices3 = new frm_APReserveInvoices();
						frm_APReserveInvoices3.Fromtype = fromtype;
						frm_APReserveInvoices3.Fromentry = text2;
						frm_APReserveInvoices3.Formcopy = frmcopy;
						frm_APReserveInvoices3.Cancellation = isCancel;
						frm_APReserveInvoices3.MdiParent = frmcopy.MdiParent;
						frm_APReserveInvoices3.Show();
					}
					break;
				case ApzObjectTypes.oPurchaseReturns:
				{
					frm_GoodsReturn frm_GoodsReturn2 = new frm_GoodsReturn();
					frm_GoodsReturn2.Fromtype = fromtype;
					frm_GoodsReturn2.Fromentry = text2;
					frm_GoodsReturn2.Formcopy = frmcopy;
					frm_GoodsReturn2.Cancellation = isCancel;
					frm_GoodsReturn2.MdiParent = frmcopy.MdiParent;
					frm_GoodsReturn2.Show();
					break;
				}
				case ApzObjectTypes.oAPReserveInvoice:
				{
					frm_APReserveInvoices frm_APReserveInvoices2 = new frm_APReserveInvoices();
					frm_APReserveInvoices2.Fromtype = fromtype;
					frm_APReserveInvoices2.Fromentry = text2;
					frm_APReserveInvoices2.Formcopy = frmcopy;
					frm_APReserveInvoices2.Cancellation = isCancel;
					frm_APReserveInvoices2.MdiParent = frmcopy.MdiParent;
					frm_APReserveInvoices2.Show();
					break;
				}
				case ApzObjectTypes.oPurchaseDownPayments:
				{
					frm_PurchaseDownPaymentRequest frm_PurchaseDownPaymentRequest2 = new frm_PurchaseDownPaymentRequest();
					frm_PurchaseDownPaymentRequest2.Fromtype = fromtype;
					frm_PurchaseDownPaymentRequest2.Fromentry = text2;
					frm_PurchaseDownPaymentRequest2.Formcopy = frmcopy;
					frm_PurchaseDownPaymentRequest2.Cancellation = isCancel;
					frm_PurchaseDownPaymentRequest2.MdiParent = frmcopy.MdiParent;
					frm_PurchaseDownPaymentRequest2.Show();
					break;
				}
				case ApzObjectTypes.oPurchaseDownPaymentInvoice:
				{
					frm_PurchaseDownPaymentInvoice frm_PurchaseDownPaymentInvoice2 = new frm_PurchaseDownPaymentInvoice();
					frm_PurchaseDownPaymentInvoice2.Fromtype = fromtype;
					frm_PurchaseDownPaymentInvoice2.Fromentry = text2;
					frm_PurchaseDownPaymentInvoice2.Formcopy = frmcopy;
					frm_PurchaseDownPaymentInvoice2.Cancellation = isCancel;
					frm_PurchaseDownPaymentInvoice2.MdiParent = frmcopy.MdiParent;
					frm_PurchaseDownPaymentInvoice2.Show();
					break;
				}
				case ApzObjectTypes.oInventoryGenEntry:
				{
					frm_GoodsReceipt frm_GoodsReceipt2 = new frm_GoodsReceipt();
					frm_GoodsReceipt2.FromType = fromtype;
					frm_GoodsReceipt2.FromEntry = Function.ParseInt(text2);
					frm_GoodsReceipt2.FormCopy = frmcopy;
					frm_GoodsReceipt2.Cancellation = isCancel;
					frm_GoodsReceipt2.MdiParent = frmcopy.MdiParent;
					frm_GoodsReceipt2.Show();
					break;
				}
				case ApzObjectTypes.oInventoryGenExit:
				{
					frm_GoodsIssue frm_GoodsIssue2 = new frm_GoodsIssue();
					frm_GoodsIssue2.FromType = fromtype;
					frm_GoodsIssue2.FromEntry = Function.ParseInt(text2);
					frm_GoodsIssue2.FormCopy = frmcopy;
					frm_GoodsIssue2.Cancellation = isCancel;
					frm_GoodsIssue2.MdiParent = frmcopy.MdiParent;
					frm_GoodsIssue2.Show();
					break;
				}
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	internal static void CopyBlanketToDocument(Enums.TypeCFL bpType, ApzObjectTypes totype, XtraFormEx frmcopy, GridViewEx gv)
	{
		WinCommons._OpenProcessing();
		try
		{
			ApzObjectTypes fromtype = ((bpType == Enums.TypeCFL.Customer) ? ApzObjectTypes.oBlanketAgreement : ApzObjectTypes.oPurchaseBlanketAgreement);
			bool cancellation = false;
			int[] selectedRows = gv.GetSelectedRows();
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			for (int i = 0; i < selectedRows.Length; i++)
			{
				string item = Function.ToString(gv.GetRowCellValue(selectedRows[i], "BpCode"));
				if (list.Count > 0)
				{
					if (!list.Contains(item))
					{
						WinCommons.Error(Enums.MessageCode.ChooseFromOneBp, "");
						WinCommons._CloseProcessing();
						return;
					}
				}
				else
				{
					list.Add(item);
				}
				list2.Add(Function.ToString(gv.GetRowCellValue(selectedRows[i], "AbsID")));
			}
			if (list2.Count > 0)
			{
				string fromentry = string.Join(",", list2.ToArray());
				switch (totype)
				{
				case ApzObjectTypes.oQuotations:
				{
					frm_SalesQuotationForBatch frm_SalesQuotationForBatch2 = new frm_SalesQuotationForBatch();
					frm_SalesQuotationForBatch2.Fromtype = fromtype;
					frm_SalesQuotationForBatch2.Fromentry = fromentry;
					frm_SalesQuotationForBatch2.Formcopy = frmcopy;
					frm_SalesQuotationForBatch2.Cancellation = cancellation;
					frm_SalesQuotationForBatch2.MdiParent = frmcopy.MdiParent;
					frm_SalesQuotationForBatch2.Show();
					break;
				}
				case ApzObjectTypes.oOrders:
				{
					frm_SalesOrder frm_SalesOrder2 = new frm_SalesOrder();
					frm_SalesOrder2.Fromtype = fromtype;
					frm_SalesOrder2.Fromentry = fromentry;
					frm_SalesOrder2.Formcopy = frmcopy;
					frm_SalesOrder2.Cancellation = cancellation;
					frm_SalesOrder2.MdiParent = frmcopy.MdiParent;
					frm_SalesOrder2.Show();
					break;
				}
				case ApzObjectTypes.oDeliveryNotes:
				{
					frm_Delivery frm_Delivery2 = new frm_Delivery();
					frm_Delivery2.Fromtype = fromtype;
					frm_Delivery2.Fromentry = fromentry;
					frm_Delivery2.Formcopy = frmcopy;
					frm_Delivery2.Cancellation = cancellation;
					frm_Delivery2.MdiParent = frmcopy.MdiParent;
					frm_Delivery2.Show();
					break;
				}
				case ApzObjectTypes.oInvoices:
				{
					frm_ARInvoices frm_ARInvoices2 = new frm_ARInvoices();
					frm_ARInvoices2.Fromtype = fromtype;
					frm_ARInvoices2.Fromentry = fromentry;
					frm_ARInvoices2.Formcopy = frmcopy;
					frm_ARInvoices2.Cancellation = cancellation;
					frm_ARInvoices2.MdiParent = frmcopy.MdiParent;
					frm_ARInvoices2.Show();
					break;
				}
				case ApzObjectTypes.oPurchaseQuotations:
				{
					frm_PurchaseQuotation frm_PurchaseQuotation2 = new frm_PurchaseQuotation();
					frm_PurchaseQuotation2.Fromtype = fromtype;
					frm_PurchaseQuotation2.Fromentry = fromentry;
					frm_PurchaseQuotation2.Formcopy = frmcopy;
					frm_PurchaseQuotation2.Cancellation = cancellation;
					frm_PurchaseQuotation2.MdiParent = frmcopy.MdiParent;
					frm_PurchaseQuotation2.Show();
					break;
				}
				case ApzObjectTypes.oPurchaseOrders:
				{
					frm_PurchaseOder frm_PurchaseOder2 = new frm_PurchaseOder();
					frm_PurchaseOder2.Fromtype = fromtype;
					frm_PurchaseOder2.Fromentry = fromentry;
					frm_PurchaseOder2.Formcopy = frmcopy;
					frm_PurchaseOder2.Cancellation = cancellation;
					frm_PurchaseOder2.MdiParent = frmcopy.MdiParent;
					frm_PurchaseOder2.Show();
					break;
				}
				case ApzObjectTypes.oPurchaseDeliveryNotes:
				{
					frm_GoodReceiptPO frm_GoodReceiptPO2 = new frm_GoodReceiptPO();
					frm_GoodReceiptPO2.Fromtype = fromtype;
					frm_GoodReceiptPO2.Fromentry = fromentry;
					frm_GoodReceiptPO2.Formcopy = frmcopy;
					frm_GoodReceiptPO2.Cancellation = cancellation;
					frm_GoodReceiptPO2.MdiParent = frmcopy.MdiParent;
					frm_GoodReceiptPO2.Show();
					break;
				}
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	internal static void LoadUDF(Form frmParent, Form frmUdf)
	{
		if (frmParent != null && frmUdf != null)
		{
			int num = frmParent.Location.X + frmParent.Size.Width;
			int y = frmParent.Location.Y;
			int height = frmParent.Height;
			int num2 = frmParent.Width + 4;
			if (num == num2)
			{
				int num3 = num - frmUdf.Size.Width - 20;
				frmParent.ClientSize = new Size(num3, height);
				frmParent.Location = new Point(0, 0);
				frmUdf.Location = new Point(num3, 0);
			}
			else
			{
				frmUdf.Location = new Point(num, y);
			}
			frmUdf.Size = new Size(230, height);
			frmUdf.StartPosition = FormStartPosition.Manual;
			frmUdf.MdiParent = UFrmMain;
			frmUdf.Show();
			frmParent.Focus();
		}
	}

	public static void ShowFormUdfMove(XtraFormEx frmParent, XtraFormEx frmUdf)
	{
		if (frmParent != null && frmUdf != null)
		{
			int num = frmParent.Location.X + frmParent.Size.Width;
			int y = frmParent.Location.Y;
			int num2 = frmParent.ClientSize.Height - 39;
			int num3 = frmParent.ClientSize.Width + 4;
			if (num == num3)
			{
				int num4 = num - frmUdf.Size.Width - 20;
				frmParent.ClientSize = new Size(num4, num2);
				frmParent.Location = new Point(0, 0);
				frmUdf.Location = new Point(num4, 0);
			}
			else
			{
				frmUdf.Location = new Point(num, y);
			}
			frmUdf.ClientSize = new Size(230, num2 + 41);
			frmParent.Focus();
		}
	}

	public static List<UDFFieldTypeCode> LoadUDFFieldType()
	{
		List<UDFFieldTypeCode> list = new List<UDFFieldTypeCode>();
		UDFFieldTypeCode item = new UDFFieldTypeCode
		{
			comboString = "String",
			dbType = "nvarchar",
			TypeID = "T",
			TypeControl = "TextEdit",
			Name = "TextEdit control"
		};
		list.Add(item);
		item.comboString = "Nummeric";
		item.dbType = "int";
		item.TypeID = "CR";
		item.TypeControl = "CalcEdit";
		item.Name = "CalcEdit control";
		list.Add(item);
		item.comboString = "Currency";
		item.dbType = "Decimal";
		item.TypeID = "CA";
		item.TypeControl = "CalcEdit";
		item.Name = "CalcEdit control";
		list.Add(item);
		item.comboString = "Datetime";
		item.dbType = "Datetime";
		item.TypeID = "D";
		item.TypeControl = "DateEdit";
		item.Name = "DateEdit control";
		list.Add(item);
		item.comboString = "Bool";
		item.dbType = "bit";
		item.TypeID = "CHK";
		item.TypeControl = "CheckEdit";
		item.Name = "CheckEdit control";
		list.Add(item);
		return list;
	}

	public static string getControlType(Control ctl)
	{
		if (ctl is TextEdit)
		{
			return "TextEdit";
		}
		if (ctl is ComboBoxEdit)
		{
			return "ComboBoxEdit";
		}
		if (ctl is CalcEdit)
		{
			return "CalcEdit";
		}
		if (ctl is DateEdit)
		{
			return "DateEdit";
		}
		if (ctl is CheckEdit)
		{
			return "CheckEdit";
		}
		if (ctl is SimpleButton)
		{
			return "SimpleButton";
		}
		if (ctl is GridControlEx)
		{
			return "GridControl";
		}
		if (ctl is CheckEdit)
		{
			return "CheckEdit";
		}
		if (ctl is RadioGroup)
		{
			return "RadioGroup";
		}
		if (ctl is ContextMenuStrip)
		{
			return "ContextMenuStrip";
		}
		return "";
	}

	public static List<Common.Table_ID_Code> getControlsValueAndType(List<Control> lstControl)
	{
		List<Common.Table_ID_Code> list = new List<Common.Table_ID_Code>();
		foreach (Control item in lstControl)
		{
			Common.Table_ID_Code table_ID_Code = new Common.Table_ID_Code();
			table_ID_Code.ID = item.Name;
			switch (getControlType(item))
			{
			case "TextEdit":
				table_ID_Code.Code = item.Text;
				table_ID_Code.Name = "TextEdit";
				break;
			case "ComboBoxEdit":
			{
				ComboBoxEdit comboBoxEdit = (ComboBoxEdit)item;
				table_ID_Code.Code = comboBoxEdit.SelectedItem.ToString();
				table_ID_Code.Name = "ComboBoxEdit";
				break;
			}
			case "CalcEdit":
				table_ID_Code.Code = item.Text;
				table_ID_Code.Name = "CalcEdit";
				break;
			case "DateEdit":
			{
				DateEdit dateEdit = (DateEdit)item;
				table_ID_Code.Code = dateEdit.DateTime.ToString();
				table_ID_Code.Name = "DateEdit";
				break;
			}
			case "CheckEdit":
			{
				CheckEdit checkEdit = (CheckEdit)item;
				table_ID_Code.Code = (checkEdit.Checked ? "1" : "0");
				table_ID_Code.Name = "CheckEdit";
				break;
			}
			}
			list.Add(table_ID_Code);
		}
		return list;
	}

	private static DataTable GetUDFFieldValueBOMItem(string Father, int ChildNum)
	{
		DataTable result = new DataTable();
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			result = httpClientHelper.Get("GetUDFFieldValueBOMItem", new { Father, ChildNum });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return result;
	}

	internal static void SetUDFFieldValueBOMItemToDoc(string Father, int ChildNum, GridViewEx gv, int rowHandle)
	{
		try
		{
			DataTable uDFFieldValueBOMItem = GetUDFFieldValueBOMItem(Father, ChildNum);
			if (uDFFieldValueBOMItem == null || uDFFieldValueBOMItem.Rows.Count == 0)
			{
				return;
			}
			for (int i = 0; i < uDFFieldValueBOMItem.Columns.Count; i++)
			{
				for (int j = 0; j < gv.Columns.Count; j++)
				{
					if (uDFFieldValueBOMItem.Columns[i].ColumnName.ToUpper() == gv.Columns[j].FieldName.ToUpper())
					{
						gv.SetRowCellValue(rowHandle, uDFFieldValueBOMItem.Columns[i].ColumnName, uDFFieldValueBOMItem.Rows[0][i]);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public static bool isValidSpecialChar(string value)
	{
		if (new Regex("[a-zA-Z\\b0-9\\s]").IsMatch(value))
		{
			return true;
		}
		return false;
	}

	public static bool isValidVNNameSpecialChar(string value)
	{
		try
		{
			for (int i = 0; i < VietNamChar.Length; i++)
			{
				for (int j = 0; j < VietNamChar[i].Length; j++)
				{
					if (value == VietNamChar[i][j].ToString())
					{
						return true;
					}
				}
			}
			return false;
		}
		catch
		{
			return false;
		}
	}

	public static bool isValidNumber(string itemcode)
	{
		if (new Regex("[^0-9\\s]").IsMatch(itemcode))
		{
			return true;
		}
		return false;
	}

	internal static bool _text(TextEdit _txt)
	{
		try
		{
			_txt.Text.Replace("'", "");
			if (_txt.Text.Trim().Length == 0)
			{
				_txt.ErrorText = "Must enter data!";
				_txt.ErrorIcon = BaseEdit.DefaultErrorIcon;
				_txt.ErrorIconAlignment = ErrorIconAlignment.MiddleRight;
				_txt.Focus();
				return false;
			}
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	internal static bool _combobox(ComboBoxEdit _cb)
	{
		try
		{
			if (_cb.SelectedIndex == -1)
			{
				_cb.ErrorText = "Phải nhập dữ liệu!";
				_cb.ErrorIcon = BaseEdit.DefaultErrorIcon;
				_cb.ErrorIconAlignment = ErrorIconAlignment.MiddleRight;
				_cb.Focus();
				return false;
			}
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	internal static bool _combobox(ComboBoxEdit _cb, string _text)
	{
		try
		{
			if (_cb.SelectedText.Equals(_text))
			{
				_cb.ErrorText = "Phải nhập dữ liệu!";
				_cb.ErrorIcon = BaseEdit.DefaultErrorIcon;
				_cb.ErrorIconAlignment = ErrorIconAlignment.MiddleRight;
				_cb.Focus();
				return false;
			}
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	internal static bool _checkedComboBoxEdit(CheckedComboBoxEdit _cb)
	{
		try
		{
			bool flag = false;
			for (int i = 0; i < _cb.Properties.Items.Count; i++)
			{
				if (_cb.Properties.Items[i].CheckState == CheckState.Checked)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				return true;
			}
			_cb.ErrorText = "Dữ liệu không hợp lệ!";
			_cb.ErrorIcon = BaseEdit.DefaultErrorIcon;
			_cb.ErrorIconAlignment = ErrorIconAlignment.MiddleRight;
			_cb.Focus();
			return false;
		}
		catch (Exception)
		{
			return false;
		}
	}

	internal static bool _calc(CalcEdit _cal)
	{
		try
		{
			if (_cal.Text.Trim().Length == 0)
			{
				_cal.ErrorText = "Dữ liệu không hợp lệ!";
				_cal.ErrorIcon = BaseEdit.DefaultErrorIcon;
				_cal.ErrorIconAlignment = ErrorIconAlignment.MiddleRight;
				_cal.Focus();
				return false;
			}
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	internal static bool _spin(SpinEdit _spin)
	{
		try
		{
			if (_spin.Text.Equals(""))
			{
				_spin.ErrorText = "Dữ liệu không hợp lệ!";
				_spin.ErrorIcon = BaseEdit.DefaultErrorIcon;
				_spin.ErrorIconAlignment = ErrorIconAlignment.MiddleRight;
				_spin.Focus();
				return false;
			}
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	internal static bool _time(TimeEdit _time)
	{
		try
		{
			if (_time.Text.Trim().Equals("00:00"))
			{
				_time.ErrorText = "Dữ liệu không hợp lệ!";
				_time.ErrorIcon = BaseEdit.DefaultErrorIcon;
				_time.ErrorIconAlignment = ErrorIconAlignment.MiddleRight;
				_time.Focus();
				return false;
			}
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	internal static bool _date(DateEdit _date)
	{
		try
		{
			if (_date.Text.Trim().Length == 0)
			{
				_date.ErrorText = "Dữ liệu không hợp lệ!";
				_date.ErrorIcon = BaseEdit.DefaultErrorIcon;
				_date.ErrorIconAlignment = ErrorIconAlignment.MiddleRight;
				_date.Focus();
				return false;
			}
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	internal static bool _email(TextEdit _txt)
	{
		try
		{
			if (_txt.Text.Trim().Length == 0)
			{
				_txt.ErrorText = "Dữ liệu không hợp lệ!";
				_txt.ErrorIcon = BaseEdit.DefaultErrorIcon;
				_txt.ErrorIconAlignment = ErrorIconAlignment.MiddleRight;
				_txt.Focus();
				return false;
			}
			if (new Regex("^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\\-+)|([A-Za-z0-9]+\\.+)|([A-Za-z0-9]+\\++))*[A-Za-z0-9]+@((\\w+\\-+)|(\\w+\\.))*\\w{1,63}\\.[a-zA-Z]{2,6}$").IsMatch(_txt.Text))
			{
				return true;
			}
			_txt.ErrorText = "Dữ liệu không hợp lệ!";
			_txt.ErrorIcon = BaseEdit.DefaultErrorIcon;
			_txt.ErrorIconAlignment = ErrorIconAlignment.MiddleRight;
			_txt.Focus();
			return false;
		}
		catch (Exception)
		{
			return false;
		}
	}

	internal static bool _phone(TextEdit _txt)
	{
		try
		{
			if (_txt.Text.Trim().Length == 0)
			{
				_txt.ErrorText = "Dữ liệu không hợp lệ!";
				_txt.ErrorIcon = BaseEdit.DefaultErrorIcon;
				_txt.ErrorIconAlignment = ErrorIconAlignment.MiddleRight;
				_txt.Focus();
				return false;
			}
			if (new Regex("(^[+][0-9]\\d{6,12}$)|(^\\d{6,12}$)").IsMatch(_txt.Text))
			{
				return true;
			}
			_txt.ErrorText = "Dữ liệu không hợp lệ!";
			_txt.ErrorIcon = BaseEdit.DefaultErrorIcon;
			_txt.ErrorIconAlignment = ErrorIconAlignment.MiddleRight;
			_txt.Focus();
			return false;
		}
		catch (Exception)
		{
			return false;
		}
	}

	internal static bool _isemail(TextEdit _txt)
	{
		try
		{
			if (_txt.Text.Trim().Length != 0)
			{
				if (new Regex("^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\\-+)|([A-Za-z0-9]+\\.+)|([A-Za-z0-9]+\\++))*[A-Za-z0-9]+@((\\w+\\-+)|(\\w+\\.))*\\w{1,63}\\.[a-zA-Z]{2,6}$").IsMatch(_txt.Text))
				{
					return true;
				}
				_txt.ErrorText = "Invalid Email Address!";
				_txt.ErrorIcon = BaseEdit.DefaultErrorIcon;
				_txt.ErrorIconAlignment = ErrorIconAlignment.MiddleRight;
				_txt.Focus();
				return false;
			}
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	internal static bool _isphone(TextEdit _txt)
	{
		try
		{
			if (_txt.Text.Trim().Length != 0)
			{
				if (new Regex("(^[+][0-9]\\d{6,12}$)|(^\\d{6,12}$)").IsMatch(_txt.Text))
				{
					return true;
				}
				_txt.ErrorText = "Dữ liệu không hợp lệ!";
				_txt.ErrorIcon = BaseEdit.DefaultErrorIcon;
				_txt.ErrorIconAlignment = ErrorIconAlignment.MiddleRight;
				_txt.Focus();
				return false;
			}
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	internal static void _onlyNumber(KeyPressEventArgs e)
	{
		if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
		{
			e.Handled = true;
		}
	}

	internal static void _addComboboxYear(ComboBoxEdit cb)
	{
		try
		{
			List<Common.Table_ID> list = new List<Common.Table_ID>();
			int num = DateTime.Now.Year + 7;
			for (int i = DateTime.Now.Year - 3; i <= num; i++)
			{
				Common.Table_ID table_ID = new Common.Table_ID();
				table_ID.ID = i;
				table_ID.Name = i.ToString();
				list.Add(table_ID);
			}
			Common.Table_ID table_ID2 = new Common.Table_ID();
			table_ID2.ID = num + 1;
			table_ID2.Name = (num + 1).ToString();
			list.Add(table_ID2);
			_addComboBox(cb, list);
			_selectItemComboBoxByValue(cb, DateTime.Now.Year.ToString());
		}
		catch (Exception)
		{
		}
	}

	public static string FormatCreatedDate(string s, bool shorten = false, string preWord = "")
	{
		try
		{
			if (!DateTime.TryParse(s, out var result))
			{
				return "";
			}
			TimeSpan timeSpan = DateTime.Now - result;
			if (timeSpan < new TimeSpan(0, 0, 10))
			{
				return shorten ? "few seconds ago" : "few seconds ago";
			}
			if (timeSpan < new TimeSpan(0, 0, 60))
			{
				return timeSpan.Seconds + " seconds ago";
			}
			if (timeSpan < new TimeSpan(0, 60, 0))
			{
				string text = ((timeSpan.Minutes > 1) ? "s" : "");
				return timeSpan.Minutes + " minute" + text + " ago";
			}
			if (timeSpan < new TimeSpan(24, 0, 0))
			{
				string text2 = ((timeSpan.Hours > 1) ? "s" : "");
				return timeSpan.Hours + " hour" + text2 + " ago";
			}
			if (timeSpan < new TimeSpan(48, 0, 0))
			{
				return "Yesterday";
			}
			if (timeSpan < new TimeSpan(168, 0, 0))
			{
				string text3 = ((timeSpan.Days > 1) ? "s" : "");
				return timeSpan.Days + " day" + text3 + " ago";
			}
			return preWord + result.ToString(GlobalData.DateTimeFormat);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return "";
	}

	public static List<Control> getControls(string what, Control where)
	{
		List<Control> list = new List<Control>();
		foreach (Control control in where.Controls)
		{
			if (control.GetType().Name == what)
			{
				list.Add(control);
			}
			else if (control.Controls.Count > 0)
			{
				list.AddRange(getControls(what, control));
			}
		}
		return list;
	}

	public static void SetFormatControlValue(Control ctl)
	{
		try
		{
			string dateTimeFormat = GlobalData.DateTimeFormat;
			List<Control> controls = getControls("GridControl", ctl);
			List<Control> controls2 = getControls("DateEdit", ctl);
			List<Control> controls3 = getControls("TextEdit", ctl);
			foreach (CalcEdit control in getControls("CalcEdit", ctl))
			{
				control.Properties.DisplayFormat.FormatType = FormatType.Numeric;
				control.Properties.EditFormat.FormatType = FormatType.Numeric;
				if (GlobalData.TotalsAccuracyFormat != null)
				{
					control.Properties.DisplayFormat.FormatString = "n" + GlobalData.TotalsAccuracyFormat.NumberDecimalDigits;
					control.Properties.EditFormat.FormatString = "n" + GlobalData.TotalsAccuracyFormat.NumberDecimalDigits;
				}
			}
			foreach (DateEdit item in controls2)
			{
				item.Properties.Mask.MaskType = MaskType.DateTime;
				item.Properties.Mask.EditMask = dateTimeFormat;
				item.Properties.Mask.UseMaskAsDisplayFormat = true;
			}
			foreach (TextEdit item2 in controls3)
			{
				_ = item2.Properties.Mask.MaskType;
				_ = 5;
			}
			foreach (GridControlEx item3 in controls)
			{
				foreach (GridColumnEx column in ((GridViewEx)item3.MainView).Columns)
				{
					if (column.ColumnEdit != null)
					{
						string editorTypeName = column.ColumnEdit.EditorTypeName;
						if (!(editorTypeName == "DateEdit"))
						{
							if (!(editorTypeName == "CalcEdit"))
							{
								continue;
							}
							if (column.FieldName.ToUpper() == "PRICE")
							{
								column.DisplayFormat.FormatType = FormatType.Numeric;
								column.DisplayFormat.Format = GlobalData.UnitPriceFormat;
								if (GlobalData.UnitPriceFormat != null)
								{
									column.DisplayFormat.FormatString = "n" + GlobalData.UnitPriceFormat.NumberDecimalDigits;
								}
							}
							else if (column.FieldName.ToUpper() == "QUANTITY")
							{
								column.DisplayFormat.FormatType = FormatType.Numeric;
								column.DisplayFormat.Format = GlobalData.QuantitiesFormat;
								if (GlobalData.QuantitiesFormat != null)
								{
									column.DisplayFormat.FormatString = "n" + GlobalData.QuantitiesFormat.NumberDecimalDigits;
								}
							}
							else if (column.FieldName.ToUpper() == "LINETOTAL" || column.FieldName.ToUpper() == "TOTALFRGN")
							{
								column.DisplayFormat.FormatType = FormatType.Numeric;
								column.DisplayFormat.Format = GlobalData.TotalsAccuracyFormat;
								if (GlobalData.TotalsAccuracyFormat != null)
								{
									column.DisplayFormat.FormatString = "n" + GlobalData.TotalsAccuracyFormat.NumberDecimalDigits;
								}
							}
							else if (column.FieldName.ToUpper() == "TOTALBEFOREDIS")
							{
								column.DisplayFormat.FormatType = FormatType.Numeric;
								column.DisplayFormat.Format = GlobalData.TotalsAccuracyFormat;
								if (GlobalData.TotalsAccuracyFormat != null)
								{
									column.DisplayFormat.FormatString = "n" + GlobalData.TotalsAccuracyFormat.NumberDecimalDigits;
								}
							}
							else if (column.FieldName.ToUpper() == "VATSUM" || column.FieldName.ToUpper() == "VATSUMFRGN" || column.FieldName.ToUpper() == "VATSUMFC" || column.FieldName.ToUpper() == "TAXTOTAL" || column.FieldName.ToUpper() == "TOTALTAX")
							{
								column.DisplayFormat.FormatType = FormatType.Numeric;
								column.DisplayFormat.Format = GlobalData.TotalsAccuracyFormat;
								if (GlobalData.TotalsAccuracyFormat != null)
								{
									column.DisplayFormat.FormatString = "n" + GlobalData.TotalsAccuracyFormat.NumberDecimalDigits;
								}
							}
							else if (column.FieldName.ToUpper() == "DOCTOTAL" || column.FieldName.ToUpper() == "DOCTOTALFC" || column.FieldName.ToUpper() == "TOTAL")
							{
								column.DisplayFormat.FormatType = FormatType.Numeric;
								column.DisplayFormat.Format = GlobalData.TotalsAccuracyFormat;
								if (GlobalData.TotalsAccuracyFormat != null)
								{
									column.DisplayFormat.FormatString = "n" + GlobalData.TotalsAccuracyFormat.NumberDecimalDigits;
								}
							}
							else if (column.FieldName.ToUpper() == "DOCRATE" || column.FieldName.ToUpper() == "RATE")
							{
								column.DisplayFormat.FormatType = FormatType.Numeric;
								column.DisplayFormat.Format = GlobalData.RateAccuracyFormat;
								if (GlobalData.RateAccuracyFormat != null)
								{
									column.DisplayFormat.FormatString = "n" + GlobalData.RateAccuracyFormat.NumberDecimalDigits;
								}
							}
							else
							{
								column.DisplayFormat.FormatType = FormatType.Numeric;
								column.DisplayFormat.Format = GlobalData.QuantitiesFormat;
								if (GlobalData.QuantitiesFormat != null)
								{
									column.DisplayFormat.FormatString = "n" + GlobalData.QuantitiesFormat.NumberDecimalDigits;
								}
							}
						}
						else
						{
							column.DisplayFormat.FormatType = FormatType.DateTime;
							column.DisplayFormat.FormatString = dateTimeFormat;
						}
					}
					else if (column.ColumnType == typeof(DateTime))
					{
						column.DisplayFormat.FormatType = FormatType.DateTime;
						column.DisplayFormat.FormatString = dateTimeFormat;
					}
					else if (column.ColumnType == typeof(decimal) || column.ColumnType == typeof(double))
					{
						column.DisplayFormat.FormatType = FormatType.Numeric;
						column.DisplayFormat.Format = GlobalData.TotalsAccuracyFormat;
						if (GlobalData.TotalsAccuracyFormat != null)
						{
							column.DisplayFormat.FormatString = "n" + GlobalData.TotalsAccuracyFormat.NumberDecimalDigits;
						}
					}
				}
				foreach (object repositoryItem in item3.RepositoryItems)
				{
					string editorTypeName = repositoryItem.GetType().Name;
					if (!(editorTypeName == "RepositoryItemDateEdit"))
					{
						if (editorTypeName == "RepositoryItemCalcEdit")
						{
							RepositoryItemCalcEdit repositoryItemCalcEdit = (RepositoryItemCalcEdit)repositoryItem;
							repositoryItemCalcEdit.DisplayFormat.FormatType = FormatType.Numeric;
							repositoryItemCalcEdit.DisplayFormat.Format = GlobalData.UnitPriceFormat;
							repositoryItemCalcEdit.DisplayFormat.Format = GlobalData.QuantitiesFormat;
							repositoryItemCalcEdit.DisplayFormat.Format = GlobalData.TotalsAccuracyFormat;
							repositoryItemCalcEdit.Mask.UseMaskAsDisplayFormat = true;
						}
					}
					else
					{
						RepositoryItemDateEdit repositoryItemDateEdit = (RepositoryItemDateEdit)repositoryItem;
						repositoryItemDateEdit.DisplayFormat.FormatType = FormatType.DateTime;
						repositoryItemDateEdit.DisplayFormat.FormatString = dateTimeFormat;
						repositoryItemDateEdit.EditMask = dateTimeFormat;
						repositoryItemDateEdit.EditFormat.FormatType = FormatType.DateTime;
						repositoryItemDateEdit.EditFormat.FormatString = dateTimeFormat;
						repositoryItemDateEdit.Mask.UseMaskAsDisplayFormat = true;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public static async Task LoadUdfColumns(GridViewEx gv, string tablename, Dictionary<string, GridViewEx> tblFormular = null, bool overrideOld = false)
	{
		try
		{
			ApzObjectTypes formObjectType = ApzObjectTypes.oNone;
			if (gv.GridControl.FindForm() is XtraFormEx xtraFormEx)
			{
				if (xtraFormEx.FormObjectType != ApzObjectTypes.oNone)
				{
					formObjectType = xtraFormEx.FormObjectType;
				}
				else if (!string.IsNullOrEmpty(xtraFormEx.FormId))
				{
					foreach (ApzObjectTypes value in Enum.GetValues(typeof(ApzObjectTypes)))
					{
						if (Function.ToString(value) == xtraFormEx.FormId)
						{
							formObjectType = value;
							break;
						}
					}
				}
			}
			List<UserFieldsImpl> lst = new List<UserFieldsImpl>();
			Task t1 = Task.Run(async () =>
			{
				using HttpClientHelper<UserFieldsImpl> helper = new HttpClientHelper<UserFieldsImpl>(Configuration.ApiServer, "UDF");
				lst = await helper.GetListAsyncByPr("GetListField", new { tablename });
			});
			DataTable dtFormula = new DataTable();
			Task t2 = Task.Run(async () =>
			{
				using HttpClientHelper<DataTable> client = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
				dtFormula = await client.GetAsync("GetFormulaSearch", new
				{
					fieldName = "",
					tblName = tablename
				});
			});
			if (gv.FirstLoadFormSetting != null)
			{
				await gv.FirstLoadFormSetting;
			}
			await Task.WhenAll(t1, t2);
			if (lst == null || lst.Count <= 0)
			{
				return;
			}
			gv.BeginUpdate();
			gv.Columns.Where((GridColumn c) => c.FieldName.StartsWith("U_") && !lst.Any((UserFieldsImpl u) => "U_" + u.Name == c.FieldName)).ToList().ForEach(gv.Columns.Remove);
			Task[] array = new Task[lst.Count];
			for (int num = 0; num < lst.Count; num++)
			{
				GridColumnEx gridColumnEx = gv.Columns["U_" + lst[num].Name] as GridColumnEx;
				if (gridColumnEx == null)
				{
					gridColumnEx = new GridColumnEx
					{
						FieldName = "U_" + lst[num].Name,
						Visible = (!gv.HasFormSetting || gv.LstOldUDF == null || !gv.LstOldUDF.Contains("U_" + lst[num].Name)),
						Caption = lst[num].Description,
						Width = 100
					};
					gv.Columns.Add(gridColumnEx);
				}
				array[num] = SetUDFColumn(gv, lst[num], tblFormular, formObjectType, dtFormula);
			}
			await Task.WhenAll(array);
			gv.EndUpdate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private static async Task SetUDFColumn(GridViewEx gv, UserFieldsImpl udf, Dictionary<string, GridViewEx> tblFormular, ApzObjectTypes formObjectType, DataTable dtFormula)
	{
		try
		{
			GridColumnEx col = gv.Columns["U_" + udf.Name] as GridColumnEx;
			col.ShowUnboundExpressionMenu = true;
			if (udf.ValidValues != null && udf.ValidValues.Count > 0)
			{
				RepositoryItemLookUpEdit repositoryItemLookUpEdit = new RepositoryItemLookUpEdit
				{
					ValueMember = "Value",
					DisplayMember = "Description",
					NullText = "",
					BestFitMode = BestFitMode.BestFitResizePopup,
					HotTrackItems = true
				};
				switch (udf.Type)
				{
				case BoFieldTypes.db_Alpha:
					repositoryItemLookUpEdit.DataSource = udf.ValidValues;
					break;
				case BoFieldTypes.db_Numeric:
					repositoryItemLookUpEdit.DataSource = udf.ValidValues.Select((ValidValuesMDImpl item) => new
					{
						Value = Function.ParseInt(item.Value),
						Description = item.Description
					});
					break;
				case BoFieldTypes.db_Float:
					repositoryItemLookUpEdit.DataSource = udf.ValidValues.Select((ValidValuesMDImpl item) => new
					{
						Value = Function.ParseDecimal(item.Value),
						Description = item.Description
					});
					break;
				default:
					repositoryItemLookUpEdit.DataSource = udf.ValidValues;
					break;
				}
				gv.GridControl.RepositoryItems.Add(repositoryItemLookUpEdit);
				col.ColumnEdit = repositoryItemLookUpEdit;
			}
			else if (!string.IsNullOrEmpty(udf.LinkedTable))
			{
				RepositoryItemLookUpEdit repositoryItemLookUpEdit2 = await CreateEditorLinkedTable(gv, col, udf.LinkedTable);
				gv.GridControl.RepositoryItems.Add(repositoryItemLookUpEdit2);
				col.ColumnEdit = repositoryItemLookUpEdit2;
			}
			RepositoryItemButtonEdit btn = null;
			ButtonPressedEventHandler value = null;
			string udoCode = "";
			string udoType = "3";
			string udoDescrip = "";
			ApzObjectTypes objType = ApzObjectTypes.oNone;
			if (!string.IsNullOrEmpty(udf.LinkedUDO))
			{
				col.UnboundType = UnboundColumnType.String;
				btn = new RepositoryItemButtonEdit();
				btn.Buttons[0].Kind = ButtonPredefines.Search;
				udoDescrip = udf.Description;
				btn.Buttons.Add(new EditorButton
				{
					Kind = ButtonPredefines.Glyph,
					Image = Resources.right,
					IsLeft = true,
					ToolTip = udoDescrip
				});
				udoCode = udf.LinkedUDO;
				udoType = await HttpClientHelperFactory.GetHttpClient().Udo.GetTypeByCode(udoCode);
				value = (object s, ButtonPressedEventArgs e) =>
				{
					try
					{
						e.Button.Enabled = false;
						int num = -1;
						string code = "";
						if (udoType.Equals("3"))
						{
							num = Function.ParseInt(gv.GetFocusedRowCellValue(col.FieldName));
						}
						else
						{
							code = Function.ToString(gv.GetFocusedRowCellValue(col.FieldName));
							num = 0;
						}
						frm_UDO frm_UDO2 = new frm_UDO(udoCode, num, code);
						frm_UDO2.Text = udoDescrip;
						frm_UDO2.dataRow = gv.GetDataRow(gv.FocusedRowHandle);
						frm_UDO2.ShowDialog();
						if (frm_UDO2.ChoosedEntry != null)
						{
							gv.SetFocusedRowCellValue(col.FieldName, frm_UDO2.ChoosedEntry);
						}
					}
					catch (Exception ex2)
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
						WinCommons.ErrorLineNumber(ex2);
					}
					finally
					{
						e.Button.Enabled = true;
					}
				};
				gv.GridControl.RepositoryItems.Add(btn);
				col.ColumnEdit = btn;
			}
			else if (!string.IsNullOrEmpty(udf.LinkedObject))
			{
				col.UnboundType = UnboundColumnType.String;
				col.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
				btn = new RepositoryItemButtonEdit();
				btn.DisplayFormat.FormatType = col.DisplayFormat.FormatType;
				btn.DisplayFormat.FormatString = col.DisplayFormat.FormatString;
				btn.EditFormat.FormatType = col.DisplayFormat.FormatType;
				btn.EditFormat.FormatString = col.DisplayFormat.FormatString;
				btn.Buttons.Clear();
				btn.Buttons.Add(new EditorButton
				{
					Kind = ButtonPredefines.Search,
					IsLeft = false,
					Tag = udf.LinkedObject
				});
				btn.Buttons.Add(new EditorButton
				{
					Kind = ButtonPredefines.Glyph,
					Image = Resources.right,
					IsLeft = true,
					ToolTip = "Linked to SystemObject " + Function.ToString((ApzObjectTypes)Function.ParseInt(udf.LinkedObject)),
					Tag = udf.LinkedObject
				});
				btn.Tag = udf.LinkedObject;
				objType = (ApzObjectTypes)Function.ParseInt(udf.LinkedObject);
				value = async (object s, ButtonPressedEventArgs e) =>
				{
					try
					{
						e.Button.Enabled = false;
						if (e.Button.IsLeft)
						{
							string text2 = Function.ToString(gv.GetFocusedRowCellValue(col.FieldName));
							if (!(text2 == ""))
							{
								((XtraFormEx)gv.GridControl.FindForm()).DrillDown(objType, objType, text2);
							}
						}
						else
						{
							Tuple<string, DataTable> tuple = await HttpClientHelperFactory.GetHttpClient().Commons.GetDataByObject(objType, GlobalData.UserName, GlobalData.CURRENTLANGUAGE);
							DataTable item = tuple.Item2;
							string item2 = tuple.Item1;
							if (item.IsNotNull() && !string.IsNullOrEmpty(item2))
							{
								System.Data.DataSet dataSource = new System.Data.DataSet
								{
									Tables = { item }
								};
								using frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.UserDefinedData, false, "", Enums.TypeCFL.UserDefinedData.ToString() + "-" + objType, Enums.ItemObjectType.NoItemType, null, "");
								frm_CFL2.DataSource = dataSource;
								frm_CFL2.ShowDialog(gv.GridControl.FindForm());
								if (frm_CFL2.ChooseItem != null)
								{
									gv.SetFocusedRowCellValue(col.FieldName, frm_CFL2.ChooseItem[item2]);
								}
								return;
							}
						}
					}
					catch (Exception ex2)
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
						WinCommons.ErrorLineNumber(ex2);
					}
					finally
					{
						e.Button.Enabled = true;
					}
				};
				gv.GridControl.RepositoryItems.Add(btn);
				col.ColumnEdit = btn;
			}
			if (btn != null)
			{
				btn.ButtonClick += value;
			}
			if (tblFormular != null)
			{
				DataRow[] drFormula = (from t in dtFormula.AsEnumerable()
					where Function.ToString(t["FieldName"]) == col.FieldName && tblFormular.Keys.Any((string k) => k == Function.ToString(t["DependTable"]))
					select t).ToArray();
				if (drFormula != null && drFormula.Length != 0)
				{
					string type = Function.ToString(drFormula[0]["FormulaType"]).ToUpper();
					switch (type)
					{
					case "CBX":
					{
						RepositoryItemLookUpEdit r = new RepositoryItemLookUpEdit();
						IHttpClient httpClient = HttpClientHelperFactory.GetHttpClient();
						string sql = Function.ToString(drFormula[0]["Formula"]);
						DataTable dataTable = (DataTable)(r.DataSource = (await httpClient.Commons.ExecuteQuery(sql)).Data);
						if (dataTable != null && dataTable.Columns.Count >= 2)
						{
							r.ValueMember = dataTable.Columns[0].ColumnName;
							r.DisplayMember = dataTable.Columns[1].ColumnName;
						}
						r.NullText = string.Empty;
						gv.GridControl.RepositoryItems.Add(r);
						col.ColumnEdit = r;
						break;
					}
					case "CFL":
					case "MSCFL":
						if (!(col.ColumnEdit is RepositoryItemButtonEdit repositoryItemButtonEdit2))
						{
							RepositoryItemButtonEdit repositoryItemButtonEdit3 = new RepositoryItemButtonEdit();
							repositoryItemButtonEdit3.Buttons[0].Kind = ButtonPredefines.Search;
							repositoryItemButtonEdit3.TextEditStyle = TextEditStyles.Standard;
							repositoryItemButtonEdit3.ButtonClick += (object s, ButtonPressedEventArgs v) =>
							{
								try
								{
									v.Button.Enabled = false;
									string text2 = Function.ToString(drFormula[0]["Formula"]);
									List<string> paramFromSqlString = Function.GetParamFromSqlString(text2);
									if (paramFromSqlString != null && paramFromSqlString.Any())
									{
										foreach (string item3 in paramFromSqlString)
										{
											if (!string.IsNullOrEmpty(item3))
											{
												string[] array = item3.Replace("@", "").Replace(" ", "").Split('.');
												string key = Function.ToString(drFormula[0]["DependTable"]);
												string fieldName = "";
												if (array.Length > 1)
												{
													key = ((!string.IsNullOrEmpty(array[0])) ? array[0] : Function.ToString(drFormula[0]["DependTable"]));
													fieldName = array[1];
												}
												else if (array.Length != 0)
												{
													fieldName = array[0];
												}
												if (tblFormular != null)
												{
													GridViewEx gridViewEx = tblFormular[key];
													int rowHandle = ((!(gridViewEx.Name == gv.Name)) ? gridViewEx.FocusedRowHandle : gv.FocusedRowHandle);
													object rowCellValue = gridViewEx.GetRowCellValue(rowHandle, fieldName);
													string text3 = "";
													if (rowCellValue == null || rowCellValue == DBNull.Value)
													{
														text3 = "";
													}
													text3 = ((!(rowCellValue is DateTime)) ? Function.ToString(rowCellValue) : Function.ParseDateTimes(rowCellValue).ToString("yyyy-MM-dd"));
													text2 = text2.Replace(item3, string.IsNullOrEmpty(text3) ? "NULL" : $"'{text3}'");
												}
											}
										}
									}
									frm_ChooseFromList frmCfl = new frm_ChooseFromList(Enums.TypeCFL.FormattedSearch, type == "MSCFL", "", "", Enums.ItemObjectType.NoItemType, gv.FormId + gv.ItemId + col.FieldName);
									frmCfl.FormattedSearch = text2;
									if (type == "MSCFL")
									{
										frm_ChooseFromList frm_ChooseFromList2 = frmCfl;
										frm_ChooseFromList2.AfterLoadForm = (Action)Delegate.Combine(frm_ChooseFromList2.AfterLoadForm, (Action)(() =>
										{
											string[] array2 = Function.ToString(gv.GetFocusedRowCellValue(col)).Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
											GridColumn column = new GridColumn();
											foreach (GridColumn column2 in frmCfl.gv_CFL.Columns)
											{
												if (column2.FieldName == frmCfl.grd_CFL.DataSourceTable.Columns[0].ColumnName)
												{
													column = column2;
													break;
												}
											}
											frmCfl.gv_CFL.ClearSelection();
											string[] array3 = array2;
											foreach (string text4 in array3)
											{
												int rowHandle2 = frmCfl.gv_CFL.LocateByDisplayText(0, column, text4.Trim());
												frmCfl.gv_CFL.SelectRow(rowHandle2);
											}
										}));
									}
									frmCfl.ShowDialog(gv.GridControl.FindForm());
									if (type == "CFL" && frmCfl.ChooseItem != null)
									{
										gv.SetFocusedRowCellValue(col.FieldName, frmCfl.ChooseItem.Row[0]);
									}
									else if (type == "MSCFL" && frmCfl.ChooseItems.Count != 0)
									{
										gv.SetFocusedRowCellValue(col, string.Join(",", frmCfl.ChooseItems.Select((dynamic i) => Function.ToString(i.Row[0]))));
									}
								}
								catch (Exception ex2)
								{
									Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
								}
								finally
								{
									v.Button.Enabled = true;
								}
							};
							gv.GridControl.RepositoryItems.Add(repositoryItemButtonEdit3);
							col.ColumnEdit = repositoryItemButtonEdit3;
							break;
						}
						repositoryItemButtonEdit2.ButtonClick -= value;
						if (!string.IsNullOrEmpty(udf.LinkedUDO))
						{
							repositoryItemButtonEdit2.ButtonClick += (object s, ButtonPressedEventArgs e) =>
							{
								try
								{
									e.Button.Enabled = false;
									if (e.Button.IsLeft)
									{
										int num = -1;
										string code = "";
										if (udoType.Equals("3"))
										{
											num = Function.ParseInt(gv.GetFocusedRowCellValue(col.FieldName));
										}
										else
										{
											code = Function.ToString(gv.GetFocusedRowCellValue(col.FieldName));
											num = 0;
										}
										frm_UDO frm_UDO2 = new frm_UDO(udoCode, num, code)
										{
											Text = udoDescrip,
											dataRow = gv.GetDataRow(gv.FocusedRowHandle)
										};
										frm_UDO2.ShowDialog();
										if (frm_UDO2.ChoosedEntry != null)
										{
											gv.SetFocusedRowCellValue(col.FieldName, frm_UDO2.ChoosedEntry);
										}
									}
									else
									{
										string text2 = Function.ToString(drFormula[0]["Formula"]);
										List<string> paramFromSqlString = Function.GetParamFromSqlString(text2);
										if (paramFromSqlString != null && paramFromSqlString.Any())
										{
											foreach (string item4 in paramFromSqlString)
											{
												if (!string.IsNullOrEmpty(item4))
												{
													string[] array = item4.Replace("@", "").Replace(" ", "").Split('.');
													string key = Function.ToString(drFormula[0]["DependTable"]);
													string fieldName = "";
													if (array.Length > 1)
													{
														key = ((!string.IsNullOrEmpty(array[0])) ? array[0] : Function.ToString(drFormula[0]["DependTable"]));
														fieldName = array[1];
													}
													else if (array.Length != 0)
													{
														fieldName = array[0];
													}
													if (tblFormular != null)
													{
														GridViewEx gridViewEx = tblFormular[key];
														int rowHandle = ((!(gridViewEx.Name == gv.Name)) ? gridViewEx.FocusedRowHandle : gv.FocusedRowHandle);
														object rowCellValue = gridViewEx.GetRowCellValue(rowHandle, fieldName);
														string text3 = "";
														if (rowCellValue == null || rowCellValue == DBNull.Value)
														{
															text3 = "";
														}
														text3 = ((!(rowCellValue is DateTime)) ? Function.ToString(rowCellValue) : Function.ParseDateTimes(rowCellValue).ToString("yyyy-MM-dd"));
														text2 = text2.Replace(item4, string.IsNullOrEmpty(text3) ? "NULL" : $"'{text3}'");
													}
												}
											}
										}
										frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.FormattedSearch, type == "MSCFL", "", "", Enums.ItemObjectType.NoItemType, gv.FormId + gv.ItemId + col.FieldName);
										frm_ChooseFromList2.FormattedSearch = text2;
										frm_ChooseFromList2.ShowDialog(gv.GridControl.FindForm());
										if (frm_ChooseFromList2.ChooseItem != null)
										{
											gv.SetFocusedRowCellValue(col.FieldName, frm_ChooseFromList2.ChooseItem.Row[0]);
										}
									}
								}
								catch (Exception ex2)
								{
									Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
								}
								finally
								{
									e.Button.Enabled = true;
								}
							};
							break;
						}
						repositoryItemButtonEdit2.ButtonClick += (object s, ButtonPressedEventArgs e) =>
						{
							try
							{
								e.Button.Enabled = false;
								if (e.Button.IsLeft)
								{
									string id = Function.ToString(gv.GetFocusedRowCellValue(col.FieldName));
									((XtraFormEx)gv.GridControl.FindForm()).DrillDown(objType, objType, id);
									return;
								}
								string text2 = Function.ToString(drFormula[0]["Formula"]);
								List<string> paramFromSqlString = Function.GetParamFromSqlString(text2);
								if (paramFromSqlString != null && paramFromSqlString.Any())
								{
									foreach (string item5 in paramFromSqlString)
									{
										if (!string.IsNullOrEmpty(item5))
										{
											string[] array = item5.Replace("@", "").Replace(" ", "").Split('.');
											string key = Function.ToString(drFormula[0]["DependTable"]);
											string fieldName = "";
											if (array.Length > 1)
											{
												key = ((!string.IsNullOrEmpty(array[0])) ? array[0] : Function.ToString(drFormula[0]["DependTable"]));
												fieldName = array[1];
											}
											else if (array.Length != 0)
											{
												fieldName = array[0];
											}
											if (tblFormular != null)
											{
												GridViewEx gridViewEx = tblFormular[key];
												int rowHandle = ((!(gridViewEx.Name == gv.Name)) ? gridViewEx.FocusedRowHandle : gv.FocusedRowHandle);
												object rowCellValue = gridViewEx.GetRowCellValue(rowHandle, fieldName);
												string text3 = "";
												if (rowCellValue == null || rowCellValue == DBNull.Value)
												{
													text3 = "";
												}
												text3 = ((!(rowCellValue is DateTime)) ? Function.ToString(rowCellValue) : Function.ParseDateTimes(rowCellValue).ToString("yyyy-MM-dd"));
												text2 = text2.Replace(item5, string.IsNullOrEmpty(text3) ? "NULL" : $"'{text3}'");
											}
										}
									}
								}
								using frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.FormattedSearch, type == "MSCFL", "", "", Enums.ItemObjectType.NoItemType, gv.FormId + gv.ItemId + col.FieldName);
								frm_ChooseFromList2.FormattedSearch = text2;
								frm_ChooseFromList2.ShowDialog(gv.GridControl.FindForm());
								if (frm_ChooseFromList2.ChooseItem != null)
								{
									gv.SetFocusedRowCellValue(col.FieldName, frm_ChooseFromList2.ChooseItem.Row[0]);
								}
							}
							catch (Exception ex2)
							{
								Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
							}
							finally
							{
								e.Button.Enabled = true;
							}
						};
						break;
					case "LKO":
					{
						int obj = Function.ParseInt(drFormula[0]["DependTable"]);
						col.UnboundType = UnboundColumnType.String;
						col.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
						RepositoryItemButtonEdit repositoryItemButtonEdit = new RepositoryItemButtonEdit();
						repositoryItemButtonEdit.DisplayFormat.FormatType = col.DisplayFormat.FormatType;
						repositoryItemButtonEdit.DisplayFormat.FormatString = col.DisplayFormat.FormatString;
						repositoryItemButtonEdit.EditFormat.FormatType = col.DisplayFormat.FormatType;
						repositoryItemButtonEdit.EditFormat.FormatString = col.DisplayFormat.FormatString;
						repositoryItemButtonEdit.Buttons.Clear();
						repositoryItemButtonEdit.Buttons.Add(new EditorButton
						{
							Kind = ButtonPredefines.Glyph,
							Image = Resources.right,
							IsLeft = true,
							ToolTip = "Linked to System Object " + Function.ToString((ApzObjectTypes)obj),
							Tag = obj
						});
						repositoryItemButtonEdit.Tag = obj;
						repositoryItemButtonEdit.ButtonClick += (object s, ButtonPressedEventArgs e) =>
						{
							try
							{
								string text2 = Function.ToString(gv.GetFocusedRowCellValue(col.FieldName));
								if (!string.IsNullOrEmpty(text2))
								{
									((XtraFormEx)gv.GridControl.FindForm()).DrillDown((ApzObjectTypes)obj, (ApzObjectTypes)obj, text2);
								}
							}
							catch (Exception ex2)
							{
								Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
								WinCommons.ErrorLineNumber(ex2);
							}
						};
						col.ColumnEdit = repositoryItemButtonEdit;
						break;
					}
					case "LKU":
					{
						string udo = Function.ToString(drFormula[0]["DependTable"]);
						col.UnboundType = UnboundColumnType.Integer;
						RepositoryItemButtonEdit btnLku = new RepositoryItemButtonEdit();
						btnLku.DisplayFormat.FormatType = FormatType.Numeric;
						btnLku.DisplayFormat.FormatString = "n0";
						btnLku.EditFormat.FormatType = FormatType.Numeric;
						btnLku.EditFormat.FormatString = "n0";
						btnLku.Buttons[0].Kind = ButtonPredefines.Search;
						btnLku.Buttons.Add(new EditorButton
						{
							Kind = ButtonPredefines.Glyph,
							Image = Resources.right,
							IsLeft = true,
							ToolTip = "Linked to UDO " + udo,
							Tag = udo
						});
						btnLku.Tag = udo;
						string text = await HttpClientHelperFactory.GetHttpClient().Udo.GetTypeByCode(udo);
						udoType = text;
						btnLku.ButtonClick += (object s, ButtonPressedEventArgs e) =>
						{
							try
							{
								int docEntry = -1;
								string code = "";
								if (udoType.Equals("3"))
								{
									docEntry = Function.ParseInt(gv.GetFocusedRowCellValue(col.FieldName));
								}
								else
								{
									code = Function.ToString(gv.GetFocusedRowCellValue(col.FieldName));
								}
								frm_UDO frm_UDO2 = new frm_UDO(udo, docEntry, code);
								frm_UDO2.dataRow = gv.GetDataRow(gv.FocusedRowHandle);
								frm_UDO2.ShowDialog();
								if (frm_UDO2.ChoosedEntry != null)
								{
									gv.SetFocusedRowCellValue(col.FieldName, frm_UDO2.ChoosedEntry);
								}
							}
							catch (Exception ex2)
							{
								Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
								WinCommons.ErrorLineNumber(ex2);
							}
						};
						col.ColumnEdit = btnLku;
						break;
					}
					}
				}
			}
			if (col.ColumnEdit != null)
			{
				return;
			}
			switch (udf.Type)
			{
			case BoFieldTypes.db_Alpha:
				col.UnboundType = UnboundColumnType.String;
				break;
			case BoFieldTypes.db_Date:
				col.UnboundType = UnboundColumnType.String;
				if (udf.SubType == BoFldSubTypes.st_Time)
				{
					col.DisplayFormatType = Enums.DisplayFormatType.Time;
					RepositoryItemTimeEdit repositoryItemTimeEdit = new RepositoryItemTimeEdit();
					gv.GridControl.RepositoryItems.Add(repositoryItemTimeEdit);
					col.ColumnEdit = repositoryItemTimeEdit;
				}
				else
				{
					RepositoryItemDateEdit repositoryItemDateEdit = new RepositoryItemDateEdit();
					col.DisplayFormatType = Enums.DisplayFormatType.DateTime;
					repositoryItemDateEdit.DisplayFormat.FormatType = FormatType.DateTime;
					repositoryItemDateEdit.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
					gv.GridControl.RepositoryItems.Add(repositoryItemDateEdit);
					col.ColumnEdit = repositoryItemDateEdit;
				}
				break;
			case BoFieldTypes.db_Float:
			{
				col.UnboundType = UnboundColumnType.Decimal;
				if (udf.SubType == BoFldSubTypes.st_Percentage)
				{
					col.DisplayFormatType = Enums.DisplayFormatType.Percent;
				}
				else if (udf.SubType == BoFldSubTypes.st_Price)
				{
					col.DisplayFormatType = Enums.DisplayFormatType.Price;
				}
				else if (udf.SubType == BoFldSubTypes.st_Rate)
				{
					col.DisplayFormatType = Enums.DisplayFormatType.Rate;
				}
				else if (udf.SubType == BoFldSubTypes.st_Sum)
				{
					col.DisplayFormatType = Enums.DisplayFormatType.Amount;
				}
				else
				{
					col.DisplayFormatType = Enums.DisplayFormatType.Qty;
				}
				RepositoryItemCalcEdit repositoryItemCalcEdit2 = new RepositoryItemCalcEdit();
				repositoryItemCalcEdit2.DisplayFormat.FormatType = FormatType.Numeric;
				repositoryItemCalcEdit2.AllowMouseWheel = false;
				gv.GridControl.RepositoryItems.Add(repositoryItemCalcEdit2);
				col.ColumnEdit = repositoryItemCalcEdit2;
				break;
			}
			case BoFieldTypes.db_Memo:
			{
				col.UnboundType = UnboundColumnType.String;
				if (udf.SubType == BoFldSubTypes.st_Link)
				{
					col.UnboundType = UnboundColumnType.String;
					RepositoryItemAttachFile repositoryItemAttachFile = new RepositoryItemAttachFile();
					repositoryItemAttachFile.ObjType = formObjectType;
					gv.GridControl.RepositoryItems.Add(repositoryItemAttachFile);
					col.ColumnEdit = repositoryItemAttachFile;
					break;
				}
				RepositoryItemMemoExEdit repositoryItemMemoExEdit = new RepositoryItemMemoExEdit();
				repositoryItemMemoExEdit.NullText = "";
				repositoryItemMemoExEdit.ShowIcon = false;
				repositoryItemMemoExEdit.BestFitWidth = col.Width + 50;
				repositoryItemMemoExEdit.WordWrap = true;
				repositoryItemMemoExEdit.AcceptsReturn = true;
				repositoryItemMemoExEdit.ParseEditValue += Memo_ParseEditValue;
				repositoryItemMemoExEdit.FormatEditValue += Memo_FormatEditValue;
				repositoryItemMemoExEdit.Popup += Memo_Popup;
				gv.GridControl.RepositoryItems.Add(repositoryItemMemoExEdit);
				col.ColumnEdit = repositoryItemMemoExEdit;
				break;
			}
			case BoFieldTypes.db_Numeric:
			{
				if (udf.SubType == BoFldSubTypes.st_Time)
				{
					col.UnboundType = UnboundColumnType.String;
					col.DisplayFormatType = Enums.DisplayFormatType.Time;
					RepositoryItemTextEdit repositoryItemTextEdit = new RepositoryItemTextEdit();
					repositoryItemTextEdit.DisplayFormat.FormatType = FormatType.Custom;
					repositoryItemTextEdit.DisplayFormat.FormatString = "##:##";
					repositoryItemTextEdit.EditFormat.FormatType = FormatType.Custom;
					repositoryItemTextEdit.EditFormat.FormatString = "##:##";
					repositoryItemTextEdit.Mask.EditMask = "(0?\\d|1\\d|2[0-3])[0-5]\\d";
					repositoryItemTextEdit.Mask.MaskType = MaskType.RegEx;
					gv.GridControl.RepositoryItems.Add(repositoryItemTextEdit);
					col.ColumnEdit = repositoryItemTextEdit;
					break;
				}
				if (udf.SubType == BoFldSubTypes.st_Percentage)
				{
					col.DisplayFormatType = Enums.DisplayFormatType.Percent;
				}
				else if (udf.SubType == BoFldSubTypes.st_Price)
				{
					col.DisplayFormatType = Enums.DisplayFormatType.Price;
				}
				else if (udf.SubType == BoFldSubTypes.st_Rate)
				{
					col.DisplayFormatType = Enums.DisplayFormatType.Rate;
				}
				else
				{
					col.DisplayFormatType = Enums.DisplayFormatType.Qty;
				}
				col.UnboundType = UnboundColumnType.Decimal;
				RepositoryItemCalcEdit repositoryItemCalcEdit = new RepositoryItemCalcEdit();
				col.DisplayFormat.FormatType = FormatType.Numeric;
				col.DisplayFormat.Format = GlobalData.UnitPriceFormat;
				gv.GridControl.RepositoryItems.Add(repositoryItemCalcEdit);
				col.ColumnEdit = repositoryItemCalcEdit;
				break;
			}
			}
		}
		catch (Exception)
		{
		}
	}

	private static async Task<List<Common.TableIdCode>> GetDataSourceLinkedTable(string tblName, string guid)
	{
		List<Common.Table_Code> list = await HttpClientHelperFactory.GetHttpClient().Udf.GetLinkedTable(tblName, GlobalData.UserName, GlobalData.DefaultBranch);
		list.Insert(0, new Common.Table_Code("", ""));
		List<Common.TableIdCode> list2 = list.Select((Common.Table_Code i) => new Common.TableIdCode(i.Code, i.Code, i.Name)).ToList();
		list2.Add(new Common.TableIdCode(guid, "Define New", "Define New"));
		return list2;
	}

	public static async Task<RepositoryItemLookUpEdit> CreateEditorLinkedTable(GridViewEx gv, GridColumnEx col, string tblName)
	{
		string guid = Guid.NewGuid().ToString();
		RepositoryItemLookUpEdit repositoryItemLookUpEdit = new RepositoryItemLookUpEdit();
		RepositoryItemLookUpEdit repositoryItemLookUpEdit2 = repositoryItemLookUpEdit;
		repositoryItemLookUpEdit2.DataSource = await GetDataSourceLinkedTable(tblName, guid);
		repositoryItemLookUpEdit.ValueMember = "Id";
		repositoryItemLookUpEdit.DisplayMember = "Name";
		repositoryItemLookUpEdit.NullText = "";
		repositoryItemLookUpEdit.BestFitMode = BestFitMode.BestFitResizePopup;
		repositoryItemLookUpEdit.HotTrackItems = true;
		RepositoryItemLookUpEdit rep = repositoryItemLookUpEdit;
		rep.PopulateColumns();
		rep.Columns["Id"].Visible = false;
		rep.CustomDrawCell += (object s, LookUpCustomDrawCellArgs e) =>
		{
			if (Function.ToString(e.RowKey) == guid)
			{
				e.Appearance.FontStyleDelta = FontStyle.Bold;
			}
		};
		rep.KeyDown += (object s, KeyEventArgs e) =>
		{
			if (e.KeyCode == Keys.D && e.Modifiers == Keys.Control)
			{
				OpenUDTForm();
			}
		};
		rep.EditValueChanging += (object s, ChangingEventArgs e) =>
		{
			if (Function.ToString(e.NewValue) == guid)
			{
				e.Cancel = true;
				OpenUDTForm();
			}
		};
		return rep;
		void OpenUDTForm()
		{
			try
			{
				frm_SetUpUDT frm_SetUpUDT2 = new frm_SetUpUDT(tblName, defineNew: true);
				frm_SetUpUDT2.RefreshLookupEdit = async (string code) =>
				{
					List<Common.TableIdCode> list = await GetDataSourceLinkedTable(tblName, guid);
					rep.DataSource = list;
					if (!string.IsNullOrEmpty(code))
					{
						gv.SetFocusedRowCellValue(col, code);
					}
					else
					{
						string oldCode = Function.ToString(gv.GetFocusedRowCellValue(col));
						if (!list.Any((Common.TableIdCode i) => i.Id == oldCode))
						{
							gv.SetFocusedRowCellValue(col, "");
						}
					}
				};
				frm_SetUpUDT2.ShowDialog(gv.GridControl.FindForm());
			}
			catch (Exception ex)
			{
				WinCommons.ErrorLineNumber(ex);
			}
		}
	}

	public static async Task<RepositoryItemLookUpEdit> CreateEditorLinkedTable(TreeEx tv, TreeListColumnEx col, string tblName)
	{
		string guid = Guid.NewGuid().ToString();
		RepositoryItemLookUpEdit repositoryItemLookUpEdit = new RepositoryItemLookUpEdit();
		RepositoryItemLookUpEdit repositoryItemLookUpEdit2 = repositoryItemLookUpEdit;
		repositoryItemLookUpEdit2.DataSource = await GetDataSourceLinkedTable(tblName, guid);
		repositoryItemLookUpEdit.ValueMember = "Id";
		repositoryItemLookUpEdit.DisplayMember = "Name";
		repositoryItemLookUpEdit.NullText = "";
		repositoryItemLookUpEdit.BestFitMode = BestFitMode.BestFitResizePopup;
		repositoryItemLookUpEdit.HotTrackItems = true;
		RepositoryItemLookUpEdit rep = repositoryItemLookUpEdit;
		rep.PopulateColumns();
		rep.Columns["Id"].Visible = false;
		rep.CustomDrawCell += (object s, LookUpCustomDrawCellArgs e) =>
		{
			if (Function.ToString(e.RowKey) == guid)
			{
				e.Appearance.FontStyleDelta = FontStyle.Bold;
			}
		};
		rep.KeyDown += (object s, KeyEventArgs e) =>
		{
			if (e.KeyCode == Keys.D && e.Modifiers == Keys.Control)
			{
				OpenUDTForm();
			}
		};
		rep.EditValueChanging += (object s, ChangingEventArgs e) =>
		{
			if (Function.ToString(e.NewValue) == guid)
			{
				e.Cancel = true;
				OpenUDTForm();
			}
		};
		return rep;
		void OpenUDTForm()
		{
			try
			{
				frm_SetUpUDT frm_SetUpUDT2 = new frm_SetUpUDT(tblName, defineNew: true);
				frm_SetUpUDT2.RefreshLookupEdit = async (string code) =>
				{
					List<Common.TableIdCode> list = await GetDataSourceLinkedTable(tblName, guid);
					rep.DataSource = list;
					if (!string.IsNullOrEmpty(code))
					{
						tv.SetFocusedRowCellValue(col, code);
					}
					else
					{
						string oldCode = Function.ToString(tv.GetFocusedRowCellValue(col));
						if (!list.Any((Common.TableIdCode i) => i.Id == oldCode))
						{
							tv.SetFocusedRowCellValue(col, "");
						}
					}
				};
				frm_SetUpUDT2.ShowDialog(tv.FindForm());
			}
			catch (Exception ex)
			{
				WinCommons.ErrorLineNumber(ex);
			}
		}
	}

	public static async Task<LookUpEditEx> CreateEditorLinkedTable(string tblName)
	{
		string guid = Guid.NewGuid().ToString();
		LookUpEditEx lookUpEditEx = new LookUpEditEx();
		RepositoryItemLookUpEdit properties = lookUpEditEx.Properties;
		properties.DataSource = await GetDataSourceLinkedTable(tblName, guid);
		lookUpEditEx.Properties.ValueMember = "Id";
		lookUpEditEx.Properties.DisplayMember = "Name";
		lookUpEditEx.Properties.NullText = "";
		lookUpEditEx.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
		lookUpEditEx.Properties.HotTrackItems = true;
		LookUpEditEx rep = lookUpEditEx;
		rep.Properties.PopulateColumns();
		rep.Properties.Columns["Id"].Visible = false;
		rep.CustomDrawCell += (object s, LookUpCustomDrawCellArgs e) =>
		{
			if (Function.ToString(e.RowKey) == guid)
			{
				e.Appearance.FontStyleDelta = FontStyle.Bold;
			}
		};
		rep.KeyDown += (object s, KeyEventArgs e) =>
		{
			if (e.KeyCode == Keys.D && e.Modifiers == Keys.Control)
			{
				OpenUDTForm();
			}
		};
		rep.EditValueChanging += (object s, ChangingEventArgs e) =>
		{
			if (Function.ToString(e.NewValue) == guid)
			{
				e.Cancel = true;
				OpenUDTForm();
			}
		};
		return rep;
		void OpenUDTForm()
		{
			try
			{
				frm_SetUpUDT frm_SetUpUDT2 = new frm_SetUpUDT(tblName, defineNew: true);
				frm_SetUpUDT2.RefreshLookupEdit = async (string code) =>
				{
					List<Common.TableIdCode> list = await GetDataSourceLinkedTable(tblName, guid);
					rep.Properties.DataSource = list;
					if (!string.IsNullOrEmpty(code))
					{
						rep.EditValue = code;
					}
					else
					{
						string oldCode = Function.ToString(rep.EditValue);
						if (!list.Any((Common.TableIdCode i) => i.Id == oldCode))
						{
							rep.EditValue = "";
						}
					}
				};
				frm_SetUpUDT2.ShowDialog(rep.FindForm());
			}
			catch (Exception ex)
			{
				WinCommons.ErrorLineNumber(ex);
			}
		}
	}

	public static async Task<BaseEdit> GetUdfEditor(UserFieldsImpl field)
	{
		if (!string.IsNullOrEmpty(field.LinkedUDO))
		{
			ButtonEdit btn = new ButtonEdit();
			btn.Properties.Buttons[0].Kind = ButtonPredefines.Search;
			string udoDescrip = field.Description;
			btn.Properties.Buttons.Add(new EditorButton
			{
				Kind = ButtonPredefines.Glyph,
				Image = Resources.right,
				IsLeft = true,
				ToolTip = udoDescrip
			});
			string udoCode = field.LinkedUDO;
			string udoType = await HttpClientHelperFactory.GetHttpClient().Udo.GetTypeByCode(udoCode);
			btn.ButtonClick += (object s, ButtonPressedEventArgs e) =>
			{
				try
				{
					int num = -1;
					string code = "";
					if (udoType.Equals("3"))
					{
						num = Function.ParseInt(btn.EditValue);
					}
					else
					{
						code = Function.ToString(btn.EditValue);
						num = 0;
					}
					frm_UDO frm_UDO2 = new frm_UDO(udoCode, num, code);
					frm_UDO2.Text = udoDescrip;
					frm_UDO2.ShowDialog();
					if (frm_UDO2.ChoosedEntry != null)
					{
						btn.EditValue = frm_UDO2.ChoosedEntry;
					}
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					WinCommons.ErrorLineNumber(ex);
				}
			};
			return btn;
		}
		if (!string.IsNullOrEmpty(field.LinkedObject))
		{
			ButtonEdit btn2 = new ButtonEdit();
			btn2.Properties.Buttons.Clear();
			btn2.Properties.Buttons.Add(new EditorButton
			{
				Kind = ButtonPredefines.Glyph,
				Image = Resources.right,
				IsLeft = true,
				ToolTip = "Linked to SystemObject " + Function.ToString((ApzObjectTypes)Function.ParseInt(field.LinkedObject)),
				Tag = field.LinkedObject
			});
			btn2.Tag = field.LinkedObject;
			btn2.ButtonClick += (object s, ButtonPressedEventArgs e) =>
			{
				try
				{
					int num = Function.ParseInt(btn2.Tag);
					string id = Function.ToString(btn2.EditValue);
					((XtraFormEx)btn2.FindForm()).DrillDown((ApzObjectTypes)num, (ApzObjectTypes)num, id);
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					WinCommons.ErrorLineNumber(ex);
				}
			};
			return btn2;
		}
		switch (field.Type)
		{
		case BoFieldTypes.db_Alpha:
			if (!string.IsNullOrEmpty(field.LinkedTable))
			{
				return await CreateEditorLinkedTable(field.LinkedTable);
			}
			break;
		case BoFieldTypes.db_Date:
		{
			if (field.SubType == BoFldSubTypes.st_Time)
			{
				return new TimeEditEx();
			}
			DateEditEx dateEditEx = new DateEditEx();
			dateEditEx.Properties.DisplayFormat.FormatType = FormatType.DateTime;
			dateEditEx.Properties.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
			return dateEditEx;
		}
		case BoFieldTypes.db_Float:
		{
			CalcEditEx calcEditEx2 = new CalcEditEx();
			calcEditEx2.Properties.DisplayFormatType = Enums.DisplayFormatType.Qty;
			calcEditEx2.Properties.DisplayFormat.FormatType = FormatType.Numeric;
			calcEditEx2.Properties.DisplayFormat.Format = GlobalData.UnitPriceFormat;
			return calcEditEx2;
		}
		case BoFieldTypes.db_Memo:
		{
			MemoExEditEx memoExEditEx = new MemoExEditEx();
			memoExEditEx.Properties.NullText = "";
			memoExEditEx.Properties.ShowIcon = false;
			memoExEditEx.Properties.WordWrap = false;
			memoExEditEx.Properties.AcceptsReturn = true;
			memoExEditEx.ParseEditValue += Memo_ParseEditValue;
			memoExEditEx.FormatEditValue += Memo_FormatEditValue;
			memoExEditEx.Popup += Memo_Popup;
			return memoExEditEx;
		}
		case BoFieldTypes.db_Numeric:
		{
			if (field.SubType == BoFldSubTypes.st_Time)
			{
				TextEditEx textEditEx = new TextEditEx();
				textEditEx.Properties.DisplayFormat.FormatType = FormatType.Custom;
				textEditEx.Properties.DisplayFormat.FormatString = "##:##";
				textEditEx.Properties.EditFormat.FormatType = FormatType.Custom;
				textEditEx.Properties.EditFormat.FormatString = "##:##";
				textEditEx.Properties.Mask.EditMask = "(0?\\d|1\\d|2[0-3])[0-5]\\d";
				textEditEx.Properties.Mask.MaskType = MaskType.RegEx;
				return textEditEx;
			}
			CalcEditEx calcEditEx = new CalcEditEx();
			calcEditEx.Properties.DisplayFormat.FormatType = FormatType.Numeric;
			calcEditEx.Properties.DisplayFormat.Format = GlobalData.UnitPriceFormat;
			return calcEditEx;
		}
		}
		DataTable dt;
		using (HttpClientHelper<DataTable> client = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons"))
		{
			dt = await client.GetAsync("GetFormulaSearch", new
			{
				fieldName = field.Name,
				tblName = field.TableName
			});
		}
		if (dt != null && dt.Rows.Count > 0)
		{
			string text = Function.ToString(dt.Rows[0]["FormulaType"]).ToUpper();
			if (text == "CBX")
			{
				LookUpEditEx r = new LookUpEditEx();
				IHttpClient httpClient = HttpClientHelperFactory.GetHttpClient();
				string sql = Function.ToString(dt.Rows[0]["Formula"]);
				DataTable data = (await httpClient.Commons.ExecuteQuery(sql)).Data;
				r.Properties.DataSource = data;
				if (data != null && data.Columns.Count >= 2)
				{
					r.Properties.ValueMember = data.Columns[0].ColumnName;
					r.Properties.DisplayMember = data.Columns[1].ColumnName;
				}
				r.Properties.NullText = string.Empty;
				return r;
			}
			if (text == "CFL")
			{
				ButtonEditEx btnSearch = new ButtonEditEx();
				btnSearch.Properties.Buttons[0].Kind = ButtonPredefines.Search;
				btnSearch.Properties.TextEditStyle = TextEditStyles.Standard;
				btnSearch.ButtonClick += (object s, ButtonPressedEventArgs v) =>
				{
					try
					{
						frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.FormattedSearch, multiSelect: false, "", "", Enums.ItemObjectType.NoItemType, field.TableName + field.Name);
						string text2 = Function.ToString(dt.Rows[0]["Formula"]);
						List<string> paramFromSqlString = Function.GetParamFromSqlString(text2);
						if (paramFromSqlString != null && paramFromSqlString.Any())
						{
							foreach (string item in paramFromSqlString)
							{
								if (!string.IsNullOrEmpty(item))
								{
									string[] array = item.Replace("@", "").Replace(" ", "").Split('.');
									Function.ToString(dt.Rows[0]["DependTable"]);
									if (array.Length > 1)
									{
										if (string.IsNullOrEmpty(array[0]))
										{
											Function.ToString(dt.Rows[0]["DependTable"]);
										}
										else
										{
											_ = array[0];
										}
										_ = array[1];
									}
									else if (array.Length != 0)
									{
										_ = array[0];
									}
									Function.ToString(btnSearch.EditValue);
								}
							}
						}
						frm_ChooseFromList2.FormattedSearch = text2;
						frm_ChooseFromList2.ShowDialog(btnSearch.FindForm());
						if (frm_ChooseFromList2.ChooseItem != null)
						{
							btnSearch.EditValue = (object)frm_ChooseFromList2.ChooseItem.Row[0];
						}
					}
					catch (Exception ex)
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					}
				};
				return btnSearch;
			}
		}
		if (field.ValidValues != null && field.ValidValues.Count > 0)
		{
			LookUpEditEx lookUpEditEx = new LookUpEditEx();
			lookUpEditEx.Properties.ValueMember = "Value";
			lookUpEditEx.Properties.DisplayMember = "Description";
			lookUpEditEx.Properties.NullText = "";
			lookUpEditEx.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			lookUpEditEx.Properties.HotTrackItems = true;
			LookUpEditEx lookUpEditEx2 = lookUpEditEx;
			switch (field.Type)
			{
			case BoFieldTypes.db_Alpha:
				lookUpEditEx2.Properties.DataSource = field.ValidValues;
				break;
			case BoFieldTypes.db_Numeric:
				lookUpEditEx2.Properties.DataSource = field.ValidValues.Select((ValidValuesMDImpl item) => new
				{
					Value = Function.ParseInt(item.Value),
					Description = item.Description
				});
				break;
			case BoFieldTypes.db_Float:
				lookUpEditEx2.Properties.DataSource = field.ValidValues.Select((ValidValuesMDImpl item) => new
				{
					Value = Function.ParseDecimal(item.Value),
					Description = item.Description
				});
				break;
			default:
				lookUpEditEx2.Properties.DataSource = field.ValidValues;
				break;
			}
			return lookUpEditEx2;
		}
		return new TextEditEx();
	}

	public static void Memo_Popup(object sender, EventArgs e)
	{
		try
		{
			if (sender is IPopupControl { PopupWindow: MemoExPopupForm { ActiveControl: TextBoxMaskBox activeControl } })
			{
				string text = Convert.ToString(activeControl.EditValue);
				text = text.Replace("(newline)", Environment.NewLine);
				activeControl.EditValue = text;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public static void Memo_FormatEditValue(object sender, ConvertEditValueEventArgs e)
	{
		try
		{
			string text = Function.ToString(e.Value);
			text = text.Replace("(newline)", Environment.NewLine);
			e.Value = text;
			e.Handled = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public static void Memo_ParseEditValue(object sender, ConvertEditValueEventArgs e)
	{
		try
		{
			string text = Function.ToString(e.Value);
			text = text.Replace(Environment.NewLine, "(newline)");
			e.Value = text;
			e.Handled = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public static DataTable GetTableStructure(string table)
	{
		DataTable dataTable = new DataTable();
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BusinessMasterData");
			dataTable = httpClientHelper.Get("GetBPTableStructure", new { table });
			if (table.Equals("APZ_OCLG") && dataTable != null && !dataTable.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.Equals("OwnerAuth")))
			{
				dataTable.Columns.Add("OwnerAuth", typeof(string));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return dataTable;
	}

	public static async Task<DataTable> GetTableStructureAsync(string table)
	{
		DataTable ret = new DataTable();
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BusinessMasterData");
			ret = await hp.GetAsync("GetBPTableStructure", new { table });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return ret;
	}

	public static void GetNewRow_UDF(GridControlEx grd, string tablename)
	{
		try
		{
			DataTable dataTable = new DataTable();
			List<UserFieldsImpl> list = new List<UserFieldsImpl>();
			using HttpClientHelper<List<UserFieldsImpl>> httpClientHelper = new HttpClientHelper<List<UserFieldsImpl>>(Configuration.ApiServer, "UDF");
			list = httpClientHelper.Get("GetListField", new { tablename });
			if (list == null || list.Count <= 0)
			{
				return;
			}
			foreach (UserFieldsImpl item in list)
			{
				DataColumn dataColumn = new DataColumn();
				switch (item.Type)
				{
				case BoFieldTypes.db_Alpha:
					dataColumn.DataType = typeof(string);
					break;
				case BoFieldTypes.db_Memo:
					dataColumn.DataType = typeof(string);
					break;
				case BoFieldTypes.db_Numeric:
					dataColumn.DataType = typeof(int);
					break;
				case BoFieldTypes.db_Date:
					dataColumn.DataType = typeof(DateTime);
					break;
				case BoFieldTypes.db_Float:
					dataColumn.DataType = typeof(double);
					break;
				}
				if (!string.IsNullOrWhiteSpace(item.DefaultValue))
				{
					dataColumn.DefaultValue = item.DefaultValue;
				}
				dataColumn.ColumnName = "U_" + item.Name;
				dataColumn.Caption = item.Description;
				dataTable.Columns.Add(dataColumn);
				dataTable.AcceptChanges();
			}
			DataRow row = dataTable.NewRow();
			dataTable.Rows.Add(row);
			dataTable.AcceptChanges();
			grd.DataSource = dataTable;
			grd.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public static async Task<DataTable> Get_UDFs(string tablename)
	{
		DataTable dt = new DataTable();
		try
		{
			new List<UserFieldsImpl>();
			using HttpClientHelper<UserFieldsImpl> helper = new HttpClientHelper<UserFieldsImpl>(Configuration.ApiServer, "UDF");
			List<UserFieldsImpl> list = await helper.GetListAsyncByPr("GetListField", new { tablename });
			if (list != null && list.Count > 0)
			{
				foreach (UserFieldsImpl item in list)
				{
					DataColumn dataColumn = new DataColumn();
					switch (item.Type)
					{
					case BoFieldTypes.db_Alpha:
						dataColumn.DataType = typeof(string);
						break;
					case BoFieldTypes.db_Memo:
						dataColumn.DataType = typeof(string);
						break;
					case BoFieldTypes.db_Numeric:
						dataColumn.DataType = typeof(int);
						break;
					case BoFieldTypes.db_Date:
						dataColumn.DataType = typeof(DateTime);
						break;
					case BoFieldTypes.db_Float:
						dataColumn.DataType = typeof(double);
						break;
					}
					dataColumn.ColumnName = "U_" + item.Name;
					dataColumn.Caption = item.Description;
					dt.Columns.Add(dataColumn);
					dt.AcceptChanges();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return dt;
	}

	internal static async Task<List<UserFieldsImpl>> GetUdfFields(string tablename)
	{
		List<UserFieldsImpl> lst = new List<UserFieldsImpl>();
		try
		{
			using HttpClientHelper<UserFieldsImpl> helper = new HttpClientHelper<UserFieldsImpl>(Configuration.ApiServer, "UDF");
			lst = await helper.GetListAsyncByPr("GetListField", new { tablename });
			if (lst != null && lst.Count > 0)
			{
				foreach (UserFieldsImpl item in lst)
				{
					item.Name = "U_" + item.Name;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return lst;
	}

	internal static async Task CreateUDFFiels(GridViewEx gv, string tablename)
	{
		new DataTable();
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UDF");
			DataTable dataTable = await helper.GetAsync("GetListUDFFieldSQL", new { tablename });
			if (dataTable == null || dataTable.Rows.Count <= 0)
			{
				return;
			}
			string text = Function.ToString(dataTable.Rows[0][0]);
			if (text.Length <= 0)
			{
				return;
			}
			text = text.Remove(text.Length - 1);
			string[] array = text.Split(',');
			for (int i = 0; i < array.Length; i++)
			{
				string text2 = array[i].Split(' ')[0];
				string text3 = array[i].Split(' ')[1];
				if (gv.Columns.ColumnByFieldName(text2) == null)
				{
					GridColumnEx gridColumnEx = new GridColumnEx
					{
						FieldName = text2,
						Caption = text2,
						ShowUnboundExpressionMenu = true,
						Visible = true,
						Width = 100
					};
					switch (text3)
					{
					case "money":
					{
						gridColumnEx.UnboundType = UnboundColumnType.Decimal;
						RepositoryItemCalcEdit columnEdit8 = new RepositoryItemCalcEdit();
						gridColumnEx.DisplayFormat.FormatType = FormatType.Numeric;
						gridColumnEx.DisplayFormat.Format = GlobalData.UnitPriceFormat;
						gridColumnEx.ColumnEdit = columnEdit8;
						break;
					}
					case "numeric":
					{
						gridColumnEx.UnboundType = UnboundColumnType.Decimal;
						RepositoryItemCalcEdit columnEdit7 = new RepositoryItemCalcEdit();
						gridColumnEx.DisplayFormat.FormatType = FormatType.Numeric;
						gridColumnEx.DisplayFormat.Format = GlobalData.UnitPriceFormat;
						gridColumnEx.ColumnEdit = columnEdit7;
						break;
					}
					case "double":
					{
						gridColumnEx.UnboundType = UnboundColumnType.Decimal;
						RepositoryItemCalcEdit columnEdit6 = new RepositoryItemCalcEdit();
						gridColumnEx.DisplayFormat.FormatType = FormatType.Numeric;
						gridColumnEx.DisplayFormat.Format = GlobalData.UnitPriceFormat;
						gridColumnEx.ColumnEdit = columnEdit6;
						break;
					}
					case "float":
					{
						gridColumnEx.UnboundType = UnboundColumnType.Decimal;
						RepositoryItemCalcEdit columnEdit5 = new RepositoryItemCalcEdit();
						gridColumnEx.DisplayFormat.FormatType = FormatType.Numeric;
						gridColumnEx.DisplayFormat.Format = GlobalData.UnitPriceFormat;
						gridColumnEx.ColumnEdit = columnEdit5;
						break;
					}
					case "decimal":
					{
						gridColumnEx.UnboundType = UnboundColumnType.Decimal;
						RepositoryItemCalcEdit columnEdit4 = new RepositoryItemCalcEdit();
						gridColumnEx.DisplayFormat.FormatType = FormatType.Numeric;
						gridColumnEx.DisplayFormat.Format = GlobalData.UnitPriceFormat;
						gridColumnEx.ColumnEdit = columnEdit4;
						break;
					}
					case "datetime":
					{
						RepositoryItemDateEdit repositoryItemDateEdit2 = new RepositoryItemDateEdit();
						repositoryItemDateEdit2.DisplayFormat.FormatType = FormatType.DateTime;
						repositoryItemDateEdit2.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
						gridColumnEx.ColumnEdit = repositoryItemDateEdit2;
						break;
					}
					case "date":
					{
						RepositoryItemDateEdit repositoryItemDateEdit = new RepositoryItemDateEdit();
						repositoryItemDateEdit.DisplayFormat.FormatType = FormatType.DateTime;
						repositoryItemDateEdit.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
						gridColumnEx.ColumnEdit = repositoryItemDateEdit;
						break;
					}
					case "smallint":
					{
						gridColumnEx.UnboundType = UnboundColumnType.Integer;
						RepositoryItemCalcEdit columnEdit3 = new RepositoryItemCalcEdit();
						gridColumnEx.DisplayFormat.FormatType = FormatType.Numeric;
						gridColumnEx.DisplayFormat.Format = GlobalData.UnitPriceFormat;
						gridColumnEx.ColumnEdit = columnEdit3;
						break;
					}
					case "tinyint":
					{
						gridColumnEx.UnboundType = UnboundColumnType.Integer;
						RepositoryItemCalcEdit columnEdit2 = new RepositoryItemCalcEdit();
						gridColumnEx.DisplayFormat.FormatType = FormatType.Numeric;
						gridColumnEx.DisplayFormat.Format = GlobalData.UnitPriceFormat;
						gridColumnEx.ColumnEdit = columnEdit2;
						break;
					}
					case "bigint":
					{
						gridColumnEx.UnboundType = UnboundColumnType.Integer;
						RepositoryItemCalcEdit columnEdit = new RepositoryItemCalcEdit();
						gridColumnEx.DisplayFormat.FormatType = FormatType.Numeric;
						gridColumnEx.DisplayFormat.Format = GlobalData.UnitPriceFormat;
						gridColumnEx.ColumnEdit = columnEdit;
						break;
					}
					case "bit":
						gridColumnEx.UnboundType = UnboundColumnType.Boolean;
						break;
					default:
						gridColumnEx.UnboundType = UnboundColumnType.String;
						break;
					}
					gv.Columns.Add(gridColumnEx);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	internal static async Task<DataTable> GetUDFFielsColumn(string tablename)
	{
		DataTable dtR = new DataTable();
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UDF");
			DataTable dataTable = await helper.GetAsync("GetListUDFFieldSQL", new { tablename });
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				string text = Function.ToString(dataTable.Rows[0][0]);
				if (text.Length > 0)
				{
					text = text.Remove(text.Length - 1);
					string[] array = text.Split(',');
					for (int i = 0; i < array.Length; i++)
					{
						string text2 = array[i].Split(' ')[0];
						string text3 = array[i].Split(' ')[1];
						if (!dtR.Columns.Contains(text2))
						{
							DataColumn dataColumn = new DataColumn
							{
								ColumnName = text2,
								Caption = text2
							};
							switch (text3)
							{
							case "nvarchar":
								dataColumn.DataType = typeof(string);
								break;
							case "char":
								dataColumn.DataType = typeof(string);
								break;
							case "money":
								dataColumn.DataType = typeof(decimal);
								break;
							case "numeric":
								dataColumn.DataType = typeof(decimal);
								break;
							case "float":
								dataColumn.DataType = typeof(double);
								break;
							case "decimal":
								dataColumn.DataType = typeof(decimal);
								break;
							case "datetime":
								dataColumn.DataType = typeof(DateTime);
								break;
							case "date":
								dataColumn.DataType = typeof(DateTime);
								break;
							case "smallint":
								dataColumn.DataType = typeof(short);
								break;
							case "tinyint":
								dataColumn.DataType = typeof(byte);
								break;
							case "bigint":
								dataColumn.DataType = typeof(int);
								break;
							case "bit":
								dataColumn.DataType = typeof(bool);
								break;
							default:
								dataColumn.DataType = typeof(string);
								break;
							}
							dtR.Columns.Add(dataColumn);
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return dtR;
	}

	internal static async Task<List<Formular>> GetFormulas(int type)
	{
		try
		{
			using HttpClientHelper<Formular> client = new HttpClientHelper<Formular>(Configuration.ApiServer, "Commons");
			return await client.GetListAsyncByPr("GetFormulars", new
			{
				objType = type
			});
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		return new List<Formular>();
	}

	internal static async Task<List<Formular>> GetFormulas(string type)
	{
		try
		{
			using HttpClientHelper<Formular> client = new HttpClientHelper<Formular>(Configuration.ApiServer, "Commons");
			return await client.GetListAsyncByPr("GetFormulars", new
			{
				objType = type
			});
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		return new List<Formular>();
	}

	internal static void _addComboBox(ComboBoxEdit dtYear, List<int> lstYear, bool p)
	{
		try
		{
			if (lstYear == null)
			{
				return;
			}
			dtYear.Properties.Items.Clear();
			foreach (int item in lstYear)
			{
				dtYear.Properties.Items.Add(new Common.ListItems(item.ToString()));
			}
			if (p)
			{
				dtYear.SelectedIndex = 0;
			}
		}
		catch (Exception ex)
		{
			XtraMessageBox.Show(ex.Message);
		}
	}

	internal static async Task CreateStoreUdf(string tblName, string line = "N")
	{
		try
		{
			using HttpClientHelper<string> client = new HttpClientHelper<string>(Configuration.ApiServer, "Commons");
			await client.PostParamsAsJsonAsync("CreateStoreUdf", new string[2] { tblName, line });
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public static async Task ChangeTypeUdf(DataTable dt, string tableName)
	{
		try
		{
			using HttpClientHelper<Common.Table_Code> udfHelper = new HttpClientHelper<Common.Table_Code>(Configuration.ApiServer, "UDF");
			List<Common.Table_Code> list = await udfHelper.GetListAsync("GetDefaultValueUdf", new { tableName });
			if (list != null && list.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					Type typeFromSql = GetTypeFromSql(list[i].Name);
					dt.ChangeColumnDataType(list[i].Code, typeFromSql);
				}
			}
			else
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), $"Table {tableName} GetDefaultValueUdf Data null");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public static async Task CreateColumnsDistRule(GridViewEx gv)
	{
		try
		{
			if (GlobalData.Dimensions == null || GlobalData.Dimensions.Rows.Count <= 0)
			{
				return;
			}
			foreach (DataRow row in GlobalData.Dimensions.Rows)
			{
				AddDistRule(gv, row);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private static void AddDistRule(GridViewEx gv, DataRow r)
	{
		try
		{
			string code = ((r["DimCode"] == null || r["DimCode"] == DBNull.Value) ? "" : r["DimCode"].ToString());
			if (string.IsNullOrWhiteSpace(code))
			{
				return;
			}
			if (code == "1")
			{
				code = "";
			}
			string fieldName = "OcrCode" + code.ToString();
			RepositoryItemButtonEdit repositoryItemButtonEdit = new RepositoryItemButtonEdit();
			repositoryItemButtonEdit.Name = "rep_line_" + fieldName;
			repositoryItemButtonEdit.Buttons[0].Kind = ButtonPredefines.Search;
			repositoryItemButtonEdit.Buttons.Add(new EditorButton(ButtonPredefines.Glyph));
			repositoryItemButtonEdit.Buttons[1].ImageOptions.Image = Resources.right;
			repositoryItemButtonEdit.Buttons[1].IsLeft = true;
			repositoryItemButtonEdit.Buttons[1].Caption = "DrillDown";
			repositoryItemButtonEdit.ButtonClick += (object s, ButtonPressedEventArgs e) =>
			{
				try
				{
					Enums.TypeCFL typeCFL = Enums.TypeCFL.OrcCode;
					using frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(code switch
					{
						"2" => Enums.TypeCFL.OrcCode2, 
						"3" => Enums.TypeCFL.OrcCode3, 
						"4" => Enums.TypeCFL.OrcCode4, 
						"5" => Enums.TypeCFL.OrcCode5, 
						_ => Enums.TypeCFL.OrcCode, 
					}, multiSelect: false, "");
					frm_ChooseFromList2.ShowDialog(gv.GridControl.FindForm());
					if (frm_ChooseFromList2.ChooseItem != null)
					{
						dynamic chooseItem = frm_ChooseFromList2.ChooseItem;
						gv.SetRowCellValue(gv.FocusedRowHandle, fieldName, (chooseItem is DataRow) ? chooseItem[0] : chooseItem.Row[0]);
					}
				}
				catch (Exception ex2)
				{
					WinCommons.ErrorLineNumber(ex2);
				}
			};
			GridColumn gridColumn = gv.Columns[fieldName];
			if (gridColumn == null)
			{
				gridColumn = new GridColumnEx();
				gridColumn.FieldName = fieldName;
				gridColumn.Visible = true;
				gridColumn.Width = 100;
				gv.Columns.Add(gridColumn);
			}
			gridColumn.Caption = ((r["DimDesc"] == null || r["DimDesc"] == DBNull.Value) ? "" : r["DimDesc"].ToString());
			gv.GridControl.RepositoryItems.Add(repositoryItemButtonEdit);
			gridColumn.ColumnEdit = repositoryItemButtonEdit;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private static async Task SetDistRuleColsCaption(GridViewEx gv)
	{
		try
		{
			if (!gv.Columns.Any((GridColumn x) => string.Compare(x.FieldName, "ocrcode", ignoreCase: true) == 0 || string.Compare(x.FieldName, "ocrcode2", ignoreCase: true) == 0 || string.Compare(x.FieldName, "ocrcode3", ignoreCase: true) == 0 || string.Compare(x.FieldName, "ocrcode4", ignoreCase: true) == 0 || string.Compare(x.FieldName, "ocrcode5", ignoreCase: true) == 0))
			{
				return;
			}
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Sales");
			DataTable dataTable = await hp.GetAsync("GetDimensions");
			if (dataTable == null || dataTable.Rows.Count <= 0)
			{
				return;
			}
			foreach (DataRow row in dataTable.Rows)
			{
				string dimCode = ((Function.ToString(row["DimCode"]) == "1") ? "OcrCode" : ("OcrCode" + Function.ToString(row["DimCode"])));
				GridColumn gridColumn = gv.Columns.FirstOrDefault((GridColumn x) => x.FieldName.ToUpper() == dimCode.ToUpper());
				if (gridColumn != null)
				{
					gridColumn.Caption = ((row["DimDesc"] == null || row["DimDesc"] == DBNull.Value) ? "" : Function.ToString(row["DimDesc"]));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	internal static void AddOwnerColumn(GridViewEx gv)
	{
		try
		{
			GridColumnEx gridColumnEx = new GridColumnEx();
			gridColumnEx.FieldName = "OwnerName";
			gridColumnEx.Caption = "Owner";
			gridColumnEx.Visible = true;
			gridColumnEx.Width = 70;
			gridColumnEx.VisibleIndex = 4;
			RepositoryItemButtonEdit repositoryItemButtonEdit = new RepositoryItemButtonEdit();
			repositoryItemButtonEdit.Name = "rep_line_OwnerCode";
			SerializableAppearanceObject appearance = new SerializableAppearanceObject();
			repositoryItemButtonEdit.Buttons.Add(new EditorButton(ButtonPredefines.Search));
			repositoryItemButtonEdit.Buttons.Add(new EditorButton(ButtonPredefines.Glyph, "", -1, enabled: true, visible: true, isLeft: true, ImageLocation.MiddleCenter, Resources.right, new KeyShortcut(Keys.None), appearance, "", null, null, enableImageTransparency: true));
			if (repositoryItemButtonEdit.Buttons.Count == 3)
			{
				repositoryItemButtonEdit.Buttons.RemoveAt(0);
			}
			repositoryItemButtonEdit.ButtonClick += (object s, ButtonPressedEventArgs e) =>
			{
				try
				{
					if (e.Button.Index == 0)
					{
						LoadOwnerCode(gv);
					}
				}
				catch (Exception ex2)
				{
					WinCommons.ErrorLineNumber(ex2);
				}
			};
			gv.GridControl.RepositoryItems.Add(repositoryItemButtonEdit);
			gridColumnEx.ColumnEdit = repositoryItemButtonEdit;
			gv.Columns.Add(gridColumnEx);
			GridColumnEx gridColumnEx2 = new GridColumnEx();
			gridColumnEx2.FieldName = "OwnerCode";
			gridColumnEx2.Caption = "Owner";
			gridColumnEx2.Visible = false;
			gridColumnEx2.Width = 10;
			gv.Columns.Add(gridColumnEx2);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	internal static void LoadOwnerCode(GridViewEx gv)
	{
		try
		{
			using frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Owner, multiSelect: false, "");
			frm_ChooseFromList2.ShowDialog(gv.GridControl.FindForm());
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				DataRow row = ((DataRowView)frm_ChooseFromList2.ChooseItem).Row;
				gv.SetRowCellValue(gv.FocusedRowHandle, "OwnerCode", row["empID"]);
				gv.SetRowCellValue(gv.FocusedRowHandle, "OwnerName", row["firstName"].ToString() + ", " + row["lastName"].ToString());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	internal static DataTable GetChildItemsInBom(string p, List<Common.Table_ID_Code> _lstBomChilds)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BillOfMasterials");
			return httpClientHelper.Get("GetChildItemsInBom", new
			{
				code = p
			});
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	internal static void SetBOMTreeType(ref DocumentLineTable dt, ref List<Common.Table_ID_Code> lstBomChild)
	{
		try
		{
			lstBomChild.Clear();
			string pCode = "";
			decimal num = 1m;
			foreach (DocumentLineRow row in dt.Rows)
			{
				object obj = (row.IsExistsColumn("TreeType") ? row["TreeType"] : null);
				object obj2 = (row.IsExistsColumn("LineNum") ? row["LineNum"] : null);
				if (obj != DBNull.Value && obj != null && !string.IsNullOrWhiteSpace(obj.ToString()))
				{
					if (obj.ToString() == "S" && obj2 != DBNull.Value && obj2 != null && !string.IsNullOrWhiteSpace(obj2.ToString()))
					{
						pCode = Function.ToString(row["LineNum"]);
						num = Function.ParseDecimal(row["Quantity"]);
						row["BOMTreeType"] = "S";
					}
					else if (obj.ToString() == "I" && (pCode == "0" || Function.ParseInt(pCode) > 0))
					{
						string itCode = Function.ToString(row["ItemCode"]);
						if (!lstBomChild.Exists((Common.Table_ID_Code x) => x.Name == pCode && x.Code == itCode) && num != 0m)
						{
							Common.Table_ID_Code table_ID_Code = new Common.Table_ID_Code();
							table_ID_Code.ID = Function.ParseDecimal(row["Quantity"]) / num;
							table_ID_Code.Code = itCode;
							table_ID_Code.Name = pCode;
							lstBomChild.Add(table_ID_Code);
						}
						if (num != 0m)
						{
							row.RateBomSales = Function.ParseDecimal(row["Quantity"]) / num;
						}
						row["BOMTreeType"] = pCode;
					}
					else
					{
						pCode = "";
					}
				}
				else
				{
					pCode = "";
				}
			}
			dt.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	internal static void SetBOMTreeType(ref DataTable dt, ref List<Common.Table_ID_Code> lstBomChild)
	{
		try
		{
			lstBomChild.Clear();
			string pCode = "";
			decimal num = 1m;
			foreach (DataRow row in dt.Rows)
			{
				object obj = (row.IsExistsColumn("TreeType") ? row["TreeType"] : null);
				object obj2 = (row.IsExistsColumn("LineNum") ? row["LineNum"] : null);
				if (obj != DBNull.Value && obj != null && !string.IsNullOrWhiteSpace(obj.ToString()))
				{
					if (obj.ToString() == "S" && obj2 != DBNull.Value && obj2 != null && !string.IsNullOrWhiteSpace(obj2.ToString()))
					{
						pCode = Function.ToString(row["LineNum"]);
						num = Function.ParseDecimal(row["Quantity"]);
						row["BOMTreeType"] = "S";
					}
					else if (obj.ToString() == "I" && (pCode == "0" || Function.ParseInt(pCode) > 0))
					{
						string itCode = Function.ToString(row["ItemCode"]);
						if (!lstBomChild.Exists((Common.Table_ID_Code x) => x.Name == pCode && x.Code == itCode) && num != 0m)
						{
							Common.Table_ID_Code table_ID_Code = new Common.Table_ID_Code();
							table_ID_Code.ID = Function.ParseDecimal(row["Quantity"]) / num;
							table_ID_Code.Code = itCode;
							table_ID_Code.Name = pCode;
							lstBomChild.Add(table_ID_Code);
						}
						row["BOMTreeType"] = pCode;
					}
					else
					{
						pCode = "";
					}
				}
				else
				{
					pCode = "";
				}
			}
			dt.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	internal static bool DeleteBomChildsItem(GridViewEx gvLine, int rowHandle, ucBatchAndSerial batchSerial, bool isChooseItem = false)
	{
		try
		{
			DocumentLineTable documentLineTable = gvLine.GridControl.DataSource as DocumentLineTable;
			if (!documentLineTable.IsNotNull())
			{
				Logging.Write("WATCH", "Data line is empty");
				return false;
			}
			object rowCellValue = gvLine.GetRowCellValue(rowHandle, "BOMTreeType");
			int num = Function.ParseInt(gvLine.GetRowCellValue(rowHandle, "LineNum"));
			object rowCellValue2 = gvLine.GetRowCellValue(rowHandle, "TreeType");
			if (rowCellValue == null || rowCellValue == DBNull.Value || string.IsNullOrWhiteSpace(rowCellValue.ToString()))
			{
				DeleteBatchSerialRow(batchSerial, num);
				documentLineTable.Rows.Remove(gvLine.GetDataRow(rowHandle));
				documentLineTable.AcceptChanges();
				gvLine.GridControl.DataSource = documentLineTable;
				return true;
			}
			if (rowCellValue.ToString() == "S")
			{
				if ((isChooseItem ? DialogResult.Yes : XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteItemWhenExistsChild, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo)) == DialogResult.Yes)
				{
					List<DataRow> list = new List<DataRow>();
					list.Add(documentLineTable.Rows[gvLine.GetDataSourceRowIndex(rowHandle)]);
					foreach (DataRow row in documentLineTable.Rows)
					{
						object obj = row["BOMTreeType"];
						object obj2 = row["ItemCode"];
						if (rowCellValue != DBNull.Value && obj2 != null && obj2 != DBNull.Value && !string.IsNullOrWhiteSpace(obj2.ToString()) && obj.ToString() != "S" && Function.ToString(obj) == Function.ToString(num))
						{
							list.Add(row);
						}
					}
					if (list.Count > 0)
					{
						DeleteBatchSerialRow(batchSerial, num);
						foreach (DataRow item in list)
						{
							documentLineTable.Rows.Remove(item);
						}
						documentLineTable.AcceptChanges();
						return true;
					}
				}
			}
			else
			{
				if (rowCellValue2 == null || rowCellValue2 == DBNull.Value || !(rowCellValue2.ToString() == "I") || (!(Function.ToString(rowCellValue) == "0") && Function.ParseInt(rowCellValue) <= 0))
				{
					gvLine.UnselectRow(gvLine.DataRowCount - 1);
					DeleteBatchSerialRow(batchSerial, -1);
					gvLine.DeleteSelectedRows();
					documentLineTable.AcceptChanges();
					return true;
				}
				XtraMessageBox.Show("Can not delete child items in BOM", "Information", MessageBoxButtons.OK);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return false;
	}

	private static void DeleteBatchSerialRow(ucBatchAndSerial batchSerial, int lineNum)
	{
		try
		{
			batchSerial?.DeleteBatchAndSerialByLineNum(lineNum);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	internal static void GenerateBOMItems(ref DocumentLineTable docLine)
	{
		try
		{
			string value = "";
			foreach (DataRow row in docLine.Rows)
			{
				object obj = row["TreeType"];
				if (obj != null && obj != DBNull.Value && !string.IsNullOrWhiteSpace(obj.ToString()))
				{
					if (obj.ToString().Trim() == "S")
					{
						value = Function.ToString(row["LineNum"]).Trim();
						row["BOMTreeType"] = obj.ToString().Trim();
					}
					else if (obj.ToString().Trim() == "I")
					{
						row["BOMTreeType"] = value;
					}
					else
					{
						row["BOMTreeType"] = "";
						value = "";
					}
				}
				else
				{
					value = "";
				}
			}
			docLine.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	internal static int GetRowHandleByValue(GridViewEx gv, string fieldName, string value)
	{
		try
		{
			for (int i = 0; i < gv.DataRowCount; i++)
			{
				if (Function.ToString(gv.GetRowCellValue(i, fieldName)) == value)
				{
					return i;
				}
			}
			return -1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return -1;
		}
	}

	internal static void DeleteDocLineClick(GridViewEx gvLine, GridViewEx gvDoc)
	{
		try
		{
			gvLine.UnselectRow(gvLine.DataRowCount - 1);
			gvLine.DeleteSelectedRows();
			DocumentLineTable obj = gvLine.GridControl.DataSource as DocumentLineTable;
			obj.AcceptChanges();
			int num = obj.GetEnumerable().FindIndex((DocumentLineRow x) => !string.IsNullOrWhiteSpace(x.ItemCode));
			if (num != -1)
			{
				double num2 = Function.ParseDouble(gvLine.GetRowCellValue(gvLine.GetRowHandle(num), "Quantity"));
				gvLine.SetRowCellValue(gvLine.GetRowHandle(num), "Quantity", num2 - 1.0);
				gvLine.SetRowCellValue(gvLine.GetRowHandle(num), "Quantity", num2);
				return;
			}
			DocumentTable obj2 = gvDoc.GridControl.DataSource as DocumentTable;
			DocumentRow documentRow = obj2[gvDoc.GetFocusedDataSourceRowIndex()];
			if (documentRow != null)
			{
				documentRow.DocTotal = 0.0;
				documentRow.DocTotalFc = 0.0;
				documentRow.TotalBeforeDis = 0.0;
				documentRow.Total = 0.0;
				documentRow.VatSum = 0.0;
				documentRow.VatSumFc = 0.0;
				documentRow.TaxTotal = 0.0;
			}
			obj2.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	internal static PropertyInfo[] GetProperties(object obj)
	{
		return obj.GetType().GetProperties();
	}

	internal static DataTable GetDataTableFromObject(object obj)
	{
		if (obj != null)
		{
			Type type = obj.GetType();
			DataTable dataTable = new DataTable(type.Name);
			PropertyInfo[] properties = type.GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				dataTable.Columns.Add(new DataColumn(propertyInfo.Name));
			}
			return dataTable;
		}
		return null;
	}

	public static DataTable GetDataTableFromObjects(object[] objects)
	{
		if (objects != null && objects.Length != 0)
		{
			Type type = objects[0].GetType();
			DataTable dataTable = new DataTable(type.Name);
			PropertyInfo[] properties = type.GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				dataTable.Columns.Add(new DataColumn(propertyInfo.Name));
			}
			foreach (object obj in objects)
			{
				DataRow dataRow = dataTable.NewRow();
				foreach (DataColumn column in dataTable.Columns)
				{
					dataRow[column.ColumnName] = obj.GetType().GetProperty(column.ColumnName).GetValue(obj, null);
				}
				dataTable.Rows.Add(dataRow);
			}
			return dataTable;
		}
		return null;
	}

	internal static string UploadFileToServer(string _PathFileServer, string _PathFileLocal, string _UserName, string _PassWord)
	{
		try
		{
			using WebClient webClient = new WebClient();
			webClient.Credentials = new NetworkCredential(_UserName, _PassWord);
			Uri address = new Uri(_PathFileServer);
			webClient.UploadFile(address, _PathFileLocal);
		}
		catch (WebException ex)
		{
			return ex.Message;
		}
		return null;
	}

	internal static string DeleteFileOnServer(string _PathFileServer, string _UserName, string _PassWord)
	{
		try
		{
			FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(_PathFileServer);
			ftpWebRequest.Method = "DELE";
			ftpWebRequest.Credentials = new NetworkCredential(_UserName, _PassWord);
			_ = (FtpWebResponse)ftpWebRequest.GetResponse();
		}
		catch (WebException ex)
		{
			return ex.Message;
		}
		return null;
	}

	internal static string FileExistsOnServer(string _PathFileServer, string _UserName, string _PassWord)
	{
		try
		{
			FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(_PathFileServer);
			ftpWebRequest.Credentials = new NetworkCredential(_UserName, _PassWord);
			ftpWebRequest.Method = "SIZE";
			_ = (FtpWebResponse)ftpWebRequest.GetResponse();
		}
		catch (WebException ex)
		{
			return ex.Message;
		}
		return null;
	}

	internal static string FolderExistsOnServer(string _PathFolderServer, string _UserName, string _PassWord)
	{
		try
		{
			FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(_PathFolderServer);
			ftpWebRequest.Credentials = new NetworkCredential(_UserName, _PassWord);
			ftpWebRequest.Method = "NLST";
			_ = (FtpWebResponse)ftpWebRequest.GetResponse();
		}
		catch (WebException ex)
		{
			return ex.Message;
		}
		return null;
	}

	internal static bool IsFolderOnServer(string _PathFileServer, string userName, string passWord)
	{
		try
		{
			FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(_PathFileServer);
			ftpWebRequest.Credentials = new NetworkCredential(userName, passWord);
			ftpWebRequest.Method = "MDTM";
			_ = (FtpWebResponse)ftpWebRequest.GetResponse();
		}
		catch (WebException ex)
		{
			if (((FtpWebResponse)ex.Response).StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
			{
				return true;
			}
			return false;
		}
		return false;
	}

	internal static List<string> ListFilesOnServer(string folderOnServer, string userName, string passWord)
	{
		try
		{
			FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(folderOnServer);
			ftpWebRequest.Method = "NLST";
			ftpWebRequest.Credentials = new NetworkCredential(userName, passWord);
			FtpWebResponse ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
			StreamReader streamReader = new StreamReader(ftpWebResponse.GetResponseStream());
			string text = streamReader.ReadToEnd();
			streamReader.Close();
			ftpWebResponse.Close();
			return text.Split(new string[1] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
		}
		catch (WebException)
		{
			return null;
		}
	}

	internal static string CreateFolderOnServer(string folderOnServer, string _UserName, string _PassWord)
	{
		try
		{
			WebRequest webRequest = WebRequest.Create(folderOnServer);
			webRequest.Credentials = new NetworkCredential(_UserName, _PassWord);
			webRequest.Method = "MKD";
			webRequest.GetResponse();
		}
		catch (WebException ex)
		{
			return ex.Message;
		}
		return null;
	}

	internal static string DownLoadFolderOnServer(string folderOnServer, string folderLocal, string _UserName, string _PassWord)
	{
		List<Common.ListItems> list = new List<Common.ListItems>();
		try
		{
			Uri uri = new Uri(folderOnServer);
			List<string> list2 = ListFilesOnServer(folderOnServer, _UserName, _PassWord);
			if (list2 == null || list2.Count == 0)
			{
				return "Không tìm thấy file";
			}
			string text = folderLocal + "Old";
			if (FileIO.FolderExist(text))
			{
				if (FileIO.FolderDelete(text))
				{
					FileIO.FolderCreate(text);
				}
			}
			else
			{
				FileIO.FolderCreate(text);
			}
			string newVersion = "";
			foreach (string item in list2)
			{
				list = MoveFileOldVersionToFolderOld(Path.GetDirectoryName(folderLocal + "\\" + item), text, Path.GetFileName(item), ref newVersion);
				FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(uri?.ToString() + "/" + item);
				ftpWebRequest.Method = "RETR";
				ftpWebRequest.Credentials = new NetworkCredential(_UserName, _PassWord);
				ftpWebRequest.UsePassive = false;
				Stream responseStream = ((FtpWebResponse)ftpWebRequest.GetResponse()).GetResponseStream();
				FileStream fileStream = new FileStream(folderLocal + "\\" + item, FileMode.Create, FileAccess.ReadWrite);
				byte[] array = new byte[2048];
				int count;
				while ((count = responseStream.Read(array, 0, array.Length)) > 0)
				{
					fileStream.Write(array, 0, count);
				}
				fileStream.Close();
			}
		}
		catch (WebException ex)
		{
			if (list != null && list.Count > 0)
			{
				foreach (Common.ListItems item2 in list)
				{
					FileIO.MoveFile(item2.Value, item2.Text);
				}
			}
			return ex.Message;
		}
		return null;
	}

	internal static string RemoveOldFile(string keyName, string folderLocal)
	{
		try
		{
			string text = folderLocal + "Old";
			if (!FileIO.FolderExist(text))
			{
				FileIO.FolderCreate(text);
			}
			string[] files = Directory.GetFiles(folderLocal);
			for (int i = 0; i < files.Length; i++)
			{
				string fileName = Path.GetFileName(files[i]);
				if (fileName.Contains(keyName))
				{
					string text2 = folderLocal + "\\" + fileName;
					if (FileIO.FileExist(text + "\\" + fileName))
					{
						FileIO.DeleteFile(text + "\\" + fileName);
					}
					if (FileIO.FileExist(text2))
					{
						FileIO.MoveFile(text2, text + "\\" + fileName);
					}
				}
			}
		}
		catch (WebException ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return ex.Message;
		}
		return null;
	}

	internal static bool DownLoadFolderComplexOnServer(string folderOnServer, string folderLocal, string userName, string passWord)
	{
		bool flag = false;
		try
		{
			new Uri(folderOnServer);
			List<string> list = ListFilesOnServer(folderOnServer, userName, passWord);
			if (list == null || list.Count == 0)
			{
				return false;
			}
			if (!FileIO.FolderExist(folderLocal))
			{
				FileIO.FolderCreate(folderLocal);
			}
			foreach (string item in list)
			{
				if (IsFolderOnServer(folderOnServer + "/" + item, userName, passWord))
				{
					DownLoadFolderComplexOnServer(folderOnServer + "/" + item, folderLocal + "\\" + item, userName, passWord);
				}
				else
				{
					DownLoadFileFromServer(folderOnServer + "/" + item, folderLocal + "\\" + item, userName, passWord);
				}
			}
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	internal static string DownLoadFileFromServer(string _PathFileServer, string _PathFileLocal, string userName, string passWord)
	{
		try
		{
			Uri address = new Uri(_PathFileServer);
			using WebClient webClient = new WebClient();
			webClient.Credentials = new NetworkCredential(userName, passWord);
			byte[] array = webClient.DownloadData(address);
			using FileStream fileStream = new FileStream(_PathFileLocal, FileMode.Create, FileAccess.Write, FileShare.None);
			fileStream.Write(array, 0, array.Length);
			fileStream.Close();
		}
		catch (WebException ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return null;
	}

	internal static async Task<Function.ConfigFTPServer> GetFtpConfig()
	{
		try
		{
			using HttpClientHelper<Function.ConfigFTPServer> helper = new HttpClientHelper<Function.ConfigFTPServer>(Configuration.ApiServer, "ConfigConnect");
			return await helper.GetAsync("GetFtpConfig");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new Function.ConfigFTPServer();
	}

	internal static bool FolderExistsOnNetwork(string folderOnServer, string computerName, string userName, string password)
	{
		try
		{
			using (NetworkShareAccesser.Access(computerName, userName, password))
			{
				return FileIO.FolderExist(folderOnServer);
			}
		}
		catch (Exception ex)
		{
			if (ex.Message.StartsWith("Multiple connections"))
			{
				return FileIO.FolderExist(folderOnServer);
			}
			return false;
		}
	}

	internal static bool FileExistNetwork(string pathFile, string computerName, string userName, string password)
	{
		try
		{
			using (NetworkShareAccesser.Access(computerName, userName, password))
			{
				return FileIO.FileExist(pathFile);
			}
		}
		catch (Exception ex)
		{
			if (ex.Message.StartsWith("Multiple connections"))
			{
				return FileIO.FileExist(pathFile);
			}
			return false;
		}
	}

	internal static async Task<string> CopyFileNetwork(string fromFile, string toFile)
	{
		try
		{
			Function.ConfigFTPServer configFTPServer = await GetFtpConfig();
			if (string.IsNullOrEmpty(configFTPServer.Address) || configFTPServer.Address.Length <= 5 || !configFTPServer.Address.StartsWith("ftp://"))
			{
				return "";
			}
			string remoteComputerName = configFTPServer.Address.Substring(6, configFTPServer.Address.Length - 6);
			string user = configFTPServer.User;
			string passWord = configFTPServer.PassWord;
			using (NetworkShareAccesser.Access(remoteComputerName, user, passWord))
			{
				string extenName = FileIO.getExtenName(fromFile);
				string text = toFile.Substring(0, toFile.LastIndexOf("."));
				int num = 1;
				while (FileIO.FileExist(toFile))
				{
					toFile = text + num + "." + extenName;
					num++;
				}
				FileIO.CopyFile(fromFile, toFile, blnOverWrite: true);
			}
		}
		catch (Exception ex)
		{
			if (!ex.Message.StartsWith("Multiple connections"))
			{
				return "";
			}
			string extenName2 = FileIO.getExtenName(fromFile);
			string text2 = toFile.Substring(0, toFile.LastIndexOf("."));
			int num2 = 1;
			while (FileIO.FileExist(toFile))
			{
				toFile = text2 + num2 + "." + extenName2;
				num2++;
			}
			FileIO.CopyFile(fromFile, toFile, blnOverWrite: true);
		}
		return toFile;
	}

	internal static string CopyFileNetwork(string fromFile, string toFile, string computerName, string userName, string password)
	{
		try
		{
			using (NetworkShareAccesser.Access(computerName, userName, password))
			{
				FileIO.CopyFile(fromFile, toFile, blnOverWrite: true);
			}
		}
		catch (Exception ex)
		{
			if (!ex.Message.StartsWith("Multiple connections"))
			{
				return ex.Message;
			}
			FileIO.CopyFile(fromFile, toFile, blnOverWrite: true);
		}
		return "";
	}

	internal static string CopyFolderFromNetwork(string fromFolder, string toFolder, string computerName, string userName, string password)
	{
		try
		{
			using (NetworkShareAccesser.Access(computerName, userName, password))
			{
				if (!FileIO.FolderExist(fromFolder))
				{
					return "Folder not exists " + fromFolder;
				}
				string[] files = Directory.GetFiles(fromFolder);
				if (files == null || files.Length == 0)
				{
					return $"Folder {fromFolder} is empty!";
				}
				string text = toFolder + "Old";
				if (FileIO.FolderExist(text))
				{
					if (FileIO.FolderDelete(text))
					{
						FileIO.FolderCreate(text);
					}
				}
				else
				{
					FileIO.FolderCreate(text);
				}
				CopySubFolderNetwork(fromFolder, toFolder, text);
			}
		}
		catch (Exception ex)
		{
			if (!ex.Message.StartsWith("Multiple connections"))
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				return ex.Message;
			}
			if (!FileIO.FolderExist(fromFolder))
			{
				return "Folder not exists " + fromFolder;
			}
			string[] files2 = Directory.GetFiles(fromFolder);
			if (files2 == null || files2.Length == 0)
			{
				return $"Folder {fromFolder} is empty!";
			}
			string text2 = toFolder + "Old";
			if (FileIO.FolderExist(text2))
			{
				if (FileIO.FolderDelete(text2))
				{
					FileIO.FolderCreate(text2);
				}
			}
			else
			{
				FileIO.FolderCreate(text2);
			}
			string[] array = files2;
			foreach (string strSourceFile in array)
			{
				string text3 = $"{toFolder}\\{FileIO.getFileName(strSourceFile)}";
				if (FileIO.FileExist(text3))
				{
					FileIO.MoveFile(text3, $"{text2}\\{FileIO.getFileName(strSourceFile)}");
					FileIO.CopyFile(strSourceFile, text3, blnOverWrite: true);
				}
				else
				{
					FileIO.CopyFile(strSourceFile, text3, blnOverWrite: true);
				}
			}
		}
		return "";
	}

	internal static string CopySubFolderNetwork(string fromFolder, string toFolder, string backUpFolder)
	{
		try
		{
			string[] files = Directory.GetFiles(fromFolder);
			if (files.Length != 0)
			{
				string newVersion = "";
				string[] array = files;
				foreach (string text in array)
				{
					string strDestinationFile = $"{toFolder}\\{FileIO.getFileName(text)}";
					MoveFileOldVersionToFolderOld(toFolder, backUpFolder, Path.GetFileName(text), ref newVersion);
					FileIO.CopyFile(text, strDestinationFile, blnOverWrite: true);
				}
			}
			string[] directories = Directory.GetDirectories(fromFolder);
			if (directories.Length != 0)
			{
				string[] array = directories;
				foreach (string text2 in array)
				{
					string text3 = $"{toFolder}\\{FileIO.getFileName(text2)}";
					string text4 = $"{backUpFolder}\\{FileIO.getFileName(text2)}";
					if (!FileIO.FolderExist(text3))
					{
						FileIO.FolderCreate(text3);
					}
					FileIO.FolderCreate(text4);
					CopySubFolderNetwork(text2, text3, text4);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return "";
	}

	internal static string DeleteFileNetwork(string pathFile, string computerName, string userName, string password)
	{
		try
		{
			using (NetworkShareAccesser.Access(computerName, userName, password))
			{
				FileIO.DeleteFile(pathFile);
			}
		}
		catch (Exception ex)
		{
			if (!ex.Message.StartsWith("Multiple connections"))
			{
				return ex.Message;
			}
			FileIO.DeleteFile(pathFile);
		}
		return "";
	}

	internal static string OpenFileNetwork(string pathFile, string computerName, string userName, string password)
	{
		try
		{
			using (NetworkShareAccesser.Access(computerName, userName, password))
			{
				string extenName = FileIO.getExtenName(pathFile);
				OpenFile(pathFile, extenName);
			}
		}
		catch (Exception ex)
		{
			if (!ex.Message.StartsWith("Multiple connections"))
			{
				return ex.Message;
			}
			string extenName2 = FileIO.getExtenName(pathFile);
			OpenFile(pathFile, extenName2);
		}
		return "";
	}

	internal static async Task<string> OpenFileNetwork(string pathFile)
	{
		try
		{
			Function.ConfigFTPServer configFTPServer = await GetFtpConfig();
			if (string.IsNullOrEmpty(configFTPServer.Address) || configFTPServer.Address.Length <= 5 || !configFTPServer.Address.StartsWith("ftp://"))
			{
				return "Cannot connect to server!";
			}
			string remoteComputerName = configFTPServer.Address.Substring(6, configFTPServer.Address.Length - 6);
			string user = configFTPServer.User;
			string passWord = configFTPServer.PassWord;
			using (NetworkShareAccesser.Access(remoteComputerName, user, passWord))
			{
				FileIO.getExtenName(pathFile);
				Process.Start(pathFile);
			}
		}
		catch (Exception ex)
		{
			if (!ex.Message.StartsWith("Multiple connections"))
			{
				return ex.Message;
			}
			FileIO.getExtenName(pathFile);
			Process.Start(pathFile);
		}
		return "";
	}

	private static void OpenFile(string pathFile, string exten)
	{
		string[] source = new string[4] { "jpg", "bmp", "gif", "png" };
		_ = new string[2] { "xls", "xlsx" };
		if (source.Contains(exten.ToLower()))
		{
			Process.Start(pathFile);
		}
	}

	internal static string PathOpenFile(string fileType, string fileExten)
	{
		string result = string.Empty;
		try
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = string.Format("{0} Files (*.{1}*)|*.{1}", fileType, fileExten);
			openFileDialog.FilterIndex = 1;
			openFileDialog.Multiselect = false;
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				result = openFileDialog.FileName;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return result;
	}

	internal static string[] PathOpenMultiFile(string fileType, string fileExten)
	{
		try
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = string.Format("{0} Files (*.{1}*)|*.{1}", fileType, fileExten);
			openFileDialog.FilterIndex = 1;
			openFileDialog.Multiselect = true;
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				return openFileDialog.FileNames;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return null;
	}

	internal static string PathSaveFolder()
	{
		try
		{
			FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
			if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
			{
				return folderBrowserDialog.SelectedPath;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return "";
	}

	public static void setConfigSkins(ConfigSkins _skin)
	{
		using ResourceWriter resourceWriter = new ResourceWriter(_StartupPath + "ConfigSkins.dat");
		resourceWriter.AddResource("Caption", _skin.Caption);
		resourceWriter.Generate();
		resourceWriter.Close();
	}

	public static ConfigSkins getConfigSkins()
	{
		ConfigSkins configSkins = new ConfigSkins("Office 2010 Blue");
		try
		{
			using ResourceSet resourceSet = new ResourceSet(_StartupPath + "ConfigSkins.dat");
			configSkins.Caption = resourceSet.GetString("Caption");
			return configSkins;
		}
		catch (Exception)
		{
			return configSkins;
		}
	}

	public static void setConfigLogin(ConfigLogin _login)
	{
		try
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (File.Exists(_StartupPath + "ConfigLogin.dat"))
			{
				using ResourceSet resourceSet = new ResourceSet(_StartupPath + "ConfigLogin.dat");
				dictionary = resourceSet.GetObject("InfoLogin") as Dictionary<string, string>;
				resourceSet.Close();
			}
			if (dictionary == null)
			{
				dictionary = new Dictionary<string, string>();
			}
			if (dictionary.ContainsKey(Environment.UserName))
			{
				dictionary.Remove(Environment.UserName);
			}
			dictionary.Add(Environment.UserName, $"{Function.ToString(_login.Remeber)};{Function.ToString(_login.Domain)};{Function.ToString(_login.Database)};{Function.ToString(_login.User)};0;");
			using ResourceWriter resourceWriter = new ResourceWriter(_StartupPath + "ConfigLogin.dat");
			resourceWriter.AddResource("InfoLogin", dictionary);
			resourceWriter.Generate();
			resourceWriter.Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public static ConfigLogin getConfigLogin()
	{
		ConfigLogin configLogin = new ConfigLogin();
		try
		{
			using ResourceSet resourceSet = new ResourceSet(_StartupPath + "ConfigLogin.dat");
			if (resourceSet.GetObject("InfoLogin") is Dictionary<string, string> dictionary && dictionary.ContainsKey(Environment.UserName))
			{
				string[] array = dictionary[Environment.UserName].Split(';');
				if (array.Length == 6)
				{
					configLogin.Remeber = array[0];
					configLogin.Domain = array[1];
					configLogin.Database = array[2];
					configLogin.User = array[3];
					configLogin.RePass = array[4];
					configLogin.PassWord = array[5];
				}
			}
			resourceSet.Close();
			return configLogin;
		}
		catch (Exception)
		{
			return configLogin;
		}
	}

	public static string EncryptStringAES(string plainText, string sharedSecret)
	{
		if (string.IsNullOrEmpty(plainText))
		{
			return "";
		}
		if (string.IsNullOrEmpty(sharedSecret))
		{
			return "";
		}
		string text = null;
		RijndaelManaged rijndaelManaged = null;
		try
		{
			Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(sharedSecret, _salt);
			rijndaelManaged = new RijndaelManaged();
			rijndaelManaged.Key = rfc2898DeriveBytes.GetBytes(rijndaelManaged.KeySize / 8);
			ICryptoTransform transform = rijndaelManaged.CreateEncryptor(rijndaelManaged.Key, rijndaelManaged.IV);
			using MemoryStream memoryStream = new MemoryStream();
			memoryStream.Write(BitConverter.GetBytes(rijndaelManaged.IV.Length), 0, 4);
			memoryStream.Write(rijndaelManaged.IV, 0, rijndaelManaged.IV.Length);
			using (CryptoStream stream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
			{
				using StreamWriter streamWriter = new StreamWriter(stream);
				streamWriter.Write(plainText);
			}
			return Convert.ToBase64String(memoryStream.ToArray());
		}
		finally
		{
			rijndaelManaged?.Clear();
		}
	}

	public static string DecryptStringAES(string cipherText, string sharedSecret)
	{
		if (string.IsNullOrEmpty(cipherText))
		{
			return "";
		}
		if (string.IsNullOrEmpty(sharedSecret))
		{
			return "";
		}
		RijndaelManaged rijndaelManaged = null;
		string text = null;
		try
		{
			Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(sharedSecret, _salt);
			using MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(cipherText));
			rijndaelManaged = new RijndaelManaged();
			rijndaelManaged.Key = rfc2898DeriveBytes.GetBytes(rijndaelManaged.KeySize / 8);
			rijndaelManaged.IV = ReadByteArray(memoryStream);
			ICryptoTransform transform = rijndaelManaged.CreateDecryptor(rijndaelManaged.Key, rijndaelManaged.IV);
			using CryptoStream stream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Read);
			using StreamReader streamReader = new StreamReader(stream);
			return streamReader.ReadToEnd();
		}
		finally
		{
			rijndaelManaged?.Clear();
		}
	}

	private static byte[] ReadByteArray(Stream s)
	{
		byte[] array = new byte[4];
		if (s.Read(array, 0, array.Length) != array.Length)
		{
			throw new SystemException("Stream did not contain properly formatted byte array");
		}
		byte[] array2 = new byte[BitConverter.ToInt32(array, 0)];
		if (s.Read(array2, 0, array2.Length) != array2.Length)
		{
			throw new SystemException("Did not read byte array properly");
		}
		return array2;
	}

	internal static async Task PreviewLayout(string cardCode, object docEntry, string FormLayoutCode, XtraFormEx form, string objectId = "")
	{
		try
		{
			List<Apzon.Entities.Models.Administrator.PrintLayout> list = new List<Apzon.Entities.Models.Administrator.PrintLayout>();
			using (HttpClientHelper<Apzon.Entities.Models.Administrator.PrintLayout> helper = new HttpClientHelper<Apzon.Entities.Models.Administrator.PrintLayout>(Configuration.ApiServer, "PrintLayout"))
			{
				list = await helper.GetListAsync("GetLayouts", new
				{
					GlobalData.UserName,
					FormLayoutCode,
					cardCode
				});
			}
			if (list != null && list.Count > 0)
			{
				frm_PreviewLayouts frm = new frm_PreviewLayouts();
				frm.Source = list;
				frm.MdiParent = form.MdiParent;
				frm.FormClosing += async (object s, FormClosingEventArgs e) =>
				{
					try
					{
						Apzon.Entities.Models.Administrator.PrintLayout choosedLayout = frm.ChoosedLayout;
						List<Common.ParameterValue> lstParam = new List<Common.ParameterValue>
						{
							new Common.ParameterValue
							{
								Name = "DOCKEY@",
								Value = Function.ToString(docEntry)
							}
						};
						if (choosedLayout != null && !string.IsNullOrEmpty(choosedLayout.FileName) && choosedLayout.FileName.ToLower().EndsWith("rpt"))
						{
							string text = await ReportViewerListParam($"{Application.StartupPath}\\Report\\{GlobalData.DatabaseName}\\{choosedLayout.FileName}", lstParam, objectId);
							if (!string.IsNullOrEmpty(text))
							{
								WinCommons.Error(Enums.MessageCode.None, text);
							}
						}
						else if (choosedLayout != null && !string.IsNullOrEmpty(choosedLayout.FileName) && choosedLayout.FileName.ToLower().Contains("xls"))
						{
							PreviewExcelMerge(choosedLayout.FileName, form.DataSet());
						}
						else if (choosedLayout != null && !string.IsNullOrEmpty(choosedLayout.XtraText))
						{
							WinCommons._OpenProcessing();
							string value = DecryptStringAES(choosedLayout.XtraText, "APZ_RDOC");
							XtraReport rp;
							using (StreamWriter streamWriter = new StreamWriter(new MemoryStream()))
							{
								streamWriter.Write(value);
								streamWriter.Flush();
								rp = XtraReport.FromStream(streamWriter.BaseStream);
							}
							XtraReportViewer(rp, lstParam);
							WinCommons._CloseProcessing();
						}
					}
					catch (Exception ex2)
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
						WinCommons.ErrorLineNumber(ex2);
					}
				};
				frm.Show();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.LayoutNotFound, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public static async Task<Apzon.Entities.Models.Administrator.PrintLayout> GetDefaultPrintLayout(string userName, string formLayoutCode, string cardCode)
	{
		try
		{
			using HttpClientHelper<Apzon.Entities.Models.Administrator.PrintLayout> helper = new HttpClientHelper<Apzon.Entities.Models.Administrator.PrintLayout>(Configuration.ApiServer, "PrintLayout");
			return await helper.GetAsync("GetDefaultPrintLayout", new { userName, formLayoutCode, cardCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new Apzon.Entities.Models.Administrator.PrintLayout();
	}

	public static async Task PrintLayout(string cardCode, object docEntry, string formLayoutCode, string objectId = "")
	{
		_ = 1;
		try
		{
			string dockey = ((!(formLayoutCode == "ITT1")) ? Function.ParseInt(docEntry).ToString() : Function.ToString(docEntry));
			if ((!(formLayoutCode == "ITT1") || string.IsNullOrEmpty(dockey)) && (!(formLayoutCode != "ITT1") || Function.ParseInt(docEntry) <= 0))
			{
				return;
			}
			Apzon.Entities.Models.Administrator.PrintLayout printLayout = await GetDefaultPrintLayout(GlobalData.UserName, formLayoutCode, Function.ToString(cardCode));
			List<Common.ParameterValue> lstParam = new List<Common.ParameterValue>
			{
				new Common.ParameterValue
				{
					Name = "DOCKEY@",
					Value = dockey
				}
			};
			if (printLayout != null && !string.IsNullOrEmpty(printLayout.FileName))
			{
				string text = await ReportViewerListParam($"{Application.StartupPath}\\Report\\{GlobalData.DatabaseName}\\{printLayout.FileName}", lstParam, objectId);
				if (!string.IsNullOrEmpty(text))
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
			else if (printLayout != null && !string.IsNullOrEmpty(printLayout.XtraText))
			{
				WinCommons._OpenProcessing();
				string value = DecryptStringAES(printLayout.XtraText, "APZ_RDOC");
				XtraReport rp;
				using (StreamWriter streamWriter = new StreamWriter(new MemoryStream()))
				{
					streamWriter.Write(value);
					streamWriter.Flush();
					rp = XtraReport.FromStream(streamWriter.BaseStream);
				}
				XtraReportViewer(rp, lstParam);
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public static async Task PrintLayout(string docEntry, string formLayoutCode, Form frm_Parent = null, bool showPrintButton = true, bool showExportButton = true)
	{
		_ = 1;
		try
		{
			if (string.IsNullOrEmpty(docEntry))
			{
				return;
			}
			Apzon.Entities.Models.Administrator.PrintLayout printLayout = await GetDefaultPrintLayout(GlobalData.UserName, formLayoutCode, "");
			if (printLayout != null && !string.IsNullOrEmpty(printLayout.FileName))
			{
				List<Common.ParameterValue> lstParam = new List<Common.ParameterValue>
				{
					new Common.ParameterValue
					{
						Name = "DOCKEY@",
						Value = docEntry
					}
				};
				string text = await ReportViewer(printLayout.FileName, lstParam, frm_Parent, showPrintButton, showExportButton);
				if (!string.IsNullOrEmpty(text))
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	internal static DataTable CopyChildItemsInBom(string itemCode, int baseType, int baseEntry, int baseRow)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BillOfMasterials");
			return httpClientHelper.Get("CopyChildItemsInBom", new { itemCode, baseType, baseEntry, baseRow });
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	public static async Task Print(string cardCode, int docEntry, string formLayoutCode, string objectId = "")
	{
		_ = 1;
		try
		{
			if (docEntry == 0)
			{
				return;
			}
			Apzon.Entities.Models.Administrator.PrintLayout printLayout = await GetDefaultPrintLayout(GlobalData.UserName, formLayoutCode, Function.ToString(cardCode));
			List<Common.ParameterValue> lstParam = new List<Common.ParameterValue>
			{
				new Common.ParameterValue
				{
					Name = "DOCKEY@",
					Value = docEntry.ToString()
				}
			};
			if (printLayout != null && !string.IsNullOrEmpty(printLayout.FileName))
			{
				string text = await PrintToPrinter(printLayout.FileName, lstParam, objectId);
				if (!string.IsNullOrEmpty(text))
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
			else if (printLayout != null && !string.IsNullOrEmpty(printLayout.XtraText))
			{
				WinCommons._OpenProcessing();
				string value = DecryptStringAES(printLayout.XtraText, "APZ_RDOC");
				XtraReport rp;
				using (StreamWriter streamWriter = new StreamWriter(new MemoryStream()))
				{
					streamWriter.Write(value);
					streamWriter.Flush();
					rp = XtraReport.FromStream(streamWriter.BaseStream);
				}
				XtraReportViewer(rp, lstParam, isPrint: true);
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public static async Task PrintPreferences(ApzObjectTypes objType, int docEntry, string formLayoutCode, string objectId = "")
	{
		_ = 2;
		try
		{
			DataTable dataTable = await HttpClientHelperFactory.GetHttpClient().Commons.GetPrintPreferences(objType, GlobalData.UserName, GlobalData.DefaultBranch);
			if (!dataTable.IsNotNull())
			{
				return;
			}
			string printOnAdd = Function.ToString(dataTable.Rows[0]["PrintOnAdd"]);
			string exportOnAdd = Function.ToString(dataTable.Rows[0]["ExprtOnAdd"]);
			string pdfOnAdd = Function.ToString(dataTable.Rows[0]["PDFOnAdd"]);
			int copies = Function.ParseInt(dataTable.Rows[0]["Copies"]);
			CrystalDecisions.CrystalReports.Engine.ReportDocument _rp = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
			if (!printOnAdd.Equals("Y") && !exportOnAdd.Equals("Y") && !pdfOnAdd.Equals("Y"))
			{
				return;
			}
			Apzon.Entities.Models.Administrator.PrintLayout printLayout = await GetDefaultPrintLayout(GlobalData.UserName, formLayoutCode, "");
			if (printLayout != null && !string.IsNullOrEmpty(printLayout.FileName))
			{
				List<Common.ParameterValue> lstDataParam = new List<Common.ParameterValue>
				{
					new Common.ParameterValue
					{
						Name = "DOCKEY@",
						Value = docEntry.ToString()
					}
				};
				_rp = await ReportToPrinter(Application.StartupPath + "\\Report\\" + GlobalData.DatabaseName + "\\" + Path.GetFileName(printLayout.FileName), lstDataParam, objectId);
			}
			if (printOnAdd.Equals("Y"))
			{
				_rp?.PrintToPrinter(copies, collated: true, 1, _rp.FormatEngine.GetLastPageNumber(new ReportPageRequestContext()));
			}
			if (exportOnAdd.Equals("Y") && _rp != null)
			{
				string text = ExtensionMethods.PathSaveFile("Word Document", Enums.ExportExt.Doc);
				if (!string.IsNullOrEmpty(text))
				{
					_rp.ExportToDisk(ExportFormatType.WordForWindows, text);
				}
			}
			if (pdfOnAdd.Equals("Y") && _rp != null)
			{
				string text2 = ExtensionMethods.PathSaveFile("PDF", Enums.ExportExt.Pdf);
				if (!string.IsNullOrEmpty(text2))
				{
					_rp.ExportToDisk(ExportFormatType.PortableDocFormat, text2);
				}
			}
			foreach (CrystalDecisions.CrystalReports.Engine.Table table in _rp.Database.Tables)
			{
				table.Dispose();
			}
			_rp.Database.Dispose();
			_rp.Close();
			_rp.Dispose();
			GC.Collect();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	internal static async Task<CrystalDecisions.CrystalReports.Engine.ReportDocument> ReportToPrinter(string pathFile, List<Common.ParameterValue> lstDataParam, string objectId = "")
	{
		_ = 2;
		try
		{
			CrystalDecisions.CrystalReports.Engine.ReportDocument _rp = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
			if (!FileIO.FileExist(pathFile))
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Không tìm thấy file báo cáo: " + pathFile);
				WinCommons.Error(Enums.MessageCode.ReportFileNotFoundWithPath, "", pathFile);
				return null;
			}
			_rp.Load(pathFile);
			_rp.Refresh();
			ParameterFields parameterFields = _rp.ParameterFields;
			if (parameterFields != null)
			{
				ParameterFields parameterFields2 = new ParameterFields();
				foreach (CrystalDecisions.Shared.ParameterField item in parameterFields)
				{
					if (!string.IsNullOrEmpty(item.ReportName))
					{
						continue;
					}
					string paraName = item.Name.ToLower();
					if (!lstDataParam.Exists((Common.ParameterValue t) => t.Name.ToLower().Equals(paraName)))
					{
						if (paraName.Equals("printbyusername@"))
						{
							lstDataParam.Add(new Common.ParameterValue(item.Name, GlobalData.UserNameLogin));
						}
						else if (paraName.Equals("usercode@"))
						{
							lstDataParam.Add(new Common.ParameterValue(item.Name, GlobalData.UserName));
						}
						else if (paraName.Equals("objectid@"))
						{
							lstDataParam.Add(new Common.ParameterValue(item.Name, objectId));
						}
						else if (paraName.Equals("schema@"))
						{
							lstDataParam.Add(new Common.ParameterValue(item.Name, GlobalData.DatabaseName));
						}
						else if (!lstDataParam.Any((Common.ParameterValue m) => m.Name.ToLower().Equals(paraName)))
						{
							parameterFields2.Add(item);
						}
					}
				}
				if (parameterFields2.Count > 0)
				{
					frm_SelectionCriteria frm_SelectionCriteria2 = new frm_SelectionCriteria
					{
						lstPara = parameterFields2
					};
					frm_SelectionCriteria2.ShowDialog();
					if (frm_SelectionCriteria2.isUserEdit)
					{
						foreach (Common.ParameterValue item2 in frm_SelectionCriteria2.lstParamValue)
						{
							lstDataParam.Add(item2);
						}
					}
				}
			}
			List<Common.ParameterValue> lstEndSetParamater = new List<Common.ParameterValue>();
			if (parameterFields != null && parameterFields.Count > 0)
			{
				foreach (CrystalDecisions.Shared.ParameterField para in parameterFields)
				{
					if (string.IsNullOrEmpty(para.ReportName))
					{
						Common.ParameterValue parameterValue = lstDataParam.Where((Common.ParameterValue m) => m.Name.ToLower().Equals(para.Name.ToLower())).FirstOrDefault();
						lstEndSetParamater.Add(new Common.ParameterValue
						{
							Name = para.Name,
							Value = parameterValue?.Value
						});
						if (parameterValue != null)
						{
							_rp.SetParameterValue(para.Name, parameterValue.Value);
						}
					}
				}
			}
			ISCDReportClientDocument rptClientDoc = _rp.ReportClientDocument;
			string commandText = "";
			string text = "T";
			foreach (CrystalDecisions.ReportAppServer.DataDefModel.Table table in rptClientDoc.DatabaseController.Database.Tables)
			{
				if (table is CommandTable commandTable)
				{
					commandText = commandTable.CommandText;
					break;
				}
				if (table is Procedure procedure)
				{
					text = "S";
					commandText = procedure.Name;
					break;
				}
			}
			new DataTable();
			IHttpClient httpclient = HttpClientHelperFactory.GetHttpClient();
			DataTable dt;
			if (text.Equals("T"))
			{
				foreach (object parameterField in _rp.DataDefinition.ParameterFields)
				{
					if (!(parameterField is ParameterFieldDefinition parameterFieldDefinition))
					{
						continue;
					}
					ParameterValueKind parameterValueKind = parameterFieldDefinition.ParameterValueKind;
					string prName = parameterFieldDefinition.ParameterFieldName;
					Common.ParameterValue parameterValue2 = lstDataParam.FirstOrDefault((Common.ParameterValue t) => t.Name.ToUpper().Equals(prName.ToUpper()));
					if (parameterValue2 != null)
					{
						if (parameterValue2.Name.ToLower().Equals("schema@"))
						{
							commandText = commandText.Replace(parameterFieldDefinition.FormulaName + ".", " " + Function.ToString(parameterValue2.Value) + ".");
							commandText = commandText.Replace("\"" + parameterFieldDefinition.FormulaName + "\".", " \"" + Function.ToString(parameterValue2.Value) + "\".");
						}
						string newValue = "";
						switch (parameterValueKind)
						{
						case ParameterValueKind.NumberParameter:
							newValue = (parameterFieldDefinition.EnableAllowMultipleValue ? ("(" + string.Join(",", Function.ParseDecimal(parameterValue2.Value).ToString().Split(',')) + ")") : (" " + Function.ParseDecimal(parameterValue2.Value)));
							break;
						case ParameterValueKind.DateParameter:
						case ParameterValueKind.DateTimeParameter:
							newValue = ((!Function.ParseDateTime(parameterValue2.Value).HasValue) ? " ''" : (" '" + Convert.ToDateTime(parameterValue2.Value).ToString("yyyy-MM-dd") + "'"));
							break;
						case ParameterValueKind.CurrencyParameter:
						case ParameterValueKind.StringParameter:
							parameterValue2.Value = (Function.ToString(parameterValue2.Value).Contains("'") ? Function.ToString(parameterValue2.Value).Replace("'", "") : Function.ToString(parameterValue2.Value));
							newValue = (parameterFieldDefinition.EnableAllowMultipleValue ? ("('" + string.Join("','", Function.ToString(parameterValue2.Value).Split(',')) + "')") : (" '" + Function.ToString(parameterValue2.Value) + "'"));
							break;
						default:
							commandText = commandText.Replace(parameterFieldDefinition.FormulaName, " " + Function.ToString(parameterValue2.Value));
							break;
						case ParameterValueKind.BooleanParameter:
							break;
						}
						commandText = commandText.Replace("\"" + parameterFieldDefinition.FormulaName + "\"", newValue);
						commandText = commandText.Replace("'" + parameterFieldDefinition.FormulaName + "'", newValue);
						commandText = commandText.Replace(parameterFieldDefinition.FormulaName, newValue);
					}
				}
				DataTableResult dataTableResult = await httpclient.Commons.ExecuteQuery(commandText);
				if (!string.IsNullOrEmpty(dataTableResult.Message))
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), dataTableResult.Message, commandText);
					WinCommons.Error(Enums.MessageCode.None, dataTableResult.Message);
					return null;
				}
				dt = dataTableResult.Data;
			}
			else
			{
				List<Common.ParameterValue> list = new List<Common.ParameterValue>();
				foreach (object parameterField2 in _rp.DataDefinition.ParameterFields)
				{
					if (parameterField2 is ParameterFieldDefinition)
					{
						ParameterFieldDefinition parameterFieldDefinition2 = (ParameterFieldDefinition)parameterField2;
						_ = parameterFieldDefinition2.ParameterValueKind;
						string prName2 = parameterFieldDefinition2.ParameterFieldName;
						Common.ParameterValue parameterValue3 = lstDataParam.FirstOrDefault((Common.ParameterValue t) => t.Name.ToUpper().Equals(prName2.ToUpper()));
						if (parameterValue3 != null)
						{
							list.Add(new Common.ParameterValue
							{
								Name = string.Format("@{0}", parameterFieldDefinition2.ParameterFieldName.Replace("@", "")),
								Value = parameterValue3.Value
							});
						}
					}
				}
				DataSetResult dataSetResult = await httpclient.Commons.ExecuteStore(commandText, list);
				if (!string.IsNullOrEmpty(dataSetResult.Message) || dataSetResult.Data == null || dataSetResult.Data.Tables.Count <= 0)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), dataSetResult.Message, commandText);
					WinCommons.Error(Enums.MessageCode.None, dataSetResult.Message);
					return null;
				}
				dt = dataSetResult.Data.Tables[0];
			}
			_ = _rp.ReportDefinition.Sections;
			DataDefController dataDefController = rptClientDoc.DataDefController;
			CrystalDecisions.ReportAppServer.DataDefModel.Database boDatabase = dataDefController.Database;
			int k = 0;
			foreach (string subreportName in rptClientDoc.SubreportController.GetSubreportNames())
			{
				string sql = "";
				SubreportLinks subLinks = rptClientDoc.SubreportController.GetSubreportLinks(subreportName);
				SubreportClientDocument subreport = rptClientDoc.SubreportController.GetSubreport(subreportName);
				for (int num = 0; num < boDatabase.Tables.Count; num++)
				{
					try
					{
						if (boDatabase.Tables[num].ClassName == "CrystalReports.Table")
						{
							foreach (CrystalDecisions.ReportAppServer.DataDefModel.Table table2 in subreport.DatabaseController.Database.Tables)
							{
								if (table2 is CommandTable commandTable2)
								{
									sql = commandTable2.CommandText;
									break;
								}
							}
							continue;
						}
						CommandTable commandTable3 = (CommandTable)subreport.DatabaseController.Database.Tables[num];
						sql = commandTable3.CommandText;
					}
					catch (Exception)
					{
						continue;
					}
					break;
				}
				for (int i = 0; i < dt.Rows.Count; i++)
				{
					string text3 = sql;
					foreach (SubreportLink link in subLinks)
					{
						Common.ParameterValue parameterValue4 = lstDataParam.FirstOrDefault((Common.ParameterValue t) => "{?" + Function.ToString(t.Name.ToLower()) + "}" == Function.ToString(link.MainReportFieldName.ToLower()));
						if (parameterValue4 != null)
						{
							string newValue2 = parameterValue4.Value.ToString();
							text3 = text3.Replace(link.LinkedParameterName, newValue2);
							continue;
						}
						string columnName = link.MainReportFieldName.Replace("}", "").Split('.').Last();
						string newValue3 = Function.ToString(dt.Rows[i][columnName]);
						text3 = text3.Replace(link.LinkedParameterName, newValue3);
					}
					DataTableResult dataTableResult2 = await httpclient.Commons.ExecuteQuery(text3);
					if (string.IsNullOrEmpty(dataTableResult2.Message))
					{
						_rp.Subreports[k].SetDataSource(dataTableResult2.Data.Copy());
					}
				}
				k++;
			}
			_rp.SetDataSource(dt);
			if (lstEndSetParamater != null && lstEndSetParamater.Count > 0)
			{
				foreach (Common.ParameterValue item3 in lstEndSetParamater)
				{
					_rp.SetParameterValue(item3.Name, item3.Value);
				}
			}
			return _rp;
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex2.Message);
		}
		return null;
	}

	internal static async Task<string> PrintToPrinter(string fileName, List<Common.ParameterValue> lstParam, string objectId = "", int nCopies = 1, PrinterSettings printerSettings = null, PageSettings pageSettings = null)
	{
		try
		{
			string pathFile = Application.StartupPath + "\\Report\\" + GlobalData.DatabaseName + "\\" + Path.GetFileName(fileName);
			new CrystalDecisions.CrystalReports.Engine.ReportDocument();
			CrystalDecisions.CrystalReports.Engine.ReportDocument reportDocument = await ReportToPrinter(pathFile, lstParam, objectId);
			reportDocument.PrintToPrinter(nCopies, collated: true, 1, reportDocument.FormatEngine.GetLastPageNumber(new ReportPageRequestContext()));
			foreach (CrystalDecisions.CrystalReports.Engine.Table table in reportDocument.Database.Tables)
			{
				table.Dispose();
			}
			reportDocument.Database.Dispose();
			reportDocument.Close();
			reportDocument.Dispose();
			GC.Collect();
			return null;
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	internal static async Task<string> ReportViewer(string fileName, List<Common.ParameterValue> lstParam, Form frm_Parent = null, bool showPrintButton = true, bool showExportButton = true, string objectId = "")
	{
		try
		{
			string pathFile = Application.StartupPath + "\\Report\\" + GlobalData.DatabaseName + "\\" + Path.GetFileName(fileName);
			frm_ReportViewer _frm = new frm_ReportViewer
			{
				_ShowPrintButton = showPrintButton,
				_ShowExportButton = showExportButton
			};
			new CrystalDecisions.CrystalReports.Engine.ReportDocument();
			CrystalDecisions.CrystalReports.Engine.ReportDocument reportDocument = await ReportToPrinter(pathFile, lstParam, objectId);
			if (reportDocument == null)
			{
				return null;
			}
			_frm._rp = reportDocument;
			if (frm_Parent != null)
			{
				_frm.MdiParent = frm_Parent.MdiParent;
				_frm.MinimizeBox = true;
				_frm.Show();
			}
			else
			{
				_frm.ShowDialog();
			}
			return null;
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	internal static async Task<string> ReportViewerDatarowParam(string pathFile, DataRow dr, Form frm_Parent = null, bool showPrintButton = true, bool showExportButton = true)
	{
		try
		{
			frm_ReportViewer _frm = new frm_ReportViewer
			{
				_ShowPrintButton = showPrintButton,
				_ShowExportButton = showExportButton
			};
			CrystalDecisions.CrystalReports.Engine.ReportDocument reportDocument = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
			if (!FileIO.FileExist(pathFile))
			{
				return "Report file not found: " + pathFile;
			}
			reportDocument.Load(pathFile);
			reportDocument.Refresh();
			List<Common.ParameterValue> list = new List<Common.ParameterValue>();
			ParameterFields parameterFields = reportDocument.ParameterFields;
			if (parameterFields != null && parameterFields.Count > 0)
			{
				foreach (CrystalDecisions.Shared.ParameterField item in parameterFields)
				{
					if (string.IsNullOrEmpty(item.ReportName) && dr.IsExistsColumn(item.Name))
					{
						list.Add(new Common.ParameterValue(item.Name, dr[item.Name]));
					}
				}
			}
			reportDocument = await ReportToPrinter(pathFile, list);
			if (reportDocument == null)
			{
				return null;
			}
			_frm._rp = reportDocument;
			if (frm_Parent != null)
			{
				_frm.MdiParent = frm_Parent.MdiParent;
				_frm.MinimizeBox = true;
				_frm.Show();
			}
			else
			{
				_frm.ShowDialog();
			}
			return null;
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	internal static async Task<string> ReportViewerListParam(string pathFile, List<Common.ParameterValue> lstParam, string objectId = "", bool showPrintButton = true, bool showExportButton = true)
	{
		try
		{
			frm_ReportViewer _frm = new frm_ReportViewer
			{
				_ShowPrintButton = showPrintButton,
				_ShowExportButton = showExportButton
			};
			new CrystalDecisions.CrystalReports.Engine.ReportDocument();
			CrystalDecisions.CrystalReports.Engine.ReportDocument reportDocument = await ReportToPrinter(pathFile, lstParam, objectId);
			if (reportDocument == null)
			{
				return null;
			}
			_frm._rp = reportDocument;
			_frm.ShowDialog();
			return null;
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	internal static string ReportViewerDataSource(string fileName, DataTable dt, List<Common.ParameterValue> lstParam = null, Form frm_Parent = null, bool showPrintButton = true, bool showExportButton = true)
	{
		try
		{
			fileName = Application.StartupPath + "\\Report\\" + GlobalData.DatabaseName + "\\" + Path.GetFileName(fileName);
			frm_ReportViewer frm_ReportViewer2 = new frm_ReportViewer();
			frm_ReportViewer2._ShowPrintButton = showPrintButton;
			frm_ReportViewer2._ShowExportButton = showExportButton;
			CrystalDecisions.CrystalReports.Engine.ReportDocument reportDocument = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
			if (!FileIO.FileExist(fileName))
			{
				return "Không tìm thấy file báo cáo: " + Path.GetFileName(fileName);
			}
			reportDocument.Load(fileName);
			reportDocument.Refresh();
			if (lstParam != null && lstParam.Count > 0)
			{
				foreach (Common.ParameterValue item in lstParam)
				{
					reportDocument.SetParameterValue(item.Name, item.Value);
				}
			}
			frm_ReportViewer2.IsDataSet = true;
			reportDocument.SetDataSource(dt);
			frm_ReportViewer2._rp = reportDocument;
			if (frm_Parent != null)
			{
				frm_ReportViewer2.MdiParent = frm_Parent.MdiParent;
				frm_ReportViewer2.MinimizeBox = true;
				frm_ReportViewer2.Show();
			}
			else
			{
				frm_ReportViewer2.ShowDialog();
			}
			return null;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	internal static async void XtraReportViewer(XtraReport rp, List<Common.ParameterValue> lstParam, bool isPrint = false)
	{
		try
		{
			SqlQuery sqlQuery = (rp.DataSource as SqlDataSource).Queries.FirstOrDefault((SqlQuery m) => m.Name.Equals(rp.DataMember));
			if (sqlQuery == null || Function.ToString(sqlQuery.GetType()) == "DevExpress.DataAccess.Sql.SelectQuery")
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Cannot Execute XtraReport Query!");
				WinCommons.Warning(Enums.MessageCode.CannotExecuteXtraReportQuery, "");
				return;
			}
			string storedProcName = ((StoredProcQuery)sqlQuery).StoredProcName;
			List<Common.ParameterValue> list = new List<Common.ParameterValue>();
			if (sqlQuery.Parameters.Count > 0)
			{
				int i;
				for (i = 0; i < lstParam.Count; i++)
				{
					QueryParameter queryParameter = sqlQuery.Parameters.FirstOrDefault((QueryParameter m) => m.Value.ToString().Contains(lstParam[i].Name));
					if (queryParameter != null)
					{
						Common.ParameterValue parameterValue = new Common.ParameterValue();
						parameterValue.Name = queryParameter.Name;
						parameterValue.Value = lstParam[i].Value;
						list.Add(parameterValue);
					}
					else if (sqlQuery.Parameters.Count > i)
					{
						Common.ParameterValue parameterValue2 = new Common.ParameterValue();
						parameterValue2.Name = sqlQuery.Parameters[i].Name;
						parameterValue2.Value = lstParam[i].Value;
						list.Add(parameterValue2);
						rp.Parameters[i].Value = lstParam[i].Value;
					}
				}
			}
			foreach (DevExpress.XtraReports.Parameters.Parameter parameter in rp.Parameters)
			{
				parameter.Visible = false;
			}
			DataSetResult dataSetResult = await HttpClientHelperFactory.GetHttpClient().Commons.ExecuteStore(storedProcName, list);
			if (!string.IsNullOrEmpty(dataSetResult.Message))
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), dataSetResult.Message);
				WinCommons.Warning(Enums.MessageCode.None, dataSetResult.Message);
				return;
			}
			System.Data.DataSet data = dataSetResult.Data;
			data.DataSetName = "ds";
			if (data.IsNotNull() && data.Tables.Count > 0)
			{
				rp.DataSource = data.Tables[0];
			}
			else
			{
				rp.DataSource = data;
			}
			rp.DataMember = "ds";
			rp.RequestParameters = false;
			ReportPrintTool reportPrintTool = new ReportPrintTool(rp)
			{
				AutoShowParametersPanel = false
			};
			if (isPrint)
			{
				reportPrintTool.PrintDialog();
			}
			else
			{
				reportPrintTool.ShowPreviewDialog();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	internal static string XtraReportViewerManualParam(XtraReport rp, string txtReport = "", string pathFile = "")
	{
		try
		{
			if (rp.Parameters.Count > 0)
			{
				frm_XtraParameterSubmit frm_XtraParameterSubmit2 = new frm_XtraParameterSubmit();
				frm_XtraParameterSubmit2.lstPara = rp.Parameters;
				frm_XtraParameterSubmit2._txtReport = txtReport;
				frm_XtraParameterSubmit2._pathFile = pathFile;
				frm_XtraParameterSubmit2.MdiParent = Form.ActiveForm;
				frm_XtraParameterSubmit2.Show();
			}
			else
			{
				XtraReportViewer(rp, new List<Common.ParameterValue>());
			}
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
		return "";
	}

	internal static void PreviewExcelMerge(string _fileName, System.Data.DataSet data)
	{
		try
		{
			string path = Application.StartupPath + "\\Report\\" + GlobalData.DatabaseName + "\\" + _fileName;
			if (!Path.GetExtension(path).ToLower().Contains("xls") || !File.Exists(path))
			{
				return;
			}
			WinCommons._OpenProcessing();
			Workbook workbook = new Workbook();
			using (FileStream stream = new FileStream(path, FileMode.Open))
			{
				workbook.LoadDocument(stream, DevExpress.Spreadsheet.DocumentFormat.Xlsx);
			}
			if (data.IsNotNull() && data.Tables.Count > 1 && data.Tables[1].IsExistsColumn("DocEntry") && data.Tables[0].IsNotNull() && data.Tables[1].IsNotNull())
			{
				int num = Function.ParseInt(data.Tables[1].Rows[0]["DocEntry"]);
				DataRow[] array = data.Tables[0].Select("DocEntry <> " + num);
				foreach (DataRow dataRow in array)
				{
					if (Function.ParseInt(dataRow["DocEntry"]) != num)
					{
						data.Tables[0].Rows.Remove(dataRow);
					}
				}
				_ = data.Tables[0].Columns["DocEntry"];
				_ = data.Tables[1].Columns["DocEntry"];
				data.Relations.Add("DocumentLines", data.Tables[0].Columns["DocEntry"], data.Tables[1].Columns["DocEntry"]);
				workbook.MailMergeDataSource = data.Tables[0];
			}
			else
			{
				workbook.MailMergeDataSource = data;
			}
			foreach (IWorkbook item in workbook.GenerateMailMergeDocuments())
			{
				foreach (Worksheet worksheet in item.Worksheets)
				{
					if (!item.Worksheets.Contains(worksheet.Name))
					{
						item.Worksheets.Add(worksheet.Name);
						item.Worksheets[worksheet.Name].CopyFrom(worksheet);
					}
				}
				PrintableComponentLink printableComponentLink = new PrintableComponentLink(new PrintingSystem());
				printableComponentLink.Component = item;
				printableComponentLink.CreateDocument();
				printableComponentLink.ShowPreview();
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	internal static async Task<string> UpdatePrintedLayout(string docentry, string table, string type)
	{
		try
		{
			using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "PrintLayout");
			return await helper.GetAsync("UpdatePrinted", new
			{
				docentry = docentry,
				table = table,
				type = type,
				userName = GlobalData.UserName
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public static void UpdateReportFile(string folder = "Report")
	{
		try
		{
			if (GlobalData.FtpConfig == null || string.IsNullOrEmpty(GlobalData.FtpConfig.Address))
			{
				Logging.Write("WATCH", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Not config ftp yet");
				UpdateReportApi(folder);
			}
			else if (!string.IsNullOrEmpty(GlobalData.FtpConfig.Method))
			{
				if ("net".Equals(GlobalData.FtpConfig.Method.ToLower()))
				{
					UpdateReportFileNetwork(folder);
				}
				else if ("ftp".Equals(GlobalData.FtpConfig.Method.ToLower()))
				{
					UpdateReportFtp();
				}
				else
				{
					UpdateReportApi(folder);
				}
			}
			else
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Update method is not valid");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private static async Task UpdateReportApi(string folder)
	{
		try
		{
			IHttpClient httpClient = HttpClientHelperFactory.GetHttpClient();
			List<Common.ListItems> list = httpClient.FileNetworkManagerClient.LoadReportModified(folder);
			List<Common.ListItems> list2 = LoadLocalReportModified(folder);
			List<Common.ListItems> list3 = new List<Common.ListItems>();
			List<Common.ListItems> downloaded = new List<Common.ListItems>();
			foreach (Common.ListItems item in list)
			{
				if (string.IsNullOrEmpty(item.Value))
				{
					continue;
				}
				List<Common.ListItems> list4 = new List<Common.ListItems>();
				if (list2.Count > 0)
				{
					list4 = list2.Where((Common.ListItems t) => t.Value != null && t.Value.ToUpper().Equals(item.Value.ToUpper())).ToList();
				}
				if (list4.Count <= 0)
				{
					list3.Add(item);
					continue;
				}
				list3.AddRange(list4.Where((Common.ListItems t) => !string.IsNullOrEmpty(t.Text) && !string.IsNullOrEmpty(item.Text) && Function.ParseDouble(item.Text) > Function.ParseDouble(t.Text)).ToList());
			}
			if (!Directory.Exists(Application.StartupPath + "\\" + folder + "\\" + GlobalData.DatabaseName))
			{
				Directory.CreateDirectory(Application.StartupPath + "\\" + folder + "\\" + GlobalData.DatabaseName);
			}
			foreach (Common.ListItems item2 in list3)
			{
				string text = await httpClient.FileNetworkManagerClient.DownLoadFile("\\" + folder + "\\" + item2.Value, Application.StartupPath + "\\" + folder + "\\" + GlobalData.DatabaseName, isOverride: true);
				if (string.IsNullOrEmpty(text))
				{
					downloaded.Add(item2);
					continue;
				}
				Logging.Write("ERROR", text);
			}
			if (downloaded.Count > 0)
			{
				SaveLocalReportModified(downloaded, folder);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private static void UpdateReportFtp()
	{
		List<Common.ListItems> list = LoadReportModified();
		List<Common.ListItems> source = LoadLocalReportModified();
		List<Common.ListItems> list2 = new List<Common.ListItems>();
		List<Common.ListItems> list3 = new List<Common.ListItems>();
		foreach (Common.ListItems item in list)
		{
			if (string.IsNullOrEmpty(item.Value))
			{
				continue;
			}
			List<Common.ListItems> list4 = source.Where((Common.ListItems t) => t.Value != null && t.Value.ToUpper().Equals(item.Value.ToUpper())).ToList();
			if (list4.Count <= 0)
			{
				list2.Add(item);
				continue;
			}
			list2.AddRange(list4.Where((Common.ListItems t) => !string.IsNullOrEmpty(t.Text) && !string.IsNullOrEmpty(item.Text) && Function.ParseDouble(item.Text) > Function.ParseDouble(t.Text)).ToList());
		}
		if (!Directory.Exists(Application.StartupPath + "\\Report\\" + GlobalData.DatabaseName))
		{
			Directory.CreateDirectory(Application.StartupPath + "\\Report\\" + GlobalData.DatabaseName);
		}
		foreach (Common.ListItems item2 in list2)
		{
			string text = DownLoadFileFromServer($"{GlobalData.FtpConfig.Address}/{GlobalData.FtpConfig.Folder}/Report/{item2.Value}", $"{Application.StartupPath}\\Report\\{item2.Value}", GlobalData.FtpConfig.User, GlobalData.FtpConfig.PassWord);
			if (string.IsNullOrEmpty(text))
			{
				list3.Add(item2);
				continue;
			}
			Logging.Write("ERROR", text);
		}
		if (list3.Count > 0)
		{
			SaveLocalReportModified(list3);
		}
	}

	public static List<Common.ListItems> LoadReportModified()
	{
		List<Common.ListItems> list = new List<Common.ListItems>();
		try
		{
			List<string> list2 = new List<string>();
			try
			{
				string text = $"{GlobalData.FtpConfig.Address}/{GlobalData.FtpConfig.Folder}/Report";
				FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(text);
				ftpWebRequest.Method = "NLST";
				ftpWebRequest.Credentials = new NetworkCredential(GlobalData.FtpConfig.User, GlobalData.FtpConfig.PassWord);
				ftpWebRequest.UsePassive = true;
				ftpWebRequest.UseBinary = true;
				ftpWebRequest.KeepAlive = false;
				FtpWebResponse ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
				using (Stream stream = ftpWebResponse.GetResponseStream())
				{
					if (stream != null)
					{
						StreamReader streamReader = new StreamReader(stream);
						while (!streamReader.EndOfStream)
						{
							Application.DoEvents();
							list2.Add(streamReader.ReadLine());
						}
						streamReader.Close();
					}
					stream?.Close();
				}
				ftpWebResponse.Close();
				foreach (string item in list2)
				{
					DateTime lastModifiedFtp = GetLastModifiedFtp($"{text}/{item}");
					list.Add(new Common.ListItems
					{
						Value = item,
						Text = lastModifiedFtp.ToString("yyyyMMddHHmmss")
					});
				}
			}
			catch (Exception)
			{
				XtraMessageBox.Show("There was an error connecting to the FTP Server");
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.ErrorLineNumber(ex2);
		}
		return list;
	}

	public static List<Common.ListItems> LoadLocalReportModified(string folder = "Report")
	{
		try
		{
			return (from t in XDocument.Load(Application.StartupPath + "\\" + folder + ".xml").Descendants("File")
				where t.Attribute("DbName") == null || Function.ToString(t.Attribute("DbName").Value) == GlobalData.DatabaseName
				select new Common.ListItems
				{
					Value = Function.ToString(t.Attribute("FileName").Value),
					Text = Function.ToString(t.Attribute("Modified").Value)
				}).ToList();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new List<Common.ListItems>();
	}

	internal static void SaveLocalReportModified(List<Common.ListItems> downloaded, string folder = "Report")
	{
		try
		{
			new XDocument(new XElement("Files", downloaded.Select((Common.ListItems t) => new XElement("File", new XAttribute("FileName", t.Value), new XAttribute("Modified", t.Text), new XAttribute("DbName", GlobalData.DatabaseName))))).Save(Application.StartupPath + "\\" + folder + ".xml");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public static DateTime GetLastModifiedFtp(string uri)
	{
		try
		{
			FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(uri);
			ftpWebRequest.Method = "MDTM";
			ftpWebRequest.Credentials = new NetworkCredential(GlobalData.FtpConfig.User, GlobalData.FtpConfig.PassWord);
			ftpWebRequest.UsePassive = false;
			ftpWebRequest.UseBinary = true;
			ftpWebRequest.KeepAlive = false;
			FtpWebResponse ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
			DateTime lastModified = ftpWebResponse.LastModified;
			ftpWebResponse.Close();
			ftpWebResponse.Dispose();
			return lastModified;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message, uri);
			WinCommons.ErrorLineNumber(ex);
		}
		return default;
	}

	public static void UpdateReportFileNetwork(string folder)
	{
		try
		{
			if (GlobalData.FtpConfig == null || string.IsNullOrEmpty(GlobalData.FtpConfig.Address))
			{
				WinCommons.Error(Enums.MessageCode.NotConfigFtpYet, "");
				return;
			}
			List<Common.ListItems> list = LoadReportModifiedNetwork();
			List<Common.ListItems> source = LoadLocalReportModified();
			List<Common.ListItems> list2 = new List<Common.ListItems>();
			List<Common.ListItems> list3 = new List<Common.ListItems>();
			foreach (Common.ListItems item in list)
			{
				if (string.IsNullOrEmpty(item.Value))
				{
					continue;
				}
				List<Common.ListItems> list4 = source.Where((Common.ListItems t) => t.Value != null && t.Value.ToUpper().Equals(item.Value.ToUpper())).ToList();
				if (list4.Count <= 0)
				{
					list2.Add(item);
					continue;
				}
				list2.AddRange(list4.Where((Common.ListItems t) => !string.IsNullOrEmpty(t.Text) && !string.IsNullOrEmpty(item.Text) && Function.ParseDouble(item.Text) > Function.ParseDouble(t.Text)).ToList());
			}
			if (!Directory.Exists(Application.StartupPath + "\\" + folder + "\\" + GlobalData.DatabaseName))
			{
				Directory.CreateDirectory(Application.StartupPath + "\\" + folder + "\\" + GlobalData.DatabaseName);
			}
			string text = "";
			if (!string.IsNullOrEmpty(GlobalData.FtpConfig.Address) && GlobalData.FtpConfig.Address.Length > 5 && GlobalData.FtpConfig.Address.StartsWith("ftp://"))
			{
				text = GlobalData.FtpConfig.Address.Substring(6, GlobalData.FtpConfig.Address.Length - 6);
			}
			string text2 = string.Format("\\\\{0}\\{1}\\" + folder + "\\", text, GlobalData.FtpConfig.Folder);
			string text3 = Application.StartupPath + "\\" + folder + "\\";
			try
			{
				using (NetworkShareAccesser.Access(text, GlobalData.FtpConfig.User, GlobalData.FtpConfig.PassWord))
				{
					foreach (Common.ListItems item2 in list2)
					{
						if (FileIO.CopyFile(text2 + item2.Value, text3 + item2.Value, blnOverWrite: true))
						{
							list3.Add(item2);
						}
					}
				}
			}
			catch (Exception ex)
			{
				if (ex.Message.StartsWith("Multiple connections"))
				{
					foreach (Common.ListItems item3 in list2)
					{
						if (FileIO.CopyFile(text2 + item3.Value, text3 + item3.Value, blnOverWrite: true))
						{
							list3.Add(item3);
						}
					}
				}
			}
			if (list3.Count > 0)
			{
				SaveLocalReportModified(list3);
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.ErrorLineNumber(ex2);
		}
	}

	public static List<Common.ListItems> LoadReportModifiedNetwork()
	{
		List<Common.ListItems> list = new List<Common.ListItems>();
		try
		{
			new List<string>();
			string text = "";
			if (!string.IsNullOrEmpty(GlobalData.FtpConfig.Address) && GlobalData.FtpConfig.Address.Length > 5 && GlobalData.FtpConfig.Address.StartsWith("ftp://"))
			{
				text = GlobalData.FtpConfig.Address.Substring(6, GlobalData.FtpConfig.Address.Length - 6);
			}
			string text2 = $"\\\\{text}\\{GlobalData.FtpConfig.Folder}\\Report";
			try
			{
				using (NetworkShareAccesser.Access(text, GlobalData.FtpConfig.User, GlobalData.FtpConfig.PassWord))
				{
					if (!FileIO.FolderExist(text2))
					{
						return list;
					}
					string[] files = Directory.GetFiles(text2);
					if (files == null || files.Length == 0)
					{
						return list;
					}
					string[] array = files;
					foreach (string text3 in array)
					{
						if (FileIO.FileExist(text3))
						{
							FileInfo fileInfo = new FileInfo(text3);
							list.Add(new Common.ListItems
							{
								Value = fileInfo.Name,
								Text = fileInfo.LastWriteTime.ToString("yyyyMMddHHmmss")
							});
						}
					}
				}
			}
			catch (Exception ex)
			{
				if (ex.Message.StartsWith("Multiple connections"))
				{
					if (!FileIO.FolderExist(text2))
					{
						return list;
					}
					string[] files2 = Directory.GetFiles(text2);
					if (files2 == null || files2.Length == 0)
					{
						return list;
					}
					string[] array = files2;
					foreach (string arg in array)
					{
						string text4 = $"{text2}\\{arg}";
						if (FileIO.FileExist(text4))
						{
							FileInfo fileInfo2 = new FileInfo(text4);
							list.Add(new Common.ListItems
							{
								Value = fileInfo2.Name,
								Text = fileInfo2.LastWriteTime.ToString("yyyyMMddHHmmss")
							});
						}
					}
				}
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.ErrorLineNumber(ex2);
		}
		return list;
	}

	public static void DataRowToDataRow(DataRow fromRow, DataRow toRow)
	{
		try
		{
			foreach (object column in fromRow.Table.Columns)
			{
				if (toRow.IsExistsColumn(column.ToString()))
				{
					toRow[column.ToString()] = fromRow[column.ToString()];
				}
				else if (toRow.IsExistsColumn("U_" + column.ToString()))
				{
					toRow["U_" + column.ToString()] = fromRow[column.ToString()];
				}
				else if (toRow.IsExistsColumn(column.ToString().Replace("U_", "")))
				{
					toRow[column.ToString().Replace("U_", "")] = fromRow[column.ToString()];
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public static void DataRowToGridView(DataRow dr, GridViewEx view, int rowHandle)
	{
		try
		{
			foreach (GridColumnEx column in view.Columns)
			{
				if (dr.IsExistsColumn(column.FieldName))
				{
					view.SetRowCellValue(rowHandle, column, dr[column.FieldName]);
				}
				else if (dr.IsExistsColumn(column.FieldName.Replace("U_", "")))
				{
					view.SetRowCellValue(rowHandle, column, dr[column.FieldName.Replace("U_", "")]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public static void DataRowToTreeView(DataRow dr, TreeList tree)
	{
		try
		{
			TreeListNode focusedNode = tree.FocusedNode;
			foreach (TreeListColumn column in tree.Columns)
			{
				if (dr.IsExistsColumn(column.FieldName))
				{
					TreeNodeSetValue(focusedNode, column, dr[column.FieldName]);
				}
				else if (dr.IsExistsColumn(column.FieldName.Replace("U_", "")))
				{
					TreeNodeSetValue(focusedNode, column, dr[column.FieldName.Replace("U_", "")]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public static void TreeNodeSetValue(TreeListNode node, TreeListColumn column, object value)
	{
		TreeList treeList = node.TreeList;
		TreeListNode focusedNode = treeList.FocusedNode;
		TreeListColumn focusedColumn = treeList.FocusedColumn;
		try
		{
			treeList.BeginUpdate();
			treeList.FocusedNode = node;
			treeList.FocusedColumn = column;
			treeList.ShowEditor();
			if (treeList.ActiveEditor != null)
			{
				treeList.ActiveEditor.EditValue = value;
			}
			treeList.CloseEditor();
		}
		finally
		{
			treeList.EndUpdate();
			treeList.FocusedNode = focusedNode;
			treeList.FocusedColumn = focusedColumn;
		}
	}

	internal static Type GetTypeFromSql(string name)
	{
		try
		{
			switch (name.ToUpper())
			{
			case "INT":
				return typeof(int);
			case "NUMERIC":
				return typeof(double);
			case "SMALLINT":
				return typeof(short);
			case "NVARCHAR":
				return typeof(string);
			case "DATETIME":
				return typeof(DateTime);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return typeof(string);
	}

	public static object GetDefaultSqlValue(string dbtype)
	{
		switch (dbtype)
		{
		case "int":
		case "bigint":
		case "smallint":
		case "decimal":
		case "numeric":
		case "float":
		case "1":
		case "2":
		case "3":
		case "4":
		case "5":
		case "6":
		case "9":
		case "10":
		case "11":
			return 0;
		case "datetime":
		case "date":
		case "7":
			return new DateTime(1753, 1, 1);
		case "bit":
			return false;
		default:
			return string.Empty;
		}
	}

	internal static object GetDefaultFromType(Type type)
	{
		try
		{
			object result = null;
			if (type.IsValueType)
			{
				result = Activator.CreateInstance(type);
			}
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return null;
	}

	internal static PivotGridControlEx GetActivePivotGrid(Control control)
	{
		try
		{
			if (control != null)
			{
				if (control.GetType() == typeof(PivotGridControl) || control.GetType() == typeof(PivotGridControlEx))
				{
					return control as PivotGridControlEx;
				}
				if (control.Parent != null)
				{
					return GetActivePivotGrid(control.Parent);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return null;
	}

	internal static TreeEx GetActiveTreeList(Control control)
	{
		try
		{
			if (control != null)
			{
				if (control.GetType() == typeof(TreeEx))
				{
					return control as TreeEx;
				}
				if (control.Parent != null)
				{
					return GetActiveTreeList(control.Parent);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return null;
	}

	public static Tuple<bool, bool> CheckExistProperty(string propertyName, Control control)
	{
		try
		{
			bool flag = false;
			PropertyInfo[] properties = control.GetType().GetProperties();
			if (properties.Any((PropertyInfo x) => x.Name == propertyName))
			{
				flag = false;
				return Tuple.Create(item1: true, flag);
			}
			int num = properties.FindIndex((PropertyInfo x) => x.Name == "Properties");
			if (num == -1)
			{
				return Tuple.Create(item1: false, item2: false);
			}
			bool flag2 = properties[num].PropertyType.GetMember(propertyName).Any((MemberInfo x) => x.Name == propertyName);
			flag = true;
			if (flag2)
			{
				return Tuple.Create(item1: true, flag);
			}
			return Tuple.Create(item1: false, flag);
		}
		catch (Exception ex)
		{
			_ = ex.Message;
			return Tuple.Create(item1: false, item2: false);
		}
	}

	internal static void DisableAllControl(Control.ControlCollection Controls)
	{
		try
		{
			if (Controls == null)
			{
				return;
			}
			int count = Controls.Count;
			for (int i = 0; i < count; i++)
			{
				dynamic val = Controls[i];
				_ = (string)val.GetType().Name;
				_ = (string)val.Name;
				dynamic val2 = WinFuntions.CheckExistProperty("IsSearchControl", val);
				bool flag = val2.Item1;
				bool flag2 = val2.Item2;
				if (flag)
				{
					if (flag2)
					{
						if (val.Properties.IsSearchControl == true)
						{
							continue;
						}
					}
					else if (val.IsSearchControl == true)
					{
						continue;
					}
				}
				Control control = (Control)val;
				switch (getControlType(control))
				{
				case "SimpleButton":
					control.Enabled = false;
					break;
				case "CheckEdit":
					control.Enabled = false;
					break;
				case "RadioGroup":
					control.Enabled = false;
					break;
				case "TextEdit":
					((TextEdit)control).ReadOnly = true;
					break;
				case "ContextMenuStrip":
					((ContextMenuStrip)control).Enabled = false;
					break;
				case "Apzon.Controls.Grid.GridControlEx()":
					foreach (GridColumnEx column in ((GridViewEx)((GridControlEx)control).MainView).Columns)
					{
						column.OptionsColumn.AllowEdit = false;
						column.OptionsColumn.ReadOnly = true;
					}
					break;
				}
				if (control.Controls.Count > 0)
				{
					WinFuntions.DisableAllControl(val.Controls);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	internal static async Task SetBranchListControls(GridViewEx gv)
	{
		try
		{
			if (gv == null)
			{
				return;
			}
			RepositoryItemLookUpEdit look = new RepositoryItemLookUpEdit();
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			DataTable dataSource = await hp.GetAsync("GetBranchList", new
			{
				userCode = GlobalData.UserCodeLogin
			});
			look.Columns.Add(new LookUpColumnInfo("BPLName", "Name"));
			look.DisplayMember = "BPLName";
			look.ValueMember = "BPLId";
			look.DataSource = dataSource;
			look.NullText = "";
			look.EditValueChanged += (object s, EventArgs e) =>
			{
				try
				{
					if (s is LookUpEdit lookUpEdit)
					{
						int num = Function.ParseInt(lookUpEdit.EditValue);
						if (Function.ParseInt(num) >= 1)
						{
							DataTable dataTable = (DataTable)look.DataSource;
							if (dataTable.PrimaryKey == null || dataTable.PrimaryKey.Length == 0 || dataTable.PrimaryKey[0].ColumnName != "BPLId")
							{
								dataTable.PrimaryKey = new DataColumn[1] { dataTable.Columns["BPLId"] };
							}
							DataRow dataRow = dataTable.Rows.Find(num);
							if (dataRow != null)
							{
								object obj = dataRow["VATRegNum"];
								if (obj != DBNull.Value && obj != null && !string.IsNullOrWhiteSpace(obj.ToString()))
								{
									gv.SetFocusedRowCellValue("VATRegNum", obj);
								}
							}
						}
					}
				}
				catch (Exception ex2)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
					WinCommons.ErrorLineNumber(ex2);
				}
			};
			if (gv.Columns["BPLId"] != null)
			{
				gv.GridControl.RepositoryItems.Add(look);
				gv.Columns["BPLId"].ColumnEdit = look;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public static string GetTreeTypeText(BoItemTreeTypes _type)
	{
		try
		{
			return _type switch
			{
				BoItemTreeTypes.iNotATree => "Not a BOM", 
				BoItemTreeTypes.iAssemblyTree => "Assembly", 
				BoItemTreeTypes.iSalesTree => "Sales", 
				BoItemTreeTypes.iIngredient => "Sales BOM Component", 
				BoItemTreeTypes.iProductionTree => "Production", 
				BoItemTreeTypes.iTemplateTree => "Template", 
				_ => "Not a BOM", 
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
			return "Not a BOM";
		}
	}

	internal static ApzObjectTypes GetDocTypeByName(string docName)
	{
		try
		{
			ApzObjectTypes result = ApzObjectTypes.oSalesOpportunities;
			switch (docName)
			{
			case "Sale Quotations":
				result = ApzObjectTypes.oQuotations;
				break;
			case "Sales Orders":
				result = ApzObjectTypes.oOrders;
				break;
			case "Deliveries":
				result = ApzObjectTypes.oDeliveryNotes;
				break;
			case "A/R Invoices":
				result = ApzObjectTypes.oInvoices;
				break;
			case "Purchase Quotation":
				result = ApzObjectTypes.oPurchaseQuotations;
				break;
			case "Purchase Orders":
				result = ApzObjectTypes.oPurchaseOrders;
				break;
			case "Goods Receipt PO":
				result = ApzObjectTypes.oPurchaseDeliveryNotes;
				break;
			case "A/P Invoices":
				result = ApzObjectTypes.oQuotations;
				break;
			}
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return ApzObjectTypes.oSalesOpportunities;
		}
	}

	internal static string GetDocNameByType(int docType)
	{
		try
		{
			string result = "";
			switch (docType)
			{
			case 23:
				result = "Sale Quotations";
				break;
			case 17:
				result = "Sales Orders";
				break;
			case 15:
				result = "Deliveries";
				break;
			case 13:
				result = "A/R Invoices";
				break;
			case 540000006:
				result = "Purchase Quotation";
				break;
			case 22:
				result = "Purchase Orders";
				break;
			case 20:
				result = "Goods Receipt PO";
				break;
			case 18:
				result = "A/P Invoices";
				break;
			}
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return "";
		}
	}

	internal static async Task<DataRow> ChooseFormListCardCode(GridViewEx gvDoc, Enums.TypeCFL type)
	{
		try
		{
			DocumentTable listDoc = gvDoc.GridControl.DataSource as DocumentTable;
			int rowHandle = gvDoc.FocusedRowHandle;
			string txtSeach = Function.ToString(gvDoc.GetRowCellValue(rowHandle, "CardCode"));
			string value = "";
			using frm_CFL choose = new frm_CFL(type, false, txtSeach, "", Enums.ItemObjectType.NoItemType, null, "");
			choose.ShowDialog(gvDoc.GridControl.FindForm());
			if (listDoc != null && choose.ChooseItem != null)
			{
				DocumentRow items = listDoc[rowHandle];
				DataRow customer = choose.ChooseItem;
				if (customer == null)
				{
					return null;
				}
				items.CardCode = Function.ToString(customer["CardCode"]);
				items.CardName = Function.ToString(customer["CardName"]);
				items.ContactPersonName = Function.ToString(customer["CntctPrsn"]);
				items.ContactPersonCode = Function.ParseInt(customer["ContactPersonCode"]);
				items.SlpCode = Function.ParseInt(customer["SlpCode"]);
				items.DocDate = DateTime.Today;
				DataTable dataTable = await ExtenseionMethods.GetEmployeeCodeBySlpCode(Function.ParseInt(customer["SlpCode"]));
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					items.OwnerCode = Function.ParseInt(dataTable.Rows[0]["OwnerCode"]);
					items.OwnerName = Function.ToString(dataTable.Rows[0]["OwnerName"]);
				}
				listDoc.AcceptChanges();
				gvDoc.GridControl.DataSource = listDoc;
				gvDoc.GridControl.RefreshDataSource();
				if (string.IsNullOrEmpty(Function.ToString(customer["Currency"])))
				{
					value = GlobalData.MainCurrency;
				}
				else
				{
					value = ((!(Function.ToString(customer["Currency"]) == "##")) ? Function.ToString(customer["Currency"]) : GlobalData.MainCurrency);
				}
				gvDoc.SetRowCellValue(rowHandle, "Currency", Function.ToString(customer["Currency"]));
			}
			gvDoc.SetRowCellValue(rowHandle, "DocCur", value);
			return choose.ChooseItem;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return null;
		}
	}

	internal static void ChooseFromListItem(GridViewEx gvDoc, GridViewEx gvLine, ucBatchAndSerial batchSerial, string vatType, Enums.ItemObjectType itObjType = Enums.ItemObjectType.NoItemType, params string[] prms)
	{
		try
		{
			string txtSeach = Function.ToString(gvLine.GetFocusedRowCellValue("ItemCode"));
			DocumentRow byEntry = ((DocumentTable)gvDoc.GridControl.DataSource).GetByEntry(Function.ParseInt(gvDoc.GetFocusedRowCellValue("DocEntry")));
			if (byEntry == null)
			{
				return;
			}
			using frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListItems, multiSelect: true, txtSeach, vatType, itObjType, null, "", prms);
			frm_CFL2.ShowDialog(gvLine.GridControl.FindForm());
			if (frm_CFL2.ChooseItems == null || frm_CFL2.ChooseItems.Count == 0)
			{
				return;
			}
			DocumentLineTable documentLineTable = gvLine.GridControl.DataSource as DocumentLineTable;
			int focusedRowHandle = gvLine.FocusedRowHandle;
			int num = gvLine.FocusedRowHandle;
			foreach (DataRow chooseItem in frm_CFL2.ChooseItems)
			{
				DocumentLineRow newRow = documentLineTable.GetNewRow();
				if (num == focusedRowHandle)
				{
					DeleteBomChildsItem(gvLine, focusedRowHandle, batchSerial, isChooseItem: true);
				}
				newRow.DocEntry = byEntry.DocEntry;
				newRow.LineStatusName = "O";
				documentLineTable.Insert(num, newRow);
				gvLine.SetRowCellValue(num, "ItemCode", Function.ToString(chooseItem["ItemCode"]));
				gvLine.SetRowCellValue(num, "DocEntry", byEntry.DocEntry);
				gvLine.SetRowCellValue(num, "ShipDate", byEntry.DocDueDate);
				gvLine.SetRowCellValue(num, "UnitPrice", Function.ParseDouble(chooseItem["Price"]));
				if (vatType.ToUpper() == "O")
				{
					gvLine.SetRowCellValue(num, "VatGroup", Function.ToString(chooseItem["VatGourpSa"]));
					gvLine.SetRowCellValue(num, "TaxCode", Function.ToString(chooseItem["VatGourpSa"]));
					gvLine.SetRowCellValue(num, "RateVat", Function.ToString(chooseItem["RateSa"]));
				}
				else
				{
					gvLine.SetRowCellValue(num, "VatGroup", Function.ToString(chooseItem["VatGroupPu"]));
					gvLine.SetRowCellValue(num, "TaxCode", Function.ToString(chooseItem["VatGroupPu"]));
					gvLine.SetRowCellValue(num, "RateVat", Function.ToString(chooseItem["RatePu"]));
				}
				gvLine.SetRowCellValue(num, "Currency", byEntry.DocCurrency);
				gvLine.SetRowCellValue(num, "Quantity", 1);
				gvLine.SetRowCellValue(num, "OpenQty", 1);
				gvLine.SetRowCellValue(num, "LineStatus", "O");
				switch (itObjType)
				{
				case Enums.ItemObjectType.PurchaseItem:
					gvLine.SetRowCellValue(num, "unitMsr", Function.ToString(chooseItem["BuyUnitMsr"]));
					break;
				case Enums.ItemObjectType.SellItem:
					gvLine.SetRowCellValue(num, "unitMsr", Function.ToString(chooseItem["SalUnitMsr"]));
					break;
				default:
					gvLine.SetRowCellValue(num, "unitMsr", Function.ToString(chooseItem["CntUnitMsr"]));
					break;
				}
				gvLine.SetRowCellValue(num, "FrgnName", Function.ToString(chooseItem["FrgnName"]));
				documentLineTable.AcceptChanges();
				if (chooseItem.IsExistsColumn("Currency") && !string.IsNullOrEmpty(Function.ToString(chooseItem["Currency"])))
				{
					gvLine.SetRowCellValue(num, "Currency", Function.ToString(chooseItem["Currency"]));
				}
				else
				{
					gvLine.SetRowCellValue(num, "Currency", byEntry.DocCurrency);
					gvLine.SetRowCellValue(num, "Rate", (decimal)byEntry.DocRate);
				}
				gvLine.SetRowCellValue(num, "ShipDate", byEntry.DocDueDate);
				gvLine.SetRowCellValue(num, "Quantity", 1);
				gvLine.SetRowCellValue(num, "BOMTreeType", "");
				num++;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	internal static async Task<DataTable> CreateCloneTabl(string tableName)
	{
		try
		{
			DataTable dataTable = await HttpClientHelperFactory.GetHttpClient().Commons.GetTableStructure(tableName);
			if (dataTable.IsNotNull())
			{
				DataTable dataTable2 = new DataTable();
				foreach (DataRow row in dataTable.Rows)
				{
					if (!row.IsExistsColumn("COLUMN_NAME") || !row.IsExistsColumn("DATA_TYPE"))
					{
						continue;
					}
					string text = Function.ToString(row["DATA_TYPE"]);
					string text2 = Function.ToString(row["COLUMN_NAME"]);
					if (!string.IsNullOrEmpty(text2))
					{
						switch (text.ToLower())
						{
						case "datetime":
						case "date":
						case "datetime2":
						case "timestamp":
							dataTable2.Columns.Add(text2, typeof(DateTime));
							break;
						case "bit":
							dataTable2.Columns.Add(text2, typeof(bool));
							break;
						case "binary":
							dataTable2.Columns.Add(text2, typeof(byte));
							break;
						case "tinyint":
						case "int":
						case "smallint":
						case "bigint":
							dataTable2.Columns.Add(text2, typeof(int));
							break;
						case "decimal":
						case "numeric":
						case "smallmoney":
						case "money":
						case "float":
						case "real":
							dataTable2.Columns.Add(text2, typeof(double));
							break;
						default:
							dataTable2.Columns.Add(text2, typeof(string));
							break;
						}
					}
				}
				if (dataTable2.Columns.Count > 0)
				{
					DataRow dataRow2 = dataTable2.NewRow();
					foreach (DataColumn column in dataTable2.Columns)
					{
						if (column.DataType == typeof(DateTime))
						{
							dataRow2[column] = DateTime.Today;
						}
						else if (column.DataType == typeof(bool))
						{
							dataRow2[column] = false;
						}
						if (column.DataType == typeof(byte))
						{
							dataRow2[column] = 0;
						}
						if (column.DataType == typeof(int))
						{
							dataRow2[column] = 0;
						}
						if (column.DataType == typeof(double))
						{
							dataRow2[column] = 0;
						}
						if (column.DataType == typeof(string))
						{
							dataRow2[column] = string.Empty;
						}
					}
					dataTable2.Rows.Add(dataRow2);
				}
				return dataTable2;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	internal static async Task AddPriceListColumn(GridViewEx gvLine)
	{
		try
		{
			GridColumnEx gridColumnEx = new GridColumnEx();
			gridColumnEx.FieldName = "PriceListName";
			gridColumnEx.Caption = "PriceList";
			gridColumnEx.Visible = true;
			gridColumnEx.Width = 70;
			gridColumnEx.VisibleIndex = 4;
			RepositoryItemButtonEdit repositoryItemButtonEdit = new RepositoryItemButtonEdit();
			repositoryItemButtonEdit.Name = "rep_line_PriceList";
			if (repositoryItemButtonEdit.Buttons.Count > 0)
			{
				repositoryItemButtonEdit.Buttons.RemoveAt(0);
			}
			SerializableAppearanceObject appearance = new SerializableAppearanceObject();
			repositoryItemButtonEdit.Buttons.Add(new EditorButton(ButtonPredefines.Search));
			repositoryItemButtonEdit.Buttons.Add(new EditorButton(ButtonPredefines.Glyph, "", -1, enabled: true, visible: true, isLeft: true, ImageLocation.MiddleCenter, Resources.right, new KeyShortcut(Keys.None), appearance, "", null, null, enableImageTransparency: true));
			repositoryItemButtonEdit.ButtonPressed += (object s, ButtonPressedEventArgs e) =>
			{
				try
				{
					if (e.Button.Index == 0)
					{
						GlobalData._ItemCode = Function.ToString(gvLine.GetFocusedRowCellValue("ItemCode"));
						using frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.PriceList, multiSelect: false, "");
						frm_ChooseFromList2.ShowDialog(gvLine.GridControl.FindForm());
						if (frm_ChooseFromList2.ChooseItem != null)
						{
							DataRow row = ((DataRowView)frm_ChooseFromList2.ChooseItem).Row;
							gvLine.SetRowCellValue(gvLine.FocusedRowHandle, "PriceListName", Function.ToString(row["PriceListName"]));
							gvLine.SetRowCellValue(gvLine.FocusedRowHandle, "Price", Function.ParseDecimal(row["Price"]));
						}
						return;
					}
				}
				catch (Exception ex2)
				{
					WinCommons.ErrorLineNumber(ex2);
				}
			};
			gridColumnEx.ColumnEdit = repositoryItemButtonEdit;
			gvLine.Columns.Add(gridColumnEx);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	internal static async Task<DataTable> GetPriceList(string itmCode)
	{
		DataTable dt = new DataTable();
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "PriceList");
			dt = await helper.PostDataTableAsync("GetPriceList", itmCode);
			return dt;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return dt;
	}

	public static async Task<string> ShowFilterSetting(string filterObject)
	{
		try
		{
			if (GlobalData.FilterSelect != null)
			{
				Common.ListItems listItems = GlobalData.FilterSelect.FirstOrDefault((Common.ListItems t) => t.Text.Equals(filterObject));
				if (listItems != null)
				{
					return listItems.Value;
				}
			}
			DataTable dataTable = await HttpClientHelperFactory.GetHttpClient().FilterSetting.GetFilterSelect(GlobalData.UserName, filterObject);
			if (dataTable.IsNotNull())
			{
				if (dataTable.Rows.Count == 1)
				{
					return Function.ToString(dataTable.Rows[0]["Code"]);
				}
				frm_FilterSelect frm_FilterSelect2 = new frm_FilterSelect();
				frm_FilterSelect2.ObjType = filterObject;
				frm_FilterSelect2.Source = dataTable;
				frm_FilterSelect2.ShowDialog();
				return frm_FilterSelect2.Selected;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return string.Empty;
	}

	public static string ExportGridcontrolToXls(string fileName, GridControlEx grd)
	{
		try
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.FileName = fileName + "-" + DateTime.Now.ToString("yyyyMMdd");
			saveFileDialog.ValidateNames = true;
			saveFileDialog.Filter = "Excel 97-2003 Workbook (*.xls)|*.xls |Excel Workbook (*.xlsx)|*.xlsx";
			saveFileDialog.Title = "Export to excel";
			if (saveFileDialog.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(saveFileDialog.FileName))
			{
				string text = Path.GetExtension(saveFileDialog.FileName).ToLower().Trim();
				if (!(text == ".xls"))
				{
					if (text == ".xlsx")
					{
						grd.ExportToXlsx(saveFileDialog.FileName);
					}
					else
					{
						grd.ExportToXls(saveFileDialog.FileName);
					}
				}
				else
				{
					grd.ExportToXls(saveFileDialog.FileName);
				}
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	internal static void ResetControl(Control.ControlCollection Controls)
	{
		try
		{
			foreach (Control Control in Controls)
			{
				switch (getControlType(Control))
				{
				case "CheckEdit":
					((CheckEdit)Control).EditValue = "N";
					break;
				case "RadioGroup":
					((RadioButton)Control).Checked = false;
					break;
				case "TextEdit":
					((TextEdit)Control).EditValue = string.Empty;
					break;
				case "ButtonEdit":
					((ButtonEdit)Control).EditValue = string.Empty;
					break;
				case "MemoEdit":
					((MemoEdit)Control).EditValue = string.Empty;
					break;
				case "LookUpEdit":
					((LookUpEdit)Control).EditValue = string.Empty;
					break;
				case "ComboBoxEdit":
					((ComboBoxEdit)Control).SelectedIndex = 0;
					break;
				}
				if (Control.Controls.Count > 0)
				{
					ResetControl(Control.Controls);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public static async Task<DataTable> DocumentLoadTabAccountingColumns()
	{
		DataTable dt = new DataTable();
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			dt = await hp.GetAsync("DocumentLoadTabAccountingColumns");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return dt;
	}

	public static async Task<string> LoadConfigFileSharingUpload()
	{
		string attachPath = "";
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "GeneralSetting");
			DataTable dataTable = await hp.GetAsync("GetFileSharingUploadPath");
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				attachPath = Function.ToString(dataTable.Rows[0]["AttachPath"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return attachPath;
	}

	public static void GetControl(Control control, ref List<GridControlEx> lst)
	{
		try
		{
			if (control == null)
			{
				return;
			}
			if (control.GetType() == typeof(GridControlEx))
			{
				lst.Add((GridControlEx)control);
			}
			else
			{
				if (control.Controls == null)
				{
					return;
				}
				{
					foreach (Control control2 in control.Controls)
					{
						GetControl(control2, ref lst);
					}
					return;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public static List<GridViewEx> GetListGridViewEx(XtraFormEx form)
	{
		List<GridViewEx> list = new List<GridViewEx>();
		try
		{
			List<GridControlEx> lst = new List<GridControlEx>();
			foreach (Control control in form.Controls)
			{
				GetControl(control, ref lst);
			}
			if (string.IsNullOrEmpty(GetActiveGriview(form)))
			{
				for (int i = 0; i < lst.Count; i++)
				{
					for (int j = i + 1; j < lst.Count; j++)
					{
						if (lst[i].Location.Y > lst[j].Location.Y)
						{
							GridControlEx value = lst[i];
							lst[i] = lst[j];
							lst[j] = value;
						}
					}
					list.Add((GridViewEx)lst[i].MainView);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return list;
	}

	public static string GetActiveGriview(XtraFormEx form)
	{
		try
		{
			Control control = form.ActiveControl;
			while (control != null && control.GetType() != typeof(GridControlEx))
			{
				control = control.Parent;
			}
			if (control != null && control.GetType() == typeof(GridControlEx))
			{
				return ((GridControlEx)control).MainView.Name;
			}
			return "";
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static string GetActiveColumns(XtraFormEx form)
	{
		try
		{
			return form.ActiveControl.Name;
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static bool CheckExistValidValue(string queryID, string value)
	{
		bool result = false;
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			DataTable dataTable = httpClientHelper.Get("CheckExistValidValue", new { queryID, value });
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				result = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			result = false;
		}
		return result;
	}

	public static string GetDateFormat(string format)
	{
		try
		{
			string text = format;
			return (format.ToUpper() switch
			{
				"DD/MM/YY" => "dd/MM/yy", 
				"DD/MM/CCYY" => "dd/MM/yyyy", 
				"MM/DD/YY" => "MM/dd/yy", 
				"MM/DD/CCYY" => "MM/dd/yyyy", 
				"CCYY/MM/DD" => "yyyy/MM/dd", 
				"DD/Month/YYYY" => "dd/MMMM/yyyy", 
				"YY/MM/DD" => "yy/MM/dd", 
				_ => "dd/MM/yy", 
			}).Replace("/", GlobalData.DateSeparatorFormat ?? "-");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "dd/MM/yyyy HH:mm:ss";
		}
	}

	public static string GetTimeFormat()
	{
		try
		{
			string result = "";
			if (GlobalData.TimeFormat == "24H")
			{
				result = "HH:mm:ss";
			}
			else if (GlobalData.TimeFormat == "12H")
			{
				result = "hh:mm:ss";
			}
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "HH:mm:ss";
		}
	}

	internal static string GetBPCurrency(string cardCode)
	{
		try
		{
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "Commons");
			return httpClientHelper.Get("GetBPCurrency", new { cardCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return "##";
		}
	}

	public static object DataRowToEntity(object obj, DataRow row)
	{
		for (int i = 0; i < row.Table.Columns.Count; i++)
		{
			string columnName = row.Table.Columns[i].ColumnName;
			PropertyInfo property = obj.GetType().GetProperty(columnName);
			if (property == null)
			{
				continue;
			}
			switch (Function.GetTypeName(property.PropertyType).ToLower())
			{
			case "string":
				obj.GetType().GetProperty(columnName).SetValue(obj, Function.ToString(row[row.Table.Columns[i]]));
				break;
			case "double":
				obj.GetType().GetProperty(columnName).SetValue(obj, Function.ParseDouble(row[row.Table.Columns[i]]));
				break;
			case "int16":
				obj.GetType().GetProperty(columnName).SetValue(obj, Function.ParseShort(row[row.Table.Columns[i]]));
				break;
			case "int32":
				obj.GetType().GetProperty(columnName).SetValue(obj, Function.ParseInt(row[row.Table.Columns[i]]));
				break;
			case "int64":
				obj.GetType().GetProperty(columnName).SetValue(obj, Function.ParseLong(row[row.Table.Columns[i]]));
				break;
			case "datetime":
			{
				DateTime? dateTime = ((property == null) ? ((DateTime?)null) : Function.ParseDateTime(row[row.Table.Columns[i]]));
				PropertyInfo property2 = obj.GetType().GetProperty(columnName);
				DateTime dateTime2;
				if (dateTime.HasValue)
				{
					_ = dateTime.Value;
					dateTime2 = dateTime.Value;
				}
				else
				{
					dateTime2 = new DateTime(1900, 1, 1);
				}
				property2.SetValue(obj, dateTime2);
				break;
			}
			}
		}
		return obj;
	}

	public static string GetExceptionLineNumber(Exception exc)
	{
		try
		{
			StackFrame frame = new StackTrace(exc, fNeedFileInfo: true).GetFrame(0);
			return " Line:[" + frame.GetFileLineNumber() + "]";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return " Line:[0]";
		}
	}

	public static DataTable GetInventoryUoMByItemCode(string itemCode, Enums.ItemUomType type)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			return httpClientHelper.Get("GetInventoryUoMByItemCode", new { itemCode, type });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	public static DataTable GetBaseUomByUomGroup(int uomGroup)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			return httpClientHelper.Get("GetBaseUomByUomGroup", new { uomGroup });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	public static string GetUoMCodeByName(int uomGroup, string uomName)
	{
		try
		{
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "Commons");
			return httpClientHelper.Get("GetUoMCodeByName", new { uomGroup, uomName });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return "";
		}
	}

	public static DataTable GetDataUomByCode(int uomGroup, string uomCode)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			return httpClientHelper.Get("GetDataUomByCode", new { uomGroup, uomCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	public static void PasteFromExcelToDataTableBomDetail(GridViewEx gv, List<string> key, int type)
	{
		try
		{
			if (type != 290 && type != 4)
			{
				return;
			}
			DataObject dataObject = (DataObject)Clipboard.GetDataObject();
			string clipboardData = dataObject.GetData(DataFormats.UnicodeText).ToString();
			if (!dataObject.GetDataPresent(DataFormats.Text))
			{
				return;
			}
			DataTable dataTable = Function.PasteFromExcel(clipboardData);
			if (dataTable == null || (dataTable.Rows.Count <= 0 && dataTable.Columns.Count <= 0))
			{
				return;
			}
			int num = gv.FocusedRowHandle;
			GridColumn focusedColumn = gv.FocusedColumn;
			if (key.Contains(focusedColumn.FieldName))
			{
				int focusedRowHandle = gv.FocusedRowHandle;
				for (int num2 = dataTable.Rows.Count - 1; num2 >= 0; num2--)
				{
					DataRow row = gv.GridControl.DataSourceTable().NewRow();
					gv.GridControl.DataSourceTable().Rows.InsertAt(row, focusedRowHandle);
					gv.GridControl.DataSourceTable().AcceptChanges();
					int colunmIndex = focusedColumn.VisibleIndex;
					for (int i = 0; i < dataTable.Columns.Count; i++)
					{
						gv.FocusedRowHandle = focusedRowHandle;
						GridColumn gridColumn = gv.Columns.FirstOrDefault((GridColumn t) => t.VisibleIndex == colunmIndex);
						if (gridColumn == null)
						{
							break;
						}
						gv.SetRowCellValue(num, gridColumn, dataTable.Rows[num2][i]);
						colunmIndex++;
					}
					gv.SetRowCellValue(num, "Type", type);
				}
			}
			else
			{
				int focusedRowHandle2 = gv.FocusedRowHandle;
				for (int num3 = focusedRowHandle2; num3 < gv.RowCount; num3++)
				{
					int colIndex = focusedColumn.VisibleIndex;
					bool flag = true;
					for (int num4 = 0; num4 < key.Count; num4++)
					{
						if (string.IsNullOrEmpty(Function.ToString(gv.GetRowCellValue(num3, key[num4]))))
						{
							flag = false;
							break;
						}
					}
					if (!flag || num3 - focusedRowHandle2 >= dataTable.Rows.Count)
					{
						break;
					}
					for (int num5 = 0; num5 < dataTable.Columns.Count; num5++)
					{
						GridColumn gridColumn2 = gv.Columns.FirstOrDefault((GridColumn t) => t.VisibleIndex == colIndex);
						if (gridColumn2 == null)
						{
							break;
						}
						gv.SetRowCellValue(num, gridColumn2, dataTable.Rows[num3 - focusedRowHandle2][num5]);
						colIndex++;
					}
					num++;
				}
				gv.GridControl.RefreshDataSource();
			}
			Clipboard.Clear();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
	}

	public static void SetReadOnlyListColumnGrid(GridViewEx gv, List<string> lstColumnReadOnly)
	{
		try
		{
			if (gv == null || gv.Columns.Count == 0 || lstColumnReadOnly == null || lstColumnReadOnly.Count == 0)
			{
				return;
			}
			int count = gv.Columns.Count;
			for (int i = 0; i < count; i++)
			{
				if (lstColumnReadOnly.Contains(gv.Columns[i].FieldName))
				{
					gv.Columns[i].OptionsColumn.ReadOnly = true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public static string GetVersionString()
	{
		return Function.ConvertDoubleToStringEnUs(Versions.ApplicationVersion, 2, 1) + "." + Function.ConvertDoubleToStringEnUs(Versions.BuildVersion, 0, 3);
	}

	internal static GridControlEx GetActiveGridControl(Control control)
	{
		try
		{
			if (control != null)
			{
				if (control.GetType() == typeof(GridControl) || control.GetType() == typeof(GridControlEx))
				{
					return control as GridControlEx;
				}
				if (control.GetType() == typeof(ucBatchAndSerial))
				{
					return (control as ucBatchAndSerial).grdBatches;
				}
				if (control.Parent != null)
				{
					return GetActiveGridControl(control.Parent);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			XtraMessageBox.Show(ex.Message);
		}
		return null;
	}

	internal static TreeList GetActiveTreeListControl(Control control)
	{
		try
		{
			if (control != null && control.GetType() == typeof(GridControl))
			{
				return control as TreeList;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			XtraMessageBox.Show(ex.Message);
		}
		return null;
	}

	public static List<Common.ListItems> MoveFileOldVersionToFolderOld(string sourceFolder, string oldFolder, string fileUpdate, ref string newVersion)
	{
		List<Common.ListItems> list = new List<Common.ListItems>();
		if (!Directory.Exists(sourceFolder) || string.IsNullOrEmpty(fileUpdate))
		{
			return list;
		}
		if (!fileUpdate.StartsWith("DevExpress.") || !fileUpdate.EndsWith(".dll"))
		{
			string text = string.Join("\\", sourceFolder, fileUpdate);
			if (File.Exists(text))
			{
				string text2 = Path.Combine(oldFolder, fileUpdate);
				File.Move(text, text2);
				list.Add(new Common.ListItems
				{
					Text = text,
					Value = text2
				});
			}
			return list;
		}
		if (string.IsNullOrEmpty(newVersion))
		{
			newVersion = Function.GetVersionDevexpressDllFile(Path.GetFileName(fileUpdate));
		}
		string[] files = Directory.GetFiles(sourceFolder);
		for (int i = 0; i < files.Length; i++)
		{
			string fileName = Path.GetFileName(files[i]);
			if (fileName == null || !fileName.StartsWith("DevExpress.") || !fileName.EndsWith(".dll"))
			{
				continue;
			}
			string versionDevexpressDllFile = Function.GetVersionDevexpressDllFile(fileName);
			if (fileUpdate.Equals(fileName.Replace(versionDevexpressDllFile, newVersion)))
			{
				string text3 = string.Join("\\", oldFolder, Path.GetFileName(files[i]));
				if (File.Exists(files[i]))
				{
					File.Move(files[i], text3);
					list.Add(new Common.ListItems
					{
						Text = files[i],
						Value = text3
					});
				}
			}
			if (versionDevexpressDllFile.Equals(newVersion))
			{
				continue;
			}
			string[] files2 = Directory.GetFiles(sourceFolder);
			for (int j = 0; j < files2.Length; j++)
			{
				string fileName2 = Path.GetFileName(files2[j]);
				if (fileName2 != null && fileName2.StartsWith("DevExpress.") && fileName2.EndsWith(".dll") && fileName2.Contains(versionDevexpressDllFile))
				{
					string text4 = Path.Combine(oldFolder, fileName2);
					File.Move(files2[j], text4);
					list.Add(new Common.ListItems
					{
						Text = files2[j],
						Value = text4
					});
				}
			}
		}
		return list;
	}

	public static BarSubItem GetMenubyCode(Bar MenuBar, string menucode)
	{
		foreach (object itemLink in MenuBar.ItemLinks)
		{
			if (itemLink.GetType() == typeof(BarSubItemLink))
			{
				if (!(itemLink is BarSubItemLink { Item: not null } barSubItemLink))
				{
					continue;
				}
				BarSubItem barSubItem = barSubItemLink.Item as BarSubItem;
				if (barSubItem != null && barSubItem.Description == menucode)
				{
					return barSubItem;
				}
				if (barSubItem != null && barSubItem.ItemLinks.Count > 0)
				{
					BarSubItem menubyCode = GetMenubyCode(barSubItem, menucode);
					if (menubyCode != null)
					{
						return menubyCode;
					}
				}
			}
			else
			{
				if (!(itemLink is BarItemLink { Item: not null } barItemLink))
				{
					continue;
				}
				BarSubItem barSubItem2 = barItemLink.Item as BarSubItem;
				if (barSubItem2 != null && barSubItem2.Description == menucode)
				{
					return barSubItem2;
				}
				if (barSubItem2 != null && barSubItem2.ItemLinks.Count > 0)
				{
					BarSubItem menubyCode2 = GetMenubyCode(barSubItem2, menucode);
					if (menubyCode2 != null)
					{
						return menubyCode2;
					}
				}
			}
		}
		return null;
	}

	private static BarSubItem GetMenubyCode(BarSubItem item, string menucode)
	{
		foreach (object itemLink in item.ItemLinks)
		{
			if (itemLink.GetType() == typeof(BarSubItemLink))
			{
				if (itemLink is BarSubItemLink { Item: not null } barSubItemLink)
				{
					BarSubItem barSubItem = barSubItemLink.Item as BarSubItem;
					if (barSubItem != null && barSubItem.Description == menucode)
					{
						return barSubItem;
					}
					if (barSubItem != null && barSubItem.ItemLinks.Count > 0)
					{
						return GetMenubyCode(barSubItem, menucode);
					}
				}
			}
			else if (itemLink is BarItemLink { Item: not null } barItemLink)
			{
				BarSubItem barSubItem2 = barItemLink.Item as BarSubItem;
				if (barSubItem2 != null && barSubItem2.Description == menucode)
				{
					return barSubItem2;
				}
				if (barSubItem2 != null && barSubItem2.ItemLinks.Count > 0)
				{
					return GetMenubyCode(barSubItem2, menucode);
				}
			}
		}
		return null;
	}

	public static async Task<List<UserFieldsImpl>> DataRowToUserFields(DataRow row, string tableName)
	{
		List<UserFieldsImpl> source = new List<UserFieldsImpl>();
		using (HttpClientHelper<UserFieldsImpl> helper = new HttpClientHelper<UserFieldsImpl>(Configuration.ApiServer, "UDF"))
		{
			source = await helper.GetListAsyncByPr("GetListField", new
			{
				tablename = tableName
			});
		}
		List<UserFieldsImpl> list = new List<UserFieldsImpl>();
		for (int i = 0; i < row.Table.Columns.Count; i++)
		{
			DataColumn col = row.Table.Columns[i];
			if (!col.ColumnName.StartsWith("U_"))
			{
				continue;
			}
			UserFieldsImpl userFieldsImpl = source.FirstOrDefault((UserFieldsImpl u) => u.Name == col.ColumnName.Remove(0, 2));
			if (userFieldsImpl == null)
			{
				continue;
			}
			string name = col.DataType.Name;
			UserFieldsImpl userFieldsImpl2 = new UserFieldsImpl();
			switch (name)
			{
			case "DateTime":
			{
				if (userFieldsImpl.SubType == BoFldSubTypes.st_Time)
				{
					userFieldsImpl2.Type = BoFieldTypes.db_Alpha;
					userFieldsImpl2.Value = Function.ParseInt(row[col]).ToString("0000");
					break;
				}
				DateTime? dateTime = Function.ParseDateTime(row[col]);
				DateTime dateTime2;
				if (dateTime.HasValue)
				{
					_ = dateTime.Value;
					dateTime2 = dateTime.Value;
				}
				else
				{
					dateTime2 = new DateTime(1900, 1, 1);
				}
				userFieldsImpl2.Value = Function.ToString(dateTime2);
				userFieldsImpl2.Type = BoFieldTypes.db_Date;
				break;
			}
			case "Int16":
			case "Int32":
			case "Int64":
				if (userFieldsImpl.SubType == BoFldSubTypes.st_Time)
				{
					userFieldsImpl2.Type = BoFieldTypes.db_Alpha;
					userFieldsImpl2.Value = Function.ParseInt(row[col]).ToString("0000");
				}
				else
				{
					userFieldsImpl2.Value = Function.ToString(Function.ParseInt(row[col]));
					userFieldsImpl2.Type = BoFieldTypes.db_Numeric;
				}
				break;
			case "Single":
			case "Double":
			case "Float":
				userFieldsImpl2.Value = Function.ToString(Function.ParseDouble(row[col]));
				userFieldsImpl2.Type = BoFieldTypes.db_Float;
				break;
			default:
				userFieldsImpl2.Value = Function.ToString(row[col]);
				userFieldsImpl2.Type = BoFieldTypes.db_Alpha;
				break;
			}
			userFieldsImpl2.Name = col.ColumnName;
			list.Add(userFieldsImpl2);
		}
		return list;
	}

	public static async Task CheckForUpdateClientHttp(string databaseName, bool isOpenAplication)
	{
		string text = await CheckFileUpdate(databaseName);
		if (!string.IsNullOrEmpty(text))
		{
			await UpdateClient(text);
		}
		if (!isOpenAplication)
		{
			WinCommons.Info(Enums.MessageCode.YourClientIsLatest, "");
		}
	}

	private static async Task<string> CheckFileUpdate(string databaseName)
	{
		_ = 1;
		try
		{
			using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "UpdateClient");
			HttpResult result = await http.GetAsync("CheckForUpdateClient", new
			{
				databaseName = databaseName,
				appVersion = Function.ConvertDoubleToStringEnUs(Versions.ApplicationVersion, 2, 1),
				buldVersion = Function.ConvertDoubleToStringEnUs(Versions.BuildVersion, 0, 3)
			});
			if (string.IsNullOrEmpty(result.Message))
			{
				return "";
			}
			if (result.Message.Equals("1") || result.Message.Equals("2"))
			{
				WinCommons._OpenProcessing();
				WinCommons.Info(Enums.MessageCode.None, "Please wait while updating the database");
				GlobalData.Updating = true;
				http.HttpClient.DefaultRequestHeaders.Add("Database", databaseName);
				HttpResult httpResult = await http.PostAsync("UpdateDatabase", databaseName);
				if (httpResult.MessageCode == Enums.MessageCode.Success)
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
				}
				else
				{
					WinCommons.Warning(Enums.MessageCode.None, httpResult.Message);
				}
				WinCommons._CloseProcessing();
				return result.Message.Equals("1") ? Function.ToString(result.Content) : "";
			}
			WinCommons.Warning(Enums.MessageCode.None, result.Message);
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Warning(Enums.MessageCode.ExceptionMessage, ex.Message);
			return "";
		}
		finally
		{
			GlobalData.Updating = false;
		}
	}

	private static async Task UpdateClient(string newApi)
	{
		if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdateNewVersion, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
		{
			return;
		}
		WinCommons.Info(Enums.MessageCode.DownloadFileUpdatePleaseWait, "");
		GlobalData.Updating = true;
		WinCommons._OpenProcessing();
		string path = "";
		string folderName = "";
		try
		{
			using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "UpdateClient");
			http.UpdateSessionId();
			bool isParseData = false;
			string uri = http.GetUri("DownLoadFileUpdateClient", typeof(string), ref isParseData);
			Enums.PlatformClient value = ((Marshal.SizeOf(typeof(IntPtr)) == 8) ? Enums.PlatformClient.X64 : Enums.PlatformClient.X86);
			using HttpResponseMessage response = await http.HttpClient.PostAsJsonAsync(uri, value);
			if (response.IsSuccessStatusCode)
			{
				HttpContent content = response.Content;
				Stream stream = await content.ReadAsStreamAsync();
				string startupPath = Application.StartupPath;
				path = string.Format("{1}\\{0}", content.Headers.ContentDisposition.FileName, startupPath);
				folderName = $"{startupPath}\\Update";
				if (Directory.Exists(folderName))
				{
					Function.DeleteFolder(folderName);
				}
				if (File.Exists(path))
				{
					File.Delete(path);
				}
				using (FileStream destination = File.Create(path))
				{
					stream.CopyTo(destination);
				}
				Function.UnZipFile(path, folderName);
				string configurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
				XElement content2 = XDocument.Load(configurationFile).Element("configuration").Element("appSettings");
				if (Directory.Exists(startupPath))
				{
					MoveFileToFolderOld(startupPath, folderName, $"{startupPath}\\Old");
				}
				CopyFolder(folderName, startupPath);
				XDocument xDocument = XDocument.Load(configurationFile);
				XElement xElement = xDocument.Element("configuration");
				xElement.Element("appSettings")?.Remove();
				xElement.Element("configSections").AddAfterSelf(content2);
				xDocument.Save(configurationFile);
				File.Delete(path);
				Function.DeleteFolder(folderName);
				WinCommons.Success(Enums.MessageCode.UpdateSuccess, "");
				if (XtraMessageBoxEx.Show(Enums.MessageCode.InfoRestartSystem, Enums.MessageCode.SystemMessage) == DialogResult.OK)
				{
					Main.Exit = true;
					Main.IsRestart = true;
					Program.AplicationRestart();
				}
				else
				{
					Program.AplicationExit();
				}
			}
			else
			{
				WinCommons.Warning(Enums.MessageCode.CouldNotDownloadFile2Update, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Warning(Enums.MessageCode.ExceptionMessage, ex.Message);
			if (File.Exists(path))
			{
				File.Delete(path);
			}
			if (Directory.Exists(folderName))
			{
				Function.DeleteFolder(folderName);
			}
		}
		finally
		{
			GlobalData.Updating = false;
		}
		WinCommons._CloseProcessing();
	}

	private static bool CopyFolder(string sourceFolder, string destFolder)
	{
		if (!Directory.Exists(destFolder))
		{
			Directory.CreateDirectory(destFolder);
		}
		string[] files = Directory.GetFiles(sourceFolder);
		foreach (string text in files)
		{
			string fileName = Path.GetFileName(text);
			if (fileName != null)
			{
				string destFileName = Path.Combine(destFolder, fileName);
				File.Copy(text, destFileName);
			}
		}
		files = Directory.GetDirectories(sourceFolder);
		foreach (string text2 in files)
		{
			string fileName2 = Path.GetFileName(text2);
			if (fileName2 != null)
			{
				string destFolder2 = Path.Combine(destFolder, fileName2);
				CopyFolder(text2, destFolder2);
			}
		}
		return true;
	}

	private static void MoveFileToFolderOld(string destFolder, string sourceFolder, string oldFolder)
	{
		if (!Directory.Exists(destFolder))
		{
			return;
		}
		if (Directory.Exists(oldFolder))
		{
			DeleteFolder(oldFolder);
		}
		Directory.CreateDirectory(oldFolder);
		DirectoryInfo directoryInfo = new DirectoryInfo(oldFolder);
		DirectorySecurity accessControl = directoryInfo.GetAccessControl();
		accessControl.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
		directoryInfo.SetAccessControl(accessControl);
		string[] files = Directory.GetFiles(sourceFolder);
		string newVersion = "";
		string[] array = files;
		for (int i = 0; i < array.Length; i++)
		{
			string fileName = Path.GetFileName(array[i]);
			if (fileName != null && File.Exists(Path.Combine(destFolder, fileName)))
			{
				try
				{
					MoveFileOldVersionToFolderOld(destFolder, oldFolder, fileName, ref newVersion);
				}
				catch (Exception)
				{
				}
			}
		}
		array = Directory.GetDirectories(sourceFolder);
		foreach (string text in array)
		{
			string fileName2 = Path.GetFileName(text);
			if (fileName2 == null)
			{
				continue;
			}
			string text2 = Path.Combine(destFolder, fileName2);
			string oldFolder2 = Path.Combine(oldFolder, fileName2);
			if (Directory.Exists(text2))
			{
				try
				{
					MoveFileToFolderOld(text2, text, oldFolder2);
				}
				catch (Exception)
				{
				}
			}
		}
	}

	private static void DeleteFolder(string sourceFolder)
	{
		try
		{
			if (Directory.Exists(sourceFolder))
			{
				string[] files = Directory.GetFiles(sourceFolder);
				for (int i = 0; i < files.Length; i++)
				{
					File.Delete(files[i]);
				}
				files = Directory.GetDirectories(sourceFolder);
				for (int i = 0; i < files.Length; i++)
				{
					DeleteFolder(files[i]);
				}
				Function.DeleteFolder(sourceFolder);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public static List<Common.Table_Code> GetListOriginType()
	{
		List<Common.Table_Code> list = new List<Common.Table_Code>();
		try
		{
			list.Add(new Common.Table_Code
			{
				Name = "MRP",
				Code = "700003"
			});
			list.Add(new Common.Table_Code
			{
				Name = "Schedule",
				Code = "800004"
			});
			list.Add(new Common.Table_Code
			{
				Name = "ATP",
				Code = "700013"
			});
			list.Add(new Common.Table_Code
			{
				Name = "Schedule ATP",
				Code = "700015"
			});
			list.Add(new Common.Table_Code
			{
				Name = "Material Shortage",
				Code = "700016"
			});
			list.Add(new Common.Table_Code
			{
				Name = "Capacity",
				Code = "800005"
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return list;
	}

	public static string ValidateDistrRule(string value)
	{
		try
		{
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "CFL");
			string text = httpClientHelper.Get("ValidateDistrRule", new { value });
			if (!text.ToLower().Contains("error"))
			{
				return text;
			}
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), text);
			WinCommons.Error(Enums.MessageCode.None, text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return "";
	}

	public static void ShowCFL(ButtonEdit cbDistrRule)
	{
		try
		{
			string value = Function.ToString(cbDistrRule.EditValue);
			using frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.DistributeRule, multiSelect: false, "");
			frm_ChooseFromList2._strOcrDistRule = Function.ToString(cbDistrRule.EditValue);
			frm_ChooseFromList2.ShowDialog(cbDistrRule.FindForm());
			DataTable dtSource = frm_ChooseFromList2._dtSource;
			string text = "";
			if (dtSource != null && dtSource.Rows.Count > 0)
			{
				for (int i = 0; i < dtSource.Rows.Count; i++)
				{
					text = text + Function.ToString(dtSource.Rows[i]["OcrCode"]) + ";";
				}
			}
			string value2 = text.Replace(";", "");
			if (string.IsNullOrEmpty(value) || !string.IsNullOrEmpty(value2))
			{
				while (text.Length > 0 && text[text.Length - 1].Equals(';'))
				{
					text = text.Remove(text.Length - 1);
				}
				cbDistrRule.EditValue = text;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public static void DrawMergedCells(GridViewEx gvLine, GridViewExPainter painter, GridColumn colStart, GridColumn colEnd, string colName, string colValue, PaintEventArgs e)
	{
		try
		{
			if (!(gvLine.GridControl.DataSource is DataTable dataTable) || dataTable.Rows.Count == 0)
			{
				return;
			}
			List<DataRow> list = (from t in dataTable.AsEnumerable()
				where Function.ToString(t[colName]) == colValue
				select t).ToList();
			if (list == null || list.Count <= 0)
			{
				return;
			}
			foreach (DataRow item in list)
			{
				int num = gvLine.FindRowHandleByDataRow(item);
				if (num >= 0)
				{
					GridViewExMergeCell cell = new GridViewExMergeCell(num, colStart, colEnd);
					painter.DrawMergedCell(cell, e);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public static DataTable GetControlsValueToDataTable(string table, Dictionary<string, Control> _dicControls)
	{
		DataTable tableStructure = GetTableStructure(table);
		DataRow dataRow = tableStructure.NewRow();
		try
		{
			if (_dicControls.Any((KeyValuePair<string, Control> t) => t.Key.Split('.')[0] == table))
			{
				List<KeyValuePair<string, Control>> list = _dicControls.Where((KeyValuePair<string, Control> t) => t.Key.Split('.')[0] == table).ToList();
				for (int num = 0; num < list.Count(); num++)
				{
					string text = ((list[num].Key.Split('.') != null && list[num].Key.Split('.').Length == 2) ? list[num].Key.Split('.')[1] : "");
					if (string.IsNullOrWhiteSpace(text) || !tableStructure.Columns.Contains(text))
					{
						continue;
					}
					switch (tableStructure.Columns[text].DataType.ToString().ToLower())
					{
					case "system.string":
						dataRow[text] = Function.ToString(((BaseEdit)list[num].Value).EditValue);
						break;
					case "system.double":
						dataRow[text] = Function.ParseDouble(((BaseEdit)list[num].Value).EditValue);
						break;
					case "system.float":
						dataRow[text] = Function.ParseDouble(((BaseEdit)list[num].Value).EditValue);
						break;
					case "system.single":
						dataRow[text] = Function.ParseDouble(((BaseEdit)list[num].Value).EditValue);
						break;
					case "system.decimal":
						dataRow[text] = Function.ParseDouble(((BaseEdit)list[num].Value).EditValue);
						break;
					case "system.int16":
						dataRow[text] = Convert.ToInt16(Function.ParseInt(((BaseEdit)list[num].Value).EditValue));
						break;
					case "system.int32":
						dataRow[text] = Convert.ToInt32(Function.ParseInt(((BaseEdit)list[num].Value).EditValue));
						break;
					case "system.int64":
						dataRow[text] = Convert.ToInt64(Function.ParseInt(((BaseEdit)list[num].Value).EditValue));
						break;
					case "system.datetime":
					{
						DateTime? dateTime = Function.ParseDateTime(((BaseEdit)list[num].Value).EditValue);
						DateTime dateTime2;
						if (dateTime.HasValue)
						{
							_ = dateTime.Value;
							dateTime2 = dateTime.Value;
						}
						else
						{
							dateTime2 = new DateTime(1900, 1, 1);
						}
						dataRow[text] = dateTime2;
						break;
					}
					}
				}
			}
			tableStructure.Rows.Add(dataRow);
			tableStructure.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return tableStructure;
	}

	public static void BindControlsValue(string table, DataTable dt, Dictionary<string, Control> _dicControls, ref bool isBinding, bool startIsBindingValue, List<string> lstManualBinding = null)
	{
		isBinding = startIsBindingValue;
		new List<PropertyInfo>();
		try
		{
			if (_dicControls.Any((KeyValuePair<string, Control> t) => t.Key.Split('.')[0] == table))
			{
				List<KeyValuePair<string, Control>> list = _dicControls.Where((KeyValuePair<string, Control> t) => t.Key.Split('.')[0] == table).ToList();
				DataRow dataRow = dt.Rows[0];
				if (lstManualBinding != null)
				{
					IEnumerable<KeyValuePair<string, Control>> enumerable = list.Where((KeyValuePair<string, Control> t) => lstManualBinding.Contains(t.Key.Split('.')[1]));
					if (enumerable != null)
					{
						KeyValuePair<string, Control>[] array = enumerable.ToArray();
						for (int num = 0; num < array.Length; num++)
						{
							list.Remove(array[num]);
						}
					}
				}
				for (int num2 = 0; num2 < list.Count; num2++)
				{
					string text = ((list[num2].Key.Split('.') != null && list[num2].Key.Split('.').Length == 2) ? list[num2].Key.Split('.')[1] : "");
					if (string.IsNullOrWhiteSpace(text) || !dt.Columns.Contains(text))
					{
						continue;
					}
					switch (dt.Columns[text].DataType.ToString().ToLower())
					{
					case "system.string":
						((BaseEdit)list[num2].Value).EditValue = Function.ToString(dataRow[dt.Columns[text]]);
						break;
					case "system.double":
						((BaseEdit)list[num2].Value).EditValue = Function.ParseDouble(dataRow[dt.Columns[text]]);
						break;
					case "system.decimal":
						((BaseEdit)list[num2].Value).EditValue = Function.ParseDouble(dataRow[dt.Columns[text]]);
						break;
					case "system.float":
						((BaseEdit)list[num2].Value).EditValue = Function.ParseDouble(dataRow[dt.Columns[text]]);
						break;
					case "system.single":
						((BaseEdit)list[num2].Value).EditValue = Function.ParseDouble(dataRow[dt.Columns[text]]);
						break;
					case "system.int16":
						if (dataRow[dt.Columns[text]] == null || dataRow[dt.Columns[text]] == DBNull.Value)
						{
							((BaseEdit)list[num2].Value).EditValue = null;
						}
						else
						{
							((BaseEdit)list[num2].Value).EditValue = Convert.ToInt16(Function.ParseInt(dataRow[dt.Columns[text]]));
						}
						break;
					case "system.int32":
						if (dataRow[dt.Columns[text]] == null || dataRow[dt.Columns[text]] == DBNull.Value)
						{
							((BaseEdit)list[num2].Value).EditValue = null;
						}
						else
						{
							((BaseEdit)list[num2].Value).EditValue = Function.ParseInt(dataRow[dt.Columns[text]]);
						}
						break;
					case "system.int64":
						if (dataRow[dt.Columns[text]] == null || dataRow[dt.Columns[text]] == DBNull.Value)
						{
							((BaseEdit)list[num2].Value).EditValue = null;
						}
						else
						{
							((BaseEdit)list[num2].Value).EditValue = Convert.ToInt64(Function.ParseInt(dataRow[dt.Columns[text]]));
						}
						break;
					case "system.datetime":
						((BaseEdit)list[num2].Value).EditValue = Function.ParseDateTime(dataRow[dt.Columns[text]]);
						break;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		isBinding = !startIsBindingValue;
	}

	public static bool UploadPictureFile(string _picFilePath, ref string pictureName)
	{
		try
		{
			FtpHelper ftpHelper = new FtpHelper();
			string fileName = Path.GetFileName(_picFilePath);
			if (File.Exists(_picFilePath))
			{
				string upLoadPath = CopyFileShareHelper.GetUpLoadPath("jpg");
				pictureName = fileName;
				string text = upLoadPath + pictureName;
				string extension = Path.GetExtension(text);
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(text);
				string text2 = text;
				string text3 = pictureName;
				int num = 1;
				while (File.Exists(text2))
				{
					string text4 = fileNameWithoutExtension;
					text2 = text2.Remove(text2.Length - 1 - (text4.Length + extension.Length));
					text3 = text4 + num + extension;
					text2 = text2 + "\\" + text3;
					num++;
				}
				text = text2;
				pictureName = text3;
				string text5 = ftpHelper.CopyFileSharing(CopyFileSharingCfg.Domain, _picFilePath, CopyFileSharingCfg.UserName, CopyFileSharingCfg.Password, text);
				if (text5 != text)
				{
					WinCommons._CloseProcessing();
					WinCommons.Error(Enums.MessageCode.UploadPictureFailed, "", text5);
					return false;
				}
				return true;
			}
			WinCommons._CloseProcessing();
			WinCommons.Error(Enums.MessageCode.UploadPictureFileFailedSourceFileNotFoundWithPath, "", _picFilePath);
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
			return false;
		}
	}

	public static async Task<DataTable> LoadSearchSetting(string FormId, string ItemId)
	{
		try
		{
			IHttpClient httpClient = HttpClientHelperFactory.GetHttpClient();
			string userCodeLogin = GlobalData.UserCodeLogin;
			return await httpClient.SearchSetting.GetSearchSetting(FormId, ItemId, userCodeLogin);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	public static bool GetTableStruct(ref DataTable dt, string tableName, List<DataTable> lstStructDT)
	{
		try
		{
			DataTable dataTable = lstStructDT.FirstOrDefault((DataTable t) => t.TableName == tableName);
			if (dataTable != null)
			{
				dt = dataTable.Clone();
				return true;
			}
			WinCommons.Error(Enums.MessageCode.CannotGetTableStructureWithTableName, "", tableName);
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return false;
		}
	}

	public static DataTable GetDataTableNewRow(DataTable dt, bool dfltValue = true)
	{
		try
		{
			string tableName = dt.TableName;
			IHttpClient httpClient = HttpClientHelperFactory.GetHttpClient();
			if (dt == null || dt.Columns.Count == 0)
			{
				return dt;
			}
			DataTable dataTableNewRow = httpClient.Commons.GetDataTableNewRow(tableName);
			if (dataTableNewRow.IsNotNull())
			{
				for (int i = 0; i < dataTableNewRow.Rows.Count; i++)
				{
					string text = Function.ToString(dataTableNewRow.Rows[i]["DATA_TYPE"]).ToLower();
					string text2 = Function.ToString(dataTableNewRow.Rows[i]["COLUMN_NAME"]);
					string text3 = Function.ToString(dataTableNewRow.Rows[i]["COLUMN_DEFAULT"]).Replace("(", "").Replace(")", "");
					bool flag = Function.ToString(dataTableNewRow.Rows[i]["IS_NULLABLE"]).Equals("YES");
					if (!dfltValue || (flag && string.IsNullOrEmpty(text3)))
					{
						continue;
					}
					for (int j = 0; j < dt.Columns.Count; j++)
					{
						if (!(dt.Columns[j].ColumnName == text2))
						{
							continue;
						}
						switch (text)
						{
						case "int":
						case "bigint":
						case "smallint":
							dt.Columns[j].DefaultValue = Function.ParseInt(text3);
							break;
						case "decimal":
							dt.Columns[j].DefaultValue = Function.ParseDecimal(text3);
							break;
						case "datetime":
						case "date":
							if (!string.IsNullOrEmpty(text3) || !flag)
							{
								DateTime? dateTime = Function.ParseDateTime(text3);
								dt.Columns[j].DefaultValue = dateTime ?? new DateTime(1900, 1, 1);
							}
							break;
						case "numeric":
						case "float":
							dt.Columns[j].DefaultValue = Function.ParseDouble(text3);
							break;
						case "bit":
							dt.Columns[j].DefaultValue = Function.ParseBool(text3);
							break;
						default:
							dt.Columns[j].DefaultValue = Function.ToString(text3.Replace("'", ""));
							break;
						}
						break;
					}
				}
			}
			dt.Rows.Add(dt.NewRow());
			dt.AcceptChanges();
			return dt;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public static GridViewInfo GetGridViewInfo(GridView view)
	{
		return typeof(GridView).GetField("fViewInfo", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(view) as GridViewInfo;
	}

	public static async Task LoadFormatRuleGrid(List<GridViewEx> FormItem, Control form, string formId)
	{
		if (form == null || FormItem == null || FormItem.Count <= 0)
		{
			return;
		}
		foreach (GridViewEx item in FormItem)
		{
			if (item == null)
			{
				return;
			}
			await ExtensionMethods.ReloadFormatRuleGrid(item, formId);
		}
	}

	public static async Task RefreshVisibleCalendars()
	{
		try
		{
			List<object> list = new List<object>();
			for (int i = 0; i < Application.OpenForms.Count; i++)
			{
				dynamic val = Application.OpenForms[i];
				if (val.GetType() == typeof(frm_ActivityCalendar))
				{
					list.Add(val);
				}
			}
			foreach (dynamic item in list)
			{
				if (((frm_ActivityCalendar)item).GetType().GetMethod("ReloadCalendar") != null)
				{
					await item.ReloadCalendar();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public static string QueryReplaceUserAndBranch(string querytext)
	{
		try
		{
			querytext = Regex.Replace(querytext, "@CURRENTUSER", $"N'{GlobalData.UserName}'", RegexOptions.IgnoreCase);
			querytext = Regex.Replace(querytext, "$[USER]", $"N'{GlobalData.UserID}'", RegexOptions.IgnoreCase);
			querytext = Regex.Replace(querytext, "@CURRENTBRANCH", $"N'{GlobalData.DefaultBranch}'", RegexOptions.IgnoreCase);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return querytext;
	}

	public static bool SearchforCom(ref string sCom)
	{
		RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("HARDWARE\\DEVICEMAP\\SERIALCOMM");
		string[] valueNames = registryKey.GetValueNames();
		for (int i = 0; i < valueNames.Length; i++)
		{
			string text = "";
			text = registryKey.GetValue(valueNames[i]).ToString();
			if (text == "")
			{
				continue;
			}
			sCom = "";
			if (!(valueNames[i] == "\\Device\\USBSER000"))
			{
				continue;
			}
			for (int j = 0; j <= 10; j++)
			{
				string text2 = "";
				RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum\\USB\\VID_1B55&PID_B400\\" + j + "\\Device Parameters");
				if (registryKey2 != null)
				{
					text2 = registryKey2.GetValue("PortName").ToString();
					if (text == text2)
					{
						sCom = text2;
						return true;
					}
				}
			}
		}
		return false;
	}

	public static List<Common.Table_Code> GetDomainList()
	{
		List<Common.Table_Code> list = new List<Common.Table_Code>();
		try
		{
			string path = Environment.CurrentDirectory + "\\Certificate";
			if (Directory.Exists(path))
			{
				string[] files = Directory.GetFiles(path, "*.txt");
				foreach (string path2 in files)
				{
					try
					{
						using StreamReader streamReader = new StreamReader(path2, Encoding.UTF8);
						streamReader.ReadLine();
						streamReader.ReadLine();
						streamReader.ReadLine();
						streamReader.ReadLine();
						streamReader.ReadLine();
						streamReader.ReadLine();
						string domain = Function.Decrypt(RSA.PrivateKey, streamReader.ReadLine());
						if (domain != "-12345678" && !list.Any((Common.Table_Code d) => d.Code == domain))
						{
							list.Add(new Common.Table_Code
							{
								Code = domain,
								Name = domain
							});
						}
					}
					catch
					{
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return list;
	}

	public static List<Common.Table_Code> GetDataPriceSource()
	{
		return new List<Common.Table_Code>
		{
			new Common.Table_Code
			{
				Name = "Special Prices for Business Partner",
				Code = "Y"
			},
			new Common.Table_Code
			{
				Name = "Manual",
				Code = "N"
			},
			new Common.Table_Code
			{
				Name = "Active Price List, Discount Groups",
				Code = "W"
			},
			new Common.Table_Code
			{
				Name = "Active Price List",
				Code = "R"
			},
			new Common.Table_Code
			{
				Name = "Inactive Price List",
				Code = "U"
			},
			new Common.Table_Code
			{
				Name = "Blanket Agreement",
				Code = "A"
			},
			new Common.Table_Code
			{
				Name = "Period and Volume Discounts",
				Code = "P"
			},
			new Common.Table_Code
			{
				Name = "Period and Volume Discounts, Discount Groups",
				Code = "Q"
			},
			new Common.Table_Code
			{
				Name = "Inactive Price List, Discount Groups",
				Code = "V"
			}
		};
	}

	public static System.Data.DataSet ReadExcelToDataTable(string path, string extension, bool hasHeader = true)
	{
		try
		{
			System.Data.DataSet result = new System.Data.DataSet();
			ExcelDataSetConfiguration configuration = new ExcelDataSetConfiguration
			{
				ConfigureDataTable = (IExcelDataReader _) => new ExcelDataTableConfiguration
				{
					UseHeaderRow = hasHeader
				}
			};
			using (FileStream fileStream = File.Open(path, FileMode.Open, FileAccess.Read))
			{
				switch (extension)
				{
				case ".xlsx":
				case ".xlsm":
				{
					using (IExcelDataReader self2 = ExcelReaderFactory.CreateOpenXmlReader(fileStream))
					{
						result = self2.AsDataSet(configuration);
					}
					break;
				}
				case ".xls":
				{
					using (IExcelDataReader self = ExcelReaderFactory.CreateBinaryReader(fileStream))
					{
						result = self.AsDataSet(configuration);
					}
					break;
				}
				}
			}
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new System.Data.DataSet();
		}
	}
}
