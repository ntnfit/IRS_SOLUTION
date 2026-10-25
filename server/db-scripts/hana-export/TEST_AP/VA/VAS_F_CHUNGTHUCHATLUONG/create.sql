CREATE procedure VAS_F_ChungThuChatLuong (
	in DocID	int
	
)
as
begin
	create local temporary table #tbl (
		 CompName		nvarchar(100)
		,CompAdd		nvarchar(254)
		,DocEntry		int  
		,BaseEntry		int
		,BaseLine		int
		,BaseType		int
		,Product		nvarchar(50)   
		,ProductName	nvarchar(50)  
		,ProductDate	timestamp
		,ExpDate		timestamp
		,Qty			numeric(21,6)  
		,IssuedDate		timestamp  
		,PO				nvarchar(50)
		,LotNumber		nvarchar(100)
		,Parameter		nvarchar(100)  
		,Unit			nvarchar(100)  
		,Specs			nvarchar(100)  
		,InnOut			nvarchar(100)  
		,Result			nvarchar(100)  
		,Method			nvarchar(100)  
	);

	insert into #tbl(DocEntry, Qty, IssuedDate, Parameter, Unit, Specs,  Result,InnOut, Method,
					Product,ProductName, LotNumber,BaseEntry )
	select a."DocEntry",
		    a."U_QuantityTotal",
		    a."U_QCDate",
		    b."U_Name" as "TenChiTieu",
		    b."U_UoM",
		    case when b."U_ValueType"  = 'N' and  b."U_Operator" = 'BT' and ifnull("U_Target_Number_1",0) > 0 and ifnull("U_Target_Number_2",0) > 0 then
		    		to_varchar(b."U_Target_Number_1",'0.00') || ' - ' || to_varchar(b."U_Target_Number_2",'0.00')
		    		
		    	when b."U_ValueType"  = 'N'and  b."U_Operator" = 'BT' and ifnull("U_Target_Number_1",0) = 0 and ifnull("U_Target_Number_2",0) > 0 then
		    		 '≤ ' || to_varchar(b."U_Target_Number_2",'0.00')
		    		 
		    	when b."U_ValueType"  = 'N'and  b."U_Operator"  = '>='   then
		    		 '≥ ' || to_varchar(b."U_Target_Number_1",'0.00')
		    	when b."U_ValueType"  = 'N'and  b."U_Operator"  in ('<','>')   then
		    		b."U_Operator" || ' ' || to_varchar(b."U_Target_Number_1",'0.00')	 
		    		 
		    	when b."U_ValueType"  = 'N' and b."U_Operator" not in ('BT','>','>=','<','<=') then b."U_Operator" || ' ' || ifnull(to_varchar(b."U_Target_Number_2",'0.00'),0)	 
		       else  '' end as  "Specs",
		    --InnOut --> Actual_Number
			(case 
					when b."U_ValueType" = 'B' then b."U_Actual_Boolean" 
					when b."U_ValueType" = 'N' and ifnull(b."U_Actual_Number",0) != 0 then to_varchar(b."U_Actual_Number",'0.00')
					else '0'
			end) as "Result",
		    --
		case when ifnull(b."U_QAResult",'') ='NG' then 'NOT OK' else ifnull(b."U_QAResult",'') end  as "InnOut",
		b."U_Method",
		
		"U_ItemCode",
		"U_ItemName",
		a."U_Batch" as "LotNumber",
		case when ifnull("U_BaseDocType",'') = '202'  then a."U_BaseDocEntry" else  0 end POEnTry
		
	from "@QC_RESULT_H" a 
	left join "@QC_RESULT_CHAR_L" b on a."DocEntry"  = b."DocEntry"
	where a."DocEntry"  =:DocID;
	 
-- Update thong tin cong ty
	update #tbl set CompName = (select "CompnyName" from "OADM");
	update #tbl set	CompAdd = (select SUBSTR_BEFORE("CompnyAddr", char(13)) from "OADM");
-- Update thong tin Batch
update a
set  a.ProductDate = b."MnfDate",
	 a.ExpDate =  b."ExpDate"
from #tbl a 
left join (select distinct "MnfDate","ExpDate","MnfSerial" from OBTN ) b on b."MnfSerial"  =  a.LotNumber ;
--
update a
set PO  = (select "DocNum" from OWOR where "DocEntry" = a.BaseEntry)
from #tbl a;


	select * from #tbl;
	drop table #tbl;
end

