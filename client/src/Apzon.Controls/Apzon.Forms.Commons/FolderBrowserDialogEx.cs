using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms;

namespace Apzon.Forms.Commons;

public class FolderBrowserDialogEx : CommonDialog
{
	private class CSIDL
	{
		public const int PRINTERS = 4;

		public const int NETWORK = 18;
	}

	private class BrowseFlags
	{
		public const int BIF_DEFAULT = 0;

		public const int BIF_BROWSEFORCOMPUTER = 4096;

		public const int BIF_BROWSEFORPRINTER = 8192;

		public const int BIF_BROWSEINCLUDEFILES = 16384;

		public const int BIF_BROWSEINCLUDEURLS = 128;

		public const int BIF_DONTGOBELOWDOMAIN = 2;

		public const int BIF_EDITBOX = 16;

		public const int BIF_NEWDIALOGSTYLE = 64;

		public const int BIF_NONEWFOLDERBUTTON = 512;

		public const int BIF_RETURNFSANCESTORS = 8;

		public const int BIF_RETURNONLYFSDIRS = 1;

		public const int BIF_SHAREABLE = 32768;

		public const int BIF_STATUSTEXT = 4;

		public const int BIF_UAHINT = 256;

		public const int BIF_VALIDATE = 32;

		public const int BIF_NOTRANSLATETARGETS = 1024;
	}

	private static class BrowseForFolderMessages
	{
		public const int BFFM_INITIALIZED = 1;

		public const int BFFM_SELCHANGED = 2;

		public const int BFFM_VALIDATEFAILEDA = 3;

		public const int BFFM_VALIDATEFAILEDW = 4;

		public const int BFFM_IUNKNOWN = 5;

		public const int BFFM_SETSTATUSTEXT = 1124;

		public const int BFFM_ENABLEOK = 1125;

		public const int BFFM_SETSELECTIONA = 1126;

		public const int BFFM_SETSELECTIONW = 1127;
	}

	private static readonly int MAX_PATH = 260;

	private PInvoke.BrowseFolderCallbackProc _callback;

	private string _descriptionText;

	private Environment.SpecialFolder _rootFolder;

	private string _selectedPath;

	private bool _selectedPathNeedsCheck;

	private bool _showNewFolderButton;

	private bool _showEditBox;

	private bool _showBothFilesAndFolders;

	private bool _newStyle = true;

	private bool _showFullPathInEditBox = true;

	private bool _dontIncludeNetworkFoldersBelowDomainLevel;

	private int _uiFlags;

	private IntPtr _hwndEdit;

	private IntPtr _rootFolderLocation;

	public bool IsSearchControl { get; set; }

	public string Description
	{
		get
		{
			return _descriptionText;
		}
		set
		{
			_descriptionText = ((value == null) ? string.Empty : value);
		}
	}

	public Environment.SpecialFolder RootFolder
	{
		get
		{
			return _rootFolder;
		}
		set
		{
			if (!Enum.IsDefined(typeof(Environment.SpecialFolder), value))
			{
				throw new InvalidEnumArgumentException("value", (int)value, typeof(Environment.SpecialFolder));
			}
			_rootFolder = value;
		}
	}

	public string SelectedPath
	{
		get
		{
			if (_selectedPath != null && _selectedPath.Length != 0 && _selectedPathNeedsCheck)
			{
				new FileIOPermission(FileIOPermissionAccess.PathDiscovery, _selectedPath).Demand();
				_selectedPathNeedsCheck = false;
			}
			return _selectedPath;
		}
		set
		{
			_selectedPath = ((value == null) ? string.Empty : value);
			_selectedPathNeedsCheck = true;
		}
	}

	public bool ShowNewFolderButton
	{
		get
		{
			return _showNewFolderButton;
		}
		set
		{
			_showNewFolderButton = value;
		}
	}

	public bool ShowEditBox
	{
		get
		{
			return _showEditBox;
		}
		set
		{
			_showEditBox = value;
		}
	}

	public bool NewStyle
	{
		get
		{
			return _newStyle;
		}
		set
		{
			_newStyle = value;
		}
	}

	public bool DontIncludeNetworkFoldersBelowDomainLevel
	{
		get
		{
			return _dontIncludeNetworkFoldersBelowDomainLevel;
		}
		set
		{
			_dontIncludeNetworkFoldersBelowDomainLevel = value;
		}
	}

	public bool ShowFullPathInEditBox
	{
		get
		{
			return _showFullPathInEditBox;
		}
		set
		{
			_showFullPathInEditBox = value;
		}
	}

	public bool ShowBothFilesAndFolders
	{
		get
		{
			return _showBothFilesAndFolders;
		}
		set
		{
			_showBothFilesAndFolders = value;
		}
	}

	public new event EventHandler HelpRequest
	{
		add
		{
			base.HelpRequest += value;
		}
		remove
		{
			base.HelpRequest -= value;
		}
	}

	public FolderBrowserDialogEx()
	{
		Reset();
	}

	public static FolderBrowserDialogEx PrinterBrowser()
	{
		FolderBrowserDialogEx folderBrowserDialogEx = new FolderBrowserDialogEx();
		folderBrowserDialogEx.BecomePrinterBrowser();
		return folderBrowserDialogEx;
	}

	public static FolderBrowserDialogEx ComputerBrowser()
	{
		FolderBrowserDialogEx folderBrowserDialogEx = new FolderBrowserDialogEx();
		folderBrowserDialogEx.BecomeComputerBrowser();
		return folderBrowserDialogEx;
	}

	private void BecomePrinterBrowser()
	{
		_uiFlags += 8192;
		Description = "Select a printer:";
		PInvoke.Shell32.SHGetSpecialFolderLocation(IntPtr.Zero, 4, ref _rootFolderLocation);
		ShowNewFolderButton = false;
		ShowEditBox = false;
	}

	private void BecomeComputerBrowser()
	{
		_uiFlags += 4096;
		Description = "Select a computer:";
		PInvoke.Shell32.SHGetSpecialFolderLocation(IntPtr.Zero, 18, ref _rootFolderLocation);
		ShowNewFolderButton = false;
		ShowEditBox = false;
	}

	private int FolderBrowserCallback(IntPtr hwnd, int msg, IntPtr lParam, IntPtr lpData)
	{
		switch (msg)
		{
		case 1:
			if (_selectedPath.Length != 0)
			{
				PInvoke.User32.SendMessage(new HandleRef(null, hwnd), 1127, 1, _selectedPath);
				if (_showEditBox && _showFullPathInEditBox)
				{
					_hwndEdit = PInvoke.User32.FindWindowEx(new HandleRef(null, hwnd), IntPtr.Zero, "Edit", null);
					PInvoke.User32.SetWindowText(_hwndEdit, _selectedPath);
				}
			}
			break;
		case 2:
		{
			if (!(lParam != IntPtr.Zero))
			{
				break;
			}
			if ((_uiFlags & 0x2000) == 8192 || (_uiFlags & 0x1000) == 4096)
			{
				PInvoke.User32.SendMessage(new HandleRef(null, hwnd), 1125, 0, 1);
				break;
			}
			IntPtr intPtr = Marshal.AllocHGlobal(MAX_PATH * Marshal.SystemDefaultCharSize);
			bool flag = PInvoke.Shell32.SHGetPathFromIDList(lParam, intPtr);
			string text = Marshal.PtrToStringAuto(intPtr);
			Marshal.FreeHGlobal(intPtr);
			PInvoke.User32.SendMessage(new HandleRef(null, hwnd), 1125, 0, flag ? 1 : 0);
			if (flag && !string.IsNullOrEmpty(text))
			{
				if (_showEditBox && _showFullPathInEditBox && _hwndEdit != IntPtr.Zero)
				{
					PInvoke.User32.SetWindowText(_hwndEdit, text);
				}
				if ((_uiFlags & 4) == 4)
				{
					PInvoke.User32.SendMessage(new HandleRef(null, hwnd), 1124, 0, text);
				}
			}
			break;
		}
		}
		return 0;
	}

	private static PInvoke.IMalloc GetSHMalloc()
	{
		PInvoke.IMalloc[] array = new PInvoke.IMalloc[1];
		PInvoke.Shell32.SHGetMalloc(array);
		return array[0];
	}

	public override void Reset()
	{
		_rootFolder = Environment.SpecialFolder.Desktop;
		_descriptionText = string.Empty;
		_selectedPath = string.Empty;
		_selectedPathNeedsCheck = false;
		_showNewFolderButton = true;
		_showEditBox = true;
		_newStyle = true;
		_dontIncludeNetworkFoldersBelowDomainLevel = false;
		_hwndEdit = IntPtr.Zero;
		_rootFolderLocation = IntPtr.Zero;
	}

	protected override bool RunDialog(IntPtr hWndOwner)
	{
		bool result = false;
		if (_rootFolderLocation == IntPtr.Zero)
		{
			PInvoke.Shell32.SHGetSpecialFolderLocation(hWndOwner, (int)_rootFolder, ref _rootFolderLocation);
			if (_rootFolderLocation == IntPtr.Zero)
			{
				PInvoke.Shell32.SHGetSpecialFolderLocation(hWndOwner, 0, ref _rootFolderLocation);
				if (_rootFolderLocation == IntPtr.Zero)
				{
					throw new InvalidOperationException("FolderBrowserDialogNoRootFolder");
				}
			}
		}
		_hwndEdit = IntPtr.Zero;
		if (_dontIncludeNetworkFoldersBelowDomainLevel)
		{
			_uiFlags += 2;
		}
		if (_newStyle)
		{
			_uiFlags += 64;
		}
		if (!_showNewFolderButton)
		{
			_uiFlags += 512;
		}
		if (_showEditBox)
		{
			_uiFlags += 16;
		}
		if (_showBothFilesAndFolders)
		{
			_uiFlags += 16384;
		}
		if (Control.CheckForIllegalCrossThreadCalls && Application.OleRequired() != ApartmentState.STA)
		{
			throw new ThreadStateException("DebuggingException: ThreadMustBeSTA");
		}
		IntPtr intPtr = IntPtr.Zero;
		IntPtr intPtr2 = IntPtr.Zero;
		IntPtr intPtr3 = IntPtr.Zero;
		try
		{
			PInvoke.BROWSEINFO bROWSEINFO = new PInvoke.BROWSEINFO();
			intPtr2 = Marshal.AllocHGlobal(MAX_PATH * Marshal.SystemDefaultCharSize);
			intPtr3 = Marshal.AllocHGlobal(MAX_PATH * Marshal.SystemDefaultCharSize);
			_callback = FolderBrowserCallback;
			bROWSEINFO.pidlRoot = _rootFolderLocation;
			bROWSEINFO.Owner = hWndOwner;
			bROWSEINFO.pszDisplayName = intPtr2;
			bROWSEINFO.Title = _descriptionText;
			bROWSEINFO.Flags = _uiFlags;
			bROWSEINFO.callback = _callback;
			bROWSEINFO.lParam = IntPtr.Zero;
			bROWSEINFO.iImage = 0;
			intPtr = PInvoke.Shell32.SHBrowseForFolder(bROWSEINFO);
			if ((_uiFlags & 0x2000) == 8192 || (_uiFlags & 0x1000) == 4096)
			{
				_selectedPath = Marshal.PtrToStringAuto(bROWSEINFO.pszDisplayName);
				result = true;
			}
			else if (intPtr != IntPtr.Zero)
			{
				PInvoke.Shell32.SHGetPathFromIDList(intPtr, intPtr3);
				_selectedPathNeedsCheck = true;
				_selectedPath = Marshal.PtrToStringAuto(intPtr3);
				result = true;
			}
		}
		finally
		{
			PInvoke.IMalloc sHMalloc = GetSHMalloc();
			sHMalloc.Free(_rootFolderLocation);
			_rootFolderLocation = IntPtr.Zero;
			if (intPtr != IntPtr.Zero)
			{
				sHMalloc.Free(intPtr);
			}
			if (intPtr3 != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(intPtr3);
			}
			if (intPtr2 != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(intPtr2);
			}
			_callback = null;
		}
		return result;
	}
}
