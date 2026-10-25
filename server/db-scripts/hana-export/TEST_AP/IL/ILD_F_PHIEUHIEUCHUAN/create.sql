CREATE procedure ILD_F_PhieuHieuChuan
(
	in DocID int
)
as
Begin
	create local temporary table #tbl
	(
			RptName		nvarchar(100) default 'Calibration Certificate', 
	 		CompName	nvarchar(300),
			CompAddr	nvarchar(300), 
			CompTel		nvarchar(30),
			CompFax		nvarchar(30),
			--format number
			AmountDec		smallint,
			QtyDec			smallint,
			PriceDec		smallint,
			RateDec			smallint,
			PercentDec		smallint,
			DecSep			nvarchar(1),
			ThousSep		nvarchar(1),
			DocCurr			nvarchar(10),
			-- Group report
			GroupReport smallint,
			--Position
	 		P_ItemCode nvarchar(150),
			P_ItemName	nvarchar(150), 
			P_Area	nvarchar(100),
			--Device
	 		DV_Manufacturer_Model nvarchar(100),
			DV_InstallDate	nvarchar(10),--date
			DV_Range	nvarchar(100), 
			DV_Classification	nvarchar(50),
			DV_ProcessMedium 	nvarchar(100),
			--Calibration Procedure
	 		C_Strategy nvarchar(50),
			C_Uncertainty	decimal(21,6),
			
			C_Reject decimal(21,6),
			C_Adjust decimal(21,6),
			C_Interval int,
			C_Time 	nvarchar(10),--date
			C_NextDate 	nvarchar(10),--date
	 		C_ReferenceStandard 	nvarchar(100),
	 		C_ServiceProvider	nvarchar(100),
	 		
	 		Remark nvarchar(100),
			---Detail Title
			AsFoundTitle   nvarchar(20) default 'AS FOUND',
			AsFoundResult nvarchar(10) default 'PASSED',
			--
			AdjustTitle   nvarchar(20) default 'ADJUST',
			AdjustResult nvarchar(10) default 'PASSED',
			
			--- Detail
			Nominal_Input nvarchar(50),
			Actual_Input decimal(21,6),
			Nominal_Output decimal(21,6),
			Actual_Output decimal(21,6),
			FoundError decimal(21,6),
			
			Actual_OutputAsLeft decimal(21,6),
			FoundError_AsLeft decimal(21,6),
			CalibrationEvaluation nvarchar(10)
		
	);
	 
	insert into #tbl (	GroupReport,
						Remark,
						--Position
						P_ItemCode,
						P_ItemName,
						P_Area,
						--Device
						DV_Manufacturer_Model,
						DV_InstallDate,
						DV_Range,
						DV_Classification,
						DV_ProcessMedium,
						--Calibration Procedure
						C_Strategy,
						C_Uncertainty,
						C_Reject,
						C_Adjust,
						C_Interval,
						C_Time,
						C_NextDate,
						C_ReferenceStandard,
						C_ServiceProvider,
						
						--- Detail
						Nominal_Input,
						Actual_Input,
						Nominal_Output,
						Actual_Output,
						FoundError,
						
						Actual_OutputAsLeft,
						FoundError_AsLeft,
						CalibrationEvaluation)
	
	select  1 as "GroupReport",
			a."Remark",
			a."U_ItemCode" as "P_ItemCode",
			a."U_ItemName" as "P_ItemName",
			a."U_Area" as "P_Area",
--Device
			"U_Manufacturer" || '/ ' || "U_Model" as "DV_Manufacturer_Model",
			TO_VARCHAR(a."U_InstallationDate",'DD/MM/YYYY') as "DV_InstallDate",
			a."U_Range" as "DV_Range",
			a."U_Classification" as "DV_Classification",
			a."U_ProcessMedium" as "DV_ProcessMedium",
--Calibration Procedure
			a."U_Strategy" as "C_Strategy",
			a."U_Uncertainty" as "C_Uncertainty",
			
			b."U_RejectIfError" as "C_Reject",
			b."U_AdjustIfError" as "C_Adjust",
			a."U_IntervalC" as "C_Interval",
			TO_VARCHAR(a."U_CalibrationTime",'DD/MM/YYYY') as "C_Time", -- Date
			TO_VARCHAR(a."U_NextDate",'DD/MM/YYYY') as "C_NextDate", -- Date
			a."U_ReferenceStandard" as "C_ReferenceStandard",
			a."U_ServiceProvider" as "C_ServiceProvider",
--- Detail
			b."U_NominalInput" as "Nominal_Input",
			b."U_ActualInput" as "Actual_Input",
			b."U_NominalOutput" as "Nominal_Output",
			b."U_ActualOutputAsFound" as "Actual_Output",
			b."U_FoundErrorAsFound" as "FoundError",
			
			b."U_ActualOutputAsLeft" as "Actual_OutputAsLeft",
			b."U_FoundErrorAsLeft" as "FoundError_AsLeft",
			b."U_CalibrationEvaluation" as "CalibrationEvaluation"
	
	from  "@CALIBRATIONS_H" a
	left  join "@CALIBRATIONS_L" b on a."DocEntry"  =  b."DocEntry"  
	where  a."DocEntry"  =  :DocID;

------ Group report  = 2--> Co dieu chinh Adjust Input 
	insert into #tbl (	GroupReport,
						Remark,
						
						--- Detail
						Nominal_Input,
						Actual_Input,
						Nominal_Output,
						Actual_Output,
						FoundError,
						
						Actual_OutputAsLeft,
						FoundError_AsLeft,
						CalibrationEvaluation)
	
	select  2 as "GroupReport",
			a."Remark",
			
--- Detail
			b."U_NominalInput" as "Nominal_Input",
			b."U_ActualInput" as "Actual_Input",
			b."U_NominalOutput" as "Nominal_Output",
			b."U_ActualOutputAsFound" as "Actual_Output",
			b."U_FoundErrorAsFound" as "FoundError",
			
			b."U_ActualOutputAsLeft" as "Actual_OutputAsLeft",
			b."U_FoundErrorAsLeft" as "FoundError_AsLeft",
			b."U_CalibrationEvaluation" as "CalibrationEvaluation"
	
	from  "@CALIBRATIONS_H" a
	left  join "@CALIBRATIONS_L" b on a."DocEntry"  =  b."DocEntry"  
	where  a."DocEntry"  =  :DocID and ifnull(b."U_ActualOutputAsLeft",0) >0;

	IF (Select count (*) from "@CALIBRATIONS_L" where "DocEntry" = :DocID    and ifnull("U_ActualOutputAsLeft",0) >0)>0 then
		update a
		set AsFoundResult  = 'FAILED'
		from #tbl a;
		
		If (Select count (*) from "@CALIBRATIONS_L" where "DocEntry" = :DocID    and ifnull("U_CalibrationEvaluation",'') ='KD')>0  then
			update a
			set AdjustResult  = 'FAILED'
			from #tbl a;
		Else 
			update a
			set AdjustResult  = 'PASSED'
			from #tbl a;
		
		End If;
	End IF;
	
-- update thong tin cong ty va dinh dang so
	update x set CompName = y."CompnyName", CompAddr = SUBSTR_BEFORE(y."CompnyAddr",char(13)) from #tbl x, OADM y;
	
	update x set 
				AmountDec	= 1,
				QtyDec		= 1,
				PriceDec	= 2,
				RateDec		= 1,
				PercentDec	= 1,
				DecSep		= '.',
				ThousSep	= ','
	from #tbl x;
	--x inner join OADM y on 1=1;	
	 
		
	select * from #tbl order by GroupReport;
	
	drop table #tbl;

End;

