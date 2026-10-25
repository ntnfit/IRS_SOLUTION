
create procedure APZ_TrackingIssue_Save(in type char(1))
as
begin
	Declare Id int =0;
	Declare isIssueDate int=0;
	Declare isStatus int=0;
	Declare stt nvarchar(50)='';
	/* Attachment Files */
	Declare AbsEntry int =0;
	Declare atc int =0;
	declare lineId int = 0;
	declare checkCount int = 0;
	declare objCode nvarchar(50) = '';
	
	create local temporary table #APZ_TGIEData as (select top 0 y.* from (select 1 TempEntry from DUMMY) x left join APZ_TGIE y on 1=1);
		
	-- ở đây là lệnh đổ dữ liệu từ table bulkcopy sang table tep vừa tạo chú ý table temp từ bulk copy có dấu nháy "" vì phân biệt hoa thường
	execute immediate 'insert into #APZ_TGIEData select * from "#APZ_TGIE"';
	
	create local temporary table #APZ_TGIE1Data as (select top 0 y.* from (select 1 TempEntry from DUMMY) x left join APZ_TGIE1 y on 1=1);
	execute immediate 'insert into #APZ_TGIE1Data select * from "#APZ_TGIE1"';
	
	create local temporary table #ATC1Data as (select top 0 y.* from (select 1 TempEntry from DUMMY) x left join ATC1 y on 1=1);
	execute immediate 'insert into #ATC1Data select * from "#ATC1"';

	create local temporary table #APZ_D_UUPIMLocal as (select top 0 * from APZ_D_UUPIM);
	execute immediate 'insert into #APZ_D_UUPIMLocal select * from "#APZ_D_UUPIM"';
	
	if :type='U'
	then
		select ifnull((select top 1 "Id" from #APZ_TGIEData),0) into Id from DUMMY;
		if :Id=0
			then
				select 100 ,'Id not null' from dummy;
				return;
			else
				select IFNULL((SELECT "AttachedFiled" FROM APZ_TGIE  WHERE  "Id"= :Id),0) into AbsEntry FROM DUMMY;
				delete from APZ_TGIE1 where "Id" =:Id;
				delete from APZ_TGIE where "Id"=:Id;
			end if;
		
	else
		
		select IFNULL((select max("Id") from APZ_TGIE),0) + 1 into Id from DUMMY;
		update #APZ_TGIEData set "Id" = :Id,"IssueDeleted"='N';
		
	end if;
	
	objCode := :Id;

	select IFNULL((select top 1 1 from #APZ_TGIEData  where ifnull("IssueDate",'')=''),0) into isIssueDate FROM DUMMY;
	if(ifnull(:isIssueDate,0)=1)
		then

			select 123 ,'Issue Date  is missing' from dummy;
			return;

		end if;
	
		Select count(1) into atc from #ATC1Data;
		IF (atc > 0)
			then
				IF (:AbsEntry <= 0)
					then /* Add New */
						-- Set new attachment absEntry to series table
						insert into OATC ("AbsEntry")  (select ifnull(max("AbsEntry"),0)+1 From OATC);
		                Update ONNM set "AutoKey"=(select ifnull( max("AbsEntry"),0)+1 from OATC) where "ObjectCode"='221';
		             	 select ifnull(max("AbsEntry"),0) into AbsEntry from OATC; -- Get new absEntry
		
						IF (:AbsEntry > 0) -- new AbsEntry is valid
							then
								-- update new AbsEntry to attachment temp table and EmpRelatives temp table
								Update #ATC1Data SET "AbsEntry"=:AbsEntry;
								Update #APZ_TGIEData SET "AttachedFiled"=:AbsEntry;
								-- Insert new file entry
								INSERT INTO  ATC1 
			                               ( "AbsEntry" , "Line" , "srcPath" , "trgtPath" , "FileName" , "FileExt" , "Date" , "UsrID" 
			                               , "Copied" , "Override" , "subPath" )
			                    SELECT a."AbsEntry",a."Line",a."srcPath",a."trgtPath",a."FileName",a."FileExt",a."Date",a."UsrID"
			                               ,a."Copied",a."Override",a."subPath" 
								FROM #ATC1Data a;
							END if;
					
					ELSE
					/* Update */
						-- delete all file entry of this document
						DELETE FROM ATC1 WHERE "AbsEntry"=:AbsEntry;
						-- update new AbsEntry to attachment temp table
						UPDATE #ATC1Data SET "AbsEntry"=:AbsEntry;
						UPDATE #APZ_TGIEData SET "AttachedFiled"=:AbsEntry;
						-- Insert new file entry
						INSERT INTO  ATC1  ( "AbsEntry" , "Line" , "srcPath" , "trgtPath" , "FileName" , "FileExt" , "Date" , "UsrID" 
		                            , "Copied" , "Override" , "subPath" )
		                SELECT a."AbsEntry",a."Line",a."srcPath",a."trgtPath",a."FileName",a."FileExt",a."Date",a."UsrID"
		                            ,a."Copied",a."Override",a."subPath" 
						FROM #ATC1Data a ;
					END IF;
	
			
		ELSE
		/* No file in documents delete all file entry of this document */
			DELETE FROM ATC1 WHERE "AbsEntry"=:AbsEntry;
			-- duplicate issue: prevent new issue's attachment link to old issue's attachment
			if :type = 'A' then 
				update #APZ_TGIEData set "AttachedFiled" = NULL;
			end if;
		END IF;

	delete from APZ_D_UUPIM where "ObjectCode" = :objCode and "ObjectType" = '1470000130';
	
	UPDATE #APZ_TGIE1Data set "Id" = :Id, "LineNum" = -1;--,"LineNum"= NumId FROM (SELECT *,  ROW_NUMBER() OVER( ORDER BY Id) NumId FROM #APZ_TGIE1Data);
	
	select IFNULL((select top 1 1 from #APZ_TGIE1Data where "LineNum" = -1), 0) into checkCount from DUMMY;
	WHILE :checkCount = 1
	DO
		update top 1 #APZ_TGIE1Data set "LineNum" = :lineId where "LineNum" = -1;
		lineId = :lineId + 1;
		select IFNULL((select top 1 1 from #APZ_TGIE1Data where "LineNum" = -1), 0) into checkCount from DUMMY;
	END WHILE;
	
	update #APZ_D_UUPIMLocal set "ObjectCode" = :objCode from #APZ_D_UUPIMLocal;
	
	insert into APZ_TGIE1 select * from #APZ_TGIE1Data;
	insert into APZ_TGIE select * from #APZ_TGIEData;
	insert into APZ_D_UUPIM select * from #APZ_D_UUPIMLocal;
	
	select :Id,'' from dummy;
end;




