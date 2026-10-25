CREATE procedure ILD_SSCC_Check_QCResult
(
	in DocEntry varchar(20),
	out tb table 
	(
		"SSCC" nvarchar(20),
		"Whs" nvarchar(20)
		
	)
)
as
begin
declare QCRESULT_SSCC table 
(
		Row_num int,
		ItemCode varchar(50),
		ItemName nvarchar(150),
		Batch	 varchar(50),
		---Detail: Ins Lot Stock
		Type varchar(20),
		Status varchar(20),
		InsLotStock_SSCC  varchar(5000),
		ToWhs varchar(50)
);
declare  SSCCDetail_QAResult table 
(
			Row_num int,
			SSCC_Part nvarchar(50),
			SSCC_Entry int,
			WhseCurrent varchar(20),
			OnHandQty decimal(19,6),
			--
			BinAbs int, --  Lấy bin hiện tại: Bin From 
			BinCode varchar(50), -- Bin code: Bin From
			ToBinAbs int,  -- Bin đến,
			ToWhs varchar(50)
			
			
);
	
	insert into :QCRESULT_SSCC (Row_num, ItemCode, ItemName,  Batch,
							Type,Status, InsLotStock_SSCC)
	select  ROW_NUMBER() OVER (),
			a."U_ItemCode",
			a."U_ItemName",
			a."U_Batch",
			---Detail: Ins Lot Stock
			b."U_Type",
			b."U_Status",
			b."U_SSCC"
			
	from "@QC_RESULT_H" a 
	inner join "@QC_RESULT_INS_L" b  on a."DocEntry"  =  b."DocEntry"
	 
	where cast(a."DocEntry" as varchar(20))=  :DocEntry
			and ifnull(b."U_Status",'')  = 'RELEASE'
			and ifnull(b."U_Type",'') <> 'RE-EXAMINATION'
			and ifnull(cast(b."U_SSCC" as varchar(5000)),'') <> '';
	
--

	Begin
		Declare loopCount int = 0;
		Declare i int = 1;
		Declare QAResult_SSCC nvarchar(5000);
		
		Select Count(*) Into loopCount From :QCRESULT_SSCC;
		       While :i <= :loopCount
					DO
					Select a.InsLotStock_SSCC Into QAResult_SSCC From :QCRESULT_SSCC a Where Row_num = :i;
					
				 	insert into :SSCCDetail_QAResult ( SSCC_Part, Row_Num)
				 	select  "part",i
				 	from  "SPLITSTRING" (:QAResult_SSCC,',');
					
						i := i + 1;
				END WHILE;
	
	End;
-- Update thông tin key của sscc
		update a
		set SSCC_Entry  = b."AbsEntry"
		from :SSCCDetail_QAResult a
		inner join OBTN b on a.SSCC_Part  = b."DistNumber"  
					and b."ItemCode" = (select top 1 ItemCode from  :QCRESULT_SSCC);
-- Update thông tin kho hiện tại
	update a
	set WhseCurrent  = b."WhsCode", OnHandQty  = b."OnHandQty"
	from :SSCCDetail_QAResult a
	inner join OBBQ b on a.SSCC_Entry = b."SnBMDAbs" ;
--- Update thông tin bin hiện tại (FromBin)
	update a
	set  BinAbs  = b."BinAbs",
		 BinCode  =  c."BinCode"
	from :SSCCDetail_QAResult a
	inner join OBBQ b on a.SSCC_Entry = b."SnBMDAbs" and a. WHSECURRENT  =  b."WhsCode" and b."OnHandQty">0
	inner join OBIN c on  b."BinAbs"  = c."AbsEntry"; 

--
	tb = select SSCC_Part as  "SSCC",WHSECURRENT as "Whs"  from :SSCCDetail_QAResult;

end;
