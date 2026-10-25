using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Administrator.Setup;

namespace Apzon.Services.Repository.Administrator;

public class GeneralSettingRepository(IDatabaseService databaseService) : BaseService<GeneralSettingImpl>(databaseService), IGeneralSetting, IService<GeneralSettingImpl>
{
	private DataTable dtATC2 = new DataTable();

	private const string TempTable = "#APZ_ATC2";

	public DataTable ShowGeneralSetting()
	{
		DataTable result = new DataTable();
		try
		{
			string query = "select a.Code,a.CompnyName,a.CompnyAddr,a.intrnAdrs,a.PrintHeadr,a.Manager,a.AliasName,\r\n                                   a.Phone1,a.Phone2,a.Fax, ISNULL( a.E_Mail,'') as E_Mail, a.TaxIdNum,a.TaxAddr,a.Logo,\r\n\t                               a.CodeP,a.CompnyNameP,a.intrnAdrsP,a.PrintHeadrP,a.ManagerP,a.AliasNameP,a.Phone1P,a.Phone2P,\r\n\t                               a.FaxP,ISNULL( a.E_MailP,'') as E_MailP,a.TaxIdNumP,a.TaxAddrP,\r\n\t                               a.HldCode,a.MainCurncy,a.SysCurrncy, \r\n\t                               case when a.DfActCurr ='Y' then 'All Currencies'\r\n\t\t\t                            when a.DfActCurr='N' then 'Local Currency'\r\n\t                               end DfActCurr,\r\n                                   case when a.InvntSystm='F' then 'FIFO'\r\n\t\t                               when a.InvntSystm='S' then 'Standard'\r\n\t\t                               when a.InvntSystm='A' then 'Moving Averager'\r\n\t                               end InvntSystm,\r\n\t                               a.DfWhsCode,a.DefaultCustomer,\r\n\t                               case when a.TypeDisc ='A' then 'Discount after tax'\r\n\t\t\t                            when a.TypeDisc ='B' then 'Discount before tax'\r\n\t\t                            end TypeDisc,\r\n\t\t                            case when a.TimeFormat ='0' then '24H'\r\n\t\t\t                            when a.TimeFormat ='1' then '12H'\r\n\t\t                            end TimeFormat,\r\n                                   case when a.DateFormat = '0' then N'dd-MM-yyyy'\r\n\t\t\t                           when a.DateFormat ='1' then N'MM-dd-yyyy'\r\n\t\t\t                           when a.DateFormat =  '2' then  N'yy/MM/dd' \r\n\t\t\t                           when a.DateFormat ='3' then  N'yyyy-MM-dd'\r\n\t\t\t                            when a.DateFormat =  '4' then  N'dd-MMM-yy'\r\n\t\t\t                            when a.DateFormat =  '5' then  N'M/d/yyyy'\r\n\t\t\t                            when a.DateFormat = '6' then  N'M/d/yy'\r\n                                      when a.DateFormat = '7' then  N'MM/dd/yy'\r\n                                      when a.DateFormat = '8' then  N'MM/dd/yyyy'\r\n\t\t                           end DateFormat,\r\n\t\t                            a.DateSep, a.SumDec,a.QtyDec,a.PriceDec,a.RateDec,a.PercentDec,a.DecSep,a.ThousSep,a.CompnyAddrP\r\n                            from OADM a ";
			result = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC  FAILED ", ex.Message);
		}
		return result;
	}

	public bool Check(string code, string codeP)
	{
		try
		{
			string query = "SELECT A.\"Code\" ,A.\"CodeP\"\r\n                            FROM OADM A \r\n                            WHERE A.\"Code\" = '" + code + "' and A.\"CodeP\" = '" + codeP + "' ";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
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

	public string Insert(GeneralSettingImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Update(GeneralSettingImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Remove(GeneralSettingImpl entity)
	{
		throw new NotImplementedException();
	}

	public GeneralSettingImpl GetById(int id)
	{
		throw new NotImplementedException();
	}

	public DataTable GetFormatString()
	{
		DataTable result = new DataTable();
		try
		{
			result = ExecuteDataTable("APZ_GeneralSetting_GetFormatString", CommandType.StoredProcedure, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable GetFileSharingUploadPath()
	{
		DataTable result = new DataTable();
		try
		{
			string query = "SELECT b.\"WordPath\",a.\"ExcelPath\",b.\"BitmapPath\",b.\"AttachPath\",a.\"XmlPath\",c.\"DisableSplitFolder\" FROM OADM a , OADP b, APZ_OADM c;";
			result = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public virtual DataSet GetData(int userId)
	{
		try
		{
			string query = "SELECT\r\n\t--TabBP \r\n\tCreditLimt\r\n\t,ObligLimit\r\n\t,AddDlnBlnc\r\n\t,SalesLimit\r\n\t,DlnLimit\r\n\t,OrderLimit\r\n\t,PickLimit\r\n\t,SlpCommUse\r\n\t,ItmCommUse\r\n\t,CrdCommUse\r\n\t,DfCustTerm\r\n\t,DfVendTerm\r\n\t,DfltCustPM\r\n\t,DfltVendPM\r\n\t,CreditDpst\r\n\t,useDocWrf\r\n\t,EnbApprDI\r\n\t,DspFrznBP\r\n\t,b.DspIBPDoc\r\n\t,MBAOnPer\r\n\t----Tab Budget\r\n\t,DoBudget\r\n\t,BgtBlock\r\n\t,BgtWarning\r\n\t,BdgtPRQDOC\r\n\t,BdgtPORDoc\r\n\t,BdgtPDNDoc\r\n\t,BdgtAcctng\r\n\t,MapService\r\n\t,MaxHistory\r\n\t,ConsumeMtd\r\n\t,DaysBack\r\n\t,DaysFwrd\r\n\t,SimReport\r\n\t,Color\r\n\t,DefLengthU\r\n\t,DefWeightU\r\n\t,TimeFormat\r\n\t,DateFormat\r\n\t,DateSep\r\n\t,SumDec\r\n\t,PriceDec\r\n\t,RateDec\r\n\t,QtyDec\r\n\t,PercentDec\r\n\t,MeasureDec\r\n\t,QueryDec\r\n\t,DecSep\r\n\t,ThousSep\r\n\t,CurOnRight\r\n\t,DirectRate\r\n\t,TPLId\r\n\t,TxtSrch\r\n\t,DspFrznITM\r\n\t,b.DspIITMDoc\r\n\t,OneBOneRec\r\n\t,AutoITW\r\n\t,WTLiable\r\n\t,AutoAddUoM\r\n\t,AutoAddPkg\r\n\t,ConsumeFCT\r\n\t,AutoResWhs\r\n\t,UseMltDims\r\n\t,ApyDRinPEC\r\n\t,ApyPRinPEC\r\n\t,ApyBsActPV\r\n\t,ApyBsActSP\r\n\t,ApyBsActPL\r\n\t,SIPLReport\r\n\t,SIPLDoc\r\n\t,SIPLSeting\r\n\t,ClnZeroPln\r\n\t,IsUpdNstdB\r\n\t,DfltByEml\t\r\n\t,MDStyle\r\n\t,IgrAllCash\r\n\t,CfwAsnMust\r\n\t,ExcelPath\r\n\t,PrjBlock\r\n\t,DftResWhs\r\n\t,StartFrom\r\n\t,Months\r\n\t,Days\r\n\t,XmlPath\r\n\t,SriUniqFld\r\n\t,BtchStatus\r\n\t,DfltWhs\r\n\t,GLMethod\r\n\t,PriceProcM\r\n\t,CfwInDflt\r\n\t,CfwOutDflt\r\n    ----Tab Tax\r\n    ,DfSVatItem\r\n    ,DfSVatServ\r\n    ,DfPVatItem\r\n    ,DfPVatServ\r\n\t FROM OADM a\r\n\tINNER JOIN dbo.ADM1 b ON b.Code = a.Code\r\n\r\n\t---Tab Services\r\n\tSELECT \r\n\tCheckFiles\r\n\t,DsplyRates\r\n\t,RcrFlag\r\n\t,RclFlag\r\n\t,ContactLog\r\n\t,ShowNewMsg\r\n\t,OpenCdt\r\n\t,OpenDps\r\n\t,ShowNewTsk\r\n\t,AlertPolFr\r\n\t,ScreenLock\r\n\t,OpenCredit\r\n\t,AdvImagePr\r\n\t,WllPprDsp\r\n\t,WallPaper\r\n\t FROM  dbo.OUSR\r\n\t WHERE USERID=@USERID\r\n\r\n\tSELECT \r\n\tIMCEEnable\r\n\t,SPNEnabled\r\n\t,MaxChoose\r\n\t,B1SgtEb\r\n\t,AutoCrIns\r\n\t,PrsWkCntEb\r\n\t,SriMngSys\r\n    ,SriCreatIn\r\n\t\r\n\tFROM CINF\r\n\r\n\tSELECT \r\n\tWordPath\r\n\t,ExtPath\r\n\t,BitmapPath\r\n\t,AttachPath\r\n\t \r\n\tFROM dbo.OADP\r\n\r\n    SELECT \r\n    DisableSplitFolder \r\n    FROM dbo.APZ_OADM ";
			SqlParameter sqlParameter = new SqlParameter("@USERID", userId);
			DataSet dataSet = ExecuteData(query, CommandType.Text, sqlParameter);
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

	public virtual DataTable GetDefaultPaymentTermforCustomer()
	{
		try
		{
			string query = "SELECT \"GroupNum\" AS \"Code\",\"PymntGroup\" AS \"Name\" FROM OCTG;";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text);
			if (dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetFormatString FAILED ", ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetDefaultPaymentMethodforCustomer()
	{
		try
		{
			string query = "SELECT \"PayMethCod\" AS \"Code\",\"Descript\" AS \"Name\" FROM OPYM;";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text);
			if (dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetFormatString FAILED ", ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetMapService()
	{
		try
		{
			string query = "SELECT \"AbsEntry\" AS \"Code\",\"Name\" FROM OMPS;";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text);
			if (dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetDfltWhs()
	{
		try
		{
			string query = "SELECT \"WhsCode\" AS \"Code\",\"WhsName\" AS \"Name\" FROM OWHS;";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text);
			if (dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public string SaveData(Tuple<DataSet, int> tuple)
	{
		try
		{
			DataSet item = tuple.Item1;
			int item2 = tuple.Item2;
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				DataTable dt = item.Tables[0];
				DataTable dt2 = item.Tables[1];
				DataTable dt3 = item.Tables[2];
				DataTable dt4 = item.Tables[3];
				DataTable dt5 = item.Tables[4];
				string text = SaveOadm(dt);
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
				text = SaveOusr(dt2, item2);
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
				text = SaveCinf(dt3);
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
				text = SaveOadp(dt4);
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
				text = SaveAPZ_OADM(dt5);
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
				dbTransaction.Commit();
				if (item.Tables[5] != null)
				{
					DataTable dt6 = item.Tables[5];
					text = SaveATC2(dt6);
					if (!string.IsNullOrEmpty(text))
					{
						return text;
					}
				}
				return string.Empty;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Save FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public string SaveOadm(DataTable dt)
	{
		try
		{
			string text = "Update Oadm Set ";
			for (int i = 0; i < dt.Columns.Count; i++)
			{
				string columnName = dt.Columns[i].ColumnName;
				if (!(columnName == "DspIITMDoc") && !(columnName == "DspIBPDoc"))
				{
					text = text + Environment.NewLine + string.Format(" \"{0}\" = @{0},", columnName);
				}
			}
			text = text.Substring(0, text.Length - 1) + ";";
			text = text + Environment.NewLine + " Update ADM1 Set ";
			for (int j = 0; j < dt.Columns.Count; j++)
			{
				string columnName2 = dt.Columns[j].ColumnName;
				if (columnName2 == "DspIITMDoc" || columnName2 == "DspIBPDoc")
				{
					text = text + Environment.NewLine + string.Format("\"{0}\" = @{0},", columnName2);
				}
			}
			text = text.Substring(0, text.Length - 1) + ";";
			List<SqlParameter> list = MappingSqlParameter(dt);
			if (ExecuteNonQuery(text, CommandType.Text, list.ToArray()) <= 0)
			{
				return "No record effected";
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SaveOadm  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public string SaveOusr(DataTable dt, int userId)
	{
		try
		{
			if (!dt.IsNotNull())
			{
				return string.Empty;
			}
			string text = "Update Ousr Set ";
			for (int i = 0; i < dt.Columns.Count; i++)
			{
				string columnName = dt.Columns[i].ColumnName;
				text = text + Environment.NewLine + string.Format(" \"{0}\" = @{0},", columnName);
			}
			text = text.Substring(0, text.Length - 1);
			text = text + Environment.NewLine + " Where USERID=@USERID;";
			List<SqlParameter> list = MappingSqlParameter(dt);
			list.Add(new SqlParameter("@USERID", userId));
			if (ExecuteNonQuery(text, CommandType.Text, list.ToArray()) <= 0)
			{
				return "No record effected";
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get Routing  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public string SaveCinf(DataTable dt)
	{
		try
		{
			string text = "Update CINF Set ";
			for (int i = 0; i < dt.Columns.Count; i++)
			{
				string columnName = dt.Columns[i].ColumnName;
				text = text + Environment.NewLine + string.Format(" \"{0}\" = @{0},", columnName);
			}
			text = text.Substring(0, text.Length - 1) + ";";
			List<SqlParameter> list = MappingSqlParameter(dt);
			if (ExecuteNonQuery(text, CommandType.Text, list.ToArray()) <= 0)
			{
				return "No record effected";
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get Routing  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public string SaveOadp(DataTable dt)
	{
		try
		{
			string text = "Update OADP Set ";
			for (int i = 0; i < dt.Columns.Count; i++)
			{
				string columnName = dt.Columns[i].ColumnName;
				text = text + Environment.NewLine + string.Format(" \"{0}\" = @{0},", columnName);
			}
			text = text.Substring(0, text.Length - 1) + ";";
			List<SqlParameter> list = MappingSqlParameter(dt);
			if (ExecuteNonQuery(text, CommandType.Text, list.ToArray()) <= 0)
			{
				return "No record effected";
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get Routing  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public string SaveAPZ_OADM(DataTable dt)
	{
		try
		{
			string text = "Update APZ_OADM Set ";
			for (int i = 0; i < dt.Columns.Count; i++)
			{
				string columnName = dt.Columns[i].ColumnName;
				text = text + Environment.NewLine + string.Format(" \"{0}\" = @{0},", columnName);
			}
			text = text.Substring(0, text.Length - 1) + ";";
			List<SqlParameter> list = MappingSqlParameter(dt);
			if (ExecuteNonQuery(text, CommandType.Text, list.ToArray()) <= 0)
			{
				return "No record effected";
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get Routing  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public string SaveATC2(DataTable dt)
	{
		try
		{
			dtATC2 = dt;
			return Process(ApzObjectTypes.oTaskManagement, -1, Enums.TransactionType.A, new GeneralSettingImpl()).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get Routing  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public virtual string GetSaveATC2Query(string tempTable)
	{
		try
		{
			return "DELETE FROM APZ_ATC2\r\n                         INSERT INTO APZ_ATC2 SELECT * FROM " + tempTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return string.Empty;
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, GeneralSettingImpl document, ref string objId, ref string objNum)
	{
		objId = "";
		objNum = "";
		string text = "";
		object obj = null;
		try
		{
			if (dtATC2.HasColumn())
			{
				CreateTempTable(dtATC2, "#APZ_ATC2");
				BulkCopy(dtATC2, "#APZ_ATC2");
				text = GetSaveATC2Query("#APZ_ATC2");
				obj = ExecuteScalar(text, CommandType.Text);
				objId = Function.ToString(obj);
				objNum = objId;
				return new HttpResult(Enums.MessageCode.Success);
			}
			return new HttpResult(Enums.MessageCode.Error, "No data to save for Custom Object Path.");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private List<SqlParameter> MappingSqlParameter(DataTable dtTable)
	{
		List<SqlParameter> list = new List<SqlParameter>();
		DataRow dataRow = dtTable.Rows[0];
		for (int i = 0; i < dtTable.Columns.Count; i++)
		{
			string text = Function.ToString(dtTable.Columns[i].ColumnName);
			if (dataRow.IsExistsColumn(text))
			{
				list.Add(new SqlParameter("@" + text, dataRow[text]));
			}
		}
		return list;
	}

	public DataTable GetObjectTypePath()
	{
		try
		{
			string query = "SELECT * FROM APZ_ATC2;";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text);
			if (dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetObjectTypePath FAILED ", ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetAllGeneralSetting()
	{
		return ExecuteDataTable("select * from OADM;", CommandType.Text);
	}

	public string GetMainCurrency()
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("SELECT \"MainCurncy\" FROM OADM;", CommandType.Text);
			if (dataTable.IsNotNull())
			{
				return Function.ToString(dataTable.Rows[0][0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return string.Empty;
	}

	public DataTable GetLengthUnits()
	{
		try
		{
			return ExecuteDataTable("select \"UnitCode\" \"ID\", \"UnitName\" \"Name\" from OLGT order by \"UnitCode\" asc;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetWeightUnits()
	{
		try
		{
			return ExecuteDataTable("select \"UnitCode\" \"ID\", \"UnitName\" \"Name\" from OWGT order by \"UnitCode\" asc;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}
}
