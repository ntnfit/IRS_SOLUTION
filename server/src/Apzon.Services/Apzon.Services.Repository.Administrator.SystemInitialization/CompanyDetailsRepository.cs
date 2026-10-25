using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Services.Interfaces.Administrator.SystemInitialization;

namespace Apzon.Services.Repository.Administrator.SystemInitialization;

public class CompanyDetailsRepository : BaseService<DataTable>, ICompanyDetails
{
	public CompanyDetailsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public string Save(DataSet ds)
	{
		string text = string.Empty;
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				text = SaveDoc(ds.Tables[0].Rows[0]);
				if (string.IsNullOrWhiteSpace(text))
				{
					text = SaveLine(ds.Tables[1].Rows[0]);
				}
				if (string.IsNullOrWhiteSpace(text))
				{
					dbTransaction.Commit();
				}
				else
				{
					dbTransaction.Rollback();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE ", ex.Message);
			text = ex.Message;
		}
		return text;
	}

	private string SaveDoc(DataRow r)
	{
		string result = "Save Error";
		try
		{
			string query = "UPDATE OADM\r\n                               SET \"CompnyName\" = @CompnyName\r\n                                  --,CompnyAddr = @CompnyAddr\r\n                                  ,\"Country\" = @Country\r\n                                  ,\"PrintHeadr\" = @PrintHeadr\r\n                                  ,\"Phone1\" = @Phone1\r\n                                  ,\"Phone2\" = @Phone2\r\n                                  ,\"Fax\" = @Fax\r\n                                  ,\"E_Mail\" = @E_Mail\r\n                                  ,\"Manager\" = @Manager\r\n                                  --,CompType = @CompType\r\n                                  --,MainCurncy = @MainCurncy\r\n                                  --,SysCurrncy = @SysCurrncy\r\n                                  --,DispPosDeb = @DispPosDeb\r\n                                  ,\"BankCountr\" = @BankCountr\r\n                                  ,\"TaxIdNum\" = @TaxIdNum\r\n                                  ,\"RevOffice\" = @RevOffice\r\n                                  ,\"FreeZoneNo\" = @FreeZoneNo\r\n                                  ,\"DdctFileNo\" = @DdctFileNo\r\n                                  ,\"IncomeTax\" = @IncomeTax\r\n                                  ,\"DdctOffice\" = @DdctOffice\r\n                                  ,\"UseExtRpt\" = @UseExtRpt\r\n                                  --,CCMask = @CCMask\r\n                                  ,\"MultiLang\" = @MultiLang\r\n                                  --,CmpnyAddrF = @CmpnyAddrF\r\n                                  ,\"PrintHdrF\" = @PrintHdrF\r\n                                  ,\"Phone1F\" = @Phone1F\r\n                                  ,\"Phone2F\" = @Phone2F\r\n                                  ,\"FaxF\" = @FaxF\r\n                                  ,\"ManagerF\" = @ManagerF\r\n                                  --,ContInvnt = @ContInvnt\r\n                                  ,\"InvntSystm\" = @InvntSystm\r\n                                  ,\"MltpBrnchs\" = @MltpBrnchs\r\n                                  ,\"IRSFileNo\" = @IRSFileNo\r\n                                  ,\"DeferrTax\" = @DeferrTax\r\n                                  ,\"DflBnkCode\" = @DflBnkCode\r\n                                  ,\"DflBnkAcct\" = @DflBnkAcct\r\n                                  ,\"DflBranch\" = @DflBranch\r\n                                  --,UsePaSys = @UsePaSys\r\n                                  ,\"TaxIdNum2\" = @TaxIdNum2\r\n                                  ,\"TaxIdNum3\" = @TaxIdNum3\r\n                                  --,PriceSys = @PriceSys\r\n                                  ,\"TaxRateDet\" = @TaxRateDet\r\n                                  ,\"NegAmount\" = @NegAmount\r\n                                  ,\"HldCode\" = @HldCode\r\n                                  ,\"OrderParty\" = @OrderParty\r\n                                  ,\"AliasName\" = @AliasName\r\n                                  ,\"DfActCurr\" = @DfActCurr\r\n                                  ,\"State\" = @State\r\n                                  ,\"ExRtDefTax\" = @ExRtDefTax\r\n                                  ,\"InstFixAst\" = @InstFixAst\r\n                                  ,\"DeprecCalc\" = @DeprecCalc\r\n                                  ,\"SnBDfltSB\" = @SnBDfltSB;\r\n                            UPDATE CINF SET \r\n                                  --EnbSgmnAct=@EnbSgmnAct,\r\n                                  \"RelStkNoPr\"=@RelStkNoPr;\r\n                                  --,DocNmMtd=@DocNmMtd";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@CompnyName", Function.ToString(r["CompnyName"])));
			list.Add(new SqlParameter("@Country", Function.ToString(r["Country"])));
			list.Add(new SqlParameter("@PrintHeadr", Function.ToString(r["PrintHeadr"])));
			list.Add(new SqlParameter("@Phone1", Function.ToString(r["Phone1"])));
			list.Add(new SqlParameter("@Phone2", Function.ToString(r["Phone2"])));
			list.Add(new SqlParameter("@Fax", Function.ToString(r["Fax"])));
			list.Add(new SqlParameter("@E_Mail", Function.ToString(r["E_Mail"])));
			list.Add(new SqlParameter("@Manager", Function.ToString(r["Manager"])));
			list.Add(new SqlParameter("@BankCountr", Function.ToString(r["BankCountr"])));
			list.Add(new SqlParameter("@TaxIdNum", Function.ToString(r["TaxIdNum"])));
			list.Add(new SqlParameter("@RevOffice", Function.ToString(r["RevOffice"])));
			list.Add(new SqlParameter("@FreeZoneNo", Function.ToString(r["FreeZoneNo"])));
			list.Add(new SqlParameter("@DdctFileNo", Function.ToString(r["DdctFileNo"])));
			list.Add(new SqlParameter("@IncomeTax", Function.ParseDecimal(r["IncomeTax"])));
			list.Add(new SqlParameter("@DdctOffice", Function.ToString(r["DdctOffice"])));
			list.Add(new SqlParameter("@UseExtRpt", Function.ToString(r["UseExtRpt"])));
			list.Add(new SqlParameter("@MultiLang", Function.ToString(r["MultiLang"])));
			list.Add(new SqlParameter("@PrintHdrF", Function.ToString(r["PrintHdrF"])));
			list.Add(new SqlParameter("@Phone1F", Function.ToString(r["Phone1F"])));
			list.Add(new SqlParameter("@Phone2F", Function.ToString(r["Phone2F"])));
			list.Add(new SqlParameter("@FaxF", Function.ToString(r["FaxF"])));
			list.Add(new SqlParameter("@ManagerF", Function.ToString(r["ManagerF"])));
			list.Add(new SqlParameter("@InvntSystm", Function.ToString(r["InvntSystm"])));
			list.Add(new SqlParameter("@MltpBrnchs", Function.ToString(r["MltpBrnchs"])));
			list.Add(new SqlParameter("@IRSFileNo", Function.ToString(r["IRSFileNo"])));
			list.Add(new SqlParameter("@DeferrTax", Function.ToString(r["DeferrTax"])));
			list.Add(new SqlParameter("@DflBnkCode", Function.ToString(r["DflBnkCode"])));
			list.Add(new SqlParameter("@DflBnkAcct", Function.ToString(r["DflBnkAcct"])));
			list.Add(new SqlParameter("@DflBranch", Function.ToString(r["DflBranch"])));
			list.Add(new SqlParameter("@TaxIdNum2", Function.ToString(r["TaxIdNum2"])));
			list.Add(new SqlParameter("@TaxIdNum3", Function.ToString(r["TaxIdNum3"])));
			list.Add(new SqlParameter("@TaxRateDet", Function.ToString(r["TaxRateDet"])));
			list.Add(new SqlParameter("@NegAmount", Function.ToString(r["NegAmount"])));
			list.Add(new SqlParameter("@HldCode", Function.ToString(r["HldCode"])));
			list.Add(new SqlParameter("@OrderParty", Function.ToString(r["OrderParty"])));
			list.Add(new SqlParameter("@AliasName", Function.ToString(r["AliasName"])));
			list.Add(new SqlParameter("@DfActCurr", Function.ToString(r["DfActCurr"])));
			list.Add(new SqlParameter("@State", Function.ToString(r["State"])));
			list.Add(new SqlParameter("@ExRtDefTax", Function.ToString(r["ExRtDefTax"])));
			list.Add(new SqlParameter("@InstFixAst", Function.ToString(r["InstFixAst"])));
			list.Add(new SqlParameter("@DeprecCalc", Function.ToString(r["DeprecCalc"])));
			list.Add(new SqlParameter("@SnBDfltSB", Function.ToString(r["SnBDfltSB"])));
			list.Add(new SqlParameter("@RelStkNoPr", Function.ToString(r["RelStkNoPr"])));
			if (ExecuteNonQuery(query, CommandType.Text, list.ToArray()) > 0)
			{
				result = "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			result = ex.Message;
		}
		return result;
	}

	private string SaveLine(DataRow r)
	{
		string result = "Save Error";
		try
		{
			string query = "UPDATE ADM1\r\n                             SET \"Street\" = @Street\r\n                              ,\"StreetF\" = @StreetF\r\n                              ,\"StreetNo\" = @StreetNo\r\n                              ,\"StreetNoF\" = @StreetNoF\r\n                              ,\"Building\" = @Building\r\n                              ,\"BuildingF\" = @BuildingF\r\n                              ,\"Block\" = @Block\r\n                              ,\"BlockF\" = @BlockF\r\n                              ,\"City\" = @City\r\n                              ,\"CityF\" = @CityF\r\n                              ,\"ZipCode\" = @ZipCode\r\n                              ,\"ZipCodeF\" = @ZipCodeF\r\n                              ,\"County\" = @County\r\n                              ,\"CountyF\" = @CountyF                          \r\n                              ,\"IntrntAdrs\" = @IntrntAdrs\r\n                              ,\"GlblLocNum\" = @GlblLocNum;";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Street", Function.ToString(r["Street"])));
			list.Add(new SqlParameter("@StreetF", Function.ToString(r["StreetF"])));
			list.Add(new SqlParameter("@StreetNo", Function.ToString(r["StreetNo"])));
			list.Add(new SqlParameter("@StreetNoF", Function.ToString(r["StreetNoF"])));
			list.Add(new SqlParameter("@Building", Function.ToString(r["Building"])));
			list.Add(new SqlParameter("@BuildingF", Function.ToString(r["BuildingF"])));
			list.Add(new SqlParameter("@Block", Function.ToString(r["Block"])));
			list.Add(new SqlParameter("@BlockF", Function.ToString(r["BlockF"])));
			list.Add(new SqlParameter("@City", Function.ToString(r["City"])));
			list.Add(new SqlParameter("@CityF", Function.ToString(r["CityF"])));
			list.Add(new SqlParameter("@ZipCode", Function.ToString(r["ZipCode"])));
			list.Add(new SqlParameter("@ZipCodeF", Function.ToString(r["ZipCodeF"])));
			list.Add(new SqlParameter("@County", Function.ToString(r["County"])));
			list.Add(new SqlParameter("@CountyF", Function.ToString(r["CountyF"])));
			list.Add(new SqlParameter("@IntrntAdrs", Function.ToString(r["IntrntAdrs"])));
			list.Add(new SqlParameter("@GlblLocNum", Function.ToString(r["GlblLocNum"])));
			if (ExecuteNonQuery(query, CommandType.Text, list.ToArray()) > 0)
			{
				result = "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			result = ex.Message;
		}
		return result;
	}

	public DataTable GetOADM()
	{
		DataTable dataTable = new DataTable();
		try
		{
			string oADMQuery = GetOADMQuery();
			dataTable = ExecuteDataTable(oADMQuery, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetOADM ", ex.Message);
			return dataTable;
		}
	}

	public DataTable GetADM1()
	{
		DataTable dataTable = new DataTable();
		try
		{
			string query = "select * from ADM1;";
			dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetADM1 ", ex.Message);
			return dataTable;
		}
	}

	public DataTable GetStructureTable(string tableName)
	{
		try
		{
			if (!tableName.Contains(" "))
			{
				string query = "SELECT COLUMN_NAME,DATA_TYPE\r\n                                    FROM INFORMATION_SCHEMA.COLUMNS\r\n                                    WHERE TABLE_NAME = '" + tableName + "'";
				DataTable dataTable = ExecuteDataTable(query, CommandType.Text);
				if (dataTable.IsNotNull())
				{
					return dataTable;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable Search(DataTable dtFind)
	{
		DataTable dataTable = new DataTable();
		try
		{
			string value = Function.ToString(dtFind.Rows[0]["Code"]);
			string value2 = Function.ToString(dtFind.Rows[0]["Name"]);
			string query = "APZ_SearchOADM";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Code", value));
			list.Add(new SqlParameter("@Name", value2));
			dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Search ", ex.Message);
			return dataTable;
		}
	}

	public DataTable GetTableColumns(string tableName)
	{
		DataTable dataTable = new DataTable();
		try
		{
			if (ExistsTable(tableName))
			{
				string query = "select top 0 * From " + tableName + ";";
				if (tableName.ToUpper() == "OADM")
				{
					query = "select top 0 * ,(select top 0 \"EnbSgmnAct\" from CINF) EnbSgmnAct\r\n                                        ,(select top 0 \"RelStkNoPr\" from CINF) RelStkNoPr\r\n                                        ,(select top 0 \"DocNmMtd\" from CINF) DocNmMtd\r\n                        FROM " + tableName + ";";
				}
				dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
				DataRow row = dataTable.NewRow();
				dataTable.Rows.Add(row);
				dataTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetColumTable ", ex.Message);
			return new DataTable();
		}
		return dataTable;
	}

	private bool ExistsTable(string tableName)
	{
		bool result = false;
		try
		{
			string query = ExistsTableQuery();
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Table", tableName));
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				result = Function.ToString(dataTable.Rows[0][0]).Trim().ToUpper() == "TRUE";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable GetHoliday()
	{
		DataTable dataTable = new DataTable();
		try
		{
			string query = "select \"HldCode\" from OHLD;";
			dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetHoliday ", ex.Message);
			return dataTable;
		}
	}

	public DataTable GetCountry()
	{
		DataTable dataTable = new DataTable();
		try
		{
			string query = "select \"Code\",\"Name\" from OCRY;";
			dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetCountry ", ex.Message);
			return dataTable;
		}
	}

	public DataTable GetState(string codeCountry)
	{
		DataTable dataTable = new DataTable();
		List<SqlParameter> list = new List<SqlParameter>();
		try
		{
			string text = "select A.\"Code\",A.\"Name\" from OCST A LEFT JOIN OCRY B ON A.\"Country\"= B.\"Code\" WHERE 1 = 1";
			if (!string.IsNullOrEmpty(codeCountry))
			{
				list.Add(new SqlParameter("@Code", codeCountry));
			}
			else
			{
				list.Add(new SqlParameter("@Code", string.Empty));
			}
			text += $" AND  A.\"Country\" = @Code;";
			dataTable = ExecuteDataTable(text, CommandType.Text, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetState ", ex.Message);
			return dataTable;
		}
	}

	protected virtual string ExistsTableQuery()
	{
		return "DECLARE @TableName VARCHAR(100) = @Table;\r\n                            DECLARE @Exists BIT = ( SELECT CAST( COUNT(1) AS BIT ) FROM sys.tables WHERE name = @TableName AND type = 'U' );\r\n                            select @Exists";
	}

	protected virtual string GetOADMQuery()
	{
		return "select top 1 a.*,b.EnbSgmnAct,b.RelStkNoPr,b.DocNmMtd from OADM a,CINF b";
	}
}
