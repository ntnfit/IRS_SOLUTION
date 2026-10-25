
CREATE PROCEDURE  apz_process_EmpRelatives
(Typ char(1) default '')
AS
BEGIN

	declare EmpId int;
	declare RelaId int;
	declare lastName nvarchar(50);
	declare firstName nvarchar(50);
	declare gender varchar(1);
	declare relationship varchar(2);
	declare checkAtc int;
	create local temporary table #APZ_EmpRela_DOC as (select top 0 b.* from (select 1 A from dummy) left join APZ_OHER b on 1=2);
	
	-- ở đây là lệnh đổ dữ liệu từ table bulkcopy sang table tep vừa tạo chú ý table temp từ bulk copy có dấu nháy "" vì phân biệt hoa thường
	execute immediate 'insert into #APZ_EmpRela_DOC select * from "#APZ_EmpRela_DOC"';
	
	create local temporary table #APZ_EmpRela_ATC as (select top 0* from ATC1);
	execute immediate 'insert into #APZ_EmpRela_ATC select * from "#APZ_EmpRela_ATC"';
	
	SELECT TOP 1 "empID","relaID","lastName","firstName","sex","relationship" into EmpId,RelaId,lastName,firstName,gender,relationship from #APZ_EmpRela_DOC;
	if :Typ='A'
	THEN
		SELECT IFNULL((Select max("relaID") from APZ_OHER where "empID" = :EmpId),0) +1 into RelaId from Dummy;

		-- set new relaID code to temp table
		UPDATE #APZ_EmpRela_DOC SET "relaID" = :RelaId;
		
	END IF;
	
	BEGIN
	declare error int =0;
	declare error_prefix int =1711000;


					if IFNULL(:RelaId,0) = 0 /* RelaID */
					then
					BEGIN
						SELECT  
							:error_prefix+1 "ErrorNumber"
							,N'Relatives ID cannot be empty or equal 0' "ErrorMessage" From Dummy;
						return;
					END;
					end if;
					if IFNULL(:EmpId,0) = 0 /* EmpID */
					then
					BEGIN
						SELECT  
							:error_prefix+2 "ErrorNumber"
							,N'Employee ID cannot be empty or equal 0' "ErrorMessage" From Dummy;
						return;
					END;
					end if;

					if ltrim(rtrim(IFNULL(:firstName,'')))='' /* First name */
					then
					BEGIN
						SELECT  
							:error_prefix+3 "ErrorNumber"
							,N'First name cannot be empty' "ErrorMessage" From Dummy;
						return;
					END;
					end if;

					if ltrim(rtrim(IFNULL(:lastName,'')))='' /* Last name */
					then
					BEGIN
						SELECT  
							:error_prefix+4 "ErrorNumber"
							,N'Last name cannot be empty' "ErrorMessage" From Dummy;
						return;
					END;
					end if;

					if ltrim(rtrim(IFNULL(:gender,'')))='' /* Last name */
					then
					BEGIN
						SELECT  
							:error_prefix+5 "ErrorNumber"
							,N'Gender cannot be empty' "ErrorMessage" From Dummy;
						return;
					END;
					end if;

					if ltrim(rtrim(IFNULL(:relationship,'')))='' /* Last name */
					then
					BEGIN
						SELECT  
							:error_prefix+6 "ErrorNumber"
							,N'Relationship cannot be empty' "ErrorMessage" From Dummy;
						return;
					END;
					end if;
					
		
	BEGIN /* Attachment Files */
		declare AbsEntry int;
		SELECT IFNULL((SELECT "AtcEntry" FROM APZ_OHER  WHERE  "empID"= :EmpId And "relaID" = :RelaId),0) into AbsEntry from DUMMY;
		select count(*) into checkAtc from #APZ_EmpRela_ATC;
		IF (checkAtc >0)
		then
		BEGIN
			IF (:AbsEntry <= 0)
			then
			BEGIN /* Add New */
				-- Set new attachment absEntry to series table
				insert into OATC ("AbsEntry")  select IFNULL(max("AbsEntry"),0)+1 From OATC;
                Update ONNM set "AutoKey"=IFNULL((select max("AbsEntry")+1 from OATC),0) where "ObjectCode"='221';
               select IFNULL((select max("AbsEntry") from OATC),0) into AbsEntry from Dummy; -- Get new absEntry

				IF :AbsEntry > 0
				then -- new AbsEntry is valid
				BEGIN
					-- update new AbsEntry to attachment temp table and EmpRelatives temp table
					Update #APZ_EmpRela_ATC SET "AbsEntry"=:AbsEntry;
					Update #APZ_EmpRela_DOC SET "AtcEntry"=:AbsEntry;
					-- Insert new file entry
					INSERT INTO  ATC1 
                               
                    SELECT *
					FROM #APZ_EmpRela_ATC a;
				END;
				end if;
			END;
			ELSE
			BEGIN /* Update */
				-- delete all file entry of this document
				DELETE FROM ATC1 WHERE "AbsEntry"=:AbsEntry;
				-- update new AbsEntry to attachment temp table
				UPDATE #APZ_EmpRela_ATC SET "AbsEntry"=:AbsEntry;
				UPDATE #APZ_EmpRela_DOC SET "AtcEntry"=:AbsEntry;
				-- Insert new file entry
				INSERT INTO ATC1         
                SELECT *
				FROM #APZ_EmpRela_ATC a;
			END;
			end if;

		END;
		ELSE
		BEGIN /* No file in documents delete all file entry of this document */
			DELETE FROM ATC1 WHERE "AbsEntry"=:AbsEntry;
		END;
		end if;
	END;
	
	DELETE FROM APZ_OHER WHERE "empID"=:EmpId and "relaID" = :RelaId;
	--select Docentry from #APZ_BP_DOC
	INSERT INTO APZ_OHER SELECT * FROM #APZ_EmpRela_DOC;
END;
END




