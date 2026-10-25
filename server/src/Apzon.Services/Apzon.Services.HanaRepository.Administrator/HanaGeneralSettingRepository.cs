using System;
using System.Data;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Repository.Administrator;

namespace Apzon.Services.HanaRepository.Administrator;

public class HanaGeneralSettingRepository : GeneralSettingRepository
{
	public HanaGeneralSettingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public override DataSet GetData(int userId)
	{
		try
		{
			string query = $"SELECT \"CreditLimt\", \"ObligLimit\", \"AddDlnBlnc\", \"SalesLimit\", \"DlnLimit\", \"OrderLimit\", \"PickLimit\", \"SlpCommUse\", \"ItmCommUse\", \"CrdCommUse\", \r\n                        \"DfCustTerm\", \"DfVendTerm\", \"DfltCustPM\", \"DfltVendPM\", \"CreditDpst\", \"useDocWrf\", \"EnbApprDI\", \"DspFrznBP\", b.\"DspIBPDoc\", \"MBAOnPer\", \"DoBudget\", \r\n                        \"BgtBlock\", \"BgtWarning\", \"BdgtPRQDOC\", \"BdgtPORDoc\", \"BdgtPDNDoc\", \"BdgtAcctng\", \"MapService\", \"MaxHistory\", \"ConsumeMtd\", \"DaysBack\", \"DaysFwrd\", \r\n                        \"SimReport\", \"Color\", \"DefLengthU\", \"DefWeightU\", \"TimeFormat\", \"DateFormat\", \"DateSep\", \"SumDec\", \"PriceDec\", \"RateDec\", \"QtyDec\", \"PercentDec\", \r\n                        \"MeasureDec\", \"QueryDec\", \"DecSep\", \"ThousSep\", \"CurOnRight\", \"DirectRate\", \"TPLId\", \"TxtSrch\", \"DspFrznITM\", b.\"DspIITMDoc\", \"OneBOneRec\", \r\n                        \"AutoITW\", \"WTLiable\", \"AutoAddUoM\", \"AutoAddPkg\", \"ConsumeFCT\", \"AutoResWhs\", \"UseMltDims\", \"ApyDRinPEC\", \"ApyPRinPEC\", \"ApyBsActPV\", \r\n                        \"ApyBsActSP\", \"ApyBsActPL\", \"SIPLReport\", \"SIPLDoc\", \"SIPLSeting\", \"ClnZeroPln\", \"IsUpdNstdB\", \"DfltByEml\", \"MDStyle\", \"IgrAllCash\", \"CfwAsnMust\", \r\n                        \"ExcelPath\", \"PrjBlock\", \"DftResWhs\", \"StartFrom\", \"Months\", \"Days\", \"XmlPath\", \"SriUniqFld\", \"BtchStatus\", \"DfltWhs\", \"GLMethod\", \"PriceProcM\", \r\n                        \"CfwInDflt\", \"CfwOutDflt\", \"DfSVatItem\", \"DfSVatServ\", \"DfPVatItem\", \"DfPVatServ\",\"DriDownBOM\"\r\n                        FROM OADM a INNER JOIN ADM1 b ON b.\"Code\" = a.\"Code\";\r\n                        SELECT \"CheckFiles\", \"DsplyRates\", \"RcrFlag\", \"RclFlag\", \"ContactLog\", \"ShowNewMsg\", \"OpenCdt\", \"OpenDps\", \"ShowNewTsk\", \"AlertPolFr\", \"ScreenLock\", \r\n                        \"OpenCredit\", \"AdvImagePr\", \"WllPprDsp\", \"WallPaper\"  FROM OUSR WHERE USERID = {userId};\r\n                        SELECT \"IMCEEnable\", \"SPNEnabled\", \"MaxChoose\", \"B1SgtEb\", \"AutoCrIns\", \"PrsWkCntEb\", \"SriMngSys\", \"SriCreatIn\" FROM CINF;\r\n                        SELECT \"WordPath\", \"ExtPath\", \"BitmapPath\", \"AttachPath\" FROM OADP;\r\n                        SELECT \"DisableSplitFolder\" FROM APZ_OADM;";
			DataSet dataSet = ExecuteData(query, CommandType.Text);
			if (dataSet.Tables.Count > 0)
			{
				return dataSet;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetFormatString FAILED ", ex.Message);
		}
		return new DataSet();
	}

	public override string GetSaveATC2Query(string tempTable)
	{
		return "EXEC 'DELETE FROM APZ_ATC2'; \r\n                     EXEC 'INSERT INTO APZ_ATC2(\"ObjType\",\"Path\") SELECT * FROM " + tempTable + "';";
	}
}
