using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Apzon.Commons.Properties;

[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
public class ContactEmployees
{
	private static ResourceManager resourceMan;

	private static CultureInfo resourceCulture;

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static ResourceManager ResourceManager
	{
		get
		{
			if (resourceMan == null)
			{
				resourceMan = new ResourceManager("Apzon.Commons.Properties.ContactEmployees", typeof(ContactEmployees).Assembly);
			}
			return resourceMan;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static CultureInfo Culture
	{
		get
		{
			return resourceCulture;
		}
		set
		{
			resourceCulture = value;
		}
	}

	public static string Active => ResourceManager.GetString("Active", resourceCulture);

	public static string Address => ResourceManager.GetString("Address", resourceCulture);

	public static string BirthCity => ResourceManager.GetString("BirthCity", resourceCulture);

	public static string BirthDate => ResourceManager.GetString("BirthDate", resourceCulture);

	public static string BirthPlace => ResourceManager.GetString("BirthPlace", resourceCulture);

	public static string BirthState => ResourceManager.GetString("BirthState", resourceCulture);

	public static string CardCode => ResourceManager.GetString("CardCode", resourceCulture);

	public static string Cellolar => ResourceManager.GetString("Cellolar", resourceCulture);

	public static string CntctCode => ResourceManager.GetString("CntctCode", resourceCulture);

	public static string DataSource => ResourceManager.GetString("DataSource", resourceCulture);

	public static string E_MailL => ResourceManager.GetString("E_MailL", resourceCulture);

	public static string EmlGrpCode => ResourceManager.GetString("EmlGrpCode", resourceCulture);

	public static string Fax => ResourceManager.GetString("Fax", resourceCulture);

	public static string FirstName => ResourceManager.GetString("FirstName", resourceCulture);

	public static string Gender => ResourceManager.GetString("Gender", resourceCulture);

	public static string LastName => ResourceManager.GetString("LastName", resourceCulture);

	public static string LogInstanc => ResourceManager.GetString("LogInstanc", resourceCulture);

	public static string MiddleName => ResourceManager.GetString("MiddleName", resourceCulture);

	public static string Name => ResourceManager.GetString("Name", resourceCulture);

	public static string NFeRcpn => ResourceManager.GetString("NFeRcpn", resourceCulture);

	public static string Notes1 => ResourceManager.GetString("Notes1", resourceCulture);

	public static string Notes2 => ResourceManager.GetString("Notes2", resourceCulture);

	public static string ObjType => ResourceManager.GetString("ObjType", resourceCulture);

	public static string Pager => ResourceManager.GetString("Pager", resourceCulture);

	public static string Password => ResourceManager.GetString("Password", resourceCulture);

	public static string Position => ResourceManager.GetString("Position", resourceCulture);

	public static string Profession => ResourceManager.GetString("Profession", resourceCulture);

	public static string ResidCity => ResourceManager.GetString("ResidCity", resourceCulture);

	public static string ResidCntry => ResourceManager.GetString("ResidCntry", resourceCulture);

	public static string ResidState => ResourceManager.GetString("ResidState", resourceCulture);

	public static string Tel1 => ResourceManager.GetString("Tel1", resourceCulture);

	public static string Tel2 => ResourceManager.GetString("Tel2", resourceCulture);

	public static string Title => ResourceManager.GetString("Title", resourceCulture);

	public static string updateDate => ResourceManager.GetString("updateDate", resourceCulture);

	public static string updateTime => ResourceManager.GetString("updateTime", resourceCulture);

	public static string UserSign => ResourceManager.GetString("UserSign", resourceCulture);

	internal ContactEmployees()
	{
	}
}
