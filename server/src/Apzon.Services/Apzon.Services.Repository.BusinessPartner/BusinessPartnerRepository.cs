using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Services.Interfaces.BusinessPartner;

namespace Apzon.Services.Repository.BusinessPartner;

public class BusinessPartnerRepository : BaseService<BusinessPartnersImpl>, IBusinessPartnerss, IService<BusinessPartnersImpl>
{
	public BusinessPartnerRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable Table()
	{
		new DataTable();
		return ExecuteData("select \"CardCode\",\"CardName\" from OCRD where \"CardType\"='C';", CommandType.Text, (SqlParameter[])null).Tables[0];
	}

	public string Insert(BusinessPartnersImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Update(BusinessPartnersImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Remove(BusinessPartnersImpl entity)
	{
		throw new NotImplementedException();
	}

	public BusinessPartnersImpl GetById(int id)
	{
		throw new NotImplementedException();
	}

	public List<BusinessPartnersImpl> GetCustomer(string username = "")
	{
		return GetBp("C", username);
	}

	public List<BusinessPartnersImpl> GetVendor(string username = "")
	{
		return GetBp("S", username);
	}

	public List<BusinessPartnersImpl> GetLead(string username = "")
	{
		return GetBp("L", username);
	}

	public List<BusinessPartnersImpl> GetBp(string CardType, string username = "")
	{
		string query = "APZ_BusinessPartner_GetBp";
		List<SqlParameter> list = new List<SqlParameter>();
		if (!string.IsNullOrEmpty(CardType))
		{
			list.Add(new SqlParameter("@CardType", CardType));
		}
		else
		{
			list.Add(new SqlParameter("@CardType", string.Empty));
		}
		if (!string.IsNullOrEmpty(username))
		{
			list.Add(new SqlParameter("@UserName", username));
		}
		else
		{
			list.Add(new SqlParameter("@UserName", string.Empty));
		}
		List<BusinessPartnersImpl> list2 = new List<BusinessPartnersImpl>();
		DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		try
		{
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				list2.Add(new BusinessPartnersImpl
				{
					CardCode = Function.ToString(dataTable.Rows[i]["CardCode"]),
					CardName = Function.ToString(dataTable.Rows[i]["CardName"]),
					CurrentAccountBalance = Function.ParseDouble(dataTable.Rows[i]["Balance"]),
					ContactPerson = Function.ToString(dataTable.Rows[i]["Name"]),
					ContactPersonCode = Function.ToString(dataTable.Rows[i]["CntctPrsn"]),
					MailAddress = Function.ToString(dataTable.Rows[i]["MailAddress"]),
					Address = Function.ToString(dataTable.Rows[i]["Address"]),
					ShippingType = Function.ParseInt(dataTable.Rows[i]["ShipType"]),
					ShipToDefault = Function.ToString(dataTable.Rows[i]["ShipToDef"]),
					BilltoDefault = Function.ToString(dataTable.Rows[i]["BillToDef"]),
					Currency = Function.ToString(dataTable.Rows[i]["Currency"]),
					CardType = Function.ToString(dataTable.Rows[i]["CardType"]),
					Tel1 = Function.ToString(dataTable.Rows[i]["Phone1"]),
					SalesPersonCode = Function.ParseInt(dataTable.Rows[i]["SlpCode"])
				});
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetBp  FAILED ", ex.Message);
		}
		return list2;
	}

	public List<BusinessPartnersImpl> GetListContactPerson(string cardCode)
	{
		List<BusinessPartnersImpl> list = new List<BusinessPartnersImpl>();
		try
		{
			string query = " select  \"Name\", \"Position\", \"Tel1\", \"CntctCode\" from  OCPR\r\n                                   where \"CardCode\" = '" + cardCode + "';";
			foreach (DataRow row in ExecuteDataTable(query, CommandType.Text).Rows)
			{
				list.Add(new BusinessPartnersImpl
				{
					Name = Function.ToString(row["Name"]),
					Position = Function.ToString(row["Position"]),
					Tel1 = Function.ToString(row["Tel1"]),
					CntctCode = Function.ParseInt(row["CntctCode"])
				});
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetListContactPerson  FAILED ", ex.Message);
		}
		return list;
	}

	public List<BusinessPartnersImpl> GetContactPerson_byCntctCode(string cardCode, string cntctCode)
	{
		List<BusinessPartnersImpl> list = new List<BusinessPartnersImpl>();
		try
		{
			string query = " select \"CntctCode\", \"Name\" from  OCPR\r\n                                   where \"CardCode\" = '" + cardCode + "' and \"CntctCode\" ='" + cntctCode + "';";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text);
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				list.Add(new BusinessPartnersImpl
				{
					CntctCode = Function.ParseInt(dataTable.Rows[i]["CntctCode"]),
					Name = Function.ToString(dataTable.Rows[i]["Name"])
				});
			}
			return list;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetContactPerson  FAILED ", ex.Message);
			return new List<BusinessPartnersImpl>();
		}
	}

	public List<BusinessPartnersImpl> GetTelePh_ByCotacPerson(string cardCode, string cntctCode)
	{
		List<BusinessPartnersImpl> list = new List<BusinessPartnersImpl>();
		try
		{
			string text = " select  \"Name\", \"Position\", \"Tel1\", \"CntctCode\" from  OCPR\r\n                                   where 1=1";
			if (!string.IsNullOrEmpty(cardCode))
			{
				text = text + " and \"CardCode\"='" + cardCode + "'";
			}
			if (!string.IsNullOrWhiteSpace(cntctCode))
			{
				text = text + " and \"CntctCode\"='" + cntctCode + "'";
			}
			text += ";";
			foreach (DataRow row in ExecuteDataTable(text, CommandType.Text).Rows)
			{
				list.Add(new BusinessPartnersImpl
				{
					Name = Function.ToString(row["Name"]),
					Position = Function.ToString(row["Position"]),
					Tel1 = Function.ToString(row["Tel1"]),
					CntctCode = Function.ParseInt(row["CntctCode"])
				});
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetListContactPerson  FAILED ", ex.Message);
		}
		return list;
	}

	public DataTable GetTel_ByContactPerson(string cardCode, string cntctCode)
	{
		try
		{
			string text = " select  \"Name\", \"Position\", \"Tel1\", \"CntctCode\" from  OCPR\r\n                                   where 1=1";
			if (!string.IsNullOrEmpty(cardCode))
			{
				text = text + " and \"CardCode\"='" + cardCode + "'";
			}
			if (!string.IsNullOrWhiteSpace(cntctCode))
			{
				text = text + " and \"CntctCode\"='" + cntctCode + "'";
			}
			text += ";";
			return ExecuteDataTable(text, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetListContactPerson  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public List<BusinessPartnersImpl> GetCardType_ByCardCode(string cardCode, string clgCode)
	{
		List<BusinessPartnersImpl> list = new List<BusinessPartnersImpl>();
		try
		{
			string query = " select CASE WHEN  B.\"CardType\" = 'C' then 'Customer'\r\n\t\t\t                               WHEN  B.\"CardType\" = 'L' then ' Lead '\r\n\t\t\t                               WHEN  B.\"CardType\" = 'S' then 'Vendor'\t\r\n                                      END AS \"CarType\"\r\n                               from OCLG A left join OCRD B on A.\"CardCode\" = B.\"CardCode\"\r\n                               where A.\"CardCode\" = '" + cardCode + "' and A.\"ClgCode\" = '" + clgCode + "';";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text);
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				list.Add(new BusinessPartnersImpl
				{
					CardType = Function.ToString(dataTable.Rows[i]["CardType"])
				});
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetListContactPerson  FAILED ", ex.Message);
		}
		return list;
	}

	public DataTable ListBPByCardType(string CardType, string userName, int BPLId)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "APZ_BusinessMaster_ListBPByCardType";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@CardType", CardType));
			list.Add(new SqlParameter("@UserName", userName));
			list.Add(new SqlParameter("@BPLId", BPLId));
			result = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}
}
