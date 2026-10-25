
CREATE  PROCEDURE  APZ_ActivitiesForOpportunity_SearchDoc(
	in ClgCode int default 0,
	in DocType int default 0,
	in AttendUser int default 0,
	in AttendEmpl int default 0,
	in CardCode nvarchar(50) default Null ,
	in StartTime datetime default NULL,
	in EndTime datetime default NULL)
	
AS
BEGIN
SELECT  
	IFNULL( A."ClgCode",0) AS "ClgCode",
    IFNULL( A."CardCode",'') as "CardCode",
	CASE WHEN  B."CardType" = 'C' then 'Customer'
			                WHEN  B."CardType" = 'L' then ' Lead '
			                WHEN  B."CardType" = 'S' then 'Vendor'	
                        END AS "CardType" ,
	IFNULL (B."CardName",'') as "CardName",
	IFNULL (A."Notes" , '') as "Notes",
	IFNULL (A."CntctDate", '') as "CntctDate" ,                                                         
	IFNULL( A."CntctTime",0) as  "CntctTime", 
	IFNULL (A."Recontact", '') as "Recontact" , 
    IFNULL (A."Closed", '') as "Closed" , 
	IFNULL (A."CloseDate", '') as "CloseDate" , 
	IFNULL ( A."ContactPer",'') as "ContactPer",
	IFNULL ( A."Tel",'') as "Tel",
	IFNULL ( A."Fax",'') as "Fax",
	IFNULL  ( A."CntctSbjct",-1) as "CntctSbjct",
	IFNULL (A."Transfered" ,'N') as "Transfered",
	CASE WHEN A."DocType" = -1 then ''
		WHEN A."DocType" = 23 then 'Sales Quotations'
		WHEN A."DocType" = 17 then 'Sales Orders'
		WHEN A."DocType" = 15 then 'Deliveries'
		WHEN A."DocType" = 16 then 'Returns'
		WHEN A."DocType" = 13 then 'A/R Invoices'
		WHEN A."DocType" = 14 then 'A/R Credit Memos'
		WHEN A."DocType" = 203 then 'A/R DownPayment'
		WHEN A."DocType" = 1470000113 then 'Purchase Request'
		WHEN A."DocType" = 540000006 then 'Purchase Quotation'
		WHEN A."DocType" = 22 then 'Purchase Orders'
		WHEN A."DocType" = 20 then 'Goods Receipt PO'
		WHEN A."DocType" = 21 then 'Goods Return'
		WHEN A."DocType" = 18 then 'A/P Invoices'
		WHEN A."DocType" = 19 then 'A/P Credit Memos'
		WHEN A."DocType" = 48 then 'Landed Costs'
		WHEN A."DocType" = 24 then 'Incoming Payments'
		WHEN A."DocType" = 46 then 'Outgoing Payments'
		WHEN A."DocType" = 1250000001 then 'Inventory Transfer Request'
		WHEN A."DocType" = 67 then 'Inventory Transfer'
		WHEN A."DocType" = 60 then 'Goods Issue'
		WHEN A."DocType" = 59 then 'Goods Receipt'
		WHEN A."DocType" = 162 then 'Inventory Revaluation'
		WHEN A."DocType" = 202 then 'Production Orders'
		WHEN A."DocType" = 4 then 'Items'
		WHEN A."DocType" = 1250000025 then 'Blanket Agreements'
	END AS "DocType",					 			
	A."DocNum",
	A."DocEntry",
	A."Attachment",
	IFNULL (A."DataSource",'I') as "DataSource",
	IFNULL (A."AttendUser", -1) as "AttendUser",
	IFNULL ( A."CntctCode", -1) as "CntctCode", 
	IFNULL ( A."UserSign", -1) as "UserSign", 
	IFNULL ( A."SlpCode", -1) as "SlpCode", 
	A."Action",
	IFNULL ( A."Details", '') as "Details", 
	IFNULL ( A."CntctType", -1) as "CntctType", 
	IFNULL ( A."Location", -1) as "Location", 	
	IFNULL ( A."BeginTime", 0) as "BeginTime", 	
	IFNULL ( A."Duration", 0) as "Duration",	
CASE WHEN A."DurType" = 'M' then 'Minutes'	
	WHEN A."DurType" = 'H' then 'Hours'		
	WHEN A."DurType" = 'D' then 'Days'	
	WHEN IFNULL( A."DurType",'') = '' then 'Minutes'		  	
			
END AS "DurType",				  		 		                         
	IFNULL ( A."ENDTime", 0) as "ENDTime", 
CASE WHEN A."Priority" = '0' then 'Low'	
	WHEN A."Priority" = '1' then 'Normal'		
	WHEN A."Priority" = '2' then 'High'	
	WHEN IFNULL( A."Priority",'') = '' then 'Low'		  	
			
END AS "Priority",				  		 						   
	IFNULL ( A."Reminder", 'N') as "Reminder",   
	IFNULL ( A."RemQty", '0') as "RemQty", 					
CASE WHEN A."RemType" = 'M' then 'Minutes'	
	WHEN A."RemType" = 'H' then 'Hours'		
	WHEN A."RemType" = 'D' then 'Days'	
	WHEN IFNULL( A."RemType",'') = '' then 'Minutes'		  	
			
END AS "RemType",							 					    					 
	IFNULL ( A."OprId", -1) as "OprId",     
	IFNULL ( A."OprLine", -1) as "OprLine",     	 
	IFNULL ( A."RemDate", '') as "RemDate",  
	IFNULL ( A."RemTime", 0) as "RemTime",    
	IFNULL ( A."RemSented", 'N') as "RemSented",    
		IFNULL ( A."Instance", -1) as "Instance",    
		IFNULL ( A."endDate", '') as "endDate",    
		IFNULL ( A."status", -1) as "status",    
		IFNULL ( A."personal", 'N') as "personal",    
	IFNULL ( A."inactive", 'N') as "inactive",    		
	IFNULL ( A."tentative", 'N') as "tentative",  
	IFNULL ( A."street", '') as "street", 
	IFNULL ( A."city", '') as "city", 
	IFNULL ( A."country", '') as "country", 
		IFNULL ( A."state", '') as "state", 
	IFNULL ( A."room", '') as "room", 
		IFNULL ( A."parentType", '') as "parentType",
	IFNULL ( A."parentId", 0) as "parentId",	
	IFNULL ( A."prevActvty", 0) as "prevActvty",		
		IFNULL ( A."AtcEntry", -1) as "AtcEntry",
    A."RecurPat",
	IFNULL (A."EndType",'') as "EndType",
	IFNULL (A."SeStartDat",'') as "SeStartDat",
	IFNULL (A."SeEndDat",'') as "SeEndDat",
	IFNULL (A."MaxOccur",-1) as "MaxOccur",
	IFNULL (A."Interval",-1) as "Interval",
	IFNULL (A."Sunday",'N') as "Sunday"	,
	IFNULL (A."Monday",'N') as "Monday"	,
	IFNULL (A."Tuesday",'N') as "Tuesday"	,
	IFNULL (A."Wednesday",'N') as "Wednesday"	,
	IFNULL (A."Thursday",'N') as "Thursday"	,
	IFNULL (A."Friday",'N') as "Friday"	,
	IFNULL (A."Saturday",'N') as "Saturday"	,
	IFNULL (A."SubOption",'N') as "SubOption",                  
	IFNULL (A."DayInMonth",0) as "DayInMonth",
    Case when A."Month" = 1 then 'January' 
       	when A."Month" = 2 then 'February' 
		when A."Month" = 3 then 'March' 
        when A."Month" = 4 then 'April' 
		when A."Month" =  5 then 'May' 
		when A."Month" =  6 then 'June' 
		when A."Month" =  7 then 'July' 
		when A."Month" =  8 then 'August' 
		when A."Month" =  9 then 'September' 
		when A."Month" =  10 then 'October' 
		when A."Month" =  11 then 'November' 
		when A."Month" =  12 then 'December' 
	END as "Month",						
    Case When A."DayOfWeek" = 1 then 'Sunday'
			When A."DayOfWeek" = 2 then 'Monday'
			When A."DayOfWeek" = 3 then 'Tuesday'
			When A."DayOfWeek" = 4 then 'Wednesday'
			When A."DayOfWeek" = 5 then 'Thursday'
			When A."DayOfWeek" = 6 then 'Friday'
			When A."DayOfWeek" = 7 then 'Saturday'	
			When A."DayOfWeek" = 0 then 'weekend day'	
			When A."DayOfWeek" = 9 then 'weekday'	
			When A."DayOfWeek" = 8 then 'day'						 
	end AS "DayOfWeek",
    CASE WHEN A."Week" = 1 then 'first'
			WHEN A."Week" = 2 then 'second'
			WHEN A."Week" = 3 then 'third'
			WHEN A."Week" = 4 then 'fourth'
			WHEN A."Week" = 5 then 'last'
	END AS "Week",
	IFNULL (A."SeriesNum",-1) as "SeriesNum",
		IFNULL (A."OrigDate",'') as "OrigDate",
			IFNULL (A."IsRemoved",'N') as "IsRemoved",
		IFNULL (A."LastRemind",'') as "LastRemind",	
	IFNULL (A."AssignedBy",-1) as "AssignedBy",		
	IFNULL (A."AddrName",'') as "AddrName",	
	IFNULL (A."AddrType",'S') as "AddrType",	
	IFNULL (A."AttendEmpl",-1) as "AttendEmpl",
	IFNULL (A."NextDate",'') as "NextDate",
	IFNULL (A."NextTime",0) as "NextTime" ,
    IFNULL (D."Name",'') as "Name"
FROM OCLG A  left join OCRD B on A."CardCode" = B."CardCode"   
			left join OCPR D ON A."CntctCode" = D."CntctCode" 
WHERE 
	(:ClgCode > 0 AND A."ClgCode" = :ClgCode)
	OR
	(
		:ClgCode <= 0 AND 
		(
			((:DocType > 0 and A."DocType" = :DocType) OR :DocType <= 0) 
			AND ((:DocType > 0 and A."DocType" = :DocType) OR :DocType <= 0)
			AND ((:StartTime is not null and A."Recontact" >= :StartTime) OR :StartTime is null)
			AND ((:EndTime is not null and A."endDate" <= :EndTime) OR :EndTime is null)
			AND ((:AttendUser > 0 and A."AttendUser" = :AttendUser) OR :AttendUser <= 0)
			AND ((:AttendEmpl > 0 and A."AttendEmpl" = :AttendEmpl) OR :AttendEmpl <= 0)
			AND ((:CardCode is not null and A."CardCode" = :CardCode) OR :CardCode is null)
		)
	);
END



