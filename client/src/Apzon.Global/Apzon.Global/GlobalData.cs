using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.DMS;

namespace Apzon.Global;

public static class GlobalData
{
	public static string DEFAULT_LANGUAGE = "en-US";

	public static string CURRENTLANGUAGE = DEFAULT_LANGUAGE;

	public static List<AuthsImpl> PublicAccessForm = new List<AuthsImpl>
	{
		new AuthsImpl
		{
			MenuType = "F",
			USER_CODE = "",
			authoriza = "F",
			menuName = "Import Excel",
			menucode = "frm_ReadExcelToGrid"
		}
	};

	private static GeneralSettingImpl _generalSetting;

	private static CompanyInfoImpl _companyInfo;

	public static Dictionary<Enums.MessageCode, string> MessageDataCenter = new Dictionary<Enums.MessageCode, string>();

	public static string ApiToken;

	public static Dictionary<string, DataSet> FormSettings = new Dictionary<string, DataSet>();

	public static List<string> ListCFLItemField;

	public static DataTable Dimensions;

	public static bool Updating;

	public static List<Tuple<string, string>> ListDocUDFNameAndType;

	public static Dictionary<string, string> ListLineUDFNameAndType;

	public static DataTable CountryOrg;

	public static DataTable CompanyDetail;

	public static int ColumnCountDashboard;

	public static bool LoginSap { get; set; }

	public static DateTime SystemDate { get; set; }

	public static string _ItemCode { get; set; }

	public static int UserID { get; set; }

	public static string OldPassword { get; set; }

	public static string UserName { get; set; }

	public static string UserCodeLogin { get; set; }

	public static string UserNameLogin { get; set; }

	public static string DeparmentLogin { get; set; }

	public static string BranchLogin { get; set; }

	public static string UserSignLogin { get; set; }

	public static bool IsRelogin { get; set; }

	public static int AlertTime { get; set; }

	public static int ScreenLock { get; set; }

	public static List<Common.ListItems> FilterSelect { get; set; }

	public static UserDefaultGroupImpl UserDefaultGroup { get; set; }

	public static Function.ConfigFTPServer FtpConfig { get; set; }

	public static string Domain { get; set; }

	public static string DatabaseName { get; set; }

	public static string CompanyName { get; set; }

	public static string SessionId { get; set; }

	public static string TimeFormat { get; set; }

	public static string DateTimeFormat { get; set; }

	public static string DateSeparatorFormat { get; set; }

	public static NumberFormatInfo TotalsAccuracyFormat { get; set; }

	public static NumberFormatInfo QuantitiesFormat { get; set; }

	public static NumberFormatInfo UnitPriceFormat { get; set; }

	public static NumberFormatInfo RateAccuracyFormat { get; set; }

	public static NumberFormatInfo PercentageRateFormat { get; set; }

	public static NumberFormatInfo MeasureFormat { get; set; }

	public static List<Common.Table_Code> Currencies { get; set; }

	public static List<CurrencyCodesImpl> CurrencyCodes { get; set; }

	public static string MainCurrency { get; set; }

	public static string SystemCurrency { get; set; }

	public static string CurrencySystemFormat { get; set; }

	public static string CardCodeDK { get; set; }

	public static string DefaultWhs { get; set; }

	public static List<AuthsImpl> ListAccessForm { get; set; }

	public static DataTable DtAccessForm { get; set; }

	public static string[] InvisibleOnSideMenu => new string[2] { "frm_FormSetting", "frm_ReportFormSettings" };

	public static List<Common.ListItems> ReportModified { get; set; }

	public static int DefaultBranch { get; set; }

	public static bool IsMultiBranch { get; set; }

	public static string ICfg_OppByUser { get; set; }

	public static string ICfg_OppForecast { get; set; }

	public static string ICfg_OppPotential { get; set; }

	public static string DocSettingsClosedQuot { get; set; }

	public static DataTable BranchList { get; set; }

	public static DataTable SystemMessageLog { get; set; }

	public static DataTable AllModule { get; set; }

	public static string StoreCode { get; set; }

	public static int POSID { get; set; }

	public static GeneralSettingImpl GeneralSetting
	{
		get
		{
			return _generalSetting;
		}
		set
		{
			_generalSetting = value ?? new GeneralSettingImpl();
		}
	}

	public static CompanyInfoImpl CompanyInfo
	{
		get
		{
			return _companyInfo;
		}
		set
		{
			_companyInfo = value ?? new CompanyInfoImpl();
		}
	}

	public static UserLicenseImpl UserLicense { get; set; }

	public static int ExchangeRatePow { get; set; }

	public static GlobalNotiSettingImpl GlobalNotiSetting { get; set; }

	public static DataTable FormWizard { get; set; }

	public static List<WeightImpl> WeightUnits { get; set; }

	public static List<LengthAndWidthImpl> LengthWidthUnits { get; set; }

	public static string SuperUser { get; set; }

	public static List<MapMarker> MapMarkers { get; set; }

	public static DataSet AdditionalGeneralSettings { get; set; }
}
