CREATE procedure APZ_CTA_TaoPhieuHieuChuanHangLoat
as
begin
	create local temporary table #temp -- Bang nay chua du lieu tu UI (giao dien)
	(
		ITEMCODE nvarchar(50),
		ITEMNAME nvarchar(150),
		STARTDATE date, 
		INTERVAL smallint,
		ENDDATE date,
		NEXTDATE date,
		CREATEDBYCODE nvarchar(10),
		NOMINALINPUT nvarchar(50)
		
	);
	execute immediate 'drop table #temp';
	execute immediate 'create local temporary table #temp  as  (select  * from #TEMPUI);';
	-- temp 2
	create local temporary table #temp2 -- Bang nay chua du lieu tu UI (giao dien)
	(
		STT smallint,
		DocDate date, 
		Creator nvarchar(10),
		ItemCode nvarchar(50),
		ItemName nvarchar(100),
		Area nvarchar(50),
		Manufacturer nvarchar(50),
		Model nvarchar(100),
		Range nvarchar(50),
		Classification nvarchar(50),
		ProcessMedium nvarchar(50),
		Strategy nvarchar(150),
		MPE nvarchar(50),
		InstallationDate date,
		ReferenceStandard nvarchar(100),
		ServiceProvider nvarchar(100),
		
		StartDate date,
		IntervalC smallint,
		EndDate date,
		NextDate date,
		Uncertainty decimal(19,6),

		-- Detail
		NominalInput nvarchar(50), --Gia tri nhap o Item master data: 50;60;80-KGS
		RejectIfError decimal(19,6),
		AdjustIfError decimal(19,6),
		Unit nvarchar(20),
		NominalOutput decimal(19,6)
		
	);

Insert into  #temp2 (ItemCode,NominalInput, STT)					
select   a.ITEMCODE	,b."U_NominalInput" ,0	
from #temp a
inner join OITM b on a.ITEMCODE  = b."ItemCode"
where INTERVAL>0;
 



IF  Exists(Select 1 From #temp2) then 
	Begin
		Declare loopCount int = 0;
		Declare item_Code,nominal_Input nvarchar(50);
		Declare i int = 1;
		Select Count(*) Into loopCount From #temp2;
		create local temporary table #tbl as (Select *, ROW_NUMBER() OVER () AS row_num From #temp2);

		While :i <= :loopCount
		  	DO
		  	
		  		Select ItemCOde Into item_Code From #tbl Where row_num = :i;
				Select NominalInput Into nominal_Input From #tbl Where row_num = :i;
				
				
				
		  		insert into #temp2 (NominalOutput,NominalInput,Unit,ItemCode, STT  )
		  		select "NOMINALINPUT", "NOMINALINPUT","UNIT", item_Code,1
		  		from  ILD_FT_SubStringNominalInput_HieuChuan (item_Code,nominal_Input);
		  		i := i + 1;
		END WHILE;
	End;

End If;

 --
delete from #temp2 where STT = 0;
--
select   

		CURRENT_DATE as DocDate,
		d.CREATEDBYCODE as CREATEDBYCODE,
		d.ITEMCODE as ItemCode,
		d.ITEMNAME as ItemName,
		b."U_Area" as Area,
		c."FirmName" as Manufacturer,
		b."U_Model" as Model,
		b."U_Range" as Range,
		--b."U_Classification" as CalibrationTime,
		b."U_ProcessMedium" as ProcessMedium,
		b."U_Strategy" as Strategy,
		b."U_InstallationDate" as InstallationDate,
		b."U_Uncertainty" as Uncertainty,
		b."U_ReferenceStandard" as ReferenceStandard,
		d.STARTDATE  as StartDate, 
		d.INTERVAL as IntervalC,
		d.ENDDATE as EndDate,
		d.NEXTDATE as NextDate,
		a.NominalInput,
		b."U_RejectIfError" as RejectIfError,
		b."U_AdjustIfError" as AdjustIfError,
		a.NominalOutput,
		a.Unit
 
from #temp2 a
inner join  OITM b on a.ITEMCODE  = b."ItemCode"
left join OMRC c on b."FirmCode"  =  c."FirmCode" 
inner join #temp d on d.ITEMCODE  = a.ItemCode
order by ITEMCODE;
--
drop table #temp2;
drop table #temp;
exec 'drop table #TEMPUI';

end;

