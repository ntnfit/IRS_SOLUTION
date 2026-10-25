
CREATE PROCEDURE APZ_SubActivities_PushAppNotification(in clgCode nvarchar(100))
AS
BEGIN
	DECLARE userSign NVARCHAR(254);
	declare groupUser nvarchar(254) =  ifnull((select top 1 "GroupUser" from APZ_OCLG where "ClgCode" = :clgCode),'');
	
	declare sendAll varchar(1) = ifnull((select top 1 "AllUserInvite" from APZ_OCLG where "ClgCode" = :clgCode),'N');
	declare title nvarchar(254) = ifnull((select top 1 "Details" from APZ_OCLG where "ClgCode" = :clgCode),'');
	
	create local temporary table #tblUser ("allUser" nvarchar(5));
	insert into #tblUser select cast(Data as int) from Split(:groupUser, ';');
	
	--Lấy User_Code theo UserID
	create local temporary table #UserCode as (select b.USER_CODE from #tblUser a join OUSR b on a."allUser" = b.USERID);
	
	select (select string_agg(USER_CODE, ',') from #UserCode) into userSign from dummy;

	insert into APZ_D_OELN ("EventCode", "Data", "SendUser", "SendAll", "DataExtra", "Message")
			values ('E002', :clgCode,:userSign,:sendAll,'{"docEntry": '|| :clgCode || '}',:title);

	
END;




