using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Entities.Models.MasterData;
using Apzon.Services.Interfaces.BusinessPartner;

namespace Apzon.Services.Repository.BusinessPartner;

public class BpAddressRepository : BaseService<AddressesImpl>, IAddresses, IService<AddressesImpl>
{
	public BpAddressRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public string Insert(AddressesImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Update(AddressesImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Remove(AddressesImpl entity)
	{
		throw new NotImplementedException();
	}

	public AddressesImpl GetById(int id)
	{
		throw new NotImplementedException();
	}

	public DataTable ShowAddress(string code, string addrType)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "SELECT *\r\n                              FROM [dbo].[CRD1] where CardCode='" + code + "' AND AdresType = '" + addrType + "'";
			return result = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "ShowAddress  FAILED ", ex.Message);
			return result;
		}
	}

	public int GetMaxCode()
	{
		try
		{
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return 0;
		}
	}

	public string SaveDoc(AddressesImpl doc)
	{
		string empty = string.Empty;
		try
		{
			bool flag = Check(doc.Address, doc.CardCode, doc.AdresType);
			string empty2 = string.Empty;
			empty2 = (flag ? $"UPDATE CRD1  SET \r\n                                                      Street=N'{doc.Street}',Block=N'{doc.Block}'\r\n                                                     ,ZipCode=N'{doc.ZipCode}',City=N'{doc.City}',County=N'{doc.County}',Country=N'{doc.Country}'\r\n                                                        ,State=N'{doc.State}',UserSign=N'{doc.UserSign}'\r\n                                                       ,LogInstanc=N'{doc.LogInstanc}',ObjType=N'{doc.ObjType}',LicTradNum=N'{doc.LicTradNum}',LineNum=N'{doc.LineNum}'\r\n                                                        ,TaxCode=N'{doc.TaxCode}',Building=N'{doc.Building}'\r\n                                                        ,Address2=N'{doc.Address2}',Address3=N'{doc.Address3}',AddrType=N'{doc.AddrType}',StreetNo=N'{doc.StreetNo}'\r\n                                                        ,AltCrdName=N'{doc.AltCrdName}',AltTaxId=N'{doc.AltTaxId}',TaxOffice=N'{doc.TaxOffice}',GlblLocNum=N'{doc.GlblLocNum}',Ntnlty=N'{doc.Ntnlty}'\r\n                                                       WHERE Address=N'{doc.Address}' AND CardCode =N'{doc.CardCode}' AND AdresType=N'{doc.AdresType}'" : $"INSERT INTO [dbo].[CRD1]\r\n           ([Address],[CardCode],[Street],[Block],[ZipCode],[City],[County],[Country],[State],[UserSign],[LogInstanc]\r\n           ,[ObjType],[LicTradNum],[LineNum],[TaxCode],[Building],[AdresType],[Address2],[Address3]\r\n           ,[AddrType],[StreetNo],[AltCrdName],[AltTaxId],[TaxOffice],[GlblLocNum],[Ntnlty])\r\n                                                     VALUES (N'{doc.Address}', N'{doc.CardCode}', N'{doc.Street}', N'{doc.Block}', N'{doc.ZipCode}', N'{doc.City}',\r\n                                                        N'{doc.County}', N'{doc.Country}', N'{doc.State}', N'{doc.UserSign}', N'{doc.LogInstanc}', N'{doc.ObjType}',\r\n                                                        N'{doc.LicTradNum}', N'{doc.LineNum}', N'{doc.TaxCode}', N'{doc.Building}', N'{doc.AdresType}', N'{doc.Address2}',\r\n                                                        N'{doc.Address3}', N'{doc.AddrType}', N'{doc.StreetNo}', N'{doc.AltCrdName}', N'{doc.AltTaxId}', N'{doc.TaxOffice}', N'{doc.GlblLocNum}', N'{doc.Ntnlty}')");
			if (ExecuteNonQueryNoParam(empty2) > 0)
			{
				return null;
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public string DeleteDoc(string addr, string code, string addrType)
	{
		try
		{
			string query = " delete CRD1 where Address = '" + addr + "' AND CardCode ='" + code + "' AND AdresType ='" + addrType + "'";
			if (ExecuteNonQuery(query, CommandType.Text, (SqlParameter[])null) > 0)
			{
				return null;
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	public bool Check(string addr, string code, string addrType)
	{
		try
		{
			string query = "SELECT * \r\n\t\t\t\t\t\t\t        FROM  CRD1\r\n                                    WHERE Address = '" + addr + "' AND CardCode ='" + code + "' AND AdresType ='" + addrType + "'";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return false;
		}
	}
}
