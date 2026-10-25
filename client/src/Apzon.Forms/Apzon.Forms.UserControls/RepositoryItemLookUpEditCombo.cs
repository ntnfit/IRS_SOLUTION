using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Editor.MyLookUpEdit;
using Apzon.Controls.Form;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;

namespace Apzon.Forms.UserControls;

[UserRepositoryItem("RegisterLookUpEditCombo")]
public class RepositoryItemLookUpEditCombo : RepositoryItemLookUpEditSimple
{
	private ApzObjectTypes _fromType;

	private ApzObjectTypes _toType;

	private bool _reserve;

	private bool _baseProduction;

	private string _id;

	public new const string CustomEditName = "LookUpEditCombo";

	[DefaultValue(ApzObjectTypes.oNone)]
	public ApzObjectTypes TypeDrillDown
	{
		get
		{
			return _fromType;
		}
		set
		{
			_fromType = value;
			_toType = value;
		}
	}

	public override string EditorTypeName => "LookUpEditCombo";

	static RepositoryItemLookUpEditCombo()
	{
		RegisterLookUpEditCombo();
	}

	public void DrillDownBy(ApzObjectTypes fromType, ApzObjectTypes toType, bool reserve = false, bool baseProduction = false, string id = "")
	{
		_fromType = fromType;
		_toType = toType;
		_id = id;
		_reserve = reserve;
		_baseProduction = baseProduction;
	}

	public RepositoryItemLookUpEditCombo()
	{
		SerializableAppearanceObject serializableAppearanceObject = new SerializableAppearanceObject();
		Buttons.Add(new EditorButton(ButtonPredefines.Glyph, "", -1, enabled: true, visible: true, isLeft: true, ImageLocation.MiddleCenter, Resources.right, new KeyShortcut(Keys.None), serializableAppearanceObject, "", null, null, enableImageTransparency: true));
		ButtonPressed += RepositoryItemLookUpEditCombo_ButtonPressed;
	}

	private void RepositoryItemLookUpEditCombo_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Kind == ButtonPredefines.Glyph)
			{
				string text = Function.ToString((sender as LookUpEditCombo).EditValue);
				if (!(text == "0") && !(text == "") && _fromType != ApzObjectTypes.oNone && (sender as Control).FindForm() is XtraFormEx)
				{
					((sender as Control).FindForm() as XtraFormEx).DrillDown(_fromType, _toType, text, _reserve, _baseProduction, _id);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public static void RegisterLookUpEditCombo()
	{
		Image image = null;
		EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo("LookUpEditCombo", typeof(LookUpEditCombo), typeof(RepositoryItemLookUpEditCombo), typeof(LookUpEditComboViewInfo), new LookUpEditComboPainter(), designTimeVisible: true, image));
	}

	public override void Assign(RepositoryItem item)
	{
		BeginUpdate();
		try
		{
			base.Assign(item);
			_ = item is RepositoryItemLookUpEditCombo;
		}
		finally
		{
			EndUpdate();
		}
	}
}
