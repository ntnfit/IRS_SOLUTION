

CREATE PROCEDURE APZ_D_InsertEventListener
(
	IN objtype NVARCHAR(50) DEFAULT '',
	IN transtype NCHAR(1) DEFAULT '',
	IN num_of_key INT DEFAULT 0,
	IN key_columns NVARCHAR(254) DEFAULT '',
	IN obj_id NVARCHAR(40000) DEFAULT ''
)
AS
BEGIN
	DECLARE error INT = 0;
	DECLARE error_message NVARCHAR(254) = '';
	
	if :objtype = '17' and :transtype = 'A'	THEN
		declare userCreateOrder nvarchar(50) = IFNULL((SELECT TOP 1 a.USER_CODE FROM OUSR a INNER JOIN ORDR b ON a.USERID = b."UserSign" WHERE cast(b."DocEntry" as nvarchar) = :obj_id AND b."ObjType" = :objtype), '');
		INSERT INTO APZ_D_OELN ("EventCode", "Data", "SendUser", "DataExtra") VALUES ('E001', :obj_id, :userCreateOrder, '{"docEntry": ' || :obj_id || '}');
	end if;
	
	-- Thông báo khi có ghi chú mới (Note)
	--if :objtype = '19000102' and :transtype = 'A' then
	
	--	DECLARE userSign NVARCHAR(4000);
	--	declare groupUser nvarchar(4000) =  ifnull((select top 1 "GroupUser" from APZ_OCLG where cast("ClgCode" as nvarchar) = :obj_id),'');
	--	declare sendAll varchar(1) = ifnull((select top 1 "AllUserInvite" from APZ_OCLG where cast("ClgCode" as nvarchar) = :obj_id),'N');
	--	declare title nvarchar(254) = ifnull((select top 1 "Details" from APZ_OCLG where cast("ClgCode" as nvarchar) = :obj_id),'');
	--	create local temporary table #tblUser ("allUser" int);
	--	insert into #tblUser select cast(Data as int) from Split(:groupUser, ';') where Data <> '';
	--	--Lấy User_Code theo UserID
	--	create local temporary table #UserCode as (select b.USER_CODE from #tblUser a join OUSR b on a."allUser" = b.USERID);
		
	--	userSign = ifnull((select string_agg(USER_CODE, ',') from (select USER_CODE from #UserCode group by USER_CODE)), '');
	    
	--	INSERT INTO APZ_D_OELN ("EventCode", "Data", "SendUser", "SendAll", "DataExtra", "Message") VALUES 
	--	('E002', :obj_id, :userSign, :sendAll, '{"docEntry": ' || :obj_id || '}', :title);
		
	--end if;
	
	--Tải lại dữ liệu ở màn hình Comment ---
	if :objtype = '19000111' and :transtype = 'A' then
		
		declare userComment Nvarchar(4000);
		declare cmtID nvarchar(254) = ifnull((select top 1 cast(ID as nvarchar) from APZ_CLG1 where cast("CmtCode" as nvarchar) = :obj_id), '');
		declare objectType nvarchar(100) = ifnull((select top 1 cast("ObjType" as nvarchar) from APZ_CLG1 where cast("CmtCode" as nvarchar) = :obj_id), '');
		declare contentComment nvarchar(200) = ifnull((select top 1 cast("Content" as nvarchar) from APZ_CLG1 where cast("CmtCode" as nvarchar) = :obj_id), '');
		declare title nvarchar(100) = ifnull((select case when :objectType = '1470000128' then N'Bình luận nhiệm vụ' when :objectType = '1470000130' then N'Bình luận vấn đề' else N'Bình luận mới' end as "Title" from dummy),'');
		create local temporary table #tblCmtUser (USER_CODE nvarchar(50));
		insert into #tblCmtUser SELECT c.USER_CODE FROM APZ_CLG1 a INNER JOIN 
		(SELECT "CmtCode", "UserSign" FROM APZ_CLG3 WHERE cast("CmtCode" as nvarchar) = :obj_id) AS b ON a."CmtCode" = b."CmtCode" INNER JOIN OUSR c ON b."UserSign" = c.USERID WHERE cast(a."CmtCode" as nvarchar) = :obj_id;
		userComment = ifnull((select string_agg(USER_CODE, ',') from (SELECT USER_CODE from #tblCmtUser group by USER_CODE)), '');
		INSERT INTO APZ_D_OELN ("EventCode", "Data", "SendUser", "DataExtra","Title","Message") 
		VALUES ('E004', :cmtID, :userComment, '{"docEntry": ' || :cmtID || ', "objectType": ' || :objtype || ', "type": ' || :objectType || '}',
		:title,:contentComment);
		
	end if;
		
	--Thông báo có vấn đề mới được assign ---
	if :objtype = '1470000130' and :transtype = 'A' then
		declare issueID nvarchar(254);
	declare empID int;
	declare summary nvarchar(4000);
	declare userId int;
	declare userName nvarchar(254);
	SELECT "Id", "HandleBy", "Summary" INTO issueID, empID, summary FROM APZ_TGIE where  cast("Id" as nvarchar) = :obj_id;
	userId = IFNULL((SELECT TOP 1 "userId" FROM OHEM WHERE "empID" = :empID), 0);
	userName = IFNULL((SELECT TOP 1 USER_CODE FROM OUSR WHERE USERID = :userId), '');
	INSERT INTO APZ_D_OELN ("EventCode", "Data", "SendUser", "DataExtra", "Message") VALUES ('E003', :issueID, :userName, '{"docEntry": ' || :issueID || ', "objectType": ' || :objtype || '}', SUBSTRING(:summary,1,254));
	END IF;
	
	--Thông báo có vấn đề đổi người assign ---
	if :objtype = '1470000130' and :transtype = 'U' then
		declare issueID nvarchar(254);
	declare empID int;
	declare summary nvarchar(4000);
	declare userId int;
	declare userName nvarchar(254);
	declare oldHandleBy nvarchar(254);
	
	SELECT "Id", "HandleBy", "Summary" INTO issueID, empID, summary FROM APZ_TGIE where  cast("Id" as nvarchar) = :obj_id;
	userId = IFNULL((SELECT TOP 1 "userId" FROM OHEM WHERE "empID" = :empID), 0);
	userName = IFNULL((SELECT TOP 1 USER_CODE FROM OUSR WHERE USERID = :userId), '');
	oldHandleBy = ifnull((select top 1 "SendUser" from APZ_D_ELN1 where "EventCode" = 'E003' and "Data" = :issueID order by "CreateDate"),'');
	if(:oldHandleBy != :userName AND :userName != '') then
	INSERT INTO APZ_D_OELN ("EventCode", "Data", "SendUser", "DataExtra", "Message") VALUES ('E003', :issueID, :userName, '{"docEntry": ' || :issueID || ', "objectType": ' || :objtype || '}', SUBSTRING(:summary,1,254));
	end if;
	END IF;
	

	select :error, :error_message FROM DUMMY;
END

