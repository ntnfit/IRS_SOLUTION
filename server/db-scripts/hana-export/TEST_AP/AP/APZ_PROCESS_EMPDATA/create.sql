
CREATE PROCEDURE apz_process_EmpData(in Typ char(1) default '')
as
BEGIN
	declare EmpId int default 0;
	declare lastName nvarchar(50) default '';
	declare firstName nvarchar(50) default '';
	declare userCode int default 0;
	declare EmpLinkedUser int default 0;
	
	create local temporary table #APZ_Emp_DOC as (select top 0 * from OHEM);	
	-- ở đây là lệnh đổ dữ liệu từ table bulkcopy sang table tep vừa tạo chú ý table temp từ bulk copy có dấu nháy "" vì phân biệt hoa thường
	execute immediate 'drop table #APZ_Emp_DOC';
	execute immediate 'create local temporary table #APZ_Emp_DOC as (select * from "#APZ_Emp_DOC")';
	
	create local temporary table #APZ_Emp_HEM6 as (select top 0 * from HEM6);	
	-- ở đây là lệnh đổ dữ liệu từ table bulkcopy sang table tep vừa tạo chú ý table temp từ bulk copy có dấu nháy "" vì phân biệt hoa thường
	execute immediate 'drop table #APZ_Emp_HEM6';
	execute immediate 'create local temporary table #APZ_Emp_HEM6 as (select * from "#APZ_Emp_HEM6")';
	
	create local temporary table #APZ_Emp_HTM1 as (select top 0 * from HTM1);
	-- ở đây là lệnh đổ dữ liệu từ table bulkcopy sang table tep vừa tạo chú ý table temp từ bulk copy có dấu nháy "" vì phân biệt hoa thường
	execute immediate 'drop table #APZ_Emp_HTM1';
	execute immediate 'create local temporary table #APZ_Emp_HTM1 as (select * from "#APZ_Emp_HTM1")';
	
	create local temporary table #APZ_Emp_HEM1 as (select top 0 * from HEM1);
	-- ở đây là lệnh đổ dữ liệu từ table bulkcopy sang table tep vừa tạo chú ý table temp từ bulk copy có dấu nháy "" vì phân biệt hoa thường
	execute immediate 'drop table #APZ_Emp_HEM1';
	execute immediate 'create local temporary table #APZ_Emp_HEM1 as (select * from "#APZ_Emp_HEM1")';
	
	create local temporary table #APZ_Emp_HEM2 as (select top 0 * from HEM2);
	-- ở đây là lệnh đổ dữ liệu từ table bulkcopy sang table tep vừa tạo chú ý table temp từ bulk copy có dấu nháy "" vì phân biệt hoa thường
	execute immediate 'drop table #APZ_Emp_HEM2';
	execute immediate 'create local temporary table #APZ_Emp_HEM2 as (select * from "#APZ_Emp_HEM2")';
	
	create local temporary table #APZ_Emp_HEM3 as (select top 0 * from HEM3);
	-- ở đây là lệnh đổ dữ liệu từ table bulkcopy sang table tep vừa tạo chú ý table temp từ bulk copy có dấu nháy "" vì phân biệt hoa thường
	execute immediate 'drop table #APZ_Emp_HEM3';
	execute immediate 'create local temporary table #APZ_Emp_HEM3 as (select * from "#APZ_Emp_HEM3")';
	
	create local temporary table #APZ_Emp_HEM4 as (select top 0 * from HEM4);
	-- ở đây là lệnh đổ dữ liệu từ table bulkcopy sang table tep vừa tạo chú ý table temp từ bulk copy có dấu nháy "" vì phân biệt hoa thường
	execute immediate 'drop table #APZ_Emp_HEM4';
	execute immediate 'create local temporary table #APZ_Emp_HEM4 as (select * from "#APZ_Emp_HEM4")';
	
	create local temporary table #APZ_Emp_OHEH as (select top 0 * from APZ_OHEH);
	-- ở đây là lệnh đổ dữ liệu từ table bulkcopy sang table tep vừa tạo chú ý table temp từ bulk copy có dấu nháy "" vì phân biệt hoa thường
	execute immediate 'drop table #APZ_Emp_OHEH';
	execute immediate 'create local temporary table #APZ_Emp_OHEH as (select * from "#APZ_Emp_OHEH")';
		
	create local temporary table #APZ_Contract as (select top 0 * from APZ_OHEC);
	-- ở đây là lệnh đổ dữ liệu từ table bulkcopy sang table tep vừa tạo chú ý table temp từ bulk copy có dấu nháy "" vì phân biệt hoa thường
	execute immediate 'drop table #APZ_Contract';
	execute immediate 'create local temporary table #APZ_Contract as (select * from "#APZ_Contract")';
		
	update #APZ_Emp_DOC set "userId"= null where "userId"<=0;
	
	
	SELECT TOP 1 "empID","lastName","firstName" into EmpId,lastName,firstName FROM #APZ_Emp_DOC;
	EmpLinkedUser := EmpId;
	
	BEGIN
		declare Line int = 0;
		declare error int =0;
		declare error_prefix int =1710000;
		declare existHem6 int = 0;
		declare existHtm1 int = 0;
		declare existHem1 int = 0;
		declare existHem2 int = 0;
		declare existHem3 int = 0;
		declare existHem4 int = 0;
		declare existOheh int = 0;
		
		if :EmpId = 0  and :Typ !='A'/* EmpID */
		THEN
			SELECT  :error_prefix+1 "ErrorNumber",'Employee ID cannot be empty or equal 0' "ErrorMessage" FROM DUMMY;
			return;
		END IF;
		
		if ltrim(rtrim(:firstName))='' /* First name */
		THEN
			SELECT :error_prefix+2 "ErrorNumber",'First name cannot be empty' "ErrorMessage" FROM DUMMY;
			return;
		END IF;
		
		if ltrim(rtrim(:lastName))='' /* Last name */
		THEN
			SELECT :error_prefix+3 "ErrorNumber",N'Last name cannot be empty' "ErrorMessage" FROM DUMMY;
			return;
		END IF;
				
		if (:EmpId != :EmpLinkedUser AND :EmpLinkedUser != 0)
		THEN
			SELECT  :error_prefix+11 "ErrorNumber",	N'User already associated with another employee, or employee already associated with another user' "ErrorMessage" FROM DUMMY;
			return;
		END IF;
				
		-- Role
		SELECT IFNULL((Select top 1 1  From #APZ_Emp_HEM6 group by "empID","line" having count(*)>1),0) into existHem6 from DUMMY;
		if (:existHem6 >0)
		THEN
			Select "empID" into empID From (Select top 1 "empID","line" From #APZ_Emp_HEM6 group by "empID","line" having count(*)>1) a;
			Select "line" into line From (Select top 1 "empID","line" From #APZ_Emp_HEM6 group by "empID","line" having count(*)>1) a;
		
			SELECT  :error_prefix+4 "ErrorNumber", 'This entry is already exist in the following table HEM6 ((' || :empID || ', ' || :Line || '))' "ErrorMessage" FROM DUMMY;
			return;
		END IF;
				
		
		-- Team Member
		SELECT IFNULL((Select top 1 1 From #APZ_Emp_HTM1 group by "empID","line" having count(*)>1),0) into existHtm1 from DUMMY;
		if (:existHtm1 >0)
		THEN
			Select "empID" into empID From (Select top 1 "empID","line" From #APZ_Emp_HTM1 group by "empID","line" having count(*)>1) a;
			Select "line" into line From (Select top 1 "empID","line" From #APZ_Emp_HTM1 group by "empID","line" having count(*)>1) a;
	
			SELECT  :error_prefix+5 "ErrorNumber", 'This entry is already exist in the following table HTM1 ((' || :empID || ', ' || :Line || '))' "ErrorMessage" FROM DUMMY;
			return;
		END IF;
		
		-- Absence
		select IFNULL((Select top 1 1 From #APZ_Emp_HEM1 group by "empID","line" having count(*)>1),0) into existHem1 from DUMMY;
		if (:existHem1 >0)
		THEN
			Select "empID" into empID From (Select top 1 "empID","line" From #APZ_Emp_HEM1 group by "empID","line" having count(*)>1) a;
			Select "line" into line From (Select top 1 "empID","line" From #APZ_Emp_HEM1 group by "empID","line" having count(*)>1) a;
	
			SELECT  :error_prefix+6 "ErrorNumber", 'This entry is already exist in the following table HEM1 ((' || :empID || ', ' || :Line || '))' "ErrorMessage" FROM DUMMY;
			return;
		END IF;
	
		-- Education
		SELECT IFNULL((Select top 1 1 From #APZ_Emp_HEM2 group by "empID","line" having count(*)>1),0) into existHem2 from DUMMY;
		if (:existHem2 >0)
		THEN
			Select "empID" into empID From (Select top 1 "empID","line" From #APZ_Emp_HEM2 group by "empID","line" having count(*)>1) a;
			Select "line" into line From (Select top 1 "empID","line" From #APZ_Emp_HEM2 group by "empID","line" having count(*)>1) a;
	
			SELECT :error_prefix+7 "ErrorNumber",'This entry is already exist in the following table HEM2 ((' || :empID || ', ' || :Line || '))' "ErrorMessage" FROM DUMMY;
			return;
		END IF;
	
		-- Review
		SELECT IFNULL((Select top 1 1 From #APZ_Emp_HEM3 group by "empID","line" having count(*)>1),0) into existHem3 from DUMMY;
		if (:existHem3 >0)
		THEN
			Select "empID" into empID From (Select top 1 "empID","line" From #APZ_Emp_HEM3 group by "empID","line" having count(*)>1) a;
			Select "line" into line From (Select top 1 "empID","line" From #APZ_Emp_HEM3 group by "empID","line" having count(*)>1) a;
	
			SELECT  :error_prefix+8 "ErrorNumber", 'This entry is already exist in the following table HEM3 ((' || :empID || ', ' || :Line || '))' "ErrorMessage" FROM DUMMY;
			return;
		END IF;
	
		-- Previous Employment
		SELECT IFNULL((Select top 1 1 From #APZ_Emp_HEM4 group by "empID","line" having count(*)>1),0)  into existHem4  from DUMMY;
		if (:existHem4 >0)
		THEN
			Select "empID" into empID From (Select top 1 "empID","line" From #APZ_Emp_HEM4 group by "empID","line" having count(*)>1) a;
			Select "line" into line From (Select top 1 "empID","line" From #APZ_Emp_HEM4 group by "empID","line" having count(*)>1) a;
	
			SELECT :error_prefix+9 "ErrorNumber", 'This entry is already exist in the following table HEM4 ((' || :empID || ', ' || :Line || '))' "ErrorMessage" FROM DUMMY;
			return;
		END IF;
	
		-- Health Status
		SELECT IFNULL((Select top 1 1 From #APZ_Emp_OHEH group by "empID","healthID" having count(*)>1),0) into existOheh FROM DUMMY;
		if (:existOheh >0)
		THEN
			Select "empID" into empID From (Select top 1 "empID","healthID" From #APZ_Emp_OHEH group by "empID","healthID" having count(*)>1) a;
			Select "healthID" into line From (Select top 1 "empID","healthID" From #APZ_Emp_OHEH group by "empID","healthID" having count(*)>1) a;
	
			SELECT :error_prefix+10 "ErrorNumber", N'This entry is already exist in the following table APZ_OHEH ((' || :empID || ', ' || :Line || '))' "ErrorMessage" FROM DUMMY;
			return;
		END IF;
	END;	
	
	BEGIN /* Other table */
	
		BEGIN /* Attachment Files */
			declare AbsEntry int;
			declare existAtc int;
			
			create local temporary table #APZ_Emp_ATC as (select top 0 * from ATC1);
			-- ở đây là lệnh đổ dữ liệu từ table bulkcopy sang table tep vừa tạo chú ý table temp từ bulk copy có dấu nháy "" vì phân biệt hoa thường
			execute immediate 'insert into #APZ_Emp_ATC select * from "#APZ_Emp_ATC"';
			
			SELECT IFNULL((SELECT "AtcEntry" FROM OHEM  WHERE  "empID"= :EmpId),0) into AbsEntry from DUMMY;
			SELECT IFNULL((Select top 1 1 from #APZ_Emp_ATC),0) into existAtc from DUMMY;
			
			IF (:existAtc >0)
			THEN
				BEGIN
					IF (:AbsEntry <= 0)
					THEN
						BEGIN /* Add New */
							-- Set new attachment absEntry to series table
							insert into OATC ("AbsEntry")  (select IFNULL(max("AbsEntry"),0)||1 From OATC);
			                Update ONNM set "AutoKey"= IFNULL((select max("AbsEntry")||1 from OATC),0) where "ObjectCode"='221';
			                select IFNULL((select max("AbsEntry") from OATC),0) into AbsEntry from Dummy; -- Get new absEntry
			
							IF :AbsEntry > 0 -- new AbsEntry is valid
							THEN
								BEGIN
									-- update new AbsEntry to attachment temp table and BP temp table
									Update #APZ_Emp_ATC SET "AbsEntry"=:AbsEntry;
									Update #APZ_Emp_DOC SET "AtcEntry"=:AbsEntry;
									-- Insert new file entry
									INSERT INTO  ATC1 
				                    	SELECT * FROM #APZ_Emp_ATC a;
								END;
							END IF;
						END;
					ELSE
						BEGIN /* Update */
							-- DELETE FROM  all file entry of this document
							DELETE FROM  ATC1 WHERE "AbsEntry"=:AbsEntry;
							-- update new AbsEntry to attachment temp table
							UPDATE #APZ_Emp_ATC SET "AbsEntry"=:AbsEntry;
							UPDATE #APZ_Emp_DOC SET "AtcEntry"=:AbsEntry;
							-- Insert new file entry
							INSERT INTO  ATC1 
				                SELECT * FROM #APZ_Emp_ATC a;
						END;
					END IF;
		
				END;
			ELSE
			 /* No file in documents DELETE FROM  all file entry of this document */
				DELETE FROM  ATC1 WHERE AbsEntry=:AbsEntry;
			END IF;
		END;
	
	END;
	
	if :Typ='A'
	THEN
		BEGIN /* Add New */
		
			BEGIN /* AutoKey*/
				-- Get new CardCode from numering and series table
				declare docnum int;
				SELECT IFNULL((Select top 1 IFNULL( "AutoKey", 1) from onnm where "ObjectCode" = '171'),0) into docnum from DUMMY;
	
				SELECT IFNULL((Select top 1 IFNULL( "AutoKey", 0)
					 from onnm where "ObjectCode" = '171'),0) into EmpId from DUMMY;
	
				-- set new card code to temp table
				UPDATE #APZ_Emp_DOC SET "empID" = :EmpId;
				-- increase next auto number
				UPDATE onnm set "AutoKey" = :docnum || 1  where "ObjectCode" = '171';	
			END;
		
			--select Docentry from #APZ_BP_DOC
			INSERT INTO OHEM SELECT * FROM #APZ_Emp_DOC;
		END;
	ELSEIF  :Typ='U' 
		THEN
			BEGIN /* UPDATE */
				
				DELETE FROM  OHEM WHERE "empID"=:EmpId;
				INSERT INTO OHEM SELECT * FROM #APZ_Emp_DOC;
			END;
	END IF;
	
	BEGIN /* Other table */
	
		BEGIN /* Role */
			declare hasRowHem6 int;
			SELECT IFNULL((Select top 1 1 from #APZ_Emp_HEM6),0) into hasRowHem6 from DUMMY;
			IF :hasRowHem6>0
			THEN
				BEGIN
					
					DELETE FROM  HEM6 WHERE "empID"=:EmpId;
					UPDATE #APZ_Emp_HEM6 SET "empID" = :EmpId;
					-- Insert new Role entry
					INSERT INTO  HEM6 
		            	SELECT * FROM #APZ_Emp_HEM6 a where a."roleID" is not null;
				END;
			ELSE
		 		/* No Role then DELETE FROM  all Role entry of this */
				DELETE FROM  HEM6 WHERE "empID"=:EmpId;
			END IF;
		END;
	
		BEGIN /* Team Member */
		
			declare hasRowHtm1 int;
			SELECT IFNULL((Select top 1 1 from #APZ_Emp_HTM1),0) into hasRowHtm1 from DUMMY;
			IF ( hasRowHtm1>0)
			THEN
				BEGIN
					
					DELETE FROM  HTM1 WHERE "empID"=:EmpId;
					UPDATE #APZ_Emp_HTM1 SET "empID" = :EmpId;
					-- Insert new Team entry
					INSERT INTO  HTM1 
		            	SELECT * FROM #APZ_Emp_HTM1 a where a."teamID" is not null;
				END;
			ELSE
		 		/* No Team then DELETE FROM  all Team entry of this */
				DELETE FROM  HTM1 WHERE "empID"=:EmpId;
			END IF;
		END;
	
		BEGIN /* Absence */
			
			declare hasRowHem1 int;
			SELECT IFNULL((Select top 1 1 from #APZ_Emp_HEM1),0) into hasRowHem1 from DUMMY;
			IF :hasRowHem1>0
			THEN
				BEGIN
					
					DELETE FROM  HEM1 WHERE "empID"=:EmpId;
					UPDATE #APZ_Emp_HEM1 SET "empID" = :EmpId;
					-- Insert new Absence entry
					INSERT INTO  HEM1 
			            SELECT * FROM #APZ_Emp_HEM1 a;
				END;
			ELSE
		 		/* No Absence then DELETE FROM  all absence entry of this */
				DELETE FROM  HEM1 WHERE "empID"=:EmpId;
			END IF;
		END;
		
		BEGIN /* Education */
		
			declare hasRowHem2 int;
			SELECT IFNULL((Select top 1 1 from #APZ_Emp_HEM2),0) into hasRowHem2 from DUMMY;
			IF :hasRowHem2>0
			THEN
				BEGIN
					
					DELETE FROM  HEM2 WHERE "empID"=:EmpId;
					UPDATE #APZ_Emp_HEM2 SET "empID" = :EmpId;
					-- Insert new Education entry
					INSERT INTO HEM2 
		            	SELECT * FROM #APZ_Emp_HEM2 a;
				END;
			ELSE
				/* No Education then DELETE FROM  all Education entry of this*/
				DELETE FROM  HEM2 WHERE "empID"=:EmpId;
			END IF;
		END;
	
		BEGIN /* Review */
		
			declare hasRowHem3 int;
			SELECT IFNULL((Select top 1 1 from #APZ_Emp_HEM3),0) into hasRowHem3 from DUMMY;
			IF :hasRowHem3>0
			THEN
				BEGIN
					
					DELETE FROM  HEM3 WHERE "empID"=:EmpId;
					UPDATE #APZ_Emp_HEM3 SET "empID" = :EmpId;
					-- Insert Review
					INSERT INTO HEM3 
		            	SELECT * FROM #APZ_Emp_HEM3 a;
				END;
			ELSE
			 	/* No Review then DELETE FROM  all Review of this */
				DELETE FROM  HEM3 WHERE "empID"=:EmpId;
			END IF;
		END;
	
		
		BEGIN /* Previous Employment*/
		
			declare hasRowHem4 int;
			SELECT IFNULL((Select top 1 1 from #APZ_Emp_HEM4),0) into hasRowHem4 from DUMMY;
			IF :hasRowHem4>0
			THEN
				BEGIN
					
					DELETE FROM  HEM4 WHERE "empID"=:EmpId;
					UPDATE #APZ_Emp_HEM4 SET "empID" = :EmpId;
					-- Insert Review
					INSERT INTO  HEM4 
		            	SELECT  * FROM #APZ_Emp_HEM4 a;
				END;
			ELSE
			 	/* No Previous Employment then DELETE FROM  all Previous Employment of this */
				DELETE FROM  HEM4 WHERE "empID"=:EmpId;
			END IF;
		END;
	
		BEGIN /* Health Status*/
		
			declare hasRowOheh int;
			SELECT IFNULL((Select top 1 1 from #APZ_Emp_OHEH),0) into hasRowOheh from DUMMY;
			IF :hasRowOheh>0
			THEN
				BEGIN
					
					DELETE FROM  APZ_OHEH WHERE "empID"=:EmpId;
					UPDATE #APZ_Emp_OHEH SET "empID" = :EmpId;
					-- Insert Health Status
					INSERT INTO APZ_OHEH
		            	SELECT   * FROM #APZ_Emp_OHEH a;
				END;
			ELSE
		 		/* No Health Status then DELETE FROM  all Health Status of this */
				DELETE FROM  APZ_OHEH WHERE "empID"=:EmpId;
			END IF;
		END;
	
		BEGIN /* Contract*/
		
			declare hasRowOhec int;
			SELECT IFNULL((Select top 1 1 from #APZ_Contract),0) into hasRowOhec from DUMMY;
			IF :hasRowOhec>0
			THEN
				BEGIN
					--DELETE FROM  row
					declare i int;
					declare isUpdateLineID int;
					--set empID
					UPDATE #APZ_Contract SET "empID" = :EmpId;
					update #APZ_Contract set "contractID" = 0 where ifnull("contractID", 0) = 0;
					select ifnull(max("contractID"),0) into i from #APZ_Contract;
					select ifnull((select top 1 1 from #APZ_Contract where "contractID" = 0),0) into isUpdateLineID from dummy;
					
					while(:isUpdateLineID = 1)
					DO
						i = :i + 1;
						UPDATE top 1 #APZ_Contract SET "contractID" = :i where "contractID" = 0;
						select ifnull((select top 1 1 from #APZ_Contract where "contractID" = 0),0) into isUpdateLineID from dummy;
					END while;
					-- Insert Contract
					DELETE FROM  APZ_OHEC WHERE "empID"=:EmpId;
					INSERT INTO APZ_OHEC
		            	SELECT   * FROM #APZ_Contract a;
				END;
			ELSE
			 	/* No Health Status then DELETE FROM  all Health Status of this */
				DELETE FROM  APZ_OHEC WHERE "empID"=:EmpId;
			END IF;
		END;
	
		select empID as "EmpId" from DUMMY;
		
	END;

END;




