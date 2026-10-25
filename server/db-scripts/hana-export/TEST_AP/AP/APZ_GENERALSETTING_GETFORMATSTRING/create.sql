
CREATE PROCEDURE APZ_GeneralSetting_GetFormatString
as
BEGIN

	SELECT a."Code",a."CompnyName",a."CompnyAddr",a."PrintHeadr",a."Manager",a."AliasName",
                                   a."Phone1",a."Phone2",a."Fax", IFNULL(a."E_Mail",'') as "E_Mail", a."TaxIdNum",
	                               a."HldCode",a."MainCurncy",a."SysCurrncy", 
	                               case when a."DfActCurr" ='Y' then 'All Currencies'
			                            when a."DfActCurr"='N' then 'Local Currency'
	                               end "DfActCurr",
                                   case when a."InvntSystm"='F' then 'FIFO'
		                               when a."InvntSystm"='S' then 'Standard'
		                               when a."InvntSystm"='A' then 'Moving Averager'
	                               end "InvntSystm",
		                            case when a."TimeFormat" ='0' then '24H'
			                            when a."TimeFormat" ='1' then '12H'
		                            end "TimeFormat",
                                   case when a."DateFormat" = '0' then N'DD/MM/YY'
			                           when a."DateFormat" ='1' then N'DD/MM/CCYY'
			                           when a."DateFormat" =  '2' then  N'MM/DD/YY' 
			                           when a."DateFormat" ='3' then  N'MM/DD/CCYY'
			                            when a."DateFormat" =  '4' then  N'CCYY/MM/DD'
			                            when a."DateFormat" = '5' then  N'DD/Month/YYYY'
                                      when a."DateFormat" = '6' then  N'YY/MM/DD'
		                           end "DateFormat",
		                            a."DateSep", a."SumDec",a."QtyDec",a."PriceDec",a."RateDec",a."PercentDec",a."DecSep",a."ThousSep"
									,IFNULL(a."ClosedQuot",'N') as "ClosedQuot"
                            from OADM a;
 END;




