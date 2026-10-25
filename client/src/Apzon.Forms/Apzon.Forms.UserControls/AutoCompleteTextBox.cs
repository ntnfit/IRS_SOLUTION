using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Controls;
using DevExpress.XtraEditors;

namespace Apzon.Forms.UserControls;

[ToolboxItem(true)]
public class AutoCompleteTextBox : ButtonEdit, IMessageFilter
{
	private ListBox _listBox;

	private bool _isAdded;

	private string[] _values;

	public Dictionary<string, List<char>> AdappterText = new Dictionary<string, List<char>>();

	public char[] LstCharacter = new List<char>
	{
		'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p',
		'a', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'z',
		'x', 'c', 'v', 'b', 'n', 'm'
	}.ToArray();

	public bool IsSearchControl { get; set; }

	public string[] Values
	{
		get
		{
			return _values;
		}
		set
		{
			_values = value;
		}
	}

	public List<string> SelectedValues => new List<string>(Text.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

	public AutoCompleteTextBox()
	{
		InitializeComponent();
		_listBox.Visible = false;
		Application.AddMessageFilter(this);
		_listBox.HandleDestroyed += (object sender, EventArgs args) =>
		{
			Application.RemoveMessageFilter(this);
		};
		AdappterText.Add("a", new List<char>
		{
			'a', 'á', 'à', 'ạ', 'ã', 'ả', 'ă', 'ắ', 'ằ', 'ặ',
			'ẵ', 'ẳ', 'â', 'ấ', 'ầ', 'ậ', 'ẫ', 'ẩ'
		});
		AdappterText.Add("d", new List<char> { 'd', 'đ' });
		AdappterText.Add("e", new List<char>
		{
			'e', 'é', 'è', 'ẹ', 'ẽ', 'ẻ', 'ê', 'ế', 'ề', 'ệ',
			'ễ', 'ể'
		});
		AdappterText.Add("i", new List<char> { 'i', 'ì', 'í', 'ị', 'ĩ', 'ỉ' });
		AdappterText.Add("o", new List<char>
		{
			'o', 'ò', 'ó', 'ọ', 'õ', 'ỏ', 'ơ', 'ờ', 'ớ', 'ợ',
			'ỡ', 'ở', 'ô', 'ồ', 'ố', 'ộ', 'ỗ', 'ổ'
		});
		AdappterText.Add("u", new List<char>
		{
			'u', 'ù', 'ú', 'ụ', 'ũ', 'ủ', 'ư', 'ứ', 'ừ', 'ự',
			'ữ', 'ử'
		});
		AdappterText.Add("y", new List<char> { 'y', 'ỳ', 'ý', 'ỵ', 'ỹ', 'ỷ' });
	}

	private void InitializeComponent()
	{
		this._listBox = new System.Windows.Forms.ListBox();
		this._listBox.ItemHeight = 50;
		base.KeyDown += new System.Windows.Forms.KeyEventHandler(this.this_KeyDown);
		base.KeyUp += new System.Windows.Forms.KeyEventHandler(this.this_KeyUp);
		this._listBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ListBox_MouseHover);
		this._listBox.Click += new System.EventHandler(this.ListBox_Click);
	}

	private void ListBox_MouseHover(object sender, EventArgs e)
	{
		Point p = _listBox.PointToClient(Cursor.Position);
		int num = _listBox.IndexFromPoint(p);
		if (num >= 0)
		{
			_listBox.SelectedIndex = num;
		}
	}

	private void ShowListBox()
	{
		if (!_isAdded)
		{
			Parent.Controls.Add(_listBox);
			_isAdded = true;
		}
		_listBox.Left = Left;
		_listBox.Top = Top + Height;
		_listBox.Visible = true;
		_listBox.BringToFront();
	}

	private void this_KeyUp(object sender, KeyEventArgs e)
	{
		if (e.KeyCode != Keys.Down && e.KeyCode != Keys.Up)
		{
			UpdateListBox();
		}
	}

	private void ListBox_Click(object sender, EventArgs e)
	{
		try
		{
			Text = Function.ToString(_listBox.SelectedItem);
			_listBox.Visible = false;
			MethodInfo[] methods = typeof(Control).GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic);
			KeyEventArgs e2 = new KeyEventArgs(Keys.Return);
			List<int> listIndexKeyDown = methods.GetListIndexKeyDown();
			for (int i = 0; i < listIndexKeyDown.Count; i++)
			{
				methods[listIndexKeyDown[i]].Invoke(this, new object[1] { e2 });
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void this_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			switch (e.KeyCode)
			{
			case Keys.Tab:
			case Keys.Return:
				if (_listBox.Visible)
				{
					Text = _listBox.SelectedItem.ToString();
					_listBox.Visible = false;
					Select(Text.Length, 0);
					e.Handled = true;
				}
				break;
			case Keys.Down:
				if (_listBox.Visible && _listBox.SelectedIndex < _listBox.Items.Count - 1)
				{
					_listBox.SelectedIndex++;
				}
				e.Handled = true;
				break;
			case Keys.Up:
				if (_listBox.Visible && _listBox.SelectedIndex > 0)
				{
					_listBox.SelectedIndex--;
				}
				e.Handled = true;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	protected override bool IsInputKey(Keys keyData)
	{
		if (keyData == Keys.Tab)
		{
			if (_listBox.Visible)
			{
				return true;
			}
			return false;
		}
		return base.IsInputKey(keyData);
	}

	private void UpdateListBox()
	{
		string text = Text;
		if (_values != null && text.Length > 0)
		{
			GetResultComplete(text);
			if (_listBox.Items.Count > 0)
			{
				ShowListBox();
				_listBox.SelectedIndex = 0;
				int num = Width;
				int num2 = 0;
				using (Graphics graphics = _listBox.CreateGraphics())
				{
					for (int i = 0; i < _listBox.Items.Count; i++)
					{
						if (i < 20)
						{
							num2 += _listBox.GetItemHeight(i);
						}
						int num3 = (int)graphics.MeasureString((string)_listBox.Items[i] + "_", _listBox.Font).Width;
						if (num < num3)
						{
							num = num3;
						}
					}
				}
				num2 += 20;
				_listBox.Height = num2;
				_listBox.Width = ((num > ClientSize.Width) ? ClientSize.Width : num);
			}
			else
			{
				_listBox.Visible = false;
			}
		}
		else
		{
			_listBox.Visible = false;
		}
		Focus();
	}

	public bool PreFilterMessage(ref Message m)
	{
		if (m.Msg == 513 && _listBox.Visible && !ClientRectangle.Contains(PointToClient(Control.MousePosition)) && !_listBox.ClientRectangle.Contains(_listBox.PointToClient(Control.MousePosition)))
		{
			_listBox.Visible = false;
		}
		return false;
	}

	public void GetResultComplete(string compareText)
	{
		compareText = compareText.Trim().ToLower();
		if (string.IsNullOrEmpty(compareText))
		{
			_listBox = new ListBox();
			return;
		}
		string[] matches = Array.FindAll(_values, (string x) => GetStandardName(x).Contains(GetStandardName(compareText)));
		string[] array = Array.FindAll(_values, (string x) => GetPrefixName(x).Contains(GetStandardName(compareText)) && !matches.Contains(x));
		_listBox.BeginUpdate();
		_listBox.Items.Clear();
		Array.ForEach(matches, (string x) =>
		{
			_listBox.Items.Add(x);
		});
		Array.ForEach(array, (string x) =>
		{
			_listBox.Items.Add(x);
		});
		_listBox.EndUpdate();
	}

	public string GetPrefixName(string name)
	{
		name = GetStandardName(name);
		if (string.IsNullOrEmpty(name))
		{
			return "";
		}
		string[] array = name.Split(' ');
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < array.Length; i++)
		{
			if (!string.IsNullOrEmpty(array[i]) && array[i] != "-")
			{
				stringBuilder.Append(array[i].Substring(0, 1));
			}
		}
		return stringBuilder.ToString();
	}

	private string GetStandardName(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return "";
		}
		char[] array = name.ToLower().ToCharArray();
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < array.Length; i++)
		{
			if (LstCharacter.Contains(array[i]))
			{
				stringBuilder.Append(array[i]);
				continue;
			}
			bool flag = false;
			foreach (string key in AdappterText.Keys)
			{
				if (AdappterText[key].Contains(array[i]))
				{
					stringBuilder.Append(key);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				stringBuilder.Append(array[i]);
			}
		}
		return stringBuilder.ToString();
	}

	public void CloseAutoCompeteBox()
	{
		_listBox.Visible = false;
	}

	protected override void Dispose(bool disposing)
	{
		Application.RemoveMessageFilter(this);
		base.Dispose(disposing);
	}
}
