
CALL APZ_EXISTSPROC('APZ_ProjectManager_SetApproval');

GO

create procedure APZ_ProjectManager_SetApproval(in status nvarchar(20),user_code nvarchar(50),id int)
as
begin
	declare isBaseLine int;
	declare baseProject int;
	declare user_sign_code nvarchar(50);
	declare next_approver nvarchar(100);
	declare current_step nvarchar(100);
	declare current_step_order int;
	declare  approver nvarchar(100);
	declare kbpd int;
	select IFNULL((select top 1 1 from apz_oprg a where a."BaseLine"='Y' and a."AbsEntry"=:id),0) into isBaseLine from DUMMY;
	select ifnull((select top 1 "BaseProject" from apz_oprg a where a."AbsEntry"=:id),0) into baseProject from DUMMY;
	select ifnull((select top 1 "U_KBAPR" from apz_oprg a where a."AbsEntry"=:id),0) into kbpd from DUMMY;
	
	
	select (select "UserCodeSign" from apz_oprg where "AbsEntry"=:id) into user_sign_code from DUMMY; 
	select (select "NextApprover" from apz_oprg where "AbsEntry"=:id) into current_step from dummy;
	select (select "Approver" from apz_oprg where "AbsEntry"=:id) into approver from dummy;
	
	
	
	select  ifnull((select top 1 "U_Approver2" from "@APPROVAL_ROUT" 
			where 
			"Code"=:kbpd  and 
			"U_Approver"=:current_step and
			 ("U_Value" like :user_sign_code||',%' or "U_Value" like '%,'||:user_sign_code or "U_Value" like '%,'||:user_sign_code||',%' or "U_Value" =:user_sign_code)),'')
	 		into next_approver from DUMMY;
	if(next_approver='')
	then
		select  ifnull((select top 1 "U_Approver3" from "@APPROVAL_ROUT" 
			where 
			"Code"=:kbpd  and 
			"U_Approver2"=:current_step and
			 ("U_Value" like :user_sign_code||',%' or "U_Value" like '%,'||:user_sign_code or "U_Value" like '%,'||:user_sign_code||',%' or "U_Value" =:user_sign_code)),'')
	 		into next_approver from DUMMY;
	end if;
	if(next_approver='')
	then
		select  ifnull((select top 1 "U_Approver4" from "@APPROVAL_ROUT" 
			where 
			"Code"=:kbpd  and 
			"U_Approver3"=:current_step and
			 ("U_Value" like :user_sign_code||',%' or "U_Value" like '%,'||:user_sign_code or "U_Value" like '%,'||:user_sign_code||',%' or "U_Value" =:user_sign_code)),'')
	 		into next_approver from DUMMY;
	end if;
	if(approver=:user_code and :status='W')
	then
		Update apz_oprg set "WddStatus"='W' where "AbsEntry"=:id;
	end if;
	if(current_step=next_approver and :status='P')
	then
		-- Nếu nó là baseline==> sau khi duyệt xong ( kết thúc phê duyệt) thì cập nhật các baseLine khác inactive. đồng thời active baseline hiện tại
		if(:isBaseLine=1)
		then
			Update apz_oprg set "BaseLineActive"='N'  where "BaseProject"= :baseProject and "AbsEntry"!=:id;
			update apz_oprg set "WddStatus"=:status,"BaseLineActive"='Y',"NextApprover"='',"Approver"=:user_code where "AbsEntry"=:id;
			-- Sau khi phe duyet xong, update thong tin UDF Approved = 'Y'
			update a set "U_Approved"='Y' from APZ_PRG4 aa
													inner join APZ_PRG4 a on aa."LineNum" = a."LineNum" and a."AbsEntry"=:baseProject
											where aa."AbsEntry"=:id and IFNULL(a."U_Approved",'N') = 'N' and a."ItemCode" is not null;
		end if;
	elseif((current_step like :user_code||',%' or current_step like '%,'||:user_code or current_step like '%,'||:user_code||',%' or current_step =:user_code) and :status='N')
	then
		Update apz_oprg set "WddStatus"='N' where "AbsEntry"=:id;
	elseif(next_approver='' and :status='P')
	then
	-- Nếu nó là baseline==> sau khi duyệt xong ( kết thúc phê duyệt) thì cập nhật các baseLine khác inactive. đồng thời active baseline hiện tại
		if(:isBaseLine=1)
		then
			Update apz_oprg set "BaseLineActive"='N'  where "BaseProject"= :baseProject and "AbsEntry"!=:id;
			update apz_oprg set "WddStatus"=:status,"BaseLineActive"='Y',"NextApprover"='',"Approver"=:user_code where "AbsEntry"=:id;
			-- Sau khi phe duyet xong, update thong tin UDF Approved = 'Y'
			update a set "U_Approved"='Y' from APZ_PRG4 aa
													inner join APZ_PRG4 a on aa."LineNum" = a."LineNum" and a."AbsEntry"=:baseProject
											where aa."AbsEntry"=:id and IFNULL(a."U_Approved",'N') = 'N' and a."ItemCode" is not null;
		end if;
	else 
		if(:status='N') then
			Update apz_oprg set "WddStatus"=:status,"Approver"=:user_code,"NextApprover"='' where "AbsEntry"=:id;
		else 
			Update apz_oprg set "WddStatus"='W',"Approver"=:user_code,"NextApprover"=:next_approver where "AbsEntry"=:id;
		end if;
	end if;
	insert into APZ_LOG_APPROVAL ("ObjType","ObjectCode","UserCode","Status","CreateDate")
	select '1470000132',cast(:id as nvarchar(50)),:user_code,:status,now() from dummy;
end